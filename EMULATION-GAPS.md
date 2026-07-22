# Emulation gaps — `Net4x.Gtk.Windows.Forms` vs `System.Windows.Forms`, `Net4x.Gtk.Drawing` vs `System.Drawing`

**Scope of this document.** It assumes the stated intent: *an existing WinForms application should compile unchanged against these two assemblies and behave the same on Windows, Linux and macOS.* Everything below is measured against that bar. Findings were taken by reading the generated sources in `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` at the current working tree, and file/line references point there.

**Caveat on sources.** Both directories are *generated* by the prepare pipeline from the `gtksystem-windows-forms` submodule (see [CLAUDE.md](CLAUDE.md)). Line numbers below are accurate for the current tree but will drift after the next pipeline run; the type and member names will not.

---

## 1. Executive summary

The emulation is broad but shallow in specific, load-bearing places. Three classes of gap dominate:

| Class | What it means | Why it matters |
|---|---|---|
| **A — Missing types** | The type does not exist at all. | *Compile-time* failure. The "compile once" promise fails immediately; the user must edit source. |
| **B — Present but throwing** | The member exists, throws `NotImplementedException`. | *Runtime* failure, but loud and diagnosable. |
| **C — Present but silently wrong** | The member exists, returns/does something plausible but not WinForms-equivalent. | The worst class: the app builds, runs, and misbehaves subtly. Several of these are in threading and rendering. |

Class C is the most important finding of this review and is treated first.

---

## 2. Class C — present but semantically wrong (highest risk)

### 2.1 `Control.Invoke` is asynchronous — it is not

[Control.cs:2225](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:2225):

```csharp
public virtual object? Invoke(Delegate method, params object[]? args)
{
    lock (this)
    {
        object? result = null;
        GLib.Idle.Add(() => { result = method?.DynamicInvoke(args); return false; });
        return result;   // <-- returns before the idle callback ever runs
    }
}
```

In WinForms, `Invoke` **blocks** until the delegate has run on the UI thread and returns the delegate's result. Here it queues the delegate onto the GLib main loop and returns immediately — `result` is always `null`. `Invoke(Action)` ([:2246](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:2246)) has the same shape.

Consequences for real apps:
- `var x = (string)ctl.Invoke(new Func<string>(() => ctl.Text));` returns `null`, then throws or corrupts downstream logic.
- Worker-thread code that uses `Invoke` as a *synchronisation barrier* ("update the UI, then continue knowing it's done") now races.
- Ordering guarantees between successive `Invoke` calls and subsequent non-UI work are lost.

**Full emulation requires:** a real synchronous marshal — post to the GTK main context and wait on a `ManualResetEventSlim`/`Task`, with the fast path of running inline when already on the UI thread, and exception marshalling (WinForms rethrows the delegate's exception on the calling thread).

### 2.2 `Control.InvokeRequired` always returns `false`

[Control.cs:1200](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1200):

```csharp
public virtual bool InvokeRequired { get; }
```

An auto-property with no setter and no backing logic — permanently `false`. The single most common WinForms threading idiom is:

```csharp
if (InvokeRequired) { Invoke(new Action(UpdateUi)); return; }
UpdateUi();
```

With `InvokeRequired == false` this **always takes the direct path**, so a background thread touches GTK widgets directly. GTK is not thread-safe; on Linux/macOS this produces intermittent corruption, X/Wayland protocol errors, or hard crashes that are extremely hard to attribute.

**Full emulation requires:** capturing the thread that created the widget and comparing against `Thread.CurrentThread`.

### 2.3 `Control.BeginInvoke` runs on a thread-pool thread, not the UI thread

[Control.cs:1763](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1763):

```csharp
public virtual IAsyncResult BeginInvoke(Delegate method, params object[]? args)
{
    var task = Task<object?>.Factory.StartNew(state => method.DynamicInvoke((object[])state!), args);
    return task;
}
```

`BeginInvoke` is *the* documented way to get work onto the UI thread asynchronously. Here it does the opposite: it dispatches to the thread pool. Any `BeginInvoke(() => label.Text = "...")` is now an off-thread GTK call.

Note the inconsistency: `Invoke` uses `GLib.Idle.Add` (right thread, wrong synchrony); `BeginInvoke` uses the thread pool (wrong thread, right synchrony). Neither matches.

`EndInvoke` ([:1917](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1917)) also deviates: on a faulted task it *returns* the `AggregateException` as the result value rather than rethrowing it.

**Full emulation requires:** both `Invoke` and `BeginInvoke` built on one main-context posting primitive, differing only in whether the caller waits.

> **These three together are the single biggest correctness gap in the library.** A WinForms app that is correctly written for threading becomes incorrectly threaded when recompiled here, with no diagnostic.

> **Status: fixed.** `Invoke` now marshals synchronously (inline fast path on the owning thread,
> otherwise posted to the GTK main loop and awaited), rethrowing the delegate's own exception on the
> caller; `InvokeRequired` compares against the thread that created the control; and `BeginInvoke` posts
> to the main loop instead of the thread pool. When no main loop is running there is no thread to
> marshal to, so the work runs inline rather than deadlocking. Covered by
> `ControlThreadingFunctionalTests`; carried in `patches/0088`.


### 2.4 `SetStyle(ControlStyles, bool)` is a no-op — owner-draw controls do not work

[Control.cs:3746](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:3746):

```csharp
protected virtual void SetStyle(ControlStyles styles, bool value) { }
```

The `ControlStyles` enum exists, but setting it has no effect. This breaks the standard custom-control recipe:

```csharp
SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
       | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
```

Custom-painted controls — the reason many WinForms codebases subclass `Control` at all — will not honour `UserPaint`, will not suppress background erase, will not opt into double buffering, and will not repaint on resize.

`GetStyle` is likewise not backed by real state.

### 2.5 `Control.CreateGraphics()` draws to an off-screen surface

[Control.cs:1839](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1839) allocates a fresh Cairo `ImageSurface`/`Context` each call and returns a `Graphics` over it. The comment at [:61](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:61) explains why (drawing to the live GDK window during realize throws), and `Override_PaintGraphics` later blits that surface during the paint cycle.

The deviation: in WinForms `CreateGraphics()` gives you an *immediate-mode* device context — you draw and it appears now, outside any paint event. Here the effect is deferred to the next expose, and each `CreateGraphics()` call **disposes the previous surface**, so two live `Graphics` objects on one control cannot coexist. The classic "rubber-band a selection rectangle in `MouseMove` via `CreateGraphics`" pattern will not behave correctly.

### 2.6 `Control.Handle` is a GTK widget handle, not an `HWND`

[Control.cs:1126](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1126) returns `Widget.Handle`. This is correct-by-necessity but is a hard boundary that should be documented: any application code that passes `Control.Handle` to `user32.dll`/`gdi32.dll`, or that implements `IWin32Window` for interop, is broken. This is inherent to the cross-platform goal, not a defect — but it is a gap in *emulation intent*.

Related: `WndProc(ref Message m)` ([:3777](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:3777)) has an empty body and is never fed real messages. `Message` and `WndProc` exist so message-pumping code *compiles*, but overriding `WndProc` to intercept `WM_*` is a silent no-op. Apps that implement drag-move, custom hit-testing, or non-client painting via `WndProc` will build and do nothing.

### 2.7 `System.Drawing.Region` is a single rectangle

[Region.cs:15](Net4x.Gtk.Drawing/System/Drawing/Region.cs:15):

```csharp
private RectangleF rectangle = new RectangleF(0, 0, 0, 0);
```

The entire 1400-line `Region` class is backed by one `RectangleF`. All of `Union`, `Intersect`, `Exclude`, `Complement`, `Region(GraphicsPath)`, `GetRegionScans`, `Transform` therefore collapse to rectangle arithmetic. Non-rectangular regions — the whole point of `Region` — are not representable.

This propagates: `Graphics.SetClip(Region)` ([Graphics.cs:2144](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:2144)) reduces the region to its bounding rectangle before clipping, so path-based and elliptical clipping silently become rectangular clipping.

**Full emulation requires:** a real region representation — either a scanline/band list (as GDI+ uses) or delegation to Cairo's path-based clipping, which can express arbitrary clip shapes natively.

### 2.8 `Graphics` quality/rendering modes are stored but never applied

`SmoothingMode` ([Graphics.cs:178](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:178)), `InterpolationMode` ([:114](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:114)), `CompositingMode` ([:83](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:83)), `PixelOffsetMode` ([:160](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:160)), `PageUnit`/`PageScale` ([:142](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:142)), `TextRenderingHint` ([:202](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:202)) and `RenderingOrigin` are all plain auto-properties. Grepping `Graphics.cs` finds no consumer of `SmoothingMode` and no `Antialias` call anywhere in the file.

So the very common:

```csharp
g.SmoothingMode = SmoothingMode.AntiAlias;
```

reads back correctly, sets nothing, and rendering is whatever Cairo defaults to. `PageUnit`/`PageScale` being inert is more serious: a `Graphics` set to `GraphicsUnit.Millimeter` or `Inch` will draw at pixel coordinates, so unit-aware drawing and printing code is geometrically wrong, not just cosmetically different.

**Full emulation requires:** mapping each mode onto its Cairo counterpart (`Antialias`, `Filter`, `Operator`) at draw time, and folding `PageUnit`/`PageScale` into the CTM.

### 2.9 `Graphics.Transform` getter throws; the setter silently discards

[Graphics.cs:207](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:207):

```csharp
public Drawing2D.Matrix Transform
{
    get { throw new System.NotImplementedException(); }
    set { }
}
```

A throwing getter is a Class B gap; a **silently swallowing setter is Class C and worse**. `g.Transform = myMatrix;` is a standard way to apply a world transform, and it compiles, runs, and does nothing. Meanwhile `RotateTransform`/`ScaleTransform`/`TranslateTransform`/`MultiplyTransform` *are* implemented ([:1957](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1957) onward), so the transform pipeline works via one API and is inert via the other. Save/restore round-tripping (`var old = g.Transform; ... ; g.Transform = old;`) throws on the first line.

> **Status: fixed.** The getter now reports the live Cairo matrix and the setter assigns it. Fixing this
> also uncovered that `ConvertToMatrix` applied the default zero scale (collapsing every matrix to
> all-zero), passed degrees to Cairo where radians were expected, and passed `null` to
> `Cairo.Matrix.Multiply` (a native abort). Covered by `GraphicsTransformOverloadFunctionalTests`;
> carried in `patches/0087`.


### 2.10 `PrintDocument.Print()` shows a dialog

[PrintDocument.cs:189](Net4x.Gtk.Windows.Forms/System/Drawing/Printing/PrintDocument.cs:189) constructs a `PrintDialog` and calls `RunPrint`. In WinForms, `PrintDocument.Print()` prints **silently** to `PrinterSettings.PrinterName`; showing a dialog is the caller's choice via `PrintDialog`. Unattended/batch printing paths therefore block on a UI prompt.

### 2.11 `Font.FromHfont` / `FromHdc` / `FromLogFont` return Arial 12

[Font.cs:478-514](Net4x.Gtk.Drawing/System/Drawing/Font.cs:478) — all four factory methods return `new Font("Arial", 12)` regardless of input. Silent wrong-font rather than a diagnosable failure. (These are interop entry points, so arguably they should throw `PlatformNotSupportedException` instead.)

### 2.12 `ListView.VirtualMode` is spelled `VirtualModel`

[ListView.cs:712](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs:712) declares `VirtualModel`, not WinForms' `VirtualMode`. Source using virtual-mode list views fails to compile, and the designer-generated `.Designer.cs` from a real WinForms project will not build. This looks like a straightforward naming defect rather than a design decision. (`DataGridView.VirtualMode` at [DataGridView.cs:1238](Net4x.Gtk.Windows.Forms/System/Windows/Forms/DataGridView.cs:1238) is spelled correctly.)

### 2.13 `Form.AutoScaleMode` / `AutoScaleDimensions` are inert

[Form.cs:141](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs:141) and [:150](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs:150) are auto-properties. Every VS-designer-generated form sets `AutoScaleMode = AutoScaleMode.Font` and `AutoScaleDimensions = new SizeF(7F, 15F)`. With these inert, layout produced at one system font/DPI is not rescaled at another — which is precisely the scenario a cross-platform layer hits hardest, since Linux/macOS default font metrics differ from Windows'. `Application.SetHighDpiMode` ([Application.cs:583](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs:583)) should be audited on the same grounds.

### 2.14 `Control.DrawToBitmap` is an empty method

[Control.cs:1895](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1895) — returns without touching the bitmap. Callers get a blank image, not an error. Used for print-preview, thumbnails, and report generation.

---

## 3. Class A — types that do not exist (compile-time breaks)

Verified absent from `Net4x.Gtk.Windows.Forms` by type-declaration search.

### 3.1 Rendering & metrics helpers — the highest-frequency break

| Missing type | Impact |
|---|---|
| **`TextRenderer`** | `TextRenderer.MeasureText` / `DrawText` is the *recommended* WinForms text API since .NET 2.0 and appears in an enormous share of custom-drawn WinForms code. Its absence is probably the most-hit compile error. |
| **`ControlPaint`** | `ControlPaint.DrawBorder3D`, `.Light`, `.Dark`, `.DrawFocusRectangle`, `.DrawCheckBox`… Ubiquitous in owner-draw code. |
| **`SystemInformation`** | Border widths, scrollbar sizes, double-click time, `MenuHeight`. Layout code reads these constantly. |

### 3.2 The `System.Windows.Forms.VisualStyles` namespace — entirely absent

`VisualStyleRenderer`, `VisualStyleElement`, and the whole renderer family (`ButtonRenderer`, `CheckBoxRenderer`, `RadioButtonRenderer`, `ComboBoxRenderer`, `TextBoxRenderer`, `ProgressBarRenderer`, `ScrollBarRenderer`, `GroupBoxRenderer`, `TabRenderer`, `TrackBarRenderer`). Note that `Application.EnableVisualStyles()` ([Application.cs:241](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs:241)) *does* exist — so the `Program.cs` boilerplate compiles while the namespace it enables does not.

There is a legitimate design argument that themed rendering should defer to the GTK theme rather than emulate Windows visual styles. If so, that is a *documented non-goal*, not a gap — but the types should still exist as compiling shims so source ports build.

### 3.3 ToolStrip infrastructure

Present: `ToolStrip`, `MenuStrip`, `StatusStrip`, `ContextMenuStrip`, and the item classes.
Missing: **`ToolStripManager`** (merging, `RevertMerge`, renderer selection), **`ToolStripRenderer`** / **`ToolStripProfessionalRenderer`** / **`ProfessionalColorTable`** (all custom ToolStrip theming), **`ToolStripContainer`**, **`ToolStripPanel`**, **`ToolStripContentPanel`** (rafting/docking of toolbars — designer-generated for any multi-toolbar form), **`ToolStripControlHost`** (hosting arbitrary controls in a strip).

### 3.4 Common controls

`Splitter` (the legacy splitter, distinct from the present `SplitContainer`), `DomainUpDown` and `UpDownBase` (`NumericUpDown` exists but does not derive from a real `UpDownBase`), `ErrorProvider`, `HelpProvider`, `BindingNavigator`, `WebBrowser` (its four `*EventArgs` types are present — [see the type list] — but the control itself is not, so the event args are unreachable), `HtmlDocument`/`HtmlElement`.

### 3.5 Legacy menu/status API

`MainMenu`, `MenuItem`, `ContextMenu`, `Menu`, `StatusBar`, `StatusBarPanel`. These are obsolete in modern WinForms but are exactly what an *old* application — the migration target this project exists to serve — will be using.

### 3.6 Misc

`NativeWindow`, `SendKeys`, `Help`, `AmbientProperties`, `AutoCompleteStringCollection` (`AutoCompleteMode` exists as an enum, so the feature is half-declared), `IWindowsFormsEditorService`.
Absent on `Control`: `Region` (non-rectangular control shapes) and, on `Form`, `TransparencyKey`.

### 3.7 `Net4x.Gtk.Drawing` missing types

| Missing | Impact |
|---|---|
| **`TextureBrush`** | One of the five core GDI+ brush types. `Brush`, `SolidBrush`, `HatchBrush`, `LinearGradientBrush`, `PathGradientBrush` are all present — `TextureBrush` alone is not. |
| **`SystemFonts`** | `SystemFonts.DefaultFont`/`MenuFont`/`MessageBoxFont`. Designer-generated code and dialog layout use these. `SystemColors`/`SystemPens` *are* present, making the omission look accidental. |
| **`SystemBrushes`** | `SystemPens` exists; the brush counterpart does not. |
| **`SystemIcons`** | `SystemIcons.Error`/`Warning`/`Information` — used in custom message dialogs. |
| **`BufferedGraphics`, `BufferedGraphicsContext`, `BufferedGraphicsManager`** | Manual double-buffering; the standard flicker-free custom-painting pattern. |
| **`PrivateFontCollection`, `InstalledFontCollection`** | The base `FontCollection` exists ([FontCollection.cs](Net4x.Gtk.Drawing/System/Drawing/Text/FontCollection.cs)) but neither concrete subclass. Apps that ship embedded fonts cannot load them. |
| **`ToolboxBitmapAttribute`** | Applied to custom control classes; a compile break on the *declaration*, which is worse than on a call site. |

**Correctly delegated, not a gap:** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are absent from the project by design — `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the real BCL types. This is the right call and should be preserved.

---

## 4. Class B — present but throwing `NotImplementedException`

Loud failures. Ordered by likely hit rate.

### 4.1 `Graphics` (18 sites, [Graphics.cs](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs))

- **All eight `IsVisible` overloads** ([:1636](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1636)–[:1823](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1823)). This is a clipping/culling primitive used in the paint loop of any scrolling custom control — `if (!g.IsVisible(itemRect)) continue;`. Given that `Clip` bounds *are* tracked ([:37](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:37)), these look cheap to implement and are high value.
- `Transform` getter ([:207](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:207)) — see §2.9.
- `TextContrast` getter ([:189](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:189)) — low value; note the setter silently accepts, so getter-throws/setter-ignores is an odd asymmetry.
- `MeasureCharacterRanges` ([:1831](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1831)) — *partially* addressed; the comment records that it now returns one region per range positioned approximately, i.e. it no longer throws but is not accurate. Text selection/hit-testing in custom text controls needs real per-range extents (Pango can supply these).
- Interop entry points, correctly hopeless: `FromHdc`, `FromHdcInternal`, `FromHwnd`, `FromHwndInternal`, `GetHdc`, `GetHalftonePalette`. These should arguably throw `PlatformNotSupportedException` — that is the documented .NET convention and it tells the caller "never going to work" instead of "not written yet."

### 4.2 `Clipboard` / `DataObject` ([OLE/Clipboard.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs))

`GetDataPresent(string)`, `GetDataPresent(Type)`, `GetDataPresent(string, bool)`, `GetFormats()`, `GetFormats(bool)` all throw ([:137](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs:137)–[:157](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs:157)). `GetDataPresent` is the *guard* every well-written paste handler calls before `GetData` — so the defensive code path throws while the naive one works. Drag-and-drop `DragEventArgs.Data.GetDataPresent(...)` in `DragEnter` is the same pattern and equally affected.

### 4.3 Control-level throw counts

By file, highest first:

| File | Sites |
|---|---|
| [LinkLabel.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/LinkLabel.cs) | 24 |
| [ToolStripItem.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs) | 15 |
| [TabControl.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/TabControl.cs) | 13 |
| [ListBox.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListBox.cs) | 13 |
| [ListViewGroupConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupConverter.cs) | 8 |
| [PropertyGrid.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGrid.cs) | 6 |
| [ListViewItemConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewItemConverter.cs) | 6 |
| [GridEntry.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGridInternal/PropertyGridBase/GridEntry.cs) | 5 |
| [ListView.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs), [ComponentDesigner.cs](Net4x.Gtk.Windows.Forms/System/ComponentModel/Design/Designer/ComponentDesigner.cs), [ListViewGroupEventArgs.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupEventArgs.cs) | 4 each |

`LinkLabel` standing at 24 is notable: it is a simple, commonly-used control, which suggests it was stubbed wholesale rather than partially implemented.

### 4.4 `Drawing2D` remainder

- `CustomLineCap` — 5 throw sites; effectively unimplemented. Custom arrowheads/caps on pens are unavailable.
- `Matrix.Elements`, `Matrix.IsIdentity`, `Matrix.IsInvertible` ([Matrix.cs](Net4x.Gtk.Drawing/System/Drawing/Drawing2D/Matrix.cs)) throw. `Elements` is how application code inspects or serialises a transform; its absence blocks round-tripping a `Matrix` even though the transform operations themselves work.
- `Pen.PenType` ([Pen.cs](Net4x.Gtk.Drawing/System/Drawing/Pen.cs)) throws.
- `Icon.FromHandle` ([Icon.cs](Net4x.Gtk.Drawing/System/Drawing/Icon.cs)) throws — an interop path, same `PlatformNotSupportedException` argument as §4.1.

---

## 5. Structural gaps (not fixable member-by-member)

These are the areas where "full emulation" needs a design decision, not a patch.

1. **No Win32 message pump.** `Message`, `WndProc`, `Application.AddMessageFilter`/`IMessageFilter`, `NativeWindow`, `Control.DefWndProc` — the whole message-interception layer is absent or inert. Any app that reaches below the event model is unportable by construction. **Recommendation:** state this as an explicit non-goal and make the affected members throw `PlatformNotSupportedException` on first use rather than no-op, so it fails loudly at test time instead of shipping.

2. **`Application` surface is thin.** Present: `Run(Form)`, `Exit`, `DoEvents`, `EnableVisualStyles`, path properties. Missing: `Run()`, `Run(ApplicationContext)`, `Idle`, `ApplicationExit`, `ThreadExit`, `ThreadException` / `SetUnhandledExceptionMode`, `MessageLoop`, `AddMessageFilter`, `Restart`, `ProductName`/`ProductVersion`/`CompanyName`, `RenderWithVisualStyles`, `VisualStyleState`. `ApplicationContext` exists as a type but has no `Run` overload to consume it — so the multi-form / no-main-form startup pattern has no entry point.

3. **Metafiles are declared, not implemented.** `Metafile`, `MetafileHeader`, `MetaHeader`, `WmfPlaceableFileHeader`, `EmfPlusRecordType`, `PlayRecordCallback` all exist as types under [Imaging/](Net4x.Gtk.Drawing/System/Drawing/Imaging/Drawing/Drawing2D/) — but EMF/WMF is a Windows record format with no cross-platform playback. This is almost certainly a permanent non-goal and should be labelled as one.

4. **Data binding depth is unverified.** `Binding`, `BindingSource`, `CurrencyManager`, `PropertyManager`, `BindingContext`, `Formatter`, `ListBindingHelper` are all present, which is encouraging. What is *not* established by this review is whether `Format`/`Parse` events, `DataSourceUpdateMode`, `IEditableObject` transaction semantics and `INotifyPropertyChanged` propagation behave to spec. **This deserves its own targeted test pass** rather than an assertion here either way.

5. **Accessibility is nominal.** `AccessibleObject`, `AccessibleRole`, `AccessibleStates`, `AccessibleEvents` exist. Whether they are wired to ATK (GTK's accessibility bus) is not established; if they are not, screen-reader support is absent on all platforms despite the API being present.

6. **`Control.Handle` semantics** (§2.6) — inherent, but should be spelled out in the package README so integrators do not discover it in production.

---

## 6. Suggested priority order

Ranked by (damage × frequency) ÷ effort. The top group is where I would start.

**P0 — silent correctness, cheap to fix**
1. `InvokeRequired` — record the creating thread (§2.2).
2. `Invoke` — make it genuinely synchronous, with exception marshalling (§2.1).
3. `BeginInvoke` — post to the main context, not the thread pool (§2.3).
4. `Graphics.Transform` setter — apply it or throw; never silently discard (§2.9).
5. `ListView.VirtualModel` → `VirtualMode` — a rename (§2.12).

**P1 — compile breaks on very common APIs**
6. `TextRenderer` (measure + draw over Pango).
7. `ControlPaint`.
8. `SystemFonts`, `SystemBrushes`, `SystemIcons` — trivial next to the existing `SystemColors`/`SystemPens`.
9. `TextureBrush` — Cairo has native surface-pattern support with repeat modes.

**P2 — rendering fidelity**
10. Wire `SmoothingMode`/`InterpolationMode`/`CompositingMode` to Cairo (§2.8).
11. Honour `PageUnit`/`PageScale` in the CTM — currently a geometric error, not a cosmetic one (§2.8).
12. `Graphics.IsVisible` ×8 — the clip bounds are already tracked (§4.1).
13. Real `Region` over Cairo paths (§2.7) — larger, unblocks correct non-rectangular clipping.

**P3 — behavioural**
14. `SetStyle`/`GetStyle` backing state, at minimum for `UserPaint`, `OptimizedDoubleBuffer`, `ResizeRedraw`, `AllPaintingInWmPaint` (§2.4).
15. `Clipboard.GetDataPresent` / `GetFormats` (§4.2).
16. `AutoScaleMode` — highest value of all on a cross-platform layer, and the largest single job here (§2.13).
17. `PrintDocument.Print()` — print silently; leave the dialog to `PrintDialog` (§2.10).
18. `Control.DrawToBitmap` (§2.14).

**P4 — declare non-goals explicitly**
19. `WndProc`/`Message`/`NativeWindow`, EMF/WMF metafiles, `Graphics.FromHwnd`/`GetHdc`, `Font.FromHfont`, `Icon.FromHandle` — convert `NotImplementedException` to `PlatformNotSupportedException` and document. A clear "never" is more useful to an integrator than an ambiguous "not yet."

---

## 7. What this review did *not* cover

Stated so the gaps in the gap analysis are visible:

- **No runtime behaviour was exercised.** Everything here is from source reading. Static analysis reliably finds missing types, throwing members, and inert auto-properties; it does *not* find layout drift, event-ordering differences, or focus/tab-order divergence, which are likely to be a large additional category.
- **Layout engines** (`TableLayoutPanel`, `FlowLayoutPanel`, anchor/dock) were not compared against WinForms measure/arrange semantics. Given that these are fully reimplemented on GTK's allocate cycle, pixel-level and edge-case divergence should be assumed until tested.
- **Data binding and accessibility** — flagged as unverified in §5, not assessed.
- **`Net4x.Gtk.Windows.FormsDesigner`** and the ResX/designer round-trip were out of scope.
- **Per-member API-surface diffing.** A mechanical comparison of the real `System.Windows.Forms` / `System.Drawing` reference assemblies against these two — public members, signatures, and attributes — would be strictly more complete than this document at the type level and would catch further cases of the §2.12 kind (subtly renamed or re-signatured members, which are the most insidious because the designer emits code against the real names). That is the single highest-value follow-up, and it can be automated.

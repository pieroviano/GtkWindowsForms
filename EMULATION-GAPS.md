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

## 1a. Current state

*This section records what has since been addressed. Sections 2–5 below are the original review; each item
that has been dealt with now carries a **Status** note in place.*

| Area | State |
|---|---|
| **§2.1–2.3 Threading** (`Invoke`, `InvokeRequired`, `BeginInvoke`) | **Fixed.** Called "the single biggest correctness gap" below. |
| **§2.3a Data binding** *(found later)* | **Fixed.** Four chained gaps meant a `Binding` wrote **null** into the bound control property. |
| **§2.4 `SetStyle`/`GetStyle`** | **Fixed** — real backing state; `GetStyle` did not exist at all. |
| **§2.7 `Region`** | **Fixed** via a band list. Curved paths still contribute their bounds. |
| **§2.8 Quality modes, `PageUnit`/`PageScale`** | **Fixed** — mapped onto Cairo/Gdk; page units fold into the transform. |
| **§2.9 `Graphics.Transform`** | **Fixed**, and uncovered a matrix-collapsing `Scale(0,0)`, a degrees/radians error and a native abort. |
| **§2.10 `PrintDocument.Print()`** | **Fixed** — prints silently; `RunPrint` had been ignoring `showDialog`. |
| **§2.11 `Font.FromHfont`/`FromHdc`/`FromLogFont` | **Fixed** — `PlatformNotSupportedException` instead of a silent wrong font. |
| **§2.12 `ListView.VirtualModel`** | **Fixed** — renamed to `VirtualMode`, old spelling kept as an obsolete alias. |
| **§2.13 `AutoScaleMode`** | **API implemented; automatic application deliberately withheld.** See the note in place — it needs a design-time metric commensurate with the runtime one. |
| **§2.14 `DrawToBitmap`** | **Fixed.** |
| **§3.1 `TextRenderer`, `ControlPaint`, `SystemInformation`** | **Fixed.** Building `TextRenderer` exposed that `MeasureString` never wrapped. |
| **§3.5 Legacy menu/status API** | **Fixed** — adapters over the ToolStrip family. |
| **§3.7 `TextureBrush`, `SystemFonts`, `SystemBrushes`, `SystemIcons`, `ToolboxBitmapAttribute`, buffered graphics** | **Fixed.** Buffered graphics exposed that the Cairo surface was **static**, so two `Graphics.FromImage` objects could not coexist. |
| **§3.4 `ErrorProvider`, `Splitter`** | **Partly fixed** — API and logic exact; the error icon and splitter dragging are not rendered/wired. |
| **§4.1 `Graphics.IsVisible` ×8, interop entries, `TextContrast`** | **Fixed.** |
| **§4.2 `Clipboard.GetDataPresent`/`GetFormats`** | **Fixed.** |
| **§4.3 `LinkLabel` 24, `TabControl` 13, `ListBox` 13** | **All now 0.** |
| **§4.4 `Matrix.Elements`/`IsIdentity`/`IsInvertible`** | **Fixed.** |

**Still open:** §3.2 the `VisualStyles` namespace; §3.3 ToolStrip infrastructure; the rest of §3.4
(`DomainUpDown`/`UpDownBase`, `HelpProvider`, `BindingNavigator`, `WebBrowser`, `HtmlDocument`); §3.6 misc
(`NativeWindow`, `SendKeys`, `Help`, `AmbientProperties`, `AutoCompleteStringCollection`, `Control.Region`,
`Form.TransparencyKey`); the font collections; §2.5/§2.6 (`CreateGraphics` immediacy, `Handle`/`WndProc`),
which the review frames as boundaries rather than defects; and the remaining throw sites led by
`ToolStripItem` and the ListView/PropertyGrid type-converters.

All changes are carried in `patches/0087` (Net4x.Gtk.Drawing) and `patches/0088` (Net4x.Gtk.Windows.Forms),
so they survive regeneration by the prepare pipeline.

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


### 2.3a Data binding never delivered a value to a control  *(found later; fixed)*

Four independent gaps combined so that `control.DataBindings.Add("Text", source, "Name")` compiled, ran, and
wrote **null** into the bound property:

- `Control.DataBindings` was an auto-property nothing ever assigned, so it was `null` on every control that
  did not override it.
- `Control.BindingContext` was likewise an always-null auto-property, so `BindingContext.UpdateBinding` had
  no context to work with and no `BindingManagerBase` was ever created.
- `Control.Created` was only set by `PerformLayout`/`CreateControl`, so a live control with a real GTK handle
  still reported itself uncreated — and a `Binding` only activates for a *created* component.
- `Binding.CheckBinding` never finished with `UpdateIsBinding()`, so a binding attached after its manager
  already existed stayed inactive and `PushData` took its "not binding" branch, which calls
  `SetPropValue(null)`.

The result was silent and total: no exception, no diagnostic, the control simply stayed empty. This is the
same Class C shape as the threading gaps above — the app builds and runs and is quietly wrong.

> **Status: fixed.** All four are corrected and covered by `BindingValueFlowFunctionalTests`
> (source → control, control → source, cursor movement, formatting, null substitution); carried in
> `patches/0088`.

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

> **Status: fixed.** `SetStyle` now records the flags and `GetStyle` (which did not exist at all) reads
> them back, with the WinForms default set applied to a fresh control. Covered by
> `EmulationGapFunctionalTests`; carried in `patches/0088`.


### 2.5 `Control.CreateGraphics()` draws to an off-screen surface

[Control.cs:1839](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1839) allocates a fresh Cairo `ImageSurface`/`Context` each call and returns a `Graphics` over it. The comment at [:61](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:61) explains why (drawing to the live GDK window during realize throws), and `Override_PaintGraphics` later blits that surface during the paint cycle.

> **Root cause found and fixed.** The observation that "each `CreateGraphics()` call **disposes the
> previous surface**" had a concrete cause: `Graphics.imagesurface` and `Graphics.imagecontext` were
> **static**, so every `Graphics` created by `FromImage` shared one Cairo surface and context. Allocating a
> second one disposed the first one's surface out from under it, and two `Graphics` objects over two
> different bitmaps could not coexist at all — which is exactly what double buffering requires. They are
> now per-instance, and a `Graphics` only disposes a context it owns. Found while implementing
> `BufferedGraphics`; carried in `patches/0087`.
>
> The remaining half of this section stands: `CreateGraphics()` is still deferred rather than immediate-mode.

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

> **Status: fixed via the band list.** The interior is now a list of disjoint rectangles — the first of the
> two options above — so the whole combine family is exact for rectangular inputs and non-rectangular
> regions are representable. What this actually corrected:
>
> - `Xor` of two partly overlapping rectangles returned their **union**, keeping the overlap that an
>   exclusive-or exists to remove.
> - `Union` of two disjoint rectangles reported the gap between them as inside the region.
> - `Exclude` shrank the bounding box instead of leaving a hole.
> - `IsVisible` tested the bounding rectangle, so a point in the hole of an L-shaped region counted as
>   inside.
> - `GetRegionScans` returned the single bounding rectangle rather than the bands.
> - `Region.Clone` shared the interior with the original.
> - `Graphics.SetClip(Region)` reduced the region to its bounding box, so clipping to a non-rectangular
>   region silently clipped to its bounds. It now clips to the real bands — verified by filling through an
>   L-shaped clip and asserting the hole stays unpainted.
>
> **Still approximate:** a `GraphicsPath` contributes its *bounding rectangle* to the band list, so a
> genuinely curved region (an ellipse, say) is still treated as its bounds for the combine operations. The
> path is kept alongside the bands, so closing that gap means feeding the path to Cairo for clipping and
> flattening it into bands for the set operations.
>
> This also invalidated two tests written earlier against the old behaviour: one asserted that
> complementing the infinite region *becomes* the supplied area (it should be empty — `Complement` keeps
> the part of the argument **outside** this region), and the `Xor`/`Complement` tests that had been
> deliberately written around exactly-representable cases now assert the real semantics. Covered by
> `RegionBandsFunctionalTests`; carried in `patches/0087`.

### 2.8 `Graphics` quality/rendering modes are stored but never applied

`SmoothingMode` ([Graphics.cs:178](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:178)), `InterpolationMode` ([:114](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:114)), `CompositingMode` ([:83](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:83)), `PixelOffsetMode` ([:160](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:160)), `PageUnit`/`PageScale` ([:142](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:142)), `TextRenderingHint` ([:202](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:202)) and `RenderingOrigin` are all plain auto-properties. Grepping `Graphics.cs` finds no consumer of `SmoothingMode` and no `Antialias` call anywhere in the file.

So the very common:

```csharp
g.SmoothingMode = SmoothingMode.AntiAlias;
```

reads back correctly, sets nothing, and rendering is whatever Cairo defaults to. `PageUnit`/`PageScale` being inert is more serious: a `Graphics` set to `GraphicsUnit.Millimeter` or `Inch` will draw at pixel coordinates, so unit-aware drawing and printing code is geometrically wrong, not just cosmetically different.

**Full emulation requires:** mapping each mode onto its Cairo counterpart (`Antialias`, `Filter`, `Operator`) at draw time, and folding `PageUnit`/`PageScale` into the CTM.

> **Status: fixed.** `SmoothingMode` maps onto `Cairo.Antialias`, `CompositingMode` onto
> `Cairo.Operator`, `InterpolationMode` onto the Gdk filter used when images are resampled, and
> `PageUnit`/`PageScale` now fold into the current transform, so a Graphics in millimetres draws at the
> right size. Covered by `GraphicsEmulationGapFunctionalTests`; carried in `patches/0087`.


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

> **Status: fixed.** `PrintDialog.RunPrint` now honours its `showDialog` argument (it was ignored) and
> `PrintDocument.Print()` passes `false`, so printing is silent unless the caller asks for the dialog.


### 2.11 `Font.FromHfont` / `FromHdc` / `FromLogFont` return Arial 12

[Font.cs:478-514](Net4x.Gtk.Drawing/System/Drawing/Font.cs:478) — all four factory methods return `new Font("Arial", 12)` regardless of input. Silent wrong-font rather than a diagnosable failure. (These are interop entry points, so arguably they should throw `PlatformNotSupportedException` instead.)

> **Status: fixed.** All four now throw `PlatformNotSupportedException` with a message pointing at the
> family-name constructor, as this section suggested. Carried in `patches/0087`.


### 2.12 `ListView.VirtualMode` is spelled `VirtualModel`

[ListView.cs:712](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs:712) declares `VirtualModel`, not WinForms' `VirtualMode`. Source using virtual-mode list views fails to compile, and the designer-generated `.Designer.cs` from a real WinForms project will not build. This looks like a straightforward naming defect rather than a design decision. (`DataGridView.VirtualMode` at [DataGridView.cs:1238](Net4x.Gtk.Windows.Forms/System/Windows/Forms/DataGridView.cs:1238) is spelled correctly.)

> **Status: fixed.** The property is now `VirtualMode`; `VirtualModel` remains as an `[Obsolete]` alias
> so existing callers keep working. Carried in `patches/0088`.


### 2.13 `Form.AutoScaleMode` / `AutoScaleDimensions` are inert

[Form.cs:141](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs:141) and [:150](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs:150) are auto-properties. Every VS-designer-generated form sets `AutoScaleMode = AutoScaleMode.Font` and `AutoScaleDimensions = new SizeF(7F, 15F)`. With these inert, layout produced at one system font/DPI is not rescaled at another — which is precisely the scenario a cross-platform layer hits hardest, since Linux/macOS default font metrics differ from Windows'. `Application.SetHighDpiMode` ([Application.cs:583](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs:583)) should be audited on the same grounds.

> **Status: API implemented; automatic application deliberately withheld — and this is the interesting
> part of the finding.**
>
> `AutoScaleMode`/`AutoScaleDimensions` were inert auto-properties duplicated on both `Form` and
> `UserControl`. They now live on `ContainerControl`, where WinForms puts them, joined by
> `CurrentAutoScaleDimensions`, `AutoScaleFactor` and a `PerformAutoScale()` that genuinely rescales a
> container and its whole child tree.
>
> What could **not** be made correct is applying it automatically. A designer-generated form carries
> `AutoScaleDimensions = new SizeF(9F, 20F)` — a number the *Windows* designer recorded for Segoe UI 9pt.
> Measuring the running font through Pango and dividing gives the difference between two different
> typefaces, not the change of environment WinForms scales against. Wiring it into `ResumeLayout` shrank
> the sample application's controls to about 79% while leaving the window size untouched: dead space at
> the bottom and right, `"Split Container Layout"` clipped to `"Split Container …"`, `"Slider Value"`
> wrapped onto two lines, `button7` cut off. The UI-baseline tests caught it at 21–25% pixel difference.
>
> So the members are live and opt-in — call `PerformAutoScale()` and it really scales — but nothing
> rescales a form behind the caller's back. **Closing this properly needs a design-time metric that is
> commensurate with the runtime one**, e.g. persisting the font the layout was authored against rather
> than inheriting a Windows-derived constant.
>
> A related trap found on the way: giving `Control.Font` an ambient fallback (parent font, then a default)
> looks like an obvious fix for it being `null` on a fresh control — but returning a hardcoded font
> overrides the GTK theme font and shifts text metrics across the entire application. The UI baselines
> flagged it at 0.78–1.68%. `Font` is left returning `null`; the auto-scale measurement falls back locally.
>
> Covered by `AutoScaleFunctionalTests`; carried in `patches/0088`.

### 2.14 `Control.DrawToBitmap` is an empty method

[Control.cs:1895](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1895) — returns without touching the bitmap. Callers get a blank image, not an error. Used for print-preview, thumbnails, and report generation.

> **Status: fixed.** It now renders the widget when the control is realized, and otherwise lays down the
> control's background, so the caller always receives the control's appearance rather than nothing.
> Carried in `patches/0088`.


---

## 3. Class A — types that do not exist (compile-time breaks)

Verified absent from `Net4x.Gtk.Windows.Forms` by type-declaration search.

### 3.1 Rendering & metrics helpers — the highest-frequency break

| Missing type | Impact |
|---|---|
| **`TextRenderer`** | `TextRenderer.MeasureText` / `DrawText` is the *recommended* WinForms text API since .NET 2.0 and appears in an enormous share of custom-drawn WinForms code. Its absence is probably the most-hit compile error. |
| **`ControlPaint`** | `ControlPaint.DrawBorder3D`, `.Light`, `.Dark`, `.DrawFocusRectangle`, `.DrawCheckBox`… Ubiquitous in owner-draw code. |
| **`SystemInformation`** | Border widths, scrollbar sizes, double-click time, `MenuHeight`. Layout code reads these constantly. |

> **Status: fixed.** All three now exist.
>
> - `TextRenderer` — `MeasureText`/`DrawText` in every WinForms overload, measuring and drawing through
>   `Graphics` (Pango-backed) and honouring the alignment, `SingleLine` and padding bits of
>   `TextFormatFlags`. Implementing it surfaced a further defect: `Graphics.MeasureString` measured a
>   single line and then clamped to the layout area, so text that wraps in WinForms reported the width of
>   the box and the height of one line. It now lays the words out against the available width.
> - `ControlPaint` — the colour helpers (`Light`, `LightLight`, `Dark`, `DarkDark`, `DisabledColor`) and
>   the draw helpers (`DrawBorder`, `DrawBorder3D`, `DrawFocusRectangle`, `DrawCheckBox`, `DrawButton`,
>   `FillReversibleRectangle`). This also needed `ButtonBorderStyle`, which was missing.
> - `SystemInformation` — border, scrollbar, caption, menu and icon metrics, monitor geometry from GDK,
>   and the environment strings.
>
> `SystemBrushes` and `SystemFonts` (§P1 #8) were added at the same time, alongside the existing
> `SystemColors`/`SystemPens`. Covered by `RenderingHelpersFunctionalTests`; carried in `patches/0087`
> and `patches/0088`.

### 3.2 The `System.Windows.Forms.VisualStyles` namespace — entirely absent

`VisualStyleRenderer`, `VisualStyleElement`, and the whole renderer family (`ButtonRenderer`, `CheckBoxRenderer`, `RadioButtonRenderer`, `ComboBoxRenderer`, `TextBoxRenderer`, `ProgressBarRenderer`, `ScrollBarRenderer`, `GroupBoxRenderer`, `TabRenderer`, `TrackBarRenderer`). Note that `Application.EnableVisualStyles()` ([Application.cs:241](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs:241)) *does* exist — so the `Program.cs` boilerplate compiles while the namespace it enables does not.

There is a legitimate design argument that themed rendering should defer to the GTK theme rather than emulate Windows visual styles. If so, that is a *documented non-goal*, not a gap — but the types should still exist as compiling shims so source ports build.

### 3.3 ToolStrip infrastructure

Present: `ToolStrip`, `MenuStrip`, `StatusStrip`, `ContextMenuStrip`, and the item classes.
Missing: **`ToolStripManager`** (merging, `RevertMerge`, renderer selection), **`ToolStripRenderer`** / **`ToolStripProfessionalRenderer`** / **`ProfessionalColorTable`** (all custom ToolStrip theming), **`ToolStripContainer`**, **`ToolStripPanel`**, **`ToolStripContentPanel`** (rafting/docking of toolbars — designer-generated for any multi-toolbar form), **`ToolStripControlHost`** (hosting arbitrary controls in a strip).

### 3.4 Common controls

`Splitter` (the legacy splitter, distinct from the present `SplitContainer`), `DomainUpDown` and `UpDownBase` (`NumericUpDown` exists but does not derive from a real `UpDownBase`), `ErrorProvider`, `HelpProvider`, `BindingNavigator`, `WebBrowser` (its four `*EventArgs` types are present — [see the type list] — but the control itself is not, so the event args are unreachable), `HtmlDocument`/`HtmlElement`.

> **Partly fixed.** `ErrorProvider` and `Splitter` now exist, along with the `ErrorIconAlignment` and
> `ErrorBlinkStyle` enumerations.
>
> - **`ErrorProvider`** is an extender of the same shape as `ToolTip`: the error text is stored, reported by
>   `GetError`, and surfaced on the control itself. **The adjacent blinking icon is not reproduced** —
>   placing a floating adornment next to an arbitrary child needs layout surgery in the GTK container
>   model. `Icon`, `BlinkStyle`, per-control alignment and padding all round-trip so designer output and
>   application logic survive intact; only the visual indicator differs.
> - **`Splitter`** docks to an edge and resizes the sibling docked to the same edge, honouring
>   `MinSize`/`MinExtra` and raising `SplitterMoving`/`SplitterMoved`. **Interactive dragging is not wired
>   to GTK input**; setting `SplitPosition` from code performs the resize.
>
> Still missing from this section: `DomainUpDown`/`UpDownBase`, `HelpProvider`, `BindingNavigator`,
> `WebBrowser`, `HtmlDocument`/`HtmlElement`. Covered by `CommonControlsFunctionalTests`; carried in
> `patches/0088`.

### 3.5 Legacy menu/status API

~~`MainMenu`, `MenuItem`, `ContextMenu`, `Menu`, `StatusBar`, `StatusBarPanel`~~ — **done**. These are obsolete in modern WinForms but are exactly what an *old* application — the migration target this project exists to serve — will be using.

> **Status: fixed.** `Menu`, `MenuItem`, `MainMenu`, `ContextMenu`, `StatusBar` and `StatusBarPanel` now
> exist, along with the `Shortcut` enum and the `StatusBarPanel*` enums and event args.
>
> They are adapters over the ToolStrip family, which is what actually renders: `MainMenu` drives a
> `MenuStrip`, `MenuItem` a `ToolStripMenuItem`, `ContextMenu` a `ContextMenuStrip`, `StatusBarPanel` a
> `ToolStripStatusLabel`. `StatusBar` derives from `StatusStrip` so it is a real dockable control. The
> legacy hierarchy is preserved (`MenuItem : Menu`), so `Menu m = someMenuItem;` compiles.
>
> `Form.Menu` and `Control.ContextMenu` attach them and also set `MainMenuStrip`/`ContextMenuStrip`, so
> legacy and modern menu code share one attachment point — an old form and a new one can coexist.
>
> **Known limitation:** `ContextMenu.Show(control, position)` cannot place the menu. `ContextMenuStrip`
> derives from `ToolStripItem` rather than `Control` in this shim, so it exposes no `Location` and no
> `Show(control, position)`; the menu is made visible and GTK positions it. `SourceControl`, the item
> contents and the `Popup` event are exact.
>
> Adding `Menu` also reintroduced a name collision: `ToolStripItemCollection` had an unqualified `Menu`
> field that had been binding to `Gtk.Menu`. It is now qualified.
>
> Covered by `LegacyMenuFunctionalTests`; carried in `patches/0088`.

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
| ~~**`ToolboxBitmapAttribute`**~~ — **done** | Applied to custom control classes; a compile break on the *declaration*, which is worse than on a call site. Now resolves the image from assembly resources or disk, returning `null` for a missing image as the framework does. Carried in `patches/0087`. |

> **Partly fixed.** `TextureBrush` (Cairo surface pattern, tile and clamp wrap modes), `SystemFonts`,
> `SystemBrushes` and `SystemIcons` (GTK icon theme where available, drawn otherwise — no shipped assets)
> now exist. Still missing from this table: `BufferedGraphics`/`BufferedGraphicsContext`/
> `BufferedGraphicsManager`, `PrivateFontCollection`, `InstalledFontCollection`, `ToolboxBitmapAttribute`.
> Covered by `TextureBrushAndSystemIconsFunctionalTests`; carried in `patches/0087`.

**Correctly delegated, not a gap:** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are absent from the project by design — `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the real BCL types. This is the right call and should be preserved.

---

## 4. Class B — present but throwing `NotImplementedException`

Loud failures. Ordered by likely hit rate.

### 4.1 `Graphics` (18 sites, [Graphics.cs](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs))

> **Partly fixed.** The eight `IsVisible` overloads now answer from the tracked clip bounds instead of
> throwing. Covered by `GraphicsEmulationGapFunctionalTests`; carried in `patches/0087`.


- **All eight `IsVisible` overloads** ([:1636](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1636)–[:1823](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1823)). This is a clipping/culling primitive used in the paint loop of any scrolling custom control — `if (!g.IsVisible(itemRect)) continue;`. Given that `Clip` bounds *are* tracked ([:37](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:37)), these look cheap to implement and are high value.
- `Transform` getter ([:207](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:207)) — see §2.9.
- `TextContrast` getter ([:189](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:189)) — low value; note the setter silently accepts, so getter-throws/setter-ignores is an odd asymmetry.
- `MeasureCharacterRanges` ([:1831](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1831)) — *partially* addressed; the comment records that it now returns one region per range positioned approximately, i.e. it no longer throws but is not accurate. Text selection/hit-testing in custom text controls needs real per-range extents (Pango can supply these).
- Interop entry points, correctly hopeless: `FromHdc`, `FromHdcInternal`, `FromHwnd`, `FromHwndInternal`, `GetHdc`, `GetHalftonePalette`. These should arguably throw `PlatformNotSupportedException` — that is the documented .NET convention and it tells the caller "never going to work" instead of "not written yet."

### 4.2 `Clipboard` / `DataObject` ([OLE/Clipboard.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs))

`GetDataPresent(string)`, `GetDataPresent(Type)`, `GetDataPresent(string, bool)`, `GetFormats()`, `GetFormats(bool)` all throw ([:137](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs:137)–[:157](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs:157)). `GetDataPresent` is the *guard* every well-written paste handler calls before `GetData` — so the defensive code path throws while the naive one works. Drag-and-drop `DragEventArgs.Data.GetDataPresent(...)` in `DragEnter` is the same pattern and equally affected.

> **Status: fixed.** `GetDataPresent` (all three overloads) and `GetFormats` (both) now answer from the
> clipboard's text/image availability instead of throwing, so the guarded paste idiom works. Note the
> reported format list covers what `GetData` can actually return — the GtkSharp `WaitForTargets` binding
> cannot be called from managed code, so the full advertised target list is unavailable. Covered by
> `ClipboardFormatsFunctionalTests`; carried in `patches/0088`.
>
> Separately noted while testing: this shim nests `DataObject` **inside** `Clipboard`, where WinForms
> declares it at namespace level, so `new DataObject()` does not compile against the shim.

### 4.3 Control-level throw counts

By file, highest first:

| File | Sites |
|---|---|
| ~~[LinkLabel.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/LinkLabel.cs)~~ | ~~24~~ → **0** |
| [ToolStripItem.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs) | 15 |
| ~~[TabControl.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/TabControl.cs)~~ | ~~13~~ → **0** |
| ~~[ListBox.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListBox.cs)~~ | ~~13~~ → **0** |
| [ListViewGroupConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupConverter.cs) | 8 |
| [PropertyGrid.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGrid.cs) | 6 |
| [ListViewItemConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewItemConverter.cs) | 6 |
| [GridEntry.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGridInternal/PropertyGridBase/GridEntry.cs) | 5 |
| [ListView.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs), [ComponentDesigner.cs](Net4x.Gtk.Windows.Forms/System/ComponentModel/Design/Designer/ComponentDesigner.cs), [ListViewGroupEventArgs.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupEventArgs.cs) | 4 each |

> **`TabControl` and `ListBox` are now at 0 as well.** Both showed the same shape as `LinkLabel`: a
> working public surface with the gaps just behind it. Every explicit `IList`/`ICollection` member of
> `TabPageCollection` threw, so `IList list = tabControl.TabPages` failed; and the whole `FindString`
> family of `ListBox` threw, along with `DrawMode`. Fixing `IndexFromPoint` also removed a latent
> `NullReferenceException` — the `int` overload dereferenced the null `GetRowAtY` returns when the point
> is past the last item. Covered by `TabControlListBoxGapFunctionalTests`; carried in `patches/0088`.

`LinkLabel` standing at 24 is notable: it is a simple, commonly-used control, which suggests it was stubbed wholesale rather than partially implemented.

> **Status: fixed — and the suspicion in that sentence was right.** `LinkLabel` is now at **0** throw sites.
> The stubbing went deeper than the count suggested:
>
> - `Links` was an auto-property nothing ever assigned, so it was `null` — every use threw a
>   `NullReferenceException` before it even reached the unimplemented members.
> - `LinkCollection` had **no backing store**. `Add` overwrote the owner's `Text` with the value's string
>   form and returned a hard-coded `1`; `Clear` wiped the `Text`; `Contains` did a substring search on it.
>   Everything else threw.
> - `Link` discarded its constructor arguments and had no `Start`, `Length`, `Enabled`, `Visited`, `Name`
>   or `Tag` — so a link could not describe the span of text it covered.
>
> A multi-link `LinkLabel`, the reason the control exists over a plain `Label`, was therefore impossible.
> All three layers are now implemented, including keyed lookup and the `IList` face.
>
> Three existing tests had to be replaced: they asserted the broken behaviour as the contract
> (`Add` setting `Text`, `Clear` wiping it, `Contains` matching a substring). Covered by
> `LinkLabelLinksFunctionalTests`; carried in `patches/0088`.



### 4.4 `Drawing2D` remainder

- `CustomLineCap` — 5 throw sites; effectively unimplemented. Custom arrowheads/caps on pens are unavailable.
- ~~`Matrix.Elements`, `Matrix.IsIdentity`, `Matrix.IsInvertible`~~ — **fixed**; all three now report real values, so a transform can be inspected and round-tripped. Carried in `patches/0087`.
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
6. ~~`TextRenderer` (measure + draw over Pango)~~ — **done**.
7. ~~`ControlPaint`~~ — **done**.
8. ~~`SystemFonts`, `SystemBrushes`, `SystemIcons`~~ — **done**.
9. ~~`TextureBrush`~~ — **done**, over a Cairo surface pattern.

**P2 — rendering fidelity**
10. Wire `SmoothingMode`/`InterpolationMode`/`CompositingMode` to Cairo (§2.8).
11. Honour `PageUnit`/`PageScale` in the CTM — currently a geometric error, not a cosmetic one (§2.8).
12. `Graphics.IsVisible` ×8 — the clip bounds are already tracked (§4.1).
13. ~~Real `Region`~~ — **done** via a band list; curved paths still contribute their bounds (§2.7).

**P3 — behavioural**
14. `SetStyle`/`GetStyle` backing state, at minimum for `UserPaint`, `OptimizedDoubleBuffer`, `ResizeRedraw`, `AllPaintingInWmPaint` (§2.4).
15. ~~`Clipboard.GetDataPresent` / `GetFormats`~~ — **done** (§4.2).
16. `AutoScaleMode` — **API done, auto-application blocked** on a commensurate design-time metric (§2.13).
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

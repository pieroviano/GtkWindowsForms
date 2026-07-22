# Emulation gaps — `Net4x.Gtk.Windows.Forms` vs `System.Windows.Forms`, `Net4x.Gtk.Drawing` vs `System.Drawing`

**Bar being measured against.** The stated intent: *an existing WinForms application should compile unchanged against these two assemblies and behave the same on Windows, Linux and macOS.* Everything below is scored against that.

**Method and its limits.** Findings come from reading the generated sources in `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` at the current working tree (post-`f58a01a8`). Nothing here was produced by running the library. Static reading reliably finds missing types, throwing members and inert auto-properties; it does **not** find layout drift, event-ordering divergence, or focus/tab-order differences. See §7.

**Caveat on line numbers.** Both directories are *generated* by the prepare pipeline from the `gtksystem-windows-forms` submodule (see [CLAUDE.md](CLAUDE.md)). Line references are accurate now and will drift after the next pipeline run; type and member names will not.

---

## 1. Executive summary

Three classes of gap, ordered by how much damage they do:

| Class | Meaning | Why it matters |
|---|---|---|
| **A — Missing type** | Does not exist. | *Compile-time* break. "Compile once" fails immediately. |
| **B — Present but throwing** | Exists, throws `NotImplementedException`. | *Runtime* failure — but loud and diagnosable. |
| **C — Present but silently wrong** | Exists, does something plausible but not WinForms-equivalent. | Worst: builds, runs, quietly misbehaves. |

**State of play.** A prior pass ([`817026d2` "Address emulation gaps"](.)) closed most of the Class C findings and a large block of Class A. Verified closed — see §6 for the full list — are the three threading defects, `SetStyle`/`GetStyle`, `Graphics.Transform`, the quality/unit modes, `Region`'s single-rectangle backing, `ListView.VirtualMode`, `Clipboard.GetDataPresent`, `PrintDocument.Print`, `AutoScaleMode`, the data-binding value chain, and roughly a dozen missing types including `TextRenderer`, `ControlPaint` and `SystemInformation`.

**What that leaves.** The remaining profile is different in character from the last review. Class C is now down to essentially two items (§2). The dominant categories today are **missing types clustered in three coherent families** (§3) and **`Application`'s startup/lifecycle surface** (§5.2). Those are additive work, not correctness archaeology — a better position to be in.

---

## 2. Class C — still present, still silently wrong

Only two survive, but both are real.

### 2.1 `Control.CreateGraphics()` returns a deferred off-screen surface

[Control.cs:2003](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:2003) allocates a fresh Cairo `ImageSurface`/`Context` per call and returns a `Graphics` over it; `Override_PaintGraphics` blits that surface during the next paint cycle. The comment at [Control.cs:61](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:61) explains the constraint — drawing to the live GDK window during realize throws.

Two deviations from WinForms:

- **Deferred, not immediate.** In WinForms `CreateGraphics()` yields an immediate-mode DC: you draw, it appears now, outside any paint event. Here the effect waits for the next expose.
- **Single live surface.** Each call disposes the previous surface, so two `Graphics` objects on one control cannot coexist.

The pattern this breaks is the classic rubber-band: `CreateGraphics()` in `MouseMove` to XOR a selection rectangle. That code compiles and runs and does the wrong thing.

Worth noting this one is *substrate-bound* — GTK's retained paint cycle genuinely does not offer an immediate-mode DC on an unrealized widget. A faithful fix means either accepting a documented deviation or routing such drawing through an overlay the paint cycle composites. This may belong in §5's non-goals rather than on the fix list; that is a design call.

### 2.2 `WndProc` is an empty no-op

[Control.cs:4046](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:4046):

```csharp
protected virtual void WndProc(ref Message m)
{
    //GtkLogger.Info($"HWnd:{m.HWnd},WParam:{m.WParam},LParam:{m.LParam},Msg:{m.Msg}");
}
```

`Message` and `WndProc` exist so message-pumping code *compiles*, but nothing ever feeds them. Overriding `WndProc` to intercept `WM_NCHITTEST`, `WM_ERASEBKGND`, drag-move, or custom non-client painting builds cleanly and silently does nothing.

This is Class C only because it is silent. See §5.1 — the recommendation is to convert it to a loud, documented non-goal rather than to implement it.

---

## 3. Class A — types that still do not exist

Verified absent by type-declaration search over both projects.

### 3.1 The `VisualStyles` namespace — entirely absent

`VisualStyleRenderer`, `VisualStyleElement`, and the renderer family (`ButtonRenderer`, `CheckBoxRenderer`, `RadioButtonRenderer`, `ComboBoxRenderer`, `TextBoxRenderer`, `ProgressBarRenderer`, `ScrollBarRenderer`, `GroupBoxRenderer`, `TabRenderer`, `TrackBarRenderer`).

Note the trap: `Application.EnableVisualStyles()` ([Application.cs:241](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs:241)) *does* exist, so the standard `Program.cs` boilerplate compiles while the namespace it nominally enables does not.

There is a good design argument that themed rendering should defer to the GTK theme rather than emulate Windows visual styles. If that is the position, it is a *non-goal*, not a gap — but the types should still exist as compiling shims so ported source builds. That distinction matters: a missing type breaks the build; a shim that draws GTK-themed output merely looks different.

### 3.2 ToolStrip infrastructure — the largest coherent hole

Present: `ToolStrip`, `MenuStrip`, `StatusStrip`, `ContextMenuStrip` and the item classes.

Missing:

| Type | What it costs |
|---|---|
| `ToolStripManager` | Merging, `RevertMerge`, renderer selection |
| `ToolStripRenderer`, `ToolStripProfessionalRenderer`, `ProfessionalColorTable` | All custom ToolStrip theming |
| `ToolStripContainer`, `ToolStripPanel`, `ToolStripContentPanel` | Toolbar rafting/docking — **designer-generated for any multi-toolbar form** |
| `ToolStripControlHost` | Hosting arbitrary controls in a strip |

`ToolStripContainer` is the sharp edge: it appears in `.Designer.cs` output, so affected projects fail to build on a file the developer does not hand-maintain.

### 3.3 Controls

`DomainUpDown`, `UpDownBase` (`NumericUpDown` exists but does not derive from a real `UpDownBase`), `BindingNavigator`, `WebBrowser`, `HtmlDocument`/`HtmlElement`.

`WebBrowser` deserves a note: its four `*EventArgs` types (`WebBrowserDocumentCompletedEventArgs`, `WebBrowserNavigatedEventArgs`, `WebBrowserNavigatingEventArgs`, `WebBrowserProgressChangedEventArgs`) are all present, but the control itself is not — so those types are currently unreachable. Either the control is coming or the event args are dead weight.

### 3.4 Interop, help and misc

`NativeWindow`, `SendKeys`, `Help`, `HelpProvider`, `AmbientProperties`, `IWindowsFormsEditorService`, `AutoCompleteStringCollection` (the `AutoCompleteMode` enum exists, so the feature is half-declared).

Absent on `Control`: `Region` (non-rectangular control shapes). Absent on `Form`: `TransparencyKey`.

### 3.5 `Net4x.Gtk.Drawing`

Only two remain:

- **`PrivateFontCollection`** — apps that ship embedded fonts cannot load them.
- **`InstalledFontCollection`** — font enumeration.

The base `FontCollection` exists ([FontCollection.cs](Net4x.Gtk.Drawing/System/Drawing/Text/FontCollection.cs)) with neither concrete subclass, which is the same half-declared shape as `AutoCompleteMode`. Both are backable by Pango/fontconfig.

**Correctly delegated, not a gap:** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are deliberately absent — `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the genuine BCL types. Preserve this.

---

## 4. Class B — remaining `NotImplementedException` sites

Down sharply from the last pass. `LinkLabel` (was 24), `TabControl` (13) and `ListBox` (13) are now clear.

### 4.1 `Net4x.Gtk.Windows.Forms`

| File | Sites |
|---|---|
| [ToolStripItem.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/MenuStripBase/ToolStripItem.cs) | 11 |
| [ListViewGroupConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupConverter.cs) | 8 |
| [PropertyGrid.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGrid.cs) | 6 |
| [ListViewItemConverter.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewItemConverter.cs) | 6 |
| [GridEntry.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGridInternal/PropertyGridBase/GridEntry.cs) | 5 |
| [ListView.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs), [ComponentDesigner.cs](Net4x.Gtk.Windows.Forms/System/ComponentModel/Design/Designer/ComponentDesigner.cs), [ListViewGroupEventArgs.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupEventArgs.cs) | 4 each |
| [ListViewGroupImageIndexer.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ListViewGroupImageIndexer.cs) | 3 |
| [ColumnHeader.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListViewBase/ColumnHeader.cs), [Control.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs) | 2 each |
| `TableLayoutPanel`, `DataGridViewCellStyle`, `DataGridViewCellLinkedList`, `DataGridViewCellCollection`, `ArrangedElement` | 1 each |

Two clusters stand out:

**`ToolStripItem` (11)** — the throwing members are `Children`, `ParticipatesInLayout`, `BackgroundImageLayout`, `CheckState`, `Created`, `Parent`, `Width` and the `SuspendLayout`/`ResumeLayout`/`CreateControl` trio. That is the `IArrangedElement` contract plus core layout state, i.e. anything that lays out or measures a ToolStrip item hits one. Combined with §3.2, ToolStrip is the weakest subsystem in the library.

**`ListView` group/converter machinery (25 across five files)** — `ListViewGroupConverter`, `ListViewItemConverter`, `ListViewGroupEventArgs`, `ListViewGroupImageIndexer`, `ListView` itself. The converters are what the designer and ResX serialization use, so this is a design-time surface as much as a runtime one.

`Control.cs`'s two remaining are both explicit-interface `IArrangedElement` members — `Properties` ([:1756](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1756)) and `PerformLayout` ([:3011](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:3011)) — the same contract that `ToolStripItem` leaves unimplemented. Worth fixing together.

### 4.2 `Net4x.Gtk.Drawing` — effectively clear

`Graphics.cs` has one remaining mention and `Image.cs` one; both are in comments or `catch` clauses, not throw sites. The interop entry points were converted to `PlatformNotSupportedException` — `Graphics.FromHdc`/`FromHwnd`/`GetHdc`/`GetHalftonePalette` (7 sites), `Font.FromHdc`/`FromHfont`/`FromLogFont` (4), `Icon.FromHandle`, `BufferedGraphics`, `BufferedGraphicsContext`. That is the right convention: it tells a caller "never" rather than "not yet."

**Remaining accuracy caveat, not a throw:** `MeasureCharacterRanges` ([Graphics.cs:1963](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1963)) no longer throws but returns approximate regions positioned by estimate. Text selection and hit-testing in custom text controls need true per-range extents. Pango exposes these; the approximation is a placeholder.

---

## 5. Structural gaps — design decisions, not patches

### 5.1 No Win32 message pump

`Message`, `WndProc` (§2.2), `IMessageFilter`/`Application.AddMessageFilter`, `NativeWindow`, `DefWndProc` — the whole message-interception layer is absent or inert. Any application reaching below the event model is unportable by construction.

**Recommendation:** declare this a non-goal and make the affected members throw `PlatformNotSupportedException` on first use rather than no-op. `Net4x.Gtk.Drawing` already adopted exactly this convention for its interop surface (§4.2); the Forms side has not followed. Doing so converts §2.2 from a silent Class C into a loud, honest boundary — the single cheapest correctness improvement left in the library.

Related and inherent: `Control.Handle` ([Control.cs:1126](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1126)) returns `Widget.Handle`, a GTK handle, not an `HWND`. Correct-by-necessity, but it should be stated in the package README so integrators do not discover it in production.

### 5.2 `Application` startup and lifecycle is thin — now the top open gap

Present: `Run(Form)`, `Exit`, `ExitThread`, `DoEvents`, `EnableVisualStyles`, `SetCompatibleTextRenderingDefault`, `SetHighDpiMode`, `OpenForms`, the path properties.

Missing:

- **`Run()` and `Run(ApplicationContext)`** — `ApplicationContext` exists as a type with no `Run` overload to consume it, so the multi-form / no-main-form startup pattern has **no entry point at all**. This is the notable one: it is not a rarely-used corner, it is one of the two standard ways to start a WinForms app.
- `Idle`, `ApplicationExit`, `ThreadExit` events.
- `ThreadException` / `SetUnhandledExceptionMode` — the standard global UI exception handler. Its absence means an unhandled exception on the UI thread has no application-level hook.
- `MessageLoop`, `AddMessageFilter`, `RemoveMessageFilter` (see §5.1).
- `Restart`, `ProductName`, `ProductVersion`, `CompanyName`, `RenderWithVisualStyles`, `VisualStyleState`.

### 5.3 Metafiles are declared, not implemented

`Metafile`, `MetafileHeader`, `MetaHeader`, `WmfPlaceableFileHeader`, `EmfPlusRecordType`, `PlayRecordCallback` all exist as types under [Imaging/](Net4x.Gtk.Drawing/System/Drawing/Imaging/Drawing/Drawing2D/), but EMF/WMF is a Windows record format with no cross-platform playback. Almost certainly a permanent non-goal; label it as one.

### 5.4 `Region` fidelity — improved, now bounded by representation

`Region` was a single `RectangleF`; it is now backed by a band list ([Region.cs:36](Net4x.Gtk.Drawing/System/Drawing/Region.cs:36)) with `Compact`/`BoundsOf` helpers, and `Region(GraphicsPath)` decomposes the path into rectangles.

The residual limit is inherent to rectangle decomposition: **curved boundaries are approximated**, and the approximation's accuracy depends on how the path is decomposed. For clipping to an ellipse or a rounded rectangle this is visible at the edges. Cairo can clip to an arbitrary path natively, so routing `Graphics.SetClip(Region)` through the region's `GraphicsPath` rather than its bands would be exact. Whether current fidelity is adequate is a question for a rendering test, not for source reading — flagged, not asserted.

### 5.5 Accessibility is nominal

`AccessibleObject`, `AccessibleRole`, `AccessibleStates`, `AccessibleEvents` exist. Whether they are wired to ATK (GTK's accessibility bus) is **not established by this review**. If they are not, screen-reader support is absent on every platform despite the API being present — which would be a Class C gap of the most consequential kind. This needs a runtime check before anyone relies on it.

### 5.6 Data binding — value flow fixed, depth unverified

The prior pass fixed the four-way break that made bindings write `null` (§6). Now established by `BindingPipelineFunctionalTests`: `Format`/`Parse` events, `FormatString`/`FormatInfo`/`NullValue` round-trips, `DataSourceUpdateMode` (`OnPropertyChanged` required fixing `TextBox.TextChanged`, which was suppressed while invisible; `Never`/`OnValidation` verified), `AddNew`/`AddingNew`/`CancelEdit`, related binding sources following parent currency, and `CurrencyManager` position/allow semantics. Still unestablished: `IEditableObject` transactions and `INotifyPropertyChanged` propagation from the data source side.

### 5.7 Printing model and `Image.RawFormat` — recorded deviations

The `System.Drawing.Printing` model types now behave (see §6), with three deliberate deviations from real WinForms:

- **Enum numbering differs** — `PaperKind` (`Custom = 0`, not 256), `PaperSourceKind` (0-based, `Custom = 13`, not 257) and `PrinterResolutionKind` (`High = 1`…, not −4…) do not use the DEVMODE values. Code comparing `RawKind` against Win32 `DMPAPER_*`/`DMBIN_*` constants will misread them.
- **Virtual-device defaults** — with no print system, `PageSettings` defaults to Letter (850×1100), `PaperSourceKind.FormSource` and a 200×200 custom resolution, and `PrintableArea` is the whole page (no hard margins). Real WinForms reads these from the default printer.
- **`Image.RawFormat` on fresh bitmaps reports `Bmp`, not `MemoryBmp`** — the shim keeps an encoded buffer per image and `RawFormat` defaults to `Bmp` when the image has no source format. Stream-loaded images report their detected format correctly.

`PrintDocument.Print()` runs through the GTK print dialog machinery and stays untested (needs a live print loop).

---

## 6. Verified closed since the last review

Confirmed against the current tree, not taken on trust from the commit message.

**Threading** — `InvokeRequired` now compares against the owning thread ([Control.cs:1249](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:1249), with `_owningThreadId` and a static `UiThreadId`); `Invoke` marshals synchronously with an inline fast path and exception rethrow; `BeginInvoke` posts to the main loop instead of the thread pool. Covered by `ControlThreadingFunctionalTests`.

**Data binding** — `Control.DataBindings`, `Control.BindingContext`, `Control.Created` and `Binding.CheckBinding`/`UpdateIsBinding` all corrected; covered by `BindingValueFlowFunctionalTests`.

**Rendering / `Graphics`** — `Transform` getter reports the live Cairo matrix ([Graphics.cs:291](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:291)); `SmoothingMode` maps to `Cairo.Antialias` ([:251](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:251)); `PageUnit`/`PageScale` fold into the CTM via `ApplyPageTransform` ([:220](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:220)); all eight `IsVisible` overloads implemented ([:1915](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs:1915)); `Matrix.Elements` returns the live values ([Matrix.cs:126](Net4x.Gtk.Drawing/System/Drawing/Drawing2D/Matrix.cs:126)); `Pen.PenType` and `CustomLineCap` clear.

**Control behaviour** — `SetStyle`/`GetStyle` back onto real `_controlStyles` state ([Control.cs:3990](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:3990)); `DrawToBitmap` renders ([:2059](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs:2059)); `AutoScaleMode`/`AutoScaleDimensions` moved to `ContainerControl` with a working `PerformAutoScale` ([ContainerControl.cs:91](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/ContainerControl.cs:91)).

**API correctness** — `ListView.VirtualMode` correctly spelled, with `VirtualModel` retained as an `[Obsolete]` forwarder ([ListView.cs:716](Net4x.Gtk.Windows.Forms/System/Windows/Forms/ListView.cs:716)) — a good compatibility choice; `Clipboard.GetDataPresent`/`GetFormats` implemented; `PrintDocument.Print()` prints silently ([PrintDocument.cs:189](Net4x.Gtk.Windows.Forms/System/Drawing/Printing/PrintDocument.cs:189)).

**Types added** — Forms: `TextRenderer`, `ControlPaint`, `SystemInformation`, `ErrorProvider`, `Splitter`, `StatusBar`, `StatusBarPanel`, `MainMenu`, `MenuItem`, `ContextMenu`. Drawing: `TextureBrush`, `SystemFonts`, `SystemBrushes`, `SystemIcons`, `BufferedGraphics`, `BufferedGraphicsContext`, `BufferedGraphicsManager`, `ToolboxBitmapAttribute`.

**DataGridView row/column model** (`DataGridViewRowCollectionFunctionalTests`) — `DataGridViewRow.State` reports the real flag combination (it was hard-coded to `None`, so every state-filtered operation matched nothing); the `GetFirstRow`/`GetNextRow`/`GetPreviousRow`/`GetLastRow`/`GetRowCount`/`GetRowsHeight` family tests flags, honours the exclude filter and scans correct ranges; `Rows.Insert(index, count)` no longer throws `NullReferenceException`; `DataGridViewColumn.Clone` returns a configured copy instead of null; `DisplayIndex = 0` is reachable.

**ListView sorting** (`ListViewSortAndKeysFunctionalTests`) — `Sort()` honours `ListViewItemSorter` by sorting the item model; it was a no-op unless a sort column was set. Remaining approximation: the visual order of an already-realized view still follows column sorting only.

**Control plumbing** (`ControlPlumbingFunctionalTests`, 21 tests) — `FindForm` no longer loops forever for controls nested below the first level (the parent walk re-read `this.Parent`); `EndInvoke` rethrows a faulted delegate's exception instead of returning the `Exception` object; `DeviceDpi` reports the GDK screen resolution (≥ 96) instead of a never-assigned 0, with `LogicalToDeviceUnits` scaling by it.

**Printing model** (`PrintingFunctionalTests`, 54 tests) — `PaperSize.Kind`/`PaperSource.Kind` report the stored kind instead of a constant; `PageSettings.Bounds` is computed from the paper size and `Landscape` (was a stored, always-empty rectangle) with non-null paper/source/resolution defaults; `PrinterSettings.Copies` defaults to 1 and `Copies`/`FromPage`/`ToPage`/`MinimumPage`/`MaximumPage`/`PrintRange` validate their ranges; `IsDirectPrintingSupported` distinguishes JPEG/PNG instead of returning false unconditionally; the `PrintDocument.PageSetup` setter no longer swaps the top/right margins and converts GTK points into hundredths of an inch. Underneath it, the `Image.Pixbuf` setter no longer re-encodes unknown-format buffers as lossy JPEG nor overwrites `RawFormat` — a stream-loaded BMP used to come back as degraded `Jpeg` (deviations that remain: §5.7).

---

## 7. Priority order

Ranked by (damage × frequency) ÷ effort.

**P0 — silent wrongness and no-entry-point**
1. **`Application.Run(ApplicationContext)` and `Run()`** (§5.2) — a standard startup pattern with no entry point at all.
2. **`WndProc` → `PlatformNotSupportedException`** (§2.2, §5.1) — converts a silent no-op into an honest boundary. Nearly free; follow the convention `Net4x.Gtk.Drawing` already adopted.
3. **Verify accessibility is wired to ATK** (§5.5) — cheap to check, severe if it is not.

**P1 — compile breaks on designer-generated code**
4. `ToolStripContainer`, `ToolStripPanel`, `ToolStripContentPanel` (§3.2) — these appear in `.Designer.cs`.
5. `ToolStripItem`'s `IArrangedElement` members, together with `Control.cs`'s two (§4.1) — one contract, two files.
6. `ToolStripManager`, `ToolStripRenderer`, `ToolStripProfessionalRenderer`, `ProfessionalColorTable`, `ToolStripControlHost`.
7. `VisualStyles` namespace as compiling shims over GTK theming (§3.1) — decide shim-vs-non-goal first.

**P2 — remaining controls and collections**
8. `ListView` group/converter cluster (§4.1) — 25 sites, design-time as well as runtime.
9. `UpDownBase`/`DomainUpDown`, `BindingNavigator`, `AutoCompleteStringCollection`.
10. `PrivateFontCollection`/`InstalledFontCollection` over Pango/fontconfig (§3.5).
11. `WebBrowser` — or delete the four orphaned event-args types (§3.3).

**P3 — fidelity**
12. `MeasureCharacterRanges` with true Pango extents (§4.2).
13. `Graphics.SetClip(Region)` through the region's path rather than its bands (§5.4).
14. `CreateGraphics` immediate-mode semantics — or document the deviation (§2.1).
15. `Application` events: `Idle`, `ApplicationExit`, `ThreadException`/`SetUnhandledExceptionMode` (§5.2).

**P4 — declare non-goals**
16. EMF/WMF metafiles, `NativeWindow`, `SendKeys`, `Control.Handle` semantics — document rather than implement (§5.1, §5.3).

---

## 8. What this review did not cover

Stated so the gaps in the gap analysis are visible.

- **No runtime behaviour was exercised.** Source reading only. Layout drift, event ordering, and focus/tab-order divergence are likely a large additional category and are invisible to this method.
- **Layout engines** — `TableLayoutPanel`, `FlowLayoutPanel`, anchor/dock — were not compared against WinForms measure/arrange semantics. These are fully reimplemented on GTK's allocate cycle; assume pixel-level and edge-case divergence until tested.
- **Data binding depth** (§5.6) and **accessibility wiring** (§5.5) — flagged as unverified, not assessed.
- **`Net4x.Gtk.Windows.FormsDesigner`** and the ResX/designer round-trip were out of scope.
- **Per-member API-surface diffing.** A mechanical comparison of the real `System.Windows.Forms` / `System.Drawing` reference assemblies against these two — public members, signatures, attributes — would be strictly more complete than this document at the type level, and would catch further cases of the `VirtualModel` kind: subtly renamed or re-signatured members, which are the most insidious because the designer emits code against the real names. **This remains the highest-value follow-up and it can be automated.** It also replaces recall-based assertion with ground truth, which is the right way to settle the questions §5.6 and §7 currently leave open.

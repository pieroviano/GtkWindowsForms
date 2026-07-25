# Emulation gaps — `Net4x.Gtk.Windows.Forms` vs `System.Windows.Forms`, `Net4x.Gtk.Drawing` vs `System.Drawing`

**Bar being measured against.** The stated intent: *an existing WinForms application should compile unchanged against these two assemblies and behave the same on Windows, Linux and macOS.* Everything below is scored against that.

**This document lists only what is still open.** Earlier revisions carried a large record of already-closed findings (three threading defects, `SetStyle`/`GetStyle`, `Graphics.Transform`, `Region` banding, data-binding value flow, `Padding` arithmetic, the `IArrangedElement` contract, `Application.Run(ApplicationContext)`, accessibility wiring, the `VisualStyles` namespace, the ToolStrip container/panel/renderer family, `PrivateFontCollection`/`InstalledFontCollection`, `MeasureCharacterRanges`, the full data-binding depth surface, and — as of patches 0110 and 0111 — the entire `NotImplementedException` set, `Application`'s residual lifecycle members, shaped controls, colour-keyed transparency and exact curved-region combination). Those sections were pruned once verified — the history is in git and the surviving evidence is the functional-test suite that pins each one. What remains here is the open frontier.

> **See also — [API-COMPLETENESS.md](API-COMPLETENESS.md).** The per-member API-surface diff run against the
> real .NET reference assemblies supersedes this document on *what exists* (type and member counts, renamed
> or re-signatured members). This document is authoritative on *behaviour* — members that exist and compile
> but do something other than what WinForms does, which a metadata diff cannot see.

**Method and its limits.** Findings come from reading the generated sources in `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` and from the functional-test suite. Static reading reliably finds missing types, throwing members and inert auto-properties; it does **not** find layout drift, event-ordering divergence, or focus/tab-order differences — those need tests. See §6.

**Caveat on line numbers.** Both directories are *generated* by the prepare pipeline from the `gtksystem-windows-forms` submodule (see [CLAUDE.md](CLAUDE.md)). Line references drift after the next pipeline run; type and member names will not.

---

## 1. Executive summary

Three classes of gap, ordered by how much damage they do:

| Class | Meaning | Why it matters |
|---|---|---|
| **A — Missing type** | Does not exist. | *Compile-time* break. "Compile once" fails immediately. |
| **B — Present but throwing** | Exists, throws `NotImplementedException`. | *Runtime* failure — but loud and diagnosable. |
| **C — Present but silently wrong** | Exists, does something plausible but not WinForms-equivalent. | Worst: builds, runs, quietly misbehaves. |

**State of play.** **Class B is empty** — no `NotImplementedException` throw site remains in either project (§3). **Class A is closed too**: every remaining absent type is a declared non-goal (§2). Class C's last two open items were closed by patch 0111 — `Control.Region` (an inert auto-property that shaped nothing) and curved-region combination — leaving one accepted deviation, `Control.CreateGraphics()`, which is substrate-bound (§4.6). What is left is a set of deliberate deviations and non-goals (§4), a handful of substrate limits recorded alongside the features they bound, and the dominant *unmeasured* risk, which is behavioural: layout, event ordering, focus (§6).

---

## 2. Class A — types that still do not exist

Verified absent by type-declaration search over both projects. Everything in this class is now a declared
non-goal; there is no absent type or member left that is merely *not done yet*.

### 2.1 Non-goals (documented in §4.1 and §4.3)

`WebBrowser`, `HtmlDocument`/`HtmlElement`, `NativeWindow` and `SendKeys` do not exist and are not planned: the first three have no cross-platform substrate, the last two are Win32-message-model constructs. They are non-goals, not to-dos.

`WebBrowser`'s four `*EventArgs` types (`WebBrowserDocumentCompletedEventArgs`, `WebBrowserNavigatedEventArgs`, `WebBrowserNavigatingEventArgs`, `WebBrowserProgressChangedEventArgs`) are present while the control is not, so nothing can raise them. **Decision: keep them.** They exist in the reference assembly, so deleting them would open four new holes in the surface [API-COMPLETENESS.md](API-COMPLETENESS.md) measures, in exchange for nothing — an application that names one still compiles, which is the bar. They are inert, not wrong.

The ToolStrip family is complete: the container/panel/renderer types, the standard hosted items (`ToolStripComboBox`/`ToolStripTextBox`/`ToolStripProgressBar`), and `ToolStripControlHost` (patch 0108) all exist; `BindingNavigator` exists (patch 0107). `Help`, `HelpProvider`, `AmbientProperties`, `IWindowsFormsEditorService` and `AutoCompleteStringCollection` exist.

### 2.2 Closed since the last revision

- **`Control.Region`** was recorded here as absent. That was wrong: it existed as an inert auto-property, which is a Class C defect, not a Class A one — assigning a region compiled, ran and shaped nothing. It now shapes the widget (§4.7).
- **`Form.TransparencyKey`** genuinely was absent, along with `AllowTransparency`; both now exist (§4.7).

### 2.3 `Net4x.Gtk.Drawing`

No missing types remain.

**Correctly delegated, not a gap:** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are deliberately absent — `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the genuine BCL types. Preserve this.

---

## 3. Class B — closed

There is no `NotImplementedException` throw site left in `Net4x.Gtk.Windows.Forms` or `Net4x.Gtk.Drawing`. The last fifteen were closed by patch 0110:

- **`PropertyGrid`/`GridEntry` (11 sites)** — the property browser's editing machinery. `GridEntry` now honours the `ITypeDescriptorContext` contract it declares (`Container`, `Instance`, `GetService`, `OnComponentChanged`, `OnComponentChanging`, and a `PropertyDescriptor` resolved from the owning object rather than a permanently null auto-property), and the six `IComPropertyBrowser` members commit the open cell editor, close it, focus the grid, and load/save the view state.
- **`ArrangedElement.Handle`** — a layout participant is not a window; it reports `IntPtr.Zero`.
- **`DataGridViewCellCollection`'s `IList` indexer**, **`DataGridViewCellStyle`'s `ICloneable.Clone`**, and **`DataGridViewCellLinkedList.GetEnumerator`** — the last of which was a stub with no storage at all and is now a working list.

Three Class C defects in the same subsystem were found while testing those and fixed with them: `PropertyGrid.InPropertySet` returned a constant `true`; assigning `SelectedObject` (as opposed to `SelectedObjects`) never populated the grid and never raised `SelectedObjectsChanged`; and the view treated a *missing* `BrowsableAttribute` as "not browsable", so an ordinary object — one carrying no design-time attributes — displayed no properties whatsoever.

`Net4x.Gtk.Drawing`'s interop entry points (`Graphics.FromHdc`/`FromHwnd`/`GetHdc`/`GetHalftonePalette`, `Font.FromHdc`/`FromHfont`/`FromLogFont`, `Icon.FromHandle`) throw `PlatformNotSupportedException` by design — "never", not "not yet".

Stale `<exception cref="T:System.NotImplementedException">` doc tags still sit on members that no longer throw; they are documentation noise, not behaviour.

---

## 4. Structural gaps — design decisions and documented deviations

### 4.1 No Win32 message pump (non-goal)

`Message`/`WndProc` compile but `WndProc`/`DefWndProc` throw `PlatformNotSupportedException` naming GTK as the reason (covered by `MessagePumpNonGoalFunctionalTests`). `NativeWindow` and `SendKeys` remain absent. `IMessageFilter` with `Application.AddMessageFilter`/`RemoveMessageFilter` exists, but no internal pump invokes the filters — they run only when the application calls `Application.FilterMessage` itself, which is stated on the members. Any application reaching below the event model is unportable by construction — this is a non-goal, not a to-do.

Related and inherent: `Control.Handle` returns `Widget.Handle`, a GTK handle, not an `HWND`. Correct-by-necessity; it is stated so integrators do not discover it in production.

### 4.2 `Application` — closed

Present and tested: `Run(Form)`, `Run()`, `Run(ApplicationContext)` (all routed through one `ApplicationContext` path), `MessageLoop`, `ApplicationExit`, `ThreadExit`, `ThreadException`, `SetUnhandledExceptionMode`, `Exit`, `ExitThread`, `DoEvents`, `EnableVisualStyles`, `SetCompatibleTextRenderingDefault`, `SetHighDpiMode`, `OpenForms`, the path properties, and — since patch 0110 — `Idle`, `Restart`, `ProductName`/`ProductVersion`/`CompanyName`, `RenderWithVisualStyles` and `VisualStyleState`.

One deviation to record. GTK has no "the message queue just drained" notification, so `Idle` is *sampled*: while something is subscribed, a 50 ms GLib source watches the main context and raises the event once each time it finds the queue empty, re-arming as soon as events reappear. The once-per-lull contract matches WinForms; the latency (up to 50 ms) and the sampling cost do not exist in WinForms. Nothing is installed when nobody is subscribed.

### 4.3 Metafiles are declared, not implemented (non-goal)

`Metafile`, `MetafileHeader`, `MetaHeader`, `WmfPlaceableFileHeader`, `EmfPlusRecordType`, `PlayRecordCallback` exist as types, but EMF/WMF is a Windows record format with no cross-platform playback. A permanent non-goal.

### 4.4 `Region` fidelity — exact for curves, quantised to pixels

`Region` is backed by a band list, and `Region(GraphicsPath)` decomposes the path into rectangles. `SetClip` honours the full `CombineMode` family (Replace/Intersect/Union/Exclude/Xor/Complement), each pixel-verified in `GraphicsClipFunctionalTests`.

Curved boundaries no longer clip as their bounding box: a region built from a `GraphicsPath` keeps that path, and `Graphics.SetClip`/`Graphics.Clip` now clip through it via Cairo's native path clip, so an ellipse or a rounded rectangle clips on its true curve (`GraphicsRegionPathClipFunctionalTests` pins this on the corner pixel). The path-replay loop was extracted out of `DrawPathCore` for this, with text contributing glyph outlines instead of painted glyphs when the target is a clip.

Combining is exact too, as of patch 0111. A combine works on band lists, and a path used to contribute only its bounding rectangle to one — so `region.Intersect(circle)` intersected with the circle's *square*. The interior of a curve is now obtained the way GDI+ obtains it: the path is scan-converted (filled into a coverage bitmap, runs of covered pixels become bands, identical adjacent scanlines merge into one taller band). Pinned by `RegionCurvedCombineFunctionalTests`, whose oracle is a point inside the bounding box and outside the shape.

**Residual limits**, both recorded rather than open:

- **Pixel resolution.** Scan conversion quantises a curve to whole pixels, using half-coverage as the centre-in rule. GDI+ scan-converts too, so this is the same *kind* of approximation, but the two rasterizers will not agree on every edge pixel. A region combined and then queried with `IsVisible` can therefore differ from GDI+ by a pixel on the boundary.
- **Rectangles skip it.** A path made only of rectangles keeps its exact floating-point edges and never reaches the rasterizer — deliberately, since rounding those would be a regression. Pinned by `CombiningRectangles_KeepsExactFloatingPointEdges`.
- **Very large regions.** Past 16 megapixels the rasterizer is skipped and the bounding rectangle is used, as before. That is a pathological input, not a normal one, but it is a silent widening when it happens.

### 4.5 Printing model and `Image.RawFormat` — recorded deviations

The `System.Drawing.Printing` model types behave, with three deliberate deviations from real WinForms:

- **Enum numbering differs** — `PaperKind` (`Custom = 0`, not 256), `PaperSourceKind` (0-based, `Custom = 13`, not 257) and `PrinterResolutionKind` (`High = 1`…, not −4…) do not use the DEVMODE values. Code comparing `RawKind` against Win32 `DMPAPER_*`/`DMBIN_*` constants will misread them.
- **Virtual-device defaults** — with no print system, `PageSettings` defaults to Letter (850×1100), `PaperSourceKind.FormSource` and a 200×200 custom resolution, and `PrintableArea` is the whole page (no hard margins). Real WinForms reads these from the default printer.
- **`Image.RawFormat` on fresh bitmaps reports `Bmp`, not `MemoryBmp`** — the shim keeps an encoded buffer per image and `RawFormat` defaults to `Bmp` when the image has no source format. Stream-loaded images report their detected format correctly.

The print-controller family (`PrintController`/`StandardPrintController`/`PreviewPrintController`, `PrintControllerWithStatusDialog`) exists (patch 0105). `PrintDocument.Print()` runs through the GTK print dialog machinery and stays untested (needs a live print loop).

### 4.6 `Control.CreateGraphics()` is deferred, not immediate — accepted deviation

[Control.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs) allocates a fresh Cairo `ImageSurface`/`Context` per call and returns a `Graphics` over it; `Override_PaintGraphics` blits that surface during the next paint cycle. The comment near the top of `Control.cs` explains the constraint — drawing to the live GDK window during realize throws.

Two deviations from WinForms:

- **Deferred, not immediate.** In WinForms `CreateGraphics()` yields an immediate-mode DC: you draw, it appears now, outside any paint event. Here the effect waits for the next expose.
- **Single live surface.** Each call disposes the previous surface, so two `Graphics` objects on one control cannot coexist.

The pattern this breaks is the classic rubber-band: `CreateGraphics()` in `MouseMove` to XOR a selection rectangle. That code compiles and runs and does the wrong thing.

**Decision: accept and document.** GTK's retained paint cycle does not offer an immediate-mode DC on an unrealized widget, so a faithful fix would mean compositing such drawing through an overlay the paint cycle owns — a redesign of the paint path whose cost is out of proportion to the pattern's frequency, and which would still not give callers a second live surface. Applications that need immediate feedback should draw in `OnPaint` and call `Invalidate()`, which is the portable idiom. This item is closed as a deviation, not as a fix; reopen it if overlay compositing is undertaken for other reasons.

### 4.7 Shaped controls and colour-keyed transparency — what the substrate allows

`Control.Region` and `Form.TransparencyKey`/`AllowTransparency` work as of patch 0111. `Region` was an inert auto-property; it now drives GTK's shape-combine, is applied at handle creation for a region assigned before the control was realized, and clears back to the full rectangle when set to `null`. `TransparencyKey` did not exist at all, so the standard shaped-window idiom (`BackColor = X; TransparencyKey = X;`) did not compile.

Two substrate limits come with them:

- **A shape needs a GDK window.** Lightweight GTK widgets — buttons, labels, anything that draws onto its parent instead of owning a window — cannot be shaped. On those, `Region` is stored and reported but changes nothing; it is logged at debug level rather than throwing. Forms and container controls do own a window and are shaped for real, which covers the shaped-window and custom-drawn-container cases the property exists for.
- **`TransparencyKey` is not per-pixel.** What is emulated is the case the member exists for: a form whose `BackColor` *is* the key, so the window background is punched out (RGBA visual, background painted with zero alpha) and only the controls remain. A key matching a colour inside a child control or a background image leaves those pixels opaque, and keyed areas are not click-through — WinForms lets the mouse pass through them. Both would need an input shape recomputed from the rendered pixels on every paint. `Form.IsTransparencySupported` (no WinForms equivalent) reports whether the screen composites at all, so a form that stays opaque on an X11 session without a compositor is diagnosable rather than mysterious.

---

## 5. Priority order

Ranked by (damage × frequency) ÷ effort.

**P1 — measurement, not code.** Behavioural divergence — layout, event ordering, focus/tab order (§6) — is
now the largest unknown in the whole comparison by a wide margin, and the only class of gap where nobody
can say how big it is. Every remaining code item below is smaller than it.

1. Compare `TableLayoutPanel` / `FlowLayoutPanel` / anchor-dock against WinForms measure-arrange on
   concrete layouts. Start where the reimplementation is furthest from the original.
2. Pin event *order* for the common lifecycles (show, resize, click, focus change, close) — ordering is
   invisible to the API diff and to a reading pass alike.
3. Exercise the property browser past its contract: category grouping, sorting, expansion, editor
   selection (§3 closed its throw sites, not its behaviour).

**P2 — recorded deviations worth revisiting if they bite**

4. Per-pixel `TransparencyKey` and click-through on keyed areas (§4.7).
5. `Control.Region` on windowless widgets (§4.7) — would mean hosting them in a windowed GTK container.
6. Scan-conversion edge agreement with GDI+ (§4.4).

**P3 — accepted, reopen only with a reason**

7. `CreateGraphics` immediate-mode semantics (§4.6), EMF/WMF metafiles (§4.3), the Win32 message pump and
   what depends on it (§4.1), the printing enum numbering and virtual-device defaults (§4.5).

## 6. What this review does not cover

Stated so the gaps in the gap analysis are visible.

- **Behavioural divergence is the dominant unmeasured risk.** Layout drift, event ordering, and focus/tab-order differences are invisible to source reading. This limit was demonstrated, not asserted: several past fixes (unraised `HandleCreated`/`HandleDestroyed`, one-member `AccessibleRole`, `Padding.Horizontal` = `Right − Left`) were found only by *writing tests* — as were the three property-browser defects in §3, none of which a reading pass had flagged. Wrong-but-plausible code is invisible to a reading pass in a way that missing code is not.
- **Layout engines** — `TableLayoutPanel`, `FlowLayoutPanel`, anchor/dock — have not been compared against WinForms measure/arrange semantics. Fully reimplemented on GTK's allocate cycle; assume pixel-level and edge-case divergence until tested.
- **The property browser beyond its contract.** §3 closed the throw sites and the three defects the tests reached, but the grid's category grouping, sorting, expansion and value-editor selection are still only lightly exercised. Treat the subsystem as under-tested rather than verified.
- **Data-binding depth** is now covered (nested/dotted members, source-side `INotifyPropertyChanged`, `IEditableObject` transactions — see the binding functional tests), leaving no open depth question there.
- **`Net4x.Gtk.Windows.FormsDesigner`** and the ResX/designer round-trip are out of scope here.
- **Per-member API-surface diffing** is handled by [API-COMPLETENESS.md](API-COMPLETENESS.md) and the re-runnable tool in [api-diff/](api-diff/), which is strictly more complete than this document at the type/member level.

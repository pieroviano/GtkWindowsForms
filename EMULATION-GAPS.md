# Emulation gaps — `Net4x.Gtk.Windows.Forms` vs `System.Windows.Forms`, `Net4x.Gtk.Drawing` vs `System.Drawing`

**Bar being measured against.** The stated intent: *an existing WinForms application should compile unchanged against these two assemblies and behave the same on Windows, Linux and macOS.* Everything below is scored against that.

**This document lists only what is still open.** Earlier revisions carried a large record of already-closed findings (three threading defects, `SetStyle`/`GetStyle`, `Graphics.Transform`, `Region` banding, data-binding value flow, `Padding` arithmetic, the `IArrangedElement` contract, `Application.Run(ApplicationContext)`, accessibility wiring, the `VisualStyles` namespace, the ToolStrip container/panel/renderer family, `PrivateFontCollection`/`InstalledFontCollection`, `MeasureCharacterRanges`, and the full data-binding depth surface). Those sections were pruned once verified — the history is in git and the surviving evidence is the functional-test suite that pins each one. What remains here is the open frontier.

> **See also — [API-COMPLETENESS.md](API-COMPLETENESS.md).** The per-member API-surface diff run against the
> real .NET reference assemblies supersedes this document on *what exists* (type and member counts, renamed
> or re-signatured members). This document is authoritative on *behaviour* — members that exist and compile
> but do something other than what WinForms does, which a metadata diff cannot see.

**Method and its limits.** Findings come from reading the generated sources in `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` and from the functional-test suite. Static reading reliably finds missing types, throwing members and inert auto-properties; it does **not** find layout drift, event-ordering divergence, or focus/tab-order differences — those need tests. See §7.

**Caveat on line numbers.** Both directories are *generated* by the prepare pipeline from the `gtksystem-windows-forms` submodule (see [CLAUDE.md](CLAUDE.md)). Line references drift after the next pipeline run; type and member names will not.

---

## 1. Executive summary

Three classes of gap, ordered by how much damage they do:

| Class | Meaning | Why it matters |
|---|---|---|
| **A — Missing type** | Does not exist. | *Compile-time* break. "Compile once" fails immediately. |
| **B — Present but throwing** | Exists, throws `NotImplementedException`. | *Runtime* failure — but loud and diagnosable. |
| **C — Present but silently wrong** | Exists, does something plausible but not WinForms-equivalent. | Worst: builds, runs, quietly misbehaves. |

**State of play.** Class C is down to a single substrate-bound item (§2). Class A has collapsed to a short tail of genuinely peripheral controls plus one ToolStrip host type (§3). Class B is a PropertyGrid/GridEntry cluster and four isolated collection members (§4). The remaining structural work is `Application`'s residual lifecycle surface (§5.2) and a set of deliberate, documented deviations and non-goals (§5). The dominant *unmeasured* risk is now behavioural — layout, event ordering, focus (§7) — not missing surface.

---

## 2. Class C — present, still silently wrong

### 2.1 `Control.CreateGraphics()` returns a deferred off-screen surface

[Control.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs) allocates a fresh Cairo `ImageSurface`/`Context` per call and returns a `Graphics` over it; `Override_PaintGraphics` blits that surface during the next paint cycle. The comment near the top of `Control.cs` explains the constraint — drawing to the live GDK window during realize throws.

Two deviations from WinForms:

- **Deferred, not immediate.** In WinForms `CreateGraphics()` yields an immediate-mode DC: you draw, it appears now, outside any paint event. Here the effect waits for the next expose.
- **Single live surface.** Each call disposes the previous surface, so two `Graphics` objects on one control cannot coexist.

The pattern this breaks is the classic rubber-band: `CreateGraphics()` in `MouseMove` to XOR a selection rectangle. That code compiles and runs and does the wrong thing.

This one is *substrate-bound* — GTK's retained paint cycle genuinely does not offer an immediate-mode DC on an unrealized widget. A faithful fix means either accepting a documented deviation or routing such drawing through an overlay the paint cycle composites. It likely belongs in §5's non-goals rather than on the fix list; that is a design call still open.

---

## 3. Class A — types that still do not exist

Verified absent by type-declaration search over both projects.

### 3.1 ToolStrip — one host base class left

The container/panel/renderer family (`ToolStripManager`, `ToolStripRenderer`, `ToolStripProfessionalRenderer`, `ProfessionalColorTable`, `ToolStripContainer`, `ToolStripPanel`, `ToolStripContentPanel`) now exists, and so do the standard hosted items `ToolStripComboBox`/`ToolStripTextBox`/`ToolStripProgressBar` (implemented independently). The one remaining hole is **`ToolStripControlHost`** — the public base class for hosting an *arbitrary* `Control` inside a strip. Its absence does not break the standard items; it blocks user code that derives a custom hosted item from `ToolStripControlHost`, and any type reference to the class name in ported source.

### 3.2 Controls

`WebBrowser`, `HtmlDocument`/`HtmlElement` do not exist. (`BindingNavigator` now exists — patch 0107 — as a `ToolStrip` subclass wiring the standard navigation items to a `BindingSource`.)

`WebBrowser` deserves a note: its four `*EventArgs` types (`WebBrowserDocumentCompletedEventArgs`, `WebBrowserNavigatedEventArgs`, `WebBrowserNavigatingEventArgs`, `WebBrowserProgressChangedEventArgs`) are all present, but the control itself is not — so those types are currently unreachable. Either the control is coming or the event args are dead weight. `WebBrowser`/`HtmlDocument`/`HtmlElement` have no cross-platform substrate and are likely non-goals (§5.1).

### 3.3 Interop and misc

`NativeWindow` and `SendKeys` do not exist — both are Win32-message-model constructs (§5.1) and are candidate non-goals. Also absent: `Region` on `Control` (non-rectangular control shapes) and `TransparencyKey` on `Form`.

`Help`, `HelpProvider`, `AmbientProperties`, `IWindowsFormsEditorService` and `AutoCompleteStringCollection` now exist.

### 3.4 `Net4x.Gtk.Drawing`

No missing types remain. `PrivateFontCollection` and `InstalledFontCollection` now exist over the base `FontCollection`.

**Correctly delegated, not a gap:** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are deliberately absent — `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the genuine BCL types. Preserve this.

---

## 4. Class B — remaining `NotImplementedException` throw sites

Six files, 15 sites (counting `throw` statements, not stale doc tags).

| File | Sites |
|---|---|
| [PropertyGrid.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGrid.cs) | 6 |
| [GridEntry.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/PropertyGridInternal/PropertyGridBase/GridEntry.cs) | 5 |
| [DataGridViewCellStyle.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/DataGridViewBase/DataGridViewCellStyle.cs), [DataGridViewCellLinkedList.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/DataGridViewBase/DataGridViewCellLinkedList.cs), [DataGridViewCellCollection.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/DataGridViewBase/DataGridViewCellCollection.cs), [ArrangedElement.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Collection/ArrangedElement.cs) | 1 each |

The `PropertyGrid`/`GridEntry` cluster (11 of the 15) is the weakest remaining subsystem — the property browser's editing/expansion machinery. The four isolated sites are collection/layout members reached only on specific operations.

`Net4x.Gtk.Drawing` is clear of throw sites: the interop entry points (`Graphics.FromHdc`/`FromHwnd`/`GetHdc`/`GetHalftonePalette`, `Font.FromHdc`/`FromHfont`/`FromLogFont`, `Icon.FromHandle`) throw `PlatformNotSupportedException` by design — "never", not "not yet".

---

## 5. Structural gaps — design decisions and documented deviations

### 5.1 No Win32 message pump (non-goal)

`Message`/`WndProc` compile but `WndProc`/`DefWndProc` now throw `PlatformNotSupportedException` naming GTK as the reason (covered by `MessagePumpNonGoalFunctionalTests`). `NativeWindow`, `SendKeys`, `IMessageFilter`/`Application.AddMessageFilter` remain absent. Any application reaching below the event model is unportable by construction — this is a non-goal, not a to-do.

Related and inherent: `Control.Handle` returns `Widget.Handle`, a GTK handle, not an `HWND`. Correct-by-necessity; it is stated so integrators do not discover it in production.

### 5.2 `Application` — residual lifecycle surface

Present and tested: `Run(Form)`, `Run()`, `Run(ApplicationContext)` (all routed through one `ApplicationContext` path), `MessageLoop`, `ApplicationExit`, `ThreadExit`, `ThreadException`, `SetUnhandledExceptionMode`, `Exit`, `ExitThread`, `DoEvents`, `EnableVisualStyles`, `SetCompatibleTextRenderingDefault`, `SetHighDpiMode`, `OpenForms`, the path properties.

Still missing: `Idle`, `Restart`, `ProductName`/`ProductVersion`/`CompanyName`, `RenderWithVisualStyles`/`VisualStyleState`, and the `AddMessageFilter`/`RemoveMessageFilter` pair (see §5.1). The gap is now a tail of informational/rarely-critical members, not a missing startup entry point.

### 5.3 Metafiles are declared, not implemented (non-goal)

`Metafile`, `MetafileHeader`, `MetaHeader`, `WmfPlaceableFileHeader`, `EmfPlusRecordType`, `PlayRecordCallback` exist as types, but EMF/WMF is a Windows record format with no cross-platform playback. A permanent non-goal.

### 5.4 `Region` fidelity — bounded by representation

`Region` is backed by a band list, and `Region(GraphicsPath)` decomposes the path into rectangles. `SetClip` honours the full `CombineMode` family (Replace/Intersect/Union/Exclude/Xor/Complement), each pixel-verified in `GraphicsClipFunctionalTests`.

The residual limit is inherent to rectangle decomposition: **curved boundaries are approximated**. `Region(GraphicsPath)` keeps the real path, so routing `Graphics.SetClip(Region)` through that path rather than its bands would clip curves (ellipse, rounded-rect) exactly via Cairo's native path clip. Low frequency, and the clean fix requires extracting the path-replay loop out of `DrawPathCore` (which also handles glyph and nested-path side effects) — flagged, not yet done.

### 5.5 Printing model and `Image.RawFormat` — recorded deviations

The `System.Drawing.Printing` model types behave, with three deliberate deviations from real WinForms:

- **Enum numbering differs** — `PaperKind` (`Custom = 0`, not 256), `PaperSourceKind` (0-based, `Custom = 13`, not 257) and `PrinterResolutionKind` (`High = 1`…, not −4…) do not use the DEVMODE values. Code comparing `RawKind` against Win32 `DMPAPER_*`/`DMBIN_*` constants will misread them.
- **Virtual-device defaults** — with no print system, `PageSettings` defaults to Letter (850×1100), `PaperSourceKind.FormSource` and a 200×200 custom resolution, and `PrintableArea` is the whole page (no hard margins). Real WinForms reads these from the default printer.
- **`Image.RawFormat` on fresh bitmaps reports `Bmp`, not `MemoryBmp`** — the shim keeps an encoded buffer per image and `RawFormat` defaults to `Bmp` when the image has no source format. Stream-loaded images report their detected format correctly.

The print-controller family (`PrintController`/`StandardPrintController`/`PreviewPrintController`, `PrintControllerWithStatusDialog`) now exists (patch 0105). `PrintDocument.Print()` runs through the GTK print dialog machinery and stays untested (needs a live print loop).

---

## 6. Priority order

Ranked by (damage × frequency) ÷ effort.

**P1 — compile breaks on real source**
1. `ToolStripControlHost` (§3.1) — only custom hosted items reference it (the standard three already exist), so this is a compile break for a narrower slice of source than its position in the ToolStrip hierarchy suggests.

**P2 — remaining controls and collections**
3. `PropertyGrid`/`GridEntry` throw cluster (§4) — 11 sites in the property browser.
4. `WebBrowser`/`HtmlDocument`/`HtmlElement` (§3.2) — decide non-goal vs implement, and either build the control or delete the four orphaned event-args types.

**P3 — fidelity**
5. `Graphics.SetClip(Region)` through the region's path rather than its bands (§5.4).
6. `CreateGraphics` immediate-mode semantics — or document the deviation (§2.1).
7. `Application.Idle`, `Restart`, product/company metadata (§5.2).

**P4 — declare non-goals**
8. EMF/WMF metafiles, `NativeWindow`, `SendKeys`, `WebBrowser`/`Html*`, `Control.Handle` semantics — document rather than implement (§5.1, §5.3).

---

## 7. What this review does not cover

Stated so the gaps in the gap analysis are visible.

- **Behavioural divergence is the dominant unmeasured risk.** Layout drift, event ordering, and focus/tab-order differences are invisible to source reading. This limit was demonstrated, not asserted: several past fixes (unraised `HandleCreated`/`HandleDestroyed`, one-member `AccessibleRole`, `Padding.Horizontal` = `Right − Left`) were found only by *writing tests*. Wrong-but-plausible code is invisible to a reading pass in a way that missing code is not.
- **Layout engines** — `TableLayoutPanel`, `FlowLayoutPanel`, anchor/dock — have not been compared against WinForms measure/arrange semantics. Fully reimplemented on GTK's allocate cycle; assume pixel-level and edge-case divergence until tested.
- **Data-binding depth** is now covered (nested/dotted members, source-side `INotifyPropertyChanged`, `IEditableObject` transactions — see the binding functional tests), leaving no open depth question there.
- **`Net4x.Gtk.Windows.FormsDesigner`** and the ResX/designer round-trip are out of scope here.
- **Per-member API-surface diffing** is handled by [API-COMPLETENESS.md](API-COMPLETENESS.md) and the re-runnable tool in [api-diff/](api-diff/), which is strictly more complete than this document at the type/member level.

# API completeness — measured gap to `System.Windows.Forms` / `System.Drawing`

**Question this document answers.** *If an existing WinForms application is recompiled against
`Gtk.Windows.Forms` + `Net4x.Gtk.Drawing`, what does it fail to compile against?*

That is deliberately a narrower question than "does it behave the same". Behaviour is
[EMULATION-GAPS.md](EMULATION-GAPS.md)'s subject; this document is the mechanical API-surface diff,
ground truth from assembly metadata rather than recall. **It lists only what is still missing** — closed
gaps are dropped, not catalogued.

## Method

A `MetadataLoadContext` reflection pass compares the public + protected surface of the real reference
assemblies against the shipped ones. Protected members count (WinForms is an inheritance framework);
inherited members count as present (the whole base chain is walked); type-forwards are resolved
(`System.Windows.Forms.Primitives` merged in); signatures match on name + parameter type names, so a
member present under a *different* signature reads as missing — correctly, since that too is a compile
break.

| Side | Assemblies |
|---|---|
| Reference (net10) | `System.Windows.Forms.dll` + `.Primitives.dll` (WindowsDesktop.App.Ref **10.0.10**), `System.Drawing.Common.dll` (**10.0.6**) |
| Reference (net48 baseline) | net48 `System.Windows.Forms.dll` / `System.Drawing.dll` |
| Under test | `Net4x.Gtk.Windows.Forms.dll` + `Net4x.Gtk.Drawing.dll`, `bin/Release/netstandard2.0` |

Reproduce with the tool in [api-diff/tool/](api-diff/tool/) (outputs in [api-diff/](api-diff/); the
`wf48.*`/`dr48.*` pair is the net48 baseline, `wf.*`/`dr.*` the full net10 surface). Every count below is
checkable against those files.

---

## 1. Headline numbers

| | Forms (net48) | Forms (net10) | Drawing (net48) | Drawing (net10) |
|---|---|---|---|---|
| Public types in reference | 1034 | 1081 | 189 | 183 |
| Present | **924 (89.4 %)** | **928 (85.8 %)** | **141 (74.6 %)** | **138 (75.4 %)** |
| Missing types | 110 | 153 | 48 | 45 |
| Members on present types | 52 170 | 57 902 | 5 487 | 6 548 |
| …missing | **8 685 (16.6 %)** | **9 633 (16.6 %)** | **71 (1.3 %)** | **100 (1.5 %)** |
| Types member-incomplete | 164 | 168 | 25 | 15 |

`Net4x.Gtk.Drawing` is essentially done as an API surface (98.6 % of members present on net48).
`Net4x.Gtk.Windows.Forms` is ~89 % of the type surface but still ~17 % short on members — and, as §2 shows,
a large share of that member gap is one structural decision, not many pieces of work.

The net10 figures carry ~40 modern-.NET types (TaskDialog, dark-mode, `FormCornerPreference`, async
overloads) a .NET Framework-era application never uses; the net48 column is the truer target.

---

## 2. The structural finding: `ToolStripDropDown` is not a `Control`

**One base-class choice is the single biggest lever left in the whole diff.** `ToolStripDropDown :
ToolStripItem` in the shim, where real WinForms has `ToolStripDropDown : ToolStrip : ScrollableControl :
Control`. The three worst types in the entire diff are this one mistake:

| Type | Missing / total (net10) |
|---|---|
| `ToolStripDropDown` | 382 / 508 |
| `ContextMenuStrip` | 374 / 512 |
| `ToolStripDropDownMenu` | 374 / 510 |

Because a drop-down is not a `Control`, the entire `Control` surface — `BackColorChanged`, `DragDrop`,
`ControlAdded`, `ClientSizeChanged`, `FontChanged`, sizing, layout, the whole event set — is missing from
every drop-down and from `ContextMenuStrip`, which is what every right-click menu in every ported app
uses. Re-parenting onto `ToolStrip`/`Control` closes ~1 130 member gaps across these three types at once.

It is deferred, not cheap: `ToolStripDropDown` is backed by a native `Gtk.Menu` (a popup-menu widget, not
a container control), so re-parenting is menu-model surgery that risks the working right-click/submenu
system — one change, but a deep one, needing heavy menu testing.

---

## 3. Missing types, triaged by family

153 missing (net10) / 110 (net48), grouped so the number is actionable.

| Family | ~Count | Verdict |
|---|---|---|
| Legacy `DataGrid` (pre-2.0 `DataGrid`, `DataGridColumnStyle`, `GridTableStylesCollection`…) | 15 | Low priority — superseded by `DataGridView`, deprecated in WinForms itself |
| `DataGridView` completeness (`DataGridViewBand`/`DataGridViewElement`, editing controls, converters) | 8 | Real gaps in an otherwise-present control (§5) |
| `Html*` / `WebBrowser` | 17 | Needs a browser engine — **likely non-goal** |
| `TaskDialog` family | 16 | Modern (Core-era) Win32 dialog; mappable to GTK dialogs; absent from the net48 baseline |
| `AxHost` / COM / ActiveX (`AxHost`, `WindowsFormsSection`, `IComponentEditorPageSite`) | 3 | **Non-goal** — COM/ActiveX is Windows-only |
| Legacy `ToolBar`/`ToolBarButton` | 7 | Superseded by `ToolStrip`; low priority |
| ToolStrip residue (`ToolStripOverflow`, `ToolStripOverflowButton`, `ToolStripDropDownItemAccessibleObject`) | 3 | Real but narrow |
| Modern .NET (`FormCornerPreference`, `ScreenCaptureMode`, `SystemColorMode`, dark-mode, `IAsyncDropTarget`/`ITypedDataObject`/`DataObjectExtensions`/`DropImageType`) | ~9 | Never needed by a net4x app; absent from the net48 baseline |
| Misc (`MdiClient`, `BindableComponent`, `NativeWindow`, `SendKeys`, stray converters) | rest | Mixed — `MdiClient`/`BindableComponent` real; `NativeWindow`/`SendKeys` non-goals (§4) |

---

## 4. Core missing types that break ordinary application code

The subset a mainstream line-of-business app is most likely to hit:

- **`MdiClient`** — the container an MDI parent form exposes as `Form.MdiClient`; absent, so MDI child
  management by that surface is unavailable.
- **`BindableComponent`** — the base that carries `DataBindings`/`BindingContext` for non-control bindable
  components; absent in net10 (present in the net48 baseline).
- **`DataGridView` editing/support types** — the editing-control classes and their base: `DataGridViewBand`,
  `DataGridViewElement`, `DataGridViewComboBoxEditingControl`, `DataGridViewTextBoxEditingControl`,
  `IDataGridViewEditingControl`, `DataGridViewTopLeftHeaderCell` and a couple of converters. A custom
  `DataGridView` editing control cannot be written against the real names. (The nine enums are present.)
- **`ToolStripOverflow`/`ToolStripOverflowButton`** — the overflow chevron surface; a strip that overflows
  cannot expose it by name.

**Candidate non-goals** (document rather than implement): `WebBrowser`/`Html*` (browser engine),
`AxHost`/COM/ActiveX, `NativeWindow`/`SendKeys` (Win32 message model, EMULATION-GAPS.md §1), legacy
`DataGrid`/`ToolBar`, `TaskDialog` (or map to GTK dialogs), and the .NET 9+ dark-mode surface.

---

## 5. Member gaps on types that *do* exist

168 of 928 present types are member-incomplete (net10). Excluding the §2 hierarchy artefacts, the largest:

| Type | Missing / total (net10) |
|---|---|
| `PrintPreviewDialog` | 186 / 565 |
| `DataGridView` | 186 / 796 |
| `ToolStripTextBox` | 154 / 265 |
| `MenuStrip` | 143 / 489 |
| `Form` / `ThreadExceptionDialog` | 142 / 142 |
| `ToolStrip` / `BindingNavigator` | 139 / 139 |
| `StatusStrip` | 138 / 487 |
| `TreeView` | 136 / 477 |
| `ToolStripComboBox` | 135 / 252 |
| `RichTextBox` | 133 / 509 |
| `Control` | 68 / 409 |
| `Application` | 19 / 62 |

(`ThreadExceptionDialog`, `ToolStrip*`, `MenuStrip`, `BindingNavigator` all inherit from `ToolStrip`/
`Form`/`Control`; much of their gap is the same `Control`-surface tail, not per-type work.)

Numbers below are the **net48** baseline (the truer target), which runs a little lower than the net10 table.

### 5.1 `Form` — 114 missing (net48)

Absent include `RestoreBounds`, `MaximizedBounds`, `RightToLeftLayout` and a long tail of DPI/modern
members. The high-frequency designer-emitted surface (`AcceptButton`/`CancelButton`, the legacy events,
`CenterToScreen`, `OwnedForms`, `TransparencyKey`) is present, so the tail is lower-frequency than the
count suggests.

### 5.2 `Control` — 58 missing (net48), in the deep extensibility surface

The common override points (keyboard routing, `OnParentXxxChanged`, `RtlTranslate*`) are present. What
remains is the lowest-level override surface a custom control rarely needs, plus modern additions:

- **Sizing/scaling core** — `SetBoundsCore`, `SetClientSizeCore`, `ScaleControl`, `GetScaledBounds`,
  `SizeFromClientSize`. Standard override points for custom layout behaviour.
- **Handle lifecycle** — `CreateHandle`, `DestroyHandle`, `RecreateHandle` (entangled with the
  GTK-not-HWND `Handle` semantics, EMULATION-GAPS.md §1).
- **Modern additions** — `DataContext`/`DataContextChanged`/`OnParentDataContextChanged`, `InvokeAsync`
  (3 overloads). A .NET Framework-era app uses none of these.

### 5.3 `Application` — 12 missing (net48)

What is left presumes a Win32 pump or Windows host: `EnterThreadModal`/`LeaveThreadModal`,
`Register`/`UnregisterMessageLoop`, `RaiseIdle`, `OleRequired`, the registry properties
(`UserAppDataRegistry`/`CommonAppDataRegistry`), `AllowQuit`, `UseWaitCursor`, `SafeTopLevelCaptionFormat`
and `SetSuspendState`.

---

## 6. `Net4x.Gtk.Drawing` — near-complete

100 missing members across 6 548 (**98.5 % present**, net10). The 45 missing types:

| Group | Types | Verdict |
|---|---|---|
| `Imaging.Effects.*` | 25 | .NET 8+ GDI+ effects; niche, Windows-only implementation |
| `PrinterSettings` nested collections | `PaperSizeCollection`, `PaperSourceCollection`, `PrinterResolutionCollection`, `StringCollection` | Real but narrow — enumerating a printer's capabilities |
| Converters | `FontConverter` (+2 nested), `IconConverter`, `Design.CategoryNameCollection` | Design-time |
| Path | `GraphicsPathIterator`, `AdjustableArrowCap`, `PathPointType` | Real but narrow |
| Interop/misc | `Interop.LOGFONT`, `CachedBitmap`, `DitherType`, `PaletteType`, `StringUnit`, `StockIconId`/`StockIconOptions`, the two `BitmapSuffix*` attributes | Mostly Windows-bound |

Member gaps concentrate in `Graphics` (43 / 292), `GraphicsPath` (14 / 120, all modern `Span` overloads)
and `PrinterSettings` (12 / 44); everything else is single digits. A .NET Framework-era application uses
essentially none of the `GraphicsPath` misses.

---

## 7. Surface the shim adds that WinForms does not have

~262 public types exist in the shim's `System.Windows.Forms*` namespaces but not in real WinForms — GTK
plumbing (`GtkRender.CellRenderer*`, `IControl`, `IControlBase`, `IToolMenuItem`, `GtkMenuItem`) plus
additions (`DataGridViewRadioColumn`, `SwitchBox`). Full list:
[api-diff/wf.extra-types.txt](api-diff/wf.extra-types.txt).

One item worth deciding on: **`LayoutEngine` is declared as `System.Windows.Forms.LayoutEngine`**, where
WinForms has `System.Windows.Forms.Layout.LayoutEngine` — so it reads as both "missing" and "extra", and
`using System.Windows.Forms.Layout;` code breaks. Moving GTK plumbing to a `GtkSystem.*` namespace would
also remove a class of future name collisions.

---

## 8. Suggested order of work

Ranked by (breadth of impact ÷ effort), taking the structural finding into account.

**P0 — one change, disproportionate reach**
1. **Re-parent `ToolStripDropDown` onto `ToolStrip`/`Control`** (§2). ~1 130 member gaps across the three
   worst types, including `ContextMenuStrip` — but a deep, risky change against a native-`Gtk.Menu`
   backing, so it needs careful staging and heavy menu testing.

**P1 — compile breaks on idiomatic code**
2. `MdiClient`, `BindableComponent` (§4).
3. `DataGridView` editing-control cluster — `DataGridViewBand`/`DataGridViewElement`/the editing controls
   (§4/§5).
4. `Layout.LayoutEngine` namespace fix (§7).

**P2 — member completeness on existing types**
5. `Control`'s sizing/scaling core (§5.2).
6. `Form`'s remaining placement/RTL members (§5.1).
7. `PrinterSettings` capability collections (§6), unblocking parts of `PrintPreviewDialog`.

**P3 — declare non-goals in the package README**
`AxHost`/COM/ActiveX, `WebBrowser`/`Html*`, `TaskDialog` (or map to GTK), EMF/WMF metafiles,
`NativeWindow`, `SendKeys`, legacy `DataGrid`/`ToolBar`, `Imaging.Effects`, the .NET 9+ dark-mode surface,
and `Control.Handle`'s GTK-not-HWND semantics.

---

## 9. What this document does *not* measure

This diff and [EMULATION-GAPS.md](EMULATION-GAPS.md) are disjoint: this one measures *absent* members;
EMULATION-GAPS.md measures *behaviour* — members that exist and compile but do the wrong thing. Neither
subsumes the other. And type/member presence is a floor, not a ceiling:

- **Behaviour.** A member that exists may still be wrong (`Padding.Horizontal` once returned `Right - Left`
  — present and correctly signatured, invisible here).
- **Attributes** (`[DefaultValue]`, `[Browsable]`, `[DesignerSerializationVisibility]`) that drive designer
  round-tripping — a worthwhile second automated pass given how much of this repo is designer support.
- **Member accessibility beyond public/protected**, generic arity, `ref`/`out` beyond the type-name level,
  and explicit interface implementations.
- **Whether a missing member matters to *your* app.** Frequency-weighting against a corpus of real
  WinForms source would sharpen §8.
- **Reference-version sensitivity.** The net10 column carries recent additions a .NET Framework 4.x app
  never uses; the net48 column is the truer target and is reported alongside it throughout.

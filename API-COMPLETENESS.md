# API completeness — measured gap to `System.Windows.Forms` / `System.Drawing`

**Question this document answers.** *If an existing WinForms application is recompiled against
`Gtk.Windows.Forms` + `Net4x.Gtk.Drawing`, what does it fail to compile against?*

That is deliberately a narrower question than "does it behave the same". Behaviour is
[EMULATION-GAPS.md](EMULATION-GAPS.md)'s subject; this document is the mechanical API-surface diff,
ground truth from assembly metadata rather than recall. **It lists only what is still missing** — closed
gaps are dropped, not catalogued.

**State of the surface.** Every structural finding and every additive gap is closed. What remains missing
is **declared non-goals** (§2) plus a member tail spread across many present types (§3); no missing type
breaks ordinary line-of-business code. The net48 surface is at **90.6 % of types** with an **~15 % member
tail** that is mostly modern-.NET or rarely-hand-written.

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
| Present | **937 (90.6 %)** | **942 (87.1 %)** | **145 (76.7 %)** | **142 (77.6 %)** |
| Missing types | 97 | 139 | 44 | 41 |
| Members on present types | 54 529 | 60 356 | 5 543 | 6 604 |
| …missing | **7 760 (14.2 %)** | **8 756 (14.5 %)** | **68 (1.2 %)** | **97 (1.5 %)** |
| Types member-incomplete | 172 | 176 | 25 | 15 |

`Net4x.Gtk.Drawing` is essentially done as an API surface (98.6 % of members present on net48). Every
`Net4x.Gtk.Windows.Forms` type still missing on the net48 baseline is a declared non-goal (§2). The net10
column carries ~40 modern-.NET types (TaskDialog, dark-mode, `FormCornerPreference`, async overloads) a
.NET Framework-era application never uses; the net48 column is the truer target.

---

## 2. Missing types — all declared non-goals

97 missing (net48) / 139 (net10), and each is a deliberate omission, not a backlog item. They are
documented in the package README's "Platform notes and non-goals".

| Family | ~Count | Why it is a non-goal |
|---|---|---|
| Legacy `DataGrid` (pre-2.0 `DataGrid`, `DataGridColumnStyle`, `GridTableStylesCollection`, the two `IDataGrid*` interfaces…) | 15 | Superseded by `DataGridView`, deprecated in WinForms itself |
| `Html*` / `WebBrowser` | 17 | No cross-platform browser engine |
| `AxHost` / COM / ActiveX (`AxHost`, `WindowsFormsSection`, `IComponentEditorPageSite`) | 3 | COM/ActiveX is Windows-only |
| Legacy `ToolBar`/`ToolBarButton` | 7 | Superseded by `ToolStrip` |
| `NativeWindow`, `SendKeys` | 2 | Win32 message model (EMULATION-GAPS.md §1) |
| `TaskDialog` family | 16 | Modern (Core-era) Win32 dialog; mappable to GTK dialogs; absent from the net48 baseline |
| Modern .NET (`FormCornerPreference`, `ScreenCaptureMode`, `SystemColorMode`, dark-mode, `IAsyncDropTarget`/`ITypedDataObject`/`DataObjectExtensions`/`DropImageType`) | ~9 | Never needed by a net4x app; absent from the net48 baseline |

**One documented partial, not a missing type.** `DataGridViewBand`/`DataGridViewElement` exist and
`DataGridViewRow`/`DataGridViewCell` derive from them, but `DataGridViewColumn` keeps its GTK
`TreeViewColumn` base, so `column is DataGridViewBand` is `false`. Giving up the GTK column base would lose
the grid's native column behaviour.

---

## 3. Member gaps on types that *do* exist

176 of 942 present types are member-incomplete (net10). The member gap is now a broad tail rather than a
structural mistake. The largest:

| Type | Missing / total (net10) |
|---|---|
| `PrintPreviewDialog` | 177 / 565 |
| `DataGridView` | 177 / 796 |
| `ToolStripTextBox` / `ToolStripComboBox` | 154 / 135 |
| `TreeView` | 127 / 477 |
| `ToolStripDropDown` / `ContextMenuStrip` / `ToolStripDropDownMenu` | 126 / 123 / 123 |
| `RichTextBox` | 124 / 509 |
| `Form` / `ThreadExceptionDialog` | 122 / 122 |
| `CheckedListBox` | 123 / 494 |
| `MenuStrip` / `StatusStrip` / `ToolStrip` | 107 / 104 / 103 |
| `Control` | 59 / 409 |
| `Application` | 19 / 62 |

The `ToolStrip*`/`MenuStrip`/drop-down cluster all inherit from `ToolStrip`/`Control`; much of their
remaining gap is the same `ToolStrip`-specific member tail, not per-type work (the hosted
`ToolStripTextBox`/`ToolStripComboBox` derive from `ToolStripControlHost` instead, so they do not share it).
On the net48
baseline the frequently-used types run lower — `Form` 94, `Control` 49, `Application` 12 — and what remains
in each is the deep extensibility surface, modern-.NET additions, or Win32-host members a ported .NET
Framework application does not use:

- **`Form`** — the placement/designer surface is present; the tail is DPI/modern and rarely-hand-written.
- **`Control`** — the override points and bounds-core/scaling/handle surface are present; the tail is
  `DataContext`/`DataContextChanged`/`OnParentDataContextChanged` and `InvokeAsync` (modern).
- **`Application`** — the tail presumes a Win32 pump or Windows host (`EnterThreadModal`,
  `Register`/`UnregisterMessageLoop`, `RaiseIdle`, the registry properties, `SetSuspendState`, …).

---

## 4. `Net4x.Gtk.Drawing` — near-complete

97 missing members across 6 604 (**98.5 % present**, net10). The 41 missing types:

| Group | Types | Verdict |
|---|---|---|
| `Imaging.Effects.*` | 25 | .NET 8+ GDI+ effects; niche, Windows-only implementation |
| Converters | `FontConverter` (+2 nested), `IconConverter`, `Design.CategoryNameCollection` | Design-time |
| Path | `GraphicsPathIterator`, `AdjustableArrowCap`, `PathPointType` | Real but narrow |
| Interop/misc | `Interop.LOGFONT`, `CachedBitmap`, `DitherType`, `PaletteType`, `StringUnit`, `StockIconId`/`StockIconOptions`, the two `BitmapSuffix*` attributes | Mostly Windows-bound |

Member gaps concentrate in `Graphics` (43 / 292), `GraphicsPath` (14 / 120, all modern `Span` overloads)
and `PrinterSettings` (12 / 44); everything else is single digits. A .NET Framework-era application uses
essentially none of the `GraphicsPath` misses.

---

## 5. Surface the shim adds that WinForms does not have

~262 public types exist in the shim's `System.Windows.Forms*` namespaces but not in real WinForms — GTK
plumbing (`GtkRender.CellRenderer*`, `IControl`, `IControlBase`, `IToolMenuItem`, `GtkMenuItem`) plus
additions (`DataGridViewRadioColumn`, `SwitchBox`). Full list:
[api-diff/wf.extra-types.txt](api-diff/wf.extra-types.txt).

One open consideration: moving the GTK plumbing types out of the `System.Windows.Forms` namespace into a
`GtkSystem.*` one would remove a class of future name collisions (the `ButtonBase`/`TextBoxBase` wrapper
clash was one).

---

## 6. What this document does *not* measure

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
  WinForms source would sharpen the member-tail view in §3.
- **Reference-version sensitivity.** The net10 column carries recent additions a .NET Framework 4.x app
  never uses; the net48 column is the truer target and is reported alongside it throughout.

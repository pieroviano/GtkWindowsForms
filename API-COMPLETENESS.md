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
| Present | **934 (90.3 %)** | **939 (86.9 %)** | **145 (76.7 %)** | **142 (77.6 %)** |
| Missing types | 100 | 142 | 44 | 41 |
| Members on present types | 54 396 | 60 224 | 5 543 | 6 604 |
| …missing | **7 953 (14.6 %)** | **8 950 (14.9 %)** | **68 (1.2 %)** | **97 (1.5 %)** |
| Types member-incomplete | 171 | 175 | 25 | 15 |

**Every structural finding and additive gap is now closed** — including both base-class re-parents
(`ToolStripDropDown` onto `Control`, and `DataGridViewRow`/`Cell` onto the new
`DataGridViewBand`/`Element`). What remains missing is declared non-goals and a short tail of minor
design-time helpers (§2/§7).

`Net4x.Gtk.Drawing` is essentially done as an API surface (98.6 % of members present on net48).
`Net4x.Gtk.Windows.Forms` is ~90 % of the type surface and ~15 % short on members — the member gap is now a
long tail across many types rather than one structural mistake (the `ToolStripDropDown` re-parenting that
dominated earlier revisions is done, closing ~670 member gaps at once).

The net10 figures carry ~40 modern-.NET types (TaskDialog, dark-mode, `FormCornerPreference`, async
overloads) a .NET Framework-era application never uses; the net48 column is the truer target.

---

## 2. Missing types, triaged by family

146 missing (net10) / 104 (net48), grouped so the number is actionable.

| Family | ~Count | Verdict |
|---|---|---|
| Legacy `DataGrid` (pre-2.0 `DataGrid`, `DataGridColumnStyle`, `GridTableStylesCollection`…) | 15 | Low priority — superseded by `DataGridView`, deprecated in WinForms itself |
| `DataGridView` design-time helpers (`DataGridViewCellStyleConverter`, `DataGridViewColumnDesignTimeVisibleAttribute`) | 2 | Design-time only; the `Band`/`Element` bases landed |
| `Html*` / `WebBrowser` | 17 | Needs a browser engine — **likely non-goal** |
| `TaskDialog` family | 16 | Modern (Core-era) Win32 dialog; mappable to GTK dialogs; absent from the net48 baseline |
| `AxHost` / COM / ActiveX (`AxHost`, `WindowsFormsSection`, `IComponentEditorPageSite`) | 3 | **Non-goal** — COM/ActiveX is Windows-only |
| Legacy `ToolBar`/`ToolBarButton` | 7 | Superseded by `ToolStrip`; low priority |
| ToolStrip residue (`ToolStripDropDownItemAccessibleObject`) | 1 | Narrow accessibility helper; `ToolStripOverflow`/`ToolStripOverflowButton` landed |
| Modern .NET (`FormCornerPreference`, `ScreenCaptureMode`, `SystemColorMode`, dark-mode, `IAsyncDropTarget`/`ITypedDataObject`/`DataObjectExtensions`/`DropImageType`) | ~9 | Never needed by a net4x app; absent from the net48 baseline |
| Misc (`NativeWindow`, `SendKeys`, stray converters) | rest | `NativeWindow`/`SendKeys` are non-goals (§3) |

---

## 3. Core missing types that break ordinary application code

**None remain.** Every type a mainstream line-of-business app is likely to hit is present. What is still
missing is either a declared non-goal (below) or a minor design-time helper — `DataGridViewCellStyleConverter`,
`DataGridViewColumnDesignTimeVisibleAttribute`, `ToolStripDropDownItemAccessibleObject` — that does not
break ordinary application code.

One documented partial: `DataGridViewColumn` derives from a GTK `TreeViewColumn` and so, unlike `Row`/`Cell`,
does not derive from `DataGridViewBand`. `DataGridViewBand` and `DataGridViewElement` exist and `Row`/`Cell`
derive from them, but `column is DataGridViewBand` is `false`. The GTK column base cannot be given up
without losing the grid's native column behaviour.

**Candidate non-goals** (document rather than implement): `WebBrowser`/`Html*` (browser engine),
`AxHost`/COM/ActiveX, `NativeWindow`/`SendKeys` (Win32 message model, EMULATION-GAPS.md §1), legacy
`DataGrid`/`ToolBar`, `TaskDialog` (or map to GTK dialogs), and the .NET 9+ dark-mode surface. All are
documented in the package README's "Platform notes and non-goals".

---

## 4. Member gaps on types that *do* exist

175 of 939 present types are member-incomplete (net10). The largest:

| Type | Missing / total (net10) |
|---|---|
| `PrintPreviewDialog` | 177 / 565 |
| `DataGridView` | 177 / 796 |
| `ToolStripTextBox` | 154 / 265 |
| `ToolStripDropDown` / `ContextMenuStrip` / `ToolStripDropDownMenu` | 153 / 150 / 150 |
| `ToolStripComboBox` | 135 / 252 |
| `MenuStrip` / `ToolStrip` / `StatusStrip` | 134 / 130 / 129 |
| `BindingNavigator` | 130 / 504 |
| `TreeView` | 127 / 477 |
| `RichTextBox` | 124 / 509 |
| `Form` | 122 / 562 |
| `Control` | 59 / 409 |
| `Application` | 19 / 62 |

The `ToolStrip*`/`MenuStrip`/`BindingNavigator`/drop-down cluster all inherit from `ToolStrip`/`Control`;
much of their remaining gap is the same `ToolStrip`-specific member tail, not per-type work. Numbers below
are the **net48** baseline (the truer target), which runs a little lower than the net10 table.

### 4.1 `Form` — 94 missing (net48)

The high-frequency designer-emitted and placement surface is present; what remains is a long tail of
DPI/modern and rarely-hand-written members.

### 4.2 `Control` — 49 missing (net48), in the deep extensibility surface

The common override points and the bounds-core/scaling/handle-lifecycle surface are present; what remains
is mostly modern additions a .NET Framework-era app never uses:
`DataContext`/`DataContextChanged`/`OnParentDataContextChanged` and `InvokeAsync` (3 overloads).

### 4.3 `Application` — 12 missing (net48)

What is left presumes a Win32 pump or Windows host: `EnterThreadModal`/`LeaveThreadModal`,
`Register`/`UnregisterMessageLoop`, `RaiseIdle`, `OleRequired`, the registry properties
(`UserAppDataRegistry`/`CommonAppDataRegistry`), `AllowQuit`, `UseWaitCursor`, `SafeTopLevelCaptionFormat`
and `SetSuspendState`.

---

## 5. `Net4x.Gtk.Drawing` — near-complete

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

## 6. Surface the shim adds that WinForms does not have

~262 public types exist in the shim's `System.Windows.Forms*` namespaces but not in real WinForms — GTK
plumbing (`GtkRender.CellRenderer*`, `IControl`, `IControlBase`, `IToolMenuItem`, `GtkMenuItem`) plus
additions (`DataGridViewRadioColumn`, `SwitchBox`). Full list:
[api-diff/wf.extra-types.txt](api-diff/wf.extra-types.txt).

One open consideration: moving the GTK plumbing types out of the `System.Windows.Forms` namespace into a
`GtkSystem.*` one would remove a class of future name collisions (the `ButtonBase`/`TextBoxBase` wrapper
clash was one).

---

## 7. Remaining work

**The structural and additive order of work is complete.** Both base-class re-parents landed — the
`ToolStripDropDown → Control` lever (patch 0119, ~670 member gaps) with the `ToolStripOverflow` types it
unblocked (0120), and the `DataGridViewRow`/`Cell → DataGridViewBand`/`Element` hierarchy (0121). What is
left is not a to-do list so much as a set of standing decisions:

- **Declared non-goals** — `AxHost`/COM, `WebBrowser`/`Html*`, `TaskDialog`, EMF/WMF metafiles,
  `NativeWindow`, `SendKeys`, legacy `DataGrid`/`ToolBar`, `Imaging.Effects`, the .NET 9+ dark-mode surface,
  documented in the package README.
- **Minor design-time helpers** — `DataGridViewCellStyleConverter`, `DataGridViewColumnDesignTimeVisibleAttribute`,
  `ToolStripDropDownItemAccessibleObject`. Additive if wanted, but they do not break ordinary code.
- **One documented partial** — `DataGridViewColumn` keeps its GTK `TreeViewColumn` base and so does not
  derive from `DataGridViewBand` (§3).
- **Member tails** — the ~15 % member gap is now spread across many present types (`PrintPreviewDialog`,
  `DataGridView`, the `ToolStrip*` cluster, `RichTextBox`, …) rather than concentrated in one structural
  mistake; most of it is modern-.NET or rarely-hand-written surface.

Both re-parents are worth a note for future work: each looked like deep surgery from a reading pass, but the
empirical change was small and the full suite stayed green — `ToolStripDropDown` was two edits (it was never
used as a `ToolStripItem`; its `Gtk.Menu` backing was redirected through `GtkControl`), and the DataGridView
hierarchy was reconciled by having each leaf **override** the shared members it already implements, so the
row's computed `State` and the cell's computed `DataGridView` were preserved rather than rewritten. When a
re-parent's conflicts look structural, an override-based spike behind the test suite is cheaper than the
analysis.

---

## 8. What this document does *not* measure

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
  WinForms source would sharpen §7.
- **Reference-version sensitivity.** The net10 column carries recent additions a .NET Framework 4.x app
  never uses; the net48 column is the truer target and is reported alongside it throughout.

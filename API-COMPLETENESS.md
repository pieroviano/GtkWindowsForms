# API completeness — measured gap to `System.Windows.Forms` / `System.Drawing`

**Question this document answers.** *If an existing WinForms application is recompiled against
`Gtk.Windows.Forms` + `Net4x.Gtk.Drawing`, what does it fail to compile against?*

That is deliberately a narrower question than "does it behave the same". Behaviour is
[EMULATION-GAPS.md](EMULATION-GAPS.md)'s subject; this document is the mechanical API-surface diff that
[EMULATION-GAPS.md §8](EMULATION-GAPS.md) names as *"the highest-value follow-up and it can be automated."*
It replaces recall and source-reading with ground truth from assembly metadata.

## Method

A `MetadataLoadContext` reflection pass compares the public + protected surface of the real reference
assemblies against the shipped ones.

| Side | Assemblies |
|---|---|
| Reference | `System.Windows.Forms.dll` + `System.Windows.Forms.Primitives.dll` (WindowsDesktop.App.Ref **10.0.10**), `System.Drawing.Common.dll` (**10.0.6**) |
| Under test | `Net4x.Gtk.Windows.Forms.dll` + `Net4x.Gtk.Drawing.dll`, `bin/Release/netstandard2.0` |

Rules that make the numbers mean what they claim:

- **Protected members count.** WinForms is an inheritance framework; `protected override void OnPaint`
  is user-facing API.
- **Inherited members count as present.** The comparison walks the whole base chain, so a member the shim
  inherits rather than redeclares is *not* reported missing. (Constructors, which are not inherited, are
  matched per-type.) An earlier `DeclaredOnly` run inflated the member gap by roughly 3× — the figures
  below are the corrected ones.
- **Type-forwards are resolved.** `System.Windows.Forms.Primitives` is merged into the reference set, so
  forwarded types such as `HighDpiMode` and `Message` are not falsely reported missing.
- Signature matching is by name + parameter type names, so a member present under a *different signature*
  reads as missing — correctly, since that is also a compile break.

Reproduce (tool source in [api-diff/tool/](api-diff/tool/), outputs in [api-diff/](api-diff/)):

```sh
dotnet build api-diff/tool/ApiDiff.csproj -c Release
ApiDiff.exe <ref.dll> <impl.dll> <implDir> <outPrefix> <nsFilter> [extra impl dlls...]
```

Every count below is checkable against [api-diff/wf.missing-types.tsv](api-diff/wf.missing-types.tsv),
[api-diff/wf.missing-members.txt](api-diff/wf.missing-members.txt) and their `dr.*` counterparts.

---

## 1. Headline numbers

| | `System.Windows.Forms` | `System.Drawing` |
|---|---|---|
| Public types in reference | 1081 | 183 |
| Present | **526 (48.7 %)** | **131 (71.6 %)** |
| Missing | 555 | 52 |
| Members on the types that *do* exist | 43 073 | 6 446 |
| …missing | **9 930 (23.1 %)** | **101 (1.6 %)** |
| Types present but member-incomplete | 140 of 526 | 16 of 131 |
| Types present and member-complete | 386 | 115 |

**The two libraries are in very different states.** `Net4x.Gtk.Drawing` is essentially done as an API
surface: 98.4 % of members on the types it implements are present, and the 52 missing types are
concentrated in genuinely Windows-bound or exotic areas. `Net4x.Gtk.Windows.Forms` is roughly half the
type surface and roughly three-quarters of the member surface.

The headline "48.7 % of types" is the least useful number here, because 239 of the 555 missing types
(43 %) are the `VisualStyles` element catalogue — one design decision, not 239 pieces of work. §3 breaks
this down honestly.

---

## 2. The structural finding: missing intermediate base classes

This is the most consequential result, it is not in EMULATION-GAPS.md, and it explains a large share of
the member gap at once.

**WinForms' control hierarchy has intermediate abstract bases that carry most of a control family's API.
The shim mostly omits them and re-parents the leaf control directly onto `Control`.**

| Real WinForms | Net4x shim | Consequence |
|---|---|---|
| `Button`/`CheckBox`/`RadioButton` : **`ButtonBase`** : `Control` | all three : `Control` | `ButtonBase` does not exist |
| `TextBox`/`RichTextBox`/`MaskedTextBox` : **`TextBoxBase`** : `Control` | `TextBox : Control`, `RichTextBox : ScrollableControl` | `TextBoxBase` does not exist |
| `NumericUpDown`/`DomainUpDown` : **`UpDownBase`** : `ContainerControl` | `NumericUpDown : Control` | `UpDownBase` does not exist |
| `ToolStripDropDown` : `ToolStrip` : `ScrollableControl` : `Control` | **`ToolStripDropDown : ToolStripItem`** | a drop-down is not a `Control` at all |

Three distinct costs:

1. **Subclassing breaks.** `class MyButton : ButtonBase` — the documented way to write a custom button —
   does not compile.
2. **Type tests break.** `if (c is TextBoxBase tb) tb.SelectAll();` — the standard idiom for "any editable
   text control" — does not compile.
3. **The base's members vanish from every leaf.** This is the expensive one. Because `TextBoxBase` is
   absent and its members were not pushed down into `TextBox`, `TextBox` today has **no** `Clear()`,
   `AppendText()`, `Copy()`, `Cut()`, `Paste()`, `Undo()`, `ClearUndo()`, `ScrollToCaret()`, `Modified`,
   `CanUndo`, `SelectionLength`-adjacent editing surface, or the `ModifiedChanged` /
   `ReadOnlyChanged` / `MultilineChanged` / `TextAlignChanged` / `BorderStyleChanged` events. Verified
   directly: [TextBox.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/TextBox.cs) declares 20 public
   members, and `SelectAll()`/`Select(int,int)` are present while `Clear()` is not.

`TextBox.Clear()` is not a corner of the API. Its absence is a good illustration of why a type-level
review reported `TextBox` as "present".

**Naming hazard — real but narrow.** The names `ButtonBase` and `TextBoxBase` are already in use for the
internal GTK widget wrappers (`public sealed class ButtonBase : Gtk.Button` in
[GTKSystem/Windows/Forms/GtkControls/ControlBase/ButtonBase.cs](Net4x.Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/ButtonBase.cs);
`TextBox.self` is typed `TextBoxBase`). Those wrappers live in namespace
`GtkSystem.Windows.Forms.GtkControls.ControlBase`, **not** `System.Windows.Forms`, so this is not a
namespace clash. It bites in exactly two files —
[Button.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Button.cs) and
[TextBox.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/TextBox.cs) — which declare
`namespace System.Windows.Forms` while importing that GTK namespace, so `public readonly ButtonBase self`
would become ambiguous. A `using` alias in those two files resolves it. Not a design constraint.

### 2.1 Why `ToolStripDropDown` shows 390 missing members

The three worst types in the whole diff are one instance of this problem:

| Type | Missing / total |
|---|---|
| `ToolStripDropDown` | 390 / 508 |
| `ContextMenuStrip` | 382 / 512 |
| `ToolStripDropDownMenu` | 382 / 510 |

`ToolStripDropDown : ToolStripItem` instead of `: ToolStrip : ScrollableControl : Control`. So the
entire `Control` surface — `BackColorChanged`, `DragDrop`, `ControlAdded`, `ClientSizeChanged`,
`FontChanged`, sizing, layout, the whole event set — is missing from every drop-down and from
`ContextMenuStrip`, which is what every right-click menu in every ported app uses.

This is one re-parenting, not 390 members of work.

---

## 3. Missing types, triaged by family

555 missing, grouped so the number is actionable rather than alarming.

| Family | Count | Verdict |
|---|---|---|
| `VisualStyles` (`VisualStyleElement` + its ~190 nested element classes, renderers, enums) | **239** | Shim or non-goal — one decision |
| **Core WinForms** (see §4) | **229** | The real work |
| Legacy `DataGrid` (pre-2.0, superseded by `DataGridView`) | 32 | Low priority — deprecated in WinForms itself |
| `Html*` / `WebBrowser` | 17 | Needs a browser engine; likely non-goal |
| `TaskDialog` family | 17 | Modern Win32 dialog; mappable to GTK dialogs |
| `AxHost` / `Com2Interop` / ActiveX | 13 | **Non-goal** — COM/ActiveX is Windows-only by definition |
| Legacy `ToolBar` / `ToolBarButton` | 8 | Superseded by `ToolStrip`; low priority |

Of the 229 core types, **49 are the ToolStrip family** — making ToolStrip the single largest coherent
hole, exactly as EMULATION-GAPS.md §3.2 concluded.

---

## 4. Core missing types that break ordinary application code

The subset of §3's 229 that a mainstream line-of-business app is most likely to hit.

**Base classes** (§2) — `ButtonBase`, `TextBoxBase`, `UpDownBase`, `ScrollProperties`,
`HScrollProperties`, `VScrollProperties`, `Layout.LayoutEngine`, `BindableComponent`.

**Clipboard & drag-drop** — `DataObject` exists only as **`Clipboard.DataObject`** (nested inside the
static `Clipboard` class, [Clipboard.cs:70](Net4x.Gtk.Windows.Forms/System/Windows/Forms/OLE/Clipboard.cs)),
not as top-level `System.Windows.Forms.DataObject`. `new DataObject(...)` — the normal way to start a
drag — does not compile. Also missing: `IDropTarget`, `IAsyncDropTarget`, `ITypedDataObject`,
`DataObjectExtensions`, `DropImageType`.

**Enums that appear in ordinary designer output** — `CharacterCasing` (`TextBox.CharacterCasing`),
`ScrollBars` (`TextBox.ScrollBars` on every multiline textbox), `LinkBehavior`, `LinkArea`,
`TabAppearance`, `ItemBoundsPortion`, `FrameStyle`, `ArrowDirection`, `InsertKeyMode`,
`ColumnHeaderAutoResizeStyle`, `AutoCompleteSource` (+ `AutoCompleteStringCollection`).

**Owner-draw and hit-testing** — `DrawListViewItemEventArgs`, `DrawListViewSubItemEventArgs`,
`DrawListViewColumnHeaderEventArgs`, `DrawTreeNodeEventArgs` and their handlers; `ListViewHitTestInfo`,
`ListViewHitTestLocations`, `TreeViewHitTestInfo`, `TreeViewHitTestLocations`, `TreeViewDrawMode`,
`ListViewInsertionMark`. Any custom-drawn list or tree is unportable.

**RichTextBox support types** — `RichTextBoxStreamType`, `RichTextBoxScrollBars`, `RichTextBoxFinds`,
`RichTextBoxSelectionAttribute`, `RichTextBoxSelectionTypes`, `RichTextBoxLanguageOptions`,
`RichTextBoxWordPunctuations`. `LoadFile`/`SaveFile`/`Find` cannot be called with their real signatures;
`RichTextBox` itself is missing 192 of 509 members.

**Application infrastructure** — `IMessageFilter`, `NativeWindow`, `WindowsFormsSynchronizationContext`,
`ThreadExceptionDialog`, `AmbientProperties`, `SendKeys`, `Help`, `HelpProvider`, `OSFeature`/
`FeatureSupport`, `MdiClient`, `ListBindingHelper`, `BindingNavigator`, `FlatButtonAppearance`,
`FlowLayoutSettings`, `DomainUpDown`.

**ToolStrip** (49) — `ToolStripManager`, `ToolStripRenderer` + `ToolStripProfessionalRenderer` +
`ToolStripSystemRenderer` + `ProfessionalColorTable`/`ProfessionalColors`, `ToolStripContainer`,
`ToolStripPanel`, `ToolStripContentPanel`, `ToolStripControlHost`, `ToolStripOverflow`, and the
`ToolStrip*RenderEventArgs`/`*EventHandler` set. `ToolStripContainer` is designer-emitted, so this
breaks files developers do not hand-maintain.

**The eleven `*Renderer` classes** — `ButtonRenderer`, `CheckBoxRenderer`, `RadioButtonRenderer`,
`ComboBoxRenderer`, `TextBoxRenderer`, `GroupBoxRenderer`, `ProgressBarRenderer`, `ScrollBarRenderer`,
`TabRenderer`, `TrackBarRenderer` (+ `VisualStyles` proper). These are the supported way to draw a
themed control part in owner-draw code.

---

## 5. Member gaps on types that *do* exist

140 of the 526 present types are member-incomplete. Excluding the §2.1 hierarchy artefacts, the largest:

| Type | Missing / total |
|---|---|
| `PrintPreviewDialog` | 212 / 565 |
| `DataGridView` | 210 / 796 |
| `RichTextBox` | 192 / 509 |
| `Form` | 189 / 562 |
| `MenuStrip` / `ToolStrip` / `StatusStrip` | 167 / 163 / 162 |
| `TreeView` | 160 / 477 |
| `CheckedListBox` | 156 / 494 |
| `MaskedTextBox` | 152 / 485 |
| `NumericUpDown` | 150 / 477 |
| `ListBox` / `ListView` / `ComboBox` / `TextBox` | 142 / 140 / 133 / 133 |
| `Control` | 92 / 392 |
| `Application` | 29 / 54 |

### 5.1 `Form` — 189 missing, including things every dialog uses

Verified absent from [Form.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs):

- **`AcceptButton` and `CancelButton`.** Designer-emitted for essentially every dialog
  (`this.AcceptButton = this.btnOK;`). The only declarations of these names in the library are on
  `PrintPreviewDialog`. This is a compile break on generated code — the same class of problem as
  `ToolStripContainer`, and more common.
- **`Activated`, `Deactivate`, `Closing`, `Closed`, `MdiChildActivate`, `ResizeBegin`/`ResizeEnd`,
  `InputLanguageChanged`, `HelpButtonClicked`.** `FormClosing`/`FormClosed` *are* present, so the modern
  pair works and the legacy pair does not — a partial surface, which is worse than a uniformly missing
  one because it looks supported.
- `CenterToScreen()`, `CenterToParent()`, `AddOwnedForm()`/`RemoveOwnedForm()`/`OwnedForms`,
  `ParentForm`, `DesktopBounds`/`DesktopLocation`/`RestoreBounds`, `MaximizedBounds`,
  `TransparencyKey`, `SizeGripStyle`, `HelpButton`, `Modal`, `RightToLeftLayout`,
  `HorizontalScroll`/`VerticalScroll`, `Validate()`/`ValidateChildren()`.

### 5.2 `Control` — 92 missing, concentrated in the extensibility surface

The gap is not in the common properties (`Anchor`, `Dock`, `Text`, `Size` are all present) but in
**everything you override when you write a custom control**:

- **Keyboard routing — entirely absent.** `ProcessCmdKey`, `ProcessDialogKey`, `ProcessDialogChar`,
  `ProcessMnemonic`, `ProcessKeyPreview`, `ProcessKeyEventArgs`, `IsInputKey`, `IsInputChar`.
  `ProcessCmdKey` is *the* documented way to handle a shortcut key on a form; the name does not appear
  anywhere in the library. Note these take `ref Message`, so they are entangled with the
  message-pump non-goal (EMULATION-GAPS.md §5.1) — but shortcut handling itself is not Windows-bound and
  needs *some* supported route.
- **Sizing/scaling core** — `SetBoundsCore`, `SetClientSizeCore`, `SetVisibleCore`, `ScaleControl`,
  `ScaleCore`, `GetScaledBounds`, `RescaleConstantsForDpi`, `SizeFromClientSize`, `UpdateBounds`.
  These are the standard override points for custom layout behaviour.
- **Handle lifecycle** — `CreateHandle`, `DestroyHandle`, `RecreateHandle`, `UpdateStyles`,
  `UpdateZOrder`, `Control.FromHandle`/`FromChildHandle`.
- **Parent-change notifications** — the whole `OnParentXxxChanged` family (11 methods).
- **RTL helpers** — the six `RtlTranslate*` methods.
- `InitLayout`, `OnCreateControl`, `GetAutoSizeMode`/`SetAutoSizeMode`, `DefaultBackColor`/
  `DefaultForeColor`, `CheckForIllegalCrossThreadCalls`, `DataContext`.
- Modern async additions: `InvokeAsync` (3 overloads), `DoDragDropAsJson`.

### 5.3 `Application` — 29 missing

`Run`, `Run(ApplicationContext)`, `MessageLoop`, `ApplicationExit`, `ThreadExit` are **present** — this
diff independently confirms EMULATION-GAPS.md §6b's claim that those landed.

Still missing: `Idle`, `EnterThreadModal`/`LeaveThreadModal`, `AddMessageFilter`/`RemoveMessageFilter`/
`FilterMessage`, `Restart()`, `SetDefaultFont`, `ProductName`, `ProductVersion`, `CompanyName`,
`UseWaitCursor`, `AllowQuit`, `HighDpiMode`, `RenderWithVisualStyles`, `VisualStyleState`,
`UserAppDataRegistry`/`CommonAppDataRegistry`, and the .NET 9+ dark-mode surface
(`ColorMode`, `SystemColorMode`, `IsDarkModeEnabled`, `SetColorMode`).

Note `ThreadException`/`SetUnhandledExceptionMode` are **not** in the missing list — but
`ThreadExceptionDialog`, the default handler's UI, is (§4).

---

## 6. `Net4x.Gtk.Drawing` — near-complete

101 missing members across 6 446 (**98.4 % present**). The 52 missing types:

| Group | Types | Verdict |
|---|---|---|
| `Imaging.Effects.*` | 25 | .NET 8+ GDI+ effects; niche, Windows-only implementation |
| `Printing` controllers | `PrintController`, `StandardPrintController`, `PreviewPrintController`, `PreviewPageInfo`, `InvalidPrinterException`, the four `PrinterSettings` nested collections | **Real gap** — `PrintPreviewDialog` needs `PreviewPrintController` |
| Font | `PrivateFontCollection`, `InstalledFontCollection`, `FontConverter` (+2 nested) | **Real gap**, Pango/fontconfig-backable (EMULATION-GAPS.md §3.5) |
| Type converters | `IconConverter`, `Design.CategoryNameCollection` | Design-time |
| Path | `GraphicsPathIterator`, `AdjustableArrowCap`, `PathPointType` | Real but narrow |
| Interop/misc | `Interop.LOGFONT` (+nested), `StockIconId`, `StockIconOptions`, `CachedBitmap`, `DitherType`, `PaletteType`, `StringUnit`, the two `BitmapSuffix*` attributes | Mostly Windows-bound |

Member gaps are concentrated in `Graphics` (43 / 292), `GraphicsPath` (14 / 120) and
`PrinterSettings` (12 / 44); everything else is in single digits.

**These counts overstate the practical gap.** All 14 `GraphicsPath` misses are modern-.NET additions —
`Span`/`ReadOnlySpan` overloads of `AddCurve`/`AddLines`/`AddPolygon` etc., plus `AddRoundedRectangle`
and `GetPathPoints(Span<>)`. A .NET Framework-era application uses none of them. `PathTypes` itself is
present ([GraphicsPath.cs:195](Net4x.Gtk.Drawing/System/Drawing/Drawing2D/GraphicsPath.cs), typed
`byte[]?`); what's missing is only the `PathPointType` enum used to *interpret* those bytes by name.
See §10 on version sensitivity — this effect is much stronger on the Drawing side than the Forms side.

---

## 7. Surface the shim adds that WinForms does not have

255 public types exist in `System.Windows.Forms*` namespaces in the shim but not in real WinForms — GTK
plumbing (`GtkRender.CellRenderer*`, `IControl`, `IControlBase`, `IToolMenuItem`, `GtkMenuItem`,
`Column`), plus additions (`AutoClosingMessageBox`, `DataGridViewRadioColumn`, `SwitchBox`).

Two consequences worth deciding on deliberately:

1. **`LayoutEngine` is declared as `System.Windows.Forms.LayoutEngine`**, where WinForms has
   `System.Windows.Forms.Layout.LayoutEngine`. Right name, wrong namespace — so it reads as both
   "missing" and "extra". Any code with `using System.Windows.Forms.Layout;` breaks.
2. **Namespace pollution risk.** GTK implementation types in the `System.Windows.Forms` namespace can
   collide with real WinForms names under `using System.Windows.Forms;` — which is exactly what happened
   to `ButtonBase` and `TextBoxBase` (§2). Moving GTK plumbing to a `GtkSystem.*` namespace would remove
   a whole class of future collisions.

Full list: [api-diff/wf.extra-types.txt](api-diff/wf.extra-types.txt).

---

## 8. Corrections and confirmations to EMULATION-GAPS.md

**Confirmed by measurement:** `VisualStyles` absent (§3.1 — now quantified at 239 types); the ToolStrip
hole (§3.2 — 49 types); `DomainUpDown`, `BindingNavigator`, `WebBrowser`, `Html*` absent (§3.3);
`NativeWindow`, `SendKeys`, `Help`, `HelpProvider`, `AmbientProperties`, `AutoCompleteStringCollection`
absent (§3.4); `PrivateFontCollection`/`InstalledFontCollection` absent (§3.5); `Application.Run()`
overloads now present (§6b); `Color`/`Point`/`Rectangle` correctly delegated to
`System.Drawing.Primitives` (§3.5) — confirmed, they resolve from the BCL and are not reported missing.

**Not found by the reading pass, found here** — each verified in source:

| Gap | Why reading missed it |
|---|---|
| `TextBoxBase`/`ButtonBase` missing, taking `TextBox.Clear()`/`Copy()`/`Paste()`/`Undo()` etc. with them | §3.3 noted `UpDownBase` but generalised no further; `TextBox` "exists", so it looked fine |
| `Form.AcceptButton`/`CancelButton` absent | Absence of a designer-emitted property is invisible without a reference list |
| `ProcessCmdKey`/`ProcessDialogKey` and all keyboard routing absent | ditto |
| `ToolStripDropDown : ToolStripItem` — drop-downs are not `Control`s | Requires comparing hierarchies, not files |
| `DataObject` nested in `Clipboard` rather than top-level | The name is present in the codebase; only the *shape* is wrong |
| `Form.Activated`/`Deactivate`/`Closing`/`Closed` absent while `FormClosing`/`FormClosed` present | Partial surfaces read as complete |
| `ScrollBars`, `CharacterCasing` and ~60 other enums absent | Enums are easy to overlook |

**Refined, not contradicted:** EMULATION-GAPS.md §4's `NotImplementedException` counts measure *throwing*
members. This diff measures *absent* ones. They are disjoint problems — a throwing member is present in
metadata and counts as "present" here. Both lists matter; neither subsumes the other.

---

> **A costed, sequenced plan built on this analysis is in
> [GAP-CLOSURE-PLAN.md](GAP-CLOSURE-PLAN.md)**, targeting the net48 baseline
> (`api-diff/wf48.*`: 512 missing types, 9 130 missing members) rather than the .NET 10 figures below.

## 9. Suggested order of work

Ranked by (breadth of impact ÷ effort), taking the structural finding into account.

**P0 — cheap fixes with disproportionate reach**
1. **Re-parent `ToolStripDropDown` onto `ToolStrip`/`Control`** (§2.1). One change; ~1 150 member gaps
   across three of the four worst types in the diff, including `ContextMenuStrip`.
2. **Introduce `TextBoxBase` and `ButtonBase`** (§2); the GTK-wrapper name clash is a `using` alias in two
   files. Restores `TextBox.Clear()`/`Copy()`/`Cut()`/`Paste()`/`Undo()` and makes `is TextBoxBase` work.
3. **`Form.AcceptButton`/`CancelButton`** (§5.1) — designer-emitted, trivially implementable over GTK
   default-widget handling.

**P1 — compile breaks on generated or idiomatic code**
4. `ToolStripContainer`/`ToolStripPanel`/`ToolStripContentPanel` (designer-emitted).
5. The missing enums of §4 — nearly free, and each unblocks a property that already exists.
6. Top-level `DataObject`; `IDropTarget`.
7. `Form`'s legacy event pair and the window-placement members (§5.1).
8. `UpDownBase`/`DomainUpDown`; `Layout.LayoutEngine` namespace fix.

**P2 — extensibility surface**
9. `Control`'s keyboard routing (§5.2) — needs a design decision on the `ref Message` signatures given
   the message-pump non-goal. Shortcut handling must have *some* supported route.
10. `Control`'s sizing/scaling core and `OnParentXxxChanged` family.
11. Owner-draw event args + hit-test info for `ListView`/`TreeView` (§4).
12. RichTextBox support enums, unblocking `LoadFile`/`SaveFile`/`Find`.

**P3 — theming, printing, fonts**
13. `ToolStripRenderer`/`ProfessionalColorTable`/`ToolStripManager`.
14. The eleven `*Renderer` classes + a `VisualStyles` shim over GTK theming — **decide shim-vs-non-goal
    first**; as compiling shims they cost little and unblock builds.
15. `PreviewPrintController`/`PrintController` (needed by `PrintPreviewDialog`, itself 212 members short).
16. `PrivateFontCollection`/`InstalledFontCollection` over Pango/fontconfig.

**P4 — declare non-goals in the package README**
`AxHost`/COM/ActiveX, `WebBrowser`/`Html*`, EMF/WMF metafiles, `NativeWindow`, `SendKeys`, legacy
`DataGrid`/`ToolBar`, `Imaging.Effects`, and `Control.Handle`'s GTK-not-HWND semantics.

---

## 10. What this document does *not* measure

- **Behaviour.** A member that exists may still be wrong. `Padding.Horizontal` returning `Right - Left`
  ([EMULATION-GAPS.md §6b](EMULATION-GAPS.md)) is invisible to this method — it is present and correctly
  signatured. **Type-level and member-level presence is a floor, not a ceiling.**
- **Attributes.** `[DesignerSerializationVisibility]`, `[DefaultValue]`, `[Browsable]` and friends drive
  designer round-tripping and are not compared. Given how much of this repo is designer support, this is
  a worthwhile second automated pass.
- **Accessibility of members** beyond public/protected, generic arity mismatches, and `ref`/`out`
  differences beyond the type-name level.
- **Explicit interface implementations**, which can satisfy an interface without appearing by name.
- **Whether missing members matter to *your* app.** Frequency-weighting this list against a corpus of
  real WinForms source would sharpen the priorities in §9 considerably.
- Reference version sensitivity: measured against **.NET 10** reference assemblies. Some gaps
  (`InvokeAsync`, `DoDragDropAsJson`, dark mode, `FormCornerPreference`) are recent additions that a
  .NET Framework 4.x-era application will never use. Re-running against
  `Microsoft.WindowsDesktop.App.Ref/6.0.x` would separate "missing" from "missing but never needed".

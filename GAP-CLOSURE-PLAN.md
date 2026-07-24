# Gap closure plan — reaching WinForms parity

**Goal.** An existing .NET Framework 4.x WinForms application recompiles unchanged against
`Gtk.Windows.Forms` + `Net4x.Gtk.Drawing` and behaves the same on Windows, Linux and macOS.

**Inputs.** [API-COMPLETENESS.md](API-COMPLETENESS.md) (what is *absent*, measured) and
[EMULATION-GAPS.md](EMULATION-GAPS.md) (what is *present but wrong*, reviewed). This plan sequences both.

## Decisions taken

| Decision | Choice | Consequence |
|---|---|---|
| **Baseline** | **net48** reference assemblies, not .NET 10 | Target is 512 missing types / 9 130 missing members, not 555 / 9 930. Excludes `InvokeAsync`, `DoDragDropAsJson`, dark mode, `FormCornerPreference`, `Span<>` overloads — no app being ported uses them. |
| **VisualStyles + the 11 `*Renderer` classes** | **Compiling shims over GTK theming** | Ported code builds; rendering defers to the GTK theme rather than emulating Windows chrome. A documented visual deviation, not a compile break. |
| **Scope** | API gaps **and** open behavioural items | API first, because behavioural tests need the API to exist to assert against. |
| **Verification** | Behavioural `*FunctionalTests` **+** a ratcheting api-diff gate in CI | Prevents both silent wrongness and surface regression. |

**Measured net48 baseline** (`api-diff/wf48.*`, `api-diff/dr48.*`):

| | Types present | Members present |
|---|---|---|
| `System.Windows.Forms` | 522 / 1034 (50.5 %) | 30 268 / 39 398 (76.8 %) |
| `System.Drawing` | 134 / 189 (70.9 %) | 5 313 / 5 385 (98.7 %) |

---

## How changes land in this repo — read before starting

`Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` are **generated** by the prepare pipeline
([CLAUDE.md](CLAUDE.md)). Hand edits are lost on the next `PrepareProjects` run. The delivery mechanism
for this plan is **patches**, confirmed by how the last comparable work landed — `0087` added 8 new
files, `0088` added 15.

Per unit of work:

```sh
# 1. edit / add files under Net4x.Gtk.Windows.Forms/ or Net4x.Gtk.Drawing/
# 2. add behavioural tests under the relevant *.Tests project
dotnet build Net4x.Gtk.Windows.Forms.sln && dotnet test Net4x.Gtk.Windows.Forms.sln
git commit -am "NNNN description"
git format-patch -o patches/ -1 HEAD -- :(exclude)*.csproj     # Patches.txt
```

Mechanics that matter here:

- **New files need no `.csproj` registration** — the projects are SDK-style and glob `**/*.cs`. This is
  load-bearing, since `format-patch` deliberately excludes `*.csproj`.
- **Patch numbering continues from `0090`.** This plan's phases are numbered `0091+` in order; patches
  are replayed sequentially by `GitPatchApplier.CommitAndApplyPatches()`, so a later patch must not
  conflict with an earlier one. Keep each phase's patch touching a disjoint file set where possible.
- **`Nullable` is `enable` and `GenerateDocumentationFile` is `true`.** Every new public member needs XML
  documentation and correct nullability, or the build warns/fails.
- Tests follow the existing `*FunctionalTests.cs` convention (48 such files today, ~3 400 tests).
- Upstream-logic changes belong in the `gtksystem-windows-forms` submodule instead; nothing in this plan
  requires that.

---

## Phase 0 — instrument first (do before any code)

Without this, progress is unmeasurable and regressions are silent.

1. **Move `api-diff` into CI.** Re-run the tool on every build; fail if missing-type or missing-member
   counts *increase* against a checked-in baseline. Ratchet the baseline down as phases land.
   Tool and instructions: [api-diff/README.md](api-diff/README.md).
2. **Add a compile-corpus project.** A handful of representative WinForms forms — including
   designer-generated `.Designer.cs` with `AcceptButton`, `ToolStripContainer`, `TextBox.ScrollBars` —
   that must *compile* against the shim. This catches the designer-emitted breaks (§Phase 1, §Phase 3)
   directly rather than by inference. It should fail today; that is the point.
3. **Record the baseline numbers** above in the CI gate so the ratchet has a starting point.

**Exit:** CI reports the gap on every build and cannot silently widen.

---

## Phase 1 — structural fixes (highest leverage in the plan)

Three changes. Together they close roughly **1 500 of the 9 130 missing members (≈16 %)** without
implementing 1 500 members, because they fix inheritance rather than add API.

### 1.1 Re-parent `ToolStripDropDown` → `ToolStrip` → `ScrollableControl` → `Control`

Today `ToolStripDropDown : ToolStripItem`, so a drop-down is not a `Control`.

| Type | Missing / total (net48) |
|---|---|
| `ToolStripDropDown` | 376 / 494 |
| `ContextMenuStrip` | 368 / 498 |
| `ToolStripDropDownMenu` | 368 / 496 |

**≈1 112 members recovered by one re-parenting.** `ContextMenuStrip` is every right-click menu in every
ported app, so this is also the highest-frequency fix available.

Risk: the largest single behavioural change in the plan — a drop-down becomes a `Control` with a
layout/paint cycle it did not previously have. Expect popup positioning and sizing fallout, and budget
for it. Do this first precisely *because* it is disruptive; everything downstream builds on the corrected
hierarchy.

### 1.2 Introduce `TextBoxBase` and `ButtonBase`

Insert the real WinForms bases and move the shared surface onto them:

- `TextBoxBase : Control` ← `TextBox`, `RichTextBox`, `MaskedTextBox`
- `ButtonBase : Control` ← `Button`, `CheckBox`, `RadioButton`

This restores `TextBox.Clear()`, `AppendText()`, `Copy()`, `Cut()`, `Paste()`, `Undo()`, `ClearUndo()`,
`ScrollToCaret()`, `Modified`, `CanUndo` and the `ModifiedChanged`/`ReadOnlyChanged`/`MultilineChanged`/
`TextAlignChanged`/`BorderStyleChanged` events — none of which exist today — and makes
`class MyButton : ButtonBase` and `if (c is TextBoxBase tb)` compile.

Affected: `TextBox` 122, `RichTextBox` 181, `MaskedTextBox` 141, `Button` 91, `CheckBox` 93,
`RadioButton` 91 missing members.

**Name clash is trivial**, contrary to the first reading: the GTK widget wrappers of the same name live
in `GtkSystem.Windows.Forms.GtkControls.ControlBase`, a different namespace. Only
[Button.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Button.cs) and
[TextBox.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/TextBox.cs) import both; a `using` alias
(`using GtkButtonBase = GtkSystem.Windows.Forms.GtkControls.ControlBase.ButtonBase;`) resolves it.

### 1.3 `Form.AcceptButton` / `CancelButton`

Designer-emitted for essentially every dialog, so their absence breaks files developers do not
hand-maintain. Map onto GTK default-widget handling (`Gtk.Widget.HasDefault` / `Window.Default`) plus
Escape-key handling for `CancelButton`.

**Tests:** `ControlHierarchyFunctionalTests` (base-class identity, `is`-checks, member reachability),
`TextBoxBaseEditingFunctionalTests` (`Clear`/`Copy`/`Cut`/`Paste`/`Undo` against real clipboard and
undo-stack oracles, not just non-throwing), `ContextMenuStripFunctionalTests` (popup as a `Control`:
sizing, `ControlAdded`, paint), `FormDefaultButtonFunctionalTests` (Enter activates `AcceptButton`,
Escape activates `CancelButton`).

**Exit:** the three hierarchy types report <60 missing members; `TextBox.Clear()` works; a dialog with
`AcceptButton` compiles and responds to Enter.

---

## Phase 2 — free wins: enums, event args, support types

Almost all mechanical, each one unblocking a property or method that *already exists*.

1. **~60 missing enums** (`api-diff/wf48.missing-types.tsv`, kind `enum`). Highest value:
   `CharacterCasing`, `ScrollBars` (every multiline `TextBox`), `LinkBehavior`, `TabAppearance`,
   `FrameStyle`, `ArrowDirection`, `ItemBoundsPortion`, `InsertKeyMode`,
   `ColumnHeaderAutoResizeStyle`, `AutoCompleteSource`, `MenuMerge`, `ToolStripDropDownCloseReason`.
   Use the real underlying values — `Shortcut` alone is 123 missing members, and its values are
   compared against by real code.
2. **Owner-draw event args + handlers** — `DrawListViewItemEventArgs`, `DrawListViewSubItemEventArgs`,
   `DrawListViewColumnHeaderEventArgs`, `DrawTreeNodeEventArgs` and their delegates; wire them to the
   existing `OwnerDraw` paths.
3. **Hit-testing** — `ListViewHitTestInfo`, `ListViewHitTestLocations`, `TreeViewHitTestInfo`,
   `TreeViewHitTestLocations`, `TreeViewDrawMode`, `ListViewInsertionMark`.
4. **RichTextBox support enums** — `RichTextBoxStreamType`, `RichTextBoxScrollBars`, `RichTextBoxFinds`,
   `RichTextBoxSelectionAttribute`, `RichTextBoxSelectionTypes`, `RichTextBoxLanguageOptions`,
   `RichTextBoxWordPunctuations`. Unblocks `LoadFile`/`SaveFile`/`Find` with real signatures.
5. **Top-level `DataObject`.** Today it exists only as `Clipboard.DataObject`, nested inside the static
   `Clipboard` class, so `new DataObject(...)` does not compile. Promote it to
   `System.Windows.Forms.DataObject`, leaving a nested alias if internal callers depend on the old shape.
   Add `IDropTarget`.
6. `FlatButtonAppearance`, `LinkArea`, `AutoCompleteStringCollection`, `NumericUpDownAcceleration(Collection)`,
   `OwnerDrawPropertyBag`.

**Note the enum trap:** [EMULATION-GAPS.md §5.7](EMULATION-GAPS.md) records that `PaperKind`,
`PaperSourceKind` and `PrinterResolutionKind` already deviate from the DEVMODE numbering. Do not repeat
that here — enum *values*, not just names, are API.

**Tests:** per-group functional tests asserting values against the documented WinForms constants, and a
round-trip test that a designer-emitted property assignment (`textBox.ScrollBars = ScrollBars.Vertical`)
both compiles and takes effect.

**Exit:** no missing types of kind `enum` or `*EventArgs`/`*EventHandler` in the core set.

---

## Phase 3 — ToolStrip subsystem

46 missing types, ~2 707 missing members across the family — **30 % of the whole member gap**, and the
weakest subsystem per both analyses. Phase 1.1 must land first.

1. **`ToolStripContainer`, `ToolStripPanel`, `ToolStripContentPanel`, `ToolStripPanelRow`** —
   designer-emitted for any multi-toolbar form, so these are compile breaks on generated code.
2. **`ToolStripManager`** — merging, `RevertMerge`, renderer selection.
3. **`ToolStripRenderer`** + `ToolStripProfessionalRenderer` + `ToolStripSystemRenderer` +
   `ProfessionalColorTable`/`ProfessionalColors`, and the `ToolStrip*RenderEventArgs`/`*EventHandler`
   set. Render through the GTK theme (per the VisualStyles decision) rather than emulating Office chrome.
4. **`ToolStripControlHost`**, `ToolStripOverflow`, `ToolStripOverflowButton`.
5. Close the remaining `ToolStripItem` gaps and the `MenuStrip`/`StatusStrip`/`ToolStrip` member sets
   (152–156 each).

**Tests:** `ToolStripContainerFunctionalTests` (dock/raft placement), `ToolStripMergeFunctionalTests`
(merge/revert produces the documented item ordering), `ToolStripRendererFunctionalTests` (renderer
override is actually invoked for each part).

**Exit:** a two-toolbar designer form compiles and renders; ToolStrip family <300 missing members total.

---

## Phase 4 — `Control` and `Form` extensibility surface

The gap is not in common properties — `Anchor`, `Dock`, `Text`, `Size` are all present — but in what you
override to write a custom control. `Control` 82 missing, `Form` 161.

### 4.1 Keyboard routing — needs a design decision first

`ProcessCmdKey`, `ProcessDialogKey`, `ProcessDialogChar`, `ProcessMnemonic`, `ProcessKeyPreview`,
`ProcessKeyEventArgs`, `IsInputKey`, `IsInputChar` are **entirely absent** — `ProcessCmdKey` does not
appear anywhere in the library, and it is *the* documented way to handle a shortcut key.

The tension: their real signatures take `ref Message`, and the message pump is a declared non-goal
([EMULATION-GAPS.md §5.1](EMULATION-GAPS.md)). **Recommendation:** implement them with the real
signatures, synthesising a `Message` from the GTK key event (populating `Msg`/`WParam`/`LParam` with
`WM_KEYDOWN`-equivalent values) and routing GTK key events through the real WinForms chain
(`ProcessCmdKey` → `ProcessDialogKey` → `OnKeyDown`). This keeps overriding code source-compatible and
functional without a general message pump. Decide explicitly and record it — a `PlatformNotSupported`
throw here would break a very common pattern.

### 4.2 Sizing / scaling / layout core

`SetBoundsCore`, `SetClientSizeCore`, `SetVisibleCore`, `ScaleControl`, `ScaleCore`, `GetScaledBounds`,
`RescaleConstantsForDpi`, `SizeFromClientSize`, `UpdateBounds`, `InitLayout`, `GetAutoSizeMode`/
`SetAutoSizeMode`. These are the documented override points for custom layout behaviour.

### 4.3 Remaining `Control` surface

Handle lifecycle (`CreateHandle`, `DestroyHandle`, `RecreateHandle`, `UpdateStyles`, `UpdateZOrder`,
`Control.FromHandle`/`FromChildHandle`), the 11 `OnParentXxxChanged` notifications, the six
`RtlTranslate*` helpers, `DefaultBackColor`/`DefaultForeColor`, `CheckForIllegalCrossThreadCalls`,
`OnCreateControl`.

### 4.4 `Form`

- **Legacy event pair** `Closing`/`Closed` alongside the existing `FormClosing`/`FormClosed`, plus
  `Activated`, `Deactivate`, `MdiChildActivate`, `ResizeBegin`/`ResizeEnd`, `HelpButtonClicked`.
  A *partial* surface is worse than a missing one — it looks supported.
- Placement: `CenterToScreen()`, `CenterToParent()`, `DesktopBounds`, `DesktopLocation`, `RestoreBounds`,
  `MaximizedBounds`.
- Ownership: `AddOwnedForm`/`RemoveOwnedForm`/`OwnedForms`, `ParentForm`.
- `Validate()`/`ValidateChildren()`, `SizeGripStyle`, `HelpButton`, `Modal`, `RightToLeftLayout`,
  `HorizontalScroll`/`VerticalScroll`, `TransparencyKey`.

**Tests:** `KeyboardRoutingFunctionalTests` (a `ProcessCmdKey` override actually intercepts a key before
`KeyDown`, and returning `true` suppresses further routing), `ControlScalingFunctionalTests`,
`FormLifecycleEventsFunctionalTests` (legacy and modern event pairs both fire, in the documented order),
`FormPlacementFunctionalTests`.

**Exit:** `Control` and `Form` each <30 missing members; a custom control overriding `ProcessCmdKey` and
`SetBoundsCore` compiles and behaves.

---

## Phase 5 — remaining controls, theming shims, printing, fonts

1. **`UpDownBase` + `DomainUpDown`**, re-parenting `NumericUpDown` (139 missing) onto the real base —
   same pattern as Phase 1.2.
2. **The 11 `*Renderer` classes** — `ButtonRenderer`, `CheckBoxRenderer`, `RadioButtonRenderer`,
   `ComboBoxRenderer`, `TextBoxRenderer`, `GroupBoxRenderer`, `ProgressBarRenderer`, `ScrollBarRenderer`,
   `TabRenderer`, `TrackBarRenderer` — drawing through the GTK theme.
3. **`VisualStyles` namespace as compiling shims** (238 types). Mostly the `VisualStyleElement` catalogue:
   generate it rather than hand-write it. `VisualStyleRenderer` draws via GTK theme context where a part
   maps, and throws a documented exception where it does not. `Application.RenderWithVisualStyles` /
   `VisualStyleState` come with it.
4. **Printing** — `PrintController`, `StandardPrintController`, `PreviewPrintController`,
   `PreviewPageInfo`, `InvalidPrinterException`, the four `PrinterSettings` nested collections.
   `PrintPreviewDialog` (184 missing) cannot work without `PreviewPrintController`.
5. **Fonts** — `PrivateFontCollection` and `InstalledFontCollection` over Pango/fontconfig. Apps shipping
   embedded fonts currently cannot load them.
6. **Misc infrastructure** — `WindowsFormsSynchronizationContext`, `ThreadExceptionDialog` (the UI for
   the already-present `Application.ThreadException`), `IMessageFilter` +
   `Application.AddMessageFilter`/`RemoveMessageFilter`, `Help`/`HelpProvider`, `AmbientProperties`,
   `MdiClient`, `ListBindingHelper`, `BindingNavigator`, `OSFeature`/`FeatureSupport`,
   `FlowLayoutSettings`, `Layout.LayoutEngine` (fix the namespace — it is currently
   `System.Windows.Forms.LayoutEngine`).
7. **`Application` remainder** (22 missing) — `Idle`, `Restart()`, `ProductName`, `ProductVersion`,
   `CompanyName`, `UseWaitCursor`, `AllowQuit`, `SetDefaultFont`, `EnterThreadModal`/`LeaveThreadModal`.

**Exit:** core missing-type count (excluding declared non-goals) at zero.

---

## Phase 6 — behavioural fidelity

API presence is a floor. These are the open items from [EMULATION-GAPS.md](EMULATION-GAPS.md), plus what
its §8 admits was never assessed. **This phase is where the remaining risk lives**, because a
metadata diff cannot see any of it.

1. **Layout engines — untested against WinForms semantics.** `TableLayoutPanel`, `FlowLayoutPanel`,
   anchor/dock are fully reimplemented on GTK's measure/allocate cycle and have never been compared to
   WinForms measure/arrange. Assume pixel-level and edge-case divergence. Build an oracle-based suite:
   fixed control trees with known WinForms output, asserted position-by-position. **Highest-value item in
   this phase** — layout drift affects every form, silently.
2. **Event ordering and focus/tab order** — likewise never verified. Assert documented orderings
   (`Enter`/`GotFocus`/`Leave`/`Validating`/`Validated`, `Load`/`Shown`/`Activated`).
3. **`Control.CreateGraphics()`** (§2.1) — deferred off-screen surface, not immediate-mode; two
   `Graphics` on one control cannot coexist. Breaks rubber-band drawing. Either route through a
   composited overlay or document the deviation loudly. Substrate-bound; decide, don't drift.
4. **`Graphics.MeasureCharacterRanges`** — returns estimated regions; Pango exposes true per-range
   extents. Needed for hit-testing in custom text controls.
5. **`Graphics.SetClip(Region)`** through the region's `GraphicsPath` rather than its rectangle bands, so
   curved clips are exact (§5.4).
6. **Data-binding depth** (§5.6) — `IEditableObject` transactions and `INotifyPropertyChanged`
   propagation from the data-source side remain unestablished.
7. **Accessibility beyond announce-ability** (§6b scope limit) — per-control default roles,
   `AccessibleObject` state/value.
8. **`PathPointType`** so `GraphicsPath.PathTypes` bytes can be interpreted by name.

---

## Non-goals — document in the package README

Declare these loudly rather than leaving them to be discovered. Where a type must exist for source
compatibility, prefer a shim that throws `PlatformNotSupportedException` naming GTK as the reason — the
convention `Net4x.Gtk.Drawing` already uses — over a silent no-op.

| Non-goal | Count | Why |
|---|---|---|
| `AxHost` / `Com2Interop` / ActiveX | 12 | COM/ActiveX is Windows-only by definition |
| `WebBrowser` / `Html*` | 17 | Needs an embedded browser engine. **Either implement or delete the four orphaned `WebBrowser*EventArgs` types that are currently unreachable.** |
| Legacy `DataGrid` family | 32 | Deprecated in WinForms itself since 2.0 |
| Legacy `ToolBar`/`ToolBarButton` | 8 | Superseded by `ToolStrip` |
| EMF/WMF metafiles | — | Windows record format with no cross-platform playback |
| `NativeWindow`, `SendKeys`, Win32 message pump | — | Below the event model; unportable by construction |
| `Imaging.Effects.*` | 25 | GDI+-specific, .NET 8+ only |
| `Control.Handle` returns a GTK handle, not an `HWND` | — | Correct by necessity — **must be stated in the README** |

---

## Sequencing and effort shape

```
Phase 0  instrument           ── must precede everything
Phase 1  structural           ── ≈1 500 members, highest leverage, most behavioural risk
Phase 2  enums / event args   ── mechanical, parallelisable, unblocks existing members
Phase 3  ToolStrip            ── ≈2 707 members (30 %), needs Phase 1.1
Phase 4  Control / Form       ── needs the keyboard-routing decision
Phase 5  remaining + shims    ── VisualStyles catalogue should be generated, not written
Phase 6  behavioural fidelity ── where the residual risk actually lives
```

Phases 2 and 5 parallelise well (independent file sets, low conflict risk in the patch sequence).
Phases 1, 3 and 4 are ordered and should be serialised.

**Definition of done:** the compile corpus builds; core missing-type count is zero excluding declared
non-goals; missing members <500 against the net48 baseline; the layout oracle suite passes; the CI
api-diff gate is green at the ratcheted threshold.

## Two honest caveats

- **Member counts are a proxy for effort, not a measure of it.** `ToolStripDropDown`'s 376 are one
  re-parenting; `Graphics`'s 43 are 43 separate rendering problems. Weight by subsystem, not by count.
- **The plan is ordered by leverage, not by user-visible value.** If the actual goal is "*our* app runs",
  the fastest route is to compile that app against the shim, and let its error list reorder these phases.
  This plan optimises for general parity, which is a different and slower objective.

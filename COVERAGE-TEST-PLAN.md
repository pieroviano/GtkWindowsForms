# Coverage Test Plan — Net4x.Gtk.Windows.Forms

Plan for raising `Net4x.Gtk.Windows.Forms.csproj` line **and** branch coverage to **≥ 70%** with
functional unit tests, driven by the per-namespace coverage report (VS `.coverage`, 2026-07-22:
**49.3% line / 51.2% branch** overall — consistent with the cobertura measure of 49.9%).

## Ground rules (unchanged, restated so the plan is self-contained)

- Tests must verify **behaviour against a real oracle** (WinForms semantics). No reflection-driven
  line-hitting, no trivial getter/setter tests, no test that would still pass on broken code.
- If a test exposes a defect, **fix the production code**; never weaken a test to make it pass.
  Every production fix goes into `patches/0087-*` (Drawing) / `patches/0088-*` (Forms), regenerated
  against base `e316905b`, `git apply --reverse --check` verified.
- All three suites stay green after every iteration: Drawing (1,171), Forms (2,010+), UI baselines
  (10 — never updated to hide a regression).
- Each iteration starts with the mandated **“What do you suggest as the next step?”** proposal and
  ends with a re-measure.

## Reading of the coverage image

| Namespace (image row) | Line % | Branch % | What lives there | Verdict |
|---|---|---|---|---|
| `System.Windows.Forms` | 49.7 | 50.9 | The bulk of the assembly: `Control`, `ListView`, `ToolStripItem`, `BindingSource`, `Binding`, `CurrencyManager`, `Form`, `DataGridView*`, `NotifyIcon`, `PictureBoxBase`, … | **Primary target** — largest absolute mass |
| `System.Drawing.Printing` | 0.0 | 0.0 | 22 files / ~2,650 source lines: `Margins`, `MarginsConverter`, `PageSettings`, `PaperSize/Source`, `PrinterSettings`, `PrintDocument`, event args | **Best ratio** — pure model code, fully testable, currently untouched |
| `System.Windows.Forms.PropertyGridInternal` | 22.8 | 35.8 | `GridEntry`, `PropertyGridView`, converters (8+6+5 throw sites in the census) | Target — model-level entry population testable |
| `GtkSystem.Windows.Forms` | 0.0 | 0.0 | `ControlUtility` (Gtk.Widget → WinForms `Control` mapping) | Target — small, real logic, testable with in-process widgets |
| `System.Resources` / `GtkSystem.Resources` / `.Extensions` | 56.7 / 56.4 / 61.0 | | `DeserializingResourceReader` (345 uncovered), `ResXDataNode` (275), `ResXResourceReader` (203) | Target — round-trip oracles already proven in `ResXRoundTripFunctionalTests` |
| `System.Windows.Forms.GtkRender` | 46.1 | 48.7 | 7 cell/render helper files | Partial — model-level pieces only; paint paths need a realized toplevel |
| `System.Windows.Forms.Layout` | 53.6 | 48.2 | `ArrangedElementCollection`, `IArrangedElement` | Target — collection semantics |
| `GtkSystem.Windows.Forms.Utility` | 46.8 | 45.6 | `ImageUtility` remainder, `ControlUtility` siblings | Target — image conversion has bitmap oracles |
| `GtkSystem.Windows.Forms.GtkControls.ControlBase` | 51.6 | 58.4 | GTK widget base classes | Partial — covered indirectly via control tests; no direct widget-internals tests |
| `System.Runtime.CompilerServices` | 50.0 | 66.7 | `BitOperations` (internal; `InternalsVisibleTo` grants access) | Target — real bit-twiddling logic, exact oracles |
| `System.ComponentModel.Design` | 61.1 | 70.6 | Designer service types | Top-up only if still short of 70% |
| `System.Windows.Forms.Design` | 75.6 | 67.4 | Already above target | Branch top-up only |
| `GtkControls` / `GtkSystem.IO` / `GtkSystem.ComponentModel` | 81.8 / 84.2 / 83.3 | | Already above target | No action |

## Explicit exclusions (the 70% is understood net of these)

| Area | Image row | Why excluded |
|---|---|---|
| `System.Diagnostics.CodeAnalysis` (0%) | attribute shims (`AllowNull`, `DynamicallyAccessedMembers`, …) | No behaviour — only trivial construction tests exist, which the quality rules forbid. |
| `System.HashCode` (~157 lines, most of the `System` 14.4% row) | `System` | Untestable from the net10.0 test project: CS0433 collision with the BCL type; disambiguating would require aliasing the entire Forms reference and breaking every other test file. Internal-consumption only. |
| `MessageBoxOld` (240) + `NonStaticMessageBox` (223) | `System.Windows.Forms` | Require a live GTK dialog loop; a unit test would deadlock or assert nothing real. Candidate for the UI-test harness later, not for this plan. |
| GTK paint/realize internals (`ControlBase` widget overrides, `GtkRender` paint paths, `System.Drawing.Printing` GTK print-operation dispatch) | several | Only reachable with a realized toplevel + event loop; covered indirectly by `GtkWinFormsApp.UITests` pixel baselines. |
| `Handle`/`WndProc`/GDI-handle interop | `System.Windows.Forms` | Platform boundary; by convention throws `PlatformNotSupportedException` (already tested where reachable). |

Estimated excluded mass: ~900–1,100 coverable lines. The ≥70% target remains achievable without
them (gap at 49.9% ≈ 6,500 lines; identified testable mass below ≈ 5,900+ lines of which roughly
70–80% is realistically coverable, plus the long tail).

## Iterations (priority order)

Each iteration = propose → write functional tests → fix any exposed defects (durable via patch) →
full suites + UI baselines → re-measure → sync `EMULATION-GAPS.md`.

### 1. `System.Drawing.Printing` — from 0%
The whole namespace is model code with exact WinForms oracles. **Expected: +2.5–3% overall.**
- `Margins`: ctor validation (negative → `ArgumentException`), equality/`==`/`GetHashCode`, `Clone`.
- `MarginsConverter`: `CanConvertFrom/To`, string ⇄ `Margins` round-trip, `InstanceDescriptor`.
- `PaperSize`/`PaperSource`/`PrinterResolution`: ctor + `Kind` logic (custom vs named), `ToString`.
- `PageSettings`: defaults, `Bounds` swaps with `Landscape`, margins interplay, `Clone` independence.
- `PrinterSettings`: defaults (`Copies`, `FromPage/ToPage` validation), `Clone`.
- `PrintDocument`: `OnBeginPrint`/`OnQueryPageSettings`/`OnEndPrint` raiser contract (no actual
  print dispatch), `DocumentName` default, `PrintEventArgs.Cancel` flow.
- Expect stub pathologies (the namespace has never been exercised) — budget for production fixes.

### 2. `Control` (914 uncovered) — the largest single block
Break down per-method first; test only what has observable WinForms semantics:
- Anchoring/docking recompute, `Scale`/bounds math, `PointToClient/PointToScreen` (model side),
  `Controls` add/remove/`SetChildIndex` (dovetails with the open **BringToFront/SendToBack** gap —
  implementing real z-order closes a gap *and* covers code), `Select`/`GetNextControl` tab order,
  ambient property fallbacks (`Font`/`ForeColor`/`Cursor` inheritance), `Visible`/`Enabled`
  cascading to children, coordinate/`ClientRectangle` math, `Text` change events.
- Skip: drag-drop, IME, `WndProc` surface (excluded above).
- **Expected: +2–2.5%** (realistic half of the block).

### 3. Data binding triad: `BindingSource` (396) + `Binding` (248) + `CurrencyManager` (199)
- `BindingSource`: `Filter`/`Sort` residue, `AddNew`/`RemoveCurrent`/`EndEdit`/`CancelEdit`
  transactionality, `ListChanged` reasons, currency (`Position`/`Current`) movement.
- `Binding`: format/parse event pipeline (`Format`, `Parse`, `FormattingEnabled`,
  `NullValue`, `DataSourceUpdateMode` write-back timing).
- `CurrencyManager`: position clamping, `ItemChanged`, `Refresh` after list mutation.
- Oracle: real WinForms two-way binding semantics. **Expected: +1.5–2%.**

### 4. Resources readers: `DeserializingResourceReader` (345) + `ResXDataNode` (275) + `ResXResourceReader` (203)
- Extend the proven round-trip harness: binary `.resources` written by the shim's writer and read
  back (strings, primitives, byte[], streams, serializable objects, type-converter values),
  `ResXDataNode.GetValue` with assembly-name resolution, `FileRef` nodes, culture fallback.
- **Expected: +1.5–2%.**

### 5. `ListView` (504) + `ToolStripItem` (402) remainder
- `ListView`: virtual-mode contract, `TopItem`/`EnsureVisible` model side, checked/selected index
  collections' cross-consistency, label-edit event flow, sorting with `ListViewItemSorter`.
- `ToolStripItem`: remaining layout/paint-free members — `TextImageRelation`/`ImageAlign`
  preferred-size math, `Available`/`Visible` distinction, owner re-parenting, `MergeAction` model.
- **Expected: +1.5%.**

### 6. `DataGridView` family remainder: `DataGridViewRowCollection` (224) + `DataGridViewColumn` (196)
- Row collection: `AddCopy`/`Insert`/shared-row semantics, `GetRowCount(states)` filters,
  `RemoveAt` renumbering; column: `Frozen`/`Visible` index maps, autosize mode fallback, clone.
- Builds on this iteration's binding suite. **Expected: +1%.**

### 7. `Form` (227) + `NotifyIcon` (189) + `PictureBoxBase` (169)
- `Form`: **implement `AcceptButton`/`CancelButton`** (top open emulation gap — compile break on
  common code; `IButtonControl` + `Button.DialogResult` already exist), `Modal`/`DialogResult`
  close flow, `OwnedForms`, `MdiChildren` model.
- `NotifyIcon`: property/event model without a live tray (icon assignment, visibility latching,
  `ContextMenuStrip` wiring).
- `PictureBoxBase`: `SizeMode` layout math against known bitmap sizes, `Load(string)` errors.
- **Expected: +1.5%** and closes two `EMULATION-GAPS.md` entries.

### 8. `PropertyGridInternal` (22.8%) + `PropertyGrid` throw census
- `SelectedObject` → `GridEntry` tree population, category grouping, value get/set through
  entries, `TypeConverter` usage; convert census throw sites (`GridEntry` 5, `PropertyGrid` 6,
  `ListViewGroupConverter` 8, `ListViewItemConverter` 6) into behaviour + tests.
- **Expected: +1%.**

### 9. Small full-closure targets (batched into one iteration)
- `GtkSystem.Windows.Forms.ControlUtility` (0%): widget→`Control` resolution walking parents,
  `TryToControl<T>` type filtering — real GTK widgets in-process.
- `System.Runtime.CompilerServices.BitOperations` (50%, internal via `InternalsVisibleTo`):
  exact-value oracles for `Log2`/`RoundUpToPowerOf2`/rotates.
- `System.Windows.Forms.Layout.ArrangedElementCollection` (53.6%): enumeration/equality contract.
- `GtkSystem.Windows.Forms.Utility.ImageUtility` remainder (46.8%): conversion round-trips.
- `GtkRender` model-level pieces (46.1%): whatever is reachable without a realized widget.
- **Expected: +1%.**

### 10. Re-measure and top-up
- If still < 70% line or branch: `System.ComponentModel.Design` (61.1%), branch-only deficits
  (error paths, argument validation branches in already-touched classes), `System.Resources`
  remainder. Branch coverage typically lags line by ~2 points — the error-condition tests in each
  iteration above are what closes it.

## Projected arithmetic

Sum of expected gains: **+13.5–16%** on top of 49.9% → **63–66%** after iteration 9, with the
long tail (hundreds of classes each missing 10–50 lines, which every iteration's tests also graze)
and iteration 10 covering the rest to ≥70%. If the projection falls short after iteration 7, the
plan is re-based on a fresh per-class uncovered-lines table before proceeding.

## Tracking

| Iteration | Target | Status | Line % after | Branch % after |
|---|---|---|---|---|
| 0 | Baseline (image) | done | 49.3 (49.9 cobertura) | 51.2 |
| 1 | System.Drawing.Printing | **done** — 54 tests, 7 defects fixed (6 Printing + `Image.Pixbuf` lossy re-encode), namespace 0 → 83.4% | 51.1 (cobertura) | 41.1 (cobertura¹) |
| 2 | Control | **done** — 21 tests, 4 defects fixed (`FindForm` infinite loop, `EndInvoke` swallowed exceptions, `DeviceDpi` = 0, `LogicalToDeviceUnits` identity); Control.cs 53.9 → 57.5%. Remaining mass is `Widget_WidgetEvent` (201) + `SetStyle(Gtk.Widget)` (177) + dock/anchor realize paths — excluded (GDK-event/render-bound) | 51.4 | 41.4 |
| 3 | BindingSource/Binding/CurrencyManager | **done** — 17 tests; 1 defect fixed (`TextBox.TextChanged` suppressed while invisible, breaking `OnPropertyChanged` bindings) + `DataBindings` annotated non-nullable; triad at 68.5/70.3/69.7%. Two WinForms contracts encoded: `Find` needs a searchable `IBindingList`; `ModifierKeys`/`MouseButtons` read live device state (untestable) | 51.8 | 41.9 |
| 4 | Resource readers | **done** — 14 tests (binary round trip of all 15 primitive type codes + byte[]/stream/hash-order/reset; ResX typed values, wrapped base64, FromFileContents, path ctor, FileRef+BasePath); no defects found; readers at 55.8/54.4/67.3% (rest is BinaryFormatter-era deserialization and error paths) | 52.4 | 42.7 |
| 5 | ListView + ToolStripItem | **done** — 35 tests (ListView sorter/keys/ranges/checked state; ToolStripItem OnXxx raiser sweep); 1 defect fixed (`ListView.Sort()` ignored `ListViewItemSorter` — the custom-sorter API was a no-op). Remaining mass is realized-widget native paths (NativeAdd 140 etc.), which belong to the UI pixel suite | 52.7 | 43.0 |
| 6 | DataGridView collections | **done** — 11 tests, 6 defects fixed (`Row.State` hard-coded to `None`; whole `GetFirstRow`/`GetNextRow`/… family used equality instead of flag tests, inverted the exclude filter and indexed `items[-1]`; `Insert(index, count)` NRE; `Column.Clone` returned null; `DisplayIndex = 0` unreachable) | 53.2 | 43.5 |
| 7 | Form + NotifyIcon + PictureBoxBase | pending | | |
| 8 | PropertyGridInternal | pending | | |
| 9 | Small full-closure batch | pending | | |
| 10 | Top-up to ≥70/≥70 | pending | | |

¹ The cobertura branch metric counts differently from the VS `.coverage` viewer (41.1 vs 51.2 on
near-identical trees); track trend per tool, not across tools.

# Plan — Gtk3↔Gtk4 sample parity UI tests

**Status:** **infrastructure implemented and green; catalogue is an initial slice.** See §9.
**Goal:** drive every interactive control on every form of the sample application, first against the
**Gtk3 sample in the `gtksystem-windows-forms` submodule** (the reference), then against the **Gtk4 sample**
(`GtkWinFormsApp`), and assert the two behave the same — so that porting Gtk3 → Gtk4 cannot introduce a
behavioural regression without a test failing.

This extends, and does not replace, the existing single-app suite described in
[SampleUITests-Plan.md](SampleUITests-Plan.md) and implemented in
[GtkWinFormsApp.UITests](GtkWinFormsApp.UITests). That suite answers "does the Gtk4 sample still work?".
This one answers a different question: "does the Gtk4 sample still do **what the Gtk3 one did**?"

---

## 0. Blocking constraints — settle these before writing any code

Each is a fact about the two code bases that the architecture has to be built around, not a risk to be
managed later. B1 in particular rules out the obvious design.

### B1 — Gtk3 and Gtk4 cannot be loaded into one process. **(architecture-defining)**

The reference sample links the vendored upstream `GTKSystem.Windows.Forms`, which binds **GTK 3**; the Gtk4
sample links this repo's `Gtk.Windows.Forms`, which binds **GTK 4**. Loading both native stacks into one
process is not supported and will not survive `gtk_init`. Both also define the same managed type names
(`System.Windows.Forms.Form`, …), so a single test assembly cannot even reference both.

**Consequence:** a test cannot open both samples and compare them in-process. The suite must be:

1. two separate **harness executables**, one per GTK generation, each of which runs the whole scenario
   catalogue against its own sample and writes a machine-readable **trace**; and
2. one xUnit **comparison project** that runs both harnesses as child processes and asserts their traces
   agree.

**Verify first:** build and run a throwaway console app that references the submodule sample project and
calls `Application.Init()` + shows `Form1`, confirming the Gtk3 sample is drivable from a process we own.

### B2 — Control names diverge between the two samples

The sample transform translates Chinese identifiers, so a minority of `Name` values differ. Measured across
the designers:

| Form | Names identical | Total |
|---|---|---|
| Form1 | 58 | 61 |
| Form2 | 6 | 6 |
| Form3 | 56 | 61 |
| Form4 | 25 | 30 |
| Test1 | 58 | 58 |
| UserControl11 | 2 | 2 |

The 13 that differ are exactly those whose name contained Chinese, e.g. `bb菜单ToolStripMenuItem` →
`" bbMenu2ToolStripMenuItem"`. Note the **leading space** the translation introduces; scenarios must not
assume names are trimmed.

**Consequence:** scenarios address controls by a *logical* name resolved through a small alias table, seeded
from [Resources/samplesTranslations.xml](Resources/samplesTranslations.xml) (375 entries). A guard test must
assert every logical name in the catalogue resolves on **both** sides — otherwise a scenario silently
degrades into "control not found" on one side and the parity check passes vacuously.

### B3 — Captions differ by language, so literal text is not comparable across the two runs

The reference sample renders Chinese (`打开主窗口`, `菜单1`); the Gtk4 sample renders English (`Open Main
Window`, `Menu 1`). Any oracle that captures a caption will differ between the runs by design.

**Consequence:** the trace records, for text-valued observations, a **normalized** value: either the
translation-mapped counterpart or a stable token (`<text:changed>`, `<text:unchanged>`). Raw captions may be
compared **within** one engine across time, never across engines.

### B4 — The submodule is vendored upstream and must not be modified

The Gtk4 sample has internalised Designer fields and an `InternalsVisibleTo` for the UI tests, both
reproduced by `GtkSampleAppProjectsHandler`. The submodule sample has **neither**, and adding them would
mean editing vendored upstream — which the project rules forbid.

**Consequence:** the Gtk3 harness reaches controls by recursive `Name` lookup over `Controls`, not by field
access. For symmetry the shared scenario code uses that same lookup on both sides, so one scenario
definition runs unchanged against either sample.

### B5 — Absolute pixel geometry is not comparable across GTK generations

Gtk3 and Gtk4 ship different themes and control metrics. A strip that is 30px in Gtk3 is legitimately 33px
in Gtk4 (the Gtk3 build clamps every strip to 30, ignoring the designed size — see the commit history for
why that clamp was removed).

**Consequence:** geometry oracles compare **invariants**, not numbers, across engines — "the submenu opens
to the right of its parent", "the strip is no taller than its designed height", "items are ordered
left-to-right without overlap". Absolute values are still recorded, and may be gated **within** one engine
as a regression baseline.

---

## 1. Decisions taken

| # | Decision |
|---|---|
| D1 | **Two-process trace-and-compare**, forced by B1. Gtk3 is the reference; the Gtk4 trace must match it. |
| D2 | **Hybrid input.** API-level verbs drive the bulk; a small quarantined set uses real OS input for behaviour the API level cannot observe. |
| D3 | **Oracles are observable state + widget geometry.** No screenshot baselines in this suite — the existing suite already gates the Gtk4 sample's pixels, and cross-engine pixel comparison is meaningless (B5). |
| D4 | **Catalogue granularity: every wired handler, plus one representative scenario per control type.** A manual pass exercises what the sample actually wires; unwired controls still get a state round-trip so the control type is covered. |

### Why not one scenario per list item

An exhaustive per-item enumeration would add hundreds of entries whose only assertion is "selection
changed", and it would need rewriting whenever the sample's seed data changes — `Test1` seeds its grid from
`DateTime.Now.Ticks`. Representative coverage per control type plus every wired handler is complete against
*behaviour* without that churn. Where an item's identity matters (a tree node that opens a specific form),
it is a wired handler and therefore already enumerated.

---

## 2. Architecture

```
SampleParity/
  Shared/                     scenario catalogue + runner, compiled into BOTH harnesses as linked source
    Scenario.cs               a scenario: id, form, steps, oracles
    ScenarioCatalogue.cs      the catalogue (section 4)
    ControlLocator.cs         recursive Name lookup + alias table (B2, B4)
    TraceWriter.cs            newline-delimited JSON observations
    ApiInput.cs               Click/Select/Check/Type/Expand verbs (API level)
  Gtk3Harness/                exe, references gtksystem-windows-forms sample + upstream library
  Gtk4Harness/                exe, references GtkWinFormsApp + Gtk.Windows.Forms
  SampleParity.Tests/         xUnit (net10.0): runs both harnesses, compares traces
```

**The shared layer is linked source, not a shared assembly.** It cannot be a library, because each side must
compile against a *different* `System.Windows.Forms`:

```xml
<Compile Include="..\Shared\**\*.cs" LinkBase="Shared" />
```

**Trace format** — one JSON object per line, ordered deterministically by scenario then step:

```json
{"scenario":"form2.listbox.select","step":3,"kind":"state","control":"listBox1","property":"SelectedIndex","value":"2"}
{"scenario":"form2.listbox.select","step":3,"kind":"state","control":"label1","property":"Text","value":"<text:changed>"}
{"scenario":"form3.menu.submenu","step":2,"kind":"geometry","control":"bMenu","property":"DropDownSide","value":"Right"}
```

`kind` is `state`, `geometry`, `window` (a form opened/closed) or `error`. Normalization per B3/B5 happens
in the harness, so the comparison is a plain ordered diff.

**The comparison test** asserts: same scenario ids in the same order; same observation sequence per
scenario; no `error` records on either side. A mismatch reports scenario, step and both values.

---

## 3. Input model (D2)

**API level (default).** Extends the verbs already in
[UiContext.cs](GtkWinFormsApp.UITests/Infrastructure/UiContext.cs) — `Click`, `Type`, `Select` — with the
ones this catalogue needs: `Check(CheckBox/RadioButton)`, `CheckItem(CheckedListBox, index)`,
`SelectNode(TreeView, path)`, `SelectListViewItem`, `SelectTab`, `SetCell(DataGridView, row, col)`,
`SetValue(NumericUpDown/TrackBar)`. Each raises the same event path a real click would and then pumps.

**Real OS input (quarantined).** Scenarios tagged `RequiresPointer` drive the actual window with synthetic
mouse input. They exist because the API level provably cannot see this class of defect — every menu bug
fixed in this repo recently (submenu cascading downwards over its own menu, missing submenu arrow, absent
hover highlight, drop-down captions painted white-on-white) is invisible to `OnClick`. They are:

- skipped unless a real desktop session is available;
- run last, so a lost pointer grab cannot destabilise the deterministic scenarios;
- compared as **invariants** (B5), not pixels.

---

## 4. Scenario catalogue

Handler inventory below is extracted from the reference sample's designers and code-behind, so it reflects
what is actually wired rather than what the forms could in principle do. ~64 wired events across 7 forms.

### Form1 — `gtksystem-windows-forms/Samples/GTKWinFormsApp/Form1.cs`

| Scenario | Trigger | Oracle |
|---|---|---|
| `form1.load` | show the form | no `error`; expected control set resolves |
| `form1.button1.click` | `button1.Click` | state after handler |
| `form1.button1.paint` | force a redraw | paint ran without error |
| `form1.button2.click` | `button2.Click` | state |
| `form1.button6.click` | `button6.Click` | state |
| `form1.button7.click` | `button7.Click` | state |
| `form1.checkedlistbox.itemcheck` | `checkedListBox1.ItemCheck` via `SetItemChecked(0,true)` | item check state + handler side effect |
| `form1.grid.click` / `.doubleclick` | `dataGridView1.Click`, `.DoubleClick` | current cell, spawned window |
| `form1.picturebox2.mousemove` | pointer move over `pictureBox2` (`RequiresPointer`) | handler observed |
| `form1.picturebox2.paint` | redraw | paint ran |
| `form1.textbox1.keys` | `KeyDown`/`KeyPress`/`KeyUp` on `textBox1` | resulting `Text` (normalized) |
| `form1.treeview.select` | select node path | `SelectedNode` text (normalized) |
| `form1.tabcontrol.select` | select each tab | `SelectedIndex` |
| `form1.radio.check` ×3, `form1.checkbox.check` ×3 | set `Checked` | `Checked` round-trip, mutual exclusion for radios |
| `form1.numericupdown.value`, `form1.maskedtextbox.text` ×2, `form1.richtextbox.text` | set value | round-trip |
| `form1.splitcontainer.splitter` ×2 | set `SplitterDistance` | round-trip + panel geometry invariant |
| `form1.menu.*` | each of the 9 `ToolStripMenuItem`s | item enabled/checked state; drop-down side (geometry) |

### Form2 — `Form2.cs`

| Scenario | Trigger | Oracle |
|---|---|---|
| `form2.button1.click` | `button1.Click` | state |
| `form2.listbox.select` | `listBox1.SelectedIndexChanged` per index | `SelectedIndex` + handler effect |
| `form2.listview.click` | `listView1.Click` | state |
| `form2.listview.itemselection` | `ItemSelectionChanged`, `SelectedIndexChanged` | selected indices |
| `form2.listview.mousedown` | `MouseDown` (`RequiresPointer`) | handler observed |
| `form2.timer.tick` | pump until `timer1.Tick` fires | text advanced (`<text:changed>`; the handler appends `DateTime.Now`, so the literal value is not comparable) |
| `form2.formclosing` | close | `FormClosing` ran; auto-dismiss any modal |

### Form3 — `Form3.cs` (startup form)

| Scenario | Trigger | Oracle |
|---|---|---|
| `form3.load`, `form3.shown` | show | client size equals designed `ClientSize` (geometry) |
| `form3.button1..5.click` | 5 handlers | state; `button5` opens `Test1` → `window` record |
| `form3.panel1.click` / `.doubleclick` / `.mouseup` | panel handlers | state |
| `form3.panel3.paint`, `form3.panel5.click` | redraw / click | ran |
| `form3.trackbar.scroll` | `trackBar1.Scroll` | `Value` + the label the handler updates |
| `form3.menu.top` ×2 | open each menu-bar drop-down | drop-down opens **below** its item (geometry) |
| `form3.menu.submenu` | open `b Menu`'s submenu | opens **to the right**, top edge level with the row; parent menu still fully visible (geometry) |
| `form3.menu.item.click` ×3 | `ddddToolStripMenuItem1`, `toolStripMenuItem2/7/8` | handler effect; menu closes |
| `form3.menu.hover` | hover a row (`RequiresPointer`) | row reports selected styling |
| `form3.toolstrip.splitbutton` | `Click`, `DoubleClick`, `MouseUp` | state |
| `form3.toolstrip.geometry` | measure | strip height ≤ designed; all items within the strip width; text box at its designed width |
| `form3.statusstrip.geometry` | measure | strip sits at the client bottom, does not overlap the content above |
| `form3.linklabel.click` | click | handler ran; no browser launch in test mode |
| `form3.formclosing` | close | ran |

### Form4 — `Form4.cs`

| Scenario | Trigger | Oracle |
|---|---|---|
| `form4.shown` | show | control set resolves |
| `form4.button3/4/6/7/8/9/10.click` | 7 handlers | state; any spawned window recorded |
| `form4.treeview.select` | select nodes | `SelectedNode` (normalized) |
| `form4.splitcontainer.splitter` | set distance | round-trip |
| `form4.menu.*` | 5 menu items | state |

### Form5 — `Form5.cs`

The form declares **no controls and wires no events**. One scenario: `form5.open` — it constructs, shows and
closes without error. Worth keeping precisely because an empty form is where sizing bugs surface unmasked.

### Test1 — `Test1.cs`

| Scenario | Trigger | Oracle |
|---|---|---|
| `test1.button17.click` | the one wired button | state |
| `test1.buttons.smoke` | remaining 16 buttons | no `error` (unwired: existence + enabled round-trip) |
| `test1.radio.check` ×6 | set `Checked` | mutual exclusion within each group |
| `test1.grid.click` | `dataGridView1.Click` | current cell |
| `test1.grid.content` | read row/column counts | counts match between engines (**not** cell values — seeded from `DateTime.Now.Ticks`) |
| `test1.picturebox.paint` ×8 | redraw each chart box | each `Paint` ran without error |
| `test1.timer.tick` | pump | tick observed |

### UserControl11 — `UserControl11.cs`

| Scenario | Trigger | Oracle |
|---|---|---|
| `usercontrol11.host` | host it in a form | renders, control set resolves |
| `usercontrol11.mousewheel` | scroll (`RequiresPointer`) | `MouseWheel` handler observed |
| `usercontrol11.button.click` | click | state |

---

## 5. Verification

1. **B1 spike passes** — a console app drives the submodule sample.
2. **Locator guard test** — every logical name in the catalogue resolves on both sides; fails loudly rather
   than degrading to a vacuous pass (B2).
3. **Self-parity** — run the Gtk3 harness twice; traces must be byte-identical. Any difference is
   non-determinism (timers, seeded data) to normalize before comparing engines at all.
4. **Cross-engine parity** — Gtk3 trace vs Gtk4 trace, the actual deliverable.
5. **Suite stays green** — `dotnet test Gtk.Windows.Forms.sln -m:1` (serial; parallel assemblies time out
   `Application.Init`).
6. **The two-process runner must not leave orphans** — a harness that hangs has to be killed with a timeout,
   or CI wedges.

## 6. Risks

| # | Risk | Impact | Mitigation |
|---|---|---|---|
| R1 | Gtk3 harness cannot be built or run at all (upstream sample expects its own solution) | Whole plan | B1 spike, before anything else |
| R2 | Non-determinism (`timer1`, `DateTime.Now.Ticks` grid seed, animation) makes traces differ run to run | Constant false failures | Verification step 3 is a hard gate; normalize or exclude before comparing engines |
| R3 | Legitimate Gtk3↔Gtk4 differences get encoded as "regressions" | Suite fights the port | Every accepted difference goes in an explicit, commented allow-list keyed by scenario+step — reviewed, not silently tolerated |
| R4 | `RequiresPointer` scenarios flake or steal focus | Noisy suite | Quarantined, run last, skipped without a desktop |
| R5 | The submodule is updated and its sample changes | Catalogue drifts | Locator guard (step 2) fails on a renamed control rather than passing vacuously |
| R6 | Modal dialogs block a harness | Hang | Reuse the auto-closing `MessageBox` the existing suite installs |
| R7 | Catalogue rots as the sample gains controls | Coverage illusion | A coverage test enumerates wired events by reflection and asserts each has a scenario |

## 7. Out of scope

Screenshot baselines (D3, and the existing suite covers the Gtk4 sample's pixels); the second sample pair
(`GTKSystemWinFormsApp11` / `GtkSystemWinFormsApp11`); designer-time behaviour; localisation beyond the
normalization in B3.

## 9. Implementation record

### Delivered

| Artefact | Location |
|---|---|
| Trace format and writer | [SampleParity/Shared/Trace.cs](SampleParity/Shared/Trace.cs) |
| Control/strip locator | [SampleParity/Shared/ControlLocator.cs](SampleParity/Shared/ControlLocator.cs) |
| Scenario model, context and input verbs | [SampleParity/Shared/Scenario.cs](SampleParity/Shared/Scenario.cs) |
| Scenario catalogue | [SampleParity/Shared/ScenarioCatalogue.cs](SampleParity/Shared/ScenarioCatalogue.cs) |
| Shared harness body | [SampleParity/Shared/HarnessMain.cs](SampleParity/Shared/HarnessMain.cs) |
| Reference (Gtk3) harness | [SampleParity/Gtk3Harness](SampleParity/Gtk3Harness) |
| Ported (Gtk4) harness | [SampleParity/Gtk4Harness](SampleParity/Gtk4Harness) |
| Parity tests | [SampleParity/SampleParity.Tests](SampleParity/SampleParity.Tests) |
| Divergence backlog | [KnownDivergences.cs](SampleParity/SampleParity.Tests/KnownDivergences.cs) |

Both harnesses and the test project are in `Gtk.Windows.Forms.sln`, under the `SampleParity` solution
folder; the three upstream projects needed to build the reference sample are under `Submodules`.

### Blocking constraints — outcomes

- **B1 — CONFIRMED, and the design holds.** The reference sample builds and is drivable from a harness we
  own. Both harnesses run to completion and emit 85 records each with zero errors.
- **B2 — implemented as planned**, with one correction: the divergent names are addressed by `path:`
  (structural position) rather than an alias table, because on the Gtk4 side several are localized resource
  lookups rather than literals, so there is no fixed string to alias.
- **B3 — implemented.** `ScenarioContext.Text` reduces captions to `<text:changed>` / `<text:unchanged>` /
  `<text:cleared>`.
- **B4 — confirmed.** The reference sample's forms are `public` but its Designer fields are `private`, so
  the recursive name lookup is required, as anticipated.
- **B5 — implemented.** Geometry is recorded as invariants, never pixel counts.

### Deviations from the plan as written

- **Strips are found by having an `Items` collection, not by base class.** The plan assumed
  `OfType<ToolStrip>()`. That is wrong: upstream declares `MenuStrip : Control` while the port declares
  `MenuStrip : ToolStrip`, so the filter silently dropped the menu bar on the Gtk3 side only - every menu
  scenario would have passed vacuously there.
- **Modal dialogs are defused per engine, via a new `ISampleForms.InstallModalGuard`.** The plan assumed the
  auto-closing MessageBox could be reused on both sides. It cannot: upstream's `MessageBox` is a static
  class with no replaceable instance, and the submodule must not be modified. It blocks in
  `Gtk.Dialog.Run()`, which spins a nested main loop, so the Gtk3 harness dismisses it from a GLib timer
  instead. Without this the harness hung on `Form2`'s `FormClosing`.

### What the suite found on its first run

- **`Test1` exposes 54 controls on Gtk3 but 29 on Gtk4** (Labels 16→2, Panels 5→1, PictureBoxes 8→1). The
  two designers declare identical counts, and `Form3` - which does not nest controls in panels - agrees
  exactly at 24. The likely cause is that a container's `Controls` collection does not expose its children
  on the Gtk4 side, which would affect any consumer enumerating a container. Recorded in
  `KnownDivergences` and **unresolved**.

### Still to do

The catalogue is an initial slice: the structural census of all six forms plus `Form2`, `Form3` and `Form5`
behaviour. The remaining per-handler scenarios in §4 - Form1's 14 wired events, Form4's 8, Test1's 12,
UserControl11 - and the whole `RequiresPointer` tier are not yet written. The infrastructure they need is
in place.

## 8. Open question for the operator

The Gtk3 harness pins behaviour **as upstream implements it**, including its bugs — for instance the Gtk3
build clamps every strip to 30px and ignores the designed 33px. Where Gtk4 is *deliberately more correct*
than Gtk3, the parity check will flag it. R3's allow-list is where those go; it needs to be reviewed as a
record of intentional divergence, not treated as a suppression file.

# SampleUITests-Plan.md — Automated UI regression tests for `GtkWinFormsApp`

## 1. Goal

The upstream author regression-tests the library by **running `GtkWinFormsApp` by hand**: opening each form, clicking buttons, typing in fields, switching tabs/grids, and eyeballing that nothing broke. This is thorough but slow and error-prone.

This plan describes a **new automated test project — `GtkWinFormsApp.UITests`** — that drives *the real sample forms* end-to-end and asserts, automatically, that:

1. every form loads and every scripted interaction runs **without throwing or crashing the GTK loop** (the failure class we have been fixing all session), and
2. after each interaction the **observable control/model state** is what it should be (state oracle), and
3. each form still **looks** right, via **screenshot comparison against committed baselines with tolerance** (visual oracle).

It is not a headless/CI target — it is designed to run on a **developer's Windows desktop with a real display** (see §9).

## 2. Fixed decisions (agreed before writing this plan)

| # | Decision | Choice |
|---|---|---|
| D1 | How "clicks" are simulated | **WinForms API level** — call the control's own API / raise the handler and pump the loop; no OS-level mouse. |
| D2 | Pass/fail oracle | **State/behavior assertions** *and* **screenshot baselines** (both). |
| D3 | What is driven | **The real `GtkWinFormsApp` forms** (Form1–Form5, Test1, UserControl11). |
| D4 | Coupling to the forms | **Promote `private` members to `internal` + `InternalsVisibleTo`** so tests reach controls/handlers directly (no reflection). |
| D5 | Screenshot comparison | **Tolerance compare** (absorbs anti-alias/theme noise) **+ an explicit "update baselines" command**. |
| D6 | Environment | **Windows dev machine with a display only** (no headless CI in v1). |
| D7 | Coverage | **All forms up front.** |

These are constraints, not open questions.

## 3. The three hard problems and how each is solved

### 3.1 "Simulating the click" — driving at the WinForms API level (D1)

We do **not** synthesize GDK/OS input. We invoke the same code path a click would run:

- **Buttons:** the sample's `Click` handlers are ordinary methods (e.g. `button17_Click(sender, e)`). Once promoted to `internal` (§3.2) the test calls them directly, or calls a small `UiDriver.Click(button)` helper that raises the control's `Click`/`OnClick` path. We add a **`PerformClick()`** convenience to the harness (not the library) that emits the button's `clicked` route so the library's own signal→event translation runs — closest to a real click while staying in-process.
- **Text entry:** set `TextBox.Text = "…"` then let the harness fire `TextChanged`/`Leave` as needed.
- **Selection/tabs/grids:** set `ComboBox.SelectedIndex`, `TabControl.SelectedTab`, `DataGridView` current cell, etc., through the now-`internal` fields.
- **Menus / dialogs:** invoke the menu item's handler; `CommonDialog`-derived dialogs are driven by pre-seeding their result (the harness stubs `ShowDialog` outcomes rather than blocking on native pickers).

**The modal-loop reality (critical).** `Form.ShowDialog()` calls `self.Run()` → `Gtk.Dialog.Run`, a *nested modal loop*. Interaction therefore cannot happen "after `ShowDialog()` returns" — it must happen **while the loop is running**. The established pattern in the existing tests is `form.Load += (s,e) => { …interact…; form.Close(); }`. We generalise this into a **`UiDriver`** (see §5) that:

- posts a **script of steps** onto the GTK main loop (via `GLib.Idle.Add` / `GLib.Timeout.Add`), pumping between steps with `Application.DoEvents()` so GTK lays out and renders,
- **captures any exception thrown inside a GTK callback**, stores it, closes the form, and **re-throws it on the test thread** after `ShowDialog()` returns. This is essential: an assertion that throws *inside* a GTK signal callback otherwise escapes as `Exception in Gtk# callback delegate` and **crashes the .NET test host natively** (exactly the class of failure seen this session). All assertions run through the driver's guarded step, never raw inside a callback.

### 3.2 Reaching the controls — `private → internal` + `InternalsVisibleTo` (D4)

The sample forms are `public`, but their Designer fields (`private System.Windows.Forms.Button button1;`) and event handlers (`private void button17_Click`) are not accessible. We make them accessible **the same way the library already exposes internals to its own test projects** — an assembly-level `InternalsVisibleTo` keyed to the strong name:

```xml
<!-- same shape Net4x.Gtk.Drawing.csproj already uses for its test projects -->
<ItemGroup>
  <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
    <_Parameter1>GtkWinFormsApp.UITests, PublicKey=$(GtkFormsPublicKey)</_Parameter1>
  </AssemblyAttribute>
</ItemGroup>
```

The `PublicKey=…` value is **`GtkForms.snk`'s public key — copy it verbatim from the existing `InternalsVisibleTo` entries in [Net4x.Gtk.Drawing.csproj](Net4x.Gtk.Drawing/Net4x.Gtk.Drawing.csproj)** (do not re-transcribe it by hand). The sample already signs with `..\GtkForms.snk`, and the UITests project signs with the same key, so the token matches with no new key management.

Then the members must actually be `internal`. Two moving parts:

1. **`private` → `internal` promotion** on the sample's own types (`.cs` **and** `.Designer.cs`, covering Designer fields *and* handler methods). Implemented as a **Roslyn rewrite step in the sample transform** (`SampleTransformPipeline` / `GtkSampleAppProjectsHandler`), scoped to the sample's declared types only — mirroring the library's existing Roslyn rewrites (e.g. the "`OnXxx` must raise its event" rewrite). Leaf app ⇒ widening these members is safe.
2. **`InternalsVisibleTo` injection** into the generated sample csproj, added where the sample project is generated (`GtkSampleAppProjectsHandler` / the csproj overlay that already sets `SignAssembly`).

Both live in the **pipeline**, so they survive regeneration (see §7 — Durability). Until the pipeline step is wired, the same effect can be pinned with a **sample patch** so work isn't blocked.

### 3.3 Replacing the human eye — the two oracles (D2)

- **State oracle (primary, deterministic).** After each step the driver asserts observable state: `label.Text`, `comboBox.SelectedIndex`, `dataGridView.Rows.Count`, `checkBox.Checked`, a `bool handlerFired` flag toggled by the real handler, `form.Visible`, spawned child-form type/title, etc. This is precise and stable and is what actually **gates** the build.
- **Visual oracle (secondary, tolerant).** At defined checkpoints the driver captures the form window to a PNG and compares it to a committed baseline (§6). Meaningful diffs fail; sub-threshold AA/theme noise does not. Screenshots also make failures diagnosable (actual/expected/diff written as artifacts).

## 4. Project layout

```
GtkWinFormsApp.UITests/
  GtkWinFormsApp.UITests.csproj      # net10.0, xUnit, signed with GtkForms.snk
  Infrastructure/
    UiDriver.cs                      # loop pump + guarded step scripting + PerformClick/typing helpers
    FormHarness.cs                   # show a form under a script, collect result, re-throw captured errors
    ScreenshotCapturer.cs            # Gdk.Window -> PNG for a realized top-level
    ImageComparer.cs                 # tolerance compare + baseline update switch
    UiTestBase.cs                    # one-time Application.Init(), theme/culture pinning
  Forms/
    Form1Tests.cs Form2Tests.cs … Form5Tests.cs
    Test1Tests.cs UserControl11Tests.cs
  Baselines/                         # committed golden PNGs, one dir per form
    Form1/… Test1/… …
```

- **`.csproj`:** `net10.0`, `xUnit` + `Microsoft.NET.Test.Sdk` (matches every sibling test project), `SignAssembly`+`GtkForms.snk`, `ProjectReference` to **`GtkWinFormsApp.csproj`** (a `net10.0` test project can consume the `net8.0` sample assembly). Image decode/compare via **`SixLabors.ImageSharp`** (pure-managed, keeps the project off a `-windows` TFM and off the `System.Drawing` shim that is itself under test).
- **Solution:** add to **`Net4x.Gtk.Windows.Forms.sln`** (the product solution), not `PrepareProjects.sln`.
- This project is **hand-authored and NOT generated** — only the sample it references is generated, so the tests are not clobbered by the pipeline.

## 5. The `UiDriver` / `FormHarness` (heart of it)

Shape (illustrative):

```csharp
// Runs 'script' against a freshly-constructed form during its own modal loop,
// pumping GTK between steps, capturing the first exception, then closing.
internal static FormRun Run<TForm>(Func<TForm> create, Action<UiContext> script) where TForm : Form
{
    var form = create();
    Exception? captured = null;
    var ctx = new UiContext(form);

    form.Load += (_, __) =>
    {
        try { script(ctx); }                    // all clicks + assertions happen here...
        catch (Exception ex) { captured = ex; } // ...never allowed to escape into the GTK callback
        finally { form.Close(); }               // always unwind the nested Run() loop
    };

    form.ShowDialog();                           // nested Gtk.Dialog.Run loop
    if (captured != null) throw captured;        // surface on the test thread as a normal xUnit failure
    return new FormRun(ctx);
}
```

`UiContext` exposes:

- `Click(Control c)` / `PerformClick(Button b)` — API-level activation that runs the library's real signal→event path.
- `Type(TextBoxBase t, string s)`, `Select(ComboBox c, int i)`, `Check(CheckBox c, bool v)`, `SelectTab(TabControl t, …)`, grid cell helpers.
- `Pump()` — `Application.DoEvents()` a bounded number of times (lets layout/render settle).
- `Shot(string name)` — capture + compare (or update) a screenshot checkpoint.
- `Assert*` wrappers that record context for good failure messages.

`CommonDialog` interactions are handled by a harness stub that supplies a canned `DialogResult`/selection instead of opening a native picker (native pickers can't be scripted at the API level and would hang the run).

## 6. Screenshots (D5)

- **Capture:** in-process from the form's realized top-level `Gdk.Window` → `Gdk.Pixbuf` → PNG (`ScreenshotCapturer`). In-process capture is deterministic and needs no window focus/foreground (unlike OS `CopyFromScreen`), so it won't fight other windows on the dev desktop. Capture only after `Pump()` so GTK has drawn.
- **Compare (`ImageComparer`, tolerance):** decode baseline+actual with ImageSharp; if dimensions differ → fail. Per-pixel max channel delta `≤ CHANNEL_TOL` (e.g. 12/255); fail only if the fraction of pixels exceeding tolerance `> PIXEL_FRACTION_TOL` (e.g. 0.5%). Both thresholds live in one config constant block.
- **Determinism controls:** `UiTestBase` pins a **fixed GTK theme** (the repo's `theme/`) and a **fixed `CurrentUICulture`** so text/colours don't vary run-to-run; window is set to a **fixed client size** before capture.
- **Update command (required by D5):** when `UPDATE_UI_BASELINES=1` (env var, also surfaced as a `dotnet test -e UPDATE_UI_BASELINES=1` / runsettings parameter), `Shot()` **overwrites** the baseline instead of asserting. Documented workflow: review a real diff → set the flag → re-run → commit the regenerated PNGs. On failure the driver always writes `*.actual.png` and `*.diff.png` next to the run for triage.
- Baselines are **committed** under `Baselines/<Form>/` and are the machine-specific golden set for the dev environment (single-machine assumption per D6; noted in the README so a second machine regenerates rather than "fails visually").

## 7. Durability against pipeline regeneration (§3.2 dependencies)

`GtkWinFormsApp` is regenerated by `SampleTransformPipeline`. Two things must be produced by the pipeline (not hand-edited into the generated tree, which is overwritten):

1. **`private → internal` Roslyn rewrite** — a new step in the sample transform, scoped to the sample's own types, run alongside the existing sample transforms.
2. **`InternalsVisibleTo` csproj injection** — added where the generated sample csproj is emitted (`GtkSampleAppProjectsHandler`), next to the existing `SignAssembly`/key wiring.

Add a `Prepare.Tests` case asserting a regenerated sample (a) has `internal` (not `private`) Designer fields/handlers and (b) carries the `InternalsVisibleTo("GtkWinFormsApp.UITests, PublicKey=…")` attribute — so a future regeneration that drops either fails fast. Until these steps land, a **sample patch** pins the same two changes so the test project is unblocked. The UITests project itself is outside the generated tree and needs no durability handling.

## 8. Coverage (D7 — all forms)

One test class per form; each scripts the interactions the author does by hand:

| Form | Representative scripted interactions | State assertions (examples) |
|---|---|---|
| **Form3** (startup) | app launches into Form3; click the button that opens **Test1** | Test1 instance created & shown; Form3 still alive |
| **Form1** | data-binding buttons; dataset/datatable/print paths; grid population | grid `Rows.Count` > 0; no throw on bind/print route |
| **Form2** | its buttons/inputs; layout toggles | control state after each toggle |
| **Form4 / Form5** | each interactive control exercised | per-control observable state |
| **Test1** (dashboard) | tab switches, picture-box/gif, culture-dependent background, controls panel | active tab; image loaded; culture-correct assets |
| **UserControl11** | hosted-control interactions | embedded control state |

Every class ends with at least one `Shot()` checkpoint per major visual state. "All forms up front" (D7) means the initial deliverable includes a class for **every** form; §10 still sequences the *harness* before the *bulk* so there's something runnable early.

## 9. Environment (D6)

- **Windows desktop with a display.** `Application.Init()` once per test run (xUnit collection fixture) to avoid re-init issues.
- No Xvfb/offscreen/CI backend in v1. The README states the prerequisite (GTK runtime + display) and that headless CI is explicitly out of scope, so the suite isn't wired into an unattended pipeline where it would flake.
- GTK theme + culture pinned (see §6) for reproducibility.

## 10. Implementation sequence

Even though coverage is "all forms," build the **harness first** so failures are debuggable before scaling out:

1. **Scaffold** `GtkWinFormsApp.UITests` (csproj, signing, xUnit, ImageShare ref, add to product sln, `UiTestBase` with one-time `Application.Init`).
2. **Coupling** — land the `private→internal` rewrite + `InternalsVisibleTo` (pipeline step, with a stop-gap sample patch) and the `Prepare.Tests` guard.
3. **`UiDriver`/`FormHarness`** — the guarded modal-loop scripting + exception re-throw; validate on the simplest form (Form3 → open Test1) with **state assertions only**.
4. **Screenshots** — `ScreenshotCapturer` + `ImageComparer` + `UPDATE_UI_BASELINES`; capture the first baselines; prove tolerance absorbs a re-run.
5. **Fan out to all forms** — one class per form (§8), each with state + screenshot checkpoints.
6. **README** — how to run, how to update baselines, why it's Windows-desktop-only.

## 11. Risks & mitigations

| Risk | Mitigation |
|---|---|
| Assertion inside a GTK callback crashes the host natively | Driver captures inside the callback and re-throws on the test thread (§3.1) — never assert raw inside a signal handler. |
| Screenshot flakiness (theme/DPI/font/AA) | Tolerance compare + pinned theme/culture/size + update command (§6); state oracle is the real gate. |
| Coupling to upstream Designer field names (`button1`, `button17_Click`) | Inherent to "drive the real forms"; keep locator constants in one place; `Prepare.Tests` guard catches the exposure mechanism breaking. |
| Pipeline regeneration drops the internal/IVT exposure | Produced by the pipeline + asserted by `Prepare.Tests` (§7). |
| Native pickers/`CommonDialog` block the run | Harness stubs the dialog result (§5). |
| `net10.0` test consuming `net8.0` sample | Supported (newer TFM consumes older); no sample multi-targeting needed. |

## 12. Out of scope (v1)

- Headless/CI execution (Xvfb, broadway, offscreen) — deferred (§9).
- OS-level real input synthesis — explicitly rejected (D1).
- `GtkSystemWinFormsApp11` — this suite targets `GtkWinFormsApp` only.

---

*Related pipeline docs: [AlignSamples-Plan.md](AlignSamples-Plan.md), [PrepareProjects.md](PrepareProjects.md), [Patches.txt](Patches.txt).*

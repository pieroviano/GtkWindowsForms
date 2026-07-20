# Plan: build the `AlignSamples` console application

## Purpose

`AlignSamples` is a console tool (a **post-pipeline**, run after `PrepareProjects` — and normally after `AlignProjects`) that forward-ports the latest gitee upstream changes onto the generated **sample apps** — the counterpart of [AlignProjects](AlignProjects-Plan.md), whose delta scope deliberately *excludes* samples. It covers the two upstream-derived samples:

| Upstream (submodule) | Generated (repo root) |
|---|---|
| `gtksystem-windows-forms/Samples/GTKWinFormsApp` | `GtkWinFormsApp` |
| `gtksystem-windows-forms/Samples/GTKSystemWinFormsApp11` | `GtkSystemWinFormsApp11` |

`AutoClosingMessageBoxApp` is fork-only (no upstream counterpart) and is **out of scope**.

Beyond porting upstream changes, `AlignSamples` has a second duty AlignProjects doesn't: **preserving the samples' globalization**. The original upstream samples are written with Chinese literals; the generated samples are fully culture-aware — every UI string lives in `Properties/GtkWinFormsAppResources.resx` (invariant = English) + `GtkWinFormsAppResources.zh-Hans.resx` (Chinese) and is referenced as `Properties.GtkWinFormsAppResources.<Key>`, with culture-suffixed data files (`TestData1.json` / `TestData1.en.json`). `AlignSamples` must keep that invariant true as upstream evolves: every new Chinese string/file/resource is translated *and* keyed so the samples stay culture-aware. As in AlignProjects, translation is **additive dictionary enrichment + an interactive pause for manual fill** — no AI/LLM component in the tool.

## Existing machinery this builds on

- **The sample transform** already exists: [GtkSampleAppProjectsHandler.PrepareAsync](PrepareProjects/ProjectHandling/GtkSampleAppProjectsHandler.cs) — copy `Samples/<src>` → `<target>`, replace TestData JSONs from `Resources/`, copy the localization resx sources (`Resources/GtkWinFormsAppResources.xml` / `.zh-Hans.xml`), render the templated `GtkWinFormsAppResources.Designer.cs`, rename the csproj, retarget references to `Net4x.*`, then `ProjectTranslation.TranslateFiles(..., "GtkWinFormsAppResources", "samplesTranslations")` — which both translates via the **dedicated sample dictionary** `Resources/samplesTranslations.xml` and maps resource literals onto `Properties.GtkWinFormsAppResources.<Key>` via the `zh-Hans` resx.
- **The alignment driver** exists in AlignProjects (checkpoint, staging worktrees, structural preflight, 3-way cherry-pick merge, conflict pause, report). Per the decisions below it will be **extracted and shared**, not duplicated.
- **Enrichment + apply-guard + pause** (AlignProjects Phase 2) already scan the *whole submodule including Samples*, and `TranslationsHelper`/`Translator` skip blank-English entries so unfilled translations never erase Chinese.

## Core mechanism

Same 3-way merge in **transformed space** as AlignProjects, with the sample transform `fs` in place of the library transform `f`:

```
BASE   = fs(submodule @ AlignSamples' last-aligned checkpoint, or bootstrap base e5227ae)
THEIRS = fs(submodule @ target commit, default HEAD = main, gitee-merged)
OURS   = the live GtkWinFormsApp + GtkSystemWinFormsApp11        # already on disk
DELTA  = transformed-space diff BASE -> THEIRS   (scoped to the two sample dirs; libraries excluded)
result = 3-way merge of DELTA onto OURS + alignment report
```

For the delta to be clean English with resource keys (matching OURS), `fs` must translate **and key** all Chinese in both snapshots — hence the globalization enrichment (Phase 2) runs before `fs`.

## Incremental & repeatable

Identical model to AlignProjects, with an **independent checkpoint** (decision):

- **Checkpoint file:** `AlignSamples/last-aligned.txt`, committed. First run bootstraps from `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`); every later run aligns `checkpoint → new HEAD` and advances the checkpoint on success only.
- Separate from `AlignProjects/last-aligned.txt` so samples can lag or lead the libraries without blocking either tool.
- **Ordering (decision — recommend, don't enforce):** run `AlignProjects` first. `AlignSamples` **warns** (never errors) when its target commit is ahead of AlignProjects' checkpoint, since upstream sample changes may depend on new library APIs; the post-align build is the hard safety net.

## Globalization model (the part AlignProjects doesn't have)

The decisions that shape it:

- **Cultures (decision):** keep the current convention — invariant resx = **English**, one **`zh-Hans`** satellite. No it/fr/de/es for samples.
- **Runtime culture (decision):** OS UI culture only — satellite resolution stays implicit (`zh-Hans` OS → Chinese, everything else → English). No `CurrentUICulture` code is added to the samples; AlignSamples' job is solely to guarantee the satellite resources are complete.

### New Chinese UI strings → resx keys (decision: auto-generate + review at pause)

When upstream adds a Chinese string not yet in the dictionaries:

1. The enrichment scan (regex `[一-鿿]+` over the upstream sample `.cs`/`.Designer.cs`/`.resx` files) finds it.
2. The tool **proposes a key** from context — `Gtk_<FormOrClass>_<sanitized-English-or-ordinal>`, matching the existing hand-authored style (`Gtk_MainForm_Menu_1`, `Gtk_CommonDialogsForm_Node_1`) — and appends, additively:
   - the Chinese value under the proposed key to `Resources/GtkWinFormsAppResources.zh-Hans.xml`;
   - a **blank** English value under the same key to `Resources/GtkWinFormsAppResources.xml`;
   - a blank `<English>` entry to `Resources/samplesTranslations.xml` for non-resource occurrences (comments, inline code).
3. The **manual pause** (same mechanics as AlignProjects' `PauseForManualTranslation`) lists the new keys with file/line context; the operator fixes key names and fills the English values, then continues.
4. `fs` then rewrites the literal to `Properties.GtkWinFormsAppResources.<Key>` exactly as it does for existing keys. The apply-side guard keeps unfilled blanks visible as Chinese instead of erasing them.

Additive guarantees are the same as AlignProjects Phase 2: existing entries never dropped/reordered/reformatted; BOM preserved; filled entries cached forever.

### Culture-specific data files (decision: generalize the pattern)

Generalize today's hardcoded `TestData1.json` / `TestData1.en.json` handling:

- The enrichment scan also covers **non-code sample assets** (`.json`, `.txt`, …) for Chinese content.
- For each such file, `AlignSamples` maintains a translated `<name>.en.<ext>` sibling in `Resources/` (the overlay dir the transform copies from). New or changed Chinese content produces blank-English placeholders in the sibling and is included in the pause listing.
- The transform copies both the original (Chinese) and the `.en` sibling into the generated sample, and ensures the csproj `None`/`CopyToOutputDirectory` items exist for both (generalizing `EnsureJsonNoneItems`).
- Binary assets (images) are exempt — they are copied as-is.

### Culture-completeness check (samples-specific verification)

After the merge, in addition to AlignProjects' residual-Chinese scan, `AlignSamples` verifies the culture-awareness invariant:

- every `Properties.GtkWinFormsAppResources.<Key>` referenced in the generated sample code exists in **both** resx files;
- the invariant and `zh-Hans` resx have **identical key sets**, with no blank invariant values;
- the residual `[一-鿿]` scan runs over both generated sample dirs, whitelisting `*.zh-Hans.resx` and the recognized Chinese-culture data files (e.g. `TestData1.json`).

Failures are reported and exit non-zero, mirroring AlignProjects §3.7.

## Status at a glance

| Piece | State |
|---|---|
| Phase 0 — extract the shared alignment driver out of AlignProjects | Pending |
| Phase 1 — extract & harden the sample transform `fs` (`RunSampleTransform`) | Pending |
| Phase 2 — globalization enrichment (keys, data-file siblings) + pause | Pending |
| Phase 3 — the AlignSamples driver (checkpoint, preflight, merge, conflict pause, report + culture check) | Pending |
| Phase 4 — wire-up & docs | Pending |

## Project setup

- Console project **`AlignSamples/AlignSamples.csproj`** in `PrepareProjects.sln`, `net8.0-windows`, `OutputType=Exe`, mirroring `AlignProjects.csproj`.
- **References:** `Prepare.Common` (and the shared driver library, wherever Phase 0 lands it), `XmlDocSync` if needed by `fs`.
- **Config (env vars):** `ALIGNSAMPLES_BASE_COMMIT` (default: the checkpoint, else `e5227ae`), `ALIGNSAMPLES_TARGET_COMMIT` (default `main` HEAD), `ALIGN_SOURCE_DIR` (shared; default the submodule). Testing seams analogous to AlignProjects: `ALIGNSAMPLES_BASE_TREE`/`ALIGNSAMPLES_THEIRS_TREE` to run the merge against pre-produced trees.

## Implementation phases

### Phase 0 — Extract the shared alignment driver (decision: extract, not duplicate)

Factor AlignProjects' Phase-3 driver into a reusable component (in `Prepare.Common` or a new `Align.Common`), parameterized by:

- **scope** — the set of generated dirs to merge (`Net4x.Gtk.Windows.Forms` + `Net4x.Gtk.Drawing` for AlignProjects; `GtkWinFormsApp` + `GtkSystemWinFormsApp11` for AlignSamples);
- **transform delegate** — `RunTransform` vs `RunSampleTransform`, both `(repoRoot) → generated dirs + provenance manifest`;
- **checkpoint file path**, temp-branch names (`align-samples/base` / `align-samples/theirs`), and report labels.

`AlignProjects` is refactored to call the shared driver (behavior-preserving — its existing tests must stay green); `AlignSamples` calls it with the sample parameters. This is the same no-drift argument that settled "extract `f`" in AlignProjects.

**Tests (Prepare.Tests):** driver parameterization — scope filtering (files outside the scope are never touched by the merge), checkpoint isolation (each tool reads/advances only its own file).

### Phase 1 — Extract & harden `fs` (`RunSampleTransform`)

Factor the body of `GtkSampleAppProjectsHandler.PrepareAsync` into a callable `RunSampleTransform(repoRoot, sourceProjectName, targetProjectName)`:

- **Parameterize the repo root** (option B, as settled for AlignProjects §3.1): all `RepoPaths.At(...)` lookups (`Samples/<src>`, `Resources/`, the `Net4x.*` reference targets) resolve against `repoRoot`, so the transform reads a source worktree and writes into a private staging root — never the live samples or submodule.
- **Skip non-alignment steps** when running under AlignSamples: `EnsureLatestGtkSharpAsync` (network/version churn would pollute the delta) — the live csproj keeps its pinned versions; version normalization is OURS-side state.
- **Determinism:** stable culture-invariant ordering wherever files/entries are enumerated; two-run byte-identical self-check, mirroring AlignProjects.
- **Provenance manifest:** record `generated-file → upstream-source-file` for every copy/rename/translate step so conflict listings can always name the raw `Samples/...` original. (Simpler than the library case — no splits — but the hardcoded per-file fix-ups in `PrepareAsync`, e.g. the `Form1.cs`/`Form3.cs` string replacements, must be folded into `samplesTranslations.xml` entries so they're dictionary-driven and manifest-visible, not code-side specials.)

**Tests (Prepare.Tests):** `RunSampleTransform` against a fixture tree — repoRoot isolation (nothing outside the staging root is written), determinism (two runs byte-identical), manifest correctness for a renamed + translated file.

### Phase 2 — Globalization enrichment + pause

Implements the [globalization model](#globalization-model-the-part-alignprojects-doesnt-have):

1. **`EnrichSampleResources(repoRoot, sampleSourceDirs)`** — the string scan + key proposal + additive append to `GtkWinFormsAppResources.xml`/`.zh-Hans.xml` and `samplesTranslations.xml`.
2. **`EnrichSampleDataFiles(repoRoot, sampleSourceDirs)`** — the Chinese-in-assets scan + `.en` sibling maintenance in `Resources/`.
3. **Pause** — one combined `PauseForManualTranslation`-style stop listing new keys (with proposed names + context) and new/changed data-file content; ENTER to continue; auto-continue when non-interactive. Blanks left unfilled remain Chinese at apply time (guard already in place).

**Tests (Prepare.Tests):** key generation (style, sanitization, collision handling → ordinal suffixes), additivity (existing entries and BOM/format untouched), data-file sibling creation and gap-filling, blank-skip behavior for both resx mapping and data siblings.

### Phase 3 — The AlignSamples driver

Instantiate the shared driver with the sample parameters. Flow (all semantics inherited from AlignProjects §3.0–§3.9 unless noted):

1. Resolve **base** (checkpoint, else `e5227ae`) and **target** (submodule `HEAD`); **warn** if target is ahead of AlignProjects' checkpoint (ordering decision).
2. Run Phase-2 enrichment → pause → commit the enriched/filled dictionaries.
3. Produce `BASE`/`THEIRS` via `RunSampleTransform` in two staging roots (git worktrees + `Resources` junction), scoped to the two sample dirs.
4. **Structural preflight** — `BASE` vs `OURS` must differ only inside files, not in layout; abort with a report otherwise.
5. **3-way merge** via throwaway commits + cherry-pick, scoped to `GtkWinFormsApp/` + `GtkSystemWinFormsApp11/`.
6. **Conflict pause** — full paths of the conflicted generated file, the raw `Samples/...` source (from the manifest), and the `build/theirs`/`build/base` aids; resolve → `git add` → ENTER; re-check for remaining unmerged paths.
7. **Report** — files added/modified/deleted, conflicts, residual-Chinese scan **plus the culture-completeness check** (key parity, no blank invariant values, referenced keys exist).
8. **Checkpoint advance + commit** on success; restore from git on failure; cleanup worktrees/staging/temp branches.

Exit codes mirror AlignProjects: `0` clean · `1` clean but residual Chinese / culture-check failures · `2` structural-drift abort · `3` conflicts unresolved.

**Tests (Prepare.Tests):** structural preflight on fixture trees (added/removed path → abort), merge scope (library dirs untouched), culture-completeness check (missing zh-Hans key, blank invariant value, orphan code reference each → failure), checkpoint not advanced on conflict.

### Phase 4 — Wire-up & docs

- Run order documented: `PrepareProjects` → `AlignProjects` → `AlignSamples` (manual or scripted; not chained automatically).
- Document in [PrepareProjects.md](PrepareProjects.md) (or a sibling section) alongside AlignProjects.
- `AlignSamples/last-aligned.txt` ships pointing at `e5227ae`.

## How to run (once built)

Prerequisites: PrepareProjects has run (samples generated = `OURS` committed, submodule on gitee-merged `HEAD`), AlignProjects ideally already aligned, clean working tree.

```sh
dotnet run --project AlignSamples/AlignSamples.csproj
```

Interactive runs hit up to two pauses (translation/keys, then merge conflicts). Non-interactive runs skip the translation pause and exit non-zero on conflicts. Re-running after a clean alignment is a no-op.

## Verification

- [ ] Phase 0: AlignProjects still passes its existing tests after the driver extraction (behavior-preserving refactor).
- [ ] Phase 1: `RunSampleTransform` is repoRoot-isolated, deterministic (two-run byte-identical), and emits an exact provenance manifest; the `Form1.cs`/`Form3.cs` code-side replacements are migrated into `samplesTranslations.xml`.
- [ ] Phase 2: new Chinese strings get proposed keys + blank English additively; new/changed Chinese data content gets `.en` sibling placeholders; the pause lists everything with context; unfilled blanks leave Chinese in place.
- [ ] Phase 3: preflight aborts on layout drift; merge touches only the two sample dirs; conflict pause prints generated + raw upstream full paths; checkpoint advances only on success.
- [ ] Culture invariant holds after alignment: resx key parity (invariant vs zh-Hans), no blank invariant values, every referenced `GtkWinFormsAppResources` key present, no residual `[一-鿿]` outside whitelisted files.
- [ ] `dotnet build Net4x.Gtk.Windows.Forms.sln` (Debug + Release) clean; both samples start; UI shows English on a non-Chinese OS culture and Chinese under `zh-Hans`.
- [ ] Re-run right after a clean alignment is a no-op; advancing the submodule and re-running applies only the new delta.

## Risks

- **Driver-extraction regressions in AlignProjects** — mitigated by Phase 0 being behavior-preserving with its existing tests kept green before AlignSamples work starts.
- **Key-proposal collisions / poor names** — mitigated by ordinal-suffix collision handling + the review pause (names are operator-approved before they ossify into `OURS`).
- **Upstream restructures a sample** (new forms, renamed files) — layout changes surface as add/delete in the delta (fine) or as preflight drift if `fs` itself drifts; the preflight aborts before damage.
- **Non-deterministic `fs`** — same mitigation as AlignProjects: neutralize + two-run self-check that fails loud.
- **Sample depends on library APIs not yet aligned** — mitigated by the ordering warning + the post-align build gate.
- **`EnsureLatestGtkSharpAsync` in the transform** — network-dependent version bumps would pollute the delta; excluded from `fs` under AlignSamples (kept in PrepareProjects' use).
- **Data-file false positives** (Chinese in binary-ish or generated assets) — scan restricted to known text extensions; anything else is report-only.

## Decisions taken (clarified with the owner before writing this plan)

- **Scope** — `GtkWinFormsApp` + `GtkSystemWinFormsApp11`; `AutoClosingMessageBoxApp` excluded.
- **Code reuse** — extract the alignment driver into a shared library used by both tools; AlignSamples is a thin console app.
- **Checkpoint** — separate `AlignSamples/last-aligned.txt`, bootstrap `e5227ae`.
- **Cultures** — keep invariant-English + `zh-Hans` only.
- **New keys** — auto-generated (`Gtk_<Form>_...` style) with blank English, reviewed/renamed/filled at the pause.
- **Data files** — generalize the `TestData1` pattern: Chinese-bearing text assets get maintained `.en` siblings via additive enrichment.
- **Runtime culture** — OS UI culture only; no culture-selection code added to samples.
- **Ordering vs AlignProjects** — recommended order documented, warning (not error) when samples align ahead of the libraries.
- **Doc location** — this file, `AlignSamples-Plan.md`, at the repo root.
- **Tests** — per-phase unit tests in `Prepare.Tests` are completion criteria.

## Open decisions

None blocking. Implementation-time details remain: exact home of the shared driver (`Prepare.Common` vs a new `Align.Common`), the key-sanitization rules (max length, ordinal format), and the text-asset extension whitelist for the data-file scan.

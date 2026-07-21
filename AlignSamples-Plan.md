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
| Phase 0 — extract the shared alignment driver out of AlignProjects | **Done** — lives in `Align.Common` |
| Phase 1 — extract & harden the sample transform `fs` (`RunSampleTransform`) | **Done** — `SampleTransformPipeline` |
| Phase 2 — globalization enrichment (keys, data-file siblings) + pause | **Done** — `SampleGlobalizationEnricher` |
| Phase 3 — the AlignSamples driver (checkpoint, preflight, merge, conflict pause, report + culture check) | **Done** — `AlignSamples` |
| Phase 4 — wire-up & docs | **Done** |

## Project setup

- Console project **`AlignSamples/AlignSamples.csproj`** in `PrepareProjects.sln`, `net8.0-windows`, `OutputType=Exe`, mirroring `AlignProjects.csproj`.
- **References:** `Align.Common` (the shared driver, Phase 0), `Prepare.Common`, `XmlDocSync` if needed by `fs`.
- **Config (env vars):** `ALIGNSAMPLES_BASE_COMMIT` (default: the checkpoint, else `e5227ae`), `ALIGNSAMPLES_TARGET_COMMIT` (default `main` HEAD), `ALIGN_SOURCE_DIR` (shared; default the submodule). Testing seams analogous to AlignProjects: `ALIGNSAMPLES_BASE_TREE`/`ALIGNSAMPLES_THEIRS_TREE` to run the merge against pre-produced trees.

## Implementation phases

### Phase 0 — Extract the shared alignment driver (decision: extract, not duplicate) — **Done**

AlignProjects' Phase-3 driver now lives in the new **`Align.Common`** class library (`net8.0`, `IsPackable=false`, in `PrepareProjects.sln`) — not in `Prepare.Common`, which is a shipped NuGet package and multi-targets `netstandard2.0`:

| Type | Role |
|---|---|
| [AlignmentOptions](Align.Common/AlignmentOptions.cs) | Everything tool-specific: `Scope`, `Transform`, `CheckpointFile`, `ToolName`, `SubmoduleName`, `BranchPrefix`, `ConflictLogName`, `ProvenanceFallback`, `PostMergeChecks`. Derives `ToolDirectory` / `BuildDirectory` / `SelfCheckDirectory`. |
| [AlignmentDriver](Align.Common/AlignmentDriver.cs) | `RunAsync` (produce trees → align → retain/clean `build/`) and `AlignFromTrees` (preflight §3.4 → merge §3.5 → conflict pause §3.6 → residual-Chinese + post-merge report §3.7 → checkpoint §3.8). |
| [TransformedTreeProducer](Align.Common/TransformedTreeProducer.cs) | Staging-root mirror plan, `ProduceOneAsync`/`ProduceTransformedTreesAsync`, and the §3.1 determinism self-check. |
| [ConflictResolutionSession](Align.Common/ConflictResolutionSession.cs) | The auto-merge pass, `conflicts.log`, fork-copy index and the single operator pause. |
| [ManualTranslationPause](Align.Common/ManualTranslationPause.cs) | The translation pause, with an `extraDetails` hook for Phase 2's proposed keys / data-file placeholders. |

Parameterization landed in the two `Prepare.Common` types the driver leans on: `DeltaMerge` takes a `branchPrefix` (default `align`, so `align-samples/base` / `align-samples/theirs` never collide with a pending libraries run), and `AlignmentCheckpoint.Write` takes a `toolName` for the header comment. Both defaults preserve existing behavior.

[AlignProjects/Program.cs](AlignProjects/Program.cs) is now just configuration: scope, the `f` delegate (`RepoPaths.Root` swap + `TransformPipeline.RunTransform`), the `Translations.xml`/resx enrichment + pause, and the `*.Files.xml` provenance fallback. `AlignSamples` will instantiate the same driver with the sample parameters.

**Tests (Prepare.Tests):** [AlignmentDriverTests](Prepare.Tests/AlignmentDriverTests.cs) — scope filtering (an out-of-scope dir with a real upstream change is left untouched), checkpoint isolation (the samples run advances only its own file; the libraries checkpoint is unchanged), post-merge-check failures → exit 1 without blocking the merge, and mirror-plan exclusions (tool dir, scope dirs, submodule; `Resources` copied not junctioned). Full suite green (153 tests).

### Phase 1 — Extract & harden `fs` (`RunSampleTransform`) — **Done**

`fs` is [SampleTransformPipeline](PrepareProjects/SampleTransformPipeline.cs) — the sample-side counterpart of `TransformPipeline` (`f`):

- **`RunSampleTransformAsync(repoRoot, source, target, options)`** runs one sample; **`RunAllSampleTransformsAsync(repoRoot)`** runs both (the order in `SampleTransformPipeline.Samples` is fixed) and writes `provenance.tsv` at the root. That second one is what AlignSamples will hand the driver as its `Transform` delegate.
- **Repo root parameterized** (§3.1 option B): the body of `GtkSampleAppProjectsHandler.PrepareAsync` was left resolving everything through `RepoPaths`, and `fs` points `RepoPaths.Root` at the given root for the duration, restoring it after — so a staging root is read and written, never the live samples or submodule. A test asserts the root is restored and that nothing lands in the real repo.
- **Non-alignment steps skipped**: `SampleTransformOptions.EnsureLatestGtkSharp` gates the nuget.org GtkSharp bump (on for PrepareProjects, off for AlignSamples). Version normalization stays OURS-side state.
- **Determinism**: ordinal ordering added where the transform enumerates the filesystem (`ProjectUtility.CopyDirectory`, `Translator.ReplaceStringsInFiles`) and where the manifest is written (`ProvenanceManifest.Save`). A test runs `fs` twice over identical fixture roots and compares every produced file byte for byte.
- **Provenance manifest**: `ProvenanceManifest` entries are recorded at each copy / overlay / rename, and dropped for files the transform deletes (`工程说明.txt`, the linked `System.*.cs`). **Both sides are repo-relative** — the staging root is deleted before the operator ever reads the manifest — and `AlignmentDriver` rebases relative sources onto the live repo, where the file actually opens. `TransformedTreeProducer` now carries `provenance.tsv` out of the staging root into the captured tree.
- **Per-sample designer regeneration**: `ResGenService.RegenerateSampleDesigner` split out of `RegenerateDesignersFromResources`, so `fs` is complete per sample instead of depending on the library pipeline.
- **Hardcoded fix-ups migrated**: the `Form1.cs` / `Form3.cs` string replacements are now three `Resources/samplesTranslations.xml` entries, keyed *with* their `//` comment prefix (`//打印` → `// Print`) so the translator's "prefix a space / lowercase after punctuation" rule doesn't fire and the output is byte-identical to the old code path. Those three comments are the only matches anywhere under `Samples/`, so generated output is unchanged.

**Tests (Prepare.Tests):** [SampleTransformPipelineTests](Prepare.Tests/SampleTransformPipelineTests.cs) — repoRoot isolation, two-run byte-identical determinism, manifest correctness (renamed csproj, translated `.cs`, renamed asset, overlaid resx/json, deleted file absent, every path relative), and that the migrated dictionary entries render exactly as the old hardcodes did. `Prepare.Tests` now targets `net8.0-windows` so it can reference `PrepareProjects` (where `f`/`fs` live).

### Phase 2 — Globalization enrichment + pause — **Done**

Implemented in [SampleGlobalizationEnricher](Prepare.Common/Languages/SampleGlobalizationEnricher.cs) (`Enrich` = both halves; `EnrichSampleResources` / `EnrichSampleDataFiles` individually):

1. **Two channels, decided by where the Chinese sits.** A Chinese run inside a double-quoted literal (or a `.resx` `<value>`) is a UI string → the **whole literal** becomes a proposed key, because that is exactly what the transform maps onto `Properties.GtkWinFormsAppResources.<Key>`. Chinese anywhere else — comments, inline code — becomes a blank `samplesTranslations.xml` entry. Resource literals are never duplicated into the dictionary, so a filled dictionary entry can't pre-empt the resource mapping (which runs first in `Translator.TranslateFile`).
2. **Key proposal** — `Gtk_<Form>_<hint>`, the form taken from the file name (`Form1.Designer.cs` → `Form1`) and the hint from the literal's ASCII content (`datatable数据源` → `Gtk_MainForm_datatable`), falling back to a per-form ordinal (`Gtk_MainForm_1`) exactly like the hand-authored `Gtk_MainForm_Menu_1`. Collisions with existing *or* freshly-proposed keys get `_2`, `_3`, …
3. **Data files** — the hardcoded `TestData1.json` / `TestData1.en.json` pair generalized: any `.json`/`.txt`/`.csv`/`.md` asset containing Chinese gets both a Chinese original and an `<name>.en.<ext>` sibling in `Resources/`. New siblings are seeded from the Chinese original rather than blanked — the file must stay structurally valid (the sample loads it), and "unfilled ⇒ still Chinese" is the same guarantee the dictionary makes. **Existing siblings are never overwritten**; one that still contains Chinese is reported as a gap. Files the transform deletes (`工程说明.txt`) and binary assets are excluded.
4. **Pause** — `ManualTranslationPause.RunForSamples(result, resourcesDir)` prints one combined listing: each proposed key with its zh-Hans value and `file:line`, the new blank dictionary entries, and the data-file siblings; ENTER to continue, auto-continue when non-interactive.

Additive contract, same as AlignProjects Phase 2: entries are appended as **text** just before `</root>` / `</Translations>` (an `XDocument` round-trip would reformat all 150 entries and bury the change), existing entries are untouched and in order, BOM and line endings are preserved, filled translations are cached forever, and a second run over the same input adds nothing.

Smoke-checked against the live submodule (into a throwaway overlay copy): 63 proposed keys, 48 new dictionary entries, 0 spurious data files.

**Tests (Prepare.Tests):** [SampleGlobalizationEnricherTests](Prepare.Tests/SampleGlobalizationEnricherTests.cs) — key style and ASCII-hint/ordinal fallback, collision suffixes, already-keyed Chinese not re-proposed, comment-vs-literal channel split, `.resx` values treated as UI strings, additivity (prefix-identical file + BOM intact + filled entries kept), idempotence, data-file sibling creation, never-overwrite + gap reporting, transform-deleted files skipped, and the combined pause listing.

### Phase 3 — The AlignSamples driver — **Done**

[AlignSamples/Program.cs](AlignSamples/Program.cs) — a thin console app (`net8.0-windows`, in `PrepareProjects.sln`) that instantiates the Phase-0 driver with the sample parameters: scope = the two samples, transform = `SampleTransformPipeline.RunAllSampleTransformsAsync`, checkpoint = `AlignSamples/last-aligned.txt`, branch prefix `align-samples`, conflict log `conflicts-samples.log`, post-merge check = the culture verifier. Flow:

1. **Base/target** from its own checkpoint (bootstrap `e5227ae`) and `main`; env overrides `ALIGNSAMPLES_BASE_COMMIT` / `_TARGET_COMMIT` / `ALIGN_SOURCE_DIR`, test seams `ALIGNSAMPLES_BASE_TREE` / `_THEIRS_TREE` / `_PROVENANCE`, self-check `ALIGNSAMPLES_SELFCHECK`.
2. **Ordering warning** when AlignProjects' checkpoint differs from the samples' base — never an error.
3. **Phase-2 enrichment → pause → commit.** The enriched overlay files are committed by pathspec (`Resources/GtkWinFormsAppResources*.xml`, `samplesTranslations.xml`, any data siblings) — the 3-way merge needs a clean tree, and hand-filled translations must not ride along in the cherry-pick.
4. **BASE/THEIRS** produced by running `fs` in two staging worktrees, **5. merge**, **6. conflict pause**, **8. checkpoint advance** — all from the shared driver.
7. **Report** — the driver's residual-Chinese scan plus [SampleCultureVerifier](Prepare.Common/Languages/SampleCultureVerifier.cs): identical key sets between the invariant and `zh-Hans` resources, no blank invariant values, every `Properties.GtkWinFormsAppResources.<Key>` referenced in code present in both, and no residual `[一-鿿]` outside the files meant to hold it (the `zh-Hans` resx and any asset with an `.en.<ext>` sibling — generalizing the hardcoded `TestData1.json` exemption).

Exit codes: `0` clean · `1` clean but residual Chinese / culture-check failures · `2` structural-drift abort · `3` conflicts unresolved.

**Deviation from §3.4 (measured, deliberate):** the structural preflight **reports** rather than aborts. `AlignmentOptions.AbortOnStructuralDrift` implements the abort (exit 2, tested), but AlignSamples leaves it off: comparing `fs(e5227ae)` against the live samples shows the residual difference is five files upstream ships and this fork does not keep (`GTKWinFormsApp.xml`, `GTKSystemWinFormsApp11.xml`, `Properties/launchSettings.json`, both `PublishProfiles/*`). That is the same "patch deletions / committed settings" category AlignProjects already treats as informational, so aborting would fail every run. The merge plus conflict pause remain the real safety net.

Measuring that also removed a real source of delta noise: `fs` no longer copies upstream's `.vs/` directory (suo files, content indexes, caches — 40 files of IDE state that would have entered the delta as upstream "changes").

**Verified end to end on real data:** `ALIGNSAMPLES_SELFCHECK` ran `fs` twice against the live submodule in isolated staging worktrees — byte-identical both at `main` and at the bootstrap base, with no leftover worktrees.

**Tests (Prepare.Tests):** [SampleCultureVerifierTests](Prepare.Tests/SampleCultureVerifierTests.cs) — a culture-aware sample passes; missing `zh-Hans` key, key only in `zh-Hans`, blank invariant value, orphan code reference, residual Chinese and a missing resource each fail, while the `zh-Hans` resx and `TestData1.json`/`.en.json` pair are whitelisted. [AlignmentDriverTests](Prepare.Tests/AlignmentDriverTests.cs) adds structural-drift abort (exit 2, checkpoint untouched, nothing merged) and unresolved conflicts (exit 3, checkpoint untouched, markers + conflict log left for the operator).

### Phase 4 — Wire-up & docs — **Done**

- **[AlignSamples.md](AlignSamples.md)** — the as-built document (why a second align tool, core mechanism, the globalization model, checkpoint, how to run, exit codes, env overrides, stages, `fs`, the structural-preflight decision, dependencies), mirroring [AlignProjects.md](AlignProjects.md).
- **[PrepareProjects.md](PrepareProjects.md)** — an "AlignSamples (post-pipeline, samples)" section alongside the AlignProjects one, plus a **Run order** section: `PrepareProjects` → `AlignProjects` → `AlignSamples`, invoked by hand, never chained, with the libraries-before-samples rationale.
- **[AlignProjects.md](AlignProjects.md)** — refreshed for the Phase-0 extraction: the driver now lives in `Align.Common` and `Program.cs` is configuration only; renamed helpers (`ManualTranslationPause`, `TransformedTreeProducer`, `ConflictResolutionSession`); `ALIGN_SELFCHECK` documented; the plan-vs-implementation row for §3.4 updated to name `AbortOnStructuralDrift`; cross-link to AlignSamples.
- **[CLAUDE.md](CLAUDE.md)** — both post-pipelines described together, with the run order and the shared-driver/independent-checkpoint facts.
- `AlignSamples/last-aligned.txt` ships pointing at `e5227ae`; `/AlignSamples/build/` and `/AlignSamples/build-selfcheck/` are gitignored; the project is in `PrepareProjects.sln`, and both new docs are in its Documentation folder.

## How to run (once built)

Prerequisites: PrepareProjects has run (samples generated = `OURS` committed, submodule on gitee-merged `HEAD`), AlignProjects ideally already aligned, clean working tree.

```sh
dotnet run --project AlignSamples/AlignSamples.csproj
```

Interactive runs hit up to two pauses (translation/keys, then merge conflicts). Non-interactive runs skip the translation pause and exit non-zero on conflicts. Re-running after a clean alignment is a no-op.

## Verification

- [x] Phase 0: AlignProjects still passes its existing tests after the driver extraction (behavior-preserving refactor).
- [x] Phase 1: `RunSampleTransform` is repoRoot-isolated, deterministic (two-run byte-identical), and emits an exact provenance manifest; the `Form1.cs`/`Form3.cs` code-side replacements are migrated into `samplesTranslations.xml`.
- [x] Phase 2: new Chinese strings get proposed keys + blank English additively; new/changed Chinese data content gets `.en` sibling placeholders; the pause lists everything with context; unfilled blanks leave Chinese in place.
- [x] Phase 3: preflight reports layout drift (abort available via `AbortOnStructuralDrift`; measured as unusable for the samples — see above); merge touches only the two sample dirs; conflict pause prints generated + raw upstream full paths; checkpoint advances only on success.
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

None blocking. The shared driver's home is settled (`Align.Common`, Phase 0). Both remaining implementation details are settled in Phase 2: key sanitization (ASCII-only hint, 40-char cap, `_N` collision suffixes) and the text-asset whitelist (`.json`, `.txt`, `.csv`, `.md`).

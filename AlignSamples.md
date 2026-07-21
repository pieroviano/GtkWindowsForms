# AlignSamples

`AlignSamples` is the **sample-side post-pipeline**: a console tool run *after* [PrepareProjects](PrepareProjects.md) (and normally after [AlignProjects](AlignProjects.md)) that forward-ports newer gitee upstream changes onto the generated sample apps — `GtkWinFormsApp` and `GtkSystemWinFormsApp11` — incrementally, preserving the fork's patches and manual edits, **and keeping the samples fully culture-aware**.

It is the counterpart of AlignProjects, whose delta scope deliberately excludes the samples. `AutoClosingMessageBoxApp` is fork-only (no upstream counterpart) and out of scope.

This document describes the tool **as built** ([AlignSamples/Program.cs](AlignSamples/Program.cs) + `Align.Common/` + `Prepare.Common/Alignment|Languages/`). The design rationale, decisions, and phase status live in [AlignSamples-Plan.md](AlignSamples-Plan.md).

## Why a second align tool

| | AlignProjects | AlignSamples |
|---|---|---|
| Scope | `Net4x.Gtk.Windows.Forms` + `Net4x.Gtk.Drawing` | `GtkWinFormsApp` + `GtkSystemWinFormsApp11` |
| Transform | `f` — `TransformPipeline.RunTransform` | `fs` — `SampleTransformPipeline.RunAllSampleTransformsAsync` |
| Checkpoint | `AlignProjects/last-aligned.txt` | `AlignSamples/last-aligned.txt` (independent) |
| Extra duty | — | Globalization: new Chinese is **keyed and translated**, and the culture invariant is verified after the merge |

The checkpoints are separate on purpose: the samples may lag or lead the libraries without either tool blocking the other.

## Core mechanism

Identical to AlignProjects, with `fs` in place of `f`:

```
BASE   = fs(submodule @ last-aligned checkpoint)    # bootstrap: e5227ae
THEIRS = fs(submodule @ target commit)              # default: the tracking branch (main, gitee-merged)
OURS   = the live GtkWinFormsApp + GtkSystemWinFormsApp11
DELTA  = BASE -> THEIRS, scoped to the two sample dirs
result = 3-way merge of DELTA onto OURS
```

The whole driver — staging roots, structural preflight, cherry-pick merge, conflict pause, report, checkpoint — is the **shared** `Align.Common.AlignmentDriver`, the same code AlignProjects runs. Only the [options](Align.Common/AlignmentOptions.cs) differ (scope, transform delegate, checkpoint path, branch prefix `align-samples`, conflict log `conflicts-samples.log`, the culture check).

For the delta to be clean English-with-resource-keys on both sides, `fs` must translate **and key** all Chinese in both snapshots — which is why the globalization enrichment runs *before* the transform.

## Globalization

The generated samples are culture-aware: every UI string lives in `Properties/GtkWinFormsAppResources.resx` (invariant = English) plus `GtkWinFormsAppResources.zh-Hans.resx` (Chinese), referenced as `Properties.GtkWinFormsAppResources.<Key>`, with culture-suffixed data files (`TestData1.json` / `TestData1.en.json`). Cultures are **invariant-English + `zh-Hans` only**, resolved from the OS UI culture — the samples contain no culture-selection code.

[SampleGlobalizationEnricher](Prepare.Common/Languages/SampleGlobalizationEnricher.cs) keeps that true as upstream evolves. It splits new Chinese by where it sits:

| Where the Chinese is | What happens |
|---|---|
| Inside a string literal, or a `.resx` `<value>` | A UI string: the **whole literal** gets a proposed `Gtk_<Form>_<hint>` key — Chinese value appended to `GtkWinFormsAppResources.zh-Hans.xml`, **blank** English appended to `GtkWinFormsAppResources.xml` |
| Anywhere else (comments, inline code) | A blank `<English></English>` entry appended to `samplesTranslations.xml` |
| Inside a text asset (`.json`, `.txt`, `.csv`, `.md`) | The overlay gets both the Chinese original and an `<name>.en.<ext>` sibling to translate |

The key hint comes from the literal's ASCII content (`datatable数据源` → `Gtk_MainForm_datatable`), falling back to a per-form ordinal (`Gtk_MainForm_1`) — matching the hand-authored style; collisions get `_2`, `_3`, … Proposed names are **suggestions**: the operator renames them at the pause before they ossify into `OURS`.

Every write is strictly additive — entries are appended as text just before `</root>` / `</Translations>` (an XML round-trip would reformat all 150 entries and bury the change), existing entries keep their order and formatting, the BOM is preserved, and filled translations are cached forever. Unfilled blanks leave the Chinese **in place** at apply time rather than erasing it (`TranslationsHelper`/`Translator` skip blank-English entries) — which is what stage 8 then reports.

## Incremental & repeatable — the checkpoint

- **First run:** base = `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`), target = the tracking branch.
- **Every later run:** base = the previous run's target, from [AlignSamples/last-aligned.txt](AlignSamples/last-aligned.txt).

Advanced **only on a successful alignment**; an aborted run re-targets the same delta next time. Re-running right after a clean alignment is a no-op.

## How to run

Prerequisites: PrepareProjects has run (the samples are generated and committed, the submodule sits on the gitee-merged tracking branch), a clean working tree on a non-default branch, `git` on `PATH`. Windows only (`net8.0-windows`; staging roots use `mklink /J`).

```sh
dotnet run --project AlignSamples/AlignSamples.csproj
```

Run it **interactively** — both pauses (translation/keys, then merge conflicts) need a real console. With stdin redirected the translation pause is skipped and conflicts abort the run.

**Recommended order:** `PrepareProjects` → `AlignProjects` → `AlignSamples`. Upstream sample changes may depend on library APIs that AlignProjects has not ported yet, so AlignSamples **warns** when the two checkpoints differ — it never refuses to run. Building the solution afterwards is the hard safety net. Nothing is chained automatically; each tool is invoked by hand.

### Exit codes

| Code | Meaning |
|---|---|
| `0` | Clean alignment; checkpoint advanced |
| `1` | Aligned, but residual untranslated Chinese and/or culture-check failures remain; checkpoint advanced |
| `2` | Structural-drift abort (only when `AbortOnStructuralDrift` is enabled — it is not, see [Structural preflight](#structural-preflight)) |
| `3` | Conflicts left unresolved (or a non-interactive run hit conflicts); checkpoint **not** advanced |
| `4` | `ALIGNSAMPLES_SELFCHECK` found the transform non-deterministic |
| `5` | The target ref could not be resolved in the submodule |

### Environment overrides

| Variable | Default | Purpose |
|---|---|---|
| `ALIGN_REPO_ROOT` | 4 levels up from the exe | Repo root (test seam) |
| `ALIGNSAMPLES_BASE_COMMIT` | checkpoint, else `e5227ae` | Delta base |
| `ALIGNSAMPLES_TARGET_COMMIT` | `SourceRepositorySync.DefaultTrackingBranch` (`main`) | Delta target; resolved to a commit id via `<ref>` → `origin/<ref>` → `gitee/<ref>` (exit `5` if none matches) |
| `ALIGN_SOURCE_DIR` | `gtksystem-windows-forms/` | Tree the enrichment scan reads (shared with AlignProjects) |
| `ALIGNSAMPLES_BASE_TREE` / `_THEIRS_TREE` | — | Supply pre-produced transformed trees and **skip running the transform** |
| `ALIGNSAMPLES_PROVENANCE` | `<theirsTree>/provenance.tsv` | Generated→source manifest for the conflict report |
| `ALIGNSAMPLES_SELFCHECK` | — | `1` (or a commit) → transform that commit **twice** and compare byte for byte; exits without aligning |

## Stages (order of `Program.Main`)

1. **Resolve configuration** — repo root from the exe location (CWD-independent), then base / target / source dir from the checkpoint and env overrides; all are echoed at startup, followed by the ordering warning if AlignProjects' checkpoint differs.

2. **Enrich the sample globalization** — `SampleGlobalizationEnricher.Enrich` scans the two upstream `Samples/<name>` dirs and appends proposed keys, blank dictionary entries and `.en` data siblings as described above. Idempotent: a second run over the same input adds nothing.

3. **PAUSE — manual translation & key review** (`ManualTranslationPause.RunForSamples`) — lists every proposed key with its `zh-Hans` value and `file:line`, the new blank dictionary entries, and the data files awaiting translation; ENTER continues. Skipped when nothing is new or stdin is redirected.

4. **Commit the enriched overlay** — `Resources/GtkWinFormsAppResources*.xml`, `samplesTranslations.xml` and any touched data siblings are committed **by pathspec** (nothing else is staged). The 3-way merge needs a clean working tree, and hand-filled translations must not ride along inside the cherry-pick.

5. **Produce BASE and THEIRS** (`TransformedTreeProducer`) — trees land in `AlignSamples/build/{base,theirs}` (gitignored, cleared on entry); the staging roots live under `%TEMP%\alignsamples-staging-<guid>\` and are deleted afterwards. Per commit: a detached `git worktree` of the submodule at that commit, junctions for the read-only top-level repo dirs, a **copy** of `Resources/`, every top-level file, and empty output dirs for the two samples; `RepoPaths.Root` is redirected at the staging root while `fs` runs and restored in a `finally`. The live samples and the live submodule are never mutated. Skipped when `ALIGNSAMPLES_BASE_TREE`/`_THEIRS_TREE` are supplied.

6. **Resolve provenance** — `fs` **does** emit `provenance.tsv` (unlike `f`): generated file → upstream source for every copy, overlay and rename, with entries dropped for files the transform deletes. Both sides are repo-relative (the staging root is gone by the time anyone reads it), and the driver rebases the source side onto the live repo, where the file actually opens.

7. **Structural preflight** — see below. **8. 3-way merge** (`DeltaMerge`, branches `align-samples/base` / `align-samples/theirs`, scoped to the two sample dirs). **9. Conflict pause** (`ConflictResolutionSession` → `conflicts-samples.log`, one stop for the whole list). These are the shared driver's stages; they behave exactly as documented in [AlignProjects.md](AlignProjects.md#stages-order-of-programmain).

10. **Report** — the residual-Chinese scan over the merged samples, **plus** [SampleCultureVerifier](Prepare.Common/Languages/SampleCultureVerifier.cs):
    - the invariant and `zh-Hans` resources have identical key sets;
    - no invariant value is blank (an unfilled English translation);
    - every `Properties.GtkWinFormsAppResources.<Key>` referenced in code exists in both;
    - no residual `[一-鿿]` outside the files meant to hold it — the `zh-Hans` resx and any asset with an `.en.<ext>` sibling (this generalizes the old hardcoded `TestData1.json` exemption).

    Any failure ⇒ exit `1`, listed file by file.

11. **Advance the checkpoint** — on success only, recording the **resolved commit id** and committing that one file (pathspec-scoped) so it cannot be reverted by a later checkout. `AlignSamples/build/` is deleted unless conflicts were left unresolved, in which case it is retained and its path printed (the pause just told the operator to diff files inside it).

## The transform `fs`

[SampleTransformPipeline](PrepareProjects/SampleTransformPipeline.cs) is the sample counterpart of `TransformPipeline`, and PrepareProjects runs the **same** code — no drift between the two tools:

- `RunSampleTransformAsync(repoRoot, source, target, options)` for one sample; `RunAllSampleTransformsAsync(repoRoot)` for both, writing `provenance.tsv` at the root.
- **Isolation is by repo root**: everything resolves through `RepoPaths`, which is pointed at the staging root for the duration.
- **`EnsureLatestGtkSharp` is off under AlignSamples** — that step queries nuget.org and rewrites the csproj, so whatever the feed served that minute would enter the delta as a fake upstream change. The live sample keeps its pinned version; version normalization is `OURS`-side state.
- **Determinism**: ordinal ordering wherever the transform enumerates the filesystem, and `.vs/` (upstream's IDE state — suo files, content indexes, caches) is not copied at all. Verified with `ALIGNSAMPLES_SELFCHECK`: two runs against the live submodule are byte-identical, at both `main` and the bootstrap base.

## Structural preflight

The preflight compares `fs(base)`'s layout against `OURS` and **reports** rather than aborts. `AlignmentOptions.AbortOnStructuralDrift` implements the abort (exit `2`), but AlignSamples leaves it off deliberately: measured against the live tree, the residual difference is five files upstream ships and this fork does not keep — `GTKWinFormsApp.xml`, `GTKSystemWinFormsApp11.xml`, `Properties/launchSettings.json` and both `PublishProfiles/*`. Enabling the abort would fail every run. The 3-way merge plus the conflict pause remain the real safety net.

## External dependencies

- **`git`** on `PATH` — worktrees, throwaway branches, cherry-pick, index inspection, the enrichment commit.
- **`cmd /c mklink /J`** — staging-root junctions (no elevation required; Windows-only).
- Initialized **submodules** — `gtksystem-windows-forms` (the source).
- Whatever `fs` itself needs. `fs` is far lighter than `f` (no `dotnet format`, no Roslyn rewrites), so an alignment takes seconds rather than minutes.

## Related documents

- [AlignSamples-Plan.md](AlignSamples-Plan.md) — design, decisions, phase status, verification checklist.
- [AlignProjects.md](AlignProjects.md) — the library-side post-pipeline; the shared driver's stages are documented there in full.
- [PrepareProjects.md](PrepareProjects.md) — the generation pipeline and the transforms.
- [CLAUDE.md](CLAUDE.md) — where durable changes belong (patches, `Resources/`, post-processing, submodule).

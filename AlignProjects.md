# AlignProjects

`AlignProjects` is the **post-pipeline**: a console tool run *after* [PrepareProjects](PrepareProjects.md) that forward-ports newer gitee upstream changes onto the already-generated `Net4x.Gtk.Windows.Forms` and `Net4x.Gtk.Drawing` projects — **incrementally**, and **preserving the fork's patches and manual edits**.

This document describes the tool **as built** ([AlignProjects/Program.cs](AlignProjects/Program.cs) + `Prepare.Common/Alignment/`). The design rationale, decisions, and remaining verification checklist live in [AlignProjects-Plan.md](AlignProjects-Plan.md); where the two disagree, this file describes what the code actually does today (see [Plan vs. implementation](#plan-vs-implementation)).

## Why it exists (vs. PrepareProjects)

| | PrepareProjects | AlignProjects |
|---|---|---|
| Input | The submodule pinned at the **frozen generation base** (`e5227ae…`) | The submodule at **checkpoint → latest gitee** |
| Method | Full regeneration, then replay `patches/*.patch` | Transformed-space **delta**, 3-way merged onto the live projects |
| Output | `Net4x.*` rebuilt from scratch + build + NuGet publish | `Net4x.*` updated in place; no build, no publish |
| Patches | Re-applied on top | Never touched — they are *already* in `OURS` and the merge respects them |

PrepareProjects deliberately regenerates from a *frozen* commit so its line-anchored patch series still applies. That freeze is exactly why upstream progress can't ride in through it — absorbing new gitee commits is AlignProjects' job.

## Core mechanism

The delta is computed in **transformed space** — i.e. *after* running the shared transform `f`, so it already lands on the correct `Net4x.*` paths, namespaces, English text, and formatting:

```
BASE   = f(submodule @ last-aligned checkpoint)     # bootstrap: e5227ae
THEIRS = f(submodule @ target commit)               # default: the submodule's tracking branch (main, gitee-merged)
OURS   = the live Net4x.* projects on disk          # = BASE + patches + manual edits
DELTA  = BASE -> THEIRS, scoped to Net4x.Gtk.Windows.Forms + Net4x.Gtk.Drawing
result = 3-way merge of DELTA onto OURS
```

`f` is `TransformPipeline.RunTransform()` — **the very same method PrepareProjects calls**, which is what guarantees `BASE` matches the real base of `OURS` (no drift between the two tools). AlignProjects references the `PrepareProjects` exe project purely for that type.

## Incremental & repeatable — the checkpoint

Each run absorbs only what is new:

- **First run:** base = `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`), target = latest gitee.
- **Every later run:** base = the previous run's target, read from [AlignProjects/last-aligned.txt](AlignProjects/last-aligned.txt).

This is a **correctness** requirement, not an optimization: after one alignment the live tree is `f(C1) + patches`, so the next base must be `C1`. Reusing the bootstrap base would recompute a delta that re-includes changes already present → double-applies and spurious conflicts.

The checkpoint file is managed by `AlignmentCheckpoint.Read/Write` and is **only advanced on a successful alignment** — an aborted run re-targets the same delta next time. It is independent of the submodule pointer: the pointer tracks the current source `HEAD`, the checkpoint records the last commit actually absorbed.

## How to run

Prerequisites: run PrepareProjects first (so `OURS` is committed and the submodule sits on the gitee-merged tracking branch), a clean working tree on a **non-default branch**, and `git` on `PATH`. Windows only (`net8.0-windows`; staging roots use `mklink /J`).

```sh
dotnet run --project AlignProjects/AlignProjects.csproj
```

Run it **interactively** — both operator pauses need a real console. With stdin redirected the translation pause is skipped and merge conflicts abort the run.

### Exit codes

| Code | Meaning |
|---|---|
| `0` | Clean alignment; checkpoint advanced |
| `1` | Aligned, but residual untranslated Chinese remains; checkpoint advanced |
| `3` | Conflicts left unresolved (or a non-interactive run hit conflicts); checkpoint **not** advanced |

### Environment overrides

| Variable | Default | Purpose |
|---|---|---|
| `ALIGN_REPO_ROOT` | 4 levels up from the exe | Repo root (test seam) |
| `ALIGN_BASE_COMMIT` | checkpoint, else `e5227ae` | Delta base |
| `ALIGN_TARGET_COMMIT` | `SourceRepositorySync.DefaultTrackingBranch` (`main`) | Delta target |
| `ALIGN_SOURCE_DIR` | `gtksystem-windows-forms/` | Tree scanned for new Chinese |
| `ALIGN_BASE_TREE` / `ALIGN_THEIRS_TREE` | — | Supply pre-produced transformed trees and **skip running the transform** — the seam used for smoke-testing the merge half |
| `ALIGN_PROVENANCE` | `<theirsTree>/provenance.tsv` | Generated→source manifest for the conflict report; when absent, `<theirsTree>/_manifests/*.Files.xml` is used instead |

## Stages (order of `Program.Main`)

1. **Resolve configuration** — repo root from the exe location (CWD-independent), then base / target / source dir from the checkpoint and env overrides; all three are echoed at startup.

2. **Enrich translations (additive)** — `TranslationEnrichment.EnrichTranslationsDictionary` scans the target submodule (`.cs`, regex `[一-鿿]+`, excluding `bin`/`obj`) and **appends** every unseen Chinese string to `Resources/Translations.xml` as a **blank-English** entry. Existing entries are never dropped, reordered, or reformatted; BOM/formatting are preserved. `EnrichResxLocales` does the same for locale resx files, adding blank values for keys present in `*.zh-Hans.xml`.

3. **PAUSE — manual translation** (`PauseForManualTranslation`) — if anything new was added, prints counts + file paths and waits for ENTER so the blank `<English>` values can be filled by hand. Skipped when nothing is new or stdin is redirected. Filled entries are cached forever, so later runs re-add nothing.
   > The **apply-side guard** in `TranslationsHelper`/`Translator` skips empty-English entries, so an unfilled blank leaves its Chinese **in place** rather than erasing it. That is what makes "enrich now, fill later" safe, and what stage 8 then reports.

4. **Produce BASE and THEIRS** (`ProduceTransformedTrees` → `ProduceOne`) — the trees land in `AlignProjects/build/{base,theirs}` (gitignored, cleared on entry); the **staging roots** themselves go under `%TEMP%\alignprojects-staging-<guid>\` and are deleted afterwards. For each commit, in its own staging root:
   - `StagingRoot.Create` adds a detached `git worktree` of the submodule at that commit, junctions (`mklink /J`) the read-only top-level repo directories, **copies** `Resources/` (the transform *writes* there — see below), copies every top-level file (props, `Nuget.Config`, `.snk`, …), and creates empty `Net4x.*` output dirs — so the transform sees a full repo layout but **writes in isolation**;
   - `RepoPaths.Root` is redirected at the staging root, `TransformPipeline.RunTransform()` runs, and `RepoPaths.Root` is restored in a `finally`;
   - the two generated `Net4x.*` dirs are copied out to `build/base` / `build/theirs`, that run's `*.Files.xml` manifests are captured into `build/<side>/_manifests/`, and the staging root is disposed (worktree removed, junctions deleted — their targets untouched).

   The live submodule and live `Net4x.*` are never mutated by this stage. Skipped entirely when `ALIGN_BASE_TREE`/`ALIGN_THEIRS_TREE` are supplied.

   > **Why `Resources/` is copied, not junctioned.** `ProjectUtility.PersistProjectFiles` writes its `*.Files.xml` manifests to `<repoRoot>/Resources/`. With a junction there, both transform passes wrote *through* into the live repo, silently overwriting the committed manifests twice per run. Copying restores the isolation the staging root exists to provide — and makes each side's manifests a per-snapshot artifact.

   > **Why staging roots must live outside the repo.** The mirror junctions the repo's top-level directories into the staging root. A staging root placed *inside* the repo therefore becomes reachable through its own mirror — `<staging>/AlignProjects → repo/AlignProjects → build/base-root → <staging> → …` — and every `SearchOption.AllDirectories` walk in the transform (e.g. `PostProcessing.GetFileNames`) recurses until Windows rejects the path length. `AlignProjects` is additionally excluded from the mirror: the transform has no use for it, and mirroring it would expose the previous run's `build/{base,theirs}` trees to the transform's whole-root rewrites.

5. **Resolve provenance** — the map from each generated file back to its raw upstream source, used by the conflict pause:
   - first choice: `ProvenanceManifest.Load` reads `provenance.tsv` (`generated<TAB>source`) — the purpose-built format, which `RunTransform` does not emit yet;
   - fallback (what actually runs today): `FilesXmlProvenance.Load` derives the map from the captured `_manifests/*.Files.xml`, re-keying each `<Item Source Destination>` to a repo-relative generated path and resolving `Source` against the **live** submodule — which sits at the target commit, so the paths are real files the operator can open after the staging worktrees are gone. Entries whose source doesn't exist are dropped (this filters the Drawing manifest's `Source == Destination` identity rows).

   Known gap: `TypeSplitService` records nothing, so files produced by a split have no provenance. For those, the `DIFFED base`/`theirs` pair is the reference — which it is in every case anyway.

6. **Structural preflight** (`StructuralMatch.Compare` + `StructuralMatchReducer.Reduce`) — compares `f(base)`'s layout to `OURS` per project. Patches legitimately rename, move, split, and delete files, so differences are expected; the reducer discards those explained by rename/move/split, and anything left is **printed as a note only**. This never aborts — the 3-way merge and the conflict pause are the real safety net.

7. **3-way merge** (`DeltaMerge.Apply`) — uses git's real merge machinery so the base blobs actually exist as objects: stage `build/base` as commit **`align/base`**, `build/theirs` as **`align/theirs`**, then `git cherry-pick align/theirs` back on the working branch. Result: `OURS + (THEIRS − BASE)`, touching only the two scoped project dirs. A clean pick is committed automatically.

8. **PAUSE — conflict resolution** (`PauseForConflictResolution`, driven by `ConflictReporter`) — the upstream delta is first auto-merged into **every** conflicted file; whatever still needs a human is then written to **`conflicts.log` in the solution directory** and the run pauses **once**. The operator works through the whole list in an editor, `git add`s each resolved file, and presses ENTER; anything still outstanding rewrites the log and pauses again. The log is deleted once nothing remains. (Stopping per file does not scale — a large upstream delta produces hundreds of conflicts.)

   Each log entry carries full paths, ordered by usefulness:
   - `edit here (fork)` — the conflicted file in the live tree;
   - `DIFFED base` / `DIFFED theirs` — **the pair the delta was actually computed from**. Both are transformed output: formatted, type-split, translated, in the fork's layout. Diffing these two reproduces exactly the hunk git tried to apply. These are located with `TreePathResolver`, which follows renames: where a patch moved a file, git reports the conflict under the *fork's* path while the tree still holds the transform's path, so a naive join would point at a non-existent file. Unresolvable references are printed with `[NOT FOUND in tree]` rather than silently looking valid;
   - `raw upstream (context only)` — the `gtksystem-windows-forms` original from provenance. It is unformatted and in the pre-transform layout, so it explains *intent* but cannot be used to reconstruct hunks.

   Three cases:
   - **same-path** → `IncorporateUpstreamInto` merges the upstream delta into the fork's version, leaving markers only where they truly overlap; resolve, `git add`, ENTER;
   - **a patch moved the file** (exactly one same-named fork copy) → `IncorporateIntoForkCopy` merges into that copy and `git rm`s the stale path; a clean merge continues silently, otherwise resolve markers and ENTER (the tool stages it for you);
   - **ambiguous** (no fork copy, or several) → the operator is guided by hand.

   The re-check covers both index-unmerged paths and moved-file fork copies that still carry markers (the latter never appear as unmerged, because their old path was already resolved by `git rm`). Once nothing is outstanding, `DeltaMerge.Continue()` finalizes the cherry-pick. Non-interactive ⇒ the log is written, conflicts are left in place, exit `3`.

9. **Residual-Chinese report** (`ResidualChineseScanner.Scan`) — scans the merged `Net4x.*` for `[一-鿿]` and lists each offending file with a match count. Non-empty ⇒ exit code `1`; it means a dictionary blank was left unfilled in stage 3.

10. **Advance the checkpoint** — `AlignmentCheckpoint.Write(checkpointFile, targetCommit)`. `AlignProjects/build/` is then deleted — **unless conflicts were left unresolved** (exit `≥ 2`), in which case it is retained and its location printed, because stage 8 just told the operator to diff files inside it.

## Plan vs. implementation

[AlignProjects-Plan.md](AlignProjects-Plan.md) is the design document and is ahead of the code in three places — worth knowing before relying on it:

| Plan says | Code does |
|---|---|
| §3.4 structural preflight **aborts** the run (exit `2`) on layout drift | Informational note only; never aborts. Exit code `2` is unused. |
| §3.0/§3.8 the tool **auto-commits** the filled translations before the merge and the aligned `Net4x.*` + checkpoint after | Not implemented. `DeltaMerge` commits the cherry-pick itself; translations and the checkpoint file are left as working-tree changes for you to commit. |
| §3.9 failure restores `Net4x.*` and the submodule from git | Not implemented as an outer handler. `DeltaMerge` exposes `Abort()`/`Cleanup()`, but `Program` does not call them — after a `3` exit the cherry-pick is still in progress (`git cherry-pick --abort` to back out). |

Also still pending per the plan's status table: the transform's **determinism self-check** and native **provenance emission** (§3.1). `RunTransform` still does not write `provenance.tsv`; stage 5 now derives the mapping from the `*.Files.xml` manifests instead, which covers copies/moves/renames but not type splits.

## External dependencies

- **`git`** on `PATH` — worktrees, throwaway branches, cherry-pick, index inspection.
- **`cmd /c mklink /J`** — staging-root junctions (no elevation required; Windows-only).
- Initialized **submodules** — `gtksystem-windows-forms` (the source) and `mono` (referenced by the transform).
- Whatever `TransformPipeline.RunTransform` itself needs (see [PrepareProjects.md](PrepareProjects.md)) — notably `dotnet` for `dotnet format`. Each alignment runs the full transform **twice**, so expect minutes.

## Related documents

- [PrepareProjects.md](PrepareProjects.md) — the generation pipeline and the shared transform `f`.
- [AlignProjects-Plan.md](AlignProjects-Plan.md) — design, decisions, phase status, verification checklist.
- [Gitee-Update-Plan.md](Gitee-Update-Plan.md) — the overall upstream-delta strategy.
- [CLAUDE.md](CLAUDE.md) — where durable changes belong (patches, `Resources/`, post-processing, submodule).

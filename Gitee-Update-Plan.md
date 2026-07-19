# Plan: `AlignProjects` — post-pipeline to align Net4x.* with the merged gitee upstream

## Context

`Net4x.Gtk.Windows.Forms` / `Net4x.Gtk.Drawing` are **generated** by **PrepareProjects** from the vendored `gtksystem-windows-forms` submodule (your fork), which finishes by replaying 88 `patches/*.patch`. The current `Net4x.*` were generated from the fork at its old base and therefore miss a year of upstream (gitee) work.

You have now **manually merged the latest gitee into the submodule** (HEAD `9dc1f7d` "Merge branch 'up' into main", resolving conflicts by hand), while the old base commit **`337d7df` remains reachable**. So the upstream delta to forward-port is simply `git diff 337d7df..HEAD` inside the submodule.

**Goal:** a **new, separate executable** (`AlignProjects`) that runs *after* PrepareProjects and propagates that upstream delta onto the generated `Net4x.*` library projects. **Sample apps are out of scope.** PrepareProjects is left untouched; nothing is committed back into the submodule beyond the merge you already did.

### Verified facts (measured)

- Submodule HEAD `9dc1f7d` = your fork (`e5227ae`, incl. 3 trivial custom commits) merged with gitee `e9abb91`. `337d7df` (2025‑07‑26) is an ancestor of HEAD.
- **Delta `337d7df..HEAD`, library only** (`Source/GTKSystem.Windows.Forms`, excluding bin/obj/xml): **217 files, +9,363 / −5,123**. Whole-repo delta is 366 files, but ~104 are Samples (ignored) plus docs/Libs/VS-plugin.
- Gitee still has **no drawing split and no reorg** — those are your fork's own features, produced by the transform, and are preserved automatically.

## Core design decisions (locked)

1. **Separate executable, not a change to PrepareProjects.** New tool `AlignProjects`, analogous to `PrepareProjects`, run as a post-pipeline.
2. **Single source, two commits.** BASE and target both come from the submodule history — `337d7df` and HEAD — so the delta is a clean in-repo diff. The `gitee.gtksystem-windows-forms/` folder is not needed here.
3. **Library only.** Only `Net4x.Gtk.Windows.Forms` and `Net4x.Gtk.Drawing` are aligned; sample-app changes are excluded from the delta.
4. **No conflict-resolution persistence.** The fork is frozen at `9dc1f7d`, so the delta is stable run-to-run; the tool re-applies it each run onto the freshly regenerated `Net4x.*`. Rejects are *reported*, not stored.
5. **Diff in transformed space** (the essential correctness rule): compare the two commits **after** running the PrepareProjects transform on each, so the delta is already in `Net4x.*` paths/format and applies. A raw `337d7df..HEAD` diff would fail exactly like the patches, because `Net4x.*` is moved/renamed/translated/reformatted and drawing is split out.

## How `AlignProjects` works

Let `f` = the PrepareProjects transform (copy → translate → reorg → split drawing → format → XML-doc merge → type-split) — everything PrepareProjects does **except** applying patches and publishing. `AlignProjects` reuses the same `Prepare.Common` + `XmlDocSync` building blocks.

```
Input already on disk after a normal PrepareProjects run:
  OURS = the live Net4x.* projects        (= f(old fork) + patches)

Steps performed by AlignProjects:
  1. Materialize both commits without disturbing the submodule working tree:
        git worktree add <tmp/base>   337d7df
        git worktree add <tmp/head>   HEAD        # 9dc1f7d
  2. BASE   = f(tmp/base)    # old upstream, transformed -> Net4x.* layout, no patches
     THEIRS = f(tmp/head)    # merged upstream, transformed -> Net4x.* layout, no patches
  3. DELTA  = git diff BASE THEIRS
             scoped to the LIBRARY projects only (Net4x.Gtk.Windows.Forms, Net4x.Gtk.Drawing);
             sample-app outputs excluded
  4. Apply DELTA onto OURS with `git apply --3way`
  5. Write an alignment report: files changed, new files added, any *.rej / conflicts
  6. Remove the worktrees
```

Result: the live `Net4x.*` = your work (patches, in OURS) **+** the upstream delta, in the correct transformed paths. Because `BASE` and `THEIRS` come from the **same** `f`, `BASE` matches the pre-patch base of `OURS`, so the only possible conflicts are where a patch and an upstream change touch the same lines — expected to be rare and simply reported.

### Base commit note
`337d7df` (per your instruction) is the base. It predates your 3 trivial custom commits, so `DELTA` technically re-includes their effect (`.gitignore` — not in the library; an encoding fix — yields identical translated output; an empty commit), a harmless near-no-op in library scope. If you want a *pure* gitee delta with zero double-apply, use `e5227ae` (the fork HEAD just before the merge, also an ancestor of `9dc1f7d`) as BASE instead — one-line change.

### Why transformed-space, concretely
An upstream edit to `GTKControls/Drawing/Drawing2D/LinearGradientBrush.cs` (Chinese, upstream layout) has no clean target in `Net4x.Gtk.Drawing/System/Drawing/Drawing2D/LinearGradientBrush.cs` (English, reformatted, split out). Running `f` on each commit first moves/renames/translates/reformats it into the `Net4x.*` shape, so `DELTA` lands on the right file with matching context. New upstream files (MDIForm etc.) are placed automatically by `f`'s namespace-driven `FolderFixer`; drawing types are routed by `DrawingService` — so split/reorg are honored for new content too.

### Reusing the transform without changing PrepareProjects
`PrepareProjects.Program.Main` orchestrates `Prepare.Common`/`XmlDocSync` calls. `AlignProjects` composes the **same** calls against the worktrees. Sharing the sequence:
- **(Recommended) Extract `f`** into a reusable `(sourceDir, outputDir)` method in a shared lib (e.g. `Prepare.Common`); both tools call it. Guarantees `BASE` matches `OURS`'s real base (no drift). Behavior-preserving extraction, not a functional change.
- **(Zero-touch) Duplicate the sequence** in `AlignProjects` — keeps PrepareProjects literally untouched but risks drift (shows up as spurious `BASE` diff noise). Acceptable short-term.

## Prerequisite: harden `f` to survive the merged source

`THEIRS = f(HEAD)` only works if `f` runs to completion on the year-newer merged source. Add fail-loud guards to the hardcoded upstream-path assumptions (so a missing/moved target reports clearly instead of throwing or silently no-op-ing):
- `PrepareWorker.RenameDirectoriesInSourceRepository()` (the `GTKControls`/`GTKSystem` case-rename)
- `ProjectPreparation.RenameProjectFiles()` (expects `GTKSystem.Windows.Forms.csproj`)
- `DrawingService.SeparateDrawing()` / `PrepareWorker.OptimizeNet4xGtkDrawingFileSystem()` (assume `System\Drawing` with `Printing` + nested `Drawing2D\Drawing\`)
- `Program.cs` hardcoded lists (`TypeSplitService` excludes; `PatchFile`/`PatchCommentsInFile` targets)

These live in the shared `f` code, so both tools benefit.

## Phased execution

### Phase 0 — Baseline
- Outer repo: `git checkout -b gitee-align-2026-07`.
- Run PrepareProjects as today; confirm `Net4x.*` builds (`dotnet build Net4x.Gtk.Windows.Forms.sln`). This is `OURS` / known-good.

### Phase 1 — Harden `f`
- Add the path guards + fail-loud reporting so `f` can run on the merged HEAD. Confirm PrepareProjects still produces today's output unchanged.

### Phase 2 — Build `AlignProjects`
- New console project in `PrepareProjects.sln` (Tools), `net8.0-windows`, referencing `Prepare.Common` + `XmlDocSync` (mirrors `PrepareProjects.csproj`).
- Implement: worktree checkouts of `337d7df` and HEAD → `f`=BASE / `f`=THEIRS → library-scoped `DELTA` → `git apply --3way` onto live `Net4x.*` → alignment report → cleanup.
- Make the two commit refs and the library-scope filter configurable.

### Phase 3 — First alignment run
- Run PrepareProjects, then `AlignProjects`.
- Iterate on the `f` guards until `f(HEAD)` completes cleanly; review the report and resolve the (expected-few) rejects directly in `Net4x.*`.

### Phase 4 — Verify new upstream features
- Confirm new files land right: MDIForm (`MdiLayout.cs`), `ToolStripGripStyle/RenderMode.cs`, `ControlUtility.cs`, `GTKSystem/SR*.cs` → in `Net4x.Gtk.Windows.Forms`, not `Net4x.Gtk.Drawing`. Watch for an `SR.cs` duplicate-type collision with `RegenerateSrDesigner`.
- New Chinese strings captured by `Resources/Translations.xml`.

### Phase 5 — Build, test, finalize
- `dotnet build` (Debug + Release) and `dotnet test Net4x.Gtk.Windows.Forms.sln` — no new failures vs Phase 0 (the test suite is the primary "did I lose a modification?" signal).
- Commit on `gitee-align-2026-07`: the new `AlignProjects` project, the `f` hardening, and the aligned `Net4x.*`.
- Wire the run: invoke `AlignProjects` manually after PrepareProjects, or chain it in a script (kept out of PrepareProjects to honor "don't modify it").

## Your modifications — how each survives

| Layer | Where | Preserved by |
|---|---|---|
| Downstream code changes | `Net4x.*` beyond `f(old fork)` (patches + manual) | Live in `OURS`; `DELTA` is applied *on top* via 3-way |
| Overlays / post-processing | `Resources/*.cs`, `Translations.xml`, `*PostProcessing.xml` | Part of `f`; present in both `BASE` and `THEIRS`, so no spurious diffs |
| Pipeline logic (split/reorg) | `Prepare.Common` / `XmlDocSync` | The shared `f`, applied identically to both commits |
| Your manual gitee conflict resolutions | Already in submodule HEAD `9dc1f7d` | Flow in through `THEIRS = f(HEAD)` |

## Verification checklist
- [ ] `f` completes on **both** `337d7df` and HEAD worktrees (guards report no missing targets); the submodule working tree and `gitee.gtksystem-windows-forms/` are left untouched; worktrees removed.
- [ ] Alignment report shows the library delta applied; no unresolved `*.rej` / `<<<<<<<` in `Net4x.*`; no sample-app files modified.
- [ ] `dotnet build Net4x.Gtk.Windows.Forms.sln` clean (Debug + Release).
- [ ] `dotnet test` — no new failures vs Phase 0.
- [ ] `Net4x.Gtk.Drawing` still a separate assembly referenced by Forms (split intact).
- [ ] MDIForm + other new upstream types present and usable in `Net4x.Gtk.Windows.Forms`.
- [ ] Re-running PrepareProjects + `AlignProjects` reproduces the same aligned result (idempotent).

## Risks
- **`f` breaks on the merged source** (highest) — mitigated by Phase 1 hardening; `THEIRS` can't be produced until `f` survives HEAD.
- **Sequence drift** if `AlignProjects` duplicates `f` instead of sharing it — prefer the extraction.
- **Source mutation** — `f`'s in-place folder renames must run on the **worktrees** (throwaway), never the submodule working tree.
- **Patch × upstream overlap conflicts** — expected few (fork frozen); reported for manual fix, not persisted.
- **`SR.cs` collision** between upstream `GTKSystem/SR.cs` and `RegenerateSrDesigner`.

## Key files / new artifacts
- **New:** `AlignProjects/AlignProjects.csproj` + `Program.cs`, added to `PrepareProjects.sln`.
- `Prepare.Common` (+ `XmlDocSync`): the reusable `f` (extraction recommended) and path-guard hardening; `GitPatchApplier.cs` fail-loud reporting reused for the alignment report.
- Unchanged: `PrepareProjects` behavior, the `gtksystem-windows-forms` submodule content (beyond the merge you did), `gitee.gtksystem-windows-forms/`.

# Plan: build the `AlignProjects` console application

## Purpose

`AlignProjects` is a console tool (a **post-pipeline**, run after `PrepareProjects`) that forward-ports the latest gitee upstream changes onto the generated `Net4x.*` library projects. Chinese→English is handled by **additive dictionary enrichment + an interactive pause for manual translation** — there is no AI/LLM component in the codebase; the fill is done by hand during the pause (which can be assisted, e.g. by asking an AI in the chat, but nothing is wired into the tool).

It complements the entry/exit source hooks already in PrepareProjects (`SourceRepositorySync.CheckoutGenerationBase` / `AdvanceToLatestGitee`): after a normal PrepareProjects run, the submodule is left on `main` (gitee-merged) while `Net4x.*` was generated from the frozen base `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`). `AlignProjects` consumes exactly those two states. It is designed to be **re-run over time** — each iteration absorbs only the upstream changes since the previous one (see [Incremental & repeatable](#incremental--repeatable)). The overall delta strategy lives in [Gitee-Update-Plan.md](Gitee-Update-Plan.md); this plan is about **constructing the tool**.

## Core mechanism

The alignment is a 3-way merge in **transformed space** — the delta is computed *after* running the PrepareProjects transform `f` (copy → translate → reorg → split drawing → format → …, minus patches) on each upstream snapshot, so it lands on the correct `Net4x.*` paths/format:

```
BASE   = f(submodule @ last-aligned checkpoint, or bootstrap base e5227ae on the first run)
THEIRS = f(submodule @ target commit, default HEAD = main, gitee-merged)
OURS   = the live Net4x.* projects (= BASE + patches)   # already on disk
DELTA  = transformed-space diff BASE -> THEIRS   (scoped to Net4x.Gtk.Windows.Forms + Net4x.Gtk.Drawing; samples excluded)
result = 3-way merge of DELTA onto OURS (see §3.4) + alignment report
```

For the delta to be clean English (matching OURS), `f` must translate **all** Chinese in the new snapshot — hence the enrichment + pause below, which run before `f`.

## Incremental & repeatable

`AlignProjects` applies only the *new* upstream delta each iteration — never re-applying everything from the original base:

- **First run:** base = the bootstrap commit `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`, what the current `Net4x.*` were generated from); target = the latest downloaded gitee commit (`C1`). Absorbs `e5227ae → C1`.
- **Every later run:** base = the **previous run's target** (the last commit already absorbed); target = the newly downloaded gitee `HEAD`. Absorbs `Cn → Cn+1`.

This is **required for correctness**, not just efficiency. After iteration 1 the live `Net4x.*` equals `f(C1) + patches`, so iteration 2's base *must* be `C1`: reusing `e5227ae` would recompute a delta that re-includes the `C1` changes already present, causing double-applies and conflicts. Keeping `base = f(last-aligned)` guarantees `DELTA = f(target) − f(base)` is exactly the fresh upstream changes, applied onto the matching `OURS`.

**The checkpoint.** After a successful alignment, the tool records the target commit as the new "last-aligned" checkpoint — a small committed file (e.g. `AlignProjects/last-aligned.txt`) stored alongside the aligned `Net4x.*` (a git ref/tag in the submodule is an alternative). The next run reads it as its base, falling back to the bootstrap base when absent. Because the checkpoint is a commit on the **fork's** `main` history (which merges gitee in), it stays reachable even if gitee later rewrites its own history. Typical future cycle: download newer gitee into the submodule → run `AlignProjects` → it aligns `checkpoint → new HEAD` and advances the checkpoint.

## Status at a glance

| Piece | State |
|---|---|
| Project setup (`AlignProjects.csproj` + `Program.cs`, added to `PrepareProjects.sln`) | **Done** |
| Phase 1 — harden `f` | **Done** |
| Phase 2 — enrichment (additive) + apply-side guard + manual-translation pause | **Done** |
| Phase 3 driver — checkpoint, structural preflight, 3-way merge, conflict pause, report, `AlignFromTrees` orchestration (unit-tested + smoke-tested via `ALIGN_*_TREE`) | **Done** |
| Phase 3 transform — §3.1 `RunTransform`+`RepoPaths` (extraction **done**; `repoRoot` threading/determinism/provenance pending), §3.2 staging roots | **Pending** (needs GTK env to validate) |
| Phase 4 — wire-up & docs | Pending |

## Translation handling (as implemented)

The existing dictionary machinery is reused unchanged for **applying** translations: `TranslationsHelper.LoadTranslations` loads `<Chinese>/<English>` pairs (longest-first); `Translator` / `ProjectTranslation` find/replace across files (comments + inline Chinese), and map resource literals to `Properties.<Resource>.<Key>` via `ResxReader` on the `zh-Hans.resx`.

`AlignProjects` adds three things:

1. **Additive enrichment** ([Prepare.Common/Languages/TranslationEnrichment.cs](Prepare.Common/Languages/TranslationEnrichment.cs)):
   - `EnrichTranslationsDictionary(translationsXmlPath, sourceDirectories)` — scans `.cs` files (regex `[一-鿿]+`, excluding `bin`/`obj`) and **appends** every Chinese string not already present to `Resources/Translations.xml` as a **blank-English** entry. It is strictly additive: existing entries are never dropped, reordered, or reformatted, and the file's BOM/formatting are preserved. (Note: this deliberately does **not** use `ChineseDetector.DetectChinese`, which rebuilds the file from the scan and would prune entries not found — that caused translation loss in testing.)
   - `EnrichResxLocales(resourcesDirectory)` — for each `*.zh-Hans.xml`, ensures its sibling locale files (invariant + `it/fr/de/es`) have a blank-value entry for every key in the Chinese source, via `ResxLocaleEnricher.EnsureBlankEntriesForSourceKeys` (adds only genuine gaps; safe no-op when in sync).
   - Scope: the whole submodule (Source + Samples).

2. **Apply-side safety guard** — `TranslationsHelper.LoadTranslations` and `Translator.MakeRawTranslationOnFile` now **skip empty-English entries**, so an unfilled blank leaves its Chinese **in place** instead of erasing it. This makes "enrich now, fill later" safe regardless of ordering. (Shared with PrepareProjects — benefits both.)

3. **Interactive manual-translation pause** ([AlignProjects/Program.cs](AlignProjects/Program.cs) `PauseForManualTranslation`) — after enrichment, if new blanks were added it prints a prominent notice with counts + file paths, then waits for ENTER so the blank `<English>` values can be filled by hand. It skips the pause when there is nothing new, and when stdin is redirected (non-interactive runs continue automatically). This is where the one-time translation happens — replacing the previously-considered separate AI fill utility.

### Ordering

```
enrich Translations.xml + resx locales with BLANK entries (additive; preserves existing)
  -> PAUSE: fill the blank <English> values by hand, then continue
  -> f(base) = BASE  and  f(HEAD) = THEIRS   (apply SKIPS any still-blank entries -> untranslated Chinese stays visible)
  -> DELTA -> apply to OURS -> report
```

Filled entries are cached in `Translations.xml` forever — future runs re-add nothing and need no re-translation.

## Reusing the transform `f` (for Phase 3)

`PrepareProjects.Program.Main` orchestrates `Prepare.Common`/`XmlDocSync` calls (including `ProjectTranslation.TranslateFiles`). Two options:
- **(Recommended) Extract `f`** — a behavior-preserving `(sourceDir, outputDir)` method in a shared lib that both `PrepareProjects` and `AlignProjects` call. Guarantees `BASE` matches `OURS`'s real base (no drift).
- **(Zero-touch) Mirror the sequence** in `AlignProjects` — keeps PrepareProjects untouched but risks drift.

Either way, `AlignProjects` runs `f` on **git worktrees** of the two commits (never the live submodule working tree), so the source stays pristine.

## Project setup (done)

- Console project **`AlignProjects/AlignProjects.csproj`** in `PrepareProjects.sln`, `net8.0-windows`, `OutputType=Exe`, mirroring `PrepareProjects.csproj` (`NoWarn=CA1416;IDE0055`; no publish/`SkipPush`).
- **References:** `Prepare.Common`, `XmlDocSync`.
- **Config (env vars):** `ALIGN_BASE_COMMIT` (default: the last-aligned checkpoint, else `SourceRepositorySync.GenerationBaseCommit`), `ALIGN_TARGET_COMMIT` (default `main` HEAD), `ALIGN_SOURCE_DIR` (default the submodule). Paths resolve against the exe location, so CWD does not matter.

## Implementation phases

### Phase 1 — Harden `f` (done)
`PathGuard` ([Prepare.Common/IO/PathGuard.cs](Prepare.Common/IO/PathGuard.cs)) fails loud + skips when a hardcoded upstream path is missing, applied to `PrepareWorker.RenameDirectoriesInSourceRepository`, `ProjectPreparation.RenameProjectFiles`, `DrawingService.SeparateDrawing`, `PrepareWorker.OptimizeNet4xGtkDrawingFileSystem`, and `PrepareWorker.PatchFile`/`PatchCommentsInFile`.

### Phase 2 — Enrichment + apply guard + pause (done)
Additive `TranslationEnrichment` + `ResxLocaleEnricher`; the empty-English skip guard in `TranslationsHelper`/`Translator`; the `PauseForManualTranslation` step. Wired into `AlignProjects.Program.Main`. Tests: [TranslationEnrichmentTests](Prepare.Tests/TranslationEnrichmentTests.cs), [ResxLocaleEnricherTests](Prepare.Tests/ResxLocaleEnricherTests.cs), [TranslationApplyGuardTests](Prepare.Tests/TranslationApplyGuardTests.cs).

### Phase 3 — The alignment driver (`Program.Main`) — pending (the core)

This is the heart of `AlignProjects`. It produces the two transformed trees, computes the upstream delta between them, and 3-way-merges that delta onto the live `Net4x.*` — preserving the fork's patches/manual edits. Two hard realities shape the design:

- **`f` writes to fixed repo-root-relative paths.** The PrepareProjects transform reads `..\..\..\..\gtksystem-windows-forms\Source`, `..\Resources`, `..\mono`, `..\.editorconfig` and writes `..\Net4x.Gtk.Windows.Forms` / `..\Net4x.Gtk.Drawing`. It is not currently retargetable to an arbitrary output dir, so producing `BASE` and `THEIRS` means either (a) running it in place and snapshotting, or (b) parameterizing its repo root. See §3.1.
- **A true 3-way merge needs the base blobs in git's object store.** `git diff --no-index base theirs | git apply --3way` cannot 3-way-merge, because the preimage blobs aren't objects git can look up. The robust mechanism is to stage `BASE`/`THEIRS` as real commits and let git's merge machinery do the 3-way. See §3.4.

#### 3.0 Preconditions & commit boundaries (tool auto-commits)
- Enrichment + the manual-translation pause (Phase 2) have run, so `Translations.xml` is complete; **both** `BASE` and `THEIRS` will translate fully via the same dictionary.
- The tool runs on a **non-default working branch** and requires the tree to be clean *outside the files it manages* (so its git-restores never clobber unrelated work); it refuses otherwise.
- **The tool auto-commits at defined points** (creates clean restore points and a reproducible history):
  1. Before the merge — commit the enriched + filled `Translations.xml` and resx changes (`AlignProjects: enrich+fill translations for <target>`), yielding the clean tree §3.5 needs.
  2. After the merge — commit the aligned `Net4x.*` **plus** the advanced checkpoint together (§3.8).
- On any failure it restores `Net4x.*` and the submodule from git (§3.9), so the repo is never left half-transformed.

#### 3.1 Extract a reusable, deterministic, patches-off transform `RunTransform()`
Factor the transform steps out of `PrepareProjects.Program.Main` into a callable `RunTransform()` — everything from `RenameDirectoriesInSourceRepository` through `TypeSplitService`, **excluding** the source hooks (`CheckoutGenerationBase`/`AdvanceToLatestGitee`), `GitPatchApplier.CommitAndApplyPatches`, the build, publish, and package-prune. `PrepareProjects` then calls `RunTransform()` between its hooks/patches; `AlignProjects` calls it directly. This is the "extract `f`" option (settled — over mirroring) and guarantees `BASE` matches `OURS`'s real base.

Two hardening requirements land in `RunTransform`:
- **Determinism — neutralize non-deterministic sources** (decision). `f(base)` and `f(target)` must be byte-reproducible so the delta contains *only* real upstream changes. Audit and fix: stable, culture-invariant ordering everywhere files/entries are enumerated (`FolderFixer`, `TypeSplitService`, XML-doc merge, `dotnet format`), and strip timestamps / GUIDs / machine paths from generated output. (Version's `DateTime.Now` writes `Directory.Build.Props`, outside the snapshotted dirs, so it's already out of scope.) A CI/self-check can assert `RunTransform(target)` run twice is byte-identical.
- **Provenance — full manifest** (decision). `RunTransform` records the `generated-file → source-file` mapping at each move / rename / **split** / copy step and emits a manifest, so §3.6 can always name the exact raw `gtksystem-windows-forms` source of a conflicted file (including split files → their originating source), not a guess.

Isolation — **option B (parameterized repo root), chosen.** Thread a `repoRoot` through the transform, replacing each hardcoded `..\..\..\..\X` with `Path.Combine(repoRoot, "X")`, so it reads a source worktree and writes into a private **staging root** — never touching the real `Net4x.*` or the live submodule. Each staging root holds: the submodule as a `git worktree` at the wanted commit, the shared `Resources`/`mono`/`.editorconfig` linked from the real repo, and empty `Net4x.*` output dirs. (Option A — run in place + snapshot + git-restore — was rejected to avoid clobbering the live tree.)

#### 3.2 Produce `BASE` and `THEIRS`
Resolve the two commits: **`BASE`** = the last-aligned checkpoint if present, else the bootstrap base `SourceRepositorySync.GenerationBaseCommit` (`e5227ae`); **`THEIRS`** = the target, default submodule `HEAD` (gitee-merged `main`). Both overridable via `ALIGN_BASE_COMMIT` / `ALIGN_TARGET_COMMIT`. Then, for each commit, in its own staging root:
1. `git worktree add <root>/gtksystem-windows-forms <sha>` — a private checkout of the source at that commit (the live submodule is never touched).
2. Populate the staging root: link `Resources` and `mono` as directory **junctions** (`mklink /J` — no elevation needed on Windows) and **copy** `.editorconfig`; create empty `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/`.
3. `RunTransform(repoRoot = <root>)` — writes the transformed `Net4x.*` into the staging root (deterministic; emits the provenance manifest for `THEIRS`). The folder-rename mutations land in the worktree, not the live submodule.
4. `BASE` / `THEIRS` = the staging root's `Net4x.Gtk.Windows.Forms/` + `Net4x.Gtk.Drawing/` (referred to as `build/base` / `build/theirs` below) — only these two dirs feed the delta, so the sample apps are excluded.

#### 3.3 Working-tree state
Under option B the transforms never modify the live `Net4x.*` or submodule, so `OURS` is intact throughout and the submodule stays on its tracking branch at `HEAD` (gitee-merged), consistent with the PrepareProjects exit hook. The temporary `git worktree` checkouts and staging roots are removed once `BASE`/`THEIRS` are captured (final cleanup in §3.9).

#### 3.4 Structural-match preflight (abort-guard)
Before any temp commit or merge, compare the `build/base/` snapshot against `OURS` (HEAD's `Net4x.*`): they must differ only **inside** files the fork patched/edited — i.e. **no added, removed, or renamed paths**. If `BASE` has paths `OURS` lacks (or vice-versa), `f`'s layout has drifted from how `OURS` was generated and cherry-pick would produce duplicates/orphans. On mismatch, **abort with a report** listing the offending paths — nothing is modified. (Guards gap #2; the two-run determinism self-check from §3.1 guards gap #1.)

#### 3.5 The 3-way merge (delta → `OURS`)
Use git's real merge machinery via throwaway commits so the base blobs exist:
1. On a temp branch, replace the two library dirs with `build/base/`, commit → **`align/base`**.
2. From `align/base`, replace them with `build/theirs/`, commit → **`align/theirs`** (its diff vs `align/base` *is* the pure upstream delta, in the transformed layout).
3. Back on the working branch (`OURS`), `git cherry-pick align/theirs` — a 3-way merge with `align/base` as base, `OURS` as *ours*, `align/theirs` as *theirs* = `OURS + (THEIRS − BASE)`. Only the two library dirs change.
   - New upstream files → added; deletions → removed; edits → merged. Conflicts surface as standard markers → §3.6.
   - (Equivalent: `git diff align/base align/theirs -- <scope> | git apply --3way` — valid because the committed blobs give real index lines. Cherry-pick is preferred for conflict handling.)

#### 3.6 Conflict handling & manual-resolution pause
Conflicts come in several kinds, each shown with its category: **content overlap** (a patch/manual edit meets an upstream change), **rewritten-from-scratch** (whole-file), **delete/modify** (upstream removed a file a patch changed), **add/add** (upstream added a path a patch already created). For each conflicted file (`git diff --name-only --diff-filter=U`), print — all as **full paths**:
- the conflicted **`Net4x.*`** file in the working tree — *where to apply the change*;
- the exact raw **`gtksystem-windows-forms`** source file from the provenance manifest (§3.1) — *the upstream original to port from*;
- `build/theirs/<rel>` (new upstream in `Net4x.*` layout) and `build/base/<rel>` (previous baseline) as diff aids.

Then **PAUSE** for operator confirmation (same mechanics as `PauseForManualTranslation`: prominent notice, wait for ENTER; auto-continue only when non-interactive). The operator edits each conflicted `Net4x.*` file, `git add`s it, then confirms. **Partial resolution:** on confirm, re-check `git diff --name-only --diff-filter=U`; if any paths remain unmerged, re-list them and re-pause — never `cherry-pick --continue` with unresolved paths. Non-interactive runs leave conflicts in place, list them in the report, and exit non-zero.

This is the merge-side analogue of the translation flow: **notify (with full paths) → manual fix → confirm → continue.**

#### 3.7 Alignment report + residual-Chinese check
Emit a report: files added / modified / deleted, and any conflicts (`.rej` / `<<<<<<<`). Run a `[一-鿿]` scan over the merged `Net4x.*` and list residual Chinese (a translation gap — a blank left unfilled, protected by the apply guard). Non-zero conflicts or residual Chinese ⇒ exit non-zero so the run isn't mistaken for clean.

#### 3.8 Record the checkpoint
On a successful alignment (cherry-pick clean, or conflicts resolved), the tool commits the aligned `Net4x.*` **and** writes the **target** commit as the new last-aligned checkpoint in the same commit. This becomes the next run's base. The checkpoint is **independent of the submodule pointer**: the pointer tracks the current source `HEAD`, the checkpoint records the last commit actually absorbed — they legitimately differ once the submodule advances past the last alignment. If conflicts are unresolved, the checkpoint is **not** advanced, so a re-run targets the same delta until finalized.

#### 3.9 Cleanup
Remove the staging roots — `git worktree remove` the source checkouts, delete the junctions and output dirs — and the `align/base`/`align/theirs` temp branches. On failure at any step, restore `Net4x.*` and the submodule from git so the repo is never left half-transformed (under option B the transforms don't touch them, but the merge in §3.5 does).

#### Phase 3 design notes
- **Determinism (neutralized).** The two-run byte-identical self-check (§3.1) turns any residual non-determinism into a loud failure rather than silent delta noise.
- **Cherry-pick vs merge commit.** Cherry-pick keeps history linear and is easy to abort (`git cherry-pick --abort`); a merge commit would record `align/theirs` as a parent (undesirable for a throwaway branch).
- **Two operator pauses.** Phase 2 fills translations; §3.6 resolves merge conflicts — same notify → confirm → continue pattern; both auto-continue only when stdin is non-interactive.
- **Cost.** Each alignment runs the full transform **twice** (+ builds) — minutes; acceptable for an occasional upstream sync.

### Phase 4 — Wire-up & docs — pending
- Run `AlignProjects` after `PrepareProjects` (manually or via a small script; kept out of PrepareProjects).
- Document it (a section in [PrepareProjects.md](PrepareProjects.md) or a sibling doc).

## How to run (once §3.1/§3.2 land)

Prerequisites: **run PrepareProjects first** (so `Net4x.*` = `OURS` is committed and the submodule is on the gitee-merged `HEAD`), a clean working tree, and `AlignProjects/last-aligned.txt` present (the first run already ships it pointing at the bootstrap base `e5227ae`).

```sh
dotnet run --project AlignProjects/AlignProjects.csproj
```

What a run does, in order:
1. Resolves **base** = checkpoint (`last-aligned.txt`, else `e5227ae`) and **target** = submodule `HEAD` (`main`).
2. Enriches `Translations.xml` + resx with any new Chinese as blanks, then **pauses** for you to fill the translations (ENTER to continue; skipped if nothing new).
3. Runs `RunTransform` on base and target in staging roots → `build/base` / `build/theirs` (§3.1/§3.2).
4. **Structural preflight** (§3.4) — aborts if `f(base)`'s layout drifted from `OURS`.
5. **3-way merge** the delta onto the live `Net4x.*` (§3.5).
6. On conflicts, **pauses** listing each file's Net4x path + raw upstream source; resolve, `git add`, ENTER (§3.6).
7. **Report** + residual-Chinese scan (§3.7), then **advance the checkpoint** and commit (§3.8), and clean up (§3.9).

Exit codes: `0` clean · `1` clean but residual Chinese remains · `2` structural-drift abort · `3` conflicts unresolved (or non-interactive run with conflicts).

Run interactively (a real console) so the two pauses work; a non-interactive run skips the translation pause and aborts on merge conflicts.

**Next iterations:** download newer gitee into the submodule (or let PrepareProjects' exit hook pull it), then just re-run `AlignProjects` — it aligns `checkpoint → new HEAD` and advances the checkpoint. Re-running with nothing new is a no-op.

Env overrides (all optional): `ALIGN_BASE_COMMIT`, `ALIGN_TARGET_COMMIT`, `ALIGN_SOURCE_DIR`; plus testing seams `ALIGN_REPO_ROOT` and `ALIGN_BASE_TREE`/`ALIGN_THEIRS_TREE` (run the merge/preflight/pause/checkpoint against pre-produced trees, bypassing the transform).

## Verification
- [x] Enrichment is additive: new Chinese lands as blank entries; existing entries untouched; BOM/format preserved (unit-tested + verified on the real repo).
- [x] A blank-English entry leaves its Chinese **in place** after apply (erasure guard) — unit-tested.
- [x] The pause prints counts/paths and waits (or continues when non-interactive / nothing new).
- [ ] Phase 3: `f` completes on both the base and target commits; originals untouched; worktrees removed.
- [ ] Phase 3 (determinism): `RunTransform(target)` run twice is **byte-identical** (self-check passes; residual non-determinism fails loud, not silently).
- [ ] Phase 3 (structural preflight): a layout drift between `f(base)` and `OURS` (added/removed/renamed paths) **aborts** the run with a report before anything is modified.
- [ ] Phase 3 (auto-commit): the tool commits the enriched+filled translations before the merge, and the aligned `Net4x.*` + checkpoint after; a failure mid-run restores `Net4x.*` and the submodule from git.
- [ ] Phase 3: alignment report shows the library delta applied; no unresolved `.rej`/`<<<<<<<`; no sample-app files modified.
- [ ] Phase 3: a conflicted file triggers the pause, printing the conflicted `Net4x.*` full path **and** the **exact** raw `gtksystem-windows-forms` source full path (from the provenance manifest), and waits for confirmation before continuing.
- [ ] Phase 3: the checkpoint advances to the target on success, and does **not** advance while conflicts are unresolved.
- [ ] Phase 3 (repeatable): re-running right after a clean alignment is a **no-op** (base == target → empty delta); advancing the submodule and re-running applies only the new delta.
- [ ] After alignment, **no residual Chinese** (`[一-鿿]`) remains in `Net4x.*`.
- [ ] `dotnet build` (Debug + Release) clean; `dotnet test Net4x.Gtk.Windows.Forms.sln` no new failures; `Net4x.Gtk.Drawing` still a separate assembly; MDIForm + new upstream types present.

## Risks
- **Non-deterministic `f`** — would pollute the delta with non-upstream noise. Mitigated by neutralizing non-deterministic steps (§3.1) + a two-run byte-identical self-check that fails loud.
- **Layout drift `OURS` vs `f(base)`** — would make cherry-pick add duplicates/orphans. Mitigated by the structural-match preflight (§3.4), which aborts before any change.
- **`f` breaks on the merged source** — mitigated by Phase 1 (done); guards report any remaining missing paths on first run.
- **Source mutation** — `RunTransform` renames folders in its source; under option B this happens inside a throwaway `git worktree` staging root, so the live submodule and `Net4x.*` are never mutated by the transforms.
- **Mapping a conflicted `Net4x.*` file to its raw source** — the transform moves/renames/splits files. Addressed by the **exact** provenance manifest `RunTransform` emits (§3.1); `build/theirs` is an always-exact secondary reference.
- **Resource-key additions** (new *resource* literals, not just comments) — may need new keys in `zh-Hans.resx` plus values across `en/it/fr/de/es`; the resx enricher only fills gaps for keys already in `zh-Hans`, so genuinely new resource keys remain a manual/curation step.
- **Blank-English erasing Chinese** — resolved by the Phase 2 apply guard (kept as a regression-test invariant).

## Decisions taken (this review)
- **`f` determinism** — neutralize non-deterministic sources (+ two-run self-check). §3.1
- **Structural drift** — preflight abort-guard comparing `f(base)` to `OURS`. §3.4
- **Commit workflow** — the tool auto-commits at defined checkpoints. §3.0 / §3.8
- **Provenance** — `RunTransform` emits a full, exact generated→source manifest. §3.1 / §3.6
- **`f` sharing** — extract `RunTransform` (shared by both tools), not mirror.
- **Phase 3 isolation** — option **B**: parameterize the repo root, transform into staging roots (git worktrees); the live tree is never clobbered. §3.1–3.3
- **Resx locales** — enrich **all** existing locale files, including the invariant `Resources.xml` (current `TranslationEnrichment.LocaleSuffixes` behavior — no change).

## Open decisions
None blocking — the items above are decided. Remaining choices are implementation-time details (e.g. junction vs copy for staging-root links on filesystems without symlink support; exact provenance-manifest format).

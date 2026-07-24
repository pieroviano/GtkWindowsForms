# PrepareProjects

`PrepareProjects` is the **code-generation / build pipeline** that regenerates the shippable `Net4x.Gtk.Windows.Forms` and `Net4x.Gtk.Drawing` libraries from the vendored `gtksystem-windows-forms` git submodule, then builds and (optionally) publishes the NuGet packages. It also **pins that submodule to a fixed generation base on entry** and **advances it to the latest gitee upstream on exit** (see [Source-repository management](#source-repository-management-entry--exit)).

It is a console `Exe` with no configuration file — the sequence of steps *is* the pipeline. The code is split in two:

- [PrepareProjects/TransformPipeline.cs](PrepareProjects/TransformPipeline.cs) — `TransformPipeline.RunTransform()`, the **transform `f`** (copy → translate → reorganize → split drawing → format → type-split). It is deliberately free of source-repo hooks, patching, build, and publish so that **AlignProjects runs the identical transform**. It returns a `TransformResult` carrying the paths and the deferred `updatePropsFiles` action.
- [PrepareProjects/Program.cs](PrepareProjects/Program.cs) — a thin driver: pin the submodule, `await TransformPipeline.RunTransform()`, then apply patches, persist backups, build, prune dev packages, publish, and advance the submodule.

## Why this exists

The shippable library is a maintained fork of upstream [GTKSystem.Windows.Forms](https://github.com/easywebfactory/gtksystem-windows-forms) (Chinese-language, MIT). Rather than a git fork (too many merge conflicts on every upstream sync), upstream is vendored as the `gtksystem-windows-forms` submodule and the product is *derived* from it on demand. `PrepareProjects` performs that derivation deterministically so an upstream update can be re-absorbed by re-running the tool.

> **Consequence for contributors:** files under `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` are pipeline output and can be overwritten. Durable changes belong in `patches/`, `Resources/`, the `Resources/*PostProcessing.xml` files, or the submodule. See [the repository CLAUDE.md](CLAUDE.md).

## The project file

[PrepareProjects/PrepareProjects.csproj](PrepareProjects/PrepareProjects.csproj):

| Setting | Value | Meaning |
|---|---|---|
| `OutputType` | `Exe` | Runnable console app. |
| `TargetFramework` | `net8.0-windows` | Windows-only — it drives `git`, `dotnet`, WinMerge, and `PublishNuget.exe`, and uses code-page encodings for the Chinese→UTF-8 conversion. |
| `Nullable` / `ImplicitUsings` | `enable` | — |
| `GenerateAssemblyInfo` | `false` | No generated assembly-info. |
| `NoWarn` | `CA1416;IDE0055` | Suppresses the platform-compatibility analyzer (CA1416, expected for a Windows-only tool) and a formatting rule. |
| `SkipPush` | `true` | **MSBuild property** consumed by the `Net4x.PublishNuget` package to skip pushing during a normal build. Note this is distinct from the `SkipPush` *environment variable* the program checks at runtime (see below). |

**Project references** — the pipeline draws on two library projects:

- **`Prepare.Common`** — reusable primitives: IO/backup, git patch application, `dotnet` build/format invocation, translation, ResX handling, version bumping, post-processing engine.
- **`XmlDocSync`** — Roslyn-based source rewriters and beautifiers (XML-doc merge/format, event-invocator normalization, type splitting, unused-using removal, `var` rewriting, etc.).

**Package reference** — `Net4x.PublishNuget` (v1.0.0.2) provides the packing/publishing tooling; the actual push at the end calls `PublishNuget.exe` from the NuGet package cache.

The `<Content Include>` items (`ControlExtension.cs`, `DeserializingResourceReader.cs`, `TreeView.cs` under `Resources\`, each guarded by `Condition="Exists(...)"`) are optional overlay sources copied only when present.

## How to run

```sh
# From the PrepareProjects project directory
dotnet run --project PrepareProjects/PrepareProjects.csproj
```

**Path assumptions:** the program uses relative paths such as `..\..\..\..\` to reach the repository root, resolved from the executing assembly location (i.e. `bin/<Config>/net8.0-windows/`). Running via `dotnet run` from the repo puts the binary at that depth, which is why the `../../../..` hops land on the repo root. Running the built exe from a different location will break these paths.

**`DEBUG` vs `Release` matters** — the version-suffix logic ([VersionPrepare.cs](Prepare.Common/Projects/VersionPrepare.cs)) branches on the build configuration:
- **Debug:** suffix is `<last>-a<yyyyMMddHHmmss>` (timestamped pre-release; these `*-a<year>*` packages are deleted from `Packages/` near the end).
- **Release:** suffix is `YYDDD` (2-digit year + day-of-year), a stable build number.

**Skipping the NuGet push:** at the end the program reads the `SkipPush` *environment variable* — the push runs only when it is set to an empty string (`SkipPush=""`); any other/unset value skips it. (Independent of the csproj `<SkipPush>` MSBuild property above.)

## Source-repository management (entry & exit)

The pipeline brackets its work with two operations on the `gtksystem-windows-forms` submodule, implemented in [Prepare.Common/IO/SourceRepositorySync.cs](Prepare.Common/IO/SourceRepositorySync.cs):

- **Entry — pin to the generation base.** `SourceRepositorySync.CheckoutGenerationBase()` force-checks out the frozen base commit `e5227ae…` (`GenerationBaseCommit` — the exact commit the current `Net4x.*` projects and the `patches/` series were generated from) in **detached HEAD**, so every run transforms identical source and the line-anchored patches apply. Detached on purpose: the `main` branch — which may carry a manually-resolved gitee merge — is never moved or lost. It returns the checked-out branch name for the exit step. This **force-discards any uncommitted changes** in the submodule working tree.
- **Exit — advance to latest gitee.** `SourceRepositorySync.AdvanceToLatestGitee(branch)` returns to the tracking branch (`git checkout -f main`, discarding the pipeline's in-place source mutations), aligns it with its own remote (`git pull`), then merges the newest upstream (`git pull gitee master`). Only new gitee commits are merged, so an existing resolved merge is preserved. Each pull **fails safe**: on a network error or merge conflict it runs `git merge --abort` and warns (via `RedConsole`), leaving the submodule intact rather than half-merged.

The base commit and tracking branch are `SourceRepositorySync.GenerationBaseCommit` / `DefaultTrackingBranch` (`main`).

## Pipeline stages (execution order)

Each stage prints a colored banner via `ConsoleColor.*.Write`. Stages 2–26 are the body of `TransformPipeline.RunTransform()`; stages 1 and 27–33 are `Program.Main`.

### Driver — before the transform (`Program.Main`)

1. **Pin source repository to the generation base** — `SourceRepositorySync.CheckoutGenerationBase()` (see [above](#source-repository-management-entry--exit)); detached force-checkout of the frozen base commit. Runs first, before any transformation; returns the branch to advance on exit.

### The transform `f` (`TransformPipeline.RunTransform`)

2. **Normalize submodule folders** — `PrepareWorker.RenameDirectoriesInSourceRepository()` renames case-variant folders (`GTKControls`→`GtkControls`, `GTKSystem`→`GtkSystem`) inside the submodule via a two-step move (git is case-insensitive on Windows).
3. **Register code-page encodings** — needed to read the original non-UTF-8 Chinese source.
4. **Prepare `Net4x.Gtk.Windows.Forms`** — `GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", "Net4x.Gtk.Windows.Forms")`:
   - Copies the submodule project into the target dir, patches namespaces, normalizes the netstandard symbol, renames `GTKSystem.Windows.Forms.csproj` → `Net4x.Gtk.Windows.Forms.csproj`, ensures the latest GtkSharp, copies localization resources (`Resources.*`, `MessageBoxResources.*`), translates Chinese → English, and ensures `NoWarn`.
5. **Prepare `Net4x.System.Resources.Extensions`** — `SystemResourcesExtensionsProjectHandler.Prepare(...)`.
6. **Compute & stage version** — `VersionPrepare.PrepareSolutionVersion()` reads the upstream `Version`, returns the text plus a deferred `updatePropsFiles` action that later writes `VersionSuffix` into `Directory.Build.Props` and `Net4xGtkWindowsFormsVersion` into `Directory.Nuget.Props`.
7. **Prepare PoC sample apps** — `GtkSampleAppProjectsHandler.PrepareAsync` for `GtkWinFormsApp` and `GtkSystemWinFormsApp11`.
8. **Regenerate `*.Designer.cs`** — `ResGenService.RegenerateDesignersFromResources(...)` for the sample apps.
9. **Overlay source from `Resources/`** — `ResourcesCopyService.CopyCodeFromResources(...)` copies maintained `.cs` files (e.g. `ControlExtension.cs`, `NonStaticMessageBox.cs`) into the generated tree.
10. **First post-processing + translations** — `PostProcessing.ApplyPostProcessing(...)` using `Resources/PostProcessing.xml` (submodules `gtksystem-windows-forms`, `mono` excluded from processing).
11. **Normalize comments** — `SummaryCommentTransformer` + `MemberSpacingFormatter`.
12. **Remove stale upstream XML comments** — `PrepareWorker.RemoveOldXmlComments(csprojPath)`.
13. **Reorganize files to match namespaces** — `FolderFixer.ReorganizeFiles(...)`, then `GtkFoldersService.NormalizeGtkFolders` / `RenameFolder` and `FixService.FixClasses`.
14. **Split out `Net4x.Gtk.Drawing`** — `DrawingService.SeparateDrawing(...)` extracts the `System.Drawing` shim into its own project; `OptimizeNet4xGtkDrawingFileSystem` tidies it.
15. **Substitute unimplemented types with `mono` equivalents** — e.g. `FileContentReplacer.ReplaceContent(... mono/.../CharacterRange.cs ...)`.
16. **Intermediate post-processing** — `PrepareWorker.GetSubsequentPostprocessingXml(false)` → `IntermediatePostProcessing.xml`.
17. **Regenerate `SR.Designer.cs`** — `ResGenService.RegenerateSrDesigner(...)`.
18. **Clean & persist intermediate backup** — `BackupUtility.CleanEmptyFolders/CleanBackup` then `ProjectUtility.PersistProjectFiles(..., IntermediateBackup)` (creates the `*.LatestBackup`/intermediate snapshots).
19. **Merge XML documentation** — `await PrepareWorker.MergeXmlDocumentationAsync()`.
20. **Ensure `OnXxx` raise events** — `PrepareWorker.EnsureOnEventRaisesEvent()` (Roslyn rewrite from `XmlDocSync`).
21. **Format with `dotnet format`** — `DotnetFormatter.Format(csproj, .editorconfig)` for both the Forms and Drawing projects, using the root [.editorconfig](.editorconfig).
22. **Latest post-processing** — `GetSubsequentPostprocessingXml(true)` → `LatestPostProcessing.xml`; then `CleanBackup(..., LatestBackup)`.
23. **Targeted file/comment patches** — `PrepareWorker.PatchFile` / `PatchCommentsInFile` on specific files (`FileDialog.cs`, `PrintPreviewDialog.cs`, `MessageBox.cs`, `ImageAnimator.cs`, `PrintDialog.cs`).
24. **Beautify source** — `CsBeautyfier.ProcessFolderAsync(...)` (removes unused usings; excludes `GtkSystem\Resources\ResourceManager.cs`).
25. **Normalize line endings** — `LineEndingNormalizer.NormalizeCsFiles(...)`.
26. **Split multi-type files** — `TypeSplitService.ProcessDirectory(...)` splits files containing multiple top-level types into one-type-per-file (with an explicit exclusion list, e.g. `.Designer.cs`, several `PropertyGrid.*`, `ListBox.*`, `ComboBox.*` partials).
    *(end of the transform — `RunTransform` returns a `TransformResult`; everything below is `Program.Main` again)*

### Driver — after the transform (`Program.Main`)

27. **Commit and apply patches** — `GitPatchApplier.CommitAndApplyPatches()`: `git add .`, commit if staged changes exist ("Auto commit before applying patches"), then apply every `patches/*.patch` **in sorted order** via `git apply --3way`. (Sorted order is why patches are numbered `0001-`, `0002-`, …)
28. **Persist latest project files** — `ProjectUtility.PersistProjectFiles(..., LatestBackup)` for both projects, then `BackupUtility.DeleteBackupFolders(...)`.
29. **Apply version to props & build** — invoke the deferred `updatePropsFiles()`, then `ProjectCompilation.RestoreSolution` + `BuildSolution` on the main solution and `Prepare.Common`.
30. **Prune dated dev packages** — delete `Packages/*-a<year>*` (the Debug timestamped pre-release packages).
31. **Publish** — if the `SkipPush` env var is `""`, run `%USERPROFILE%\.nuget\packages\net4x.publishnuget\1.0.0\tools\PublishNuget.exe`.
32. **Advance source repository to latest gitee** — `SourceRepositorySync.AdvanceToLatestGitee(branch)` (see [above](#source-repository-management-entry--exit)): `git checkout -f main` → `git pull` → `git pull gitee master`, each failing safe.
33. **Done** — prints "Preparation complete".

## Where durable changes live (quick reference)

| Change you want | Put it here |
|---|---|
| Behavioral code fix to the library | A new `patches/NNNN-*.patch` (create with the recipe in [Patches.txt](Patches.txt)) |
| Injected/overlaid source or translations | `Resources/*.cs`, `Resources/Translations.xml`, `Resources/*Resources.*.xml` |
| Text/structure transforms applied during prep | `Resources/PostProcessing.xml`, `IntermediatePostProcessing.xml`, `LatestPostProcessing.xml` (edit via the `UpdatePostProcessing` tool) |
| Upstream logic | The `gtksystem-windows-forms` submodule |
| Version scheme / package versions | `Directory.Build.Props`, `Directory.Nuget.Props` (written automatically in step 29) |

## External dependencies at runtime

- **`git`** on `PATH` — for the auto-commit and `git apply --3way` of patches, and for the entry/exit source-repository management (detached base checkout on entry; `checkout` + `pull` + `pull gitee master` on exit). The exit `git pull` steps also need network access and a configured `gitee` remote + branch upstream on the submodule.
- **`dotnet`** on `PATH` — for `dotnet format`, restore, and build.
- **`PublishNuget.exe`** — from the `Net4x.PublishNuget` NuGet package cache (only used when pushing).
- Initialized **submodules** — `gtksystem-windows-forms` (source of truth) and `mono` (fallback `System.Drawing` implementations).

## AlignProjects (post-pipeline)

`AlignProjects` (in `PrepareProjects.sln`) is a companion tool that runs **after** PrepareProjects to forward-port newer gitee upstream changes onto the generated `Net4x.*` projects, incrementally. Where PrepareProjects regenerates `Net4x.*` from the frozen base commit + patches, AlignProjects computes the upstream delta in *transformed space* and 3-way-merges it onto the live projects, preserving the fork's patches/manual edits.

- **Shared transform:** both tools run the same transform `f` — `TransformPipeline.RunTransform` (extracted from `Program.Main`; PrepareProjects then applies patches/build/publish, AlignProjects diffs two transformed snapshots).
- **Incremental & checkpointed:** each run aligns `last-aligned checkpoint → gitee HEAD` and advances the checkpoint (`AlignProjects/last-aligned.txt`); re-running with nothing new is a no-op.
- **Two operator pauses:** fill new translations (blank dictionary entries), and resolve merge conflicts (each shown with the `Net4x.*` path and the raw `gtksystem-windows-forms` source).
- **Status:** the driver (checkpoint, structural preflight, 3-way merge, conflict pause, report) is implemented and tested; the transform-producer half (`repoRoot` threading, staging roots, provenance, determinism) is still pending a GTK-environment validation run.

The as-built tool — stages, exit codes, env overrides, and where the code differs from the plan — is documented in [AlignProjects.md](AlignProjects.md); the design rationale and status live in [AlignProjects-Plan.md](AlignProjects-Plan.md).

## AlignSamples (post-pipeline, samples)

`AlignSamples` (in `PrepareProjects.sln`) does for the **sample apps** what AlignProjects does for the libraries: it forward-ports newer gitee upstream changes onto `GtkWinFormsApp` and `GtkSystemWinFormsApp11`, which AlignProjects' scope deliberately excludes. `AutoClosingMessageBoxApp` is fork-only and out of scope.

- **Shared driver:** both align tools run the same `Align.Common.AlignmentDriver` (staging roots, preflight, 3-way merge, conflict pause, report, checkpoint); only the options differ — scope, transform, checkpoint file, branch prefix, conflict log.
- **Shared transform `fs`:** `SampleTransformPipeline` — the sample counterpart of `f`, called by PrepareProjects *and* AlignSamples, so `BASE` matches the real base of `OURS`.
- **Independent checkpoint:** `AlignSamples/last-aligned.txt`, so the samples may lag or lead the libraries.
- **Extra duty — globalization:** new Chinese is keyed into the `zh-Hans`/invariant resource pair (or the sample dictionary, or an `.en` data sibling) *before* the transform, reviewed at an operator pause; after the merge the culture invariant is verified (key parity, no blank English, no orphan references, no stray Chinese).

The as-built tool is documented in [AlignSamples.md](AlignSamples.md); the design rationale and phase status live in [AlignSamples-Plan.md](AlignSamples-Plan.md).

## Run order

The three tools are **never chained automatically** — each is invoked by hand, in this order:

```sh
dotnet run --project PrepareProjects/PrepareProjects.csproj   # regenerate Net4x.* + samples from the frozen base, build, publish
dotnet run --project AlignProjects/AlignProjects.csproj       # absorb new upstream into the libraries
dotnet run --project AlignSamples/AlignSamples.csproj         # absorb new upstream into the samples
```

Libraries before samples: upstream sample changes may use library APIs that have not been ported yet. AlignSamples only **warns** when its base differs from AlignProjects' checkpoint — building the solution afterwards is the hard safety net. Both align tools need a clean working tree and a real console (they pause for translations and for conflicts).

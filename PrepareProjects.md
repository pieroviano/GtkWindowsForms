# PrepareProjects

`PrepareProjects` is the **code-generation / build pipeline** that regenerates the shippable `Net4x.Gtk.Windows.Forms` and `Net4x.Gtk.Drawing` libraries from the vendored `gtksystem-windows-forms` git submodule, then builds and (optionally) publishes the NuGet packages.

It is a console `Exe` — the whole pipeline is the `Main` method in [Program.cs](Program.cs), executed top to bottom. There is no configuration file; the sequence of steps *is* the pipeline.

## Why this exists

The shippable library is a maintained fork of upstream [GTKSystem.Windows.Forms](https://github.com/easywebfactory/gtksystem-windows-forms) (Chinese-language, MIT). Rather than a git fork (too many merge conflicts on every upstream sync), upstream is vendored as the `gtksystem-windows-forms` submodule and the product is *derived* from it on demand. `PrepareProjects` performs that derivation deterministically so an upstream update can be re-absorbed by re-running the tool.

> **Consequence for contributors:** files under `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` are pipeline output and can be overwritten. Durable changes belong in `patches/`, `Resources/`, the `Resources/*PostProcessing.xml` files, or the submodule. See [the repository CLAUDE.md](../CLAUDE.md).

## The project file

[PrepareProjects.csproj](PrepareProjects.csproj):

| Setting | Value | Meaning |
|---|---|---|
| `OutputType` | `Exe` | Runnable console app. |
| `TargetFramework` | `net8.0-windows` | Windows-only — it drives `git`, `dotnet`, WinMerge, and `PublishNuget.exe`, and uses code-page encodings for the Chinese→UTF-8 conversion. |
| `Nullable` / `ImplicitUsings` | `enable` | — |
| `GenerateAssemblyInfo` | `false` | No generated assembly-info. |
| `NoWarn` | `CA1416;IDE0055` | Suppresses the platform-compatibility analyzer (CA1416, expected for a Windows-only tool) and a formatting rule. |
| `SkipPush` | `true` | **MSBuild property** consumed by the `Net4x.PublishNuget` package to skip pushing during a normal build. Note this is distinct from the `SkipPush` *environment variable* the program checks at runtime (see below). |

**Project references** — the pipeline is split across three projects:

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

**`DEBUG` vs `Release` matters** — the version-suffix logic ([VersionPrepare.cs](../Prepare.Common/Projects/VersionPrepare.cs)) branches on the build configuration:
- **Debug:** suffix is `<last>-a<yyyyMMddHHmmss>` (timestamped pre-release; these `*-a<year>*` packages are deleted from `Packages/` near the end).
- **Release:** suffix is `YYDDD` (2-digit year + day-of-year), a stable build number.

**Skipping the NuGet push:** at the end the program reads the `SkipPush` *environment variable* — the push runs only when it is set to an empty string (`SkipPush=""`); any other/unset value skips it. (Independent of the csproj `<SkipPush>` MSBuild property above.)

## Pipeline stages (order of `Program.Main`)

Each stage prints a colored banner via `ConsoleColor.*.Write`. Stages, in order:

1. **Normalize submodule folders** — `PrepareWorker.RenameDirectoriesInSourceRepository()` renames case-variant folders (`GTKControls`→`GtkControls`, `GTKSystem`→`GtkSystem`) inside the submodule via a two-step move (git is case-insensitive on Windows).
2. **Register code-page encodings** — needed to read the original non-UTF-8 Chinese source.
3. **Prepare `Net4x.Gtk.Windows.Forms`** — `GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", "Net4x.Gtk.Windows.Forms")`:
   - Copies the submodule project into the target dir, patches namespaces, normalizes the netstandard symbol, renames `GTKSystem.Windows.Forms.csproj` → `Net4x.Gtk.Windows.Forms.csproj`, ensures the latest GtkSharp, copies localization resources (`Resources.*`, `MessageBoxResources.*`), translates Chinese → English, and ensures `NoWarn`.
4. **Prepare `Net4x.System.Resources.Extensions`** — `SystemResourcesExtensionsProjectHandler.Prepare(...)`.
5. **Compute & stage version** — `VersionPrepare.PrepareSolutionVersion()` reads the upstream `Version`, returns the text plus a deferred `updatePropsFiles` action that later writes `VersionSuffix` into `Directory.Build.Props` and `Net4xGtkWindowsFormsVersion` into `Directory.Nuget.Props`.
6. **Prepare PoC sample apps** — `GtkSampleAppProjectsHandler.PrepareAsync` for `GtkWinFormsApp` and `GtkSystemWinFormsApp11`.
7. **Regenerate `*.Designer.cs`** — `ResGenService.RegenerateDesignersFromResources(...)` for the sample apps.
8. **Overlay source from `Resources/`** — `ResourcesCopyService.CopyCodeFromResources(...)` copies maintained `.cs` files (e.g. `AutoClosingMessageBox.cs`, `ControlExtension.cs`) into the generated tree.
9. **First post-processing + translations** — `PostProcessing.ApplyPostProcessing(...)` using `Resources/PostProcessing.xml` (submodules `gtksystem-windows-forms`, `mono` excluded from processing).
10. **Normalize comments** — `SummaryCommentTransformer` + `MemberSpacingFormatter`.
11. **Remove stale upstream XML comments** — `PrepareWorker.RemoveOldXmlComments(csprojPath)`.
12. **Reorganize files to match namespaces** — `FolderFixer.ReorganizeFiles(...)`, then `GtkFoldersService.NormalizeGtkFolders` / `RenameFolder` and `FixService.FixClasses`.
13. **Split out `Net4x.Gtk.Drawing`** — `DrawingService.SeparateDrawing(...)` extracts the `System.Drawing` shim into its own project; `OptimizeNet4xGtkDrawingFileSystem` tidies it.
14. **Substitute unimplemented types with `mono` equivalents** — e.g. `FileContentReplacer.ReplaceContent(... mono/.../CharacterRange.cs ...)`.
15. **Intermediate post-processing** — `PrepareWorker.GetSubsequentPostprocessingXml(false)` → `IntermediatePostProcessing.xml`.
16. **Regenerate `SR.Designer.cs`** — `ResGenService.RegenerateSrDesigner(...)`.
17. **Clean & persist intermediate backup** — `BackupUtility.CleanEmptyFolders/CleanBackup` then `ProjectUtility.PersistProjectFiles(..., IntermediateBackup)` (creates the `*.LatestBackup`/intermediate snapshots).
18. **Merge XML documentation** — `await PrepareWorker.MergeXmlDocumentationAsync()`.
19. **Ensure `OnXxx` raise events** — `PrepareWorker.EnsureOnEventRaisesEvent()` (Roslyn rewrite from `XmlDocSync`).
20. **Format with `dotnet format`** — `DotnetFormatter.Format(csproj, .editorconfig)` for both the Forms and Drawing projects, using the root [.editorconfig](../.editorconfig).
21. **Latest post-processing** — `GetSubsequentPostprocessingXml(true)` → `LatestPostProcessing.xml`; then `CleanBackup(..., LatestBackup)`.
22. **Targeted file/comment patches** — `PrepareWorker.PatchFile` / `PatchCommentsInFile` on specific files (`FileDialog.cs`, `PrintPreviewDialog.cs`, `MessageBox.cs`, `ImageAnimator.cs`, `PrintDialog.cs`).
23. **Beautify source** — `CsBeautyfier.ProcessFolderAsync(...)` (removes unused usings; excludes `GtkSystem\Resources\ResourceManager.cs`).
24. **Normalize line endings** — `LineEndingNormalizer.NormalizeCsFiles(...)`.
25. **Split multi-type files** — `TypeSplitService.ProcessDirectory(...)` splits files containing multiple top-level types into one-type-per-file (with an explicit exclusion list, e.g. `.Designer.cs`, several `PropertyGrid.*`, `ListBox.*`, `ComboBox.*` partials).
26. **Commit and apply patches** — `GitPatchApplier.CommitAndApplyPatches()`: `git add .`, commit if staged changes exist ("Auto commit before applying patches"), then apply every `patches/*.patch` **in sorted order** via `git apply --3way`. (Sorted order is why patches are numbered `0001-`, `0002-`, …)
27. **Persist latest project files** — `ProjectUtility.PersistProjectFiles(..., LatestBackup)` for both projects, then `BackupUtility.DeleteBackupFolders(...)`.
28. **Apply version to props & build** — invoke the deferred `updatePropsFiles()`, then `ProjectCompilation.RestoreSolution` + `BuildSolution` on the main solution and `Prepare.Common`.
29. **Prune dated dev packages** — delete `Packages/*-a<year>*` (the Debug timestamped pre-release packages).
30. **Publish** — if the `SkipPush` env var is `""`, run `%USERPROFILE%\.nuget\packages\net4x.publishnuget\1.0.0\tools\PublishNuget.exe`.
31. **Done** — prints "Preparation complete".

## Where durable changes live (quick reference)

| Change you want | Put it here |
|---|---|
| Behavioral code fix to the library | A new `patches/NNNN-*.patch` (create with the recipe in [../Patches.txt](../Patches.txt)) |
| Injected/overlaid source or translations | `Resources/*.cs`, `Resources/Translations.xml`, `Resources/*Resources.*.xml` |
| Text/structure transforms applied during prep | `Resources/PostProcessing.xml`, `IntermediatePostProcessing.xml`, `LatestPostProcessing.xml` (edit via the `UpdatePostProcessing` tool) |
| Upstream logic | The `gtksystem-windows-forms` submodule |
| Version scheme / package versions | `Directory.Build.Props`, `Directory.Nuget.Props` (written automatically in step 28) |

## External dependencies at runtime

- **`git`** on `PATH` — for the auto-commit and `git apply --3way` of patches.
- **`dotnet`** on `PATH` — for `dotnet format`, restore, and build.
- **`PublishNuget.exe`** — from the `Net4x.PublishNuget` NuGet package cache (only used when pushing).
- Initialized **submodules** — `gtksystem-windows-forms` (source of truth) and `mono` (fallback `System.Drawing` implementations).

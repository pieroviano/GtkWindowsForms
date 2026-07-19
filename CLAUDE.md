# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repository is

`Net4x.Gtk.Windows.Forms` is a WinForms-compatibility layer implemented on top of GTK (via GtkSharp), letting existing C# WinForms apps compile once and run cross-platform (Windows, Linux, macOS) with the native Visual Studio WinForms designer. It ships as the NuGet packages **`Gtk.Windows.Forms`** (`Net4x.Gtk.Windows.Forms.csproj`) and **`Net4x.Gtk.Drawing`** (a `System.Drawing` shim split out of the main library).

This is a **maintained fork** of [GTKSystem.Windows.Forms](https://github.com/easywebfactory/gtksystem-windows-forms) (originally EasyWebFactory, MIT). Instead of a git fork (which produced too many merge conflicts), upstream is vendored as the **`gtksystem-windows-forms` git submodule** and the shippable library is *regenerated* from it by a build pipeline (see below). The `mono` submodule supplies reference implementations for unimplemented `System.Drawing` types.

## The two solutions — this is the key mental model

| Solution | Purpose |
|---|---|
| `Net4x.Gtk.Windows.Forms.sln` | **The product**: the library projects, sample apps (`GtkWinFormsApp`, `GtkSystemWinFormsApp11`), and test projects. Build/run/test this normally. |
| `PrepareProjects.sln` | **The build/generation pipeline**: `PrepareProjects` (orchestrator, see [PrepareProjects/Program.cs](PrepareProjects/Program.cs)), `Prepare.Common`, `XmlDocSync`, `UpdatePostProcessing`. Running it regenerates the library from the submodule. |

### CRITICAL: the shippable library source is generated

The contents of `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` are **produced by the prepare pipeline** from the `gtksystem-windows-forms` submodule. `PrepareProjects` performs, in order (see [Program.cs](PrepareProjects/Program.cs)): UTF-8 conversion, Chinese→English translation, multi-language resource extraction, applying `patches/*.patch`, file reorganization to match namespaces, splitting the `System.Drawing` code into `Net4x.Gtk.Drawing`, substituting unimplemented classes with `mono` equivalents, XML-doc merge, Roslyn rewrites (e.g. ensuring `OnXxx` methods raise their events), `dotnet format`, then a build and NuGet publish.

**Consequence:** hand edits to files under `Net4x.Gtk.Windows.Forms/` or `Net4x.Gtk.Drawing/` can be overwritten the next time the pipeline runs. To make a change durable, put it in the right layer:

- **Behavioral code fixes** → add/update a patch in `patches/` (there are 80+, named `NNNN-Description.patch`). New patches are created with the recipe in [Patches.txt](Patches.txt): `git format-patch -o patches/ -1 HEAD -- :(exclude)*.csproj`. Patches are applied by `GitPatchApplier.CommitAndApplyPatches()` at the end of the pipeline.
- **Injected/overlaid source & translations** → files under `Resources/` (e.g. `AutoClosingMessageBox.cs`, `ControlExtension.cs`, `MessageBoxResources.*.xml`, `Translations.xml`). `ResourcesCopyService.CopyCodeFromResources` copies these into the generated tree.
- **Post-processing rules** → the `Resources/*PostProcessing.xml` files (`PostProcessing.xml`, `IntermediatePostProcessing.xml`, `LatestPostProcessing.xml`), edited via the `UpdatePostProcessing` tool.
- **Upstream logic** → the `gtksystem-windows-forms` submodule itself.

Directories ending in `.LatestBackup` (e.g. `Net4x.Gtk.Windows.Forms.LatestBackup`) are pipeline-generated backups — not source.

## Common commands

Prerequisites: .NET SDK (net10.0 SDK required — test projects target `net10.0`) and GTK runtime for actually running the apps.

```sh
# Build / test the product
dotnet build Net4x.Gtk.Windows.Forms.sln
dotnet test  Net4x.Gtk.Windows.Forms.sln

# Run a single test project
dotnet test Net4x.Gtk.Windows.Forms.Tests/Net4x.Gtk.Windows.Forms.Tests.csproj

# Run a single test by name (xUnit)
dotnet test Net4x.Gtk.Windows.Forms.Tests/Net4x.Gtk.Windows.Forms.Tests.csproj --filter "FullyQualifiedName~SomeTestName"

# Run a sample app
dotnet run --project GtkWinFormsApp/GtkWinFormsApp.csproj

# Regenerate the library from the submodule + publish (destructive to generated dirs).
# Set SkipPush to a non-empty value to skip the NuGet push at the end.
dotnet run --project PrepareProjects/PrepareProjects.csproj
```

Test projects (all xUnit + `Microsoft.NET.Test.Sdk`): `Net4x.Gtk.Windows.Forms.Tests`, `Net4x.Gtk.Drawing.Tests`, `Gtk.Windows.FormsDesigner.Tests`, `Net4x.ResxProcessor.Tests`, `AutoClosingMessageBox.Tests`, and `Prepare.Tests` (tests the pipeline, in `PrepareProjects.sln`).

## Build configuration

- **Central versioning & package pinning** live in [Directory.Nuget.Props](Directory.Nuget.Props) (a large generated-ish property/version manifest) and [Directory.Build.Props](Directory.Build.Props). Package version is `Net4xGtkWindowsFormsVersion` (currently `1.3.24`) + `VersionSuffix`; `GtkSharpLatestVersion` (currently `3.24.24`) pins GtkSharp. Prefer editing these variables over hardcoding versions in individual csproj files.
- **Target frameworks**: the shippable library is `netstandard2.0` (deliberately not tied to a specific .NET version). Tests target `net10.0`. Tooling targets modern .NET.
- **Signing**: assemblies are strong-name signed with `GtkForms.snk`; `InternalsVisibleTo` for test projects is declared with the matching public key.
- **NuGet feeds** ([Nuget.Config](Nuget.Config)): a local `Packages` folder (symlink) plus nuget.org. Built packages are emitted to `$(SolutionDir)Packages/`.

## Code architecture

- **`Net4x.Gtk.Windows.Forms`** — the WinForms API surface. Public types live under `System/` mirroring the real `System.Windows.Forms` namespaces; GTK-backed implementations live under `GtkControls/` and `GtkSystem/`. The pattern throughout: public WinForms classes wrap GtkSharp widgets and translate GTK signals into WinForms events with compatible delegate signatures (e.g. `RichTextBoxBase` wraps `Gtk.TextView`). Layout/rendering rules are reimplemented on top of GTK's measure/allocate cycle. Image assets are `EmbeddedResource`s under `Resources/System/` and are loaded at runtime by assembly resource name (`Resources.System.<file>`).
- **`Net4x.Gtk.Drawing`** — `System.Drawing` compatibility shim (brushes, `Matrix`, `StringFormat`, etc.) over GTK/Cairo, referenced by the Forms library. Referenced in test projects with the `gtk` extern alias to disambiguate from real `System.Drawing`.
- **`Net4x.Gtk.Windows.FormsDesigner`** / **`Net4x.System.Resources.Extensions`** — design-time support and a `System.Resources.Extensions` reimplementation (many patches target ResX reading/serialization).
- **`Net4x.ResxProcessor`** — a tool packaged with the library (shipped under `tools/`) for resource processing; regenerates `.Designer.cs` from resources during the pipeline.

## Conventions (from `.github/copilot-instructions.md`)

- Gradient brushes (`PathGradientBrush`) should mimic `System.Drawing` semantics — use `Matrix` operations and clone semantics.
- On `Gdk.Pixbuf.SaveToBuffer` failure, catch `GLib.GException`, fall back to null `PixbufData`, and log via `System.Diagnostics.GtkLogger.Debug`.
- Public members require XML documentation (the pipeline merges and enforces this); when fixing docs on fields, handle `FieldDeclaration` by inspecting `VariableDeclarator` symbols.
- Formatting is enforced by the root [.editorconfig](.editorconfig) via `dotnet format` during the pipeline.

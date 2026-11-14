# Plan — `Gtk.Windows.Forms.Base` consumer-build targets

Add two build-time behaviours to the `Gtk.Windows.Forms.Base` package so a plain WinForms
project that adds the package reference builds and runs cross-platform:

1. **Strip the Windows TFM flavour + drop real WinForms** — rewrite the consumer `.csproj`:
   `net*-windows` → `net*`, remove `<UseWindowsForms>true</UseWindowsForms>`.
2. **Provide native GTK 4** on Linux/macOS/Windows, shipped **inside the package** and deployed
   to the consumer output **only if not already present** (no apt/brew download).

Decisions already taken (do not re-ask):

| Topic | Decision |
|---|---|
| TFM/UseWindowsForms change | **Rewrite the `.csproj` on disk** (extend the existing ResxProcessor tool) |
| Scope | **Always**, every consumer, every OS |
| Native GTK source | **Bundle in the package**; binaries **to be sourced** (see blocking step) |
| GTK deploy trigger | **Only if not already present** |

---

## ⛔ Blocking verification — settle before any code

### B1. A build `<Target>` cannot change the framework of the build in progress
`TargetFramework` / `UseWindowsForms` are consumed during **evaluation**, long before any target
runs. On Linux a consumer whose csproj hardcodes `net10.0-windows` fails at **restore/eval**
(`Microsoft.WindowsDesktop.App` has no Linux pack) — *before* our rewrite target could fire. So:

- Rewriting the csproj at build time fixes the **next** build, not the one that triggered it.
- **Required decision:** is the rewrite acceptable as a *one-time conversion* (first add of the
  package converts the project; a second restore/build is clean), or must conversion happen before
  the consumer's first restore? MSBuild offers **no** pre-evaluation in-build hook, so "before first
  restore" means a separate `dotnet gtk-convert <proj>`-style command, not a package target.
- Recommendation: ship the rewrite on the earliest hook (`BeforeTargets="Restore;CollectPackageReferences"`),
  make it **idempotent**, and document the one-time re-run. Confirm this is acceptable.

### B2. Source of native GTK 4 binaries (the real cost of Part 2)
There is **no** drop-in redistributable GTK4 bundle for Linux/macOS equivalent to the Windows
gvsbuild zip that `GtkSharp.targets` already downloads. Sourcing is per-platform and pulls a large
transitive closure (glib, gio, cairo, pango, harfbuzz, gdk-pixbuf, graphene, fontconfig, freetype,
epoxy, …). Ecosystem tools: `gtk-mac-bundler` (macOS), `linuxdeploy-plugin-gtk` (Linux/AppImage).

- **Required inputs before packing is wired:** exact RID list (`win-x64`, `linux-x64`,
  `osx-x64`, `osx-arm64`?), where each platform's binaries come from, and the full dependency
  closure per RID. Package size will grow by ~100–300 MB/RID — confirm this is wanted in the
  shipped `.nupkg` vs. a separate `Gtk.Windows.Forms.Native.<rid>` package.
- Licensing: GTK is LGPL; dynamic bundling is fine and matches the repo's LGPL v3.

---

## Current state (file:line)

| What | Where |
|---|---|
| Consumer build hook (packed) | `Gtk.Windows.Forms/Gtk.Windows.Forms.Base.targets:2` — `PrepareBuild` runs `tools/ResxProcessor.exe "<projectDir>"` at `BeforeBuild` |
| Early-eval props (packed) | `Gtk.Windows.Forms/Gtk.Windows.Forms.Base.props` |
| Packed to build/ + buildTransitive/ | `Gtk.Windows.Forms/Gtk.Windows.Forms.csproj:97-98` |
| Tool packed under `tools/` | `Gtk.Windows.Forms/Gtk.Windows.Forms.csproj:314` (`ResxProcessor/bin/$(Configuration)/net8.0/**`) |
| ResxProcessor entry | `ResxProcessor/Program.cs:26` — **only edits `.resx`; never touches `.csproj` today** |
| Windows GTK provisioning (reference pattern) | `Net4x.GtkSharp` `build/GtkSharp.targets` — `InstallGtk` downloads gvsbuild zip to `%LOCALAPPDATA%\Gtk\4.22.4`, sentinel `bin/gtk-4-1.dll`, gated `$(OS)==Windows_NT`, skip via `SkipGtkInstall` |
| CI native GTK (reference) | `.github/workflows/build-and-test.yml:64-74` — apt `libgtk-4-1` (Linux), brew `gtk4` (macOS) |

Note: `Gtk.Windows.Forms/` is pipeline-generated, **but** `Base.props`/`Base.targets` are
git-tracked, not gitignored, and not re-emitted by the pipeline — safe to edit directly.

---

## Part 1 — csproj rewrite

**New tool behaviour (in `ResxProcessor`, invoked with the project file path):**
- Load the `.csproj` (XML).
- For every `TargetFramework`/`TargetFrameworks` whose value matches `net<ver>-windows`, drop the
  `-windows` (and any `-windows10.0.x` moniker) → `net<ver>`.
- Remove `<UseWindowsForms>true</UseWindowsForms>` (and `<UseWPF>` if present — confirm).
- Idempotent: no-op when already converted. Preserve formatting/BOM as much as `XDocument` allows.
- Guard: only rewrite a project that references this package (it is, by construction of the target).

**Wiring (`Base.targets`):** add a second target on the earliest hook, passing the project path.
Invoke the ResxProcessor **apphost** (`tools/ResxProcessor[.exe]`), not `dotnet <dll>`: the latter
names the process `dotnet` and ResxProcessor's instance guard (`Processes.cs`) then deadlocks waiting
on the build's own dotnet nodes (same trap fixed in the sample `PrepareBuild` targets). Keep the
invocation a single-hyphen-free XML comment (`--` is illegal in XML comments, MSB4025).
```xml
<Target Name="GtkConvertProject" BeforeTargets="Restore;CollectPackageReferences"
        Condition="'$(DesignTimeBuild)' != 'true' and '$(SkipGtkConvert)' != 'true'">
  <Exec Command="&quot;$(MSBuildThisFileDirectory)../tools/ResxProcessor$(_GtkAppHostExt)&quot; --convert-project &quot;$(MSBuildProjectFullPath)&quot;" />
</Target>
```
(`_GtkAppHostExt` = `.exe` on Windows, empty elsewhere.) Consequence of B1: the first build after
adding the package rewrites the file; re-run to build clean.

## Part 2 — bundle + deploy native GTK

**Packaging:** add `runtimes/<rid>/native/**` to the package (or a sibling
`Gtk.Windows.Forms.Native.<rid>` package referenced by `Base.props`). Content sourced per B2.

**Deploy target (`Base.targets`), only-if-missing:**
```xml
<Target Name="GtkDeployNative" AfterTargets="ResolveProjectReferences" BeforeTargets="Build"
        Condition="'$(DesignTimeBuild)' != 'true'">
  <!-- Detect existing GTK4: Windows sentinel %LOCALAPPDATA%\Gtk\...\bin\gtk-4-1.dll (GtkSharp.targets),
       Linux libgtk-4.so.1 on loader path, macOS libgtk-4.1.dylib. Copy packaged runtimes/<rid>/native
       into $(OutDir) only when none found. Keep in lockstep with GtkSharp GtkDir + GLibrary.SetDllDirectory. -->
</Target>
```
- Reuse GtkSharp's `SkipGtkInstall`/`GtkDir` contract on Windows rather than duplicating the download.
- On Linux/macOS set the loader path to `$(OutDir)` (or deploy beside the apphost) so
  `Application.Init()` resolves the bundled libs; verify against `GLibrary` resolution in the repo.

---

## Verification

| Step | Check |
|---|---|
| Part 1 unit test | `ResxProcessor.Tests`: feed a csproj with `net10.0-windows` + `UseWindowsForms` → assert `net10.0`, no `UseWindowsForms`; idempotent on re-run. **Fails before the tool change.** |
| Part 1 e2e | A throwaway `net10.0-windows` WinForms project refs the package → builds on Linux after one conversion pass. |
| Part 2 | On a runner with **no** system GTK, the consumer app's `Application.Init()` succeeds using only deployed libs; with system GTK present, nothing is copied. |
| Regression | Existing samples + `dotnet build Gtk.Windows.Forms.sln` stay green on all 3 OSes. |

## Risks

| Risk | Mitigation |
|---|---|
| B1: first-build-too-late | One-time conversion documented; idempotent; `SkipGtkConvert` escape hatch. |
| Rewriting a consumer's csproj is destructive/surprising | Idempotent, minimal diff, opt-out property, clear log line. |
| B2: package bloat (100s MB/RID) | Prefer per-RID `Native` satellite packages; let RID restore pull only the needed one. |
| Native dep closure incomplete → runtime dlopen failure | Enumerate full closure per RID in B2; test on clean runners. |
| Duplicating GtkSharp's Windows download | Defer to `GtkSharp.targets` on Windows; bundle only Linux/macOS. |

## Open questions for you
1. B1: accept one-time conversion (recommended) or require a separate pre-restore convert command?
2. B2: which RIDs, and bundle in-package vs. satellite `Native.<rid>` packages?
3. Also strip `<UseWPF>` / Windows-only `<FrameworkReference>`s, or only `UseWindowsForms`?

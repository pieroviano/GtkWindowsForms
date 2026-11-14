# Plan — `Gtk.Windows.Forms.Base` consumer-build target (csproj rewrite)

Make a plain WinForms project that adds the `Gtk.Windows.Forms.Base` package build cross-platform by
**rewriting the consumer `.csproj`**: `net*-windows` → `net*`, and remove
`<UseWindowsForms>true</UseWindowsForms>`.

**Native GTK 4 runtime install is out of scope here** — it will be handled in **GtkSharp4** by
extending its `GtkSharp.targets` `InstallGtk` target (today Windows-only, downloads the gvsbuild zip
to `%LOCALAPPDATA%\Gtk\<ver>`) to Linux/macOS. That package is the right layer and a separate repo;
nothing about native GTK is added to `Gtk.Windows.Forms.Base`.

Decisions already taken (do not re-ask):

| Topic | Decision |
|---|---|
| TFM/UseWindowsForms change | **Rewrite the `.csproj` on disk** (extend the existing ResxProcessor tool) |
| Scope | **Always**, every consumer, every OS |
| Native GTK runtime | **Delegated to GtkSharp4** (`GtkSharp.targets`), not this package |

---

## ⛔ Blocking decision — settle before any code

### A build `<Target>` cannot change the framework of the build in progress
`TargetFramework` / `UseWindowsForms` are consumed during **evaluation**, long before any target
runs. On Linux a consumer whose csproj hardcodes `net10.0-windows` fails at **restore/eval**
(`Microsoft.WindowsDesktop.App` has no Linux pack) — *before* our rewrite target could fire. So:

- Rewriting the csproj at build time fixes the **next** build, not the one that triggered it.
- **Required decision:** accept the rewrite as a *one-time conversion* (first add of the package
  converts the project; a second restore/build is clean), or require conversion before the
  consumer's first restore? MSBuild offers **no** pre-evaluation in-build hook, so "before first
  restore" means a separate `dotnet gtk-convert <proj>`-style command, not a package target.
- Recommendation: ship the rewrite on the earliest hook (`BeforeTargets="Restore;CollectPackageReferences"`),
  make it **idempotent**, document the one-time re-run. Confirm this is acceptable.

---

## Current state (file:line)

| What | Where |
|---|---|
| Consumer build hook (packed) | `Gtk.Windows.Forms/Gtk.Windows.Forms.Base.targets:2` — `PrepareBuild` runs `tools/ResxProcessor.exe "<projectDir>"` at `BeforeBuild` |
| Early-eval props (packed) | `Gtk.Windows.Forms/Gtk.Windows.Forms.Base.props` |
| Packed to build/ + buildTransitive/ | `Gtk.Windows.Forms/Gtk.Windows.Forms.csproj:97-98` |
| Tool packed under `tools/` | `Gtk.Windows.Forms/Gtk.Windows.Forms.csproj:314` (`ResxProcessor/bin/$(Configuration)/net8.0/**`) |
| ResxProcessor entry | `ResxProcessor/Program.cs:26` — **only edits `.resx`; never touches `.csproj` today** |
| ResxProcessor instance guard | `ResxProcessor/Processes.cs:22` — waits on same-named processes; must stay invoked via apphost |

Note: `Gtk.Windows.Forms/` is pipeline-generated, **but** `Base.props`/`Base.targets` are
git-tracked, not gitignored, and not re-emitted by the pipeline — safe to edit directly.

---

## Change

**New tool mode (in `ResxProcessor`, e.g. `--convert-project <csproj>`):**
- Load the `.csproj` (XML).
- For every `TargetFramework`/`TargetFrameworks` value matching `net<ver>-windows[10.0.x]`, drop the
  `-windows` moniker → `net<ver>`.
- Remove `<UseWindowsForms>true</UseWindowsForms>` (and `<UseWPF>` — **confirm**, open Q2).
- Idempotent: no-op when already converted. Preserve formatting/BOM as far as `XDocument` allows.
- Keep it ≤200 lines / its own class; today ResxProcessor is `Program.cs` + `Processes.cs`.

**Wiring (`Base.targets`):** add a second target on the earliest hook, passing the project path.
Invoke the ResxProcessor **apphost** (`tools/ResxProcessor[.exe]`), not `dotnet <dll>`: the latter
names the process `dotnet`, and ResxProcessor's instance guard (`Processes.cs`) then deadlocks
waiting on the build's own dotnet nodes (the trap just fixed in the sample `PrepareBuild` targets).
Do not write `--` inside the XML comment (illegal, MSB4025).
```xml
<Target Name="GtkConvertProject" BeforeTargets="Restore;CollectPackageReferences"
        Condition="'$(DesignTimeBuild)' != 'true' and '$(SkipGtkConvert)' != 'true'">
  <Exec Command="&quot;$(MSBuildThisFileDirectory)../tools/ResxProcessor$(_GtkAppHostExt)&quot; --convert-project &quot;$(MSBuildProjectFullPath)&quot;" />
</Target>
```
(`_GtkAppHostExt` = `.exe` on Windows, empty elsewhere.) Consequence of the blocking decision: the
first build after adding the package rewrites the file; re-run to build clean.

---

## Verification

| Step | Check |
|---|---|
| Unit test | `ResxProcessor.Tests`: feed a csproj with `net10.0-windows` + `UseWindowsForms` → assert `net10.0`, no `UseWindowsForms`; idempotent on re-run. **Fails before the tool change.** |
| e2e | A throwaway `net10.0-windows` WinForms project refs the package → builds on Linux after one conversion pass. |
| Regression | Existing samples + `dotnet build Gtk.Windows.Forms.sln` stay green on all 3 OSes. |

## Risks

| Risk | Mitigation |
|---|---|
| First-build-too-late (blocking decision) | One-time conversion documented; idempotent; `SkipGtkConvert` escape hatch. |
| Rewriting a consumer's csproj is destructive/surprising | Idempotent, minimal diff, opt-out property, clear log line. |
| Apphost not present when target runs | Same ordering lesson as the samples: ensure the tool apphost ships under `tools/` and is invoked by path, not rebuilt. |
| XML comment `--` / per-RID apphost name | No `--` in comments; `_GtkAppHostExt` handles the Windows `.exe`. |

## Open questions for you
1. Blocking: accept one-time conversion (recommended) or require a separate pre-restore convert command?
2. Also strip `<UseWPF>` / Windows-only `<FrameworkReference>`s, or only `UseWindowsForms`?
3. Should I implement this Part now, or hold until GtkSharp4's native-install change lands?

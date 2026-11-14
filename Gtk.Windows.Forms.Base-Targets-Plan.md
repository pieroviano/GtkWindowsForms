# Plan — `Gtk.Windows.Forms.Base` consumer-build target (csproj rewrite)

**Status: implemented.** `ResxProcessor --convert-project <csproj>` +
`Gtk.Windows.Forms.Base.targets` `GtkConvertProject` target, covered by `CsprojConverterTests`.

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

**Tool mode (`ResxProcessor --convert-project <csproj>`)** — `ResxProcessor/CsprojConverter.cs`:
- Textual, minimal-diff transform (`Transform`, pure + unit-tested) over the raw `.csproj`:
  - strip `-windows[10.0.x]` from each `TargetFramework`/`TargetFrameworks` moniker → `net<ver>`;
  - remove the whole `<UseWindowsForms>true</UseWindowsForms>` line.
- Idempotent (no-op when already converted); preserves surrounding text and the UTF-8 BOM state.
- `Program.DoWork` dispatches `--convert-project` **before** the resx path and **runs no instance
  guard** — so invoking as `dotnet <dll>` is safe (see wiring).
- `<UseWPF>` is **not** stripped (open Q2) — one extra regex when confirmed.

**Wiring (`Base.targets`)** — new `GtkConvertProject` target:
```xml
<Target Name="GtkConvertProject" BeforeTargets="BeforeBuild"
        Condition="'$(DesignTimeBuild)' != 'true' and '$(SkipGtkConvert)' != 'true'">
  <Exec Command='dotnet "$(MSBuildThisFileDirectory)../tools/ResxProcessor.dll" --convert-project "$(MSBuildProjectFullPath)"' />
</Target>
```
Two reasoned deviations from the draft:
- **`dotnet <dll>`, not the apphost.** The draft feared the `Processes.cs` deadlock, but convert mode
  runs no guard, so `dotnet <dll>` cannot deadlock — and it is cross-platform (the packed `tools/`
  apphost is whatever RID packed the library, typically Windows-only). Still: no `--` in the XML
  comment (MSB4025).
- **`BeforeBuild`, not `Restore`.** A package's `build/*.targets` is only imported *after* the first
  restore (it comes from the generated `nuget.g.targets`), so a `BeforeTargets="Restore"` hook never
  fires on first add. `BeforeBuild` is the earliest reliably-imported consumer hook.

Consequence of the blocking decision: the first build after adding the package rewrites the file;
re-run to build clean. A consumer whose first evaluation *fails* (hardcoded `net*-windows` on Linux,
no WindowsDesktop pack) never reaches `BeforeBuild` — it must run `ResxProcessor --convert-project`
once by hand, or convert on Windows/CI first. Documented limitation, not fixable from a build target.

---

## Verification

| Step | Check | Result |
|---|---|---|
| Unit tests | `CsprojConverterTests`: strip moniker (plain + versioned), multi-TFM, idempotent, no-op on cross-platform project, minimal diff | ✅ 6/6 |
| Regression | Full `ResxProcessor.Tests` | ✅ 26/26 |
| CLI e2e | `dotnet run ... -- --convert-project <tmp>` rewrites `net10.0-windows`+`UseWindowsForms` → `net10.0` | ✅ |
| Consumer e2e (not runnable here) | A `net*-windows` WinForms project refs the package → builds on Linux after one conversion pass | pending external consumer |

## Risks

| Risk | Mitigation |
|---|---|
| First-build-too-late (blocking decision) | One-time conversion documented; idempotent; `SkipGtkConvert` escape hatch. |
| Rewriting a consumer's csproj is destructive/surprising | Idempotent, minimal diff, opt-out property, clear log line. |
| Tool not present / not cross-platform | Invoke the packed `tools/ResxProcessor.dll` via `dotnet` (RID-agnostic), not a per-RID apphost. |
| Convert-mode deadlock | Convert mode runs no `Processes` instance guard, so `dotnet <dll>` is safe. |

## Open questions for you
1. ~~Blocking: one-time conversion?~~ **Done** — one-time, idempotent, `SkipGtkConvert` opt-out.
2. Also strip `<UseWPF>` / Windows-only `<FrameworkReference>`s, or only `UseWindowsForms`? **Not done** — currently only `UseWindowsForms`.
3. ~~Implement now?~~ **Done.**

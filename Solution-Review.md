# Solution review — `Net4x.Gtk.Windows.Forms.sln`

**Type:** architectural + implementation analysis, written in the shape of a pull-request review.
**Scope:** the product solution [Net4x.Gtk.Windows.Forms.sln](Net4x.Gtk.Windows.Forms.sln) — the six shipped
libraries, the sample apps, and the seven test projects. The generation pipeline
(`PrepareProjects.sln`) is out of scope except where it constrains the product.
**Basis:** static reading of the sources, a full `dotnet restore` + `dotnet build` of the solution, a
`dotnet pack` of the main package with its `.nuspec` inspected, inspection of the assemblies that land
beside the sample apps, and a full run of every test project that does not need a display.

Companion documents, not duplicated here: [Architecture.md](Architecture.md) (how the runtime works),
[Solution-Projects.md](Solution-Projects.md) (per-project inventory), [EMULATION-GAPS.md](EMULATION-GAPS.md)
(API-fidelity gaps vs. real WinForms), [CLAUDE.md](CLAUDE.md) (the pipeline). This document is about
**structure, engineering quality and risk**, and deliberately avoids re-listing emulation gaps.

---

## 1. Verdict

| | |
|---|---|
| **Overall** | Approve with comments. The architecture is sound and unusually well-documented for a compatibility shim; the defects found are localized, not structural. |
| **Build** | ✅ `dotnet restore` + `dotnet build Net4x.Gtk.Windows.Forms.sln` → 0 errors, 0 warnings. |
| **Tests** | ✅ 3,696 passing across six projects; `GtkWinFormsApp.UITests` (10 more) needs a desktop session and was not run. |
| **Should fix** | 1 item — the `System.Resources.Extensions` displacement does not take effect (§5.1); a product decision rather than a defect to patch. |
| **Consider** | the band approximation when clipping to a curved path (§4.2); `Graphics.GetContextInfo()` handing out the live Cairo context (§4.4). |

**Scale for context.** ~127 kLOC of generated library source (99.9 k Forms + 27.4 k Drawing, 681 + 131
public types), against ~43 kLOC of tests. That test-to-code ratio is the single most reassuring number in
this repository, and — §6 — the tests assert behaviour rather than coverage.

---

## 2. Architecture — what the solution actually is

Three layers, cleanly separated, and the separation is real rather than nominal:

```
        user WinForms app (unchanged .cs / .Designer.cs / .resx)
                    │                            │
   System.Windows.Forms.*            System.Drawing.*
                    ▼                            ▼
      Net4x.Gtk.Windows.Forms  ──────►   Net4x.Gtk.Drawing
      (pkg Gtk.Windows.Forms)            (pkg Gtk.Drawing)
                    │                            │
              wraps Gtk.Widget            draws via Cairo.Context
                    ▼                            ▼
                       GtkSharp 3.24.x → GTK/GDK/Cairo/Pango
```

Around that core sit four support assemblies: `Net4x.System.Resources.Extensions`,
`Net4x.Gtk.Windows.FormsDesigner` (MSBuild-integrated design-time support), `Net4x.ResxProcessor`
(`net8.0` tool shipped inside the main package under `tools/`), and `Net4x.Gtk.Windows.Extensions`. Two of
those four are empty — see §5.1.

### 2.1 The central pattern is the right one

Every control is a **facade/base pair**: a public `System.Windows.Forms.X` type owning WinForms
semantics, and a `GtkSystem.Windows.Forms.GtkControls.ControlBase.XBase` type owning the GTK widget.
Cross-cutting painting, CSS-class and background/image concerns are factored out into a single
composable helper,
[GtkControlOverride.cs](Net4x.Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/GtkControlOverride.cs),
attached to each base rather than inherited.

This is the correct decomposition for the problem. It keeps the substrate swappable in principle, keeps
`Control` free of per-widget GTK trivia, and makes the paint pipeline a single auditable place
(`OnDrawnBackground` → `OnDrawnImage` → `OnPaint`, raising both a Cairo-level `PaintGraphics` and a
`System.Drawing`-level `Paint`). The `ControlBase` layer is only ~3.9 kLOC across 30 files — proof that
the abstraction carries its weight instead of being bypassed.

### 2.2 Threading is modelled properly, which is rare in this class of project

[Control.cs:1329](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1329) onwards is the
best code in the repository. `InvokeRequired` compares against a **per-control owning thread id**
(matching WinForms' per-handle rule, not a global "is this the UI thread"); `MarshalToUiThread` posts to
`GLib.Idle`, blocks on a `ManualResetEventSlim`, and rethrows with `ExceptionDispatchInfo` while
unwrapping `TargetInvocationException` — the exact `Invoke` contract. `BeginInvoke` posts to the GTK loop
and returns a `TaskCompletionSource.Task` as its `IAsyncResult`.

The deadlock edge is handled explicitly: with no running main loop (`Gtk.Main.Level() == 0`) the work runs
inline, because nothing would ever drain the idle queue. That is the kind of detail that only gets written
after someone has actually hit the hang.

`Application.Run(ApplicationContext)`
([Application.cs:604](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs#L604)) is
similarly disciplined: single-loop-per-thread enforcement, `Control.CaptureUiThread()`, context
save/restore, and `ThreadExit`/context-dispose/`ApplicationExit` ordering in a `finally`.

The unhandled-exception path is defined rather than left to chance. `Application.ThreadException` and
`SetUnhandledExceptionMode` exist, and — the part that actually matters — something feeds them:
`InstallUnhandledExceptionHook` subscribes to `GLib.ExceptionManager.UnhandledException`, which is how an
exception thrown inside a click handler leaves managed code on this substrate. The policy itself is a
pure function, `Application.Decide(mode, hasHandler)`, kept apart from the GLib plumbing so it can be
read and tested without a message loop; `ApplicationThreadExceptionFunctionalTests` walks the whole
matrix. The default (`Automatic` with no handler) deliberately does *not* swallow — silently continuing
after an unhandled exception hides the failure, which is the opposite of what a last line of defence is
for.

### 2.3 Comments explain *why*, and that is a genuine asset

The generated code carries an unusual density of rationale comments, and they are the load-bearing kind:

- [Control.cs:61](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L61) — why the
  `CreateGraphics` surface must *not* derive from `Widget.Window` (upstream's version corrupts GTK
  rendering during a modal dialog's realize path).
- [Graphics.cs:36](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs#L36) — why the Cairo surface/context
  fields are not `static` (two `Graphics` over two bitmaps could not otherwise coexist, which is exactly
  what double buffering needs).
- [Region.cs:23](Net4x.Gtk.Drawing/System/Drawing/Region.cs#L23) — why the region is a band list rather
  than one rectangle, and which case (`Xor` of partially overlapping rectangles) forced it.

Because the shipped source is regenerated, these comments are the only durable record of hard-won
substrate knowledge. Keep enforcing that habit in patches.

### 2.4 The structural constraint is the generation pipeline

`Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` are **build outputs that are committed**. Every durable
change must be expressed as a `patches/*.patch`, a `Resources/` overlay, a post-processing rule, or a
submodule change. This is a deliberate and, given the merge-conflict history with upstream, a defensible
trade — but it means the repository has an unusual failure mode: *a correct fix that is not captured in the
right layer is silently temporary.* §3 covers how well that is currently defended.

---

## 3. Generation & change durability

### 3.1 ✅ The regeneration invariants are themselves tested

`GtkWinFormsApp.UITests` depends on the sample's designer fields being `internal` plus an
`InternalsVisibleTo` grant — both of which the pipeline must reproduce on every regeneration. Rather than
hoping, `Prepare.Tests` asserts it, so a regeneration that dropped either fails a test instead of quietly
breaking the suite. The right instinct applied in the right place.

### 3.2 ✅ The pipeline refuses to destroy uncaptured work

[GeneratedTreeGuard.cs](Prepare.Common/IO/GeneratedTreeGuard.cs), called first thing in
[Program.cs](PrepareProjects/Program.cs#L13), inspects `git status --porcelain` before the transform runs
and aborts if uncommitted edits are sitting in a generated tree — naming each file and pointing at the
patch recipe. Without it, the transform rewrites those files from the submodule and the edits are gone
with no message at all. The parsing half is pure and unit-tested
([GeneratedTreeGuardTests.cs](Prepare.Tests/GeneratedTreeGuardTests.cs)), including renames, staged vs.
untracked, and the `.csproj` exclusion that mirrors [Patches.txt](Patches.txt).

`AllowDirtyGeneratedTrees=1` is the deliberate override.

### 3.3 ✅ The patch series is ordered unambiguously and applies loudly

95 patches, applied in filename order. Two guards sit on that:

- **Unique ordinals.** `GitPatchApplier.EnsureOrdinalsAreUnique` throws if two patches share a leading
  number, because their relative order would then be decided by their *description text* rather than by
  intent — a trap the first time two same-numbered patches touch the same hunk. Where a number genuinely
  needs a second patch, the letter-suffix convention (`0007-…`, `0007a-…`) keeps the intended order under
  a plain sort. [GitPatchApplierOrdinalTests.cs](Prepare.Tests/GitPatchApplierOrdinalTests.cs) asserts
  both the rule and that the committed series satisfies it.
- **Failures are reported.** A patch that does not apply is collected and listed in red at the end of the
  run, with an explicit "the generated library is missing the fixes above" — rather than being swallowed
  by a discarded return value.

### 3.4 ✅ The generated-tree changes are captured

Every behavioural change made to `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` has a patch behind
it, so a regeneration reproduces it rather than silently dropping it: `0089` covers the disposal,
clipping and culture work, `0090` the `Disposed` event, the unhandled-exception policy and the z-order
methods. Both were produced with the [Patches.txt](Patches.txt) recipe, and each body is the commit diff
verbatim — so they apply to a freshly transformed tree by construction.

---

## 4. Implementation findings

### 4.1 🟢 `Control` raises `Disposed`, and the reason it once could not is worth knowing

`Control` overrides `CanRaiseEvents`, and it must return `true`
([Control.cs:1854](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1854)). The override
being an auto-property — permanently `false` — is not inert, which is the interesting part:
`EventHandlerList` holds a reference to the component that owns it and its **indexer getter** returns
`null` whenever that owner reports it cannot raise events. `Component` stores the `Disposed`
subscription in exactly that list. So `+=` succeeded, the handler was genuinely in the list, and every
lookup returned `null` — `Component.Dispose` found nothing to invoke and the event never fired for any
control, through any entry point.

It is a good example of a defect that reads as impossible from either end alone: the handler *is* added,
and `Dispose` *is* reached. Only the interaction explains it, and only for a type that overrides
`CanRaiseEvents`.

`Disposed` firing exactly once — on direct disposal, through `IDisposable`, and on children reached by a
parent's walk — is covered by
[ControlDisposalFunctionalTests.cs](Net4x.Gtk.Windows.Forms.Tests/System/Windows/Forms/ControlDisposalFunctionalTests.cs),
along with the rest of the disposal contract: `Dispose(bool)` guards re-entry on `IsDisposed`, honours
`disposing` before touching finalizable Cairo objects, and walks each descendant once.

### 4.2 🟢 Clipping is coherent, with one documented approximation

Every `SetClip`/`IntersectClip` overload funnels through
[SetClip(Region, CombineMode)](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs#L2374): combining happens on
the `Region` — which implements the whole `CombineMode` family exactly for rectangular inputs — and the
result is applied to the Cairo context in one pass by `ApplyClip`. This is the right shape, because Cairo's
own `Clip()` can only ever *narrow*, so `Union`, `Exclude`, `Xor` and `Complement` are not expressible
incrementally; computing the resulting region first sidesteps that entirely. `Clip`, `ClipBounds` and the
Cairo state are updated together, so they cannot drift apart, and an unknown `CombineMode` throws rather
than guessing.

The approximation to know about: a `Region` built from a `GraphicsPath` decomposes curves into bands, so
clipping to a curved path is clipped to that decomposition rather than the true outline. It is stated in
the XML doc on the overload and in `Region`'s own header comment — visible rather than silent, which is
the right call — but it is a real difference from GDI+ for a caller clipping to an ellipse.

### 4.3 🟢 The z-order and handle-creation methods do what they say

`BringToFront`, `SendToBack` and `CreateControl` were empty bodies (`SendToBack` set the toplevel
window-manager hint `KeepBelow`, which is not what a child control among siblings needs). All three now
carry the half of the WinForms contract that is observable without a display — the parent's child order,
and `Created` — as well as the GDK raise/lower and the realize walk that need a live window.
[ControlZOrderFunctionalTests.cs](Net4x.Gtk.Windows.Forms.Tests/System/Windows/Forms/ControlZOrderFunctionalTests.cs)
asserts the ordering directly via `Controls.GetChildIndex`, and that `CreateControl` creates the whole
child tree rather than just the control it was called on.

A scan of the 4,670-line `Control.cs` now finds 6 empty-bodied public/protected methods, all of them
legitimately empty (`BeginInit`/`EndInit`, the `OnPaint`/`OnPaintBackground` base bodies, `Select`,
`ScaleBitmapLogicalToDevice`).

### 4.4 🟢 Overall code-health markers are good

Library-wide: 49 `NotImplementedException` throws (all in genuinely unimplemented surface — see
[EMULATION-GAPS.md](EMULATION-GAPS.md)), **zero** `TODO`/`HACK`/`FIXME` markers, and **zero** silently
empty `catch {}` blocks. Every catch found routes through `GtkLogger`. For a 127 kLOC generated tree
derived from a translated upstream, that is a notably clean profile.

One residual encapsulation point: `Graphics.GetContextInfo()` returns the live Cairo context as `object`.
It is real GDI+ API and marked `EditorBrowsable(Never)`, so it is defensible, but it hands out an
unsynchronised substrate handle that defeats the `ownsContext`/`ownedSurface` lifetime bookkeeping beside
it. The context field itself is correctly `internal`.

---

## 5. Packaging & build configuration

### 5.1 🟡 The empty assemblies are deliberate; one of them is not achieving what it is for

Two of the shipped projects contain no source, and both `.csproj` files say why:

```xml
<!-- Net4x.System.Resources.Extensions -->
<!-- The assembly is intentionally left empty it replaces the System.Resources.Extensions.dll Microsoft assembly -->

<!-- Net4x.Gtk.Windows.Extensions -->
<!-- The assembly is intentionally left empty. It is a project where every developer can add its extension classes -->
```

Taken with the rest of the code these read as sound designs, not omissions:

- **`Net4x.Gtk.Windows.Extensions`** is a seam - a published package a consumer fills with their own
  extension classes over the Forms library. Empty is its steady state.
- **`Net4x.System.Resources.Extensions`** builds an assembly *named* `System.Resources.Extensions` in
  order to **displace Microsoft's** in an application's output folder. Nothing needs the displaced types:
  the working reader/writer live in the Forms library under
  [GTKSystem/Resources/Extensions/](Net4x.Gtk.Windows.Forms/GTKSystem/Resources/Extensions), in namespace
  `GtkSystem.Resources.Extensions`, and no file in the repository references a
  `System.Resources.Extensions.*` type. Displacing an implementation that deserialises through
  `BinaryFormatter` - the thing that does not work cross-platform - is exactly the right instinct. All
  three sample apps carry a `ProjectReference` to it, which is what that strategy requires.

**What does not hold is the displacement.** The empty assembly this project builds is 4,608 bytes; the
`System.Resources.Extensions.dll` that actually lands beside every sample app is Microsoft's:

```
Net4x.System.Resources.Extensions/bin/Debug/netstandard2.0/  4,608 bytes   (today)
GtkWinFormsApp/bin/Debug/net8.0/                            54,136 bytes   (13 Sep 2019)
GtkSystemWinFormsApp11/bin/Debug/net8.0/                    54,136 bytes   (13 Sep 2019)
AutoClosingMessageBoxApp/bin/Debug/net10.0/                 54,136 bytes   (13 Sep 2019)
```

The `System.Resources.Extensions` **package** reference - which reaches the apps transitively through
`Gtk.Windows.Forms` and `Gtk.Drawing` (§5.2) - carries the higher assembly version, so MSBuild's conflict
resolution prefers it over the `ProjectReference`. The project reference is present, builds, and is then
overruled.

Today the practical consequence is small, because nothing binds to those types either way; what is
missing is the guarantee the design was reaching for. Three ways out, and the choice is a product
decision rather than a defect to patch:

1. **Make the displacement win** - give the shim an assembly version above Microsoft's so conflict
   resolution picks it. Cheapest, and it makes the existing `ProjectReference`s mean what they say.
2. **Remove the package reference** from the shipped libraries, so Microsoft's assembly never enters the
   graph. Cleanest, but it is an outward-facing packaging change and needs the resx round-trip suites
   re-run against a real app.
3. **Accept Microsoft's assembly** and retire the shim, on the grounds that nothing binds to it.

I would not pick one from the outside: option 1 hands every consuming app an empty
`System.Resources.Extensions.dll`, and whether that is safe depends on what else in a *user's*
application might reference the real one - which this repository cannot see.

Also worth noting while here: `Gtk.System.Resources.Extensions` has no `GeneratePackageOnBuild`, so
unlike the other five it is never packed. That is consistent with options 2 and 3 and inconsistent with
option 1, so it is the same decision, not a separate one.

### 5.2 🟢 Shipped dependencies are honest and Release builds are reproducible

Verified by packing the main project and reading the resulting `.nuspec`:

```xml
<dependency id="Gtk.Drawing"                 version="1.3.24.26203" />
<dependency id="Gtk.Windows.FormsDesigner"   version="1.3.24.26203" />
<dependency id="GtkSharp"                    version="3.24.24.95"   />
<dependency id="System.Memory"               version="4.6.0"        />
<dependency id="System.Resources.Extensions" version="4.6.0"        />
```

Every entry is a package the library genuinely needs, at an exact version. The pinning is configured in
[Directory.Build.Props](Directory.Build.Props) and applies to `Release` only — Debug keeps floating so
local work picks up new revisions — and both pins are overridable from the command line. This matters more
here than in most projects: for a GTK interop layer, the GtkSharp *revision* is exactly where marshalling
behaviour changes, so two builds of one commit resolving different revisions would be a real hazard.

### 5.3 🟢 `Directory.Nuget.Props` carries this solution's versions and nothing else

[Directory.Nuget.Props](Directory.Nuget.Props) declares eight properties, each read by a `.csproj`,
`.props` or `.targets` in this repository, each with a comment naming its reader:

| Property | Read by |
|---|---|
| `DirectoryNuGet` | import guard for `Directory.Build.Props` |
| `NuGetPackageRoot` | resolves the `NuGet.Utility` props/targets imports in the shippable csprojs |
| `VersionBuildSuffix` | revision wildcard for the in-house references in `Prepare.Common` |
| `Net4xGtkWindowsFormsVersion` | this solution's own version |
| `GtkSharpLatestVersion` | the GTK3 binding the product is built on |
| `NuGetUtilityVersion` | build-time utility, `PrivateAssets="All"` |
| `Net4xBaseTypesVersion`, `SystemTextJson10Version` | the generation pipeline projects |

That is the whole file, and the discipline is worth keeping. A version manifest is an unusually effective
way to import someone else's problems: one line moving in a manifest shared across sibling repositories is
enough to stop a solution restoring (`NU1102`) for as long as the named version is unresolvable - with
nothing in the affected repository having changed, and an error naming a build utility rather than
anything the developer was working on. Scoping the file to what this solution actually consumes removes
that surface instead of mitigating it.

Both directions are enforced by
[DirectoryNugetPropsTests.cs](Prepare.Tests/DirectoryNugetPropsTests.cs), so drift fails a test rather
than lingering: a declared-but-unreferenced property fails, a referenced-but-undefined `*Version` fails,
and exceeding 20 properties fails with "check whether the shared CommonLibrary manifest has been copied
back over it". The placement contract is asserted too - `VersionPrepare` rewrites
`Net4xGtkWindowsFormsVersion` in place during the pipeline and looks for it in the *first*
`PropertyGroup` only.

[Directory.Build.Props](Directory.Build.Props) imports the file and may override anything in it
afterwards, which is where the configuration-dependent pins of §5.2 live.

### 5.4 🟢 The rest of the packaging is careful

Strong-name signing via `GtkForms.snk` with matching `InternalsVisibleTo` public keys; symbol packages
(`snupkg`); `netstandard2.0` for everything shippable (deliberately not pinned to a .NET version) with
`net10.0` only for tests. These are all the right calls.

---

## 6. Testing

### 6.1 ✅ The tests assert behaviour, not coverage

The oracles are real. [GraphicsClipFunctionalTests.cs](Net4x.Gtk.Drawing.Tests/System/Drawing/GraphicsClipFunctionalTests.cs)
does not ask the `Graphics` what it thinks its clip is — it fills the whole surface and then reads back
**actual pixels** inside, outside, and in the gap between two unioned regions, with the observed colour in
the failure message. [ResourceManagerCultureFunctionalTests.cs](Net4x.Gtk.Windows.Forms.Tests/GtkSystem/Resources/ResourceManagerCultureFunctionalTests.cs)
runs against a real pair of compiled resources — a neutral `.resx` and an Italian sibling the build turns
into an actual satellite assembly — so what it proves about the culture fallback chain (`it-IT` → `it` →
neutral, per key rather than per file) is true of the real mechanism, not a stand-in.

| Project | Tests | Result |
|---|---:|---|
| `Net4x.Gtk.Windows.Forms.Tests` | 2,242 | ✅ |
| `Net4x.Gtk.Drawing.Tests` | 1,184 | ✅ |
| `Prepare.Tests` | 197 | ✅ |
| `Gtk.Windows.FormsDesigner.Tests` | 45 | ✅ |
| `Net4x.ResxProcessor.Tests` | 20 | ✅ |
| `AutoClosingMessageBox.Tests` | 8 | ✅ |
| `GtkWinFormsApp.UITests` | 10 | needs a display; not run |

### 6.2 ✅ `GtkWinFormsApp.UITests` is the right answer to "does the app still work?"

It drives the **real sample forms** at the WinForms API level, pumping the GTK loop with
`Application.DoEvents` on the test thread so failures surface as xUnit failures rather than native
crashes; it asserts observable control state *and* gates static forms against committed screenshot
baselines (tolerant compare, seed-on-missing, `UPDATE_UI_BASELINES=1` to re-bless); and it deliberately
excludes time-varying forms from the screenshot gate instead of making them flaky. Auto-dismissing the
modal `MessageBox`es sample handlers pop, via the library's own auto-closing box, is a neat re-use.

Its stated limitation is the right one to attack next: **Windows desktop with a real display only**.

### 6.3 ✅ CI runs the gate

[.github/workflows/build-and-test.yml](.github/workflows/build-and-test.yml) builds the solution and runs
the five display-independent test projects on `windows-latest` with the GTK runtime installed, and — in a
second job — builds `PrepareProjects.sln` and runs `Prepare.Tests`, so the regeneration invariants of §3.1
are actually exercised on every push. The UI suite is deliberately excluded with the reason stated inline;
the pipeline itself is deliberately not *run*, since it is destructive to the generated trees and publishes
packages.

### 6.4 🟢 The thin spots have been thickened

`AutoClosingMessageBox.Tests` and `Net4x.ResxProcessor.Tests` were the two places where a silent
regression would have been expensive and nothing would have caught it — the resx processor rewrites every
`.resx` in the tree unattended on every pipeline run, and a wrong answer there corrupts a regenerated
sample rather than failing.

Both now assert the cases that matter rather than the happy path only: for the processor, idempotency
across repeated runs, nested relative paths, existing entries and `resheader`s surviving, an existing
stamp not being overwritten, non-resx XML left byte-identical, and the quoted-folder argument the
pipeline actually passes; for the message box, the thread-static singleton semantics and the timeout
configuration that decides when a dialog closes. Neither needs a display.

---

## 7. Recommended order of work

| # | Item | § | Effort |
|---|---|---|---|
| 1 | Decide how the `System.Resources.Extensions` displacement should work — or that it should not | 5.1 | a decision, then ~half a day |
| 2 | Clip to the true outline of a curved `GraphicsPath` rather than its band decomposition | 4.2 | ~a day |
| 3 | Stop `Graphics.GetContextInfo()` handing out the live Cairo context | 4.4 | ~an hour |
| 4 | Run the UI suite somewhere automated, so rendering order and layout are gated too | 6.2 | ~a day |

Items 2 and 3 touch generated trees and therefore need a `patches/` entry, not just an edit.

---

## 8. What this review did not cover

- **Runtime behaviour on Linux and macOS.** Everything here was read, built and tested on Windows. The
  cross-platform claim is the product's central promise and is untested by this review.
- **Emulation fidelity** — see [EMULATION-GAPS.md](EMULATION-GAPS.md). Its §2.1 note on `CreateGraphics`
  returning a *deferred* off-screen surface rather than an immediate-mode DC is the one item there with
  architectural weight, and it is substrate-bound rather than a defect.
- **Performance.** No profiling was done. The layout/measure path over GTK's allocate cycle and the
  per-paint Cairo surface allocation in `CreateGraphics` are the two places I would look first.
- **The pipeline solution** (`PrepareProjects.sln`, ~24 kLOC) beyond its coupling to the product.
- **Security.** Nothing in scope handles untrusted input except the resx deserialisation path in
  `GTKSystem/Resources/Extensions/`, which reproduces BCL binary deserialisation behaviour and deserves
  its own dedicated review.

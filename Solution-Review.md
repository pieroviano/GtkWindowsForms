# Solution review — `Gtk.Windows.Forms.sln`

**Type:** architectural + implementation analysis, written in the shape of a pull-request review.
**Scope:** the product solution [Gtk.Windows.Forms.sln](Gtk.Windows.Forms.sln) — 20 projects:
five shipped libraries, one bundled tool, three sample apps, six test projects. The generation pipeline
(`PrepareProjects.sln`) is out of scope except where it constrains the product.
**Basis:** static reading of the sources, a full `dotnet restore` + `dotnet build` of the solution, a
build of all five NuGet packages with every `.nuspec` extracted and read, inspection of the assemblies
that land beside the sample apps after a clean rebuild, a structural pass over all 104 files in
`patches/`, and a full run of every test project — the UI suite included.

Companion documents, not duplicated here: [Architecture.md](Architecture.md) (how the runtime works),
[Solution-Projects.md](Solution-Projects.md) (per-project inventory), [EMULATION-GAPS.md](EMULATION-GAPS.md)
(API-fidelity gaps vs. real WinForms), [CLAUDE.md](CLAUDE.md) (the pipeline). This document is about
**structure, engineering quality and risk**, and deliberately avoids re-listing emulation gaps.

---

## 1. Verdict

| | |
|---|---|
| **Overall** | **Approve.** Nothing found in this pass blocks a merge or a publish. The architecture is sound and unusually well-documented for a compatibility shim, the packaging is now internally consistent, and the two defects that were quietly costing something — a displacement that never happened and a patch that never applied — are fixed and verified rather than described. |
| **Build** | ✅ `dotnet restore` + `dotnet build Gtk.Windows.Forms.sln` → 0 errors, 0 warnings. |
| **Tests** | ✅ 3,597 passing across all six test projects, UI suite included. A further 625 in `Prepare.Tests` guard the pipeline. |
| **Open question** | The licence the packages declare is now consistent — LGPL-3.0, the one the vendored submodule carries — but that it is the *right* licence is a question for upstream, not for this repository (§5.1). |
| **Deferred** | Clipping to the true outline of a curved `GraphicsPath` rather than its band decomposition (§4.3). Deliberately not attempted in this pass; the reason is specific and stated. |

**Scale for context.** ~133.6 kLOC of library source (105.2 k Forms + 27.5 k Drawing + 0.6 k Designer +
0.4 k Extensions; 755 public types), against 45.3 kLOC of tests. That test-to-code ratio remains the
single most reassuring number in this repository, and — §6 — the tests assert behaviour rather than
coverage.

---

## 2. Architecture — what the solution actually is

Three layers, cleanly separated, and the separation is real rather than nominal:

```
        user WinForms app (unchanged .cs / .Designer.cs / .resx)
                    │                            │
   System.Windows.Forms.*            System.Drawing.*
                    ▼                            ▼
      Gtk.Windows.Forms  ──────►   Gtk.Drawing
      (pkg Gtk.Windows.Forms)            (pkg Gtk.Drawing)
                    │                            │
              wraps Gtk.Widget            draws via Cairo.Context
                    ▼                            ▼
                       GtkSharp 3.24.x → GTK/GDK/Cairo/Pango
```

Around that core sit four support assemblies: `Gtk.Windows.Extensions` (non-WinForms additions,
layered *on* the Forms library), `System.Resources.Extensions` (an intentionally empty displacing
assembly), `Gtk.Windows.FormsDesigner` (MSBuild-integrated design-time support), and
`ResxProcessor` (a `net8.0` tool shipped inside the main package under `tools/`).

### 2.1 The central pattern is the right one

Every control is a **facade/base pair**: a public `System.Windows.Forms.X` type owning WinForms
semantics, and a `GtkSystem.Windows.Forms.GtkControls.ControlBase.XBase` type owning the GTK widget.
Cross-cutting painting, CSS-class and background/image concerns are factored out into a single
composable helper,
[GtkControlOverride.cs](Gtk.Windows.Forms/GTKSystem/Windows/Forms/GtkControls/ControlBase/GtkControlOverride.cs),
attached to each base rather than inherited.

This is the correct decomposition for the problem. It keeps the substrate swappable in principle, keeps
`Control` free of per-widget GTK trivia, and makes the paint pipeline a single auditable place
(`OnDrawnBackground` → `OnDrawnImage` → `OnPaint`, raising both a Cairo-level `PaintGraphics` and a
`System.Drawing`-level `Paint`). The `ControlBase` layer is 3,888 lines across 46 files — against 105 k
in the library it serves, which is the proof that the abstraction carries its weight instead of being
bypassed.

### 2.2 Threading is modelled properly, which is rare in this class of project

[Control.cs:1329](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1329) onwards is the
best code in the repository. `InvokeRequired` compares against a **per-control owning thread id**
(matching WinForms' per-handle rule, not a global "is this the UI thread"); `MarshalToUiThread` posts to
`GLib.Idle`, blocks on a `ManualResetEventSlim`, and rethrows with `ExceptionDispatchInfo` while
unwrapping `TargetInvocationException` — the exact `Invoke` contract. `BeginInvoke` posts to the GTK loop
and returns a `TaskCompletionSource.Task` as its `IAsyncResult`.

The deadlock edge is handled explicitly: with no running main loop (`Gtk.Main.Level() == 0`) the work runs
inline, because nothing would ever drain the idle queue. That is the kind of detail that only gets written
after someone has actually hit the hang.

`Application.Run(ApplicationContext)`
([Application.cs:604](Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs#L604)) is
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

- [Control.cs:61](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L61) — why the
  `CreateGraphics` surface must *not* derive from `Widget.Window` (upstream's version corrupts GTK
  rendering during a modal dialog's realize path).
- [Graphics.cs:36](Gtk.Drawing/System/Drawing/Graphics.cs#L36) — why the Cairo surface/context
  fields are not `static` (two `Graphics` over two bitmaps could not otherwise coexist, which is exactly
  what double buffering needs).
- [Region.cs:23](Gtk.Drawing/System/Drawing/Region.cs#L23) — why the region is a band list rather
  than one rectangle, and which case (`Xor` of partially overlapping rectangles) forced it.

Because the shipped source is regenerated, these comments are the only durable record of hard-won
substrate knowledge. Keep enforcing that habit in patches.

### 2.4 The structural constraint is the generation pipeline

`Gtk.Windows.Forms/` and `Gtk.Drawing/` are **build outputs that are committed**. Every durable
change must be expressed as a `patches/*.patch`, a `Resources/` overlay, a post-processing rule, or a
submodule change. This is a deliberate and, given the merge-conflict history with upstream, a defensible
trade — but it means the repository has an unusual failure mode: *a correct fix that is not captured in the
right layer is silently temporary.* §3 covers how well that is currently defended, and §3.4 is what
happens when the defence has a hole.

Two projects sit **outside** that constraint and are hand-maintained: `Gtk.Windows.Extensions` and
`Gtk.Windows.FormsDesigner`. Edits to those are permanent without a patch. The asymmetry is worth
stating in onboarding, because the two kinds of library look identical in the solution tree.
`System.Resources.Extensions` is a third case again: its whole directory is deleted and re-copied
from the submodule on every run, so *nothing* hand-written survives there — it is emitted by
`SystemResourcesExtensionsProjectHandler` instead.

---

## 3. Generation & change durability

### 3.1 ✅ The regeneration invariants are themselves tested

`GtkWinFormsApp.UITests` depends on the sample's designer fields being `internal` plus an
`InternalsVisibleTo` grant — both of which the pipeline must reproduce on every regeneration. Rather than
hoping, `Prepare.Tests` asserts it, so a regeneration that dropped either fails a test instead of quietly
breaking the suite.

The same instinct now covers the packaging of `System.Resources.Extensions`
([SystemResourcesExtensionsProjectHandlerTests.cs](Prepare.Tests/SystemResourcesExtensionsProjectHandlerTests.cs)),
which matters because that project's directory does not survive a run: its package id, README, pack
settings and displacement sentinel exist only for as long as the handler keeps emitting them.

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

104 patches, applied in filename order. Two guards sit on that:

- **Unique ordinals.** `GitPatchApplier.EnsureOrdinalsAreUnique` throws if two patches share a leading
  number, because their relative order would then be decided by their *description text* rather than by
  intent. Where a number genuinely needs a second patch, the letter-suffix convention (`0007-…`,
  `0007a-…`) keeps the intended order under a plain sort.
  [GitPatchApplierOrdinalTests.cs](Prepare.Tests/GitPatchApplierOrdinalTests.cs) asserts both the rule and
  that the committed series satisfies it.
- **Failures are reported.** A patch that does not apply is collected and listed in red at the end of the
  run, with an explicit "the generated library is missing the fixes above" — rather than being swallowed
  by a discarded return value.

### 3.4 ✅ A corrupt patch had been contributing nothing, and the series is now structurally checked

Reading the series rather than trusting it turned up **`0006-Cleaned-code.patch`, which is not a patch**.
It is a *rendered dump* of one: it began with two junk lines (`File: patches\0006-Cleaned-code.patch` and a
markdown code fence), carried CRLF endings where every sibling has LF, had 7 truncated hunk headers
(`@@ -307`, `@@ -546,7 +546`, …), and — decisively — **summarised 130 files in its own diffstat while
containing 12 `diff --git` blocks**. Surviving hunks were damaged too; one adds
`for (var i = 0; i < typeCount; i++) ;`, a loop whose body was elided.

It had been failing for as long as it had been there, and the tree proves it: the namespace move it
performs is absent, so `GTKSystem/IO/Resources/BinaryReaderExtensions.cs` and `PinnedBufferMemoryStream.cs`
still declare `namespace GtkSystem.IO;`. Nothing behavioural was lost — it was a naming and namespace
cleanup — but §3.3's loud red report had been printing for a while with nobody acting on it.

It could not be repaired: ~90% of the file diffs are simply gone, and the commit its header names
(`c801188f845bc15905fb20c2e7e7acb26106d12e`) is not in this repository's history, so `git format-patch`
cannot regenerate it. Reconstructing 118 missing file diffs by inference would be guessing, and applying a
half-reconstructed patch is worse than applying none. It is quarantined in
[patches/corrupt/](patches/corrupt/) with a README recording exactly what was lost and how to redo it.

The recurrence guard is [PatchSeriesIntegrityTests.cs](Prepare.Tests/PatchSeriesIntegrityTests.cs), which
asserts three cheap properties of every file in `patches/`: it starts with a mailbox `From <sha>` line and
contains no markdown fence; every `@@` header is complete; and where a diffstat is present, its entry
count matches the number of file diffs. It was verified against the corrupt file rather than only against
the clean series — dropping that file back in fails three checks with the byte offsets named.

Worth recording what this guard deliberately does **not** do. The obvious stronger check — replay the
series with `git apply --check` — is not available: patches apply mid-pipeline to a *freshly transformed*
tree, not to the committed one, so their paths and context belong to a state this repository never stores.
Measured against the committed trees, only 36 of 104 patches reverse cleanly and many legitimately target
paths that no longer exist. Context drift therefore remains detectable only by running the pipeline, which
CI deliberately does not do. That is a real residual gap, and it is stated here rather than papered over
with a check that would not catch it.

### 3.5 ✅ Two patches were repaired after the `AutoClosingMessageBox` move

`0004` and `0009` still carried hunks against
`Gtk.Windows.Forms/System/Windows/Forms/AutoClosingMessageBox.cs`, a file the pipeline no longer
produces since that type moved to the hand-maintained Extensions project. Because `git apply` is atomic
per patch, both would have been rejected **in their entirety** on the next run, taking ~150 and ~215
unrelated file diffs down with them. The stale hunks are removed; both patches still parse and both are
covered by §3.4's guard.

This is the §2.4 failure mode in miniature, and it is worth noting that it was introduced by a change that
was itself careful about durability. Moving a file out of a generated tree means auditing the series for
patches that target it.

---

## 4. Implementation findings

### 4.1 🟢 `Control` raises `Disposed`, and the reason it once could not is worth knowing

`Control` overrides `CanRaiseEvents`, and it must return `true`
([Control.cs:1863](Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs#L1863)). The override
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
[ControlDisposalFunctionalTests.cs](Gtk.Windows.Forms.Tests/System/Windows/Forms/ControlDisposalFunctionalTests.cs),
along with the rest of the disposal contract.

### 4.2 🟢 The z-order and handle-creation methods do what they say

`BringToFront`, `SendToBack` and `CreateControl` were empty bodies (`SendToBack` set the toplevel
window-manager hint `KeepBelow`, which is not what a child control among siblings needs). All three now
carry the half of the WinForms contract that is observable without a display — the parent's child order,
and `Created` — as well as the GDK raise/lower and the realize walk that need a live window.
[ControlZOrderFunctionalTests.cs](Gtk.Windows.Forms.Tests/System/Windows/Forms/ControlZOrderFunctionalTests.cs)
asserts the ordering directly via `Controls.GetChildIndex`, and that `CreateControl` creates the whole
child tree rather than just the control it was called on.

A scan of the 4,790-line `Control.cs` finds exactly 6 empty-bodied public/protected methods, all of them
legitimately empty: `BeginInit`/`EndInit`, the `OnPaint`/`OnPaintBackground` base bodies, `Select`, and
`ScaleBitmapLogicalToDevice`.

### 4.3 🟡 Clipping is coherent; the curved-path approximation is deliberately still there

Every `SetClip`/`IntersectClip` overload funnels through
[SetClip(Region, CombineMode)](Gtk.Drawing/System/Drawing/Graphics.cs#L2312): combining happens on
the `Region` — which implements the whole `CombineMode` family exactly for rectangular inputs — and the
result is applied to the Cairo context in one pass by `ApplyClip`. This is the right shape, because Cairo's
own `Clip()` can only ever *narrow*, so `Union`, `Exclude`, `Xor` and `Complement` are not expressible
incrementally; computing the resulting region first sidesteps that entirely. `Clip`, `ClipBounds` and the
Cairo state are updated together, so they cannot drift apart, and an unknown `CombineMode` throws rather
than guessing.

The approximation: a `Region` built from a `GraphicsPath` decomposes curves into bands, and `ApplyClip`
replays those bands, so clipping to an ellipse clips to its decomposition rather than its true outline. It
is stated in the XML doc on the overload and in `Region`'s own header comment — visible rather than
silent — but it is a real difference from GDI+.

**Why it was not fixed in this pass.** `Region(GraphicsPath)` already keeps the exact path for this very
purpose ("Keep the real path so that clipping can use the true shape rather than the bounding box"), so
the missing piece is only replaying it into Cairo. But the replay that exists, `DrawPathCore`, is 206
lines that also render text and recurse into nested paths — neither meaningful for a clip — and it wraps
its work in `context.Save()`/`Restore()`, which in Cairo **discards a clip set between them**. So the fix
is either extracting a geometry-only replay or threading a clip mode through that method while getting
the save/restore and translate interaction right, plus a `HasExactPath` flag on `Region` (every combine
funnels through `SetBands`, which overwrites the exact path with band rectangles), plus pixel-level tests.
That is a focused change with its own test story, not a line in a sweep — and it sits on the paint path of
every control, so getting it subtly wrong is expensive. Left as a deliberate deferral rather than attempted
and half-landed.

### 4.4 ✅ `Graphics.GetContextInfo()` no longer hands out the Cairo context

It returned `this.context` — a raw, unsynchronised substrate handle — through a public method on the type
whose whole job is to own that handle's lifetime. A caller could draw through it, dispose it or
reconfigure it behind the `ownsContext`/`ownedSurface` bookkeeping declared a few lines above, and making
the field `internal` (which a previous pass did, with a comment explaining why) bought nothing while this
method published it anyway.

It now answers what GDI+ callers actually ask this API: `object[] { Region clip, PointF transformOffset }`.
[GraphicsContextInfoTests.cs](Gtk.Drawing.Tests/System/Drawing/GraphicsContextInfoTests.cs) asserts
the shape, that the reported clip tracks the clip actually in force rather than the initial surface, that
the offset follows `TranslateTransform`, and — the point of the change — that no Cairo type escapes.
Captured as
[patch 0100](patches/0100-Stop-Graphics.GetContextInfo-handing-out-the-Cairo-context.patch).

### 4.5 ✅ `AutoClosingMessageBox.DialogAvailable` now fires

The class declared a public `DialogAvailable` event and a `protected virtual OnDialogAvailable` that
raises it, and nothing called it. It *did* subscribe to the inner `NonStaticMessageBox.DialogAvailable` —
which genuinely fires — but its handler stored the dialog in a field and stopped, so a consumer
subscribing in order to inspect or restyle the dialog, which is what the XML doc promises, was never
called back. The invocator was a pipeline artefact, generated by the Roslyn rewrite that gives every event
one; the wiring to it was missing.

Pre-existing, but it stopped being a dead member of a large library when `AutoClosingMessageBox` became a
third of the public surface of `Gtk.Windows.Extensions`. The handler now forwards, and
`AutoClosingMessageBoxBehaviourTests` covers it without needing a display, because the notification
originates from the inner implementation rather than from GTK.

### 4.6 🟢 Code-health markers are good

Library-wide: **zero** `TODO`/`HACK`/`FIXME` markers and **zero** silently empty `catch {}` blocks — every
catch routes through `GtkLogger`. Unimplemented surface throws deliberately: 15 `NotImplementedException`
throws (all in the Forms library; Drawing has none) and 58 `NotSupportedException`. The remaining 34
mentions of `NotImplementedException` in the tree are XML-doc `<exception>` declarations, not throws —
worth stating, because an earlier pass counted mentions and reported "49 throws", overstating it
threefold. For a 133 kLOC generated tree derived from a translated upstream, this is a notably clean
profile.

### 4.7 🟢 The `MessageBox.ReplaceInstance` break is handled about as well as it can be

Moving `AutoClosingMessageBox` into a package that depends on the Forms library made
`MessageBox.ReplaceInstance(int timeout)` unimplementable — the core library can no longer name the type
it used to construct. The resolution is sound: a new `ReplaceInstance(IMessageBox? = null)` overload, the
timeout-based installation moved to `AutoClosingMessageBox.Install(timeout)` in the package that owns the
behaviour, and the old `int` overload kept, `[Obsolete]`, with a message naming the replacement. Giving
the `int` overload no default value keeps bare `ReplaceInstance()` unambiguous, so the two in-repo no-arg
callers are untouched.

What a consumer should know, and what the obsoletion message says plainly: `ReplaceInstance(500)` still
compiles and still returns a working message box, but it no longer auto-closes. A compile-time warning is
the strongest available signal short of deleting the overload, and deleting it would break the build of
every existing caller instead of warning them.

---

## 5. Packaging & build configuration

The solution emits **five** NuGet packages — `Gtk.Windows.Forms`, `Gtk.Drawing`,
`Gtk.Windows.FormsDesigner`, `Gtk.Windows.Extensions` and `Gtk.Resources.Extensions`. Everything
below was read from the built `.nupkg` files rather than inferred from the csprojs.

### 5.1 ✅ The five packages now tell one story; whether it is the right story is upstream's to say

They did not. Read from the built `.nuspec`s, three of the five packed a `LICENSE.txt`,
`Gtk.Windows.Extensions` declared the expression **`Apache-2.0`**, and `Gtk.Windows.FormsDesigner`
declared **no licence at all**. Nobody chose the Apache-2.0: that project set neither
`PackageLicenseFile` nor `PackageLicenseUrl`, its own attempt to pack one was a
`Condition="Exists('$(SolutionDir)License.txt')"` on a file that does not exist, and the condition being
false let `Net4x.NuGetUtility.targets`' default stamp Apache-2.0 silently. That was harmless while the
assembly was empty; it stopped being harmless when `AutoClosingMessageBox` moved in and the package began
shipping code derived from the same upstream as its differently-licensed siblings.

All five now pack the same file — `gtksystem-windows-forms/Source/licenses/LICENSE.txt`, the licence the
vendored submodule actually carries — and declare it via `PackageLicenseFile`, verified in each built
nuspec.

**The open question is what that file says.** It is the GNU **LGPL v3**, as is the submodule's root
`LICENSE`. [CLAUDE.md](CLAUDE.md) described the upstream as MIT, which nothing in the tree supports; that
line now states what the files say and flags the discrepancy. LGPL-3.0 and MIT impose materially different
obligations on a consumer, so *which is correct* is a question to put to upstream. What a code review can
fix is the inconsistency, and that is fixed.

### 5.2 ✅ Package metadata is real

`Gtk.Windows.Extensions` shipped `<description>Package Description</description>` and
`<authors>Gtk.Windows.Extensions</authors>` — the SDK's placeholders, which render as the headline
and by-line on nuget.org — and `Gtk.Windows.FormsDesigner` shipped the placeholder author. Both now carry
author, company, copyright, title, description and project URL. A dead `<PackegeId>` typo (the real one is
declared correctly further down) went with it.

### 5.3 ✅ The `System.Resources.Extensions` displacement now actually happens

`System.Resources.Extensions` builds an assembly *named* `System.Resources.Extensions` in order to
**displace Microsoft's** in an application's output folder, so nothing can bind to an implementation that
deserialises through `BinaryFormatter`. Nothing needs the displaced types — the working reader/writer live
in the Forms library under
[GTKSystem/Resources/Extensions/](Gtk.Windows.Forms/GTKSystem/Resources/Extensions) — and all three
sample apps carry a `ProjectReference` to the shim.

**It had never worked.** MSBuild's conflict resolution keeps the highest `AssemblyVersion` for a given
simple name, and the shim inherited the package version:

```
shim              System.Resources.Extensions, Version=1.3.24.26204   (5,632 bytes)
Microsoft's       System.Resources.Extensions, Version=4.0.0.0       (54,136 bytes)
```

`1.3.24.26204 < 4.0.0.0`, so Microsoft's assembly won every time and landed beside all three sample apps —
the project reference was present, built, and then overruled. The `System.Resources.Extensions` package
reference reaching the apps transitively through `Gtk.Windows.Forms` is what put it in the graph.

The shim now sets `AssemblyVersion`/`FileVersion` to a `99.0.0.0` sentinel — deliberately far above
anything Microsoft is likely to ship, affecting only the assembly identity, not the NuGet package version.
Verified by deleting the sample output folders and rebuilding: **5,632 bytes at v99.0.0.0** now lands
beside `GtkWinFormsApp` and `AutoClosingMessageBoxApp`, and the full suite — including the 10 UI tests that
drive the real sample forms and the resx round-trip suites — stays green, which is the evidence that
nothing was binding to the displaced types.

The sentinel is emitted by `SystemResourcesExtensionsProjectHandler`, not hand-written into the csproj, so
it survives the directory wipe (§2.4), and is asserted by that handler's tests.

### 5.4 🟢 Shipped dependencies are honest and Release builds are reproducible

Verified by reading the main package's `.nuspec`:

```xml
<dependency id="Gtk.Drawing"                 version="1.3.24.26204" />
<dependency id="Gtk.Windows.FormsDesigner"   version="1.3.24.26204" />
<dependency id="GtkSharp"                    version="3.24.24.95"   />
<dependency id="System.Memory"               version="4.6.0"        />
<dependency id="System.Resources.Extensions" version="4.6.0"        />
```

Every entry is a package the library genuinely needs, at an exact version. The pinning is configured in
[Directory.Build.props](Directory.Build.props) and applies to `Release` only — Debug keeps floating so
local work picks up new revisions — and both pins are overridable from the command line. This matters more
here than in most projects: for a GTK interop layer, the GtkSharp *revision* is exactly where marshalling
behaviour changes, so two builds of one commit resolving different revisions would be a real hazard.

`Gtk.Windows.Extensions` correctly declares its single dependency on `Gtk.Windows.Forms` at the exact
matching version.

### 5.5 🟢 `Directory.Nuget.props` carries this solution's versions and nothing else

[Directory.Nuget.props](Directory.Nuget.props) declares eight properties, each read by a `.csproj`,
`.props` or `.targets` in this repository, each with a comment naming its reader:

| Property | Read by |
|---|---|
| `DirectoryNuGet` | import guard for `Directory.Build.props` |
| `NuGetPackageRoot` | resolves the `NuGet.Utility` props/targets imports in the shippable csprojs |
| `VersionBuildSuffix` | revision wildcard for the in-house references in `Prepare.Common` |
| `GtkWindowsFormsVersion` | this solution's own version |
| `GtkSharpLatestVersion` | the GTK3 binding the product is built on |
| `NuGetUtilityVersion` | build-time utility, `PrivateAssets="All"` |
| `Net4xBaseTypesVersion`, `SystemTextJson10Version` | the generation pipeline projects |

That is the whole file, and the discipline is worth keeping. A version manifest is an unusually effective
way to import someone else's problems: one line moving in a manifest shared across sibling repositories is
enough to stop a solution restoring (`NU1102`) for as long as the named version is unresolvable — with
nothing in the affected repository having changed, and an error naming a build utility rather than
anything the developer was working on. Scoping the file to what this solution actually consumes removes
that surface instead of mitigating it.

Both directions are enforced by
[DirectoryNugetPropsTests.cs](Prepare.Tests/DirectoryNugetPropsTests.cs), so drift fails a test rather
than lingering: a declared-but-unreferenced property fails, a referenced-but-undefined `*Version` fails,
and exceeding 20 properties fails with "check whether the shared CommonLibrary manifest has been copied
back over it". The placement contract is asserted too — `VersionPrepare` rewrites
`GtkWindowsFormsVersion` in place during the pipeline and looks for it in the *first*
`PropertyGroup` only.

[Directory.Build.props](Directory.Build.props) imports the file and may override anything in it
afterwards, which is where the configuration-dependent pins of §5.4 live.

### 5.6 🟢 The rest of the packaging is careful

Strong-name signing via `GtkForms.snk` with matching `InternalsVisibleTo` public keys; symbol packages
(`snupkg`) for all five; `netstandard2.0` for everything shippable (deliberately not pinned to a .NET
version) with `net10.0` only for tests; XML documentation generated and packed for every library; and
`Nullable` is now `enable` across all four hand-written and generated libraries alike —
`Gtk.Windows.Extensions` had been on `annotations`, publishing nullable annotations to consumers without
null-checking the source that produced them.

---

## 6. Testing

### 6.1 ✅ The tests assert behaviour, not coverage

The oracles are real. [GraphicsClipFunctionalTests.cs](Gtk.Drawing.Tests/System/Drawing/GraphicsClipFunctionalTests.cs)
does not ask the `Graphics` what it thinks its clip is — it fills the whole surface and then reads back
**actual pixels** inside, outside, and in the gap between two unioned regions, with the observed colour in
the failure message. [ResourceManagerCultureFunctionalTests.cs](Gtk.Windows.Forms.Tests/GtkSystem/Resources/ResourceManagerCultureFunctionalTests.cs)
runs against a real pair of compiled resources — a neutral `.resx` and an Italian sibling the build turns
into an actual satellite assembly — so what it proves about the culture fallback chain (`it-IT` → `it` →
neutral, per key rather than per file) is true of the real mechanism, not a stand-in.

| Project | Tests | Result |
|---|---:|---|
| `Gtk.Windows.Forms.Tests` | 2,320 | ✅ |
| `Gtk.Drawing.Tests` | 1,193 | ✅ |
| `Gtk.Windows.FormsDesigner.Tests` | 45 | ✅ |
| `ResxProcessor.Tests` | 20 | ✅ |
| `GtkWinFormsApp.UITests` | 10 | ✅ (ran — display available) |
| `AutoClosingMessageBox.Tests` | 9 | ✅ |
| **Product solution total** | **3,597** | ✅ |
| `Prepare.Tests` *(pipeline solution)* | 625 | ✅ |

`Prepare.Tests` grew from 208 to 625 because §3.4's integrity checks are theories over every file in
`patches/`. That is a real increase in what is checked, not in what is written.

### 6.2 🟡 `GtkWinFormsApp.UITests` is the right answer to "does the app still work?" — and still runs in one place only

It drives the **real sample forms** at the WinForms API level, pumping the GTK loop with
`Application.DoEvents` on the test thread so failures surface as xUnit failures rather than native
crashes; it asserts observable control state *and* gates static forms against committed screenshot
baselines (tolerant compare, seed-on-missing, `UPDATE_UI_BASELINES=1` to re-bless); and it deliberately
excludes time-varying forms from the screenshot gate instead of making them flaky. Auto-dismissing the
modal `MessageBox`es sample handlers pop, via `AutoClosingMessageBox.Install`, is a neat re-use — and it
makes the suite an incidental smoke test of the moved package.

It remains **Windows desktop with a real display only**, and this pass did not change that. Adding it to
CI is not a one-line job: the screenshot baselines were captured on a developer machine, and font
rasterisation on a hosted runner will not match them, so enabling the job as it stands would produce a red
build that says nothing about the product. Per-environment baselines, or restricting the CI run to the
non-screenshot assertions, is the actual work — worth doing, but it is a change to the suite rather than
to the workflow file.

### 6.3 ✅ CI runs the gate

[.github/workflows/build-and-test.yml](.github/workflows/build-and-test.yml) builds the solution and runs
the five display-independent test projects on `windows-latest` with the GTK runtime installed, and — in a
second job — builds `PrepareProjects.sln` and runs `Prepare.Tests`, so the regeneration invariants of §3.1
and the patch-integrity checks of §3.4 are exercised on every push. The UI suite is excluded with the
reason stated inline; the pipeline itself is deliberately not *run*, since it is destructive to the
generated trees and publishes packages.

### 6.4 🟢 The thin spots have been thickened

`AutoClosingMessageBox.Tests` and `ResxProcessor.Tests` were the two places where a silent
regression would have been expensive and nothing would have caught it — the resx processor rewrites every
`.resx` in the tree unattended on every pipeline run, and a wrong answer there corrupts a regenerated
sample rather than failing.

Both assert the cases that matter rather than the happy path only: for the processor, idempotency across
repeated runs, nested relative paths, existing entries and `resheader`s surviving, an existing stamp not
being overwritten, non-resx XML left byte-identical, and the quoted-folder argument the pipeline actually
passes; for the message box, the thread-static singleton semantics, the timeout configuration that decides
when a dialog closes, and now the dialog notification of §4.5. Neither needs a display.

---

## 7. Recommended order of work

| # | Item | § | Effort |
|---|---|---|---|
| 1 | Confirm with upstream which licence this fork ships under, and correct the packed file if it is not LGPL-3.0 | 5.1 | a question, then ~15 minutes |
| 2 | Redo the `0006` cleanup against the current tree and capture a fresh patch | 3.4 | ~half a day |
| 3 | Clip to the true outline of a curved `GraphicsPath` rather than its band decomposition | 4.3 | ~a day |
| 4 | Make the UI suite runnable on a hosted runner (per-environment baselines, or a non-screenshot subset) | 6.2 | ~a day |

Item 3 touches a generated tree and therefore needs a `patches/` entry, not just an edit. Item 2 does too.
None of the four blocks a publish.

---

## 8. What this review did not cover

- **Runtime behaviour on Linux and macOS.** Everything here was read, built and tested on Windows. The
  cross-platform claim is the product's central promise and is untested by this review.
- **Legal analysis of the licensing question in §5.1.** What is reported there is what the files in this
  repository say; whether LGPL-3.0 is the correct answer is not a question a code review can settle.
- **Whether the `99.0.0.0` displacement sentinel is safe in a consumer's application** (§5.3). It is
  verified against this repository's three sample apps and its full suite. An application that genuinely
  binds to Microsoft's `System.Resources.Extensions` types would now get the empty assembly instead —
  which is the intended behaviour of the design, and which this repository cannot test on someone else's
  dependency graph.
- **Emulation fidelity** — see [EMULATION-GAPS.md](EMULATION-GAPS.md). Its §2.1 note on `CreateGraphics`
  returning a *deferred* off-screen surface rather than an immediate-mode DC is the one item there with
  architectural weight, and it is substrate-bound rather than a defect.
- **Performance.** No profiling was done. The layout/measure path over GTK's allocate cycle and the
  per-paint Cairo surface allocation in `CreateGraphics` are the two places I would look first.
- **The pipeline solution** (`PrepareProjects.sln`) beyond its coupling to the product.
- **Security.** Nothing in scope handles untrusted input except the resx deserialisation path in
  `GTKSystem/Resources/Extensions/`, which reproduces BCL binary deserialisation behaviour and deserves
  its own dedicated review.

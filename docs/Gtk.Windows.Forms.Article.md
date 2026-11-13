# Running Windows Forms Applications on Windows, Linux and macOS with Gtk.Windows.Forms

## Introduction

Windows Forms is still one of the most widely used desktop UI stacks in the .NET world. It is
productive, it has a designer that people actually like, and there are millions of lines of
line-of-business code written against it. It has exactly one problem: the name is not a metaphor.
`System.Windows.Forms` runs on Windows.

When the requirement "it must also run on Linux" arrives, the usual answer is a rewrite — Avalonia,
MAUI, Uno, or a web front end. A rewrite means re-doing every form, every custom control, every
designer file, and re-testing the whole surface, all to end up exactly where you already were,
functionally speaking.

**Gtk.Windows.Forms** takes the other route. It is a compatibility layer that re-implements the
Windows Forms API on top of **GTK 3** (through GtkSharp), so that an existing WinForms application
can be *recompiled* rather than rewritten, and then run on Windows, Linux and macOS. You keep your
`Form1.Designer.cs`. You keep the Visual Studio designer. You keep `button1_Click`.

This article explains what the library is, shows how to convert a WinForms project step by step,
describes how the emulation works internally, and — importantly — is explicit about where the
boundary is, because a compatibility layer that pretends to have no limits is a compatibility layer
that will surprise you in production.

## What Gtk.Windows.Forms Is

Gtk.Windows.Forms is distributed as a set of NuGet packages, the two central ones being:

| Package | Role |
| --- | --- |
| `Gtk.Windows.Forms` | The `System.Windows.Forms` API surface implemented over GTK widgets |
| `Gtk.Drawing` | A `System.Drawing` compatibility shim implemented over GTK/Cairo |

Both are built as **.NET Standard 2.0**. That is a deliberate choice: the library is not pinned to a
particular .NET release, so it can be consumed from .NET 5 and every version after it without the
project being dragged along by a framework upgrade cycle it does not control.

The design goals, in order:

1. **Compile once, run everywhere.** The same assembly runs on Windows, Linux and macOS.
2. **No new framework to learn.** The same classes, properties, events and delegate signatures.
3. **Keep the tooling.** Development still happens in Visual Studio with the native WinForms
   designer; the designer files it generates are consumed unchanged.

The project is a maintained fork of **GTKSystem.Windows.Forms** by EasyWebFactory. The upstream
sources are vendored and the shipped library is regenerated from them by a build pipeline, so
upstream improvements can be pulled forward without a permanent merge-conflict tax. The upstream
repository ships the GNU LGPL v3 — check the repository for the licensing terms that apply to your
use.

## Converting a WinForms Project: Step by Step

The conversion is mostly a matter of removing the Windows-specific bits from the project file and
adding a package reference.

### 1. Check the .NET SDK

```bash
dotnet --version
```

If it is missing, install it from <https://dotnet.microsoft.com/en-us/download>.

### 2. Create (or open) a Windows Forms project

For a fresh start:

```bash
dotnet new winforms -n MyWinFormsApp
cd MyWinFormsApp
```

For an existing application, just open the project you already have. Nothing about the following
steps requires a new project.

### 3. Add the package reference

Edit the `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Gtk.Windows.Forms" Version="1.3.24.26204" />
</ItemGroup>
```

Use the latest version published on NuGet rather than copying the number above verbatim.

### 4. Make the project cross-platform

Two edits, both in the `.csproj`.

Change the target framework from the Windows-specific flavour:

```xml
<!-- before -->
<TargetFramework>net10.0-windows</TargetFramework>

<!-- after -->
<TargetFramework>net10.0</TargetFramework>
```

And remove the property that pulls in the real Windows Desktop WinForms:

```xml
<!-- delete this line -->
<UseWindowsForms>true</UseWindowsForms>
```

That second edit is the one that matters. `UseWindowsForms` makes the SDK reference the Windows
Desktop `System.Windows.Forms.dll`; with it gone, the types resolve to `Gtk.Windows.Forms` instead —
which is precisely the substitution the whole approach depends on.

### 5. Restore and run

```bash
dotnet restore
dotnet run
```

### 6. Make sure GTK 3 is present on the target machine

The managed layer is in the NuGet package, but GTK itself is a native dependency and has to exist on
the machine that runs the application:

* **Linux** — GTK 3 is present on any mainstream desktop distribution; on a headless or minimal image
  install the distribution's `gtk3` package.
* **macOS** — `brew install gtk+3`.
* **Windows** — install a GTK 3 runtime.

This is the one genuinely new deployment concern compared with a classic WinForms application, and
it is worth putting in your installer or container image early rather than discovering it on a test
machine.

### A note on the designer

If Visual Studio shows designer errors right after the conversion, close and reopen the project. The
designer caches the previously resolved WinForms assembly, and the reload is usually all that is
needed.

## How the Emulation Works

It is worth understanding the mechanism, because it explains both why most code ports cleanly and
why some code does not.

### Controls wrap GTK widgets

Every public WinForms type is a wrapper over one or more GtkSharp widgets. `RichTextBoxBase`, for
example, holds a `Gtk.TextView` internally and exposes the WinForms rich-text API over it. Container
controls such as `TableLayoutPanel` map onto GTK containers.

### Events are translated, not re-invented

The library subscribes to GTK signals internally and converts them into WinForms events, preserving
the delegate signatures and `EventArgs` types your code already handles. `MouseDown`, `KeyPress`,
`Click` and the rest arrive with the shapes you expect, which is why event handlers usually need no
changes at all.

### Layout rules are re-implemented on top of GTK

This is the least obvious part of the work. GTK has its own measure/allocate cycle, which is not how
WinForms lays out. The WinForms rules — docking, anchoring, `TableLayoutPanel` proportions, padding
and margins — are re-implemented *on top of* the GTK cycle, so that controls end up sized and placed
the way a WinForms developer expects rather than the way a GTK developer would.

### Drawing goes through Cairo

Anything written against `System.Drawing` is served by the `Gtk.Drawing` shim, which implements
`Graphics`, `Brush`, `Pen`, `Matrix`, `StringFormat` and friends over GTK/Cairo. Custom `OnPaint`
code keeps working because the paint call is forwarded into GTK's draw cycle.

### Resources are embedded and resolved by name

Image assets are embedded resources loaded at runtime through the assembly resource name, for
example:

```csharp
self.Icon = new Gdk.Pixbuf(this.GetType().Assembly, "Resources.System.view-more.png");
```

Designer-generated resource handling and serialization metadata are mapped to match WinForms
expectations, which is what keeps `.resx` files and designer round-tripping usable.

## How Complete Is It, Really?

This is the question that decides whether the approach is viable for a given application, so it
deserves a number rather than an adjective. The project measures its API surface by reflecting over
the real reference assemblies and the shipped ones and diffing the public and protected members.

Against the .NET Framework 4.8 baseline:

| Assembly | Types present | Members present on those types |
| --- | --- | --- |
| `Gtk.Windows.Forms` | 90.6% | ~86% |
| `Gtk.Drawing` | 76.7% | 98.6% |

Two observations matter more than the raw percentages:

* **`Gtk.Drawing` is effectively complete as an API surface.** The missing types are mostly ones that
  `netstandard2.0` already supplies from `System.Drawing.Primitives` — `Color`, `Point`, `Size`,
  `Rectangle` and so on are deliberately *not* redefined, so your code gets the genuine BCL types.
* **The types still missing from `Gtk.Windows.Forms` are declared non-goals, not backlog.** They are
  the Windows-bound families listed in the next section. No missing type breaks ordinary
  line-of-business code.

The remaining member tail is concentrated in modern-.NET additions and rarely hand-written members.

## Limitations You Should Know Before You Start

Some things are Windows-bound by nature and will never work on a GTK substrate. Knowing them up
front is the difference between a smooth port and an unpleasant surprise.

**Not implemented, by decision:**

* **The Win32 message pump.** `Control.WndProc` and `DefWndProc` throw
  `PlatformNotSupportedException`. `NativeWindow` and `SendKeys` do not exist. If your code reaches
  below the event model, it is not portable here.
* **`Control.Handle` is a GTK widget handle, not an `HWND`.** Anything that P/Invokes with it will
  need rethinking.
* **COM and ActiveX.** `AxHost` and the COM interop surface are Windows-only by definition.
* **`WebBrowser`, `HtmlDocument`, `HtmlElement`.** There is no cross-platform engine behind them.
* **EMF/WMF metafiles.** The types exist, but the Windows record format has no cross-platform
  playback.
* **Legacy controls.** The pre-2.0 `DataGrid` family and `ToolBar`/`ToolBarButton` are superseded by
  `DataGridView` and `ToolStrip`.

**Present but deliberately different:**

* **`Control.CreateGraphics()` is deferred, not immediate.** GTK's retained paint model has no
  immediate-mode device context, so drawing is composited on the next paint cycle instead of
  appearing at once. The classic rubber-band idiom (call `CreateGraphics()` in `MouseMove` and XOR a
  rectangle) compiles and runs but does the wrong thing. The portable idiom is to draw in `OnPaint`
  and call `Invalidate()`.
* **`Form.TransparencyKey`** emulates the whole-form-background case only, not per-pixel keying.

The project tracks these deliberately: an `API-COMPLETENESS.md` for the mechanical surface diff and
an `EMULATION-GAPS.md` for behavioural deviations. Reading them before committing to a port is time
well spent.

## When This Is the Right Tool

Reach for Gtk.Windows.Forms when:

* You have a **real, existing** WinForms application and the cost of a rewrite is not justified by
  its UI.
* You need Linux or macOS reach for an internal or line-of-business tool.
* Your team's productivity is tied to the WinForms model and the Visual Studio designer.

Look elsewhere when:

* You are starting a **greenfield** desktop application — a modern cross-platform toolkit will serve
  you better over its lifetime.
* Your application depends structurally on Win32 interop, embedded browsers or ActiveX.
* You need pixel-identical native look and feel on every platform.

## What This Fork Adds

The fork exists because the upstream project, while sound in its architecture, had some practical
obstacles for non-Chinese-speaking teams and for long-term maintenance. The maintained version adds:

* **English sources.** Comments and XML documentation were originally in Chinese and have been
  translated.
* **Multi-language runtime messages.** Errors and framework messages are localised in Chinese,
  English, French, German, Italian and Spanish.
* **A unit test suite.** The original project had none, which made every refactoring a gamble.
  Tests came first specifically so the code could then be restructured safely.
* **A regeneration pipeline instead of a git fork.** Upstream is vendored as a submodule and the
  shipped library is regenerated from it, with every local change expressed as a `.patch` file. New
  upstream work can be pulled forward without a merge-conflict avalanche.
* **Enforced XML documentation and formatting** on the public surface, applied by the pipeline.

## Summary

Gtk.Windows.Forms is not a general-purpose replacement for modern cross-platform UI frameworks, and
it does not claim to be. It is a targeted answer to a specific and very common situation: a working
WinForms application, a new requirement to run on Linux or macOS, and no appetite for rewriting a UI
that already does its job.

Within that scope it is a genuinely economical option. You keep the codebase, the designer and the
development workflow; the cost is a package reference, two lines of `.csproj` edits, a GTK runtime on
the target machine, and a clear-eyed check that your application does not depend on the Windows-bound
areas listed above.

If those boxes tick, a port that would otherwise have been a project becomes an afternoon.

## References

* Project repository — <https://bitbucket.org/piero-viano/net4x.gtk.windows.forms>
* Tutorial repository (each step is a separate commit) —
  <https://bitbucket.org/piero-viano/net4x.gtk.windows.forms.tutorial>
* Upstream project, GTKSystem.Windows.Forms —
  <https://github.com/easywebfactory/gtksystem-windows-forms>
* .NET SDK downloads — <https://dotnet.microsoft.com/en-us/download>

# Net4x.Gtk.Windows.Forms — Architecture

This document explains **how the product solution works**: the runtime architecture of the libraries in
[Net4x.Gtk.Windows.Forms.sln](Net4x.Gtk.Windows.Forms.sln). The PoC sample apps (`GtkWinFormsApp`,
`GtkSystemWinFormsApp11`, `AutoClosingMessageBoxApp`) are intentionally excluded.

For a project-by-project inventory (packages, TFMs, references) see
[Solution-Projects.md](Solution-Projects.md). For the build/generation pipeline see
[CLAUDE.md](CLAUDE.md) and `PrepareProjects.sln`.

> **Reminder:** the source under `Net4x.Gtk.Windows.Forms/` and `Net4x.Gtk.Drawing/` is *generated*
> by the prepare pipeline from the `gtksystem-windows-forms` submodule. Durable changes belong in
> `patches/`, `Resources/`, or the submodule — not in the generated trees.

## The big picture

The goal: an existing C# WinForms application compiles **unchanged** against these libraries and runs
on Windows, Linux, and macOS, because every `System.Windows.Forms` / `System.Drawing` type is
re-implemented on top of GTK3 (via GtkSharp) and Cairo.

```
┌───────────────────────────────────────────────────────────────┐
│                     User's WinForms app                       │
│      (Form1.cs, Form1.Designer.cs, .resx — unchanged)         │
└──────────────┬──────────────────────────────┬─────────────────┘
               │ System.Windows.Forms.*       │ System.Drawing.*
┌──────────────▼───────────────┐  ┌───────────▼─────────────────┐
│  Net4x.Gtk.Windows.Forms     │  │  Net4x.Gtk.Drawing          │
│  (package: Gtk.Windows.Forms)│─►│  (package: Gtk.Drawing)     │
│  Control/Form/Button/...     │  │  Graphics, Pen, Brush,      │
│  Application, MessageBox,    │  │  Bitmap, Font, Matrix, ...  │
│  layout, events, resources   │  │  implemented over Cairo     │
└──────────────┬───────────────┘  └───────────┬─────────────────┘
               │ wraps Gtk.Widget             │ draws with Cairo.Context
┌──────────────▼──────────────────────────────▼─────────────────┐
│                   GtkSharp (GTK3 bindings)                    │
│              GTK / GDK / Cairo / Pango / GLib                 │
└──────────────────────────────┬────────────────────────────────┘
                               │ native
                 Windows ── Linux ── macOS (GTK runtime)
```

Supporting assemblies around that core:

- **`Net4x.System.Resources.Extensions`** — a drop-in reimplementation of
  `System.Resources.Extensions` (its `AssemblyName` *is* `System.Resources.Extensions`), so
  designer-generated `.resx` resources (including binary-serialized objects) deserialize correctly
  cross-platform. Forms and Drawing consume it as the `System.Resources.Extensions` package.
- **`Net4x.Gtk.Windows.FormsDesigner`** — MSBuild-integrated design-time support
  (`Microsoft.Build.*` tasks plus `build/` props+targets) so the **native Visual Studio WinForms
  designer** can open forms that target this framework.
- **`Net4x.ResxProcessor`** — a `net8.0` tool bundled inside the `Gtk.Windows.Forms` NuGet package
  under `tools/`; regenerates `.Designer.cs` from `.resx` during builds.
- **`Net4x.Gtk.Windows.Extensions`** — optional helper layer on top of the Forms library.

## The core pattern: public WinForms facade + GTK "base" widget

Every control is **two classes working as a pair**:

1. **The public facade** in namespace `System.Windows.Forms`
   (files under [Net4x.Gtk.Windows.Forms/System/Windows/Forms](Net4x.Gtk.Windows.Forms/System/Windows/Forms)) —
   the API your WinForms code sees: `Button`, `TextBox`, `Form`, `DataGridView`, …
2. **A GTK-derived base widget** in namespace `GtkSystem.Windows.Forms.GtkControls.ControlBase`
   (files under
   [Net4x.Gtk.Windows.Forms/GtkSystem/Windows/Forms/GtkControls/ControlBase](Net4x.Gtk.Windows.Forms/GtkSystem/Windows/Forms/GtkControls/ControlBase)) —
   a subclass of the actual GTK widget: `ButtonBase : Gtk.Button`, `FormBase : Gtk.Window`,
   `RichTextBoxBase` wrapping `Gtk.TextView`, `TreeViewBase` wrapping `Gtk.TreeView`, and so on.

The facade holds its widget in a public field named `self` and exposes it through the
`GtkControl` property. Taking `Button` as the canonical example
([Button.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Button.cs),
[ButtonBase.cs](Net4x.Gtk.Windows.Forms/GtkSystem/Windows/Forms/GtkControls/ControlBase/ButtonBase.cs)):

```csharp
public partial class Button : Control
{
    public readonly ButtonBase self = new();          // the real Gtk.Button
    public override object GtkControl => self;

    public Button() : base()
    {
        self.Clicked += Self_Clicked;                 // GTK signal → WinForms event
    }

    public override string Text                       // WinForms property → GTK state
    {
        get => ((Gtk.Label)self.Child).Text ?? string.Empty;
        set => ((Gtk.Label)self.Child).Text = value;
    }
}
```

Three translation duties are visible here and repeated across the whole library:

- **Properties** map WinForms semantics onto GTK state (`Text` ↔ the button's `Gtk.Label` child,
  `RightToLeft` ↔ `Gtk.TextDirection`, `TextAlign` ↔ `Xalign`/`Yalign`, …).
- **Events**: GTK signals are subscribed once and re-raised as WinForms events with compatible
  delegate signatures (`self.Clicked` → `Click`).
- **Identity**: the GTK window stores a back-pointer to its facade in `Widget.Data["Control"]`, which
  is how e.g. `Application.OpenForms` maps GTK toplevels back to `Form` instances.

### Control — the shared base

[Control.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs) is the common facade
base (`Control : Component, IControl, ISynchronizeInvoke, …`). It centralizes:

- **Low-level event wiring** — one place subscribes the underlying `Gtk.Widget`'s
  `ButtonPressEvent`, `MotionNotifyEvent`, `KeyPressEvent`, `FocusInEvent`,
  `SizeAllocated`, `Realized`, etc., and translates them into the WinForms event surface
  (`MouseDown`, `MouseMove`, `KeyPress`, `GotFocus`, `Resize`, …), converting GDK event structs into
  WinForms `EventArgs` types along the way.
- **Geometry & layout state** — `Bounds`, `Size`, `Location`, `ClientRectangle` read/write the GTK
  allocation (`Widget.Allocation`, `SetSizeRequest`); `Dock` and `Anchor` are re-implemented on top
  of GTK's measure/allocate cycle (anchors are applied per-widget as parents resize; `DockStyle.Fill`
  etc. adjust alignment/expansion). WinForms coordinate-based layout inside containers is emulated
  with GTK fixed/layout containers (`LayoutBase`, `ScrollableBoxBase`, `ViewportBase`).
- **Threading** — `ISynchronizeInvoke` (`Invoke`/`BeginInvoke`) marshals onto the GTK main loop.

### GtkControlOverride — custom painting and styling

Every `*Base` widget implements `IControlGtk`, which exposes a `GtkControlOverride`
([GtkControlOverride.cs](Net4x.Gtk.Windows.Forms/GtkSystem/Windows/Forms/GtkControls/ControlBase/GtkControlOverride.cs)).
This object centralizes what WinForms calls owner-drawing and what GTK calls CSS styling:

- Each base widget overrides `OnDrawn(Cairo.Context)` and calls `Override.OnPaint(cr, rect)` before
  the default GTK drawing — this is where `BackColor`, `BackgroundImage`, and the WinForms `Paint`
  event happen. The Cairo context is wrapped into a `System.Drawing.Graphics` so user paint handlers
  receive a standard `PaintEventArgs`.
- On `OnShown` it applies CSS classes (`Override.AddClass("Button")` …), connecting each control to
  the application-level stylesheet (see next section).

## Application lifecycle and theming

[Application.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Application/Application.cs) replaces the
WinForms message pump with the GTK main loop:

- The static constructor calls `Init()`, which runs `Gtk.Application.Init()`, creates a
  `Gtk.Application`, and registers a quit action — so merely touching any `System.Windows.Forms` type
  boots GTK.
- `Application.Run(mainForm)` shows the form's `FormBase` (`Gtk.Window`) and enters
  `Gtk.Application.Run()`; window destruction quits the loop.
- `Init()` also installs the **default control stylesheet**: an embedded GTK CSS block
  (`.DefaultThemeStyle`, class names like `.Button`, `.BorderNone`) loaded through `Gtk.CssProvider`
  for the whole screen. On first run it writes a `theme setup` config file next to the app; users can
  toggle the built-in style, follow the system theme (`AutoTheme`), point at a custom GTK theme
  folder, or add their own CSS — all without recompiling. This is how WinForms apps pick up native
  look-and-feel per platform.
- `Application.OpenForms` enumerates GTK toplevel windows and resolves them back to `Form` facades
  via the `Data["Control"]` back-pointer.

`Form` ([Form.cs](Net4x.Gtk.Windows.Forms/System/Windows/Forms/Form.cs)) extends the same facade
pattern to windows: `FormBase : Gtk.Window` is the `self`, and modal `ShowDialog()` semantics,
`DialogResult`, owner windows, close events (`FormClosing`/`FormClosed`) are mapped onto GTK
window/dialog behavior.

## Drawing: System.Drawing over Cairo

`Net4x.Gtk.Drawing` reimplements the `System.Drawing` object model with Cairo as the render backend:

- [Graphics.cs](Net4x.Gtk.Drawing/System/Drawing/Graphics.cs) owns a `Cairo.Context` plus the target
  rectangle; every `DrawLine/DrawArc/FillPath/DrawString/DrawImage` overload translates GDI+ calls
  into Cairo path/fill/stroke operations (text via Pango).
- `Pen`, `Brush`/`SolidBrush`, gradient brushes, `Region`, `Matrix`, `StringFormat` are state objects
  applied to the Cairo context at draw time. Gradient brushes deliberately mimic `System.Drawing`
  semantics (matrix transforms, clone semantics).
- `Bitmap`/`Image`/`Icon` wrap `Gdk.Pixbuf`; on `Pixbuf.SaveToBuffer` failure the code catches
  `GLib.GException`, falls back to a null `PixbufData`, and logs via
  `System.Diagnostics.GtkLogger.Debug`.
- Types whose GDI+ behavior is hard to reproduce are substituted from the **mono** submodule's
  reference implementations by the prepare pipeline.

The two libraries meet in the paint path: a GTK `OnDrawn` gives a `Cairo.Context`, the Forms library
wraps it in a `Graphics`, and the user's unmodified `OnPaint` code draws through the familiar
`System.Drawing` API.

Because the shim defines types with BCL names, test projects reference it with the **`gtk` extern
alias** to disambiguate from the real `System.Drawing`.

## Resources: making designer .resx work cross-platform

WinForms designer files lean heavily on `.resx` resources (images, icons, layout data), historically
via `BinaryFormatter`-based serialization. The chain here:

1. **`Net4x.System.Resources.Extensions`** ships as `System.Resources.Extensions` so
   `DeserializingResourceReader` & co. resolve to this cross-platform implementation. The Forms
   library additionally carries mirrored reader code under
   `GtkSystem/Resources/` (`ResourceManager`, `FastResourceComparer`, …) and a WinForms-aware
   `ComponentResourceManager` under `GtkSystem/ComponentModel/`.
2. **`Net4x.ResxProcessor`** (bundled in the package under `tools/`) regenerates
   `.Designer.cs` from `.resx` at build time.
3. **MSBuild glue**: the `Gtk.Windows.Forms` package ships `build/Gtk.Windows.Forms.props|targets`
   and `content/` source files (`System.ComponentModel.cs`, `System.Resources.ResourceManager.cs`)
   injected into consuming projects so resource type resolution points at the GTK implementations.
4. The library's own strings (e.g. `MessageBox` button captions) are localized `.resx` embedded
   resources (en, zh-Hans, it, fr, de, es), enriched by the prepare pipeline's translation step.
   Stock images/cursors are `EmbeddedResource`s under `Resources/System/` and `Resources/Cursors/`,
   loaded by assembly resource name at runtime.

## Design-time support

`Net4x.Gtk.Windows.FormsDesigner` keeps the **standard Visual Studio WinForms designer** usable:

- It contains MSBuild tasks/helpers ([Designer.cs](Net4x.Gtk.Windows.FormsDesigner/Designer.cs),
  [CsprojFixer.cs](Net4x.Gtk.Windows.FormsDesigner/CsprojFixer.cs),
  [Framework.cs](Net4x.Gtk.Windows.FormsDesigner/Framework.cs)) plus packaged
  `build/Gtk.Windows.FormsDesigner.props|targets`.
- At design time the project is presented to VS so the designer loads real
  `System.Windows.Forms` types; at build/run time the same code compiles against the GTK
  implementation. The facade classes carry designer attributes (`[DesignerCategory("Component")]`,
  `SubType` entries in the csproj) so the designer treats them like the originals.
- The Forms library references this project, and both ship as separate NuGet packages.

## Assembly & packaging conventions

- All shippable libraries target **`netstandard2.0`** (deliberately not tied to a .NET version) and
  are strong-named with `GtkForms.snk`; `InternalsVisibleTo` grants use the matching public key.
- Versioning and package pinning are centralized in
  [Directory.Build.Props](Directory.Build.Props) / [Directory.Nuget.Props](Directory.Nuget.Props)
  (`Net4xGtkWindowsFormsVersion`, `GtkSharpLatestVersion`, `VersionSuffix`).
- **Package IDs differ from project names**: `Net4x.Gtk.Windows.Forms` → `Gtk.Windows.Forms`,
  `Net4x.Gtk.Drawing` → `Gtk.Drawing`, `Net4x.System.Resources.Extensions` →
  `Gtk.System.Resources.Extensions`, etc. Packages are emitted to `Packages/`.

## Project dependency graph

```
                                  ┌────────────────────────────────┐
                                  │ Net4x.System.Resources.Ext.    │
                                  │ (assembly: System.Resources.   │
                                  │  Extensions)                   │
                                  └───────┬────────────────────────┘
                                          │ (as NuGet package)
        ┌──────────────────┐              ▼
        │ Net4x.Gtk.Drawing│──────► Net4x.Gtk.Windows.Forms ──────► Net4x.Gtk.Windows.Extensions
        └──────────────────┘              ▲            │
                                          │            │ bundles under tools/
        ┌─────────────────────────────┐   │            ▼
        │ Net4x.Gtk.Windows.Forms-    │───┘      Net4x.ResxProcessor
        │ Designer (MSBuild/design-   │
        │ time)                       │
        └─────────────────────────────┘

        GtkSharp (NuGet) ◄── referenced by Forms and Drawing
```

Tests (`Tests` solution folder, all xUnit): `Net4x.Gtk.Windows.Forms.Tests`,
`Net4x.Gtk.Drawing.Tests`, `Gtk.Windows.FormsDesigner.Tests`, `Net4x.ResxProcessor.Tests`,
`AutoClosingMessageBox.Tests`. The first two are strong-named to match the `InternalsVisibleTo`
grants and use the `gtk` extern alias for the Drawing shim.

## How a click travels through the stack (end-to-end example)

1. The user clicks a button. GTK dispatches the GDK button event to the `ButtonBase`
   (`Gtk.Button`) widget and emits its `Clicked` signal.
2. `ButtonBase` was subscribed by its facade in the `Button` constructor
   (`self.Clicked += Self_Clicked`).
3. The facade raises the WinForms `Click` event with `(object sender, EventArgs e)` — the user's
   original `button1_Click` handler runs untouched.
4. If the handler mutates state (`button1.Text = "Done"`), the property setter writes through to the
   GTK widget (`((Gtk.Label)self.Child).Text = …`).
5. GTK invalidates and redraws; `ButtonBase.OnDrawn` runs `GtkControlOverride.OnPaint` first
   (background, image, user `Paint` event via a Cairo-backed `Graphics`), then the themed GTK
   rendering — styled by the application-level CSS installed in `Application.Init()`.

The same shape — *facade property/event ⇄ base widget ⇄ GTK signal/allocation/draw* — applies to
every control in the library.

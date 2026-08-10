# Plan — replace `GtkSharp` (Gtk 3) with `Net4x.GtkSharp` (Gtk 4) in the Gtk.Windows.Forms solution

**Goal.** Swap the `GtkSharp` 3.24 NuGet package for `Net4x.GtkSharp` 4.22 across the whole solution
and port the shim's GTK back-end from Gtk 3 to Gtk 4, so the libraries, tests, and sample apps compile
against Net4x.GtkSharp **and run on a real Gtk 4.22 runtime**.

**Decisions taken (clarifying round).**

1. **Core action = the package swap**; use the `GtkSharp` repository (at `d:\CommonLibrary\GtkSharp`) as
   the reference to diff the visible surface/classes between Gtk 3 and Gtk 4 and drive the code migration.
2. **Scope = everything** — `Gtk.Windows.Forms`, `Gtk.Drawing`, all test projects, and the three sample
   apps (`GtkWinFormsApp`, `GtkSystemWinFormsApp11`, `AutoClosingMessageBoxApp`).
3. **Acceptance = runs on a real Gtk 4.22 runtime** (not just compile + headless tests).
4. **`Gtk.Layout` replacement = the plan evaluates `Gtk.Fixed` vs a custom `LayoutManager`** and
   recommends one (see §5.1).

**Reality check up front.** This is a **rewrite of the GTK back-end**, not a version bump.
`Net4x.GtkSharp` keeps the *assembly* names (`GtkSharp.dll`, namespace `Gtk`, plus `GdkSharp`,
`GLibSharp`, `GioSharp`, `CairoSharp`, `PangoSharp`) but targets **Gtk 4.22**, which *deleted*
`GtkContainer`/packing, the event-signal model, `ShowAll`, `OnDrawn`, `dialog.Run()`, stock items, the
Gtk 3 theming API, `Gtk.Menu`/`Gtk.MenuBar`, `Gtk.RadioButton`, and **`Gtk.Layout`**. Measured blast
radius in the shim: **~137 of 948** `.cs` files touch removed container/styling APIs, **~28** use removed
event signals, **~30** use dialogs/menus. The authoritative migration reference is
[GtkSharp/Docs/migrating-to-gtk4.md](../GtkSharp/Docs/migrating-to-gtk4.md).

---

## Progress log (kept current during execution)

Branch: `gtk4`. Milestones are committed individually; error counts are `dotnet build -c Release`.

> ⚠️ **SCOPE CHANGE (discovered milestone #9b).** The `Net4x.GtkSharp` package was **rebuilt to remove
> its Gtk3 member-compat shims** (matching the doc's *"no compatibility shim, and none should be added"*).
> Milestones #1–#9 compiled against an **older 26222** that still carried compat for `Widget.Window`,
> `Box.PackStart`/`Children`, `StyleContext`, `WindowPosition`, `WindowType`, `SkipTaskbarHint`,
> `Gdk.Screen`, etc. Against the **current** package those members are gone → **~1454 member-level errors**
> (the doc's full ~137-file blast radius). The removed-*types* work in #1–#9b is still valid and needed;
> the removed-*members* churn (`Window`→`Surface`, `PackStart`→`Append`, `Children`→`FirstChild`/`NextSibling`,
> `StyleContext`→CSS, window-hint props gone, `Gdk.Screen`→`Display`) is a **large additional layer** —
> pending a decision on how to proceed (see end of log).

| # | Milestone | Status | Commit | Notes |
|---|---|---|---|---|
| 1 | Pre-flight (B1–B4) | ✅ done | `8a395428` | pipeline left as-is; consumption verified; surface baseline in `gtk4-migration/` |
| 2 | Package swap (§1) | ✅ done | `f12e69e5` | `GtkSharp`→`Net4x.GtkSharp` (5 refs); `GtkSharpLatestVersion` 3.24.24→4.22.4, floats `.*` |
| 3 | `Gtk.Drawing` (Cairo/Gdk/Pango) | ✅ done — **0 errors** | `384c96b5` | `CairoSurfacePixbuf.ToPixbuf`; `FontFamily` via `CairoHelper`; `SystemIcons` IconTheme/LookupIcon |
| 4 | `Gtk.Windows.Forms` — Control.cs input core | ✅ done — Control.cs **0 errors** | `ec1320a3` | 241-line dispatch → 5 EventControllers; styling/Container/Atk fixes. **Forms 236→222** |
| 5 | `FormBase` (window/dialog lifecycle) | ✅ done — FormBase **0 errors** | _this commit_ | `EventArgs` alias; `DeleteEvent`→`CloseRequest`; `Drawn`→`DrawFunc`; `WindowStateEvent` removed. **Forms 222→200**. Found: Net4x.GtkSharp **kept** `PackStart`/`Add`/`StyleContext`/`Events`/`BorderWidth` compat → remaining work is removed *types*, not container churn |
| 6 | Menus/toolbars → **custom widgets** (§5.3) | ✅ compiles — menu tree **0 errors** | _this commit_ | Bases → `Popover`/`Box`/`Button`/`MenuButton`; items are Gtk4 widgets w/ `GestureClick`; `ShowAll`→`Show` (netstandard2.0, no default-interface); `ActivateCurrent` retyped. **Forms 200→138.** ⚠ compiles only — dropdown **popup/activation** behavior still to wire + verify at runtime |
| 7 | Containers/layout (`Gtk.Layout`→`Gtk.Fixed`) | ✅ base classes compile | _this commit_ | `LayoutBase`→`Gtk.Fixed` (§5.1 A); `RadioButtonBase`→`Gtk.CheckButton`+`Group`; `ScrollbarBase` de-generic'd→`Gtk.Scrollbar`; explicit `Add(Widget)` on Scrollable/GroupBox; **6 `OnDrawn` overrides removed** (no overridable `OnSnapshot` — BackColor→CSS, deferred); `Drawn`→`DrawFunc`, `ParentSet`/`SizeAllocated`/`WidgetEventAfter` deferred, `ButtonRelease`→`GestureClick`. **Forms 138→98** |
| 8 | Accessibility (`Atk` removed) | ✅ neutralized | _this commit_ | `AccessibleObject` peer-less (Name/Description → backing fields; Parent/Role/GetChild → defaults); `Atk.Role` maps dropped; dead `AtkPeer` setter blocks removed from `Control.cs`; `using Atk` dropped. **Forms 98→88.** Inert until a real `Gtk.Accessible` port |
| 9 | Drawing/override (`Drawn` removed → CSS) | ✅ drawing **0 errors** | _this commit_ | `GtkControlOverride.DrawnBackground` → `Action<object,Cairo.Context>`; 6 per-control `Drawn` handlers removed (Button/Label/LinkLabel/ComboBox/ListView/PrintPreview). No `Widget.Drawn`/overridable `OnSnapshot` → all custom paint deferred to CSS. **Forms 88→72** |
| 9b | Removed-members churn (full no-compat) — kick-off | ✅ done | `7335da0d`,`cdefad6a` | Neutralized removed *types* (Clipboard/NotifyIcon/Cursor/CellRenderer stubs). Then began the removed-*members* layer: `ShowAll`→`Show`, `PackStart`/`PackEnd`→`Append`, `Expand`→`Hexpand`/`Vexpand`, dropped Gtk-only props (BorderWidth/NoShowAll/Events/DrawIndicator/…), `WindowPosition` removed (WM-managed note). **Forms 1454→1046** |
| 9c | Container model (`Container.Add`/`Remove`/`Children` removed) | ✅ done | `41b8653b` | Backend-only `Gtk4Compat` extension helpers in the `Gtk` namespace: `GetChildrenArray()` (FirstChild/NextSibling → `Widget[]`), `AddChild()`/`RemoveChild()` (runtime dispatch per container type: Box.Append, Overlay.Child/AddOverlay, `*.Child`, Fixed.Put, Grid.Attach, Notebook pages). Edits applied **only at exact compiler-error positions** so `List<T>.Add`/collection APIs are untouched. **Forms 1046→886** |
| 9d | `Widget.Window` (Gdk.Window removed) | ✅ done | `6bd81f7b` | `IsRealized` guards; `Gtk4Compat.GetOrigin` (translate-to-root; Wayland has no global coords); invalidation→`QueueDraw`; cursor→`Widget.Cursor`; z-order neutralized; `Screen.cs`/monitor→`Display.Monitors`+`GetMonitorAtSurface`+`Geometry`; `Control.Shape` inert (Gtk4 dropped window shaping). **Forms 886→764** |
| 9e | Pixbuf/Image/IconSize/Button icons | ✅ done | `3b6dc9f0` | `Gtk4Compat.ToPixbuf` (Cairo-surface→straight-RGBA); `Button(iconName,IconSize)`→`{IconName}`; `Relief/ReliefStyle`→`HasFrame`; `Image.NewFromIconName` (no size); `Strip*Button` `IconWidget`→`Child`; ToolStrip `Toolbar/MenuBar` branches→Box orientation. **Forms 764→570** |
| 9f | Dialog `Run()` (sync via nested loop) | ✅ done | `3b6dc9f0` | `Gtk4Compat.RunDialog(Gtk.Dialog)`: present modal + nested `GLib.MainLoop` quitting on `response`/`close` — preserves synchronous `ShowDialog`. Wired Color/Font/File/MessageBox/Form. **Forms 570→552** |
| 9g | `DateTime`↔`GLib.DateTime`, margins, event masks, focus, appearance, WindowPosition | ✅ done | `c9ed4de0` | `Gtk4Compat.ToDateTime/ToGLibDateTime` (Gtk4 `Calendar.Date`); `SetMargin`; event masks + `Xalign/Yalign` neutralized; `IsFocus`→`GrabFocus`; `DrawIndicator`→`Appearance` field; `{WindowPosition=0}` initializers dropped. **Forms 552→424** |
| 9h | Long tail (Toplevel/Measure/Paned/window hints/etc.) | ✅ done | `6bd81f7b`→`c9ed4de0` | `Gtk4Compat.GetToplevel/GetPreferredHeight/GetPointer`; `Label.LineWrap`→`Wrap`; `Iconify`→`Minimize`; `Paned.Pack1/2`→`StartChild/EndChild`; `Button/CheckButton.Image`→`Child`; `ReorderChild`→`ReorderChildAfter`; `WidgetPath`→`Name`; Window icon-from-Pixbuf/tray icon dropped. **Forms 424→272** |
| 9i | Text-insert args, clipboard, Control.cs, Application, dialogs (File/Color) | ✅ done | `78f31303`→`2c6908f4` | `TextInsertedArgs.NewText`→`Text`+`Marshal.ReadInt32(Position)`; `Gtk4Compat.SetClipboardText/GetClipboardTextSync` (Gtk4 async read deferred); `Gdk.Screen`/`Gtk.Main`/`CursorType`/`CssProvider.LoadFromData(void)`; `Application.Quit` static; ColorDialog `SetRgba`(float); FileChooser GFile (`File/Files`, `NewForPath`, `IListModel`). **Forms 272→66** |
| **10a** | **Both shipping libraries compile on Gtk 4.22** | ✅ **done — 0 errors** | `42c92442` | **`Gtk.Windows.Forms`: 1454 → 0**; `Gtk.Drawing`: 0. All GTK-backend only via the `Gtk4Compat` global-namespace helper + per-site fixes — **no `System.Windows.Forms`/`System.Drawing` public API changed**. Deferred behavioral items (menu popup/activation, custom paint→CSS/DrawFunc, RadioButton auto-group, async clipboard/paste, screen-coord accuracy, cursor textures, window-state minimize, dock-anchor re-layout) are commented `TODO(gtk4)` and slated for #11. |
| 10b | Test project + sample apps | ✅ **done — 0 errors** | `2cda3ad3` | Full `Gtk.Windows.Forms.sln` compiles on Gtk 4.22. Sample `AddProviderForScreen`→`AddProviderForDisplay`. Tests: updated to Gtk4 equivalents where the oracle survives (`FlowBox.Children`→`GetChildrenArray`, `PanedChild`→`ResizeStart/EndChild`, `Grid.Add`→`AddChild`, `CheckButton.Group` leader); **skipped-with-explicit-reason** (not fake-passed) where the behavior is genuinely removed — window shaping, `Gdk.CursorType` (8 methods), `Gtk.RadioButton` group-array; removed-widget-prop asserts (`Xalign/Yalign/CanDefault/BorderWidth`) commented while surrounding WinForms-API asserts still run. UITests `Widget.Draw(cairo)`→ deferred `WidgetPaintable` snapshot. |
| 11 | **Run on Gtk 4.22 runtime** (acceptance) | ☐ todo | — | Windows + WSL Debian available. Verify input mapping (wheel ×120, scroll sign, click counting, `CloseRequest` veto). Then wire the deferred `TODO(gtk4)` behavior (below) and un-skip the corresponding tests. |

### Deferred `TODO(gtk4)` behavior to wire at #11 (all compile today; commented in-code)
Menu dropdown popup/activation (`Popover.Popup`, item activation, `ContextMenuStrip` right-click) · all custom painting → CSS providers or `Gtk.DrawingArea`+`DrawFunc` (`BackColor`/bg-image, Button/Label/ComboBox cell/ListView header/print-preview paper, owner-draw tabs & list items) · `RadioButton` auto-grouping by shared parent (over `CheckButton.Group`) · async clipboard **paste** (`Gdk.Clipboard.ReadTextAsync` + nested loop; copy/cut already work) · named/texture `Gdk.Cursor` mapping (custom cursors fall back to default) · window minimize-state query (Gtk4 exposes maximize/fullscreen only) · screen-coord accuracy on Wayland (`GetOrigin` is root-relative) · dock-anchor re-layout on resize (`SizeAllocated`/`ParentSet`) · `Control.Region` visual (CSS clip-path) · `Form` Activated/Deactivate via `EventControllerFocus` · accessibility via `Gtk.Accessible` (Atk fully removed).

**Deferred/○ carried forward (restore in the noted milestone):**
- Runtime re-layout on resize/parent-change (`SizeAllocated`/`ParentSet`) — deferred in #4 → restore in #7 via the `*Base` allocate cycle.
- `ContextMenuStrip` right-click popup — TODO in `Control.cs` → wire in #6.
- Accessibility — **neutralized** in #8 (`Atk` fully removed in Gtk4). `AccessibleObject` compiles and round-trips Name/Description but exposes no real a11y tree; a genuine `Gtk.Accessible`/`Gtk.AccessibleRole` port is a future task (not required for compile/run).
- Window maximize/restore icon on *external* state change (`WindowStateEvent`) — removed in #5 → restore via `notify::maximized` (or fold into #7).
- **Menu popup + activation behavior** (#6) — the custom-widget menu tree *compiles* but the dropdown popup (`Popover.Popup`/`PopupAt`), menu-bar layout (Box of `MenuButton`s), item activation, and `ContextMenuStrip` right-click are **not yet wired**; do this + verify on Gtk4 in #11 (runtime). Menu-item hover-highlight (removed `Gtk.MenuItem.Selected`) → CSS.
- **All custom painting → CSS** (#7 + #9) — every `OnDrawn`/`Drawn` draw hook was removed (Gtk4 has no `Widget.Drawn` and Net4x.GtkSharp no overridable `OnSnapshot`). WinForms `BackColor`/background image, and per-control custom rendering (`Button`/`Label`/`LinkLabel`/`ComboBox` cell/`ListView` header/**print-preview paper**) must be re-expressed as CSS providers (or, for print-preview, a `Gtk.DrawingArea`+`DrawFunc`). This is a dedicated **styling/paint pass** before runtime (#11).
- **RadioButton auto-grouping by shared parent** (#7) — the `ParentSet` walk (removed `Container.Children`+`Gtk.RadioButton`) is dropped; reimplement over `CheckButton.Group` + `FirstChild`/`NextSibling`, or require explicit `RadioButtonBase(group)`. GroupBox scroll-offset tweaks (`Fixedcontaner` `SizeAllocated`/`WidgetEventAfter`) also deferred.

---

## 0. BLOCKING pre-flight — resolve before any code changes

- **B1 — Where does the migrated code live? — DECIDED.** The source under `Gtk.Windows.Forms/` and
  `Gtk.Drawing/` is pipeline-generated from the `gtksystem-windows-forms` submodule (Gtk 3). **Decision:
  leave the pipeline (`PrepareProjects`/`AlignProjects`/`AlignSamples`) exactly as-is and edit the
  projects in the `Gtk.Windows.Forms` solution directly.** The migration is a set of hand edits to the
  generated trees + the sample/test projects — no pipeline changes, no patch series, no submodule fork.
  Accepted consequence: re-running `PrepareProjects` would regenerate Gtk 3 code and overwrite the port,
  so **do not run the prepare pipeline on this line** while/after migrating (there is no need to; the
  Gtk 4 code is maintained directly). No `CLAUDE.md`/pipeline edits are required by this choice.
- **B2 — Confirm the Net4x.GtkSharp consumption model & native runtime.** The local `Packages/` feed has
  `Net4x.GtkSharp 4.22.4.26222` plus `.Ref`/`.Runtime`/`.Sdk`/`.Manifest`. The GtkSharp repo's own
  template references it as a **plain** `<PackageReference Include="Net4x.GtkSharp" Version="4.22.4.*" />`
  under `Microsoft.NET.Sdk` (see
  `GtkSharp/Source/Templates/GtkSharp.Template.CSharp/content/.../GtkNamespace.csproj:16`). Verify a
  bare PackageReference restores `GdkSharp`/`GLibSharp`/`CairoSharp`/`PangoSharp` transitively (as
  GtkSharp 3.24 did) and that **a Gtk 4.22 native runtime** is available on the dev/CI machine (from
  `Net4x.GtkSharp.Runtime` or a system Gtk 4). Required for the §7 "runs on Gtk4" bar; the doc warns many
  breaks surface only at runtime as an `NullReferenceException` at the call site.
- **B3 — Baseline the surface diff (see §2) before editing** so every code change maps to a concrete
  Gtk 3→4 delta rather than guesswork.
- **B4 — Branch from a clean tree** (the Net4x-prefix rename work is a separate concern; do this on its
  own branch off whatever is current).

---

## 1. The package swap (mechanical)

Five `PackageReference` sites, all on the `GtkSharp` metapackage via two version variables:

| File:line | Current | New |
|---|---|---|
| [Gtk.Windows.Forms/Gtk.Windows.Forms.csproj:195](Gtk.Windows.Forms/Gtk.Windows.Forms.csproj) | `GtkSharp` `$(GtkSharpPackageVersion)` | `Net4x.GtkSharp` `$(GtkSharpPackageVersion)` |
| [Gtk.Drawing/Gtk.Drawing.csproj:67](Gtk.Drawing/Gtk.Drawing.csproj) | `GtkSharp` | `Net4x.GtkSharp` |
| [Resources/Gtk.Drawing.csproj:57](Resources/Gtk.Drawing.csproj) | `GtkSharp` | `Net4x.GtkSharp` (overlay copy — keep in sync) |
| [GtkSystemWinFormsApp11/GtkSystemWinFormsApp11.csproj:25](GtkSystemWinFormsApp11/GtkSystemWinFormsApp11.csproj) | `GtkSharp` `$(GtkSharpLatestVersion).*` | `Net4x.GtkSharp` |
| [GtkWinFormsApp/GtkWinFormsApp.csproj:74](GtkWinFormsApp/GtkWinFormsApp.csproj) | `GtkSharp` `$(GtkSharpLatestVersion).*` | `Net4x.GtkSharp` |

Version variables:

- [Directory.Nuget.Props:13](Directory.Nuget.Props) — `<GtkSharpLatestVersion>3.24.24</…>` → **`4.22.4`**.
- [Directory.Build.Props:35](Directory.Build.Props) (Release) — `$(GtkSharpLatestVersion).95` → the
  Net4x.GtkSharp build suffix (e.g. `.26222`, matching the feed) **or** float `.$(VersionBuildSuffix)`.
- [Directory.Build.Props:38](Directory.Build.Props) (Debug) — `$(GtkSharpLatestVersion).*` stays floating.

Also:

- **`api-diff/tool/ApiDiff.csproj`** and any other tool that references GtkSharp assemblies at 3.24 —
  grep `GtkSharp` again after the swap (`git grep -n 'GtkSharp' -- '*.csproj' '*.props'`) and retarget.
- The **pipeline** version pin: `Prepare.Common/Projects/VersionPrepare.cs` and any prepare code that
  reads `GtkSharpLatestVersion` — confirm it still writes the value (name unchanged, value 4.22.4).
- **`Directory.Build.Props` prose** around the pin (lines 26–33) describes GtkSharp 3.x floating
  semantics — update to Gtk 4.

> After only the swap, expect **hundreds of compile errors** — that is the intended starting point
> ([doc §"Start by making it fail to compile"](../GtkSharp/Docs/migrating-to-gtk4.md)). §3–§5 close them.

---

## 2. Method — derive the Gtk 3→4 surface diff from the GtkSharp repo

Reuse this repo's existing surface-diff harness. `api-diff/tool` already runs a `MetadataLoadContext`
public/protected diff between two assemblies (built for the `System.Windows.Forms` comparison). Point the
same tool at the two GtkSharp assemblies to get an exact, per-type delta for the classes the shim uses:

```sh
# old (Gtk3) vs new (Gtk4) GtkSharp.dll — and likewise Gdk/GLib/Cairo/Pango
GTK3=~/.nuget/packages/gtksharp/3.24.24.95/lib/net6.0/GtkSharp.dll
GTK4=~/.nuget/packages/net4x.gtksharp/4.22.4.26222/lib/net10.0/GtkSharp.dll   # confirm TFM path
ApiDiff.exe "$GTK3" "$GTK4" <dir> gtk3v4 Gtk
```

Then intersect the "removed/changed members" list with the **types the shim actually consumes**, so the
work list is grounded in metadata, not recall. Produce a checked-in `gtk3-to-gtk4-surface.md` mapping
keyed by the shim's base widgets (below). The GtkSharp repo's `Source/Samples` port notes and
`migrating-to-gtk4.md` tables fill in the *behavioural* replacements the metadata diff can't show
(e.g. `Clicked` connect-after-default, `CloseRequest` inverted `RetVal`).

---

## 3. The migration core — the shim's Gtk-derived base widgets

The GTK back-end is **27 `*Base` classes** that subclass GTK widgets, under
`Gtk.Windows.Forms/GtkSystem/Windows/Forms/GtkControls/ControlBase/` (plus the `MenuStripBase/` tree).
Each is a migration unit. Ordered by severity (⛔ = subclasses a **removed** Gtk 4 type → redesign):

| Base class | Gtk 3 base | Gtk 4 disposition |
|---|---|---|
| ⛔ `LayoutBase` | `Gtk.Layout` | **removed** → `Gtk.Fixed` or custom `LayoutManager` (§5.1) — the WinForms coordinate-layout linchpin |
| ⛔ `ToolStripDropDownBase` | `Gtk.Menu` | **removed** → `GMenu` model + `PopoverMenu` |
| ⛔ `ToolStripBase` / `MenuStripBase` | `Gtk.MenuBar` | **removed** → `PopoverMenuBar` + `GMenu` |
| ⛔ `RadioButtonBase` | `Gtk.RadioButton` | **removed** → `Gtk.CheckButton` with `Group` |
| `BoxBase` | `Gtk.Box` | survives; `PackStart/PackEnd` → `Append` + `Hexpand/Halign` |
| `ScrollableBoxBase` | `Gtk.ScrolledWindow` | survives; `Add` → `Child`; viewport implicit |
| `ViewportBase` / `PictureBoxBase` / `ListViewBase` / `TrackBarBase` | `Gtk.Viewport` | survives but no longer a `Container.Add`; set `Child` |
| `FormBase` | `Gtk.Dialog` | survives; `Run()` removed → async `Response`; `DeleteEvent`→`CloseRequest` (invert `RetVal`) |
| `ComboBoxBase` | `Gtk.ComboBoxText` | survives; model/APIs adjusted |
| `TabControlBase` | `Gtk.Notebook` | survives; child add API changed |
| `SplitContainerBase` / `PropertyGridBase` | `Gtk.Paned` | survives; `Pack1/Pack2` → `StartChild/EndChild` |
| `GroupBoxBase` | `Gtk.Frame` | survives; `Add` → `Child` |
| `FlowLayoutPanelBase` | `Gtk.FlowBox` | survives; insert API changed |
| `ButtonBase` `TextBoxBase` `CheckBoxBase` `LabelBase` `LinkLabelBase` `NumericUpDownBase` `MonthCalendarBase` `SwitchBoxBase` `ProgressBarBase` | `Button`/`Entry`/`CheckButton`/`Label`/`LinkButton`/`SpinButton`/`Calendar`/`Switch`/`LevelBar` | mostly survive; event signals → controllers, `ShowAll` drop, styling → CSS |

For each: apply the container/event/drawing/style rules from §4, verified against the §2 surface diff.

---

## 4. Cross-cutting subsystems (apply across the files above)

Ordered as the doc recommends; each row lists where it concentrates in the shim.

1. **Containers & packing** (largest, mechanical — ~137 files). `Add`/`PackStart`/`PackEnd`/`Children`/
   `Remove` → parent-specific API (`Child`, `Append`, `Attach`, `FirstChild`/`NextSibling`, `Unparent`);
   packing `expand`/`fill` → `Hexpand`/`Vexpand`/`Halign`/`Valign` + the four `Margin*` props. Hubs:
   `LayoutBase`, `BoxBase`, `ScrollableBoxBase`, `ViewportBase`, and `Control.cs`'s child management.
2. **Visibility.** Delete `ShowAll()` (widgets visible by default); use `Visible=false` where hiding was
   intended. `SizeRequest`/`SizeAllocate` → `Measure(orientation, forSize, …)`.
3. **Input events → controllers** (~28 files). `ButtonPressEvent`/`MotionNotifyEvent`/`KeyPressEvent`/
   `FocusIn/Out`/`ScrollEvent`/`Enter/LeaveNotify` → `GestureClick`/`EventControllerMotion`/`…Key`/
   `…Focus`/`…Scroll` attached via `AddController`. Central hub:
   `Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs` (the doc's "one place subscribes the
   widget's events"). Drop `AddEvents` masks. `Gtk.Drag.*` → `DragSource`/`DropTarget`.
   **Watch the silent break:** a lambda `Clicked +=` connects *after* the default handler — for
   handlers that must precede it, use a named method with `[GLib.ConnectBefore]`.
4. **Drawing.** `OnDrawn`/`Drawn` (Cairo `cr` from allocation) → `DrawingArea.DrawFunc` or override
   `OnSnapshot`; Cairo body ports unchanged, only how `cr`/size arrive. Hub:
   `GtkSystem/.../ControlBase/GtkControlOverride.cs` and `Gtk.Drawing`'s Cairo usage. `Gdk.Threads.Enter/
   Leave` → `GLib.Idle.Add`.
5. **Dialogs → async.** Remove every `.Run()`; connect `Response` / use `Gtk.FileDialog`/`AlertDialog`/
   `ColorDialog`/`FontDialog`. Hubs: `FormBase` (`Gtk.Dialog`), `MessageBox`/`NonStaticMessageBox.cs`,
   and any file/color/font choosers. This is the change most likely to force control-flow restructuring
   (WinForms `ShowDialog()` is synchronous and returns a `DialogResult`; emulate with a nested
   `GLib.MainLoop` **only** where the WinForms contract truly needs blocking).
6. **Menus/actions.** `Gtk.Menu`/`MenuBar`/`MenuItem`/`ImageMenuItem`/`Toolbar`/`AccelGroup` →
   `GMenu` + `PopoverMenu(Bar)` + `GAction`/`SimpleActionGroup`; accelerators → `ShortcutController`.
   Hubs: the `MenuStripBase/` tree and `ToolStrip*` bases.
7. **Styling → CSS.** `StyleContext.AddClass`/`ModifyBg`/`ModifyFont` → `AddCssClass` + `CssProvider`
   (`AddProviderForDisplay(Gdk.Display.Default, …)`). Grep `StyleContext`/`ModifyBg`/`Gtk.Rc`.
8. **Gdk types.** `Gdk.Window` → `Gdk.Surface` (`widget.Native`); `Gdk.Screen` → `Gdk.Display`
   (monitors are a `GListModel`); `Gdk.Color` → `Gdk.RGBA`; `GtkClipboard` → `GdkClipboard` (async).
   The WinForms `Clipboard` shim and screen/monitor code are the hotspots.
9. **Lists/trees.** `TreeView`/`ListStore`/`TreeStore`/`CellRenderer` **still ship** in 4.22 (deprecated).
   **Decision:** keep them for now (compiles; `CellRenderer` subclasses need `get_size` attention) —
   the shim's `GtkRender.CellRenderer*` and `ListViewBase`/`DataGridView` back-ends are large; defer the
   `GListModel`/`ColumnView` rewrite. Note the deprecation warnings.

---

## 5. Specific design decisions

### 5.1 `Gtk.Layout` replacement (evaluate both, then commit)

`LayoutBase : Gtk.Layout` is the substrate for WinForms absolute x,y positioning and the Dock/Anchor
arithmetic (see the dock/anchor logic the current code centralizes in `Control.cs` and the geometry
notes in `Architecture.md`). `Gtk.Layout` is removed in Gtk 4.

- **Option A — `Gtk.Fixed`.** Survives in Gtk 4 and supports `Put(child, x, y)`/`Move(child, x, y)` —
  the closest drop-in for coordinate placement. Lowest effort. Caveats: `Fixed` does no scrolling of its
  own (the old `Gtk.Layout` had scroll adjustments — route scrolling through the enclosing
  `ScrolledWindow`), and it does not clip/allocate exactly like `Layout`; verify against the
  measure/allocate expectations the shim's layout code encodes.
- **Option B — custom `Gtk.LayoutManager`.** Implement `Measure`/`Allocate` explicitly on a manager
  attached to a plain `Gtk.Widget`/`Box`. Most faithful to WinForms Dock/Anchor/measure semantics and to
  the arrangement rules already implemented, but materially more work and new surface to test.

**Recommendation:** start with **A (`Gtk.Fixed`)** to get the tree compiling and running, keeping the
managed Dock/Anchor arithmetic that already pushes positions to margins/allocations; escalate specific
containers to **B** only where `Fixed` demonstrably can't reproduce the arrangement. Decide per the §2
diff + the existing `TableLayoutPanel`/`FlowLayoutPanel` behaviour notes.

### 5.3 Menu/toolbar model — DECIDED: custom Gtk 4 widgets (not GMenu)

Gtk 4 removed `Gtk.Menu`/`MenuBar`/`MenuItem`/`Toolbar`/`ToolItem`/`ToolButton`/`MenuToolButton`
(no compat), and the idiomatic replacement is a `GMenu` **data model** rendered by `PopoverMenu`.
But the shim's `ToolStrip`/`MenuStrip` model is **widget-based**: `IToolMenuItem` exposes each item as a
`Gtk.Widget`, items carry `Image`/`CheckState`/custom display, and `Control` treats them as widgets.
Rebuilding on `GMenu` would discard that fidelity. **Decision: rebuild the menu/toolbar tree as custom
widgets on surviving Gtk 4 types**, preserving the widget/image/event model:

| Shim class | Gtk 3 base (removed) | Gtk 4 base |
|---|---|---|
| `ToolStripDropDownBase`, `StripDropDown` | `Gtk.Menu` | `Gtk.Popover` |
| `MenuStripBase`, `ToolStripBase` | `Gtk.MenuBar` | `Gtk.Box` (horizontal) of `MenuButton`s |
| `StripMenuItem` | `Gtk.MenuItem` | `Gtk.Box` (a menu row) |
| `StripToolItem` | `Gtk.ToolItem` | `Gtk.Box` |
| `StripToolButton` | `Gtk.ToolButton` | `Gtk.Button` |
| `StripMenuToolButton` | `Gtk.MenuToolButton` | `Gtk.MenuButton` |
| `StripSeparator` | (none) | `Gtk.Separator` |

Consequences to wire (need Gtk4-runtime verification): dropdown **popup** (`Popover.Popup`/`PopupAt`),
item **activation**, `ContextMenuStrip` right-click (the TODO in `Control.cs`), and cascading secondary
API (`CheckButton.DrawIndicator`/`ReliefStyle`/`AlwaysShowImage`/`ImagePosition`).

### 5.2 Synchronous `ShowDialog` over async Gtk 4 dialogs

WinForms `Form.ShowDialog()` returns a `DialogResult` synchronously. Gtk 4 has no nested main loop by
default. Emulate the blocking contract with a scoped `GLib.MainLoop` run/quit around the dialog's
`Response`, confined to the `ShowDialog`/`MessageBox.Show` shims — do **not** reintroduce a general
nested-loop facility.

---

## 6. Sample apps + tests

- **Sample apps** (`GtkWinFormsApp`, `GtkSystemWinFormsApp11`, `AutoClosingMessageBoxApp`): swap the
  package ref (§1); they exercise the shim, not GtkSharp directly, so most breaks are inside the
  libraries. Any direct GtkSharp use in a sample follows the same §4 rules.
- **Tests**: `Gtk.Windows.Forms.Tests` / `Gtk.Drawing.Tests` construct controls and assert GTK-backed
  behaviour. Expect assertions tied to Gtk 3 specifics (event names, container child counts, `ShowAll`,
  style classes) to need updating. The suites are the fastest regression signal for §3–§4; keep them
  green per subsystem as you go. `Prepare.Tests` is unaffected by the GTK version (pipeline logic) —
  but if B1 retires the pipeline for this line, prune the tests that assert regeneration of these trees.

---

## 7. Verification (in order)

1. **Restore/compile the swap alone** — confirm B2 (transitive Gdk/GLib/Cairo/Pango at 4.22, native
   runtime present), catalogue the compile errors as the work list.
2. **Per-subsystem build green** — drive the error count to zero subsystem by subsystem (§4 order:
   containers → visibility → events → drawing → dialogs → menus → styling → gdk), rebuilding
   `Gtk.Windows.Forms.sln` after each.
3. **Test suites** — `dotnet test`; keep 2523 (Forms) + 1216 (Drawing) + the rest green, updating
   Gtk-3-specific assertions to Gtk 4 equivalents (not deleting coverage).
4. **Run on Gtk 4.22** (the acceptance bar) — launch each sample app on an installed Gtk 4 runtime and
   walk the paths the doc flags as runtime-only: window close, button click vs `Activate`, dialogs
   (open/save/message), menus, drawing/repaint, clipboard, scrolling, DnD. A removed native call surfaces
   as an `NullReferenceException` at the call site — treat every such NRE as an unported path.
5. **Grep converges to empty** — re-run the doc's blast-radius grep over the shim until it returns
   nothing:
   ```sh
   git grep -lE "\b(Container|VBox|HBox|PackStart|PackEnd|ShowAll|EventBox|Bin|Stock|IconFactory|UIManager|StatusIcon|Gdk\.Window|Gdk\.Screen|Gdk\.Color|StyleContext|ButtonPressEvent|MotionNotifyEvent|KeyPressEvent|OnDrawn|DeleteEvent|AccelGroup|\.Run\(\))\b" -- 'Gtk.Windows.Forms/**/*.cs' 'Gtk.Drawing/**/*.cs'
   ```

---

## 8. Risks

- **Silent behavioural changes** (compile-clean, wrong at runtime) — the doc's "dangerous part":
  `Clicked` connect-after-default; `Activate()` doesn't raise `Clicked`; `CloseRequest` `RetVal` inverted
  vs `DeleteEvent`; `SimpleAction.change-state` semantics; leaked `Cairo.Path`/surface crashing the
  process from the finalizer; `int`→`IntPtr` implicit conversions hitting raw-pointer ctors
  (`new ValueArray(2)` → use `2u`); `GLib.Bytes.Data` returns `null` on empty. Test by **running**, not
  compiling.
- **Generated-source overwrite** — per B1 the pipeline is left untouched, so **running `PrepareProjects`
  would revert the port** (it regenerates Gtk 3 code). Simply do not run the prepare pipeline on this
  line during or after the migration; the Gtk 4 trees are maintained by hand.
- **Absolute-layout fidelity** — WinForms coordinate layout on `Gtk.Fixed` may diverge in
  measure/allocate/clipping/scrolling; this is the highest-risk functional area (§5.1).
- **Dialog control-flow** — turning synchronous `ShowDialog`/`MessageBox.Show` into a scoped nested loop
  is subtle (re-entrancy, modality, `DialogResult` plumbing).
- **Native runtime coupling** — Net4x.GtkSharp resolves native functions at runtime against Gtk 4.22;
  a mismatched/absent native runtime fails only when a path executes. Pin the runtime for CI.
- **Deprecated TreeView tail** — deferring the list-stack rewrite leaves deprecation warnings and a known
  future migration; acceptable, but track it.

---

## 9. Suggested sequencing

1. Branch (pipeline left as-is per B1 — no pipeline/CLAUDE edits).
2. Package swap (§1) + confirm restore/native runtime (§7.1); capture the error list.
3. Build the §2 surface diff (`gtk3-to-gtk4-surface.md`).
4. Migrate subsystem by subsystem (§4 order), keeping tests green (§6/§7.3) after each.
5. `Gtk.Layout` → `Gtk.Fixed` early (unblocks the container tree); revisit per §5.1.
6. Sample apps + run-on-Gtk4 acceptance (§7.4); drive the grep to empty (§7.5).
7. Update docs (`Architecture.md`, `Solution-Projects.md`, README) to say Gtk 4 / Net4x.GtkSharp.

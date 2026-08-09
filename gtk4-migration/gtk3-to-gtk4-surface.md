# Gtk 3 → Gtk 4 surface baseline (pre-flight B3)

Metadata diff of the **public** type surface in namespace `Gtk` between
`GtkSharp 3.24.24.95` (Gtk 3) and `Net4x.GtkSharp 4.22.4.26222` (Gtk 4), produced via
`MetadataLoadContext`. This is the baseline the migration maps each change against
([../Migrate-to-Net4x-GtkSharp-Gtk4-Plan.md](../Migrate-to-Net4x-GtkSharp-Gtk4-Plan.md) §2).

## Headline

| | Gtk 3 (3.24) | Gtk 4 (4.22) |
|---|---|---|
| Public types in namespace `Gtk` | 1322 | 1105 |
| Removed in Gtk 4 | — | **704** |
| Added in Gtk 4 | — | **487** |

Full lists: [gtk-namespace-removed-in-gtk4.txt](gtk-namespace-removed-in-gtk4.txt),
[gtk-namespace-added-in-gtk4.txt](gtk-namespace-added-in-gtk4.txt). (Counts include the generated
`*Args`/`*Handler`/signal helper types, so the *widget*-level churn is a fraction of 704.)

## Shim-relevant removed types → the migration core

The shim's 27 `*Base` widgets subclass Gtk widgets. These parents are **gone in Gtk 4** (⛔ = redesign):

| Removed Gtk 3 type | Shim base affected | Gtk 4 replacement |
|---|---|---|
| ⛔ `Gtk.Layout` | `LayoutBase` | `Gtk.Fixed` (survives — confirmed) or custom `LayoutManager` (plan §5.1) |
| ⛔ `Gtk.Menu` | `ToolStripDropDownBase` | `GMenu` model + `PopoverMenu` |
| ⛔ `Gtk.MenuBar` | `ToolStripBase`, `MenuStripBase` | `PopoverMenuBar` + `GMenu` |
| ⛔ `Gtk.MenuItem`, `Gtk.ImageMenuItem` | menu item wrappers | `GMenuItem` / `GAction` |
| ⛔ `Gtk.RadioButton` | `RadioButtonBase` | `Gtk.CheckButton` with `Group` |
| `Gtk.Container`, `Gtk.Bin`, `Gtk.EventBox` | container plumbing (`Control.cs`, `LayoutBase`, …) | parent-specific `Child`/`Append`; every widget can take a controller |
| `Gtk.HBox`, `Gtk.VBox`, `Gtk.Table`, `Gtk.Alignment`, `Gtk.Misc`, `Gtk.Toolbar` | misc layout/toolbar usage | `Box`+`Orientation` / `Grid` / `Halign`+`Margin*` |

**Confirmed to SURVIVE** (subclassing bases only need API-level changes, not a new parent):
`Fixed`, `Dialog`, `Viewport`, `Frame`, `Notebook`, `Paned`, `FlowBox`, `Calendar`, `Switch`,
`LevelBar`, `SpinButton`, `LinkButton`, `ComboBoxText`, `Entry`, `Label`, `CheckButton`, `Button`,
`Box`, `ScrolledWindow`.

→ So the container linchpin (`LayoutBase : Gtk.Layout`) has a surviving drop-in in `Gtk.Fixed`
(plan §5.1 Option A), and only **4 shim bases** (`LayoutBase`, `ToolStripDropDownBase`,
`ToolStrip/MenuStripBase`, `RadioButtonBase`) subclass a removed type.

## Regenerating / extending to other namespaces

Re-run the same dump for `Gdk` (expect `Gdk.Window`→`Surface`, `Gdk.Screen`→`Display`,
`Gdk.Color`→`RGBA`), `GLib`, `Cairo`, `Pango`. Method: stage both full assembly sets, load
`GtkSharp.dll` (or the sibling) under `MetadataLoadContext`, list `GetExportedTypes()` filtered by
namespace, diff. (The repo's `api-diff/tool` can also do member-level diffs once both assemblies'
dependency closures are staged for base-type resolution.)

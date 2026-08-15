# Gtk3 → Gtk4 visual differences

Snapshots of every sample form, taken from the **real windows** of both harnesses and compared side by
side. This is the visual counterpart to the behavioural parity suite: the traces say the two builds *do*
the same thing, these say whether they *look* the same.

| | |
|---|---|
| Reference | `SampleParity/Snapshots/gtk3/` — the sample in the `gtksystem-windows-forms` submodule (Gtk3) |
| Ported | `SampleParity/Snapshots/gtk4/` — this repo's `GtkWinFormsApp` (Gtk4) |
| How | `SampleParity.Gtk3Harness --showall --hold 240`, then each toplevel captured by handle; same for Gtk4 |

Captions differ by language (the reference renders Chinese, the port English) — that is by design and is
never a finding below. Window sizes differ slightly and are not findings either.

> **Why the windows and not the in-process capture.** The existing UI suite captures a form by rendering it
> to a Cairo surface, but that path is a Gtk3 facility: Gtk4 replaced `gtk_widget_draw` with the snapshot
> API and [Screenshots.cs](../GtkWinFormsApp.UITests/Infrastructure/Screenshots.cs) still carries a
> `TODO(gtk4)` that leaves the surface **blank**. Every committed baseline PNG is therefore empty, and that
> gate is currently comparing image *dimensions* only. Worth fixing separately — it is why none of the
> differences below were ever caught by it.

---

## Severity summary

| # | Difference | Forms | Impact |
|---|---|---|---|
| [D1](#d1) | `ListView` items render without their text | Form2 | **High** — list looks empty |
| ~~[D2](#d2)~~ | ~~`BackgroundImage` not painted~~ — **FIXED**: the custom-paint path was disabled on every widget; restored via `OnSnapshot` | Form2, Form4 | **High** |
| [D3](#d3) | Test1's button icons missing — **theme CSS**, not `Button.Image` (diagnosis corrected) | Test1 | **High** |
| [D4](#d4) | Custom `Paint` geometry rotated ~180° | Test1 | **High** — charts point the wrong way |
| [D5](#d5) | Custom `Paint` fill level wrong | Test1 | **High** — a 50% gauge renders ~5% |
| ~~[D6](#d6)~~ | ~~`PasswordChar` ignored~~ — **FIXED**: masking hung off `GtkWidget::show`, a signal Gtk4 removed | Form1 | **High** |
| [D7](#d7) | `\r\n` in a grid header rendered literally | Form1 | Medium |
| [D8](#d8) | `FlowLayoutPanel` wraps too early | Form1 | Medium |
| [D9](#d9) | List/grid rows over-tall; content clipped | Form1, Form2, Test1 | Medium |
| [D10](#d10) | `GroupBox` title not notched into its border | Form1, Form4 | Low |
| [D11](#d11) | `Label.ForeColor` / `BackColor` not applied | Form2, Form4, Test1 | Low |
| [D12](#d12) | Scrollbars heavier and appear where Gtk3 has none | all | Low |

---

## D1 — `ListView` items render without their text {#d1}

**Form2.** The reference shows seven rows, each with a check box, a small image and a caption
(`将数据绑定到 ListView`, `什么是 ListView?`, …) plus values in the `组2`/`组3`/`4`/`5` sub-columns. The port
shows **two** rows, each with a check box and an image but **no caption at all**, and only one sub-column
value survives.

Two distinct faults are stacked here: fewer items are materialised, and those that are show no text.

**Where to look:** `ListView`'s item/sub-item rendering — whether the text cell is being added to the row
at all, and why item creation stops at two.

## D2 — `BackgroundImage` is not painted {#d2}

- **Form2**: the reference paints a large photograph across the middle panel with explanatory red text over
  it. The port shows plain background — no image, and the overlaid text is gone with it.
- **Form4**: the reference's *Test Button 1* carries a blue gradient bitmap; the port's is empty.
- **Test1**: the workflow steps are chevron-shaped because each is a `RadioButton` with `Appearance =
  Button` and a chevron bitmap. The port draws plain radio buttons with a circle and a label — see also
  [D3](#d3).

**Where to look:** `Control.BackgroundImage` / `BackgroundImageLayout` — it appears not to reach the Gtk4
draw path for any control type.

## D3 — Test1's button icons are missing (theme CSS, not `Button.Image`) {#d3}

**Test1.** Every button down the right edge (*Stop*, *Log out*, *Stop sampling*, *Emergency mode*, *Alarm*,
*Print*, *Start*) has an icon above its caption in the reference — a red stop sign, a clock, a warning
triangle, a printer, a green check. The port renders **text only**, and the captions are truncated
(*"Stop samp…"*, *"Emergenc…"*) because without the icon the layout budget differs.

> **Corrected diagnosis.** This first looked like `Button.Image` not reaching the draw path. It is not:
> `Test1.Designer.cs` sets **no `Image` and no `BackgroundImage` at all** — grep both, there are zero
> assignments. The icons come from the sample's own CSS theme, `theme/Test1.css` with the PNGs under
> `theme/Test1/`, which the csproj copies next to the binary.
>
> The theme-loading code in `Application.Init` is line-for-line the same on both engines, so the theme *is*
> being read. What differs is Gtk4's CSS support: it dropped and changed several of the properties a Gtk3
> stylesheet relies on to attach an image to a widget. So this is a **CSS-compatibility** item — translate
> or replace the unsupported declarations — not a missing code path.
>
> Note this makes D3 independent of [D2](#d2): restoring the paint path fixed `BackgroundImage` (verified on
> Form4) without changing Test1 at all, which is what pointed at the theme in the first place.

## D4 — Custom `Paint` geometry is rotated by about 180° {#d4}

**Test1.** The radar/pie chart draws its coloured wedges near **12 o'clock** in the reference and near
**6 o'clock** in the port. The ring, spokes and the numbers 1–18 are otherwise identical and in the same
places, so this is not a layout difference: it is the *filled arc* that lands opposite.

This points at the sweep/start-angle convention in the arc-filling primitive, or at a flipped Y axis in the
device transform.

**Where to look:** `Graphics.FillPie` / `DrawArc` in `Gtk.Drawing` — start angle sign and direction.

## D5 — A custom-painted fill level is wrong {#d5}

**Test1.** Each gauge is labelled `50%`. In the reference *PC1* is filled to roughly half its height; in
the port *PC1* shows only a thin sliver at the bottom. The other gauges (*CC1*, *PC2*, *CC2* …) fill
plausibly, so this is not a uniform scale error but something about how one rectangle's height is derived.

Worth diagnosing together with [D4](#d4): both are custom `Paint` handlers drawing through the
`System.Drawing` shim.

## D6 — `PasswordChar` is ignored {#d6}

**Form1.** The second text box in the group box shows `*******` in the reference and the literal text
`sdfdf43` in the port. A password box that renders in clear text is worth treating as more than cosmetic.

**Where to look:** `TextBox.PasswordChar` / `UseSystemPasswordChar` → `Gtk.Entry.Visibility` /
`InvisibleChar`.

## D7 — `\r\n` in a grid header is rendered literally {#d7}

**Form1.** The date column header is two lines (`日期` / `时间`) in the reference. The port prints
`Date\r\nTime` — the escape sequence itself, on one line.

Two things to check: that the header text is being *unescaped* when read from resources, and that the cell
renderer honours embedded newlines at all.

## D8 — `FlowLayoutPanel` wraps too early {#d8}

**Form1.** The check-box grid inside the group box flows **four per row across twelve items** in the
reference, with no scrollbar. The port fits **two per row**, shows six, and adds a horizontal scrollbar;
the remaining items are unreachable.

This is very likely the same root cause as the one open entry in the parity backlog
(`form1.checkboxes.toggle`: the reference reaches three check boxes, the port only two — the missing one
being the only child of this `FlowLayoutPanel`). Fixing the flow width should close both.

## D9 — Rows are over-tall and content is clipped {#d9}

- **Form1**: the `DateTimePicker` at the bottom of the group box is cut off mid-control.
- **Form2**: the yellow `ListBox` shows six lines in the reference and three, widely spaced, in the port.
- **Test1**: the grid's `Test` column is truncated and gains a horizontal scrollbar; the reference fits.

The recent `ToolStripTextBox` fix was exactly this shape — Adwaita's minimum control metrics being larger
than the designed size, and a size request being unable to shrink below them. The same treatment (a style
class collapsing `min-height`, loaded at User priority) is the likely remedy per control type.

## D10 — `GroupBox` title is not notched into its border {#d10}

**Form1, Form4.** The reference interrupts the top border and sets the caption into the gap — the classic
WinForms group box. The port draws a complete rectangle with the caption sitting inside it on a header
strip. Purely cosmetic, but it is the most recognisable "this isn't WinForms" tell.

## D11 — `Label` colours are not applied {#d11}

- **Form4**: the *Banner Bar* caption is large and light grey in the reference, plain black in the port —
  `ForeColor` lost.
- **Test1**: *XXX item analysis in progress…* has a transparent background in the reference and an opaque
  grey box in the port — `BackColor`/transparency lost.
- **Form2**: the red explanatory text over the photograph is missing entirely, though that may simply
  follow from [D2](#d2).

## D12 — Scrollbars are heavier and appear where the reference has none {#d12}

**All forms.** The port shows thick grey scrollbars — on the Form1 tree, the Form2 list box, the Test1
grid — where the reference either shows a slim one or none. Partly a consequence of [D8](#d8)/[D9](#d9)
(content genuinely overflowing), partly Adwaita's scrollbar metrics.

---

## Suggested order

1. ~~**[D6](#d6)**~~ — **done.** Masking was applied from a handler wired to `GtkWidget::show`, which Gtk4
   removed, so neither `PasswordChar` nor `Mask` ever ran; both now apply on assignment.
2. ~~**[D2](#d2)**~~ — **done, and it was bigger than one property.** WinForms custom painting had been
   disabled on *every* widget on the premise that Gtk4 offers no overridable `OnSnapshot`. It does; the
   drawing code was intact and only needed calling. `BackgroundImage` and every `Paint` handler were dead
   until this.
3. **[D3](#d3)** — now known to be CSS compatibility, independent of D2. Needs the Gtk3 stylesheet's
   image declarations translated to what Gtk4 accepts.
4. **[D1](#d1)** — a list that renders no text reads as broken.
4. **[D4](#d4)** / **[D5](#d5)** — both in the `System.Drawing` shim's arc/rect painting; diagnose together.
5. **[D8](#d8)** / **[D9](#d9)** — shared "Gtk4 minimum metrics vs designed size" family, with a known
   remedy already applied once to `ToolStripTextBox`.
6. **[D7](#d7)**, then the cosmetic **[D10](#d10)**–**[D12](#d12)**.

Each fix should land with a test, and re-running the two harnesses with `--showall` regenerates the
snapshots for a before/after check.

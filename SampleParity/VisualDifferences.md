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
| ~~[D1](#d1)~~ | ~~`ListView` items render without their text~~ — **FIXED**, two causes: the scroll area lost Gtk3's expand flag, *and* `CheckButton.Child` destroyed the caption | Form2 | **High** |
| ~~[D2](#d2)~~ | ~~`BackgroundImage` not painted~~ — **FIXED**, two causes: the custom-paint path was disabled on every widget, *and* the CSS `url()` was relative | Form2, Form4 | **High** |
| [D3](#d3) | Test1's button icons missing — **theme CSS**, not `Button.Image` (diagnosis corrected) | Test1 | **High** |
| ~~[D4](#d4)~~ | ~~Custom `Paint` geometry rotated ~180°, fill level wrong~~ (was D4 **and** D5) — **FIXED**: one defect, and neither was in the arc code — a custom `Paint` ran exactly once, freezing every animated control on its first frame | Test1 | **High** |
| ~~[D6](#d6)~~ | ~~`PasswordChar` ignored~~ — **FIXED**: masking hung off `GtkWidget::show`, a signal Gtk4 removed | Form1 | **High** |
| [D7](#d7) | `\r\n` in a grid header rendered literally | Form1 | Medium |
| ~~[D8](#d8)~~ | ~~`FlowLayoutPanel` wraps too early~~ — **FIXED**, and it was neither a `FlowLayoutPanel` nor one defect: `CheckedListBox.ColumnWidth` never clamped because a Gtk4 button keeps its label outside the child slot, *and*, separately, `Controls` stored widget wrappers instead of controls | Form1 | Medium |
| [D9](#d9) | List/grid rows over-tall; content clipped | Form1, Form2, Test1 | Medium |
| ~~[D10](#d10)~~ | ~~`GroupBox` title not notched into its border~~ — **FIXED**: Gtk4 deleted `gtk_render_frame_gap`, so the shim draws the notched border itself | Form1, Form4 | Low |
| ~~[D11](#d11)~~ | ~~`Label.ForeColor` / `BackColor` not applied~~ — **FIXED**, and no colour was ever lost: the grey box is a **text selection**, and the grey caption is the reference's backdrop state | Form2, Form4, Test1 | Low |
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

> **Fixed — and it took two independent changes**, neither of which was an item-creation failure:
>
> 1. **The rows were all there, just scrolled out of sight.** Gtk3 packed the scroll area with
>    `PackStart(scrolledWindow, expand: true, fill: true, 0)`; Gtk4's `Box.Append` has no expand argument
>    (expansion is a child property) and the port dropped it. The vertical box therefore granted the
>    scroller only its natural height — **46px against 154px of rows** — so all but the first two were
>    below the viewport. `Valign = Fill` cannot substitute: it only distributes space already granted.
>    The scroll area now sets `Vexpand` itself.
> 2. **The captions were destroyed by `Gtk.CheckButton.Child`.** Gtk3's check button derives from
>    `GtkButton`, which carries an image *and* a label, so upstream set `AlwaysShowImage` /
>    `ImagePosition` and assigned `.Image`. Gtk4 removed all three, and a check button has exactly one
>    content slot — its caption *is* its `Child` — so the ported `.Child = new Gtk.Image(...)` deleted the
>    label and every item with an image list rendered blank. `CheckBoxBase.SetImageAndLabel` now builds one
>    `[icon][caption]` box, the same remedy the Gtk4 menu items needed. The item's `ForeColor` moves with
>    the caption instead of being applied to a label that is about to be destroyed.
>
> **Not a defect, deliberately left alone:** the third clause, "only one sub-column value survives". The
> port de-duplicates sub-items where upstream adds `items[0]` a second time, so the Gtk3 reference renders
> the caption twice and shifts every sub-value one column right of where real WinForms puts it. Chasing
> pixel parity here would re-introduce an upstream bug. The apparent missing sub-values were fault 1.

## D2 — `BackgroundImage` is not painted {#d2}

- **Form2**: the reference paints a large photograph across the middle panel with explanatory red text over
  it. The port shows plain background — no image, and the overlaid text is gone with it.
- **Form4**: the reference's *Test Button 1* carries a blue gradient bitmap; the port's is empty.
- **Test1**: the workflow steps are chevron-shaped because each is a `RadioButton` with `Appearance =
  Button` and a chevron bitmap. The port draws plain radio buttons with a circle and a label — see also
  [D3](#d3).

> **Fixed — and it took two independent changes**, which is why the first one alone showed no effect on
> Form2:
>
> 1. **The custom-paint path was disabled on every widget** (see the commit restoring `OnSnapshot`). That
>    covered controls painted through the override — Form4's *Test Button 1* came back at this point.
> 2. **`BackgroundImage` is actually rendered through CSS, not that path.** `UpdateStyle` writes the image
>    to a PNG under `Resources/` and emits `background-image:url(...)` — with a **relative** path. Gtk3
>    resolved that against the process working directory; Gtk4 loads the stylesheet from a *string*, which
>    has no base URI, so the relative `url()` resolved to nothing and every form and panel background
>    silently vanished. It now emits an absolute `file:///` URI.
>
> Verified: Form2's photograph and Form4's gradient button both render, matching the reference.

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

## D4 — Every animated custom-painted control is frozen on its first frame {#d4}

*Was filed as two entries — D4 "geometry rotated ~180°" and D5 "fill level wrong". They are **one** defect,
and neither of them is in the drawing code.* <a id="d5"></a>

**Test1.** The radar/pie chart draws its coloured wedges near **12 o'clock** in the reference and near
**6 o'clock** in the port; the ring, spokes and the numbers 1–18 land identically. Each gauge is labelled
`50%`, yet the port's *PC1* shows only a sliver where the reference fills half the bar.

> **Corrected diagnosis — no arc-angle bug and no Y-flip; the shim's `FillPie` is right.** All three of the
> affected visuals are driven by the same `ticks` counter, which a 20 ms `Timer` advances and whose handler
> calls `Refresh()` on the two picture boxes and the chart. Solving each snapshot for `ticks` from three
> independently measured quantities gives one consistent value per engine:
>
> | | gtk3 | gtk4 |
> |---|---|---|
> | PC1 fill offset | 72/168 → \|sin(t+1.2)\|=0.4286 | 156/168 → 0.9286 |
> | CC1 fill offset | 88/168 → \|sin(t+2.2)\|=0.5238 | 135/168 → 0.8036 |
> | consistent solution | t = 1.4957 | **t = −0.003, i.e. `ticks == 0`** |
> | predicted wedge mid-angle | 254° | 79° |
> | **measured wedge centroid** | **253.4°** | **80.2°** |
>
> Both engines therefore draw exactly what WinForms semantics prescribe for the value of `ticks` they hold.
> The port simply still holds the value from *before the first timer tick*. A sign flip or a flipped Y axis
> would show up as a 2× or 180° error in at least one of those numbers; it does not. The five *static*
> gauges (`Height/2`) are pixel-equivalent across engines, which no `FillRectangle` or transform fault
> could leave intact.
>
> **The real cause: an invalidation never reaches the widget that paints.** A picture box, a panel, a group
> box and a form each host a child `Gtk.DrawingArea` inside a `Gtk.Overlay` and raise the WinForms paint
> events from *that child's* draw callback — but `Refresh`, `Invalidate` and `Update` only queued a draw on
> the outer control widget. Gtk3 redrew the child anyway, because the invalidation went to the shared
> `GdkWindow`; upstream even passed the `invalidateChildren` flag straight to `Gdk.Window.InvalidateRect`.
> Gtk4 removed `GdkWindow`, caches a render node per widget, and `gtk_widget_queue_draw` marks only the
> widget it is called on **and its ancestors** — never its children. The drawing area kept its cached node,
> its callback never ran a second time, and every `Paint` handler in the sample ran exactly once. The Gtk4
> rewrite had also silently dropped the `invalidateChildren` argument when it reduced `Invalidate` to a
> bare `QueueDraw()`.
>
> Confirmed on the live window before the fix: two captures 20 s apart differ by 58 px in the clock label —
> so the timer *is* firing and `ticks` *is* advancing — while the gauges and the whole chart rectangle are
> pixel-identical. After the fix the same comparison shows the fills tracking `|sin|` and thousands of
> differing pixels in the chart.
>
> **Fixed** by recording the widget that raises `Paint` on the drawing override and invalidating it from
> `Refresh` / `Invalidate` / `Update`, with `Refresh` and `Invalidate(rc, invalidateChildren: true)`
> recursing over the WinForms `Controls` collection the way Gtk3's window invalidation did.
>
> **Consequence for this baseline:** Test1's chart wedge and its PC1/CC1 gauges now land on an arbitrary
> animation phase at capture time, so those three regions can no longer be compared pixel-wise between the
> two snapshots — they need masking or excluding.

## D6 — `PasswordChar` is ignored {#d6}

**Form1.** The second text box in the group box shows `*******` in the reference and the literal text
`sdfdf43` in the port. A password box that renders in clear text is worth treating as more than cosmetic.

**Where to look:** `TextBox.PasswordChar` / `UseSystemPasswordChar` → `Gtk.Entry.Visibility` /
`InvisibleChar`.

## D7 — `\r\n` in a grid header is rendered literally {#d7}

**Form1.** The date column header is two lines (`日期` / `时间`) in the reference. The port prints
`Date\r\nTime` — the escape sequence itself, on one line.

> **Root cause found, and it is not in the library.** The reference sample has the C# literal
> `CreateDate.HeaderText = "日期
时间";` — where `
` is a real escape the compiler turns into CR LF.
> The sample transform lifts that string into a resx, and
> [SampleGlobalizationEnricher.cs:331-333](../Prepare.Common/Languages/SampleGlobalizationEnricher.cs#L331-L333)
> captures the literal's **raw source text** rather than its decoded value. So the resx ends up holding two
> literal characters, backslash and `r`, and the runtime dutifully prints them:
>
> ```xml
> <data name="Gtk_Date_r_nTime" xml:space="preserve">
>   <value>Date
Time</value>
> </data>
> ```
>
> The key name `Gtk_Date_r_nTime` is itself derived from the escaped text, which is a second tell.
>
> **Fix in two parts.** The durable one is in the pipeline: decode C# escape sequences when a literal
> becomes a resource value, with a `Prepare.Tests` case. The already-generated `.resx` files also need
> correcting (both the invariant and `zh-Hans` entries) for the sample to render correctly before the next
> regeneration — a hand edit alone would be lost, so the pipeline fix has to come first.
>
> Worth checking separately whether the cell renderer honours an embedded newline at all, since that has
> never been exercised.

## ~~D8~~ — `CheckedListBox.ColumnWidth` was never applied {#d8}

**Form1.** The check-box grid inside the group box flows **four per row across twelve items** in the
reference, every caption ellipsized to a uniform column (`内蒙…`, `黑龙…`). The port fit **two per row**,
showed six, and added a horizontal scrollbar; the remaining items were unreachable.

**The heading was wrong on two counts.** The control is not a `FlowLayoutPanel`: it is `checkedListBox1`,
a `CheckedListBox` with `MultiColumn = true` and `ColumnWidth = 70`. (`flowLayoutPanel1` lives on
tabPage2 and is not on screen in either snapshot.) And the entry merged two unrelated defects.

**Defect 1 — the column clamp was skipped.** `CheckedListBox` enforces `ColumnWidth` from the item's
`realize` handler by requesting the width *and* ellipsizing the caption — the width request alone only
raises a minimum, so without the ellipsize the label's minimum stays its natural width and `ColumnWidth`
means nothing. The handler reached the caption with Gtk3's idiom, `checkButton.Child is Gtk.Label`. In
Gtk4 that is **always false** for a labelled button: `gtk_check_button_set_label()` keeps the label it
creates outside the child slot, which only `gtk_check_button_set_child()` fills. The label is a real
child — reachable through `FirstChild`/`NextSibling` — just not through `Child`. Measured before the fix,
the items' minimum widths were 220/142/70/226px instead of 70; after it, exactly 70 each.

Note that `realize` *does* fire in Gtk4, so this is not the "signal Gtk4 removed" family of [D6](#d6); it
is the child-slot semantics change. The same `Child is Gtk.Label` idiom was silently dropping `ListView`
item `ForeColor` and alignment, and is fixed with it.

**Defect 2 — `FlowLayoutPanel` children were invisible to `Controls`.** This is what the parity backlog's
`form1.checkboxes.toggle` entry was really about. `Control.ControlCollection.AddWidget` stored an
`ArrangedElementWidget` wrapper rather than the `Control` it was handed, so `flp.Controls.Count` was
right but every entry failed `is Control` and the indexer threw `InvalidCastException` — the same shape as
the already-fixed `TableLayoutPanel` divergence. The collection now records the control whenever there is
one, falling back to the wrapper only for widget-only children.

**Fixed.** The port now renders the same four ellipsized columns as the reference and the parity backlog
is empty. A thinner horizontal scrollbar remains: with the clamp applied the flow box still measures
350px of content against a 267px viewport, because Gtk3's `ScrolledWindow` allocated the flow box the
viewport width and let it clip (which is why the reference's fourth column is cut mid-glyph) while Gtk4
allocates the full content width and scrolls. That remainder belongs with [D9](#d9)/[D12](#d12).

## D9 — Rows are over-tall and content is clipped {#d9}

- **Form1**: the `DateTimePicker` at the bottom of the group box is cut off mid-control.
- **Form2**: the yellow `ListBox` shows six lines in the reference and three, widely spaced, in the port.
- **Test1**: the grid's `Test` column is truncated and gains a horizontal scrollbar; the reference fits.

The recent `ToolStripTextBox` fix was exactly this shape — Adwaita's minimum control metrics being larger
than the designed size, and a size request being unable to shrink below them. The same treatment (a style
class collapsing `min-height`, loaded at User priority) is the likely remedy per control type.

## ~~D10~~ — `GroupBox` title is not notched into its border {#d10}

**Form1, Form4.** The reference interrupts the top border and sets the caption into the gap — the classic
WinForms group box. The port draws a complete rectangle with the caption sitting inside it on a header
strip. Purely cosmetic, but it is the most recognisable "this isn't WinForms" tell.

> **Fixed — and nothing had been lost from the library.** The two `GroupBoxBase` implementations are
> functionally identical: both only set `Frame.Label` and `LabelXalign = 0.03f`. The notch was never the
> shim's doing. Gtk3's `gtk_frame_draw()` painted the border with
> `gtk_render_frame_gap(…, GTK_POS_TOP, labelX, labelX + labelWidth)`, which leaves the caption's extent out
> of the top edge. **Gtk4 deleted that primitive** — this repo's own binding says so, at
> `GtkSharp/Source/Libs/GtkSharp/StyleContext.cs:45`: *"gtk_render_extension, gtk_render_frame_gap and
> gtk_render_slider are gone in Gtk 4"* — and a `GtkFrame` now allocates its caption *inside* the content
> box above the child, letting ordinary CSS box rendering draw an uninterrupted (Adwaita: rounded)
> rectangle. There was nothing to re-enable and no CSS-only way back, so the shim now draws the border
> itself, from the same snapshot vfunc the custom-paint path uses.
>
> The geometry comes from the caption widget, not from constants: the top edge runs through the middle of
> the caption whatever the font, the alignment or the theme's padding say, and with no caption allocated
> yet — the very first frame — it degenerates to a plain closed rectangle, which is also what a captionless
> WinForms group box draws. The colour is the theme's own `borders`.
>
> **The two halves only work as a pair**, the same pairing [D2](#d2) needed: without a CSS rule taking the
> theme's own rectangle away the notch is invisible behind it. **And making that border transparent is not
> enough** — this is the part that only the live window revealed. Gtk4 gives a widget's own drawing a
> context whose origin is its CSS *content box* and clips it there. With Adwaita's 1px frame border in
> place, the shim's border came out a pixel too far in and its **right and bottom edges vanished
> altogether**: measured on screen, the top and left edges stopped exactly at the content box while the two
> segments that would have crossed it were dropped entirely. The rule therefore takes the border *and* the
> padding to zero, making the content box the widget box — the only geometry in which a border drawn at the
> widget's own edges survives. The frame's outer size becomes its child's requested size rather than that
> plus two pixels, and its children sit a pixel further out. Nothing touches the background, which is how
> `GroupBox.BackColor` reaches the widget.
>
> Confirmed on the live window: `Form4`'s two group boxes now draw all four edges with the caption set into
> the top one, and `SampleParity/Snapshots/gtk4/Form4.png` is re-captured from that run.

## ~~D11~~ — `Label` colours are not applied {#d11}

*The heading was wrong: no `Label` colour is lost on either count, and only one of the three bullets was a
defect at all.*

- **Form4 — not a defect, the premise is false.** `Form4.Designer.cs` sets `Dock`, `Font`, `Location`,
  `Size` and `Text` on the *Banner Bar* caption and **no `ForeColor`**; the vendored Gtk3 sample sets none
  either, and across both sample trees no `Label` assigns `ForeColor` or `BackColor` at all. Measured
  pixels: the Gtk3 caption's darkest glyph is `rgb(146,149,149)` — Adwaita-3's `mix(fg,bg,50%)`, its
  backdrop grey — and *every other caption in that screenshot measures the same value*, because the window
  was captured unfocused and Gtk3's Adwaita dims all text in `:backdrop`. The Gtk4 caption is
  `rgb(46,52,54)` = `#2e3436`, Adwaita-4's normal foreground; Gtk4 dropped backdrop text dimming. The
  `ForeColor` code path is byte-for-byte the same on both engines, and `BackColor` demonstrably works on
  Gtk4 — Form3's `label1` renders its designed green in both snapshots.
- **Test1 — a real defect, but a text selection, not `BackColor`.** *XXX item analysis in progress…* is
  painted `rgb(184,183,186)`, which is the foreground at 25% over the panel background: Adwaita-4's
  *unfocused selection*. It is one text line tall against a taller label allocation and only as wide as the
  glyph run — not the label's box. Decisively, **the Gtk3 reference has the identical artefact**, on the
  *PC1* caption, in Adwaita's focused blue `#3584e4` with white glyphs. Both builds set `Selectable` on the
  label; under Gtk3 that was only a mouse affordance, but Gtk4 makes a selectable label *focusable*, the
  window's initial focus lands on one, and `gtk_label_grab_focus()` honours `gtk-label-select-on-focus` by
  selecting the whole caption. A WinForms `Label` has `ControlStyles.Selectable` clear and `TabStop` false:
  it never takes focus and its text is never selectable. **Fixed** by matching that contract — a deliberate
  divergence from the vendored Gtk3 build, at the cost of drag-selecting a caption with the mouse. Confirmed
  on the live window: the selection colour no longer occurs anywhere in `Test1`, whose snapshot is
  re-captured from that run.
- **Form2 — not this entry.** The red explanatory text is not missing for want of colour; it followed from
  [D2](#d2), and what remains of the difference is the resx transform of [D7](#d7).

## D12 — Scrollbars are heavier and appear where the reference has none {#d12}

**All forms.** The port shows thick grey scrollbars — on the Form1 tree, the Form2 list box, the Test1
grid — where the reference either shows a slim one or none. Partly a consequence of [D9](#d9) and of the
`ScrolledWindow` allocation difference noted under [D8](#d8) (content genuinely overflowing), partly
Adwaita's scrollbar metrics.

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
4. ~~**[D4](#d4)** / **[D5](#d5)**~~ — **done, and they were one defect in neither of the places filed.**
   Nothing was wrong with the shim's arc or rect painting: a custom `Paint` handler ran exactly once, so
   every animated control was stuck on its first frame. An invalidation now reaches the child drawing area
   that actually raises `Paint`.
5. ~~**[D8](#d8)**~~ — **done, and it was not the control the entry named nor a single defect.** The grid
   is a `CheckedListBox`, and its `ColumnWidth` clamp was skipped because a Gtk4 button keeps its label
   outside the child slot. A second, unrelated defect hid `FlowLayoutPanel`'s children from `Controls`.
   **[D9](#d9)** remains, as the "Gtk4 minimum metrics vs designed size" family, with a known remedy
   already applied once to `ToolStripTextBox`.
6. **[D7](#d7)**, then the cosmetic **[D10](#d10)**–**[D12](#d12)**.
7. ~~**[D10](#d10)** / **[D11](#d11)**~~ — **done, and one of the two was not the defect it was filed as.**
   The group box notch is drawn by the shim, because Gtk4 deleted `gtk_render_frame_gap` and there is no
   CSS-only substitute. No `Label` colour was ever lost: the Gtk4 grey box is a *text selection* — a
   selectable label became focusable under Gtk4 and select-on-focus selected the caption — and the Gtk3
   reference has the same artefact in blue, while the Form4 caption's grey is the reference window's
   backdrop state. **[D12](#d12)** remains, with **[D9](#d9)**.

Each fix should land with a test, and re-running the two harnesses with `--showall` regenerates the
snapshots for a before/after check.

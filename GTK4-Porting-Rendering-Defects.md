# GTK3 → GTK4 porting — rendering defects (Form3 "Default Style Interface")

Re-assessment after the first round of fixes, from a **fresh snapshot of the current build**.

| Backend | Sample | Capture |
|---|---|---|
| **GTK4 (this fork — current)** | `GtkWinFormsApp` (main repo) | [`main-repo-GtkWinFormsApp-after.png`](main-repo-GtkWinFormsApp-after.png) — 864×573 |
| **GTK4 (this fork — before fixes)** | `GtkWinFormsApp` (main repo) | [`main-repo-GtkWinFormsApp.png`](main-repo-GtkWinFormsApp.png) — 864×540 |
| **GTK3 (reference)** | `gtksystem-windows-forms` submodule `Samples/GTKWinFormsApp` | [`submodule-GTKWinFormsApp.png`](submodule-GTKWinFormsApp.png) — 888×610 |

Both apps are the same code; only the shim backend differs. Captured with `PrintWindow` (not foregrounded).

> **Content is not a defect.** The reference is localized to **Chinese** (shorter strings) and the fork to
> **English** (longer strings) with a **different, longer demo URL**. Differences that are *only* a
> consequence of longer text — a wider auto-size label, a truncated hyperlink, a label reaching a
> scrollbar — are **excluded** from the defect list below and listed separately at the end.

---

## Resolved since the first assessment ✅

All six defects from the original report are fixed and hold in the current snapshot (full test suite green,
2533 passed / 12 skipped):

| # | Was | Now |
|---|---|---|
| 1 | Toolbar image buttons showed icon only | Icon **+** caption on every item |
| 2 | Drop-down / split buttons had no ▼ arrow | Arrows restored |
| 3 | Toolbar items boxed; first item "pressed" | Flat items; no stuck state |
| 4 | Bottom ProgressBar missing | Visible above the status bar |
| 5 | Vertical TrackBar trough bulky | Thin channel + thumb |
| 6 | Window too short, bottom clipped | Window grows to fit content (573 px) |

---

## Remaining rendering defects (current snapshot)

Ordered descending by severity. These are **layout/paint** differences, not content differences.

> **Update (2026-08-14):** R1 and R2 are now **fixed generically** — see the ✅ notes. Both traced to real
> library bugs, not sample quirks: a wrong `Control.Anchor` default and a hardcoded height clamp. R3/R4 were
> re-assessed as **not porting defects** (WinForms-correct paint / cosmetic slack). A separate timing race in
> the window-grow pass was also fixed so the bottom controls no longer clip intermittently.

### MEDIUM

#### R1. The "Open Main Window" button overlaps the toolbar's right-hand items
- **GTK3:** the toolbar is a single clean row — two combo boxes, the in-toolbar progress bar and the
  `测试` button all sit fully inside it, with `打开主窗口` (Open Main Window) as a separate button *below*.
- **GTK4:** the toolbar renders **taller than its designed height** (~42 px vs. the designed 33 px), so its
  bottom edge reaches the top of `button1` "Open Main Window", which is anchored just beneath it
  (`Location.Y = 96`, toolbar designed at `Y = 62..95`). The button paints over the toolbar's lower edge and
  clips the right-side items — the `Button menu` caption is cut to "Butt…", and the combos/progress bar lose
  their bottom edge.
- **Evidence:** `main-repo-GtkWinFormsApp-after.png` — the rounded button overlaps the toolbar row; the
  toolbar's right items are only partly visible above the button's top edge.
- **Likely cause:** the GTK4 toolbar (`Gtk.Box` with `.toolbar`) reports a natural height larger than the
  WinForms `ToolStrip.Height` (33). The combo/progress-bar/20 px-icon item metrics inflate the row; it needs
  its height constrained to the designed `ToolStrip.Height` so the anchored button below no longer collides.
- **✅ FIXED** — the real cause was a generic layout bug: `Control._anchor` defaulted to `AnchorStyles.None`
  instead of the WinForms default `Top|Left`, so every default-anchored control (the toolbar included) drifted
  by *half the container size-delta* whenever the window grew, sliding the toolbar down into the button. With
  the default corrected, all controls sit at their design positions and the toolbar/button no longer overlap.
  Commit *"default Control.Anchor to Top|Left as WinForms does"*.

#### R2. Empty gap between the menu strip and the toolbar
- **GTK3:** the menu strip (icons stacked over `菜单1`/`菜单2`) fills its 62 px height and the toolbar sits
  directly beneath it.
- **GTK4:** the menu's icon-over-text items sit at the **top** of the 62 px strip and don't fill it, leaving a
  large blank band between the menu and the toolbar (which starts at its designed `Y = 62`).
- **Evidence:** `main-repo-GtkWinFormsApp-after.png` — a wide empty strip separates "Menu 1 / Menu 2" from
  "Image and text menu".
- **Likely cause:** the GTK4 `MenuStrip` (custom widget replacing the removed `Gtk.MenuBar`) lays its items
  out at their natural height and top-aligns them rather than filling the `MenuStrip.Height` (62); the item
  box should expand to the strip height (or the icon-over-text item should be vertically centered/filled).
- **✅ FIXED** — root cause was a hardcoded hack in `ToolStrip.Size`: `set => base.Size = new Size(value.Width,
  30)` clamped *every* strip's height to 30px, ignoring the WinForms `Size`/`AutoSize` contract, so the
  AutoSize=false 62px menu collapsed to 30 and left the gap. Now honours the contract (explicit `Size` when
  AutoSize is off, content-driven when on); the two tests that asserted the magic number were rewritten to
  assert the real contract. The menu now fills 62px and the toolbar sits directly beneath it. Commit
  *"honour ToolStrip/MenuStrip explicit Size instead of clamping to 30px"*. (A minor residual remains: the
  icon-over-text menu items are top-aligned within the 62px strip rather than filling it — cosmetic.)

#### R5. The "Slider Value" label is clipped and overlaps the progress bar
- **GTK3:** the `滑块数值` (Slider Value) label sits clearly above the bottom progress bar with a gap between
  them, and both are fully visible.
- **GTK4:** the "Slider Value" label is pushed down onto the progress bar — its lower half is clipped where
  the two overlap. (New snapshot `snap-now.png`, bottom-right.)
- **Root cause:** `label1` is `Bottom|Right`-anchored and designed *above* `progressBar1` (which is
  `Top|Left`). The content-fit pass raises the ScrolledView's height **after** the first layout, so the
  re-layout sees a size delta and moves bottom-anchored controls *down* — `label1` descends into the
  progress bar below it. The grow is legitimate (it compensates for the GTK4 ScrolledView being squeezed by
  the header/status bar), but it must not re-anchor controls that are already placed; the anchor baseline
  should be re-seeded so the grow keeps every control at its designed position.
- **✅ FIXED** — the content-fit pass now forgets the anchor baseline (`ResetLayoutBaseline`) whenever it
  grows the ScrolledView, so the re-layout applies a zero delta and every control stays at its designed
  position. "Slider Value" now sits clearly above the progress bar, both fully visible. Commit
  *"content-fit grow must not re-anchor already-placed controls"*.

### LOW

#### R3. Vertical TrackBar sits on a grey box
- **GTK3:** the slider sits directly on the white form background.
- **GTK4:** a light-grey rectangle (the `Gtk.Viewport`/control background) is painted behind the slider.
- **Likely cause:** the TrackBar's `Viewport` paints the control `BackColor` (Control grey) instead of the
  transparent/inherited background the Gtk3 path used; the per-control style (User-priority) reasserts the
  grey and overrides a plain `background:none`. Would need the TrackBar background left unpainted when no
  explicit `BackColor` is set.
- **⏸ NOT PURSUED — likely WinForms-correct.** A WinForms `TrackBar.BackColor` defaults to `SystemColors.Control`
  (light grey), and WinForms *does* paint it — so the grey box is arguably the correct render and Gtk3's
  transparent trackbar was the deviation. Matching Gtk3 here would make the shim *less* WinForms-accurate, so
  it is left as-is per the "fix WinForms behaviour, not the sample" principle.

#### R4. Window ~37 px shorter than the Gtk3 reference
- **GTK3:** 610 px tall. **GTK4:** 573 px. All content now fits (fixed in #6), but with less bottom margin
  than Gtk3, so the layout is slightly more cramped vertically.
- **Likely cause:** the grow-to-fit pass sizes the window to the content's exact extent + chrome; Gtk3's
  window carried a little extra slack. Cosmetic.
- **⏸ NOT A DEFECT — cosmetic slack.** All content is visible and the window is deterministically sized to fit
  it (the earlier intermittent clipping was a timing race in the grow pass, now fixed). The ~35px less bottom
  margin than Gtk3 is not a rendering error.

---

## Excluded — content-driven or backend-inherent (not porting defects)

These change only because of the English translation / longer URL, or are intrinsic to the GTK4/Cairo stack,
so they are **not** counted as porting defects (per the content caveat above):

- **"…eholder" over the panel scrollbar** — `label2`'s longer English string ("This is a UserControl
  placeholder") reaches `panel5`'s AutoScroll scrollbar; the Chinese string is shorter and doesn't. Z-order
  and position are correct.
- **Hyperlink truncated at the right edge** — the fork's URL
  (`https://bitbucket.org/piero-viano/net4x.gtk.windows.forms`) is far longer than the reference's
  `https://www.gtkapp.com`; an AutoSize right-anchored label clips the same way in real WinForms.
- **Custom `Graphics` demo glyph/stroke** — sub-pixel font-metric and Cairo rounding differences between the
  GTK3 and GTK4 stacks; the custom-paint path itself is correct.

---

## Summary

| # | Defect | Severity | Status |
|---|---|---|---|
| R1 | "Open Main Window" button overlaps the toolbar's right items | Medium | ✅ Fixed (Anchor default Top\|Left) |
| R2 | Empty gap between the menu strip and the toolbar | Medium | ✅ Fixed (honour Size; drop force-30) |
| R5 | "Slider Value" label clipped / overlaps the progress bar | Medium | ✅ Fixed (re-seed anchor baseline on grow) |
| R3 | Vertical TrackBar sits on a grey box | Low | ⏸ Not a defect (WinForms `BackColor`) |
| R4 | Window ~35 px shorter than Gtk3 | Low | ⏸ Not a defect (cosmetic slack) |

Both medium defects were real, **generic** library bugs — not sample quirks: `Control.Anchor` defaulted to
`None` instead of the WinForms `Top|Left` (so default-anchored controls drifted on resize), and
`ToolStrip.Size` hardcoded every strip to 30px tall (so an AutoSize=false menu couldn't keep its height).
Both are fixed with the full suite green (2533 passed / 12 skipped), and a timing race that intermittently
clipped the bottom controls was fixed too. R3/R4 were re-assessed as WinForms-correct / cosmetic, not
porting defects.

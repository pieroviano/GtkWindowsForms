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

### LOW

#### R3. Vertical TrackBar sits on a grey box
- **GTK3:** the slider sits directly on the white form background.
- **GTK4:** a light-grey rectangle (the `Gtk.Viewport`/control background) is painted behind the slider.
- **Likely cause:** the TrackBar's `Viewport` paints the control `BackColor` (Control grey) instead of the
  transparent/inherited background the Gtk3 path used; the per-control style (User-priority) reasserts the
  grey and overrides a plain `background:none`. Would need the TrackBar background left unpainted when no
  explicit `BackColor` is set.

#### R4. Window ~37 px shorter than the Gtk3 reference
- **GTK3:** 610 px tall. **GTK4:** 573 px. All content now fits (fixed in #6), but with less bottom margin
  than Gtk3, so the layout is slightly more cramped vertically.
- **Likely cause:** the grow-to-fit pass sizes the window to the content's exact extent + chrome; Gtk3's
  window carried a little extra slack. Cosmetic.

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

| # | Remaining defect | Severity | Kind |
|---|---|---|---|
| R1 | "Open Main Window" button overlaps the toolbar's right items | Medium | Layout — toolbar too tall |
| R2 | Empty gap between the menu strip and the toolbar | Medium | Layout — menu under-fills its height |
| R3 | Vertical TrackBar sits on a grey box | Low | Paint — control background |
| R4 | Window ~37 px shorter than Gtk3 | Low | Layout — cosmetic slack |

Both medium items (R1, R2) are the same theme: the **menu/toolbar vertical metrics** don't match the
WinForms design heights (menu under-fills 62 px; toolbar over-fills 33 px), producing a gap above the
toolbar and an overlap below it.

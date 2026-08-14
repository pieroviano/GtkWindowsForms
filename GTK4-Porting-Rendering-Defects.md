# GTK3 → GTK4 porting — rendering defects (Form3 "Default Style Interface")

Visual comparison of the same sample form rendered by the two backends:

| Backend | Sample | Capture |
|---|---|---|
| **GTK3 (reference)** | `gtksystem-windows-forms` submodule `Samples/GTKWinFormsApp` | [`submodule-GTKWinFormsApp.png`](submodule-GTKWinFormsApp.png) — 888×610 |
| **GTK4 (this fork)** | `GtkWinFormsApp` (main repo) | [`main-repo-GtkWinFormsApp.png`](main-repo-GtkWinFormsApp.png) — 864×540 |

Both windows were captured with `PrintWindow` (not brought to the foreground). The two apps are the
same code; only the shim backend differs, so every difference below is attributable to the GTK3→GTK4
port. Defects are ordered **descending by severity**. Causes are the most likely explanation from the
visual evidence and the known GTK4 API removals; each still needs code confirmation before fixing.

> Note: a few differences are *content*, not defects — the reference is localized to Chinese and the
> fork to English (menu/toolbar captions, the demo hyperlink target). Those are excluded below.

---

## HIGH

### 1. Toolbar buttons with an image drop their text caption (render icon-only)
- **GTK3:** the toolbar reads `图文菜单` (icon **+** text), `文本菜单`, `下拉列表1`, `按钮菜单`, `测试` — every
  item shows its caption next to its icon.
- **GTK4:** items that carry an image (`图文菜单`, `下拉列表1`, `按钮菜单`, `测试`) render as an **icon only** —
  the text caption is gone. Only the text-only item (`文本菜单` / "Text menu") still shows its label.
- **Impact:** the toolbar is largely unlabeled and unreadable — the user can't tell what most buttons do.
- **Likely cause:** for `DisplayStyle = ImageAndText`, setting the backing `Gtk.Button`/`Gtk.MenuButton`
  image (`self.Image = pixbuf`) replaces the button child instead of composing image **+** label. GTK4
  removed the toolbar/tool-item that used to lay both out; the item box needs to hold an image *and* a
  label child, honouring `DisplayStyle` / `TextImageRelation`.

### 2. Drop-down and split toolbar buttons lose their ▼ arrow
- **GTK3:** `下拉列表1 ▼` (drop-down button) and `按钮菜单 ▼` (split button) show a drop-down arrow indicator.
- **GTK4:** both render as a plain icon with **no arrow** — they don't read as drop-downs at all.
- **Impact:** drop-down affordance is lost; the controls look like ordinary buttons.
- **Likely cause:** the `Gtk.MenuButton`/split-button backing isn't showing its indicator (`AlwaysShowArrow`
  / the split arrow child), or the arrow was drawn by the removed GTK3 tool-button and was never
  re-added in the Box-based port.

---

## MEDIUM

### 3. Toolbar is not a compact flat strip; the first image button is stuck "pressed"
- **GTK3:** a single compact, flat toolbar row of tightly-packed items.
- **GTK4:** the row is roughly **twice as tall**, each item is an oversized boxed button, and the first
  image button (`图文菜单`) renders with a **dark, pressed/toggled background** as if selected.
- **Impact:** the toolbar looks like a row of separate large buttons (earlier it looked like notebook
  tabs; the flat-button CSS improved it but item sizing and the stuck-pressed state remain).
- **Likely cause:** GTK3 `Gtk.Toolbar` sized and flattened its children natively; the GTK4 `Gtk.Box`
  replacement needs explicit compact sizing (item min-height/padding) and the first button's
  active/checked state cleared (a `ToolStripButton.Checked`/toggle mapping left on).

### 4. Bottom docked ProgressBar is missing from the window
- **GTK3:** a full-width horizontal progress bar (≈10 % filled) sits just above the status bar.
- **GTK4:** that progress bar is **not visible** at all.
- **Impact:** a whole docked control is absent from the view.
- **Likely cause:** clipped by the reduced window height (see #6) and/or the same Dock/anchor placement
  path that affected the status bar — worth confirming whether `progressBar1` is allocated at all.

### 5. Vertical TrackBar chrome is oversized
- **GTK3:** a thin vertical slider with a subtle track and a small thumb.
- **GTK4:** the trough renders as a **thick, wide grey bar**; the whole slider is visually bulkier.
- **Impact:** cosmetic but conspicuous — the control looks heavier and out of proportion.
- **Likely cause:** `Gtk.Scale`/trough default styling in GTK4 (trough width, `slider-width`) not
  constrained to the WinForms `TrackBar` metrics the GTK3 theme produced.

### 6. Form default height is smaller in GTK4 (≈540 vs ≈610 px)
- **GTK3:** the window opens ~610 px tall, showing all bottom content (progress bar + status bar).
- **GTK4:** the same form opens ~540 px tall, tightening/clipping the bottom band.
- **Impact:** bottom content (see #4) is squeezed or cut; overall layout differs from the designed size.
- **Likely cause:** the Form default/`ClientSize` → GTK window size mapping (the `DefaultHeight` the
  height setter never updates — already seen in the layout work) yields a shorter window than the
  designed client size.

---

## LOW

### 7. "placeholder" TextBox overlaps the split-container panel
- **GTK3:** the green/blue split-container panel and `button7` sit cleanly; no stray text.
- **GTK4:** a `placeholder` textbox is mispositioned so only `…eholder` shows, **overlapping** the green
  panel's right edge and clipped by it.
- **Impact:** minor visual overlap of two controls.
- **Likely cause:** absolute-position (`Location`) → GTK margin mapping for that control, or z-order
  between the panel overlay and the textbox.

### 8. Right-anchored LinkLabel overflows the window edge
- **GTK3:** the demo hyperlink fits within the window.
- **GTK4:** the right-anchored link runs **off the right edge** and is truncated mid-text.
- **Impact:** minor — partly content (the fork's URL is longer) but the anchored label neither wraps nor
  clips to the client area.
- **Likely cause:** anchor/auto-size interaction for a right-anchored `AutoSize` `LinkLabel` in the
  narrower window.

### 9. Minor cosmetic differences in the custom GDI-draw test
- The `System.Drawing`/`Graphics` demo area (red text, squares, circle, poly-lines) renders correctly in
  both, with only small differences in glyph placement (`testText…` vs `test□…`) and the circle's stroke.
- **Impact:** negligible; the custom-paint path itself works.
- **Likely cause:** font metrics / Cairo stroke rounding differences between the backends.

---

## Summary

| # | Defect | Severity |
|---|---|---|
| 1 | Toolbar image buttons drop their text caption | High |
| 2 | Drop-down / split toolbar buttons lose the ▼ arrow | High |
| 3 | Toolbar not compact/flat; first image button stuck pressed | Medium |
| 4 | Bottom docked ProgressBar missing | Medium |
| 5 | Vertical TrackBar chrome oversized | Medium |
| 6 | Form default height smaller (bottom content clipped) | Medium |
| 7 | "placeholder" TextBox overlaps the split-container panel | Low |
| 8 | Right-anchored LinkLabel overflows the window edge | Low |
| 9 | Minor custom GDI-draw glyph/stroke differences | Low |

**Already fixed this session** (present and correct in the GTK4 capture, listed for context): the status
bar is restored at the bottom, the `Dock.None` content no longer collapses, the "Slider Value" label sits
below the trackbar, and the toolbar buttons are flattened (no longer notebook-tab-like).

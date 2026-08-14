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

> **Resolution status (2026-08-14):** defects 1–6 (all six genuine porting defects) are **fixed** and
> verified against a live capture, with the full test suite green (2533 passed / 12 skipped). Defects 7–9
> are **assessed and deferred** — investigation showed each is content-driven (English text vs. Chinese) or
> an inherent backend-rendering difference rather than a clean porting bug; see the notes on each. Each
> defect below is annotated with its outcome.

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
- **✅ FIXED** (`StripToolButton`/`StripMenuToolButton` now compose an image+label `Box` child honouring
  `DisplayStyle`, propagated from the `SetIcon` paths). Commit *"restore toolbar button captions and dropdown arrows"*.

### 2. Drop-down and split toolbar buttons lose their ▼ arrow
- **GTK3:** `下拉列表1 ▼` (drop-down button) and `按钮菜单 ▼` (split button) show a drop-down arrow indicator.
- **GTK4:** both render as a plain icon with **no arrow** — they don't read as drop-downs at all.
- **Impact:** drop-down affordance is lost; the controls look like ordinary buttons.
- **Likely cause:** the `Gtk.MenuButton`/split-button backing isn't showing its indicator (`AlwaysShowArrow`
  / the split arrow child), or the arrow was drawn by the removed GTK3 tool-button and was never
  re-added in the Box-based port.
- **✅ FIXED** (`StripMenuToolButton` now sets `AlwaysShowArrow = true`, which a custom `Child` had been
  suppressing). Same commit as #1.

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
- **✅ FIXED** (each button now carries a direct `.ToolStripButtonFlat` class flattened via a User-priority
  `CssProvider`; the "stuck pressed" first item resolved once #1 gave it an image+label child instead of an
  image-only one). Commit *"flatten ToolStrip/MenuStrip buttons"*.

### 4. Bottom docked ProgressBar is missing from the window
- **GTK3:** a full-width horizontal progress bar (≈10 % filled) sits just above the status bar.
- **GTK4:** that progress bar is **not visible** at all.
- **Impact:** a whole docked control is absent from the view.
- **Likely cause:** clipped by the reduced window height (see #6) and/or the same Dock/anchor placement
  path that affected the status bar — worth confirming whether `progressBar1` is allocated at all.
- **✅ FIXED** together with #6 — the bar was allocated (at y≈438–467) but the ScrolledView viewport was
  only ~434 tall, so it scrolled off. Growing the window to fit content (see #6) revealed it.

### 5. Vertical TrackBar chrome is oversized
- **GTK3:** a thin vertical slider with a subtle track and a small thumb.
- **GTK4:** the trough renders as a **thick, wide grey bar**; the whole slider is visually bulkier.
- **Impact:** cosmetic but conspicuous — the control looks heavier and out of proportion.
- **Likely cause:** `Gtk.Scale`/trough default styling in GTK4 (trough width, `slider-width`) not
  constrained to the WinForms `TrackBar` metrics the GTK3 theme produced.
- **✅ FIXED** (CSS now constrains `.TrackBar scale trough` min-size and gives the slider an explicit size,
  rendering a thin channel with a distinct thumb). Commit *"slim the TrackBar trough"*.

### 6. Form default height is smaller in GTK4 (≈540 vs ≈610 px)
- **GTK3:** the window opens ~610 px tall, showing all bottom content (progress bar + status bar).
- **GTK4:** the same form opens ~540 px tall, tightening/clipping the bottom band.
- **Impact:** bottom content (see #4) is squeezed or cut; overall layout differs from the designed size.
- **Likely cause:** the Form default/`ClientSize` → GTK window size mapping (the `DefaultHeight` the
  height setter never updates — already seen in the layout work) yields a shorter window than the
  designed client size.
- **✅ FIXED** — root cause confirmed by runtime diagnostics: the controls live in an `Overlay` whose size
  follows its 0-sized background child, so the `ScrolledView` was given only the space left after the header
  and status bar (~434 px of a 511 window) and clipped the lower controls. GTK3 grew the window to fit;
  the port now measures the children's real extent after allocation and raises the ScrolledView's
  `MinContentHeight`, so the resizable window grows to contain them (opens ~573 px now). Commit
  *"grow the form window to fit its content"*.

---

## LOW

### 7. "placeholder" TextBox overlaps the split-container panel
- **GTK3:** the green/blue split-container panel and `button7` sit cleanly; no stray text.
- **GTK4:** a `placeholder` textbox is mispositioned so only `…eholder` shows, **overlapping** the green
  panel's right edge and clipped by it.
- **Impact:** minor visual overlap of two controls.
- **Likely cause:** absolute-position (`Location`) → GTK margin mapping for that control, or z-order
  between the panel overlay and the textbox.
- **⏸ DEFERRED — not a clean porting bug.** Investigation: the control is `label2` (an `AutoSize` `Label`,
  *not* a textbox) inside `panel5`, whose z-order is correct (it sits behind `panel2`) and whose design Y
  (20) doesn't even overlap `panel2` (Y 55+). The grey box behind "…eholder" is `panel5`'s **AutoScroll
  scrollbar** (`panel2` is 1294 px tall in a 104 px panel), which the *longer English* label text overlaps.
  So the effect is driven by the English string length + AutoScroll, not a GTK4 layout error; a fix would
  mean changing content or `AutoSize`/scroll behaviour (a WinForms-API change, out of scope here).

### 8. Right-anchored LinkLabel overflows the window edge
- **GTK3:** the demo hyperlink fits within the window.
- **GTK4:** the right-anchored link runs **off the right edge** and is truncated mid-text.
- **Impact:** minor — partly content (the fork's URL is longer) but the anchored label neither wraps nor
  clips to the client area.
- **Likely cause:** anchor/auto-size interaction for a right-anchored `AutoSize` `LinkLabel` in the
  narrower window.
- **⏸ DEFERRED — content-driven.** Runtime diagnostics: `linkLabel1` is `AutoSize`, sits at X≈515 and grows
  to 345 px wide (right edge ≈860 in an 836-wide window). The width comes entirely from the *fork's longer
  URL* (`https://bitbucket.org/piero-viano/net4x.gtk.windows.forms`) vs. the reference's short
  `https://www.gtkapp.com`; a real WinForms form clips an AutoSize label the same way. Growing the window
  width can't help (the label is right-anchored → it would move further right), and wrapping/ellipsizing
  would change WinForms `AutoSize` behaviour (out of scope). No GTK4 layout error to fix.

### 9. Minor cosmetic differences in the custom GDI-draw test
- The `System.Drawing`/`Graphics` demo area (red text, squares, circle, poly-lines) renders correctly in
  both, with only small differences in glyph placement (`testText…` vs `test□…`) and the circle's stroke.
- **Impact:** negligible; the custom-paint path itself works.
- **Likely cause:** font metrics / Cairo stroke rounding differences between the backends.
- **⏸ DEFERRED — inherent backend difference.** The custom `Graphics` path renders correctly; the residual
  glyph/stroke deltas are sub-pixel font-metric and Cairo rounding differences between the GTK3 and GTK4
  stacks, not a defect to "fix" (chasing pixel-parity here would risk the working paint path for no
  functional gain).

---

## Summary

| # | Defect | Severity | Status |
|---|---|---|---|
| 1 | Toolbar image buttons drop their text caption | High | ✅ Fixed |
| 2 | Drop-down / split toolbar buttons lose the ▼ arrow | High | ✅ Fixed |
| 3 | Toolbar not compact/flat; first image button stuck pressed | Medium | ✅ Fixed |
| 4 | Bottom docked ProgressBar missing | Medium | ✅ Fixed |
| 5 | Vertical TrackBar chrome oversized | Medium | ✅ Fixed |
| 6 | Form default height smaller (bottom content clipped) | Medium | ✅ Fixed |
| 7 | "placeholder" label vs. AutoScroll scrollbar (English text) | Low | ⏸ Deferred (content) |
| 8 | AutoSize LinkLabel overflows (longer fork URL) | Low | ⏸ Deferred (content) |
| 9 | Minor custom GDI-draw glyph/stroke differences | Low | ⏸ Deferred (backend) |

All six genuine GTK3→GTK4 porting defects (1–6) are fixed and verified against a live capture, full test
suite green (2533 passed / 12 skipped). Defects 7–9 were investigated and found to be content-driven
(English vs. Chinese strings) or inherent backend-rendering differences rather than porting bugs.

**Already fixed this session** (present and correct in the GTK4 capture, listed for context): the status
bar is restored at the bottom, the `Dock.None` content no longer collapses, the "Slider Value" label sits
below the trackbar, and the toolbar buttons are flattened (no longer notebook-tab-like).

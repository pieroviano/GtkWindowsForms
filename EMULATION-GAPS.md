# Emulation gaps — `Net4x.Gtk.Windows.Forms` vs `System.Windows.Forms`, `Net4x.Gtk.Drawing` vs `System.Drawing`

**Bar being measured against.** The stated intent: *an existing WinForms application should compile unchanged against these two assemblies and behave the same on Windows, Linux and macOS.* Everything below is scored against that.

**This document lists only what is still open** — non-goals, deviations, and unmeasured behaviour. Closed findings are pruned once verified: their record is in git and their guarantee is the functional-test suite that pins each one. As of the current tree there is **no `NotImplementedException` throw site** left in either project (Class B is empty) and **no absent type that is merely not-done-yet** (every remaining absent type is a declared non-goal below). What is left is deliberate deviations and behaviour that has not been measured against real WinForms.

> **See also — [API-COMPLETENESS.md](API-COMPLETENESS.md).** The per-member API-surface diff supersedes this
> document on *what exists*. This document is authoritative on *behaviour* — members that exist and compile
> but do something other than what WinForms does, which a metadata diff cannot see.

**Method and its limits.** Findings come from reading the generated sources and from the functional-test suite. Static reading reliably finds missing types, throwing members and inert auto-properties; it does **not** find layout drift, event-ordering divergence, or focus/tab-order differences — those need tests. See §4.

**Caveat on line numbers.** Both directories are *generated* by the prepare pipeline from the `gtksystem-windows-forms` submodule (see [CLAUDE.md](CLAUDE.md)). Line references drift after the next pipeline run; type and member names will not.

---

## 1. Non-goals — absent by decision, not backlog

These will not be implemented; an application that needs them is not portable to this substrate. Named so the boundary is explicit rather than discovered in production.

- **Win32 message pump.** `Message`/`WndProc` compile, but `WndProc`/`DefWndProc` throw `PlatformNotSupportedException` naming GTK as the reason (`MessagePumpNonGoalFunctionalTests`). `NativeWindow` and `SendKeys` do not exist. `IMessageFilter` with `Application.AddMessageFilter`/`RemoveMessageFilter` exists, but no internal pump invokes the filters — they run only when the application calls `Application.FilterMessage` itself, as stated on the members. Anything reaching below the event model is unportable by construction.
- **`Control.Handle` is a GTK handle, not an `HWND`.** Returns `Widget.Handle`. Correct by necessity; stated so integrators do not assume Win32 semantics.
- **`WebBrowser`, `HtmlDocument`/`HtmlElement`.** No cross-platform browser substrate. `WebBrowser`'s four `*EventArgs` types are present while the control is not — **kept deliberately**: they exist in the reference assembly, so deleting them would open four new surface holes for nothing; an application that names one still compiles. They are inert, not wrong.
- **EMF/WMF metafiles.** `Metafile`, `MetafileHeader`, `MetaHeader`, `WmfPlaceableFileHeader`, `EmfPlusRecordType`, `PlayRecordCallback` exist as types, but the record format has no cross-platform playback.

**Correctly delegated, not a gap — preserve this.** `Color`, `Point`, `PointF`, `Size`, `SizeF`, `Rectangle`, `RectangleF` are deliberately absent from `Net4x.Gtk.Drawing`; `netstandard2.0` supplies them from `System.Drawing.Primitives`, so they are the genuine BCL types. `Net4x.Gtk.Drawing`'s interop entry points (`Graphics.FromHdc`/`FromHwnd`/`GetHdc`/`GetHalftonePalette`, `Font.FromHdc`/`FromHfont`/`FromLogFont`, `Icon.FromHandle`) throw `PlatformNotSupportedException` by design — "never", not "not yet".

---

## 2. Recorded deviations — present and running, but not WinForms-identical

Each of these works; each differs from WinForms in a way a caller could observe. Documented so the difference is a known quantity, not a surprise.

### 2.1 `Control.CreateGraphics()` is deferred, not immediate

Allocates a fresh Cairo `ImageSurface`/`Context` per call and blits it during the next paint cycle (drawing to the live GDK window during realize throws). Two consequences: the effect is **deferred, not immediate** (WinForms gives an immediate-mode DC), and only a **single live surface** exists (each call disposes the previous). This breaks the classic rubber-band (`CreateGraphics()` in `MouseMove` to XOR a rectangle): it compiles, runs, and does the wrong thing.

**Accepted.** GTK's retained paint cycle has no immediate-mode DC on an unrealized widget; a faithful fix means compositing through an overlay the paint cycle owns — out of proportion to the pattern's frequency, and still no second live surface. The portable idiom is to draw in `OnPaint` and call `Invalidate()`. Reopen only if overlay compositing is undertaken for other reasons.

### 2.2 `Application.Idle` is sampled

GTK has no "message queue drained" notification, so `Idle` is sampled: while something is subscribed, a 50 ms GLib source watches the main context and raises the event once each time it finds the queue empty, re-arming when events reappear. The once-per-lull contract matches WinForms; the latency (up to 50 ms) and the sampling cost do not exist in WinForms. Nothing is installed when nobody is subscribed.

### 2.3 `Region` — exact for curves, quantised to pixels

`Region(GraphicsPath)` keeps the path, and `Graphics.SetClip`/`Clip` clip through it via Cairo's native path clip, so an ellipse or rounded rectangle clips on its true curve. Combining is exact too: a curve's interior is scan-converted the way GDI+ obtains it (fill to a coverage bitmap, covered runs become bands). Residual limits, recorded:

- **Pixel resolution.** Scan conversion quantises a curve to whole pixels (half-coverage = centre-in). GDI+ scan-converts too, but the two rasterizers will not agree on every edge pixel, so `IsVisible` on a boundary pixel can differ by one.
- **Rectangles skip it.** A path of only rectangles keeps exact floating-point edges and never reaches the rasterizer — deliberately (pinned by `CombiningRectangles_KeepsExactFloatingPointEdges`).
- **Very large regions.** Past 16 megapixels the rasterizer is skipped and the bounding rectangle used — a pathological input, but a silent widening when it happens.

### 2.4 Shaped controls and colour-keyed transparency — what the substrate allows

`Control.Region` drives GTK's shape-combine and `Form.TransparencyKey`/`AllowTransparency` work, within two substrate limits:

- **A shape needs a GDK window.** Lightweight widgets (buttons, labels — anything drawing onto its parent) cannot be shaped; on those `Region` is stored and reported but changes nothing (logged at debug level, not thrown). Forms and container controls own a window and are shaped for real.
- **`TransparencyKey` is not per-pixel.** Emulated case: a form whose `BackColor` *is* the key, so the window background is punched out and only controls remain. A key matching a colour inside a child or a background image leaves those pixels opaque, and keyed areas are not click-through — both would need an input shape recomputed from rendered pixels each paint. `Form.IsTransparencySupported` (no WinForms equivalent) reports whether the screen composites, so an opaque form on a compositor-less X11 session is diagnosable rather than mysterious.

### 2.5 Printing model and `Image.RawFormat`

- **Enum numbering differs** — `PaperKind` (`Custom = 0`, not 256), `PaperSourceKind` (0-based, `Custom = 13`) and `PrinterResolutionKind` (`High = 1`…) do not use the DEVMODE values. Code comparing `RawKind` against Win32 `DMPAPER_*`/`DMBIN_*` constants misreads them.
- **Virtual-device defaults** — with no print system, `PageSettings` defaults to Letter (850×1100), `PaperSourceKind.FormSource`, a 200×200 custom resolution, and a `PrintableArea` of the whole page (no hard margins). Real WinForms reads these from the default printer.
- **`Image.RawFormat` on fresh bitmaps reports `Bmp`, not `MemoryBmp`** — the shim keeps an encoded buffer and defaults `RawFormat` to `Bmp` when there is no source format. Stream-loaded images report their detected format correctly.

`PrintDocument.Print()` runs through the GTK print dialog machinery and stays untested (needs a live print loop).

### 2.6 Dock/anchor — a residual pixel artifact

Dock and anchor reproduce WinForms arithmetic (pinned in `LayoutArrangementFunctionalTests` against real `System.Windows.Forms` oracle values). One deviation remains: exact rectangles occasionally leave a filled child a hair larger than its GTK viewport, which can raise a scrollbar WinForms would not show — a one-to-two-pixel artifact, cosmetic, recorded rather than chased. (Separately, the resize *rule* is pinned against oracle values rather than a live resize, because the headless GTK host does not propagate a runtime resize to a modal dialog's allocation; the *wiring* is covered by the dock tests, which run through a real allocation. A display-backed run would close that coverage gap.)

### 2.7 `TableLayoutPanel`/`FlowLayoutPanel` — arranged on screen, positions not surfaced

Measured against real WinForms (oracles: a 2×2 table with `Absolute 60`/`Percent 100` columns and `Absolute 30`/`Percent 100` rows gives `c00=(3,3,54,24) c10=(63,3,134,24) c01=(3,33,54,64) c11=(63,33,134,64)`; a 100-wide `LeftToRight`+`WrapContents` flow of three 40×20 panels gives `a=(3,3) b=(49,3) c=(3,29)`). Both **diverge**, and unlike dock/anchor the divergence is not in the arithmetic but in what reaches the WinForms API. These panels lay out with the native `Gtk.Grid`/`Gtk.FlowBox`, which position children by allocation rather than the `MarginStart`/`MarginTop` that `Control.Left`/`Top` read. So a child is arranged correctly on screen but reports the origin: every table cell reports the same border-offset position with unset size, and flow children report `(0,0)` while keeping their explicit size. Code that reads a child's `Left`/`Top`/`Width` after adding it to one of these panels gets the wrong value. Pinned as the known state by `TableFlowLayoutFunctionalTests`. Closing it means reproducing the table/flow measure-arrange in managed code and pushing margins, as the dock/anchor work did (§2.6) — a dedicated effort, not a patch.

---

## 3. Open — not yet measured against WinForms

Genuinely unfinished, distinct from the deliberate deviations above.

- **Event ordering is not pinned.** The common lifecycles (show, resize, click, focus change, close) have a defined WinForms order that is invisible to the API diff and to a reading pass; it has not been asserted here.
- **The property browser beyond its contract.** Its throw sites and three value-flow defects are closed, but category grouping, sorting, expansion and value-editor selection are only lightly exercised. Treat as under-tested rather than verified.

---

## 4. Priority order

Ranked by (damage × frequency) ÷ effort.

**P1 — measurement, not code.** Behavioural divergence is the largest remaining unknown, but it is now partly charted (anchor/dock measured and fixed).

1. Reproduce `TableLayoutPanel`/`FlowLayoutPanel` measure-arrange in managed code so child positions reach the WinForms API — the divergence is now measured (§2.7); this is the fix, and it is the same shape of work as the dock/anchor arithmetic.
2. Pin event *order* for the common lifecycles (§3).
3. Exercise the property browser past its contract — grouping, sorting, expansion, editor selection (§3).
4. Re-run the layout suite on a display-backed host where a live resize propagates (§2.6).

**P2 — recorded deviations worth revisiting if they bite**

5. Per-pixel `TransparencyKey` and click-through on keyed areas (§2.4).
6. `Control.Region` on windowless widgets (§2.4) — would mean hosting them in a windowed GTK container.
7. Scan-conversion edge agreement with GDI+ (§2.3).

**P3 — accepted, reopen only with a reason**

8. `CreateGraphics` immediate-mode semantics (§2.1), EMF/WMF metafiles (§1), the Win32 message pump and what depends on it (§1), the printing enum numbering and virtual-device defaults (§2.5).

---

## 5. What this review does not cover

Stated so the gaps in the gap analysis are visible.

- **Behavioural divergence is the dominant unmeasured risk.** Layout drift, event ordering and focus/tab-order are invisible to source reading. This limit was demonstrated, not asserted: several past fixes (unraised `HandleCreated`/`HandleDestroyed`, one-member `AccessibleRole`, `Padding.Horizontal` = `Right − Left`, the three property-browser defects, the dock/anchor collapse) were found only by *writing tests*. Wrong-but-plausible code is invisible to a reading pass in a way that missing code is not.
- **The property browser beyond its contract** (§3) is under-tested.
- **`Net4x.Gtk.Windows.FormsDesigner`** and the ResX/designer round-trip are out of scope here.
- **Per-member API-surface diffing** is handled by [API-COMPLETENESS.md](API-COMPLETENESS.md) and the re-runnable tool in [api-diff/](api-diff/), strictly more complete than this document at the type/member level.

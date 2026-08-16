# Gtk.Windows.Forms
### Introduction

Native Visual Studio development, no learning curve, compile once, run cross-platform.
A cross-platform (Windows, Linux, macOS) C# desktop application framework based on GTK components.
Using this framework, projects can be developed in Visual Studio with the native C# WinForms designer, using the same properties, methods, and events as standard WinForms. Development is done entirely with familiar C# techniques — no additional learning required.

Compile once and run across platforms.
It makes cross-platform WinForms application development easy and enables existing C# WinForms applications to be upgraded to cross-platform applications with minimal effort.

Details:
[Details](https://bitbucket.org/piero-viano/net4x.gtk.windows.forms/src/main/Resources/README.md)

### Platform notes and non-goals

The library targets *compile-once, behave-the-same* for mainstream WinForms code. A few areas are Windows-
or Win32-bound by nature and are **deliberately not implemented** — an application that depends on them is
not portable to the GTK substrate. They are listed here so the boundary is known up front rather than
discovered in production:

- **Win32 message pump and native windows.** `Control.WndProc`/`DefWndProc` throw `PlatformNotSupportedException`;
  `NativeWindow`, `SendKeys` and `IMessageFilter`-driven pumping are not supported. `Control.Handle` returns
  a **GTK** widget handle, not an `HWND`.
- **COM / ActiveX.** `AxHost` and the COM interop surface are Windows-only by definition.
- **EMF/WMF metafiles.** The types exist but the Windows record format has no cross-platform playback.
- **Legacy controls.** The pre-2.0 `DataGrid` family and `ToolBar`/`ToolBarButton` are superseded by
  `DataGridView` and `ToolStrip`.
- **GDI+ `Imaging.Effects` (.NET 8+)** and the **.NET 9+ dark-mode** surface are not implemented.

**Web / HTML hosting is present, with one design consequence.** `WebBrowser`, `HtmlDocument`,
`HtmlElement`, `HtmlWindow` and the rest drive WebKitGTK 6.0 on Linux and macOS, and the Microsoft Edge
WebView2 runtime on Windows through the companion package `Gtk.Windows.Forms.WebView2`. Where neither is
installed the control still constructs and still raises its navigation events, showing a message naming
what to install. Both engines run the page **out of process**, unlike the in-process Internet Explorer
real WinForms hosted, so `HtmlDocument`/`HtmlElement` getters read a cached snapshot of the page and
`InvokeScript`/`InvokeMember` post the call and return `null`. `Gtk.Windows.Extensions` carries the
asynchronous siblings that do return a value. The full list is in `EMULATION-GAPS.md` §2.8.

A small number of members exist but deviate from Windows behaviour by necessity — for example
`Control.CreateGraphics()` returns a deferred (next-paint) surface rather than an immediate-mode DC, and
`Form.TransparencyKey` emulates only the whole-form-background case, not per-pixel keying. These and other
substrate-bound deviations are catalogued in the project's `EMULATION-GAPS.md`; the measured API surface is
tracked in `API-COMPLETENESS.md`.
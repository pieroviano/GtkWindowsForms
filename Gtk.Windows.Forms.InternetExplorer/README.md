# Gtk.Windows.Forms.InternetExplorer

Gives `System.Windows.Forms.WebBrowser` (from **Gtk.Windows.Forms**) the legacy **Internet Explorer
(Trident)** engine on Windows, by hosting the `SHDocVw.WebBrowser` ActiveX control.

Referencing the package is the whole opt-in — a module initializer registers the backend.

## Engine selection

| Backend | Priority | Chosen when |
|---------|---------:|-------------|
| WebView2 (`Gtk.Windows.Forms.WebView2`) | 100 | its runtime is installed |
| **Internet Explorer (this package)** | **75** | WebView2 is not available |
| WebKitGTK | 50 | on Linux/macOS, or Windows without the above |

To prefer Internet Explorer even when WebView2 is installed, call at startup:

```csharp
Gtk.Windows.Forms.InternetExplorer.InternetExplorerBackendRegistration.PreferOverWebView2();
```

## Requirements

- **Windows** with Internet Explorer / Trident present (`ieframe.dll`, `mshtml.dll`).
- The hosting thread must be a **single-threaded apartment** — the usual WinForms `[STAThread] Main`. The
  probe declines otherwise and the control is not used.

## Rendering mode

On first use the package writes `FEATURE_BROWSER_EMULATION` for the host executable under `HKEY_CURRENT_USER`
(value `11001`, IE11 edge mode), so pages render as IE11 rather than the control's default IE7 quirks mode.
No administrator rights are needed.

## Limitations

- **Airspace:** GTK 4 gives only toplevel windows an HWND, so the browser is a child window painted *above*
  all GTK content in that window; overlapping widgets, popovers and menus do not clip it. (Same deviation as
  the WebView2 backend.)
- **Delegated DOM events:** `HtmlDocument`/`HtmlElement` reads, mutations and `InvokeScript` work, but
  bridge-delegated element events are not delivered on this backend — Trident offers no host callback channel
  without an `IDocHostUIHandler` sink.
- **Security:** Internet Explorer is end of life. Use this only for legacy intranet content that requires
  Trident; prefer the WebView2 backend for anything new.

## Print and dialogs

`Print`, `ShowPrintDialog`, `ShowPrintPreviewDialog`, `ShowPageSetupDialog`, `ShowPropertiesDialog` and
`ShowSaveAsDialog` are routed to the control through `IWebBrowser2.ExecWB`, so they work on this backend
rather than throwing.

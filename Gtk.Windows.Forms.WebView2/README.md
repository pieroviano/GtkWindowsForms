# Gtk.Windows.Forms.WebView2

The Windows engine behind `System.Windows.Forms.WebBrowser` in
[Gtk.Windows.Forms](https://www.nuget.org/packages/Gtk.Windows.Forms).

Reference the package and nothing else: a module initializer registers the backend with
`WebViewBackendFactory`, above the WebKitGTK one, so `WebBrowser` picks it up on Windows.

```xml
<PackageReference Include="Gtk.Windows.Forms.WebView2" Version="..." />
```

Without this package a `WebBrowser` on Windows finds no engine — WebKitGTK 6.0 is a Linux and macOS
library — and falls back to a placeholder that shows a message and reports every navigation as failed.

## What you are accepting

**Airspace.** GTK 4 gives a window handle only to toplevel windows; every widget inside one is drawn, not
hosted. The browser is therefore a child window of the *form's* handle, positioned over the area the
control occupies. It paints above all GTK content in that window: overlapping widgets, popovers, menus,
drop-downs and window opacity do **not** clip it. There is no fix short of compositing the page into a
GTK texture, which costs a frame copy on every paint.

**Stale reads.** As with the WebKitGTK backend, `HtmlDocument` and `HtmlElement` read a cached snapshot of
the page rather than the live DOM, and `InvokeMember` / `InvokeScript` post the call and return `null`.
Both are recorded in the library's `EMULATION-GAPS.md`; `Gtk.Windows.Extensions` carries async siblings
that do return values.

**No incremental progress.** WebView2 reports no load percentage, so `ProgressChanged` fires with 0 when a
navigation starts and 100 when it completes, rather than counting up.

## The runtime

The package needs the Microsoft Edge WebView2 Runtime, which ships with Windows 11 and with recent Edge
installs. Check for it before you rely on it:

```csharp
if (!WebView2Runtime.IsRuntimeAvailable)
{
    // WebBrowser will fall back to the placeholder.
}
```

Nothing is downloaded unasked. If your application is willing to fetch Microsoft's Evergreen bootstrapper
on its users' machines, say so explicitly before the first `WebBrowser` is created:

```csharp
WebView2Runtime.AllowRuntimeBootstrap = true;
```

That is an application's decision, not a library's, which is why the switch lives here rather than on
`System.Windows.Forms.WebBrowser` — that type mirrors real WinForms, and additions belong in companion
packages.

## Licence

LGPL v3, the licence the vendored upstream carries. See `LICENSE.txt` in the package.

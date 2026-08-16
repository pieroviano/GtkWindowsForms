Gtk.Windows.Extensions

Additions to the `System.Windows.Forms` surface provided by `Gtk.Windows.Forms` that are **not** part
of the real WinForms API. They live in a separate package so the core library stays a faithful
WinForms compatibility layer; add your own extension classes here.

## AutoClosingMessageBox

An `IMessageBox` that shows a message box and disposes the underlying GTK dialog after a timeout —
useful for unattended/automated runs where no one is there to click OK.

```csharp
using System.Windows.Forms;

// Install behind the static MessageBox helpers: every MessageBox.Show(...) now
// auto-closes after 500 ms.
AutoClosingMessageBox.Install(500);
MessageBox.Show("Auto closing");

// Restore the normal, blocking message box.
MessageBox.ReplaceInstance();
```

`MessageBox.ReplaceInstance(int)` in `Gtk.Windows.Forms` used to install this type; it is now obsolete
and always installs the default blocking message box. Call `AutoClosingMessageBox.Install(timeout)`
instead.

## WebBrowser: the asynchronous siblings

`System.Windows.Forms.HtmlDocument.InvokeScript` and `HtmlElement.InvokeMember` post the call and return
`null` in this library: WebKitGTK and WebView2 both run the page in a separate process, so returning the
value would mean blocking the UI thread on it. `WebBrowserAsyncExtensions` is the way to get the answer.

```csharp
using System.Windows.Forms;

var title = await browser.InvokeScriptAsync("document.title");
var markup = await browser.GetDocumentTextAsync();
var value = await element.InvokeMemberAsync("getAttribute", "href");

// After the page has changed itself, so the synchronous Html* getters see it:
await browser.RefreshDomAsync();
```

`WebBrowserDefaults.HomeUrl` and `.SearchUrl` say where `WebBrowser.GoHome()` and `GoSearch()` go. Both
default to `about:blank`: real WinForms reads Internet Explorer's registry settings for these, which is
not something a cross-platform library can do, and hard-coding a vendor's page is not something it should.

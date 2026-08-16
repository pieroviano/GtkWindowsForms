//
// WebBrowser.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2007, 2008 Novell, Inc.
//
// Authors:
//  Andreia Gaita <avidigal@novell.com>
//
// The Mozilla/XPCOM host this originally drove is gone; navigation and state now go through
// IWebViewBackend, which is WebKitGTK on Linux and macOS and WebView2 on Windows.
//

using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GtkSystem.Windows.Forms.WebBrowsing;

namespace System.Windows.Forms;

/// <summary>
/// Enables the user to navigate Web pages inside a form.
/// </summary>
/// <remarks>
/// Backed by WebKitGTK on Linux and macOS and by the Microsoft Edge WebView2 runtime on Windows. When
/// neither is present the control still constructs and still raises its navigation events, showing a
/// message naming what to install; see
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.NullWebViewBackend" />.
/// </remarks>
[DefaultProperty("Url")]
[DefaultEvent("DocumentCompleted")]
[Docking(DockingBehavior.AutoDock)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DesignerCategory("Component")]
public class WebBrowser : WebBrowserBase
{
    private bool _allowNavigation = true;

    private HtmlDocument? _document;

    private bool _hasNavigated;

    private int _historyLength;

    private object? _objectForScripting;

    private WebBrowserReadyState _readyState;

    private bool _reportedCanGoBack;

    private bool _reportedCanGoForward;

    private string _reportedTitle = string.Empty;

    private WebBrowserEncryptionLevel _securityLevel;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WebBrowser" /> class.
    /// </summary>
    public WebBrowser()
    {
        Backend.Dom.SnapshotUpdated += OnSnapshotUpdated;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control may navigate more than once.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> to allow every navigation. When
    /// <c>
    /// false
    /// </c>, only the first navigation is performed and later ones are ignored.
    /// </value>
    [DefaultValue(true)]
    public bool AllowNavigation
    {
        get => _allowNavigation;
        set => _allowNavigation = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page may accept dropped files.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when drops are allowed.
    /// </value>
    [DefaultValue(true)]
    public bool AllowWebBrowserDrop
    {
        get => Backend.AllowDrop;
        set => Backend.AllowDrop = value;
    }

    /// <summary>
    /// Gets a value indicating whether the control can navigate back.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when there is a previous entry.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CanGoBack => Backend.CanGoBack;

    /// <summary>
    /// Gets a value indicating whether the control can navigate forward.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when there is a following entry.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CanGoForward => Backend.CanGoForward;

    /// <summary>
    /// Gets the document currently displayed.
    /// </summary>
    /// <value>
    /// The document, or
    /// <c>
    /// null
    /// </c> until a load has completed - which matches both real WinForms and mono, and is why
    /// <see cref="E:System.Windows.Forms.WebBrowser.DocumentCompleted" /> is the event to read it from.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HtmlDocument? Document
    {
        get
        {
            if (_document == null && documentReady)
            {
                _document = new HtmlDocument(this, Backend.Dom);
            }

            return _document;
        }
    }

    /// <summary>
    /// Gets or sets a stream containing the document to display.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// null
    /// </c> on get, as in mono: the engines do not hand back the bytes they parsed. Setting loads the
    /// stream's contents as the document.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Stream? DocumentStream
    {
        get => null;
        set
        {
            if (value == null)
            {
                return;
            }

            using (var reader = new StreamReader(value, Encoding.UTF8, true, 1024, true))
            {
                DocumentText = reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Gets or sets the document's markup.
    /// </summary>
    /// <value>
    /// The markup of the loaded document, taken from the last snapshot, or the empty string when nothing
    /// has loaded. Setting loads the markup as a new document.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DocumentText
    {
        get
        {
            var snapshot = Backend.Dom.Snapshot;
            var root = snapshot.Find(snapshot.DocumentElementHandle);
            return root?.OuterHtml ?? string.Empty;
        }
        set
        {
            documentReady = false;
            _document = null;
            Backend.LoadHtml(value ?? string.Empty, null);
        }
    }

    /// <summary>
    /// Gets the title of the document currently displayed.
    /// </summary>
    /// <value>
    /// The document title, or the empty string.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DocumentTitle => Backend.DocumentTitle;

    /// <summary>
    /// Gets the type of the document currently displayed.
    /// </summary>
    /// <value>
    /// The doctype name, or the empty string.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DocumentType => _document?.DocType ?? string.Empty;

    /// <summary>
    /// Gets how strongly the current document's transport is encrypted.
    /// </summary>
    /// <value>
    /// The encryption level.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WebBrowserEncryptionLevel EncryptionLevel => _securityLevel;

    /// <summary>
    /// Gets a value indicating whether the control has keyboard focus.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the control is focused.
    /// </value>
    public override bool Focused => base.Focused;

    /// <summary>
    /// Gets a value indicating whether a load is in progress.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> while the engine is loading.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsBusy => !documentReady;

    /// <summary>
    /// Gets a value indicating whether the engine is working offline.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// false
    /// </c>: neither engine has a global offline mode to read.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsOffline => false;

    /// <summary>
    /// Gets or sets a value indicating whether the engine shows its own context menu.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the engine's context menu is enabled.
    /// </value>
    [DefaultValue(true)]
    public bool IsWebBrowserContextMenuEnabled
    {
        get => Backend.ContextMenuEnabled;
        set => Backend.ContextMenuEnabled = value;
    }

    /// <summary>
    /// Gets or sets the object the page's script can reach as
    /// <c>
    /// window.external
    /// </c>.
    /// </summary>
    /// <value>
    /// The scripting object, or
    /// <c>
    /// null
    /// </c>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? ObjectForScripting
    {
        get => _objectForScripting;
        set => _objectForScripting = value;
    }

    /// <summary>
    /// Gets or sets the control's padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    /// <summary>
    /// Gets how far the current document has been parsed.
    /// </summary>
    /// <value>
    /// The ready state.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public WebBrowserReadyState ReadyState => _readyState;

    /// <summary>
    /// Gets or sets a value indicating whether script errors are hidden.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when script errors are suppressed.
    /// </value>
    [DefaultValue(false)]
    public bool ScriptErrorsSuppressed
    {
        get => SuppressDialogs;
        set => SuppressDialogs = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page may show scroll bars.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when scroll bars are allowed.
    /// </value>
    [DefaultValue(true)]
    public bool ScrollBarsEnabled
    {
        get => Backend.ScrollBarsEnabled;
        set => Backend.ScrollBarsEnabled = value;
    }

    /// <summary>
    /// Gets the text the engine last offered for a status bar.
    /// </summary>
    /// <value>
    /// The status text.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual string StatusText => status;

    /// <summary>
    /// Gets or sets the URL of the document currently displayed.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c> when nothing has loaded. Setting navigates.
    /// </value>
    [Bindable(true)]
    [DefaultValue(null)]
    [TypeConverter(typeof(WebBrowserUriTypeConverter))]
    public Uri? Url
    {
        get => Backend.Url;
        set => Navigate(value ?? WebViewBackendFactory.BlankUri);
    }

    /// <summary>
    /// Gets the version of the engine behind the control.
    /// </summary>
    /// <value>
    /// The version of the assembly implementing the backend.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Version Version => Backend.GetType().GetTypeInfo().Assembly.GetName().Version;

    /// <summary>
    /// Gets or sets a value indicating whether the engine handles its own keyboard shortcuts.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when shortcuts are enabled.
    /// </value>
    [DefaultValue(true)]
    public bool WebBrowserShortcutsEnabled
    {
        get => Backend.ShortcutsEnabled;
        set => Backend.ShortcutsEnabled = value;
    }

    /// <summary>
    /// Gets the control's default size.
    /// </summary>
    /// <value>
    /// The default size.
    /// </value>
    protected override Size DefaultSize => base.DefaultSize;

    /// <summary>
    /// Gets how many documents this control has navigated to.
    /// </summary>
    /// <value>
    /// The history length.
    /// </value>
    internal int HistoryLength => _historyLength;

    /// <summary>
    /// Navigates back one entry.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> when a previous entry existed to go back to.
    /// </returns>
    public bool GoBack()
    {
        if (!Backend.CanGoBack)
        {
            return false;
        }

        BeginNavigation();
        Backend.GoBack();
        return true;
    }

    /// <summary>
    /// Navigates forward one entry.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> when a following entry existed to go forward to.
    /// </returns>
    public bool GoForward()
    {
        if (!Backend.CanGoForward)
        {
            return false;
        }

        BeginNavigation();
        Backend.GoForward();
        return true;
    }

    /// <summary>
    /// Navigates to the configured home page.
    /// </summary>
    /// <remarks>
    /// mono hard-coded its vendor's page here and real WinForms reads Internet Explorer's registry
    /// setting, neither of which is right for a cross-platform library. The destination is
    /// <see cref="P:System.Windows.Forms.WebBrowser.HomeUrl" />, which defaults to
    /// <c>
    /// about:blank
    /// </c>.
    /// </remarks>
    public void GoHome()
    {
        Navigate(HomeUrl);
    }

    /// <summary>
    /// Navigates to the configured search page.
    /// </summary>
    /// <remarks>
    /// See
    /// <see cref="M:System.Windows.Forms.WebBrowser.GoHome" />: the destination is
    /// <see cref="P:System.Windows.Forms.WebBrowser.SearchUrl" />, not a hard-coded vendor URL.
    /// </remarks>
    public void GoSearch()
    {
        Navigate(SearchUrl);
    }

    /// <summary>
    /// Gets or sets the page
    /// <see cref="M:System.Windows.Forms.WebBrowser.GoHome" /> navigates to.
    /// </summary>
    /// <value>
    /// The home page. Defaults to
    /// <c>
    /// about:blank
    /// </c>.
    /// </value>
    /// <remarks>
    /// Internal, and surfaced publicly as
    /// <c>
    /// WebBrowserDefaults.HomeUrl
    /// </c> by the
    /// <c>
    /// Gtk.Windows.Extensions
    /// </c> package: real WinForms has no such member, and this type is a compatibility surface rather
    /// than a place to add to.
    /// </remarks>
    internal static Uri HomeUrl
    {
        get;
        set;
    } = WebViewBackendFactory.BlankUri;

    /// <summary>
    /// Gets or sets the page
    /// <see cref="M:System.Windows.Forms.WebBrowser.GoSearch" /> navigates to.
    /// </summary>
    /// <value>
    /// The search page. Defaults to
    /// <c>
    /// about:blank
    /// </c>.
    /// </value>
    /// <remarks>
    /// Internal for the reason given on
    /// <see cref="P:System.Windows.Forms.WebBrowser.HomeUrl" />.
    /// </remarks>
    internal static Uri SearchUrl
    {
        get;
        set;
    } = WebViewBackendFactory.BlankUri;

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate. A blank string navigates to
    /// <c>
    /// about:blank
    /// </c>; a string with no scheme is read as
    /// <c>
    /// http
    /// </c>.
    /// </param>
    public void Navigate(string urlString)
    {
        Navigate(ToNavigableUri(urlString));
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    public void Navigate(Uri url)
    {
        Navigate(url, null, null, null);
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate.
    /// </param>
    /// <param name="newWindow">
    /// Whether to open a new window. Ignored: one control hosts one engine view. Handle
    /// <see cref="E:System.Windows.Forms.WebBrowser.NewWindow" /> to host a second control yourself.
    /// </param>
    public void Navigate(string urlString, bool newWindow)
    {
        Navigate(ToNavigableUri(urlString));
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate.
    /// </param>
    /// <param name="targetFrameName">
    /// The frame to navigate, or
    /// <c>
    /// null
    /// </c> for the main frame.
    /// </param>
    public void Navigate(string urlString, string targetFrameName)
    {
        Navigate(ToNavigableUri(urlString), targetFrameName, null, null);
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    /// <param name="newWindow">
    /// Whether to open a new window. Ignored; see
    /// <see cref="M:System.Windows.Forms.WebBrowser.Navigate(System.String,System.Boolean)" />.
    /// </param>
    public void Navigate(Uri url, bool newWindow)
    {
        Navigate(url);
    }

    /// <summary>
    /// Navigates to a URL.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    /// <param name="targetFrameName">
    /// The frame to navigate, or
    /// <c>
    /// null
    /// </c> for the main frame.
    /// </param>
    public void Navigate(Uri url, string? targetFrameName)
    {
        Navigate(url, targetFrameName, null, null);
    }

    /// <summary>
    /// Navigates to a URL, posting data.
    /// </summary>
    /// <param name="urlString">
    /// Where to navigate.
    /// </param>
    /// <param name="targetFrameName">
    /// The frame to navigate.
    /// </param>
    /// <param name="postData">
    /// The request body.
    /// </param>
    /// <param name="additionalHeaders">
    /// Extra request headers, CRLF-separated.
    /// </param>
    public void Navigate(string urlString, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        Navigate(ToNavigableUri(urlString), targetFrameName, postData, additionalHeaders);
    }

    /// <summary>
    /// Navigates to a URL, posting data.
    /// </summary>
    /// <param name="url">
    /// Where to navigate.
    /// </param>
    /// <param name="targetFrameName">
    /// The frame to navigate.
    /// </param>
    /// <param name="postData">
    /// The request body.
    /// </param>
    /// <param name="additionalHeaders">
    /// Extra request headers, CRLF-separated.
    /// </param>
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (!_allowNavigation && _hasNavigated)
        {
            return;
        }

        BeginNavigation();
        Backend.Navigate(url, targetFrameName, postData, additionalHeaders);
    }

    /// <summary>
    /// Prints the document.
    /// </summary>
    public void Print()
    {
        _ = Backend.InvokeScriptAsync("window.print()", CancellationToken.None);
    }

    /// <summary>
    /// Reloads the document, revalidating it against the server.
    /// </summary>
    public override void Refresh()
    {
        Refresh(WebBrowserRefreshOption.IfExpired);
    }

    /// <summary>
    /// Reloads the document.
    /// </summary>
    /// <param name="opt">
    /// How thoroughly to reload. The engines offer two modes rather than four:
    /// <see cref="F:System.Windows.Forms.WebBrowserRefreshOption.Completely" /> bypasses every cache and
    /// the other three revalidate.
    /// </param>
    public void Refresh(WebBrowserRefreshOption opt)
    {
        BeginNavigation();
        Backend.Reload(opt == WebBrowserRefreshOption.Completely);
    }

    /// <summary>
    /// Shows the engine's page setup dialog.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. Neither engine exposes a page setup dialog separately from its print dialog.
    /// </exception>
    public void ShowPageSetupDialog()
    {
        throw new NotSupportedException(
            "Neither WebKitGTK nor WebView2 exposes a page setup dialog separately from its print dialog. " +
            "Call Print instead.");
    }

    /// <summary>
    /// Shows the engine's print dialog.
    /// </summary>
    public void ShowPrintDialog()
    {
        Print();
    }

    /// <summary>
    /// Shows the engine's print preview.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. Both engines fold preview into their print dialog.
    /// </exception>
    public void ShowPrintPreviewDialog()
    {
        throw new NotSupportedException(
            "Neither WebKitGTK nor WebView2 exposes a print preview separately from its print dialog. " +
            "Call Print instead.");
    }

    /// <summary>
    /// Shows the document's properties dialog.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. This was an Internet Explorer dialog; neither engine has one.
    /// </exception>
    public void ShowPropertiesDialog()
    {
        throw new NotSupportedException(
            "The document properties dialog was an Internet Explorer feature; neither WebKitGTK nor " +
            "WebView2 provides one.");
    }

    /// <summary>
    /// Shows the engine's save-as dialog.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always. Neither engine exposes its save-as dialog to the host.
    /// </exception>
    public void ShowSaveAsDialog()
    {
        throw new NotSupportedException(
            "Neither WebKitGTK nor WebView2 exposes a save-as dialog to the host. Read DocumentText and " +
            "write it yourself.");
    }

    /// <summary>
    /// Stops the current load.
    /// </summary>
    public void Stop()
    {
        Backend.Stop();
    }

    /// <summary>
    /// Attaches to the underlying ActiveX interfaces.
    /// </summary>
    /// <param name="nativeActiveXObject">
    /// The ActiveX object.
    /// </param>
    /// <exception cref="T:System.NotSupportedException">
    /// Always.
    /// </exception>
    protected override void AttachInterfaces(object nativeActiveXObject)
    {
        base.AttachInterfaces(nativeActiveXObject);
    }

    /// <summary>
    /// Creates the ActiveX event sink.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always.
    /// </exception>
    protected override void CreateSink()
    {
        base.CreateSink();
    }

    /// <summary>
    /// Creates the ActiveX host site.
    /// </summary>
    /// <returns>
    /// Never returns.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// Always.
    /// </exception>
    protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
    {
        return base.CreateWebBrowserSiteBase();
    }

    /// <summary>
    /// Detaches from the underlying ActiveX interfaces.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always.
    /// </exception>
    protected override void DetachInterfaces()
    {
        base.DetachInterfaces();
    }

    /// <summary>
    /// Releases the ActiveX event sink.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">
    /// Always.
    /// </exception>
    protected override void DetachSink()
    {
        base.DetachSink();
    }

    /// <summary>
    /// Releases the control.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> when called from
    /// <see cref="M:System.ComponentModel.Component.Dispose" />.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Backend.Dom.SnapshotUpdated -= OnSnapshotUpdated;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.CanGoBackChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnCanGoBackChanged(EventArgs e)
    {
        CanGoBackChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.CanGoForwardChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnCanGoForwardChanged(EventArgs e)
    {
        CanGoForwardChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.DocumentCompleted" /> event.
    /// </summary>
    /// <param name="e">
    /// The document that finished loading.
    /// </param>
    protected virtual void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
    {
        DocumentCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.DocumentTitleChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnDocumentTitleChanged(EventArgs e)
    {
        DocumentTitleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.EncryptionLevelChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnEncryptionLevelChanged(EventArgs e)
    {
        EncryptionLevelChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.FileDownload" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnFileDownload(EventArgs e)
    {
        FileDownload?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.Navigated" /> event.
    /// </summary>
    /// <param name="e">
    /// Where the control navigated.
    /// </param>
    protected virtual void OnNavigated(WebBrowserNavigatedEventArgs e)
    {
        Navigated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.Navigating" /> event.
    /// </summary>
    /// <param name="e">
    /// The navigation, which a handler may cancel.
    /// </param>
    protected virtual void OnNavigating(WebBrowserNavigatingEventArgs e)
    {
        Navigating?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.NewWindow" /> event.
    /// </summary>
    /// <param name="e">
    /// The request, which a handler may cancel.
    /// </param>
    protected virtual void OnNewWindow(CancelEventArgs e)
    {
        NewWindow?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.ProgressChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The progress.
    /// </param>
    protected virtual void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
    {
        ProgressChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="E:System.Windows.Forms.WebBrowser.StatusTextChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// Empty.
    /// </param>
    protected virtual void OnStatusTextChanged(EventArgs e)
    {
        StatusTextChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    internal override void OnBackendLoadFinished(object? sender, BackendLoadEventArgs e)
    {
        documentReady = true;
        _readyState = e.Succeeded ? WebBrowserReadyState.Complete : WebBrowserReadyState.Uninitialized;
        _securityLevel = Backend.EncryptionLevel;
        RaiseStateChanges();
        OnDocumentCompleted(new WebBrowserDocumentCompletedEventArgs(e.Url));
    }

    /// <inheritdoc />
    internal override void OnBackendNavigated(object? sender, BackendNavigatedEventArgs e)
    {
        _readyState = WebBrowserReadyState.Loaded;
        _historyLength++;
        RaiseStateChanges();
        OnNavigated(new WebBrowserNavigatedEventArgs(e.Url));
    }

    /// <inheritdoc />
    internal override void OnBackendNavigating(object? sender, BackendNavigatingEventArgs e)
    {
        documentReady = false;
        _document = null;
        _readyState = WebBrowserReadyState.Loading;

        var args = new WebBrowserNavigatingEventArgs(e.Url, e.TargetFrameName ?? string.Empty);
        OnNavigating(args);
        e.Cancel = args.Cancel;
    }

    /// <inheritdoc />
    internal override void OnBackendNewWindow(object? sender, BackendNewWindowEventArgs e)
    {
        var args = new CancelEventArgs();
        OnNewWindow(args);
        e.Cancel = args.Cancel;
    }

    /// <inheritdoc />
    internal override void OnBackendProgressChanged(object? sender, BackendProgressEventArgs e)
    {
        _readyState = WebBrowserReadyState.Interactive;
        OnProgressChanged(new WebBrowserProgressChangedEventArgs(e.Progress, e.MaximumProgress));
    }

    /// <inheritdoc />
    internal override void OnBackendStatusChanged(object? sender, EventArgs e)
    {
        status = Backend.StatusText;
        OnStatusTextChanged(EventArgs.Empty);
    }

    /// <inheritdoc />
    internal override void OnBackendTitleChanged(object? sender, EventArgs e)
    {
        RaiseStateChanges();
    }

    /// <summary>
    /// Occurs when
    /// <see cref="P:System.Windows.Forms.WebBrowser.CanGoBack" /> changes.
    /// </summary>
    [Browsable(false)]
    public event EventHandler? CanGoBackChanged;

    /// <summary>
    /// Occurs when
    /// <see cref="P:System.Windows.Forms.WebBrowser.CanGoForward" /> changes.
    /// </summary>
    [Browsable(false)]
    public event EventHandler? CanGoForwardChanged;

    /// <summary>
    /// Occurs when a document has finished loading.
    /// </summary>
    public event WebBrowserDocumentCompletedEventHandler? DocumentCompleted;

    /// <summary>
    /// Occurs when
    /// <see cref="P:System.Windows.Forms.WebBrowser.DocumentTitle" /> changes.
    /// </summary>
    [Browsable(false)]
    public event EventHandler? DocumentTitleChanged;

    /// <summary>
    /// Occurs when
    /// <see cref="P:System.Windows.Forms.WebBrowser.EncryptionLevel" /> changes.
    /// </summary>
    [Browsable(false)]
    public event EventHandler? EncryptionLevelChanged;

    /// <summary>
    /// Occurs when the engine starts a file download.
    /// </summary>
    public event EventHandler? FileDownload;

    /// <summary>
    /// Occurs once a navigation has committed.
    /// </summary>
    public event WebBrowserNavigatedEventHandler? Navigated;

    /// <summary>
    /// Occurs before a navigation starts. Cancelling stops it.
    /// </summary>
    public event WebBrowserNavigatingEventHandler? Navigating;

    /// <summary>
    /// Occurs when the page asks to open a window. Cancelling refuses it.
    /// </summary>
    public event CancelEventHandler? NewWindow;

    /// <summary>
    /// Occurs as the current load advances.
    /// </summary>
    public event WebBrowserProgressChangedEventHandler? ProgressChanged;

    /// <summary>
    /// Occurs when
    /// <see cref="P:System.Windows.Forms.WebBrowser.StatusText" /> changes.
    /// </summary>
    [Browsable(false)]
    public event EventHandler? StatusTextChanged;

    /// <summary>
    /// Occurs when the control's padding changes.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new event EventHandler? PaddingChanged;

    /// <summary>
    /// Reads a URL the way real WinForms does: a blank string is
    /// <c>
    /// about:blank
    /// </c>, and a string with no scheme is read as
    /// <c>
    /// http
    /// </c> rather than rejected.
    /// </summary>
    /// <param name="urlString">
    /// The string the caller passed.
    /// </param>
    /// <returns>
    /// The URL to navigate to.
    /// </returns>
    internal static Uri ToNavigableUri(string? urlString)
    {
        if (string.IsNullOrWhiteSpace(urlString))
        {
            return WebViewBackendFactory.BlankUri;
        }

        var text = urlString!.Trim();

        if (Uri.TryCreate(text, UriKind.Absolute, out var absolute))
        {
            return absolute;
        }

        return Uri.TryCreate("http://" + text, UriKind.Absolute, out var prefixed)
            ? prefixed
            : WebViewBackendFactory.BlankUri;
    }

    private void BeginNavigation()
    {
        documentReady = false;
        _document = null;
        _hasNavigated = true;
    }

    private void OnSnapshotUpdated(object? sender, EventArgs e)
    {
        RaiseStateChanges();
    }

    /// <summary>
    /// Raises the change notifications whose backing values have actually moved. WinForms fires these
    /// on change, not on every poll, and applications bind to them.
    /// </summary>
    private void RaiseStateChanges()
    {
        if (_reportedCanGoBack != Backend.CanGoBack)
        {
            _reportedCanGoBack = Backend.CanGoBack;
            OnCanGoBackChanged(EventArgs.Empty);
        }

        if (_reportedCanGoForward != Backend.CanGoForward)
        {
            _reportedCanGoForward = Backend.CanGoForward;
            OnCanGoForwardChanged(EventArgs.Empty);
        }

        if (!string.Equals(_reportedTitle, Backend.DocumentTitle, StringComparison.Ordinal))
        {
            _reportedTitle = Backend.DocumentTitle;
            OnDocumentTitleChanged(EventArgs.Empty);
        }

        if (_securityLevel != Backend.EncryptionLevel)
        {
            _securityLevel = Backend.EncryptionLevel;
            OnEncryptionLevelChanged(EventArgs.Empty);
        }
    }
}

/// <summary>
/// Converts a string to the
/// <see cref="T:System.Uri" /> that
/// <see cref="P:System.Windows.Forms.WebBrowser.Url" /> takes.
/// </summary>
internal class WebBrowserUriTypeConverter : UriTypeConverter
{
}

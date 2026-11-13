using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gtk.Windows.Forms.WebBrowsing;
using Microsoft.Web.WebView2.Core;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// The DOM bridge over a WebView2 browser: the same injected script the WebKitGTK backend uses, driven
/// through <c>ExecuteScriptAsync</c>.
/// </summary>
internal sealed class WebView2HtmlDomBridge : IHtmlDomBridge
{
    private readonly WebView2Backend _backend;

    internal WebView2HtmlDomBridge(WebView2Backend backend)
    {
        _backend = backend;
    }

    /// <inheritdoc />
    public DomSnapshot Snapshot
    {
        get;
        private set;
    } = DomSnapshot.Empty;

    /// <inheritdoc />
    public async Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args,
        CancellationToken cancellationToken)
    {
        var script = string.Format(CultureInfo.InvariantCulture, "{0}.invoke({1}, {2}, {3})",
            DomBridgeScript.Global, nodeHandle.ToString(CultureInfo.InvariantCulture),
            JavaScriptLiteral.Quote(member), JavaScriptLiteral.Array(args));

        return await _backend.InvokeScriptAsync(script, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Post(DomMutation mutation)
    {
        if (mutation == null)
        {
            throw new ArgumentNullException(nameof(mutation));
        }

        var request = string.Format(CultureInfo.InvariantCulture,
            "{{ kind: {0}, target: {1}, related: {2}, reference: {3}, name: {4}, value: {5}, intValue: {6}, flag: {7} }}",
            JavaScriptLiteral.Quote(mutation.Kind.ToString()),
            mutation.TargetHandle.ToString(CultureInfo.InvariantCulture),
            mutation.RelatedHandle.ToString(CultureInfo.InvariantCulture),
            mutation.ReferenceHandle.ToString(CultureInfo.InvariantCulture),
            JavaScriptLiteral.Quote(mutation.Name),
            JavaScriptLiteral.Quote(mutation.Value),
            mutation.IntValue.ToString(CultureInfo.InvariantCulture),
            mutation.Flag ? "true" : "false");

        // Fire-and-forget: Post is called from synchronous WinForms setters.
        _ = _backend.InvokeScriptAsync(
            DomBridgeScript.Global + ".mutate(" + request + ")", CancellationToken.None);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var json = await _backend
            .InvokeScriptAsync(DomBridgeScript.Global + ".snapshot()", cancellationToken)
            .ConfigureAwait(false);

        Replace(DomSnapshot.FromJson(json as string));
    }

    internal void RaiseDomEvent(DomEventArgs e) => DomEvent?.Invoke(this, e);

    internal void Replace(DomSnapshot snapshot)
    {
        Snapshot = snapshot;
        SnapshotUpdated?.Invoke(this, global::System.EventArgs.Empty);
    }

    /// <inheritdoc />
    public event global::System.EventHandler<DomEventArgs>? DomEvent;

    /// <inheritdoc />
    public event global::System.EventHandler? SnapshotUpdated;
}

/// <summary>
/// The Microsoft Edge WebView2 backend: the engine behind
/// <see cref="T:System.Windows.Forms.WebBrowser" /> on Windows.
/// </summary>
/// <remarks>
/// Registers itself above the WebKitGTK backend, so it wins on Windows where both could in principle
/// load. Its probe declines when the runtime is not installed, and the factory then falls through - to
/// WebKitGTK if somebody installed it, otherwise to the placeholder.
///
/// Initialisation is asynchronous. Commands issued before the browser is ready are queued and replayed,
/// so an application that navigates from a form's constructor is not silently ignored.
/// </remarks>
public sealed class WebView2Backend : IWebViewBackend
{
    private readonly WebView2HtmlDomBridge _bridge;

    private readonly global::Gtk.DrawingArea _placeholder;

    private readonly Queue<Action> _queued = new Queue<Action>();

    private readonly WebView2HostWindow _host;

    private bool _disposed;

    private bool _initializing;

    private string _statusText = string.Empty;

    private string _title = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebView2Backend" /> class.
    /// </summary>
    public WebView2Backend()
    {
        // GTK still needs a widget to lay out and to give the control its area, even though the browser
        // paints into a child HWND above it rather than into this widget.
        _placeholder = new global::Gtk.DrawingArea
        {
            Hexpand = true,
            Vexpand = true
        };

        _bridge = new WebView2HtmlDomBridge(this);
        _host = new WebView2HostWindow(_placeholder);
        _host.Ready += OnHostReady;

        _placeholder.Realized += OnRealized;
    }

    /// <inheritdoc />
    public bool AllowDrop
    {
        get;
        set;
    } = true;

    /// <inheritdoc />
    public bool CanGoBack => _host.Core?.CanGoBack ?? false;

    /// <inheritdoc />
    public bool CanGoForward => _host.Core?.CanGoForward ?? false;

    /// <inheritdoc />
    public bool ContextMenuEnabled
    {
        get => _contextMenuEnabled;
        set
        {
            _contextMenuEnabled = value;
            if (_host.Core != null)
            {
                _host.Core.Settings.AreDefaultContextMenusEnabled = value;
            }
        }
    }

    /// <inheritdoc />
    public string DocumentTitle => _host.Core?.DocumentTitle ?? string.Empty;

    /// <inheritdoc />
    public IHtmlDomBridge Dom => _bridge;

    /// <inheritdoc />
    /// <remarks>
    /// WebView2 reports a server certificate, not a cipher strength, so the Internet-Explorer-era tiers
    /// this enumeration names cannot be reproduced. An <c>https</c> document is reported as
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Bit128" /> and anything else as
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Insecure" />.
    /// </remarks>
    public WebBrowserEncryptionLevel EncryptionLevel =>
        Url != null && string.Equals(Url.Scheme, "https", StringComparison.OrdinalIgnoreCase)
            ? WebBrowserEncryptionLevel.Bit128
            : WebBrowserEncryptionLevel.Insecure;

    /// <inheritdoc />
    public bool IsBusy
    {
        get;
        private set;
    }

    /// <summary>Gets a value indicating whether a real engine is behind this backend.</summary>
    /// <value>Always <c>true</c>.</value>
    public bool IsFunctional => true;

    /// <inheritdoc />
    /// <remarks>
    /// WebView2 reports no incremental load progress, so this steps from 0 to 100 rather than counting up.
    /// Recorded in EMULATION-GAPS.md.
    /// </remarks>
    public int Progress => IsBusy ? 0 : 100;

    /// <inheritdoc />
    public WebBrowserReadyState ReadyState
    {
        get;
        private set;
    } = WebBrowserReadyState.Uninitialized;

    /// <inheritdoc />
    public bool ScriptErrorsSuppressed
    {
        get;
        set;
    }

    /// <inheritdoc />
    /// <remarks>WebView2 has no setting for this either; it is applied as injected CSS.</remarks>
    public bool ScrollBarsEnabled
    {
        get => _scrollBarsEnabled;
        set
        {
            _scrollBarsEnabled = value;
            _ = InvokeScriptAsync(
                value
                    ? "(function(){var s=document.getElementById('__gtkwfNoScroll'); if (s) { s.remove(); }})()"
                    : "(function(){if(document.getElementById('__gtkwfNoScroll')){return;}" +
                      "var s=document.createElement('style');s.id='__gtkwfNoScroll';" +
                      "s.textContent='html,body{overflow:hidden}::-webkit-scrollbar{display:none}';" +
                      "(document.head||document.documentElement).appendChild(s);})()",
                CancellationToken.None);
        }
    }

    /// <inheritdoc />
    public bool ShortcutsEnabled
    {
        get => _shortcutsEnabled;
        set
        {
            _shortcutsEnabled = value;
            if (_host.Core != null)
            {
                _host.Core.Settings.AreBrowserAcceleratorKeysEnabled = value;
            }
        }
    }

    /// <inheritdoc />
    public string StatusText => _statusText;

    /// <inheritdoc />
    public Uri? Url
    {
        get
        {
            var source = _host.Core?.Source;
            return Uri.TryCreate(source, UriKind.Absolute, out var url) ? url : null;
        }
    }

    /// <inheritdoc />
    public global::Gtk.Widget Widget => _placeholder;

    /// <summary>
    /// Gets how many commands are waiting for the browser to finish initialising.
    /// </summary>
    /// <value>
    /// The queue depth. Exposed for tests: that a navigation issued before the browser is ready is held
    /// rather than dropped is the contract, and it has no other observable effect.
    /// </value>
    internal int PendingCommandCount => _queued.Count;

    private bool _contextMenuEnabled = true;

    private bool _scrollBarsEnabled = true;

    private bool _shortcutsEnabled = true;

    /// <summary>
    /// Probes for a usable WebView2 runtime.
    /// </summary>
    /// <returns>A backend, or <c>null</c> when Windows, GTK or the runtime cannot provide one.</returns>
    public static IWebViewBackend? TryCreate()
    {
        if (!GdkWin32Interop.IsSupported)
        {
            // Not Windows, or a GTK 4 runtime with no win32 backend: there is no HWND to parent to.
            return null;
        }

        if (!WebView2Runtime.IsRuntimeAvailable && !WebView2Runtime.AllowRuntimeBootstrap)
        {
            return null;
        }

        if (!WebView2Runtime.IsRuntimeAvailable &&
            !WebView2Runtime.TryBootstrapAsync().GetAwaiter().GetResult())
        {
            return null;
        }

        return new WebView2Backend();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _placeholder.Realized -= OnRealized;
        _host.Ready -= OnHostReady;
        _host.Dispose();
        _placeholder.Dispose();
    }

    /// <inheritdoc />
    public void GoBack() => Post(() => _host.Core?.GoBack());

    /// <inheritdoc />
    public void GoForward() => Post(() => _host.Core?.GoForward());

    /// <inheritdoc />
    public async Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken)
    {
        var core = _host.Core;
        if (core == null)
        {
            return null;
        }

        try
        {
            // ExecuteScriptAsync answers with the value as JSON, so "abc" comes back quoted and 3 comes
            // back bare; unwrap so callers see the same CLR shapes the WebKitGTK backend produces.
            var json = await core.ExecuteScriptAsync(script).ConfigureAwait(true);
            return FromJsonResult(json);
        }
        catch (Exception exception) when (exception is InvalidOperationException or ObjectDisposedException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public void LoadHtml(string html, Uri? baseUri) =>
        Post(() => _host.Core?.NavigateToString(html ?? string.Empty));

    /// <inheritdoc />
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        Post(() =>
        {
            var core = _host.Core;
            if (core == null)
            {
                return;
            }

            if (postData == null && string.IsNullOrEmpty(additionalHeaders))
            {
                core.Navigate(url.ToString());
                return;
            }

            var environment = CoreWebView2Environment.CreateAsync().GetAwaiter().GetResult();
            var content = postData == null
                ? null
                : new global::System.IO.MemoryStream(postData);

            var request = environment.CreateWebResourceRequest(
                url.ToString(),
                postData == null ? "GET" : "POST",
                content,
                additionalHeaders ?? string.Empty);

            core.NavigateWithWebResourceRequest(request);
        });
    }

    /// <inheritdoc />
    public void Reload(bool bypassCache) => Post(() => _host.Core?.Reload());

    /// <inheritdoc />
    public void Stop() => Post(() => _host.Core?.Stop());

    /// <inheritdoc />
    public event global::System.EventHandler<BackendLoadEventArgs>? LoadFinished;

    /// <inheritdoc />
    public event global::System.EventHandler<BackendNavigatedEventArgs>? Navigated;

    /// <inheritdoc />
    public event global::System.EventHandler<BackendNavigatingEventArgs>? Navigating;

    /// <inheritdoc />
    public event global::System.EventHandler<BackendNewWindowEventArgs>? NewWindow;

    /// <inheritdoc />
    public event global::System.EventHandler<BackendProgressEventArgs>? ProgressChanged;

    /// <inheritdoc />
    public event global::System.EventHandler? StatusChanged;

    /// <inheritdoc />
    public event global::System.EventHandler? TitleChanged;

    private void OnDocumentTitleChanged(object? sender, object e)
    {
        var title = DocumentTitle;
        if (string.Equals(_title, title, StringComparison.Ordinal))
        {
            return;
        }

        _title = title;
        TitleChanged?.Invoke(this, global::System.EventArgs.Empty);
    }

    private void OnHostReady(object? sender, global::System.EventArgs e)
    {
        var core = _host.Core;
        if (core == null)
        {
            return;
        }

        core.Settings.AreDefaultContextMenusEnabled = _contextMenuEnabled;
        core.Settings.AreBrowserAcceleratorKeysEnabled = _shortcutsEnabled;

        core.AddScriptToExecuteOnDocumentCreatedAsync(DomBridgeScript.Source);
        core.WebMessageReceived += OnWebMessageReceived;
        core.NavigationStarting += OnNavigationStarting;
        core.SourceChanged += OnSourceChanged;
        core.NavigationCompleted += OnNavigationCompleted;
        core.NewWindowRequested += OnNewWindowRequested;
        core.DocumentTitleChanged += OnDocumentTitleChanged;
        core.StatusBarTextChanged += OnStatusBarTextChanged;

        while (_queued.Count > 0)
        {
            _queued.Dequeue()();
        }
    }

    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        IsBusy = false;
        var url = Url ?? new Uri("about:blank");

        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(100, 100));

        // The snapshot lands before DocumentCompleted is announced, so Document is usable in the handler.
        _ = RefreshThenAnnounceAsync(url, e.IsSuccess, e.WebErrorStatus.ToString());
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        IsBusy = true;
        ReadyState = WebBrowserReadyState.Loading;
        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(0, 100));

        if (!Uri.TryCreate(e.Uri, UriKind.Absolute, out var url))
        {
            return;
        }

        var args = new BackendNavigatingEventArgs(url, null);
        Navigating?.Invoke(this, args);
        e.Cancel = args.Cancel;
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        Uri.TryCreate(e.Uri, UriKind.Absolute, out var url);

        var args = new BackendNewWindowEventArgs(url);
        NewWindow?.Invoke(this, args);

        // Handled either way: this backend hosts exactly one browser, so a second window would have
        // nowhere to go. The event still runs so an application can host another WebBrowser itself.
        e.Handled = true;
    }

    private void OnRealized(object? sender, global::System.EventArgs e)
    {
        if (_initializing || _host.IsReady)
        {
            return;
        }

        _initializing = true;
        _ = _host.InitializeAsync();
    }

    private void OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        ReadyState = WebBrowserReadyState.Loaded;
        var url = Url;
        if (url != null)
        {
            Navigated?.Invoke(this, new BackendNavigatedEventArgs(url));
        }
    }

    private void OnStatusBarTextChanged(object? sender, object e)
    {
        var text = _host.Core?.StatusBarText ?? string.Empty;
        if (string.Equals(_statusText, text, StringComparison.Ordinal))
        {
            return;
        }

        _statusText = text;
        StatusChanged?.Invoke(this, global::System.EventArgs.Empty);
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        string payload;
        try
        {
            payload = e.TryGetWebMessageAsString();
        }
        catch (ArgumentException)
        {
            // A message that is not a string: not ours.
            return;
        }

        var domEvent = ReadDomEvent(payload);
        if (domEvent != null)
        {
            _bridge.RaiseDomEvent(domEvent);
        }
    }

    /// <summary>
    /// Queues work until the browser is ready, then runs it. A navigation issued from a form's
    /// constructor arrives long before initialisation completes, and dropping it is the failure mode
    /// this exists to prevent.
    /// </summary>
    private void Post(Action action)
    {
        if (_host.IsReady)
        {
            action();
            return;
        }

        _queued.Enqueue(action);
    }

    /// <summary>
    /// Unwraps what <c>ExecuteScriptAsync</c> answers with.
    /// </summary>
    /// <param name="json">The result, which WebView2 always encodes as JSON.</param>
    /// <returns>
    /// The value in the CLR shapes the WebKitGTK backend produces, so that code above the seam cannot
    /// tell the two engines apart: <see cref="T:System.Double" /> for a number,
    /// <see cref="T:System.Boolean" /> for a boolean, <see cref="T:System.String" /> for a string.
    /// </returns>
    private static object? FromJsonResult(string? json)
    {
        if (string.IsNullOrEmpty(json) || string.Equals(json, "null", StringComparison.Ordinal))
        {
            return null;
        }

        return DomJson.Parse(json!);
    }

    /// <summary>
    /// Reads a DOM event posted by the injected bridge script.
    /// </summary>
    /// <param name="payload">The message body.</param>
    /// <returns>The event, or <c>null</c> when the message did not come from the bridge.</returns>
    private static DomEventArgs? ReadDomEvent(string? payload)
    {
        if (string.IsNullOrEmpty(payload) || !(DomJson.Parse(payload!) is Dictionary<string, object?> message))
        {
            return null;
        }

        if (!message.ContainsKey("type"))
        {
            return null;
        }

        return new DomEventArgs(DomJson.Int(message, "handle", DomNode.NoHandle), DomJson.String(message, "type"))
        {
            AltKeyPressed = DomJson.Bool(message, "alt"),
            CtrlKeyPressed = DomJson.Bool(message, "ctrl"),
            ShiftKeyPressed = DomJson.Bool(message, "shift"),
            KeyPressedCode = DomJson.Int(message, "keyCode", 0),
            Buttons = ToMouseButtons(DomJson.Int(message, "buttons", 0)),
            MousePosition = new Point(DomJson.Int(message, "screenX", 0), DomJson.Int(message, "screenY", 0)),
            ClientMousePosition = new Point(DomJson.Int(message, "clientX", 0), DomJson.Int(message, "clientY", 0)),
            OffsetMousePosition = new Point(DomJson.Int(message, "offsetX", 0), DomJson.Int(message, "offsetY", 0)),
            FromHandle = DomJson.Int(message, "from", DomNode.NoHandle),
            ToHandle = DomJson.Int(message, "to", DomNode.NoHandle)
        };
    }

    private static MouseButtons ToMouseButtons(int buttons)
    {
        // The DOM's MouseEvent.buttons bit field, not its button index.
        var result = MouseButtons.None;
        if ((buttons & 1) != 0)
        {
            result |= MouseButtons.Left;
        }

        if ((buttons & 2) != 0)
        {
            result |= MouseButtons.Right;
        }

        if ((buttons & 4) != 0)
        {
            result |= MouseButtons.Middle;
        }

        if ((buttons & 8) != 0)
        {
            result |= MouseButtons.XButton1;
        }

        if ((buttons & 16) != 0)
        {
            result |= MouseButtons.XButton2;
        }

        return result;
    }

    private async Task RefreshThenAnnounceAsync(Uri url, bool succeeded, string error)
    {
        try
        {
            await _bridge.RefreshAsync(CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception exception) when (exception is InvalidOperationException or ObjectDisposedException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "Could not pull a DOM snapshot after the navigation completed.", exception);
        }

        ReadyState = succeeded ? WebBrowserReadyState.Complete : WebBrowserReadyState.Uninitialized;
        LoadFinished?.Invoke(this, new BackendLoadEventArgs(url, succeeded, succeeded ? null : error));
    }
}

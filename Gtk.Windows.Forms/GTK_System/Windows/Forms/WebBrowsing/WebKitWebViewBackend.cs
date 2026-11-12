// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// The DOM bridge over a WebKitGTK view: reads and writes go through the injected
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomBridgeScript" />.
/// </summary>
internal sealed class WebKitHtmlDomBridge : IHtmlDomBridge
{
    private readonly WebKitWebViewBackend _backend;

    internal WebKitHtmlDomBridge(WebKitWebViewBackend backend)
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

        // Fire-and-forget: Post is called from synchronous WinForms setters, which must not block on the
        // engine's content process.
        _ = _backend.InvokeScriptAsync($"{DomBridgeScript.Global}.mutate({request})", CancellationToken.None);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var json = await _backend.InvokeScriptAsync($"{DomBridgeScript.Global}.snapshot()", cancellationToken)
            .ConfigureAwait(false);

        Replace(DomSnapshot.FromJson(json as string));
    }

    /// <summary>
    /// Replaces the snapshot and announces it.
    /// </summary>
    /// <param name="snapshot">
    /// The new snapshot.
    /// </param>
    internal void Replace(DomSnapshot snapshot)
    {
        Snapshot = snapshot;
        SnapshotUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Raises
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebKitHtmlDomBridge.DomEvent" /> for an event the
    /// page posted.
    /// </summary>
    /// <param name="e">
    /// The event.
    /// </param>
    internal void RaiseDomEvent(DomEventArgs e)
    {
        DomEvent?.Invoke(this, e);
    }

    /// <inheritdoc />
    public event EventHandler<DomEventArgs>? DomEvent;

    /// <inheritdoc />
    public event EventHandler? SnapshotUpdated;
}

/// <summary>
/// The WebKitGTK 6.0 backend: the engine behind
/// <see cref="T:System.Windows.Forms.WebBrowser" /> on Linux and macOS.
/// </summary>
/// <remarks>
/// Registered with
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebViewBackendFactory" /> below the Windows backend, so
/// that where both could load the platform-native one wins. Its probe fails - and the factory falls
/// through - when
/// <c>
/// libwebkitgtk-6.0
/// </c> is not installed, which is the normal case on Windows and common on macOS.
/// </remarks>
public sealed class WebKitWebViewBackend : IWebViewBackend
{
    private readonly WebKitHtmlDomBridge _bridge;

    private readonly WebKit.WebView _view;

    private bool _disposed;

    private string _statusText = string.Empty;

    private string _title = string.Empty;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebKitWebViewBackend" /> class.
    /// </summary>
    /// <exception cref="T:System.DllNotFoundException">
    /// The WebKitGTK library is not installed. Callers reach this type through
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebViewBackendFactory.Create" />, which treats that
    /// as "try the next engine".
    /// </exception>
    public WebKitWebViewBackend()
    {
        _view = new WebKit.WebView();
        _bridge = new WebKitHtmlDomBridge(this);

        InstallBridgeScript();

        _view.LoadChanged += OnLoadChanged;
        _view.LoadFailed += OnLoadFailed;
        _view.DecidePolicy += OnDecidePolicy;
        _view.MouseTargetChanged += OnMouseTargetChanged;
        _view.Create += OnCreate;
        _view.ContextMenu += OnContextMenu;
        _view.AddNotification("title", OnTitleNotified);
        _view.AddNotification("estimated-load-progress", OnProgressNotified);
    }

    /// <inheritdoc />
    public bool AllowDrop
    {
        get;
        set;
    } = true;

    /// <inheritdoc />
    public bool CanGoBack => _view.CanGoBack();

    /// <inheritdoc />
    public bool CanGoForward => _view.CanGoForward();

    /// <inheritdoc />
    public bool ContextMenuEnabled
    {
        get;
        set;
    } = true;

    /// <inheritdoc />
    public string DocumentTitle => _view.Title ?? string.Empty;

    /// <inheritdoc />
    public IHtmlDomBridge Dom => _bridge;

    /// <inheritdoc />
    /// <remarks>
    /// WebKitGTK reports a certificate and a set of validation flags, not a cipher strength, so the
    /// Internet-Explorer-era tiers this enumeration names cannot be reproduced. Plain HTTP is
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Insecure" />, TLS with a clean
    /// certificate is
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Bit128" />, and TLS with any flag set
    /// is
    /// <see cref="F:System.Windows.Forms.WebBrowserEncryptionLevel.Mixed" />.
    /// </remarks>
    public WebBrowserEncryptionLevel EncryptionLevel
    {
        get
        {
            try
            {
                if (!_view.GetTlsInfo(out _, out var errors))
                {
                    return WebBrowserEncryptionLevel.Insecure;
                }

                return errors == GLib.TlsCertificateFlags.NoFlags
                    ? WebBrowserEncryptionLevel.Bit128
                    : WebBrowserEncryptionLevel.Mixed;
            }
            catch (GLib.GException exception)
            {
                GtkLogger.Debug("Could not read the TLS state of the loaded document.", exception);
                return WebBrowserEncryptionLevel.Unknown;
            }
        }
    }

    /// <inheritdoc />
    public bool IsBusy => _view.IsLoading;

    /// <summary>
    /// Gets a value indicating whether a real engine is behind this backend.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// true
    /// </c>: construction fails when WebKitGTK is missing, so an instance implies an engine.
    /// </value>
    public bool IsFunctional => true;

    /// <inheritdoc />
    public int Progress => (int)Math.Round(_view.EstimatedLoadProgress * 100, MidpointRounding.AwayFromZero);

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
    /// <remarks>
    /// Neither engine has a setting for this, so it is applied as injected CSS - which is what a browser's
    /// own "hide scrollbars" preference does too.
    /// </remarks>
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
        get;
        set;
    } = true;

    /// <inheritdoc />
    public string StatusText => _statusText;

    /// <inheritdoc />
    public Uri? Url =>
        Uri.TryCreate(_view.Uri, UriKind.Absolute, out var url) ? url : null;

    /// <inheritdoc />
    public Gtk.Widget Widget => _view;

    private bool _scrollBarsEnabled = true;

    /// <summary>
    /// Registers this backend with
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebViewBackendFactory" />.
    /// </summary>
    /// <remarks>
    /// Called from the factory's own type initializer rather than from a module initializer, which
    /// <c>
    /// netstandard2.0
    /// </c> does not have.
    /// </remarks>
    internal static void Register()
    {
        WebViewBackendFactory.Register("WebKitGTK", 50, TryCreate);
    }

    /// <summary>
    /// Probes for WebKitGTK.
    /// </summary>
    /// <returns>
    /// A backend, or
    /// <c>
    /// null
    /// </c> when the library is not installed.
    /// </returns>
    internal static IWebViewBackend? TryCreate()
    {
        try
        {
            return new WebKitWebViewBackend();
        }
        catch (DllNotFoundException exception)
        {
            GtkLogger.Debug("WebKitGTK 6.0 is not installed.", exception);
            return null;
        }
        catch (EntryPointNotFoundException exception)
        {
            // An installed but older WebKitGTK: the library loads and the entry points this binding needs
            // are absent. Same outcome as absent, so fall through rather than fault the control.
            GtkLogger.Debug("The installed WebKitGTK is older than 6.0.", exception);
            return null;
        }
        catch (TypeInitializationException exception)
        {
            GtkLogger.Debug("The WebKitGTK binding could not initialize.", exception);
            return null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _view.LoadChanged -= OnLoadChanged;
        _view.LoadFailed -= OnLoadFailed;
        _view.DecidePolicy -= OnDecidePolicy;
        _view.MouseTargetChanged -= OnMouseTargetChanged;
        _view.Create -= OnCreate;
        _view.ContextMenu -= OnContextMenu;
        _view.RemoveNotification("title", OnTitleNotified);
        _view.RemoveNotification("estimated-load-progress", OnProgressNotified);

        _view.Dispose();
    }

    /// <inheritdoc />
    public void GoBack()
    {
        _view.GoBack();
    }

    /// <inheritdoc />
    public void GoForward()
    {
        _view.GoForward();
    }

    /// <inheritdoc />
    public Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<object?>();

        if (_disposed)
        {
            completion.SetResult(null);
            return completion.Task;
        }

        var registration = cancellationToken.CanBeCanceled
            ? cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken))
            : default;

        _view.EvaluateJavascript(script, null, null, null, (_, result, _) =>
        {
            try
            {
                completion.TrySetResult(ToClrValue(_view.EvaluateJavascriptFinish(result)));
            }
            catch (GLib.GException exception)
            {
                // A script error is the page's problem, not the host's: report it as no result rather
                // than faulting a task nobody may be awaiting.
                if (!ScriptErrorsSuppressed)
                {
                    GtkLogger.Debug("A script evaluated in the page failed.", exception);
                }

                completion.TrySetResult(null);
            }
            finally
            {
                registration.Dispose();
            }
        });

        return completion.Task;
    }

    /// <inheritdoc />
    public void LoadHtml(string html, Uri? baseUri)
    {
        _view.LoadHtml(html ?? string.Empty, baseUri?.ToString());
    }

    /// <inheritdoc />
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (postData == null && string.IsNullOrEmpty(additionalHeaders))
        {
            _view.LoadUri(url.ToString());
            return;
        }

        var request = new WebKit.URIRequest(url.ToString());
        if (postData != null)
        {
            // WebKitGTK 6.0's URIRequest carries no body, so a POST is expressed as a self-submitting
            // form. Doing nothing, or silently issuing a GET, would send the application's data nowhere.
            _view.LoadHtml(BuildPostForm(url, postData), url.ToString());
            return;
        }

        _view.LoadRequest(request);
    }

    /// <inheritdoc />
    public void Reload(bool bypassCache)
    {
        if (bypassCache)
        {
            _view.ReloadBypassCache();
        }
        else
        {
            _view.Reload();
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        _view.StopLoading();
    }

    /// <inheritdoc />
    public event EventHandler<BackendLoadEventArgs>? LoadFinished;

    /// <inheritdoc />
    public event EventHandler<BackendNavigatedEventArgs>? Navigated;

    /// <inheritdoc />
    public event EventHandler<BackendNavigatingEventArgs>? Navigating;

    /// <inheritdoc />
    public event EventHandler<BackendNewWindowEventArgs>? NewWindow;

    /// <inheritdoc />
    public event EventHandler<BackendProgressEventArgs>? ProgressChanged;

    /// <inheritdoc />
    public event EventHandler? StatusChanged;

    /// <inheritdoc />
    public event EventHandler? TitleChanged;

    private static string BuildPostForm(Uri url, byte[] postData)
    {
        var body = Encoding.UTF8.GetString(postData);
        var fields = new StringBuilder();

        foreach (var pair in body.Split('&'))
        {
            if (pair.Length == 0)
            {
                continue;
            }

            var separator = pair.IndexOf('=');
            var name = separator < 0 ? pair : pair.Substring(0, separator);
            var value = separator < 0 ? string.Empty : pair.Substring(separator + 1);

            fields.AppendFormat(CultureInfo.InvariantCulture,
                "<input type=\"hidden\" name=\"{0}\" value=\"{1}\">",
                HtmlAttributeEncode(Uri.UnescapeDataString(name)),
                HtmlAttributeEncode(Uri.UnescapeDataString(value)));
        }

        return string.Format(CultureInfo.InvariantCulture,
            "<!doctype html><html><body onload=\"document.forms[0].submit()\">" +
            "<form method=\"post\" action=\"{0}\">{1}</form></body></html>",
            HtmlAttributeEncode(url.ToString()), fields);
    }

    private static string HtmlAttributeEncode(string value)
    {
        return value.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
    }

    private static object? ToClrValue(JavaScriptCore.Value? value)
    {
        if (value == null || value.IsNull || value.IsUndefined)
        {
            return null;
        }

        if (value.IsBoolean)
        {
            return value.ToBoolean();
        }

        if (value.IsNumber)
        {
            return value.ToDouble();
        }

        return value.ToString();
    }

    private void InstallBridgeScript()
    {
        var manager = _view.UserContentManager;
        if (manager == null)
        {
            GtkLogger.Debug("The WebKit view has no user content manager; the DOM bridge is unavailable.");
            return;
        }

        manager.AddScript(new WebKit.UserScript(DomBridgeScript.Source,
            WebKit.UserContentInjectedFrames.AllFrames, WebKit.UserScriptInjectionTime.Start, null, null));

        manager.RegisterScriptMessageHandler(DomBridgeScript.EventChannel, null);
        manager.ScriptMessageReceived += OnScriptMessageReceived;
    }

    private void OnContextMenu(object o, WebKit.ContextMenuArgs args)
    {
        // The signal is "return true to suppress the menu", expressed here as RetVal.
        args.RetVal = !ContextMenuEnabled;
    }

    private void OnCreate(object o, WebKit.CreateArgs args)
    {
        Uri? url = null;
        try
        {
            var requested = args.NavigationAction?.Request?.Uri;
            if (requested != null)
            {
                Uri.TryCreate(requested, UriKind.Absolute, out url);
            }
        }
        catch (GLib.GException exception)
        {
            GtkLogger.Debug("Could not read the URL of a requested new window.", exception);
        }

        var e = new BackendNewWindowEventArgs(url);
        NewWindow?.Invoke(this, e);

        // Returning no widget refuses the window either way: this backend hosts exactly one view. The
        // event still runs so that an application can open a second WebBrowser itself.
        args.RetVal = null;
    }

    private void OnDecidePolicy(object o, WebKit.DecidePolicyArgs args)
    {
        if (args.DecisionType != WebKit.PolicyDecisionType.NavigationAction)
        {
            args.RetVal = false;
            return;
        }

        if (!(args.Decision is WebKit.NavigationPolicyDecision decision))
        {
            args.RetVal = false;
            return;
        }

        var action = decision.NavigationAction;
        if (action == null || !Uri.TryCreate(action.Request?.Uri, UriKind.Absolute, out var url))
        {
            args.RetVal = false;
            return;
        }

        var e = new BackendNavigatingEventArgs(url, action.FrameName);
        Navigating?.Invoke(this, e);

        if (e.Cancel)
        {
            decision.Ignore();
            args.RetVal = true;
            return;
        }

        args.RetVal = false;
    }

    private void OnLoadChanged(object o, WebKit.LoadChangedArgs args)
    {
        switch (args.LoadEvent)
        {
            case WebKit.LoadEvent.Started:
                ReadyState = WebBrowserReadyState.Loading;
                break;
            case WebKit.LoadEvent.Committed:
                ReadyState = WebBrowserReadyState.Loaded;
                if (Url != null)
                {
                    Navigated?.Invoke(this, new BackendNavigatedEventArgs(Url));
                }

                break;
            case WebKit.LoadEvent.Finished:
                ReadyState = WebBrowserReadyState.Interactive;
                OnLoadFinished();
                break;
        }
    }

    private void OnLoadFailed(object o, WebKit.LoadFailedArgs args)
    {
        ReadyState = WebBrowserReadyState.Uninitialized;

        var url = Uri.TryCreate(args.FailingUri, UriKind.Absolute, out var failing)
            ? failing
            : WebViewBackendFactory.BlankUri;

        _bridge.Replace(DomSnapshot.Empty);
        LoadFinished?.Invoke(this, new BackendLoadEventArgs(url, false, args.FailingUri));

        // false: let WebKit show its own error page, as a browser would.
        args.RetVal = false;
    }

    private async void OnLoadFinished()
    {
        var url = Url ?? WebViewBackendFactory.BlankUri;

        try
        {
            // The snapshot is pulled before DocumentCompleted is announced, so that Document is usable
            // inside the handler - which is the only place WinForms code can reasonably read it.
            await _bridge.RefreshAsync(CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            // An async void continuation: an escaping exception would take the process down, and a
            // snapshot that could not be pulled is a stale document, not a fatal error.
            GtkLogger.Debug("Could not pull a DOM snapshot after the load finished.", exception);
        }

        ReadyState = WebBrowserReadyState.Complete;
        LoadFinished?.Invoke(this, new BackendLoadEventArgs(url, true, null));
    }

    private void OnMouseTargetChanged(object o, WebKit.MouseTargetChangedArgs args)
    {
        string status;
        try
        {
            var hit = args.HitTestResult;
            status = hit != null && hit.ContextIsLink() ? hit.LinkUri ?? string.Empty : string.Empty;
        }
        catch (GLib.GException exception)
        {
            GtkLogger.Debug("Could not read the hit test result under the pointer.", exception);
            status = string.Empty;
        }

        if (string.Equals(_statusText, status, StringComparison.Ordinal))
        {
            return;
        }

        _statusText = status;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnProgressNotified(object o, GLib.NotifyArgs args)
    {
        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(Progress, 100));
    }

    private void OnScriptMessageReceived(object o, WebKit.ScriptMessageReceivedArgs args)
    {
        var payload = args.Value?.ToString();
        if (string.IsNullOrEmpty(payload))
        {
            return;
        }

        if (!(DomJson.Parse(payload!) is Dictionary<string, object?> message))
        {
            return;
        }

        var e = new DomEventArgs(DomJson.Int(message, "handle", DomNode.NoHandle),
            DomJson.String(message, "type"))
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

        _bridge.RaiseDomEvent(e);
    }

    private void OnTitleNotified(object o, GLib.NotifyArgs args)
    {
        var title = _view.Title ?? string.Empty;
        if (string.Equals(_title, title, StringComparison.Ordinal))
        {
            return;
        }

        _title = title;
        TitleChanged?.Invoke(this, EventArgs.Empty);
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
}

/// <summary>
/// Writes CLR values as JavaScript literals for the snippets the bridge evaluates.
/// </summary>
/// <remarks>
/// Quoting rather than concatenating is the whole point: an attribute value or an element's inner text
/// is arbitrary user content, and pasting it unescaped into an evaluated script is a script injection
/// against the host's own bridge.
/// </remarks>
internal static class JavaScriptLiteral
{
    /// <summary>
    /// Writes an argument list as a JavaScript array literal.
    /// </summary>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <returns>
    /// The literal.
    /// </returns>
    internal static string Array(object[]? values)
    {
        if (values == null || values.Length == 0)
        {
            return "[]";
        }

        return "[" + string.Join(", ", values.Select(Value).ToArray()) + "]";
    }

    /// <summary>
    /// Writes a string as a quoted, escaped JavaScript string literal.
    /// </summary>
    /// <param name="value">
    /// The string, which may be
    /// <c>
    /// null
    /// </c>.
    /// </param>
    /// <returns>
    /// The literal, or
    /// <c>
    /// null
    /// </c> spelled as JavaScript spells it.
    /// </returns>
    internal static string Quote(string? value)
    {
        if (value == null)
        {
            return "null";
        }

        var builder = new StringBuilder(value.Length + 2);
        builder.Append('"');

        foreach (var character in value)
        {
            switch (character)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    // U+2028 and U+2029 are ordinary characters in a C# string and line terminators
                    // in a JavaScript one, so pasting them through unescaped ends the literal.
                    if (character < ' ' || character == '\u2028' || character == '\u2029')
                    {
                        builder.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:X4}", (int)character);
                    }
                    else
                    {
                        builder.Append(character);
                    }

                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }

    /// <summary>
    /// Writes one value as a JavaScript literal.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The literal. Values the bridge cannot marshal are written as their invariant string form, which
    /// is what a script bridge can carry.
    /// </returns>
    internal static string Value(object? value)
    {
        switch (value)
        {
            case null:
                return "null";
            case bool flag:
                return flag ? "true" : "false";
            case string text:
                return Quote(text);
            case sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal:
                return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null";
            default:
                return Quote(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }
}

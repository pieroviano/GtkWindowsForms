// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// The DOM bridge behind
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.NullWebViewBackend" />: there is no page, so the
/// snapshot is permanently empty and every mutation is dropped.
/// </summary>
internal sealed class NullHtmlDomBridge : IHtmlDomBridge
{
    /// <summary>
    /// Gets the empty snapshot.
    /// </summary>
    /// <value>
    /// Always
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.DomSnapshot.Empty" />.
    /// </value>
    public DomSnapshot Snapshot => DomSnapshot.Empty;

    /// <summary>
    /// Returns
    /// <c>
    /// null
    /// </c> without invoking anything.
    /// </summary>
    /// <param name="nodeHandle">
    /// Ignored.
    /// </param>
    /// <param name="member">
    /// Ignored.
    /// </param>
    /// <param name="args">
    /// Ignored.
    /// </param>
    /// <param name="cancellationToken">
    /// Ignored.
    /// </param>
    /// <returns>
    /// A completed task carrying
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    public Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<object?>(null);
    }

    /// <summary>
    /// Drops the mutation: there is no page to apply it to.
    /// </summary>
    /// <param name="mutation">
    /// Ignored.
    /// </param>
    public void Post(DomMutation mutation)
    {
    }

    /// <summary>
    /// Completes immediately without replacing the snapshot.
    /// </summary>
    /// <param name="cancellationToken">
    /// Ignored.
    /// </param>
    /// <returns>
    /// A completed task.
    /// </returns>
    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    /// <summary>
    /// Never raised.
    /// </summary>
    public event EventHandler<DomEventArgs>? DomEvent;

    /// <summary>
    /// Never raised.
    /// </summary>
    public event EventHandler? SnapshotUpdated;
}

/// <summary>
/// The placeholder backend: what
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.WebViewBackendFactory" /> chooses when no web engine
/// could be loaded.
/// </summary>
/// <remarks>
/// It exists so that a
/// <see cref="T:System.Windows.Forms.WebBrowser" /> on a machine without WebKitGTK or the WebView2
/// runtime still constructs, still lays out, and still drives the navigation events an application's
/// state machine is written against - it simply reports every navigation as failed rather than throwing
/// out of the constructor. The control shows a localised message naming what to install.
/// </remarks>
public sealed class NullWebViewBackend : IWebViewBackend
{
    private readonly Gtk.Label _label;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.NullWebViewBackend" /> class.
    /// </summary>
    public NullWebViewBackend()
    {
        _label = new Gtk.Label(System.Properties.Resources.Gtk_WebBrowser_Placeholder_No_web_engine_available)
        {
            Wrap = true,
            Justify = Gtk.Justification.Center,
            Halign = Gtk.Align.Center,
            Valign = Gtk.Align.Center,
            Hexpand = true,
            Vexpand = true
        };
        _label.StyleContext.AddClass("WebBrowserPlaceholder");
        Dom = new NullHtmlDomBridge();
    }

    /// <inheritdoc />
    public bool AllowDrop
    {
        get;
        set;
    }

    /// <inheritdoc />
    public bool CanGoBack => false;

    /// <inheritdoc />
    public bool CanGoForward => false;

    /// <inheritdoc />
    public bool ContextMenuEnabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public string DocumentTitle => string.Empty;

    /// <inheritdoc />
    public IHtmlDomBridge Dom
    {
        get;
    }

    /// <inheritdoc />
    public WebBrowserEncryptionLevel EncryptionLevel => WebBrowserEncryptionLevel.Insecure;

    /// <inheritdoc />
    public bool IsBusy => false;

    /// <summary>
    /// Gets a value indicating whether a real engine is behind this backend.
    /// </summary>
    /// <value>
    /// Always
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsFunctional => false;

    /// <inheritdoc />
    public int Progress => 0;

    /// <inheritdoc />
    public WebBrowserReadyState ReadyState => WebBrowserReadyState.Uninitialized;

    /// <inheritdoc />
    public bool ScriptErrorsSuppressed
    {
        get;
        set;
    }

    /// <inheritdoc />
    public bool ScrollBarsEnabled
    {
        get;
        set;
    } = true;

    /// <inheritdoc />
    public bool ShortcutsEnabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public string StatusText => string.Empty;

    /// <inheritdoc />
    public Uri? Url
    {
        get;
        private set;
    }

    /// <inheritdoc />
    public Gtk.Widget Widget => _label;

    /// <summary>
    /// Releases the placeholder widget.
    /// </summary>
    public void Dispose()
    {
        _label.Dispose();
    }

    /// <inheritdoc />
    public void GoBack()
    {
    }

    /// <inheritdoc />
    public void GoForward()
    {
    }

    /// <inheritdoc />
    public Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken)
    {
        return Task.FromResult<object?>(null);
    }

    /// <inheritdoc />
    public void LoadHtml(string html, Uri? baseUri)
    {
        Fail(baseUri ?? WebViewBackendFactory.BlankUri);
    }

    /// <inheritdoc />
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        var navigating = new BackendNavigatingEventArgs(url, targetFrameName);
        Navigating?.Invoke(this, navigating);
        if (navigating.Cancel)
        {
            return;
        }

        Url = url;
        Fail(url);
    }

    /// <inheritdoc />
    public void Reload(bool bypassCache)
    {
        if (Url != null)
        {
            Fail(Url);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
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

    /// <summary>
    /// Reports a load that could never start. Raised synchronously so that an application driving a
    /// state machine off the navigation events is not left waiting for an event that will never come.
    /// </summary>
    /// <param name="url">
    /// What was asked for.
    /// </param>
    private void Fail(Uri url)
    {
        LoadFinished?.Invoke(this,
            new BackendLoadEventArgs(url, false, System.Properties.Resources.Gtk_WebBrowser_Placeholder_No_web_engine_available));
    }
}

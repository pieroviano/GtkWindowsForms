using System.Windows.Forms;
using GtkSystem.Windows.Forms.WebBrowsing;

namespace GtkWinForms.Tests.Mocks;

/// <summary>
/// A DOM bridge whose snapshot the test sets directly, recording every mutation and member invocation so
/// a test can assert that a WinForms setter posted exactly one change rather than several or none.
/// </summary>
public sealed class FakeHtmlDomBridge : IHtmlDomBridge
{
    /// <summary>The mutations posted through <see cref="Post"/>, in order.</summary>
    public List<DomMutation> Mutations { get; } = new();

    /// <summary>The member invocations requested, as (handle, member, args) triples.</summary>
    public List<(int Handle, string Member, object[] Args)> Invocations { get; } = new();

    /// <summary>How many times <see cref="RefreshAsync"/> was called.</summary>
    public int RefreshCount { get; private set; }

    /// <inheritdoc />
    public DomSnapshot Snapshot { get; set; } = new();

    /// <inheritdoc />
    public Task<object?> InvokeMemberAsync(int nodeHandle, string member, object[] args,
        CancellationToken cancellationToken)
    {
        Invocations.Add((nodeHandle, member, args));
        return Task.FromResult<object?>(null);
    }

    /// <inheritdoc />
    public void Post(DomMutation mutation) => Mutations.Add(mutation);

    /// <inheritdoc />
    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        RefreshCount++;
        RaiseSnapshotUpdated();
        return Task.CompletedTask;
    }

    /// <summary>Raises <see cref="DomEvent"/>, standing in for an event delegated from the page.</summary>
    public void RaiseDomEvent(DomEventArgs e) => DomEvent?.Invoke(this, e);

    /// <summary>Raises <see cref="SnapshotUpdated"/>.</summary>
    public void RaiseSnapshotUpdated() => SnapshotUpdated?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public event EventHandler<DomEventArgs>? DomEvent;

    /// <inheritdoc />
    public event EventHandler? SnapshotUpdated;
}

/// <summary>
/// A backend the test drives by hand: navigation commands are recorded rather than performed, and the
/// engine events are raised on demand. It exists so the WebBrowser control's own behaviour - event order,
/// cancellation, state change notifications - can be asserted without a web engine being installed.
/// </summary>
public sealed class FakeWebViewBackend : IWebViewBackend
{
    private readonly Gtk.Label _widget;

    /// <summary>Creates the fake. GTK must already be initialised: the fake owns a real widget, because
    /// the host WebViewBase parents whatever the backend hands it.</summary>
    public FakeWebViewBackend()
    {
        _widget = new Gtk.Label("fake");
        Dom = FakeDom;
    }

    /// <summary>The bridge behind <see cref="Dom"/>, typed so tests can drive it.</summary>
    public FakeHtmlDomBridge FakeDom { get; } = new();

    /// <summary>The navigations requested, in order.</summary>
    public List<Uri> Navigations { get; } = new();

    /// <summary>The markup passed to <see cref="LoadHtml"/>, in order.</summary>
    public List<string> LoadedHtml { get; } = new();

    /// <summary>How many times <see cref="Reload"/> was called, and with what.</summary>
    public List<bool> Reloads { get; } = new();

    /// <summary>How many times <see cref="Stop"/> was called.</summary>
    public int StopCount { get; private set; }

    /// <summary>How many times <see cref="GoBack"/> was called.</summary>
    public int BackCount { get; private set; }

    /// <summary>How many times <see cref="GoForward"/> was called.</summary>
    public int ForwardCount { get; private set; }

    /// <summary>Whether <see cref="Dispose"/> has been called.</summary>
    public bool Disposed { get; private set; }

    /// <inheritdoc />
    public bool AllowDrop { get; set; }

    /// <inheritdoc />
    public bool CanGoBack { get; set; }

    /// <inheritdoc />
    public bool CanGoForward { get; set; }

    /// <inheritdoc />
    public bool ContextMenuEnabled { get; set; }

    /// <inheritdoc />
    public string DocumentTitle { get; set; } = string.Empty;

    /// <inheritdoc />
    public IHtmlDomBridge Dom { get; }

    /// <inheritdoc />
    public WebBrowserEncryptionLevel EncryptionLevel { get; set; } = WebBrowserEncryptionLevel.Insecure;

    /// <inheritdoc />
    public bool IsBusy { get; set; }

    /// <inheritdoc />
    public bool IsFunctional => true;

    /// <inheritdoc />
    public int Progress { get; set; }

    /// <inheritdoc />
    public WebBrowserReadyState ReadyState { get; set; } = WebBrowserReadyState.Uninitialized;

    /// <inheritdoc />
    public bool ScriptErrorsSuppressed { get; set; }

    /// <inheritdoc />
    public bool ScrollBarsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool ShortcutsEnabled { get; set; } = true;

    /// <inheritdoc />
    public string StatusText { get; set; } = string.Empty;

    /// <inheritdoc />
    public Uri? Url { get; set; }

    /// <inheritdoc />
    public Gtk.Widget Widget => _widget;

    /// <inheritdoc />
    public void Dispose()
    {
        Disposed = true;
        _widget.Dispose();
    }

    /// <inheritdoc />
    public void GoBack() => BackCount++;

    /// <inheritdoc />
    public void GoForward() => ForwardCount++;

    /// <inheritdoc />
    public Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken)
    {
        Scripts.Add(script);
        return Task.FromResult<object?>(null);
    }

    /// <summary>The scripts passed to <see cref="InvokeScriptAsync"/>, in order.</summary>
    public List<string> Scripts { get; } = new();

    /// <inheritdoc />
    public void LoadHtml(string html, Uri? baseUri) => LoadedHtml.Add(html);

    /// <inheritdoc />
    public void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders)
    {
        var navigating = new BackendNavigatingEventArgs(url, targetFrameName);
        Navigating?.Invoke(this, navigating);
        if (navigating.Cancel)
        {
            return;
        }

        Navigations.Add(url);
        Url = url;
    }

    /// <inheritdoc />
    public void Reload(bool bypassCache) => Reloads.Add(bypassCache);

    /// <inheritdoc />
    public void Stop() => StopCount++;

    /// <summary>Completes the last navigation the way a real engine would: commit, then finish.</summary>
    public void CompleteNavigation(Uri url, bool succeeded = true)
    {
        Url = url;
        Navigated?.Invoke(this, new BackendNavigatedEventArgs(url));
        LoadFinished?.Invoke(this, new BackendLoadEventArgs(url, succeeded, succeeded ? null : "failed"));
    }

    /// <summary>Raises <see cref="ProgressChanged"/>.</summary>
    public void RaiseProgress(long progress, long maximum) =>
        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(progress, maximum));

    /// <summary>Raises <see cref="TitleChanged"/> after setting the title.</summary>
    public void RaiseTitleChanged(string title)
    {
        DocumentTitle = title;
        TitleChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raises <see cref="StatusChanged"/> after setting the status text.</summary>
    public void RaiseStatusChanged(string statusText)
    {
        StatusText = statusText;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raises <see cref="NewWindow"/> and reports whether it was refused.</summary>
    public bool RaiseNewWindow(Uri? url)
    {
        var args = new BackendNewWindowEventArgs(url);
        NewWindow?.Invoke(this, args);
        return args.Cancel;
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
}

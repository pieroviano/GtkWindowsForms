using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gtk.Windows.Forms.InternetExplorer.Interop;
using GtkSystem.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>Turns the control's <c>DWebBrowserEvents2</c> notifications into the backend's seam events.</summary>
public sealed partial class InternetExplorerBackend
{
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

    private void OnHostReady(object? sender, global::System.EventArgs e)
    {
        var events = _host.Events;
        if (events == null)
        {
            return;
        }

        events.Navigating += OnNavigating;
        events.Navigated += OnNavigated;
        events.DocumentCompleted += OnDocumentCompleted;
        events.ProgressChanged += OnProgress;
        events.TitleChanged += OnTitleChanged;
        events.StatusChanged += OnStatusChanged;
        events.CommandStateChanged += OnCommandStateChanged;
        events.NewWindow += OnNewWindow;

        ApplySilent(_scriptErrorsSuppressed);

        while (_queued.Count > 0)
        {
            _queued.Dequeue()();
        }
    }

    private void OnNavigating(string url, string frame, ref bool cancel)
    {
        IsBusyState = true;
        ReadyStateValue = WebBrowserReadyState.Loading;
        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(0, 100));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var parsed))
        {
            return;
        }

        var args = new BackendNavigatingEventArgs(parsed, string.IsNullOrEmpty(frame) ? null : frame);
        Navigating?.Invoke(this, args);
        cancel = args.Cancel;
    }

    private void OnNavigated(string url)
    {
        ReadyStateValue = WebBrowserReadyState.Loaded;
        if (Uri.TryCreate(url, UriKind.Absolute, out var parsed))
        {
            Navigated?.Invoke(this, new BackendNavigatedEventArgs(parsed));
        }
    }

    private void OnDocumentCompleted(string url)
    {
        IsBusyState = false;
        var parsed = Uri.TryCreate(url, UriKind.Absolute, out var u) ? u : new Uri("about:blank");

        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(100, 100));

        // Inject the DOM bridge into the fresh document, flush any LoadHtml markup, then pull a snapshot so
        // Document is usable inside the DocumentCompleted handler, as the other backends guarantee.
        _bridge.Channel.Inject();
        FlushPendingHtml();
        _ = RefreshThenAnnounceAsync(parsed);
    }

    private void OnProgress(int progress, int progressMax)
    {
        ReadyStateValue = WebBrowserReadyState.Interactive;
        if (progress < 0 || progressMax <= 0)
        {
            ProgressState = 100;
        }
        else
        {
            ProgressState = (int)Math.Min(100L, progress * 100L / progressMax);
        }

        ProgressChanged?.Invoke(this, new BackendProgressEventArgs(
            progress < 0 ? progressMax : progress, progressMax <= 0 ? 100 : progressMax));
    }

    private void OnTitleChanged(string text)
    {
        if (string.Equals(TitleValue, text, StringComparison.Ordinal))
        {
            return;
        }

        TitleValue = text;
        TitleChanged?.Invoke(this, global::System.EventArgs.Empty);
    }

    private void OnStatusChanged(string text)
    {
        if (string.Equals(StatusTextValue, text, StringComparison.Ordinal))
        {
            return;
        }

        StatusTextValue = text;
        StatusChanged?.Invoke(this, global::System.EventArgs.Empty);
    }

    private void OnCommandStateChanged(int command, bool enable)
    {
        if (command == CommandStateChangeCommand.UpdateBack)
        {
            CanGoBackState = enable;
        }
        else if (command == CommandStateChangeCommand.UpdateForward)
        {
            CanGoForwardState = enable;
        }
    }

    private void OnNewWindow(ref bool cancel)
    {
        var args = new BackendNewWindowEventArgs(null);
        NewWindow?.Invoke(this, args);

        // This backend hosts exactly one browser, so a popup has nowhere to go; refuse it either way. The
        // event still runs so an application can host another WebBrowser itself.
        cancel = true;
    }

    private async Task RefreshThenAnnounceAsync(Uri url)
    {
        try
        {
            await _bridge.RefreshAsync(global::System.Threading.CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception exception) when (exception is InvalidOperationException or COMException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "Could not pull a DOM snapshot after the navigation completed.", exception);
        }

        ReadyStateValue = WebBrowserReadyState.Complete;
        LoadFinished?.Invoke(this, new BackendLoadEventArgs(url, true, null));
    }
}

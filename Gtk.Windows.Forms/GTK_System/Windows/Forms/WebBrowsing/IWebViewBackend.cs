// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.WebBrowsing;

/// <summary>
/// Provides data for
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.Navigating" />. Cancelling stops the
/// navigation before it commits.
/// </summary>
public sealed class BackendNavigatingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendNavigatingEventArgs" /> class.
    /// </summary>
    /// <param name="url">
    /// Where the engine is about to navigate.
    /// </param>
    /// <param name="targetFrameName">
    /// The frame being navigated, or
    /// <c>
    /// null
    /// </c> for the main frame.
    /// </param>
    public BackendNavigatingEventArgs(Uri url, string? targetFrameName)
    {
        Url = url;
        TargetFrameName = targetFrameName;
    }

    /// <summary>
    /// Gets the frame being navigated.
    /// </summary>
    /// <value>
    /// The frame name, or
    /// <c>
    /// null
    /// </c> for the main frame.
    /// </value>
    public string? TargetFrameName
    {
        get;
    }

    /// <summary>
    /// Gets where the engine is about to navigate.
    /// </summary>
    /// <value>
    /// The target URL.
    /// </value>
    public Uri Url
    {
        get;
    }
}

/// <summary>
/// Provides data for
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.Navigated" />, raised once a navigation
/// has committed and the engine is showing the new document.
/// </summary>
public sealed class BackendNavigatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendNavigatedEventArgs" /> class.
    /// </summary>
    /// <param name="url">
    /// Where the engine navigated.
    /// </param>
    public BackendNavigatedEventArgs(Uri url)
    {
        Url = url;
    }

    /// <summary>
    /// Gets where the engine navigated.
    /// </summary>
    /// <value>
    /// The committed URL.
    /// </value>
    public Uri Url
    {
        get;
    }
}

/// <summary>
/// Provides data for
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.ProgressChanged" />.
/// </summary>
public sealed class BackendProgressEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendProgressEventArgs" /> class.
    /// </summary>
    /// <param name="progress">
    /// How much of the document has loaded, from 0 to
    /// <paramref name="maximumProgress" />.
    /// </param>
    /// <param name="maximumProgress">
    /// The value
    /// <paramref name="progress" /> reaches at completion.
    /// </param>
    public BackendProgressEventArgs(long progress, long maximumProgress)
    {
        Progress = progress;
        MaximumProgress = maximumProgress;
    }

    /// <summary>
    /// Gets the value
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendProgressEventArgs.Progress" /> reaches at
    /// completion.
    /// </summary>
    /// <value>
    /// The maximum progress.
    /// </value>
    public long MaximumProgress
    {
        get;
    }

    /// <summary>
    /// Gets how much of the document has loaded.
    /// </summary>
    /// <value>
    /// The current progress.
    /// </value>
    public long Progress
    {
        get;
    }
}

/// <summary>
/// Provides data for
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.LoadFinished" />, raised whether the
/// load succeeded or failed.
/// </summary>
public sealed class BackendLoadEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendLoadEventArgs" /> class.
    /// </summary>
    /// <param name="url">
    /// The document that finished loading.
    /// </param>
    /// <param name="succeeded">
    /// Whether the load succeeded.
    /// </param>
    /// <param name="errorMessage">
    /// Why the load failed, when it did.
    /// </param>
    public BackendLoadEventArgs(Uri url, bool succeeded, string? errorMessage)
    {
        Url = url;
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets why the load failed.
    /// </summary>
    /// <value>
    /// The failure message, or
    /// <c>
    /// null
    /// </c> when the load succeeded.
    /// </value>
    public string? ErrorMessage
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether the load succeeded.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the document loaded.
    /// </value>
    public bool Succeeded
    {
        get;
    }

    /// <summary>
    /// Gets the document that finished loading.
    /// </summary>
    /// <value>
    /// The URL.
    /// </value>
    public Uri Url
    {
        get;
    }
}

/// <summary>
/// Provides data for
/// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.NewWindow" />. Cancelling refuses the
/// new window; the backend must then not open one.
/// </summary>
public sealed class BackendNewWindowEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.BackendNewWindowEventArgs" /> class.
    /// </summary>
    /// <param name="url">
    /// What the page asked to open, when it named anything.
    /// </param>
    public BackendNewWindowEventArgs(Uri? url)
    {
        Url = url;
    }

    /// <summary>
    /// Gets what the page asked to open.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c> when the page named none.
    /// </value>
    public Uri? Url
    {
        get;
    }
}

/// <summary>
/// The engine seam behind
/// <see cref="T:System.Windows.Forms.WebBrowser" />. Replaces mono's
/// <c>
/// Mono.WebBrowser.IWebBrowser
/// </c>, whose only implementation was a Mozilla/XPCOM backend against
/// <c>
/// libgluezilla
/// </c> - a native shim that has not existed for over a decade.
/// </summary>
/// <remarks>
/// Every member is non-blocking. Commands are posted and state is read from the last snapshot, because
/// both supported engines run the page out of process and a synchronous WinForms getter must never spin
/// a nested main loop waiting for it.
/// </remarks>
public interface IWebViewBackend : IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether the page may accept dropped content.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when drops are allowed.
    /// </value>
    bool AllowDrop
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the engine can navigate back.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when there is a previous entry.
    /// </value>
    bool CanGoBack
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether the engine can navigate forward.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when there is a following entry.
    /// </value>
    bool CanGoForward
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the engine shows its own context menu.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when the engine's context menu is enabled.
    /// </value>
    bool ContextMenuEnabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the JavaScript-synthesised DOM the
    /// <c>
    /// Html*
    /// </c> types read.
    /// </summary>
    /// <value>
    /// The DOM bridge.
    /// </value>
    IHtmlDomBridge Dom
    {
        get;
    }

    /// <summary>
    /// Gets the title of the document currently loaded.
    /// </summary>
    /// <value>
    /// The document title, or the empty string.
    /// </value>
    string DocumentTitle
    {
        get;
    }

    /// <summary>
    /// Gets how strongly the current document's transport is encrypted.
    /// </summary>
    /// <value>
    /// The encryption level.
    /// </value>
    WebBrowserEncryptionLevel EncryptionLevel
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether a real engine is behind this backend.
    /// </summary>
    /// <value>
    /// <c>
    /// false
    /// </c> for the placeholder backend that is chosen when no engine could be loaded.
    /// </value>
    bool IsFunctional
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether a load is in progress.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> while the engine is loading.
    /// </value>
    bool IsBusy
    {
        get;
    }

    /// <summary>
    /// Gets how far the current load has got, from 0 to 100.
    /// </summary>
    /// <value>
    /// The load progress.
    /// </value>
    int Progress
    {
        get;
    }

    /// <summary>
    /// Gets how far the current document has been parsed.
    /// </summary>
    /// <value>
    /// The ready state.
    /// </value>
    WebBrowserReadyState ReadyState
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the engine reports script errors.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when script errors are swallowed.
    /// </value>
    bool ScriptErrorsSuppressed
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page may show scroll bars.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when scroll bars are allowed.
    /// </value>
    bool ScrollBarsEnabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the engine handles its own keyboard shortcuts.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> when shortcuts are enabled.
    /// </value>
    bool ShortcutsEnabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the text the engine last offered for a status bar - typically the URL under the pointer.
    /// </summary>
    /// <value>
    /// The status text, or the empty string.
    /// </value>
    string StatusText
    {
        get;
    }

    /// <summary>
    /// Gets the URL of the document currently loaded.
    /// </summary>
    /// <value>
    /// The URL, or
    /// <c>
    /// null
    /// </c> when nothing has loaded.
    /// </value>
    Uri? Url
    {
        get;
    }

    /// <summary>
    /// Gets the GTK widget that occupies the control's area.
    /// </summary>
    /// <value>
    /// The engine's widget.
    /// </value>
    Gtk.Widget Widget
    {
        get;
    }

    /// <summary>
    /// Navigates back one history entry.
    /// </summary>
    void GoBack();

    /// <summary>
    /// Navigates forward one history entry.
    /// </summary>
    void GoForward();

    /// <summary>
    /// Evaluates a script in the page.
    /// </summary>
    /// <param name="script">
    /// The script source.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the wait for the result, not the evaluation itself.
    /// </param>
    /// <returns>
    /// The value the script produced, or
    /// <c>
    /// null
    /// </c>.
    /// </returns>
    Task<object?> InvokeScriptAsync(string script, CancellationToken cancellationToken);

    /// <summary>
    /// Loads markup directly, without a network request.
    /// </summary>
    /// <param name="html">
    /// The document markup.
    /// </param>
    /// <param name="baseUri">
    /// The base against which the document's relative URLs resolve.
    /// </param>
    void LoadHtml(string html, Uri? baseUri);

    /// <summary>
    /// Starts a navigation.
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
    /// <param name="postData">
    /// The request body, which makes the request a
    /// <c>
    /// POST
    /// </c>.
    /// </param>
    /// <param name="additionalHeaders">
    /// Extra request headers, CRLF-separated.
    /// </param>
    void Navigate(Uri url, string? targetFrameName, byte[]? postData, string? additionalHeaders);

    /// <summary>
    /// Reloads the current document.
    /// </summary>
    /// <param name="bypassCache">
    /// <c>
    /// true
    /// </c> to re-fetch everything rather than revalidate.
    /// </param>
    void Reload(bool bypassCache);

    /// <summary>
    /// Stops the current load.
    /// </summary>
    void Stop();

    /// <summary>
    /// Occurs when a load finishes, whether it succeeded or failed.
    /// </summary>
    event EventHandler<BackendLoadEventArgs> LoadFinished;

    /// <summary>
    /// Occurs once a navigation has committed.
    /// </summary>
    event EventHandler<BackendNavigatedEventArgs> Navigated;

    /// <summary>
    /// Occurs before a navigation starts. Cancelling stops it.
    /// </summary>
    event EventHandler<BackendNavigatingEventArgs> Navigating;

    /// <summary>
    /// Occurs when the page asks to open a window. Cancelling refuses it.
    /// </summary>
    event EventHandler<BackendNewWindowEventArgs> NewWindow;

    /// <summary>
    /// Occurs as the current load advances.
    /// </summary>
    event EventHandler<BackendProgressEventArgs> ProgressChanged;

    /// <summary>
    /// Occurs when
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.StatusText" /> changes.
    /// </summary>
    event EventHandler StatusChanged;

    /// <summary>
    /// Occurs when
    /// <see cref="GtkSystem.Windows.Forms.WebBrowsing.IWebViewBackend.DocumentTitle" /> changes.
    /// </summary>
    event EventHandler TitleChanged;
}

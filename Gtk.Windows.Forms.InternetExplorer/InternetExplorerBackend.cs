using System;
using System.Collections.Generic;
using System.Threading;
using GtkSystem.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// The Internet Explorer (Trident) backend behind <see cref="T:System.Windows.Forms.WebBrowser" /> on
/// Windows, hosting the <c>SHDocVw.WebBrowser</c> ActiveX control.
/// </summary>
/// <remarks>
/// Registers below the WebView2 backend, so WebView2 wins on Windows where both can load and this is the
/// fallback before WebKitGTK. Its probe declines off Windows, when the GTK runtime exposes no win32 surface
/// handle, or when the hosting thread is not a single-threaded apartment - all cases where the ActiveX
/// control cannot be hosted. Initialisation is synchronous once the widget is realised; commands issued
/// before then are queued and replayed, so navigating from a form's constructor is not lost.
///
/// Internet Explorer is end of life. This exists for legacy intranet content that still requires Trident;
/// new work should use the WebView2 backend.
/// </remarks>
public sealed partial class InternetExplorerBackend : IWebViewBackend
{
    private readonly IeHtmlDomBridge _bridge;

    private readonly global::Gtk.DrawingArea _placeholder;

    private readonly Queue<global::System.Action> _queued = new Queue<global::System.Action>();

    private readonly AxWebBrowserHost _host;

    private readonly ComDispatcher _com;

    private bool _disposed;

    private bool _initializing;

    /// <summary>Initializes a new instance of the <see cref="InternetExplorerBackend" /> class, hosting the
    /// control directly on the calling (STA) thread.</summary>
    public InternetExplorerBackend()
        : this(null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="InternetExplorerBackend" /> class.</summary>
    /// <param name="apartment">
    /// The dedicated STA apartment to host the control on when the GTK thread is not itself an STA, or
    /// <c>null</c> to host directly on the GTK thread (the normal case).
    /// </param>
    internal InternetExplorerBackend(StaApartment? apartment)
    {
        // GTK still needs a widget to lay out and to give the control its area, even though the browser
        // paints into a child HWND above it rather than into this widget.
        _placeholder = new global::Gtk.DrawingArea
        {
            Hexpand = true,
            Vexpand = true
        };

        _com = new ComDispatcher(apartment);
        _bridge = new IeHtmlDomBridge(this, _com);
        _host = new AxWebBrowserHost(_placeholder, _com);
        _host.Ready += OnHostReady;
        _placeholder.Realized += OnRealized;
    }

    /// <inheritdoc />
    public IHtmlDomBridge Dom => _bridge;

    /// <summary>Gets a value indicating whether a real engine is behind this backend.</summary>
    /// <value>Always <c>true</c>.</value>
    public bool IsFunctional => true;

    /// <inheritdoc />
    public global::Gtk.Widget Widget => _placeholder;

    /// <summary>Gets how many commands are waiting for the control to finish initialising.</summary>
    /// <value>The queue depth. Exposed for tests: a navigation issued before the control is ready is held
    /// rather than dropped, which has no other observable effect.</value>
    internal int PendingCommandCount => _queued.Count;

    /// <summary>The control's automation object, driven through late binding, or <c>null</c> before ready.</summary>
    internal dynamic? Browser => _host.Browser;

    /// <summary>
    /// Probes for a usable Trident host.
    /// </summary>
    /// <returns>A backend, or <c>null</c> when Windows, GTK or the apartment cannot provide one.</returns>
    public static IWebViewBackend? TryCreate()
    {
        if (!GdkWin32Interop.IsSupported)
        {
            // Not Windows, or a GTK 4 runtime with no win32 backend: there is no HWND to parent to.
            return null;
        }

        if (!TridentProbe.IsControlAvailable)
        {
            return null;
        }

        if (TridentProbe.IsStaThread())
        {
            // The common case: the GTK loop runs on the [STAThread] Main thread, so host directly on it.
            return new InternetExplorerBackend();
        }

        // Fallback for an MTA host: give the control its own STA apartment to live on.
        StaApartment? apartment;
        try
        {
            apartment = new StaApartment();
        }
        catch (Exception exception) when (exception is OutOfMemoryException or InvalidOperationException)
        {
            return null;
        }

        return new InternetExplorerBackend(apartment);
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
        _com.Dispose();
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task<object?> InvokeScriptAsync(string script,
        CancellationToken cancellationToken)
    {
        return await _bridge.Channel.EvaluateAsync(script, cancellationToken).ConfigureAwait(false);
    }

    private void OnRealized(object? sender, global::System.EventArgs e)
    {
        if (_initializing || _host.IsReady)
        {
            return;
        }

        _initializing = true;
        _host.Initialize();
    }

    /// <summary>
    /// Queues work until the control is ready, then runs it. A navigation issued from a form's constructor
    /// arrives long before initialisation completes, and dropping it is the failure this prevents.
    /// </summary>
    private void Post(global::System.Action action)
    {
        if (_host.IsReady)
        {
            action();
            return;
        }

        _queued.Enqueue(action);
    }
}

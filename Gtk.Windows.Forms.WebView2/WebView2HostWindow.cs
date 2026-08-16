using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// Owns the WebView2 controller and keeps it over the area a GTK widget occupies.
/// </summary>
/// <remarks>
/// The controller is a child window of the GTK toplevel's HWND, because GTK 4 gives only toplevels a
/// window handle. That has a consequence worth stating plainly: the browser paints above every GTK
/// widget in the same window. Overlapping widgets, popovers, menus and window opacity do not clip it.
/// Recorded as a Windows-only deviation in EMULATION-GAPS.md.
/// </remarks>
internal sealed class WebView2HostWindow : IDisposable
{
    /// <summary>
    /// How often the geometry is re-read, in milliseconds.
    /// </summary>
    /// <remarks>
    /// A tick rather than a set of signal handlers. GTK 4 removed the <c>size-allocate</c> signal - a
    /// widget learns its allocation by overriding a vfunc, which a plain <see cref="global::Gtk.Widget" />
    /// obtained from a binding cannot do - and the positions that matter also move for reasons with no
    /// signal at all: an ancestor scrolling, the toplevel moving between monitors of different scale.
    /// One periodic recomputation covers every case, and it applies nothing when nothing changed.
    /// </remarks>
    private const uint GeometryTickMilliseconds = 33;

    private readonly global::Gtk.Widget _anchor;

    private Rectangle _appliedBounds;

    private bool _appliedVisible;

    private CoreWebView2Controller? _controller;

    private bool _disposed;

    private uint _tick;

    internal WebView2HostWindow(global::Gtk.Widget anchor)
    {
        _anchor = anchor;
    }

    /// <summary>
    /// Gets the underlying browser, once it has finished initialising.
    /// </summary>
    /// <value>The browser, or <c>null</c> before <see cref="Ready" /> is raised.</value>
    internal CoreWebView2? Core => _controller?.CoreWebView2;

    /// <summary>
    /// Gets a value indicating whether the browser is ready to be driven.
    /// </summary>
    /// <value><c>true</c> once initialisation has completed.</value>
    internal bool IsReady => _controller?.CoreWebView2 != null;

    /// <summary>
    /// Occurs once the browser is ready. Navigation requests made before this point are queued by the
    /// backend and replayed here: a <c>Navigate</c> that silently vanishes because the control was not
    /// ready yet is the failure this exists to prevent.
    /// </summary>
    internal event global::System.EventHandler? Ready;

    /// <summary>
    /// Occurs when the browser asks to hand keyboard focus back, so Tab does not trap inside the page.
    /// </summary>
    internal event global::System.EventHandler<bool>? MoveFocusRequested;

    /// <summary>
    /// Releases the controller and stops the geometry tick.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_tick != 0)
        {
            GLib.Source.Remove(_tick);
            _tick = 0;
        }

        if (_controller != null)
        {
            _controller.MoveFocusRequested -= OnMoveFocusRequested;
            _controller.Close();
            _controller = null;
        }
    }

    /// <summary>
    /// Gives the browser keyboard focus.
    /// </summary>
    internal void Focus()
    {
        try
        {
            _controller?.MoveFocus(CoreWebView2MoveFocusReason.Programmatic);
        }
        catch (InvalidOperationException)
        {
            // The controller was closed between the check and the call. Focus is not worth faulting over.
        }
    }

    /// <summary>
    /// Creates the controller once the widget is in a realised toplevel.
    /// </summary>
    /// <returns>A task that completes when the browser is ready, or immediately when it cannot be.</returns>
    internal async Task InitializeAsync()
    {
        var parent = GdkWin32Interop.GetToplevelHandle(_anchor);
        if (parent == IntPtr.Zero)
        {
            return;
        }

        try
        {
            var environment = await CoreWebView2Environment.CreateAsync().ConfigureAwait(true);
            var controller = await environment.CreateCoreWebView2ControllerAsync(parent).ConfigureAwait(true);

            if (_disposed)
            {
                controller.Close();
                return;
            }

            _controller = controller;
            _controller.IsVisible = false;
            _controller.MoveFocusRequested += OnMoveFocusRequested;

            StartGeometryTick();
            UpdateGeometry();

            Ready?.Invoke(this, global::System.EventArgs.Empty);
        }
        catch (WebView2RuntimeNotFoundException exception)
        {
            global::System.Diagnostics.GtkLogger.Debug("The WebView2 runtime is not installed.", exception);
        }
        catch (Exception exception) when (exception is DllNotFoundException or BadImageFormatException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "WebView2Loader.dll is missing or of the wrong architecture.", exception);
        }
    }

    /// <summary>
    /// Re-reads the widget's geometry and applies it, if anything moved.
    /// </summary>
    internal void UpdateGeometry()
    {
        if (_controller == null || _disposed)
        {
            return;
        }

        var allocation = _anchor.Allocation;
        var mapped = _anchor.IsMapped;
        var scale = _anchor.ScaleFactor;

        var origin = OriginInToplevel();
        var bounds = WebView2Bounds.Compute(origin.X, origin.Y, allocation.Width, allocation.Height, scale);
        var visible = WebView2Bounds.IsVisible(mapped, allocation.Width, allocation.Height);

        if (bounds != _appliedBounds)
        {
            _appliedBounds = bounds;
            try
            {
                _controller.Bounds = bounds;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        if (visible == _appliedVisible)
        {
            return;
        }

        _appliedVisible = visible;
        try
        {
            _controller.IsVisible = visible;
        }
        catch (InvalidOperationException)
        {
            // Closed underneath us; the next tick will find _controller null.
        }
    }

    private void OnMoveFocusRequested(object? sender, CoreWebView2MoveFocusRequestedEventArgs e)
    {
        // Handled, then passed on: the host moves GTK focus to the next or previous widget, so Tab leaves
        // the page instead of cycling inside it forever.
        e.Handled = true;
        MoveFocusRequested?.Invoke(this, e.Reason == CoreWebView2MoveFocusReason.Previous);
    }

    private Point OriginInToplevel()
    {
        if (!(_anchor.Root is global::Gtk.Widget root))
        {
            return Point.Empty;
        }

        try
        {
            var source = new Graphene.Point { X = 0, Y = 0 };
            if (_anchor.ComputePoint(root, source, out var target))
            {
                return new Point((int)Math.Round(target.X), (int)Math.Round(target.Y));
            }
        }
        catch (Exception exception) when (exception is NullReferenceException or InvalidOperationException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "Could not translate the WebBrowser's position into its toplevel.", exception);
        }

        return Point.Empty;
    }

    private void StartGeometryTick()
    {
        if (_tick != 0)
        {
            return;
        }

        _tick = GLib.Timeout.Add(GeometryTickMilliseconds, () =>
        {
            if (_disposed)
            {
                return false;
            }

            UpdateGeometry();
            return true;
        });
    }
}

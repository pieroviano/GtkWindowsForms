using System;
using System.Drawing;
using Gtk.Windows.Forms.InternetExplorer.Interop;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Owns the WebBrowser ActiveX control and keeps it over the area a GTK widget occupies.
/// </summary>
/// <remarks>
/// The control is in-place activated into a bare child window of the GTK toplevel's HWND, because GTK 4
/// gives only toplevels a window handle. As with the WebView2 backend, the browser therefore paints above
/// every GTK widget in the same window; overlapping widgets, popovers and menus do not clip it. Recorded as
/// a Windows-only deviation in EMULATION-GAPS.md.
///
/// Everything here runs on the GTK main-loop thread, which the application enters through <c>[STAThread]</c>
/// <c>Main</c> - the single-threaded apartment OLE in-place activation requires.
/// </remarks>
internal sealed class AxWebBrowserHost : IDisposable
{
    private const uint GeometryTickMilliseconds = 33;

    private readonly global::Gtk.Widget _anchor;

    private readonly ComDispatcher _com;

    private Rectangle _appliedBounds;

    private bool _appliedVisible;

    private IntPtr _childHwnd;

    private IConnectionPoint? _connectionPoint;

    private int _cookie;

    private bool _disposed;

    private IOleInPlaceObject? _inPlaceObject;

    private bool _oleInitialized;

    private IOleObject? _oleObject;

    private OleHostSite? _site;

    private uint _tick;

    internal AxWebBrowserHost(global::Gtk.Widget anchor, ComDispatcher com)
    {
        _anchor = anchor;
        _com = com;
    }

    /// <summary>Gets the control's automation object (<c>IWebBrowser2</c>), driven through late binding.</summary>
    /// <value>The browser, or <c>null</c> before <see cref="Ready" /> is raised.</value>
    internal object? Browser { get; private set; }

    /// <summary>Gets the event sink, once the control has been advised.</summary>
    internal WebBrowserEventsSink? Events { get; private set; }

    /// <summary>Gets a value indicating whether the control is ready to be driven.</summary>
    internal bool IsReady => Browser != null;

    /// <summary>Occurs once the control is ready. Commands issued earlier are queued by the backend and replayed.</summary>
    internal event global::System.EventHandler? Ready;

    /// <summary>
    /// Instantiates and activates the control once the widget is in a realised toplevel. A no-op if that has
    /// not happened yet (the realize handler calls again) or if it has already run.
    /// </summary>
    internal void Initialize()
    {
        if (_disposed || IsReady)
        {
            return;
        }

        // GTK is read on the GTK thread; the OLE work (which must run on the control's apartment) is marshalled.
        var frame = GdkWin32Interop.GetToplevelHandle(_anchor);
        if (frame == IntPtr.Zero)
        {
            return;
        }

        BrowserEmulation.EnsureApplied();

        _com.Send(() =>
        {
            if (OleNativeMethods.OleInitialize(IntPtr.Zero) >= 0)
            {
                _oleInitialized = true;
            }

            _childHwnd = OleNativeMethods.CreateWindowExW(
                0, "STATIC", null,
                OleNativeMethods.WS_CHILD | OleNativeMethods.WS_CLIPCHILDREN | OleNativeMethods.WS_CLIPSIBLINGS,
                0, 0, 0, 0, frame, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (_childHwnd == IntPtr.Zero)
            {
                return;
            }

            if (AxActivation.TryActivate(this, _childHwnd, frame, CurrentPositionRect,
                    out var browser, out var oleObject, out var inPlaceObject, out var site,
                    out var sink, out var connectionPoint, out var cookie))
            {
                Browser = browser;
                _oleObject = oleObject;
                _inPlaceObject = inPlaceObject;
                _site = site;
                Events = sink;
                _connectionPoint = connectionPoint;
                _cookie = cookie;
            }
        });

        if (Browser == null)
        {
            return;
        }

        StartGeometryTick();
        UpdateGeometry();
        Ready?.Invoke(this, global::System.EventArgs.Empty);
    }

    /// <summary>Re-reads the widget's geometry and applies it to the child window and the control.</summary>
    internal void UpdateGeometry()
    {
        if (_disposed || Browser == null)
        {
            return;
        }

        // Read the GTK geometry on the GTK thread, then apply it to the window and control on their thread.
        var allocation = _anchor.Allocation;
        var scale = _anchor.ScaleFactor;
        var origin = OriginInToplevel();
        var bounds = IeBounds.Compute(origin.X, origin.Y, allocation.Width, allocation.Height, scale);
        var visible = IeBounds.IsVisible(_anchor.IsMapped, allocation.Width, allocation.Height);

        if (bounds == _appliedBounds && visible == _appliedVisible)
        {
            return;
        }

        var boundsChanged = bounds != _appliedBounds;
        _appliedBounds = bounds;
        _appliedVisible = visible;

        _com.Send(() =>
        {
            if (boundsChanged)
            {
                OleNativeMethods.SetWindowPos(_childHwnd, IntPtr.Zero, bounds.X, bounds.Y, bounds.Width,
                    bounds.Height,
                    OleNativeMethods.SWP_NOZORDER | OleNativeMethods.SWP_NOACTIVATE | OleNativeMethods.SWP_NOOWNERZORDER);
                var rect = new RECT(0, 0, bounds.Width, bounds.Height);
                try
                {
                    _inPlaceObject?.SetObjectRects(ref rect, ref rect);
                }
                catch (Exception exception) when (exception is System.Runtime.InteropServices.COMException)
                {
                    // The control was torn down between the check and the call; the next tick finds it gone.
                }
            }

            OleNativeMethods.ShowWindow(_childHwnd, visible ? OleNativeMethods.SW_SHOW : OleNativeMethods.SW_HIDE);
        });
    }

    /// <summary>The control's position rectangle in its container window - it fills the child window.</summary>
    internal RECT CurrentPositionRect() => new RECT(0, 0, _appliedBounds.Width, _appliedBounds.Height);

    /// <inheritdoc />
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

        _com.Send(() =>
        {
            AxActivation.Teardown(_oleObject, _inPlaceObject, _connectionPoint, _cookie);
            Browser = null;
            _oleObject = null;
            _inPlaceObject = null;
            _connectionPoint = null;
            _site = null;
            Events = null;

            if (_childHwnd != IntPtr.Zero)
            {
                OleNativeMethods.DestroyWindow(_childHwnd);
                _childHwnd = IntPtr.Zero;
            }

            if (_oleInitialized)
            {
                OleNativeMethods.OleUninitialize();
                _oleInitialized = false;
            }
        });
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

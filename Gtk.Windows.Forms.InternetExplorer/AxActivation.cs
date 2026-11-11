using System;
using System.Runtime.InteropServices;
using Gtk.Windows.Forms.InternetExplorer.Interop;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// The OLE in-place activation handshake that brings the WebBrowser control up inside the host's child
/// window, and the matching teardown. Separated from <see cref="AxWebBrowserHost" /> so the host keeps to
/// its lifecycle and geometry role.
/// </summary>
internal static class AxActivation
{
    /// <summary>
    /// Instantiates the WebBrowser control, sites it in <paramref name="childHwnd" />, in-place activates
    /// it and advises the event sink.
    /// </summary>
    /// <returns><c>true</c> when the control is up and advised; <c>false</c> on any COM failure (the caller
    /// then leaves the backend unready and the factory's guard reports the engine as unavailable).</returns>
    internal static bool TryActivate(AxWebBrowserHost owner, IntPtr childHwnd, IntPtr frameHwnd,
        Func<RECT> positionRect, out object? browser, out IOleObject? oleObject,
        out IOleInPlaceObject? inPlaceObject, out OleHostSite? site, out WebBrowserEventsSink? sink,
        out IConnectionPoint? connectionPoint, out int cookie)
    {
        browser = null;
        oleObject = null;
        inPlaceObject = null;
        site = null;
        sink = null;
        connectionPoint = null;
        cookie = 0;

        try
        {
            var clsid = OleGuids.WebBrowser;
            var iid = OleNativeMethods.IID_IUnknown;
            var hr = OleNativeMethods.CoCreateInstance(
                ref clsid, IntPtr.Zero, OleNativeMethods.CLSCTX_INPROC_SERVER, ref iid, out var control);
            if (hr < 0 || control == null)
            {
                return false;
            }

            oleObject = (IOleObject)control;
            inPlaceObject = (IOleInPlaceObject)control;
            site = new OleHostSite(() => childHwnd, () => frameHwnd, positionRect);

            OleNativeMethods.OleSetContainedObject(control, true);
            oleObject.SetClientSite(site);
            TrySetHostNames(oleObject);

            var rect = positionRect();
            oleObject.DoVerb(OleVerb.InPlaceActivate, IntPtr.Zero, site, 0, childHwnd, ref rect);

            var container = (IConnectionPointContainer)control;
            var events = OleGuids.DWebBrowserEvents2;
            container.FindConnectionPoint(ref events, out connectionPoint);
            sink = new WebBrowserEventsSink(control);
            connectionPoint.Advise(sink, out cookie);

            browser = control;
            return true;
        }
        catch (Exception exception) when (exception is COMException or InvalidCastException or NotSupportedException)
        {
            global::System.Diagnostics.GtkLogger.Debug(
                "The Internet Explorer WebBrowser control could not be hosted.", exception);
            return false;
        }
    }

    /// <summary>Unadvises the sink, deactivates and closes the control, and releases the COM references.</summary>
    internal static void Teardown(IOleObject? oleObject, IOleInPlaceObject? inPlaceObject,
        IConnectionPoint? connectionPoint, int cookie)
    {
        Guard(() =>
        {
            if (connectionPoint != null && cookie != 0)
            {
                connectionPoint.Unadvise(cookie);
            }
        });
        Guard(() => inPlaceObject?.InPlaceDeactivate());
        Guard(() => oleObject?.Close(OleClose.NoSave));
        Guard(() => oleObject?.SetClientSite(null!));

        Release(connectionPoint);
        Release(oleObject);
    }

    private static void Release(object? comObject)
    {
        if (comObject == null || !Marshal.IsComObject(comObject))
        {
            return;
        }

        try
        {
            Marshal.FinalReleaseComObject(comObject);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidComObjectException)
        {
            // Already released (several interface references wrap one control): nothing more to do.
        }
    }

    private static void TrySetHostNames(IOleObject oleObject)
    {
        try
        {
            oleObject.SetHostNames("Gtk.Windows.Forms", "WebBrowser");
        }
        catch (COMException)
        {
            // Optional cosmetic hint for the control; not worth failing activation over.
        }
    }

    private static void Guard(global::System.Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is COMException or InvalidComObjectException or NullReferenceException)
        {
            // Teardown is best effort: the process may be exiting and the control already gone.
        }
    }
}

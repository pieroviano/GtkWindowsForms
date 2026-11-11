using System;
using System.Threading;
using Microsoft.Win32;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// Answers whether this machine and thread can host the Internet Explorer WebBrowser ActiveX control.
/// </summary>
/// <remarks>
/// Two things have to hold: Trident must be installed (the control's CLSID is registered), and the thread
/// that will drive it must be a single-threaded apartment, which OLE in-place activation requires. The
/// application enters its GTK main loop through <c>[STAThread]</c> <c>Main</c>, so the probe runs on the
/// STA thread the control will live on.
/// </remarks>
internal static class TridentProbe
{
    /// <summary>
    /// Gets a value indicating whether Trident is installed, independent of the current thread's apartment.
    /// </summary>
    /// <value><c>true</c> when the WebBrowser control's CLSID is registered.</value>
    internal static bool IsControlAvailable => IsControlRegistered();

    /// <summary>
    /// Gets a value indicating whether the calling thread is a single-threaded apartment.
    /// </summary>
    internal static bool IsStaThread() => Thread.CurrentThread.GetApartmentState() == ApartmentState.STA;

    private static bool IsControlRegistered()
    {
        try
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(
                       @"CLSID\{8856F961-340A-11D0-A96B-00C04FD705A2}\InprocServer32"))
            {
                return key?.GetValue(null) is string server && server.Length > 0;
            }
        }
        catch (Exception exception) when (exception is System.Security.SecurityException or
            UnauthorizedAccessException or global::System.IO.IOException)
        {
            // Cannot read the registry: assume the control is not usable rather than fault the probe.
            return false;
        }
    }
}

using System;
using System.Runtime.InteropServices;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.WebView2;

/// <summary>
/// Reaches the window handle behind a GTK 4 toplevel on Windows.
/// </summary>
/// <remarks>
/// GTK 4 has no child windows: only a toplevel owns an HWND, and every widget inside it is drawn rather
/// than hosted. A WebView2 controller needs a parent HWND, so it is parented to the toplevel's and
/// positioned by hand over the area the control occupies. That is the airspace deviation recorded in
/// EMULATION-GAPS.md.
///
/// The entry point is resolved through the already-loaded GTK module rather than by
/// <see cref="T:System.Runtime.InteropServices.DllImportAttribute" />: the GTK 4 runtime ships under
/// different file names depending on how it was built - <c>gtk-4-1.dll</c> for the gvsbuild layout this
/// repository deploys, <c>libgtk-4-1.dll</c> for MSYS2 - and a wrong guess is a hard
/// <see cref="T:System.DllNotFoundException" /> rather than a fallback.
/// </remarks>
internal static class GdkWin32Interop
{
    private static readonly string[] CandidateModules =
    {
        "gtk-4-1.dll",
        "libgtk-4-1.dll",
        "libgtk-4-1.0.dll",
        "gtk-4.dll"
    };

    private static IntPtr _module = IntPtr.Zero;

    private static GetHandleDelegate? _getHandle;

    private static bool _resolved;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GetHandleDelegate(IntPtr surface);

    /// <summary>
    /// Gets a value indicating whether the GTK 4 runtime on this machine exports the entry point the
    /// WebView2 backend needs.
    /// </summary>
    /// <value>
    /// <c>true</c> when the handle can be read.
    /// </value>
    internal static bool IsSupported => Resolve() != null;

    /// <summary>
    /// Reads the window handle of the toplevel that contains a widget.
    /// </summary>
    /// <param name="widget">The widget.</param>
    /// <returns>
    /// The window handle, or <see cref="F:System.IntPtr.Zero" /> when the widget is not yet in a realised
    /// toplevel - which is the normal state before the form is shown, not an error.
    /// </returns>
    internal static IntPtr GetToplevelHandle(global::Gtk.Widget? widget)
    {
        var getHandle = Resolve();
        if (getHandle == null || widget == null)
        {
            return IntPtr.Zero;
        }

        if (!(widget.Root is global::Gtk.Window window))
        {
            return IntPtr.Zero;
        }

        var surface = window.Surface;
        return surface == null || surface.Handle == IntPtr.Zero
            ? IntPtr.Zero
            : getHandle(surface.Handle);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
    private static extern IntPtr GetModuleHandleA(string moduleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr module, string procedureName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
    private static extern IntPtr LoadLibraryA(string fileName);

    private static GetHandleDelegate? Resolve()
    {
        if (_resolved)
        {
            return _getHandle;
        }

        _resolved = true;

        if (!IsWindows())
        {
            return null;
        }

        foreach (var candidate in CandidateModules)
        {
            // Already loaded is the expected case: nothing gets this far without GTK running.
            _module = GetModuleHandleA(candidate);
            if (_module != IntPtr.Zero)
            {
                break;
            }
        }

        if (_module == IntPtr.Zero)
        {
            foreach (var candidate in CandidateModules)
            {
                _module = LoadLibraryA(candidate);
                if (_module != IntPtr.Zero)
                {
                    break;
                }
            }
        }

        if (_module == IntPtr.Zero)
        {
            return null;
        }

        var address = GetProcAddress(_module, "gdk_win32_surface_get_handle");
        if (address == IntPtr.Zero)
        {
            return null;
        }

        _getHandle = (GetHandleDelegate)Marshal.GetDelegateForFunctionPointer(address, typeof(GetHandleDelegate));
        return _getHandle;
    }

    private static bool IsWindows()
    {
#if NET462
        return true;
#else
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    }
}

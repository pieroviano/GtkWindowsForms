using System;
using System.Runtime.InteropServices;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// Positions a GTK4 top-level window at a centered location. Gtk4 removed the managed window-positioning API
/// (<c>gtk_window_move</c>), so this reaches the native window of the running backend and moves it directly:
/// Win32 <c>SetWindowPos</c> on Windows, <c>XMoveWindow</c> on X11. Wayland (the compositor owns placement)
/// and macOS are a deliberate no-op. Every native call is guarded; any failure degrades to "not moved" and
/// never throws, so a backend that cannot position simply leaves the window where the window manager put it.
/// </summary>
internal static class NativeWindowPlacement
{
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint MONITOR_DEFAULTTONEAREST = 2;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GdkWin32SurfaceGetHandle(IntPtr surface);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate ulong GdkX11SurfaceGetXid(IntPtr surface);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GdkX11DisplayGetXdisplay(IntPtr display);

    // Resolved from the already-loaded GTK4 library. Exactly one of the Win32/X11 pair is present at runtime,
    // which also serves as the backend probe. Null on Wayland/macOS/unknown -> centering is a no-op there.
    private static readonly IntPtr GtkLib = LoadGtk();
    private static readonly GdkWin32SurfaceGetHandle? Win32GetHandle =
        Resolve<GdkWin32SurfaceGetHandle>("gdk_win32_surface_get_handle");
    private static readonly GdkX11SurfaceGetXid? X11GetXid = Resolve<GdkX11SurfaceGetXid>("gdk_x11_surface_get_xid");
    private static readonly GdkX11DisplayGetXdisplay? X11GetXdisplay =
        Resolve<GdkX11DisplayGetXdisplay>("gdk_x11_display_get_xdisplay");

    /// <summary>Centers the form on the work area of the monitor it is on. Returns false if it could not be moved.</summary>
    public static bool CenterOnScreen(FormBase? form) => TryMove(form, null);

    /// <summary>Centers the form over its owner (falling back to the screen). Returns false if it could not be moved.</summary>
    public static bool CenterOverOwner(FormBase? form, FormBase? owner) => TryMove(form, owner);

    private static bool TryMove(FormBase? form, FormBase? owner)
    {
        try
        {
            if (FuncLoader.IsWindows)
            {
                var hwnd = WindowsHandleOf(form);
                if (hwnd == IntPtr.Zero)
                {
                    return false; // the native window does not exist yet
                }

                var ownerHwnd = WindowsHandleOf(owner);
                return ownerHwnd != IntPtr.Zero ? WinCenterOverOwner(hwnd, ownerHwnd) : WinCenterOnWorkArea(hwnd);
            }

            var surface = SurfaceOf(form);
            if (X11GetXid != null && X11GetXdisplay != null && surface != IntPtr.Zero)
            {
                var dpy = X11GetXdisplay(DisplayOf(form));
                if (dpy == IntPtr.Zero)
                {
                    return false;
                }

                var xid = X11GetXid(surface);
                if (xid == 0)
                {
                    return false;
                }

                var ownerSurface = SurfaceOf(owner);
                var ownerXid = ownerSurface != IntPtr.Zero ? X11GetXid(ownerSurface) : 0;
                return ownerXid != 0 ? X11CenterOverOwner(dpy, xid, ownerXid) : X11CenterOnScreen(dpy, xid);
            }

            if (FuncLoader.IsOSX)
            {
                return MacCenter(form);
            }

            return false; // Wayland / unknown backend: cannot set a toplevel position
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Native window centering failed", ex);
            return false;
        }
    }

    /// <summary>
    /// The Windows HWND of a form's toplevel. Prefers the exact handle from the GTK surface when the gtk-4
    /// library is reachable by name; otherwise falls back to the current process's top-level window whose
    /// caption matches the form title - which does not depend on the native library name/location at all.
    /// </summary>
    private static IntPtr WindowsHandleOf(FormBase? form)
    {
        if (form == null)
        {
            return IntPtr.Zero;
        }

        var surface = SurfaceOf(form);
        if (Win32GetHandle != null && surface != IntPtr.Zero)
        {
            var hwnd = Win32GetHandle(surface);
            if (hwnd != IntPtr.Zero)
            {
                return hwnd;
            }
        }

        return FindProcessWindowByTitle(TitleOf(form));
    }

    /// <summary>
    /// Sets (or clears) the Windows title-bar / taskbar icon of a form from raw .ico bytes. Gtk4 on Windows
    /// does not surface a WinForms form icon on the native window, so this applies it directly to the HWND via
    /// WM_SETICON (Windows decodes the .ico natively, unlike GdkPixbuf). Passing null bytes removes the icon,
    /// which is how a form left at the default shows none. No-op off Windows and never throws.
    /// </summary>
    public static bool SetWindowsIcon(FormBase? form, byte[]? icoBytes)
    {
        if (!FuncLoader.IsWindows)
        {
            return false;
        }

        try
        {
            var hwnd = WindowsHandleOf(form);
            if (hwnd == IntPtr.Zero)
            {
                return false;
            }

            IntPtr big = IntPtr.Zero;
            IntPtr small = IntPtr.Zero;
            if (icoBytes != null && icoBytes.Length > 0)
            {
                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                    "gwf_" + System.Guid.NewGuid().ToString("N") + ".ico");
                System.IO.File.WriteAllBytes(tempPath, icoBytes);
                try
                {
                    big = LoadImage(IntPtr.Zero, tempPath, IMAGE_ICON, 0, 0, LR_LOADFROMFILE | LR_DEFAULTSIZE);
                    small = LoadImage(IntPtr.Zero, tempPath, IMAGE_ICON, GetSystemMetrics(SM_CXSMICON),
                        GetSystemMetrics(SM_CYSMICON), LR_LOADFROMFILE);
                }
                finally
                {
                    try
                    {
                        System.IO.File.Delete(tempPath);
                    }
                    catch
                    {
                        // best effort
                    }
                }
            }

            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_BIG, big);
            SendMessage(hwnd, WM_SETICON, (IntPtr)ICON_SMALL, small != IntPtr.Zero ? small : big);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Native window icon apply failed", ex);
            return false;
        }
    }

    /// <summary>
    /// Test/diagnostic probe (Windows): the current rectangle of this process's window with the given title,
    /// and the work area of the monitor it is on. Returns false if no such window exists.
    /// </summary>
    internal static bool TryGetWindowAndWorkArea(string? title, out int windowX, out int windowY,
        out int windowWidth, out int windowHeight, out int areaX, out int areaY, out int areaWidth, out int areaHeight)
    {
        windowX = windowY = windowWidth = windowHeight = areaX = areaY = areaWidth = areaHeight = 0;
        if (!FuncLoader.IsWindows)
        {
            return false;
        }

        var hwnd = FindProcessWindowByTitle(title);
        if (hwnd == IntPtr.Zero || !GetWindowRect(hwnd, out var rect))
        {
            return false;
        }

        windowX = rect.Left;
        windowY = rect.Top;
        windowWidth = rect.Right - rect.Left;
        windowHeight = rect.Bottom - rect.Top;
        var work = WorkArea(hwnd);
        areaX = work.Left;
        areaY = work.Top;
        areaWidth = work.Right - work.Left;
        areaHeight = work.Bottom - work.Top;
        return true;
    }

    private static IntPtr FindProcessWindowByTitle(string? title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return IntPtr.Zero;
        }

        var processId = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
        var match = IntPtr.Zero;
        EnumWindows((hWnd, _) =>
        {
            GetWindowThreadProcessId(hWnd, out var windowProcessId);
            if (windowProcessId == processId && GetParent(hWnd) == IntPtr.Zero)
            {
                var length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    var caption = new System.Text.StringBuilder(length + 1);
                    GetWindowText(hWnd, caption, caption.Capacity);
                    if (caption.ToString() == title)
                    {
                        match = hWnd;
                        return false; // stop enumerating
                    }
                }
            }

            return true;
        }, IntPtr.Zero);
        return match;
    }

    private static IntPtr SurfaceOf(FormBase? form)
    {
        try
        {
            return form?.Surface?.Handle ?? IntPtr.Zero;
        }
        catch
        {
            return IntPtr.Zero;
        }
    }

    private static IntPtr DisplayOf(FormBase? form)
    {
        try
        {
            return form?.Display?.Handle ?? (Gdk.Display.Default?.Handle ?? IntPtr.Zero);
        }
        catch
        {
            return IntPtr.Zero;
        }
    }

    private static IntPtr LoadGtk()
    {
        // GTK4 bundles gdk inside the gtk-4 library, so the gdk_* backend helpers live here too. The library
        // is already loaded by the binding; LoadLibrary by name returns the existing handle.
        foreach (var name in new[] { "libgtk-4-1.dll", "libgtk-4.so.1", "libgtk-4.so", "libgtk-4.1.dylib" })
        {
            var handle = FuncLoader.LoadLibrary(name);
            if (handle != IntPtr.Zero)
            {
                return handle;
            }
        }

        return IntPtr.Zero;
    }

    private static T? Resolve<T>(string function) where T : class
    {
        if (GtkLib == IntPtr.Zero)
        {
            return null;
        }

        var address = FuncLoader.GetProcAddress(GtkLib, function);
        return address == IntPtr.Zero ? null : FuncLoader.LoadFunction<T>(address);
    }

    // ---------------- Windows ----------------

    private static bool WinCenterOnWorkArea(IntPtr hwnd)
    {
        if (!GetWindowRect(hwnd, out var rect))
        {
            return false;
        }

        var work = WorkArea(hwnd);
        var w = rect.Right - rect.Left;
        var h = rect.Bottom - rect.Top;
        var x = work.Left + Math.Max(0, (work.Right - work.Left - w) / 2);
        var y = work.Top + Math.Max(0, (work.Bottom - work.Top - h) / 2);
        return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

    private static bool WinCenterOverOwner(IntPtr hwnd, IntPtr owner)
    {
        if (!GetWindowRect(hwnd, out var rect) || !GetWindowRect(owner, out var ownerRect))
        {
            return false;
        }

        var w = rect.Right - rect.Left;
        var h = rect.Bottom - rect.Top;
        var x = ownerRect.Left + (ownerRect.Right - ownerRect.Left - w) / 2;
        var y = ownerRect.Top + (ownerRect.Bottom - ownerRect.Top - h) / 2;

        // Keep the window on its monitor's work area, as WinForms' CenterToParent does.
        var work = WorkArea(hwnd);
        x = Math.Min(Math.Max(x, work.Left), Math.Max(work.Left, work.Right - w));
        y = Math.Min(Math.Max(y, work.Top), Math.Max(work.Top, work.Bottom - h));
        return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

    private static RECT WorkArea(IntPtr hwnd)
    {
        var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
        if (GetMonitorInfo(MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST), ref info))
        {
            return info.rcWork;
        }

        GetWindowRect(hwnd, out var rect);
        return rect;
    }

    // ---------------- X11 ----------------

    private static bool X11CenterOnScreen(IntPtr display, ulong xid)
    {
        if (!X11TryGetSize(display, xid, out var w, out var h))
        {
            return false;
        }

        var screen = XDefaultScreen(display);
        var sw = XDisplayWidth(display, screen);
        var sh = XDisplayHeight(display, screen);
        XMoveWindow(display, xid, Math.Max(0, (sw - w) / 2), Math.Max(0, (sh - h) / 2));
        XFlush(display);
        return true;
    }

    private static bool X11CenterOverOwner(IntPtr display, ulong xid, ulong owner)
    {
        if (!X11TryGetSize(display, xid, out var w, out var h))
        {
            return false;
        }

        if (!X11TryGetRootBounds(display, owner, out var ox, out var oy, out var ow, out var oh))
        {
            return X11CenterOnScreen(display, xid);
        }

        XMoveWindow(display, xid, ox + (ow - w) / 2, oy + (oh - h) / 2);
        XFlush(display);
        return true;
    }

    private static bool X11TryGetSize(IntPtr display, ulong window, out int width, out int height)
    {
        width = 0;
        height = 0;
        if (XGetGeometry(display, window, out _, out _, out _, out var w, out var h, out _, out _) == 0)
        {
            return false;
        }

        width = (int)w;
        height = (int)h;
        return width > 0 && height > 0;
    }

    private static bool X11TryGetRootBounds(IntPtr display, ulong window, out int x, out int y, out int width, out int height)
    {
        x = 0;
        y = 0;
        width = 0;
        height = 0;
        if (XGetGeometry(display, window, out var root, out _, out _, out var w, out var h, out _, out _) == 0)
        {
            return false;
        }

        if (XTranslateCoordinates(display, window, root, 0, 0, out var rootX, out var rootY, out _) == 0)
        {
            return false;
        }

        x = rootX;
        y = rootY;
        width = (int)w;
        height = (int)h;
        return width > 0 && height > 0;
    }

    // ---------------- P/Invoke ----------------

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO info);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int maxCount);

    private const uint WM_SETICON = 0x0080;
    private const int ICON_SMALL = 0;
    private const int ICON_BIG = 1;
    private const uint IMAGE_ICON = 1;
    private const uint LR_LOADFROMFILE = 0x0010;
    private const uint LR_DEFAULTSIZE = 0x0040;
    private const int SM_CXSMICON = 49;
    private const int SM_CYSMICON = 50;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadImage(IntPtr hinst, string name, uint type, int cx, int cy, uint load);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [DllImport("libX11.so.6")]
    private static extern int XMoveWindow(IntPtr display, ulong window, int x, int y);

    [DllImport("libX11.so.6")]
    private static extern int XFlush(IntPtr display);

    [DllImport("libX11.so.6")]
    private static extern int XDefaultScreen(IntPtr display);

    [DllImport("libX11.so.6")]
    private static extern int XDisplayWidth(IntPtr display, int screen);

    [DllImport("libX11.so.6")]
    private static extern int XDisplayHeight(IntPtr display, int screen);

    [DllImport("libX11.so.6")]
    private static extern int XGetGeometry(IntPtr display, ulong drawable, out ulong root, out int x, out int y,
        out uint width, out uint height, out uint borderWidth, out uint depth);

    [DllImport("libX11.so.6")]
    private static extern int XTranslateCoordinates(IntPtr display, ulong sourceWindow, ulong destWindow,
        int sourceX, int sourceY, out int destX, out int destY, out ulong child);

    // ---------------- macOS (Quartz / Cocoa) ----------------

    private const string ObjCLibrary = "/usr/lib/libobjc.dylib";

    private static bool MacCenter(FormBase? form)
    {
        var title = TitleOf(form);
        if (string.IsNullOrEmpty(title) || !MacTryFindWindowByTitle(title!, out var window))
        {
            return false;
        }

        // NSWindow -center places the window at the center of its screen - exactly CenterToScreen. A precise
        // CenterToParent would need the owner's NSRect frame, whose struct return is architecture-dependent
        // on macOS; a modal child already centers over its parent via GTK, so screen-centering is the safe
        // common behaviour here.
        objc_msgSend(window, sel_registerName("center"));
        return true;
    }

    private static bool MacTryFindWindowByTitle(string title, out IntPtr window)
    {
        window = IntPtr.Zero;
        var app = objc_msgSend(objc_getClass("NSApplication"), sel_registerName("sharedApplication"));
        if (app == IntPtr.Zero)
        {
            return false;
        }

        var windows = objc_msgSend(app, sel_registerName("windows"));
        if (windows == IntPtr.Zero)
        {
            return false;
        }

        var count = (long)objc_msgSend(windows, sel_registerName("count"));
        var selTitle = sel_registerName("title");
        var selUtf8 = sel_registerName("UTF8String");
        var selObjectAtIndex = sel_registerName("objectAtIndex:");
        for (long i = 0; i < count; i++)
        {
            var candidate = objc_msgSend_index(windows, selObjectAtIndex, (IntPtr)i);
            if (candidate == IntPtr.Zero)
            {
                continue;
            }

            var nsTitle = objc_msgSend(candidate, selTitle);
            var utf8 = nsTitle == IntPtr.Zero ? IntPtr.Zero : objc_msgSend(nsTitle, selUtf8);
            if (utf8 != IntPtr.Zero && Marshal.PtrToStringAnsi(utf8) == title)
            {
                window = candidate;
                return true;
            }
        }

        return false;
    }

    private static string? TitleOf(FormBase? form)
    {
        try
        {
            return form?.Title;
        }
        catch
        {
            return null;
        }
    }

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjCLibrary, CharSet = CharSet.Ansi)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend_index(IntPtr receiver, IntPtr selector, IntPtr index);
}

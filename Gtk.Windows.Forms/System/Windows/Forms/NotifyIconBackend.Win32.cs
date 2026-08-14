using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// The Windows system tray backend, built on <c>Shell_NotifyIcon</c>.
/// </summary>
/// <remarks>
/// <para>
/// The shell delivers tray input by posting a callback message to a window, so this backend owns a private
/// message-only window (<c>HWND_MESSAGE</c>) with its own window class. It deliberately does not touch,
/// subclass or borrow Gdk's own <c>HWND</c>: an independent window cannot interfere with Gtk's message
/// handling, and its lifetime is ours to control.
/// </para>
/// <para>
/// This relies on Gdk's Win32 event source pumping the thread message queue and dispatching messages it
/// does not own - which is how Gtk 3's own <c>GtkStatusIcon</c> worked on Windows, and which was verified
/// against this Gtk 4 build before the backend was written.
/// </para>
/// </remarks>
internal sealed class Win32NotifyIconBackend : NotifyIconBackendBase
{
    private const int HWND_MESSAGE = -3;

    private const uint WM_TRAYCALLBACK = 0x8001; // WM_APP + 1

    private const uint WM_NULL = 0x0000;
    private const uint WM_MOUSEMOVE = 0x0200;
    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint WM_LBUTTONDBLCLK = 0x0203;
    private const uint WM_RBUTTONDOWN = 0x0204;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_RBUTTONDBLCLK = 0x0206;
    private const uint WM_MBUTTONDOWN = 0x0207;
    private const uint WM_MBUTTONUP = 0x0208;

    private const uint NIN_BALLOONSHOW = 0x0402;
    private const uint NIN_BALLOONHIDE = 0x0403;
    private const uint NIN_BALLOONTIMEOUT = 0x0404;
    private const uint NIN_BALLOONUSERCLICK = 0x0405;

    private const uint NIM_ADD = 0x00000000;
    private const uint NIM_MODIFY = 0x00000001;
    private const uint NIM_DELETE = 0x00000002;

    private const uint NIF_MESSAGE = 0x00000001;
    private const uint NIF_ICON = 0x00000002;
    private const uint NIF_TIP = 0x00000004;
    private const uint NIF_INFO = 0x00000010;

    private const uint NIIF_NONE = 0x00000000;
    private const uint NIIF_INFO = 0x00000001;
    private const uint NIIF_WARNING = 0x00000002;
    private const uint NIIF_ERROR = 0x00000003;

    private const uint MF_BYPOSITION = 0x00000400;
    private const uint MF_SEPARATOR = 0x00000800;
    private const uint MF_STRING = 0x00000000;
    private const uint MF_POPUP = 0x00000010;
    private const uint MF_GRAYED = 0x00000001;
    private const uint MF_CHECKED = 0x00000008;

    private const uint TPM_RETURNCMD = 0x0100;
    private const uint TPM_RIGHTBUTTON = 0x0002;

    private const int SM_CXSMICON = 49;
    private const int SM_CYSMICON = 50;

    private const int BI_RGB = 0;
    private const int DIB_RGB_COLORS = 0;

    private static readonly object s_classLock = new object();

    // Held statically for the process lifetime: a collected window procedure delegate means a hard crash
    // the first time the shell posts to the window.
    private static readonly WndProcDelegate s_wndProc = StaticWndProc;

    private static readonly Dictionary<IntPtr, Win32NotifyIconBackend> s_windows =
        new Dictionary<IntPtr, Win32NotifyIconBackend>();

    private static ushort s_classAtom;

    private static uint s_taskbarCreatedMessage;

    private readonly IntPtr _hwnd;

    private IntPtr _hIcon;

    private string _toolTip = string.Empty;

    private bool _shown;

    private ContextMenuStrip? _menu;

    /// <summary>
    /// Initialises a new instance of the <see cref="Win32NotifyIconBackend" /> class, creating its private
    /// message-only window.
    /// </summary>
    /// <exception cref="InvalidOperationException">The window class or window could not be created.</exception>
    public Win32NotifyIconBackend()
    {
        EnsureWindowClass();

        var hInstance = GetModuleHandleW(null);
        _hwnd = CreateWindowExW(0, new IntPtr(s_classAtom), string.Empty, 0, 0, 0, 0, 0,
            new IntPtr(HWND_MESSAGE), IntPtr.Zero, hInstance, IntPtr.Zero);

        if (_hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Unable to create the system tray message window (error " + Marshal.GetLastWin32Error() + ").");
        }

        lock (s_classLock)
        {
            s_windows[_hwnd] = this;
        }
    }

    /// <inheritdoc />
    public override bool IsAvailable => true;

    /// <summary>
    /// Gets the marshalled size of <c>NOTIFYICONDATAW</c>, which the shell validates on every call.
    /// </summary>
    /// <value>976 on 64-bit and 956 on 32-bit - the documented <c>NOTIFYICONDATA_V4_SIZE</c>.</value>
    /// <remarks>
    /// Exposed so the struct layout can be asserted in tests. A wrong size makes every
    /// <c>Shell_NotifyIcon</c> call fail with no diagnostic beyond a false return.
    /// </remarks>
    internal static int NativeDataSize => Marshal.SizeOf(typeof(NOTIFYICONDATAW));

    /// <summary>
    /// Gets a value indicating whether the shell currently holds this icon.
    /// </summary>
    /// <value>
    /// <c>true</c> between a successful <c>NIM_ADD</c> and the matching <c>NIM_DELETE</c>. This is the
    /// shell's answer, not the requested state: a rejected add leaves it <c>false</c>.
    /// </value>
    internal bool IsIconAdded => _shown;

    /// <inheritdoc />
    public override void SetIcon(Drawing.Icon? icon)
    {
        if (IsDisposed)
        {
            return;
        }

        var replacement = CreateHIcon(icon);

        // Free the outgoing icon only after the new one is in place, so a failed conversion leaves the
        // previous icon on screen rather than blanking it.
        var previous = _hIcon;
        _hIcon = replacement;

        if (_shown)
        {
            Notify(NIM_MODIFY, NIF_ICON);
        }

        if (previous != IntPtr.Zero)
        {
            DestroyIcon(previous);
        }
    }

    /// <inheritdoc />
    public override void SetToolTip(string text)
    {
        if (IsDisposed)
        {
            return;
        }

        _toolTip = text ?? string.Empty;

        if (_shown)
        {
            Notify(NIM_MODIFY, NIF_TIP);
        }
    }

    /// <inheritdoc />
    public override void SetVisible(bool visible)
    {
        if (IsDisposed || visible == _shown)
        {
            return;
        }

        if (visible)
        {
            _shown = Notify(NIM_ADD, NIF_MESSAGE | NIF_ICON | NIF_TIP);
        }
        else
        {
            Notify(NIM_DELETE, 0);
            _shown = false;
        }
    }

    /// <inheritdoc />
    public override void SetMenu(ContextMenuStrip? menu)
    {
        _menu = menu;
    }

    /// <inheritdoc />
    public override void ShowBalloonTip(int timeout, string title, string text, ToolTipIcon icon)
    {
        if (IsDisposed || !_shown)
        {
            return;
        }

        var data = CreateData(NIF_INFO);
        data.szInfo = Truncate(text, 255);
        data.szInfoTitle = Truncate(title, 63);
        data.dwInfoFlags = icon switch
        {
            ToolTipIcon.Info => NIIF_INFO,
            ToolTipIcon.Warning => NIIF_WARNING,
            ToolTipIcon.Error => NIIF_ERROR,
            _ => NIIF_NONE,
        };

        // uTimeoutOrVersion is ignored from Windows Vista onwards - notification duration is a system
        // accessibility setting - so the caller's timeout cannot be honoured here.
        Shell_NotifyIconW(NIM_MODIFY, ref data);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_shown)
        {
            Notify(NIM_DELETE, 0);
            _shown = false;
        }

        if (_hIcon != IntPtr.Zero)
        {
            DestroyIcon(_hIcon);
            _hIcon = IntPtr.Zero;
        }

        lock (s_classLock)
        {
            s_windows.Remove(_hwnd);
        }

        if (_hwnd != IntPtr.Zero)
        {
            DestroyWindow(_hwnd);
        }

        base.Dispose(disposing);
    }

    private static void EnsureWindowClass()
    {
        lock (s_classLock)
        {
            if (s_classAtom != 0)
            {
                return;
            }

            var wc = new WNDCLASSW
            {
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(s_wndProc),
                hInstance = GetModuleHandleW(null),
                lpszClassName = "GtkWindowsFormsNotifyIcon",
            };

            s_classAtom = RegisterClassW(ref wc);

            if (s_classAtom == 0)
            {
                throw new InvalidOperationException(
                    "Unable to register the system tray window class (error " + Marshal.GetLastWin32Error() + ").");
            }

            // Explorer restarts are routine; without re-adding the icon on this broadcast it would vanish
            // permanently for the rest of the process's life.
            s_taskbarCreatedMessage = RegisterWindowMessageW("TaskbarCreated");
        }
    }

    private static IntPtr StaticWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        Win32NotifyIconBackend? backend;

        lock (s_classLock)
        {
            // Messages sent during CreateWindowEx arrive before the instance is registered.
            s_windows.TryGetValue(hWnd, out backend);
        }

        if (backend != null)
        {
            try
            {
                backend.HandleMessage(msg, lParam);
            }
            catch (Exception ex)
            {
                Diagnostics.GtkLogger.Debug("Unexpected exception handling a system tray message", ex);
            }
        }

        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    private void HandleMessage(uint msg, IntPtr lParam)
    {
        if (msg == s_taskbarCreatedMessage && s_taskbarCreatedMessage != 0)
        {
            if (_shown)
            {
                Notify(NIM_ADD, NIF_MESSAGE | NIF_ICON | NIF_TIP);
            }

            return;
        }

        if (msg != WM_TRAYCALLBACK)
        {
            return;
        }

        GetCursorPos(out var pt);
        var notification = (uint)(lParam.ToInt64() & 0xFFFF);

        switch (notification)
        {
            case WM_MOUSEMOVE:
                RaisePointerMove(pt.x, pt.y);
                break;

            case WM_LBUTTONDOWN:
                RaisePointerDown(MouseButtons.Left, pt.x, pt.y);
                break;

            case WM_LBUTTONUP:
                RaisePointerUp(MouseButtons.Left, pt.x, pt.y);
                RaiseActivated(MouseButtons.Left, 1, pt.x, pt.y);
                break;

            case WM_LBUTTONDBLCLK:
                RaiseActivated(MouseButtons.Left, 2, pt.x, pt.y);
                break;

            case WM_RBUTTONDOWN:
                RaisePointerDown(MouseButtons.Right, pt.x, pt.y);
                break;

            case WM_RBUTTONUP:
                RaisePointerUp(MouseButtons.Right, pt.x, pt.y);
                RaiseActivated(MouseButtons.Right, 1, pt.x, pt.y);
                ShowContextMenu(pt);
                break;

            case WM_RBUTTONDBLCLK:
                RaiseActivated(MouseButtons.Right, 2, pt.x, pt.y);
                break;

            case WM_MBUTTONDOWN:
                RaisePointerDown(MouseButtons.Middle, pt.x, pt.y);
                break;

            case WM_MBUTTONUP:
                RaisePointerUp(MouseButtons.Middle, pt.x, pt.y);
                RaiseActivated(MouseButtons.Middle, 1, pt.x, pt.y);
                break;

            case NIN_BALLOONSHOW:
                RaiseBalloonShown();
                break;

            case NIN_BALLOONHIDE:
            case NIN_BALLOONTIMEOUT:
                RaiseBalloonClosed();
                break;

            case NIN_BALLOONUSERCLICK:
                RaiseBalloonClicked();
                RaiseBalloonClosed();
                break;
        }
    }

    private void ShowContextMenu(POINT pt)
    {
        // Translated now rather than at assignment time, so a strip mutated since then still renders
        // correctly.
        var menu = TrayMenuModel.From(_menu);
        if (menu == null || !menu.Root.HasChildren)
        {
            return;
        }

        var hMenu = BuildPopupMenu(menu.Root);
        if (hMenu == IntPtr.Zero)
        {
            return;
        }

        try
        {
            // Documented requirement: the owning window must be foreground or the menu never dismisses
            // when the user clicks away from it.
            SetForegroundWindow(_hwnd);

            var command = TrackPopupMenuEx(hMenu, TPM_RETURNCMD | TPM_RIGHTBUTTON, pt.x, pt.y, _hwnd, IntPtr.Zero);

            // The other half of the same workaround.
            PostMessageW(_hwnd, WM_NULL, IntPtr.Zero, IntPtr.Zero);

            if (command != 0)
            {
                var id = (int)command;
                InvokeOnMainLoop(() => menu.Activate(id));
            }
        }
        finally
        {
            DestroyMenu(hMenu);
        }
    }

    private static IntPtr BuildPopupMenu(TrayMenuNode parent)
    {
        var hMenu = CreatePopupMenu();
        if (hMenu == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        var position = 0u;

        foreach (var child in parent.Children)
        {
            if (child.IsSeparator)
            {
                InsertMenuW(hMenu, position++, MF_BYPOSITION | MF_SEPARATOR, UIntPtr.Zero, null);
                continue;
            }

            var flags = MF_BYPOSITION | MF_STRING;

            if (!child.Enabled)
            {
                flags |= MF_GRAYED;
            }

            if (child.IsCheckable && child.Checked)
            {
                flags |= MF_CHECKED;
            }

            if (child.HasChildren)
            {
                var submenu = BuildPopupMenu(child);
                if (submenu != IntPtr.Zero)
                {
                    InsertMenuW(hMenu, position++, flags | MF_POPUP, new UIntPtr((ulong)submenu.ToInt64()),
                        child.Text);
                    continue;
                }
            }

            // Win32 reserves 0 for "cancelled", and TrayMenuModel numbers real items from 1, so ids pass
            // through unchanged.
            InsertMenuW(hMenu, position++, flags, new UIntPtr((uint)child.Id), child.Text);
        }

        return hMenu;
    }

    private bool Notify(uint message, uint flags)
    {
        var data = CreateData(flags);
        var result = Shell_NotifyIconW(message, ref data);

        if (!result && message != NIM_DELETE)
        {
            Diagnostics.GtkLogger.Debug(
                "Shell_NotifyIcon failed (message " + message + ", error " + Marshal.GetLastWin32Error() + ")");
        }

        return result;
    }

    private NOTIFYICONDATAW CreateData(uint flags)
    {
        if (_hIcon == IntPtr.Zero)
        {
            flags &= ~NIF_ICON;
        }

        return new NOTIFYICONDATAW
        {
            cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONDATAW)),
            hWnd = _hwnd,
            uID = 1,
            uFlags = flags,
            uCallbackMessage = WM_TRAYCALLBACK,
            hIcon = _hIcon,
            // WinForms caps NotifyIcon.Text at 127 characters precisely because szTip holds 128 including
            // the terminator, so this truncation is a belt-and-braces guard rather than a policy.
            szTip = Truncate(_toolTip, 127),
            szInfo = string.Empty,
            szInfoTitle = string.Empty,
        };
    }

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value!.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    /// <summary>
    /// Converts a WinForms icon to an <c>HICON</c> sized for the notification area.
    /// </summary>
    /// <param name="icon">The icon to convert, or <c>null</c>.</param>
    /// <returns>A new <c>HICON</c> the caller owns, or <see cref="IntPtr.Zero" />.</returns>
    internal static IntPtr CreateHIcon(Drawing.Icon? icon)
    {
        var pixbuf = icon?.Pixbuf;
        if (pixbuf == null)
        {
            return IntPtr.Zero;
        }

        Gdk.Pixbuf? withAlpha = null;
        Gdk.Pixbuf? scaled = null;

        try
        {
            var source = pixbuf;

            if (!source.HasAlpha)
            {
                withAlpha = source.AddAlpha(false, 0, 0, 0);
                source = withAlpha;
            }

            var width = GetSystemMetrics(SM_CXSMICON);
            var height = GetSystemMetrics(SM_CYSMICON);

            if (width <= 0 || height <= 0)
            {
                width = 16;
                height = 16;
            }

            if (source.Width != width || source.Height != height)
            {
                scaled = source.ScaleSimple(width, height, Gdk.InterpType.Bilinear);
                if (scaled != null)
                {
                    source = scaled;
                }
            }

            return CreateHIconFromPixbuf(source);
        }
        catch (Exception ex)
        {
            Diagnostics.GtkLogger.Debug("Unable to convert the icon for the Windows notification area", ex);
            return IntPtr.Zero;
        }
        finally
        {
            scaled?.Dispose();
            withAlpha?.Dispose();
        }
    }

    private static IntPtr CreateHIconFromPixbuf(Gdk.Pixbuf pixbuf)
    {
        var width = pixbuf.Width;
        var height = pixbuf.Height;
        var bgra = ToBgra(pixbuf);

        var header = new BITMAPINFOHEADER
        {
            biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
            biWidth = width,
            // Negative height requests a top-down DIB, matching the pixbuf's row order.
            biHeight = -height,
            biPlanes = 1,
            biBitCount = 32,
            biCompression = BI_RGB,
        };

        var hdc = GetDC(IntPtr.Zero);
        var hbmColor = IntPtr.Zero;
        var hbmMask = IntPtr.Zero;

        try
        {
            hbmColor = CreateDIBSection(hdc, ref header, DIB_RGB_COLORS, out var bits, IntPtr.Zero, 0);
            if (hbmColor == IntPtr.Zero || bits == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            Marshal.Copy(bgra, 0, bits, bgra.Length);

            // A 32bpp colour bitmap carries its own alpha, so an all-zero mask (meaning "draw every
            // pixel") is what CreateIconIndirect wants alongside it.
            hbmMask = CreateBitmap(width, height, 1, 1, IntPtr.Zero);
            if (hbmMask == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var info = new ICONINFO
            {
                fIcon = true,
                xHotspot = 0,
                yHotspot = 0,
                hbmMask = hbmMask,
                hbmColor = hbmColor,
            };

            return CreateIconIndirect(ref info);
        }
        finally
        {
            if (hbmMask != IntPtr.Zero)
            {
                DeleteObject(hbmMask);
            }

            if (hbmColor != IntPtr.Zero)
            {
                DeleteObject(hbmColor);
            }

            if (hdc != IntPtr.Zero)
            {
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }
    }

    /// <summary>
    /// Converts pixbuf pixels to the straight-alpha BGRA layout a 32bpp DIB expects.
    /// </summary>
    /// <param name="pixbuf">The source pixbuf, which must have an alpha channel.</param>
    /// <returns>The pixel data, four bytes per pixel, top row first.</returns>
    /// <remarks>
    /// <c>CreateIconIndirect</c> wants straight, not premultiplied, alpha - which is exactly how a pixbuf
    /// stores it, so only the channel order changes. Rows are read through <c>Rowstride</c>, which is not
    /// generally <c>Width * NChannels</c>.
    /// </remarks>
    internal static byte[] ToBgra(Gdk.Pixbuf pixbuf)
    {
        var width = pixbuf.Width;
        var height = pixbuf.Height;
        var channels = pixbuf.NChannels;
        var rowstride = pixbuf.Rowstride;
        var hasAlpha = pixbuf.HasAlpha;

        var row = new byte[rowstride];
        var result = new byte[width * height * 4];
        var pixels = pixbuf.Pixels;

        for (var y = 0; y < height; y++)
        {
            Marshal.Copy(pixels + (y * rowstride), row, 0, rowstride);

            for (var x = 0; x < width; x++)
            {
                var src = x * channels;
                var dst = ((y * width) + x) * 4;

                result[dst + 0] = row[src + 2]; // B
                result[dst + 1] = row[src + 1]; // G
                result[dst + 2] = row[src + 0]; // R
                result[dst + 3] = hasAlpha ? row[src + 3] : (byte)0xFF;
            }
        }

        return result;
    }

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;

        public int y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSW
    {
        public uint style;

        public IntPtr lpfnWndProc;

        public int cbClsExtra;

        public int cbWndExtra;

        public IntPtr hInstance;

        public IntPtr hIcon;

        public IntPtr hCursor;

        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATAW
    {
        public uint cbSize;

        public IntPtr hWnd;

        public uint uID;

        public uint uFlags;

        public uint uCallbackMessage;

        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;

        public uint dwState;

        public uint dwStateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;

        public uint uTimeoutOrVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;

        public uint dwInfoFlags;

        public Guid guidItem;

        public IntPtr hBalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public uint biSize;

        public int biWidth;

        public int biHeight;

        public ushort biPlanes;

        public ushort biBitCount;

        public uint biCompression;

        public uint biSizeImage;

        public int biXPelsPerMeter;

        public int biYPelsPerMeter;

        public uint biClrUsed;

        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ICONINFO
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool fIcon;

        public int xHotspot;

        public int yHotspot;

        public IntPtr hbmMask;

        public IntPtr hbmColor;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClassW(ref WNDCLASSW lpWndClass);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint RegisterWindowMessageW(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint dwExStyle, IntPtr lpClassName, string lpWindowName,
        uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool PostMessageW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyMenu(IntPtr hMenu);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool InsertMenuW(IntPtr hMenu, uint uPosition, uint uFlags, UIntPtr uIDNewItem,
        string? lpNewItem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hwnd,
        IntPtr lptpm);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreateIconIndirect(ref ICONINFO piconinfo);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFOHEADER pbmi, uint usage,
        out IntPtr ppvBits, IntPtr hSection, uint offset);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint nPlanes, uint nBitCount,
        IntPtr lpBits);

    [DllImport("gdi32.dll", SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandleW(string? lpModuleName);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool Shell_NotifyIconW(uint dwMessage, ref NOTIFYICONDATAW lpData);
}

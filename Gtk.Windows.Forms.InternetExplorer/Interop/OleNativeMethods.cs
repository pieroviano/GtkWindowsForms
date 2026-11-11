using System;
using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.InternetExplorer.Interop;

/// <summary>
/// The Win32 and OLE entry points the host calls directly: OLE apartment init, COM instantiation, and the
/// child-window management that keeps the control over the GTK widget's area.
/// </summary>
internal static class OleNativeMethods
{
    internal const int WS_CHILD = unchecked((int)0x40000000);
    internal const int WS_VISIBLE = 0x10000000;
    internal const int WS_CLIPCHILDREN = 0x02000000;
    internal const int WS_CLIPSIBLINGS = 0x04000000;

    internal const uint SWP_NOZORDER = 0x0004;
    internal const uint SWP_NOACTIVATE = 0x0010;
    internal const uint SWP_NOOWNERZORDER = 0x0200;

    internal const int SW_HIDE = 0;
    internal const int SW_SHOW = 5;

    internal const int CLSCTX_INPROC_SERVER = 1;

    [DllImport("ole32.dll")]
    internal static extern int OleInitialize(IntPtr pvReserved);

    [DllImport("ole32.dll")]
    internal static extern void OleUninitialize();

    [DllImport("ole32.dll")]
    internal static extern int CoCreateInstance(
        [In] ref Guid rclsid, IntPtr pUnkOuter, int dwClsContext, [In] ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out object ppv);

    [DllImport("ole32.dll")]
    internal static extern int OleSetContainedObject(
        [MarshalAs(UnmanagedType.IUnknown)] object pUnknown, [MarshalAs(UnmanagedType.Bool)] bool fContained);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr CreateWindowExW(
        int dwExStyle, string lpClassName, string? lpWindowName, int dwStyle,
        int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
        IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>IID_IUnknown, for <c>CoCreateInstance</c>.</summary>
    internal static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
}

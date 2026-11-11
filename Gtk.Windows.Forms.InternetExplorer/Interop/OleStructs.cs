using System;
using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.InternetExplorer.Interop;

/// <summary>A Win32 rectangle (LTRB), as the OLE in-place interfaces pass it.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int left;
    public int top;
    public int right;
    public int bottom;

    public RECT(int l, int t, int r, int b)
    {
        left = l;
        top = t;
        right = r;
        bottom = b;
    }
}

/// <summary>A Win32 point.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}

/// <summary>A Win32 size.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct SIZE
{
    public int cx;
    public int cy;
}

/// <summary>A thread message, as <c>IOleInPlaceActiveObject.TranslateAccelerator</c> receives it.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public IntPtr hwnd;
    public int message;
    public IntPtr wParam;
    public IntPtr lParam;
    public int time;
    public POINT pt;
}

/// <summary>The in-place frame info the site hands back from <c>GetWindowContext</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct OLEINPLACEFRAMEINFO
{
    public int cb;
    [MarshalAs(UnmanagedType.Bool)] public bool fMDIApp;
    public IntPtr hwndFrame;
    public IntPtr haccel;
    public int cAccelEntries;
}

/// <summary>Verbs for <c>IOleObject.DoVerb</c>; only the ones this host issues are named.</summary>
internal static class OleVerb
{
    internal const int Primary = 0;
    internal const int Show = -1;
    internal const int Hide = -3;
    internal const int UiActivate = -4;
    internal const int InPlaceActivate = -5;
}

/// <summary>Flags for <c>IOleObject.Close</c>.</summary>
internal static class OleClose
{
    internal const int NoSave = 0;
}

/// <summary>Draw aspect for <c>SetExtent</c> / <c>GetExtent</c>.</summary>
internal static class DvAspect
{
    internal const int Content = 1;
}

/// <summary>
/// The browser commands routed through <c>IWebBrowser2.ExecWB</c>. These back the control's print and
/// dialog methods, which the base library otherwise reports as unsupported.
/// </summary>
internal enum OleCmdId
{
    SaveAs = 4,
    Print = 6,
    PrintPreview = 7,
    PageSetup = 8,
    Properties = 10,
}

/// <summary>Execution options for <c>IWebBrowser2.ExecWB</c>.</summary>
internal enum OleCmdExecOpt
{
    DoDefault = 0,
    PromptUser = 1,
    DontPromptUser = 2,
}

/// <summary>GUIDs the host needs by value.</summary>
internal static class OleGuids
{
    /// <summary>CLSID of the Internet Explorer WebBrowser ActiveX control (<c>Shell.Explorer.2</c>).</summary>
    internal static readonly Guid WebBrowser = new Guid("8856F961-340A-11D0-A96B-00C04FD705A2");

    /// <summary>DIID_DWebBrowserEvents2 - the outgoing event dispinterface.</summary>
    internal static readonly Guid DWebBrowserEvents2 = new Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D");
}

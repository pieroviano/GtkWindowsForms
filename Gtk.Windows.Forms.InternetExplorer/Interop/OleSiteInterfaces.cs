using System;
using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.InternetExplorer.Interop;

// Interfaces this host IMPLEMENTS and the control CALLS into (the client site). Declared as plain managed
// interfaces (not [ComImport]) so the CLR builds a COM-callable wrapper exposing each [Guid]. Every method
// returns an HRESULT via [PreserveSig], so the site can answer S_OK / E_NOTIMPL precisely rather than
// relying on exception translation. The inheritance chain mirrors OLE's, so a QueryInterface for the base
// IIDs (IOleWindow, IOleInPlaceUIWindow) also succeeds on the frame.

/// <summary>Standard OLE HRESULTs the site returns.</summary>
internal static class HResult
{
    internal const int S_OK = 0;
    internal const int S_FALSE = 1;
    internal const int E_NOTIMPL = unchecked((int)0x80004001);
    internal const int E_NOINTERFACE = unchecked((int)0x80004002);
    internal const int E_FAIL = unchecked((int)0x80004005);
    internal const int INPLACE_E_NOTOOLSPACE = unchecked((int)0x800401A1);
}

/// <summary>OLE's base window interface (implemented side).</summary>
[ComVisible(true), Guid("00000114-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleWindow
{
    [PreserveSig] int GetWindow(out IntPtr phwnd);
    [PreserveSig] int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);
}

/// <summary>OLE's in-place UI window interface (implemented side).</summary>
[ComVisible(true), Guid("00000115-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleInPlaceUIWindow : IOleWindow
{
    [PreserveSig] int GetBorder(out RECT lprectBorder);
    [PreserveSig] int RequestBorderSpace([In] ref RECT pborderwidths);
    [PreserveSig] int SetBorderSpace([In] ref RECT pborderwidths);
    [PreserveSig] int SetActiveObject(IntPtr pActiveObject,
        [MarshalAs(UnmanagedType.LPWStr)] string? pszObjName);
}

/// <summary>The in-place frame the site hands the control through <c>GetWindowContext</c>.</summary>
[ComVisible(true), Guid("00000116-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleInPlaceFrame : IOleInPlaceUIWindow
{
    [PreserveSig] int InsertMenus(IntPtr hmenuShared, IntPtr lpMenuWidths);
    [PreserveSig] int SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject);
    [PreserveSig] int RemoveMenus(IntPtr hmenuShared);
    [PreserveSig] int SetStatusText([MarshalAs(UnmanagedType.LPWStr)] string? pszStatusText);
    [PreserveSig] int EnableModeless([MarshalAs(UnmanagedType.Bool)] bool fEnable);
    [PreserveSig] int TranslateAccelerator([In] ref MSG lpmsg, short wID);
}

/// <summary>The client site the control uses to reach its container.</summary>
[ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleClientSite
{
    [PreserveSig] int SaveObject();
    [PreserveSig] int GetMoniker(int dwAssign, int dwWhichMoniker, out IntPtr ppmk);
    [PreserveSig] int GetContainer(out IntPtr ppContainer);
    [PreserveSig] int ShowObject();
    [PreserveSig] int OnShowWindow([MarshalAs(UnmanagedType.Bool)] bool fShow);
    [PreserveSig] int RequestNewObjectLayout();
}

/// <summary>The in-place site the control activates into.</summary>
[ComVisible(true), Guid("00000119-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleInPlaceSite : IOleWindow
{
    [PreserveSig] int CanInPlaceActivate();
    [PreserveSig] int OnInPlaceActivate();
    [PreserveSig] int OnUIActivate();
    [PreserveSig] int GetWindowContext(out IOleInPlaceFrame ppFrame, out IntPtr ppDoc,
        out RECT lprcPosRect, out RECT lprcClipRect, [In, Out] ref OLEINPLACEFRAMEINFO lpFrameInfo);
    [PreserveSig] int Scroll(SIZE scrollExtant);
    [PreserveSig] int OnUIDeactivate([MarshalAs(UnmanagedType.Bool)] bool fUndoable);
    [PreserveSig] int OnInPlaceDeactivate();
    [PreserveSig] int DiscardUndoState();
    [PreserveSig] int DeactivateAndUndo();
    [PreserveSig] int OnPosRectChange([In] ref RECT lprcPosRect);
}

using System;
using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.InternetExplorer.Interop;

// Interfaces this host RECEIVES from the control and CALLS into: declared [ComImport] so the CLR treats a
// cast of the control's RCW to them as a QueryInterface. Every method occupies a vtable slot and must be
// declared in the exact OLE order; slots this host never calls still have to be present, so they are given
// placeholder signatures. HRESULT failures surface as exceptions (the caller guards them).

/// <summary>The embeddable-object interface of the WebBrowser control.</summary>
[ComImport, Guid("00000112-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleObject
{
    void SetClientSite(IOleClientSite pClientSite);
    void GetClientSite(out IOleClientSite ppClientSite);
    void SetHostNames([MarshalAs(UnmanagedType.LPWStr)] string szContainerApp,
        [MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);
    void Close(int dwSaveOption);
    void SetMoniker(int dwWhichMoniker, IntPtr pmk);
    void GetMoniker(int dwAssign, int dwWhichMoniker, out IntPtr ppmk);
    void InitFromData(IntPtr pDataObject, [MarshalAs(UnmanagedType.Bool)] bool fCreation, int dwReserved);
    void GetClipboardData(int dwReserved, out IntPtr ppDataObject);
    [PreserveSig]
    int DoVerb(int iVerb, IntPtr lpmsg, IOleClientSite pActiveSite, int lindex, IntPtr hwndParent,
        [In] ref RECT lprcPosRect);
    void EnumVerbs(out IntPtr ppEnumOleVerb);
    void Update();
    [PreserveSig] int IsUpToDate();
    void GetUserClassID(ref Guid pClsid);
    void GetUserType(int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string pszUserType);
    void SetExtent(int dwDrawAspect, [In] ref SIZE psizel);
    void GetExtent(int dwDrawAspect, out SIZE psizel);
    void Advise(IntPtr pAdvSink, out int pdwConnection);
    void Unadvise(int dwConnection);
    void EnumAdvise(out IntPtr ppenumAdvise);
    void GetMiscStatus(int dwAspect, out int pdwStatus);
    void SetColorScheme(IntPtr pLogpal);
}

/// <summary>The in-place object interface, used to resize and deactivate the view.</summary>
[ComImport, Guid("00000113-0000-0000-C000-000000000046"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleInPlaceObject
{
    void GetWindow(out IntPtr phwnd);
    void ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);
    void InPlaceDeactivate();
    void UIDeactivate();
    void SetObjectRects([In] ref RECT lprcPosRect, [In] ref RECT lprcClipRect);
    void ReactivateAndUndo();
}

/// <summary>Enumerates the control's outgoing event interfaces.</summary>
[ComImport, Guid("B196B284-BAB4-101A-B69C-00AA00341D07"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IConnectionPointContainer
{
    void EnumConnectionPoints(out IntPtr ppEnum);
    void FindConnectionPoint([In] ref Guid riid, out IConnectionPoint ppCP);
}

/// <summary>A single outgoing event interface, used to advise the <c>DWebBrowserEvents2</c> sink.</summary>
[ComImport, Guid("B196B286-BAB4-101A-B69C-00AA00341D07"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IConnectionPoint
{
    void GetConnectionInterface(out Guid pIID);
    void GetConnectionPointContainer(out IConnectionPointContainer ppCPC);
    void Advise([MarshalAs(UnmanagedType.IUnknown)] object pUnkSink, out int pdwCookie);
    void Unadvise(int dwCookie);
    void EnumConnections(out IntPtr ppEnum);
}

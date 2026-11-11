using System;
using Gtk.Windows.Forms.InternetExplorer.Interop;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// The minimal OLE container the WebBrowser ActiveX control activates into: a client site, an in-place
/// site, and an in-place frame rolled into one object.
/// </summary>
/// <remarks>
/// It owns no UI of its own - menus, borders and status are declined - because the control lives in a bare
/// child window the host positions over a GTK widget. The only live information it hands the control is the
/// in-place window handle and the current position rectangle, supplied by the host through the delegates
/// passed to the constructor.
/// </remarks>
internal sealed class OleHostSite : IOleClientSite, IOleInPlaceSite, IOleInPlaceFrame
{
    private readonly Func<IntPtr> _inPlaceWindow;

    private readonly Func<IntPtr> _frameWindow;

    private readonly Func<RECT> _positionRect;

    internal OleHostSite(Func<IntPtr> inPlaceWindow, Func<IntPtr> frameWindow, Func<RECT> positionRect)
    {
        _inPlaceWindow = inPlaceWindow;
        _frameWindow = frameWindow;
        _positionRect = positionRect;
    }

    // ---- IOleClientSite ----
    public int SaveObject() => HResult.S_OK;

    public int GetMoniker(int dwAssign, int dwWhichMoniker, out IntPtr ppmk)
    {
        ppmk = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetContainer(out IntPtr ppContainer)
    {
        // No container object to expose; the control treats this as "top-level container".
        ppContainer = IntPtr.Zero;
        return HResult.E_NOINTERFACE;
    }

    public int ShowObject() => HResult.S_OK;

    public int OnShowWindow(bool fShow) => HResult.S_OK;

    public int RequestNewObjectLayout() => HResult.E_NOTIMPL;

    // ---- IOleWindow (shared by the in-place site and frame) ----
    public int GetWindow(out IntPtr phwnd)
    {
        phwnd = _inPlaceWindow();
        return phwnd == IntPtr.Zero ? HResult.E_FAIL : HResult.S_OK;
    }

    public int ContextSensitiveHelp(bool fEnterMode) => HResult.S_OK;

    // ---- IOleInPlaceSite ----
    public int CanInPlaceActivate() => HResult.S_OK;

    public int OnInPlaceActivate() => HResult.S_OK;

    public int OnUIActivate() => HResult.S_OK;

    public int GetWindowContext(out IOleInPlaceFrame ppFrame, out IntPtr ppDoc, out RECT lprcPosRect,
        out RECT lprcClipRect, ref OLEINPLACEFRAMEINFO lpFrameInfo)
    {
        ppFrame = this;
        ppDoc = IntPtr.Zero;
        lprcPosRect = _positionRect();
        lprcClipRect = lprcPosRect;
        lpFrameInfo.fMDIApp = false;
        lpFrameInfo.hwndFrame = _frameWindow();
        lpFrameInfo.haccel = IntPtr.Zero;
        lpFrameInfo.cAccelEntries = 0;
        return HResult.S_OK;
    }

    public int Scroll(SIZE scrollExtant) => HResult.E_NOTIMPL;

    public int OnUIDeactivate(bool fUndoable) => HResult.S_OK;

    public int OnInPlaceDeactivate() => HResult.S_OK;

    public int DiscardUndoState() => HResult.S_OK;

    public int DeactivateAndUndo() => HResult.S_OK;

    public int OnPosRectChange(ref RECT lprcPosRect) => HResult.S_OK;

    // ---- IOleInPlaceUIWindow / IOleInPlaceFrame (no menu, border or status negotiation) ----
    public int GetBorder(out RECT lprectBorder)
    {
        lprectBorder = default;
        return HResult.INPLACE_E_NOTOOLSPACE;
    }

    public int RequestBorderSpace(ref RECT pborderwidths) => HResult.INPLACE_E_NOTOOLSPACE;

    public int SetBorderSpace(ref RECT pborderwidths) => HResult.S_OK;

    public int SetActiveObject(IntPtr pActiveObject, string? pszObjName) => HResult.S_OK;

    public int InsertMenus(IntPtr hmenuShared, IntPtr lpMenuWidths) => HResult.S_OK;

    public int SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject) => HResult.S_OK;

    public int RemoveMenus(IntPtr hmenuShared) => HResult.S_OK;

    public int SetStatusText(string? pszStatusText) => HResult.S_OK;

    public int EnableModeless(bool fEnable) => HResult.S_OK;

    public int TranslateAccelerator(ref MSG lpmsg, short wID) => HResult.S_FALSE;
}

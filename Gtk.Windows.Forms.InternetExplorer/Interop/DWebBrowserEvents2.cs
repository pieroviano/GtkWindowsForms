using System;
using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.InternetExplorer.Interop;

/// <summary>
/// The outgoing event dispinterface of the WebBrowser ActiveX control. Implemented by the host's event
/// sink and advised through the control's connection point.
/// </summary>
/// <remarks>
/// A dispinterface, not a vtable interface: the control raises events by calling
/// <c>IDispatch::Invoke</c> with a DISPID, so the sink is dispatched by <see cref="System.Runtime.InteropServices.DispIdAttribute" />.
/// Only the events the host acts on are declared; the control tolerates <c>DISP_E_MEMBERNOTFOUND</c> for
/// the rest. The parameter shapes match the type library's, so a VARIANT by reference is <c>ref object</c>
/// and a VARIANT_BOOL by reference is <c>ref bool</c>.
/// </remarks>
[ComVisible(true), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
 InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
internal interface DWebBrowserEvents2
{
    /// <summary>The status bar text changed.</summary>
    [DispId(102)]
    void StatusTextChange([MarshalAs(UnmanagedType.BStr)] string text);

    /// <summary>The load progress advanced.</summary>
    [DispId(108)]
    void ProgressChange(int progress, int progressMax);

    /// <summary>A command's enabled state changed (used for CanGoBack / CanGoForward).</summary>
    [DispId(105)]
    void CommandStateChange(int command, bool enable);

    /// <summary>The document title changed.</summary>
    [DispId(113)]
    void TitleChange([MarshalAs(UnmanagedType.BStr)] string text);

    /// <summary>A navigation is about to start; set <paramref name="cancel" /> to stop it.</summary>
    [DispId(250)]
    void BeforeNavigate2(
        [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
        ref object url,
        ref object flags,
        ref object targetFrameName,
        ref object postData,
        ref object headers,
        ref bool cancel);

    /// <summary>The page asked to open a new window.</summary>
    [DispId(251)]
    void NewWindow2(ref object ppDisp, ref bool cancel);

    /// <summary>A navigation committed.</summary>
    [DispId(252)]
    void NavigateComplete2([MarshalAs(UnmanagedType.IDispatch)] object pDisp, ref object url);

    /// <summary>The document finished loading (fires per frame; the top frame is the one that matters).</summary>
    [DispId(259)]
    void DocumentComplete([MarshalAs(UnmanagedType.IDispatch)] object pDisp, ref object url);
}

/// <summary>Command codes carried by <see cref="DWebBrowserEvents2.CommandStateChange" />.</summary>
internal static class CommandStateChangeCommand
{
    internal const int UpdateBack = 2;
    internal const int UpdateForward = 1;
}

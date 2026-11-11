using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Gtk.Windows.Forms.InternetExplorer.Interop;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>A navigation is starting; a handler may cancel it.</summary>
internal delegate void IeNavigatingHandler(string url, string frame, ref bool cancel);

/// <summary>The page asked to open a new window; a handler may refuse it.</summary>
internal delegate void IeNewWindowHandler(ref bool cancel);

/// <summary>
/// Receives the WebBrowser control's <c>DWebBrowserEvents2</c> notifications and re-raises them as plain
/// CLR events the backend subscribes to.
/// </summary>
/// <remarks>
/// <c>DocumentComplete</c> fires once per frame; only the top frame - the one whose <c>pDisp</c> is the
/// control itself - is forwarded, so a page with iframes raises one completion, as WinForms does. Frame
/// identity is compared by IUnknown pointer, the one reliable COM identity.
/// </remarks>
internal sealed class WebBrowserEventsSink : DWebBrowserEvents2
{
    private readonly object _topBrowser;

    internal WebBrowserEventsSink(object topBrowser)
    {
        _topBrowser = topBrowser;
    }

    internal event IeNavigatingHandler? Navigating;

    internal event IeNewWindowHandler? NewWindow;

    internal event global::System.Action<string>? Navigated;

    internal event global::System.Action<string>? DocumentCompleted;

    internal event global::System.Action<int, int>? ProgressChanged;

    internal event global::System.Action<string>? TitleChanged;

    internal event global::System.Action<string>? StatusChanged;

    internal event global::System.Action<int, bool>? CommandStateChanged;

    public void StatusTextChange(string text) => StatusChanged?.Invoke(text ?? string.Empty);

    public void ProgressChange(int progress, int progressMax) =>
        ProgressChanged?.Invoke(progress, progressMax);

    public void CommandStateChange(int command, bool enable) =>
        CommandStateChanged?.Invoke(command, enable);

    public void TitleChange(string text) => TitleChanged?.Invoke(text ?? string.Empty);

    public void BeforeNavigate2(object pDisp, ref object url, ref object flags, ref object targetFrameName,
        ref object postData, ref object headers, ref bool cancel)
    {
        var handler = Navigating;
        if (handler == null)
        {
            return;
        }

        var target = Convert.ToString(url, CultureInfo.InvariantCulture) ?? string.Empty;
        var frame = Convert.ToString(targetFrameName, CultureInfo.InvariantCulture) ?? string.Empty;
        handler(target, frame, ref cancel);
    }

    public void NewWindow2(ref object ppDisp, ref bool cancel)
    {
        var handler = NewWindow;
        handler?.Invoke(ref cancel);
    }

    public void NavigateComplete2(object pDisp, ref object url) =>
        Navigated?.Invoke(Convert.ToString(url, CultureInfo.InvariantCulture) ?? string.Empty);

    public void DocumentComplete(object pDisp, ref object url)
    {
        if (!IsTopFrame(pDisp))
        {
            return;
        }

        DocumentCompleted?.Invoke(Convert.ToString(url, CultureInfo.InvariantCulture) ?? string.Empty);
    }

    private bool IsTopFrame(object pDisp)
    {
        if (pDisp == null)
        {
            return false;
        }

        var a = IntPtr.Zero;
        var b = IntPtr.Zero;
        try
        {
            a = Marshal.GetIUnknownForObject(pDisp);
            b = Marshal.GetIUnknownForObject(_topBrowser);
            return a == b;
        }
        catch (Exception exception) when (exception is InvalidComObjectException or NullReferenceException)
        {
            return false;
        }
        finally
        {
            if (a != IntPtr.Zero)
            {
                Marshal.Release(a);
            }

            if (b != IntPtr.Zero)
            {
                Marshal.Release(b);
            }
        }
    }
}

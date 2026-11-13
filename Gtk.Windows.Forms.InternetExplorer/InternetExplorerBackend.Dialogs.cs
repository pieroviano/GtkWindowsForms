using System;
using Gtk.Windows.Forms.InternetExplorer.Interop;
using Gtk.Windows.Forms.WebBrowsing;

// This package's namespace is nested under Gtk, so the unqualified names EventHandler and global::System.EventArgs
// bind to GtkSharp's own delegates rather than the BCL ones. Both appear here - the backend seam
// uses System's, GTK widget signals use GTK's - so every use is qualified rather than aliased.

namespace Gtk.Windows.Forms.InternetExplorer;

/// <summary>
/// The print and document dialogs, which Trident really has, routed through <c>IWebBrowser2.ExecWB</c>.
/// These back <see cref="T:System.Windows.Forms.WebBrowser" />'s print and dialog methods, which otherwise
/// report as unsupported.
/// </summary>
public sealed partial class InternetExplorerBackend : IWebBrowserDialogs
{
    /// <inheritdoc />
    void IWebBrowserDialogs.Print() => Exec(OleCmdId.Print, OleCmdExecOpt.DontPromptUser);

    /// <inheritdoc />
    void IWebBrowserDialogs.ShowPrintDialog() => Exec(OleCmdId.Print, OleCmdExecOpt.PromptUser);

    /// <inheritdoc />
    void IWebBrowserDialogs.ShowPrintPreviewDialog() => Exec(OleCmdId.PrintPreview, OleCmdExecOpt.PromptUser);

    /// <inheritdoc />
    void IWebBrowserDialogs.ShowPageSetupDialog() => Exec(OleCmdId.PageSetup, OleCmdExecOpt.PromptUser);

    /// <inheritdoc />
    void IWebBrowserDialogs.ShowPropertiesDialog() => Exec(OleCmdId.Properties, OleCmdExecOpt.PromptUser);

    /// <inheritdoc />
    void IWebBrowserDialogs.ShowSaveAsDialog() => Exec(OleCmdId.SaveAs, OleCmdExecOpt.PromptUser);

    private void Exec(OleCmdId command, OleCmdExecOpt option)
    {
        var id = (int)command;
        var opt = (int)option;
        Drive(browser =>
        {
            object input = Type.Missing;
            object output = Type.Missing;
            browser.ExecWB(id, opt, ref input, ref output);
        });
    }
}

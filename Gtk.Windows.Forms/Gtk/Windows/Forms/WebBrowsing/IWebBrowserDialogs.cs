// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Gtk.Windows.Forms.WebBrowsing;

/// <summary>
/// An optional capability a backend may offer: the print and document dialogs the Internet Explorer
/// <see cref="T:System.Windows.Forms.WebBrowser" /> exposed.
/// </summary>
/// <remarks>
/// Kept separate from <see cref="IWebViewBackend" /> because only an engine that actually has these dialogs
/// implements it. <see cref="T:System.Windows.Forms.WebBrowser" /> checks for it and, when a backend does
/// not provide it, keeps its default behaviour - printing through <c>window.print()</c> and reporting the
/// Internet-Explorer-only dialogs as unsupported. The Trident backend implements this through
/// <c>IWebBrowser2.ExecWB</c>.
/// </remarks>
public interface IWebBrowserDialogs
{
    /// <summary>Prints the document without prompting.</summary>
    void Print();

    /// <summary>Shows the engine's print dialog.</summary>
    void ShowPrintDialog();

    /// <summary>Shows the engine's print preview.</summary>
    void ShowPrintPreviewDialog();

    /// <summary>Shows the engine's page setup dialog.</summary>
    void ShowPageSetupDialog();

    /// <summary>Shows the document's properties dialog.</summary>
    void ShowPropertiesDialog();

    /// <summary>Shows the engine's save-as dialog.</summary>
    void ShowSaveAsDialog();
}

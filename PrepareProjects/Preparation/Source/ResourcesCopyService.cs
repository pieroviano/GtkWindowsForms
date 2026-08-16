using Prepare.Common.IO;

namespace PrepareProjects.Preparation.Source;

public static class ResourcesCopyService
{
    public static void CopyCodeFromResources(string targetDir)
    {
        var sourceFileName = RepoPaths.At(@"Resources\ControlExtension.cs");
        if (File.Exists(sourceFileName))
        {
            LogUtility.FileCopyAndLog(sourceFileName,
                Path.Combine(targetDir, @"GTKControls\ControlExtension.cs"));
        }

        sourceFileName = RepoPaths.At(@"Resources\SystemResources.cs");
        if (File.Exists(sourceFileName))
        {
            var destFileName = Path.Combine(targetDir, @"System\SystemResources.cs");
#pragma warning disable CS8604 // Possible null reference argument.
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
#pragma warning restore CS8604 // Possible null reference argument.
            LogUtility.FileCopyAndLog(sourceFileName, destFileName);
        }

        // AutoClosingMessageBox is not overlaid here: it ships from the hand-maintained
        // Gtk.Windows.Extensions project, which references this library.
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\DialogEventArgs.cs"), Path.Combine(targetDir, @"GTKControls\DialogEventArgs.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\ExtensionsMessageBox.Static.cs"), Path.Combine(targetDir, @"GTKControls\ExtensionsMessageBox.Static.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\IMessageBox.cs"), Path.Combine(targetDir, @"GTKControls\IMessageBox.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\NonStaticMessageBox.cs"), Path.Combine(targetDir, @"GTKControls\NonStaticMessageBox.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\GridCell.cs"), Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\PaintGraphicsEventHandler.cs"), Path.Combine(targetDir, @"GtkControls\ControlBase\PaintGraphicsEventHandler.cs"));

        // The system tray backends behind NotifyIcon. Copied into GTKControls and relocated to
        // System\Windows\Forms by the later namespace-driven reorganisation, exactly as the message box
        // overlays above are. They are overlays rather than a patch because they are wholly new files with
        // no upstream counterpart; only the NotifyIcon rewrite that consumes them is a patch.
        foreach (var trayFile in new[]
                 {
                     "NotifyIconBackend.cs",
                     "TrayMenuModel.cs",
                     "NotifyIconBackend.Win32.cs",
                     "NotifyIconBackend.StatusNotifier.cs",
                     "NotifyIconBackend.DBusMenu.cs",
                     "NotifyIconBackend.Quartz.cs"
                 })
        {
            LogUtility.FileCopyAndLog(RepoPaths.At(Path.Combine("Resources", trayFile)),
                Path.Combine(targetDir, "GTKControls", trayFile));
        }

        // The WebBrowser control, its Html* DOM layer and the engine backends. Overlays rather than a
        // patch for the same reason the tray backends above are: wholly new files with no upstream
        // counterpart. The public types are ported from the mono submodule
        // (mono/mcs/class/System.Windows.Forms/System.Windows.Forms) with the Mozilla/XPCOM backend
        // replaced by IWebViewBackend; the reorganisation step files them by namespace.
        foreach (var webFile in new[]
                 {
                     "WebBrowser.cs", "WebBrowserBase.cs", "WebBrowserSiteBase.cs",
                     "WebBrowserReadyState.cs", "WebBrowserRefreshOption.cs", "WebBrowserEncryptionLevel.cs",
                     "HtmlDocument.cs", "HtmlElement.cs", "HtmlElementCollection.cs",
                     "HtmlWindow.cs", "HtmlWindowCollection.cs", "HtmlHistory.cs",
                     "HtmlElementEventArgs.cs", "HtmlElementErrorEventArgs.cs",
                     "HtmlElementEventHandler.cs", "HtmlElementErrorEventHandler.cs",
                     "HtmlElementInsertionOrientation.cs",
                     "IWebViewBackend.cs", "IHtmlDomBridge.cs", "DomSnapshot.cs",
                     "WebViewBackendFactory.cs", "NullWebViewBackend.cs", "WebViewBase.cs"
                 })
        {
            LogUtility.FileCopyAndLog(RepoPaths.At(Path.Combine("Resources", webFile)),
                Path.Combine(targetDir, "GTKControls", webFile));
        }
    }
}

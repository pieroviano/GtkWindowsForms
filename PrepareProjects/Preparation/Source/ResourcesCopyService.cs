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
        // Net4x.Gtk.Windows.Extensions project, which references this library.
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\DialogEventArgs.cs"), Path.Combine(targetDir, @"GTKControls\DialogEventArgs.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\ExtensionsMessageBox.Static.cs"), Path.Combine(targetDir, @"GTKControls\ExtensionsMessageBox.Static.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\IMessageBox.cs"), Path.Combine(targetDir, @"GTKControls\IMessageBox.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\NonStaticMessageBox.cs"), Path.Combine(targetDir, @"GTKControls\NonStaticMessageBox.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\GridCell.cs"), Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
        LogUtility.FileCopyAndLog(RepoPaths.At(@"Resources\PaintGraphicsEventHandler.cs"), Path.Combine(targetDir, @"GtkControls\ControlBase\PaintGraphicsEventHandler.cs"));
    }
}

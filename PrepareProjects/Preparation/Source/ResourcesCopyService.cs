using Prepare.Common.IO;

namespace PrepareProjects.Preparation.Source;

public static class ResourcesCopyService
{
    public static void CopyCodeFromResources(string targetDir)
    {
        var sourceFileName = Path.GetFullPath(@"..\..\..\..\Resources\ControlExtension.cs");
        if (File.Exists(sourceFileName))
        {
            LogUtility.FileCopyAndLog(sourceFileName,
                Path.Combine(targetDir, @"GTKControls\ControlExtension.cs"));
        }

        sourceFileName = Path.GetFullPath(@"..\..\..\..\Resources\SystemResources.cs");
        if (File.Exists(sourceFileName))
        {
            var destFileName = Path.Combine(targetDir, @"System\SystemResources.cs");
#pragma warning disable CS8604 // Possible null reference argument.
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
#pragma warning restore CS8604 // Possible null reference argument.
            LogUtility.FileCopyAndLog(sourceFileName, destFileName);
        }

        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\AutoClosingMessageBox.cs", Path.Combine(targetDir, @"GTKControls\AutoClosingMessageBox.cs"));
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\DialogEventArgs.cs", Path.Combine(targetDir, @"GTKControls\DialogEventArgs.cs"));
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\ExtensionsMessageBox.Static.cs", Path.Combine(targetDir, @"GTKControls\ExtensionsMessageBox.Static.cs"));
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\IMessageBox.cs", Path.Combine(targetDir, @"GTKControls\IMessageBox.cs"));
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\NonStaticMessageBox.cs", Path.Combine(targetDir, @"GTKControls\NonStaticMessageBox.cs"));
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\GridCell.cs", Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
    }
}

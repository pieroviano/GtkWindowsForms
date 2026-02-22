namespace PrepareProjects.Preparation.Source;

public static class ResourcesCopyService
{
    public static void CopyCodeFromResources(string targetDir)
    {
        if (File.Exists(@"..\..\..\Resources\ControlExtension.cs"))
        {
            File.Copy(@"..\..\..\Resources\ControlExtension.cs",
                Path.Combine(targetDir, @"GTKControls\ControlExtension.cs"));
        }

        if (File.Exists(@"..\..\..\Resources\SystemResources.cs"))
        {
            var destFileName = Path.Combine(targetDir, @"System\SystemResources.cs");
#pragma warning disable CS8604 // Possible null reference argument.
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
#pragma warning restore CS8604 // Possible null reference argument.
            File.Copy(@"..\..\..\Resources\SystemResources.cs",
                destFileName);
        }

        File.Copy(@"..\..\..\Resources\AutoClosingMessageBox.cs", Path.Combine(targetDir, @"GTKControls\AutoClosingMessageBox.cs"));
        File.Copy(@"..\..\..\Resources\DialogEventArgs.cs", Path.Combine(targetDir, @"GTKControls\DialogEventArgs.cs"));
        File.Copy(@"..\..\..\Resources\ExtensionsMessageBox.Static.cs", Path.Combine(targetDir, @"GTKControls\ExtensionsMessageBox.Static.cs"));
        File.Copy(@"..\..\..\Resources\IMessageBox.cs", Path.Combine(targetDir, @"GTKControls\IMessageBox.cs"));
        File.Copy(@"..\..\..\Resources\NonStaticMessageBox.cs", Path.Combine(targetDir, @"GTKControls\NonStaticMessageBox.cs"));

        File.Copy(@"..\..\..\Resources\GridCell.cs", Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
    }
}
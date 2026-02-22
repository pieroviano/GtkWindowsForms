using Prepare.Common.IO;

namespace PrepareProjects.Preparation.Directories;

public static class GtkFoldersService
{
    public static void NormalizeGtkFolders(string csprojPath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;

        RenameFolder(projectDirectory, "GTKSystem", "GtkSystem", csprojPath);
        RenameFolder(projectDirectory, "GTKControls", "GtkControls", csprojPath);

        // After renaming, update .cs files inside GtkControls
        var gtkControlsPath = Path.Combine(projectDirectory, "GtkControls");

        if (Directory.Exists(gtkControlsPath))
        {
            var csFiles = Directory.GetFiles(
                gtkControlsPath,
                "*.cs",
                SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var content = File.ReadAllText(file);

                if (content.Contains("GTKControls"))
                {
                    content = content.Replace("GTKControls", "GtkControls");
                    File.WriteAllText(file, content);
                }
            }
        }

        BackupUtility.CleanBackupEmptyFolders(projectDirectory, BackupUtility.Backup);
    }

    public static void RenameFolder(
        string parentDirectory,
        string oldName,
        string newName, string csprojPath)
    {
        if (FolderService.RenameFolder(parentDirectory, oldName, newName, out var newPath))
        {
            return;
        }

        FilesXmlService.UpdateFilesXmlEntries(parentDirectory, oldName, newName, csprojPath, newPath);
    }
}
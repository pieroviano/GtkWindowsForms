namespace Prepare.Common.IO;

public static class BackupUtility
{

    public static void CleanBackup(string projectDirectory)
    {
        projectDirectory = GetBackupFullPath(projectDirectory);
        if (Directory.Exists(projectDirectory))
        {
            Directory.Delete(projectDirectory, true);
        }
    }

    public static string GetDestFileName(string fullPath)
    {
        var newFullPath = GetBackupFullPath(fullPath);

        var directoryName = Path.GetDirectoryName(newFullPath);
#pragma warning disable CS8604 // Possible null reference argument.
        Directory.CreateDirectory(directoryName);
#pragma warning restore CS8604 // Possible null reference argument.
        File.Copy(fullPath, newFullPath);
        return newFullPath;
    }

    private static string GetBackupFullPath(string fullPath)
    {
        var lastIndexOf = fullPath.LastIndexOf("Net4x.Gtk.Windows.Forms", 
            StringComparison.InvariantCultureIgnoreCase);
        var newFullPath = fullPath;
        if (lastIndexOf >= 0)
        {
            newFullPath = fullPath.Remove(lastIndexOf, "Net4x.Gtk.Windows.Forms".Length)
                .Insert(lastIndexOf, "Net4x.Gtk.Windows.Forms.Backup");
        }

        return newFullPath;
    }

    public static void CleanBackupEmptyFolders(string projectDirectory)
    {
        projectDirectory = GetBackupFullPath(projectDirectory);

        if (!Directory.Exists(projectDirectory))
            return;

        var root = new DirectoryInfo(projectDirectory);

        // Get all directories ordered by depth (deepest first)
        var directories = root
            .GetDirectories("*", SearchOption.AllDirectories)
            .OrderByDescending(d => d.FullName.Length)
            .ToList();

        // Include root at the end
        directories.Add(root);

        foreach (var dir in directories)
        {
            if (!dir.Exists)
                continue;

            bool hasFiles = dir.EnumerateFiles().Any();
            bool hasSubDirs = dir.EnumerateDirectories().Any();

            if (!hasFiles && !hasSubDirs)
            {
                dir.Delete();
            }
        }
    }
}
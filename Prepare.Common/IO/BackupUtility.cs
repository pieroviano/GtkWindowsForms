namespace Prepare.Common.IO;

public static class BackupUtility
{
    public static string LatestPostProcessing => "LatestPostProcessing.xml";
    public static string IntermediatePostProcessing => "IntermediatePostProcessing.xml";
    public static string PostProcessing => "PostProcessing.xml";
    public static string LatestBackup => "LatestBackup";
    public static string IntermediateBackup => "IntermediateBackup";
    public static string Backup => "Backup";
    public static string Files => "Files";

    public static void CleanBackup(string directory, string backupName, string projectName)
    {
        directory = GetBackupFullPath(directory, backupName, projectName);
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    public static void CleanBackupEmptyFolders(string directory, string backupName, string projectName)
    {
        directory = GetBackupFullPath(directory, backupName, projectName);

        CleanEmptyFolders(directory);
    }

    public static void CleanEmptyFolders(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        var root = new DirectoryInfo(directory);

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
            {
                continue;
            }

            var hasFiles = dir.EnumerateFiles().Any();
            var hasSubDirs = dir.EnumerateDirectories().Any();

            if (!hasFiles && !hasSubDirs)
            {
                dir.Delete();
            }
        }
    }

    public static string GetDestFileName(string fullPath, string backupName, string projectName)
    {
        var newFullPath = GetBackupFullPath(fullPath, backupName, projectName);

        var directoryName = Path.GetDirectoryName(newFullPath);
#pragma warning disable CS8604 // Possible null reference argument.
        Directory.CreateDirectory(directoryName);
#pragma warning restore CS8604 // Possible null reference argument.
        LogUtility.FileCopyAndLog(fullPath, newFullPath);
        return newFullPath;
    }

    private static string GetBackupFullPath(string fullPath, string backupName, string projectName)
    {
        var lastIndexOf = fullPath.LastIndexOf(projectName,
            StringComparison.InvariantCultureIgnoreCase);
        var newFullPath = fullPath;
        if (lastIndexOf >= 0)
        {
            newFullPath = fullPath.Remove(lastIndexOf, projectName.Length)
                .Insert(lastIndexOf, $"{projectName}.{backupName}");
        }

        return newFullPath;
    }
}

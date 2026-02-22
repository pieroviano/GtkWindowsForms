namespace Prepare.Common.IO;

public static class DirectoryMover
{
    public static void MoveAllExcept(
        string sourceRoot,
        string destinationRoot,
        string excludedFolderFullPath,
        bool overwrite = true)
    {
        if (!Directory.Exists(sourceRoot))
            throw new DirectoryNotFoundException(sourceRoot);

        Directory.CreateDirectory(destinationRoot);

        var excludedFullPath = Path.GetFullPath(excludedFolderFullPath)
            .TrimEnd(Path.DirectorySeparatorChar);

        MoveDirectoryRecursive(
            new DirectoryInfo(sourceRoot),
            new DirectoryInfo(destinationRoot),
            excludedFullPath,
            overwrite);
    }

    private static void MoveDirectoryRecursive(
        DirectoryInfo sourceDir,
        DirectoryInfo destDir,
        string excludedFullPath,
        bool overwrite)
    {
        var currentSourcePath = sourceDir.FullName.TrimEnd(Path.DirectorySeparatorChar);

        // Skip excluded directory and everything under it
        if (currentSourcePath.StartsWith(excludedFullPath, StringComparison.OrdinalIgnoreCase))
            return;

        Directory.CreateDirectory(destDir.FullName);

        // Move files
        foreach (var file in sourceDir.GetFiles())
        {
            var targetFilePath = Path.Combine(destDir.FullName, file.Name);

            if (File.Exists(targetFilePath))
            {
                if (overwrite)
                    File.Delete(targetFilePath);
                else
                    continue;
            }

            file.MoveTo(targetFilePath);
        }

        // Recurse directories
        foreach (var subDir in sourceDir.GetDirectories())
        {
            var newDestSubDir = new DirectoryInfo(
                Path.Combine(destDir.FullName, subDir.Name));

            MoveDirectoryRecursive(subDir, newDestSubDir, excludedFullPath, overwrite);
        }

        // Remove directory if empty (and not root)
        if (!sourceDir.EnumerateFileSystemInfos().Any())
        {
            sourceDir.Delete();
        }
    }
}
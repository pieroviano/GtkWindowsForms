namespace PrepareProjects.Preparation.Directories;

public static class DrawingDirectoryMover
{
    public static void MoveDrawingDirectory(string source, string destination, string folderToDelete)
    {
        if (!Directory.Exists(source))
            return;

        // Ensure destination exists
        Directory.CreateDirectory(destination);

        // Move all files and subdirectories
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var relativePath = RelativePath.Get(Path.GetFullPath(source), Path.GetFullPath(file));
            var destFile = Path.Combine(destination, relativePath);

#pragma warning disable CS8604 // Possible null reference argument.
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
#pragma warning restore CS8604 // Possible null reference argument.
            File.Move(file, destFile, overwrite: true);
        }

        // Delete the original parent folder recursively
        if (Directory.Exists(folderToDelete))
        {
            Directory.Delete(folderToDelete, recursive: true);
        }
    }
}

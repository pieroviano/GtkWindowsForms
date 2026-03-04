using Prepare.Common.Logging;

namespace Prepare.Common.IO;

public static class FolderService
{
    public static bool RenameFolder(string parentDirectory, string oldName, string newName, out string newPath)
    {
        var oldPath = Path.Combine(parentDirectory, oldName);
        newPath = Path.Combine(parentDirectory, newName);

        return RenameFolder(parentDirectory, oldName, newName, newPath, oldPath);
    }

    public static bool RenameFolder(string parentDirectory, string oldName, string newName, string newPath, string oldPath)
    {
        if (!Directory.Exists(oldPath))
        {
            return true;
        }

        if (!CaseSensitiveChecker.DirectoryExists(newPath))
        {
            try
            {
                var path = oldPath + Guid.NewGuid();
                Directory.Move(oldPath, path);
                oldPath = path;
                if (Directory.Exists(newPath))
                {
                    throw new InvalidOperationException(
                        $"Cannot rename '{oldName}' because '{newName}' already exists.");
                }
            }
            catch (Exception e)
            {
                RedConsole.Write($"Unexpected exception: {e}.");
            }
        }

        if (!CaseSensitiveChecker.DirectoryExists(newPath))
        {
            try
            {
                // Windows is case-insensitive, so do a two-step rename if only casing changes
                var tempPath = Path.Combine(parentDirectory, Guid.NewGuid().ToString());

                Directory.Move(oldPath, tempPath);
                Directory.Move(tempPath, newPath);
            }
            catch (Exception e)
            {
                RedConsole.Write($"Unexpected exception: {e}.");
                if (string.Compare(oldPath, newPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw;
                }
            }
        }

        return false;
    }
}

namespace Prepare.Common.IO;

public static class PathService
{
    public static string ChangeLocationToParentFolder(string path)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        path= Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),"..", "Resources", Path.GetFileName(path)));
#pragma warning restore CS8604 // Possible null reference argument.
        return path;
    }
}

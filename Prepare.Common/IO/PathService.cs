namespace Prepare.Common.IO;

public static class PathService
{
    public static string ChangeLocationToParentFolder(string path)
    {
        path= Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),"..", "Resources", Path.GetFileName(path)));
        return path;
    }
}

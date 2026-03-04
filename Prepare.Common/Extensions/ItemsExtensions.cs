using Prepare.Common.Models;

namespace Prepare.Common.Extensions;

internal static class ItemsExtensions
{
    public static FileItem? GetItem(this List<FileItem> items, string resourceFile, out string fileName)
    {
        fileName = Path.GetFileName(resourceFile);

        bool Predicate(FileItem fileItem, string fileName) => fileItem.Destination.EndsWith(fileName, StringComparison.OrdinalIgnoreCase);

        var name = fileName;
        return items.FirstOrDefault(fileItem => Predicate(fileItem, name));
    }
}
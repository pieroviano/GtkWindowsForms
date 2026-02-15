using System.Diagnostics;
using UpdatePostProcessing.Model;

namespace UpdatePostProcessing.Extensions;

internal static class ItemsExtensions
{
    public static FileItem? GetItem(this List<FileItem> items, string resourceFile, out string fileName)
    {
        fileName = Path.GetFileName(resourceFile);

        bool Predicate(FileItem i, string fileName) => i.Destination.EndsWith(fileName, StringComparison.OrdinalIgnoreCase);
        var name = fileName;
        var item = items
            .FirstOrDefault((i) => Predicate(i, name));
        return item;
    }
}
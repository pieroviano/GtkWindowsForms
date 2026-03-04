using System.Xml.Linq;
using Prepare.Common.Models;

namespace Prepare.Common.IO;

internal static class FileItemsService
{
    public static List<FileItem> GetFileItems(string filesXmlPath)
    {
        var document = XDocument.Load(filesXmlPath);

        var items = document
            .Root?
            .Elements("Item")
            .Select(e =>
            {
                var readAllText = !string.IsNullOrEmpty(e.Value) && File.Exists(e.Value)?File.ReadAllText(e.Value):string.Empty;
                return new FileItem
                {
                    Source = (string?)e.Attribute("Source") ?? string.Empty,
                    Destination = (string?)e.Attribute("Destination") ?? string.Empty,
                    OldSource = readAllText
                };
            })
            .Where(i =>
                i.Source.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
                i.Destination.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<FileItem>();
        return items;
    }
}

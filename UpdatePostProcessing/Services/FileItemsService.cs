using System.Xml.Linq;
using UpdatePostProcessing.Model;

namespace UpdatePostProcessing.Services;

internal static class FileItemsService
{
    public static List<FileItem> GetFileItems(string filesXmlPath)
    {
        var document = XDocument.Load(filesXmlPath);

        var items = document
            .Root?
            .Elements("Item")
            .Select(e => new FileItem
            {
                Source = (string?)e.Attribute("Source") ?? string.Empty,
                Destination = (string?)e.Attribute("Destination") ?? string.Empty,
                OldSource = File.ReadAllText(e.Value)
            })
            .Where(i =>
                i.Source.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
                i.Destination.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<FileItem>();
        return items;
    }
}
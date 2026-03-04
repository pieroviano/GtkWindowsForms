using System.Xml.Linq;

namespace Prepare.Common.IO;

public static class ResxReader
{
    public static List<KeyValuePair<string, string>> ReadLocalizableItemsOrderedByValueLengthDecending(
        string resxPath)
    {
        if (!File.Exists(resxPath))
        {
            throw new FileNotFoundException(
                "Resx file not found.",
                resxPath);
        }

        var doc = XDocument.Load(resxPath);

        var xElement = doc.Root ?? new XElement("");
        var entries =
            xElement
                .Elements("data")
                .Select(data =>
                {
                    var name = (string?)data.Attribute("name");
                    var value = data.Element("value")?.Value;

                    if (string.IsNullOrWhiteSpace(name) || value == null)
                    {
                        return null;
                    }

                    return (KeyValuePair<string, string>?)new KeyValuePair<string, string>(
#pragma warning disable CS8604 // Possible null reference argument.
                        name,
#pragma warning restore CS8604 // Possible null reference argument.
                        value);
                })
                .Where(kv => kv != null)
#pragma warning disable CS8629 // Nullable value type may be null.
                .Select(kv => kv.Value)
#pragma warning restore CS8629 // Nullable value type may be null.
                .OrderByDescending(kv => kv.Value.Length)
                .ToList();

        return entries;
    }
}

using System.Xml.Linq;

namespace Prepare.Common;

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

        var entries =
            doc
                .Root!
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
                        name,
                        value);
                })
                .Where(kv => kv != null)
                .Select(kv => kv!.Value)
                .OrderByDescending(kv => kv.Value.Length)
                .ToList();

        return entries;
    }
}
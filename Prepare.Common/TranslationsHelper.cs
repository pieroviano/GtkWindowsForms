using System.Xml.Linq;

namespace Prepare.Common;

public static class TranslationsHelper
{
    public static List<KeyValuePair<string, string>> LoadTranslations(string path)
    {
        var doc = XDocument.Load(path);

        return doc.Root!
            .Elements("Entry")
            .Select(e => new KeyValuePair<string, string>(
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                e.Element("Chinese").Value,
                e.Element("English").Value))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            .Where(kv => kv.Key.Length > 0)
            // IMPORTANT: longer strings first to avoid partial replacements
            .OrderByDescending(kv => kv.Key.Length)
            .ToList();
    }
}
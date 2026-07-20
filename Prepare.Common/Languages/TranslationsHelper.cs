using System.Xml.Linq;

namespace Prepare.Common.Languages;

public static class TranslationsHelper
{
    public static List<KeyValuePair<string, string>> LoadTranslations(string path)
    {
        var doc = XDocument.Load(path);

        return doc.Root!
            .Elements("Entry")
            .Select(e => new KeyValuePair<string, string>(
                e.Element("Chinese")?.Value ?? string.Empty,
                e.Element("English")?.Value ?? string.Empty))
            // Skip entries with empty Chinese, and skip empty-English (untranslated) entries so
            // that untranslated Chinese is left intact rather than replaced with an empty string.
            .Where(kv => kv.Key.Length > 0 && !string.IsNullOrEmpty(kv.Value))
            // IMPORTANT: longer strings first to avoid partial replacements
            .OrderByDescending(kv => kv.Key.Length)
            .ToList();
    }
}
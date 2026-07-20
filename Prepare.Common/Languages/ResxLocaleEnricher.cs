using System.Xml.Linq;

namespace Prepare.Common.Languages;

/// <summary>
/// Keeps ResX-format locale files (<c>root &gt; data[name] &gt; value</c>) in sync with their
/// <c>zh-Hans</c> source: for every key present in the Chinese source but missing in a target
/// locale, it adds a <b>blank-value</b> <c>data</c> entry (to be filled once), preserving every
/// existing entry. This is the resx-side analogue of <see cref="ChineseDetector"/> enriching
/// <c>Translations.xml</c> with blank English. It never invents keys, so running it when the
/// locales are already complete is a safe no-op.
/// </summary>
public static class ResxLocaleEnricher
{
    /// <summary>
    /// Ensures each locale file has a (possibly blank) <c>data</c> entry for every key present in
    /// the <paramref name="zhHansResxPath"/> source. Returns the number of blank entries added
    /// across all locale files.
    /// </summary>
    public static int EnsureBlankEntriesForSourceKeys(string zhHansResxPath, IEnumerable<string> localeResxPaths)
    {
        if (!File.Exists(zhHansResxPath))
        {
            return 0;
        }

        var sourceKeys = ReadDataNames(zhHansResxPath);
        if (sourceKeys.Count == 0)
        {
            return 0;
        }

        var added = 0;
        foreach (var localePath in localeResxPaths)
        {
            if (File.Exists(localePath))
            {
                added += AddMissingBlankEntries(localePath, sourceKeys);
            }
        }

        return added;
    }

    private static int AddMissingBlankEntries(string resxPath, IReadOnlyCollection<string> keys)
    {
        var doc = XDocument.Load(resxPath, LoadOptions.PreserveWhitespace);
        var root = doc.Root;
        if (root == null)
        {
            return 0;
        }

        var existing = new HashSet<string>(
            root.Elements("data")
                .Select(d => (string?)d.Attribute("name"))
                .Where(n => !string.IsNullOrEmpty(n))
                .Select(n => n!),
            StringComparer.Ordinal);

        var added = 0;
        foreach (var key in keys)
        {
            if (existing.Contains(key))
            {
                continue;
            }

            root.Add(new XElement("data",
                new XAttribute("name", key),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", string.Empty)));
            existing.Add(key);
            added++;
        }

        if (added > 0)
        {
            doc.Save(resxPath);
        }

        return added;
    }

    private static IReadOnlyCollection<string> ReadDataNames(string resxPath)
    {
        var doc = XDocument.Load(resxPath);
        return doc.Root?
            .Elements("data")
            .Select(d => (string?)d.Attribute("name"))
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => n!)
            .ToList() ?? new List<string>();
    }
}

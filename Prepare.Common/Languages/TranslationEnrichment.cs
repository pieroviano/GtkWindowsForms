using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Prepare.Common.Logging;

namespace Prepare.Common.Languages;

/// <summary>
/// Ensures every Chinese string in a source snapshot has a (possibly blank) translation entry, so
/// it can be filled once and cached. Two channels:
/// comments / inline Chinese -> the <c>Translations.xml</c> dictionary (additive merge);
/// resource literals -> the ResX locale files kept in sync with their <c>zh-Hans</c> source
/// (via <see cref="ResxLocaleEnricher"/>).
///
/// The dictionary merge is strictly <b>additive</b>: it keeps every existing entry as-is and only
/// appends new Chinese as blank-English entries. It never drops, reorders, or reformats existing
/// content (unlike <see cref="ChineseDetector"/>, which rebuilds the file from the scan and prunes
/// anything not found).
/// </summary>
public static class TranslationEnrichment
{
    private static readonly Regex ChineseRegex = new(@"[一-鿿]+", RegexOptions.Compiled);

    private static readonly string[] LocaleSuffixes = { ".xml", ".it.xml", ".fr.xml", ".de.xml", ".es.xml" };

    private const string ZhHansSuffix = ".zh-Hans.xml";

    /// <summary>
    /// Adds every Chinese string found under <paramref name="sourceDirectories"/> that is not
    /// already present in <paramref name="translationsXmlPath"/> as a blank-English entry, appended
    /// to the end. Existing entries are preserved verbatim. Returns the number of entries added.
    /// </summary>
    public static int EnrichTranslationsDictionary(string translationsXmlPath, IEnumerable<string> sourceDirectories)
    {
        if (!File.Exists(translationsXmlPath))
        {
            RedConsole.Write($"[AlignProjects] Translations.xml not found: {Path.GetFullPath(translationsXmlPath)} - skipping.");
            return 0;
        }

        // Existing Chinese keys (read-only; the file's content/formatting is left untouched).
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in XDocument.Load(translationsXmlPath).Root?.Elements("Entry") ?? Enumerable.Empty<XElement>())
        {
            var cn = entry.Element("Chinese")?.Value;
            if (!string.IsNullOrEmpty(cn))
            {
                seen.Add(cn!);
            }
        }

        // Collect new Chinese from the sources, in first-seen order.
        var newChinese = new List<string>();
        foreach (var dir in sourceDirectories)
        {
            if (!Directory.Exists(dir))
            {
                RedConsole.Write($"[AlignProjects] Source directory not found: {Path.GetFullPath(dir)} - skipping.");
                continue;
            }

            foreach (var file in EnumerateCsFiles(dir))
            {
                foreach (Match match in ChineseRegex.Matches(File.ReadAllText(file)))
                {
                    if (seen.Add(match.Value))
                    {
                        newChinese.Add(match.Value);
                    }
                }
            }
        }

        if (newChinese.Count == 0)
        {
            return 0;
        }

        AppendBlankEntries(translationsXmlPath, newChinese);
        return newChinese.Count;
    }

    /// <summary>
    /// For every <c>*.zh-Hans.xml</c> resource in <paramref name="resourcesDirectory"/>, ensures its
    /// sibling locale files (invariant + it/fr/de/es, those that exist) have a blank entry for each
    /// key in the Chinese source. Returns the total number of blank entries added.
    /// </summary>
    public static int EnrichResxLocales(string resourcesDirectory)
    {
        if (!Directory.Exists(resourcesDirectory))
        {
            RedConsole.Write($"[AlignProjects] Resources directory not found for resx enrichment: {Path.GetFullPath(resourcesDirectory)} - skipping.");
            return 0;
        }

        var added = 0;
        foreach (var zhHansPath in Directory.EnumerateFiles(resourcesDirectory, "*" + ZhHansSuffix))
        {
            var baseName = Path.GetFileName(zhHansPath);
            baseName = baseName.Substring(0, baseName.Length - ZhHansSuffix.Length);

            var siblings = LocaleSuffixes
                .Select(suffix => Path.Combine(resourcesDirectory, baseName + suffix))
                .Where(File.Exists);

            added += ResxLocaleEnricher.EnsureBlankEntriesForSourceKeys(zhHansPath, siblings);
        }

        return added;
    }

    private static IEnumerable<string> EnumerateCsFiles(string dir) =>
        Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
            .Where(f =>
            {
                var normalized = f.Replace('/', '\\');
                return !normalized.Contains("\\bin\\") && !normalized.Contains("\\obj\\");
            });

    private static void AppendBlankEntries(string path, IReadOnlyList<string> chineseTexts)
    {
        var rawBytes = File.ReadAllBytes(path);
        var hadBom = rawBytes.Length >= 3 && rawBytes[0] == 0xEF && rawBytes[1] == 0xBB && rawBytes[2] == 0xBF;

        var text = File.ReadAllText(path);
        var newline = text.Contains("\r\n") ? "\r\n" : "\n";

        const string closeTag = "</Translations>";
        var idx = text.LastIndexOf(closeTag, StringComparison.Ordinal);
        if (idx < 0)
        {
            RedConsole.Write("[AlignProjects] Translations.xml is missing </Translations> - skipping append.");
            return;
        }

        var sb = new StringBuilder();
        foreach (var cn in chineseTexts)
        {
            sb.Append(newline)                                    // blank line before the entry
              .Append('\t').Append("<Entry>").Append(newline)
              .Append("\t\t").Append("<Chinese>").Append(Escape(cn)).Append("</Chinese>").Append(newline)
              .Append("\t\t").Append("<English></English>").Append(newline)
              .Append('\t').Append("</Entry>").Append(newline);
        }

        File.WriteAllText(path, text.Substring(0, idx) + sb + text.Substring(idx),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: hadBom));
    }

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}

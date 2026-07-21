using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Common.Languages;

/// <summary>
/// The samples-specific post-merge verification (AlignSamples-Plan.md §"Culture-completeness check"):
/// after an alignment, the generated samples must still be fully culture-aware. Three invariants:
///
/// <list type="number">
/// <item>the invariant and <c>zh-Hans</c> resources have identical key sets, and no invariant value is blank;</item>
/// <item>every <c>Properties.&lt;Resource&gt;.&lt;Key&gt;</c> referenced in code exists in both;</item>
/// <item>no residual Chinese outside the files that are *supposed* to hold it - the <c>zh-Hans</c> resource
/// and the Chinese-culture data assets (those with an <c>.en.&lt;ext&gt;</c> sibling).</item>
/// </list>
///
/// Returns a list of human-readable failures; empty means the culture invariant holds.
/// </summary>
public static class SampleCultureVerifier
{
    private static readonly Regex Chinese = new(@"[一-鿿]", RegexOptions.Compiled);

    /// <summary>Files whose whole point is to hold Chinese - never a residual-Chinese failure.</summary>
    private static readonly Regex ChineseResourceFile = new(@"\.zh-Hans\.resx$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>Verifies every generated sample directory. Empty result = the culture invariant holds.</summary>
    public static IReadOnlyList<string> Verify(IEnumerable<string> generatedSampleDirs,
        string resourceBaseName = SampleGlobalizationEnricher.ResourceBaseName)
    {
        var failures = new List<string>();

        foreach (var dir in generatedSampleDirs)
        {
            if (!Directory.Exists(dir))
            {
                continue;
            }

            var name = Path.GetFileName(dir.TrimEnd('\\', '/'));
            var invariantPath = Path.Combine(dir, "Properties", resourceBaseName + ".resx");
            var zhHansPath = Path.Combine(dir, "Properties", resourceBaseName + ".zh-Hans.resx");

            if (!File.Exists(invariantPath) || !File.Exists(zhHansPath))
            {
                failures.Add($"{name}: missing localization resource ({resourceBaseName}.resx / .zh-Hans.resx) - the sample is not culture-aware.");
                continue;
            }

            var invariant = ReadResx(invariantPath);
            var zhHans = ReadResx(zhHansPath);

            foreach (var key in zhHans.Keys.Where(k => !invariant.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal))
            {
                failures.Add($"{name}: key '{key}' exists in zh-Hans but not in the invariant resource.");
            }

            foreach (var key in invariant.Keys.Where(k => !zhHans.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal))
            {
                failures.Add($"{name}: key '{key}' exists in the invariant resource but not in zh-Hans - it would show English under a Chinese UI culture.");
            }

            foreach (var key in invariant.Where(kv => string.IsNullOrWhiteSpace(kv.Value))
                         .Select(kv => kv.Key).OrderBy(k => k, StringComparer.Ordinal))
            {
                failures.Add($"{name}: invariant value for '{key}' is blank - the English translation was never filled in.");
            }

            failures.AddRange(VerifyReferencedKeys(dir, name, resourceBaseName, invariant, zhHans));
            failures.AddRange(VerifyNoResidualChinese(dir, name));
        }

        return failures;
    }

    /// <summary>Every <c>Properties.&lt;Resource&gt;.&lt;Key&gt;</c> the code uses must resolve in both cultures.</summary>
    private static IEnumerable<string> VerifyReferencedKeys(string dir, string sampleName, string resourceBaseName,
        IReadOnlyDictionary<string, string> invariant, IReadOnlyDictionary<string, string> zhHans)
    {
        var reference = new Regex(@"Properties\." + Regex.Escape(resourceBaseName) + @"\.(?<key>[A-Za-z_][A-Za-z0-9_]*)",
            RegexOptions.Compiled);

        var reported = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in SourceFiles(dir))
        {
            foreach (Match match in reference.Matches(File.ReadAllText(file)))
            {
                var key = match.Groups["key"].Value;

                // The generated Designer exposes the accessors themselves; a reference is only orphaned
                // when the resource files don't define it.
                if (!invariant.ContainsKey(key) && reported.Add(key + "|invariant"))
                {
                    yield return $"{sampleName}: code references '{key}' which is missing from the invariant resource ({Path.GetFileName(file)}).";
                }

                if (!zhHans.ContainsKey(key) && reported.Add(key + "|zh"))
                {
                    yield return $"{sampleName}: code references '{key}' which is missing from the zh-Hans resource ({Path.GetFileName(file)}).";
                }
            }
        }
    }

    /// <summary>
    /// Residual Chinese anywhere it should not be. Whitelisted: the <c>zh-Hans</c> resource, and any data
    /// asset that has an <c>.en.&lt;ext&gt;</c> sibling - that pair IS the culture mechanism, so the
    /// Chinese half is expected (generalizes the hardcoded <c>TestData1.json</c> exemption).
    /// </summary>
    private static IEnumerable<string> VerifyNoResidualChinese(string dir, string sampleName)
    {
        foreach (var file in SourceFiles(dir, includeAllText: true))
        {
            var fileName = Path.GetFileName(file);
            if (ChineseResourceFile.IsMatch(fileName) || HasEnglishSibling(file))
            {
                continue;
            }

            var count = Chinese.Matches(File.ReadAllText(file)).Count;
            if (count > 0)
            {
                yield return $"{sampleName}: {Relative(dir, file)} still contains {count} Chinese character(s) - an unfilled translation.";
            }
        }
    }

    /// <summary>Path of <paramref name="file"/> relative to <paramref name="dir"/> (netstandard2.0-safe).</summary>
    private static string Relative(string dir, string file)
    {
        var root = Path.GetFullPath(dir).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(file);
        return full.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? full.Substring(root.Length) : full;
    }

    /// <summary>True when <c>&lt;name&gt;.en.&lt;ext&gt;</c> sits next to the file (or the file IS that sibling).</summary>
    private static bool HasEnglishSibling(string file)
    {
        var directory = Path.GetDirectoryName(file);
        var stem = Path.GetFileNameWithoutExtension(file);
        var extension = Path.GetExtension(file);
        if (string.IsNullOrEmpty(directory) || stem.EndsWith(".en", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return File.Exists(Path.Combine(directory!, stem + ".en" + extension));
    }

    private static IEnumerable<string> SourceFiles(string dir, bool includeAllText = false)
    {
        var textExtensions = new HashSet<string>(
            new[] { ".cs", ".resx", ".json", ".txt", ".csv", ".md", ".xml", ".config" }, StringComparer.OrdinalIgnoreCase);

        return Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
            .Where(f =>
            {
                var normalized = f.Replace('/', '\\');
                if (normalized.IndexOf("\\bin\\", StringComparison.OrdinalIgnoreCase) >= 0
                    || normalized.IndexOf("\\obj\\", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }

                var ext = Path.GetExtension(f);
                return includeAllText
                    ? textExtensions.Contains(ext)
                    : string.Equals(ext, ".cs", StringComparison.OrdinalIgnoreCase);
            })
            .OrderBy(f => f, StringComparer.Ordinal);
    }

    private static IReadOnlyDictionary<string, string> ReadResx(string path)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var data in XDocument.Load(path).Root?.Elements("data") ?? Enumerable.Empty<XElement>())
        {
            var name = (string?)data.Attribute("name");
            if (!string.IsNullOrEmpty(name))
            {
                map[name!] = data.Element("value")?.Value ?? string.Empty;
            }
        }

        return map;
    }
}

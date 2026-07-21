using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Common.Languages;

/// <summary>A resource key the enricher proposes for a newly-found Chinese UI string.</summary>
public sealed class SampleResourceKeyProposal
{
    public SampleResourceKeyProposal(string key, string chinese, string file, int line)
    {
        Key = key;
        Chinese = chinese;
        File = file;
        Line = line;
    }

    /// <summary>The proposed <c>Gtk_&lt;Form&gt;_&lt;hint-or-ordinal&gt;</c> key (operator may rename it at the pause).</summary>
    public string Key { get; }

    /// <summary>The Chinese value, exactly as it must appear in the <c>zh-Hans</c> resource.</summary>
    public string Chinese { get; }

    /// <summary>Upstream file the string was found in.</summary>
    public string File { get; }

    /// <summary>1-based line number in <see cref="File"/>.</summary>
    public int Line { get; }
}

/// <summary>A culture-specific data asset that has no complete English sibling yet.</summary>
public sealed class SampleDataFileGap
{
    public SampleDataFileGap(string chineseFile, string englishSibling, bool created)
    {
        ChineseFile = chineseFile;
        EnglishSibling = englishSibling;
        Created = created;
    }

    /// <summary>The Chinese-culture asset in the overlay directory (what the sample ships as-is).</summary>
    public string ChineseFile { get; }

    /// <summary>The <c>&lt;name&gt;.en.&lt;ext&gt;</c> sibling to translate.</summary>
    public string EnglishSibling { get; }

    /// <summary>True when this run created the sibling; false when it already existed but still holds Chinese.</summary>
    public bool Created { get; }
}

/// <summary>Everything one enrichment pass added or found still outstanding.</summary>
public sealed class SampleEnrichmentResult
{
    public SampleEnrichmentResult(IReadOnlyList<SampleResourceKeyProposal> newKeys,
        IReadOnlyList<string> newDictionaryEntries, IReadOnlyList<SampleDataFileGap> dataFiles)
    {
        NewKeys = newKeys;
        NewDictionaryEntries = newDictionaryEntries;
        DataFiles = dataFiles;
    }

    /// <summary>Keys appended to the sample resx pair (Chinese value + blank English).</summary>
    public IReadOnlyList<SampleResourceKeyProposal> NewKeys { get; }

    /// <summary>Non-resource Chinese (comments, inline code) appended to <c>samplesTranslations.xml</c> as blanks.</summary>
    public IReadOnlyList<string> NewDictionaryEntries { get; }

    /// <summary>Culture-specific data assets whose English sibling is new or still untranslated.</summary>
    public IReadOnlyList<SampleDataFileGap> DataFiles { get; }

    /// <summary>True when the operator has something to fill in before the transform runs.</summary>
    public bool HasWork => NewKeys.Count > 0 || NewDictionaryEntries.Count > 0 || DataFiles.Count > 0;

    /// <summary>Human-readable listing for the manual pause: what was proposed, where it came from.</summary>
    public IReadOnlyList<string> Describe()
    {
        var lines = new List<string>();

        if (NewKeys.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add($"{NewKeys.Count} new resource key(s) proposed (rename freely, then fill the English value):");
            foreach (var k in NewKeys)
            {
                lines.Add($"  {k.Key}");
                lines.Add($"      zh-Hans : {k.Chinese}");
                lines.Add($"      from    : {k.File}:{k.Line}");
            }
        }

        if (NewDictionaryEntries.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add($"{NewDictionaryEntries.Count} non-resource Chinese string(s) added as blank dictionary entries:");
            foreach (var cn in NewDictionaryEntries)
            {
                lines.Add($"  {cn}");
            }
        }

        if (DataFiles.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add($"{DataFiles.Count} culture-specific data file(s) need an English translation:");
            foreach (var d in DataFiles)
            {
                lines.Add($"  {d.EnglishSibling} {(d.Created ? "(created from the Chinese original)" : "(still contains Chinese)")}");
            }
        }

        return lines;
    }
}

/// <summary>
/// The samples' globalization enrichment (AlignSamples-Plan.md Phase 2): finds Chinese that upstream
/// added to the sample apps and makes sure every occurrence has somewhere to be translated <b>before</b>
/// the sample transform <c>fs</c> runs, so the generated samples stay culture-aware:
///
/// <list type="bullet">
/// <item>Chinese in a string literal (or a <c>.resx</c> value) is a UI string → a proposed
/// <c>Gtk_&lt;Form&gt;_…</c> key, with the Chinese value appended to the <c>zh-Hans</c> resource and a
/// <b>blank</b> English value appended to the invariant one.</item>
/// <item>Chinese anywhere else (comments, inline code) → a blank entry in the sample dictionary.</item>
/// <item>Chinese inside a text asset (json/txt/…) → a maintained <c>&lt;name&gt;.en.&lt;ext&gt;</c> sibling
/// in the overlay directory.</item>
/// </list>
///
/// Every write is strictly additive: existing entries are never dropped, reordered or reformatted, the
/// BOM is preserved, and filled translations are cached forever. Unfilled blanks leave the Chinese in
/// place at apply time (<see cref="Translator"/> skips blank-English entries) rather than erasing it.
/// </summary>
public static class SampleGlobalizationEnricher
{
    /// <summary>Base name of the samples' localization resource pair in the overlay directory.</summary>
    public const string ResourceBaseName = "GtkWinFormsAppResources";

    /// <summary>The samples' own translation dictionary (separate from the library's Translations.xml).</summary>
    public const string DictionaryName = "samplesTranslations";

    /// <summary>Text assets scanned for Chinese content; anything else (images, binaries) is copied as-is.</summary>
    public static readonly IReadOnlyList<string> DataFileExtensions = new[] { ".json", ".txt", ".csv", ".md" };

    /// <summary>
    /// Upstream files the sample transform deletes outright - never worth a translation slot, since
    /// nothing they contain reaches a generated sample.
    /// </summary>
    private static readonly string[] DroppedByTransform = { "工程说明.txt" };

    private static readonly Regex ChineseRegex = new(@"[一-鿿]+", RegexOptions.Compiled);

    /// <summary>
    /// Scans the upstream sample directories and appends whatever is new: proposed keys to the resx pair,
    /// blank dictionary entries for non-resource Chinese, and English siblings for Chinese data assets.
    /// </summary>
    /// <param name="repoRoot">Repository root holding the <c>Resources</c> overlay directory.</param>
    /// <param name="sampleSourceDirs">Upstream sample directories (<c>Samples/&lt;name&gt;</c>).</param>
    public static SampleEnrichmentResult Enrich(string repoRoot, IEnumerable<string> sampleSourceDirs)
    {
        var dirs = sampleSourceDirs.ToList();
        var resources = EnrichSampleResources(repoRoot, dirs);
        var data = EnrichSampleDataFiles(repoRoot, dirs);
        return new SampleEnrichmentResult(resources.NewKeys, resources.NewDictionaryEntries, data);
    }

    /// <summary>
    /// The code/resx half: proposes keys for new Chinese UI strings and records non-resource Chinese as
    /// blank dictionary entries. Returns what was added (the data-file list is always empty here).
    /// </summary>
    public static SampleEnrichmentResult EnrichSampleResources(string repoRoot, IEnumerable<string> sampleSourceDirs)
    {
        var resourcesDir = Path.Combine(repoRoot, "Resources");
        var zhHansPath = Path.Combine(resourcesDir, ResourceBaseName + ".zh-Hans.xml");
        var invariantPath = Path.Combine(resourcesDir, ResourceBaseName + ".xml");
        var dictionaryPath = Path.Combine(resourcesDir, DictionaryName + ".xml");

        var knownValues = ReadResxValues(zhHansPath);
        var knownKeys = ReadResxKeys(zhHansPath);
        foreach (var key in ReadResxKeys(invariantPath))
        {
            knownKeys.Add(key);
        }

        var knownDictionary = ReadDictionaryChinese(dictionaryPath);

        var proposals = new List<SampleResourceKeyProposal>();
        var newDictionary = new List<string>();
        var ordinals = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var dir in sampleSourceDirs.Where(Directory.Exists))
        {
            foreach (var file in EnumerateSourceFiles(dir))
            {
                var form = FormNameOf(file);
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    foreach (var found in ScanLine(lines[i], Path.GetExtension(file)))
                    {
                        if (found.IsResourceString)
                        {
                            if (!knownValues.Add(found.Text))
                            {
                                continue; // already keyed in the zh-Hans resource
                            }

                            var key = ProposeKey(form, found.Text, knownKeys, ordinals);
                            knownKeys.Add(key);
                            proposals.Add(new SampleResourceKeyProposal(key, found.Text, file, i + 1));
                        }
                        else if (knownDictionary.Add(found.Text))
                        {
                            newDictionary.Add(found.Text);
                        }
                    }
                }
            }
        }

        if (proposals.Count > 0)
        {
            AppendResxEntries(zhHansPath, proposals.Select(p => new KeyValuePair<string, string>(p.Key, p.Chinese)).ToList());
            AppendResxEntries(invariantPath, proposals.Select(p => new KeyValuePair<string, string>(p.Key, string.Empty)).ToList());
        }

        if (newDictionary.Count > 0)
        {
            AppendDictionaryEntries(dictionaryPath, newDictionary);
        }

        return new SampleEnrichmentResult(proposals, newDictionary, Array.Empty<SampleDataFileGap>());
    }

    /// <summary>
    /// The data-asset half: generalizes the hand-maintained <c>TestData1.json</c> /
    /// <c>TestData1.en.json</c> pair. For every Chinese-bearing text asset in the samples, ensures the
    /// overlay directory holds both the Chinese original and an <c>&lt;name&gt;.en.&lt;ext&gt;</c> sibling.
    /// An existing sibling is never overwritten - it is only reported when it still contains Chinese.
    /// </summary>
    public static IReadOnlyList<SampleDataFileGap> EnrichSampleDataFiles(string repoRoot, IEnumerable<string> sampleSourceDirs)
    {
        var resourcesDir = Path.Combine(repoRoot, "Resources");
        Directory.CreateDirectory(resourcesDir);

        var gaps = new List<SampleDataFileGap>();
        var handled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in sampleSourceDirs.Where(Directory.Exists))
        {
            foreach (var file in EnumerateDataFiles(dir))
            {
                var name = Path.GetFileName(file);
                if (IsEnglishSibling(name) || !handled.Add(name))
                {
                    continue;
                }

                var text = File.ReadAllText(file);
                if (!ChineseRegex.IsMatch(text))
                {
                    continue;
                }

                // The Chinese-culture copy lives in the overlay dir so the fork owns it (upstream may
                // change it; the overlay is what the transform actually ships).
                var chineseOverlay = Path.Combine(resourcesDir, name);
                if (!File.Exists(chineseOverlay))
                {
                    File.Copy(file, chineseOverlay);
                }

                var siblingName = Path.GetFileNameWithoutExtension(name) + ".en" + Path.GetExtension(name);
                var sibling = Path.Combine(resourcesDir, siblingName);
                if (!File.Exists(sibling))
                {
                    // Seeded with the Chinese content, not blanked: the file must stay structurally valid
                    // (it is loaded by the sample), and an unfilled translation showing Chinese is exactly
                    // the guarantee the rest of the pipeline makes.
                    File.Copy(chineseOverlay, sibling);
                    gaps.Add(new SampleDataFileGap(chineseOverlay, sibling, true));
                }
                else if (ChineseRegex.IsMatch(File.ReadAllText(sibling)))
                {
                    gaps.Add(new SampleDataFileGap(chineseOverlay, sibling, false));
                }
            }
        }

        return gaps;
    }

    // --- scanning ---------------------------------------------------------------------------------

    private sealed class Found
    {
        public Found(string text, bool isResourceString)
        {
            Text = text;
            IsResourceString = isResourceString;
        }

        public string Text { get; }

        /// <summary>True for a UI string (a whole string literal / resx value) → gets a resource key.</summary>
        public bool IsResourceString { get; }
    }

    /// <summary>
    /// Splits one line's Chinese into the two channels. A Chinese run inside a double-quoted literal
    /// yields the WHOLE literal (that is what the transform maps onto a resource reference); anything
    /// else - comments, inline code - yields the bare Chinese run for the dictionary.
    /// </summary>
    private static IEnumerable<Found> ScanLine(string line, string extension)
    {
        if (string.Equals(extension, ".resx", StringComparison.OrdinalIgnoreCase))
        {
            // <value>中文</value> - a UI string by construction.
            foreach (Match value in Regex.Matches(line, @"<value>(?<v>[^<]*)</value>"))
            {
                var text = value.Groups["v"].Value;
                if (ChineseRegex.IsMatch(text))
                {
                    yield return new Found(text, true);
                }
            }

            yield break;
        }

        var literals = new List<(int start, int end, string text)>();
        foreach (Match literal in Regex.Matches(line, "\"(?<v>(?:[^\"\\\\]|\\\\.)*)\""))
        {
            literals.Add((literal.Index, literal.Index + literal.Length, literal.Groups["v"].Value));
        }

        var reportedLiterals = new HashSet<int>();
        foreach (Match match in ChineseRegex.Matches(line))
        {
            var index = literals.FindIndex(l => match.Index > l.start && match.Index < l.end);
            if (index >= 0)
            {
                if (reportedLiterals.Add(index) && ChineseRegex.IsMatch(literals[index].text))
                {
                    yield return new Found(literals[index].text, true);
                }
            }
            else
            {
                yield return new Found(match.Value, false);
            }
        }
    }

    private static IEnumerable<string> EnumerateSourceFiles(string dir) =>
        Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
            .Where(f =>
            {
                var ext = Path.GetExtension(f);
                return (string.Equals(ext, ".cs", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(ext, ".resx", StringComparison.OrdinalIgnoreCase))
                       && !IsBuildOutput(f);
            })
            .OrderBy(f => f, StringComparer.Ordinal);

    private static IEnumerable<string> EnumerateDataFiles(string dir) =>
        Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
            .Where(f => DataFileExtensions.Any(e => string.Equals(Path.GetExtension(f), e, StringComparison.OrdinalIgnoreCase))
                        && !IsBuildOutput(f)
                        && !DroppedByTransform.Contains(Path.GetFileName(f), StringComparer.OrdinalIgnoreCase))
            .OrderBy(f => f, StringComparer.Ordinal);

    /// <summary>True for a <c>&lt;name&gt;.en.&lt;ext&gt;</c> file - the translation, not a source to translate.</summary>
    private static bool IsEnglishSibling(string fileName) =>
        Path.GetFileNameWithoutExtension(fileName).EndsWith(".en", StringComparison.OrdinalIgnoreCase);

    private static bool IsBuildOutput(string path)
    {
        var normalized = path.Replace('/', '\\');
        return normalized.IndexOf("\\bin\\", StringComparison.OrdinalIgnoreCase) >= 0
               || normalized.IndexOf("\\obj\\", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    // --- key proposal -----------------------------------------------------------------------------

    /// <summary>The form/class a file belongs to: <c>Form1.Designer.cs</c> → <c>Form1</c>.</summary>
    private static string FormNameOf(string file)
    {
        var name = Path.GetFileName(file);
        var dot = name.IndexOf('.');
        if (dot > 0)
        {
            name = name.Substring(0, dot);
        }

        return Sanitize(name);
    }

    /// <summary>
    /// <c>Gtk_&lt;Form&gt;_&lt;hint&gt;</c>, matching the hand-authored style. The hint is the ASCII
    /// content of the string when there is any (<c>datatable数据源</c> → <c>datatable</c>); otherwise a
    /// per-form ordinal, as in the existing <c>Gtk_MainForm_Menu_1</c>. Collisions get <c>_2</c>, <c>_3</c>…
    /// </summary>
    private static string ProposeKey(string form, string chinese, ISet<string> knownKeys, IDictionary<string, int> ordinals)
    {
        var hint = Sanitize(new string(chinese.Where(c => c < 128).ToArray()));
        hint = hint.Trim('_');
        if (hint.Length > 40)
        {
            hint = hint.Substring(0, 40).TrimEnd('_');
        }

        if (hint.Length == 0)
        {
            ordinals.TryGetValue(form, out var ordinal);
            ordinal++;
            ordinals[form] = ordinal;
            hint = ordinal.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        var baseKey = $"Gtk_{form}_{hint}";
        var key = baseKey;
        var suffix = 1;
        while (knownKeys.Contains(key))
        {
            suffix++;
            key = $"{baseKey}_{suffix}";
        }

        return key;
    }

    /// <summary>Keeps only key-safe characters, collapsing every run of the rest into a single underscore.</summary>
    private static string Sanitize(string text)
    {
        var sb = new StringBuilder(text.Length);
        var lastWasSeparator = false;
        foreach (var c in text)
        {
            if (char.IsLetterOrDigit(c) && c < 128 || c == '_')
            {
                sb.Append(c);
                lastWasSeparator = false;
            }
            else if (!lastWasSeparator && sb.Length > 0)
            {
                sb.Append('_');
                lastWasSeparator = true;
            }
        }

        return sb.ToString().Trim('_');
    }

    // --- additive writes --------------------------------------------------------------------------

    private static HashSet<string> ReadResxValues(string path) =>
        ReadResx(path, d => d.Element("value")?.Value);

    private static HashSet<string> ReadResxKeys(string path) =>
        ReadResx(path, d => (string?)d.Attribute("name"));

    private static HashSet<string> ReadResx(string path, Func<XElement, string?> select)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        if (!File.Exists(path))
        {
            return set;
        }

        foreach (var data in XDocument.Load(path).Root?.Elements("data") ?? Enumerable.Empty<XElement>())
        {
            var value = select(data);
            if (!string.IsNullOrEmpty(value))
            {
                set.Add(value!);
            }
        }

        return set;
    }

    private static HashSet<string> ReadDictionaryChinese(string path)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        if (!File.Exists(path))
        {
            return set;
        }

        foreach (var entry in XDocument.Load(path).Root?.Elements("Entry") ?? Enumerable.Empty<XElement>())
        {
            var cn = entry.Element("Chinese")?.Value;
            if (!string.IsNullOrEmpty(cn))
            {
                set.Add(cn!);
            }
        }

        return set;
    }

    /// <summary>
    /// Appends <c>data</c> elements as TEXT just before <c>&lt;/root&gt;</c>. Deliberately not an
    /// XDocument round-trip: saving would reformat the whole 150-entry file and bury the real change.
    /// </summary>
    private static void AppendResxEntries(string path, IReadOnlyList<KeyValuePair<string, string>> entries)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var (text, newline, hadBom) = ReadForAppend(path);
        const string closeTag = "</root>";
        var idx = text.LastIndexOf(closeTag, StringComparison.Ordinal);
        if (idx < 0)
        {
            return;
        }

        var sb = new StringBuilder();
        foreach (var entry in entries)
        {
            sb.Append("  ").Append("<data name=\"").Append(entry.Key).Append("\" xml:space=\"preserve\">").Append(newline)
              .Append("    ").Append("<value>").Append(Escape(entry.Value)).Append("</value>").Append(newline)
              .Append("  ").Append("</data>").Append(newline);
        }

        Write(path, text.Substring(0, idx) + sb + text.Substring(idx), hadBom);
    }

    /// <summary>Appends blank-English dictionary entries, same additive contract as <see cref="TranslationEnrichment"/>.</summary>
    private static void AppendDictionaryEntries(string path, IReadOnlyList<string> chineseTexts)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var (text, newline, hadBom) = ReadForAppend(path);
        const string closeTag = "</Translations>";
        var idx = text.LastIndexOf(closeTag, StringComparison.Ordinal);
        if (idx < 0)
        {
            return;
        }

        var sb = new StringBuilder();
        foreach (var cn in chineseTexts)
        {
            sb.Append('\t').Append("<Entry>").Append(newline)
              .Append("\t\t").Append("<Chinese>").Append(Escape(cn)).Append("</Chinese>").Append(newline)
              .Append("\t\t").Append("<English></English>").Append(newline)
              .Append('\t').Append("</Entry>").Append(newline);
        }

        Write(path, text.Substring(0, idx) + sb + text.Substring(idx), hadBom);
    }

    private static (string text, string newline, bool hadBom) ReadForAppend(string path)
    {
        var rawBytes = File.ReadAllBytes(path);
        var hadBom = rawBytes.Length >= 3 && rawBytes[0] == 0xEF && rawBytes[1] == 0xBB && rawBytes[2] == 0xBF;
        var text = File.ReadAllText(path);
        var newline = text.IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        return (text, newline, hadBom);
    }

    private static void Write(string path, string content, bool hadBom) =>
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: hadBom));

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}

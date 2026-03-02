using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Common.Languages;

public static class ChineseDetector
{
    public static void DetectChinese(string inputPath,
        bool saveAsText = false)
    {
        DetectChinese(false, inputPath, true, saveAsText);
    }

    public static void DetectChinese(
        bool isDirectory,
        string inputPath,
        bool isJsonFile,
        bool saveAsText = false
    )
    {
        var chineseMatches = GetChineseMatches(isDirectory, inputPath);

        // ---- Output path ----
        string outputFileName;

        if (saveAsText)
        {
            outputFileName = Path.ChangeExtension(
                isJsonFile ? "JsonTranslations.xml" : "Translations.xml",
                ".txt"
            );
        }
        else
        {
            outputFileName = isJsonFile ? "JsonTranslations.xml" : "Translations.xml";
        }

        var outputPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Resources", outputFileName)
        );

        // ---- TEXT OUTPUT ----
        if (saveAsText)
        {
            File.WriteAllLines(outputPath, chineseMatches
                .Distinct()
                .OrderByDescending(s => s.Length)
                .ToArray());
            ColoredConsole.WriteInColor("Chinese text file generated (ordering preserved).", ConsoleColor.White, true);
            ColoredConsole.WriteInColor(outputPath, ConsoleColor.White, true);
            return;
        }

        // ---- XML OUTPUT (existing behavior) ----
        var extractedChinese = chineseMatches
            .Distinct()
            .OrderByDescending(s => s.Length)
            .ToList();

        Dictionary<string, string> existingTranslations = new();

        if (File.Exists(outputPath))
        {
            var existingDoc = XDocument.Load(outputPath);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var entry in existingDoc.Root.Elements("Entry"))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            {
                var chinese = entry.Element("Chinese")?.Value;
                var english = entry.Element("English")?.Value ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(chinese))
                {
                    existingTranslations[chinese] = english;
                }
            }
        }

        var root = new XElement("Translations");

        foreach (var chineseText in extractedChinese)
        {
            existingTranslations.TryGetValue(chineseText, out var englishText);

            root.Add(
                new XElement("Entry",
                    new XElement("Chinese", chineseText),
                    new XElement("English", englishText ?? string.Empty)
                )
            );
        }

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            root
        );

        doc.Save(outputPath);

        ColoredConsole.WriteInColor("Translations file updated safely (existing translations preserved).", ConsoleColor.White, true);
        ColoredConsole.WriteInColor(outputPath, ConsoleColor.White, true);
    }

    private static List<string> GetChineseMatches(bool isDirectory, string inputPath)
    {
        var chineseRegex = new Regex(@"[\u4E00-\u9FFF]+", RegexOptions.Compiled);
        List<string> allMatches = new();

        if (isDirectory)
        {
            foreach (var file in Directory.EnumerateFiles(inputPath, "*.cs", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);

                foreach (Match match in chineseRegex.Matches(content))
                {
                    allMatches.Add(match.Value);
                }
            }
        }
        else
        {
            var content = File.ReadAllText(inputPath);

            foreach (Match match in chineseRegex.Matches(content))
            {
                allMatches.Add(match.Value);
            }
        }

        return allMatches;
    }
}

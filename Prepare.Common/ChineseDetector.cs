using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Common;

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
        List<string> chineseMatches = GetChineseMatches(isDirectory, inputPath);

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

        string outputPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\PrepareProjects", outputFileName)
        );

        // ---- TEXT OUTPUT ----
        if (saveAsText)
        {
            File.WriteAllLines(outputPath, chineseMatches
                .Distinct()
                .OrderByDescending(s => s.Length)
                .ToArray());
            Console.WriteLine("Chinese text file generated (ordering preserved).");
            Console.WriteLine(outputPath);
            return;
        }

        // ---- XML OUTPUT (existing behavior) ----
        List<string> extractedChinese = chineseMatches
            .Distinct()
            .OrderByDescending(s => s.Length)
            .ToList();

        Dictionary<string, string> existingTranslations = new();

        if (File.Exists(outputPath))
        {
            XDocument existingDoc = XDocument.Load(outputPath);

            foreach (var entry in existingDoc.Root.Elements("Entry"))
            {
                string chinese = entry.Element("Chinese")?.Value;
                string english = entry.Element("English")?.Value ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(chinese))
                {
                    existingTranslations[chinese] = english;
                }
            }
        }

        XElement root = new XElement("Translations");

        foreach (string chineseText in extractedChinese)
        {
            existingTranslations.TryGetValue(chineseText, out string englishText);

            root.Add(
                new XElement("Entry",
                    new XElement("Chinese", chineseText),
                    new XElement("English", englishText ?? string.Empty)
                )
            );
        }

        XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            root
        );

        doc.Save(outputPath);

        Console.WriteLine("Translations file updated safely (existing translations preserved).");
        Console.WriteLine(outputPath);
    }

    private static List<string> GetChineseMatches(bool isDirectory, string inputPath)
    {
        Regex chineseRegex = new Regex(@"[\u4E00-\u9FFF]+", RegexOptions.Compiled);
        List<string> allMatches = new();

        if (isDirectory)
        {
            foreach (string file in Directory.EnumerateFiles(inputPath, "*.cs", SearchOption.AllDirectories))
            {
                string content = File.ReadAllText(file);

                foreach (Match match in chineseRegex.Matches(content))
                {
                    allMatches.Add(match.Value);
                }
            }
        }
        else
        {
            string content = File.ReadAllText(inputPath);

            foreach (Match match in chineseRegex.Matches(content))
            {
                allMatches.Add(match.Value);
            }
        }

        return allMatches;
    }
}

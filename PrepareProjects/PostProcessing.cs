using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using PrepareProjects.Model;

namespace PrepareProjects;

public static class PostProcessing
{
    public static void ApplyPostProcessing(string targetDir, string[] exclude)
    {
        var xmlPath = "PostProcessing.xml";

        if (!File.Exists(xmlPath))
        {
            Console.WriteLine("PostProcessing.xml not found.");
            return;
        }

        var doc = XDocument.Load(xmlPath);

        var replacements = doc
            .Root?
            .Elements("Item")
            .Select(i => new Replacement(i.Element("FileName")?.Value,
                i.Element("SourceString")?.Value, i.Element("TargetString")?.Value))
            .Where(i =>
                !string.IsNullOrWhiteSpace(i.FileName) &&
                !string.IsNullOrEmpty(i.Source) &&
                !string.IsNullOrEmpty(i.Target))
            .ToList();

        if (replacements == null || replacements.Count == 0)
        {
            Console.WriteLine("No post-processing items found.");
            return;
        }

        int applied = 0;

        string[] fileNames;
        foreach (var replacement in replacements)
        {
            fileNames = GetFileNames(targetDir, replacement, exclude);

            foreach (var path in fileNames)
            {
                applied = ProcessFile(path, replacement, applied);
            }
        }

        Console.WriteLine($"Post-processing applied: {applied} replacement(s).");
    }

    private static string[] GetFileNames(string targetDir, Replacement replacement, string[] exclude)
    {
        string[] fileNames;
        var filePath = Path.Combine(targetDir, replacement.FileName!);
        var fullPath = Path.GetFullPath(Path.GetDirectoryName(filePath));
        var searchPattern = Path.GetFileName(filePath);
        if (!searchPattern.Contains("*") && !searchPattern.Contains("*") && !fullPath.Contains("*") &&
            !fullPath.Contains("*"))
        {
            fileNames = [Path.Combine(fullPath, searchPattern)];
        }
        else
        {
            fileNames = Directory.GetFiles(fullPath, searchPattern, SearchOption.AllDirectories).Where(f=>
            {
                return !exclude.Any(v=>f.Contains(v));
            }).ToArray();
        }

        return fileNames;
    }

    private static Regex BuildRegexFromTemplate(string source)
    {
        var markerRegex = new Regex(@"\$RegEx\$\((.*?)\)");
        var sb = new StringBuilder();

        int lastIndex = 0;

        foreach (Match match in markerRegex.Matches(source))
        {
            // Escape literal text before the marker
            var literalPart = source.Substring(lastIndex, match.Index - lastIndex);
            sb.Append(Regex.Escape(literalPart));

            // Insert raw regex from inside $RegEx$(...)
            sb.Append(match.Groups[1].Value);

            lastIndex = match.Index + match.Length;
        }

        // Escape remaining literal text
        sb.Append(Regex.Escape(source.Substring(lastIndex)));

        return new Regex(sb.ToString());
    }


private static int ProcessFile(string filePath, Replacement replacement, int applied)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Skipped (file not found): {replacement.FileName}");
            return applied;
        }

        var text = File.ReadAllText(filePath);
        var source = (string)replacement.Source;
        var target = replacement.Target;

        string newText = text;

        // Detect embedded $RegEx$(...)
        var markerRegex = new Regex(@"\$RegEx\$\((.*?)\)");

        var match = markerRegex.Match(source);

        if (match.Success)
        {
            // Extract the actual regex pattern
            var regex = BuildRegexFromTemplate(source);

            if (!regex.IsMatch(text))
                return applied;

            newText = regex.Replace(text, target);
        }
        else
        {
            if (!text.Contains(source))
                return applied;

            newText = text.Replace(source, target);
        }

        if (!string.Equals(text, newText, StringComparison.Ordinal))
        {
            File.WriteAllText(filePath, newText);
            applied++;
            Console.WriteLine($"Updated: {filePath}");
        }

        return applied;
    }
}
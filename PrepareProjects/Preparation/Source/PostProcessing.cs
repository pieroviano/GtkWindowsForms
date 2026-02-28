using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Prepare.Common.Text;
using PrepareProjects.Model;

namespace PrepareProjects.Preparation.Source;

public static class PostProcessing
{
    public static void ApplyPostProcessing(string targetDir, string[] exclude, string postprocessingXml)
    {
        if (!File.Exists(postprocessingXml))
        {
            Console.WriteLine(@$"{postprocessingXml} not found.");
            return;
        }

        var doc = XDocument.Load(postprocessingXml);

        var replacements = doc
            .Root?
            .Elements("Item")
            .Select(i => new Replacement(i.Element("FileName")?.Value,
                i.Element("SourceString")?.Value, i.Element("TargetString")?.Value))
            .Where(i =>
                !string.IsNullOrWhiteSpace(i.FileName))
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
            fileNames = Directory.GetFiles(fullPath, searchPattern, SearchOption.AllDirectories).Where(f =>
            {
                return !exclude.Any(v => f.Contains(v));
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

        return new Regex(sb.ToString(), RegexOptions.Singleline);
    }


    private static int ProcessFile(string filePath, Replacement replacement, int applied)
    {
        if (!File.Exists(filePath))
        {
            ConsoleUtilityEx.WriteInColor($"Skipped (file not found): {replacement.FileName}", ConsoleColor.Yellow);
            return applied;
        }

        var source = (string)replacement.Source;
        var target = replacement.Target;
        if (File.Exists(filePath) && string.IsNullOrEmpty(source) && string.IsNullOrEmpty(target))
        {
            File.Delete(filePath);
            applied++;
            return applied;
        }
        var text = File.ReadAllText(filePath);


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
            markerRegex = new Regex(@"\$CommentedCode\$\(.*\)");
            match = markerRegex.Match(source);

            if (match.Success)
            {
                var commentedCode=$"//{match.Value.TrimEnd(')').Substring(match.Value.IndexOf("$CommentedCode$(") + "$CommentedCode$(".Length)}";
                
                newText = CommentCleaner.RemoveCommentBlock(text, commentedCode);
            }
            else
            {
                if (!text.Contains(source))
                    return applied;

                newText = text.Replace(source, target);
            }
        }

        if (!string.Equals(text, newText, StringComparison.Ordinal))
        {
            File.WriteAllText(filePath, newText);
            applied++;
            ConsoleUtilityEx.WriteInColor($"Updated: {filePath}", ConsoleColor.White);
        }

        return applied;
    }
}

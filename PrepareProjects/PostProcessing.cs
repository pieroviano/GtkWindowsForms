using System.Xml.Linq;
using PrepareProjects.Model;

namespace PrepareProjects;

public static class PostProcessing
{
    public static void ApplyPostProcessing(string targetDir)
    {
        var xmlPath = "PostProcessing.xml";

        if (!File.Exists(xmlPath))
        {
            Console.WriteLine("PostProcessing.xml not found.");
            return;
        }

        var doc = XDocument.Load(xmlPath);

        var items = doc
            .Root?
            .Elements("Item")
            .Select(i => new Replacement(i.Element("FileName")?.Value, 
                i.Element("SourceString")?.Value, i.Element("TargetString")?.Value))
            .Where(i =>
                !string.IsNullOrWhiteSpace(i.FileName) &&
                !string.IsNullOrEmpty(i.Source) &&
                !string.IsNullOrEmpty(i.Target))
            .ToList();

        if (items == null || items.Count == 0)
        {
            Console.WriteLine("No post-processing items found.");
            return;
        }

        int applied = 0;

        string[] strings;
        foreach (var item in items)
        {
            var filePath = Path.Combine(targetDir, item.FileName!);
            var fullPath = Path.GetFullPath(Path.GetDirectoryName(filePath));
            var searchPattern = Path.GetFileName(filePath);
            if (!searchPattern.Contains("*") && !searchPattern.Contains("*") && !fullPath.Contains("*") &&
                !fullPath.Contains("*"))
            {
                strings = [Path.Combine(fullPath, searchPattern)];
            }
            else
            {
                strings = Directory.GetFiles(fullPath, searchPattern, SearchOption.AllDirectories);
            }

            foreach (var s in strings)
            {
                applied = ProcessFile(s, item, applied);
            }
        }

        Console.WriteLine($"Post-processing applied: {applied} replacement(s).");
    }

    private static int ProcessFile(string filePath, Replacement item, int applied)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Skipped (file not found): {item.FileName}");
            return applied;
        }

        var text = File.ReadAllText(filePath);

        if (!text.Contains((string)item.Source!))
            return applied;

        var newText = text.Replace((string)item.Source!, item.Target!);

        if (!ReferenceEquals(text, newText) && text != newText)
        {
            File.WriteAllText(filePath, newText);
            applied++;
            Console.WriteLine($"Updated: {filePath}");
        }

        return applied;
    }


}
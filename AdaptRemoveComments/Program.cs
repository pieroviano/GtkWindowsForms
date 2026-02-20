using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AdaptRemoveComments;

internal static class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: AdaptRemoveComments <file.xml>");
            return 1;
        }

        var filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return 1;
        }

        try
        {
            var doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            // Transform all SourceString elements containing //
            var sourceElements = doc.Descendants("SourceString");
            foreach (var element in sourceElements)
            {
                if (element.Value.Contains("//"))
                {
                    element.Value = TransformSourceString(element.Value);
                }
            }

            // Group by FileName, remove duplicates in SourceString
            var items = doc.Root.Elements("Item").ToList();

            var cleanedItems = items
                .GroupBy(item => (string)item.Element("FileName"))
                .SelectMany(g =>
                {
                    // Track unique SourceString within this FileName
                    var seen = new HashSet<string>();
                    return g.Where(item =>
                    {
                        string sourceValue = (string)item.Element("SourceString");
                        if (seen.Contains(sourceValue))
                            return false;
                        seen.Add(sourceValue);
                        return true;
                    });
                })
                .OrderByDescending(item => (string)item.Element("FileName"))
                .ToList();

            // Replace all items with the cleaned, sorted list
            doc.Root.ReplaceNodes(cleanedItems);

            doc.Save(filePath);
            Console.WriteLine("Transformation completed successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static string TransformSourceString(string input)
    {
        // Remove $RegEx$(...)
        input = Regex.Replace(input, @"\$RegEx\$\([^\)]*\)", "");

        int commentIndex = input.IndexOf("//");
        if (commentIndex < 0)
            return input;

        // Everything after //
        string afterSlashes = input.Substring(commentIndex + 2);

        // Scan for first meaningful token
        var tokenMatch = Regex.Match(afterSlashes, @"([ \t]*)([A-Za-z_][A-Za-z0-9_]*)([ \.]?)");

        if (!tokenMatch.Success)
            return "$CommentedCode$()";

        string leadingWhitespace = tokenMatch.Groups[1].Value; // preserve spaces/tabs
        string token = tokenMatch.Groups[2].Value;
        string trailingChar = tokenMatch.Groups[3].Value; // space or dot if exists

        return $"$CommentedCode$({leadingWhitespace}{token}{trailingChar})";
    }
}
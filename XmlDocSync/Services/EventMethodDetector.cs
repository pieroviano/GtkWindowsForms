using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class EventMethodDetector
{
    public static IEnumerable<string> DetectFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return Enumerable.Empty<string>();

        var matchingFiles = new List<string>();

        // Get all .cs files recursively
        var files = Directory.EnumerateFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            string code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var walker = new EventMethodWalker();
            walker.Visit(root);

            if (walker.FoundMatch)
            {
                matchingFiles.Add(file);
            }
        }

        return matchingFiles;
    }
}

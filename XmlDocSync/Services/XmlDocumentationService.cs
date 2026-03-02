using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prepare.Common.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using XmlDocSync.Cleaning;
using XmlDocSync.Formatters;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class XmlDocumentationService
{
    public static async Task<int> MergeXmlDocumentationAsync(
        string xmlPath,
        string sourceDir,
        (string ToReplace, string DestinationValue)[] fixes)
    {
        if (!File.Exists(xmlPath))
        {
            RedConsole.Write("XML documentation file not found.");
            return 1;
        }

        if (!Directory.Exists(sourceDir))
        {
            RedConsole.Write("Source directory not found.");
            return 1;
        }

        var xmlDocumentation = LoadXmlDocumentation(xmlPath);

        var files = Directory
            .GetFiles(sourceDir, "*.cs", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .ToArray();

        var syntaxTrees = await ParseAllSyntaxTrees(files);

        var compilation = GetCSharpCompilation(syntaxTrees);

        // Process all files in parallel
        var tasks = syntaxTrees.Select(tree =>
            ProcessFile(compilation, xmlDocumentation, tree, fixes));

        await Task.WhenAll(tasks);

        return 0;
    }

    private static CSharpCompilation GetCSharpCompilation(List<SyntaxTree> syntaxTrees)
    {
        // Build references (basic framework references)
        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create(
            "XmlDocSyncTemp",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        return compilation;
    }

    private static async Task<string> GetNormalizedOriginalText(string filePath)
    {
        var readAllText = await File.ReadAllTextAsync(filePath);
        var trimEnd = readAllText.Replace("\r\n", "\n").TrimEnd('\n');
        return trimEnd;
    }

    private static Dictionary<string, string> LoadXmlDocumentation(string path)
    {
        var doc = XDocument.Load(path);

        return doc.Descendants("member")
            .Where(m => m.Attribute("name") != null)
            .ToDictionary(
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                m => m.Attribute("name").Value,
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                m => string.Concat(m.Nodes()) // preserve full XML
            );
    }

    private static string? NormalizeXmlDocBlockIndentation(string text,
        (string ToReplace, string DestinationValue)[] fixes)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var relevant = new List<int>();
        var codeLineTaken = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (Regex.IsMatch(line, @"^\s*///"))
            {
                relevant.Add(i);
                continue;
            }

            if (!codeLineTaken &&
                !string.IsNullOrWhiteSpace(line) &&
                !Regex.IsMatch(line, @"^\s*//"))
            {
                relevant.Add(i);
                codeLineTaken = true;
            }
        }

        if (relevant.Count == 0)
        {
            return text;
        }

        var minIndent = relevant
            .Select(i => lines[i].TakeWhile(char.IsWhiteSpace).Count())
            .Min();

        foreach (var i in relevant)
        {
            lines[i] = new string(' ', minIndent) + lines[i].TrimStart();
        }

        var source = string.Join("\n", lines);
        var removeTextBeforeNonOpenedSummary = XmlDocCleaner.RemoveTextBeforeNonOpenedSummary(source);
        var fixIndentationAndRemoveBlanks =
            XmlDocIndentFormatter.FixIndentationAndRemoveBlanks(removeTextBeforeNonOpenedSummary);
        return XmlDocumentationFormatter.Format(fixIndentationAndRemoveBlanks, fixes)?.TrimEnd('\n', '\r', ' ', '\t');
    }

    private static async Task<List<SyntaxTree>> ParseAllSyntaxTrees(string[] files)
    {
        var trees = new ConcurrentBag<SyntaxTree>();

        await Parallel.ForEachAsync(files, async (file, cancellationToken) =>
        {
            var text = await File.ReadAllTextAsync(file, cancellationToken);
            var tree = CSharpSyntaxTree.ParseText(text, path: file);
            trees.Add(tree);
        });

        return trees.ToList();
    }

    private static async Task ProcessFile(CSharpCompilation compilation,
        Dictionary<string, string> xmlDocumentation, SyntaxTree syntaxTree, (string ToReplace, string DestinationValue)[] valueTuples)
    {
        ColoredConsole.WriteInColor($"Processing {syntaxTree.FilePath}", ConsoleColor.DarkGray);

        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var root = await syntaxTree.GetRootAsync();

        var sourceText = await syntaxTree.GetTextAsync();
        var newline = sourceText.ToString().Contains("\r\n") ? "\r\n" : "\n";

        var rewriter = new DocumentationRewriter(
            semanticModel,
            xmlDocumentation,
            newline);

        var newRoot = rewriter.Visit(root);

#if REWRITE_NEWLINES
        var otherRewriter = new RemoveBlankLinesBetweenMembersRewriter();

        newRoot = otherRewriter.Visit(newRoot);

        var secondRewriter = new BlankLineBetweenMembersRewriter();

        newRoot = otherRewriter.Visit(newRoot);
#endif
        if (!newRoot.IsEquivalentTo(root))
        {
            var contents = NormalizeXmlDocBlockIndentation(newRoot.ToFullString()
                .Replace("GTKSystem", "GtkSystem")
                .Replace("GTKControls", "GtkControls")
                .Replace("///              ", "/// ")
                .Replace("///              ", "/// ")
                .Replace("///             ", "/// ")
                .Replace("\r\n", "\n")
                .TrimEnd('\n'), valueTuples);
            contents = XmlDocFileFormatter.FormatFile(contents)?.Replace("\r\n", "\n");
            var normalizedOriginalText = await GetNormalizedOriginalText(syntaxTree.FilePath);

            if (contents != normalizedOriginalText)
            {
#if USE_WINDIFF
                WinMergeService.CompareWithWinMerge(syntaxTree.FilePath, normalizedOriginalText, contents);
#endif
                ColoredConsole.WriteInColor($"Modified {syntaxTree.FilePath}", ConsoleColor.Blue);
                await File.WriteAllTextAsync(syntaxTree.FilePath, contents);
            }
        }
        else
        {
            var contents = NormalizeXmlDocBlockIndentation(root.ToFullString()
                .Replace("GTKSystem", "GtkSystem")
                .Replace("GTKControls", "GtkControls")
                .Replace("///              ", "/// ")
                .Replace("///              ", "/// ")
                .Replace("///             ", "/// ")
                .Replace("\r\n", "\n")
                .TrimEnd('\n'), valueTuples);
            contents = XmlDocFileFormatter.FormatFile(contents)?.Replace("\r\n", "\n");
            var normalizedOriginalText = root.ToFullString();

            if (contents != normalizedOriginalText)
            {
#if USE_WINDIFF
                WinMergeService.CompareWithWinMerge(syntaxTree.FilePath, normalizedOriginalText, contents);
#endif
                ColoredConsole.WriteInColor($"Modified {syntaxTree.FilePath}", ConsoleColor.Blue);
                await File.WriteAllTextAsync(syntaxTree.FilePath, contents);
            }
        }
    }
}

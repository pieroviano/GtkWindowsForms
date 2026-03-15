using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prepare.Common.Content;
using Prepare.Common.Logging;
using XmlDocSync.Cleaning;
using XmlDocSync.Formatters;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class XmlDocumentationService
{
    public static async Task<(int Failure, (string Path, string Content)[] Contents)> MergeXmlDocumentationAsync(
            string xmlPath,
            string sourceDir,
            (string ToReplace, string DestinationValue)[] fixes,
            bool writeToDisk,
            bool noParallel)
    {
        if (!File.Exists(xmlPath))
        {
            RedConsole.Write("XML documentation file not found.");
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return (1, null);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        if (!Directory.Exists(sourceDir))
        {
            RedConsole.Write("Source directory not found.");
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return (1, null);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        var xmlDocumentation = LoadXmlDocumentation(xmlPath);

        var files = Directory
            .GetFiles(sourceDir, "*.cs", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .ToArray();

        // Parse syntax trees in parallel
        var syntaxTrees = await ParseAllSyntaxTrees(files, noParallel);

        var compilation = GetCSharpCompilation(syntaxTrees);

        var options = new ParallelOptions();
        if (noParallel)
            options.MaxDegreeOfParallelism = 1;
        else
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;

        var concurrentBag = new ConcurrentBag<(string Path, string Content)>();
        await Parallel.ForEachAsync(syntaxTrees, options, async (tree, _) =>
        {
            var content = await ProcessFile(
                compilation,
                xmlDocumentation,
                tree,
                fixes, writeToDisk);
            concurrentBag.Add((tree.FilePath, content ?? string.Empty));
        });

        return (0, concurrentBag.ToArray());
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

        if (relevant.Count == 0) return text;

        var minIndent = relevant
            .Select(i => lines[i].TakeWhile(char.IsWhiteSpace).Count())
            .Min();

        foreach (var i in relevant) lines[i] = new string(' ', minIndent) + lines[i].TrimStart();

        var source = string.Join("\n", lines);
        var removeTextBeforeNonOpenedSummary = XmlDocCleaner.RemoveTextBeforeNonOpenedSummary(source);
        var fixIndentationAndRemoveBlanks =
            XmlDocIndentFormatter.FixIndentationAndRemoveBlanks(removeTextBeforeNonOpenedSummary);
        var normalizeXmlDocBlockIndentation = XmlDocumentationFormatter.Format(fixIndentationAndRemoveBlanks, fixes)?.TrimEnd('\n', '\r', ' ', '\t');
        var cleaned = XmlDocDuplicateCleaner.RemoveRepeatedXmlDocBlocks(normalizeXmlDocBlockIndentation??string.Empty);

        return cleaned;
    }

    private static async Task<List<SyntaxTree>> ParseAllSyntaxTrees(string[] files, bool noParallel)
    {
        var trees = new SyntaxTree[files.Length];

        var options = new ParallelOptions();
        if (noParallel)
            options.MaxDegreeOfParallelism = 1;
        else
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;

        await Parallel.ForEachAsync(
            Enumerable.Range(0, files.Length),
            options,
            async (i, ct) =>
            {
                var text = await File.ReadAllTextAsync(files[i], ct);
                trees[i] = CSharpSyntaxTree.ParseText(text, path: files[i]);
            });

        return trees.ToList();
    }

    private static async Task<string?> ProcessFile(CSharpCompilation compilation,
        Dictionary<string, string> xmlDocumentation, SyntaxTree syntaxTree,
        (string ToReplace, string DestinationValue)[] fixes, bool writeToDisk)
    {
        ConsoleColor.DarkGray.Write($"Processing {syntaxTree.FilePath}");

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
                .TrimEnd('\n'), fixes);
            contents = XmlDocFileFormatter.FormatFile(contents)?.Replace("\r\n", "\n");
            var normalizedOriginalText = await GetNormalizedOriginalText(syntaxTree.FilePath);

            if (contents != normalizedOriginalText)
            {
#if USE_WINDIFF
                WinMergeService.CompareWithWinMerge(syntaxTree.FilePath, normalizedOriginalText, contents);
#endif
                ConsoleColor.Blue.Write($"Modified {syntaxTree.FilePath}");
                if (writeToDisk) await CsFile.WriteAllTextAsync(syntaxTree.FilePath, contents);

                return contents;
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
                .TrimEnd('\n'), fixes);
            contents = XmlDocFileFormatter.FormatFile(contents)?.Replace("\r\n", "\n");
            var normalizedOriginalText = root.ToFullString();

            if (contents != normalizedOriginalText)
            {
#if USE_WINDIFF
                WinMergeService.CompareWithWinMerge(syntaxTree.FilePath, normalizedOriginalText, contents);
#endif
                ConsoleColor.Blue.Write($"Modified {syntaxTree.FilePath}");
                if (writeToDisk) await CsFile.WriteAllTextAsync(syntaxTree.FilePath, contents);

                return contents;
            }
        }

        return sourceText.ToString();
    }
}

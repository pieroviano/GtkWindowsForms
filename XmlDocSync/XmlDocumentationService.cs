using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Prepare.Common.Text;

namespace XmlDocSync;

public static class XmlDocumentationService
{
    public static async Task<int> MergeXmlDocumentationAsync(string xmlPath, string sourceDir)
    {
        if (!File.Exists(xmlPath))
        {
            Console.WriteLine("XML documentation file not found.");
            return 1;
        }

        if (!Directory.Exists(sourceDir))
        {
            Console.WriteLine("Source directory not found.");
            return 1;
        }

        var xmlDocumentation = LoadXmlDocumentation(xmlPath);

        var files = Directory.GetFiles(sourceDir, "*.cs",
            SearchOption.AllDirectories).Select(Path.GetFullPath).ToArray();

        var syntaxTrees = await ParseAllSyntaxTrees(files);

        var compilation = GetCSharpCompilation(syntaxTrees);

        // Process each file
        foreach (var syntaxTree in syntaxTrees)
        {
            await ProcessFile(compilation, xmlDocumentation, syntaxTree);
        }
        return 0;
    }

    private static async Task<List<SyntaxTree>> ParseAllSyntaxTrees(string[] files)
    {
        // Parse all syntax trees first
        var syntaxTrees = new List<SyntaxTree>();

        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(text, path: file);
            syntaxTrees.Add(tree);
        }

        return syntaxTrees;
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

    private static async Task ProcessFile(CSharpCompilation compilation,
        Dictionary<string, string> xmlDocumentation, SyntaxTree syntaxTree)
    {
        Console.WriteLine($"Processing {syntaxTree.FilePath}");

        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var root = await syntaxTree.GetRootAsync();

        var sourceText = await syntaxTree.GetTextAsync();
        var newline = sourceText.ToString().Contains("\r\n") ? "\r\n" : "\n";

        var rewriter = new DocumentationRewriter(
            semanticModel,
            xmlDocumentation,
            newline);

        var newRoot = rewriter.Visit(root);

        if (!newRoot.IsEquivalentTo(root))
        {
            var contents = NormalizeXmlDocBlockIndentation(newRoot.ToFullString()
                .Replace("///              ", "/// ")
                .Replace("///             ", "/// ")
                .Replace("\r\n", "\n")
                .TrimEnd('\n'));
            contents = XmlDocFileFormatter.FormatFile(contents).Replace("\r\n", "\n");
            var normalizedOriginalText = await GetNormalizedOriginalText(syntaxTree.FilePath);

            if (contents != normalizedOriginalText)
            {
#if USE_WINDIFF
                WinMergeService.CompareWithWinMerge(syntaxTree.FilePath, normalizedOriginalText, contents);
#endif
                ConsoleUtility.WriteInColor($"Modified {syntaxTree.FilePath}", ConsoleColor.Blue);
                await File.WriteAllTextAsync(syntaxTree.FilePath, contents);
            }
        }
    }

    private static string NormalizeXmlDocBlockIndentation(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var relevant = new List<int>();
        bool codeLineTaken = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\s*///"))
            {
                relevant.Add(i);
                continue;
            }

            if (!codeLineTaken &&
                !string.IsNullOrWhiteSpace(line) &&
                !System.Text.RegularExpressions.Regex.IsMatch(line, @"^\s*//"))
            {
                relevant.Add(i);
                codeLineTaken = true;
            }
        }

        if (relevant.Count == 0)
            return text;

        int minIndent = relevant
            .Select(i => lines[i].TakeWhile(char.IsWhiteSpace).Count())
            .Min();

        foreach (var i in relevant)
        {
            lines[i] = new string(' ', minIndent) + lines[i].TrimStart();
        }

        var source = string.Join("\n", lines);
        var removeTextBeforeNonOpenedSummary = XmlDocCleaner.RemoveTextBeforeNonOpenedSummary(source);
        var fixIndentationAndRemoveBlanks = XmlDocIndentFormatter.FixIndentationAndRemoveBlanks(removeTextBeforeNonOpenedSummary);
        var fixXmlCommentIndentation = CsXmlCommentFormatter.FixXmlCommentIndentation(fixIndentationAndRemoveBlanks);
        var normalizeXmlDocBlockIndentation = fixXmlCommentIndentation.TrimEnd(['\n', '\r', ' ', '\t']);
        return XmlCommentProcessor.Process(XmlDocFormatter.Format(normalizeXmlDocBlockIndentation));
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
}
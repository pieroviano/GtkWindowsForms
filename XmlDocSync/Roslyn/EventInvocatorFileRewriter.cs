using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace XmlDocSync.Roslyn;

public static class EventInvocatorFileRewriter
{
    public static void RewriteEventInvocations(string filePath)
    {
        if (!File.Exists(filePath)) return;

        // Read the file
        string sourceCode = File.ReadAllText(filePath);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
        CompilationUnitSyntax root = (CompilationUnitSyntax)tree.GetRoot();

        // Rewrite the syntax
        var rewriter = new EventInvocatorRewriter(new[] { "Paint" });
        var newRoot = rewriter.Visit(root);

        // Save back to disk if changes were made
        if (newRoot != root)
        {
            // Format the produced syntax tree so generated members are properly spaced/indented
            var workspace = new AdhocWorkspace();
            var formatted = (CompilationUnitSyntax)Formatter.Format(newRoot, workspace);

            File.WriteAllText(filePath, formatted.ToFullString());
            ColoredConsole.WriteInColor($"Successfully reformatted: {filePath}", ConsoleColor.White, true);
        }
    }
}

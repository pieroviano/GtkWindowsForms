using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Prepare.Common.Content;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class EventInvocatorFileRewriter
{
    public static void RewriteEventInvocations(string filePath)
    {
        if (!File.Exists(filePath)) return;

        // Read the file
        var sourceCode = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = (CompilationUnitSyntax)tree.GetRoot();

        // Rewrite the syntax
        var rewriter = new EventInvocatorRewriter(new[] { "Paint" });
        var newRoot = rewriter.Visit(root);

        // Save back to disk if changes were made
        if (newRoot != root)
        {
            // Format the produced syntax tree so generated members are properly spaced/indented
            var workspace = new AdhocWorkspace();
            var formatted = (CompilationUnitSyntax)Formatter.Format(newRoot, workspace);

            CsFile.WriteAllText(filePath, formatted.ToFullString());
            ConsoleColor.White.Write($"Successfully reformatted: {filePath}", true);
        }
    }
}

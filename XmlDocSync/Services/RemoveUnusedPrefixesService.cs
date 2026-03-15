using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class RemoveUnusedPrefixesService
{
    public static void ProcessFile(string filePath)
    {
        var sourceCode = File.ReadAllText(filePath);

        var tree = CSharpSyntaxTree.ParseText(sourceCode);

        var compilation = CSharpCompilation.Create("TempCompilation")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location))
            .AddSyntaxTrees(tree);

        var semanticModel = compilation.GetSemanticModel(tree);

        var root = tree.GetCompilationUnitRoot();

        var rewriter = new RemoveUnusedPrefixesRewriter(semanticModel, root);

        var newRoot = rewriter.Visit(root);

        var newCode = newRoot.ToFullString();

        File.WriteAllText(filePath, newCode);
    }
}
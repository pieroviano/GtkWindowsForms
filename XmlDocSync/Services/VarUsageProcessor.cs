using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class VarUsageProcessor
{
    public static void ProcessFolder(string folder)
    {
        var files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            ProcessFile(file);
        }
    }

    public static void ProcessFile(string file)
    {
        var code = File.ReadAllText(file);

        var tree = CSharpSyntaxTree.ParseText(code);

        var compilation = CSharpCompilation.Create(
            "TempCompilation",
            new[] { tree },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            });

        var semanticModel = compilation.GetSemanticModel(tree);

        var root = tree.GetRoot();

        var rewriter = new UseVarRewriter(semanticModel);

        var newRoot = rewriter.Visit(root);

        if (!newRoot.IsEquivalentTo(root))
        {
            File.WriteAllText(file, newRoot.ToFullString(), Encoding.UTF8);
        }
    }
}
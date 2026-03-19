using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class EventVirtualizer
{
    public static async Task ProcessDirectoryAsync(string rootPath)
    {
        var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(text);
            var root = await tree.GetRootAsync();

            var rewriter = new EventRewriter();
            var newRoot = rewriter.Visit(root);

            if (!root.IsEquivalentTo(newRoot))
            {
                await File.WriteAllTextAsync(file, newRoot.ToFullString());
            }
        }
    }
}
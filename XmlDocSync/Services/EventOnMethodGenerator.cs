using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class EventOnMethodGenerator
{
    public static string Process(string sourceCode, string[] eventsToExclude, Dictionary<string, string>? handlerToArgs = null)
    {
        handlerToArgs ??= new Dictionary<string, string>();
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();

        var rewriter = new EventOnRewriter(eventsToExclude, handlerToArgs);

        var newRoot = rewriter.Visit(root);

        return newRoot.ToFullString().Replace("\r\n", "\n").TrimEnd('\n');
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public static class TypeSplitter
{
    public static IReadOnlyDictionary<string, string> Split(string sourceCode)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetCompilationUnitRoot();

        var result = new Dictionary<string, string>();

        foreach (var member in root.Members)
        {
            if (member is BaseTypeDeclarationSyntax type)
            {
                AddTypeAndNested(result, root.Usings, type);
            }
            else if (member is NamespaceDeclarationSyntax ns)
            {
                foreach (var nestedType in ns.Members.OfType<BaseTypeDeclarationSyntax>())
                {
                    AddTypeAndNested(result, root.Usings, nestedType, ns);
                }
            }
            else if (member is FileScopedNamespaceDeclarationSyntax fileNs)
            {
                foreach (var nestedType in fileNs.Members.OfType<BaseTypeDeclarationSyntax>())
                {
                    AddTypeAndNested(result, root.Usings, nestedType, fileNs);
                }
            }
        }

        return result;
    }

    private static void AddTypeAndNested(
        Dictionary<string, string> result,
        SyntaxList<UsingDirectiveSyntax> usings,
        BaseTypeDeclarationSyntax type,
        MemberDeclarationSyntax? namespaceOrParent = null)
    {
        // Construct file name
        var fileName = type.Identifier.Text + ".cs";

        // Wrap in namespace if provided
        MemberDeclarationSyntax memberToAdd = namespaceOrParent != null
            ? namespaceOrParent switch
            {
                NamespaceDeclarationSyntax ns => ns.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(type))
                    .WithLeadingTrivia(ns.GetLeadingTrivia())
                    .WithTrailingTrivia(ns.GetTrailingTrivia()),
                FileScopedNamespaceDeclarationSyntax fileNs => fileNs.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(type))
                    .WithLeadingTrivia(fileNs.GetLeadingTrivia())
                    .WithTrailingTrivia(fileNs.GetTrailingTrivia()),
                _ => type
            }
            : type;

        var newRoot = CreateCompilationUnit(usings, memberToAdd);
        result[fileName] = newRoot.ToFullString();
    }

    private static CompilationUnitSyntax CreateCompilationUnit(
        SyntaxList<UsingDirectiveSyntax> usings,
        MemberDeclarationSyntax member)
    {
        var adjustedUsings = usings;

        if (usings.Count > 0)
        {
            var lastUsing = usings.Last();
            var newLastUsing = lastUsing.WithTrailingTrivia(
                lastUsing.GetTrailingTrivia()
                    .Add(SyntaxFactory.ElasticCarriageReturnLineFeed)
                    .Add(SyntaxFactory.ElasticCarriageReturnLineFeed));
            adjustedUsings = usings.Replace(lastUsing, newLastUsing);
        }

        return SyntaxFactory.CompilationUnit()
            .WithUsings(adjustedUsings)
            .AddMembers(member);
    }
}
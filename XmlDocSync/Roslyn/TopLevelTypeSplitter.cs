using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public static class TopLevelTypeSplitter
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
                var fileName = type.Identifier.Text + ".cs";

                var newRoot = CreateCompilationUnit(root.Usings, type);

                result[fileName] = newRoot.ToFullString();
            }
            else if (member is NamespaceDeclarationSyntax ns)
            {
                foreach (var nestedType in ns.Members.OfType<BaseTypeDeclarationSyntax>())
                {
                    var fileName = nestedType.Identifier.Text + ".cs";

                    var newNamespace = ns
                        .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(nestedType))
                        .WithLeadingTrivia(ns.GetLeadingTrivia())
                        .WithTrailingTrivia(ns.GetTrailingTrivia());

                    var newRoot = CreateCompilationUnit(root.Usings, newNamespace);

                    result[fileName] = newRoot.ToFullString();
                }
            }
            else if (member is FileScopedNamespaceDeclarationSyntax fileNs)
            {
                foreach (var nestedType in fileNs.Members.OfType<BaseTypeDeclarationSyntax>())
                {
                    var fileName = nestedType.Identifier.Text + ".cs";

                    var newNamespace = fileNs
                        .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(nestedType))
                        .WithLeadingTrivia(fileNs.GetLeadingTrivia())
                        .WithTrailingTrivia(fileNs.GetTrailingTrivia());

                    var newRoot = CreateCompilationUnit(root.Usings, newNamespace);

                    result[fileName] = newRoot.ToFullString();
                }
            }
        }

        return result;
    }

    private static CompilationUnitSyntax CreateCompilationUnit(
        SyntaxList<UsingDirectiveSyntax> usings,
        MemberDeclarationSyntax member)
    {
        // Ensure there's a blank line after usings (if any exist)
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
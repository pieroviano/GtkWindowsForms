using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class PropertyAccessorRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (node.AccessorList == null)
            return node;

        var accessors = node.AccessorList.Accessors;
        if (accessors.Count < 2)
            return node;

        var newAccessors = new List<AccessorDeclarationSyntax>();

        for (int i = 0; i < accessors.Count; i++)
        {
            var accessor = accessors[i];

            // Remove extra blank lines BEFORE accessor
            var leading = accessor.GetLeadingTrivia();

            leading = NormalizeTrivia(leading);

            // Ensure exactly one newline between accessors (except first)
            if (i > 0)
            {
                leading = SyntaxFactory.TriviaList(
                    SyntaxFactory.CarriageReturnLineFeed);
            }

            accessor = accessor.WithLeadingTrivia(leading);

            newAccessors.Add(accessor);
        }

        return node.WithAccessorList(
            node.AccessorList.WithAccessors(SyntaxFactory.List(newAccessors)));
    }

    private static SyntaxTriviaList NormalizeTrivia(SyntaxTriviaList trivia)
    {
        var result = new List<SyntaxTrivia>();
        bool lastWasNewLine = false;

        foreach (var t in trivia)
        {
            if (t.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                if (!lastWasNewLine)
                {
                    result.Add(t);
                    lastWasNewLine = true;
                }
                // skip extra blank lines
            }
            else if (!t.IsKind(SyntaxKind.WhitespaceTrivia))
            {
                result.Add(t);
                lastWasNewLine = false;
            }
        }

        return SyntaxFactory.TriviaList(result);
    }
}
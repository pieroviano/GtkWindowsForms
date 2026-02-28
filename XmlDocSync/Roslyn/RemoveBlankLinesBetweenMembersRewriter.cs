using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class RemoveBlankLinesBetweenMembersRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    private SyntaxList<MemberDeclarationSyntax> RewriteMembers(SyntaxList<MemberDeclarationSyntax> members)
    {
        if (members.Count < 2)
            return members;

        var newMembers = members.ToList();

        for (int i = 1; i < newMembers.Count; i++)
        {
            var member = newMembers[i];
            var leadingTrivia = member.GetLeadingTrivia();

            // Reduce all leading blank lines to at most one
            leadingTrivia = CollapseLeadingBlankLines(leadingTrivia);

            newMembers[i] = member.WithLeadingTrivia(leadingTrivia);
        }

        return SyntaxFactory.List(newMembers);
    }

    private static SyntaxTriviaList CollapseLeadingBlankLines(SyntaxTriviaList trivia)
    {
        var newTrivia = new SyntaxTriviaList();
        bool previousWasEndOfLine = false;

        foreach (var t in trivia)
        {
            if (t.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                if (!previousWasEndOfLine)
                {
                    newTrivia = newTrivia.Add(t); // keep only one consecutive newline
                    previousWasEndOfLine = true;
                }
            }
            else
            {
                newTrivia = newTrivia.Add(t);
                previousWasEndOfLine = false;
            }
        }

        return newTrivia;
    }
}

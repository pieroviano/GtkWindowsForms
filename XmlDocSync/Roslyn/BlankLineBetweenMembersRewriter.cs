using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class BlankLineBetweenMembersRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node)
        => node.WithMembers(RewriteMembers(node.Members));

    private SyntaxList<MemberDeclarationSyntax> RewriteMembers(
        SyntaxList<MemberDeclarationSyntax> members)
    {
        if (members.Count < 2)
            return members;

        var newMembers = members.ToList();

        for (var i = 1; i < newMembers.Count; i++)
        {
            var member = newMembers[i];
            var leadingTrivia = member.GetLeadingTrivia();

            var leadingNewLines = CountLeadingEndOfLines(leadingTrivia);

            // Already has a blank line (2 or more newlines)
            if (leadingNewLines >= 2)
                continue;

            var neededNewLines = 2 - leadingNewLines;

            var newTrivia = SyntaxFactory.TriviaList();

            for (var j = 0; j < neededNewLines; j++)
            {
                newTrivia = newTrivia.Add(SyntaxFactory.ElasticCarriageReturnLineFeed);
            }

            newTrivia = newTrivia.AddRange(leadingTrivia);

            newMembers[i] = member.WithLeadingTrivia(newTrivia);
        }

        return SyntaxFactory.List(newMembers);
    }

    private static int CountLeadingEndOfLines(SyntaxTriviaList trivia)
    {
        var count = 0;

        foreach (var t in trivia)
        {
            if (t.IsKind(SyntaxKind.EndOfLineTrivia))
                count++;
            else
                break;
        }

        return count;
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class ReturnBlankLineRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node.Body != null)
            node = node.WithBody((BlockSyntax)VisitBlock(node.Body));

        return base.VisitMethodDeclaration(node);
    }

    public override SyntaxNode? VisitAccessorDeclaration(AccessorDeclarationSyntax node)
    {
        if (node.Body != null)
            node = node.WithBody((BlockSyntax)VisitBlock(node.Body));

        return base.VisitAccessorDeclaration(node);
    }

    public override SyntaxNode VisitBlock(BlockSyntax node)
    {
        var newStatements = new List<StatementSyntax>();

        for (int i = 0; i < node.Statements.Count; i++)
        {
            var stmt = node.Statements[i];

            // If it's a return statement and not the first statement
            if (stmt is ReturnStatementSyntax && i > 0)
            {
                // Get leading trivia from the return statement
                var leadingTrivia = stmt.GetLeadingTrivia();

                // Check if there's already an empty line
                bool hasBlankLine = leadingTrivia.Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia));

                // Add an extra blank line before the return
                stmt = stmt.WithLeadingTrivia(
                    leadingTrivia.Insert(0, SyntaxFactory.ElasticCarriageReturnLineFeed)
                );
            }

            newStatements.Add(stmt);
        }

        return node.WithStatements(SyntaxFactory.List(newStatements));
    }
}

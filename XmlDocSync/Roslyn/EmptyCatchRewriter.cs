using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class EmptyCatchRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitCatchClause(CatchClauseSyntax node)
    {
        node = (CatchClauseSyntax)base.VisitCatchClause(node)!;

        // Only process empty catch blocks
        if (node.Block == null || node.Block.Statements.Count != 0)
            return node;

        // Get containing method or constructor name
        var method = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        var ctor = node.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

        var methodName = method?.Identifier.Text
                         ?? ctor?.Identifier.Text
                         ?? "unknown";

        TypeSyntax exceptionType;
        SyntaxToken identifier;

        if (node.Declaration == null)
        {
            // catch { }
            exceptionType = SyntaxFactory.ParseTypeName("Exception")
                .WithTrailingTrivia(SyntaxFactory.Space);

            identifier = SyntaxFactory.Identifier("ex");
        }
        else
        {
            // catch (SomeException) or catch (SomeException ex)
            exceptionType = node.Declaration.Type
                .WithTrailingTrivia(SyntaxFactory.Space);

            if (node.Declaration.Identifier.IsKind(SyntaxKind.None))
            {
                var variableName = GetExceptionVariableName(exceptionType);
                identifier = SyntaxFactory.Identifier(variableName);
            }
            else
            {
                identifier = node.Declaration.Identifier;
            }
        }

        var newDeclaration = SyntaxFactory.CatchDeclaration(exceptionType, identifier);

        var statement = SyntaxFactory.ParseStatement(
            $"System.Diagnostics.Trace.Write($\"Unexpected exception in {methodName}: {{{identifier.Text}}}\");");

        var newBlock = SyntaxFactory.Block(statement)
            .WithOpenBraceToken(
                SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                    .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed))
            .WithCloseBraceToken(
                SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                    .WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));

        var catchKeyword = node.CatchKeyword
            .WithTrailingTrivia(SyntaxFactory.Space);

        return node
            .WithCatchKeyword(catchKeyword)
            .WithDeclaration(newDeclaration)
            .WithBlock(newBlock);
    }

    private static string GetExceptionVariableName(TypeSyntax type)
    {
        var name = type.ToString();

        // Strip generic arguments (e.g., Something<T>)
        var genericIndex = name.IndexOf('<');
        if (genericIndex >= 0)
            name = name.Substring(0, genericIndex);

        // Strip namespace
        var lastDot = name.LastIndexOf('.');
        if (lastDot >= 0)
            name = name.Substring(lastDot + 1);

        // Lowercase first letter
        if (string.IsNullOrEmpty(name))
            return "ex";

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
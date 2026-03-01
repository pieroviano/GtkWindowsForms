using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace XmlDocSync.Roslyn;

public class EventInvocatorRewriter : CSharpSyntaxRewriter
{
    private readonly HashSet<string> _ignoredEventNames;

    // Default constructor: no ignored event names.
    public EventInvocatorRewriter()
        : this(Array.Empty<string>())
    { }

    // Constructor that accepts an array (or any IEnumerable) of event names to ignore.
    // The comparison is case-insensitive.
    public EventInvocatorRewriter(IEnumerable<string> ignoredEventNames)
    {
        _ignoredEventNames = new HashSet<string>(
            (ignoredEventNames ?? Array.Empty<string>()),
            StringComparer.OrdinalIgnoreCase);
    }

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // 1. Check if method name starts with "On" (e.g., OnMouseClick)
        string methodName = node.Identifier.Text;
        if (!methodName.StartsWith("On") || methodName.Length <= 2)
            return base.VisitMethodDeclaration(node);

        // 2. Extract potential event name (e.g., MouseClick)
        string eventName = methodName.Substring(2);

        // If this event name was provided to the constructor, ignore (do not rewrite) it.
        if (_ignoredEventNames.Contains(eventName))
            return base.VisitMethodDeclaration(node);

        // 3. Check for exactly one parameter ending in "EventArgs"
        var parameters = node.ParameterList.Parameters;
        if (parameters.Count != 1)
            return base.VisitMethodDeclaration(node);

        var param = parameters[0];
        string? typeName = param.Type?.ToString();
        if (typeName == null || !typeName.EndsWith("EventArgs"))
            return base.VisitMethodDeclaration(node);

        string paramName = param.Identifier.Text;

        // Only rewrite methods that have a block body and that body is "empty",
        // i.e. contains no real statements (only comments, whitespace or empty statements).
        if (node.ExpressionBody != null)
            return base.VisitMethodDeclaration(node); // expression-bodied -> do not touch

        if (node.Body == null)
            return base.VisitMethodDeclaration(node); // no body (abstract/extern) -> do not touch

        var statements = node.Body.Statements;
        if (statements.Count > 0 && !statements.All(s => s is EmptyStatementSyntax))
        {
            // contains real code -> do not rewrite
            return base.VisitMethodDeclaration(node);
        }

        // 4. Construct the body: EventName?.Invoke(this, e);
        var invokeStatement = ExpressionStatement(
            ConditionalAccessExpression(
                IdentifierName(eventName),
                InvocationExpression(
                        MemberBindingExpression(IdentifierName("Invoke")))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new[] {
                                    Argument(ThisExpression()),
                                    Argument(IdentifierName(paramName))
                                }
                            )
                        )
                    )
            )
        );

        // Create a new block but preserve the original brace tokens (and their trivia)
        var newBody = Block(invokeStatement)
            .WithOpenBraceToken(node.Body.OpenBraceToken)
            .WithCloseBraceToken(node.Body.CloseBraceToken);

        // Return the method with the new body
        return node.WithBody(newBody)
                   .WithSemicolonToken(default); // ensure no stray semicolon for expression-bodied originals
    }
}

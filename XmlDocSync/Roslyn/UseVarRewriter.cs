using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class UseVarRewriter(SemanticModel semanticModel) : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        var declaration = node.Declaration;

        if (declaration == null)
            return node;

        // const cannot use var
        if (node.Modifiers.Any(SyntaxKind.ConstKeyword))
            return node;

        if (declaration.Type.IsVar)
            return node;

        if (declaration.Variables.Count != 1)
            return node;

        var variable = declaration.Variables[0];

        if (variable.Initializer == null)
            return node;

        var initializer = variable.Initializer.Value;

        // Skip target-typed new()
        if (initializer is ImplicitObjectCreationExpressionSyntax)
            return node;

        // NEW FIX: skip default literal (var x = default;)
        if (initializer.IsKind(SyntaxKind.DefaultLiteralExpression))
            return node;

        var initializerType = semanticModel.GetTypeInfo(initializer).Type;
        var declaredType = semanticModel.GetTypeInfo(declaration.Type).Type;

        if (initializerType == null || declaredType == null)
            return node;

        if (!SymbolEqualityComparer.Default.Equals(initializerType, declaredType))
            return node;

        var varType = SyntaxFactory.IdentifierName("var")
            .WithTriviaFrom(declaration.Type);

        var newDeclaration = declaration.WithType(varType);

        return node.WithDeclaration(newDeclaration);
    }
}
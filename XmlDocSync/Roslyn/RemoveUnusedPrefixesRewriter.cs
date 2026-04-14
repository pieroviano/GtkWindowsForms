using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class RemoveUnusedPrefixesRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly string?[] _usings;

    public RemoveUnusedPrefixesRewriter(SemanticModel semanticModel, CompilationUnitSyntax root)
    {
        _semanticModel = semanticModel;

        _usings = root.Usings
            .Select(u => u.Name?.ToString())
            .ToArray();
    }

    public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
    {
        if (_semanticModel.GetSymbolInfo(node).Symbol is not INamedTypeSymbol symbol)
            return base.VisitQualifiedName(node);

        var namespaceName = symbol.ContainingNamespace?.ToDisplayString();

        if (namespaceName == null)
            return base.VisitQualifiedName(node);

        if (_usings.Contains(namespaceName))
        {
            var simplified = SyntaxFactory.ParseTypeName(symbol.Name)
                .WithTriviaFrom(node);

            return simplified;
        }

        return base.VisitQualifiedName(node);
    }
}
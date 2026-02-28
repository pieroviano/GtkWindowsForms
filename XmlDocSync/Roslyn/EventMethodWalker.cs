using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class EventMethodWalker : CSharpSyntaxWalker
{
    public bool FoundMatch { get; private set; }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // 1. Name starts with "On"
        string methodName = node.Identifier.Text;
        if (!methodName.StartsWith("On") || methodName.Length <= 2) return;

        // 2. Returns void
        if (node.ReturnType is PredefinedTypeSyntax pts && pts.Keyword.Text != "void") return;

        // 3. Exactly one parameter ending in "EventArgs"
        var parameters = node.ParameterList.Parameters;
        if (parameters.Count == 1)
        {
            string? typeName = parameters[0].Type?.ToString();
            if (typeName != null && typeName.EndsWith("EventArgs"))
            {
                FoundMatch = true;
            }
        }

        base.VisitMethodDeclaration(node);
    }
}

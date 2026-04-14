using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class EventRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        // Skip sealed classes
        if (node.Modifiers.Any(SyntaxKind.SealedKeyword))
            return base.VisitClassDeclaration(node);

        // Check if class has events
        var hasEvents = node.Members.OfType<EventFieldDeclarationSyntax>().Any();
        if (!hasEvents)
            return base.VisitClassDeclaration(node);

        var updatedMembers = node.Members.Select(member =>
        {
            if (member is EventFieldDeclarationSyntax eventField)
            {
                var modifiers = eventField.Modifiers;

                var isPublic = modifiers.Any(SyntaxKind.PublicKeyword);
                var isVirtual = modifiers.Any(SyntaxKind.VirtualKeyword);
                var isOverride = modifiers.Any(SyntaxKind.OverrideKeyword);

                if (isPublic && !isVirtual && !isOverride)
                {
                    var virtualToken = SyntaxFactory.Token(SyntaxKind.VirtualKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Whitespace(" "));

                    modifiers = modifiers.Add(virtualToken);
                    return eventField.WithModifiers(modifiers);
                }
            }

            return member;
        });

        return node.WithMembers(SyntaxFactory.List(updatedMembers));
    }
}
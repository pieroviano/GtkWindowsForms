using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

static class MemberExtensions
{
    public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax member)
    {
        return member switch
        {
            MemberDeclarationSyntax m when m is BaseFieldDeclarationSyntax f => f.Modifiers,
            MemberDeclarationSyntax m when m is BaseMethodDeclarationSyntax mm => mm.Modifiers,
            MemberDeclarationSyntax m when m is BasePropertyDeclarationSyntax p => p.Modifiers,
            MemberDeclarationSyntax m when m is DelegateDeclarationSyntax d => d.Modifiers,
            MemberDeclarationSyntax m when m is BaseTypeDeclarationSyntax t => t.Modifiers,
            _ => default
        };
    }
}

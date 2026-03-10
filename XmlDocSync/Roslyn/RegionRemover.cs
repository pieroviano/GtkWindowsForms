using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace XmlDocSync.Roslyn;

internal sealed class RegionRemover : CSharpSyntaxRewriter
{
    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        if (!trivia.IsKind(SyntaxKind.RegionDirectiveTrivia) &&
            !trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
        {
            return trivia;
        }

        // remove region directives but preserve surrounding formatting
        return default;
    }
}

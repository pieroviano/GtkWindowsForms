using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XmlDocSync.Formatters;

namespace XmlDocSync.Roslyn;

public class DocumentationRewriter : CSharpSyntaxRewriter
{
    private readonly string _newline;
    private readonly SemanticModel _semanticModel;
    private readonly Dictionary<string, string> _xmlDocs;

    public DocumentationRewriter(
        SemanticModel semanticModel,
        Dictionary<string, string> xmlDocs,
        string newline)
    {
        _semanticModel = semanticModel;
        _xmlDocs = xmlDocs;
        _newline = newline;
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitClassDeclaration);
    }

    public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitConstructorDeclaration);
    }

    public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitEnumDeclaration);
    }

    public override SyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitEnumMemberDeclaration);
    }

    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitFieldDeclaration);
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitMethodDeclaration);
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitPropertyDeclaration);
    }

    public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitStructDeclaration);
    }

    private SyntaxTriviaList CreateDocumentationTrivia(string xmlContent, string indentation)
    {
        xmlContent = RemoveCrefPrefixes(xmlContent);

        xmlContent = NormalizeXmlDocBlockIndentation(xmlContent);

        var lines = xmlContent
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var sb = new StringBuilder();

        foreach (var line in lines)
        {
            sb.Append(indentation);
            sb.Append("///");

            if (!string.IsNullOrWhiteSpace(line))
            {
                sb.Append(' ');
                sb.Append(line.TrimStart());
            }

            sb.Append(_newline);
        }

        var replace = XmlCommentFormatter.FormatXmlComment(sb.ToString().Replace("@", ""));

        return SyntaxFactory.ParseLeadingTrivia(replace);
    }

    private static string NormalizeXmlDocBlockIndentation(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Identify relevant lines:
        //  - XML doc comments (///)
        //  - The first non-comment, non-empty line after them
        var relevantLineIndices = new List<int>();
        bool codeLineCaptured = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\s*///"))
            {
                relevantLineIndices.Add(i);
                continue;
            }

            if (!codeLineCaptured &&
                !string.IsNullOrWhiteSpace(line) &&
                !System.Text.RegularExpressions.Regex.IsMatch(line, @"^\s*//"))
            {
                relevantLineIndices.Add(i);
                codeLineCaptured = true;
            }
        }

        if (relevantLineIndices.Count == 0)
            return text;

        // Compute minimal indentation
        int minIndent = relevantLineIndices
            .Select(i => lines[i]
                .TakeWhile(char.IsWhiteSpace)
                .Count())
            .Min();

        // Normalize indentation
        foreach (var i in relevantLineIndices)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();
            lines[i] = new string(' ', minIndent) + trimmed;
        }

        return string.Join("\n", lines);
    }

    private static string GetExistingDocumentation(MemberDeclarationSyntax node)
    {
        var trivia = node.GetLeadingTrivia()
            .FirstOrDefault(t => t.GetStructure() is DocumentationCommentTriviaSyntax);

        if (trivia.GetStructure() is DocumentationCommentTriviaSyntax docTrivia)
        {
            return docTrivia.ToFullString();
        }

        return "";
    }

    private static string GetIndentation(SyntaxNode node)
    {
        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
        var startLine = lineSpan.StartLinePosition.Line;

        var textLine = node.SyntaxTree
            .GetText()
            .Lines[startLine];

        return textLine.ToString()
            .TakeWhile(char.IsWhiteSpace)
            .Aggregate("", (a, c) => a + c);
    }

#if ENSURE_SINGLE_BLANK_LINE_AFTER
    private SyntaxTriviaList EnsureSingleBlankLineAfter(SyntaxTriviaList trivia)
    {
        // Convert to string
        var sb = new StringBuilder();
        foreach (var t in trivia)
        {
            sb.Append(t.ToFullString());
        }

        // Normalize newlines to \n
        var text = sb.ToString().Replace("\r\n", "\n");

        // Collapse multiple consecutive blank lines into a single blank line
        text = Regex.Replace(text, @"\n{3,}", "\n\n");

        // Ensure ends with exactly one blank line
        if (!text.EndsWith("\n\n"))
            text = text.TrimEnd() + "\n\n";

        // Parse back to trivia
        return SyntaxFactory.ParseLeadingTrivia(text);
    }
#endif

    private T Process<T>(T node) where T : MemberDeclarationSyntax
    {
        ISymbol? symbol = null;
        try
        {
            symbol = _semanticModel.GetDeclaredSymbol(node);
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
        if (symbol == null)
        {
            return node;
        }

        var docId = symbol.GetDocumentationCommentId();
        if (docId == null)
        {
            return node;
        }

        if (!_xmlDocs.TryGetValue(docId, out var externalXml))
        {
            return node;
        }

        var existingDoc = GetExistingDocumentation(node);
        if (externalXml.Length <= existingDoc.Length)
        {
            return node;
        }

        var leadingTrivia = node.GetLeadingTrivia();

        var indentation = GetIndentation(node);

        var newDocTrivia = CreateDocumentationTrivia(externalXml, indentation);

        var rebuiltTrivia = new SyntaxTriviaList();

        foreach (var trivia in leadingTrivia)
        {
            if (trivia.GetStructure() is DocumentationCommentTriviaSyntax)
            {
                rebuiltTrivia = rebuiltTrivia.AddRange(newDocTrivia);
            }
            else
            {
                rebuiltTrivia = rebuiltTrivia.Add(trivia);
            }
        }

        // No existing doc → insert before first non-whitespace trivia
        if (!leadingTrivia.Any(t => t.GetStructure() is DocumentationCommentTriviaSyntax))
        {
            var trimmedLeadingTrivia = leadingTrivia
                .Where(t => !t.IsKind(SyntaxKind.WhitespaceTrivia));

            rebuiltTrivia = newDocTrivia.AddRange(trimmedLeadingTrivia);
        }

#if ENSURE_SINGLE_BLANK_LINE_AFTER
        // Normalize spacing between members
        rebuiltTrivia = EnsureSingleBlankLineAfter(rebuiltTrivia);
#endif

        return node.WithLeadingTrivia(rebuiltTrivia);
    }

    private static string RemoveCrefPrefixes(string xml)
    {
        return Regex.Replace(
            xml,
            @"cref=""([A-Z]):([^""]+)""",
            m => $"cref=\"{m.Groups[2].Value}\"");
    }

    private T RewriteAndVisit<T>(
        T node,
        Func<T, SyntaxNode> baseVisit)
        where T : MemberDeclarationSyntax
    {
        var updated = Process(node);
        return (T)baseVisit((T)updated);
    }
}

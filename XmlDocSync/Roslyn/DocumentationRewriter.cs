using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Prepare.Common.Logging;
using XmlDocSync.Formatters;

#pragma warning disable CS8621 // Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).

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

    public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitInterfaceDeclaration);
    }

    public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitDelegateDeclaration);
    }

    public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitEventDeclaration);
    }

    public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitEventFieldDeclaration);
    }

    public override SyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitRecordDeclaration);
    }

    public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitIndexerDeclaration);
    }

    public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitOperatorDeclaration);
    }

    public override SyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitConversionOperatorDeclaration);
    }

    public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
    {
        return RewriteAndVisit(node, base.VisitDestructorDeclaration);
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

        return SyntaxFactory.ParseLeadingTrivia(replace ?? string.Empty);
    }

    private static string NormalizeXmlDocBlockIndentation(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Identify relevant lines:
        //  - XML doc comments (///)
        //  - The first non-comment, non-empty line after them
        var relevantLineIndices = new List<int>();
        var codeLineCaptured = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (Regex.IsMatch(line, @"^\s*///"))
            {
                relevantLineIndices.Add(i);
                continue;
            }

            if (!codeLineCaptured &&
                !string.IsNullOrWhiteSpace(line) &&
                !Regex.IsMatch(line, @"^\s*//"))
            {
                relevantLineIndices.Add(i);
                codeLineCaptured = true;
            }
        }

        if (relevantLineIndices.Count == 0)
            return text;

        // Compute minimal indentation
        var minIndent = relevantLineIndices
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

    private T Process<T>(T node, T? originalNode = null) where T : MemberDeclarationSyntax
    {
        ISymbol? symbol = null;
        try
        {
            symbol = _semanticModel.GetDeclaredSymbol(node);
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(ArgumentException))
            {
                RedConsole.Write(e.ToString());
            }
        }

        // If we couldn't get the symbol from the (possibly modified) node,
        // try getting it from the original node before children were visited
        if (symbol == null && originalNode != null && !ReferenceEquals(originalNode, node))
        {
            try
            {
                symbol = _semanticModel.GetDeclaredSymbol(originalNode);
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(ArgumentException))
                {
                    YellowConsole.Write(e.ToString());
                }
            }
        }

        // Special-case fields: GetDeclaredSymbol on FieldDeclarationSyntax may return null.
        // Try to resolve symbol from the first VariableDeclarator when needed.
        if (symbol == null && node is FieldDeclarationSyntax fieldDecl)
        {
            var variable = fieldDecl.Declaration?.Variables.FirstOrDefault();
            if (variable != null)
            {
                try
                {
                    symbol = _semanticModel.GetDeclaredSymbol(variable);
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(ArgumentException))
                    {
                        YellowConsole.Write(e.ToString());
                    }
                }
            }
        }

        // Special-case event fields: GetDeclaredSymbol on EventFieldDeclarationSyntax may return null.
        // Try to resolve symbol from the first VariableDeclarator when needed.
        if (symbol == null && node is EventFieldDeclarationSyntax eventFieldDecl)
        {
            var variable = eventFieldDecl.Declaration?.Variables.FirstOrDefault();
            if (variable != null)
            {
                try
                {
                    symbol = _semanticModel.GetDeclaredSymbol(variable);
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(ArgumentException))
                    {
                        RedConsole.Write(e.ToString());
                    }
                }
            }
        }

        // Special-case constructors: sometimes GetDeclaredSymbol returns null (missing references / binding),
        // so try to resolve via the containing type and match constructor symbols by parameter count / syntax location.
        if (symbol == null && node is ConstructorDeclarationSyntax ctorDecl)
        {
            try
            {
                var typeNode = ctorDecl.Parent as TypeDeclarationSyntax;
                if (typeNode != null)
                {
                    var typeSymbol = _semanticModel.GetDeclaredSymbol(typeNode) as INamedTypeSymbol;
                    if (typeSymbol != null)
                    {
                        var candidates = typeSymbol.InstanceConstructors
                            .Where(c => c.Parameters.Length == ctorDecl.ParameterList.Parameters.Count)
                            .ToList();

                        if (candidates.Count == 1)
                        {
                            symbol = candidates[0];
                        }
                        else if (candidates.Count > 1)
                        {
                            // Try to match by syntax reference (preferred)
                            foreach (var cand in candidates)
                            {
                                foreach (var r in cand.DeclaringSyntaxReferences)
                                {
                                    var syntax = r.GetSyntax();
                                    if (syntax is ConstructorDeclarationSyntax cds && cds.SpanStart == ctorDecl.SpanStart)
                                    {
                                        symbol = cand;
                                        break;
                                    }
                                }
                                if (symbol != null) break;
                            }

                            // last-resort: pick first candidate
                            if (symbol == null)
                                symbol = candidates.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
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
            // Fallbacks for mismatched documentation IDs:
            // 1) Constructors: external XML often qualifies parameter types; semantic model may produce slightly different parameter type names
            //    so try to match constructor entries by containing type and parameter count.
            if (symbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Constructor)
            {
                var containingTypeName = methodSymbol.ContainingType?.ToDisplayString();
                if (!string.IsNullOrEmpty(containingTypeName))
                {
                    var ctorPrefix = $"M:{containingTypeName}.#ctor";
                    var candidates = _xmlDocs.Keys.Where(k => k.StartsWith(ctorPrefix, StringComparison.Ordinal)).ToList();
                    if (candidates.Count > 0)
                    {
                        foreach (var cand in candidates)
                        {
                            var start = cand.IndexOf('(');
                            var end = cand.LastIndexOf(')');
                            int paramCount = 0;
                            if (start >= 0 && end > start)
                            {
                                var inside = cand.Substring(start + 1, end - start - 1).Trim();
                                if (inside.Length == 0)
                                    paramCount = 0;
                                else
                                    paramCount = inside.Split(',').Length;
                            }

                            if (paramCount == methodSymbol.Parameters.Length)
                            {
                                externalXml = _xmlDocs[cand];
                                break;
                            }
                        }

                        // if still not found, take first candidate (best-effort)
                        if (externalXml == null && candidates.Count > 0)
                        {
                            externalXml = _xmlDocs[candidates[0]];
                        }
                    }

                    // If still not found, try fuzzy matching by looking for .#ctor with similar type name
                    if (externalXml == null)
                    {
                        var typeName = methodSymbol.ContainingType?.Name;
                        var containingNamespace = methodSymbol.ContainingType?.ContainingNamespace?.ToDisplayString();

                        if (!string.IsNullOrEmpty(typeName))
                        {
                            // First try: look for constructors in the same namespace that have a similar type name
                            var ctorCandidates = _xmlDocs.Keys
                                .Where(k => k.Contains(".#ctor"))
                                .ToList();

                            // Filter by namespace if available
                            if (!string.IsNullOrEmpty(containingNamespace))
                            {
                                ctorCandidates = ctorCandidates
                                    .Where(k => k.Contains(containingNamespace))
                                    .ToList();
                            }

                            // Now find the best match based on type name similarity
                            var bestMatch = ctorCandidates
                                .Select(k => new
                                {
                                    Key = k,
                                    // Extract type name from the key (between last '.' before #ctor and '.#ctor')
                                    TypeNameInXml = ExtractTypeNameFromConstructorKey(k)
                                })
                                .Where(x => x.TypeNameInXml != null && IsSimilarTypeName(typeName, x.TypeNameInXml))
                                .FirstOrDefault();

                            if (bestMatch != null)
                            {
                                var cand = bestMatch.Key;
                                var start = cand.IndexOf('(');
                                var end = cand.LastIndexOf(')');
                                int paramCount = 0;
                                if (start >= 0 && end > start)
                                {
                                    var inside = cand.Substring(start + 1, end - start - 1).Trim();
                                    if (inside.Length == 0)
                                        paramCount = 0;
                                    else
                                        paramCount = inside.Split(',').Length;
                                }
                                else if (start < 0)
                                {
                                    paramCount = 0;
                                }

                                if (paramCount == methodSymbol.Parameters.Length)
                                {
                                    externalXml = _xmlDocs[cand];
                                }
                            }
                        }
                    }
                }
            }

            // 2) As a broader fallback try to match by simple member name (last resort)
            if (externalXml == null)
            {
                var simpleName = docId;
                // Try removing parameter type qualification differences: compare by member name without parameter types
                var parenIndex = docId.IndexOf('(');
                var prefix = parenIndex > 0 ? docId.Substring(0, parenIndex) : docId;
                var match = _xmlDocs.Keys.FirstOrDefault(k => k.StartsWith(prefix, StringComparison.Ordinal));
                if (match != null)
                {
                    externalXml = _xmlDocs[match];
                }
            }

            if (externalXml == null)
            {
                return node;
            }
        }

        // Compute indentation and the new documentation trivia early so we can compare with existing doc trivia
        var leadingTrivia = node.GetLeadingTrivia();
        var indentation = GetIndentation(node);
        var newDocTrivia = CreateDocumentationTrivia(externalXml, indentation);

        // If existing documentation trivia exists and is already identical to the generated one, skip replacement.
        var existingDocTrivia = leadingTrivia.FirstOrDefault(t => t.GetStructure() is DocumentationCommentTriviaSyntax);
        if (existingDocTrivia != default && existingDocTrivia.ToFullString() == newDocTrivia.ToFullString())
        {
            return node;
        }

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
        Func<T, SyntaxNode>? baseVisit)
        where T : MemberDeclarationSyntax
    {
        Trace.Assert(baseVisit != null, nameof(baseVisit) + " != null");

        // First, visit children using the ORIGINAL node so children can access original parents
        // when querying the semantic model (important for constructor parent lookups)
        var visited = (T)baseVisit(node);

        // Then, process the visited node to add/update documentation
        // Pass both the original and visited nodes so Process can get the symbol from whichever works
        var updated = Process(visited, node);

        return updated;
    }

    /// <summary>
    /// Extracts the type name from a constructor documentation key.
    /// For example: "M:GtkSystem.Windows.Forms.GtkControls.ControlBase.RichTextBoxBae.#ctor" returns "RichTextBoxBae"
    /// </summary>
    private static string? ExtractTypeNameFromConstructorKey(string key)
    {
        var ctorIndex = key.IndexOf(".#ctor");
        if (ctorIndex < 0) return null;

        var beforeCtor = key.Substring(0, ctorIndex);
        var lastDotBeforeCtor = beforeCtor.LastIndexOf('.');
        if (lastDotBeforeCtor < 0) return null;

        return beforeCtor.Substring(lastDotBeforeCtor + 1);
    }

    /// <summary>
    /// Checks if two type names are similar enough to be considered a match.
    /// Uses Levenshtein distance or simple prefix matching for fuzzy comparison.
    /// </summary>
    private static bool IsSimilarTypeName(string name1, string name2)
    {
        if (string.Equals(name1, name2, StringComparison.Ordinal))
            return true;

        if (string.Equals(name1, name2, StringComparison.OrdinalIgnoreCase))
            return true;

        // Check if one is a prefix of the other (at least 5 characters)
        var minLen = Math.Min(name1.Length, name2.Length);
        if (minLen >= 5)
        {
            var commonPrefix = 0;
            for (var i = 0; i < minLen; i++)
            {
                if (char.ToLowerInvariant(name1[i]) == char.ToLowerInvariant(name2[i]))
                    commonPrefix++;
                else
                    break;
            }

            // If 80% or more of the shorter name matches as prefix, consider it similar
            if (commonPrefix >= minLen * 0.8)
                return true;
        }

        // Use Levenshtein distance for small differences (typos)
        var distance = LevenshteinDistance(name1, name2);
        var maxLen = Math.Max(name1.Length, name2.Length);

        // Allow up to 2 character differences or 15% difference
        return distance <= 2 || (maxLen > 0 && distance <= maxLen * 0.15);
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings (edit distance).
    /// </summary>
    private static int LevenshteinDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
        if (string.IsNullOrEmpty(s2)) return s1.Length;

        var len1 = s1.Length;
        var len2 = s2.Length;
        var d = new int[len1 + 1, len2 + 1];

        for (var i = 0; i <= len1; i++) d[i, 0] = i;
        for (var j = 0; j <= len2; j++) d[0, j] = j;

        for (var i = 1; i <= len1; i++)
        {
            for (var j = 1; j <= len2; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[len1, len2];
    }
}

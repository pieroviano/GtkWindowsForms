using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Prepare.Common.Content;

namespace XmlDocSync.Roslyn;

public static class CsFileOrganizer
{
    public static void ProcessFile(string path, params string[] toExclude)
    {
        var code = File.ReadAllText(path);

        var tree = CSharpSyntaxTree.ParseText(code);
        var root = (CompilationUnitSyntax)tree.GetRoot();

        // Remove #region / #endregion
        var regionRemover = new RegionRemover();
        root = (CompilationUnitSyntax)regionRemover.Visit(root);

        if (!toExclude.Contains(Path.GetFileName(path)))
        {
            root = ConvertToFileScopedNamespace(root);
        }

        var newRoot = ProcessCompilationUnit(root);

        var contents = newRoot.ToFullString().Replace("\r\n", "\n");
        contents = Regex.Replace(contents, "\n[ \t]+\n", "\n\n");
        while (contents.Contains("\n\n\n"))
        {
            contents = contents.Replace("\n\n\n", "\n\n");
        }

        CsFile.WriteAllText(path, contents.Trim('\n'));
    }

    private static CompilationUnitSyntax ConvertToFileScopedNamespace(CompilationUnitSyntax root)
    {
        var namespaces = root.Members.OfType<NamespaceDeclarationSyntax>().ToList();

        if (namespaces.Count != 1)
            return root;

        var ns = namespaces[0];

        if (ns.Members.OfType<NamespaceDeclarationSyntax>().Any())
            return root;

        var newRoot = root
            .WithUsings(root.Usings.AddRange(ns.Usings))
            .WithExterns(root.Externs.AddRange(ns.Externs));

        var cleanName = ns.Name.WithoutTrailingTrivia();

        var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            .WithTrailingTrivia(
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.CarriageReturnLineFeed); // blank line after namespace

        var fileScoped = SyntaxFactory.FileScopedNamespaceDeclaration(
                attributeLists: ns.AttributeLists,
                modifiers: ns.Modifiers,
                name: cleanName,
                externs: default,
                usings: default,
                members: ns.Members)
            .WithNamespaceKeyword(ns.NamespaceKeyword)
            .WithSemicolonToken(semicolon)
            .WithLeadingTrivia(ns.GetLeadingTrivia());

        return newRoot.ReplaceNode(ns, fileScoped);
    }

    private static CompilationUnitSyntax ProcessCompilationUnit(CompilationUnitSyntax root)
    {
        var types = root.Members.OfType<BaseTypeDeclarationSyntax>().ToList();
        var others = root.Members.Where(m => m is not BaseTypeDeclarationSyntax).ToList();

        var sortedTypes = SortTypes(types).ToList();

        var newMembers = new List<MemberDeclarationSyntax>();

        foreach (var member in others)
        {
            if (member is NamespaceDeclarationSyntax ns)
                newMembers.Add(ProcessNamespace(ns));
            else if (member is FileScopedNamespaceDeclarationSyntax fns)
                newMembers.Add(ProcessFileScopedNamespace(fns));
            else
                newMembers.Add(member);
        }

        newMembers.AddRange(sortedTypes);

        var newRoot = root.WithMembers(SyntaxFactory.List(newMembers));

        var rewriter = new ReturnBlankLineRewriter();
        return (CompilationUnitSyntax)rewriter.Visit(newRoot);
    }

    private static NamespaceDeclarationSyntax ProcessNamespace(NamespaceDeclarationSyntax ns)
    {
        var types = ns.Members.OfType<BaseTypeDeclarationSyntax>().ToList();
        var others = ns.Members.Where(m => m is not BaseTypeDeclarationSyntax).ToList();

        var sortedTypes = SortTypes(types).ToList();

        var newMembers = new List<MemberDeclarationSyntax>();
        newMembers.AddRange(others);
        newMembers.AddRange(sortedTypes);

        return ns.WithMembers(SyntaxFactory.List(newMembers));
    }

    private static FileScopedNamespaceDeclarationSyntax ProcessFileScopedNamespace(FileScopedNamespaceDeclarationSyntax ns)
    {
        var types = ns.Members.OfType<BaseTypeDeclarationSyntax>().ToList();
        var others = ns.Members.Where(m => m is not BaseTypeDeclarationSyntax).ToList();

        var sortedTypes = SortTypes(types).ToList();

        var newMembers = new List<MemberDeclarationSyntax>();
        newMembers.AddRange(others);
        newMembers.AddRange(sortedTypes);

        return ns.WithMembers(SyntaxFactory.List(newMembers));
    }

    private static IEnumerable<MemberDeclarationSyntax> SortTypes(IEnumerable<BaseTypeDeclarationSyntax> types)
    {
        var sortedTypes = types
            .OrderBy(GetAccessibilityOrder)
            .ThenBy(t => t.Identifier.Text)
            .Select(ProcessType)
            .ToList();

        for (int i = 0; i < sortedTypes.Count; i++)
        {
            bool isLast = i == sortedTypes.Count - 1;
            yield return AddBlankLineAfter(sortedTypes[i], isLast);
        }
    }

    private static BaseTypeDeclarationSyntax ProcessType(BaseTypeDeclarationSyntax type)
    {
        if (type is EnumDeclarationSyntax)
            return type; // enums are not reordered

        SyntaxList<MemberDeclarationSyntax> members = default;

        switch (type)
        {
            case ClassDeclarationSyntax cls:
                members = cls.Members;
                break;

            case StructDeclarationSyntax str:
                members = str.Members;
                break;

            case InterfaceDeclarationSyntax iface:
                members = iface.Members;
                break;

            case RecordDeclarationSyntax record:
                members = record.Members;
                break;

            default:
                return type;
        }
        var fields = members.OfType<FieldDeclarationSyntax>();
        var eventsField = members.OfType<EventFieldDeclarationSyntax>();
        var eventsDecl = members.OfType<EventDeclarationSyntax>();
        var ctors = members.OfType<ConstructorDeclarationSyntax>();
        var props = members.OfType<PropertyDeclarationSyntax>();
        var indexers = members.OfType<IndexerDeclarationSyntax>();
        var methods = members.OfType<MethodDeclarationSyntax>();
        var delegates = members.OfType<DelegateDeclarationSyntax>();
        var innerTypes = members.OfType<BaseTypeDeclarationSyntax>();

        var orderedMembers = new List<MemberDeclarationSyntax>();

        orderedMembers.AddRange(SortMemberGroup(fields));
        orderedMembers.AddRange(SortMemberGroup(eventsField));
        orderedMembers.AddRange(SortMemberGroup(eventsDecl));
        orderedMembers.AddRange(SortMemberGroup(ctors));
        orderedMembers.AddRange(SortMemberGroup(props));
        orderedMembers.AddRange(SortMemberGroup(indexers));
        orderedMembers.AddRange(SortMemberGroup(methods));
        orderedMembers.AddRange(SortMemberGroup(delegates));

        var sortedInnerTypes = SortTypes(innerTypes).ToList();
        orderedMembers.AddRange(sortedInnerTypes);

        // Return a new type node with updated members
        return type switch
        {
            ClassDeclarationSyntax cls => cls.WithMembers(SyntaxFactory.List(orderedMembers)),
            StructDeclarationSyntax str => str.WithMembers(SyntaxFactory.List(orderedMembers)),
            InterfaceDeclarationSyntax iface => iface.WithMembers(SyntaxFactory.List(orderedMembers)),
            RecordDeclarationSyntax rec => rec.WithMembers(SyntaxFactory.List(orderedMembers)),
            _ => type
        };
    }

    private static IEnumerable<MemberDeclarationSyntax> SortMemberGroup(IEnumerable<MemberDeclarationSyntax> members)
    {
        var grouped = members
            .GroupBy(GetAccessibilityOrder)
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            var sortedGroup = group.OrderBy(GetMemberName).ToList();

            for (int i = 0; i < sortedGroup.Count; i++)
            {
                yield return AddBlankLineAfter(sortedGroup[i], isLast: false);
            }
        }
    }

    private static int GetAccessibilityOrder(MemberDeclarationSyntax member)
    {
        var modifiers = member.GetModifiers();

        if (modifiers.Any(SyntaxKind.PublicKeyword)) return 0;
        if (modifiers.Any(SyntaxKind.InternalKeyword)) return 1;
        if (!modifiers.Any()) return 2;
        if (modifiers.Any(SyntaxKind.ProtectedKeyword)) return 3;
        if (modifiers.Any(SyntaxKind.PrivateKeyword)) return 4;

        return 5;
    }

    private static string GetMemberName(MemberDeclarationSyntax member)
    {
        return member switch
        {
            FieldDeclarationSyntax f => f.Declaration.Variables.First().Identifier.Text,
            EventFieldDeclarationSyntax e => e.Declaration.Variables.First().Identifier.Text,
            EventDeclarationSyntax e => e.Identifier.Text,
            PropertyDeclarationSyntax p => p.Identifier.Text,
            IndexerDeclarationSyntax => "this",
            MethodDeclarationSyntax m => m.Identifier.Text,
            ConstructorDeclarationSyntax c => c.Identifier.Text,
            DelegateDeclarationSyntax d => d.Identifier.Text,
            BaseTypeDeclarationSyntax t => t.Identifier.Text,
            _ => ""
        };
    }

    private static T AddBlankLineAfter<T>(T member, bool isLast) where T : MemberDeclarationSyntax
    {
        if (isLast)
            return member; // do not add blank line after the last member by contract

        var trailingTrivia = member.GetTrailingTrivia();

        // Add exactly one blank line
        trailingTrivia = trailingTrivia
            .Add(SyntaxFactory.CarriageReturnLineFeed)   // end current line
            .Add(SyntaxFactory.CarriageReturnLineFeed);  // blank line

        return member.WithTrailingTrivia(trailingTrivia);
    }

}

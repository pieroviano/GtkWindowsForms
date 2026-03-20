using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class EventOnRewriter(string[]? eventsToExclude, Dictionary<string, string> handlerToArgs) : CSharpSyntaxRewriter
{
    private readonly string[] _eventsToExclude = eventsToExclude ?? [];

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var isSealed = node.Modifiers.Any(m => m.IsKind(SyntaxKind.SealedKeyword));

        var events = node.Members
            .OfType<EventFieldDeclarationSyntax>()
            .Where(e =>
                e.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)) &&
                !e.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            .ToList();

        var methodsToAdd = new List<MethodDeclarationSyntax>();
        var eventsToAdd = new List<EventFieldDeclarationSyntax>();

        // Process existing events to generate missing On methods
        foreach (var ev in events)
        {
            var handlerType = ev.Declaration.Type.ToString();
            var handlerFull = ResolveHandler(handlerType.TrimEnd('?'));

            if (!handlerToArgs.TryGetValue(handlerFull, out var eventArgsType))
            {
                // Special case for System.EventHandler (should map to System.EventArgs)
                if (handlerFull == "EventHandler" || handlerFull == "System.EventHandler")
                {
                    eventArgsType = "EventArgs";
                }
                else
                {
                    eventArgsType = handlerFull.Replace("Handler", "Args");
                    ConsoleColor.Yellow.Write($"Handler type '{handlerType}' not mapped.", true);
                }
            }

            foreach (var variable in ev.Declaration.Variables)
            {
                var eventName = variable.Identifier.Text;
                var methodName = "On" + eventName;

                // Add On method if it doesn't exist
                if (!node.Members.OfType<MethodDeclarationSyntax>().Any(m => m.Identifier.Text == methodName))
                {
                    methodsToAdd.Add(CreateOnMethod(eventName, methodName, eventArgsType, isSealed));
                }
            }
        }

        // Process existing OnXXX methods to generate missing events
        var onMethods = node.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(m => m.Identifier.Text.StartsWith("On") &&
                       m.ReturnType is PredefinedTypeSyntax pt && pt.Keyword.IsKind(SyntaxKind.VoidKeyword) &&
                       m.ParameterList.Parameters.Count == 1 &&
                       (m.Modifiers.Any(mod => mod.IsKind(SyntaxKind.ProtectedKeyword)) ||
                        m.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PrivateKeyword))))
            .ToList();

        foreach (var method in onMethods)
        {
            var methodName = method.Identifier.Text;
            var eventName = methodName.Substring(2); // Remove "On" prefix

            // Check if event already exists (both field-like events and events with custom accessors)
            var fieldEventExists = node.Members.OfType<EventFieldDeclarationSyntax>()
                .Any(e => e.Declaration.Variables.Any(v => v.Identifier.Text == eventName));

            var customEventExists = node.Members.OfType<EventDeclarationSyntax>()
                .Any(e => e.Identifier.Text == eventName);

            if (!fieldEventExists && !customEventExists && !eventsToAdd.Any(e => e.Declaration.Variables.Any(v => v.Identifier.Text == eventName)))
            {
                var parameter = method.ParameterList.Parameters[0];
                var eventArgsType = parameter.Type?.ToString();

                if (!string.IsNullOrEmpty(eventArgsType))
                {
                    eventsToAdd.Add(CreateEventHandlerEvent(eventName, eventArgsType));
                }
            }
        }

        var updatedNode = node;
        if (eventsToAdd.Count > 0)
            updatedNode = updatedNode.AddMembers(eventsToAdd.ToArray());

        if (methodsToAdd.Count > 0)
            updatedNode = updatedNode.AddMembers(methodsToAdd.ToArray());

        return updatedNode;
    }

    private string ResolveHandler(string handler)
    {
        if (handlerToArgs.ContainsKey(handler))
            return handler;

        var match = handlerToArgs.Keys.FirstOrDefault(k => k.EndsWith("." + handler));
        return match ?? handler;
    }

    private MethodDeclarationSyntax CreateOnMethod(
        string eventName,
        string methodName,
        string eventArgsType,
        bool isSealed)
    {
        var xmlComment = SyntaxFactory.ParseLeadingTrivia(
        $@"/// <summary>
    /// Raises the {eventName} event.
    /// </summary>
    ");

        var modifiers = isSealed
        ? new[]
        {
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.PrivateKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space))
        }
        : new[]
        {
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.ProtectedKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.VirtualKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space))
        };

        var method = SyntaxFactory.MethodDeclaration(
            SyntaxFactory.PredefinedType(SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.VoidKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space))),
            methodName)
        .AddModifiers(modifiers)
        .AddParameterListParameters(
            SyntaxFactory.Parameter(SyntaxFactory.Identifier("e"))
                .WithType(SyntaxFactory.ParseTypeName(eventArgsType)
                    .WithTrailingTrivia(SyntaxFactory.Space)))
        .WithLeadingTrivia(xmlComment);

        // Create statements depending on exclusion
        var statements = !_eventsToExclude.Contains(eventName)
        ? SyntaxFactory.SingletonList<StatementSyntax>(
            SyntaxFactory.ParseStatement($"{eventName}?.Invoke(this, e);"))
        : default; // empty block

        var body = SyntaxFactory.Block(statements)
        .WithOpenBraceToken(
            SyntaxFactory.Token(SyntaxKind.OpenBraceToken)
                .WithLeadingTrivia(SyntaxFactory.Space)
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed))
        .WithCloseBraceToken(
            SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                .WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));

        return method.WithBody(body);
    }
    private EventFieldDeclarationSyntax CreateEventHandlerEvent(string eventName, string eventArgsType)
    {
        return SyntaxFactory.EventFieldDeclaration(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier("EventHandler"))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.ParseTypeName(eventArgsType))))
                        .WithTrailingTrivia(SyntaxFactory.Space))
                    .AddVariables(SyntaxFactory.VariableDeclarator(eventName)))
            .AddModifiers(SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.PublicKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space)))
            .NormalizeWhitespace();
    }
}
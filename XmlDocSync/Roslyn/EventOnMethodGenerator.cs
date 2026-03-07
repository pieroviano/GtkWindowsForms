using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public static class EventOnMethodGenerator
{
    public static Dictionary<string, string> DefaultMap =>
        new()
        {
            { "System.EventHandler", "System.EventArgs" },
            { "System.Windows.Forms.ControlEventHandler", "System.Windows.Forms.ControlEventArgs" },
            { "System.Windows.Forms.DragEventHandler", "System.Windows.Forms.DragEventArgs" },
            { "System.Windows.Forms.GiveFeedbackEventHandler", "System.Windows.Forms.GiveFeedbackEventArgs" },
            { "System.Windows.Forms.HelpEventHandler", "System.Windows.Forms.HelpEventArgs" },
            { "System.Windows.Forms.InvalidateEventHandler", "System.Windows.Forms.InvalidateEventArgs" },
            { "System.Windows.Forms.KeyEventHandler", "System.Windows.Forms.KeyEventArgs" },
            { "System.Windows.Forms.KeyPressEventHandler", "System.Windows.Forms.KeyPressEventArgs" },
            { "System.Windows.Forms.LayoutEventHandler", "System.Windows.Forms.LayoutEventArgs" },
            { "System.Windows.Forms.MouseEventHandler", "System.Windows.Forms.MouseEventArgs" },
            { "System.Windows.Forms.PreviewKeyDownEventHandler", "System.Windows.Forms.PreviewKeyDownEventArgs" },
            { "System.Windows.Forms.QueryAccessibilityHelpEventHandler", "System.Windows.Forms.QueryAccessibilityHelpEventArgs" },
            { "System.Windows.Forms.QueryContinueDragEventHandler", "System.Windows.Forms.QueryContinueDragEventArgs" },
            { "System.Windows.Forms.UICuesEventHandler", "System.Windows.Forms.UICuesEventArgs" },
            { "System.ComponentModel.CancelEventHandler", "System.ComponentModel.CancelEventArgs" },
            { "System.ComponentModel.CollectionChangeEventHandler", "System.ComponentModel.CollectionChangeEventArgs" },
            { "System.EventHandler<DialogEventArgs>", "System.Windows.Forms.DialogEventArgs" },
        };

    public static string Process(string sourceCode, Dictionary<string, string>? handlerToArgs = null)
    {
        handlerToArgs ??= DefaultMap;
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();

        var rewriter = new EventRewriter(handlerToArgs);

        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString().Replace("\r\n", "\n").TrimEnd("\n");
    }

    private class EventRewriter : CSharpSyntaxRewriter
    {
        private readonly Dictionary<string, string> _handlerToArgs;

        public EventRewriter(Dictionary<string, string> handlerToArgs)
        {
            _handlerToArgs = handlerToArgs;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var events = node.Members
                .OfType<EventFieldDeclarationSyntax>()
                .Where(e =>
                    e.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)) &&
                    !e.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
                .ToList();

            var methodsToAdd = new List<MethodDeclarationSyntax>();

            foreach (var ev in events)
            {
                var handlerType = ev.Declaration.Type.ToString();

                var handlerFull = ResolveHandler(handlerType.TrimEnd("?"));

                if (!_handlerToArgs.TryGetValue(handlerFull, out var eventArgsType))
                {
                    eventArgsType = handlerFull.Replace("Handler", "Args");
                    ConsoleColor.Yellow.Write($"Handler type '{handlerType}' not mapped.", true);
                }

                foreach (var variable in ev.Declaration.Variables)
                {
                    var eventName = variable.Identifier.Text;
                    var methodName = "On" + eventName;

                    if (node.Members.OfType<MethodDeclarationSyntax>()
                        .Any(m => m.Identifier.Text == methodName))
                        continue;

                    methodsToAdd.Add(CreateOnMethod(eventName, methodName, eventArgsType));
                }
            }

            if (methodsToAdd.Count == 0)
                return node;

            return node.AddMembers(methodsToAdd.ToArray());
        }

        private string ResolveHandler(string handler)
        {
            if (_handlerToArgs.ContainsKey(handler))
                return handler;

            var match = _handlerToArgs.Keys
                .FirstOrDefault(k => k.EndsWith("." + handler));

            return match ?? handler;
        }

        private MethodDeclarationSyntax CreateOnMethod(string eventName, string methodName, string eventArgsType)
        {
            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    methodName)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                    SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
                .AddParameterListParameters(
                    SyntaxFactory.Parameter(SyntaxFactory.Identifier("e"))
                        .WithType(SyntaxFactory.ParseTypeName(eventArgsType)))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.ParseStatement(
                            $"{eventName}?.Invoke(this, e);"
                        )));
        }
    }
}

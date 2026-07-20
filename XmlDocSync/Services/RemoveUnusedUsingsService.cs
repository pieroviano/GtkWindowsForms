using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text.Json;
using XmlDocSync.Models;
using XmlDocSync.Roslyn;

namespace XmlDocSync.Services;

public static class RemoveUnusedUsingsService
{
    private static List<UsingDifference>? _usingDifferences;

    public static void ProcessFile(string filePath)
    {
        EnsureUsingDifferencesLoaded();

        var source = File.ReadAllText(filePath);

        var tree = CSharpSyntaxTree.ParseText(source);

        // Add more assembly references to help semantic model resolve symbols
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        };

        try
        {
            references.Add(MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location));
        }
        catch (Exception ex)
        {
            LogReferenceWarning("System.Collections", filePath, ex);
        }

        try
        {
            references.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.MethodImplAttribute).Assembly.Location));
        }
        catch (Exception ex)
        {
            LogReferenceWarning("System.Runtime", filePath, ex);
        }

        try
        {
            references.Add(
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location));
        }
        catch (Exception ex)
        {
            LogReferenceWarning("System.ComponentModel", filePath, ex);
        }

        // Try to add System.Drawing (for types like Size, Point, Color, etc.)
        try
        {
            Type? sizeType = null;

            try
            {
                sizeType = Type.GetType("System.Drawing.Size, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
            catch (Exception ex)
            {
                ConsoleColor.Yellow.Write(
                    $"Warning: Failed to resolve System.Drawing.Size from System.Drawing assembly in {filePath}.{Environment.NewLine}{ex.Message}",
                    true);
            }

            if (sizeType == null)
            {
                try
                {
                    sizeType = Type.GetType("System.Drawing.Size, System.Drawing.Common");
                }
                catch (Exception ex)
                {
                    ConsoleColor.Yellow.Write(
                        $"Warning: Failed to resolve System.Drawing.Size from System.Drawing.Common assembly in {filePath}.{Environment.NewLine}{ex.Message}",
                        true);
                }
            }

            if (sizeType == null)
            {
                try
                {
                    sizeType = Type.GetType("System.Drawing.Size, System.Drawing.Primitives");
                }
                catch (Exception ex)
                {
                    ConsoleColor.Yellow.Write(
                        $"Warning: Failed to resolve System.Drawing.Size from System.Drawing.Primitives assembly in {filePath}.{Environment.NewLine}{ex.Message}",
                        true);
                }
            }

            if (sizeType != null && !string.IsNullOrEmpty(sizeType.Assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(sizeType.Assembly.Location));
            }
            else
            {
                var drawingAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name?.StartsWith("System.Drawing") == true && !a.IsDynamic);

                if (drawingAssembly != null && !string.IsNullOrEmpty(drawingAssembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(drawingAssembly.Location));
                }
            }
        }
        catch (Exception ex)
        {
            LogReferenceWarning("System.Drawing", filePath, ex);
        }

        var compilation = CSharpCompilation.Create("Cleanup")
            .AddReferences(references)
            .AddSyntaxTrees(tree);

        var semanticModel = compilation.GetSemanticModel(tree);

        var root = tree.GetCompilationUnitRoot();

        var addedUsings = GetAddedUsingsForFile(filePath);

        var rewriter = new RemoveUnusedUsingsRewriter(semanticModel, addedUsings, root);

        var newRoot = (CompilationUnitSyntax)rewriter.Visit(root);

        newRoot = EnsureAddedUsingsPresent(newRoot, addedUsings);

        File.WriteAllText(filePath, newRoot.ToFullString());
    }

    private static void LogReferenceWarning(string assemblyName, string filePath, Exception ex)
    {
        ConsoleColor.Yellow.Write(
            $"Warning: Could not load {assemblyName} assembly reference. Some usings may be incorrectly removed in {filePath}.{Environment.NewLine}{ex.Message}",
            true);
    }

    private static CompilationUnitSyntax EnsureAddedUsingsPresent(
        CompilationUnitSyntax root,
        HashSet<string> addedUsings)
    {
        var existingUsingNodes = root.Usings.ToList();

        var normalExistingUsings = existingUsingNodes
            .Where(u => u.StaticKeyword.IsKind(SyntaxKind.None) && u.Alias == null)
            .ToList();

        var existingNamespaces = normalExistingUsings
            .Select(u => u.Name?.ToString())
            .ToHashSet();

        var missingUsings = addedUsings
            .Where(u => !existingNamespaces.Contains(u))
            .ToList();

        if (missingUsings.Count == 0)
            return root;

        SyntaxTriviaList leadingTrivia =
            existingUsingNodes.FirstOrDefault()?.GetLeadingTrivia() ?? SyntaxFactory.TriviaList();

        var newUsingNodes = missingUsings
            .Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u))
                .WithUsingKeyword(SyntaxFactory.Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(SyntaxFactory.Space))
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed))
            .ToList();

        newUsingNodes[0] = newUsingNodes[0].WithLeadingTrivia(leadingTrivia);

        var combined = existingUsingNodes
            .Concat(newUsingNodes)
            .OrderBy(u => u.Name?.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToList();

        return root.WithUsings(SyntaxFactory.List(combined));
    }

    private static void EnsureUsingDifferencesLoaded()
    {
        if (_usingDifferences != null)
            return;

        var jsonPath = Prepare.Common.IO.RepoPaths.At(@"Resources\using-differences.json");

        if (!File.Exists(jsonPath))
        {
            _usingDifferences = new();
            return;
        }

        var json = File.ReadAllText(jsonPath);

        _usingDifferences =
            JsonSerializer.Deserialize<List<UsingDifference>>(json) ?? new();
    }

    private static HashSet<string> GetAddedUsingsForFile(string filePath)
    {
        var match = _usingDifferences?
            .FirstOrDefault(x => x.FileName != null && filePath.EndsWith(x.FileName, StringComparison.OrdinalIgnoreCase));

        return match == null
            ? new HashSet<string>()
            : match.AddedUsings.ToHashSet();
    }
}
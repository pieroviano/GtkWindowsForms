using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;
using XmlDocSync.Services;

namespace Prepare.Tests;

public class RemoveUnusedUsingsRewriterTests
{
    private string Rewrite(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create("Test")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.MethodImplAttribute).Assembly.Location))
            .AddSyntaxTrees(tree);

        var semanticModel = compilation.GetSemanticModel(tree);

        var root = tree.GetCompilationUnitRoot();

        var rewriter = new RemoveUnusedUsingsRewriter(semanticModel, new HashSet<string>(), root);

        var newRoot = rewriter.Visit(root);

        return newRoot.ToFullString();
    }

    [Fact]
    public void RemovesUnusedUsing()
    {
        var source = @"
using System;
using System.Text;

class Test
{
    void Run()
    {
        Console.WriteLine(""Hello"");
    }
}";

        var result = Rewrite(source);

        Assert.Contains("using System;", result);
        Assert.DoesNotContain("using System.Text;", result);
    }

    [Fact]
    public void KeepsUsedUsing()
    {
        var source = @"
using System.Text;

class Test
{
    void Run()
    {
        var sb = new StringBuilder();
    }
}";

        var result = Rewrite(source);

        Assert.Contains("using System.Text;", result);
    }

    [Fact]
    public void RemovesMultipleUnusedUsings()
    {
        var source = @"
using System;
using System.Text;
using System.Globalization;

class Test
{
    void Run()
    {
        Console.WriteLine(""Hi"");
    }
}";

        var result = Rewrite(source);

        Assert.Contains("using System;", result);
        Assert.DoesNotContain("System.Text", result);
        Assert.DoesNotContain("System.Globalization", result);
    }

    [Fact]
    public void PreservesTriviaBeforeUsings()
    {
        var source = @"// Licensed header
// More comments

using System;
using System.Text;

class Test { }";

        var result = Rewrite(source);

        Assert.Contains("// Licensed header", result);
        Assert.Contains("// More comments", result);
    }

    [Fact]
    public void PreservesTriviaWhenAllUsingsRemoved()
    {
        var source = @"// Licensed header

using System.Text;

namespace Test;";

        var result = Rewrite(source);

        Assert.Contains("// Licensed header", result);
    }

    [Fact]
    public void KeepsUsingsForAttributesWhenSymbolNotResolved()
    {
        var source = @"using System.Runtime.CompilerServices;

class Test
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Run() { }
}";

        var result = Rewrite(source);

        // Should keep the using even if semantic model can't fully resolve it
        Assert.Contains("System.Runtime.CompilerServices", result);
    }

    [Fact]
    public void KeepsUsingsForGenericTypesWhenSymbolNotResolved()
    {
        var source = @"using System.Collections.Generic;

class Test
{
    IEqualityComparer<int> comparer;
}";

        var result = Rewrite(source);

        // Should keep the using even if semantic model can't fully resolve it
        Assert.Contains("System.Collections.Generic", result);
    }

    [Theory]
    [InlineData("HashCode.cs", "using System.Runtime.CompilerServices")]
    [InlineData("IScrollableBoxBase.cs", "using Gtk")]
    [InlineData("ToolStrip.cs", "using System.Drawing")]
    [InlineData("ToolStripItem.cs", "using System.ComponentModel")]
    [InlineData("Button.cs", "using System.ComponentModel")]
    [InlineData("PrintEventArgs.cs", "using System.ComponentModel")]
    public void RemoveUnusedUsingsService_ShouldProcessSingleFile(
        string fileName,
        string expectedUsing)
    {
        var guidFolder = Path.Combine(AppContext.BaseDirectory, Guid.NewGuid().ToString());
        Directory.CreateDirectory(guidFolder);

        try
        {
            var sourceFolder = Path.GetFullPath(@"..\..\..\UnusedUsings");

            var sourceFile = Directory
                .GetFiles(sourceFolder, fileName, SearchOption.AllDirectories)
                .First();

            var destFile = Path.Combine(guidFolder, fileName);
            File.Copy(sourceFile, destFile);

            RemoveUnusedUsingsService.ProcessFile(destFile);

            var content = File.ReadAllText(destFile);

            Assert.Contains(expectedUsing, content);
        }
        finally
        {
            if (Directory.Exists(guidFolder))
                Directory.Delete(guidFolder, true);
        }
    }
}

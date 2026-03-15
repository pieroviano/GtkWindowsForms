using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class UseVarRewriterTests
{
    [Fact]
    public void ConvertsInsideConstructor()
    {
        var input = """
                    class Test
                    {
                        public Test()
                        {
                            string name = "abc";
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("var name = \"abc\";", output);
    }

    [Fact]
    public void ConvertsInsidePropertyGetter()
    {
        var input = """
                    class Test
                    {
                        public int Value
                        {
                            get
                            {
                                int x = 10;
                                return x;
                            }
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("var x = 10;", output);
    }

    [Fact]
    public void ConvertsSimpleLocalVariable()
    {
        var input = """
                    class Test
                    {
                        void M()
                        {
                            int x = 5;
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("var x = 5;", output);
    }

    [Fact]
    public void DoesNotChangeExistingVar()
    {
        var input = """
                    class Test
                    {
                        void M()
                        {
                            var x = 5;
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("var x = 5;", output);
    }

    [Fact]
    public void DoesNotConvertConstVariables()
    {
        var input = """
                    class Test
                    {
                        void M()
                        {
                            const int x = 5;
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("const int x = 5;", output);
        Assert.DoesNotContain("const var", output);
    }

    [Fact]
    public void DoesNotConvertDefaultLiteralInitialization()
    {
        var input = """
                    using System;

                    class Test
                    {
                        void M()
                        {
                            HashCode hash = default;
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("HashCode hash = default;", output);
        Assert.DoesNotContain("var hash = default", output);
    }

    [Fact]
    public void DoesNotConvertTargetTypedNew()
    {
        var input = """
                    using System.Windows.Forms;

                    class Test
                    {
                        void M()
                        {
                            Binding binding = new(
                                "Text",
                                this,
                                "Name",
                                true,
                                DataSourceUpdateMode.OnPropertyChanged,
                                null,
                                "",
                                null);
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("Binding binding = new(", output);
        Assert.DoesNotContain("var binding", output);
    }

    [Fact]
    public void DoesNotConvertWhenTypesDiffer()
    {
        var input = """
                    class Test
                    {
                        void M()
                        {
                            object x = "hello";
                        }
                    }
                    """;

        var output = Rewrite(input);

        Assert.Contains("object x = \"hello\";", output);
    }

    private string Rewrite(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);

        var compilation = CSharpCompilation.Create(
            "Test",
            new[] { tree },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            });

        var model = compilation.GetSemanticModel(tree);

        var rewriter = new UseVarRewriter(model);

        var root = tree.GetRoot();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }
}
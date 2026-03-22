using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class TypeSplitterTests
{
    [Fact]
    public void Split_Should_Return_Single_File_When_Only_One_Type()
    {
        var code = """
                   public class A
                   {
                   }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Single(result);
        Assert.True(result.ContainsKey("A.cs"));
    }

    [Fact]
    public void Split_Should_Split_Multiple_Types()
    {
        var code = """
                   public class A { }
                   public class B { }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Equal(2, result.Count);
        Assert.Contains("A.cs", result.Keys);
        Assert.Contains("B.cs", result.Keys);
    }

    [Fact]
    public void Split_Should_Preserve_Usings()
    {
        var code = """
                   using System;

                   public class A { }
                   public class B { }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Contains("using System;", result["A.cs"]);
        Assert.Contains("using System;", result["B.cs"]);
    }

    [Fact]
    public void Split_Should_Handle_Namespace()
    {
        var code = """
                   namespace TestNamespace
                   {
                       public class A { }
                       public class B { }
                   }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Equal(2, result.Count);
        Assert.Contains("namespace TestNamespace", result["A.cs"]);
        Assert.Contains("namespace TestNamespace", result["B.cs"]);
    }

    [Fact]
    public void Split_Should_Handle_FileScopedNamespace()
    {
        var code = """
                   namespace TestNamespace;

                   public class A { }
                   public class B { }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Equal(2, result.Count);
        Assert.Contains("namespace TestNamespace;", result["A.cs"]);
        Assert.Contains("namespace TestNamespace;", result["B.cs"]);
    }

    [Fact]
    public void Split_Should_Ignore_Non_Type_Members()
    {
        var code = """
                   using System;

                   public class A { }

                   // some comment
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Single(result);
        Assert.True(result.ContainsKey("A.cs"));
    }

    [Fact]
    public void Split_Should_Preserve_All_Members_Including_Nested_Types()
    {
        var code = """
                   namespace TestNamespace;

                   public class ImageList
                   {
                       private string _field;

                       public string Property { get; set; }

                       public void Method() { }

                       private enum OriginalOptions
                       {
                           Default = 0
                       }
                   }
                   """;

        var result = TypeSplitter.Split(code);

        Assert.Single(result);
        var content = result["ImageList.cs"];

        Assert.Contains("private string _field;", content);
        Assert.Contains("public string Property", content);
        Assert.Contains("public void Method()", content);
        Assert.Contains("private enum OriginalOptions", content);
    }
}
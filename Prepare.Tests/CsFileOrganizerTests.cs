using Prepare.Common.Content;
using Prepare.Tests.Properties;
using XmlDocSync.Roslyn;

namespace Prepare.Tests;

public class CsFileOrganizerTests
{
    [Fact]
    public void AddsBlankLineBeforeReturnWithStatements()
    {
        var code = @"
class A
{
    public int Divide(int a,int b)
    {
        if(b==0) throw new System.ArgumentException(""b"");
        // comment
        return a+b;
    }
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.Contains("throw new System.ArgumentException(\"b\");\n\n        // comment", result);
    }

    [Fact]
    public void Does_Not_Reorder_Enum_Members()
    {
        var code = @"
enum E
{
    C,
    B,
    A
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.Contains("C,\n    B,\n    A", result.Replace("\r\n", "\n"));
    }

    [Fact]
    public void Handles_Comments()
    {
        var code = @"
// comment
class B{}

class A{}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.Contains("// comment", result);
    }

    [Fact]
    public void OrganizesBindingsCs()
    {
        var code = Resources.Binding;

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.Length >= code.Length * 0.9);
    }

    [Fact]
    public void OtherAddsBlankLineBeforeReturnWithStatements()
    {
        var code = @"
class A
{
    public int Divide(int a,int b)
    {
        if(b==0) throw new System.ArgumentException(""b"");
        
        // comment
        return a+b;
    }
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.Contains("throw new System.ArgumentException(\"b\");\n\n        // comment", result);
    }

    [Fact]
    public void Sorts_By_Accessibility()
    {
        var code = @"
class A
{
    private int B;
    public int A;
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("public int A") <
                    result.IndexOf("private int B"));
    }

    [Fact]
    public void Sorts_By_Name()
    {
        var code = @"
class A
{
    public void Z(){}
    public void A(){}
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("void A") <
                    result.IndexOf("void Z"));
    }

    [Fact]
    public void Sorts_Delegates()
    {
        var code = """
                   class A
                   {
                       public delegate void Z();
                       public delegate void A();
                   }
                   """;

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("delegate void A") < result.IndexOf("delegate void Z"));
    }

    [Fact]
    public void Sorts_EventFields()
    {
        var code = """
                   class A
                   {
                       public event System.EventHandler Z;
                       public event System.EventHandler A;
                   }
                   """;

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf(" A;") < result.IndexOf(" Z;"));
    }

    [Fact]
    public void Sorts_Members_By_Category()
    {
        var code = @"
class A
{
    public void M(){}
    public int P {get;set;}
    private int x;
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("int x") <
                    result.IndexOf("P"));
    }

    [Fact]
    public void Sorts_Nested_Types()
    {
        var code = @"
class A
{
    class Z{}
    class B{}
}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("class B") <
                    result.IndexOf("class Z"));
    }

    [Fact]
    public void Sorts_Record_Members()
    {
        var code = """
                   record A
                   {
                       public void Z(){}
                       public void A(){}
                   }
                   """;

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("void A") < result.IndexOf("void Z"));
    }

    [Fact]
    public void Sorts_Types_By_Accessibility_And_Name()
    {
        var code = @"
class B {}
public class A {}
";

        var path = WriteTempFile(code);

        CsFileOrganizer.ProcessFile(path);

        var result = File.ReadAllText(path);

        Assert.True(result.IndexOf("public class A") <
                    result.IndexOf("class B"));
    }

    private string WriteTempFile(string code)
    {
        var path = Path.GetTempFileName();

        CsFile.WriteAllText(path, code);

        return path;
    }
}

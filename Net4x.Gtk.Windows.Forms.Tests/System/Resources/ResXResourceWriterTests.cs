using System.Reflection;
using System.Text;

namespace System.Resources;

public class ResXResourceWriterTests
{
    [Fact]
    public void Ctor_FileName_SetsFileName()
    {
        var writer = new ResXResourceWriter("test.resx");
        Assert.NotNull(writer);
    }

    [Fact]
    public void Ctor_Stream_SetsStream()
    {
        using var ms = new MemoryStream();
        var writer = new ResXResourceWriter(ms);
        Assert.NotNull(writer);
    }

    [Fact]
    public void Ctor_TextWriter_SetsTextWriter()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        Assert.NotNull(writer);
    }

    [Fact]
    public void AddResource_String_WritesResource()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        writer.AddResource("key", "value");
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("key", xml);
        Assert.Contains("value", xml);
    }

    [Fact]
    public void AddResource_ByteArray_WritesResource()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        var data = Encoding.UTF8.GetBytes("abc");
        writer.AddResource("bytes", data);
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("bytes", xml);
    }

    [Fact]
    public void AddResource_Object_WritesResource()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        writer.AddResource("num", 42);
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("num", xml);
        Assert.Contains("42", xml);
    }

    [Fact]
    public void AddMetadata_String_WritesMetadata()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        writer.AddMetadata("meta", "data");
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("meta", xml);
        Assert.Contains("data", xml);
    }

    [Fact]
    public void AddMetadata_ByteArray_WritesMetadata()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        var data = Encoding.UTF8.GetBytes("meta");
        writer.AddMetadata("metaBytes", data);
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("metaBytes", xml);
    }

    [Fact]
    public void AddAlias_AddsAliasAndAssemblyRow()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        var asm = Assembly.GetExecutingAssembly().GetName();
        writer.AddAlias("alias", asm);
        writer.AddResource("key", "value");
        writer.Generate();
        var xml = sw.ToString();
        Assert.Contains("alias", xml);
    }

    [Fact]
    public void Generate_Twice_ThrowsInvalidOperationException()
    {
        using var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        writer.AddResource("key", "value");
        writer.Generate();
        Assert.Throws<InvalidOperationException>(() => writer.Generate());
    }

    [Fact]
    public void Dispose_CallsGenerateAndClosesWriters()
    {
        var sw = new StringWriter();
        var writer = new ResXResourceWriter(sw);
        writer.AddResource("key", "value");
        writer.Dispose();
        var xml = sw.ToString();
        Assert.Contains("key", xml);
    }
}
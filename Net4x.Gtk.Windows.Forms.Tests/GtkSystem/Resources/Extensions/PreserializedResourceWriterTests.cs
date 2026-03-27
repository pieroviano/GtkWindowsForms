namespace GtkSystem.Resources.Extensions;

public class PreserializedResourceWriterTests
{
    [Fact]
    public void Constructor_WithNullFileName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PreserializedResourceWriter((string)null));
    }

    [Fact]
    public void Constructor_WithNullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PreserializedResourceWriter((Stream)null));
    }

    [Fact]
    public void Constructor_WithUnwritableStream_Throws()
    {
        var stream = new MemoryStream(new byte[1], writable: false);
        Assert.Throws<ArgumentException>(() => new PreserializedResourceWriter(stream));
    }

    [Fact]
    public void AddResource_String_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", "value");
        writer.Generate();
    }

    [Fact]
    public void AddResource_Object_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", 123);
        writer.Generate();
    }

    [Fact]
    public void AddResource_ByteArray_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", new byte[] { 1, 2, 3 });
        writer.Generate();
    }

    [Fact]
    public void AddResource_Stream_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        using var data = new MemoryStream(new byte[] { 1, 2, 3 });
        writer.AddResource("name", data);
        writer.Generate();
    }

    [Fact]
    public void AddBinaryFormattedResource_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddBinaryFormattedResource("name", new byte[] { 1, 2, 3 });
        writer.Generate();
    }

    [Fact]
    public void AddTypeConverterResource_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddTypeConverterResource("name", new byte[] { 1, 2, 3 }, typeof(int).FullName);
        writer.Generate();
    }

    [Fact]
    public void AddResource_StringStringString_AddsResource()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", "123", typeof(int).FullName);
        writer.Generate();
    }

    [Fact]
    public void AddActivatorResource_WithNulls_Throws()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        Assert.Throws<ArgumentNullException>(() => writer.AddActivatorResource(null, ms, "type"));
        Assert.Throws<ArgumentNullException>(() => writer.AddActivatorResource("name", null, "type"));
        Assert.Throws<ArgumentNullException>(() => writer.AddActivatorResource("name", ms, null));
    }

    [Fact]
    public void AddResource_StringStringString_WithNulls_Throws()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        Assert.Throws<ArgumentNullException>(() => writer.AddResource(null, "v", "t"));
        Assert.Throws<ArgumentNullException>(() => writer.AddResource("n", null, "t"));
        Assert.Throws<ArgumentNullException>(() => writer.AddResource("n", "v", null));
    }

    [Fact]
    public void AddResource_String_ThrowsIfSaved()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", "value");
        writer.Generate();
        Assert.Throws<InvalidOperationException>(() => writer.AddResource("name2", "value2"));
    }

    [Fact]
    public void Generate_ThrowsIfCalledTwice()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.AddResource("name", "value");
        writer.Generate();
        Assert.Throws<InvalidOperationException>(() => writer.Generate());
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.Dispose();
        writer.Dispose();
    }

    [Fact]
    public void Close_CanBeCalledMultipleTimes()
    {
        using var ms = new MemoryStream();
        var writer = new PreserializedResourceWriter(ms);
        writer.Close();
        writer.Close();
    }
}

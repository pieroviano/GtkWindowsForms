using System.Resources;

namespace Gtk.Resources.Extensions;

public class ResXExtensionsTests
{
    // ----------------------------
    // OrThrowIfNull (ResXFileRef)
    // ----------------------------

    [Fact]
    public void OrThrowIfNull_ResXFileRef_WhenNotNull_ReturnsValue()
    {
        var input = new ResXFileRef("file.txt", typeof(string).AssemblyQualifiedName);

        var result = input.OrThrowIfNull();

        Assert.Same(input, result);
    }

    [Fact]
    public void OrThrowIfNull_ResXFileRef_WhenNull_Throws()
    {
        ResXFileRef? input = null;

        var ex = Assert.Throws<ArgumentNullException>(() => { return input.OrThrowIfNull(); });

        Assert.Equal("Value cannot be null. (Parameter 'ResXFileRef is null')", ex.Message);
    }
    // ----------------------------
    // OrThrowIfNull (string)
    // ----------------------------

    [Fact]
    public void OrThrowIfNull_String_WhenNotNull_ReturnsValue()
    {
        var input = "test";

        var result = input.OrThrowIfNull();

        Assert.Equal(input, result);
    }

    [Fact]
    public void OrThrowIfNull_String_WhenNull_Throws()
    {
        string? input = null;

        var ex = Assert.Throws<ArgumentNullException>(() => input.OrThrowIfNull());
        Assert.Equal("Value cannot be null. (Parameter 'string Is Null Or Empty')", ex.Message);
    }

    [Fact]
    public void OrThrowIfNullOrEmpty_WhenEmpty_Throws()
    {
        var input = string.Empty;

        var ex = Assert.Throws<ArgumentException>(() => input.OrThrowIfNullOrEmpty());

        Assert.Equal("string Is Null Or Empty", ex.Message);
    }

    [Fact]
    public void OrThrowIfNullOrEmpty_WhenNull_Throws()
    {
        string? input = null;

        var ex = Assert.Throws<ArgumentException>(() => input.OrThrowIfNullOrEmpty());

        Assert.Equal("string Is Null Or Empty", ex.Message);
    }

    // ----------------------------
    // OrThrowIfNullOrEmpty (string)
    // ----------------------------

    [Fact]
    public void OrThrowIfNullOrEmpty_WhenValid_ReturnsValue()
    {
        var input = "valid";

        var result = input.OrThrowIfNullOrEmpty();

        Assert.Equal(input, result);
    }
}
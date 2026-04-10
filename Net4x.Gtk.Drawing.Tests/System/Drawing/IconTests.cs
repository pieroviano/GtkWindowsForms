extern alias gtk;

using Icon = gtk::System.Drawing.Icon;

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class IconTests
{
    [Fact]
    public void Clone_ReturnsNewIcon()
    {
        var icon = new Icon("test.ico");
        var clone = (Icon)icon.Clone();
        Assert.NotSame(icon, clone);
        Assert.Equal(icon.Width, clone.Width);
        Assert.Equal(icon.Height, clone.Height);
    }

    [Fact]
    public void Constructor_ByteArray_CreatesIcon()
    {
        // Arrange
        var bytes = File.ReadAllBytes("test.ico"); // Ensure test.ico exists in test output

        // Act
        var icon = new Icon(bytes);

        // Assert
        Assert.NotNull(icon.Pixbuf);
        Assert.True(icon.Width > 0);
        Assert.True(icon.Height > 0);
    }

    [Fact]
    public void Constructor_FileName_CreatesIcon()
    {
        var icon = new Icon("test.ico");
        Assert.NotNull(icon.Pixbuf);
    }

    [Fact]
    public void Constructor_Stream_CreatesIcon()
    {
        using var stream = File.OpenRead("test.ico");
        var icon = new Icon(stream);

        Assert.NotNull(icon.Pixbuf);
    }

    [Fact]
    public void Dispose_ClearsPixbuf()
    {
        var icon = new Icon("test.ico");
        icon.Dispose();
        Assert.Null(icon.PixbufData);
        Assert.Null(icon.Pixbuf);
    }

    [Fact]
    public void Save_WritesData()
    {
        var icon = new Icon("test.ico");
        using var ms = new MemoryStream();
        icon.Save(ms);
        Assert.True(ms.Length > 0);
    }
}
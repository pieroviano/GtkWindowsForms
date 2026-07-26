extern alias gtk;
using Icon = gtk::System.Drawing.Icon;
using Bitmap = gtk::System.Drawing.Bitmap;

namespace System.Drawing;

/// <summary>Behavioural tests for <see cref="Icon"/> loading and conversion. Uses the test.ico that the
/// test project copies next to its binaries.</summary>
[Collection("NonParallelCollection")]
public class IconFunctionalTests
{
    private const string IconFile = "test.ico";

    [Fact]
    public void LoadFromFile_HasPositiveDimensions()
    {
        using var icon = new Icon(IconFile);

        Assert.True(icon.Width > 0, $"icon width should be positive but was {icon.Width}");
        Assert.True(icon.Height > 0, $"icon height should be positive but was {icon.Height}");
        Assert.Equal(new Size(icon.Width, icon.Height), icon.Size);
    }

    [Fact]
    public void ToBitmap_ProducesABitmapOfTheSameSize()
    {
        using var icon = new Icon(IconFile);
        using var bmp = icon.ToBitmap();

        Assert.NotNull(bmp);
        Assert.Equal(icon.Width, bmp.Width);
        Assert.Equal(icon.Height, bmp.Height);
    }

    [Fact]
    public void LoadFromStream_MatchesLoadFromFile()
    {
        using var fromFile = new Icon(IconFile);
        using var stream = File.OpenRead(IconFile);
        using var fromStream = new Icon(stream);

        Assert.Equal(fromFile.Width, fromStream.Width);
        Assert.Equal(fromFile.Height, fromStream.Height);
    }

    [Fact]
    public void Save_WritesBytesThatCanBeReadBack()
    {
        using var icon = new Icon(IconFile);
        using var ms = new MemoryStream();

        icon.Save(ms);

        Assert.True(ms.Length > 0, "saving an icon should produce bytes");
        ms.Position = 0;
        using var reloaded = new Icon(ms);
        Assert.True(reloaded.Width > 0);
    }

    [Fact]
    public void Clone_ProducesAnIconOfTheSameSize()
    {
        using var icon = new Icon(IconFile);
        using var clone = (Icon)icon.Clone();

        Assert.Equal(icon.Width, clone.Width);
        Assert.Equal(icon.Height, clone.Height);
    }
}

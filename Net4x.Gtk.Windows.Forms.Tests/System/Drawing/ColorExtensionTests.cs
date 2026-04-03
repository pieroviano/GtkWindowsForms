#pragma warning disable xUnit2002
namespace System.Drawing;

extern alias gtk;

public class ColorExtensionTests
{
    // ----------------------------
    // Internal method (IMPORTANT)
    // ----------------------------

    [Fact]
    public void GetNamedColor_KnownColor_ReturnsColor()
    {
        var result = gtk::System.Drawing.ColorExtension.FromName("Red");

        Assert.Equal(Color.Red.ToArgb(), result.ToArgb());
    }

    [Fact]
    public void GetNamedColor_SystemColor_ReturnsColor()
    {
        var result = gtk::System.Drawing.ColorExtension.FromName("Window");

        Assert.NotNull(result);
    }

    [Fact]
    public void GetNamedColor_Unknown_ReturnsTransparent()
    {
        var result = gtk::System.Drawing.ColorExtension.FromName("NotAColor");

        Assert.Equal(result.ToArgb(), Color.Transparent.ToArgb());
    }
}
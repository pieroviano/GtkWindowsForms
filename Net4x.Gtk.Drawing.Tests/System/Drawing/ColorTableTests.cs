extern alias gtk;
using ColorTable = gtk::System.Drawing.ColorTable;

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class ColorTableTests
{
    [Fact]
    public void Colors_ShouldBeCaseInsensitive()
    {
        var colors = ColorTable.Colors;

        Assert.True(colors.ContainsKey("red"));
        Assert.True(colors.ContainsKey("RED"));
        Assert.Equal(colors["red"], colors["RED"]);
    }
    // ----------------------------
    // Colors property (Lazy init)
    // ----------------------------

    [Fact]
    public void Colors_ShouldContainKnownColor()
    {
        var colors = ColorTable.Colors;

        Assert.NotNull(colors);
        Assert.Contains("Red", colors.Keys);
        Assert.Equal(Color.Red, colors["Red"]);
    }

    [Fact]
    public void Colors_ShouldContainSystemColor()
    {
        var colors = ColorTable.Colors;

        Assert.Contains("Window", colors.Keys);
        Assert.Equal(SystemColors.Window, colors["Window"]);
    }

    [Fact]
    public void Colors_ShouldNotBeEmpty()
    {
        var colors = ColorTable.Colors;

        Assert.NotEmpty(colors);
    }

    // ----------------------------
    // Lazy behavior (important)
    // ----------------------------

    [Fact]
    public void Colors_ShouldReturnSameInstance()
    {
        var first = ColorTable.Colors;
        var second = ColorTable.Colors;

        Assert.Same(first, second);
    }

    [Fact]
    public void IsKnownNamedColor_CaseInsensitive_ReturnsTrue()
    {
        var result = ColorTable.IsKnownNamedColor("rEd");

        Assert.True(result);
    }

    // ----------------------------
    // IsKnownNamedColor
    // ----------------------------

    [Fact]
    public void IsKnownNamedColor_ExistingColor_ReturnsTrue()
    {
        var result = ColorTable.IsKnownNamedColor("Red");

        Assert.True(result);
    }

    [Fact]
    public void IsKnownNamedColor_NonExisting_ReturnsFalse()
    {
        var result = ColorTable.IsKnownNamedColor("NotAColor");

        Assert.False(result);
    }

    [Fact]
    public void TryGetNamedColor_CaseInsensitive_ReturnsTrue()
    {
        var result = ColorTable.TryGetNamedColor("gReEn", out var color);

        Assert.True(result);
        Assert.Equal(Color.Green, color);
    }

    // ----------------------------
    // TryGetNamedColor
    // ----------------------------

    [Fact]
    public void TryGetNamedColor_ExistingColor_ReturnsTrueAndValue()
    {
        var result = ColorTable.TryGetNamedColor("Blue", out var color);

        Assert.True(result);
        Assert.Equal(Color.Blue, color);
    }

    [Fact]
    public void TryGetNamedColor_NonExisting_ReturnsFalse()
    {
        var result = ColorTable.TryGetNamedColor("NotAColor", out var color);

        Assert.False(result);
        Assert.Equal(default, color);
    }

    [Fact]
    public void TryGetNamedColor_SystemColor_ReturnsTrueAndValue()
    {
        var result = ColorTable.TryGetNamedColor("Window", out var color);

        Assert.True(result);
        Assert.Equal(SystemColors.Window, color);
    }
}
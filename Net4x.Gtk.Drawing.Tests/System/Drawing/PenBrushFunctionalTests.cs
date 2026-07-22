extern alias gtk;
using System.Reflection;
using Pen = gtk::System.Drawing.Pen;
using Pens = gtk::System.Drawing.Pens;
using Brushes = gtk::System.Drawing.Brushes;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using DashStyle = gtk::System.Drawing.Drawing2D.DashStyle;
using LineCap = gtk::System.Drawing.Drawing2D.LineCap;
using HatchBrush = gtk::System.Drawing.Drawing2D.HatchBrush;
using HatchStyle = gtk::System.Drawing.Drawing2D.HatchStyle;
using LinearGradientBrush = gtk::System.Drawing.Drawing2D.LinearGradientBrush;

namespace System.Drawing;

/// <summary>Behavioural tests for pens and brushes: constructor values round-trip and the named palettes
/// really expose a pen/brush of the colour their property name says.</summary>
[Collection("NonParallelCollection")]
public class PenBrushFunctionalTests
{
    [Fact]
    public void Pen_Constructor_SetsColourAndWidth()
    {
        using var pen = new Pen(Color.Crimson, 4.5f);

        Assert.Equal(Color.Crimson.ToArgb(), pen.Color.ToArgb());
        Assert.Equal(4.5f, pen.Width, 3);
    }

    [Fact]
    public void Pen_MutableProperties_RoundTrip()
    {
        using var pen = new Pen(Color.Black);

        pen.Width = 7f;
        pen.DashStyle = DashStyle.Dash;
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Square;

        Assert.Equal(7f, pen.Width, 3);
        Assert.Equal(DashStyle.Dash, pen.DashStyle);
        Assert.Equal(LineCap.Round, pen.StartCap);
        Assert.Equal(LineCap.Square, pen.EndCap);
    }

    [Fact]
    public void SolidBrush_ExposesItsColour()
    {
        using var brush = new SolidBrush(Color.SteelBlue);
        Assert.Equal(Color.SteelBlue.ToArgb(), brush.Color.ToArgb());

        brush.Color = Color.Olive;
        Assert.Equal(Color.Olive.ToArgb(), brush.Color.ToArgb());
    }

    [Fact]
    public void HatchBrush_ExposesStyleAndColours()
    {
        using var brush = new HatchBrush(HatchStyle.DiagonalCross, Color.Red, Color.White);

        Assert.Equal(HatchStyle.DiagonalCross, brush.HatchStyle);
        Assert.Equal(Color.Red.ToArgb(), brush.ForegroundColor.ToArgb());
        Assert.Equal(Color.White.ToArgb(), brush.BackgroundColor.ToArgb());
    }

    [Fact]
    public void LinearGradientBrush_KeepsItsRectangleAndEndColours()
    {
        var rect = new Rectangle(0, 0, 40, 20);
        using var brush = new LinearGradientBrush(rect, Color.Red, Color.Blue, 0f);

        Assert.Equal(rect.Width, brush.Rectangle.Width, 3);
        Assert.Equal(rect.Height, brush.Rectangle.Height, 3);
        Assert.Equal(2, brush.LinearColors.Length);
        Assert.Equal(Color.Red.ToArgb(), brush.LinearColors[0].ToArgb());
        Assert.Equal(Color.Blue.ToArgb(), brush.LinearColors[1].ToArgb());
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("Blue")]
    [InlineData("Green")]
    [InlineData("Black")]
    [InlineData("White")]
    [InlineData("Transparent")]
    public void Pens_NamedPen_HasMatchingColour(string name)
    {
        var pen = (Pen?)typeof(Pens).GetProperty(name, BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

        Assert.NotNull(pen);
        var expected = Color.FromName(name);
        Assert.Equal(expected.ToArgb(), pen!.Color.ToArgb());
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("Blue")]
    [InlineData("Green")]
    [InlineData("Black")]
    [InlineData("White")]
    public void Brushes_NamedBrush_HasMatchingColour(string name)
    {
        var brush = typeof(Brushes).GetProperty(name, BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

        var solid = Assert.IsType<SolidBrush>(brush);
        Assert.Equal(Color.FromName(name).ToArgb(), solid.Color.ToArgb());
    }
}

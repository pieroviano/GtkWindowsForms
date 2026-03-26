extern alias gtk;

using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

public class GraphicsTests
{
    [Fact]
    public void FromImage_Should_Throw_When_Width_Invalid()
    {
        var img = new Bitmap(0, 10);

        Assert.Throws<ArgumentOutOfRangeException>(() => Graphics.FromImage(img));
    }

    [Fact]
    public void FromImage_Should_Throw_When_Height_Invalid()
    {
        var img = new Bitmap(10, 0);

        Assert.Throws<ArgumentOutOfRangeException>(() => Graphics.FromImage(img));
    }

    [Fact]
    public void FromImage_Should_Create_Graphics_When_Valid()
    {
        var img = new Bitmap(10, 10);

        var g = Graphics.FromImage(img);

        Assert.NotNull(g);
        Assert.NotNull(Graphics.Context);
    }

    [Fact]
    public void ClipBounds_Should_Return_Rectangle_From_Internal_State()
    {
        var rect = new Gdk.Rectangle(10, 20, 100, 200);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 100, 100);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var bounds = g.ClipBounds;

        Assert.Equal(10, bounds.X);
        Assert.Equal(20, bounds.Y);
        Assert.Equal(100, bounds.Width);
        Assert.Equal(200, bounds.Height);
    }

    [Fact]
    public void VisibleClipBounds_Should_Match_ClipBounds()
    {
        var rect = new Gdk.Rectangle(0, 0, 50, 60);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 50, 50);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var bounds = g.VisibleClipBounds;

        Assert.Equal(50, bounds.Width);
        Assert.Equal(60, bounds.Height);
    }

    [Fact]
    public void IsClipEmpty_Should_Always_Return_False()
    {
        var rect = new Gdk.Rectangle(0, 0, 10, 10);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 10, 10);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        Assert.False(g.IsClipEmpty);
    }

    [Fact]
    public void Clear_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 10, 10);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 10, 10);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var ex = Record.Exception(() => g.Clear(Color.Red));

        Assert.Null(ex);
    }

    [Fact]
    public void ResetClip_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 10, 10);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 10, 10);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var ex = Record.Exception(() => g.ResetClip());

        Assert.Null(ex);
    }

    [Fact]
    public void Flush_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 10, 10);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 10, 10);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var ex = Record.Exception(() => g.Flush());

        Assert.Null(ex);
    }

    [Fact]
    public void Dispose_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 10, 10);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 10, 10);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);

        var ex = Record.Exception(() => g.Dispose());

        Assert.Null(ex);
    }

    [Fact]
    public void DrawLine_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 100, 100);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 100, 100);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);
        var pen = new Pen(Color.Black);

        var ex = Record.Exception(() => g.DrawLine(pen, 0, 0, 10, 10));

        Assert.Null(ex);
    }

    [Fact]
    public void DrawRectangle_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 100, 100);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 100, 100);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);
        var pen = new Pen(Color.Black);

        var ex = Record.Exception(() => g.DrawRectangle(pen, 10, 10, 20, 20));

        Assert.Null(ex);
    }

    [Fact]
    public void FillRectangle_Should_Not_Throw()
    {
        var rect = new Gdk.Rectangle(0, 0, 100, 100);
        using var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, 100, 100);
        using var context = new Cairo.Context(surface);

        var g = new Graphics(context, rect);
        var brush = new SolidBrush(Color.Blue);

        var ex = Record.Exception(() => g.FillRectangle(brush, 10, 10, 20, 20));

        Assert.Null(ex);
    }
}
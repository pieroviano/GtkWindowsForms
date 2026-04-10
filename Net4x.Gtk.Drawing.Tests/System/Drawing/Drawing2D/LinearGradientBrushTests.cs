extern alias gtk;

using Blend = gtk::System.Drawing.Drawing2D.Blend;
using ColorBlend = gtk::System.Drawing.Drawing2D.ColorBlend;
using LinearGradientBrush = gtk::System.Drawing.Drawing2D.LinearGradientBrush;
using LinearGradientMode = gtk::System.Drawing.Drawing2D.LinearGradientMode;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using MatrixOrder = gtk::System.Drawing.Drawing2D.MatrixOrder;
using WrapMode = gtk::System.Drawing.Drawing2D.WrapMode;

namespace System.Drawing.Drawing2D;

[Collection("NonParallelCollection")]
public class LinearGradientBrushTests
{
    [Fact]
    public void Ctor_Point_SetsRectangleAndColors()
    {
        var p1 = new Point(0, 0);
        var p2 = new Point(10, 20);

        var brush = new LinearGradientBrush(p1, p2, Color.Red, Color.Blue);

        Assert.Equal(new RectangleF(0, 0, 10, 20), brush.Rectangle);
        Assert.Equal(new[] { Color.Red, Color.Blue }, brush.LinearColors);
    }

    [Fact]
    public void Ctor_PointF_SetsRectangleAndColors()
    {
        var p1 = new PointF(5, 5);
        var p2 = new PointF(15, 25);

        var brush = new LinearGradientBrush(p1, p2, Color.Green, Color.Yellow);

        Assert.Equal(new RectangleF(5, 5, 10, 20), brush.Rectangle);
        Assert.Equal(new[] { Color.Green, Color.Yellow }, brush.LinearColors);
    }

    [Fact]
    public void Ctor_Rectangle_WithMode_SetsProperties()
    {
        var rect = new Rectangle(1, 2, 3, 4);

        var brush = new LinearGradientBrush(rect, Color.Black, Color.White, LinearGradientMode.Horizontal);

        Assert.Equal(new RectangleF(1, 2, 3, 4), brush.Rectangle);
        Assert.Equal(new[] { Color.Black, Color.White }, brush.LinearColors);
        Assert.Equal(LinearGradientMode.Horizontal, brush.LinearGradientMode);
    }

    [Fact]
    public void Ctor_Rectangle_WithAngle_SetsAngleAndWrapMode()
    {
        var rect = new Rectangle(0, 0, 10, 10);

        var brush = new LinearGradientBrush(rect, Color.Red, Color.Blue, 45f);

        Assert.Equal(45f, brush.Angle);
        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void Ctor_RectangleF_WithAngle_SetsAngleAndWrapMode()
    {
        var rect = new RectangleF(0, 0, 10, 10);

        var brush = new LinearGradientBrush(rect, Color.Red, Color.Blue, 90f);

        Assert.Equal(90f, brush.Angle);
        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void Default_WrapMode_IsTile()
    {
        var brush = new LinearGradientBrush(new Point(0,0), new Point(1,1), Color.Black, Color.White);

        Assert.Equal(WrapMode.Tile, brush.WrapMode);
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        var brush = new LinearGradientBrush(new Point(0,0), new Point(1,1), Color.Black, Color.White)
        {
            Angle = 30f,
            GammaCorrection = true,
            Blend = new Blend(),
            InterpolationColors = new ColorBlend(),
            Transform = new Matrix(),
            WrapMode = WrapMode.Clamp
        };

        Assert.Equal(30f, brush.Angle);
        Assert.True(brush.GammaCorrection);
        Assert.NotNull(brush.Blend);
        Assert.NotNull(brush.InterpolationColors);
        Assert.NotNull(brush.Transform);
        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void Rectangle_IsCalculatedWithAbsoluteValues()
    {
        var p1 = new Point(10, 20);
        var p2 = new Point(0, 0);

        var brush = new LinearGradientBrush(p1, p2, Color.Red, Color.Blue);

        Assert.Equal(new RectangleF(10, 20, 10, 20), brush.Rectangle);
    }

    [Fact]
    public void Clone_ReturnsNotNull()
    {
        var brush = new LinearGradientBrush(new Point(0,0), new Point(1,1), Color.Black, Color.White);

        var clone = brush.Clone();

        Assert.NotNull(clone);
    }

    // --- No-op method tests (ensure no exceptions) ---

    [Fact]
    public void TransformMethods_DoNotThrow()
    {
        var brush = new LinearGradientBrush(new Point(0,0), new Point(1,1), Color.Black, Color.White);
        var matrix = new Matrix();

        var ex = Record.Exception(() =>
        {
            brush.MultiplyTransform(matrix);
            brush.MultiplyTransform(matrix, MatrixOrder.Append);
            brush.ResetTransform();
            brush.RotateTransform(45);
            brush.RotateTransform(45, MatrixOrder.Prepend);
            brush.ScaleTransform(2, 2);
            brush.ScaleTransform(2, 2, MatrixOrder.Append);
            brush.TranslateTransform(5, 5);
            brush.TranslateTransform(5, 5, MatrixOrder.Prepend);
        });

        Assert.Null(ex);
    }

    [Fact]
    public void BlendShapeMethods_DoNotThrow()
    {
        var brush = new LinearGradientBrush(new Point(0,0), new Point(1,1), Color.Black, Color.White);

        var ex = Record.Exception(() =>
        {
            brush.SetBlendTriangularShape(0.5f);
            brush.SetBlendTriangularShape(0.5f, 0.8f);
            brush.SetSigmaBellShape(0.5f);
            brush.SetSigmaBellShape(0.5f, 0.8f);
        });

        Assert.Null(ex);
    }
}
extern alias gtk;

using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using MatrixOrder = gtk::System.Drawing.Drawing2D.MatrixOrder;
using PathGradientBrush = gtk::System.Drawing.Drawing2D.PathGradientBrush;
using WrapMode = gtk::System.Drawing.Drawing2D.WrapMode;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Drawing.Drawing2D;

public class PathGradientBrushTests
{
    [Fact]
    public void Ctor_WithPoints_SetsRectangleAndCenter()
    {
        var pts = new[]
        {
            new PointF(0, 0),
            new PointF(10, 0),
            new PointF(10, 10),
            new PointF(0, 10)
        };

        var brush = new PathGradientBrush(pts);

        Assert.Equal(new RectangleF(0, 0, 10, 10), brush.Rectangle);
        Assert.Equal(new PointF(5, 5), brush.CenterPoint);
        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void Ctor_WithPointsAndWrapMode_SetsWrapMode()
    {
        var pts = new[]
        {
            new PointF(0, 0),
            new PointF(2, 2)
        };

        var brush = new PathGradientBrush(pts, WrapMode.Tile);

        Assert.Equal(WrapMode.Tile, brush.WrapMode);
    }

    [Fact]
    public void Ctor_WithIntPoints_ConvertsCorrectly()
    {
        var pts = new[]
        {
            new Point(0, 0),
            new Point(4, 4)
        };

        var brush = new PathGradientBrush(pts);

        Assert.Equal(new RectangleF(0, 0, 4, 4), brush.Rectangle);
        Assert.Equal(new PointF(2, 2), brush.CenterPoint);
    }

    [Fact]
    public void Transform_SetNull_ResetsToNewMatrix()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.Transform = null;

        Assert.NotNull(brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_UpdatesMatrix()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        var matrix = new Matrix();
        brush.MultiplyTransform(matrix);

        Assert.NotNull(brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_WithOrder_UpdatesOrder()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        var matrix = new Matrix();
        brush.MultiplyTransform(matrix, MatrixOrder.Append);

        Assert.Equal(MatrixOrder.Append, brush.Transform.order);
    }

    [Fact]
    public void ResetTransform_ResetsMatrix()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.RotateTransform(45);
        brush.ResetTransform();

        Assert.NotNull(brush.Transform);
        Assert.Equal(0, brush.Transform.angle);
    }

    [Fact]
    public void RotateTransform_SetsAngle()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.RotateTransform(30);

        Assert.Equal(30, brush.Transform.angle);
    }

    [Fact]
    public void ScaleTransform_SetsScale()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.ScaleTransform(2, 3);

        Assert.Equal(2, brush.Transform.scaleX);
        Assert.Equal(3, brush.Transform.scaleY);
    }

    [Fact]
    public void TranslateTransform_SetsOffset()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.TranslateTransform(5, 6);

        Assert.Equal(5, brush.Transform.OffsetX);
        Assert.Equal(6, brush.Transform.OffsetY);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0),
            new PointF(10,10)
        })
        {
            CenterColor = Color.Red,
            SurroundColors = new[] { Color.Blue }
        };

        var clone = (PathGradientBrush)brush.Clone();

        Assert.NotSame(brush, clone);
        Assert.Equal(brush.CenterColor, clone.CenterColor);
        Assert.Equal(brush.Rectangle, clone.Rectangle);
        Assert.NotSame(brush.SurroundColors, clone.SurroundColors);
    }

    [Fact]
    public void SetBlendTriangularShape_SetsBlend()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.SetBlendTriangularShape(0.5f);

        Assert.NotNull(brush.Blend);
        Assert.Equal(new[] { 0f, 1f, 0f }, brush.Blend.Factors);
        Assert.Equal(new[] { 0f, 0.5f, 1f }, brush.Blend.Positions);
    }

    [Fact]
    public void SetBlendTriangularShape_WithScale_SetsBlend()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.SetBlendTriangularShape(0.5f, 0.8f);

        Assert.Equal(new[] { 0f, 0.8f, 0f }, brush.Blend.Factors);
    }

    [Fact]
    public void SetSigmaBellShape_SetsBlend()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.SetSigmaBellShape(0.5f);

        Assert.NotNull(brush.Blend);
        Assert.Equal(4, brush.Blend.Factors.Length);
    }

    [Fact]
    public void SetSigmaBellShape_WithScale_SetsBlend()
    {
        var brush = new PathGradientBrush(new[]
        {
            new PointF(0,0), new PointF(1,1)
        });

        brush.SetSigmaBellShape(0.5f, 0.7f);

        Assert.Equal(new[] { 0f, 0.7f, 0.7f, 0f }, brush.Blend.Factors);
    }
}
extern alias gtk;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using MatrixOrder = gtk::System.Drawing.Drawing2D.MatrixOrder;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Drawing.Drawing2D;

[Collection("NonParallelCollection")]
public class MatrixTests
{
    [Fact]
    public void DefaultCtor_HasIdentityValues()
    {
        var m = new Matrix();

        Assert.Equal(1, m.m11);
        Assert.Equal(0, m.m12);
        Assert.Equal(0, m.m21);
        Assert.Equal(1, m.m22);
        Assert.Equal(0, m.dx);
        Assert.Equal(0, m.dy);
        Assert.Equal(MatrixOrder.Prepend, m.order);
    }

    [Fact]
    public void Ctor_WithElements_SetsValues()
    {
        var m = new Matrix(1, 2, 3, 4, 5, 6);

        Assert.Equal(1, m.m11);
        Assert.Equal(2, m.m12);
        Assert.Equal(3, m.m21);
        Assert.Equal(4, m.m22);
        Assert.Equal(5, m.dx);
        Assert.Equal(6, m.dy);
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        var m = new Matrix();

        m.angle = 45;
        m.scaleX = 2;
        m.scaleY = 3;
        m.shearX = 1;
        m.shearY = 2;
        m.dx = 10;
        m.dy = 20;

        Assert.Equal(45, m.angle);
        Assert.Equal(2, m.scaleX);
        Assert.Equal(3, m.scaleY);
        Assert.Equal(1, m.shearX);
        Assert.Equal(2, m.shearY);
        Assert.Equal(10, m.dx);
        Assert.Equal(20, m.dy);
    }

    [Fact]
    public void Multiply_SetsInitMatrix()
    {
        var m1 = new Matrix();
        var m2 = new Matrix();

        m1.Multiply(m2);

        Assert.Equal(m2, m1.initMatrix);
    }

    [Fact]
    public void Multiply_WithOrder_SetsMultiplyAndOrder()
    {
        var m1 = new Matrix();
        var m2 = new Matrix();

        m1.Multiply(m2, MatrixOrder.Append);

        Assert.Equal(m2, m1.multiply);
        Assert.Equal(MatrixOrder.Append, m1.order);
    }

    [Fact]
    public void Rotate_SetsAngle()
    {
        var m = new Matrix();

        m.Rotate(90);

        Assert.Equal(90, m.angle);
    }

    [Fact]
    public void Rotate_WithOrder_SetsAngleAndOrder()
    {
        var m = new Matrix();

        m.Rotate(45, MatrixOrder.Append);

        Assert.Equal(45, m.angle);
        Assert.Equal(MatrixOrder.Append, m.order);
    }

    [Fact]
    public void RotateAt_SetsPointAndAngle()
    {
        var m = new Matrix();
        var point = new PointF(1, 2);

        m.RotateAt(30, point);

        Assert.Equal(point, m.rotateAtPoint);
        Assert.Equal(30, m.angle);
    }

    [Fact]
    public void Scale_SetsScale()
    {
        var m = new Matrix();

        m.Scale(2, 3);

        Assert.Equal(2, m.scaleX);
        Assert.Equal(3, m.scaleY);
    }

    [Fact]
    public void Scale_WithOrder_SetsScaleAndOrder()
    {
        var m = new Matrix();

        m.Scale(2, 3, MatrixOrder.Append);

        Assert.Equal(2, m.scaleX);
        Assert.Equal(3, m.scaleY);
        Assert.Equal(MatrixOrder.Append, m.order);
    }

    [Fact]
    public void Shear_SetsValues()
    {
        var m = new Matrix();

        m.Shear(1, 2);

        Assert.Equal(1, m.shearX);
        Assert.Equal(2, m.shearY);
    }

    [Fact]
    public void Translate_SetsOffsets()
    {
        var m = new Matrix();

        m.Translate(5, 6);

        Assert.Equal(5, m.OffsetX);
        Assert.Equal(6, m.OffsetY);
    }

    [Fact]
    public void Translate_WithOrder_SetsOffsetsAndOrder()
    {
        var m = new Matrix();

        m.Translate(5, 6, MatrixOrder.Append);

        Assert.Equal(5, m.OffsetX);
        Assert.Equal(6, m.OffsetY);
        Assert.Equal(MatrixOrder.Append, m.order);
    }

    [Fact]
    public void TransformPoints_PointF_SetsProperty()
    {
        var m = new Matrix();
        var pts = new[] { new PointF(1, 2) };

        m.TransformPoints(pts);

        Assert.Equal(pts, m.transformPoints);
    }

    [Fact]
    public void TransformPoints_Point_ConvertsToPointF()
    {
        var m = new Matrix();
        var pts = new[] { new Point(1, 2) };

        m.TransformPoints(pts);

        Assert.Equal(1, m.transformPoints[0].X);
        Assert.Equal(2, m.transformPoints[0].Y);
    }

    [Fact]
    public void TransformVectors_PointF_SetsProperty()
    {
        var m = new Matrix();
        var pts = new[] { new PointF(3, 4) };

        m.TransformVectors(pts);

        Assert.Equal(pts, m.transformVectors);
    }

    [Fact]
    public void TransformVectors_Point_ConvertsToPointF()
    {
        var m = new Matrix();
        var pts = new[] { new Point(3, 4) };

        m.TransformVectors(pts);

        Assert.Equal(3, m.transformVectors[0].X);
        Assert.Equal(4, m.transformVectors[0].Y);
    }

    [Fact]
    public void VectorTransformPoints_SetsProperty()
    {
        var m = new Matrix();
        var pts = new[] { new Point(1, 1) };

        m.VectorTransformPoints(pts);

        Assert.Equal(pts, m.vectorTransformPoints);
    }

    [Fact]
    public void Invert_SetsFlag()
    {
        var m = new Matrix();

        m.Invert();

        Assert.True(m.invert);
    }

    [Fact]
    public void Reset_ResetsState()
    {
        var m = new Matrix(2,2,2,2,2,2);

        m.Scale(5, 5);
        m.Rotate(45);

        m.Reset();

        Assert.Equal(1, m.m11);
        Assert.Equal(0, m.m12);
        Assert.Equal(0, m.m21);
        Assert.Equal(1, m.m22);
        Assert.Equal(0, m.dx);
        Assert.Equal(0, m.dy);
        Assert.Equal(0, m.angle);
        Assert.Null(m.multiply);
    }

    [Fact]
    public void Dispose_ResetsState()
    {
        var m = new Matrix();

        m.Scale(5, 5);
        m.Rotate(45);

        m.Dispose();

        Assert.Equal(1, m.m11);
        Assert.Equal(0, m.m12);
        Assert.Equal(0, m.m21);
        Assert.Equal(1, m.m22);
        Assert.Equal(0, m.dx);
        Assert.Equal(0, m.dy);
        Assert.Equal(0, m.angle);
        Assert.Null(m.multiply);
    }

    [Fact]
    public void Clone_ReturnsNull()
    {
        var m = new gtk::System.Drawing.Drawing2D.Matrix();

        var clone = m.Clone();

        Assert.Null(clone);
    }

    [Fact]
    public void Elements_ThrowsNotImplemented()
    {
        var m = new Matrix();

        Assert.Throws<NotImplementedException>(() => _ = m.Elements);
    }

    [Fact]
    public void IsIdentity_ThrowsNotImplemented()
    {
        var m = new Matrix();

        Assert.Throws<NotImplementedException>(() => _ = m.IsIdentity);
    }

    [Fact]
    public void IsInvertible_ThrowsNotImplemented()
    {
        var m = new Matrix();

        Assert.Throws<NotImplementedException>(() => _ = m.IsInvertible);
    }
}
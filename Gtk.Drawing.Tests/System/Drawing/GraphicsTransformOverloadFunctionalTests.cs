extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using MatrixOrder = gtk::System.Drawing.Drawing2D.MatrixOrder;
using CoordinateSpace = gtk::System.Drawing.Drawing2D.CoordinateSpace;
using GraphicsUnit = gtk::System.Drawing.GraphicsUnit;

namespace System.Drawing;

/// <summary>The world transform, its <see cref="MatrixOrder"/> overloads, containers and clip translation
/// must all actually move what gets painted.</summary>
[Collection("NonParallelCollection")]
public class GraphicsTransformOverloadFunctionalTests
{
    private const int Size = 64;

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    /// <summary>Paints a small marker at world origin and reports where it landed on the surface.</summary>
    private static Point MarkerCentre(Action<Graphics> setup)
    {
        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            setup(g);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 8, 8);
        }

        long sx = 0, sy = 0, n = 0;
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                if (IsWhitish(bmp.GetPixel(x, y))) continue;
                sx += x; sy += y; n++;
            }
        }

        Assert.True(n > 0, "nothing was painted at all");
        return new Point((int)(sx / n), (int)(sy / n));
    }

    [Fact]
    public void TranslateTransform_MovesSubsequentDrawing()
    {
        var untranslated = MarkerCentre(_ => { });
        var translated = MarkerCentre(g => g.TranslateTransform(30f, 20f));

        Assert.InRange(translated.X - untranslated.X, 28, 32);
        Assert.InRange(translated.Y - untranslated.Y, 18, 22);
    }

    [Fact]
    public void TranslateTransform_WithExplicitOrder_MatchesTheOrderLessOverload()
    {
        var plain = MarkerCentre(g => g.TranslateTransform(20f, 10f));
        var appended = MarkerCentre(g => g.TranslateTransform(20f, 10f, MatrixOrder.Prepend));

        Assert.Equal(plain, appended);
    }

    [Fact]
    public void ScaleTransform_EnlargesSubsequentDrawing()
    {
        var plain = MarkerCentre(_ => { });
        var scaled = MarkerCentre(g => g.ScaleTransform(3f, 3f));

        Assert.True(scaled.X > plain.X + 4,
            $"scaling by 3 should push the marker centre outwards, but it went from {plain} to {scaled}");
    }

    [Fact]
    public void ScaleTransform_WithExplicitOrder_MatchesTheOrderLessOverload()
    {
        var plain = MarkerCentre(g => g.ScaleTransform(2f, 2f));
        var ordered = MarkerCentre(g => g.ScaleTransform(2f, 2f, MatrixOrder.Prepend));

        Assert.Equal(plain, ordered);
    }

    [Fact]
    public void RotateTransform_MovesDrawingOffTheOrigin()
    {
        var plain = MarkerCentre(g => g.TranslateTransform(30f, 8f));
        var rotated = MarkerCentre(g =>
        {
            g.TranslateTransform(30f, 8f);
            g.RotateTransform(45f);
        });

        Assert.NotEqual(plain, rotated);
    }

    [Fact]
    public void RotateTransform_WithExplicitOrder_MatchesTheOrderLessOverload()
    {
        var plain = MarkerCentre(g => { g.TranslateTransform(30f, 8f); g.RotateTransform(30f); });
        var ordered = MarkerCentre(g => { g.TranslateTransform(30f, 8f); g.RotateTransform(30f, MatrixOrder.Prepend); });

        Assert.Equal(plain, ordered);
    }

    [Fact]
    public void ResetTransform_UndoesEverythingBefore()
    {
        var plain = MarkerCentre(_ => { });
        var reset = MarkerCentre(g =>
        {
            g.TranslateTransform(25f, 25f);
            g.ScaleTransform(2f, 2f);
            g.ResetTransform();
        });

        Assert.Equal(plain, reset);
    }

    [Fact]
    public void Transform_Property_RoundTripsAndAppliesTheTranslation()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);
        using var m = new Matrix();
        m.Translate(12f, 34f);

        g.Transform = m;

        var read = g.Transform;
        Assert.Equal(12f, read.OffsetX, 1);
        Assert.Equal(34f, read.OffsetY, 1);
    }

    [Fact]
    public void MultiplyTransform_ByATranslation_MovesSubsequentDrawing()
    {
        var plain = MarkerCentre(_ => { });
        var multiplied = MarkerCentre(g =>
        {
            var m = new Matrix();
            m.Translate(20f, 10f);
            g.MultiplyTransform(m, MatrixOrder.Prepend);
        });

        Assert.InRange(multiplied.X - plain.X, 18, 22);
        Assert.InRange(multiplied.Y - plain.Y, 8, 12);
    }

    [Fact]
    public void BeginContainer_And_EndContainer_RestoreTheTransform()
    {
        var plain = MarkerCentre(_ => { });
        var restored = MarkerCentre(g =>
        {
            var state = g.BeginContainer();
            g.TranslateTransform(30f, 30f);
            g.EndContainer(state);
        });

        Assert.Equal(plain, restored);
    }

    [Fact]
    public void BeginContainer_WithRectangles_RestoresTheTransform()
    {
        var plain = MarkerCentre(_ => { });
        var restoredF = MarkerCentre(g =>
        {
            var state = g.BeginContainer(new RectangleF(0, 0, 10, 10), new RectangleF(0, 0, 20, 20), GraphicsUnit.Pixel);
            g.TranslateTransform(30f, 30f);
            g.EndContainer(state);
        });
        var restoredI = MarkerCentre(g =>
        {
            var state = g.BeginContainer(new Rectangle(0, 0, 10, 10), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
            g.TranslateTransform(30f, 30f);
            g.EndContainer(state);
        });

        Assert.Equal(plain, restoredF);
        Assert.Equal(plain, restoredI);
    }

    [Fact]
    public void TranslateClip_MovesTheClippedArea()
    {
        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.SetClip(new Rectangle(0, 0, 16, 16));
            g.TranslateClip(32, 32);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        }

        Assert.True(IsWhitish(bmp.GetPixel(8, 8)), "the original clip position should no longer be painted");
        Assert.False(IsWhitish(bmp.GetPixel(40, 40)), "the translated clip position should be painted");
    }

    [Fact]
    public void TranslateClip_FloatOverload_MatchesTheIntegerOverload()
    {
        static Color Sample(Action<Graphics> translate)
        {
            using var bmp = new Bitmap(Size, Size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SetClip(new Rectangle(0, 0, 16, 16));
                translate(g);
                g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
            }

            return bmp.GetPixel(40, 40);
        }

        Assert.Equal(Sample(g => g.TranslateClip(32, 32)), Sample(g => g.TranslateClip(32f, 32f)));
    }

    [Fact]
    public void SetClip_FromAnotherGraphics_CopiesTheClipBounds()
    {
        using var srcBmp = new Bitmap(Size, Size);
        using var src = Graphics.FromImage(srcBmp);
        src.SetClip(new Rectangle(0, 0, 16, 16));

        using var bmp = new Bitmap(Size, Size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.SetClip(src);
            g.FillRectangle(new SolidBrush(Color.Red), 0, 0, Size, Size);
        }

        Assert.False(IsWhitish(bmp.GetPixel(8, 8)), "inside the copied clip should be painted");
        Assert.True(IsWhitish(bmp.GetPixel(40, 40)), "outside the copied clip should stay clear");
    }

    [Fact]
    public void TransformPoints_WorldToDevice_AppliesTheCurrentTransform()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);
        g.TranslateTransform(10f, 20f);

        var pts = new[] { new PointF(1f, 2f) };
        g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pts);

        Assert.Equal(11f, pts[0].X, 1);
        Assert.Equal(22f, pts[0].Y, 1);
    }

    [Fact]
    public void TransformPoints_DeviceToWorld_IsTheInverse()
    {
        using var bmp = new Bitmap(Size, Size);
        using var g = Graphics.FromImage(bmp);
        g.TranslateTransform(10f, 20f);

        var pts = new[] { new PointF(1f, 2f) };
        g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pts);
        g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, pts);

        Assert.Equal(1f, pts[0].X, 1);
        Assert.Equal(2f, pts[0].Y, 1);
    }
}

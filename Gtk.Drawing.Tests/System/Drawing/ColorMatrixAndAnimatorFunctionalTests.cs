extern alias gtk;
using System.Reflection;
using ColorMatrix = gtk::System.Drawing.Imaging.ColorMatrix;
using ImageAnimator = gtk::System.Drawing.ImageAnimator;
using Bitmap = gtk::System.Drawing.Bitmap;

namespace System.Drawing;

/// <summary>
/// Every MatrixRC accessor must agree with the indexer, and the animator must report a still image as
/// non-animatable and tolerate the animate/stop calls for it.
/// </summary>
[Collection("NonParallelCollection")]
public class ColorMatrixAndAnimatorFunctionalTests
{
    public static IEnumerable<object[]> Cells()
    {
        for (var row = 0; row < 5; row++)
        {
            for (var col = 0; col < 5; col++)
            {
                yield return new object[] { row, col };
            }
        }
    }

    [Theory]
    [MemberData(nameof(Cells))]
    public void NamedMatrixAccessor_MatchesTheIndexer(int row, int col)
    {
        var property = typeof(ColorMatrix).GetProperty($"Matrix{row}{col}", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);

        var cm = new ColorMatrix();
        var value = 0.125f * ((row * 5) + col + 1);

        // Written through the indexer, read through the named property.
        cm[row, col] = value;
        Assert.Equal(value, (float)property!.GetValue(cm)!, 3);

        // Written through the named property, read through the indexer.
        var other = value + 1f;
        property.SetValue(cm, other);
        Assert.Equal(other, cm[row, col], 3);
    }

    [Fact]
    public void ImageAnimator_ReportsAStillImageAsNotAnimatable()
    {
        using var still = new Bitmap(4, 4);
        Assert.False(ImageAnimator.CanAnimate(still), "a single-frame bitmap cannot be animated");
    }

    [Fact]
    public void ImageAnimator_AnimateAndStop_AreSafeForAStillImage()
    {
        using var still = new Bitmap(4, 4);
        EventHandler handler = (s, e) => { };

        // Animating a still image is a no-op in GDI+ and must not throw or start anything.
        ImageAnimator.Animate(still, handler);
        ImageAnimator.UpdateFrames(still);
        ImageAnimator.StopAnimate(still, handler);

        Assert.False(ImageAnimator.CanAnimate(still),
            "a still image must remain non-animatable after Animate/Stop");
    }
}

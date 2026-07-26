extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Region = gtk::System.Drawing.Region;
using CombineMode = gtk::System.Drawing.Drawing2D.CombineMode;

namespace System.Drawing;

/// <summary>
/// Behavioural tests for <see cref="Graphics.GetContextInfo" />.
/// <para>
/// It used to <c>return this.context</c> - the live <c>Cairo.Context</c> - through a public method on the
/// very type that owns that handle's lifetime. A caller could draw through it, dispose it, or reconfigure
/// it behind the <c>ownsContext</c>/<c>ownedSurface</c> bookkeeping, and making the field <c>internal</c>
/// bought nothing while this method published it anyway.
/// </para>
/// GDI+ answers this call with the cumulative clip and the cumulative transform offset, so these assert
/// that shape, and that no Cairo type escapes.
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsContextInfoTests
{
    private const int Size = 40;

    private static void WithGraphics(Action<Graphics> body)
    {
        using var bitmap = new Bitmap(Size, Size);
        using var graphics = Graphics.FromImage(bitmap);
        body(graphics);
    }

    [Fact]
    public void GetContextInfo_ReturnsTheClipAndTheTransformOffset()
    {
        WithGraphics(g =>
        {
            var info = Assert.IsType<object[]>(g.GetContextInfo());

            Assert.Equal(2, info.Length);
            Assert.IsType<Region>(info[0]);
            Assert.IsType<PointF>(info[1]);
        });
    }

    /// <summary>The whole point of the change: no substrate handle on the public surface.</summary>
    [Fact]
    public void GetContextInfo_DoesNotHandOutTheCairoContext()
    {
        WithGraphics(g =>
        {
            var info = (object[])g.GetContextInfo();

            foreach (var item in info)
            {
                Assert.NotNull(item);
                Assert.DoesNotContain("Cairo", item.GetType().FullName ?? string.Empty, StringComparison.Ordinal);
            }
        });
    }

    /// <summary>The reported clip has to track the clip actually in force, not the initial surface.</summary>
    [Fact]
    public void GetContextInfo_ReportsTheClipCurrentlyInForce()
    {
        WithGraphics(g =>
        {
            g.SetClip(new RectangleF(5, 6, 12, 13), CombineMode.Replace);

            var clip = (Region)((object[])g.GetContextInfo())[0];
            var bounds = clip.GetBounds(g);

            Assert.Equal(5f, bounds.X, 3);
            Assert.Equal(6f, bounds.Y, 3);
            Assert.Equal(12f, bounds.Width, 3);
            Assert.Equal(13f, bounds.Height, 3);
        });
    }

    /// <summary>An untransformed Graphics reports no offset; a translated one reports the translation.</summary>
    [Fact]
    public void GetContextInfo_ReportsTheCumulativeTransformOffset()
    {
        WithGraphics(g =>
        {
            var before = (PointF)((object[])g.GetContextInfo())[1];
            Assert.Equal(0f, before.X, 3);
            Assert.Equal(0f, before.Y, 3);

            g.TranslateTransform(7f, -4f);

            var after = (PointF)((object[])g.GetContextInfo())[1];
            Assert.Equal(7f, after.X, 3);
            Assert.Equal(-4f, after.Y, 3);
        });
    }
}

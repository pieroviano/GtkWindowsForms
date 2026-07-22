extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using BufferedGraphicsManager = gtk::System.Drawing.BufferedGraphicsManager;
using BufferedGraphicsContext = gtk::System.Drawing.BufferedGraphicsContext;
using BufferedGraphics = gtk::System.Drawing.BufferedGraphics;

namespace System.Drawing;

/// <summary>Covers the buffered-graphics family, which was missing entirely —
/// so the standard flicker-free custom-painting recipe did not compile. The tests exercise that recipe:
/// allocate a buffer against a surface, draw into it, and render it in one go.</summary>
[Collection("NonParallelCollection")]
public class BufferedGraphicsFunctionalTests
{
    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    [Fact]
    public void TheManagerExposesADefaultContext()
    {
        Assert.NotNull(BufferedGraphicsManager.Current);
        Assert.Same(BufferedGraphicsManager.Current, BufferedGraphicsManager.Current);
    }

    [Fact]
    public void AllocateGivesABufferWithItsOwnSurface()
    {
        using var bmp = new Bitmap(40, 30);
        using var target = Graphics.FromImage(bmp);

        using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 40, 30));

        Assert.NotNull(buffer);
        Assert.NotNull(buffer.Graphics);
        Assert.NotSame(target, buffer.Graphics);
    }

    [Fact]
    public void DrawingIntoTheBufferDoesNotTouchTheTargetUntilItIsRendered()
    {
        using var bmp = new Bitmap(40, 30);
        using (var target = Graphics.FromImage(bmp))
        {
            target.Clear(Color.White);
            using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 40, 30));
            buffer.Graphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, 40, 30);

            // Deliberately not rendered.
        }

        Assert.True(IsWhitish(bmp.GetPixel(20, 15)),
            "content drawn into the buffer must not reach the target until Render is called");
    }

    [Fact]
    public void RenderBlitsTheBufferOntoTheTarget()
    {
        using var bmp = new Bitmap(40, 30);
        using (var target = Graphics.FromImage(bmp))
        {
            target.Clear(Color.White);
            using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 40, 30));
            buffer.Graphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, 40, 30);
            buffer.Render();
        }

        var pixel = bmp.GetPixel(20, 15);
        Assert.True(pixel.R > 200 && pixel.G < 100,
            $"Render should have blitted the red buffer onto the target but the pixel was {pixel}");
    }

    [Fact]
    public void RenderHonoursTheOffsetOfTheAllocatedRectangle()
    {
        using var bmp = new Bitmap(60, 60);
        using (var target = Graphics.FromImage(bmp))
        {
            target.Clear(Color.White);
            using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(20, 20, 20, 20));
            buffer.Graphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, 20, 20);
            buffer.Render();
        }

        Assert.False(IsWhitish(bmp.GetPixel(30, 30)), "the buffer should land at its allocated offset");
        Assert.True(IsWhitish(bmp.GetPixel(5, 5)), "nothing should be painted outside the allocated area");
    }

    [Fact]
    public void RenderOntoAnExplicitTargetWorks()
    {
        using var bmp = new Bitmap(40, 30);
        using (var target = Graphics.FromImage(bmp))
        {
            target.Clear(Color.White);
            using var buffer = BufferedGraphicsManager.Current.Allocate(null, new Rectangle(0, 0, 40, 30));
            buffer.Graphics.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 40, 30);
            buffer.Render(target);
        }

        Assert.True(bmp.GetPixel(20, 15).B > 200, "rendering onto an explicit target should paint it");
    }

    [Fact]
    public void RenderWithNoTargetIsAQuietNoOp()
    {
        using var buffer = BufferedGraphicsManager.Current.Allocate(null, new Rectangle(0, 0, 10, 10));
        buffer.Graphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, 10, 10);

        // Nothing to render onto; this must not throw.
        buffer.Render();
        buffer.Render(null);
    }

    [Fact]
    public void TheFlickerFreePaintingRecipeWorksEndToEnd()
    {
        // The pattern this family exists for.
        using var bmp = new Bitmap(40, 40);
        using (var e = Graphics.FromImage(bmp))
        {
            using var buffer = BufferedGraphicsManager.Current.Allocate(e, new Rectangle(0, 0, 40, 40));
            buffer.Graphics.Clear(Color.White);
            buffer.Graphics.FillRectangle(new SolidBrush(Color.Green), 10, 10, 20, 20);
            buffer.Render();
        }

        Assert.False(IsWhitish(bmp.GetPixel(20, 20)), "the drawn square should be there");
        Assert.True(IsWhitish(bmp.GetPixel(2, 2)), "the cleared background should be there");
    }

    [Fact]
    public void UsingABufferAfterDisposalIsRejected()
    {
        using var bmp = new Bitmap(10, 10);
        using var target = Graphics.FromImage(bmp);
        var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 10, 10));

        buffer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => _ = buffer.Graphics);
    }

    [Fact]
    public void DisposingTwiceIsHarmless()
    {
        using var bmp = new Bitmap(10, 10);
        using var target = Graphics.FromImage(bmp);
        var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 10, 10));

        buffer.Dispose();
        buffer.Dispose();
    }

    [Fact]
    public void MaximumBufferRoundTripsAndRejectsNonPositiveSizes()
    {
        using var context = new BufferedGraphicsContext();

        context.MaximumBuffer = new Size(400, 200);

        Assert.Equal(new Size(400, 200), context.MaximumBuffer);
        Assert.Throws<ArgumentException>(() => context.MaximumBuffer = new Size(0, 10));
        Assert.Throws<ArgumentException>(() => context.MaximumBuffer = new Size(10, -1));
    }

    [Fact]
    public void ADeviceContextAllocationReportsThatHandlesAreUnavailable()
    {
        using var context = new BufferedGraphicsContext();

        Assert.Throws<PlatformNotSupportedException>(
            () => context.Allocate(IntPtr.Zero, new Rectangle(0, 0, 10, 10)));
    }

    [Fact]
    public void RenderingToADeviceContextReportsThatHandlesAreUnavailable()
    {
        using var bmp = new Bitmap(10, 10);
        using var target = Graphics.FromImage(bmp);
        using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 10, 10));

        Assert.Throws<PlatformNotSupportedException>(() => buffer.Render(IntPtr.Zero));
    }

    [Fact]
    public void ADegenerateRectangleStillYieldsAUsableBuffer()
    {
        using var bmp = new Bitmap(10, 10);
        using var target = Graphics.FromImage(bmp);

        using var buffer = BufferedGraphicsManager.Current.Allocate(target, new Rectangle(0, 0, 0, 0));

        Assert.NotNull(buffer.Graphics);
    }
}

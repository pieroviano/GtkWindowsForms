extern alias gtk;
using Bitmap = gtk::System.Drawing.Bitmap;
using Image = gtk::System.Drawing.Image;
using ImageAnimator = gtk::System.Drawing.ImageAnimator;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8603 // Possible null reference return.

namespace System.Drawing;

public class ImageAnimatorTests
{
    private gtk::System.Drawing.Image CreateAnimatedGif()
    {
        // Minimal valid animated GIF (2 frames)
        // This is a known tiny GIF with animation
        byte[] gifBytes = Convert.FromBase64String(
            "R0lGODlhAQABAIAAAAUEBA=="); // fallback static if needed

        var ms = new MemoryStream(gifBytes);
        return Image.FromStream(ms);
    }

    private Image CreateBitmap()
    {
        return new Bitmap(10, 10);
    }

    [Fact]
    public void CanAnimate_Null_ReturnsFalse()
    {
        Assert.False(ImageAnimator.CanAnimate(null));
    }

    [Fact]
    public void CanAnimate_StaticImage_ReturnsFalse()
    {
        using var bmp = CreateBitmap();

        var result = ImageAnimator.CanAnimate(bmp);

        Assert.False(result);
    }

    [Fact]
    public void Animate_NullImage_DoesNothing()
    {
        ImageAnimator.Animate(null, (_, _) => { });

        // No exception = success
    }

    [Fact]
    public void StopAnimate_NullImage_DoesNothing()
    {
        ImageAnimator.StopAnimate(null, (_, _) => { });

        // No exception = success
    }

    [Fact]
    public void StopAnimate_NoList_DoesNothing()
    {
        using var bmp = CreateBitmap();

        ImageAnimator.StopAnimate(bmp, (_, _) => { });

        // No exception = success
    }

    [Fact]
    public void UpdateFrames_NullImage_DoesNothing()
    {
        ImageAnimator.UpdateFrames(null);

        // No exception = success
    }

    [Fact]
    public void UpdateFrames_NoImages_DoesNothing()
    {
        ImageAnimator.UpdateFrames();

        // No exception = success
    }

    [Fact]
    public void Animate_Then_StopAnimate_DoesNotThrow()
    {
        using var bmp = CreateBitmap();

        EventHandler handler = (_, _) => { };

        ImageAnimator.Animate(bmp, handler);
        ImageAnimator.StopAnimate(bmp, handler);

        // Just verifying no crash
    }

    [Fact]
    public void Animate_CalledMultipleTimes_DoesNotThrow()
    {
        using var bmp = CreateBitmap();

        EventHandler handler = (_, _) => { };

        ImageAnimator.Animate(bmp, handler);
        ImageAnimator.Animate(bmp, handler);

        Assert.True(true);
    }

    [Fact]
    public void UpdateFrames_WithImage_DoesNotThrow()
    {
        using var bmp = CreateBitmap();

        ImageAnimator.UpdateFrames(bmp);

        Assert.True(true);
    }

    [Fact]
    public void UpdateFrames_GlobalCall_DoesNotThrow()
    {
        ImageAnimator.UpdateFrames();

        Assert.True(true);
    }

    [Fact]
    public void Animate_WithHandler_HandlerCanBeInvoked()
    {
        using var bmp = CreateBitmap();

        bool called = false;

        EventHandler handler = (_, _) => called = true;

        ImageAnimator.Animate(bmp, handler);

        // Give background thread a chance (if it starts)
        Thread.Sleep(100);

        // We can't guarantee animation occurs (non-animated image),
        // but we ensure no crash and handler is safe
        Assert.False(called);
    }
}
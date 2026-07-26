extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using SolidBrush = gtk::System.Drawing.SolidBrush;

namespace System.Drawing;

/// <summary>
/// Behavioural tests for <see cref="Graphics.FromImage" />' treatment of the image it is given.
/// <para>
/// It allocated a blank Cairo surface and immediately replaced the image's pixbuf with a snapshot of that
/// blank surface, so merely asking an image for a Graphics erased everything already drawn on it. GDI+
/// composes onto the existing content instead - which is what makes the ordinary "load a bitmap, then
/// annotate it" and "draw in two passes" patterns work.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class GraphicsFromImageFunctionalTests
{
    private static bool IsRed(Color c) => c.R > 200 && c.G < 80 && c.B < 80;

    private static bool IsBlue(Color c) => c.B > 200 && c.R < 80 && c.G < 80;

    [Fact]
    public void FromImage_PreservesWhatIsAlreadyOnTheImage()
    {
        using var bmp = new Bitmap(20, 20);
        using (var first = Graphics.FromImage(bmp))
        {
            first.Clear(Color.Red);
        }

        // Re-opening the image must not wipe it.
        using (var second = Graphics.FromImage(bmp))
        {
        }

        Assert.True(IsRed(bmp.GetPixel(10, 10)),
            $"content drawn in an earlier pass must survive a later FromImage, but the pixel was {bmp.GetPixel(10, 10)}");
    }

    [Fact]
    public void FromImage_SecondPass_ComposesOverTheFirst()
    {
        using var bmp = new Bitmap(20, 20);
        using (var first = Graphics.FromImage(bmp))
        {
            first.Clear(Color.Red);
        }

        using (var second = Graphics.FromImage(bmp))
        {
            second.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 10, 10);
        }

        Assert.True(IsBlue(bmp.GetPixel(5, 5)), "the second pass must paint where it drew");
        Assert.True(IsRed(bmp.GetPixel(15, 15)), "the second pass must leave the first pass alone elsewhere");
    }
}

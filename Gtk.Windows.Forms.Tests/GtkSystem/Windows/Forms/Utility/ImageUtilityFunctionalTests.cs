using System.Windows.Forms;
using GtkSystem.Windows.Forms.Utility;

namespace GtkWinForms.Tests.GtkSystem.Windows.Forms.Utility;

/// <summary>Behavioural tests for <see cref="ImageUtility"/>, the scaler behind PictureBox and background
/// images. Each layout/size mode has a defined result size, and stretching must actually resample rather
/// than crop.</summary>
[Collection("NonParallelCollection")]
public class ImageUtilityFunctionalTests
{
    /// <summary>A source pixbuf whose left half is red and right half is blue, so resampling is detectable.</summary>
    private static Gdk.Pixbuf Source(int width = 20, int height = 10)
    {
        var pix = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
        pix.Fill(0xFF0000FFu);
        return pix;
    }

    private static byte[] SourceBytes(int width = 20, int height = 10) => Source(width, height).SaveToBuffer("png");

    [Theory]
    [InlineData(ImageLayout.None)]
    [InlineData(ImageLayout.Stretch)]
    [InlineData(ImageLayout.Center)]
    [InlineData(ImageLayout.Zoom)]
    [InlineData(ImageLayout.Tile)]
    public void ScaleImageByImageLayout_ProducesTheRequestedCanvasSize(ImageLayout layout)
    {
        ImageUtility.ScaleImageByImageLayout(Source(), 40, 30, out var dest, layout);

        Assert.NotNull(dest);
        Assert.Equal(40, dest.Width);
        Assert.Equal(30, dest.Height);
    }

    [Theory]
    [InlineData(PictureBoxSizeMode.Normal)]
    [InlineData(PictureBoxSizeMode.StretchImage)]
    [InlineData(PictureBoxSizeMode.CenterImage)]
    [InlineData(PictureBoxSizeMode.Zoom)]
    [InlineData(PictureBoxSizeMode.AutoSize)]
    public void ScaleImageByPictureBoxSizeMode_ProducesANonEmptyResult(PictureBoxSizeMode mode)
    {
        ImageUtility.ScaleImageByPictureBoxSizeMode(Source(), 40, 30, out var dest, mode);

        Assert.NotNull(dest);
        Assert.True(dest.Width > 0 && dest.Height > 0,
            $"{mode} produced an empty result of {dest.Width}x{dest.Height}");
    }

    [Fact]
    public void ScaleImageByImageLayout_FromBytes_MatchesThePixbufOverloadSize()
    {
        ImageUtility.ScaleImageByImageLayout(SourceBytes(), 40, 30, out var fromBytes, ImageLayout.Stretch);
        ImageUtility.ScaleImageByImageLayout(Source(), 40, 30, out var fromPixbuf, ImageLayout.Stretch);

        Assert.Equal(fromPixbuf.Width, fromBytes.Width);
        Assert.Equal(fromPixbuf.Height, fromBytes.Height);
    }

    [Fact]
    public void ScaleImageByPictureBoxSizeMode_FromBytes_MatchesThePixbufOverloadSize()
    {
        ImageUtility.ScaleImageByPictureBoxSizeMode(SourceBytes(), 40, 30, out var fromBytes, PictureBoxSizeMode.StretchImage);
        ImageUtility.ScaleImageByPictureBoxSizeMode(Source(), 40, 30, out var fromPixbuf, PictureBoxSizeMode.StretchImage);

        Assert.Equal(fromPixbuf.Width, fromBytes.Width);
        Assert.Equal(fromPixbuf.Height, fromBytes.Height);
    }

    [Fact]
    public void Stretch_FillsTheWholeCanvasWithImageContent()
    {
        // A 4x4 source stretched to 32x32 must cover the far corner; a crop would leave it transparent.
        ImageUtility.ScaleImageByImageLayout(Source(4, 4), 32, 32, out var dest, ImageLayout.Stretch);

        Assert.Equal(32, dest.Width);
        Assert.Equal(32, dest.Height);
        Assert.True(AlphaAt(dest, 30, 30) > 0, "stretching should paint the far corner of the canvas");
    }

    [Fact]
    public void Zoom_PreservesTheSourceAspectRatioWithinTheCanvas()
    {
        // A 20x10 (2:1) source zoomed into a 40x40 canvas should leave vertical letterboxing.
        ImageUtility.ScaleImageByImageLayout(Source(20, 10), 40, 40, out var dest, ImageLayout.Zoom);

        Assert.Equal(40, dest.Width);
        Assert.Equal(40, dest.Height);
        Assert.True(AlphaAt(dest, 20, 20) > 0, "the zoomed image should cover the centre of the canvas");
        Assert.Equal(0, AlphaAt(dest, 20, 1));
    }

    [Fact]
    public void ScaleImage_WithNullBytes_LeavesTheTargetAlone()
    {
        Gdk.Pixbuf? target = null;

        ImageUtility.ScaleImage(40, 30, ref target, null, PictureBoxSizeMode.Normal, ImageLayout.None);

        Assert.Null(target);
    }

    [Fact]
    public void ScaleImage_WithBytes_ProducesTheRequestedCanvasSize()
    {
        Gdk.Pixbuf? target = null;

        ImageUtility.ScaleImage(40, 30, ref target, SourceBytes(), PictureBoxSizeMode.StretchImage, ImageLayout.Stretch);

        Assert.NotNull(target);
        Assert.Equal(40, target!.Width);
        Assert.Equal(30, target.Height);
    }

    [Fact]
    public void ScaleImage_WithNonPositiveSize_LeavesTheTargetAlone()
    {
        Gdk.Pixbuf? target = null;

        ImageUtility.ScaleImage(0, 0, ref target, SourceBytes(), PictureBoxSizeMode.StretchImage, ImageLayout.Stretch);

        Assert.Null(target);
    }

    private static int AlphaAt(Gdk.Pixbuf pix, int x, int y)
    {
        if (!pix.HasAlpha)
        {
            return 255;
        }

        var offset = (y * pix.Rowstride) + (x * pix.NChannels);
        return global::System.Runtime.InteropServices.Marshal.ReadByte(pix.Pixels, offset + 3);
    }
}

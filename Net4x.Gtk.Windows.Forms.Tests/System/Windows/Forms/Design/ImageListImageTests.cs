extern alias gtk;

using System.Drawing;
using Bitmap = gtk::System.Drawing.Bitmap;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;
using PixelFormat = gtk::System.Drawing.Imaging.PixelFormat;

namespace System.Windows.Forms.Design;

public class ImageListImageTests
{
    public ImageListImageTests()
    {
        Application.Init();
    }

    [Fact]
    public void Constructor_WithImage_SetsImage()
    {
        using var bmp = new Bitmap(10, 20);

        var wrapper = new ImageListImage(bmp);

        Assert.Equal(bmp, wrapper.Image);
    }

    [Fact]
    public void Constructor_WithImageAndName_SetsBoth()
    {
        using var bmp = new Bitmap(5, 5);

        var wrapper = new ImageListImage(bmp, "test");

        Assert.Equal(bmp, wrapper.Image);
        Assert.Equal("test", wrapper.Name);
    }

    [Fact]
    public void Name_ReturnsEmpty_WhenNull()
    {
        var wrapper = new ImageListImage(null);

        Assert.Equal(string.Empty, wrapper.Name);
    }

    [Fact]
    public void Name_Setter_Works()
    {
        var wrapper = new ImageListImage(null)
        {
            Name = "hello"
        };

        Assert.Equal("hello", wrapper.Name);
    }

    [Fact]
    public void Properties_ReturnDefault_WhenImageIsNull()
    {
        var wrapper = new ImageListImage(null);

        Assert.Equal(0, wrapper.HorizontalResolution);
        Assert.Equal(default(SizeF), wrapper.PhysicalDimension);
        Assert.Equal(default(PixelFormat), wrapper.PixelFormat);
        Assert.Null(wrapper.RawFormat);
        Assert.Equal(default(Size), wrapper.Size);
        Assert.Equal(0, wrapper.VerticalResolution);
    }

    [Fact]
    public void Properties_ReturnValues_FromImage()
    {
        using var bmp = new Bitmap(10, 20);

        var wrapper = new ImageListImage(bmp);

        Assert.Equal(bmp.HorizontalResolution, wrapper.HorizontalResolution);
        Assert.Equal(bmp.Size, wrapper.PhysicalDimension);
        Assert.Equal(bmp.PixelFormat, wrapper.PixelFormat);
        Assert.Equal(bmp.RawFormat, wrapper.RawFormat);
        Assert.Equal(bmp.Size, wrapper.Size);
        Assert.Equal(bmp.VerticalResolution, wrapper.VerticalResolution);
    }

    [Fact]
    public void ImageListImageFromStream_LoadsBitmap()
    {
        using var bmp = System.Properties.Resources.timg;
        using var ms = new MemoryStream();

        bmp.Save(ms, ImageFormat.Png);
        ms.Position = 0;

        var result = ImageListImage.ImageListImageFromStream(ms, imageIsIcon: false);

        Assert.NotNull(result.Image);
        Assert.Equal(2560, result.Size.Width);
        Assert.Equal(1440, result.Size.Height);
    }

    [Fact]
    public void ImageListImageFromStream_LoadsIcon_AsBitmap()
    {
        using var iconStream = File.OpenRead(GetTestIconPath());

        var result = ImageListImage.ImageListImageFromStream(iconStream, imageIsIcon: true);

        Assert.NotNull(result.Image);
        Assert.IsType<Bitmap>(result.Image);
    }

    private static string GetTestIconPath()
    {
        // You need to include a test .ico file in your test project
        return Path.Combine("Resources", "test.ico");
    }
}
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

extern alias gtk;

using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using NSubstitute;
using Bitmap = gtk::System.Drawing.Bitmap;
using Image = gtk::System.Drawing.Image;
using ImageConverter = gtk::System.Drawing.ImageConverter;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class ImageConverterTests
{
    private readonly ImageConverter _converter = new ImageConverter();

    private Image CreateBitmap()
    {
        Application.Init();
        return Tests.Resources.timg6;
    }

    #region CanConvert

    [Fact]
    public void CanConvertFrom_AnyType_ReturnsTrue()
    {
        var result = _converter.CanConvertFrom(null, typeof(int));

        Assert.True(result);
    }

    [Theory]
    [InlineData(typeof(byte[]), true)]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), false)]
    public void CanConvertTo_ReturnsExpected(Type type, bool expected)
    {
        var result = _converter.CanConvertTo(null, type);

        Assert.Equal(expected, result);
    }

    #endregion

    #region ConvertFrom

    [Fact]
    public void ConvertFrom_ByteArray_ReturnsImage()
    {
        using var bmp = CreateBitmap();
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);

        var bytes = ms.ToArray();

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, bytes);

        Assert.NotNull(result);
        Assert.IsType<Bitmap>(result);
    }

    [Fact]
    public void ConvertFrom_InvalidType_Throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, 123));
    }

    [Fact]
    public void ConvertFrom_InvalidByteArray_StillAttemptsImageCreation()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, bytes);

        Assert.NotNull(result);
    }

    #endregion

    #region ConvertTo String

    [Fact]
    public void ConvertTo_String_NullValue_ReturnsNone()
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string));

        Assert.Equal("none", result);
    }

    [Fact]
    public void ConvertTo_String_Image_ReturnsToString()
    {
        using var bmp = CreateBitmap();

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, bmp, typeof(string));

        Assert.Equal(bmp.ToString(), result);
    }

    #endregion

    #region ConvertTo ByteArray

    [Fact]
    public void ConvertTo_ByteArray_NullValue_ReturnsEmptyArray()
    {
        var result = _converter.ConvertTo(Substitute.For<ITypeDescriptorContext>(), CultureInfo.InvariantCulture, null, typeof(byte[]));

        Assert.Empty((byte[])result);
    }

    [Fact]
    public void ConvertTo_ByteArray_Image_ReturnsBytes()
    {
        using var bmp = CreateBitmap();

        var result = _converter.ConvertTo(Substitute.For<ITypeDescriptorContext>(), CultureInfo.InvariantCulture, bmp, typeof(byte[]));

        var bytes = Assert.IsType<byte[]>(result);
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void ConvertTo_ByteArray_JpegConvertedToPng()
    {
        using var bmp = CreateBitmap();

        // Force JPEG format
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);
        using var jpegImage = Image.FromStream(new MemoryStream(ms.ToArray()));

#pragma warning disable CS8604 // Possible null reference argument.
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, jpegImage, typeof(byte[]));
#pragma warning restore CS8604 // Possible null reference argument.

        var bytes = Assert.IsType<byte[]>(result);
        Assert.NotEmpty(bytes);
    }

    #endregion

    #region ConvertTo Invalid

    [Fact]
    public void ConvertTo_InvalidType_Throws()
    {
        using var bmp = CreateBitmap();

        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertTo(null, CultureInfo.InvariantCulture, bmp, typeof(int)));
    }

    #endregion

    #region Properties

    [Fact]
    public void GetPropertiesSupported_ReturnsTrue()
    {
        var result = _converter.GetPropertiesSupported(null);

        Assert.True(result);
    }

    [Fact]
    public void GetProperties_ReturnsImageProperties()
    {
        using var bmp = CreateBitmap();

        var props = _converter.GetProperties(null, bmp, null);

        Assert.NotNull(props);
        Assert.NotEmpty(props);
    }

    #endregion
}
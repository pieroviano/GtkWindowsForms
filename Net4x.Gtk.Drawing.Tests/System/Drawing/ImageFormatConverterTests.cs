extern alias gtk;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;
using ImageFormatConverter = gtk::System.Drawing.ImageFormatConverter;

namespace System.Drawing;

public class ImageFormatConverterTests
{
    private readonly ImageFormatConverter _converter = new ImageFormatConverter();

    #region CanConvert

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        var result = _converter.CanConvertFrom(null, typeof(string));

        Assert.True(result);
    }

    [Fact]
    public void CanConvertFrom_NonString_DelegatesToBase()
    {
        var result = _converter.CanConvertFrom(null, typeof(int));

        Assert.False(result);
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(int), false)]
    public void CanConvertTo_ReturnsExpected(Type type, bool expected)
    {
        var result = _converter.CanConvertTo(null, type);

        Assert.Equal(expected, result);
    }

    #endregion

    #region ConvertFrom - Known Formats

    [Theory]
    [InlineData("Bmp")]
    [InlineData("Emf")]
    [InlineData("Exif")]
    [InlineData("Gif")]
    [InlineData("Icon")]
    [InlineData("Jpeg")]
    [InlineData("MemoryBmp")]
    [InlineData("Png")]
    [InlineData("Tiff")]
    [InlineData("Wmf")]
    [InlineData("Heif")]
    [InlineData("Webp")]
    public void ConvertFrom_KnownFormats_ReturnsExpected(string input)
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        Assert.IsType<ImageFormat>(result);
    }

    [Fact]
    public void ConvertFrom_IsCaseInsensitive()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "png");

        Assert.Equal(ImageFormat.Png.Guid, ((ImageFormat)result).Guid);
    }

    #endregion

    #region ConvertFrom - Guid Format

    [Fact]
    public void ConvertFrom_GuidFormattedString_ReturnsImageFormat()
    {
        var guid = Guid.NewGuid();

        // Matches expected format: "[ImageFormat: guid]"
        var input = $"[ImageFormat: {guid}]";

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        var format = Assert.IsType<ImageFormat>(result);
        Assert.Equal(guid, format.Guid);
    }

    #endregion

    #region ConvertFrom - Invalid

    [Fact]
    public void ConvertFrom_InvalidString_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "InvalidFormat"));
    }

    [Fact]
    public void ConvertFrom_InvalidType_DelegatesToBase()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, 123));
    }

    #endregion

    #region ConvertTo - String

    [Fact]
    public void ConvertTo_String_ReturnsToString()
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, ImageFormat.Png, typeof(string));

        Assert.Equal(ImageFormat.Png.ToString(), result);
    }

    #endregion

    #region ConvertTo - InstanceDescriptor (Known Formats)

    [Theory]
    [InlineData(nameof(ImageFormat.Bmp))]
    [InlineData(nameof(ImageFormat.Png))]
    [InlineData(nameof(ImageFormat.Jpeg))]
    public void ConvertTo_InstanceDescriptor_KnownFormats_UsesProperty(string propertyName)
    {
        var format = (ImageFormat)typeof(ImageFormat).GetProperty(propertyName).GetValue(null);

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, format, typeof(InstanceDescriptor));

        var descriptor = Assert.IsType<InstanceDescriptor>(result);

        Assert.NotNull(descriptor.MemberInfo);
        Assert.Equal(propertyName, descriptor.MemberInfo.Name);
    }

    #endregion

    #region ConvertTo - InstanceDescriptor (Custom Guid)

    [Fact]
    public void ConvertTo_InstanceDescriptor_CustomFormat_UsesConstructor()
    {
        var custom = new ImageFormat(Guid.NewGuid());

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, custom, typeof(InstanceDescriptor));

        var descriptor = Assert.IsType<InstanceDescriptor>(result);

        Assert.NotNull(descriptor.MemberInfo);
        Assert.Equal(".ctor", descriptor.MemberInfo.Name);
    }

    #endregion

    #region ConvertTo - Invalid

    [Fact]
    public void ConvertTo_InvalidType_DelegatesToBase()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertTo(null, CultureInfo.InvariantCulture, ImageFormat.Png, typeof(int)));
    }

    #endregion

    #region Standard Values

    [Fact]
    public void GetStandardValues_ReturnsExpectedFormats()
    {
        var values = _converter.GetStandardValues(null).Cast<ImageFormat>();

        Assert.NotNull(values);
        Assert.Contains(ImageFormat.Png, values);
        Assert.Contains(ImageFormat.Jpeg, values);
        Assert.Contains(ImageFormat.Webp, values);
        Assert.Equal(12, values.Count());
    }

    [Fact]
    public void GetStandardValuesSupported_ReturnsTrue()
    {
        var result = _converter.GetStandardValuesSupported(null);

        Assert.True(result);
    }

    #endregion
}
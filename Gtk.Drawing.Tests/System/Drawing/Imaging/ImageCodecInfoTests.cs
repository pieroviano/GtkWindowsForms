extern alias gtk;

using System.Collections;
using gtk::System.Drawing.Imaging;

namespace System.Drawing.Imaging;

public class ImageCodecInfoTests
{
    [Fact]
    public void Properties_SetAndGet_WorkCorrectly()
    {
        var codec = new ImageCodecInfo();

        var clsid = Guid.NewGuid();
        var formatId = Guid.NewGuid();

        codec.Clsid = clsid;
        codec.CodecName = "TestCodec";
        codec.DllName = "test.dll";
        codec.FilenameExtension = "*.jpg";
        codec.Flags = ImageCodecFlags.Encoder;
        codec.FormatDescription = "JPEG";
        codec.FormatID = formatId;
        codec.MimeType = "image/jpeg";
        codec.Version = 42;

        Assert.Equal(clsid, codec.Clsid);
        Assert.Equal("TestCodec", codec.CodecName);
        Assert.Equal("test.dll", codec.DllName);
        Assert.Equal("*.jpg", codec.FilenameExtension);
        Assert.Equal(ImageCodecFlags.Encoder, codec.Flags);
        Assert.Equal("JPEG", codec.FormatDescription);
        Assert.Equal(formatId, codec.FormatID);
        Assert.Equal("image/jpeg", codec.MimeType);
        Assert.Equal(42, codec.Version);
    }

    [Fact]
    public void SignatureMasks_SetAndGet_Works()
    {
        var codec = new ImageCodecInfo();

        var masks = new[]
        {
            new byte[] { 1, 2 },
            new byte[] { 3, 4 }
        };

        codec.SignatureMasks = masks;

        Assert.Same(masks, codec.SignatureMasks);
    }

    [Fact]
    public void SignaturePatterns_SetAndGet_Works()
    {
        var codec = new ImageCodecInfo();

        var patterns = new[]
        {
            new byte[] { 5, 6 },
            new byte[] { 7, 8 }
        };

        codec.SignaturePatterns = patterns;

        Assert.Same(patterns, codec.SignaturePatterns);
    }

    [Fact]
    public void Properties_AllowNullValues()
    {
        var codec = new ImageCodecInfo
        {
            CodecName = null,
            DllName = null,
            FilenameExtension = null,
            FormatDescription = null,
            MimeType = null,
            SignatureMasks = null,
            SignaturePatterns = null
        };

        Assert.Null(codec.CodecName);
        Assert.Null(codec.DllName);
        Assert.Null(codec.FilenameExtension);
        Assert.Null(codec.FormatDescription);
        Assert.Null(codec.MimeType);
        Assert.Null(codec.SignatureMasks);
        Assert.Null(codec.SignaturePatterns);
    }

    [Fact]
    public void GetImageDecoders_ReturnsArray_NotNull()
    {
        var result = ImageCodecInfo.GetImageDecoders();

        Assert.NotNull(result);
        Assert.Single((IEnumerable)result); // based on current implementation
    }

    [Fact]
    public void GetImageEncoders_ReturnsArray_NotNull()
    {
        var result = ImageCodecInfo.GetImageEncoders();

        Assert.NotNull(result);
        Assert.Single((IEnumerable)result); // based on current implementation
    }

    [Fact]
    public void GetImageEncoders_And_Decoders_ReturnDifferentInstances()
    {
        var encoders = ImageCodecInfo.GetImageEncoders();
        var decoders = ImageCodecInfo.GetImageDecoders();

        Assert.NotSame(encoders, decoders);
    }

    [Fact]
    public void ReturnedArrays_AreInitializedWithNullElements()
    {
        var codecs = ImageCodecInfo.GetImageEncoders();

        Assert.Single((IEnumerable)codecs);
        Assert.Null(codecs[0]); // important subtle behavior
    }
}
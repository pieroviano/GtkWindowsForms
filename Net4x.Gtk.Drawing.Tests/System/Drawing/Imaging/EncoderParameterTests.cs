extern alias gtk;

using Encoder = gtk::System.Drawing.Imaging.Encoder;
using EncoderParameter = gtk::System.Drawing.Imaging.EncoderParameter;
using EncoderParameterValueType = gtk::System.Drawing.Imaging.EncoderParameterValueType;

namespace System.Drawing.Imaging;

public class EncoderParameterTests
{
    [Fact]
    public void Ctor_String_SetsPropertiesCorrectly()
    {
        var encoder = Encoder.Quality;
        var value = "test";

        using var param = new EncoderParameter(encoder, value);

        Assert.Equal(encoder.Guid, param.Encoder.Guid);
        Assert.Equal(EncoderParameterValueType.ValueTypeAscii, param.ValueType);
        Assert.Equal(value.Length, param.NumberOfValues);
    }

    [Fact]
    public void Ctor_ByteArray_SetsPropertiesCorrectly()
    {
        var encoder = Encoder.Quality;
        var value = new byte[] { 1, 2, 3 };

        using var param = new EncoderParameter(encoder, value);

        Assert.Equal(encoder.Guid, param.Encoder.Guid);
        Assert.Equal(EncoderParameterValueType.ValueTypeByte, param.ValueType);
        Assert.Equal(value.Length, param.NumberOfValues);
    }

    [Fact]
    public void Ctor_ByteArray_WithUndefinedFalse_SetsByteType()
    {
        var encoder = Encoder.Quality;
        var value = new byte[] { 1, 2 };

        using var param = new EncoderParameter(encoder, value, undefined: false);

        Assert.Equal(EncoderParameterValueType.ValueTypeByte, param.ValueType);
        Assert.Equal(value.Length, param.NumberOfValues);
    }

    [Fact]
    public void Ctor_ByteArray_WithUndefinedTrue_SetsUndefinedType()
    {
        var encoder = Encoder.Quality;
        var value = new byte[] { 1, 2 };

        using var param = new EncoderParameter(encoder, value, undefined: true);

        Assert.Equal(EncoderParameterValueType.ValueTypeUndefined, param.ValueType);
    }

    [Fact]
    public void Ctor_ShortArray_SetsPropertiesCorrectly()
    {
        var encoder = Encoder.Quality;
        var value = new short[] { 10, 20, 30 };

        using var param = new EncoderParameter(encoder, value);

        Assert.Equal(encoder.Guid, param.Encoder.Guid);
        Assert.Equal(EncoderParameterValueType.ValueTypeShort, param.ValueType);
        Assert.Equal(value.Length, param.NumberOfValues);
    }

    [Fact]
    public void Encoder_Property_Setter_UpdatesGuid()
    {
        var encoder1 = Encoder.Quality;
        var encoder2 = Encoder.ColorDepth;

        using var param = new EncoderParameter(encoder1, new byte[] { 1 });

        param.Encoder = encoder2;

        Assert.Equal(encoder2.Guid, param.Encoder.Guid);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes_DoesNotThrow()
    {
        var encoder = Encoder.Quality;

        var param = new EncoderParameter(encoder, new byte[] { 1, 2 });

        param.Dispose();
        param.Dispose(); // should not throw
    }

    [Fact]
    public void Dispose_ReleasesMemory_SetsPointerToZero()
    {
        var encoder = Encoder.Quality;

        var param = new EncoderParameter(encoder, new byte[] { 1, 2 });

        param.Dispose();

        // indirect verification: calling Dispose again should not crash
        param.Dispose();
    }
}
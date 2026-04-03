using System.Globalization;

namespace System.Windows.Forms.AtrributeTests;

public class OpacityConverterTests
{
    private readonly OpacityConverter _converter = new OpacityConverter();

    // -------------------------
    // CanConvertFrom
    // -------------------------

    [Fact]
    public void CanConvertFrom_AlwaysReturnsTrue_ForVariousTypes()
    {
        Assert.True(_converter.CanConvertFrom(null, typeof(string)));
        Assert.True(_converter.CanConvertFrom(null, typeof(int)));
        Assert.True(_converter.CanConvertFrom(null, typeof(double)));
        Assert.True(_converter.CanConvertFrom(null, typeof(object)));
    }

    // -------------------------
    // ConvertFrom
    // -------------------------

    [Fact]
    public void ConvertFrom_ReturnsSameValue_String()
    {
        var input = "0.5";

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        Assert.Same(input, result);
    }

    [Fact]
    public void ConvertFrom_ReturnsSameValue_Double()
    {
        var input = 0.75;

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertFrom_ReturnsSameValue_Object()
    {
        var input = new object();

        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        Assert.Same(input, result);
    }

    [Fact]
    public void ConvertFrom_Null_ReturnsNull()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, null);

        Assert.Null(result);
    }

    // -------------------------
    // ConvertTo
    // -------------------------

    [Fact]
    public void ConvertTo_ReturnsSameValue_StringDestination()
    {
        var input = 0.5;

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(string));

        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertTo_ReturnsSameValue_ObjectDestination()
    {
        var input = 0.25;

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(object));

        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertTo_ReturnsSameInstance_ForReferenceType()
    {
        var input = new object();

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, typeof(object));

        Assert.Same(input, result);
    }

    [Fact]
    public void ConvertTo_NullValue_ReturnsNull()
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string));

        Assert.Null(result);
    }

    [Fact]
    public void ConvertTo_NullDestinationType_DoesNotThrow_AndReturnsValue()
    {
        var input = 0.9;

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, input, null);

        Assert.Equal(input, result);
    }
}
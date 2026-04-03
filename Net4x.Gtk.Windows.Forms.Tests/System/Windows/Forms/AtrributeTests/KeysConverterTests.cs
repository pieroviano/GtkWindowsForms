using System.Globalization;

namespace System.Windows.Forms.AtrributeTests;

public class KeysConverterTests
{
    private readonly KeysConverter _converter = new KeysConverter();

    // -------------------------
    // CanConvert tests
    // -------------------------

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        Assert.True(_converter.CanConvertFrom(null, typeof(string)));
    }

    [Fact]
    public void CanConvertFrom_EnumArray_ReturnsTrue()
    {
        Assert.True(_converter.CanConvertFrom(null, typeof(Enum[])));
    }

    [Fact]
    public void CanConvertTo_EnumArray_ReturnsTrue()
    {
        Assert.True(_converter.CanConvertTo(null, typeof(Enum[])));
    }

    // -------------------------
    // ConvertFrom (string)
    // -------------------------

    [Fact]
    public void ConvertFrom_EmptyString_ReturnsNull()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "   ");

        Assert.Null(result);
    }

    [Fact]
    public void ConvertFrom_SingleKey_ReturnsKey()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "A");

        Assert.Equal(Keys.A, result);
    }

    [Fact]
    public void ConvertFrom_ModifierCombination_ReturnsCombinedKeys()
    {
        var result = (Keys)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, "Control + A");

        Assert.True(result.HasFlag(Keys.Control));
        Assert.True(result.HasFlag(Keys.A));
    }

    [Fact]
    public void ConvertFrom_InvalidKey_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "InvalidKey"));
    }

    [Fact]
    public void ConvertFrom_MultipleKeyCodes_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "A + B"));
    }

    // -------------------------
    // ConvertFrom (Enum[])
    // -------------------------

    [Fact]
    public void ConvertFrom_EnumArray_ReturnsCombinedKeys()
    {
        var input = new Enum[] { Keys.Control, Keys.A };

        var result = (Keys)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        Assert.True(result.HasFlag(Keys.Control));
        Assert.True(result.HasFlag(Keys.A));
    }

    // -------------------------
    // ConvertTo (string)
    // -------------------------

    [Fact]
    public void ConvertTo_String_SingleKey()
    {
        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, Keys.A, typeof(string));

        Assert.Contains("A", result.ToString());
    }

    [Fact]
    public void ConvertTo_String_WithModifier()
    {
        var key = Keys.Control | Keys.A;

        var result = _converter.ConvertTo(null, CultureInfo.InvariantCulture, key, typeof(string)).ToString();

        Assert.Contains("Control", result);
        Assert.Contains("A", result);
    }

    // -------------------------
    // ConvertTo (Enum[])
    // -------------------------

    [Fact]
    public void ConvertTo_EnumArray_ReturnsArray()
    {
        var key = Keys.Control | Keys.A;

        var result = (Enum[])_converter.ConvertTo(null, CultureInfo.InvariantCulture, key, typeof(Enum[]));

        Assert.Contains(result, e => (Keys)e == Keys.Control);
        Assert.Contains(result, e => (Keys)e == Keys.A);
    }

    [Fact]
    public void ConvertTo_NullDestination_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _converter.ConvertTo(null, CultureInfo.InvariantCulture, Keys.A, null));
    }

    // -------------------------
    // Compare
    // -------------------------

    [Fact]
    public void Compare_OrdersAlphabetically()
    {
        var result = _converter.Compare(Keys.A, Keys.B);

        Assert.True(result < 0);
    }

    // -------------------------
    // Standard values
    // -------------------------

    [Fact]
    public void GetStandardValues_ReturnsCollection()
    {
        var values = _converter.GetStandardValues(null);

        Assert.NotNull(values);
        Assert.NotEmpty(values);
    }

    [Fact]
    public void GetStandardValues_IsCached()
    {
        var v1 = _converter.GetStandardValues(null);
        var v2 = _converter.GetStandardValues(null);

        Assert.Same(v1, v2);
    }

    [Fact]
    public void GetStandardValuesSupported_ReturnsTrue()
    {
        Assert.True(_converter.GetStandardValuesSupported(null));
    }

    [Fact]
    public void GetStandardValuesExclusive_ReturnsFalse()
    {
        Assert.False(_converter.GetStandardValuesExclusive(null));
    }
}
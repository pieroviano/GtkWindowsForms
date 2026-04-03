using System.Globalization;

namespace System.Windows.Forms.ApplicationTests;

public class InputLanguageTests
{
    public InputLanguageTests()
    {
        Application.Init();
    }

    [Fact]
    public void Constructor_SetsHandleCorrectly()
    {
        IntPtr handle = (IntPtr)0x0409; // en-US LCID
        var lang = new InputLanguage(handle);

        Assert.Equal(handle, lang.Handle);
    }

    [Fact]
    public void Culture_ReturnsCorrectCulture()
    {
        IntPtr handle = (IntPtr)0x0409; // en-US
        var lang = new InputLanguage(handle);

        Assert.Equal(0x0409, lang.Culture.LCID);
    }

    [Fact]
    public void Equals_SameHandle_ReturnsTrue()
    {
        var h = (IntPtr)1234;
        var a = new InputLanguage(h);
        var b = new InputLanguage(h);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentHandle_ReturnsFalse()
    {
        var a = new InputLanguage((IntPtr)1);
        var b = new InputLanguage((IntPtr)2);

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void GetHashCode_MatchesHandle()
    {
        var h = (IntPtr)5678;
        var lang = new InputLanguage(h);

        Assert.Equal((int)(long)h, lang.GetHashCode());
    }

    [Fact]
    public void FromCulture_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            InputLanguage.FromCulture(null));
    }

    [Fact]
    public void FromCulture_NoMatch_ReturnsDefault()
    {
        var culture = new CultureInfo("fr-FR");

        var result = InputLanguage.FromCulture(culture);

        Assert.NotNull(result);
        Assert.Equal(InputLanguage.DefaultInputLanguage, result);
    }

    [Fact]
    public void CurrentInputLanguage_Get_ReturnsNotNull()
    {
        var lang = InputLanguage.CurrentInputLanguage;

        Assert.NotNull(lang);
    }

    [Fact]
    public void DefaultInputLanguage_ReturnsNotNull()
    {
        var lang = InputLanguage.DefaultInputLanguage;

        Assert.NotNull(lang);
    }

    [Fact]
    public void InstalledInputLanguages_ReturnsCollection()
    {
        var langs = InputLanguage.InstalledInputLanguages;

        Assert.NotNull(langs);
        Assert.NotEmpty(langs);
    }

    [Fact]
    public void LayoutName_ReturnsString()
    {
        var lang = InputLanguage.CurrentInputLanguage;

        Assert.False(string.IsNullOrEmpty(lang.LayoutName));
    }
}
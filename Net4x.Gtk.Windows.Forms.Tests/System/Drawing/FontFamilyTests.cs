namespace System.Drawing;

public class FontFamilyTests
{
    [Fact]
    public void Constructor_With_String_Should_Set_Name()
    {
        var ff = new FontFamily("Arial");

        Assert.Equal("Arial", ff.Name);
    }

    [Fact]
    public void Constructor_With_GenericFamily_Should_Set_Name()
    {
        var ff = new FontFamily(GenericFontFamilies.Monospace);

        Assert.Equal("Monospace", ff.Name);
    }

    [Fact]
    public void Name_Property_Should_Be_Settable()
    {
        var ff = new FontFamily("A");

        ff.Name = "B";

        Assert.Equal("B", ff.Name);
    }

    [Fact]
    public void Equals_Should_Return_True_For_Same_Instance()
    {
        var ff = new FontFamily("Arial");

        Assert.True(ff.Equals(ff));
    }

    [Fact]
    public void Equals_Should_Return_False_For_Null()
    {
        var ff = new FontFamily("Arial");

        Assert.False(ff.Equals(null));
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Type()
    {
        var ff = new FontFamily("Arial");

        Assert.False(ff.Equals("Arial"));
    }

    [Fact]
    public void Equals_Should_Compare_By_Name()
    {
        var ff1 = new FontFamily("Arial");
        var ff2 = new FontFamily("Arial");

        Assert.True(ff1.Equals(ff2));
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Name()
    {
        var ff1 = new FontFamily("Arial");
        var ff2 = new FontFamily("Times");

        Assert.False(ff1.Equals(ff2));
    }

    [Fact]
    public void GetHashCode_Should_Be_Based_On_Name()
    {
        var ff1 = new FontFamily("Arial");
        var ff2 = new FontFamily("Arial");

        Assert.Equal(ff1.GetHashCode(), ff2.GetHashCode());
    }

    [Fact]
    public void ToString_Should_Return_Name()
    {
        var ff = new FontFamily("Arial");

        Assert.Equal("Arial", ff.ToString());
    }

    [Fact]
    public void GetName_Should_Return_Name()
    {
        var ff = new FontFamily("Arial");

        var result = ff.GetName(0);

        Assert.Equal("Arial", result);
    }

    [Fact]
    public void Dispose_Should_Not_Throw()
    {
        var ff = new FontFamily("Arial");

        var ex = Record.Exception(() => ff.Dispose());

        Assert.Null(ex);
    }

    [Fact]
    public void GenericMonospace_Should_Return_Correct_Name()
    {
        var ff = FontFamily.GenericMonospace;

        Assert.Equal("Monospace", ff.Name);
    }

    [Fact]
    public void GenericSansSerif_Should_Return_Correct_Name()
    {
        var ff = FontFamily.GenericSansSerif;

        Assert.Equal("SansSerif", ff.Name);
    }

    [Fact]
    public void GenericSerif_Should_Return_Correct_Name()
    {
        var ff = FontFamily.GenericSerif;

        Assert.Equal("Serif", ff.Name);
    }

    [Fact]
    public void Obsolete_GetFamilies_Should_Return_Empty_Array()
    {
#pragma warning disable CS0618
        var result = FontFamily.GetFamilies(null);
#pragma warning restore CS0618

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Metrics_Methods_Should_Return_Zero()
    {
        var ff = new FontFamily("Arial");

        Assert.Equal(0, ff.GetCellAscent(FontStyle.Regular));
        Assert.Equal(0, ff.GetCellDescent(FontStyle.Regular));
        Assert.Equal(0, ff.GetEmHeight(FontStyle.Regular));
        Assert.Equal(0, ff.GetLineSpacing(FontStyle.Regular));
    }

    [Fact]
    public void IsStyleAvailable_Should_Return_False()
    {
        var ff = new FontFamily("Arial");

        Assert.False(ff.IsStyleAvailable(FontStyle.Bold));
    }
}
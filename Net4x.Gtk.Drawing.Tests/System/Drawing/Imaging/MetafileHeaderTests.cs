extern alias gtk;

using System.Reflection;
using MetafileHeader = gtk::System.Drawing.Imaging.MetafileHeader;
using MetafileHeaderEmf = gtk::System.Drawing.Imaging.MetafileHeaderEmf;
using MetafileHeaderWmf = gtk::System.Drawing.Imaging.MetafileHeaderWmf;
using MetafileType = gtk::System.Drawing.Imaging.MetafileType;
using MetaHeader = gtk::System.Drawing.Imaging.MetaHeader;

namespace System.Drawing.Imaging;

public class MetafileHeaderTests
{
    private static gtk::System.Drawing.Imaging.MetafileHeader Create()
    {
        return (MetafileHeader)Activator.CreateInstance(
            typeof(MetafileHeader),
            nonPublic: true)!;
    }

    private static void SetField(object obj, string fieldName, object? value)
    {
        var field = obj.GetType().GetField(fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        field!.SetValue(obj, value);
    }

    // -------------------- Bounds --------------------

    [Fact]
    public void Bounds_Returns_Empty_When_No_Data()
    {
        var header = Create();

        Assert.Equal(new Rectangle(0, 0, 0, 0), header.Bounds);
    }

    [Fact]
    public void Bounds_Uses_Emf_When_Available()
    {
        var header = Create();

        var emf = new MetafileHeaderEmf
        {
            X = 1,
            Y = 2,
            Width = 3,
            Height = 4
        };

        SetField(header, "emf", emf);

        Assert.Equal(new Rectangle(1, 2, 3, 4), header.Bounds);
    }

    [Fact]
    public void Bounds_Uses_Wmf_When_Emf_Not_Used()
    {
        var header = Create();

        var wmf = new MetafileHeaderWmf
        {
            X = 5,
            Y = 6,
            Width = 7,
            Height = 8
        };

        SetField(header, "wmf", wmf);

        Assert.Equal(new Rectangle(5, 6, 7, 8), header.Bounds);
    }

    // -------------------- DPI --------------------

    [Fact]
    public void DpiX_From_Emf()
    {
        var header = Create();

        var emf = new MetafileHeaderEmf { dpiX = 96 };
        SetField(header, "emf", emf);

        Assert.Equal(96, header.DpiX);
    }

    [Fact]
    public void DpiY_From_Wmf()
    {
        var header = Create();

        var wmf = new MetafileHeaderWmf { dpiY = 120 };
        SetField(header, "wmf", wmf);

        Assert.Equal(120, header.DpiY);
    }

    [Fact]
    public void Dpi_Default_Zero()
    {
        var header = Create();

        Assert.Equal(0, header.DpiX);
        Assert.Equal(0, header.DpiY);
    }

    // -------------------- Size --------------------

    [Fact]
    public void MetafileSize_From_Emf()
    {
        var header = Create();

        var emf = new MetafileHeaderEmf { size = 123 };
        SetField(header, "emf", emf);

        Assert.Equal(123, header.MetafileSize);
    }

    [Fact]
    public void MetafileSize_From_Wmf()
    {
        var header = Create();

        var wmf = new MetafileHeaderWmf { size = 456 };
        SetField(header, "wmf", wmf);

        Assert.Equal(456, header.MetafileSize);
    }

    // -------------------- Type --------------------

    [Fact]
    public void Type_From_Emf()
    {
        var header = Create();

        var emf = new MetafileHeaderEmf { type = MetafileType.Emf };
        SetField(header, "emf", emf);

        Assert.Equal(MetafileType.Emf, header.Type);
    }

    [Fact]
    public void Type_Default_Invalid()
    {
        var header = Create();

        Assert.Equal(MetafileType.Invalid, header.Type);
    }

    // -------------------- Version --------------------

    [Fact]
    public void Version_From_Emf()
    {
        var header = Create();

        var emf = new MetafileHeaderEmf { version = 2 };
        SetField(header, "emf", emf);

        Assert.Equal(2, header.Version);
    }

    [Fact]
    public void Version_Default_Zero()
    {
        var header = Create();

        Assert.Equal(0, header.Version);
    }

    // -------------------- WMF Specific --------------------

    [Fact]
    public void WmfHeader_Returns_Value()
    {
        var header = Create();

        var metaHeader = new MetaHeader();
        var wmf = new MetafileHeaderWmf { WmfHeader = metaHeader };

        SetField(header, "wmf", wmf);

        Assert.Equal(metaHeader, header.WmfHeader);
    }

    [Fact]
    public void LogicalDpi_From_Wmf()
    {
        var header = Create();

        var wmf = new MetafileHeaderWmf
        {
            LogicalDpiX = 200,
            LogicalDpiY = 300
        };

        SetField(header, "wmf", wmf);

        Assert.Equal(200, header.LogicalDpiX);
        Assert.Equal(300, header.LogicalDpiY);
    }

    // -------------------- Flags --------------------

    [Fact]
    public void IsDisplay_Returns_False_When_Not_EmfPlus()
    {
        var header = Create();

        Assert.False(header.IsDisplay());
    }

    // -------------------- Stubbed methods --------------------

    [Fact]
    public void IsEmf_Always_False()
    {
        var header = Create();
        Assert.False(header.IsEmf());
    }

    [Fact]
    public void IsEmfOrEmfPlus_Always_False()
    {
        var header = Create();
        Assert.False(header.IsEmfOrEmfPlus());
    }

    [Fact]
    public void IsEmfPlus_Always_False()
    {
        var header = Create();
        Assert.False(header.IsEmfPlus());
    }

    [Fact]
    public void IsEmfPlusDual_Always_False()
    {
        var header = Create();
        Assert.False(header.IsEmfPlusDual());
    }

    [Fact]
    public void IsEmfPlusOnly_Always_False()
    {
        var header = Create();
        Assert.False(header.IsEmfPlusOnly());
    }

    [Fact]
    public void IsWmf_Always_False()
    {
        var header = Create();
        Assert.False(header.IsWmf());
    }

    [Fact]
    public void IsWmfPlaceable_Always_False()
    {
        var header = Create();
        Assert.False(header.IsWmfPlaceable());
    }
}
using System.ComponentModel;

namespace System.Drawing.Drawing2D;

extern alias gtk;

using BitmapData = gtk::System.Drawing.Imaging.BitmapData;
using PixelFormat = gtk::System.Drawing.Imaging.PixelFormat;

public class BitmapDataTests
{
    [Fact]
    public void Constructor_Should_Initialize_Default_Values()
    {
        var data = new BitmapData();

        Assert.Equal(0, data.Width);
        Assert.Equal(0, data.Height);
        Assert.Equal(0, data.Stride);
        Assert.Equal(0, data.Reserved);
        Assert.Equal(IntPtr.Zero, data.Scan0);
        Assert.Equal(default, data.PixelFormat);
    }

    [Fact]
    public void Width_Should_Set_And_Get()
    {
        var data = new BitmapData
        {
            Width = 123
        };

        Assert.Equal(123, data.Width);
    }

    [Fact]
    public void Height_Should_Set_And_Get()
    {
        var data = new BitmapData
        {
            Height = 456
        };

        Assert.Equal(456, data.Height);
    }

    [Fact]
    public void Stride_Should_Set_And_Get()
    {
        var data = new BitmapData
        {
            Stride = 789
        };

        Assert.Equal(789, data.Stride);
    }

    [Fact]
    public void Reserved_Should_Set_And_Get()
    {
        var data = new BitmapData
        {
            Reserved = 42
        };

        Assert.Equal(42, data.Reserved);
    }

    [Fact]
    public void Scan0_Should_Set_And_Get()
    {
        var data = new BitmapData();
        var ptr = new IntPtr(123456);

        data.Scan0 = ptr;

        Assert.Equal(ptr, data.Scan0);
    }

    [Theory]
    [InlineData(PixelFormat.Format24bppRgb)]
    [InlineData(PixelFormat.Format32bppArgb)]
    [InlineData(PixelFormat.Format8bppIndexed)]
    [InlineData(PixelFormat.Format16bppRgb565)]
    public void PixelFormat_Should_Accept_Valid_Values(PixelFormat format)
    {
        var data = new BitmapData
        {
            PixelFormat = format
        };

        Assert.Equal(format, data.PixelFormat);
    }

    [Fact]
    public void PixelFormat_Should_Throw_On_Invalid_Value()
    {
        var data = new BitmapData();

        var invalid = (PixelFormat)(-1);

        Assert.Throws<InvalidEnumArgumentException>(() =>
        {
            data.PixelFormat = invalid;
        });
    }

    [Fact]
    public void PixelFormat_Should_Allow_All_Defined_Enum_Values()
    {
        var data = new BitmapData();

        foreach (PixelFormat value in Enum.GetValues(typeof(PixelFormat)))
        {
            try
            {
                data.PixelFormat = value;
                Assert.Equal(value, data.PixelFormat);
            }
            catch (InvalidEnumArgumentException)
            {
                // Only truly invalid values should throw
                Assert.False(Enum.IsDefined(typeof(PixelFormat), value),
                    $"Unexpected failure for {value}");
            }
        }
    }

    [Fact]
    public void GetPinnableReference_Should_Return_Reference_To_Width()
    {
        var data = new BitmapData
        {
            Width = 10
        };

        ref int reference = ref data.GetPinnableReference();

        reference = 99;

        Assert.Equal(99, data.Width);
    }
}
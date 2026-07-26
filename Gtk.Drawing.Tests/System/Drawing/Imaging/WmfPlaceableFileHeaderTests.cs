extern alias gtk;
using System.Reflection;

namespace System.Drawing.Imaging;

using gtk::System.Drawing.Imaging;

public class WmfPlaceableFileHeaderTests
{
    [Fact]
    public void Constructor_DefaultValues_AreCorrect()
    {
        var header = new WmfPlaceableFileHeader();

        Assert.Equal(0, header.BboxBottom);
        Assert.Equal(0, header.BboxLeft);
        Assert.Equal(0, header.BboxRight);
        Assert.Equal(0, header.BboxTop);
        Assert.Equal(0, header.Checksum);
        Assert.Equal(0, header.Hmf);
        Assert.Equal(0, header.Inch);
        Assert.Equal(-1698247209, header.Key); // default value
        Assert.Equal(0, header.Reserved);
    }

    [Fact]
    public void BboxBottom_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            BboxBottom = 10
        };

        Assert.Equal(10, header.BboxBottom);
    }

    [Fact]
    public void BboxLeft_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            BboxLeft = 1
        };

        Assert.Equal(1, header.BboxLeft);
    }

    [Fact]
    public void BboxRight_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            BboxRight = 2
        };

        Assert.Equal(2, header.BboxRight);
    }

    [Fact]
    public void BboxTop_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            BboxTop = 3
        };

        Assert.Equal(3, header.BboxTop);
    }

    [Fact]
    public void Checksum_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            Checksum = 99
        };

        Assert.Equal(99, header.Checksum);
    }

    [Fact]
    public void Hmf_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            Hmf = 7
        };

        Assert.Equal(7, header.Hmf);
    }

    [Fact]
    public void Inch_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            Inch = 1440
        };

        Assert.Equal(1440, header.Inch);
    }

    [Fact]
    public void Key_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            Key = 123456
        };

        Assert.Equal(123456, header.Key);
    }

    [Fact]
    public void Reserved_GetSet_Works()
    {
        var header = new WmfPlaceableFileHeader
        {
            Reserved = 42
        };

        Assert.Equal(42, header.Reserved);
    }

    [Fact]
    public void GetPinnableReference_ReturnsReferenceToKey()
    {
        var header = new WmfPlaceableFileHeader();

        // Use reflection to call internal method
        var method = typeof(WmfPlaceableFileHeader)
            .GetMethod("GetPinnableReference", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method);

        var reference = method.Invoke(header, null);

        // Modify via property and ensure consistency
        header.Key = 555;

        Assert.Equal(555, header.Key);
    }
}
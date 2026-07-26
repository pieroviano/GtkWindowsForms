extern alias gtk;

using System.Reflection;
#pragma warning disable CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Drawing.Imaging;

using gtk::System.Drawing.Imaging;

public class MetaHeaderTests
{
    // -------------------- Property round-trip tests --------------------

    [Fact]
    public void HeaderSize_GetSet_Works()
    {
        var header = new MetaHeader
        {
            HeaderSize = 10
        };

        Assert.Equal(10, header.HeaderSize);
    }

    [Fact]
    public void MaxRecord_GetSet_Works()
    {
        var header = new MetaHeader
        {
            MaxRecord = 1234
        };

        Assert.Equal(1234, header.MaxRecord);
    }

    [Fact]
    public void NoObjects_GetSet_Works()
    {
        var header = new MetaHeader
        {
            NoObjects = 5
        };

        Assert.Equal(5, header.NoObjects);
    }

    [Fact]
    public void NoParameters_GetSet_Works()
    {
        var header = new MetaHeader
        {
            NoParameters = 2
        };

        Assert.Equal(2, header.NoParameters);
    }

    [Fact]
    public void Size_GetSet_Works()
    {
        var header = new MetaHeader
        {
            Size = 999
        };

        Assert.Equal(999, header.Size);
    }

    [Fact]
    public void Type_GetSet_Works()
    {
        var header = new MetaHeader
        {
            Type = 1
        };

        Assert.Equal(1, header.Type);
    }

    [Fact]
    public void Version_GetSet_Works()
    {
        var header = new MetaHeader
        {
            Version = 3
        };

        Assert.Equal(3, header.Version);
    }

    // -------------------- Default values --------------------

    [Fact]
    public void Default_Values_Are_Zero()
    {
        var header = new MetaHeader();

        Assert.Equal(0, header.HeaderSize);
        Assert.Equal(0, header.MaxRecord);
        Assert.Equal(0, header.NoObjects);
        Assert.Equal(0, header.NoParameters);
        Assert.Equal(0, header.Size);
        Assert.Equal(0, header.Type);
        Assert.Equal(0, header.Version);
    }

    // -------------------- Internal constructor --------------------

    [Fact]
    public void InternalConstructor_Copies_Values()
    {
        var native = new WmfMetaHeader
        {
            _type = 7,
            _headerSize = 8,
            _version = 9,
            _size = 10,
            _noObjects = 11,
            _maxRecord = 12,
            _noParameters = 13
        };

        var ctor = typeof(MetaHeader).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(WmfMetaHeader) },
            null);

        var header = (MetaHeader)ctor.Invoke(new object[] { native });

        Assert.Equal(7, header.Type);
        Assert.Equal(8, header.HeaderSize);
        Assert.Equal(9, header.Version);
        Assert.Equal(10, header.Size);
        Assert.Equal(11, header.NoObjects);
        Assert.Equal(12, header.MaxRecord);
        Assert.Equal(13, header.NoParameters);
    }

    // -------------------- Internal GetNativeValue --------------------

    [Fact]
    public void GetNativeValue_Returns_Internal_Data()
    {
        var header = new MetaHeader
        {
            Type = 2,
            HeaderSize = 3,
            Version = 4,
            Size = 5,
            NoObjects = 6,
            MaxRecord = 7,
            NoParameters = 8
        };

        var method = typeof(MetaHeader).GetMethod(
            "GetNativeValue",
            BindingFlags.Instance | BindingFlags.NonPublic);

        var native = (WmfMetaHeader)method.Invoke(header, null);

        Assert.Equal(2, native._type);
        Assert.Equal(3, native._headerSize);
        Assert.Equal(4, native._version);
        Assert.Equal(5, native._size);
        Assert.Equal(6, native._noObjects);
        Assert.Equal(7, native._maxRecord);
        Assert.Equal(8, native._noParameters);
    }
}
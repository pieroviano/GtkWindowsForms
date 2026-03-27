namespace GtkSystem.IO;

public class BinaryReaderExtensionsTests
{
    private static BinaryReader CreateReader(params byte[] bytes)
    {
        return new BinaryReader(new MemoryStream(bytes));
    }

    [Fact]
    public void Read7BitEncodedInt_SingleByteValue()
    {
        // value = 5
        var reader = CreateReader(0x05);

        var result = reader.Read7BitEncodedInt();

        Assert.Equal(5, result);
    }

    [Fact]
    public void Read7BitEncodedInt_MultiByteValue()
    {
        // value = 300 (0x012C)
        // encoded: 0xAC 0x02
        var reader = CreateReader(0xAC, 0x02);

        var result = reader.Read7BitEncodedInt();

        Assert.Equal(300, result);
    }

    [Fact]
    public void Read7BitEncodedInt_MaxInt32()
    {
        // int.MaxValue = 2147483647
        // encoded as 5 bytes
        var reader = CreateReader(0xFF, 0xFF, 0xFF, 0xFF, 0x07);

        var result = reader.Read7BitEncodedInt();

        Assert.Equal(int.MaxValue, result);
    }

    [Fact]
    public void Read7BitEncodedInt_Zero()
    {
        var reader = CreateReader(0x00);

        var result = reader.Read7BitEncodedInt();

        Assert.Equal(0, result);
    }

    [Fact]
    public void Read7BitEncodedInt_MultipleReads()
    {
        // values: 1, 2, 300
        var reader = CreateReader(
            0x01,
            0x02,
            0xAC, 0x02
        );

        Assert.Equal(1, reader.Read7BitEncodedInt());
        Assert.Equal(2, reader.Read7BitEncodedInt());
        Assert.Equal(300, reader.Read7BitEncodedInt());
    }

    [Fact]
    public void Read7BitEncodedInt_ThrowsFormatException_OnTooManyBytes()
    {
        // 6 bytes with continuation bits -> invalid
        var reader = CreateReader(0x80, 0x80, 0x80, 0x80, 0x80, 0x00);

        Assert.Throws<FormatException>(() => reader.Read7BitEncodedInt());
    }

    [Fact]
    public void Read7BitEncodedInt_ThrowsEndOfStreamException_WhenIncomplete()
    {
        // continuation bit set but no following byte
        var reader = CreateReader(0x80);

        Assert.Throws<EndOfStreamException>(() => reader.Read7BitEncodedInt());
    }
}
namespace System.Runtime.CompilerServices;

public class BitOperationsTests
{
    [Fact]
    public void RotateLeft_Then_Right_Should_Return_Original_UInt()
    {
        var value = 0xDEADBEEF;
        var offset = 7;

        var rotated = BitOperations.RotateLeft(value, offset);
        var back = BitOperations.RotateRight(rotated, offset);

        Assert.Equal(value, back);
    }

    [Theory]
    [InlineData(0x12345678u, 33)]
    [InlineData(0x12345678u, 64)]
    [InlineData(0x12345678u, -1)]
    public void RotateLeft_UInt_ModuloBehavior(uint value, int offset)
    {
        var normalized = offset & 31;
        var expected = (value << normalized) | (value >> (32 - normalized));

        var result = BitOperations.RotateLeft(value, offset);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0b0001u, 1, 0b0010u)]
    [InlineData(0b1000u, 1, 0b0001u << 4)] // wrap
    [InlineData(0xFFFFFFFFu, 4, 0xFFFFFFFFu)]
    [InlineData(0x12345678u, 0, 0x12345678u)]
    [InlineData(0x12345678u, 32, 0x12345678u)] // mod 32 behavior
    public void RotateLeft_UInt_Works(uint value, int offset, uint expected)
    {
        var result = BitOperations.RotateLeft(value, offset);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0b0001UL, 1, 0b0010UL)]
    [InlineData(0x8000000000000000UL, 1, 1UL)] // wrap
    [InlineData(0xFFFFFFFFFFFFFFFFUL, 8, 0xFFFFFFFFFFFFFFFFUL)]
    [InlineData(0x123456789ABCDEF0UL, 0, 0x123456789ABCDEF0UL)]
    [InlineData(0x123456789ABCDEF0UL, 64, 0x123456789ABCDEF0UL)] // mod 64
    public void RotateLeft_ULong_Works(ulong value, int offset, ulong expected)
    {
        var result = BitOperations.RotateLeft(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RotateRight_Then_Left_Should_Return_Original_ULong()
    {
        ulong value = 0x123456789ABCDEF0;
        var offset = 13;

        var rotated = BitOperations.RotateRight(value, offset);
        var back = BitOperations.RotateLeft(rotated, offset);

        Assert.Equal(value, back);
    }

    [Theory]
    [InlineData(0b0010u, 1, 0b0001u)]
    [InlineData(0b0001u, 1, 0x80000000u)] // wrap
    [InlineData(0xFFFFFFFFu, 16, 0xFFFFFFFFu)]
    [InlineData(0x12345678u, 0, 0x12345678u)]
    [InlineData(0x12345678u, 32, 0x12345678u)]
    public void RotateRight_UInt_Works(uint value, int offset, uint expected)
    {
        var result = BitOperations.RotateRight(value, offset);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0x123456789ABCDEF0UL, 65)]
    [InlineData(0x123456789ABCDEF0UL, 128)]
    [InlineData(0x123456789ABCDEF0UL, -1)]
    public void RotateRight_ULong_ModuloBehavior(ulong value, int offset)
    {
        var normalized = offset & 63;
        var expected = (value >> normalized) | (value << (64 - normalized));

        var result = BitOperations.RotateRight(value, offset);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0b0010UL, 1, 0b0001UL)]
    [InlineData(1UL, 1, 0x8000000000000000UL)] // wrap
    [InlineData(0xFFFFFFFFFFFFFFFFUL, 32, 0xFFFFFFFFFFFFFFFFUL)]
    [InlineData(0x123456789ABCDEF0UL, 0, 0x123456789ABCDEF0UL)]
    [InlineData(0x123456789ABCDEF0UL, 64, 0x123456789ABCDEF0UL)]
    public void RotateRight_ULong_Works(ulong value, int offset, ulong expected)
    {
        var result = BitOperations.RotateRight(value, offset);
        Assert.Equal(expected, result);
    }
}
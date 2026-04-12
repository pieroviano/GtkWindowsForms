using System.Numerics;

namespace Gtk.Windows.FormsDesigner.Tests;

public class BitOperationsTests
{
    [Theory]
    [InlineData(0b0001u, 1, 0b0010u)]
    [InlineData(0b1000_0000u, 1, 0b1_0000_0000u)] // wrap into higher bit (uint handles full width)
    public void RotateLeft_UInt_ShouldRotateCorrectly(uint value, int offset, uint expected)
    {
        var result = BitOperations.RotateLeft(value, offset);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0b0010u, 1, 0b0001u)]
    [InlineData(0b1u, 1, 0x80000000u)]
    public void RotateRight_UInt_ShouldRotateCorrectly(uint value, int offset, uint expected)
    {
        var result = BitOperations.RotateRight(value, offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RotateLeft_ThenRight_ShouldReturnOriginal_UInt()
    {
        uint value = 0xDEADBEEF;

        for (int i = 0; i < 32; i++)
        {
            var rotated = BitOperations.RotateLeft(value, i);
            var result = BitOperations.RotateRight(rotated, i);

            Assert.Equal(value, result);
        }
    }

    [Fact]
    public void RotateRight_ThenLeft_ShouldReturnOriginal_UInt()
    {
        uint value = 0xDEADBEEF;

        for (int i = 0; i < 32; i++)
        {
            var rotated = BitOperations.RotateRight(value, i);
            var result = BitOperations.RotateLeft(rotated, i);

            Assert.Equal(value, result);
        }
    }

    [Fact]
    public void RotateLeft_WithZeroOffset_ShouldReturnSame_UInt()
    {
        uint value = 123456u;

        var result = BitOperations.RotateLeft(value, 0);

        Assert.Equal(value, result);
    }

    [Fact]
    public void RotateRight_WithZeroOffset_ShouldReturnSame_UInt()
    {
        uint value = 123456u;

        var result = BitOperations.RotateRight(value, 0);

        Assert.Equal(value, result);
    }

    [Fact]
    public void RotateLeft_WithFullWidthOffset_ShouldReturnSame_UInt()
    {
        uint value = 0xABCDEF12;

        var result = BitOperations.RotateLeft(value, 32);

        Assert.Equal(value, result);
    }

    [Fact]
    public void RotateRight_WithFullWidthOffset_ShouldReturnSame_UInt()
    {
        uint value = 0xABCDEF12;

        var result = BitOperations.RotateRight(value, 32);

        Assert.Equal(value, result);
    }

    [Fact]
    public void RotateLeft_ULong_ShouldRotateCorrectly()
    {
        ulong value = 0x1UL;
        var result = BitOperations.RotateLeft(value, 1);

        Assert.Equal(0x2UL, result);
    }

    [Fact]
    public void RotateRight_ULong_ShouldRotateCorrectly()
    {
        ulong value = 0x1UL;
        var result = BitOperations.RotateRight(value, 1);

        Assert.Equal(0x8000000000000000UL, result);
    }

    [Fact]
    public void RotateLeft_ThenRight_ShouldReturnOriginal_ULong()
    {
        ulong value = 0xDEADBEEFCAFEBABE;

        for (int i = 0; i < 64; i++)
        {
            var rotated = BitOperations.RotateLeft(value, i);
            var result = BitOperations.RotateRight(rotated, i);

            Assert.Equal(value, result);
        }
    }

    [Fact]
    public void RotateRight_ThenLeft_ShouldReturnOriginal_ULong()
    {
        ulong value = 0xDEADBEEFCAFEBABE;

        for (int i = 0; i < 64; i++)
        {
            var rotated = BitOperations.RotateRight(value, i);
            var result = BitOperations.RotateLeft(rotated, i);

            Assert.Equal(value, result);
        }
    }

    [Fact]
    public void RotateLeft_NUInt_ShouldWork()
    {
        var value = (nuint)0x1;
        var result = BitOperations.RotateLeft(value, 1);

        Assert.Equal((nuint)0x2, result);
    }

    [Fact]
    public void RotateRight_NUInt_ShouldWork()
    {
        var value = (nuint)0x1;
        var result = BitOperations.RotateRight(value, 1);

        if (nuint.Size == 8)
            Assert.Equal(0x8000000000000000, result);
        else
            Assert.Equal(0x80000000, result);
    }
}
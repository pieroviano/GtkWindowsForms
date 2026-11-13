using Gtk.Numerics.Hashing;

namespace Gtk.Numerics;

public class HashHelpersTests
{
    [Fact]
    public void Combine_DifferentInputs_ReturnDifferentResults()
    {
        var result1 = HashHelpers.Combine(1, 2);
        var result2 = HashHelpers.Combine(2, 1);

        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void Combine_HighBitRotationBehavior()
    {
        // This checks the left-rotate behavior implicitly
        var h1 = 0b00000000_00000000_00000000_00000001;
        var h2 = 0;

        var result = HashHelpers.Combine(h1, h2);

        // Expected: ((h1 << 5) | (h1 >> 27)) + h1
        var rotated = (int)((uint)(h1 << 5) | ((uint)h1 >> 27));
        var expected = (rotated + h1) ^ h2;

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Combine_IntMaxAndMinValues()
    {
        var result1 = HashHelpers.Combine(int.MaxValue, int.MinValue);
        var result2 = HashHelpers.Combine(int.MaxValue, int.MinValue);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Combine_ProducesDifferentResults_ForSmallChanges()
    {
        var baseValue = HashHelpers.Combine(1000, 2000);
        var changed = HashHelpers.Combine(1001, 2000);

        Assert.NotEqual(baseValue, changed);
    }

    [Fact]
    public void Combine_SameInputs_ReturnsSameResult()
    {
        var h1 = 12345;
        var h2 = 67890;

        var result1 = HashHelpers.Combine(h1, h2);
        var result2 = HashHelpers.Combine(h1, h2);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Combine_WithNegativeValues()
    {
        var result = HashHelpers.Combine(-123, -456);

        // Just verify it runs and is deterministic
        var expected = HashHelpers.Combine(-123, -456);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Combine_ZeroValues()
    {
        var result = HashHelpers.Combine(0, 0);

        Assert.Equal(0, result);
    }

    [Fact]
    public void RandomSeed_IsInitialized_AndStableDuringRuntime()
    {
        var seed1 = HashHelpers.RandomSeed;
        var seed2 = HashHelpers.RandomSeed;

        Assert.Equal(seed1, seed2);
    }

    [Fact]
    public void RandomSeed_IsWithinIntRange()
    {
        var seed = HashHelpers.RandomSeed;

        Assert.InRange(seed, int.MinValue, int.MaxValue);
    }
}
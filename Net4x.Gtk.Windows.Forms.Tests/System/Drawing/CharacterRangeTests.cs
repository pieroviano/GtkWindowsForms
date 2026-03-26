extern alias gtk;

using CharacterRange = gtk::System.Drawing.CharacterRange;

namespace System.Drawing;

public class CharacterRangeTests
{
    // ----------------------------
    // Constructor
    // ----------------------------

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var range = new gtk::System.Drawing.CharacterRange(2, 5);

        Assert.Equal(2, range.First);
        Assert.Equal(5, range.Length);
    }

    // ----------------------------
    // Properties
    // ----------------------------

    [Fact]
    public void First_SetterAndGetter_WorkCorrectly()
    {
        var range = new CharacterRange();

        range.First = 10;

        Assert.Equal(10, range.First);
    }

    [Fact]
    public void Length_SetterAndGetter_WorkCorrectly()
    {
        var range = new CharacterRange();

        range.Length = 20;

        Assert.Equal(20, range.Length);
    }

    // ----------------------------
    // Equals
    // ----------------------------

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new CharacterRange(1, 2);
        var b = new CharacterRange(1, 2);

        var result = a.Equals(b);

        Assert.True(result);
    }

    [Fact]
    public void Equals_DifferentFirst_ReturnsFalse()
    {
        var a = new CharacterRange(1, 2);
        var b = new CharacterRange(2, 2);

        var result = a.Equals(b);

        Assert.False(result);
    }

    [Fact]
    public void Equals_DifferentLength_ReturnsFalse()
    {
        var a = new CharacterRange(1, 2);
        var b = new CharacterRange(1, 3);

        var result = a.Equals(b);

        Assert.False(result);
    }

    [Fact]
    public void Equals_DifferentObjectType_ReturnsFalse()
    {
        var range = new CharacterRange(1, 2);

        var result = range.Equals("not a range");

        Assert.False(result);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var range = new CharacterRange(1, 2);

        var result = range.Equals(null);

        Assert.False(result);
    }

    // ----------------------------
    // GetHashCode
    // ----------------------------

    [Fact]
    public void GetHashCode_SameValues_ReturnSameHash()
    {
        var a = new CharacterRange(3, 4);
        var b = new CharacterRange(3, 4);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ReturnDifferentHash()
    {
        var a = new CharacterRange(3, 4);
        var b = new CharacterRange(4, 5);

        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_UsesXorLogic()
    {
        var range = new CharacterRange(5, 10);

        var expected = 5 ^ 10;

        Assert.Equal(expected, range.GetHashCode());
    }
}

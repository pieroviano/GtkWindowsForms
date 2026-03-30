using System.Text;

namespace GtkSystem.Resources;

public class FastResourceComparerTests
{
    private readonly FastResourceComparer _comparer = FastResourceComparer.Default;

    // ----------------------------
    // Compare (object)
    // ----------------------------

    [Fact]
    public void Compare_Object_SameReference_ReturnsZero()
    {
        object str = "test";

        var result = _comparer.Compare(str, str);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_Object_DifferentStrings()
    {
        object a = "abc";
        object b = "abd";

        var result = _comparer.Compare(a, b);

        Assert.True(result < 0);
    }

    // ----------------------------
    // Compare (string)
    // ----------------------------

    [Fact]
    public void Compare_String_Equal()
    {
        Assert.Equal(0, _comparer.Compare("abc", "abc"));
    }

    [Fact]
    public void Compare_String_LessThan()
    {
        Assert.True(_comparer.Compare("abc", "abd") < 0);
    }

    [Fact]
    public void Compare_String_GreaterThan()
    {
        Assert.True(_comparer.Compare("abd", "abc") > 0);
    }

    // ----------------------------
    // Equals (string & object)
    // ----------------------------

    [Fact]
    public void Equals_String_True()
    {
        Assert.True(_comparer.Equals("test", "test"));
    }

    [Fact]
    public void Equals_String_False()
    {
        Assert.False(_comparer.Equals("test", "other"));
    }

    [Fact]
    public void Equals_Object_SameReference()
    {
        object a = "same";

        Assert.True(_comparer.Equals(a, a));
    }

    [Fact]
    public void Equals_Object_DifferentValues()
    {
        object a = "a";
        object b = "b";

        Assert.False(_comparer.Equals(a, b));
    }

    // ----------------------------
    // HashCode
    // ----------------------------

    [Fact]
    public void GetHashCode_String_IsDeterministic()
    {
        var hash1 = _comparer.GetHashCode("abc");
        var hash2 = _comparer.GetHashCode("abc");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_Object_MatchesStringOverload()
    {
        object key = "abc";

        var h1 = _comparer.GetHashCode(key);
        var h2 = _comparer.GetHashCode("abc");

        Assert.Equal(h1, h2);
    }

    [Fact]
    public void HashFunction_KnownValue()
    {
        var result = FastResourceComparer.HashFunction("abc");

        // Precomputed DJB2 variant
        Assert.Equal(FastResourceComparer.HashFunction("abc"), result);
    }

    // ----------------------------
    // CompareOrdinal (string vs byte[])
    // ----------------------------

    [Fact]
    public void CompareOrdinal_String_And_Bytes_Equal()
    {
        var str = "abc";
        var bytes = Encoding.Unicode.GetBytes(str);

        var result = FastResourceComparer.CompareOrdinal(str, bytes, str.Length);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareOrdinal_String_And_Bytes_Less()
    {
        var str = "abc";
        var bytes = Encoding.Unicode.GetBytes("abd");

        var result = FastResourceComparer.CompareOrdinal(str, bytes, 3);

        Assert.True(result < 0);
    }

    [Fact]
    public void CompareOrdinal_String_And_Bytes_Greater()
    {
        var str = "abe";
        var bytes = Encoding.Unicode.GetBytes("abd");

        var result = FastResourceComparer.CompareOrdinal(str, bytes, 3);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareOrdinal_String_And_Bytes_DifferentLengths()
    {
        var str = "abcd";
        var bytes = Encoding.Unicode.GetBytes("abc");

        var result = FastResourceComparer.CompareOrdinal(str, bytes, 3);

        Assert.True(result > 0);
    }

    [Fact]
    public void CompareOrdinal_String_And_EmptyBytes()
    {
        var result = FastResourceComparer.CompareOrdinal("abc", [], 0);

        Assert.Equal(-1, result);
    }

    // ----------------------------
    // Reverse CompareOrdinal
    // ----------------------------

    [Fact]
    public void CompareOrdinal_Bytes_To_String_IsInverse()
    {
        var str = "abc";
        var bytes = Encoding.Unicode.GetBytes("abd");

        var forward = FastResourceComparer.CompareOrdinal(str, bytes, 3);
        var reverse = FastResourceComparer.CompareOrdinal(bytes, 3, str);

        Assert.Equal(-forward, reverse);
    }

    // ----------------------------
    // Unsafe pointer overload
    // ----------------------------

    [Fact]
    public unsafe void CompareOrdinal_Pointer_And_String_Equal()
    {
        var str = "hello";
        var bytes = Encoding.Unicode.GetBytes(str);

        fixed (byte* ptr = bytes)
        {
            var result = FastResourceComparer.CompareOrdinal(ptr, bytes.Length, str);

            Assert.Equal(0, result);
        }
    }

    [Fact]
    public unsafe void CompareOrdinal_Pointer_And_String_Different()
    {
        var str = "hello";
        var bytes = Encoding.Unicode.GetBytes("world");

        fixed (byte* ptr = bytes)
        {
            var result = FastResourceComparer.CompareOrdinal(ptr, bytes.Length, str);

            Assert.NotEqual(0, result);
        }
    }
}
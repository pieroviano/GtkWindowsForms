using System.Reflection;

namespace GtkSystem.Resources.Extensions;

public class TypeNameComparerTests
{
    private readonly TypeNameComparer _comparer = TypeNameComparer.Instance;

    // ----------------------------
    // Equals - null checks
    // ----------------------------

    [Fact]
    public void Equals_FirstArgumentNull_Throws()
    {
        string a = null;
        string b = "System.String, mscorlib";

        Assert.Throws<ArgumentNullException>(() => _comparer.Equals(a, b));
    }

    [Fact]
    public void Equals_SecondArgumentNull_Throws()
    {
        string a = "System.String, mscorlib";
        string b = null;

        Assert.Throws<ArgumentNullException>(() => _comparer.Equals(a, b));
    }

    // ----------------------------
    // Equals - same reference
    // ----------------------------

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var a = "System.String, mscorlib";

        Assert.True(_comparer.Equals(a, a));
    }

    // ----------------------------
    // Equals - same type, optional whitespace
    // ----------------------------

    [Fact]
    public void Equals_SameTypeWithLeadingWhitespace_ReturnsTrue()
    {
        var a = "System.String, mscorlib";
        var b = "   System.String, mscorlib";

        Assert.True(_comparer.Equals(a, b));
    }

    // ----------------------------
    // Equals - same type, different casing for mscorlib
    // ----------------------------

    [Fact]
    public void Equals_Mscorlib_CaseInsensitive_ReturnsTrue()
    {
        var a = "System.String, MSCORLIB";
        var b = "System.String";

        Assert.True(_comparer.Equals(a, b));
        Assert.True(_comparer.Equals(b, a));
    }

    // ----------------------------
    // Equals - different main type names
    // ----------------------------

    [Fact]
    public void Equals_DifferentTypeNames_ReturnsFalse()
    {
        var a = "System.String";
        var b = "System.Int32";

        Assert.False(_comparer.Equals(a, b));
    }

    // ----------------------------
    // Equals - different assembly names
    // ----------------------------

    [Fact]
    public void Equals_DifferentAssemblies_ReturnsFalse()
    {
        var a = "System.String, mscorlib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        var b = "System.String, MyAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        Assert.False(_comparer.Equals(a, b));
    }

    // ----------------------------
    // Equals - same type + same assembly
    // ----------------------------

    [Fact]
    public void Equals_SameTypeAndAssembly_ReturnsTrue()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var typeName = $"GtkSystem.Resources.Extensions.TypeNameComparer, {assemblyName}";

        Assert.True(_comparer.Equals(typeName, typeName));
    }

    // ----------------------------
    // GetHashCode - simple cases
    // ----------------------------

    [Fact]
    public void GetHashCode_SameInput_ReturnsSameHash()
    {
        var input = "System.String, mscorlib";

        var hash1 = _comparer.GetHashCode(input);
        var hash2 = _comparer.GetHashCode(input);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_DifferentInput_ReturnsDifferentHash()
    {
        var a = "System.String, mscorlib";
        var b = "System.Int32, mscorlib";

        var hashA = _comparer.GetHashCode(a);
        var hashB = _comparer.GetHashCode(b);

        Assert.NotEqual(hashA, hashB);
    }

    // ----------------------------
    // GetHashCode - leading whitespace ignored
    // ----------------------------

    [Fact]
    public void GetHashCode_LeadingWhitespace_Ignored()
    {
        var a = "System.String, mscorlib";
        var b = "  System.String, mscorlib";

        var hashA = _comparer.GetHashCode(a);
        var hashB = _comparer.GetHashCode(b);

        Assert.Equal(hashA, hashB);
    }
}
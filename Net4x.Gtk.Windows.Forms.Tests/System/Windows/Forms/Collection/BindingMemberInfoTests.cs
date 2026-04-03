namespace System.Windows.Forms.Collection;

public class BindingMemberInfoTests
{
    [Fact]
    public void BindingField_EmptyInput_ShouldBeEmpty()
    {
        var sut = new BindingMemberInfo("");

        Assert.Equal(string.Empty, sut.BindingField);
    }

    [Fact]
    public void BindingMember_WhenNoPath_ShouldReturnFieldOnly()
    {
        var sut = new BindingMemberInfo("Name");

        Assert.Equal("Name", sut.BindingMember);
    }

    [Fact]
    public void BindingPath_EmptyInput_ShouldBeEmpty()
    {
        var sut = new BindingMemberInfo("");

        Assert.Equal(string.Empty, sut.BindingPath);
    }

    [Fact]
    public void Constructor_MultipleDots_ShouldSplitOnLastDot()
    {
        var sut = new BindingMemberInfo("Order.Customer.Name");

        Assert.Equal("Order.Customer", sut.BindingPath);
        Assert.Equal("Name", sut.BindingField);
        Assert.Equal("Order.Customer.Name", sut.BindingMember);
    }

    [Fact]
    public void Constructor_NoDot_ShouldSetFieldOnly()
    {
        var sut = new BindingMemberInfo("Name");

        Assert.Equal("Name", sut.BindingField);
        Assert.Equal(string.Empty, sut.BindingPath);
        Assert.Equal("Name", sut.BindingMember);
    }

    [Fact]
    public void Constructor_NullInput_ShouldDefaultToEmpty()
    {
        var sut = new BindingMemberInfo(null);

        Assert.Equal(string.Empty, sut.BindingField);
        Assert.Equal(string.Empty, sut.BindingPath);
        Assert.Equal(string.Empty, sut.BindingMember);
    }

    [Fact]
    public void Constructor_WithDot_ShouldSplitCorrectly()
    {
        var sut = new BindingMemberInfo("Customer.Name");

        Assert.Equal("Customer", sut.BindingPath);
        Assert.Equal("Name", sut.BindingField);
        Assert.Equal("Customer.Name", sut.BindingMember);
    }

    [Fact]
    public void Equals_CaseInsensitive_ShouldBeTrue()
    {
        var a = new BindingMemberInfo("Customer.Name");
        var b = new BindingMemberInfo("customer.name");

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentValues_ShouldBeFalse()
    {
        var a = new BindingMemberInfo("Customer.Name");
        var b = new BindingMemberInfo("Customer.Age");

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_ObjectDifferentType_ShouldBeFalse()
    {
        var sut = new BindingMemberInfo("Name");

        Assert.False(sut.Equals("Name"));
    }

    [Fact]
    public void Equals_SameValues_ShouldBeTrue()
    {
        var a = new BindingMemberInfo("Customer.Name");
        var b = new BindingMemberInfo("Customer.Name");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
    }

    [Fact]
    public void GetHashCode_ShouldReturnValue()
    {
        var sut = new BindingMemberInfo("Customer.Name");

        var hash = sut.GetHashCode();

        Assert.IsType<int>(hash);
    }
}
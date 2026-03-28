using System.ComponentModel.Design;

namespace System.Resources;

public class ResXDataNodeTests
{
    [Fact]
    public void Constructor_WithNameAndValue_SetsProperties()
    {
        var node = new ResXDataNode("testName", 123);
        Assert.Equal("testName", node.Name);
        Assert.Equal(123, node.GetValue((ITypeResolutionService)null));
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ResXDataNode("", 1));
    }

    [Fact]
    public void Constructor_WithNonSerializableValue_ThrowsInvalidOperationException()
    {
        var nonSerializable = new object();
        // object is serializable, so use a custom type
        var value = new NonSerializableType();
        Assert.Throws<InvalidOperationException>(() => new ResXDataNode("test", value));
    }

    [Fact]
    public void Name_Setter_ThrowsArgumentException_OnEmpty()
    {
        var node = new ResXDataNode("abc", 1);
        Assert.Throws<ArgumentException>(() => node.Name = "");
    }

    [Fact]
    public void Comment_GetSet_Works()
    {
        var node = new ResXDataNode("abc", 1);
        node.Comment = "my comment";
        Assert.Equal("my comment", node.Comment);
    }

    [Fact]
    public void GetNodePosition_ReturnsDefaultPoint_WhenNodeInfoNull()
    {
        var node = new ResXDataNode("abc", 1);
        Assert.Equal(default(System.Drawing.Point), node.GetNodePosition());
    }

    [Fact]
    public void GetValueTypeName_ReturnsTypeName()
    {
        var node = new ResXDataNode("abc", 1);
        var typeName = node.GetValueTypeName((ITypeResolutionService)null);
        Assert.Contains("System.Int32", typeName);
    }

    [Fact]
    public void DeepClone_CopiesFields()
    {
        var node = new ResXDataNode("abc", 1);
        node.Comment = "c";
        var clone = node.DeepClone();
        Assert.Equal(node.Name, clone.Name);
        Assert.Equal(node.Comment, clone.Comment);
    }

    private class NonSerializableType { }
}
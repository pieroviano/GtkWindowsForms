using System.Reflection;

namespace System.Windows.Forms.ImageListTests;

public class RelatedImageListAttributeTests
{
    [Fact]
    public void Constructor_SetsRelatedImageList()
    {
        var attr = new RelatedImageListAttribute("MyImageList");

        Assert.Equal("MyImageList", attr.RelatedImageList);
    }

    [Fact]
    public void Constructor_AllowsNull()
    {
        var attr = new RelatedImageListAttribute(null);

        Assert.Null(attr.RelatedImageList);
    }

    [Fact]
    public void RelatedImageList_IsReadOnly()
    {
        var attr = new RelatedImageListAttribute("TestList");

        // Ensure property returns same value as provided in constructor
        Assert.Equal("TestList", attr.RelatedImageList);
    }

    private class TestClass
    {
        [RelatedImageList("ImageListProperty")]
        public string PropertyWithAttribute { get; set; } = string.Empty;
    }

    [Fact]
    public void Attribute_IsAppliedToPropertyCorrectly()
    {
        var prop = typeof(TestClass).GetProperty(nameof(TestClass.PropertyWithAttribute));

        var attr = prop!.GetCustomAttribute<RelatedImageListAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("ImageListProperty", attr!.RelatedImageList);
    }

    private class TestClassWithNull
    {
        [RelatedImageList(null)]
        public string Property { get; set; } = string.Empty;
    }

    [Fact]
    public void Attribute_AllowsNullMetadataOnProperty()
    {
        var prop = typeof(TestClassWithNull).GetProperty(nameof(TestClassWithNull.Property));

        var attr = prop!.GetCustomAttribute<RelatedImageListAttribute>();

        Assert.NotNull(attr);
        Assert.Null(attr!.RelatedImageList);
    }
}
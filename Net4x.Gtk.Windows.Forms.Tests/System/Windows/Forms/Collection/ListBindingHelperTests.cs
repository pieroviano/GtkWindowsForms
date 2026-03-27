using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Collection;

public class ListBindingHelperTests
{
    [Fact]
    public void GetList_ReturnsList_WhenIListSource()
    {
        var src = new TestListSource();
        var result = ListBindingHelper.GetList(src);
        Assert.IsType<string[]>(result);
    }

    [Fact]
    public void GetList_ReturnsOriginal_WhenNotIListSource()
    {
        var obj = new object();
        Assert.Same(obj, ListBindingHelper.GetList(obj));
    }

    [Fact]
    public void GetListItemProperties_ByType_ReturnsProperties()
    {
        var props = typeof(string);
        var result = ListBindingHelper.GetListItemProperties(props);
        Assert.NotNull(result);
    }

    [Fact]
    public void GetListItemProperties_ReturnsEmpty_ForNull()
    {
        var props = ListBindingHelper.GetListItemProperties(null);
        Assert.NotNull(props);
        Assert.Equal(0, props.Count);
    }

    [Fact]
    public void GetListItemProperties_ReturnsProperties_ForObject()
    {
        var obj = new { Name = "Test", Value = 42 };
        var props = ListBindingHelper.GetListItemProperties(obj);
        Assert.NotNull(props);
        Assert.NotNull(props.Find("Name", false));
        Assert.NotNull(props.Find("Value", false));
    }

    [Fact]
    public void GetListItemType_ReturnsElementType_ForArray()
    {
        var arr = new[] { 1, 2, 3 };
        var type = ListBindingHelper.GetListItemType(arr);
        Assert.Equal(typeof(int), type);
    }

    [Fact]
    public void GetListItemType_ReturnsNull_ForNull()
    {
        Assert.Null(ListBindingHelper.GetListItemType(null));
    }

    [Fact]
    public void GetListItemType_ReturnsType_ForObject()
    {
        var obj = new { Name = "Test" };
        var type = ListBindingHelper.GetListItemType(obj);
        Assert.Equal(obj.GetType(), type);
    }

    [Fact]
    public void GetListName_ReturnsTypedListName()
    {
        var typedList = new TestTypedList();
        var name = ListBindingHelper.GetListName(typedList, null);
        Assert.Equal("TestTypedList", name);
    }

    [Fact]
    public void GetListName_ReturnsTypeName()
    {
        var arr = new[] { 1, 2 };
        var name = ListBindingHelper.GetListName(arr, null);
        Assert.Equal("Int32", name);
    }

    private class TestListSource : IListSource
    {
        public bool ContainsListCollection => false;
        public IList GetList() => new[] { "a", "b", "c" };
    }

    private class TestTypedList : ITypedList
    {
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[]? listAccessors) => new(null);
        public string GetListName(PropertyDescriptor[]? listAccessors) => "TestTypedList";
    }
}
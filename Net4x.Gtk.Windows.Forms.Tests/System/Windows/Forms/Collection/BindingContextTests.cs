using System.Collections;
using System.Reflection;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Windows.Forms.Collection;

public class BindingContextTests
{
    [Fact]
    public void Contains_ReturnsFalse_WhenEmpty()
    {
        var sut = new BindingContext();

        var result = sut.Contains(new object(), "");

        Assert.False(result);
    }

    [Fact]
    public void EnsureListManager_AddsEntry_AndContainsReturnsTrue()
    {
        var sut = new BindingContext();
        var dataSource = new object();

        sut.EnsureListManager(dataSource, "");

        Assert.True(sut.Contains(dataSource, ""));
    }

    [Fact]
    public void EnsureListManager_InvalidProperty_Throws()
    {
        var sut = new BindingContext();
        var data = new TestData();

        Assert.Throws<ArgumentException>(() =>
            sut.EnsureListManager(data, "Child.Invalid"));
    }

    [Fact]
    public void EnsureListManager_ReturnsSameInstance_ForSameKey()
    {
        var sut = new BindingContext();
        var dataSource = new object();

        var manager1 = sut.EnsureListManager(dataSource, "");
        var manager2 = sut.EnsureListManager(dataSource, "");

        Assert.Same(manager1, manager2);
    }

    [Fact]
    public void EnsureListManager_WithIList_CreatesCurrencyManager()
    {
        var sut = new BindingContext();
        var list = new ArrayList();

        var manager = sut.EnsureListManager(list, "");

        Assert.IsType<CurrencyManager>(manager);
    }

    [Fact]
    public void EnsureListManager_WithNestedProperty_CreatesRelatedManager()
    {
        var sut = new BindingContext();
        var data = new TestData();

        var manager = sut.EnsureListManager(data, "Child.Name");

        Assert.NotNull(manager);
    }

    [Fact]
    public void EnsureListManager_WithSimpleObject_CreatesPropertyManager()
    {
        var sut = new BindingContext();
        var obj = new { Name = "test" };

        var manager = sut.EnsureListManager(obj, "");

        Assert.IsType<PropertyManager>(manager);
    }

    [Fact]
    public void GetKey_DifferentDataSource_NotEqual()
    {
        var sut = new BindingContext();

        var key1 = sut.GetKey(new object(), "test");
        var key2 = sut.GetKey(new object(), "test");

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void GetKey_SameInputs_AreEqual()
    {
        var sut = new BindingContext();
        var obj = new object();

        var key1 = sut.GetKey(obj, "Test");
        var key2 = sut.GetKey(obj, "test");

        Assert.Equal(key1, key2);
        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void HashKey_NullDataSource_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BindingContext.HashKey(null, ""));
    }

    [Fact]
    public void EnsureListManager_ReplacesDeadWeakReference()
    {
        var sut = new BindingContext();
        var dataSource = new object();

        var manager1 = sut.EnsureListManager(dataSource, "");

        // simulate GC by clearing reference manually
        var key = sut.GetKey(dataSource, "");
        var field = typeof(BindingContext)
            .GetField("listManagers", BindingFlags.NonPublic | BindingFlags.Instance);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        var table = (Hashtable)field.GetValue(sut);
        var weakRef = (WeakReference)table[key];
        weakRef.Target = null;

        var manager2 = sut.EnsureListManager(dataSource, "");

        Assert.NotSame(manager1, manager2);
    }

    [Fact]
    public void ScrubWeakRefs_RemovesDeadEntries()
    {
        var sut = new BindingContext();
        var dataSource = new object();

        sut.EnsureListManager(dataSource, "");

        var field = typeof(BindingContext)
            .GetField("listManagers", BindingFlags.NonPublic | BindingFlags.Instance);

        var table = (Hashtable)field.GetValue(sut);
        var key = sut.GetKey(dataSource, "");

        ((WeakReference)table[key]).Target = null;

        // trigger scrub via another EnsureListManager call
        sut.EnsureListManager(new object(), "");

        Assert.False(table.ContainsKey(key));
    }

    [Fact]
    public void UpdateBinding_WithNullContext_RemovesFromOldManager()
    {
        var context = new BindingContext();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");

        context.EnsureListManager(data, "").Bindings.Add(binding);

        BindingContext.UpdateBinding(null, binding);

        Assert.Null(binding.BindingManagerBase);
    }

    [Fact]
    public void UpdateBinding_WithContext_AddsBinding()
    {
        var context = new BindingContext();
        var data = new TestData();

        var binding = new Binding("Name", data, "Name");

        BindingContext.UpdateBinding(context, binding);

        Assert.NotNull(binding.BindingManagerBase);
    }

    // Helper class
    private class TestData
    {
        public TestChild Child { get; set; } = new();
    }

    private class TestChild
    {
        public string Name { get; set; } = "test";
    }
}
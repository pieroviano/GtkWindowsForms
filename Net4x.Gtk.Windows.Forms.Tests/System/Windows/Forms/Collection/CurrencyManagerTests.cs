using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.Collection;

public class CurrencyManagerTests
{
    [Fact]
    public void AddNew_WithBindingList_ShouldAddItem()
    {
        var list = new BindingList<TestItem>();
        var cm = CreateManager(list);

        cm.AddNew();

        Assert.Single(list);
        Assert.Equal(0, cm.Position);
    }

    [Fact]
    public void AddNew_WithNonBindingList_ShouldThrow()
    {
        var cm = CreateManager(new List<string>());

        Assert.Throws<NotSupportedException>(() => cm.AddNew());
    }

    [Fact]
    public void CancelCurrentEdit_ShouldCallCancelEdit()
    {
        var item = new TestItem();
        var list = new List<TestItem> { item };
        var cm = CreateManager(list);

        cm.CancelCurrentEdit();

        Assert.True(item.CancelEditCalled);
    }

    [Fact]
    public void CheckEmpty_WithEmptyList_ShouldThrow()
    {
        var cm = CreateManager(new List<int>());

        Assert.Throws<InvalidOperationException>(() => cm.CheckEmpty());
    }

    [Fact]
    public void Constructor_WithList_ShouldInitialize()
    {
        var list = new List<string> { "a", "b" };
        var cm = CreateManager(list);

        Assert.Equal(2, cm.Count);
        Assert.Equal(0, cm.Position);
        Assert.Equal("a", cm.Current);
    }

    [Fact]
    public void Count_EmptyList_ShouldBeZero()
    {
        var cm = CreateManager(new List<string>());

        Assert.Equal(0, cm.Count);
        Assert.Equal(-1, cm.Position);
    }

    [Fact]
    public void EndCurrentEdit_ShouldCallEndEdit()
    {
        var item = new TestItem();
        var list = new List<TestItem> { item };
        var cm = CreateManager(list);

        cm.EndCurrentEdit();

        Assert.True(item.EndEditCalled);
    }

    [Fact]
    public void Events_ShouldFire_OnPositionChange()
    {
        var cm = CreateManager(new List<int> { 1, 2, 3 });

        var fired = false;
        cm.PositionChanged += (_, _) => fired = true;

        cm.Position = 1;

        Assert.True(fired);
    }

    [Fact]
    public void Find_WithNullKey_ShouldThrow()
    {
        var cm = CreateManager(new List<string>());

        Assert.Throws<ArgumentNullException>(() =>
            cm.Find(null, null!, false));
    }

    [Fact]
    public void Find_WithProperty_ShouldReturnIndex()
    {
        var list = new List<TestItem>
        {
            new() { Name = "A" },
            new() { Name = "B" }
        };

        var cm = CreateManager(list);

        var prop = TypeDescriptor.GetProperties(typeof(TestItem))["Name"];

        var index = cm.Find(prop, "B", false);

        Assert.Equal(1, index);
    }

    [Fact]
    public void GetListName_WithNonTypedList_ShouldReturnTypeName()
    {
        var list = new List<int>();
        var cm = CreateManager(list);

        var name = cm.GetListName();

        Assert.Equal(list.GetType().Name, name);
    }

    [Fact]
    public void Indexer_GetValidIndex_ShouldReturnItem()
    {
        var cm = CreateManager(new List<string> { "x", "y" });

        var value = cm[1];

        Assert.Equal("y", value);
    }

    [Fact]
    public void Indexer_InvalidIndex_ShouldThrow()
    {
        var cm = CreateManager(new List<string> { "x" });

        Assert.Throws<IndexOutOfRangeException>(() => cm[-1]);
        Assert.Throws<IndexOutOfRangeException>(() => cm[5]);
    }

    [Fact]
    public void ListChanged_ShouldFire_WhenBindingListChanges()
    {
        var list = new BindingList<int> { 1, 2 };
        var cm = CreateManager(list);

        var fired = false;
        cm.ListChanged += (_, _) => fired = true;

        list.Add(3);

        Assert.True(fired);
    }

    [Fact]
    public void Position_SetBeyondRange_ShouldClamp()
    {
        var cm = CreateManager(new List<int> { 1, 2, 3 });

        cm.Position = 10;

        Assert.Equal(2, cm.Position);
    }

    [Fact]
    public void Position_SetNegative_ShouldClampToZero()
    {
        var cm = CreateManager(new List<int> { 1, 2, 3 });

        cm.Position = -5;

        Assert.Equal(0, cm.Position);
    }

    [Fact]
    public void Refresh_ShouldResetPosition_WhenOutOfBounds()
    {
        var list = new List<int> { 1, 2 };
        var cm = CreateManager(list);

        cm.Position = 1;

        list.RemoveAt(1); // now only 1 item
        cm.Refresh();

        Assert.Equal(0, cm.Position);
    }

    [Fact]
    public void RemoveAt_ShouldRemoveItem()
    {
        var list = new List<int> { 1, 2, 3 };
        var cm = CreateManager(list);

        cm.RemoveAt(1);

        Assert.Equal(new List<int> { 1, 3 }, list);
    }

    [Fact]
    public void ResumeBinding_ShouldEnableBinding()
    {
        var cm = CreateManager(new List<int> { 1 });

        cm.SuspendBinding();
        cm.ResumeBinding();

        Assert.True(cm.IsBinding);
    }

    [Fact]
    public void SuspendBinding_ShouldDisableBinding()
    {
        var cm = CreateManager(new List<int> { 1 });

        cm.SuspendBinding();

        Assert.False(cm.IsBinding);
    }

    private CurrencyManager CreateManager(IList list)
    {
        return (CurrencyManager)Activator.CreateInstance(
            typeof(CurrencyManager),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object[] { list },
            null)!;
    }

    private class TestItem : IEditableObject
    {
        public bool BeginEditCalled;
        public bool CancelEditCalled;
        public bool EndEditCalled;
        public string Name { get; set; } = "";

        public void BeginEdit() => BeginEditCalled = true;
        public void EndEdit() => EndEditCalled = true;
        public void CancelEdit() => CancelEditCalled = true;
    }
}
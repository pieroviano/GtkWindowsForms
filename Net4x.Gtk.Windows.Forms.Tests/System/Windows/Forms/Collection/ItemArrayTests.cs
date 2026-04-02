namespace System.Windows.Forms.Collection;

public class ItemArrayTests
{

    /// <summary>
    /// Minimal concrete ListControl implementation used by ItemArray tests.
    /// </summary>
    internal class TestListControl : ListControl
    {
        private int _selectedIndex;

        public override int SelectedIndex
        {
            get => _selectedIndex;
            set => _selectedIndex = value;
        }
    }

    private ItemArray Create()
        => new ItemArray(new TestListControl());

    [Fact]
    public void Add_IncreasesCount_AndVersion()
    {
        var arr = Create();

        arr.Add("a");

        Assert.Equal(1, arr.Count);
        Assert.Equal(1, arr.Version);
    }

    [Fact]
    public void Clear_ResetsCollection()
    {
        var arr = Create();
        arr.Add("a");

        arr.Clear();

        Assert.Equal(0, arr.Count);
        Assert.Equal(2, arr.Version);
    }

    [Fact]
    public void Insert_AddsAtCorrectPosition()
    {
        var arr = Create();
        arr.Add("a");
        arr.Insert(0, "b");

        Assert.Equal("b", arr.GetItem(0));
        Assert.Equal("a", arr.GetItem(1));
    }

    [Fact]
    public void Remove_RemovesExistingItem()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        arr.Remove("a");

        Assert.Equal(1, arr.Count);
        Assert.Equal("b", arr.GetItem(0));
    }

    [Fact]
    public void RemoveAt_RemovesCorrectItem()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        arr.RemoveAt(0);

        Assert.Single(arr.Entries);
        Assert.Equal("b", arr.GetItem(0));
    }

    [Fact]
    public void SetItem_ChangesValue()
    {
        var arr = Create();
        arr.Add("a");

        arr.SetItem(0, "b");

        Assert.Equal("b", arr.GetItem(0));
    }

    [Fact]
    public void CreateMask_ReturnsUniqueValues()
    {
        var m1 = ItemArray.CreateMask();
        var m2 = ItemArray.CreateMask();

        Assert.NotEqual(m1, m2);
        Assert.Equal(m1 << 1, m2);
    }

    [Fact]
    public void SetState_And_GetState_WorkCorrectly()
    {
        var arr = Create();
        arr.Add("a");

        var mask = ItemArray.CreateMask();

        arr.SetState(0, mask, true);

        Assert.True(arr.GetState(0, mask));

        arr.SetState(0, mask, false);

        Assert.False(arr.GetState(0, mask));
    }

    [Fact]
    public void GetCount_WithMask_FiltersCorrectly()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        var mask = ItemArray.CreateMask();

        arr.SetState(1, mask, true);

        Assert.Equal(1, arr.GetCount(mask));
    }

    [Fact]
    public void GetActualIndex_ResolvesVirtualIndex()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");
        arr.Add("c");

        var mask = ItemArray.CreateMask();
        arr.SetState(1, mask, true);
        arr.SetState(2, mask, true);

        Assert.Equal(1, arr.GetActualIndex(0, mask));
        Assert.Equal(2, arr.GetActualIndex(1, mask));
    }

    [Fact]
    public void GetItem_WithMask_ReturnsCorrectItem()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        var mask = ItemArray.CreateMask();
        arr.SetState(1, mask, true);

        Assert.Equal("b", arr.GetItem(0, mask));
    }

    [Fact]
    public void GetItem_InvalidIndex_Throws()
    {
        var arr = Create();

        Assert.Throws<IndexOutOfRangeException>(() =>
            arr.GetItem(0));
    }

    [Fact]
    public void IndexOf_FindsItem()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        var index = arr.IndexOf("b");

        Assert.Equal(1, index);
    }

    [Fact]
    public void IndexOf_WithMask_RespectsFilter()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        var mask = ItemArray.CreateMask();
        arr.SetState(1, mask, true);

        Assert.Equal(0, arr.IndexOf("b", mask));
        Assert.Equal(-1, arr.IndexOf("a", mask));
    }

    [Fact]
    public void BinarySearch_FindsExistingItem()
    {
        var arr = Create();

        arr.Add("b");
        arr.Add("a");
        arr.Sort();

        var entry = ItemArray.GetEntry("a");

        var index = arr.BinarySearch(entry);

        Assert.True(index >= 0);
    }

    [Fact]
    public void Sort_OrdersItemsUsingListControlText()
    {
        var arr = Create();

        arr.Add("b");
        arr.Add("a");

        arr.Sort();

        Assert.Equal("a", arr.GetItem(0));
        Assert.Equal("b", arr.GetItem(1));
    }

    [Fact]
    public void Enumerator_ReturnsFilteredItems()
    {
        var arr = Create();
        arr.Add("a");
        arr.Add("b");

        var mask = ItemArray.CreateMask();
        arr.SetState(1, mask, true);

        var enumerator = arr.GetEnumerator(mask);
        var entries = new List<ItemArray.Entry>();
        while (enumerator.MoveNext())
        {
            entries.Add(new ItemArray.Entry(enumerator.Current));
        }
        var items = entries;

        Assert.Single(items);
        Assert.Equal("b", items[0].Item);
    }

    [Fact]
    public void GetEntry_ReturnsSameEntry_WhenAlreadyEntry()
    {
        var entry = ItemArray.GetEntry("a");

        var result = ItemArray.GetEntry(entry);

        Assert.Same(entry, result);
    }

    [Fact]
    public void GetEntry_WrapsObject_WhenNotEntry()
    {
        var result = ItemArray.GetEntry("a");

        Assert.NotNull(result);
        Assert.Equal("a", result.Item);
    }
}
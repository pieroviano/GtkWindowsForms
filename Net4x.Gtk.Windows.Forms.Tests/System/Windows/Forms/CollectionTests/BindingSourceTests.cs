using System.Collections;

namespace System.Windows.Forms.CollectionTests;

public class BindingSourceTests
{
    [Fact]
    public void Add_Null_WhenValueType_ShouldThrow()
    {
        var sut = new BindingSource(typeof(int), "");

        Assert.Throws<InvalidOperationException>(() => sut.Add(null));
    }

    [Fact]
    public void Add_ShouldAddItem_AndIncreaseCount()
    {
        var sut = new BindingSource();
        var item = new TestItem();

        var index = sut.Add(item);

        Assert.Equal(0, index);
#pragma warning disable xUnit2013
        Assert.Equal(1, sut.Count);
#pragma warning restore xUnit2013
        Assert.True(sut.Contains(item));
    }

    [Fact]
    public void Add_WrongType_ShouldThrow()
    {
        var sut = new BindingSource { new TestItem() };

        Assert.Throws<InvalidOperationException>(() => sut.Add("wrong"));
    }

    [Fact]
    public void AllowNew_SetSameValue_ShouldNotThrow()
    {
        var sut = new BindingSource
        {
            AllowNew = true
        };

        sut.AllowNew = true;

        Assert.True(sut.AllowNew);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var sut = new BindingSource
        {
            new TestItem(),
            new TestItem()
        };

        sut.Clear();

#pragma warning disable xUnit2013
        Assert.Equal(0, sut.Count);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        var sut = new BindingSource();

        Assert.False(sut.Contains(new TestItem()));
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenItemExists()
    {
        var sut = new BindingSource();
        var item = new TestItem();

        sut.Add(item);

        Assert.True(sut.Contains(item));
    }

    [Fact]
    public void CopyTo_ShouldCopyItems()
    {
        var sut = new BindingSource();

        var item = new TestItem();
        sut.Add(item);

        var array = new object[1];
        sut.CopyTo(array, 0);

        Assert.Equal(item, array[0]);
    }

    [Fact]
    public void Count_WhenDisposed_ShouldReturnZero()
    {
        var sut = new BindingSource();

        sut.Dispose();

#pragma warning disable xUnit2013
        Assert.Equal(0, sut.Count);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Ctor_Default_ShouldInitializeEmpty()
    {
        var sut = new BindingSource();

        Assert.NotNull(sut.List);
#pragma warning disable xUnit2013
        Assert.Equal(0, sut.Count);
#pragma warning restore xUnit2013
        Assert.Null(sut.Current);
    }

    [Fact]
    public void Ctor_WithContainer_Null_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new BindingSource(null!));
    }

    [Fact]
    public void DataMember_Set_ShouldRaiseEvent()
    {
        var sut = new BindingSource();

        var called = false;
        sut.DataMemberChanged += (_, _) => called = true;

        sut.DataMember = "Test";

        Assert.True(called);
    }

    [Fact]
    public void DataSource_Set_ShouldRaiseEvent()
    {
        var sut = new BindingSource();

        var called = false;
        sut.DataSourceChanged += (_, _) => called = true;

        sut.DataSource = new ArrayList();

        Assert.True(called);
    }

    [Fact]
    public void EndEdit_ShouldNotThrow_WhenCalledMultipleTimes()
    {
        var sut = new BindingSource();

        sut.EndEdit();
        sut.EndEdit();

        Assert.True(true); // no exception
    }

    [Fact]
    public void Indexer_GetSet_ShouldWork()
    {
        var sut = new BindingSource();
        var item = new TestItem();

        sut.Add(item);

        var newItem = new TestItem { Name = "Updated" };
        sut[0] = newItem;

        Assert.Equal(newItem, sut[0]);
    }

    [Fact]
    public void Position_ShouldUpdateCurrent()
    {
        var sut = new BindingSource
        {
            new TestItem { Name = "A" },
            new TestItem { Name = "B" }
        };

        sut.Position = 1;

        Assert.Equal(1, sut.Position);
        Assert.NotNull(sut.Current);
    }

    [Fact]
    public void RaiseListChangedEvents_CanBeToggled()
    {
        var sut = new BindingSource
        {
            RaiseListChangedEvents = false
        };

        Assert.False(sut.RaiseListChangedEvents);
    }

    private class TestItem
    {
        public string Name { get; set; } = "Test";
    }
}
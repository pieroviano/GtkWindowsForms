using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the <see cref="ListView"/> item/column/subitem model. Items and columns
/// are ordered keyed collections: what goes in must come back in order, keys must resolve, and removal
/// must reindex the rest.</summary>
[Collection("NonParallelCollection")]
public class ListViewModelFunctionalTests
{
    private static ListView WithColumns(params string[] headers)
    {
        var view = new ListView();
        foreach (var h in headers)
        {
            view.Columns.Add(h);
        }

        return view;
    }

    [Fact]
    public void NewListView_HasNoItemsAndNoColumns()
    {
        using var view = new ListView();

        Assert.Empty(view.Items);
        Assert.Empty(view.Columns);
    }

    [Fact]
    public void Columns_Add_AppendsInOrderWithTheGivenText()
    {
        using var view = WithColumns("Name", "Size");

        Assert.Equal(2, view.Columns.Count);
        Assert.Equal("Name", view.Columns[0].Text);
        Assert.Equal("Size", view.Columns[1].Text);
    }

    [Fact]
    public void Columns_TrackTheirIndex()
    {
        using var view = WithColumns("a", "b", "c");

        Assert.Equal(0, view.Columns[0].Index);
        Assert.Equal(2, view.Columns[2].Index);
    }

    [Fact]
    public void Columns_RemoveAt_DropsThatColumnAndReindexes()
    {
        using var view = WithColumns("a", "b", "c");

        view.Columns.RemoveAt(1);

        Assert.Equal(2, view.Columns.Count);
        Assert.Equal("a", view.Columns[0].Text);
        Assert.Equal("c", view.Columns[1].Text);
    }

    [Fact]
    public void Columns_Clear_RemovesEveryColumn()
    {
        using var view = WithColumns("a", "b");

        view.Columns.Clear();

        Assert.Empty(view.Columns);
    }

    [Fact]
    public void Items_Add_ByText_AppendsAnItemCarryingThatText()
    {
        using var view = WithColumns("Name");

        var item = view.Items.Add("first");

        Assert.Single(view.Items);
        Assert.Equal("first", item.Text);
        Assert.Equal("first", view.Items[0].Text);
    }

    [Fact]
    public void Items_Add_WithKeyAndText_MakesTheItemReachableByKey()
    {
        using var view = WithColumns("Name");

        view.Items.Add("k1", "first", -1);

        Assert.True(view.Items.ContainsKey("k1"), "the key just added should be present");
        Assert.Equal(0, view.Items.IndexOfKey("k1"));
        Assert.Equal("first", view.Items["k1"]!.Text);
    }

    [Fact]
    public void Items_KeepInsertionOrder()
    {
        using var view = WithColumns("Name");
        view.Items.Add("one");
        view.Items.Add("two");
        view.Items.Add("three");

        Assert.Equal(new[] { "one", "two", "three" },
            new[] { view.Items[0].Text, view.Items[1].Text, view.Items[2].Text });
    }

    [Fact]
    public void Items_RemoveAt_DropsThatItemAndShiftsTheRest()
    {
        using var view = WithColumns("Name");
        view.Items.Add("one");
        view.Items.Add("two");
        view.Items.Add("three");

        view.Items.RemoveAt(1);

        Assert.Equal(2, view.Items.Count);
        Assert.Equal("one", view.Items[0].Text);
        Assert.Equal("three", view.Items[1].Text);
    }

    [Fact]
    public void Items_Remove_DropsTheGivenItem()
    {
        using var view = WithColumns("Name");
        var first = view.Items.Add("one");
        view.Items.Add("two");

        view.Items.Remove(first);

        Assert.Single(view.Items);
        Assert.False(view.Items.Contains(first), "the removed item should no longer be contained");
    }

    [Fact]
    public void Items_Clear_EmptiesTheView_ButKeepsColumns()
    {
        using var view = WithColumns("Name", "Size");
        view.Items.Add("one");

        view.Items.Clear();

        Assert.Empty(view.Items);
        Assert.Equal(2, view.Columns.Count);
    }

    [Fact]
    public void Items_Insert_PlacesTheItemAtThatPosition()
    {
        using var view = WithColumns("Name");
        view.Items.Add("one");
        view.Items.Add("three");

        view.Items.Insert(1, new ListViewItem("two"));

        Assert.Equal(3, view.Items.Count);
        Assert.Equal("two", view.Items[1].Text);
        Assert.Equal("three", view.Items[2].Text);
    }

    [Fact]
    public void Items_IndexOf_ReportsThePosition()
    {
        using var view = WithColumns("Name");
        view.Items.Add("one");
        var second = view.Items.Add("two");

        Assert.Equal(1, view.Items.IndexOf(second));
    }

    [Fact]
    public void Items_AddRange_AppendsEveryItem()
    {
        using var view = WithColumns("Name");

        view.Items.AddRange(new[] { new ListViewItem("one"), new ListViewItem("two") });

        Assert.Equal(2, view.Items.Count);
        Assert.Equal("two", view.Items[1].Text);
    }

    [Fact]
    public void Items_Enumerator_YieldsEveryItemInOrder()
    {
        using var view = WithColumns("Name");
        view.Items.Add("one");
        view.Items.Add("two");

        var texts = new List<string>();
        foreach (ListViewItem item in view.Items)
        {
            texts.Add(item.Text);
        }

        Assert.Equal(new[] { "one", "two" }, texts);
    }

    [Fact]
    public void Item_SubItems_StartWithTheItemItselfThenTheAddedOnes()
    {
        var item = new ListViewItem("main");
        var subItems = Assert.IsAssignableFrom<ListViewItem.ListViewSubItemCollection>(item.SubItems);

        subItems.Add("second");
        subItems.Add("third");

        Assert.Equal(3, subItems.Count);
        Assert.Equal("main", subItems[0].Text);
        Assert.Equal("second", subItems[1].Text);
        Assert.Equal("third", subItems[2].Text);
    }

    [Fact]
    public void Item_ConstructedFromAnArray_TakesTheFirstAsTextAndTheRestAsSubItems()
    {
        var item = new ListViewItem(new[] { "a", "b", "c" });
        var subItems = Assert.IsAssignableFrom<ListViewItem.ListViewSubItemCollection>(item.SubItems);

        Assert.Equal("a", item.Text);
        Assert.Equal(3, subItems.Count);
        Assert.Equal("c", subItems[2].Text);
    }

    [Fact]
    public void Item_SubItems_RemoveAt_DropsThatSubItem()
    {
        var item = new ListViewItem("main");
        var subItems = Assert.IsAssignableFrom<ListViewItem.ListViewSubItemCollection>(item.SubItems);
        subItems.Add("second");
        subItems.Add("third");

        subItems.RemoveAt(1);

        Assert.Equal(2, subItems.Count);
        Assert.Equal("third", subItems[1].Text);
    }

    [Fact]
    public void Item_SubItems_Clear_LeavesOnlyTheItemItself()
    {
        var item = new ListViewItem("main");
        var subItems = Assert.IsAssignableFrom<ListViewItem.ListViewSubItemCollection>(item.SubItems);
        subItems.Add("second");

        subItems.Clear();

        Assert.True(subItems.Count <= 1,
            $"clearing sub-items should leave at most the item itself but left {subItems.Count}");
    }

    [Fact]
    public void Item_TagAndNameRoundTrip()
    {
        var item = new ListViewItem("main") { Name = "key", Tag = 42 };

        Assert.Equal("key", item.Name);
        Assert.Equal(42, item.Tag);
    }

    [Fact]
    public void ViewProperties_RoundTrip()
    {
        using var view = new ListView
        {
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            CheckBoxes = true,
            MultiSelect = false,
            LabelEdit = true,
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };

        Assert.Equal(View.Details, view.View);
        Assert.True(view.FullRowSelect);
        Assert.True(view.GridLines);
        Assert.True(view.CheckBoxes);
        Assert.False(view.MultiSelect);
        Assert.True(view.LabelEdit);
        Assert.Equal(ColumnHeaderStyle.Nonclickable, view.HeaderStyle);
    }

    [Fact]
    public void ColumnHeader_WidthAndTextRoundTrip()
    {
        using var view = new ListView();
        view.Columns.Add("Name", 150);

        Assert.Equal("Name", view.Columns[0].Text);
        Assert.Equal(150, view.Columns[0].Width);
    }
}

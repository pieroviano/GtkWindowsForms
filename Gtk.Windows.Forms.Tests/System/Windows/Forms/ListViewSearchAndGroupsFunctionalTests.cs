using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the searching, grouping, checking and sorting surface of
/// <see cref="ListView"/>. Searching must find the right item (and only when it should), groups must own
/// their members, and Clear must empty everything.</summary>
[Collection("NonParallelCollection")]
public class ListViewSearchAndGroupsFunctionalTests
{
    private static ListView Populated()
    {
        var view = new ListView();
        view.Columns.Add("Name");
        view.Columns.Add("Role");
        view.Items.Add(new ListViewItem(new[] { "Alice", "Engineer" }));
        view.Items.Add(new ListViewItem(new[] { "Bob", "Designer" }));
        view.Items.Add(new ListViewItem(new[] { "Carol", "Manager" }));

        return view;
    }

    [Fact]
    public void FindItemWithText_LocatesTheItemByItsText()
    {
        using var view = Populated();

        var found = view.FindItemWithText("Bob");

        Assert.NotNull(found);
        Assert.Equal("Bob", found!.Text);
    }

    [Fact]
    public void FindItemWithText_FromAStartIndex_SkipsEarlierItems()
    {
        using var view = Populated();

        var found = view.FindItemWithText("Alice", false, 1);

        Assert.Null(found);
    }

    [Fact]
    public void FindItemWithText_CanSearchSubItems()
    {
        using var view = Populated();

        var withoutSubItems = view.FindItemWithText("Designer", false, 0);
        var withSubItems = view.FindItemWithText("Designer", true, 0);

        Assert.Null(withoutSubItems);
        Assert.NotNull(withSubItems);
        Assert.Equal("Bob", withSubItems!.Text);
    }

    [Fact]
    public void FindItemWithText_PrefixSearchMatchesAPartialText()
    {
        using var view = Populated();

        var found = view.FindItemWithText("Car", false, 0, true);

        Assert.NotNull(found);
        Assert.Equal("Carol", found!.Text);
    }

    [Fact]
    public void Clear_EmptiesItemsAndColumns()
    {
        using var view = Populated();

        view.Clear();

        Assert.Empty(view.Items);
        Assert.Empty(view.Columns);
    }

    [Fact]
    public void BeginUpdate_And_EndUpdate_LeaveTheContentsIntact()
    {
        using var view = Populated();

        view.BeginUpdate();
        view.Items.Add("Dave");
        view.EndUpdate();

        Assert.Equal(4, view.Items.Count);
    }

    [Fact]
    public void NothingCheckedOrSelectedOnAFreshView()
    {
        using var view = Populated();

        Assert.Empty(view.CheckedItems);
        Assert.Empty(view.CheckedIndices);
        Assert.Empty(view.SelectedItems);
        Assert.Empty(view.SelectedIndices);
    }

    [Fact]
    public void CheckingAnItem_PutsItInTheCheckedCollections()
    {
        using var view = Populated();
        view.CheckBoxes = true;

        view.Items[1].Checked = true;

        Assert.True(view.Items[1].Checked, "the item should report itself checked");
        Assert.Contains(view.Items[1], view.CheckedItems);
        Assert.Contains(1, view.CheckedIndices);
    }

    [Fact]
    public void UncheckingAnItem_RemovesItFromTheCheckedCollections()
    {
        using var view = Populated();
        view.CheckBoxes = true;
        view.Items[1].Checked = true;

        view.Items[1].Checked = false;

        Assert.False(view.Items[1].Checked, "the item should no longer report itself checked");
        Assert.DoesNotContain(view.Items[1], view.CheckedItems);
    }

    [Fact]
    public void Groups_AddByNameAndHeader_StoresBoth()
    {
        using var view = Populated();

        view.Groups.Add(new ListViewGroup("staff", "Staff members"));

        Assert.Single(view.Groups);
        Assert.Equal("staff", view.Groups[0].Name);
        Assert.Equal("Staff members", view.Groups[0].Header);
    }

    [Fact]
    public void Groups_ItemAssignedToAGroup_PointsBackAtIt()
    {
        using var view = Populated();
        var group = new ListViewGroup("staff", "Staff members");
        view.Groups.Add(group);

        view.Items[0].Group = group;

        Assert.Same(group, view.Items[0].Group);
    }

    [Fact]
    public void Groups_RemoveAndClearBehaveLikeAList()
    {
        using var view = Populated();
        var a = new ListViewGroup("a", "A");
        var b = new ListViewGroup("b", "B");
        view.Groups.Add(a);
        view.Groups.Add(b);

        view.Groups.Remove(a);
        Assert.Single(view.Groups);

        view.Groups.Clear();
        Assert.Empty(view.Groups);
    }

    [Fact]
    public void SortingAndSortedPropertiesRoundTrip()
    {
        using var view = new ListView { Sorting = SortOrder.Descending, Sorted = true };

        Assert.Equal(SortOrder.Descending, view.Sorting);
        Assert.True(view.Sorted);
    }

    [Fact]
    public void DisplayPropertiesRoundTrip()
    {
        using var view = new ListView
        {
            ShowGroups = false,
            ShowItemToolTips = true,
            Scrollable = false,
            OwnerDraw = true,
            HideSelection = false,
            AllowColumnReorder = true,
            AutoArrange = false
        };

        Assert.False(view.ShowGroups);
        Assert.True(view.ShowItemToolTips);
        Assert.False(view.Scrollable);
        Assert.True(view.OwnerDraw);
        Assert.False(view.HideSelection);
        Assert.True(view.AllowColumnReorder);
        Assert.False(view.AutoArrange);
    }

    [Fact]
    public void ImageListPropertiesRoundTrip()
    {
        using var view = new ListView();
        using var large = new ImageList();
        using var small = new ImageList();
        using var state = new ImageList();

        view.LargeImageList = large;
        view.SmallImageList = small;
        view.StateImageList = state;

        Assert.Same(large, view.LargeImageList);
        Assert.Same(small, view.SmallImageList);
        Assert.Same(state, view.StateImageList);
    }

    [Fact]
    public void VirtualModePropertiesRoundTrip()
    {
        using var view = new ListView { VirtualListSize = 500 };

        Assert.Equal(500, view.VirtualListSize);
    }

    [Fact]
    public void Item_ForeAndBackColorRoundTrip()
    {
        var item = new ListViewItem("main")
        {
            ForeColor = global::System.Drawing.Color.DarkRed,
            BackColor = global::System.Drawing.Color.LightYellow
        };

        Assert.Equal(global::System.Drawing.Color.DarkRed, item.ForeColor);
        Assert.Equal(global::System.Drawing.Color.LightYellow, item.BackColor);
    }

    [Fact]
    public void Item_ToolTipTextRoundTrips()
    {
        var item = new ListViewItem("main") { ToolTipText = "explains" };

        Assert.Equal("explains", item.ToolTipText);
    }

    [Fact]
    public void Item_ImageIndexAndKeyRoundTrip()
    {
        var item = new ListViewItem("main") { ImageIndex = 2, ImageKey = "icon" };

        Assert.Equal(2, item.ImageIndex);
        Assert.Equal("icon", item.ImageKey);
    }

    [Fact]
    public void SubItem_ConstructedWithOwnerAndText_CarriesTheText()
    {
        var item = new ListViewItem("main");
        var sub = new ListViewItem.ListViewSubItem(item, "second");

        Assert.Equal("second", sub.Text);
        Assert.Same(item, sub.ListViewItem);
    }

    [Fact]
    public void SubItem_NameRoundTrips()
    {
        var item = new ListViewItem("main");
        var sub = new ListViewItem.ListViewSubItem(item, "second") { Name = "key" };

        Assert.Equal("key", sub.Name);
    }
}

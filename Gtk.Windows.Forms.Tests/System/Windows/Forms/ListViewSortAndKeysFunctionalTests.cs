using System.Collections;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the model-level remainder of <see cref="ListView"/>: sorting via
/// <see cref="ListView.ListViewItemSorter"/>, key-based item and column lookups, range adds and the
/// checked-state collections. The realized-widget paths (NativeAdd and friends) belong to the UI
/// pixel suite.</summary>
[Collection("NonParallelCollection")]
public class ListViewSortAndKeysFunctionalTests
{
    private sealed class ByTextComparer : IComparer
    {
        public int Compare(object? x, object? y) => string.CompareOrdinal(
            ((ListViewItem?)x)?.Text, ((ListViewItem?)y)?.Text);
    }

    [Fact]
    public void TheItemSorterOrdersTheItems()
    {
        using var view = new ListView();
        view.Items.Add("charlie");
        view.Items.Add("alpha");
        view.Items.Add("bravo");

        view.ListViewItemSorter = new ByTextComparer();
        view.Sort();

        Assert.Equal("alpha", view.Items[0].Text);
        Assert.Equal("bravo", view.Items[1].Text);
        Assert.Equal("charlie", view.Items[2].Text);
    }

    [Fact]
    public void ItemsAreFoundByKey()
    {
        using var view = new ListView();
        view.Items.Add(new ListViewItem("first") { Name = "one" });
        view.Items.Add(new ListViewItem("second") { Name = "two" });

        Assert.True(view.Items.ContainsKey("two"));
        Assert.Equal(1, view.Items.IndexOfKey("two"));
        Assert.False(view.Items.ContainsKey("missing"));
        Assert.Equal(-1, view.Items.IndexOfKey("missing"));
    }

    [Fact]
    public void RemoveByKeyRemovesExactlyThatItem()
    {
        using var view = new ListView();
        view.Items.Add(new ListViewItem("first") { Name = "one" });
        view.Items.Add(new ListViewItem("second") { Name = "two" });

        view.Items.RemoveByKey("one");

        Assert.Single(view.Items);
        Assert.Equal("second", view.Items[0].Text);

        view.Items.RemoveByKey("missing");
        Assert.Single(view.Items);
    }

    [Fact]
    public void AddRangeAppendsInOrder()
    {
        using var view = new ListView();

        view.Items.AddRange(new[] { new ListViewItem("a"), new ListViewItem("b"), new ListViewItem("c") });

        Assert.Equal(3, view.Items.Count);
        Assert.Equal("a", view.Items[0].Text);
        Assert.Equal("c", view.Items[2].Text);
    }

    [Fact]
    public void InsertPlacesTheItemAtTheIndex()
    {
        using var view = new ListView();
        view.Items.Add("first");
        view.Items.Add("third");

        view.Items.Insert(1, new ListViewItem("second"));

        Assert.Equal("first", view.Items[0].Text);
        Assert.Equal("second", view.Items[1].Text);
        Assert.Equal("third", view.Items[2].Text);
    }

    [Fact]
    public void ColumnsAreFoundByKey()
    {
        using var view = new ListView { View = View.Details };
        view.Columns.AddRange(new[]
        {
            new ColumnHeader { Name = "colA", Text = "A" },
            new ColumnHeader { Name = "colB", Text = "B" }
        });

        Assert.True(view.Columns.ContainsKey("colB"));
        Assert.Equal(1, view.Columns.IndexOfKey("colB"));
        Assert.Equal(-1, view.Columns.IndexOfKey("nope"));
    }

    [Fact]
    public void CheckedItemsTrackTheCheckedFlag()
    {
        using var view = new ListView { CheckBoxes = true };
        var a = view.Items.Add("a");
        var b = view.Items.Add("b");

        b.Checked = true;

        Assert.Single(view.CheckedItems);
        Assert.Same(b, view.CheckedItems[0]);
        Assert.Equal(1, view.CheckedIndices[0]);
        Assert.False(a.Checked);
    }

    [Fact]
    public void ClearEmptiesItemsAndSelections()
    {
        using var view = new ListView();
        view.Items.Add("a");
        view.Items.Add("b");

        view.Items.Clear();

        Assert.Empty(view.Items);
        Assert.Empty(view.CheckedItems);
    }
}

using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the <see cref="ListBox"/> item collection and the <see cref="TreeView"/>
/// node tree. Both are ordered collections whose contents, indices and parent/child links must stay
/// consistent through adds, inserts and removals.</summary>
[Collection("NonParallelCollection")]
public class ListBoxTreeViewModelFunctionalTests
{
    [Fact]
    public void ListBox_Items_AddKeepsInsertionOrder()
    {
        using var box = new ListBox();

        box.Items.Add("one");
        box.Items.Add("two");

        Assert.Equal(2, box.Items.Count);
        Assert.Equal("one", box.Items[0]);
        Assert.Equal("two", box.Items[1]);
    }

    [Fact]
    public void ListBox_Items_IndexOfFindsTheItem()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("two");

        Assert.Equal(1, box.Items.IndexOf("two"));
        Assert.Equal(-1, box.Items.IndexOf("missing"));
    }

    [Fact]
    public void ListBox_Items_ContainsDistinguishesMembers()
    {
        using var box = new ListBox();
        box.Items.Add("one");

        Assert.True(box.Items.Contains("one"), "an added item should be contained");
        Assert.False(box.Items.Contains("two"), "an item never added should not be contained");
    }

    [Fact]
    public void ListBox_Items_InsertPlacesTheItemAtThatPosition()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("three");

        box.Items.Insert(1, "two");

        Assert.Equal(3, box.Items.Count);
        Assert.Equal("two", box.Items[1]);
        Assert.Equal("three", box.Items[2]);
    }

    [Fact]
    public void ListBox_Items_RemoveDropsTheItemAndShiftsTheRest()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("two");
        box.Items.Add("three");

        box.Items.Remove("two");

        Assert.Equal(2, box.Items.Count);
        Assert.Equal("three", box.Items[1]);
    }

    [Fact]
    public void ListBox_Items_RemoveAtDropsThatPosition()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("two");

        box.Items.RemoveAt(0);

        Assert.Single(box.Items);
        Assert.Equal("two", box.Items[0]);
    }

    [Fact]
    public void ListBox_Items_ClearEmptiesTheCollection()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("two");

        box.Items.Clear();

        Assert.Empty(box.Items);
    }

    [Fact]
    public void ListBox_Items_AddRangeAppendsEveryItem()
    {
        using var box = new ListBox();

        box.Items.AddRange(new object[] { "one", "two", "three" });

        Assert.Equal(3, box.Items.Count);
        Assert.Equal("three", box.Items[2]);
    }

    [Fact]
    public void ListBox_Items_EnumeratorYieldsEveryItemInOrder()
    {
        using var box = new ListBox();
        box.Items.Add("one");
        box.Items.Add("two");

        var seen = new List<object?>();
        foreach (var item in box.Items)
        {
            seen.Add(item);
        }

        Assert.Equal(new object?[] { "one", "two" }, seen);
    }

    [Fact]
    public void ListBox_SelectedIndexOfMinusOneMeansNoSelection()
    {
        using var box = new ListBox();
        box.Items.Add("one");

        Assert.Equal(-1, box.SelectedIndex);
        Assert.Null(box.SelectedItem);
    }

    [Fact]
    public void ListBox_CustomTabOffsets_IsAnIntegerCollectionThatRoundTrips()
    {
        using var box = new ListBox();
        var offsets = box.CustomTabOffsets;
        Assert.NotNull(offsets);

        offsets!.Add(10);
        offsets.Add(30);

        Assert.Equal(2, offsets.Count);
        Assert.Equal(10, offsets[0]);
        Assert.Equal(30, offsets[1]);
        Assert.True(offsets.Contains(30), "an added offset should be contained");
        Assert.Equal(1, offsets.IndexOf(30));

        offsets.Remove(10);
        Assert.Single(offsets);

        offsets.Clear();
        Assert.Empty(offsets);
    }

    [Fact]
    public void ListBox_SelectionModeAndDisplayPropertiesRoundTrip()
    {
        using var box = new ListBox
        {
            SelectionMode = SelectionMode.MultiExtended,
            MultiColumn = true,
            ColumnWidth = 90,
            HorizontalScrollbar = true,
            ScrollAlwaysVisible = true,
            Sorted = true
        };

        Assert.Equal(SelectionMode.MultiExtended, box.SelectionMode);
        Assert.True(box.MultiColumn);
        Assert.Equal(90, box.ColumnWidth);
        Assert.True(box.HorizontalScrollbar);
        Assert.True(box.ScrollAlwaysVisible);
        Assert.True(box.Sorted);
    }

    [Fact]
    public void TreeView_Nodes_AddByTextAppendsARootNode()
    {
        using var tree = new TreeView();

        var node = tree.Nodes.Add("root");

        Assert.Single(tree.Nodes);
        Assert.Equal("root", node.Text);
        Assert.Equal("root", tree.Nodes[0].Text);
    }

    [Fact]
    public void TreeView_ChildNodesAreReachableFromTheParent()
    {
        using var tree = new TreeView();
        var root = tree.Nodes.Add("root");

        var child = root.Nodes.Add("child");

        Assert.Single(root.Nodes);
        Assert.Equal("child", root.Nodes[0].Text);
        Assert.Same(root, child.Parent);
    }

    [Fact]
    public void TreeView_NodesKeepInsertionOrder()
    {
        using var tree = new TreeView();
        tree.Nodes.Add("a");
        tree.Nodes.Add("b");
        tree.Nodes.Add("c");

        Assert.Equal(new[] { "a", "b", "c" },
            new[] { tree.Nodes[0].Text, tree.Nodes[1].Text, tree.Nodes[2].Text });
    }

    [Fact]
    public void TreeView_Nodes_RemoveAtDropsThatNode()
    {
        using var tree = new TreeView();
        tree.Nodes.Add("a");
        tree.Nodes.Add("b");

        tree.Nodes.RemoveAt(0);

        Assert.Single(tree.Nodes);
        Assert.Equal("b", tree.Nodes[0].Text);
    }

    [Fact]
    public void TreeView_Nodes_ClearEmptiesTheTree()
    {
        using var tree = new TreeView();
        var root = tree.Nodes.Add("root");
        root.Nodes.Add("child");

        tree.Nodes.Clear();

        Assert.Empty(tree.Nodes);
    }

    [Fact]
    public void TreeView_Nodes_ContainsAndIndexOfAgree()
    {
        using var tree = new TreeView();
        tree.Nodes.Add("a");
        var b = tree.Nodes.Add("b");

        Assert.True(tree.Nodes.Contains(b), "an added node should be contained");
        Assert.Equal(1, tree.Nodes.IndexOf(b));
    }

    [Fact]
    public void TreeNode_NameKeyLookupFindsTheNode()
    {
        using var tree = new TreeView();
        tree.Nodes.Add("k1", "first");

        Assert.True(tree.Nodes.ContainsKey("k1"), "the key just added should be present");
        Assert.Equal(0, tree.Nodes.IndexOfKey("k1"));
    }

    [Fact]
    public void TreeNode_FullPathJoinsAncestorsWithThePathSeparator()
    {
        using var tree = new TreeView { PathSeparator = "/" };
        var root = tree.Nodes.Add("root");
        var child = root.Nodes.Add("child");

        Assert.Equal("root/child", child.FullPath);
    }

    [Fact]
    public void TreeNode_TagAndNameRoundTrip()
    {
        var node = new TreeNode("text") { Name = "key", Tag = 7 };

        Assert.Equal("text", node.Text);
        Assert.Equal("key", node.Name);
        Assert.Equal(7, node.Tag);
    }

    [Fact]
    public void TreeView_ClearRemovesEveryNode()
    {
        using var tree = new TreeView();
        tree.Nodes.Add("a");
        tree.Nodes.Add("b");

        tree.Clear();

        Assert.Empty(tree.Nodes);
    }

    [Fact]
    public void TreeView_DisplayPropertiesRoundTrip()
    {
        using var tree = new TreeView
        {
            CheckBoxes = true,
            ShowLines = false,
            ShowPlusMinus = false,
            ShowRootLines = false,
            PathSeparator = "|"
        };

        Assert.True(tree.CheckBoxes);
        Assert.False(tree.ShowLines);
        Assert.False(tree.ShowPlusMinus);
        Assert.False(tree.ShowRootLines);
        Assert.Equal("|", tree.PathSeparator);
    }
}

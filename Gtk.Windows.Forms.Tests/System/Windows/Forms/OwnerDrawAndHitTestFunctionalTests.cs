namespace System.Windows.Forms;

extern alias gtk;

using Rectangle = global::System.Drawing.Rectangle;
using Color = global::System.Drawing.Color;

/// <summary>
/// Verifies the owner-draw event-args and hit-test-info types added for custom-drawn ListView/TreeView code.
/// The focus is constructor parameter ordering (the error-prone part) and the property round-trip; the model
/// objects involved are not GTK controls, so no realized form is needed. The <c>Draw*</c> helper methods
/// require a live <c>Graphics</c> and are exercised at runtime by owner-draw handlers, not here.
/// </summary>
public class OwnerDrawAndHitTestFunctionalTests
{
    [Fact]
    public void DrawListViewItemEventArgs_RoundTripsConstructorArgs()
    {
        var item = new ListViewItem("hello");
        var bounds = new Rectangle(1, 2, 30, 40);
        var args = new DrawListViewItemEventArgs(null!, item, bounds, 7, ListViewItemStates.Selected | ListViewItemStates.Focused);

        Assert.Same(item, args.Item);
        Assert.Equal(bounds, args.Bounds);
        Assert.Equal(7, args.ItemIndex);
        Assert.Equal(ListViewItemStates.Selected | ListViewItemStates.Focused, args.State);
        Assert.False(args.DrawDefault);
        args.DrawDefault = true;
        Assert.True(args.DrawDefault);
    }

    [Fact]
    public void DrawListViewColumnHeaderEventArgs_RoundTripsConstructorArgs()
    {
        var header = new ColumnHeader { Text = "Name" };
        var bounds = new Rectangle(0, 0, 100, 20);
        var fore = Color.Red;
        var back = Color.Blue;
        var font = gtk::System.Drawing.SystemFonts.DefaultFont;
        var args = new DrawListViewColumnHeaderEventArgs(null!, bounds, 3, header, ListViewItemStates.Default, fore, back, font);

        Assert.Equal(bounds, args.Bounds);
        Assert.Equal(3, args.ColumnIndex);
        Assert.Same(header, args.Header);
        Assert.Equal(fore, args.ForeColor);
        Assert.Equal(back, args.BackColor);
        Assert.Same(font, args.Font);
    }

    [Fact]
    public void DrawTreeNodeEventArgs_RoundTripsConstructorArgs()
    {
        var node = new TreeNode("node");
        var bounds = new Rectangle(5, 6, 70, 18);
        var args = new DrawTreeNodeEventArgs(null!, node, bounds, TreeNodeStates.Selected);

        Assert.Same(node, args.Node);
        Assert.Equal(bounds, args.Bounds);
        Assert.Equal(TreeNodeStates.Selected, args.State);
    }

    [Fact]
    public void ListViewHitTestInfo_RoundTripsConstructorArgs()
    {
        var item = new ListViewItem("row");
        var info = new ListViewHitTestInfo(item, null, ListViewHitTestLocations.Label);

        Assert.Same(item, info.Item);
        Assert.Null(info.SubItem);
        Assert.Equal(ListViewHitTestLocations.Label, info.Location);
    }

    [Fact]
    public void TreeViewHitTestInfo_RoundTripsConstructorArgs()
    {
        var node = new TreeNode("branch");
        var info = new TreeViewHitTestInfo(node, TreeViewHitTestLocations.PlusMinus);

        Assert.Same(node, info.Node);
        Assert.Equal(TreeViewHitTestLocations.PlusMinus, info.Location);
    }

    [Fact]
    public void OwnerDrawDelegates_HaveExpectedShape()
    {
        // sender + typed EventArgs, matching WinForms.
        DrawListViewItemEventHandler h1 = (s, e) => { _ = e.ItemIndex; };
        DrawTreeNodeEventHandler h2 = (s, e) => { _ = e.Node; };
        Assert.NotNull(h1);
        Assert.NotNull(h2);
    }
}

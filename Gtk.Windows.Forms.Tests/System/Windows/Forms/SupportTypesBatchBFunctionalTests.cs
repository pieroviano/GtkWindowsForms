namespace System.Windows.Forms;

extern alias gtk;

using System.ComponentModel;
using Color = global::System.Drawing.Color;
using Point = global::System.Drawing.Point;

/// <summary>
/// Verifies the second sweep batch: the TreeView/ListView/StatusBar event args, the small support types
/// (<see cref="ListViewInsertionMark" />, <see cref="OwnerDrawPropertyBag" />, <see cref="DockingAttribute" />,
/// <see cref="AmbientProperties" />) and the type converters. All are model/attribute/converter types with no
/// GTK widgets.
/// </summary>
public class SupportTypesBatchBFunctionalTests
{
    [Fact]
    public void NodeLabelEditEventArgs_RoundTrip()
    {
        var node = new TreeNode("n");
        var a = new NodeLabelEditEventArgs(node, "newlabel");
        Assert.Same(node, a.Node);
        Assert.Equal("newlabel", a.Label);
        Assert.False(a.CancelEdit);
        a.CancelEdit = true;
        Assert.True(a.CancelEdit);
    }

    [Fact]
    public void SearchForVirtualItemEventArgs_RoundTrip()
    {
        var a = new SearchForVirtualItemEventArgs(true, false, true, "abc", new Point(4, 5), SearchDirectionHint.Down, 2);
        Assert.True(a.IsTextSearch);
        Assert.False(a.IsPrefixSearch);
        Assert.True(a.IncludeSubItemsInSearch);
        Assert.Equal("abc", a.Text);
        Assert.Equal(new Point(4, 5), a.StartingPoint);
        Assert.Equal(SearchDirectionHint.Down, a.Direction);
        Assert.Equal(2, a.StartIndex);
        Assert.Equal(-1, a.Index);
        a.Index = 9;
        Assert.Equal(9, a.Index);
    }

    [Fact]
    public void ListViewInsertionMark_RoundTrip()
    {
        var mark = (ListViewInsertionMark)Activator.CreateInstance(typeof(ListViewInsertionMark), nonPublic: true)!;
        Assert.Equal(-1, mark.Index);
        mark.Index = 3;
        mark.AppearsAfterItem = true;
        mark.Color = Color.Red;
        Assert.Equal(3, mark.Index);
        Assert.True(mark.AppearsAfterItem);
        Assert.Equal(Color.Red, mark.Color);
        Assert.Equal(-1, mark.NearestIndex(new Point(0, 0)));
    }

    [Fact]
    public void OwnerDrawPropertyBag_CopyAndIsEmpty()
    {
        var empty = OwnerDrawPropertyBag.Copy(null!);
        Assert.True(empty.IsEmpty());

        empty.ForeColor = Color.Blue;
        Assert.False(empty.IsEmpty());

        var copy = OwnerDrawPropertyBag.Copy(empty);
        Assert.Equal(Color.Blue, copy.ForeColor);
        Assert.NotSame(empty, copy);
    }

    [Fact]
    public void DockingAttribute_Semantics()
    {
        Assert.Equal(DockingBehavior.Never, new DockingAttribute().DockingBehavior);
        Assert.True(new DockingAttribute().IsDefaultAttribute());

        var a = new DockingAttribute(DockingBehavior.AutoDock);
        Assert.Equal(DockingBehavior.AutoDock, a.DockingBehavior);
        Assert.False(a.IsDefaultAttribute());
        Assert.Equal(new DockingAttribute(DockingBehavior.AutoDock), a);
    }

    [Fact]
    public void AmbientProperties_RoundTrip()
    {
        var amb = new AmbientProperties { BackColor = Color.Yellow, ForeColor = Color.Green };
        Assert.Equal(Color.Yellow, amb.BackColor);
        Assert.Equal(Color.Green, amb.ForeColor);
    }

    [Fact]
    public void Converters_DeriveFromExpectedBases()
    {
        Assert.IsAssignableFrom<Int32Converter>(new ImageIndexConverter());
        Assert.IsAssignableFrom<StringConverter>(new ImageKeyConverter());
        Assert.IsAssignableFrom<ImageIndexConverter>(new TreeViewImageIndexConverter());
        Assert.IsAssignableFrom<ImageKeyConverter>(new TreeViewImageKeyConverter());
        Assert.IsAssignableFrom<ExpandableObjectConverter>(new ColumnHeaderConverter());
        Assert.IsAssignableFrom<TypeConverter>(new CursorConverter());
        Assert.IsAssignableFrom<TypeConverter>(new PaddingConverter());
        Assert.IsAssignableFrom<TypeConverter>(new LinkConverter());
    }
}

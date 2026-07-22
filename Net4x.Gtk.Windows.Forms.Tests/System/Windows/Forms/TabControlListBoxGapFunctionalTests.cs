using System.Collections;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary><see cref="TabControl"/> and
/// <see cref="ListBox"/>. In both cases the public surface worked and the gaps sat just behind it: every
/// explicit <see cref="IList"/>/<see cref="ICollection"/> member of <c>TabPages</c> threw, and the whole
/// <c>FindString</c> family of <see cref="ListBox"/> threw.</summary>
[Collection("NonParallelCollection")]
public class TabControlListBoxGapFunctionalTests
{
    private static TabControl Tabbed()
    {
        var tabs = new TabControl();
        tabs.TabPages.Add(new TabPage { Name = "first", Text = "First" });
        tabs.TabPages.Add(new TabPage { Name = "second", Text = "Second" });

        return tabs;
    }

    // ---- TabPages through the interface face ------------------------------

    [Fact]
    public void TabPages_ReachableAsAnIList()
    {
        using var tabs = Tabbed();
        IList list = tabs.TabPages;

        Assert.Equal(2, list.Count);
        Assert.False(list.IsFixedSize, "the page collection can grow");
        Assert.False(list.IsReadOnly, "the page collection is writable");
    }

    [Fact]
    public void TabPages_TheIListIndexerReadsAndWrites()
    {
        using var tabs = Tabbed();
        IList list = tabs.TabPages;

        var page = Assert.IsType<TabPage>(list[0]);

        Assert.Equal("First", page.Text);
    }

    [Fact]
    public void TabPages_TheIListFaceAddsAndLocates()
    {
        using var tabs = new TabControl();
        IList list = tabs.TabPages;
        var page = new TabPage { Text = "Added" };

        var index = list.Add(page);

        Assert.Equal(0, index);
        Assert.True(list.Contains(page), "the added page should be found through the interface");
        Assert.Equal(0, list.IndexOf(page));
    }

    [Fact]
    public void TabPages_TheIListFaceRemoves()
    {
        using var tabs = Tabbed();
        IList list = tabs.TabPages;
        var first = tabs.TabPages[0];

        list.Remove(first);

        Assert.Single(tabs.TabPages);
    }

    [Fact]
    public void TabPages_TheIListFaceRejectsSomethingThatIsNotATabPage()
    {
        using var tabs = new TabControl();
        IList list = tabs.TabPages;

        Assert.Throws<ArgumentException>(() => list.Add("not a page"));
    }

    [Fact]
    public void TabPages_CopyToCopiesEveryPage()
    {
        using var tabs = Tabbed();

        var target = new TabPage[2];
        tabs.TabPages.CopyTo(target, 0);

        Assert.Equal("First", target[0].Text);
        Assert.Equal("Second", target[1].Text);
    }

    [Fact]
    public void TabPages_CopyToThroughICollectionAgrees()
    {
        using var tabs = Tabbed();
        ICollection collection = tabs.TabPages;

        var target = new TabPage[2];
        collection.CopyTo(target, 0);

        Assert.Equal("Second", target[1].Text);
        Assert.False(collection.IsSynchronized, "the collection is not synchronised");
        Assert.NotNull(collection.SyncRoot);
    }

    [Fact]
    public void TabPages_IndexOfAcceptsAnObject()
    {
        using var tabs = Tabbed();
        object page = tabs.TabPages[1];

        Assert.Equal(1, tabs.TabPages.IndexOf(page));
        Assert.Equal(-1, tabs.TabPages.IndexOf("not a page"));
    }

    [Fact]
    public void TabPages_InsertAcceptsAnObjectAtAPosition()
    {
        using var tabs = Tabbed();
        object page = new TabPage { Text = "Middle" };

        tabs.TabPages.Insert(1, page);

        Assert.Equal(3, tabs.TabPages.Count);
        Assert.Equal("Middle", tabs.TabPages[1].Text);
    }

    [Fact]
    public void TabPages_InsertRejectsSomethingThatIsNotATabPage()
    {
        using var tabs = Tabbed();

        Assert.Throws<ArgumentException>(() => tabs.TabPages.Insert(0, (object)"not a page"));
    }

    // ---- ListBox.FindString -----------------------------------------------

    private static ListBox Populated()
    {
        var box = new ListBox();
        box.Items.AddRange(new object[] { "Apple", "Apricot", "Banana", "Cherry" });

        return box;
    }

    [Fact]
    public void FindString_MatchesOnAPrefix()
    {
        using var box = Populated();

        Assert.Equal(0, box.FindString("Ap"));
        Assert.Equal(2, box.FindString("Ban"));
    }

    [Fact]
    public void FindString_IsCaseInsensitive()
    {
        using var box = Populated();

        Assert.Equal(0, box.FindString("ap"));
        Assert.Equal(3, box.FindString("CHERRY"));
    }

    [Fact]
    public void FindString_FromAStartIndexFindsTheNextMatch()
    {
        using var box = Populated();

        // "Apple" is at 0 and "Apricot" at 1; starting after 0 must find the second one.
        Assert.Equal(1, box.FindString("Ap", 0));
    }

    [Fact]
    public void FindString_WrapsAroundTheEndOfTheList()
    {
        using var box = Populated();

        Assert.Equal(0, box.FindString("Ap", 2));
    }

    [Fact]
    public void FindString_ReturnsNoMatchesWhenNothingStartsWithIt()
    {
        using var box = Populated();

        Assert.Equal(ListBox.NoMatches, box.FindString("Zebra"));
    }

    [Fact]
    public void FindString_OfEmptyOrNullTextIsNoMatch()
    {
        using var box = Populated();

        Assert.Equal(ListBox.NoMatches, box.FindString(string.Empty));
        Assert.Equal(ListBox.NoMatches, box.FindString(null!));
    }

    [Fact]
    public void FindString_OnAnEmptyListIsNoMatch()
    {
        using var box = new ListBox();

        Assert.Equal(ListBox.NoMatches, box.FindString("anything"));
    }

    [Fact]
    public void FindString_RejectsAnOutOfRangeStartIndex()
    {
        using var box = Populated();

        Assert.Throws<ArgumentOutOfRangeException>(() => box.FindString("Ap", 99));
        Assert.Throws<ArgumentOutOfRangeException>(() => box.FindString("Ap", -2));
    }

    [Fact]
    public void FindStringExact_RequiresTheWholeItemToMatch()
    {
        using var box = Populated();

        Assert.Equal(0, box.FindStringExact("Apple"));
        Assert.Equal(ListBox.NoMatches, box.FindStringExact("App"));
    }

    [Fact]
    public void FindStringExact_IsCaseInsensitive()
    {
        using var box = Populated();

        Assert.Equal(2, box.FindStringExact("banana"));
    }

    [Fact]
    public void FindStringExact_FromAStartIndexWrapsAround()
    {
        using var box = Populated();

        Assert.Equal(0, box.FindStringExact("Apple", 2));
    }

    [Fact]
    public void FindStringAndFindStringExactDisagreeOnAPrefix()
    {
        // The distinction between the two methods, which is the reason both exist.
        using var box = Populated();

        Assert.Equal(0, box.FindString("Appl"));
        Assert.Equal(ListBox.NoMatches, box.FindStringExact("Appl"));
    }

    // ---- ListBox.DrawMode and IndexFromPoint ------------------------------

    [Fact]
    public void DrawMode_RoundTrips()
    {
        using var box = new ListBox();

        Assert.Equal(DrawMode.Normal, box.DrawMode);

        box.DrawMode = DrawMode.OwnerDrawFixed;

        Assert.Equal(DrawMode.OwnerDrawFixed, box.DrawMode);
    }

    [Fact]
    public void IndexFromPoint_ReportsNoMatchWhereThereIsNoItem()
    {
        // This used to dereference null when the point was past the last item.
        using var box = Populated();

        Assert.Equal(ListBox.NoMatches, box.IndexFromPoint(new global::System.Drawing.Point(5, 10_000)));
    }

    [Fact]
    public void IndexFromPoint_BothOverloadsAgree()
    {
        using var box = Populated();

        Assert.Equal(
            box.IndexFromPoint(5, 10_000),
            box.IndexFromPoint(new global::System.Drawing.Point(5, 10_000)));
    }
}

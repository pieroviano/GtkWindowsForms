using System.Drawing;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class LinkLabelTests
{
    [Fact]
    public void Constructor_ShouldInitializeSelfAndEvents()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = linkLabel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(linkLabel.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(linkLabel.self, gtkControl);
    }

    [Fact]
    public void Text_ShouldSetAndGetValue()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            linkLabel.Text = "https://example.com";
            text = linkLabel.Text;
            selfText = linkLabel.self.Label;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("https://example.com", text);
        Assert.Equal("https://example.com", selfText);
    }

    [Fact]
    public void LinkColorProperties_ShouldSetAndGetValues()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var active = Color.Red;
        var disabled = Color.Gray;
        var visited = Color.Blue;

        form.Load += (s, e) =>
        {
            linkLabel.ActiveLinkColor = active;
            linkLabel.DisabledLinkColor = disabled;
            linkLabel.VisitedLinkColor = visited;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(active, linkLabel.ActiveLinkColor);
        Assert.Equal(disabled, linkLabel.DisabledLinkColor);
        Assert.Equal(visited, linkLabel.VisitedLinkColor);
    }

    [Fact]
    public void LinkVisited_ShouldSetAndGetValue()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var visitedBefore = false;
        var visitedAfter = false;

        form.Load += (s, e) =>
        {
            visitedBefore = linkLabel.LinkVisited;
            linkLabel.LinkVisited = true;
            visitedAfter = linkLabel.LinkVisited;
            form.Close();
        };

        form.ShowDialog();

        Assert.False(visitedBefore);
        Assert.True(visitedAfter);
    }

    [Fact]
    public void Click_ShouldRaiseEvent()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var clicked = false;

        form.Load += (s, e) =>
        {
            linkLabel.Click += (sender, args) => clicked = true;
            linkLabel.OnClick(e);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(clicked);
    }

    [Fact]
    public void LinkClicked_ShouldRaiseEvent()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var linkClicked = false;
        LinkLabel.Link? linkArg = null;

        form.Load += (s, e) =>
        {
            linkLabel.LinkClicked += (sender, args) =>
            {
                linkClicked = true;
                linkArg = args.Link;
            };

            linkLabel.OnLinkClicked(new LinkLabelLinkClickedEventArgs(new LinkLabel.Link { Description = linkLabel.Text }));
            form.Close();
        };

        form.ShowDialog();

        Assert.True(linkClicked);
        Assert.NotNull(linkArg);
        Assert.Equal(linkLabel.Text, linkArg.Description);
    }

    // The three tests that stood here asserted the old broken behaviour as the contract: that
    // Links.Add(object) overwrote the label's Text with the value's string form, that Clear() wiped the
    // Text, and that Contains() did a substring search on it. WinForms does none of those - a link
    // collection stores links and never touches Text. They are replaced by the real semantics.

    [Fact]
    public void LinkCollection_Add_StoresTheLinkAndLeavesTheTextAlone()
    {
        var linkLabel = new LinkLabel { Text = "visit our site today" };
        var collection = new LinkLabel.LinkCollection(linkLabel);

        var added = collection.Add(new LinkLabel.Link(6, 3));

        Assert.Single(collection);
        Assert.Same(added, collection[0]);
        Assert.Equal("visit our site today", linkLabel.Text);
    }

    [Fact]
    public void LinkCollection_Clear_EmptiesTheCollectionAndLeavesTheTextAlone()
    {
        var linkLabel = new LinkLabel { Text = "visit our site today" };
        var collection = new LinkLabel.LinkCollection(linkLabel);
        collection.Add(new LinkLabel.Link(6, 3));

        collection.Clear();

        Assert.Empty(collection);
        Assert.Equal("visit our site today", linkLabel.Text);
    }

    [Fact]
    public void LinkCollection_Contains_IsIdentityBased()
    {
        var linkLabel = new LinkLabel { Text = "visit our site today" };
        var collection = new LinkLabel.LinkCollection(linkLabel);
        var link = new LinkLabel.Link(6, 3);
        var stranger = new LinkLabel.Link(0, 5);

        collection.Add(link);

        Assert.True(collection.Contains(link), "the added link should be contained");
        Assert.False(collection.Contains(stranger), "a link that was never added should not be contained");
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = linkLabel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(linkLabel.self, gtkControl);
    }
}
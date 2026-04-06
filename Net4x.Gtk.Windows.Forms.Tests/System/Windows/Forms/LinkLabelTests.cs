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

    [Fact]
    public void LinkCollection_Add_ShouldSetOwnerText()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var collection = new LinkLabel.LinkCollection(linkLabel);
        var textBefore = "";
        var textAfter = "";

        form.Load += (s, e) =>
        {
            textBefore = linkLabel.Text;
            collection.Add("https://example.com");
            textAfter = linkLabel.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("", textBefore);
        Assert.Equal("https://example.com", textAfter);
    }

    [Fact]
    public void LinkCollection_Clear_ShouldClearOwnerText()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var collection = new LinkLabel.LinkCollection(linkLabel);

        form.Load += (s, e) =>
        {
            collection.Add("https://example.com");
            collection.Clear();
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("", linkLabel.Text);
    }

    [Fact]
    public void LinkCollection_Contains_ShouldReturnCorrectValue()
    {
        var form = new Form();
        var linkLabel = new LinkLabel();
        form.Controls.Add(linkLabel);

        var collection = new LinkLabel.LinkCollection(linkLabel);
        var containsBefore = false;
        var containsAfter = false;

        form.Load += (s, e) =>
        {
            containsBefore = collection.Contains("https://example.com");
            collection.Add("https://example.com");
            containsAfter = collection.Contains("https://example.com");
            form.Close();
        };

        form.ShowDialog();

        Assert.False(containsBefore);
        Assert.True(containsAfter);
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
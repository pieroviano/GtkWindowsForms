using System.Windows.Forms;
using Color = global::System.Drawing.Color;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for the menu/strip item model and <see cref="NotifyIcon"/>. Menu items form
/// an ordered tree of drop-downs whose parentage and ordering must stay consistent.</summary>
[Collection("NonParallelCollection")]
public class ToolStripNotifyIconFunctionalTests
{
    [Fact]
    public void MenuItem_TextAndNameRoundTrip()
    {
        var item = new ToolStripMenuItem { Text = "&File", Name = "fileMenu", Tag = 3 };

        Assert.Equal("&File", item.Text);
        Assert.Equal("fileMenu", item.Name);
        Assert.Equal(3, item.Tag);
    }

    [Fact]
    public void MenuItem_ConstructedWithText_CarriesIt()
    {
        var item = new ToolStripMenuItem("Open");

        Assert.Equal("Open", item.Text);
    }

    [Fact]
    public void MenuItem_StateProperties_RoundTrip()
    {
        var item = new ToolStripMenuItem("Toggle")
        {
            Enabled = false,
            Visible = false,
            Checked = true,
            AutoToolTip = true,
            ToolTipText = "explains"
        };

        Assert.False(item.Enabled);
        Assert.False(item.Visible);
        Assert.True(item.Checked);
        Assert.True(item.AutoToolTip);
        Assert.Equal("explains", item.ToolTipText);
    }

    [Fact]
    public void MenuItem_CheckStateFollowsChecked()
    {
        var item = new ToolStripMenuItem("Toggle") { Checked = true };

        Assert.Equal(CheckState.Checked, item.CheckState);

        item.Checked = false;

        Assert.Equal(CheckState.Unchecked, item.CheckState);
    }

    [Fact]
    public void MenuItem_ColorsRoundTrip()
    {
        var item = new ToolStripMenuItem("Colored")
        {
            BackColor = Color.LightBlue,
            ForeColor = Color.DarkBlue
        };

        Assert.Equal(Color.LightBlue, item.BackColor);
        Assert.Equal(Color.DarkBlue, item.ForeColor);
    }

    [Fact]
    public void DropDownItems_AddKeepsOrderAndCount()
    {
        var parent = new ToolStripMenuItem("File");
        var open = new ToolStripMenuItem("Open");
        var save = new ToolStripMenuItem("Save");

        parent.DropDownItems.Add(open);
        parent.DropDownItems.Add(save);

        Assert.Equal(2, parent.DropDownItems.Count);
        Assert.Same(open, parent.DropDownItems[0]);
        Assert.Same(save, parent.DropDownItems[1]);
    }

    [Fact]
    public void DropDownItems_AddByText_CreatesAnItemWithThatText()
    {
        var parent = new ToolStripMenuItem("File");

        var created = parent.DropDownItems.Add("Open");

        Assert.Single(parent.DropDownItems);
        Assert.Equal("Open", created.Text);
    }

    [Fact]
    public void DropDownItems_RemoveDropsTheItem()
    {
        var parent = new ToolStripMenuItem("File");
        var open = new ToolStripMenuItem("Open");
        parent.DropDownItems.Add(open);
        parent.DropDownItems.Add(new ToolStripMenuItem("Save"));

        parent.DropDownItems.Remove(open);

        Assert.Single(parent.DropDownItems);
        Assert.False(parent.DropDownItems.Contains(open), "the removed item should no longer be contained");
    }

    [Fact]
    public void DropDownItems_RemoveAtDropsThatPosition()
    {
        var parent = new ToolStripMenuItem("File");
        parent.DropDownItems.Add(new ToolStripMenuItem("first"));
        parent.DropDownItems.Add(new ToolStripMenuItem("second"));

        parent.DropDownItems.RemoveAt(0);

        Assert.Single(parent.DropDownItems);
        Assert.Equal("second", parent.DropDownItems[0].Text);
    }

    [Fact]
    public void DropDownItems_ClearEmptiesTheMenu()
    {
        var parent = new ToolStripMenuItem("File");
        parent.DropDownItems.Add(new ToolStripMenuItem("a"));
        parent.DropDownItems.Add(new ToolStripMenuItem("b"));

        parent.DropDownItems.Clear();

        Assert.Empty(parent.DropDownItems);
    }

    [Fact]
    public void DropDownItems_IndexOfReportsThePosition()
    {
        var parent = new ToolStripMenuItem("File");
        parent.DropDownItems.Add(new ToolStripMenuItem("a"));
        var b = new ToolStripMenuItem("b");
        parent.DropDownItems.Add(b);

        Assert.Equal(1, parent.DropDownItems.IndexOf(b));
    }

    [Fact]
    public void DropDownItems_InsertPlacesTheItemAtThatPosition()
    {
        var parent = new ToolStripMenuItem("File");
        parent.DropDownItems.Add(new ToolStripMenuItem("first"));
        parent.DropDownItems.Add(new ToolStripMenuItem("third"));

        parent.DropDownItems.Insert(1, new ToolStripMenuItem("second"));

        Assert.Equal(3, parent.DropDownItems.Count);
        Assert.Equal("second", parent.DropDownItems[1].Text);
    }

    [Fact]
    public void DropDownItems_AddRangeAppendsEveryItem()
    {
        var parent = new ToolStripMenuItem("File");

        parent.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("a"), new ToolStripMenuItem("b")
        });

        Assert.Equal(2, parent.DropDownItems.Count);
        Assert.Equal("b", parent.DropDownItems[1].Text);
    }

    [Fact]
    public void MenuStrip_ItemsCollectionBehavesLikeAnOrderedList()
    {
        using var strip = new MenuStrip();
        var file = new ToolStripMenuItem("File");
        var edit = new ToolStripMenuItem("Edit");

        strip.Items.Add(file);
        strip.Items.Add(edit);

        Assert.Equal(2, strip.Items.Count);
        Assert.Same(file, strip.Items[0]);
        Assert.Equal(1, strip.Items.IndexOf(edit));

        strip.Items.Remove(file);
        Assert.Single(strip.Items);

        strip.Items.Clear();
        Assert.Empty(strip.Items);
    }

    [Fact]
    public void ContextMenuStrip_ItemsCollectionBehavesLikeAnOrderedList()
    {
        using var menu = new ContextMenuStrip();

        menu.Items.Add(new ToolStripMenuItem("Cut"));
        menu.Items.Add(new ToolStripMenuItem("Copy"));

        Assert.Equal(2, menu.Items.Count);
        Assert.Equal("Copy", menu.Items[1].Text);
    }

    [Fact]
    public void NotifyIcon_TextAndTagRoundTrip()
    {
        using var notify = new NotifyIcon { Text = "tray", Tag = 11 };

        Assert.Equal("tray", notify.Text);
        Assert.Equal(11, notify.Tag);
    }

    [Fact]
    public void NotifyIcon_VisibleRoundTrips()
    {
        using var notify = new NotifyIcon();
        Assert.False(notify.Visible, "a new NotifyIcon starts hidden");

        notify.Visible = true;
        Assert.True(notify.Visible);

        notify.Visible = false;
        Assert.False(notify.Visible);
    }

    [Fact]
    public void NotifyIcon_BalloonPropertiesRoundTrip()
    {
        using var notify = new NotifyIcon
        {
            BalloonTipTitle = "Title",
            BalloonTipText = "Body",
            BalloonTipIcon = ToolTipIcon.Info
        };

        Assert.Equal("Title", notify.BalloonTipTitle);
        Assert.Equal("Body", notify.BalloonTipText);
        Assert.Equal(ToolTipIcon.Info, notify.BalloonTipIcon);
    }

    [Fact]
    public void NotifyIcon_ContextMenuStripRoundTrips()
    {
        using var notify = new NotifyIcon();
        using var menu = new ContextMenuStrip();

        notify.ContextMenuStrip = menu;

        Assert.Same(menu, notify.ContextMenuStrip);
    }
}

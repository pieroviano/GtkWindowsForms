extern alias gtk;
using System.Windows.Forms;
using ContentAlignment = gtk::System.Drawing.ContentAlignment;
using Color = global::System.Drawing.Color;
using Size = global::System.Drawing.Size;
using Rectangle = global::System.Drawing.Rectangle;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests across the <see cref="ToolStripItem"/> family. Every item kind shares the same
/// state surface (text, size, alignment, enablement) and must carry it faithfully, and a strip must own the
/// items it is given.</summary>
[Collection("NonParallelCollection")]
public class ToolStripItemTypesFunctionalTests
{
    /// <summary>The item kinds under test, named so no live GTK widget travels through theory data.</summary>
    public enum ItemKind
    {
        MenuItem,
        Button,
        Label,
        Separator,
        ComboBox,
        TextBox
    }

    private static ToolStripItem Create(ItemKind kind) => kind switch
    {
        ItemKind.MenuItem => new ToolStripMenuItem(),
        ItemKind.Button => new ToolStripButton(),
        ItemKind.Label => new ToolStripLabel(),
        ItemKind.Separator => new ToolStripSeparator(),
        ItemKind.ComboBox => new ToolStripComboBox(),
        _ => new ToolStripTextBox()
    };

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    [InlineData(ItemKind.ComboBox)]
    [InlineData(ItemKind.TextBox)]
    public void TextRoundTripsOnEveryItemKind(ItemKind kind)
    {
        var item = Create(kind);

        item.Text = "caption";

        Assert.Equal("caption", item.Text);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    [InlineData(ItemKind.Separator)]
    [InlineData(ItemKind.ComboBox)]
    [InlineData(ItemKind.TextBox)]
    public void NameAndTagRoundTripOnEveryItemKind(ItemKind kind)
    {
        var item = Create(kind);

        item.Name = "item1";
        item.Tag = 5;

        Assert.Equal("item1", item.Name);
        Assert.Equal(5, item.Tag);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    [InlineData(ItemKind.Separator)]
    public void EnabledAndVisibleRoundTripOnEveryItemKind(ItemKind kind)
    {
        var item = Create(kind);

        item.Enabled = false;
        item.Visible = false;

        Assert.False(item.Enabled);
        Assert.False(item.Visible);

        item.Enabled = true;
        item.Visible = true;

        Assert.True(item.Enabled);
        Assert.True(item.Visible);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void SizeRoundTripsOnEveryItemKind(ItemKind kind)
    {
        var item = Create(kind);

        item.Size = new Size(120, 26);

        Assert.Equal(new Size(120, 26), item.Size);
        Assert.Equal(120, item.Width);
        Assert.Equal(26, item.Height);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void ColoursRoundTripOnEveryItemKind(ItemKind kind)
    {
        var item = Create(kind);

        item.BackColor = Color.LightBlue;
        item.ForeColor = Color.DarkBlue;

        Assert.Equal(Color.LightBlue, item.BackColor);
        Assert.Equal(Color.DarkBlue, item.ForeColor);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void AlignmentAndDisplayStyleRoundTrip(ItemKind kind)
    {
        var item = Create(kind);

        item.TextAlign = ContentAlignment.MiddleRight;
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        item.TextImageRelation = TextImageRelation.ImageBeforeText;
        item.Alignment = ToolStripItemAlignment.Right;

        Assert.Equal(ContentAlignment.MiddleRight, item.TextAlign);
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
        Assert.Equal(ToolStripItemAlignment.Right, item.Alignment);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void ToolTipTextAndAutoToolTipRoundTrip(ItemKind kind)
    {
        var item = Create(kind);

        item.ToolTipText = "explains";
        item.AutoToolTip = true;

        Assert.Equal("explains", item.ToolTipText);
        Assert.True(item.AutoToolTip);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void PaddingAndMarginRoundTrip(ItemKind kind)
    {
        var item = Create(kind);

        item.Padding = new Padding(2, 3, 4, 5);
        item.Margin = new Padding(1);

        Assert.Equal(new Padding(2, 3, 4, 5), item.Padding);
        Assert.Equal(new Padding(1), item.Margin);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    public void SetBoundsAppliesThePositionAndSize(ItemKind kind)
    {
        var item = Create(kind);

        item.SetBounds(new Rectangle(4, 5, 60, 24), BoundsSpecified.All);

        Assert.Equal(60, item.Width);
        Assert.Equal(24, item.Height);
    }

    [Theory]
    [InlineData(ItemKind.MenuItem)]
    [InlineData(ItemKind.Button)]
    [InlineData(ItemKind.Label)]
    [InlineData(ItemKind.Separator)]
    public void EveryItemKindExposesItsUnderlyingWidget(ItemKind kind)
    {
        var item = Create(kind);

        Assert.NotNull(item.Widget);
        Assert.NotNull(item.GetWidget());
    }

    [Fact]
    public void Button_CheckedAndCheckStateAgree()
    {
        var button = new ToolStripButton { CheckOnClick = true };

        button.Checked = true;
        Assert.Equal(CheckState.Checked, button.CheckState);

        button.Checked = false;
        Assert.Equal(CheckState.Unchecked, button.CheckState);
    }

    [Fact]
    public void Button_PerformClickRaisesClick()
    {
        var button = new ToolStripButton();
        var clicks = 0;
        button.Click += (_, _) => clicks++;

        button.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void MenuItem_PerformClickRaisesClick()
    {
        var item = new ToolStripMenuItem("Open");
        var clicks = 0;
        item.Click += (_, _) => clicks++;

        item.PerformClick();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void ComboBox_ItemsCollectionBehavesLikeAList()
    {
        var combo = new ToolStripComboBox();

        combo.Items.Add("one");
        combo.Items.Add("two");

        Assert.Equal(2, combo.Items.Count);
        Assert.Equal("one", combo.Items[0]);

        combo.Items.Remove("one");
        Assert.Single(combo.Items);

        combo.Items.Clear();
        Assert.Empty(combo.Items);
    }

    [Fact]
    public void TextBox_TextRoundTrips()
    {
        var box = new ToolStripTextBox();

        box.Text = "typed";

        Assert.Equal("typed", box.Text);
    }

    [Fact]
    public void Label_TextRoundTrips()
    {
        var label = new ToolStripLabel { Text = "status" };

        Assert.Equal("status", label.Text);
    }

    [Fact]
    public void ToolStrip_OwnsTheItemsItIsGiven()
    {
        using var strip = new ToolStrip();
        var button = new ToolStripButton("Go");

        strip.Items.Add(button);

        Assert.Single(strip.Items);
        Assert.Same(button, strip.Items[0]);
    }

    [Fact]
    public void ToolStrip_ItemsCollectionSupportsInsertAndRemove()
    {
        using var strip = new ToolStrip();
        var first = new ToolStripButton("first");
        var third = new ToolStripButton("third");
        strip.Items.Add(first);
        strip.Items.Add(third);

        strip.Items.Insert(1, new ToolStripButton("second"));

        Assert.Equal(3, strip.Items.Count);
        Assert.Equal("second", strip.Items[1].Text);

        strip.Items.Remove(first);
        Assert.Equal(2, strip.Items.Count);
    }

    [Fact]
    public void StatusStrip_ItemsCollectionBehavesLikeAnOrderedList()
    {
        using var strip = new StatusStrip();

        strip.Items.Add(new ToolStripLabel("ready"));
        strip.Items.Add(new ToolStripLabel("busy"));

        Assert.Equal(2, strip.Items.Count);
        Assert.Equal("busy", strip.Items[1].Text);

        strip.Items.Clear();
        Assert.Empty(strip.Items);
    }

    [Fact]
    public void Separator_IsAToolStripItemWithNoText()
    {
        var separator = new ToolStripSeparator();

        Assert.IsAssignableFrom<ToolStripItem>(separator);
        Assert.True(string.IsNullOrEmpty(separator.Text), "a separator carries no caption");
    }
}

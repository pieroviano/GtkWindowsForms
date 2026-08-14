using System.Drawing;

namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class ToolStripTests
{
    [Fact]
    public void Constructor_Default_ShouldInitialize()
    {
        var form = new Form();
        var toolStrip = new ToolStrip();
        form.Controls.Add(toolStrip);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = toolStrip.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(toolStrip.self);
        Assert.NotNull(toolStrip.Items);
        Assert.Equal(toolStrip.self, gtkControl);
    }

    [Fact]
    public void Constructor_WithOwner_ShouldInitialize()
    {
        var form = new Form();
        var toolStrip = new ToolStrip("owner");
        form.Controls.Add(toolStrip);

        bool hexpand = true;
        bool vexpand = true;

        form.Load += (s, e) =>
        {
            hexpand = toolStrip.self.Hexpand;
            vexpand = toolStrip.self.Vexpand;
            form.Close();
        };

        form.ShowDialog();

        Assert.False(hexpand);
        Assert.False(vexpand);
        Assert.NotNull(toolStrip.Items);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var toolStrip = new ToolStrip();
        form.Controls.Add(toolStrip);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = toolStrip.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(toolStrip.self, gtkControl);
    }

    [Fact]
    public void ImageScalingSize_ShouldSetAndGet()
    {
        var toolStrip = new ToolStrip();

        var size = new Size(16, 16);
        toolStrip.ImageScalingSize = size;

        Assert.Equal(size, toolStrip.ImageScalingSize);
    }

    [Fact]
    public void LayoutStyle_ShouldSetAndGet()
    {
        Application.Init();
        var toolStrip = new ToolStrip
        {
            LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
        };

        Assert.Equal(ToolStripLayoutStyle.HorizontalStackWithOverflow, toolStrip.LayoutStyle);
    }

    [Fact]
    public void Size_WhenAutoSizeOff_HonoursExplicitHeight()
    {
        // WinForms: with AutoSize off the strip keeps the height it is given (it is not clamped).
        var toolStrip = new ToolStrip
        {
            AutoSize = false,
            Size = new Size(200, 100)
        };

        Assert.Equal(200, toolStrip.Size.Width);
        Assert.Equal(100, toolStrip.Size.Height);
    }

    [Fact]
    public void Items_ShouldReturnSameCollection()
    {
        var toolStrip = new ToolStrip();

        var items1 = toolStrip.Items;
        var items2 = toolStrip.Items;

        Assert.NotNull(items1);
        Assert.Same(items1, items2);
    }

    [Fact]
    public void ActivateCurrent_ShouldRaiseAllEvents()
    {
        var form = new Form();
        var toolStrip = new ToolStrip();
        form.Controls.Add(toolStrip);

        bool clickRaised = false;
        bool checkedChangedRaised = false;
        bool checkStateChangedRaised = false;
        bool dropDownClickedRaised = false;

        form.Load += (s, e) =>
        {
            toolStrip.Click += (_, __) => clickRaised = true;
            toolStrip.CheckedChanged += (_, __) => checkedChangedRaised = true;
            toolStrip.CheckStateChanged += (_, __) => checkStateChangedRaised = true;
            toolStrip.DropDownItemClicked += (_, __) => dropDownClickedRaised = true;

            // Trigger GTK event
            toolStrip.self.RaiseActivateCurrent(toolStrip);

            form.Close();
        };

        form.ShowDialog();

        Assert.True(clickRaised);
        Assert.True(checkedChangedRaised);
        Assert.True(checkStateChangedRaised);
        Assert.True(dropDownClickedRaised);
    }

    [Fact]
    public void OnClick_ShouldRaiseClickEvent()
    {
        var toolStrip = new ToolStrip();

        bool raised = false;
        toolStrip.Click += (_, __) => raised = true;

        toolStrip.OnClick(EventArgs.Empty);

        Assert.True(raised);
    }
}
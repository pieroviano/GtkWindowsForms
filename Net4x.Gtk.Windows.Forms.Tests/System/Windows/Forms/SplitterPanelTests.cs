extern alias gtk;

using System.Drawing;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class SplitterPanelTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var split = new SplitContainer();
        var panel = new SplitterPanel(split);

        form.Controls.Add(panel);

        SplitContainer? owner = null;
        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            owner = panel.Owner;
            gtkControl = panel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(panel.self);
        Assert.Equal(split, owner);
        Assert.NotNull(gtkControl);
    }

    [Fact]
    public void Dock_ShouldAlwaysBeFill()
    {
        var form = new Form();
        var split = new SplitContainer();
        var panel = new SplitterPanel(split);

        form.Controls.Add(panel);

        DockStyle dockBefore = DockStyle.None;
        DockStyle dockAfter = DockStyle.None;

        form.Load += (s, e) =>
        {
            dockBefore = panel.Dock;

            panel.Dock = DockStyle.Top; // should be ignored

            dockAfter = panel.Dock;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(DockStyle.Fill, dockBefore);
        Assert.Equal(DockStyle.Fill, dockAfter);
    }

    [Fact]
    public void Width_ShouldSetAndGet()
    {
        var form = new Form();
        var panel = new SplitterPanel(new SplitContainer());

        form.Controls.Add(panel);

        int width = 0;

        form.Load += (s, e) =>
        {
            panel.Width = 123;
            width = panel.Width;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(123, width);
    }

    [Fact]
    public void Height_ShouldSetAndGet()
    {
        var form = new Form();
        var panel = new SplitterPanel(new SplitContainer());

        form.Controls.Add(panel);

        int height = 0;

        form.Load += (s, e) =>
        {
            panel.Height = 456;
            height = panel.Height;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(456, height);
    }

    [Fact]
    public void Size_ShouldSetAndGet()
    {
        var form = new Form();
        var panel = new SplitterPanel(new SplitContainer());

        form.Controls.Add(panel);

        Size size = Size.Empty;

        form.Load += (s, e) =>
        {
            panel.Size = new Size(10, 20);
            size = panel.Size;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(new Size(10, 20), size);
    }

    [Fact]
    public void Owner_ShouldBeAssignedCorrectly()
    {
        var form = new Form();
        var split = new SplitContainer();
        var panel = new SplitterPanel(split);

        form.Controls.Add(panel);

        SplitContainer? owner = null;

        form.Load += (s, e) =>
        {
            owner = panel.Owner;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(split, owner);
    }

    [Fact]
    public void MultiplePanels_ShouldHaveDifferentOwners()
    {
        var form = new Form();
        var split1 = new SplitContainer();
        var split2 = new SplitContainer();

        var panel1 = new SplitterPanel(split1);
        var panel2 = new SplitterPanel(split2);

        form.Controls.Add(panel1);
        form.Controls.Add(panel2);

        SplitContainer? owner1 = null;
        SplitContainer? owner2 = null;

        form.Load += (s, e) =>
        {
            owner1 = panel1.Owner;
            owner2 = panel2.Owner;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(split1, owner1);
        Assert.Equal(split2, owner2);
        Assert.NotEqual(owner1, owner2);
    }

    [Fact]
    public void GtkControl_ShouldNotBeNull()
    {
        var form = new Form();
        var panel = new SplitterPanel(new SplitContainer());

        form.Controls.Add(panel);

        object? gtk = null;

        form.Load += (s, e) =>
        {
            gtk = panel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtk);
    }
}
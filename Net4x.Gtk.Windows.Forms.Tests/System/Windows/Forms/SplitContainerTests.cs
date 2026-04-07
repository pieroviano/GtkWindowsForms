namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class SplitContainerTests
{
    [Fact]
    public void Constructor_ShouldInitializePanels_AndGtkControl()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        object? gtkControl = null;
        object? panel1 = null;
        object? panel2 = null;

        form.Load += (s, e) =>
        {
            gtkControl = split.GtkControl;
            panel1 = split.Panel1;
            panel2 = split.Panel2;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(split.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(split.self, gtkControl);

        Assert.NotNull(panel1);
        Assert.NotNull(panel2);
        Assert.NotSame(panel1, panel2);
    }

    [Fact]
    public void Orientation_ShouldMapCorrectly()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        Orientation result1 = Orientation.Horizontal;
        Orientation result2 = Orientation.Horizontal;

        form.Load += (s, e) =>
        {
            split.Orientation = Orientation.Horizontal;
            result1 = split.Orientation;

            split.Orientation = Orientation.Vertical;
            result2 = split.Orientation;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(Orientation.Horizontal, result1);
        Assert.Equal(Orientation.Vertical, result2);
    }

    [Fact]
    public void SplitterDistance_ShouldSetAndGet_WithOffset()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        int result = 0;

        form.Load += (s, e) =>
        {
            split.SplitterDistance = 100;
            result = split.SplitterDistance;
            form.Close();
        };

        form.ShowDialog();

        // internal offset logic: Position + 5
        Assert.Equal(100, result);
    }

    [Fact]
    public void SplitterDistance_ShouldClampMinimum()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        int result = 0;

        form.Load += (s, e) =>
        {
            split.SplitterDistance = 0; // should clamp
            result = split.SplitterDistance;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(result >= 1);
    }

    [Fact]
    public void SplitterWidth_ShouldControlWideHandle()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        bool wide1 = false;
        bool wide2 = false;

        form.Load += (s, e) =>
        {
            split.SplitterWidth = 1;
            wide1 = split.self.WideHandle;

            split.SplitterWidth = 10;
            wide2 = split.self.WideHandle;

            form.Close();
        };

        form.ShowDialog();

        Assert.False(wide1);
        Assert.True(wide2);
    }

    [Fact]
    public void FixedPanel_ShouldUpdateResizeBehavior()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        bool panel1Resize = false;
        bool panel2Resize = false;

        form.Load += (s, e) =>
        {
            split.FixedPanel = FixedPanel.Panel2;

            var gtk = (Gtk.Paned)split.self;

            var child1 = (Gtk.Paned.PanedChild)gtk[split.Panel1.Widget];
            var child2 = (Gtk.Paned.PanedChild)gtk[split.Panel2.Widget];

            panel1Resize = child1.Resize;
            panel2Resize = child2.Resize;

            form.Close();
        };

        form.ShowDialog();

        // Panel2 fixed → Panel1 resizes
        Assert.True(panel1Resize);
        Assert.False(panel2Resize);
    }

    [Fact]
    public void PanelProperties_ShouldBeSettable()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        SplitterPanel newPanel1 = null!;
        SplitterPanel newPanel2 = null!;

        form.Load += (s, e) =>
        {
            newPanel1 = new SplitterPanel(split);
            newPanel2 = new SplitterPanel(split);

            split.Panel1 = newPanel1;
            split.Panel2 = newPanel2;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(newPanel1, split.Panel1);
        Assert.Equal(newPanel2, split.Panel2);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = split.GtkControl;
            self = split.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void SplitterIncrement_ShouldSetAndGet()
    {
        var form = new Form();
        var split = new SplitContainer();
        form.Controls.Add(split);

        int result = 0;

        form.Load += (s, e) =>
        {
            split.SplitterIncrement = 15;
            result = split.SplitterIncrement;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(15, result);
    }
}
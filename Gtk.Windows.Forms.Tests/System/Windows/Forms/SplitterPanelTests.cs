extern alias gtk;

using System.Drawing;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class SplitterPanelTests
{
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
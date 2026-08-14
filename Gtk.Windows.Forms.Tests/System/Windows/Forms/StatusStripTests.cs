using System.Drawing;

namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class StatusStripTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        object? gtkControl = null;
        DockStyle dock = DockStyle.None;

        form.Load += (s, e) =>
        {
            gtkControl = statusStrip.GtkControl;
            dock = statusStrip.Dock;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(statusStrip.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(statusStrip.self, gtkControl);

        // From constructor
        Assert.Equal(DockStyle.Bottom, dock);
    }

    [Fact]
    public void Location_ShouldApplyYOffset()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        Point result = Point.Empty;

        form.Load += (s, e) =>
        {
            statusStrip.Location = new Point(10, 20);

            result = statusStrip.Location;

            form.Close();
        };

        form.ShowDialog();

        // Getter returns base.Location (already offset internally)
        Assert.Equal(result, new Point(10, 8));
    }

    [Fact]
    public void Size_WhenAutoSizeOff_HonoursExplicitHeight()
    {
        // WinForms: with AutoSize off the strip keeps the height it is given (it is not clamped).
        var form = new Form();
        var statusStrip = new StatusStrip { AutoSize = false };
        form.Controls.Add(statusStrip);

        Size result = Size.Empty;

        form.Load += (s, e) =>
        {
            statusStrip.Size = new Size(100, 50);

            result = statusStrip.Size;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(new Size(100, 50), result);
    }

    [Fact]
    public void ShowItemToolTips_ShouldSetAndGetValue()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        bool value = false;

        form.Load += (s, e) =>
        {
            statusStrip.ShowItemToolTips = true;
            value = statusStrip.ShowItemToolTips;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(value);
    }

    [Fact]
    public void SizingGrip_ShouldSetAndGetValue()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        bool value = true;

        form.Load += (s, e) =>
        {
            statusStrip.SizingGrip = false;
            value = statusStrip.SizingGrip;
            form.Close();
        };

        form.ShowDialog();

        Assert.False(value);
    }

    [Fact]
    public void Stretch_ShouldSetAndGetValue()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        bool value = false;

        form.Load += (s, e) =>
        {
            statusStrip.Stretch = true;
            value = statusStrip.Stretch;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(value);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var statusStrip = new StatusStrip();
        form.Controls.Add(statusStrip);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = statusStrip.GtkControl;
            self = statusStrip.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}
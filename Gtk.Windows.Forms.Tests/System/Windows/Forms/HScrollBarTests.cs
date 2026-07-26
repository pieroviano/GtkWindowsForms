using Gtk;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class HScrollBarTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var scrollBar = new HScrollBar();
        form.Controls.Add(scrollBar);

        object? gtkControl = null;
        Adjustment? adjustment = null;

        form.Load += (s, e) =>
        {
            gtkControl = scrollBar.GtkControl;
            adjustment = scrollBar.Adjustment;

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(scrollBar.self);
        Assert.NotNull(gtkControl);
        Assert.NotNull(adjustment);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var scrollBar = new HScrollBar();
        form.Controls.Add(scrollBar);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = scrollBar.GtkControl;
            self = scrollBar.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void Adjustment_ShouldReturnSameInstance_AsSelfAdjustment()
    {
        var form = new Form();
        var scrollBar = new HScrollBar();
        form.Controls.Add(scrollBar);

        Adjustment? expected = null;
        Adjustment? actual = null;

        form.Load += (s, e) =>
        {
            expected = scrollBar.self.Adjustment;
            actual = scrollBar.Adjustment;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Adjustment_ShouldBeUsable()
    {
        var form = new Form();
        var scrollBar = new HScrollBar();
        form.Controls.Add(scrollBar);

        double value = -1;

        form.Load += (s, e) =>
        {
            var adj = scrollBar.Adjustment;

            adj.Value = 25;
            value = adj.Value;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(25, value);
    }

    [Fact]
    public void ScrollBar_ShouldBeAddableToForm()
    {
        var form = new Form();
        var scrollBar = new HScrollBar();

        var contains = false;
        Control? parent = null;

        form.Load += (s, e) =>
        {
            form.Controls.Add(scrollBar);

            contains = form.Controls.Contains(scrollBar);
            parent = scrollBar.Parent;

            form.Close();
        };

        form.ShowDialog();

        Assert.True(contains);
        Assert.Equal(form, parent);
    }
}
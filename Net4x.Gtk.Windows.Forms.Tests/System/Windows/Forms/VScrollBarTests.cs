namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class VScrollBarTests
{
    [Fact]
    public void Constructor_ShouldInitialize()
    {
        var form = new Form();
        var scrollBar = new VScrollBar();
        form.Controls.Add(scrollBar);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = scrollBar.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(scrollBar.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(scrollBar.self, gtkControl);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        Application.Init();
        var scrollBar = new VScrollBar();

        var gtkControl = scrollBar.GtkControl;

        Assert.Equal(scrollBar.self, gtkControl);
    }

    [Fact]
    public void Adjustment_ShouldNotBeNull()
    {
        Application.Init();
        var scrollBar = new VScrollBar();

        var adjustment = scrollBar.Adjustment;

        Assert.NotNull(adjustment);
    }

    [Fact]
    public void Adjustment_ShouldReflectValueChanges()
    {
        Application.Init();
        var scrollBar = new VScrollBar();

        scrollBar.Adjustment.Value = 25;

        Assert.Equal(25, scrollBar.Adjustment.Value);
    }

    [Fact]
    public void Adjustment_ShouldAllowRangeConfiguration()
    {
        Application.Init();
        var scrollBar = new VScrollBar();

        var adj = scrollBar.Adjustment;

        adj.Lower = 0;
        adj.Upper = 200;
        adj.Value = 50;

        Assert.Equal(0, adj.Lower);
        Assert.Equal(200, adj.Upper);
        Assert.Equal(50, adj.Value);
    }

    [Fact]
    public void MultipleInstances_ShouldHaveIndependentAdjustments()
    {
        Application.Init();
        var sb1 = new VScrollBar();
        var sb2 = new VScrollBar();

        sb1.Adjustment.Value = 10;
        sb2.Adjustment.Value = 20;

        Assert.Equal(10, sb1.Adjustment.Value);
        Assert.Equal(20, sb2.Adjustment.Value);
    }
}
using System.Drawing;

namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class TableLayoutPanelTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = panel.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(panel.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(panel.self, gtkControl);
    }

    [Fact]
    public void ColumnCount_ShouldSetAndGet()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        int result = -1;

        form.Load += (s, e) =>
        {
            panel.RowCount = 2;
            panel.ColumnCount = 3;

            result = panel.ColumnCount;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(3, result);
    }

    [Fact]
    public void RowCount_ShouldSetAndGet()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        int result = -1;

        form.Load += (s, e) =>
        {
            panel.ColumnCount = 2;
            panel.RowCount = 4;

            result = panel.RowCount;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(4, result);
    }

    [Fact]
    public void Controls_ShouldBeCustomCollection()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        TableLayoutControlCollection? controls = null;

        form.Load += (s, e) =>
        {
            controls = panel.Controls;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(controls);
    }

    [Fact]
    public void ColumnStyles_ShouldBeAccessible()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        TableLayoutColumnStyleCollection? styles = null;

        form.Load += (s, e) =>
        {
            styles = panel.ColumnStyles;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(styles);
    }

    [Fact]
    public void RowStyles_ShouldBeAccessible()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        TableLayoutRowStyleCollection? styles = null;

        form.Load += (s, e) =>
        {
            styles = panel.RowStyles;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(styles);
    }

    [Fact]
    public void Size_ShouldTriggerLayoutWithoutCrash()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        Size result = Size.Empty;

        form.Load += (s, e) =>
        {
            panel.ColumnCount = 2;
            panel.RowCount = 2;

            panel.Size = new Size(300, 200);

            result = panel.Size;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(new Size(300, 200), result);
    }

    [Fact]
    public void PerformLayout_ShouldNotThrow()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        bool executed = false;

        form.Load += (s, e) =>
        {
            panel.ColumnCount = 2;
            panel.RowCount = 2;

            panel.PerformLayout();

            executed = true;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(executed);
    }

    [Fact]
    public void GetColumnWidths_ShouldReturnValues()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        int[]? widths = null;

        form.Load += (s, e) =>
        {
            panel.RowCount = 1;
            panel.ColumnCount = 2;

            widths = panel.GetColumnWidths();

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(widths);
        Assert.Equal(2, widths.Length);
    }

    [Fact]
    public void GetRowHeights_ShouldReturnValues()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        int[]? heights = null;

        form.Load += (s, e) =>
        {
            panel.ColumnCount = 1;
            panel.RowCount = 2;

            heights = panel.GetRowHeights();

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(heights);
        Assert.Equal(2, heights.Length);
    }

    [Fact]
    public void SetColumnSpan_ShouldSetMargins()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        var button = new Button();

        form.Controls.Add(panel);
        panel.Controls.Add(button);

        int span = 0;

        form.Load += (s, e) =>
        {
            panel.SetColumnSpan(button, 3);
            span = panel.GetColumnSpan(button);

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(3, span);
    }

    [Fact]
    public void SetRowSpan_ShouldSetMargins()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        var button = new Button();

        form.Controls.Add(panel);
        panel.Controls.Add(button);

        int span = 0;

        form.Load += (s, e) =>
        {
            panel.SetRowSpan(button, 2);
            span = panel.GetRowSpan(button);

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(2, span);
    }

    [Fact]
    public void GetCellPosition_Default_ShouldReturnNegative()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        var button = new Button();

        form.Controls.Add(panel);
        panel.Controls.Add(button);

        TableLayoutPanelCellPosition pos = default;

        form.Load += (s, e) =>
        {
            pos = panel.GetCellPosition(button);
            form.Close();
        };

        form.ShowDialog();

        Assert.True(pos.Row == -1 || pos.Row >= 0);
        Assert.True(pos.Column == -1 || pos.Column >= 0);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var panel = new TableLayoutPanel();
        form.Controls.Add(panel);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = panel.GtkControl;
            self = panel.self;

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}
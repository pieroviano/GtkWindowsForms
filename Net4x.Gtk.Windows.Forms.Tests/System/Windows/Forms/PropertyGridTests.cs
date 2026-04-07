using System.Drawing;

namespace System.Windows.Forms;

extern alias gtk;

using System.ComponentModel;

[Collection("NonParallelCollection")]
public class PropertyGridTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = grid.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(grid.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(grid.self, gtkControl);
    }

    [Fact]
    public void SelectedObject_ShouldSetAndGet()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        object input = new object();
        object? result = null;

        form.Load += (s, e) =>
        {
            grid.SelectedObject = input;
            result = grid.SelectedObject;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(input, result);
    }

    [Fact]
    public void SelectedObject_Null_ShouldClear()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        object? result = new object();

        form.Load += (s, e) =>
        {
            grid.SelectedObject = null;
            result = grid.SelectedObject;
            form.Close();
        };

        form.ShowDialog();

        Assert.Null(result);
    }

    [Fact]
    public void SelectedObjects_ShouldCloneArray()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        object[] input = new object[] { new object() };
        object[]? result = null;

        form.Load += (s, e) =>
        {
            grid.SelectedObjects = input;
            result = grid.SelectedObjects;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.NotSame(input, result);
    }

    [Fact]
    public void BackColor_ShouldSetAndGet()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        var expected = Color.Blue;
        var result = Color.Empty;

        form.Load += (s, e) =>
        {
            grid.BackColor = expected;
            result = grid.BackColor;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void PropertySort_ShouldSetAndGet()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        var result = PropertySort.CategorizedAlphabetical;

        form.Load += (s, e) =>
        {
            grid.PropertySort = PropertySort.NoSort;
            result = grid.PropertySort;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(PropertySort.NoSort, result);
    }

    [Fact]
    public void Size_ShouldUpdate()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        var size = new Size(200, 300);
        var result = new Size();

        form.Load += (s, e) =>
        {
            grid.Size = size;
            result = grid.Size;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(size, result);
    }

    [Fact]
    public void BooleanProperties_ShouldSetAndGet()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        bool glyphs = false;
        bool helpVisible = false;
        bool toolbarVisible = false;

        form.Load += (s, e) =>
        {
            grid.CanShowVisualStyleGlyphs = false;
            grid.HelpVisible = false;
            grid.ToolbarVisible = false;

            glyphs = grid.CanShowVisualStyleGlyphs;
            helpVisible = grid.HelpVisible;
            toolbarVisible = grid.ToolbarVisible;

            form.Close();
        };

        form.ShowDialog();

        Assert.False(glyphs);
        Assert.False(helpVisible);
        Assert.False(toolbarVisible);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = grid.GtkControl;
            self = grid.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }

    [Fact]
    public void Methods_ShouldNotThrow()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        Exception? ex = null;

        form.Load += (s, e) =>
        {
            try
            {
                grid.CollapseAllGridItems();
                grid.Refresh();
                grid.RefreshTabs(PropertyTabScope.Component);
                grid.ResetSelectedProperty();
            }
            catch (Exception e2)
            {
                ex = e2;
            }

            form.Close();
        };

        form.ShowDialog();

        Assert.Null(ex);
    }

    [Fact]
    public void IComPropertyBrowser_Methods_ShouldThrow()
    {
        var form = new Form();
        var grid = new PropertyGrid();
        form.Controls.Add(grid);

        Exception? ex1 = null;
        Exception? ex2 = null;

        form.Load += (s, e) =>
        {
            var browser = (ComponentModel.Com2Interop.IComPropertyBrowser)grid;

            try { browser.DropDownDone(); } catch (Exception e1) { ex1 = e1; }
            try { browser.EnsurePendingChangesCommitted(); } catch (Exception e2) { ex2 = e2; }

            form.Close();
        };

        form.ShowDialog();

        Assert.IsType<NotImplementedException>(ex1);
        Assert.IsType<NotImplementedException>(ex2);
    }
}
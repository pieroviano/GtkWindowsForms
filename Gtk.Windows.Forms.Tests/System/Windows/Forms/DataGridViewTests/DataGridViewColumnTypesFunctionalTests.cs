using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms.DataGridViewTests;

/// <summary>Behavioural tests for the specialised <see cref="DataGridViewColumn"/> types. Each column type
/// exists to stamp out a particular cell type, so its CellTemplate/CellType must match, and cells created
/// from it must be independent instances that carry values.</summary>
[Collection("NonParallelCollection")]
public class DataGridViewColumnTypesFunctionalTests
{
    /// <summary>The column kinds under test, named so the theory data carries no live GTK widget.</summary>
    public enum ColumnKind
    {
        TextBox,
        Button,
        CheckBox,
        ComboBox,
        Image,
        Link
    }

    private static DataGridViewColumn Create(ColumnKind kind) => kind switch
    {
        ColumnKind.TextBox => new DataGridViewTextBoxColumn(),
        ColumnKind.Button => new DataGridViewButtonColumn(),
        ColumnKind.CheckBox => new DataGridViewCheckBoxColumn(),
        ColumnKind.ComboBox => new DataGridViewComboBoxColumn(),
        ColumnKind.Image => new DataGridViewImageColumn(),
        _ => new DataGridViewLinkColumn()
    };

    private static Type ExpectedCellType(ColumnKind kind) => kind switch
    {
        ColumnKind.TextBox => typeof(DataGridViewTextBoxCell),
        ColumnKind.Button => typeof(DataGridViewButtonCell),
        ColumnKind.CheckBox => typeof(DataGridViewCheckBoxCell),
        ColumnKind.ComboBox => typeof(DataGridViewComboBoxCell),
        ColumnKind.Image => typeof(DataGridViewImageCell),
        _ => typeof(DataGridViewLinkCell)
    };

    [Theory]
    [InlineData(ColumnKind.TextBox)]
    [InlineData(ColumnKind.Button)]
    [InlineData(ColumnKind.CheckBox)]
    [InlineData(ColumnKind.ComboBox)]
    [InlineData(ColumnKind.Image)]
    [InlineData(ColumnKind.Link)]
    public void EachColumnType_StampsOutItsOwnCellType(ColumnKind kind)
    {
        var column = Create(kind);
        var expected = ExpectedCellType(kind);

        Assert.Equal(expected, column.CellType);
        Assert.IsType(expected, column.CellTemplate);
    }

    [Theory]
    [InlineData(ColumnKind.TextBox)]
    [InlineData(ColumnKind.Button)]
    [InlineData(ColumnKind.CheckBox)]
    [InlineData(ColumnKind.ComboBox)]
    [InlineData(ColumnKind.Image)]
    [InlineData(ColumnKind.Link)]
    public void NewCell_ProducesAFreshCellOfTheColumnsType(ColumnKind kind)
    {
        var column = Create(kind);

        var first = column.NewCell();
        var second = column.NewCell();

        Assert.IsType(ExpectedCellType(kind), first);
        Assert.NotSame(first, second);
    }

    [Theory]
    [InlineData(ColumnKind.TextBox)]
    [InlineData(ColumnKind.Button)]
    [InlineData(ColumnKind.ComboBox)]
    [InlineData(ColumnKind.Link)]
    public void NewCell_WithAValue_CarriesThatValue(ColumnKind kind)
    {
        var column = Create(kind);

        var cell = column.NewCell("payload");

        Assert.IsType(ExpectedCellType(kind), cell);
        Assert.Equal("payload", cell!.Value);
    }

    [Fact]
    public void ComboBoxColumn_ItemsCollectionBehavesLikeAList()
    {
        using var column = new DataGridViewComboBoxColumn();

        column.Items.Add("one");
        column.Items.Add("two");

        Assert.Equal(2, column.Items.Count);
        Assert.Equal("one", column.Items[0]);
        Assert.True(column.Items.Contains("two"), "an added item should be contained");
        Assert.Equal(1, column.Items.IndexOf("two"));

        column.Items.Remove("one");
        Assert.Single(column.Items);

        column.Items.Clear();
        Assert.Empty(column.Items);
    }

    [Fact]
    public void CheckBoxColumn_CellValueRoundTripsAsABoolean()
    {
        using var grid = new DataGridView();
        grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "done" });
        grid.Rows.Add(true);

        Assert.Equal(true, grid.Rows[0].Cells[0].Value);

        grid.Rows[0].Cells[0].Value = false;

        Assert.Equal(false, grid.Rows[0].Cells[0].Value);
    }

    [Fact]
    public void ColumnsOfDifferentTypes_CoexistInOneGrid()
    {
        using var grid = new DataGridView();
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "text" });
        grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "flag" });
        grid.Columns.Add(new DataGridViewButtonColumn { Name = "action" });

        grid.Rows.Add("hello", true, "Go");

        Assert.Equal(3, grid.Rows[0].Cells.Count);
        Assert.IsType<DataGridViewTextBoxCell>(grid.Rows[0].Cells[0]);
        Assert.IsType<DataGridViewCheckBoxCell>(grid.Rows[0].Cells[1]);
        Assert.IsType<DataGridViewButtonCell>(grid.Rows[0].Cells[2]);
        Assert.Equal("hello", grid.Rows[0].Cells["text"].Value);
    }

    [Fact]
    public void Column_FillWeightRoundTrips()
    {
        using var column = new DataGridViewTextBoxColumn { FillWeight = 75f };

        Assert.Equal(75f, column.FillWeight, 2);
    }

    [Fact]
    public void Column_DisplayIndexReflectsThePositionWithinTheGrid()
    {
        // A column that belongs to no grid has no display position, matching WinForms.
        using var orphan = new DataGridViewTextBoxColumn();
        Assert.Equal(-1, orphan.DisplayIndex);

        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Columns.Add("b", "B");

        Assert.Equal(0, grid.Columns[0].DisplayIndex);
        Assert.Equal(1, grid.Columns[1].DisplayIndex);
    }

    [Fact]
    public void Column_SortModeAndAutoSizeModeRoundTrip()
    {
        using var column = new DataGridViewTextBoxColumn
        {
            SortMode = DataGridViewColumnSortMode.Automatic,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };

        Assert.Equal(DataGridViewColumnSortMode.Automatic, column.SortMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.Fill, column.AutoSizeMode);
    }

    [Fact]
    public void Column_MinimumWidthAndDividerWidthRoundTrip()
    {
        using var column = new DataGridViewTextBoxColumn { MinimumWidth = 40, DividerWidth = 3 };

        Assert.Equal(40, column.MinimumWidth);
        Assert.Equal(3, column.DividerWidth);
    }

    [Fact]
    public void Column_ToolTipTextRoundTrips()
    {
        using var column = new DataGridViewTextBoxColumn { ToolTipText = "explains" };

        Assert.Equal("explains", column.ToolTipText);
    }

    [Fact]
    public void Column_AddedToAGrid_PointsBackAtIt()
    {
        using var grid = new DataGridView();
        var column = new DataGridViewTextBoxColumn { Name = "a" };

        grid.Columns.Add(column);

        Assert.Same(grid, column.DataGridView);
    }

    [Fact]
    public void Cell_StyleAndErrorTextRoundTrip()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");
        var cell = grid.Rows[0].Cells[0];

        cell.ErrorText = "bad value";
        cell.ToolTipText = "hint";

        Assert.Equal("bad value", cell.ErrorText);
        Assert.Equal("hint", cell.ToolTipText);
    }

    [Fact]
    public void Cell_ReadOnlyRoundTrips()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");

        grid.Rows[0].Cells[0].ReadOnly = true;

        Assert.True(grid.Rows[0].Cells[0].ReadOnly);
    }

    [Fact]
    public void Row_HeightAndVisibilityRoundTrip()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");

        grid.Rows[0].Height = 44;
        grid.Rows[0].Visible = false;

        Assert.Equal(44, grid.Rows[0].Height);
        Assert.False(grid.Rows[0].Visible);
    }

    [Fact]
    public void Row_DataGridViewPointsBackAtTheOwningGrid()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");

        Assert.Same(grid, grid.Rows[0].DataGridView);
    }

    [Fact]
    public void GridStyleProperties_RoundTrip()
    {
        using var grid = new DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = false,
            AllowUserToResizeColumns = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            RowHeadersWidth = 60,
            AutoGenerateColumns = false,
            MultiSelect = false
        };

        Assert.False(grid.AllowUserToAddRows);
        Assert.False(grid.AllowUserToDeleteRows);
        Assert.False(grid.AllowUserToOrderColumns);
        Assert.False(grid.AllowUserToResizeColumns);
        Assert.False(grid.AllowUserToResizeRows);
        Assert.True(grid.ReadOnly);
        Assert.Equal(60, grid.RowHeadersWidth);
        Assert.False(grid.AutoGenerateColumns);
        Assert.False(grid.MultiSelect);
    }

    [Fact]
    public void SelectionMode_RoundTrips()
    {
        using var grid = new DataGridView { SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        Assert.Equal(DataGridViewSelectionMode.FullRowSelect, grid.SelectionMode);
    }

    [Fact]
    public void NoSelection_MeansEmptySelectionCollectionsAndNoCurrentRow()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Rows.Add("value");

        Assert.Empty(grid.SelectedRows);
        Assert.Empty(grid.SelectedCells);
        Assert.Empty(grid.SelectedColumns);
        Assert.Null(grid.CurrentRow);
    }
}

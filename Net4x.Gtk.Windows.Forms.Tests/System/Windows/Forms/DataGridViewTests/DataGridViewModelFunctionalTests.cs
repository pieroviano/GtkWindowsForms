using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms.DataGridViewTests;

/// <summary>Behavioural tests for the <see cref="DataGridView"/> row/column/cell model. A grid is first and
/// foremost a data structure: adding a column must give every row a matching cell, values must round-trip
/// through both the index and the column-name indexer, and removals must keep rows and cells in step.</summary>
[Collection("NonParallelCollection")]
public class DataGridViewModelFunctionalTests
{
    private static DataGridView GridWithColumns(params string[] names)
    {
        var grid = new DataGridView();
        foreach (var n in names)
        {
            grid.Columns.Add(n, n + " header");
        }

        return grid;
    }

    [Fact]
    public void NewGrid_HasNoRowsAndNoColumns()
    {
        using var grid = new DataGridView();

        Assert.Empty(grid.Rows);
        Assert.Empty(grid.Columns);
        Assert.Equal(0, grid.RowCount);
    }

    [Fact]
    public void Columns_Add_ByNameAndHeader_StoresBoth()
    {
        using var grid = GridWithColumns("id");

        Assert.Single(grid.Columns);
        Assert.Equal("id", grid.Columns[0].Name);
        Assert.Equal("id header", grid.Columns[0].HeaderText);
    }

    [Fact]
    public void Columns_NameIndexer_FindsTheSameColumnAsTheOrdinalIndexer()
    {
        using var grid = GridWithColumns("first", "second");

        Assert.Same(grid.Columns[0], grid.Columns["first"]);
        Assert.Same(grid.Columns[1], grid.Columns["second"]);
    }

    [Fact]
    public void Columns_TrackTheirIndex()
    {
        using var grid = GridWithColumns("a", "b", "c");

        Assert.Equal(0, grid.Columns[0].Index);
        Assert.Equal(1, grid.Columns[1].Index);
        Assert.Equal(2, grid.Columns[2].Index);
    }

    [Fact]
    public void Rows_AddWithValues_CreatesOneCellPerColumnAndKeepsTheValues()
    {
        using var grid = GridWithColumns("a", "b");

        var index = grid.Rows.Add("left", "right");

        Assert.Equal(0, index);
        Assert.Single(grid.Rows);
        Assert.Equal(2, grid.Rows[0].Cells.Count);
        Assert.Equal("left", grid.Rows[0].Cells[0].Value);
        Assert.Equal("right", grid.Rows[0].Cells[1].Value);
    }

    [Fact]
    public void Cells_NameIndexer_ReachesTheSameCellAsTheOrdinalIndexer()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        Assert.Same(grid.Rows[0].Cells[0], grid.Rows[0].Cells["a"]);
        Assert.Equal("right", grid.Rows[0].Cells["b"].Value);
    }

    [Fact]
    public void Cells_ReportTheirColumnAndRowIndex()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");
        grid.Rows.Add("l2", "r2");

        Assert.Equal(1, grid.Rows[1].Cells[1].ColumnIndex);
        Assert.Equal(1, grid.Rows[1].Cells[1].RowIndex);
    }

    [Fact]
    public void Rows_Add_WithoutValues_AppendsAnEmptyRow()
    {
        using var grid = GridWithColumns("a");

        var index = grid.Rows.Add();

        Assert.Equal(0, index);
        Assert.Single(grid.Rows);
    }

    [Fact]
    public void Rows_AddCount_AppendsThatManyRows()
    {
        using var grid = GridWithColumns("a");

        grid.Rows.Add(3);

        Assert.Equal(3, grid.Rows.Count);
    }

    [Fact]
    public void Rows_TrackTheirIndex()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");
        grid.Rows.Add("three");

        Assert.Equal(0, grid.Rows[0].Index);
        Assert.Equal(1, grid.Rows[1].Index);
        Assert.Equal(2, grid.Rows[2].Index);
    }

    [Fact]
    public void Rows_RemoveAt_DropsThatRowAndReindexesTheRest()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");
        grid.Rows.Add("three");

        grid.Rows.RemoveAt(1);

        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("one", grid.Rows[0].Cells[0].Value);
        Assert.Equal("three", grid.Rows[1].Cells[0].Value);
        Assert.Equal(1, grid.Rows[1].Index);
    }

    [Fact]
    public void Rows_Remove_DropsTheGivenRow()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");
        var second = grid.Rows[1];

        grid.Rows.Remove(second);

        Assert.Single(grid.Rows);
        Assert.False(grid.Rows.Contains(second), "the removed row should no longer be in the collection");
    }

    [Fact]
    public void Rows_Clear_EmptiesTheGrid_ButKeepsTheColumns()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("one", "two");

        grid.Rows.Clear();

        Assert.Empty(grid.Rows);
        Assert.Equal(2, grid.Columns.Count);
    }

    [Fact]
    public void Rows_Contains_DistinguishesMembers()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        var stranger = new DataGridViewRow();

        Assert.True(grid.Rows.Contains(grid.Rows[0]), "an added row should be contained");
        Assert.False(grid.Rows.Contains(stranger), "a row that was never added should not be contained");
    }

    [Fact]
    public void Rows_CopyTo_CopiesEveryRow()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");

        var target = new DataGridViewRow[2];
        grid.Rows.CopyTo(target, 0);

        Assert.Same(grid.Rows[0], target[0]);
        Assert.Same(grid.Rows[1], target[1]);
    }

    [Fact]
    public void Rows_Enumerator_YieldsEveryRowInOrder()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");

        var seen = new List<object?>();
        foreach (DataGridViewRow row in grid.Rows)
        {
            seen.Add(row.Cells[0].Value);
        }

        Assert.Equal(new object?[] { "one", "two" }, seen);
    }

    [Fact]
    public void Rows_AddCopy_DuplicatesTheSourceRowValues()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        var index = grid.Rows.AddCopy(0);

        Assert.Equal(1, index);
        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("left", grid.Rows[1].Cells[0].Value);
        Assert.NotSame(grid.Rows[0], grid.Rows[1]);
    }

    [Fact]
    public void Rows_AddCopies_DuplicatesTheSourceRowSeveralTimes()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("seed");

        grid.Rows.AddCopies(0, 3);

        Assert.Equal(4, grid.Rows.Count);
        Assert.Equal("seed", grid.Rows[3].Cells[0].Value);
    }

    [Fact]
    public void Rows_AddRange_AppendsAllOfThem()
    {
        using var grid = GridWithColumns("a");
        var a = new DataGridViewRow();
        var b = new DataGridViewRow();

        grid.Rows.AddRange(a, b);

        Assert.Equal(2, grid.Rows.Count);
        Assert.Same(a, grid.Rows[0]);
        Assert.Same(b, grid.Rows[1]);
    }

    [Fact]
    public void Rows_Insert_PlacesTheRowAtTheGivenPosition()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("three");

        grid.Rows.Insert(1, new DataGridViewRow());

        Assert.Equal(3, grid.Rows.Count);
        Assert.Equal("three", grid.Rows[2].Cells[0].Value);
    }

    [Fact]
    public void RowCount_ReflectsTheRowsCollection()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("one");
        grid.Rows.Add("two");

        Assert.Equal(grid.Rows.Count, grid.RowCount);
    }

    [Fact]
    public void RowCount_Setter_GrowsAndShrinksTheGrid()
    {
        using var grid = GridWithColumns("a");

        grid.RowCount = 3;
        Assert.Equal(3, grid.Rows.Count);

        grid.RowCount = 1;
        Assert.Single(grid.Rows);
    }

    [Fact]
    public void Columns_Remove_DropsTheColumn()
    {
        using var grid = GridWithColumns("a", "b");
        var second = grid.Columns[1];

        grid.Columns.Remove(second);

        Assert.Single(grid.Columns);
        Assert.Equal("a", grid.Columns[0].Name);
    }

    [Fact]
    public void Columns_RemoveAt_DropsThatColumn()
    {
        using var grid = GridWithColumns("a", "b", "c");

        grid.Columns.RemoveAt(1);

        Assert.Equal(2, grid.Columns.Count);
        Assert.Equal("a", grid.Columns[0].Name);
        Assert.Equal("c", grid.Columns[1].Name);
    }

    [Fact]
    public void Columns_Clear_RemovesEveryColumn()
    {
        using var grid = GridWithColumns("a", "b");

        grid.Columns.Clear();

        Assert.Empty(grid.Columns);
    }

    [Fact]
    public void Columns_AddRange_AppendsAllOfThem()
    {
        using var grid = new DataGridView();
        var a = new DataGridViewTextBoxColumn { Name = "a" };
        var b = new DataGridViewTextBoxColumn { Name = "b" };

        grid.Columns.AddRange(new DataGridViewColumn[] { a, b });

        Assert.Equal(2, grid.Columns.Count);
        Assert.Equal("a", grid.Columns[0].Name);
    }

    [Fact]
    public void Column_CellType_MatchesTheCellTemplate()
    {
        using var column = new DataGridViewTextBoxColumn();

        Assert.Equal(typeof(DataGridViewTextBoxCell), column.CellType);
        Assert.IsType<DataGridViewTextBoxCell>(column.CellTemplate);
    }

    [Fact]
    public void Column_PropertiesRoundTrip()
    {
        using var column = new DataGridViewTextBoxColumn
        {
            Name = "amount",
            HeaderText = "Amount",
            DataPropertyName = "Amount",
            Width = 123,
            Frozen = true,
            ReadOnly = true,
            Visible = false
        };

        Assert.Equal("amount", column.Name);
        Assert.Equal("Amount", column.HeaderText);
        Assert.Equal("Amount", column.DataPropertyName);
        Assert.Equal(123, column.Width);
        Assert.True(column.Frozen);
        Assert.True(column.ReadOnly);
        Assert.False(column.Visible);
    }

    [Fact]
    public void Cell_ValueRoundTrips()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("initial");

        grid.Rows[0].Cells[0].Value = 42;

        Assert.Equal(42, grid.Rows[0].Cells[0].Value);
    }

    [Fact]
    public void Cell_OwningRowAndColumnPointBack()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        var cell = grid.Rows[0].Cells[1];

        Assert.Same(grid.Rows[0], cell.OwningRow);
        Assert.Equal(1, cell.ColumnIndex);
    }

    [Fact]
    public void Cells_IndexOf_ReportsThePosition()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        Assert.Equal(1, grid.Rows[0].Cells.IndexOf(grid.Rows[0].Cells[1]));
    }

    [Fact]
    public void Cells_Contains_DistinguishesMembers()
    {
        using var grid = GridWithColumns("a");
        grid.Rows.Add("only");
        var stranger = new DataGridViewTextBoxCell();

        Assert.True(grid.Rows[0].Cells.Contains(grid.Rows[0].Cells[0]), "an owned cell should be contained");
        Assert.False(grid.Rows[0].Cells.Contains(stranger), "a foreign cell should not be contained");
    }

    [Fact]
    public void Cells_CopyTo_CopiesEveryCell()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        var target = new DataGridViewCell[2];
        grid.Rows[0].Cells.CopyTo(target, 0);

        Assert.Same(grid.Rows[0].Cells[0], target[0]);
        Assert.Same(grid.Rows[0].Cells[1], target[1]);
    }

    [Fact]
    public void Row_Cells_Enumerator_YieldsEveryCell()
    {
        using var grid = GridWithColumns("a", "b");
        grid.Rows.Add("left", "right");

        var values = new List<object?>();
        foreach (DataGridViewCell cell in grid.Rows[0].Cells)
        {
            values.Add(cell.Value);
        }

        Assert.Equal(new object?[] { "left", "right" }, values);
    }
}

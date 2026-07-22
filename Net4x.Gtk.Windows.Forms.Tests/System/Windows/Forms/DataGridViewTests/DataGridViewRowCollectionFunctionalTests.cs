using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms.DataGridViewTests;

/// <summary>Behavioural tests for <see cref="DataGridViewRowCollection"/>: insertion, the
/// state-filtered navigation family (GetFirstRow/GetNextRow/GetPreviousRow/GetLastRow/GetRowCount/
/// GetRowsHeight) and copy-based adds, plus <see cref="DataGridViewColumn.Clone"/> and
/// <c>DisplayIndex</c>.</summary>
[Collection("NonParallelCollection")]
public class DataGridViewRowCollectionFunctionalTests
{
    private static DataGridView GridWithRows(params string[] values)
    {
        var grid = new DataGridView();
        grid.Columns.Add("c", "C");
        foreach (var value in values)
        {
            grid.Rows.Add(value);
        }

        return grid;
    }

    [Fact]
    public void InsertPlacesValuesAtTheIndex()
    {
        using var grid = GridWithRows("first", "third");

        grid.Rows.Insert(1, "second");

        Assert.Equal("first", grid.Rows[0].Cells[0].Value);
        Assert.Equal("second", grid.Rows[1].Cells[0].Value);
        Assert.Equal("third", grid.Rows[2].Cells[0].Value);
    }

    [Fact]
    public void InsertCountAddsThatManyEmptyRows()
    {
        using var grid = GridWithRows("only");

        grid.Rows.Insert(0, 2);

        Assert.Equal(3, grid.Rows.Count);
        Assert.Equal("only", grid.Rows[2].Cells[0].Value);
    }

    [Fact]
    public void InsertRangeAddsTheRowsInOrder()
    {
        using var grid = GridWithRows("z");

        grid.Rows.InsertRange(0, new DataGridViewRow(), new DataGridViewRow());

        Assert.Equal(3, grid.Rows.Count);
        Assert.Equal("z", grid.Rows[2].Cells[0].Value);
    }

    [Fact]
    public void StateFilteredNavigationSkipsInvisibleRows()
    {
        using var grid = GridWithRows("a", "b", "c", "d");
        grid.Rows[0].Visible = false;
        grid.Rows[2].Visible = false;

        Assert.Equal(1, grid.Rows.GetFirstRow(DataGridViewElementStates.Visible));
        Assert.Equal(3, grid.Rows.GetNextRow(1, DataGridViewElementStates.Visible));
        Assert.Equal(-1, grid.Rows.GetNextRow(3, DataGridViewElementStates.Visible));
        Assert.Equal(1, grid.Rows.GetPreviousRow(3, DataGridViewElementStates.Visible));
        Assert.Equal(3, grid.Rows.GetLastRow(DataGridViewElementStates.Visible));
        Assert.Equal(2, grid.Rows.GetRowCount(DataGridViewElementStates.Visible));
    }

    [Fact]
    public void ExcludeFiltersRemoveMatchingRows()
    {
        using var grid = GridWithRows("a", "b", "c");
        grid.Rows[1].ReadOnly = true;

        var first = grid.Rows.GetFirstRow(
            DataGridViewElementStates.Visible, DataGridViewElementStates.ReadOnly);
        var next = grid.Rows.GetNextRow(first,
            DataGridViewElementStates.Visible, DataGridViewElementStates.ReadOnly);

        Assert.Equal(0, first);
        Assert.Equal(2, next);
    }

    [Fact]
    public void GetRowsHeightSumsOnlyMatchingRows()
    {
        using var grid = GridWithRows("a", "b");
        grid.Rows[0].Height = 30;
        grid.Rows[1].Height = 50;
        grid.Rows[1].Visible = false;

        Assert.Equal(30, grid.Rows.GetRowsHeight(DataGridViewElementStates.Visible));
    }

    [Fact]
    public void AddCopiesDuplicatesTheSourceRow()
    {
        using var grid = GridWithRows("template");

        grid.Rows.AddCopies(0, 2);

        Assert.Equal(3, grid.Rows.Count);
    }

    [Fact]
    public void TheIListFaceAnswersContainsAndIndexOf()
    {
        using var grid = GridWithRows("a", "b");
        var row = grid.Rows[1];

        global::System.Collections.IList list = grid.Rows;
        Assert.True(list.Contains(row));
        Assert.Equal(1, list.IndexOf(row));
    }

    [Fact]
    public void ClearEmptiesTheCollection()
    {
        using var grid = GridWithRows("a", "b");

        grid.Rows.Clear();

        Assert.Empty(grid.Rows);
    }

    // ---- DataGridViewColumn -----------------------------------------------

    [Fact]
    public void CloneCopiesTheColumnConfiguration()
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = "col",
            HeaderText = "Header",
            DataPropertyName = "Prop",
            Width = 123
        };

        var clone = Assert.IsAssignableFrom<DataGridViewColumn>(column.Clone());

        Assert.NotSame(column, clone);
        Assert.Equal("col", clone.Name);
        Assert.Equal("Header", clone.HeaderText);
        Assert.Equal("Prop", clone.DataPropertyName);
        Assert.Equal(123, clone.Width);
    }

    [Fact]
    public void DisplayIndexRoundTripsWithinTheGrid()
    {
        using var grid = new DataGridView();
        grid.Columns.Add("a", "A");
        grid.Columns.Add("b", "B");

        grid.Columns[1].DisplayIndex = 0;

        Assert.Equal(0, grid.Columns[1].DisplayIndex);
    }
}

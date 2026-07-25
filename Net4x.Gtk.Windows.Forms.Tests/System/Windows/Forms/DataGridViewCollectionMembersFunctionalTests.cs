using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the collection and style members that still threw
/// <see cref="NotImplementedException" />: the <see cref="IList" /> indexer of
/// <see cref="DataGridViewCellCollection" />, <see cref="ICloneable" /> on
/// <see cref="DataGridViewCellStyle" />, and the enumerator of the internal cell linked list. Each is
/// reached only on a specific operation - binding a grid through the non-generic list interface, cloning a
/// style through <see cref="ICloneable" /> - which is exactly why they survived so long.
/// </summary>
[Collection("NonParallelCollection")]
public class DataGridViewCollectionMembersFunctionalTests
{
    private static DataGridViewCellCollection CellsOfARow()
    {
        var row = new DataGridViewRow();
        row.Cells.Add(new DataGridViewTextBoxCell());
        row.Cells.Add(new DataGridViewTextBoxCell());

        return row.Cells;
    }

    [Fact]
    public void IListIndexer_ReadsTheSameCellAsTheTypedIndexer()
    {
        var cells = CellsOfARow();

        Assert.Same(cells[0], ((IList)cells)[0]);
        Assert.Same(cells[1], ((IList)cells)[1]);
    }

    [Fact]
    public void IListIndexer_WritesThroughToTheTypedIndexer()
    {
        var cells = CellsOfARow();
        var replacement = new DataGridViewTextBoxCell();

        ((IList)cells)[0] = replacement;

        Assert.Same(replacement, cells[0]);
    }

    [Fact]
    public void IListIndexer_RejectsAValueThatIsNotACell()
    {
        var cells = CellsOfARow();

        Assert.Throws<ArgumentException>(() => ((IList)cells)[0] = "not a cell");
    }

    [Fact]
    public void IListIndexer_RejectsNullTheWayTheTypedIndexerDoes()
    {
        var cells = CellsOfARow();

        Assert.Throws<ArgumentNullException>(() => ((IList)cells)[0] = null!);
    }

    [Fact]
    public void ICloneable_Clone_ProducesAnIndependentCopyOfTheStyle()
    {
        var style = new DataGridViewCellStyle
        {
            BackColor = Color.Red,
            ForeColor = Color.Blue,
            Format = "N2",
            Padding = new Padding(1, 2, 3, 4),
            WrapMode = DataGridViewTriState.True
        };

        var clone = Assert.IsType<DataGridViewCellStyle>(((ICloneable)style).Clone());

        Assert.NotSame(style, clone);
        Assert.Equal(style.BackColor, clone.BackColor);
        Assert.Equal(style.ForeColor, clone.ForeColor);
        Assert.Equal(style.Format, clone.Format);
        Assert.Equal(style.Padding, clone.Padding);
        Assert.Equal(style.WrapMode, clone.WrapMode);

        clone.Format = "N4";
        Assert.Equal("N2", style.Format);
    }

    [Fact]
    public void CellLinkedList_EnumeratesFromTheHead()
    {
        var list = new DataGridViewCellLinkedList();
        var first = new DataGridViewTextBoxCell();
        var second = new DataGridViewTextBoxCell();

        list.Add(first);
        list.Add(second);

        Assert.Equal(2, list.Count);
        // Insertion is at the head, so the most recent addition comes out first.
        Assert.Same(second, list.HeadCell);
        Assert.Equal(new object[] { second, first }, list.Cast<object>().ToArray());
    }

    [Fact]
    public void CellLinkedList_ContainsAndRemoveTrackTheContents()
    {
        var list = new DataGridViewCellLinkedList();
        var cell = new DataGridViewTextBoxCell();
        var other = new DataGridViewTextBoxCell();
        list.Add(cell);

        Assert.True(list.Contains(cell));
        Assert.False(list.Contains(other));

        Assert.True(list.Remove(cell));
        Assert.False(list.Remove(cell));
        Assert.Equal(0, list.Count);
        Assert.Null(list.HeadCell);
        Assert.Empty(list.Cast<object>());
    }

    [Fact]
    public void CellLinkedList_RemoveAllCellsAtBand_DropsOnlyThatColumn()
    {
        var list = new DataGridViewCellLinkedList();
        var onColumnZero = new DataGridViewTextBoxCell { ColumnIndex = 0 };
        var onColumnOne = new DataGridViewTextBoxCell { ColumnIndex = 1 };
        list.Add(onColumnZero);
        list.Add(onColumnOne);

        var removed = list.RemoveAllCellsAtBand(column: true, bandIndex: 1);

        Assert.Equal(1, removed);
        Assert.Equal(1, list.Count);
        Assert.Same(onColumnZero, list.HeadCell);
    }

    [Fact]
    public void CellLinkedList_Clear_EmptiesIt()
    {
        var list = new DataGridViewCellLinkedList();
        list.Add(new DataGridViewTextBoxCell());
        list.Add(new DataGridViewTextBoxCell());

        list.Clear();

        Assert.Equal(0, list.Count);
        Assert.Empty(list.Cast<object>());
    }
}

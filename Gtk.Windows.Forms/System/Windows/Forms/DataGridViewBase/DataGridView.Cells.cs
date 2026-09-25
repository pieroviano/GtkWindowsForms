using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// The cell-level members of <see cref="DataGridView" />: how many columns it has, which cell has the cursor,
/// whether the header bands show, and which cell is under a point.
/// </summary>
public partial class DataGridView
{
    private bool _rowHeadersVisible = true;

    /// <summary>
    /// Gets or sets a value indicating whether the column header band is shown.
    /// </summary>
    [DefaultValue(true)]
    public bool ColumnHeadersVisible
    {
        get => self.GridView.HeadersVisible;
        set => self.GridView.HeadersVisible = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the row header band is shown.
    /// </summary>
    /// <remarks>
    /// The GTK tree view has no row header band to hide, so this carries the caller's choice; a grid that turns it
    /// off already looks the way it asked for.
    /// </remarks>
    [DefaultValue(true)]
    public bool RowHeadersVisible
    {
        get => _rowHeadersVisible;
        set => _rowHeadersVisible = value;
    }

    /// <summary>
    /// Gets or sets the number of columns. Growing it appends text box columns, shrinking it drops the last ones,
    /// as WinForms does.
    /// </summary>
    public int ColumnCount
    {
        get => Columns.Count;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            while (Columns.Count > value)
            {
                Columns.RemoveAt(Columns.Count - 1);
            }
            while (Columns.Count < value)
            {
                Columns.Add(new DataGridViewTextBoxColumn());
            }
        }
    }

    /// <summary>
    /// Gets or sets the cell with the cursor, or <see langword="null" /> when the grid has none.
    /// </summary>
    /// <remarks>Setting it moves the tree view's cursor, which is what gives the cell the keyboard.</remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataGridViewCell? CurrentCell
    {
        get
        {
            self.GridView.GetCursor(out var path, out var column);
            if (path == null || path.Indices.Length == 0)
            {
                return null;
            }
            var row = path.Indices[0];
            var col = ColumnIndexOf(column);
            return Cell(row, col < 0 ? 0 : col);
        }
        set
        {
            if (value == null || value.RowIndex < 0 || value.ColumnIndex < 0)
            {
                return;
            }
            var columns = self.GridView.Columns;
            if (value.RowIndex >= Rows.Count || value.ColumnIndex >= columns.Length)
            {
                return;
            }
            using var path = new Gtk.TreePath(new[] { value.RowIndex });
            self.GridView.SetCursor(path, columns[value.ColumnIndex], false);
        }
    }

    /// <summary>The cell at a point in the grid's client area, or a miss when the point is not over one.</summary>
    /// <param name="x">Client x, in pixels.</param>
    /// <param name="y">Client y, in pixels.</param>
    public HitTestInfo HitTest(int x, int y)
    {
        self.GridView.ConvertWidgetToBinWindowCoords(x, y, out var binX, out var binY);
        if (!self.GridView.GetPathAtPos(binX, binY, out var path, out var column) || path == null)
        {
            return HitTestInfo.Nowhere;
        }
        using (path)
        {
            var row = path.Indices.Length == 0 ? -1 : path.Indices[0];
            return new HitTestInfo(ColumnIndexOf(column), row,
                row < 0 ? DataGridViewHitTestType.ColumnHeader : DataGridViewHitTestType.Cell);
        }
    }

    /// <summary>The index of a native column among this grid's columns, or -1 when it is not one of them.</summary>
    private int ColumnIndexOf(Gtk.TreeViewColumn? column)
    {
        if (column == null)
        {
            return -1;
        }
        var columns = self.GridView.Columns;
        for (var i = 0; i < columns.Length; i++)
        {
            if (ReferenceEquals(columns[i], column))
            {
                return i;
            }
        }
        return -1;
    }

    private DataGridViewCell? Cell(int row, int column) =>
        row >= 0 && row < Rows.Count && column >= 0 && column < Rows[row].Cells.Count ? Rows[row].Cells[column] : null;

    /// <summary>Where a point fell: the cell under it, or <see cref="Nowhere" /> when it was over none.</summary>
    public sealed class HitTestInfo
    {
        /// <summary>A point that is over no part of the grid.</summary>
        public static readonly HitTestInfo Nowhere = new(-1, -1, DataGridViewHitTestType.None);

        internal HitTestInfo(int columnIndex, int rowIndex, DataGridViewHitTestType type)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Type = type;
        }

        /// <summary>The column under the point, or -1.</summary>
        public int ColumnIndex { get; }

        /// <summary>The row under the point, or -1.</summary>
        public int RowIndex { get; }

        /// <summary>What part of the grid the point fell on.</summary>
        public DataGridViewHitTestType Type { get; }

        /// <inheritdoc />
        public override string ToString() => $"{{ Type={Type}, Column={ColumnIndex}, Row={RowIndex} }}";
    }
}

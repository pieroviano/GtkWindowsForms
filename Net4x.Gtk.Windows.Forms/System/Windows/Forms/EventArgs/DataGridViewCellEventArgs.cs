namespace System.Windows.Forms;

/// <summary>
/// Provides data for
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// events related to cell and row operations.
/// </summary>
public class DataGridViewCellEventArgs : EventArgs
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewCellEventArgs"/> class.
    /// </summary>
    /// <param name="columnIndex">Index of the column.</param>
    /// <param name="rowIndex">Index of the row.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">columnIndex</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">rowIndex</exception>
    public DataGridViewCellEventArgs(int columnIndex, int rowIndex)
    {
        if (columnIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }
        if (rowIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewCellEventArgs"/> class.
    /// </summary>
    /// <param name="dataGridViewCell">The data grid view cell.</param>
    internal DataGridViewCellEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
    {
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>The index of the column.</value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>The index of the row.</value>
    public int RowIndex { get; }

}

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowDividerDoubleClick" />
/// event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewRowDividerDoubleClickEventArgs"/> class.
    /// </summary>
    /// <param name="rowIndex">Index of the row.</param>
    /// <param name="e">The <see cref="HandledMouseEventArgs"/> instance containing the event data.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">rowIndex</exception>
    /// <exception cref="System.ArgumentNullException">e</exception>
    public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
    {
        if (rowIndex < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        RowIndex = rowIndex;
    }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>The index of the row.</value>
    public int RowIndex { get; }

}

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowStateChanged" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewRowStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewRowStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="dataGridViewRow">The data grid view row.</param>
    /// <param name="stateChanged">The state changed.</param>
    public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
    {
        Row = dataGridViewRow;
        StateChanged = stateChanged;
    }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>The row.</value>
    public DataGridViewRow Row { get; }

    /// <summary>
    /// Gets the state changed.
    /// </summary>
    /// <value>The state changed.</value>
    public DataGridViewElementStates StateChanged { get; }

}

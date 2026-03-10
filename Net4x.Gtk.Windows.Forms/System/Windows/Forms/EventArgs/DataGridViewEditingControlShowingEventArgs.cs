namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.EditingControlShowing" /> event.
/// </summary>
public class DataGridViewEditingControlShowingEventArgs : EventArgs
{
    /// <summary>
    /// The cell style
    /// </summary>
    private DataGridViewCellStyle _cellStyle;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewEditingControlShowingEventArgs"/> class.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="cellStyle">The cell style.</param>
    public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
    {
        Control = control;
        _cellStyle = cellStyle;
    }

    /// <summary>
    /// Gets or sets the cell style.
    /// </summary>
    /// <value>The cell style.</value>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public DataGridViewCellStyle CellStyle
    {
        get => _cellStyle;
        set => _cellStyle = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <value>The control.</value>
    public Control Control { get; }

}

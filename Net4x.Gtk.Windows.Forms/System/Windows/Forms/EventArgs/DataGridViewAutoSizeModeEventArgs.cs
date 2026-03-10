namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView" /><see cref="T:System.Windows.Forms.DataGridView.AutoSizeRowsModeChanged" /> and
/// <see cref="T:System.Windows.Forms.DataGridView.RowHeadersWidthSizeModeChanged" /> events.
/// </summary>
public class DataGridViewAutoSizeModeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewAutoSizeModeEventArgs"/> class.
    /// </summary>
    /// <param name="previousModeAutoSized">if set to <c>true</c> [previous mode automatic sized].</param>
    public DataGridViewAutoSizeModeEventArgs(bool previousModeAutoSized)
    {
        PreviousModeAutoSized = previousModeAutoSized;
    }

    /// <summary>
    /// Gets a value indicating whether [previous mode automatic sized].
    /// </summary>
    /// <value><c>true</c> if [previous mode automatic sized]; otherwise, <c>false</c>.</value>
    public bool PreviousModeAutoSized { get; }

}

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.AutoSizeColumnsModeChanged" /> event.
/// </summary>
public class DataGridViewAutoSizeColumnsModeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridViewAutoSizeColumnsModeEventArgs"/> class.
    /// </summary>
    /// <param name="previousModes">The previous modes.</param>
    public DataGridViewAutoSizeColumnsModeEventArgs(DataGridViewAutoSizeColumnMode[] previousModes)
    {
        PreviousModes = previousModes;
    }

    /// <summary>
    /// Gets the previous modes.
    /// </summary>
    /// <value>The previous modes.</value>
    public DataGridViewAutoSizeColumnMode[] PreviousModes { get; }

}

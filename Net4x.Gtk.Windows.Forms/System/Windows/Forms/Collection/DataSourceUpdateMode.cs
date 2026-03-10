namespace System.Windows.Forms;

/// <summary>
/// Specifies when a data source is updated when changes occur in the bound control.
/// </summary>
public enum DataSourceUpdateMode
{
    /// <summary>
    /// The on validation
    /// </summary>
    OnValidation,
    /// <summary>
    /// The on property changed
    /// </summary>
    OnPropertyChanged,
    /// <summary>
    /// The never
    /// </summary>
    Never
}

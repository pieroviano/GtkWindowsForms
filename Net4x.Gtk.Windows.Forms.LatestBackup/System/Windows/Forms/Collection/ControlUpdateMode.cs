namespace System.Windows.Forms;

/// <summary>
/// Determines when changes to a data source value get propagated to the corresponding data-bound control property.
/// </summary>
public enum ControlUpdateMode
{
    /// <summary>
    /// The on property changed
    /// </summary>
    OnPropertyChanged,
    /// <summary>
    /// The never
    /// </summary>
    Never
}
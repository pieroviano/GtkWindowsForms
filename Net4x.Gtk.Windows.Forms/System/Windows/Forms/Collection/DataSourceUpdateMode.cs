namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies when a data source is updated when changes occur in the bound control.
    /// </summary>
    public enum DataSourceUpdateMode
    {
        /// <summary>
        /// Data source is updated when the control property is validated,
        /// </summary>
        OnValidation,
        /// <summary>
        /// Data source is updated whenever the value of the control property changes.
        /// </summary>
        OnPropertyChanged,
        /// <summary>
        /// Data source is never updated and values entered into the control are not parsed, validated or re-formatted.
        /// </summary>
        Never
    }
}

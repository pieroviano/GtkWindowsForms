namespace System.Windows.Forms
{
    /// <summary>
    /// Determines when changes to a data source value get propagated to the corresponding data-bound control property.
    /// </summary>
    public enum ControlUpdateMode
    {
        /// <summary>
        /// The bound control is updated when the data source value changes, or the data source position changes.
        /// </summary>
        OnPropertyChanged,
        /// <summary>
        /// The bound control is never updated when a data source value changes. The data source is write-only.
        /// </summary>
        Never
    }
}

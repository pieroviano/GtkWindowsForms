namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the styles of the column headers in a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum ColumnHeaderStyle
    {
        /// <summary>
        /// The column header is not displayed in report view.
        /// </summary>
        None,
        /// <summary>
        /// The column headers do not respond to the click of a mouse.
        /// </summary>
        Nonclickable,
        /// <summary>
        /// The column headers function like buttons and can carry out an action, such as sorting, when clicked.
        /// </summary>
        Clickable
    }
}

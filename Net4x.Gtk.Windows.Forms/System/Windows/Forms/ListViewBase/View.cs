namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how list items are displayed in a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum View
    {
        /// <summary>
        /// Each item appears as a full-sized icon with a label below it.
        /// </summary>
        LargeIcon,
        /// <summary>
        /// Each item appears on a separate line with further information about each item arranged in columns. The left-most column contains a small icon and label, and subsequent columns contain sub items as specified by the application. A column displays a header which can display a caption for the column. The user can resize each column at run time.
        /// </summary>
        Details,
        /// <summary>
        /// Each item appears as a small icon with a label to its right.
        /// </summary>
        SmallIcon,
        /// <summary>
        /// Each item appears as a small icon with a label to its right. Items are arranged in columns with no column headers.
        /// </summary>
        List,
        /// <summary>
        /// Each item appears as a full-sized icon with the item label and subitem information to the right of it. The subitem information that appears is specified by the application. This view is available only on Windows XP and the Windows Server 2003 family. On earlier operating systems, this value is ignored and the
        /// <see cref="System.Windows.Forms.ListView" /> control displays in the
        /// <see cref="System.Windows.Forms.View.LargeIcon" /> view.
        /// </summary>
        Tile
    }
}

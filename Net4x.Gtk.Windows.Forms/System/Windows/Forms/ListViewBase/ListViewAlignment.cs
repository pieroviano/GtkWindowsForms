namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how items align in the
    /// <see cref="System.Windows.Forms.ListView" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public enum ListViewAlignment
    {
        /// <summary>
        /// When the user moves an item, it remains where it is dropped.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Items are aligned to the top of the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        Top = 2,
        /// <summary>
        /// Items are aligned to the left of the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Items are aligned to an invisible grid in the control. When the user moves an item, it moves to the closest juncture in the grid.
        /// </summary>
        SnapToGrid = 5
    }
}

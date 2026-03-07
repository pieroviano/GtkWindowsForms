namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies where a
    /// <see cref="System.Windows.Forms.ToolStripItem" />
    /// is to be layed out.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum ToolStripItemPlacement
    {
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is to be layed out on the main
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        Main,
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is to be layed out on the overflow
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        Overflow,
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is not to be layed out on the screen.
        /// </summary>
        None
    }
}

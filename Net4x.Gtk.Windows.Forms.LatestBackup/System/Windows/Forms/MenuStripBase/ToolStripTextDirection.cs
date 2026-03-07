namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the text orientation to use with a particular
    /// <see cref="System.Windows.Forms.ToolStrip.LayoutStyle" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum ToolStripTextDirection
    {
        /// <summary>
        /// Specifies that the text direction is inherited from the parent control.
        /// </summary>
        Inherit,
        /// <summary>
        /// Specifies horizontal text orientation.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Specifies that text is to be rotated 90 degrees.
        /// </summary>
        Vertical90,
        /// <summary>
        /// Specifies that text is to be rotated 270 degrees.
        /// </summary>
        Vertical270
    }
}

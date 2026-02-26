namespace System.Windows.Forms
{
    /// <summary>
    /// Determines whether a
    /// <see cref="System.Windows.Forms.ToolStripItem" />
    /// is placed in the overflow
    /// <see cref="System.Windows.Forms.ToolStrip" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum ToolStripItemOverflow
    {
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is never a candidate for the overflow
        /// <see cref="System.Windows.Forms.ToolStrip" /> If the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> cannot fit on the main
        /// <see cref="System.Windows.Forms.ToolStrip" />, it will not be shown.
        /// </summary>
        Never,
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is permanently shown in the overflow
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        Always,
        /// <summary>
        /// Specifies that a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> drifts between the main
        /// <see cref="System.Windows.Forms.ToolStrip" /> and the overflow
        /// <see cref="System.Windows.Forms.ToolStrip" /> as required if space is not available on the main
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        AsNeeded
    }
}

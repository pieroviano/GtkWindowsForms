namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the style that a
    /// <see cref="System.Windows.Forms.ProgressBar" />
    /// uses to indicate the progress of an operation.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum ProgressBarStyle
    {
        /// <summary>
        /// Indicates progress by increasing the number of segmented blocks in a
        /// <see cref="System.Windows.Forms.ProgressBar" />
        /// </summary>
        Blocks,
        /// <summary>
        /// Indicates progress by increasing the size of a smooth, continuous bar in a
        /// <see cref="System.Windows.Forms.ProgressBar" />
        /// </summary>
        Continuous,
        /// <summary>
        /// Indicates progress by continuously scrolling a block across a
        /// <see cref="System.Windows.Forms.ProgressBar" /> in a marquee fashion.
        /// </summary>
        Marquee
    }
}

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how rows or columns of user interface (UI) elements should be sized relative to their container.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum SizeType
    {
        /// <summary>
        /// The row or column should be automatically sized to share space with its peers.
        /// </summary>
        AutoSize,
        /// <summary>
        /// The row or column should be sized to an exact number of pixels.
        /// </summary>
        Absolute,
        /// <summary>
        /// The row or column should be sized as a percentage of the parent container.
        /// </summary>
        Percent
    }
}

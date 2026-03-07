namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies a value indicating whether the text appears from right to left, such as when using Hebrew or Arabic fonts.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum RightToLeft
    {
        /// <summary>
        /// The direction the text read is inherited from the parent control.
        /// </summary>
        Inherit,
        /// <summary>
        /// The text reads from left to right. This is the default.
        /// </summary>
        No,
        /// <summary>
        /// The text reads from right to left.
        /// </summary>
        Yes
    }
}

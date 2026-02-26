namespace System.Windows.Forms
{
    /// <summary>
    /// Indicates the result of a completed binding operation.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum BindingCompleteState
    {
        /// <summary>
        /// An indication that the binding operation completed successfully.
        /// </summary>
        Success,
        /// <summary>
        /// An indication that the binding operation failed with a data error.
        /// </summary>
        DataError,
        /// <summary>
        /// An indication that the binding operation failed with an exception.
        /// </summary>
        Exception
    }
}

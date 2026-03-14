namespace System.Windows.Forms;

/// <summary>
/// Indicates the result of a completed binding operation.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public enum BindingCompleteState
{
    /// <summary>
    /// The success
    /// </summary>
    Success,
    /// <summary>
    /// The data error
    /// </summary>
    DataError,
    /// <summary>
    /// The exception
    /// </summary>
    Exception
}
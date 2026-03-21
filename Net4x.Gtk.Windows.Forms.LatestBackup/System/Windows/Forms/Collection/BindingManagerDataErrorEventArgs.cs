using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.BindingManagerBase.DataError" /> event.
/// </summary>
public class BindingManagerDataErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingManagerDataErrorEventArgs" /> class.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public BindingManagerDataErrorEventArgs(Exception exception)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Exception Exception
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }
}
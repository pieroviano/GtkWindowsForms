using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event.
    /// </summary>
    public class BindingManagerDataErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingManagerDataErrorEventArgs" /> class.
        /// </summary>
        /// <param name="exception">The
        /// <see cref="System.Exception" /> that occurred in the binding process that caused the
        /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event to be raised.
        /// </param>
        public BindingManagerDataErrorEventArgs(Exception exception)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Exception" /> caught in the binding process that caused the
        /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event to be raised.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Exception" /> that caused the
        /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event to be raised.
        /// </returns>
        public Exception Exception
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }

    }
}

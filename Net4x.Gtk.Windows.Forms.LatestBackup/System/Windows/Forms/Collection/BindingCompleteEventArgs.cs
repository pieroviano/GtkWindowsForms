using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class BindingCompleteEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets the binding associated with this occurrence of a
        /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Binding" /> associated with this
        /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" />
        /// </returns>
        public Binding Binding
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Gets the completion state of the binding operation.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.BindingCompleteState" /> values.
        /// </returns>
        public BindingCompleteState BindingCompleteState
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Gets the direction of the binding operation.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.BindingCompleteContext" /> values.
        /// </returns>
        public BindingCompleteContext BindingCompleteContext
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Gets the text description of the error that occurred during the binding operation.
        /// </summary>
        /// <returns>The text description of the error that occurred during the binding operation.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ErrorText
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Gets the exception that occurred during the binding operation.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Exception" /> that occurred during the binding operation.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Exception Exception
        {
            [CompilerGenerated]
            get
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" /> class with the specified binding, error state and text, binding context, exception, and whether the binding should be cancelled.
        /// </summary>
        /// <param name="binding">The binding associated with this occurrence of a
        /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
        /// </param>
        /// <param name="state">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteState" /> values.
        /// </param>
        /// <param name="context">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteContext" /> values.
        /// </param>
        /// <param name="errorText">The error text or exception message for errors that occurred during the binding.
        /// </param>
        /// <param name="exception">The
        /// <see cref="System.Exception" /> that occurred during the binding.
        /// </param>
        /// <param name="cancel">true to cancel the binding and keep focus on the current control; false to allow focus to shift to another control.
        /// </param>
        public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" /> class with the specified binding, error state and text, binding context, and exception.
        /// </summary>
        /// <param name="binding">The binding associated with this occurrence of a
        /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
        /// </param>
        /// <param name="state">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteState" /> values.
        /// </param>
        /// <param name="context">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteContext" /> values.
        /// </param>
        /// <param name="errorText">The error text or exception message for errors that occurred during the binding.
        /// </param>
        /// <param name="exception">The
        /// <see cref="System.Exception" /> that occurred during the binding.
        /// </param>
        public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" /> class with the specified binding, error state and text, and binding context.
        /// </summary>
        /// <param name="binding">The binding associated with this occurrence of a
        /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
        /// </param>
        /// <param name="state">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteState" /> values.
        /// </param>
        /// <param name="context">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteContext" /> values.
        /// </param>
        /// <param name="errorText">The error text or exception message for errors that occurred during the binding.
        /// </param>
        public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" /> class with the specified binding, error state, and binding context.
        /// </summary>
        /// <param name="binding">The binding associated with this occurrence of a
        /// <see cref="System.Windows.Forms.Binding.BindingComplete" /> event.
        /// </param>
        /// <param name="state">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteState" /> values.
        /// </param>
        /// <param name="context">One of the
        /// <see cref="System.Windows.Forms.BindingCompleteContext" /> values.
        /// </param>
        public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}

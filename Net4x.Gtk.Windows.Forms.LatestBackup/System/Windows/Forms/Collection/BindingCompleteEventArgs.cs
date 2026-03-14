using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Binding.BindingComplete" /> event.
/// Instances contain the binding that completed, the completion state, the context in which
/// the binding completed, optional error text and exception information, and an optional cancel flag.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class BindingCompleteEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs" /> class with
    /// the specified binding, completion state, context, error text, exception and cancel flag.
    /// </summary>
    /// <param name="binding">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Binding" /> associated with the completion event.
    /// </param>
    /// <param name="state">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteState" /> describing the result.
    /// </param>
    /// <param name="context">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteContext" /> that identifies the area in which the binding completed.
    /// </param>
    /// <param name="errorText">
    /// Optional user-visible error text describing the completion result.
    /// </param>
    /// <param name="exception">
    /// Optional
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Exception" /> that occurred during the operation.
    /// </param>
    /// <param name="cancel">
    /// True to cancel the operation; otherwise false.
    /// </param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception, bool cancel)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs" /> class with
    /// the specified binding, completion state, context, error text and exception.
    /// </summary>
    /// <param name="binding">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Binding" /> associated with the completion event.
    /// </param>
    /// <param name="state">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteState" /> describing the result.
    /// </param>
    /// <param name="context">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteContext" /> that identifies the area in which the binding completed.
    /// </param>
    /// <param name="errorText">
    /// Optional user-visible error text describing the completion result.
    /// </param>
    /// <param name="exception">
    /// Optional
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Exception" /> that occurred during the operation.
    /// </param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs" /> class with
    /// the specified binding, completion state, context and error text.
    /// </summary>
    /// <param name="binding">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Binding" /> associated with the completion event.
    /// </param>
    /// <param name="state">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteState" /> describing the result.
    /// </param>
    /// <param name="context">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteContext" /> that identifies the area in which the binding completed.
    /// </param>
    /// <param name="errorText">
    /// Optional user-visible error text describing the completion result.
    /// </param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs" /> class with
    /// the specified binding, completion state and context.
    /// </summary>
    /// <param name="binding">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Binding" /> associated with the completion event.
    /// </param>
    /// <param name="state">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteState" /> describing the result.
    /// </param>
    /// <param name="context">
    /// The
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.BindingCompleteContext" /> that identifies the area in which the binding completed.
    /// </param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.BindingCompleteEventArgs.Binding" /> that raised the BindingComplete event.
    /// </summary>
    public Binding Binding
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets the context in which the binding completed.
    /// </summary>
    public BindingCompleteContext BindingCompleteContext
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets the state that describes the outcome of the binding operation.
    /// </summary>
    public BindingCompleteState BindingCompleteState
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets a user-visible error message associated with the completion, if any.
    /// </summary>
    public string ErrorText
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Gets the exception that occurred during binding, if any.
    /// </summary>
    public Exception Exception
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }

}
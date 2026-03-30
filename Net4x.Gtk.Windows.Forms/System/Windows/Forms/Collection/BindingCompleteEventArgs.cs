//
// BindingCompleteEventArgs.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Novell, Inc.
//
// Authors:
//	Jonathan Pobst (monkey@jpobst.com)
//

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// BindingCompleteEventArgs provides data for the <see cref="Binding.BindingComplete"/> event.
/// </summary>
/// <seealso cref="System.ComponentModel.CancelEventArgs" />
public class BindingCompleteEventArgs : CancelEventArgs
{
    private readonly Binding binding;
    private readonly BindingCompleteState state;
    private readonly BindingCompleteContext context;
    private string? error_text;
    private Exception? exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingCompleteEventArgs"/> class.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <param name="state">The state.</param>
    /// <param name="context">The context.</param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context)
        : this(binding, state, context, string.Empty, null, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingCompleteEventArgs"/> class.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <param name="state">The state.</param>
    /// <param name="context">The context.</param>
    /// <param name="errorText">The error text.</param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText)
        : this(binding, state, context, errorText, null, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingCompleteEventArgs"/> class.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <param name="state">The state.</param>
    /// <param name="context">The context.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="exception">The exception.</param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string errorText, Exception exception)
        : this(binding, state, context, errorText, exception, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingCompleteEventArgs"/> class.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <param name="state">The state.</param>
    /// <param name="context">The context.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="cancel">if set to <c>true</c> [cancel].</param>
    public BindingCompleteEventArgs(Binding binding, BindingCompleteState state, BindingCompleteContext context, string? errorText, Exception? exception, bool cancel)
        : base(cancel)
    {
        this.binding = binding;
        this.state = state;
        this.context = context;
        this.error_text = errorText;
        this.exception = exception;
    }

    /// <summary>
    /// Gets the binding.
    /// </summary>
    /// <value>
    /// The binding.
    /// </value>
    public Binding Binding
    {
        get { return this.binding; }
    }

    /// <summary>
    /// Gets the binding complete context.
    /// </summary>
    /// <value>
    /// The binding complete context.
    /// </value>
    public BindingCompleteContext BindingCompleteContext
    {
        get { return this.context; }
    }

    /// <summary>
    /// Gets the state of the binding complete.
    /// </summary>
    /// <value>
    /// The state of the binding complete.
    /// </value>
    public BindingCompleteState BindingCompleteState
    {
        get { return this.state; }
    }

    /// <summary>
    /// Gets the error text.
    /// </summary>
    /// <value>
    /// The error text.
    /// </value>
    public string? ErrorText
    {
        get { return this.error_text; }
    }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public Exception? Exception
    {
        get { return this.exception; }
    }

    internal void SetErrorText(string error_text)
    {
        this.error_text = error_text;
    }

    internal void SetException(Exception exception)
    {
        this.exception = exception;
    }
}
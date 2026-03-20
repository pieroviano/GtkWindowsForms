// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.DataError" /> event.
/// </summary>
public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
{
    private bool _throwException;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewDataErrorEventArgs" /> class.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    public DataGridViewDataErrorEventArgs(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context) : base(columnIndex, rowIndex)
    {
        Exception = exception;
        Context = context;
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    public DataGridViewDataErrorContexts Context { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public Exception Exception { get; }

    /// <summary>
    /// Gets or sets a value indicating whether throw exception.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if throw exception; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    /// <exception cref="T:System.ArgumentException">
    /// SR.DataGridView_CannotThrowNullException
    /// </exception>
    public bool ThrowException
    {
        get => _throwException;

        set
        {
            if (value && Exception == null)
            {
                throw new ArgumentException("SR.DataGridView_CannotThrowNullException");
            }

            _throwException = value;
        }
    }

}
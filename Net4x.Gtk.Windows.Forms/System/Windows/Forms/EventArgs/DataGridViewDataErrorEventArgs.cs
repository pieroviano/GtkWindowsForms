// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.DataError" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
    {
        private bool _throwException;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs" /> class.
        /// </summary>
        /// <param name="exception">The exception that occurred.
        /// </param>
        /// <param name="columnIndex">The column index of the cell that raised the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" />
        /// </param>
        /// <param name="rowIndex">The row index of the cell that raised the
        /// <see cref="System.Windows.Forms.DataGridView.DataError" />
        /// </param>
        /// <param name="context">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorContexts" /> values indicating the context in which the error occurred.
        /// </param>
        public DataGridViewDataErrorEventArgs(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context) : base(columnIndex, rowIndex)
        {
            Exception = exception;
            Context = context;
        }

        /// <summary>
        /// Gets details about the state of the
        /// <see cref="System.Windows.Forms.DataGridView" /> when the error occurred.
        /// </summary>
        /// <returns>A bitwise combination of the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorContexts" /> values that specifies the context in which the error occurred.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewDataErrorContexts Context { get; }

        /// <summary>
        /// Gets the exception that represents the error.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Exception" /> that represents the error.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Exception Exception { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw the exception after the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventHandler" /> delegate is finished with it.
        /// </summary>
        /// <returns>true if the exception should be thrown; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="System.ArgumentException">When setting this property to true, the
        /// <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs.Exception" /> property value is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
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
}

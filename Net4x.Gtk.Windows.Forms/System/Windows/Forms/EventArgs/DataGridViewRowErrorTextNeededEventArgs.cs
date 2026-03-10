using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.DataGridView.RowErrorTextNeeded" /> event of a
    /// <see cref="T:System.Windows.Forms.DataGridView" /> control.
    /// </summary>
    public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewRowErrorTextNeededEventArgs"/> class.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="errorText">The error text.</param>
        internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
        {
            Debug.Assert(rowIndex >= -1);
            RowIndex = rowIndex;
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>The error text.</value>
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex { get; }

    }
}

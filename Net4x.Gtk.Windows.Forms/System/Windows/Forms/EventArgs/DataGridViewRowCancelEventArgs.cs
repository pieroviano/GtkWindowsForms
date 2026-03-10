using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.DataGridView.UserDeletingRow" /> event of a
    /// <see cref="T:System.Windows.Forms.DataGridView" />
    /// </summary>
    public class DataGridViewRowCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewRowCancelEventArgs"/> class.
        /// </summary>
        /// <param name="dataGridViewRow">The data grid view row.</param>
        public DataGridViewRowCancelEventArgs(DataGridViewRow dataGridViewRow)
        {
            Row = dataGridViewRow;
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        public DataGridViewRow Row { get; set; }

    }
}

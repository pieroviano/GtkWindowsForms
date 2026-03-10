using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.DataGridView.DataBindingComplete" /> event.
    /// </summary>
    public class DataGridViewBindingCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewBindingCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list changed.</param>
        public DataGridViewBindingCompleteEventArgs(ListChangedType listChangedType)
        {
            ListChangedType = listChangedType;
        }

        /// <summary>
        /// Gets the type of the list changed.
        /// </summary>
        /// <value>The type of the list changed.</value>
        public ListChangedType ListChangedType { get; }

    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.EditingControlShowing" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewEditingControlShowingEventArgs : EventArgs
    {
        private DataGridViewCellStyle _cellStyle;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewEditingControlShowingEventArgs" /> class.
        /// </summary>
        /// <param name="control">A
        /// <see cref="System.Windows.Forms.Control" /> in which the user will edit the selected cell's contents.
        /// </param>
        /// <param name="cellStyle">A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> representing the style of the cell being edited.
        /// </param>
        public DataGridViewEditingControlShowingEventArgs(Control control, DataGridViewCellStyle cellStyle)
        {
            Control = control;
            _cellStyle = cellStyle;
        }

        /// <summary>
        /// Gets or sets the cell style of the edited cell.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> representing the style of the cell being edited.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The specified value when setting this property is null.
        /// </exception>
        public DataGridViewCellStyle CellStyle
        {
            get => _cellStyle;
            set => _cellStyle = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The control shown to the user for editing the selected cell's value.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Control" /> that displays an area for the user to enter or change the selected cell's value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Control Control { get; }

    }
}

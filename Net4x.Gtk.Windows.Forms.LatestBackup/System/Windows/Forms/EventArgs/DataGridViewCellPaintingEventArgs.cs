// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.CellPainting" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewCellPaintingEventArgs : HandledEventArgs
    {
        private readonly DataGridView _dataGridView;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridView">The
        /// <see cref="System.Windows.Forms.DataGridView" /> that contains the cell to be painted.
        /// </param>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> used to paint the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </param>
        /// <param name="clipBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that represents the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be repainted.
        /// </param>
        /// <param name="cellBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that contains the bounds of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that is being painted.
        /// </param>
        /// <param name="rowIndex">The row index of the cell that is being painted.
        /// </param>
        /// <param name="columnIndex">The column index of the cell that is being painted.
        /// </param>
        /// <param name="cellState">A bitwise combination of the
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values that specifies the state of the cell.
        /// </param>
        /// <param name="value">The data of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that is being painted.
        /// </param>
        /// <param name="formattedValue">The formatted data of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" /> that is being painted.
        /// </param>
        /// <param name="errorText">An error message that is associated with the cell.
        /// </param>
        /// <param name="cellStyle">A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that contains formatting and style information about the cell.
        /// </param>
        /// <param name="advancedBorderStyle">A
        /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> that contains border styles for the cell that is being painted.
        /// </param>
        /// <param name="paintParts">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewPaintParts" /> values specifying the parts to paint.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridView" /> is null.-or-
        /// <paramref name="graphics" /> is null.-or-
        /// <paramref name="cellStyle" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="paintParts" /> is not a valid bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewPaintParts" /> values.
        /// </exception>
        public DataGridViewCellPaintingEventArgs(DataGridView dataGridView,
                                                         Graphics graphics,
                                                         Rectangle clipBounds,
                                                         Rectangle cellBounds,
                                                         int rowIndex,
                                                         int columnIndex,
                                                         DataGridViewElementStates cellState,
                                                         object value,
                                                         object formattedValue,
                                                         string errorText,
                                                         DataGridViewCellStyle cellStyle,
                                                         DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                                         DataGridViewPaintParts paintParts)
        {
            if ((paintParts & ~DataGridViewPaintParts.All) != 0)
            {
                throw new ArgumentException("string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, nameof(paintParts)), nameof(paintParts)");
            }

            _dataGridView = dataGridView ?? throw new ArgumentNullException(nameof(dataGridView));
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            ClipBounds = clipBounds;
            CellBounds = cellBounds;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            State = cellState;
            Value = value;
            FormattedValue = formattedValue;
            ErrorText = errorText;
            CellStyle = cellStyle ?? throw new ArgumentNullException(nameof(cellStyle));
            AdvancedBorderStyle = advancedBorderStyle;
            PaintParts = paintParts;
        }

        internal DataGridViewCellPaintingEventArgs(DataGridView dataGridView)
        {
            Debug.Assert(dataGridView != null);
            _dataGridView = dataGridView;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Drawing.Graphics" /> used to paint the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> used to paint the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Graphics Graphics { get; private set; }
        /// <summary>
        /// Gets the border style of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> that represents the border style of the
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewAdvancedBorderStyle AdvancedBorderStyle { get; private set; }
        /// <summary>
        /// Get the bounds of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle CellBounds { get; private set; }
        /// <summary>
        /// Gets the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be repainted.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be repainted.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle ClipBounds { get; private set; }
        /// <summary>
        /// Gets the row index of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The row index of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; private set; }
        /// <summary>
        /// Gets the column index of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The column index of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ColumnIndex { get; private set; }
        /// <summary>
        /// Gets the state of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values that specifies the state of the cell.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewElementStates State { get; private set; }
        /// <summary>
        /// Gets the value of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The value of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Value { get; private set; }
        /// <summary>
        /// Gets the formatted value of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>The formatted value of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object FormattedValue { get; private set; }
        /// <summary>
        /// Gets a string that represents an error message for the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>A string that represents an error message for the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ErrorText { get; private set; }
        /// <summary>
        /// Gets the cell style of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that contains the cell style of the current
        /// <see cref="System.Windows.Forms.DataGridViewCell" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewCellStyle CellStyle { get; private set; }
        /// <summary>
        /// The cell parts that are to be painted.
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewPaintParts" /> values specifying the parts to be painted.
        /// </returns>
        public DataGridViewPaintParts PaintParts { get; private set; }
        /// <summary>
        /// Paints the specified parts of the cell for the area in the specified bounds.
        /// </summary>
        /// <param name="clipBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that specifies the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> to be painted.
        /// </param>
        /// <param name="paintParts">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewPaintParts" /> values specifying the parts to paint.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.RowIndex" /> is less than -1 or greater than or equal to the number of rows in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.ColumnIndex" /> is less than -1 or greater than or equal to the number of columns in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        public void Paint(Rectangle clipBounds, DataGridViewPaintParts paintParts)
        {
            if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange");
            }
            if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange");
            }

            //_dataGridView.GetCellInternal(ColumnIndex, RowIndex).PaintInternal(Graphics,
            //                                                                   clipBounds,
            //                                                                   CellBounds,
            //                                                                   RowIndex,
            //                                                                   State,
            //                                                                   Value,
            //                                                                   FormattedValue,
            //                                                                   ErrorText,
            //                                                                   CellStyle,
            //                                                                   AdvancedBorderStyle,
            //                                                                   paintParts);
        }
        /// <summary>
        /// Paints the cell background for the area in the specified bounds.
        /// </summary>
        /// <param name="clipBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that specifies the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> to be painted.
        /// </param>
        /// <param name="cellsPaintSelectionBackground">true to paint the background of the specified bounds with the color of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.SelectionBackColor" /> property of the
        /// <see cref="System.Windows.Forms.DataGridViewCell.InheritedStyle" />; false to paint the background of the specified bounds with the color of the
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle.BackColor" /> property of the
        /// <see cref="System.Windows.Forms.DataGridViewCell.InheritedStyle" />
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.RowIndex" /> is less than -1 or greater than or equal to the number of rows in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.ColumnIndex" /> is less than -1 or greater than or equal to the number of columns in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        public void PaintBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
        {
            if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange");
            }
            if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange");
            }

            DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
            if (cellsPaintSelectionBackground)
            {
                paintParts |= DataGridViewPaintParts.SelectionBackground;
            }
            //_dataGridView.GetCellInternal(ColumnIndex, RowIndex).PaintInternal(Graphics,
            //                                                                   clipBounds,
            //                                                                   CellBounds,
            //                                                                   RowIndex,
            //                                                                   State,
            //                                                                   Value,
            //                                                                   FormattedValue,
            //                                                                   ErrorText,
            //                                                                   CellStyle,
            //                                                                   AdvancedBorderStyle,
            //                                                                   paintParts);
        }
        /// <summary>
        /// Paints the cell content for the area in the specified bounds.
        /// </summary>
        /// <param name="clipBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that specifies the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> to be painted.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.RowIndex" /> is less than -1 or greater than or equal to the number of rows in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.-or-
        /// <see cref="System.Windows.Forms.DataGridViewCellPaintingEventArgs.ColumnIndex" /> is less than -1 or greater than or equal to the number of columns in the
        /// <see cref="System.Windows.Forms.DataGridView" /> control.
        /// </exception>
        public void PaintContent(Rectangle clipBounds)
        {
            if (RowIndex < -1 || RowIndex >= _dataGridView.Rows.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange");
            }
            if (ColumnIndex < -1 || ColumnIndex >= _dataGridView.Columns.Count)
            {
                throw new InvalidOperationException("SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange");
            }

            //_dataGridView.GetCellInternal(ColumnIndex, RowIndex).PaintInternal(Graphics,
            //                                                                   clipBounds,
            //                                                                   CellBounds,
            //                                                                   RowIndex,
            //                                                                   State,
            //                                                                   Value,
            //                                                                   FormattedValue,
            //                                                                   ErrorText,
            //                                                                   CellStyle,
            //                                                                   AdvancedBorderStyle,
            //                                                                   DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
        }

        internal void SetProperties(Graphics graphics,
                                    Rectangle clipBounds,
                                    Rectangle cellBounds,
                                    int rowIndex,
                                    int columnIndex,
                                    DataGridViewElementStates cellState,
                                    object value,
                                    object formattedValue,
                                    string errorText,
                                    DataGridViewCellStyle cellStyle,
                                    DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                    DataGridViewPaintParts paintParts)
        {
            Debug.Assert(graphics != null);

            Graphics = graphics;
            ClipBounds = clipBounds;
            CellBounds = cellBounds;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            State = cellState;
            Value = value;
            FormattedValue = formattedValue;
            ErrorText = errorText;
            CellStyle = cellStyle;
            AdvancedBorderStyle = advancedBorderStyle;
            PaintParts = paintParts;
            Handled = false;
        }
    }
}

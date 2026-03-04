// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.RowPostPaint" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DataGridViewRowPostPaintEventArgs : EventArgs
    {
        private readonly DataGridView _dataGridView;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewRowPostPaintEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridView">The
        /// <see cref="System.Windows.Forms.DataGridView" /> that owns the row that is being painted.
        /// </param>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> used to paint the
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </param>
        /// <param name="clipBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that represents the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be painted.
        /// </param>
        /// <param name="rowBounds">A
        /// <see cref="System.Drawing.Rectangle" /> that contains the bounds of the
        /// <see cref="System.Windows.Forms.DataGridViewRow" /> that is being painted.
        /// </param>
        /// <param name="rowIndex">The row index of the cell that is being painted.
        /// </param>
        /// <param name="rowState">A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values that specifies the state of the row.
        /// </param>
        /// <param name="errorText">An error message that is associated with the row.
        /// </param>
        /// <param name="inheritedRowStyle">A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that contains formatting and style information about the row.
        /// </param>
        /// <param name="isFirstDisplayedRow">true to indicate whether the current row is the first row currently displayed in the
        /// <see cref="System.Windows.Forms.DataGridView" />; otherwise, false.
        /// </param>
        /// <param name="isLastVisibleRow">true to indicate whether the current row is the last row in the
        /// <see cref="System.Windows.Forms.DataGridView" /> that has the
        /// <see cref="System.Windows.Forms.DataGridViewRow.Visible" /> property set to true; otherwise, false.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dataGridView" /> is null.-or-
        /// <paramref name="graphics" /> is null.-or-
        /// <paramref name="inheritedRowStyle" /> is null.
        /// </exception>
        public DataGridViewRowPostPaintEventArgs(DataGridView dataGridView,
                                                         Graphics graphics,
                                                         Rectangle clipBounds,
                                                         Rectangle rowBounds,
                                                         int rowIndex,
                                                         DataGridViewElementStates rowState,
                                                         string errorText,
                                                         DataGridViewCellStyle inheritedRowStyle,
                                                         bool isFirstDisplayedRow,
                                                         bool isLastVisibleRow)
        {
            _dataGridView = dataGridView ?? throw new ArgumentNullException(nameof(dataGridView));
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            ClipBounds = clipBounds;
            RowBounds = rowBounds;
            RowIndex = rowIndex;
            State = rowState;
            ErrorText = errorText;
            InheritedRowStyle = inheritedRowStyle ?? throw new ArgumentNullException(nameof(inheritedRowStyle));
            IsFirstDisplayedRow = isFirstDisplayedRow;
            IsLastVisibleRow = isLastVisibleRow;
        }

        internal DataGridViewRowPostPaintEventArgs(DataGridView dataGridView)
        {
            Debug.Assert(dataGridView != null);
            _dataGridView = dataGridView;
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Drawing.Graphics" /> used to paint the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> used to paint the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Graphics Graphics { get; private set; }
        /// <summary>
        /// Gets or sets the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be repainted.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the area of the
        /// <see cref="System.Windows.Forms.DataGridView" /> that needs to be repainted.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle ClipBounds { get; set; }
        /// <summary>
        /// Get the bounds of the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle RowBounds { get; private set; }
        /// <summary>
        /// Gets the index of the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>The index of the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int RowIndex { get; private set; }
        /// <summary>
        /// Gets the state of the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>A bitwise combination of
        /// <see cref="System.Windows.Forms.DataGridViewElementStates" /> values that specifies the state of the row.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DataGridViewElementStates State { get; private set; }
        /// <summary>
        /// Gets a string that represents an error message for the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>A string that represents an error message for the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ErrorText { get; private set; }
        /// <summary>
        /// Gets the cell style applied to the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DataGridViewCellStyle" /> that contains the cell style applied to the current
        /// <see cref="System.Windows.Forms.DataGridViewRow" />
        /// </returns>
        public DataGridViewCellStyle InheritedRowStyle { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the current row is the first row displayed in the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </summary>
        /// <returns>true if the row being painted is currently the first row displayed in the
        /// <see cref="System.Windows.Forms.DataGridView" />; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsFirstDisplayedRow { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the current row is the last visible row displayed in the
        /// <see cref="System.Windows.Forms.DataGridView" />
        /// </summary>
        /// <returns>true if the current row is the last row in the
        /// <see cref="System.Windows.Forms.DataGridView" /> that has the
        /// <see cref="System.Windows.Forms.DataGridViewRow.Visible" /> property set to true; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsLastVisibleRow { get; private set; }

        //    _dataGridView.Rows.SharedRow(RowIndex).DrawFocus(Graphics,
        //                                                     ClipBounds,
        //                                                     bounds,
        //                                                     RowIndex,
        //                                                     State,
        //                                                     InheritedRowStyle,
        //                                                     cellsPaintSelectionBackground);
        //}

        //    _dataGridView.Rows.SharedRow(RowIndex).PaintCells(Graphics,
        //                                                      clipBounds,
        //                                                      RowBounds,
        //                                                      RowIndex,
        //                                                      State,
        //                                                      IsFirstDisplayedRow,
        //                                                      IsLastVisibleRow,
        //                                                      paintParts);
        //}

        //    DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
        //    if (cellsPaintSelectionBackground)
        //    {
        //        paintParts |= DataGridViewPaintParts.SelectionBackground;
        //    }
        //    _dataGridView.Rows.SharedRow(RowIndex).PaintCells(Graphics,
        //                                                      clipBounds,
        //                                                      RowBounds,
        //                                                      RowIndex,
        //                                                      State,
        //                                                      IsFirstDisplayedRow,
        //                                                      IsLastVisibleRow,
        //                                                      paintParts);
        //}

        //    _dataGridView.Rows.SharedRow(RowIndex).PaintCells(Graphics,
        //                                                      clipBounds,
        //                                                      RowBounds,
        //                                                      RowIndex,
        //                                                      State,
        //                                                      IsFirstDisplayedRow,
        //                                                      IsLastVisibleRow,
        //                                                      DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
        //}

        //    _dataGridView.Rows.SharedRow(RowIndex).PaintHeader(Graphics,
        //                                                       ClipBounds,
        //                                                       RowBounds,
        //                                                       RowIndex,
        //                                                       State,
        //                                                       IsFirstDisplayedRow,
        //                                                       IsLastVisibleRow,
        //                                                       paintParts);
        //}

        internal void SetProperties(Graphics graphics,
                                    Rectangle clipBounds,
                                    Rectangle rowBounds,
                                    int rowIndex,
                                    DataGridViewElementStates rowState,
                                    string errorText,
                                    DataGridViewCellStyle inheritedRowStyle,
                                    bool isFirstDisplayedRow,
                                    bool isLastVisibleRow)
        {
            Debug.Assert(graphics != null);

            Graphics = graphics;
            ClipBounds = clipBounds;
            RowBounds = rowBounds;
            RowIndex = rowIndex;
            State = rowState;
            ErrorText = errorText;
            InheritedRowStyle = inheritedRowStyle;
            IsFirstDisplayedRow = isFirstDisplayedRow;
            IsLastVisibleRow = isLastVisibleRow;
        }
    }
}

using System.Drawing;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="T:System.Windows.Forms.DataGridView.RowPostPaint" /> event.
    /// </summary>
    public class DataGridViewRowPostPaintEventArgs : EventArgs
    {
        /// <summary>
        /// The data grid view
        /// </summary>
        private readonly DataGridView _dataGridView;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewRowPostPaintEventArgs"/> class.
        /// </summary>
        /// <param name="dataGridView">The data grid view.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clipBounds">The clip bounds.</param>
        /// <param name="rowBounds">The row bounds.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="rowState">State of the row.</param>
        /// <param name="errorText">The error text.</param>
        /// <param name="inheritedRowStyle">The inherited row style.</param>
        /// <param name="isFirstDisplayedRow">if set to <c>true</c> [is first displayed row].</param>
        /// <param name="isLastVisibleRow">if set to <c>true</c> [is last visible row].</param>
        /// <exception cref="System.ArgumentNullException">dataGridView</exception>
        /// <exception cref="System.ArgumentNullException">graphics</exception>
        /// <exception cref="System.ArgumentNullException">inheritedRowStyle</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewRowPostPaintEventArgs"/> class.
        /// </summary>
        /// <param name="dataGridView">The data grid view.</param>
        internal DataGridViewRowPostPaintEventArgs(DataGridView dataGridView)
        {
            Debug.Assert(dataGridView != null);
            _dataGridView = dataGridView;
        }

        /// <summary>
        /// Gets or sets the clip bounds.
        /// </summary>
        /// <value>The clip bounds.</value>
        public Rectangle ClipBounds { get; set; }

        /// <summary>
        /// Gets the error text.
        /// </summary>
        /// <value>The error text.</value>
        public string ErrorText { get; private set; }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public Graphics Graphics { get; private set; }

        /// <summary>
        /// Gets the inherited row style.
        /// </summary>
        /// <value>The inherited row style.</value>
        public DataGridViewCellStyle InheritedRowStyle { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is first displayed row.
        /// </summary>
        /// <value><c>true</c> if this instance is first displayed row; otherwise, <c>false</c>.</value>
        public bool IsFirstDisplayedRow { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is last visible row.
        /// </summary>
        /// <value><c>true</c> if this instance is last visible row; otherwise, <c>false</c>.</value>
        public bool IsLastVisibleRow { get; private set; }

        /// <summary>
        /// Gets the row bounds.
        /// </summary>
        /// <value>The row bounds.</value>
        public Rectangle RowBounds { get; private set; }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex { get; private set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public DataGridViewElementStates State { get; private set; }

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

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clipBounds">The clip bounds.</param>
        /// <param name="rowBounds">The row bounds.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="rowState">State of the row.</param>
        /// <param name="errorText">The error text.</param>
        /// <param name="inheritedRowStyle">The inherited row style.</param>
        /// <param name="isFirstDisplayedRow">if set to <c>true</c> [is first displayed row].</param>
        /// <param name="isLastVisibleRow">if set to <c>true</c> [is last visible row].</param>
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

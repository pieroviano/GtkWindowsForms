// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellPainting" /> event.
/// </summary>
public class DataGridViewCellPaintingEventArgs : HandledEventArgs
{
    private readonly DataGridView _dataGridView;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellPaintingEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridView">
    /// The data grid view.
    /// </param>
    /// <param name="graphics">
    /// The graphics.
    /// </param>
    /// <param name="clipBounds">
    /// The clip bounds.
    /// </param>
    /// <param name="cellBounds">
    /// The cell bounds.
    /// </param>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="cellState">
    /// State of the cell.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="formattedValue">
    /// The formatted value.
    /// </param>
    /// <param name="errorText">
    /// The error text.
    /// </param>
    /// <param name="cellStyle">
    /// The cell style.
    /// </param>
    /// <param name="advancedBorderStyle">
    /// The advanced border style.
    /// </param>
    /// <param name="paintParts">
    /// The paint parts.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, nameof(paintParts)), nameof(paintParts)
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// dataGridView
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// graphics
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// cellStyle
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
    /// Gets the advanced border style.
    /// </summary>
    /// <value>
    /// The advanced border style.
    /// </value>
    public DataGridViewAdvancedBorderStyle AdvancedBorderStyle
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the cell bounds.
    /// </summary>
    /// <value>
    /// The cell bounds.
    /// </value>
    public Rectangle CellBounds
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the cell style.
    /// </summary>
    /// <value>
    /// The cell style.
    /// </value>
    public DataGridViewCellStyle CellStyle
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the clip bounds.
    /// </summary>
    /// <value>
    /// The clip bounds.
    /// </value>
    public Rectangle ClipBounds
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the error text.
    /// </summary>
    /// <value>
    /// The error text.
    /// </value>
    public string ErrorText
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the formatted value.
    /// </summary>
    /// <value>
    /// The formatted value.
    /// </value>
    public object FormattedValue
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the graphics.
    /// </summary>
    /// <value>
    /// The graphics.
    /// </value>
    public Graphics Graphics
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the paint parts.
    /// </summary>
    /// <value>
    /// The paint parts.
    /// </value>
    public DataGridViewPaintParts PaintParts
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public DataGridViewElementStates State
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value
    {
        get;
        private set;
    }

    /// <summary>
    /// Paints the specified clip bounds.
    /// </summary>
    /// <param name="clipBounds">
    /// The clip bounds.
    /// </param>
    /// <param name="paintParts">
    /// The paint parts.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange
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
    /// Paints the background.
    /// </summary>
    /// <param name="clipBounds">
    /// The clip bounds.
    /// </param>
    /// <param name="cellsPaintSelectionBackground">
    /// if set to
    /// <c>
    /// true
    /// </c>cells paint selection background.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange
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

        var paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border;
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
    /// Paints the content.
    /// </summary>
    /// <param name="clipBounds">
    /// The clip bounds.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// SR.DataGridViewElementPaintingEventArgs_ColumnIndexOutOfRange
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
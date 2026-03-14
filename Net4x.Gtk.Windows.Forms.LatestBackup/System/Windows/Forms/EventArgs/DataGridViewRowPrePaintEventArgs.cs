// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowPrePaint" /> event.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewRowPrePaintEventArgs : HandledEventArgs
{
    private readonly DataGridView _dataGridView;

    //public DataGridViewRowPrePaintEventArgs(DataGridView dataGridView,
    //                                        Graphics graphics,
    //                                        Rectangle clipBounds,
    //                                        Rectangle rowBounds,
    //                                        int rowIndex,
    //                                        DataGridViewElementStates rowState,
    //                                        string errorText,
    //                                        DataGridViewCellStyle inheritedRowStyle,
    //                                        bool isFirstDisplayedRow,
    //                                        bool isLastVisibleRow)
    //{
    //    _dataGridView = dataGridView ?? throw new ArgumentNullException(nameof(dataGridView));
    //    Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
    //    ClipBounds = clipBounds;
    //    RowBounds = rowBounds;
    //    RowIndex = rowIndex;
    //    State = rowState;
    //    ErrorText = errorText;
    //    InheritedRowStyle = inheritedRowStyle ?? throw new ArgumentNullException(nameof(inheritedRowStyle));
    //    IsFirstDisplayedRow = isFirstDisplayedRow;
    //    IsLastVisibleRow = isLastVisibleRow;
    //    _paintParts = DataGridViewPaintParts.All;
    //}

    //internal DataGridViewRowPrePaintEventArgs(DataGridView dataGridView)
    //{
    //    Debug.Assert(dataGridView != null);
    //    _dataGridView = dataGridView;
    //}

    //public Rectangle ClipBounds { get; set; }

    //public DataGridViewPaintParts PaintParts
    //{
    //    get => _paintParts;
    //    set
    //    {
    //        if ((value & ~DataGridViewPaintParts.All) != 0)
    //        {
    //            throw new ArgumentException(string.Format(SR.DataGridView_InvalidDataGridViewPaintPartsCombination, nameof(value)), nameof(value));
    //        }

    //        _paintParts = value;
    //    }
    //}

    //public void DrawFocus(Rectangle bounds, bool cellsPaintSelectionBackground)
    //{
    //    if (RowIndex < 0 || RowIndex >= _dataGridView.Rows.Count)
    //    {
    //        throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
    //    }

    //    _dataGridView.Rows.SharedRow(RowIndex).DrawFocus(Graphics,
    //                                                     ClipBounds,
    //                                                     bounds,
    //                                                     RowIndex,
    //                                                     State,
    //                                                     InheritedRowStyle,
    //                                                     cellsPaintSelectionBackground);
    //}

    //public void PaintCells(Rectangle clipBounds, DataGridViewPaintParts paintParts)
    //{
    //    if (RowIndex < 0 || RowIndex >= _dataGridView.Rows.Count)
    //    {
    //        throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
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

    //public void PaintCellsBackground(Rectangle clipBounds, bool cellsPaintSelectionBackground)
    //{
    //    if (RowIndex < 0 || RowIndex >= _dataGridView.Rows.Count)
    //    {
    //        throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
    //    }

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

    //public void PaintCellsContent(Rectangle clipBounds)
    //{
    //    if (RowIndex < 0 || RowIndex >= _dataGridView.Rows.Count)
    //    {
    //        throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
    //    }

    //    _dataGridView.Rows.SharedRow(RowIndex).PaintCells(Graphics,
    //                                                      clipBounds,
    //                                                      RowBounds,
    //                                                      RowIndex,
    //                                                      State,
    //                                                      IsFirstDisplayedRow,
    //                                                      IsLastVisibleRow,
    //                                                      DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon);
    //}

    //public void PaintHeader(bool paintSelectionBackground)
    //{
    //    DataGridViewPaintParts paintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.ContentBackground | DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.ErrorIcon;
    //    if (paintSelectionBackground)
    //    {
    //        paintParts |= DataGridViewPaintParts.SelectionBackground;
    //    }
    //    PaintHeader(paintParts);
    //}

    //public void PaintHeader(DataGridViewPaintParts paintParts)
    //{
    //    if (RowIndex < 0 || RowIndex >= _dataGridView.Rows.Count)
    //    {
    //        throw new InvalidOperationException(SR.DataGridViewElementPaintingEventArgs_RowIndexOutOfRange);
    //    }

    //    _dataGridView.Rows.SharedRow(RowIndex).PaintHeader(Graphics,
    //                                                       ClipBounds,
    //                                                       RowBounds,
    //                                                       RowIndex,
    //                                                       State,
    //                                                       IsFirstDisplayedRow,
    //                                                       IsLastVisibleRow,
    //                                                       paintParts);
    //}

    //internal void SetProperties(Graphics graphics,
    //                            Rectangle clipBounds,
    //                            Rectangle rowBounds,
    //                            int rowIndex,
    //                            DataGridViewElementStates rowState,
    //                            string errorText,
    //                            DataGridViewCellStyle inheritedRowStyle,
    //                            bool isFirstDisplayedRow,
    //                            bool isLastVisibleRow)
    //{
    //    Debug.Assert(graphics != null);

    //    Graphics = graphics;
    //    ClipBounds = clipBounds;
    //    RowBounds = rowBounds;
    //    RowIndex = rowIndex;
    //    State = rowState;
    //    ErrorText = errorText;
    //    InheritedRowStyle = inheritedRowStyle;
    //    IsFirstDisplayedRow = isFirstDisplayedRow;
    //    IsLastVisibleRow = isLastVisibleRow;
    //    _paintParts = DataGridViewPaintParts.All;
    //    Handled = false;
    //}
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.SortCompare" /> event.
/// </summary>
public class DataGridViewSortCompareEventArgs : HandledEventArgs
{
    /// <summary>
    /// The cell value1
    /// </summary>
    private readonly object cellValue1, cellValue2;

    /// <summary>
    /// The data grid view column
    /// </summary>
    private readonly DataGridViewColumn dataGridViewColumn;

    /// <summary>
    /// The row index1
    /// </summary>
    private readonly int rowIndex1;

    /// <summary>
    /// The row index2
    /// </summary>
    private readonly int rowIndex2;

    /// <summary>
    /// The sort result
    /// </summary>
    private int sortResult;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewSortCompareEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridViewColumn">
    /// The data grid view column.
    /// </param>
    /// <param name="cellValue1">
    /// The cell value1.
    /// </param>
    /// <param name="cellValue2">
    /// The cell value2.
    /// </param>
    /// <param name="rowIndex1">
    /// The row index1.
    /// </param>
    /// <param name="rowIndex2">
    /// The row index2.
    /// </param>
    public DataGridViewSortCompareEventArgs(DataGridViewColumn dataGridViewColumn,
                object cellValue1,
                object cellValue2,
                int rowIndex1,
                int rowIndex2)
    {
        Debug.Assert(dataGridViewColumn != null);
        this.dataGridViewColumn = dataGridViewColumn;
        this.cellValue1 = cellValue1;
        this.cellValue2 = cellValue2;
        this.rowIndex1 = rowIndex1;
        this.rowIndex2 = rowIndex2;
    }

    /// <summary>
    /// Gets the cell value1.
    /// </summary>
    /// <value>
    /// The cell value1.
    /// </value>
    public object CellValue1
    {
        get
        {
            return cellValue1;
        }
    }

    /// <summary>
    /// Gets the cell value2.
    /// </summary>
    /// <value>
    /// The cell value2.
    /// </value>
    public object CellValue2
    {
        get
        {
            return cellValue2;
        }
    }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public DataGridViewColumn Column
    {
        get
        {
            return dataGridViewColumn;
        }
    }

    /// <summary>
    /// Gets the row index1.
    /// </summary>
    /// <value>
    /// The row index1.
    /// </value>
    public int RowIndex1
    {
        get
        {
            return rowIndex1;
        }
    }

    /// <summary>
    /// Gets the row index2.
    /// </summary>
    /// <value>
    /// The row index2.
    /// </value>
    public int RowIndex2
    {
        get
        {
            return rowIndex2;
        }
    }

    /// <summary>
    /// Gets or sets the sort result.
    /// </summary>
    /// <value>
    /// The sort result.
    /// </value>
    public int SortResult
    {
        get
        {
            return sortResult;
        }
        set
        {
            sortResult = value;
        }
    }

}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.DataGridView.SortCompare" /> event.
    /// </summary>
    public class DataGridViewSortCompareEventArgs : HandledEventArgs
    {

        private readonly DataGridViewColumn dataGridViewColumn;

        private readonly object cellValue1, cellValue2;

        private int sortResult;

        private readonly int rowIndex1;
        private readonly int rowIndex2;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DataGridViewSortCompareEventArgs" /> class.
        /// </summary>
        /// <param name="dataGridViewColumn">The column to sort.
        /// </param>
        /// <param name="cellValue1">The value of the first cell to compare.
        /// </param>
        /// <param name="cellValue2">The value of the second cell to compare.
        /// </param>
        /// <param name="rowIndex1">The index of the row containing the first cell.
        /// </param>
        /// <param name="rowIndex2">The index of the row containing the second cell.
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
        /// Gets the value of the first cell to compare.
        /// </summary>
        /// <returns>The value of the first cell.
        /// </returns>
        public object CellValue1
        {
            get
            {
                return cellValue1;
            }
        }
        /// <summary>
        /// Gets the value of the second cell to compare.
        /// </summary>
        /// <returns>The value of the second cell.
        /// </returns>
        public object CellValue2
        {
            get
            {
                return cellValue2;
            }
        }
        /// <summary>
        /// Gets the column being sorted.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.DataGridViewColumn" /> to sort.
        /// </returns>
        public DataGridViewColumn Column
        {
            get
            {
                return dataGridViewColumn;
            }
        }
        /// <summary>
        /// Gets the index of the row containing the first cell to compare.
        /// </summary>
        /// <returns>The index of the row containing the second cell.
        /// </returns>
        public int RowIndex1
        {
            get
            {
                return rowIndex1;
            }
        }
        /// <summary>
        /// Gets the index of the row containing the second cell to compare.
        /// </summary>
        /// <returns>The index of the row containing the second cell.
        /// </returns>
        public int RowIndex2
        {
            get
            {
                return rowIndex2;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the order in which the compared cells will be sorted.
        /// </summary>
        /// <returns>Less than zero if the first cell will be sorted before the second cell; zero if the first cell and second cell have equivalent values; greater than zero if the second cell will be sorted before the first cell.
        /// </returns>
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
}

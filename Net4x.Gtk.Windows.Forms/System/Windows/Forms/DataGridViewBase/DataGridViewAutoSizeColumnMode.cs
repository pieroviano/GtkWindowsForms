using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum DataGridViewAutoSizeColumnMode
    {
        //
        // Summary:
        //     The sizing behavior of the column is inherited from the System.Windows.Forms.DataGridView.AutoSizeColumnsMode
        //     property.
        NotSet = 0,
        //
        // Summary:
        //     The column width does not automatically adjust.
        None = 1,
        //
        // Summary:
        //     The column width adjusts to fit the contents of the column header cell.
        ColumnHeader = 2,
        //
        // Summary:
        //     The column width adjusts to fit the contents of all cells in the column, excluding
        //     the header cell.
        AllCellsExceptHeader = 4,
        //
        // Summary:
        //     The column width adjusts to fit the contents of all cells in the column, including
        //     the header cell.
        AllCells = 6,
        //
        // Summary:
        //     The column width adjusts to fit the contents of all cells in the column that
        //     are in rows currently displayed onscreen, excluding the header cell.
        DisplayedCellsExceptHeader = 8,
        //
        // Summary:
        //     The column width adjusts to fit the contents of all cells in the column that
        //     are in rows currently displayed onscreen, including the header cell.
        DisplayedCells = 10,
        //
        // Summary:
        //     The column width adjusts so that the widths of all columns exactly fills the
        //     display area of the control, requiring horizontal scrolling only to keep column
        //     widths above the System.Windows.Forms.DataGridViewColumn.MinimumWidth property
        //     values. Relative column widths are determined by the relative System.Windows.Forms.DataGridViewColumn.FillWeight
        //     property values.
        Fill = 16
    }
}

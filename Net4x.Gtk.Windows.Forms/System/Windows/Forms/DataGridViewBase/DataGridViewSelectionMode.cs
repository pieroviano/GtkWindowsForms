using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Describes how cells of a DataGridView control can be selected.
    public enum DataGridViewSelectionMode
    {
        //
        // Summary:
        //     One or more individual cells can be selected.
        CellSelect = 0,
        //
        // Summary:
        //     The entire row will be selected by clicking its row's header or a cell contained
        //     in that row.
        FullRowSelect = 1,
        //
        // Summary:
        //     The entire column will be selected by clicking the column's header or a cell
        //     contained in that column.
        FullColumnSelect = 2,
        //
        // Summary:
        //     The row will be selected by clicking in the row's header cell. An individual
        //     cell can be selected by clicking that cell.
        RowHeaderSelect = 3,
        //
        // Summary:
        //     The column will be selected by clicking in the column's header cell. An individual
        //     cell can be selected by clicking that cell.
        ColumnHeaderSelect = 4
    }
}

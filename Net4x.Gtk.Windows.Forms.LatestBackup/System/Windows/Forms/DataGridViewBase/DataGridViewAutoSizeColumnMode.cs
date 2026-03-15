using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how the width of a column is adjusted.
/// </summary>
public enum DataGridViewAutoSizeColumnMode
{

    /// <summary>
    /// The sizing behavior of the column is inherited from the System.Windows.Forms.DataGridView.AutoSizeColumnsMode
    /// property.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// The column width does not automatically adjust.
    /// </summary>
    None = 1,

    /// <summary>
    /// The column width adjusts to fit the contents of the column header cell.
    /// </summary>
    ColumnHeader = 2,

    /// <summary>
    /// The column width adjusts to fit the contents of all cells in the column, excluding
    /// the header cell.
    /// </summary>
    AllCellsExceptHeader = 4,

    /// <summary>
    /// The column width adjusts to fit the contents of all cells in the column, including
    /// the header cell.
    /// </summary>
    AllCells = 6,

    /// <summary>
    /// The column width adjusts to fit the contents of all cells in the column that
    /// are in rows currently displayed onscreen, excluding the header cell.
    /// </summary>
    DisplayedCellsExceptHeader = 8,

    /// <summary>
    /// The column width adjusts to fit the contents of all cells in the column that
    /// are in rows currently displayed onscreen, including the header cell.
    /// </summary>
    DisplayedCells = 10,

    /// <summary>
    /// The column width adjusts so that the widths of all columns exactly fills the
    /// display area of the control, requiring horizontal scrolling only to keep column
    /// widths above the System.Windows.Forms.DataGridViewColumn.MinimumWidth property
    /// values. Relative column widths are determined by the relative System.Windows.Forms.DataGridViewColumn.FillWeight
    /// property values.
    /// </summary>
    Fill = 16
}
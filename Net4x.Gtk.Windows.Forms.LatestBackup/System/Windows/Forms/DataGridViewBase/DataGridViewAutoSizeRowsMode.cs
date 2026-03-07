// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Defines values for specifying how the heights of rows are adjusted.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum DataGridViewAutoSizeRowsMode
    {
        /// <summary>
        /// The row heights adjust to fit the contents of all cells in the rows, including header cells.
        /// </summary>
        AllCells = 0,
        /// <summary>
        /// The row heights adjust to fit the contents of all cells in the rows, excluding header cells.
        /// </summary>
        AllCellsExceptHeaders = 1,
        /// <summary>
        /// The row heights adjust to fit the contents of the row header.
        /// </summary>
        AllHeaders = 2,
        /// <summary>
        /// The row heights adjust to fit the contents of all cells in rows currently displayed onscreen, including header cells.
        /// </summary>
        DisplayedCells = 3,
        /// <summary>
        /// The row heights adjust to fit the contents of all cells in rows currently displayed onscreen, excluding header cells.
        /// </summary>
        DisplayedCellsExceptHeaders = 4,
        /// <summary>
        /// The row heights adjust to fit the contents of the row headers currently displayed onscreen.
        /// </summary>
        DisplayedHeaders = 5,
        /// <summary>
        /// The row heights do not automatically adjust.
        /// </summary>
        None = 6,
    }
}

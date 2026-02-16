using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum DataGridViewColumnSortMode
    {

        /// <summary>
        /// The column can only be sorted programmatically, but it is not intended for sorting,
        /// so the column header will not include space for a sorting glyph.
        /// </summary>
        NotSortable = 0,

        /// <summary>
        /// The user can sort the column by clicking the column header (or pressing F3 on
        /// a cell) unless the column headers are used for selection. A sorting glyph will
        /// be displayed automatically.
        /// </summary>
        Automatic = 1,

        /// <summary>
        /// The column can only be sorted programmatically, and the column header will include
        /// space for a sorting glyph.
        /// </summary>
        Programmatic = 2
    }
}
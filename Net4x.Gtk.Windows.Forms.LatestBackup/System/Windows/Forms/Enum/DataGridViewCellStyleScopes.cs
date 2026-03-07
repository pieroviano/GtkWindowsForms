// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the
    /// <see cref="System.Windows.Forms.DataGridView" />
    /// entity that owns the cell style that was changed.
    /// </summary>
    [Flags]
    public enum DataGridViewCellStyleScopes
    {
        /// <summary>
        /// The owning entity is unspecified.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridViewCell.Style" /> property changed.
        /// </summary>
        Cell = 0x01,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridViewColumn.DefaultCellStyle" /> property changed.
        /// </summary>
        Column = 0x02,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridViewRow.DefaultCellStyle" /> property changed.
        /// </summary>
        Row = 0x04,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridView.DefaultCellStyle" /> property changed.
        /// </summary>
        DataGridView = 0x08,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridView.ColumnHeadersDefaultCellStyle" /> property changed.
        /// </summary>
        ColumnHeaders = 0x10,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowHeadersDefaultCellStyle" /> property changed.
        /// </summary>
        RowHeaders = 0x20,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridView.RowsDefaultCellStyle" /> property changed.
        /// </summary>
        Rows = 0x40,
        /// <summary>
        /// One or more values of the object returned by the
        /// <see cref="System.Windows.Forms.DataGridView.AlternatingRowsDefaultCellStyle" /> property changed.
        /// </summary>
        AlternatingRows = 0x80
    }
}

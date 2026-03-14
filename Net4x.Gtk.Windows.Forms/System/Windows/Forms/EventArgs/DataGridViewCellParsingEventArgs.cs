// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellParsing" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
public class DataGridViewCellParsingEventArgs : ConvertEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewCellParsingEventArgs" /> class.
    /// </summary>
    /// <param name="rowIndex">
    /// Index of the row.
    /// </param>
    /// <param name="columnIndex">
    /// Index of the column.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="desiredType">
    /// Type of the desired.
    /// </param>
    /// <param name="inheritedCellStyle">
    /// The inherited cell style.
    /// </param>
    public DataGridViewCellParsingEventArgs(int rowIndex,
                                             int columnIndex,
                                             object value,
                                             Type desiredType,
                                             DataGridViewCellStyle inheritedCellStyle) : base(value, desiredType)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        InheritedCellStyle = inheritedCellStyle;
    }

    /// <summary>
    /// Gets the index of the column.
    /// </summary>
    /// <value>
    /// The index of the column.
    /// </value>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets or sets the inherited cell style.
    /// </summary>
    /// <value>
    /// The inherited cell style.
    /// </value>
    public DataGridViewCellStyle InheritedCellStyle { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parsing applied.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if parsing applied; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ParsingApplied { get; set; }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex { get; }

}
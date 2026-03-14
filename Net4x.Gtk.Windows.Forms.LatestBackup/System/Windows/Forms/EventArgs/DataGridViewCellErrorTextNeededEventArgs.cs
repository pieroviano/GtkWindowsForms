// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.CellErrorTextNeeded" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" /> control.
/// </summary>
public class DataGridViewCellErrorTextNeededEventArgs : DataGridViewCellEventArgs
{
    internal DataGridViewCellErrorTextNeededEventArgs(int columnIndex, int rowIndex, string errorText) : base(columnIndex, rowIndex)
    {
        ErrorText = errorText;
    }

    /// <summary>
    /// Gets or sets the error text.
    /// </summary>
    /// <value>The error text.
    /// </value>
    public string ErrorText { get; set; }

}
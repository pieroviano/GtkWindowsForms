// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Represents the top-left corner cell of a <see cref="DataGridView" /> (where the row and column headers
/// meet). The type was absent, so code referencing <c>DataGridView.TopLeftHeaderCell</c>'s concrete type
/// could not compile.
/// </summary>
public class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
{
    /// <summary>Initializes a new instance of the <see cref="DataGridViewTopLeftHeaderCell" /> class.</summary>
    public DataGridViewTopLeftHeaderCell()
    {
    }

    /// <summary>
    /// Adjusts the border style for the top-left header cell. On this layer the placeholder is returned
    /// unchanged (falling back to the input), matching the "no extra border adjustment" behaviour.
    /// </summary>
    /// <param name="dataGridViewAdvancedBorderStyleInput">The proposed border style.</param>
    /// <param name="dataGridViewAdvancedBorderStylePlaceholder">The style to populate and return.</param>
    /// <param name="singleVerticalBorderAdded">Whether a single vertical border was added.</param>
    /// <param name="singleHorizontalBorderAdded">Whether a single horizontal border was added.</param>
    /// <param name="isFirstDisplayedColumn">Whether this is the first displayed column.</param>
    /// <param name="isFirstDisplayedRow">Whether this is the first displayed row.</param>
    /// <returns>The adjusted border style.</returns>
    public virtual DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
        DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder,
        bool singleVerticalBorderAdded,
        bool singleHorizontalBorderAdded,
        bool isFirstDisplayedColumn,
        bool isFirstDisplayedRow) =>
        dataGridViewAdvancedBorderStylePlaceholder ?? dataGridViewAdvancedBorderStyleInput;
}

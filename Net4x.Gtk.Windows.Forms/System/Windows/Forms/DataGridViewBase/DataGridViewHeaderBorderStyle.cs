// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying the border styles of a DataGridView header cell.
/// </summary>
public enum DataGridViewHeaderBorderStyle
{
    /// <summary>A border defined by the AdvancedCellBorderStyle property.</summary>
    Custom = 0,
    /// <summary>A single-line border.</summary>
    Single = 1,
    /// <summary>A three-dimensional raised border.</summary>
    Raised = 2,
    /// <summary>A three-dimensional sunken border.</summary>
    Sunken = 3,
    /// <summary>No borders.</summary>
    None = 4
}

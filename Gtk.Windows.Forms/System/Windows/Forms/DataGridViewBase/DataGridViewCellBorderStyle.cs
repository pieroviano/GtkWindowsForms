// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying the border styles of the cells in a DataGridView.
/// </summary>
public enum DataGridViewCellBorderStyle
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
    None = 4,
    /// <summary>A single-line vertical border.</summary>
    SingleVertical = 5,
    /// <summary>A three-dimensional raised vertical border.</summary>
    RaisedVertical = 6,
    /// <summary>A three-dimensional sunken vertical border.</summary>
    SunkenVertical = 7,
    /// <summary>A single-line horizontal border.</summary>
    SingleHorizontal = 8,
    /// <summary>A three-dimensional raised horizontal border.</summary>
    RaisedHorizontal = 9,
    /// <summary>A three-dimensional sunken horizontal border.</summary>
    SunkenHorizontal = 10
}

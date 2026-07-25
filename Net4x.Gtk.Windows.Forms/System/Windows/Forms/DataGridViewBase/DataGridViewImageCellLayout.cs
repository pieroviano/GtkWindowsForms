// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how an image is positioned within a DataGridViewImageCell.
/// </summary>
public enum DataGridViewImageCellLayout
{
    /// <summary>No layout is specified.</summary>
    NotSet = 0,
    /// <summary>The image is centered in the cell without scaling.</summary>
    Normal = 1,
    /// <summary>The image is stretched to fill the cell.</summary>
    Stretch = 2,
    /// <summary>The image is scaled to fit while preserving its aspect ratio.</summary>
    Zoom = 3
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines constants that represent areas in a <see cref="T:System.Windows.Forms.ListView" /> or <see cref="T:System.Windows.Forms.ListViewItem" />.</summary>
[Flags]
public enum ListViewHitTestLocations
{
    /// <summary>A position in the client area of the control, but not on an item.</summary>
    None = 1,

    /// <summary>A position within the bounds of an image contained in a ListView or ListViewItem.</summary>
    Image = 2,

    /// <summary>A position within the bounds of a label contained in a ListView or ListViewItem.</summary>
    Label = 4,

    /// <summary>A position below the client portion of a ListView control.</summary>
    BelowClientArea = 16,

    /// <summary>A position to the right of the client portion of a ListView control.</summary>
    RightOfClientArea = 32,

    /// <summary>A position to the left of the client portion of a ListView control.</summary>
    LeftOfClientArea = 64,

    /// <summary>A position above the client portion of a ListView control.</summary>
    AboveClientArea = 256,

    /// <summary>A position within the bounds of a state image contained in a ListView or ListViewItem.</summary>
    StateImage = 512,
}

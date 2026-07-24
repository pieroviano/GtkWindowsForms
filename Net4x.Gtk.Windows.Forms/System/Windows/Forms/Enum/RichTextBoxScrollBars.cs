// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the type of scroll bars to display in a <see cref="T:System.Windows.Forms.RichTextBox" /> control.</summary>
public enum RichTextBoxScrollBars
{
    /// <summary>No scroll bars are shown.</summary>
    None = 0,

    /// <summary>Displays a horizontal scroll bar when needed.</summary>
    Horizontal = 1,

    /// <summary>Displays a vertical scroll bar when needed.</summary>
    Vertical = 2,

    /// <summary>Displays both a horizontal and vertical scroll bar when needed.</summary>
    Both = 3,

    /// <summary>Always displays a horizontal scroll bar.</summary>
    ForcedHorizontal = 17,

    /// <summary>Always displays a vertical scroll bar.</summary>
    ForcedVertical = 18,

    /// <summary>Always displays both a horizontal and a vertical scroll bar.</summary>
    ForcedBoth = 19,
}

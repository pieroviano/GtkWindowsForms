// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the <see cref="!:StatusBar.DrawItem" /> event.</summary>
public class StatusBarDrawItemEventArgs : DrawItemEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.StatusBarDrawItemEventArgs" /> class.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="font">The font used to draw text.</param>
    /// <param name="r">The bounds of the panel.</param>
    /// <param name="itemId">The index of the panel.</param>
    /// <param name="itemState">The state of the panel.</param>
    /// <param name="panel">The panel being drawn.</param>
    public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel)
        : base(g, font, r, itemId, itemState)
    {
        Panel = panel;
    }

    /// <summary>Initializes a new instance with explicit fore and back colors.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="font">The font used to draw text.</param>
    /// <param name="r">The bounds of the panel.</param>
    /// <param name="itemId">The index of the panel.</param>
    /// <param name="itemState">The state of the panel.</param>
    /// <param name="panel">The panel being drawn.</param>
    /// <param name="foreColor">The foreground color.</param>
    /// <param name="backColor">The background color.</param>
    public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel, Color foreColor, Color backColor)
        : base(g, font, r, itemId, itemState, foreColor, backColor)
    {
        Panel = panel;
    }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.StatusBarPanel" /> being drawn.</summary>
    public StatusBarPanel Panel { get; }
}

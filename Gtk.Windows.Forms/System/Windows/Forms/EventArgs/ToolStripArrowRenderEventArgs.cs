// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the arrow rendering event on tool strip items.</summary>
public class ToolStripArrowRenderEventArgs : EventArgs
{
    /// <summary>Initializes a new instance.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="toolStripItem">The item whose arrow is painted.</param>
    /// <param name="arrowRectangle">The bounds of the arrow.</param>
    /// <param name="arrowColor">The color of the arrow.</param>
    /// <param name="arrowDirection">The direction the arrow points.</param>
    public ToolStripArrowRenderEventArgs(Graphics g, ToolStripItem toolStripItem, Rectangle arrowRectangle, Color arrowColor, ArrowDirection arrowDirection)
    {
        Graphics = g;
        Item = toolStripItem;
        ArrowRectangle = arrowRectangle;
        ArrowColor = arrowColor;
        Direction = arrowDirection;
    }

    /// <summary>Gets or sets the bounds of the arrow.</summary>
    public Rectangle ArrowRectangle { get; set; }

    /// <summary>Gets or sets the color of the arrow.</summary>
    public Color ArrowColor { get; set; }

    /// <summary>Gets or sets the direction the arrow points.</summary>
    public ArrowDirection Direction { get; set; }

    /// <summary>Gets the graphics used to paint.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the item whose arrow is painted.</summary>
    public ToolStripItem Item { get; }
}

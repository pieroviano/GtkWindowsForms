// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the ToolStrip background and border rendering events.</summary>
public class ToolStripRenderEventArgs : EventArgs
{
    /// <summary>Initializes a new instance for the specified tool strip.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="toolStrip">The tool strip to paint.</param>
    public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip)
        : this(g, toolStrip, toolStrip == null ? Rectangle.Empty : new Rectangle(Point.Empty, toolStrip.Size), Color.Empty)
    {
    }

    /// <summary>Initializes a new instance with the specified affected bounds and back color.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="toolStrip">The tool strip to paint.</param>
    /// <param name="affectedBounds">The bounds affected by the painting.</param>
    /// <param name="backColor">The background color of the affected area.</param>
    public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip, Rectangle affectedBounds, Color backColor)
    {
        Graphics = g;
        ToolStrip = toolStrip;
        AffectedBounds = affectedBounds;
        BackColor = backColor;
    }

    /// <summary>Gets the bounds affected by the painting.</summary>
    public Rectangle AffectedBounds { get; }

    /// <summary>Gets the background color of the affected area.</summary>
    public Color BackColor { get; }

    /// <summary>Gets the graphics used to paint.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the tool strip to paint.</summary>
    public ToolStrip ToolStrip { get; }

    /// <summary>Gets the area where the tool strip joins an adjacent control when raftable.</summary>
    public Rectangle ConnectedArea => Rectangle.Empty;
}

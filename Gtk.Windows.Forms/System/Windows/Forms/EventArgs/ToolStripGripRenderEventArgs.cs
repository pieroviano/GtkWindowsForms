// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the move handle (grip) rendering event on a tool strip.</summary>
public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs
{
    /// <summary>Initializes a new instance for the specified tool strip.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="toolStrip">The tool strip whose grip is painted.</param>
    public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip)
    {
    }

    /// <summary>Gets the bounds of the grip.</summary>
    public Rectangle GripBounds => Rectangle.Empty;

    /// <summary>Gets the orientation of the grip.</summary>
    public ToolStripGripDisplayStyle GripDisplayStyle => ToolStripGripDisplayStyle.Vertical;

    /// <summary>Gets the visibility and mouse reaction of the grip.</summary>
    public ToolStripGripStyle GripStyle => ToolStripGripStyle.Visible;
}

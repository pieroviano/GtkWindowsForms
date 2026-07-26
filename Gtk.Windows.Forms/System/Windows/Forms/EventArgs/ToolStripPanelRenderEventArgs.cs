// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the background rendering of a <see cref="T:System.Windows.Forms.ToolStripPanel" />.</summary>
public class ToolStripPanelRenderEventArgs : EventArgs
{
    /// <summary>Initializes a new instance.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="toolStripPanel">The panel to paint.</param>
    public ToolStripPanelRenderEventArgs(Graphics g, ToolStripPanel toolStripPanel)
    {
        Graphics = g;
        ToolStripPanel = toolStripPanel;
    }

    /// <summary>Gets the graphics used to paint.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the panel to paint.</summary>
    public ToolStripPanel ToolStripPanel { get; }

    /// <summary>Gets or sets a value indicating whether the event was handled.</summary>
    public bool Handled { get; set; }
}

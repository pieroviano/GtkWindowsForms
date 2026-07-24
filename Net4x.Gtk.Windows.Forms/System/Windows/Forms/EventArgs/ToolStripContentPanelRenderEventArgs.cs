// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the background rendering of a <see cref="T:System.Windows.Forms.ToolStripContentPanel" />.</summary>
public class ToolStripContentPanelRenderEventArgs : EventArgs
{
    /// <summary>Initializes a new instance.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="contentPanel">The content panel to paint.</param>
    public ToolStripContentPanelRenderEventArgs(Graphics g, ToolStripContentPanel contentPanel)
    {
        Graphics = g;
        ToolStripContentPanel = contentPanel;
    }

    /// <summary>Gets the graphics used to paint.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the content panel to paint.</summary>
    public ToolStripContentPanel ToolStripContentPanel { get; }

    /// <summary>Gets or sets a value indicating whether the event was handled.</summary>
    public bool Handled { get; set; }
}

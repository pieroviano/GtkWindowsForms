// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a row of tool strips within a <see cref="T:System.Windows.Forms.ToolStripPanel" />.
/// </summary>
public class ToolStripPanelRow : Component
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripPanelRow" /> class.</summary>
    /// <param name="parent">The <see cref="T:System.Windows.Forms.ToolStripPanel" /> that contains this row.</param>
    public ToolStripPanelRow(ToolStripPanel parent)
    {
        ToolStripPanel = parent;
    }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ToolStripPanel" /> that contains this row.</summary>
    public ToolStripPanel ToolStripPanel { get; }

    /// <summary>Gets the orientation of the row, which follows the orientation of its containing panel.</summary>
    public Orientation Orientation => ToolStripPanel?.Orientation ?? Orientation.Horizontal;
}

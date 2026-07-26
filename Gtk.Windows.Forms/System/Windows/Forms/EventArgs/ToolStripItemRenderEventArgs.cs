// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the ToolStrip item rendering events.</summary>
public class ToolStripItemRenderEventArgs : EventArgs
{
    /// <summary>Initializes a new instance for the specified item.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="item">The item to paint.</param>
    public ToolStripItemRenderEventArgs(Graphics g, ToolStripItem item)
    {
        Graphics = g;
        Item = item;
    }

    /// <summary>Gets the graphics used to paint.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the item to paint.</summary>
    public ToolStripItem Item { get; }

    /// <summary>Gets the tool strip that owns the item, if any.</summary>
    public ToolStrip? ToolStrip => Item?.Owner;
}

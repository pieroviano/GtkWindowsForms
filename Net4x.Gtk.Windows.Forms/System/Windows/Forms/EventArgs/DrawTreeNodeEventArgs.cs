// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the <see cref="!:TreeView.DrawNode" /> event.
/// </summary>
public class DrawTreeNodeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.DrawTreeNodeEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> used to draw the node.</param>
    /// <param name="node">The <see cref="T:System.Windows.Forms.TreeNode" /> to draw.</param>
    /// <param name="bounds">The bounds of the node to draw.</param>
    /// <param name="state">The current state of the node to draw.</param>
    public DrawTreeNodeEventArgs(Graphics graphics, TreeNode node, Rectangle bounds, TreeNodeStates state)
    {
        Graphics = graphics;
        Node = node;
        Bounds = bounds;
        State = state;
    }

    /// <summary>Gets or sets a value indicating whether the system draws the node rather than the owner.</summary>
    public bool DrawDefault { get; set; }

    /// <summary>Gets the <see cref="T:System.Drawing.Graphics" /> used to draw the node.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.TreeNode" /> to draw.</summary>
    public TreeNode Node { get; }

    /// <summary>Gets the size and location of the node to draw.</summary>
    public Rectangle Bounds { get; }

    /// <summary>Gets the current state of the node to draw.</summary>
    public TreeNodeStates State { get; }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.TreeView.NodeMouseClick" /> and
/// <see cref="!:System.Windows.Forms.TreeView.NodeMouseDoubleClick" /> events.
/// </summary>
public class TreeNodeMouseClickEventArgs : MouseEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNodeMouseClickEventArgs" /> class.
    /// </summary>
    /// <param name="node">The node.
    /// </param>
    /// <param name="button">The button.
    /// </param>
    /// <param name="clicks">The clicks.
    /// </param>
    /// <param name="x">The x.
    /// </param>
    /// <param name="y">The y.
    /// </param>
    public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
    {
        Node = node;
    }

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <value>The node.
    /// </value>
    public TreeNode Node { get; }

}
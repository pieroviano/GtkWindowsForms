// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.TreeView.NodeMouseHover" /> event.
/// </summary>
[ComVisible(true)]
public class TreeNodeMouseHoverEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeNodeMouseHoverEventArgs" /> class.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    public TreeNodeMouseHoverEventArgs(TreeNode node)
    {
        Node = node;
    }

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <value>
    /// The node.
    /// </value>
    public TreeNode Node { get; }

}
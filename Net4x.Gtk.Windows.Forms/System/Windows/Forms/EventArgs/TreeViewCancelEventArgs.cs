// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.TreeView.BeforeCheck" /> ,
/// <see cref="!:System.Windows.Forms.TreeView.BeforeCollapse" /> ,
/// <see cref="!:System.Windows.Forms.TreeView.BeforeExpand" /> , and
/// <see cref="T:System.Windows.Forms.TreeView.BeforeSelect" /> events of a
/// <see cref="T:System.Windows.Forms.TreeView" /> control.
/// </summary>
public class TreeViewCancelEventArgs : CancelEventArgs
{
    private readonly TreeViewAction action;

    private readonly TreeNode? node;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TreeViewCancelEventArgs" /> class.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <param name="cancel">
    /// if set to
    /// <c>
    /// true
    /// </c>cancel.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    public TreeViewCancelEventArgs(TreeNode? node, bool cancel, TreeViewAction action)
            : base(cancel)
    {
        this.node = node;
        this.action = action;
    }

    /// <summary>
    /// Gets the action.
    /// </summary>
    /// <value>
    /// The action.
    /// </value>
    public TreeViewAction Action
    {
        get
        {
            return action;
        }
    }

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <value>
    /// The node.
    /// </value>
    public TreeNode? Node
    {
        get
        {
            return node;
        }
    }
}
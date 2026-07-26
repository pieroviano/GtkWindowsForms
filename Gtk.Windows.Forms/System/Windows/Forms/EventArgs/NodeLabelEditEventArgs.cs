// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides data for the <see cref="!:TreeView.BeforeLabelEdit" /> and <see cref="!:TreeView.AfterLabelEdit" /> events.</summary>
public class NodeLabelEditEventArgs : EventArgs
{
    /// <summary>Initializes a new instance for the specified node.</summary>
    /// <param name="node">The tree node being edited.</param>
    public NodeLabelEditEventArgs(TreeNode node) { Node = node; }

    /// <summary>Initializes a new instance for the specified node and new label.</summary>
    /// <param name="node">The tree node being edited.</param>
    /// <param name="label">The new label text.</param>
    public NodeLabelEditEventArgs(TreeNode node, string? label) { Node = node; Label = label; }

    /// <summary>Gets or sets a value indicating whether the edit has been cancelled.</summary>
    public bool CancelEdit { get; set; }

    /// <summary>Gets the new label text, or <see langword="null" /> if the label is unchanged.</summary>
    public string? Label { get; }

    /// <summary>Gets the tree node being edited.</summary>
    public TreeNode Node { get; }
}

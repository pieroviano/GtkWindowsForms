// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Contains information about an area of a <see cref="T:System.Windows.Forms.TreeView" /> control or a
/// <see cref="T:System.Windows.Forms.TreeNode" />, returned by the <c>HitTest</c> methods.
/// </summary>
public class TreeViewHitTestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.TreeViewHitTestInfo" /> class.
    /// </summary>
    /// <param name="hitNode">The <see cref="T:System.Windows.Forms.TreeNode" /> located at the hit-tested point.</param>
    /// <param name="hitLocation">One of the <see cref="T:System.Windows.Forms.TreeViewHitTestLocations" /> values.</param>
    public TreeViewHitTestInfo(TreeNode? hitNode, TreeViewHitTestLocations hitLocation)
    {
        Node = hitNode;
        Location = hitLocation;
    }

    /// <summary>Gets the location of the hit test on the <see cref="T:System.Windows.Forms.TreeView" /> control, in relation to the control and its nodes.</summary>
    public TreeViewHitTestLocations Location { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.TreeNode" /> located at the hit-tested point.</summary>
    public TreeNode? Node { get; }
}

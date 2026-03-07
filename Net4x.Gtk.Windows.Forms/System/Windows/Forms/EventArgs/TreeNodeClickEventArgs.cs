// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.TreeView.NodeMouseClick" /> and
    /// <see cref="!:System.Windows.Forms.TreeView.NodeMouseDoubleClick" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TreeNodeMouseClickEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeNodeMouseClickEventArgs" /> class.
        /// </summary>
        /// <param name="node">The node that was clicked.
        /// </param>
        /// <param name="button">One of the
        /// <see cref="System.Windows.Forms.MouseButtons" /> members.
        /// </param>
        /// <param name="clicks">The number of clicks that occurred.
        /// </param>
        /// <param name="x">The x-coordinate where the click occurred.
        /// </param>
        /// <param name="y">The y-coordinate where the click occurred.
        /// </param>
        public TreeNodeMouseClickEventArgs(TreeNode node, MouseButtons button, int clicks, int x, int y) : base(button, clicks, x, y, 0)
        {
            Node = node;
        }
        /// <summary>
        /// Gets the node that was clicked.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TreeNode" /> that was clicked.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeNode Node { get; }
    }
}

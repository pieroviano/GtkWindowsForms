// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.TreeView.AfterCheck" /> ,
    /// <see cref="System.Windows.Forms.TreeView.AfterCollapse" /> ,
    /// <see cref="System.Windows.Forms.TreeView.AfterExpand" /> , or
    /// <see cref="System.Windows.Forms.TreeView.AfterSelect" /> events of a
    /// <see cref="System.Windows.Forms.TreeView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TreeViewEventArgs : EventArgs
    {
        readonly TreeViewAction action = TreeViewAction.Unknown;

        readonly TreeNode node;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeViewEventArgs" /> class for the specified tree node.
        /// </summary>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> that the event is responding to.
        /// </param>
        public TreeViewEventArgs(TreeNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeViewEventArgs" /> class for the specified tree node and with the specified type of action that raised the event.
        /// </summary>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> that the event is responding to.
        /// </param>
        /// <param name="action">The type of
        /// <see cref="System.Windows.Forms.TreeViewAction" /> that raised the event.
        /// </param>
        public TreeViewEventArgs(TreeNode node, TreeViewAction action)
        {
            this.node = node;
            this.action = action;
        }

        /// <summary>
        /// Gets the type of action that raised the event.
        /// </summary>
        /// <returns>The type of
        /// <see cref="System.Windows.Forms.TreeViewAction" /> that raised the event.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }

        /// <summary>
        /// Gets the tree node that has been checked, expanded, collapsed, or selected.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TreeNode" /> that has been checked, expanded, collapsed, or selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TreeNode Node
        {
            get
            {
                return node;
            }
        }

    }
}

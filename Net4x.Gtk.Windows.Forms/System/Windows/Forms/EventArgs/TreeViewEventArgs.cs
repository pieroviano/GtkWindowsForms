// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.TreeView.AfterCheck" /> ,
    /// <see cref="T:System.Windows.Forms.TreeView.AfterCollapse" /> ,
    /// <see cref="T:System.Windows.Forms.TreeView.AfterExpand" /> , or
    /// <see cref="T:System.Windows.Forms.TreeView.AfterSelect" /> events of a
    /// <see cref="T:System.Windows.Forms.TreeView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TreeViewEventArgs : EventArgs
    {
        readonly TreeViewAction action = TreeViewAction.Unknown;

        readonly TreeNode node;

        public TreeViewEventArgs(TreeNode node)
        {
            this.node = node;
        }

        public TreeViewEventArgs(TreeNode node, TreeViewAction action)
        {
            this.node = node;
            this.action = action;
        }

        /// <summary>
        /// An event specific action-flag.
        /// </summary>
        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }

        public TreeNode Node
        {
            get
            {
                return node;
            }
        }

    }
}

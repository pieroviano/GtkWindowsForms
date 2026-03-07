// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.TreeView.BeforeCheck" /> ,
    /// <see cref="!:System.Windows.Forms.TreeView.BeforeCollapse" /> ,
    /// <see cref="!:System.Windows.Forms.TreeView.BeforeExpand" /> , and
    /// <see cref="System.Windows.Forms.TreeView.BeforeSelect" /> events of a
    /// <see cref="System.Windows.Forms.TreeView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TreeViewCancelEventArgs : CancelEventArgs
    {
        private readonly TreeViewAction action;

        private readonly TreeNode node;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TreeViewCancelEventArgs" /> class with the specified tree node, a value specifying whether the event is to be canceled, and the type of tree view action that raised the event.
        /// </summary>
        /// <param name="node">The
        /// <see cref="System.Windows.Forms.TreeNode" /> that the event is responding to.
        /// </param>
        /// <param name="cancel">true to cancel the event; otherwise, false.
        /// </param>
        /// <param name="action">One of the
        /// <see cref="System.Windows.Forms.TreeViewAction" /> values indicating the type of action that raised the event.
        /// </param>
        public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action)
                : base(cancel)
        {
            this.node = node;
            this.action = action;
        }
        /// <summary>
        /// Gets the type of
        /// <see cref="System.Windows.Forms.TreeView" /> action that raised the event.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TreeViewAction" /> values.
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
        /// Gets the tree node to be checked, expanded, collapsed, or selected.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.TreeNode" /> to be checked, expanded, collapsed, or selected.
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

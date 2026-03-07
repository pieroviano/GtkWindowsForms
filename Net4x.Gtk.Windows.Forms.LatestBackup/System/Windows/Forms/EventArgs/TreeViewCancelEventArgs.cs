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

        public TreeViewCancelEventArgs(TreeNode node, bool cancel, TreeViewAction action)
        : base(cancel)
        {
            this.node = node;
            this.action = action;
        }

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

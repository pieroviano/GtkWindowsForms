// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the action that raised a
    /// <see cref="System.Windows.Forms.TreeViewEventArgs" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum TreeViewAction
    {
        /// <summary>
        /// The action is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The event was caused by a keystroke.
        /// </summary>
        ByKeyboard = 1,

        /// <summary>
        /// The event was caused by a mouse click.
        /// </summary>
        ByMouse = 2,

        /// <summary>
        /// The event was caused by the
        /// <see cref="System.Windows.Forms.TreeNode" /> collapsing.
        /// </summary>
        Collapse = 3,

        /// <summary>
        /// The event was caused by the
        /// <see cref="System.Windows.Forms.TreeNode" /> expanding.
        /// </summary>
        Expand = 4,
    }
}

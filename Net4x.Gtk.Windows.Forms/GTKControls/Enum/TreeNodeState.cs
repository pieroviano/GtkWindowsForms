// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Defines constants that represent the possible states of a System.Windows.Forms.TreeNode.
    [Flags]
    public enum TreeNodeStates
    {
        //
        // Summary:
        //     The node is selected.
        Selected = 1,
        //
        // Summary:
        //     The node is disabled.
        Grayed = 2,
        //
        // Summary:
        //     The node is checked.
        Checked = 8,
        //
        // Summary:
        //     The node has focus.
        Focused = 16,
        //
        // Summary:
        //     The node is in its default state.
        Default = 32,
        //
        // Summary:
        //     The node is hot. This state occurs when the System.Windows.Forms.TreeView.HotTracking
        //     property is set to true and the mouse pointer is over the node.
        Hot = 64,
        //
        // Summary:
        //     The node is marked.
        Marked = 128,
        //
        // Summary:
        //     The node in an indeterminate state.
        Indeterminate = 256,
        //
        // Summary:
        //     The node should indicate a keyboard shortcut.
        ShowKeyboardCues = 512
    }
}

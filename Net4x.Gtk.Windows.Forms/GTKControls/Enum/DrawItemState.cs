// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Specifies the state of an item that is being drawn.
    [Flags]
    public enum DrawItemState
    {
        //
        // Summary:
        //     The item currently has no state.
        None = 0,
        //
        // Summary:
        //     The item is selected.
        Selected = 1,
        //
        // Summary:
        //     The item is grayed. Only menu controls use this value.
        Grayed = 2,
        //
        // Summary:
        //     The item is unavailable.
        Disabled = 4,
        //
        // Summary:
        //     The item is checked. Only menu controls use this value.
        Checked = 8,
        //
        // Summary:
        //     The item has focus.
        Focus = 16,
        //
        // Summary:
        //     The item is in its default visual state.
        Default = 32,
        //
        // Summary:
        //     The item is being hot-tracked, that is, the item is highlighted as the mouse
        //     pointer passes over it.
        HotLight = 64,
        //
        // Summary:
        //     The item is inactive.
        Inactive = 128,
        //
        // Summary:
        //     The item displays without a keyboard accelerator.
        NoAccelerator = 256,
        //
        // Summary:
        //     The item displays without the visual cue that indicates it has focus.
        NoFocusRect = 512,
        //
        // Summary:
        //     The item is the editing portion of a System.Windows.Forms.ComboBox.
        ComboBoxEdit = 4096
    }
}

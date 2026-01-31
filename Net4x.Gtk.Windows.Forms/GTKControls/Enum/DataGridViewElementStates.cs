using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Specifies the user interface (UI) state of a element within a System.Windows.Forms.DataGridView
    //     control.
    [Flags]
    public enum DataGridViewElementStates
    {
        //
        // Summary:
        //     Indicates that an element is in its default state.
        None = 0,
        //
        // Summary:
        //     Indicates the an element is currently displayed onscreen.
        Displayed = 1,
        //
        // Summary:
        //     Indicates that an element cannot be scrolled through the UI.
        Frozen = 2,
        //
        // Summary:
        //     Indicates that an element will not accept user input to change its value.
        ReadOnly = 4,
        //
        // Summary:
        //     Indicates that an element can be resized through the UI. This value is ignored
        //     except when combined with the System.Windows.Forms.DataGridViewElementStates.ResizableSet
        //     value.
        Resizable = 8,
        //
        // Summary:
        //     Indicates that an element does not inherit the resizable state of its parent.
        ResizableSet = 16,
        //
        // Summary:
        //     Indicates that an element is in a selected (highlighted) UI state.
        Selected = 32,
        //
        // Summary:
        //     Indicates that an element is visible (displayable).
        Visible = 64
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>Provides data for the <see cref="!:ToolStripDropDown.Closing" /> event.</summary>
public class ToolStripDropDownClosingEventArgs : CancelEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripDropDownClosingEventArgs" /> class.</summary>
    /// <param name="reason">The reason the drop-down is closing.</param>
    public ToolStripDropDownClosingEventArgs(ToolStripDropDownCloseReason reason) { CloseReason = reason; }

    /// <summary>Gets the reason the drop-down is closing.</summary>
    public ToolStripDropDownCloseReason CloseReason { get; }
}

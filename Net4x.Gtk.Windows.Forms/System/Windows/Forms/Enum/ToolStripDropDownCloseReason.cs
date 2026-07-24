// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the reason that a <see cref="T:System.Windows.Forms.ToolStripDropDown" /> was closed.</summary>
public enum ToolStripDropDownCloseReason
{
    /// <summary>Closed because another application received the focus.</summary>
    AppFocusChange = 0,

    /// <summary>Closed because the user clicked within another application.</summary>
    AppClicked = 1,

    /// <summary>Closed because the user clicked an item.</summary>
    ItemClicked = 2,

    /// <summary>Closed by keyboard activity.</summary>
    Keyboard = 3,

    /// <summary>Closed programmatically by a call to the Close method.</summary>
    CloseCalled = 4,
}

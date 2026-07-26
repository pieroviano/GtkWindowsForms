// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Represents the state of a data-bound
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// control when a data error occurred.
/// </summary>
[Flags]
public enum DataGridViewDataErrorContexts
{
    /// <summary>
    /// The formatting
    /// </summary>
    Formatting = 0x0001,
    /// <summary>
    /// The display
    /// </summary>
    Display = 0x0002,
    /// <summary>
    /// The preferred size
    /// </summary>
    PreferredSize = 0x0004,
    /// <summary>
    /// The row deletion
    /// </summary>
    RowDeletion = 0x0008,
    /// <summary>
    /// The parsing
    /// </summary>
    Parsing = 0x0100,
    /// <summary>
    /// The commit
    /// </summary>
    Commit = 0x0200,
    /// <summary>
    /// The initial value restoration
    /// </summary>
    InitialValueRestoration = 0x0400,
    /// <summary>
    /// The leave control
    /// </summary>
    LeaveControl = 0x0800,
    /// <summary>
    /// The current cell change
    /// </summary>
    CurrentCellChange = 0x1000,
    /// <summary>
    /// The scroll
    /// </summary>
    Scroll = 0x2000,
    /// <summary>
    /// The clipboard content
    /// </summary>
    ClipboardContent = 0x4000
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides data for <see cref="T:System.Windows.Forms.ToolStripItem" /> events.</summary>
public class ToolStripItemEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.ToolStripItemEventArgs" /> class.</summary>
    /// <param name="item">The item associated with the event.</param>
    public ToolStripItemEventArgs(ToolStripItem item) { Item = item; }

    /// <summary>Gets the item associated with the event.</summary>
    public ToolStripItem Item { get; }
}

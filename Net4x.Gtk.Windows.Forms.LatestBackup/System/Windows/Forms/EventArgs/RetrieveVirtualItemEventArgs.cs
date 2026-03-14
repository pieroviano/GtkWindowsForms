// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ListView.RetrieveVirtualItem" /> event.
/// </summary>
public class RetrieveVirtualItemEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RetrieveVirtualItemEventArgs" /> class.
    /// </summary>
    /// <param name="itemIndex">
    /// Index of the item.
    /// </param>
    public RetrieveVirtualItemEventArgs(int itemIndex)
    {
        ItemIndex = itemIndex;
    }

    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    /// <value>
    /// The item.
    /// </value>
    public ListViewItem Item { get; set; }

    /// <summary>
    /// Gets the index of the item.
    /// </summary>
    /// <value>
    /// The index of the item.
    /// </value>
    public int ItemIndex { get; }

}
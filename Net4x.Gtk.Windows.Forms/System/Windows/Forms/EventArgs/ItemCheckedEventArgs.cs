// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.ListView.ItemChecked" />
/// event of the
/// <see cref="T:System.Windows.Forms.ListView" /> control.
/// </summary>
public class ItemCheckedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ItemCheckedEventArgs" /> class.
    /// </summary>
    /// <param name="item">The item.
    /// </param>
    public ItemCheckedEventArgs(ListViewItem item)
    {
        Item = item;
    }

    /// <summary>
    /// The index of the item that is about to change.
    /// </summary>
    /// <value>The item.
    /// </value>
    public ListViewItem Item { get; }

}
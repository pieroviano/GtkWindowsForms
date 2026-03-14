// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ListView.ItemMouseHover" /> event.
/// </summary>
[ComVisible(true)]
public class ListViewItemMouseHoverEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItemMouseHoverEventArgs" /> class.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public ListViewItemMouseHoverEventArgs(ListViewItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <value>
    /// The item.
    /// </value>
    public ListViewItem Item { get; }

}
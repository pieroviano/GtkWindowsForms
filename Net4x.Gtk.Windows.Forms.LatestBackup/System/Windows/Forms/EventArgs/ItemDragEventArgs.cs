// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ListView.ItemDrag" />
/// event of the
/// <see cref="T:System.Windows.Forms.ListView" /> and
/// <see cref="T:System.Windows.Forms.TreeView" /> controls.
/// </summary>
[ComVisible(true)]
public class ItemDragEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ItemDragEventArgs" /> class.
    /// </summary>
    /// <param name="button">
    /// The button.
    /// </param>
    public ItemDragEventArgs(MouseButtons button) : this(button, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ItemDragEventArgs" /> class.
    /// </summary>
    /// <param name="button">
    /// The button.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    public ItemDragEventArgs(MouseButtons button, object item)
    {
        Button = button;
        Item = item;
    }

    /// <summary>
    /// Gets the button.
    /// </summary>
    /// <value>
    /// The button.
    /// </value>
    public MouseButtons Button { get; }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <value>
    /// The item.
    /// </value>
    public object Item { get; }

}
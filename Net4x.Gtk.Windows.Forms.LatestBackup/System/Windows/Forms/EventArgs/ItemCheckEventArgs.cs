// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.CheckedListBox.ItemCheck" /> event of the
/// <see cref="T:System.Windows.Forms.CheckedListBox" /> and
/// <see cref="T:System.Windows.Forms.ListView" /> controls.
/// </summary>
[ComVisible(true)]
public class ItemCheckEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ItemCheckEventArgs" /> class.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="newCheckValue">
    /// The new check value.
    /// </param>
    /// <param name="currentValue">
    /// The current value.
    /// </param>
    public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
    {
        Index = index;
        NewValue = newCheckValue;
        CurrentValue = currentValue;
    }

    /// <summary>
    /// The current state of the CheckBox.
    /// </summary>
    /// <value>
    /// The current value.
    /// </value>
    public CheckState CurrentValue { get; }

    /// <summary>
    /// The index of the item that is about to change.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index { get; }

    /// <summary>
    /// The proposed new value of the CheckBox.
    /// </summary>
    /// <value>
    /// The new value.
    /// </value>
    public CheckState NewValue { get; set; }

}
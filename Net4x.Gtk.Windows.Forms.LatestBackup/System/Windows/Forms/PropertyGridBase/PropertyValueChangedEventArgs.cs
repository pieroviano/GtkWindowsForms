// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.PropertyGrid.PropertyValueChanged" />
/// event of a
/// <see cref="T:System.Windows.Forms.PropertyGrid" />
/// </summary>
[ComVisible(true)]
public class PropertyValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PropertyValueChangedEventArgs" /> class.
    /// </summary>
    /// <param name="changedItem">
    /// The changed item.
    /// </param>
    /// <param name="oldValue">
    /// The old value.
    /// </param>
    public PropertyValueChangedEventArgs(GridItem changedItem, object oldValue)
    {
        ChangedItem = changedItem;
        OldValue = oldValue;
    }

    /// <summary>
    /// Gets the changed item.
    /// </summary>
    /// <value>
    /// The changed item.
    /// </value>
    public GridItem ChangedItem { get; }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    /// <value>
    /// The old value.
    /// </value>
    public object OldValue { get; }

}
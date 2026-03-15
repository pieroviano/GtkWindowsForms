// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.ListControl.Format" /> event.
/// </summary>
public class ListControlConvertEventArgs : ConvertEventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListControlConvertEventArgs" /> class.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="desiredType">
    /// Type of the desired.
    /// </param>
    /// <param name="listItem">
    /// The list item.
    /// </param>
    public ListControlConvertEventArgs(object value, Type desiredType, object listItem) : base(value, desiredType)
    {
        ListItem = listItem;
    }

    /// <summary>
    /// Gets the list item.
    /// </summary>
    /// <value>
    /// The list item.
    /// </value>
    public object ListItem { get; }

}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Design;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.PropertyGrid.PropertyTabChanged" /> event of a
/// <see cref="T:System.Windows.Forms.PropertyGrid" />
/// </summary>
public class PropertyTabChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PropertyTabChangedEventArgs" /> class.
    /// </summary>
    /// <param name="oldTab">
    /// The old tab.
    /// </param>
    /// <param name="newTab">
    /// The new tab.
    /// </param>
    public PropertyTabChangedEventArgs(PropertyTab? oldTab, PropertyTab? newTab)
    {
        OldTab = oldTab;
        NewTab = newTab;
    }

    /// <summary>
    /// Creates new tab.
    /// </summary>
    /// <value>
    /// The new tab.
    /// </value>
    public PropertyTab? NewTab { get; }

    /// <summary>
    /// Gets the old tab.
    /// </summary>
    /// <value>
    /// The old tab.
    /// </value>
    public PropertyTab? OldTab { get; }

}
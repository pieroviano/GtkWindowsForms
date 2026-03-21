// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.PropertyGrid.SelectedGridItemChanged" />
/// event of the
/// <see cref="T:System.Windows.Forms.PropertyGrid" /> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class SelectedGridItemChangedEventArgs : EventArgs
{
    /// <summary>
    /// Constructs a SelectedGridItemChangedEventArgs object.
    /// </summary>
    public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
    {
        OldSelection = oldSel;
        NewSelection = newSel;
    }

    /// <summary>
    /// The newly selected GridItem object
    /// </summary>
    public GridItem NewSelection { get; }

    /// <summary>
    /// The previously selected GridItem object. This can be null.
    /// </summary>
    public GridItem OldSelection { get; }
}
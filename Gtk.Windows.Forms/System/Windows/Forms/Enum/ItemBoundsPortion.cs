// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Specifies a portion of the <see cref="T:System.Windows.Forms.ListViewItem" /> from which to retrieve the bounding rectangle.
/// </summary>
public enum ItemBoundsPortion
{
    /// <summary>The bounding rectangle of the entire item, including the icon, the item text, and the subitem text (in details view), should be retrieved.</summary>
    Entire = 0,

    /// <summary>The bounding rectangle of the icon or small icon should be retrieved.</summary>
    Icon = 1,

    /// <summary>The bounding rectangle of the item text should be retrieved.</summary>
    Label = 2,

    /// <summary>The bounding rectangle of the icon or small icon and the item text should be retrieved.</summary>
    ItemOnly = 3,
}

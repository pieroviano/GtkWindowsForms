// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Contains information about an area of a <see cref="T:System.Windows.Forms.ListView" /> control or a
/// <see cref="T:System.Windows.Forms.ListViewItem" />, returned by the <c>HitTest</c> methods.
/// </summary>
public class ListViewHitTestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.ListViewHitTestInfo" /> class.
    /// </summary>
    /// <param name="hitItem">The <see cref="T:System.Windows.Forms.ListViewItem" /> located at the hit-tested point.</param>
    /// <param name="hitSubItem">The <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> located at the hit-tested point.</param>
    /// <param name="hitLocation">One of the <see cref="T:System.Windows.Forms.ListViewHitTestLocations" /> values.</param>
    public ListViewHitTestInfo(ListViewItem? hitItem, ListViewItem.ListViewSubItem? hitSubItem, ListViewHitTestLocations hitLocation)
    {
        Item = hitItem;
        SubItem = hitSubItem;
        Location = hitLocation;
    }

    /// <summary>Gets the location of the hit test on the <see cref="T:System.Windows.Forms.ListView" /> control, in relation to the control and its contents.</summary>
    public ListViewHitTestLocations Location { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ListViewItem" /> located at the hit-tested point.</summary>
    public ListViewItem? Item { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> located at the hit-tested point.</summary>
    public ListViewItem.ListViewSubItem? SubItem { get; }
}

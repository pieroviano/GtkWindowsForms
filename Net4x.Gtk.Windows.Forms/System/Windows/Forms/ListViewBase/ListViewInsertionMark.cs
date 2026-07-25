// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Used to indicate the expected drop location when an item is dragged within a
/// <see cref="T:System.Windows.Forms.ListView" /> control.</summary>
public sealed class ListViewInsertionMark
{
    internal ListViewInsertionMark() { }

    /// <summary>Gets or sets a value indicating whether the insertion mark appears after the item it references.</summary>
    public bool AppearsAfterItem { get; set; }

    /// <summary>Gets the bounds of the insertion mark.</summary>
    public Rectangle Bounds { get; }

    /// <summary>Gets or sets the color of the insertion mark.</summary>
    public Color Color { get; set; } = SystemColors.Highlight;

    /// <summary>Gets or sets the index of the item next to which the insertion mark appears, or -1 if hidden.</summary>
    public int Index { get; set; } = -1;

    /// <summary>Retrieves the index of the item closest to the specified point.</summary>
    /// <param name="pt">The point to test.</param>
    /// <returns>The nearest item index, or -1.</returns>
    public int NearestIndex(Point pt) => -1;
}

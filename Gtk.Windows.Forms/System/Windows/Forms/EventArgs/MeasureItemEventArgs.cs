// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the MeasureItem event of the
/// <see cref="T:System.Windows.Forms.ListBox" /> ,
/// <see cref="T:System.Windows.Forms.ComboBox" /> ,
/// <see cref="T:System.Windows.Forms.CheckedListBox" /> , and
/// <see cref="!:System.Windows.Forms.MenuItem" /> controls.
/// </summary>
public class MeasureItemEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MeasureItemEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">
    /// The graphics.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    public MeasureItemEventArgs(Graphics graphics, int index) : this(graphics, index, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.MeasureItemEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">
    /// The graphics.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="itemHeight">
    /// Height of the item.
    /// </param>
    public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight)
    {
        Graphics = graphics;
        Index = index;
        ItemHeight = itemHeight;
        ItemWidth = 0;
    }

    /// <summary>
    /// A Graphics object to measure relative to.
    /// </summary>
    /// <value>
    /// The graphics.
    /// </value>
    public Graphics Graphics { get; }

    /// <summary>
    /// The index of item for which the height/width is needed.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index { get; }

    /// <summary>
    /// Where the recipient of the event should put the height of the item specified by
    /// the index.
    /// </summary>
    /// <value>
    /// The height of the item.
    /// </value>
    public int ItemHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Where the recipient of the event should put the width of the item specified by
    /// the index.
    /// </summary>
    /// <value>
    /// The width of the item.
    /// </value>
    public int ItemWidth
    {
        get;
        set;
    }
}
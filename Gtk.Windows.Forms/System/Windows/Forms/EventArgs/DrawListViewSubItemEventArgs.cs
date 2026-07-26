// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the <see cref="!:ListView.DrawSubItem" /> event.
/// </summary>
public class DrawListViewSubItemEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.DrawListViewSubItemEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> used to draw the subitem.</param>
    /// <param name="bounds">The bounds of the subitem to draw.</param>
    /// <param name="item">The parent <see cref="T:System.Windows.Forms.ListViewItem" /> of the subitem.</param>
    /// <param name="subItem">The <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> to draw.</param>
    /// <param name="itemIndex">The index of the parent item.</param>
    /// <param name="columnIndex">The index of the column containing the subitem.</param>
    /// <param name="header">The <see cref="T:System.Windows.Forms.ColumnHeader" /> of the column containing the subitem.</param>
    /// <param name="itemState">The current state of the parent item.</param>
    public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item, ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, ColumnHeader header, ListViewItemStates itemState)
    {
        Graphics = graphics;
        Bounds = bounds;
        Item = item;
        SubItem = subItem;
        ItemIndex = itemIndex;
        ColumnIndex = columnIndex;
        Header = header;
        ItemState = itemState;
    }

    /// <summary>Gets or sets a value indicating whether the system draws the subitem rather than the owner.</summary>
    public bool DrawDefault { get; set; }

    /// <summary>Gets the <see cref="T:System.Drawing.Graphics" /> used to draw the subitem.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the size and location of the subitem to draw.</summary>
    public Rectangle Bounds { get; }

    /// <summary>Gets the parent <see cref="T:System.Windows.Forms.ListViewItem" /> of the subitem to draw.</summary>
    public ListViewItem Item { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> to draw.</summary>
    public ListViewItem.ListViewSubItem SubItem { get; }

    /// <summary>Gets the index of the parent <see cref="T:System.Windows.Forms.ListViewItem" /> of the subitem.</summary>
    public int ItemIndex { get; }

    /// <summary>Gets the index of the column that contains the subitem.</summary>
    public int ColumnIndex { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ColumnHeader" /> that represents the column containing the subitem.</summary>
    public ColumnHeader Header { get; }

    /// <summary>Gets the current state of the parent <see cref="T:System.Windows.Forms.ListViewItem" /> of the subitem.</summary>
    public ListViewItemStates ItemState { get; }

    /// <summary>Draws the background of the subitem using its current background color.</summary>
    public void DrawBackground()
    {
        using var brush = new SolidBrush(SystemColors.Window);
        Graphics.FillRectangle(brush, Bounds);
    }

    /// <summary>Draws a focus rectangle within the specified bounds.</summary>
    /// <param name="bounds">The bounds within which to draw the focus rectangle.</param>
    public void DrawFocusRectangle(Rectangle bounds)
    {
        if ((ItemState & ListViewItemStates.Focused) != 0)
        {
            ControlPaint.DrawFocusRectangle(Graphics, bounds);
        }
    }

    /// <summary>Draws the text of the subitem using its current font and colors.</summary>
    public void DrawText() => DrawText(TextFormatFlags.Default);

    /// <summary>Draws the text of the subitem using its current font and colors and the specified formatting.</summary>
    /// <param name="flags">A bitwise combination of <see cref="T:System.Windows.Forms.TextFormatFlags" /> values.</param>
    public void DrawText(TextFormatFlags flags)
    {
        TextRenderer.DrawText(Graphics, SubItem?.Text, SystemFonts.DefaultFont, Bounds, SystemColors.WindowText, flags);
    }
}

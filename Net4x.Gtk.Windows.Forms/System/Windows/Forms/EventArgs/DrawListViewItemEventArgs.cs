// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the <see cref="!:ListView.DrawItem" /> event.
/// </summary>
public class DrawListViewItemEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.DrawListViewItemEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> used to draw the item.</param>
    /// <param name="item">The <see cref="T:System.Windows.Forms.ListViewItem" /> to draw.</param>
    /// <param name="bounds">The bounds of the item to draw.</param>
    /// <param name="itemIndex">The index of the item to draw.</param>
    /// <param name="state">The current state of the item to draw.</param>
    public DrawListViewItemEventArgs(Graphics graphics, ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
    {
        Graphics = graphics;
        Item = item;
        Bounds = bounds;
        ItemIndex = itemIndex;
        State = state;
    }

    /// <summary>Gets or sets a value indicating whether the system draws the item rather than the owner.</summary>
    public bool DrawDefault { get; set; }

    /// <summary>Gets the <see cref="T:System.Drawing.Graphics" /> used to draw the item.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ListViewItem" /> to draw.</summary>
    public ListViewItem Item { get; }

    /// <summary>Gets the size and location of the item to draw.</summary>
    public Rectangle Bounds { get; }

    /// <summary>Gets the index of the item to draw.</summary>
    public int ItemIndex { get; }

    /// <summary>Gets the current state of the item to draw.</summary>
    public ListViewItemStates State { get; }

    /// <summary>Draws the background of the item using its current background color.</summary>
    public void DrawBackground()
    {
        using var brush = new SolidBrush(SystemColors.Window);
        Graphics.FillRectangle(brush, Bounds);
    }

    /// <summary>Draws a focus rectangle around the item if it currently has focus.</summary>
    public void DrawFocusRectangle()
    {
        if ((State & ListViewItemStates.Focused) != 0)
        {
            ControlPaint.DrawFocusRectangle(Graphics, Bounds);
        }
    }

    /// <summary>Draws the text of the item using its current font and colors.</summary>
    public void DrawText() => DrawText(TextFormatFlags.Default);

    /// <summary>Draws the text of the item using its current font and colors and the specified formatting.</summary>
    /// <param name="flags">A bitwise combination of <see cref="T:System.Windows.Forms.TextFormatFlags" /> values.</param>
    public void DrawText(TextFormatFlags flags)
    {
        TextRenderer.DrawText(Graphics, Item?.Text, SystemFonts.DefaultFont, Bounds, SystemColors.WindowText, flags);
    }
}

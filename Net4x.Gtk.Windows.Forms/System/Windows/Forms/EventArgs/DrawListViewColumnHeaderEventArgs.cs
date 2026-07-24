// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the <see cref="!:ListView.DrawColumnHeader" /> event.
/// </summary>
public class DrawListViewColumnHeaderEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.DrawListViewColumnHeaderEventArgs" /> class.
    /// </summary>
    /// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> used to draw the column header.</param>
    /// <param name="bounds">The bounds of the column header to draw.</param>
    /// <param name="columnIndex">The index of the column header to draw.</param>
    /// <param name="header">The <see cref="T:System.Windows.Forms.ColumnHeader" /> to draw.</param>
    /// <param name="state">The current state of the column header to draw.</param>
    /// <param name="foreColor">The foreground color of the column header.</param>
    /// <param name="backColor">The background color of the column header.</param>
    /// <param name="font">The font used to draw the column header text.</param>
    public DrawListViewColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, int columnIndex, ColumnHeader header, ListViewItemStates state, Color foreColor, Color backColor, Font font)
    {
        Graphics = graphics;
        Bounds = bounds;
        ColumnIndex = columnIndex;
        Header = header;
        State = state;
        ForeColor = foreColor;
        BackColor = backColor;
        Font = font;
    }

    /// <summary>Gets or sets a value indicating whether the system draws the column header rather than the owner.</summary>
    public bool DrawDefault { get; set; }

    /// <summary>Gets the <see cref="T:System.Drawing.Graphics" /> used to draw the column header.</summary>
    public Graphics Graphics { get; }

    /// <summary>Gets the size and location of the column header to draw.</summary>
    public Rectangle Bounds { get; }

    /// <summary>Gets the index of the column header to draw.</summary>
    public int ColumnIndex { get; }

    /// <summary>Gets the <see cref="T:System.Windows.Forms.ColumnHeader" /> to draw.</summary>
    public ColumnHeader Header { get; }

    /// <summary>Gets the current state of the column header to draw.</summary>
    public ListViewItemStates State { get; }

    /// <summary>Gets the foreground color of the column header.</summary>
    public Color ForeColor { get; }

    /// <summary>Gets the background color of the column header.</summary>
    public Color BackColor { get; }

    /// <summary>Gets the font applied to the text of the column header.</summary>
    public Font Font { get; }

    /// <summary>Draws the background of the column header using its current background color.</summary>
    public void DrawBackground()
    {
        using var brush = new SolidBrush(BackColor.IsEmpty ? SystemColors.Control : BackColor);
        Graphics.FillRectangle(brush, Bounds);
    }

    /// <summary>Draws the text of the column header using its current font and colors.</summary>
    public void DrawText() => DrawText(TextFormatFlags.Default);

    /// <summary>Draws the text of the column header using its current font and colors and the specified formatting.</summary>
    /// <param name="flags">A bitwise combination of <see cref="T:System.Windows.Forms.TextFormatFlags" /> values.</param>
    public void DrawText(TextFormatFlags flags)
    {
        TextRenderer.DrawText(Graphics, Header?.Text, Font ?? SystemFonts.DefaultFont, Bounds,
            ForeColor.IsEmpty ? SystemColors.ControlText : ForeColor, flags);
    }
}

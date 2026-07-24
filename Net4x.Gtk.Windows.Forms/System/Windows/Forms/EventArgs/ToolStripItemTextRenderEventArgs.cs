// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the text rendering event on a tool strip item.</summary>
public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs
{
    /// <summary>Initializes a new instance with the specified text format flags.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="item">The item to paint.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="textRectangle">The bounds of the text.</param>
    /// <param name="textColor">The color of the text.</param>
    /// <param name="textFont">The font of the text.</param>
    /// <param name="format">The formatting of the text.</param>
    public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
        : base(g, item)
    {
        Text = text;
        TextRectangle = textRectangle;
        TextColor = textColor;
        TextFont = textFont;
        TextFormat = format;
    }

    /// <summary>Initializes a new instance with the specified content alignment.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="item">The item to paint.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="textRectangle">The bounds of the text.</param>
    /// <param name="textColor">The color of the text.</param>
    /// <param name="textFont">The font of the text.</param>
    /// <param name="textAlign">The alignment of the text.</param>
    public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign)
        : base(g, item)
    {
        Text = text;
        TextRectangle = textRectangle;
        TextColor = textColor;
        TextFont = textFont;
        TextFormat = TextFormatFlags.Default;
    }

    /// <summary>Gets or sets the text to draw.</summary>
    public string Text { get; set; }

    /// <summary>Gets or sets the color of the text.</summary>
    public Color TextColor { get; set; }

    /// <summary>Gets or sets the font of the text.</summary>
    public Font TextFont { get; set; }

    /// <summary>Gets or sets the bounds of the text.</summary>
    public Rectangle TextRectangle { get; set; }

    /// <summary>Gets or sets the formatting of the text.</summary>
    public TextFormatFlags TextFormat { get; set; }

    /// <summary>Gets or sets the direction of the text.</summary>
    public ToolStripTextDirection TextDirection { get; set; } = ToolStripTextDirection.Horizontal;
}

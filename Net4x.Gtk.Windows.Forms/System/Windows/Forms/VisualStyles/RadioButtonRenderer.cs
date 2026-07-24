// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render an option (radio) button control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class RadioButtonRenderer
{
    /// <summary>Gets or sets a value indicating whether the renderer follows the application render mode.</summary>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    /// <summary>Indicates whether the background is semitransparent. Returns <see langword="false" />.</summary>
    /// <param name="state">The visual state.</param>
    /// <returns><see langword="false" />.</returns>
    public static bool IsBackgroundPartiallyTransparent(RadioButtonState state) => false;

    /// <summary>Draws the background of a control's parent.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="bounds">The area to draw.</param>
    /// <param name="childControl">The control whose parent background is drawn.</param>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
    {
        using var brush = new SolidBrush(childControl?.BackColor ?? SystemColors.Control);
        g.FillRectangle(brush, bounds);
    }

    /// <summary>Gets the size of the glyph.</summary>
    /// <param name="g">The graphics used to measure.</param>
    /// <param name="state">The visual state.</param>
    /// <returns>The glyph size.</returns>
    public static Size GetGlyphSize(Graphics g, RadioButtonState state) => new Size(13, 13);

    /// <summary>Draws a DrawRadioButton glyph.</summary>
    public static void DrawRadioButton(Graphics g, Point glyphLocation, RadioButtonState state)
    {
        using var pen = new Pen(SystemColors.ControlDarkDark);
        g.DrawEllipse(pen, new Rectangle(glyphLocation, GetGlyphSize(g, state)));
    }

    /// <summary>Draws a DrawRadioButton glyph with text.</summary>
    public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string text, Font font, bool focused, RadioButtonState state)
        => DrawRadioButton(g, glyphLocation, textBounds, text, font, TextFormatFlags.Default, focused, state);

    /// <summary>Draws a DrawRadioButton glyph with text and format flags.</summary>
    public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string text, Font font, TextFormatFlags flags, bool focused, RadioButtonState state)
    {
        DrawRadioButton(g, glyphLocation, state);
        TextRenderer.DrawText(g, text, font, textBounds, SystemColors.ControlText, flags);
        if (focused) { ControlPaint.DrawFocusRectangle(g, textBounds); }
    }

    /// <summary>Draws a DrawRadioButton glyph with text and an image.</summary>
    public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string text, Font font, Image image, Rectangle imageBounds, bool focused, RadioButtonState state)
        => DrawRadioButton(g, glyphLocation, textBounds, text, font, TextFormatFlags.Default, image, imageBounds, focused, state);

    /// <summary>Draws a DrawRadioButton glyph with text, an image and format flags.</summary>
    public static void DrawRadioButton(Graphics g, Point glyphLocation, Rectangle textBounds, string text, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, RadioButtonState state)
    {
        DrawRadioButton(g, glyphLocation, state);
        if (image != null) { g.DrawImage(image, imageBounds); }
        TextRenderer.DrawText(g, text, font, textBounds, SystemColors.ControlText, flags);
        if (focused) { ControlPaint.DrawFocusRectangle(g, textBounds); }
    }
}

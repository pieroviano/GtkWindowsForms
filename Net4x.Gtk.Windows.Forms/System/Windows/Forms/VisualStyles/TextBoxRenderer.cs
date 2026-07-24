// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a text box control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class TextBoxRenderer
{
    /// <summary>Gets a value indicating whether the renderer is supported. Always <see langword="true" />.</summary>
    public static bool IsSupported => true;

    /// <summary>Draws a box using the specified state.</summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, TextBoxState state)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.Sunken);

    /// <summary>Draws a box with text.</summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string text, Font font, TextBoxState state)
    {
        DrawTextBox(g, bounds, state);
        TextRenderer.DrawText(g, text, font, bounds, SystemColors.ControlText, TextFormatFlags.Default);
    }

    /// <summary>Draws a box with text within a text bounds.</summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string text, Font font, Rectangle textBounds, TextBoxState state)
    {
        DrawTextBox(g, bounds, state);
        TextRenderer.DrawText(g, text, font, textBounds, SystemColors.ControlText, TextFormatFlags.Default);
    }

    /// <summary>Draws a box with text and format flags.</summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string text, Font font, TextFormatFlags flags, TextBoxState state)
    {
        DrawTextBox(g, bounds, state);
        TextRenderer.DrawText(g, text, font, bounds, SystemColors.ControlText, flags);
    }

    /// <summary>Draws a box with text, a text bounds and format flags.</summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string text, Font font, Rectangle textBounds, TextFormatFlags flags, TextBoxState state)
    {
        DrawTextBox(g, bounds, state);
        TextRenderer.DrawText(g, text, font, textBounds, SystemColors.ControlText, flags);
    }
}

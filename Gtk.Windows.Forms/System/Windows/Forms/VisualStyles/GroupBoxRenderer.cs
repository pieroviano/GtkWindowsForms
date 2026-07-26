// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a group box control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class GroupBoxRenderer
{
    /// <summary>Gets or sets a value indicating whether the renderer follows the application render mode.</summary>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    /// <summary>Indicates whether the background is semitransparent. Returns <see langword="false" />.</summary>
    /// <param name="state">The visual state.</param>
    /// <returns><see langword="false" />.</returns>
    public static bool IsBackgroundPartiallyTransparent(GroupBoxState state) => false;

    /// <summary>Draws the background of a control's parent.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="bounds">The area to draw.</param>
    /// <param name="childControl">The control whose parent background is drawn.</param>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
    {
        using var brush = new SolidBrush(childControl?.BackColor ?? SystemColors.Control);
        g.FillRectangle(brush, bounds);
    }

    /// <summary>Draws a group box.</summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, GroupBoxState state)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.Etched);

    /// <summary>Draws a group box with text.</summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, GroupBoxState state)
        => DrawGroupBox(g, bounds, groupBoxText, font, SystemColors.ControlText, TextFormatFlags.Default, state);

    /// <summary>Draws a group box with colored text.</summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, GroupBoxState state)
        => DrawGroupBox(g, bounds, groupBoxText, font, textColor, TextFormatFlags.Default, state);

    /// <summary>Draws a group box with text and format flags.</summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, TextFormatFlags flags, GroupBoxState state)
        => DrawGroupBox(g, bounds, groupBoxText, font, SystemColors.ControlText, flags, state);

    /// <summary>Draws a group box with colored text and format flags.</summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, string groupBoxText, Font font, Color textColor, TextFormatFlags flags, GroupBoxState state)
    {
        DrawGroupBox(g, bounds, state);
        TextRenderer.DrawText(g, groupBoxText, font, bounds, textColor, flags);
    }
}

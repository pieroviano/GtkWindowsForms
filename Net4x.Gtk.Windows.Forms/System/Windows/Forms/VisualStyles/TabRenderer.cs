// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a tab control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class TabRenderer
{
    /// <summary>Gets a value indicating whether the renderer is supported. Always <see langword="true" />.</summary>
    public static bool IsSupported => true;

    /// <summary>Draws a tab item.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, TabItemState state)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.Raised);

    /// <summary>Draws a tab item, optionally focused.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, state);
        if (focused) { ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(bounds, -3, -3)); }
    }

    /// <summary>Draws a tab item with text.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TabItemState state)
        => DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.Default, false, state);

    /// <summary>Draws a tab item with text, optionally focused.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, bool focused, TabItemState state)
        => DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.Default, focused, state);

    /// <summary>Draws a tab item with text, format flags and focus.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TextFormatFlags flags, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, focused, state);
        TextRenderer.DrawText(g, tabItemText, font, bounds, SystemColors.ControlText, flags);
    }

    /// <summary>Draws a tab item with an image.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, focused, state);
        if (image != null) { g.DrawImage(image, imageRectangle); }
    }

    /// <summary>Draws a tab item with text and an image.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
        => DrawTabItem(g, bounds, tabItemText, font, TextFormatFlags.Default, image, imageRectangle, focused, state);

    /// <summary>Draws a tab item with text, an image and format flags.</summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string tabItemText, Font font, TextFormatFlags flags, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, focused, state);
        if (image != null) { g.DrawImage(image, imageRectangle); }
        TextRenderer.DrawText(g, tabItemText, font, bounds, SystemColors.ControlText, flags);
    }

    /// <summary>Draws a tab page.</summary>
    public static void DrawTabPage(Graphics g, Rectangle bounds)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.Raised);
}

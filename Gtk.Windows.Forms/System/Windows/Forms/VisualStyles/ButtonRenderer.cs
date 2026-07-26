// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a button control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class ButtonRenderer
{
    /// <summary>Gets or sets a value indicating whether the renderer follows the application render mode.</summary>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    /// <summary>Indicates whether the background of the button has semitransparent or alpha-blended pieces.</summary>
    /// <param name="state">The visual state of the button.</param>
    /// <returns><see langword="false" />.</returns>
    public static bool IsBackgroundPartiallyTransparent(PushButtonState state) => false;

    /// <summary>Draws the background of a control's parent in the specified area.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="bounds">The area to draw.</param>
    /// <param name="childControl">The control whose parent background is drawn.</param>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
    {
        using var brush = new SolidBrush(childControl?.BackColor ?? SystemColors.Control);
        g.FillRectangle(brush, bounds);
    }

    private static ButtonState Map(PushButtonState state) => state switch
    {
        PushButtonState.Pressed => ButtonState.Pushed,
        PushButtonState.Disabled => ButtonState.Inactive,
        _ => ButtonState.Normal,
    };

    /// <summary>Draws a button control.</summary>
    /// <param name="g">The graphics used to draw.</param>
    /// <param name="bounds">The bounds of the button.</param>
    /// <param name="state">The visual state of the button.</param>
    public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a button control, optionally with a focus rectangle.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, state);
        if (focused) { ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(bounds, -3, -3)); }
    }

    /// <summary>Draws a button control with text.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, bool focused, PushButtonState state)
        => DrawButton(g, bounds, buttonText, font, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, focused, state);

    /// <summary>Draws a button control with text and format flags.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, focused, state);
        TextRenderer.DrawText(g, buttonText, font, bounds, SystemColors.ControlText, flags);
    }

    /// <summary>Draws a button control with an image.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, focused, state);
        if (image != null) { g.DrawImage(image, imageBounds); }
    }

    /// <summary>Draws a button control with text and an image.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
        => DrawButton(g, bounds, buttonText, font, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, image, imageBounds, focused, state);

    /// <summary>Draws a button control with text, an image and format flags.</summary>
    public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, focused, state);
        if (image != null) { g.DrawImage(image, imageBounds); }
        TextRenderer.DrawText(g, buttonText, font, bounds, SystemColors.ControlText, flags);
    }
}

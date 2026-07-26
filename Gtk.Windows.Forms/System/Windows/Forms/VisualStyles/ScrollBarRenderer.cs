// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a scroll bar control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class ScrollBarRenderer
{
    /// <summary>Gets a value indicating whether the renderer is supported. Always <see langword="true" />.</summary>
    public static bool IsSupported => true;

    private static ButtonState Map(ScrollBarState state)
        => state == ScrollBarState.Pressed ? ButtonState.Pushed
         : state == ScrollBarState.Disabled ? ButtonState.Inactive : ButtonState.Normal;

    /// <summary>Draws a scroll arrow button.</summary>
    public static void DrawArrowButton(Graphics g, Rectangle bounds, ScrollBarArrowButtonState state)
    {
        var n = state.ToString();
        var pushed = n.EndsWith("Pressed") ? ButtonState.Pushed
                   : n.EndsWith("Disabled") ? ButtonState.Inactive : ButtonState.Normal;
        ControlPaint.DrawButton(g, bounds, pushed);
    }

    /// <summary>Draws a horizontal scroll thumb.</summary>
    public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a vertical scroll thumb.</summary>
    public static void DrawVerticalThumb(Graphics g, Rectangle bounds, ScrollBarState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws the grip of a horizontal scroll thumb.</summary>
    public static void DrawHorizontalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state) { }

    /// <summary>Draws the grip of a vertical scroll thumb.</summary>
    public static void DrawVerticalThumbGrip(Graphics g, Rectangle bounds, ScrollBarState state) { }

    private static void Track(Graphics g, Rectangle bounds)
    {
        using var brush = new SolidBrush(SystemColors.ScrollBar);
        g.FillRectangle(brush, bounds);
    }

    /// <summary>Draws the right part of a horizontal scroll track.</summary>
    public static void DrawRightHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state) => Track(g, bounds);

    /// <summary>Draws the left part of a horizontal scroll track.</summary>
    public static void DrawLeftHorizontalTrack(Graphics g, Rectangle bounds, ScrollBarState state) => Track(g, bounds);

    /// <summary>Draws the upper part of a vertical scroll track.</summary>
    public static void DrawUpperVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state) => Track(g, bounds);

    /// <summary>Draws the lower part of a vertical scroll track.</summary>
    public static void DrawLowerVerticalTrack(Graphics g, Rectangle bounds, ScrollBarState state) => Track(g, bounds);

    /// <summary>Draws a scroll bar size box.</summary>
    public static void DrawSizeBox(Graphics g, Rectangle bounds, ScrollBarSizeBoxState state)
        => ControlPaint.DrawButton(g, bounds, ButtonState.Normal);

    /// <summary>Gets the size of a scroll thumb grip.</summary>
    public static Size GetThumbGripSize(Graphics g, ScrollBarState state) => new Size(16, 16);

    /// <summary>Gets the size of a scroll size box.</summary>
    public static Size GetSizeBoxSize(Graphics g, ScrollBarState state) => new Size(17, 17);
}

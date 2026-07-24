// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a track bar control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class TrackBarRenderer
{
    /// <summary>Gets a value indicating whether the renderer is supported. Always <see langword="true" />.</summary>
    public static bool IsSupported => true;

    private static ButtonState Map(TrackBarThumbState state)
        => state == TrackBarThumbState.Pressed ? ButtonState.Pushed
         : state == TrackBarThumbState.Disabled ? ButtonState.Inactive : ButtonState.Normal;

    /// <summary>Draws a horizontal track.</summary>
    public static void DrawHorizontalTrack(Graphics g, Rectangle bounds)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.SunkenOuter);

    /// <summary>Draws a vertical track.</summary>
    public static void DrawVerticalTrack(Graphics g, Rectangle bounds)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.SunkenOuter);

    /// <summary>Draws a horizontal thumb.</summary>
    public static void DrawHorizontalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a vertical thumb.</summary>
    public static void DrawVerticalThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a left-pointing thumb.</summary>
    public static void DrawLeftPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a right-pointing thumb.</summary>
    public static void DrawRightPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a top-pointing thumb.</summary>
    public static void DrawTopPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws a bottom-pointing thumb.</summary>
    public static void DrawBottomPointingThumb(Graphics g, Rectangle bounds, TrackBarThumbState state)
        => ControlPaint.DrawButton(g, bounds, Map(state));

    /// <summary>Draws horizontal ticks.</summary>
    public static void DrawHorizontalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle)
        => DrawTicks(g, bounds, numTicks, true);

    /// <summary>Draws vertical ticks.</summary>
    public static void DrawVerticalTicks(Graphics g, Rectangle bounds, int numTicks, EdgeStyle edgeStyle)
        => DrawTicks(g, bounds, numTicks, false);

    private static void DrawTicks(Graphics g, Rectangle bounds, int numTicks, bool horizontal)
    {
        if (numTicks <= 1) { return; }
        using var pen = new Pen(SystemColors.ControlDark);
        for (var i = 0; i < numTicks; i++)
        {
            if (horizontal)
            {
                var x = bounds.Left + (bounds.Width * i / (numTicks - 1));
                g.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
            }
            else
            {
                var y = bounds.Top + (bounds.Height * i / (numTicks - 1));
                g.DrawLine(pen, bounds.Left, y, bounds.Right, y);
            }
        }
    }

    /// <summary>Gets the size of a left-pointing thumb.</summary>
    public static Size GetLeftPointingThumbSize(Graphics g, TrackBarThumbState state) => new Size(11, 21);

    /// <summary>Gets the size of a right-pointing thumb.</summary>
    public static Size GetRightPointingThumbSize(Graphics g, TrackBarThumbState state) => new Size(11, 21);

    /// <summary>Gets the size of a top-pointing thumb.</summary>
    public static Size GetTopPointingThumbSize(Graphics g, TrackBarThumbState state) => new Size(21, 11);

    /// <summary>Gets the size of a bottom-pointing thumb.</summary>
    public static Size GetBottomPointingThumbSize(Graphics g, TrackBarThumbState state) => new Size(21, 11);
}

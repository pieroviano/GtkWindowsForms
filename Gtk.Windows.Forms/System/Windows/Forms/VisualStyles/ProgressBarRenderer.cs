// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>Provides methods used to render a progress bar control with visual styles. Rendering falls back to classic (non-themed) drawing; GTK draws real chrome for live controls.</summary>
public static class ProgressBarRenderer
{
    /// <summary>Gets a value indicating whether the renderer is supported. Always <see langword="true" />.</summary>
    public static bool IsSupported => true;

    /// <summary>Gets the thickness, in pixels, of a single progress bar chunk.</summary>
    public static int ChunkThickness => 6;

    /// <summary>Gets the thickness, in pixels, of the space between progress bar chunks.</summary>
    public static int ChunkSpaceThickness => 2;

    /// <summary>Draws the frame of a horizontal progress bar.</summary>
    public static void DrawHorizontalBar(Graphics g, Rectangle bounds)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.SunkenOuter);

    /// <summary>Draws the frame of a vertical progress bar.</summary>
    public static void DrawVerticalBar(Graphics g, Rectangle bounds)
        => ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.SunkenOuter);

    /// <summary>Draws the filled chunks of a horizontal progress bar.</summary>
    public static void DrawHorizontalChunks(Graphics g, Rectangle bounds)
    {
        using var brush = new SolidBrush(SystemColors.Highlight);
        g.FillRectangle(brush, bounds);
    }

    /// <summary>Draws the filled chunks of a vertical progress bar.</summary>
    public static void DrawVerticalChunks(Graphics g, Rectangle bounds)
    {
        using var brush = new SolidBrush(SystemColors.Highlight);
        g.FillRectangle(brush, bounds);
    }
}

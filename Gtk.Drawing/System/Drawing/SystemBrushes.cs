// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
/// Each property of the <see cref="T:System.Drawing.SystemBrushes" /> class is a
/// <see cref="T:System.Drawing.SolidBrush" /> in a colour from the Windows display palette.
/// </summary>
/// <remarks>
/// This type was missing entirely while <see cref="T:System.Drawing.SystemColors" /> and
/// <see cref="T:System.Drawing.SystemPens" /> were present, so owner-draw code that filled with a system
/// brush did not compile.
/// </remarks>
public static class SystemBrushes
{
    /// <summary>Gets a brush in the colour of the active window border.</summary>
    public static Brush ActiveBorder => FromSystemColor(SystemColors.ActiveBorder);

    /// <summary>Gets a brush in the background colour of the active window title bar.</summary>
    public static Brush ActiveCaption => FromSystemColor(SystemColors.ActiveCaption);

    /// <summary>Gets a brush in the colour of the text of the active window title bar.</summary>
    public static Brush ActiveCaptionText => FromSystemColor(SystemColors.ActiveCaptionText);

    /// <summary>Gets a brush in the colour of the application workspace.</summary>
    public static Brush AppWorkspace => FromSystemColor(SystemColors.AppWorkspace);

    /// <summary>Gets a brush in the face colour of a three-dimensional element.</summary>
    public static Brush ButtonFace => FromSystemColor(SystemColors.ButtonFace);

    /// <summary>Gets a brush in the highlight colour of a three-dimensional element.</summary>
    public static Brush ButtonHighlight => FromSystemColor(SystemColors.ButtonHighlight);

    /// <summary>Gets a brush in the shadow colour of a three-dimensional element.</summary>
    public static Brush ButtonShadow => FromSystemColor(SystemColors.ButtonShadow);

    /// <summary>Gets a brush in the face colour of a control.</summary>
    public static Brush Control => FromSystemColor(SystemColors.Control);

    /// <summary>Gets a brush in the shadow colour of a control.</summary>
    public static Brush ControlDark => FromSystemColor(SystemColors.ControlDark);

    /// <summary>Gets a brush in the dark shadow colour of a control.</summary>
    public static Brush ControlDarkDark => FromSystemColor(SystemColors.ControlDarkDark);

    /// <summary>Gets a brush in the light colour of a control.</summary>
    public static Brush ControlLight => FromSystemColor(SystemColors.ControlLight);

    /// <summary>Gets a brush in the highlight colour of a control.</summary>
    public static Brush ControlLightLight => FromSystemColor(SystemColors.ControlLightLight);

    /// <summary>Gets a brush in the colour of text in a control.</summary>
    public static Brush ControlText => FromSystemColor(SystemColors.ControlText);

    /// <summary>Gets a brush in the colour of the desktop.</summary>
    public static Brush Desktop => FromSystemColor(SystemColors.Desktop);

    /// <summary>Gets a brush in the colour of dimmed text.</summary>
    public static Brush GrayText => FromSystemColor(SystemColors.GrayText);

    /// <summary>Gets a brush in the background colour of selected items.</summary>
    public static Brush Highlight => FromSystemColor(SystemColors.Highlight);

    /// <summary>Gets a brush in the colour of the text of selected items.</summary>
    public static Brush HighlightText => FromSystemColor(SystemColors.HighlightText);

    /// <summary>Gets a brush in the colour used to designate a hot-tracked item.</summary>
    public static Brush HotTrack => FromSystemColor(SystemColors.HotTrack);

    /// <summary>Gets a brush in the colour of an inactive window border.</summary>
    public static Brush InactiveBorder => FromSystemColor(SystemColors.InactiveBorder);

    /// <summary>Gets a brush in the background colour of an inactive window title bar.</summary>
    public static Brush InactiveCaption => FromSystemColor(SystemColors.InactiveCaption);

    /// <summary>Gets a brush in the colour of the text of an inactive window title bar.</summary>
    public static Brush InactiveCaptionText => FromSystemColor(SystemColors.InactiveCaptionText);

    /// <summary>Gets a brush in the background colour of a ToolTip.</summary>
    public static Brush Info => FromSystemColor(SystemColors.Info);

    /// <summary>Gets a brush in the colour of the text of a ToolTip.</summary>
    public static Brush InfoText => FromSystemColor(SystemColors.InfoText);

    /// <summary>Gets a brush in the background colour of a menu.</summary>
    public static Brush Menu => FromSystemColor(SystemColors.Menu);

    /// <summary>Gets a brush in the colour of menu text.</summary>
    public static Brush MenuText => FromSystemColor(SystemColors.MenuText);

    /// <summary>Gets a brush in the background colour of a scroll bar.</summary>
    public static Brush ScrollBar => FromSystemColor(SystemColors.ScrollBar);

    /// <summary>Gets a brush in the background colour of a window.</summary>
    public static Brush Window => FromSystemColor(SystemColors.Window);

    /// <summary>Gets a brush in the colour of a window frame.</summary>
    public static Brush WindowFrame => FromSystemColor(SystemColors.WindowFrame);

    /// <summary>Gets a brush in the colour of the text of a window.</summary>
    public static Brush WindowText => FromSystemColor(SystemColors.WindowText);

    /// <summary>
    /// Creates a <see cref="T:System.Drawing.Brush" /> from the specified system colour.
    /// </summary>
    /// <param name="c">A system colour.</param>
    /// <returns>A brush in that colour.</returns>
    public static Brush FromSystemColor(Color c)
    {
        return new SolidBrush(c);
    }
}

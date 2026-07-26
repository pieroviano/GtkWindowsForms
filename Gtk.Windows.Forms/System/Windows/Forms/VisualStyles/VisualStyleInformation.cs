// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.VisualStyles;

/// <summary>
/// Provides information about the current visual style of the operating system. On this GTK layer the values
/// reflect the active GTK theme, which is always present, so visual styles are reported as supported and
/// enabled.
/// </summary>
public static class VisualStyleInformation
{
    /// <summary>Gets a value indicating whether the operating system supports visual styles. Always <see langword="true" /> (GTK themes).</summary>
    public static bool IsSupportedByOS => true;

    /// <summary>Gets a value indicating whether visual styles are enabled by the user. Always <see langword="true" />.</summary>
    public static bool IsEnabledByUser => true;

    /// <summary>Gets the color scheme of the current visual style.</summary>
    public static string ColorScheme => string.Empty;

    /// <summary>Gets the size of the current visual style.</summary>
    public static string Size => string.Empty;

    /// <summary>Gets the display name of the current visual style.</summary>
    /// <remarks>
    /// Reading the active GTK theme name requires an initialized GTK toolkit; to keep this property safe to
    /// call at any time (including before a window exists), it returns an empty string rather than probing
    /// <c>Gtk.Settings</c>.
    /// </remarks>
    public static string DisplayName => string.Empty;

    /// <summary>Gets the company that created the current visual style.</summary>
    public static string Company => string.Empty;

    /// <summary>Gets the author of the current visual style.</summary>
    public static string Author => string.Empty;

    /// <summary>Gets the copyright information for the current visual style.</summary>
    public static string Copyright => string.Empty;

    /// <summary>Gets the URL supplied by the author of the current visual style.</summary>
    public static string Url => string.Empty;

    /// <summary>Gets the version of the current visual style.</summary>
    public static string Version => string.Empty;

    /// <summary>Gets the description of the current visual style.</summary>
    public static string Description => string.Empty;

    /// <summary>Gets a value indicating whether the current visual style supports flat menus. Always <see langword="true" />.</summary>
    public static bool SupportsFlatMenus => true;

    /// <summary>Gets the minimum color depth supported by the current visual style.</summary>
    public static int MinimumColorDepth => 32;

    /// <summary>Gets the color that the current visual style uses to draw a text-control border.</summary>
    public static Color TextControlBorder => SystemColors.ControlDark;

    /// <summary>Gets the color that the current visual style uses to draw highlighted, hot-tracked controls.</summary>
    public static Color ControlHighlightHot => SystemColors.Highlight;
}

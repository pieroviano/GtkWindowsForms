// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
/// Specifies the fonts used to display text in Windows display elements.
/// </summary>
/// <remarks>
/// This type was missing entirely, so layout code reading a system font did not compile. Each property
/// returns a fresh <see cref="T:System.Drawing.Font" />, as the framework does, so the caller owns it and
/// may dispose it.
/// </remarks>
public static class SystemFonts
{
    /// <summary>The family used for every system font on this platform.</summary>
    private const string DefaultFamily = "Sans";

    /// <summary>The point size of the standard UI fonts.</summary>
    private const float DefaultSize = 9f;

    /// <summary>Gets the font used for captions of active windows.</summary>
    public static Font CaptionFont => new(DefaultFamily, DefaultSize, FontStyle.Bold);

    /// <summary>Gets the default font used for dialog boxes and forms.</summary>
    public static Font DefaultFont => new(DefaultFamily, DefaultSize);

    /// <summary>Gets the font used for dialog box text.</summary>
    public static Font DialogFont => new(DefaultFamily, DefaultSize);

    /// <summary>Gets the font used for icon titles.</summary>
    public static Font IconTitleFont => new(DefaultFamily, DefaultSize);

    /// <summary>Gets the font used for menus.</summary>
    public static Font MenuFont => new(DefaultFamily, DefaultSize);

    /// <summary>Gets the font used for message boxes.</summary>
    public static Font MessageBoxFont => new(DefaultFamily, DefaultSize);

    /// <summary>Gets the font used for captions of small windows, such as tool windows.</summary>
    public static Font SmallCaptionFont => new(DefaultFamily, DefaultSize - 1f, FontStyle.Bold);

    /// <summary>Gets the font used to display text in the status bar.</summary>
    public static Font StatusFont => new(DefaultFamily, DefaultSize);

    /// <summary>
    /// Returns the font associated with the specified Windows system font name.
    /// </summary>
    /// <param name="systemFontName">The name of a system font, such as <c>MenuFont</c>.</param>
    /// <returns>The matching font, or <see langword="null" /> when the name is not recognised.</returns>
    public static Font? GetFontByName(string? systemFontName) => systemFontName switch
    {
        nameof(CaptionFont) => CaptionFont,
        nameof(DefaultFont) => DefaultFont,
        nameof(DialogFont) => DialogFont,
        nameof(IconTitleFont) => IconTitleFont,
        nameof(MenuFont) => MenuFont,
        nameof(MessageBoxFont) => MessageBoxFont,
        nameof(SmallCaptionFont) => SmallCaptionFont,
        nameof(StatusFont) => StatusFont,
        _ => null
    };
}

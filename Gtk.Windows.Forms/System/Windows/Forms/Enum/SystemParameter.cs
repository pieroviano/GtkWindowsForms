// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies system parameters whose presence can be tested through <see cref="T:System.Windows.Forms.OSFeature" />.</summary>
public enum SystemParameter
{
    /// <summary>The drop shadow effect.</summary>
    DropShadow = 0,
    /// <summary>The flat menu appearance.</summary>
    FlatMenu = 1,
    /// <summary>The font-smoothing contrast metric.</summary>
    FontSmoothingContrastMetric = 2,
    /// <summary>The font-smoothing type metric.</summary>
    FontSmoothingTypeMetric = 3,
    /// <summary>Whether menu fade animation is enabled.</summary>
    MenuFadeEnabled = 4,
    /// <summary>The selection fade effect.</summary>
    SelectionFade = 5,
    /// <summary>The tool tip animation metric.</summary>
    ToolTipAnimationMetric = 6,
    /// <summary>Whether UI effects are enabled.</summary>
    UIEffects = 7,
    /// <summary>The caret width metric.</summary>
    CaretWidthMetric = 8,
    /// <summary>The vertical focus thickness metric.</summary>
    VerticalFocusThicknessMetric = 9,
    /// <summary>The horizontal focus thickness metric.</summary>
    HorizontalFocusThicknessMetric = 10,
}

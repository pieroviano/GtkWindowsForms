// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Contains the metrics of a text font drawn with visual styles.</summary>
public struct TextMetrics
{
    /// <summary>Gets or sets the height of the characters.</summary>
    public int Height { get; set; }

    /// <summary>Gets or sets the ascent of the characters.</summary>
    public int Ascent { get; set; }

    /// <summary>Gets or sets the descent of the characters.</summary>
    public int Descent { get; set; }

    /// <summary>Gets or sets the internal leading space.</summary>
    public int InternalLeading { get; set; }

    /// <summary>Gets or sets the external leading space.</summary>
    public int ExternalLeading { get; set; }

    /// <summary>Gets or sets the average character width.</summary>
    public int AverageCharWidth { get; set; }

    /// <summary>Gets or sets the maximum character width.</summary>
    public int MaxCharWidth { get; set; }

    /// <summary>Gets or sets the font weight.</summary>
    public int Weight { get; set; }

    /// <summary>Gets or sets the extra width per string added to characters.</summary>
    public int Overhang { get; set; }

    /// <summary>Gets or sets the horizontal digitized aspect.</summary>
    public int DigitizedAspectX { get; set; }

    /// <summary>Gets or sets the vertical digitized aspect.</summary>
    public int DigitizedAspectY { get; set; }

    /// <summary>Gets or sets the first character defined in the font.</summary>
    public char FirstChar { get; set; }

    /// <summary>Gets or sets the last character defined in the font.</summary>
    public char LastChar { get; set; }

    /// <summary>Gets or sets the default character substituted for characters not in the font.</summary>
    public char DefaultChar { get; set; }

    /// <summary>Gets or sets the character used to define word breaks.</summary>
    public char BreakChar { get; set; }

    /// <summary>Gets or sets a value indicating whether the font is italic.</summary>
    public bool Italic { get; set; }

    /// <summary>Gets or sets a value indicating whether the font is underlined.</summary>
    public bool Underlined { get; set; }

    /// <summary>Gets or sets a value indicating whether the font is struck out.</summary>
    public bool StruckOut { get; set; }

    /// <summary>Gets or sets the pitch and family of the font.</summary>
    public TextMetricsPitchAndFamilyValues PitchAndFamily { get; set; }

    /// <summary>Gets or sets the character set of the font.</summary>
    public TextMetricsCharacterSet CharSet { get; set; }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Identifies a TextMetricsPitchAndFamilyValues used with visual styles.</summary>
[Flags]
public enum TextMetricsPitchAndFamilyValues
{
    /// <summary>The FixedPitch value.</summary>
    FixedPitch = 1,

    /// <summary>The Vector value.</summary>
    Vector = 2,

    /// <summary>The TrueType value.</summary>
    TrueType = 4,

    /// <summary>The Device value.</summary>
    Device = 8,
}

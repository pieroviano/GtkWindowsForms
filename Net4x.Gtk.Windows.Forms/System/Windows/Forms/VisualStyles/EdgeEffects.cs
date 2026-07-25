// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Identifies a EdgeEffects used with visual styles.</summary>
[Flags]
public enum EdgeEffects
{
    /// <summary>The None value.</summary>
    None = 0,

    /// <summary>The FillInterior value.</summary>
    FillInterior = 2048,

    /// <summary>The Flat value.</summary>
    Flat = 4096,

    /// <summary>The Soft value.</summary>
    Soft = 16384,

    /// <summary>The Mono value.</summary>
    Mono = 32768,
}

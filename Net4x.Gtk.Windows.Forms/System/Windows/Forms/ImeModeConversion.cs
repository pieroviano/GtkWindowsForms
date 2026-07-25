// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms;

/// <summary>Provides mappings between IME conversion modes and <see cref="T:System.Windows.Forms.ImeMode" /> values.</summary>
public struct ImeModeConversion
{
    private static readonly Dictionary<ImeMode, ImeModeConversion> conversionTable = new();

    /// <summary>Gets the mapping of IME modes to their conversion bits.</summary>
    public static Dictionary<ImeMode, ImeModeConversion> ImeModeConversionBits => conversionTable;

    /// <summary>Gets a value indicating whether the current keyboard layout supports an IME conversion table.</summary>
    public static bool IsCurrentConversionTableSupported => false;
}

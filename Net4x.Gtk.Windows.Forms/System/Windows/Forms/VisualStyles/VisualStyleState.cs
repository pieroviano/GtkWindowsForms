// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Identifies a VisualStyleState used with visual styles.</summary>
public enum VisualStyleState
{
    /// <summary>The NoneEnabled value.</summary>
    NoneEnabled = 0,

    /// <summary>The ClientAreaEnabled value.</summary>
    ClientAreaEnabled = 2,

    /// <summary>The NonClientAreaEnabled value.</summary>
    NonClientAreaEnabled = 1,

    /// <summary>The ClientAndNonClientAreasEnabled value.</summary>
    ClientAndNonClientAreasEnabled = 3,
}

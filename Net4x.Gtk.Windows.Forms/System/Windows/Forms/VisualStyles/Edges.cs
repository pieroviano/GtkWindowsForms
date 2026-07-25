// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Identifies a Edges used with visual styles.</summary>
[Flags]
public enum Edges
{
    /// <summary>The Left value.</summary>
    Left = 1,

    /// <summary>The Top value.</summary>
    Top = 2,

    /// <summary>The Right value.</summary>
    Right = 4,

    /// <summary>The Bottom value.</summary>
    Bottom = 8,

    /// <summary>The Diagonal value.</summary>
    Diagonal = 16,
}

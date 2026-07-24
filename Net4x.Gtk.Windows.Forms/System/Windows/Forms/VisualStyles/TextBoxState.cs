// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Specifies the visual state of a TextBox drawn with visual styles.</summary>
public enum TextBoxState
{
    /// <summary>The Normal state.</summary>
    Normal = 1,

    /// <summary>The Hot state.</summary>
    Hot = 2,

    /// <summary>The Selected state.</summary>
    Selected = 3,

    /// <summary>The Disabled state.</summary>
    Disabled = 4,

    /// <summary>The Readonly state.</summary>
    Readonly = 6,

    /// <summary>The Assist state.</summary>
    Assist = 7,
}

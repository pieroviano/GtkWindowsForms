// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Specifies the style of an edge drawn with visual styles.</summary>
public enum EdgeStyle
{
    /// <summary>A raised outer edge and a raised inner edge.</summary>
    Raised = 5,

    /// <summary>A sunken outer edge and a sunken inner edge.</summary>
    Sunken = 10,

    /// <summary>A raised outer edge and a sunken inner edge.</summary>
    Etched = 6,

    /// <summary>A sunken outer edge and a raised inner edge.</summary>
    Bump = 9,
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>Identifies a HitTestOptions used with visual styles.</summary>
[Flags]
public enum HitTestOptions
{
    /// <summary>The BackgroundSegment value.</summary>
    BackgroundSegment = 0,

    /// <summary>The FixedBorder value.</summary>
    FixedBorder = 2,

    /// <summary>The Caption value.</summary>
    Caption = 4,

    /// <summary>The ResizingBorderLeft value.</summary>
    ResizingBorderLeft = 16,

    /// <summary>The ResizingBorderTop value.</summary>
    ResizingBorderTop = 32,

    /// <summary>The ResizingBorderRight value.</summary>
    ResizingBorderRight = 64,

    /// <summary>The ResizingBorderBottom value.</summary>
    ResizingBorderBottom = 128,

    /// <summary>The ResizingBorder value.</summary>
    ResizingBorder = 240,

    /// <summary>The SizingTemplate value.</summary>
    SizingTemplate = 256,

    /// <summary>The SystemSizingMargins value.</summary>
    SystemSizingMargins = 512,
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Specifies the type of scroll arrow to draw on a scroll bar.
/// </summary>
public enum ScrollButton
{
    /// <summary>An up-scroll arrow.</summary>
    Up = 0,

    /// <summary>A down-scroll arrow.</summary>
    Down = 1,

    /// <summary>A left-scroll arrow.</summary>
    Left = 2,

    /// <summary>A right-scroll arrow.</summary>
    Right = 3,

    /// <summary>The minimum-scroll arrow (equivalent to <see cref="F:System.Windows.Forms.ScrollButton.Up" />).</summary>
    Min = 0,

    /// <summary>The maximum-scroll arrow (equivalent to <see cref="F:System.Windows.Forms.ScrollButton.Right" />).</summary>
    Max = 3,
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the direction the system uses to arrange minimized windows.</summary>
[Flags]
public enum ArrangeDirection
{
    /// <summary>Arranges horizontally, from left to right.</summary>
    Left = 0,

    /// <summary>Arranges horizontally, from right to left.</summary>
    Right = 0,

    /// <summary>Arranges vertically, from top to bottom.</summary>
    Down = 4,

    /// <summary>Arranges vertically, from bottom to top.</summary>
    Up = 4,
}

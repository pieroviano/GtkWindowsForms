// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies a glyph, such as a check mark, that is drawn on a menu item.</summary>
public enum MenuGlyph
{
    /// <summary>A submenu arrow.</summary>
    Arrow = 0,

    /// <summary>A menu item check mark.</summary>
    Checkmark = 1,

    /// <summary>A menu item bullet.</summary>
    Bullet = 2,

    /// <summary>The minimum glyph value.</summary>
    Min = 0,

    /// <summary>The maximum glyph value.</summary>
    Max = 2,
}

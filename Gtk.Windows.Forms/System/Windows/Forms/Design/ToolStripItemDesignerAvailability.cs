// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design;

/// <summary>Specifies ToolStripItemDesignerAvailability.</summary>
[Flags]
public enum ToolStripItemDesignerAvailability
{
    /// <summary>The None value.</summary>
    None = 0,

    /// <summary>The ToolStrip value.</summary>
    ToolStrip = 1,

    /// <summary>The MenuStrip value.</summary>
    MenuStrip = 2,

    /// <summary>The ContextMenuStrip value.</summary>
    ContextMenuStrip = 4,

    /// <summary>The StatusStrip value.</summary>
    StatusStrip = 8,

    /// <summary>The All value.</summary>
    All = 15,
}

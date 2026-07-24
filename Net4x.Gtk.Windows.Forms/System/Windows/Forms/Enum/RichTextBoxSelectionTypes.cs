// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the type of selection in a <see cref="T:System.Windows.Forms.RichTextBox" />.</summary>
[Flags]
public enum RichTextBoxSelectionTypes
{
    /// <summary>The selection is empty.</summary>
    Empty = 0,

    /// <summary>The selection contains text.</summary>
    Text = 1,

    /// <summary>The selection contains at least one OLE object.</summary>
    Object = 2,

    /// <summary>The selection contains more than one character.</summary>
    MultiChar = 4,

    /// <summary>The selection contains more than one OLE object.</summary>
    MultiObject = 8,
}

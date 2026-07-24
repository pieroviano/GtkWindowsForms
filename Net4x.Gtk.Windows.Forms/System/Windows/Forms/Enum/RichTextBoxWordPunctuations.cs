// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the type of punctuation table to use in a <see cref="T:System.Windows.Forms.RichTextBox" />.</summary>
public enum RichTextBoxWordPunctuations
{
    /// <summary>Uses a level-1 punctuation table for word wrapping in East Asian versions of the control.</summary>
    Level1 = 128,

    /// <summary>Uses a level-2 punctuation table for word wrapping in East Asian versions of the control.</summary>
    Level2 = 256,

    /// <summary>Uses a custom punctuation table.</summary>
    Custom = 512,

    /// <summary>The combination of the punctuation flags.</summary>
    All = 896,
}

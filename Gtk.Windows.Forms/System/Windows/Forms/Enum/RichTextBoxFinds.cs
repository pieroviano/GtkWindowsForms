// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies how a text search is conducted in a <see cref="T:System.Windows.Forms.RichTextBox" />.</summary>
[Flags]
public enum RichTextBoxFinds
{
    /// <summary>Locates all instances of the search text whether or not it is part of a larger word.</summary>
    None = 0,

    /// <summary>Locates only instances of the search text that are whole words.</summary>
    WholeWord = 2,

    /// <summary>Locates instances of the search text matching the specified letter casing.</summary>
    MatchCase = 4,

    /// <summary>Prevents the located search text from being highlighted.</summary>
    NoHighlight = 8,

    /// <summary>Searches from the end of the control to the beginning.</summary>
    Reverse = 16,
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;/// <summary>
/// Represents the mode characters are entered into a <see cref="T:System.Windows.Forms.MaskedTextBox" /> control.
/// </summary>
public enum InsertKeyMode
{
    /// <summary>Uses the current INSERT key mode of the keyboard.</summary>
    Default = 0,

    /// <summary>Uses insert mode.</summary>
    Insert = 1,

    /// <summary>Uses overwrite mode.</summary>
    Overwrite = 2,
}

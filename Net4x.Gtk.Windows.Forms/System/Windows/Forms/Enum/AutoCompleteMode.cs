// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Specifies the mode for the automatic completion feature used in the
/// <see cref="T:System.Windows.Forms.ComboBox" /> and
/// <see cref="T:System.Windows.Forms.TextBox" /> controls.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public enum AutoCompleteMode
{
    /// <summary>
    /// Disables the AutoComplete Feature for ComboBox and TextBox.
    /// </summary>
    None = 0,

    /// <summary>
    /// Displays the auxiliary drop-down list associated with the edit control,
    /// this drop-down is populated with one or more suggested completed strings.
    /// </summary>
    Suggest = 0x1,

    /// <summary>
    /// Appends the remainder of the most likely candidate string to the existing
    /// characters, hightlighting the appended characters.
    /// </summary>
    Append = 0x2,

    /// <summary>
    /// The AutoSuggest and AutoAppend are applied in conjuction.
    /// </summary>
    SuggestAppend = Suggest | Append
}

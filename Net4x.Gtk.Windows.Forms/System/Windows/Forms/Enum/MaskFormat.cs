using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// Defines how to format the text inside of a
/// <see cref="T:System.Windows.Forms.MaskedTextBox" /> .
/// </summary>
public enum MaskFormat
{

    /// <summary>
    /// Return only text input by the user.
    /// </summary>
    ExcludePromptAndLiterals = 0,

    /// <summary>
    /// Return text input by the user as well as any instances of the prompt character.
    /// </summary>
    IncludePrompt = 1,

    /// <summary>
    /// Return text input by the user as well as any literal characters defined in the
    /// mask.
    /// </summary>
    IncludeLiterals = 2,

    /// <summary>
    /// Return text input by the user as well as any literal characters defined in the
    /// mask and any instances of the prompt character.
    /// </summary>
    IncludePromptAndLiterals = 3
}
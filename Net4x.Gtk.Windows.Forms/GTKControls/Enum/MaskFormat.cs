using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum MaskFormat
    {
        //
        // Summary:
        //     Return only text input by the user.
        ExcludePromptAndLiterals = 0,
        //
        // Summary:
        //     Return text input by the user as well as any instances of the prompt character.
        IncludePrompt = 1,
        //
        // Summary:
        //     Return text input by the user as well as any literal characters defined in the
        //     mask.
        IncludeLiterals = 2,
        //
        // Summary:
        //     Return text input by the user as well as any literal characters defined in the
        //     mask and any instances of the prompt character.
        IncludePromptAndLiterals = 3
    }
}

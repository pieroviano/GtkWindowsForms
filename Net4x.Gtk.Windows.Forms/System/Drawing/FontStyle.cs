// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing
{
    //
    // Summary:
    //     Specifies style information applied to text.
    [Flags]
    public enum FontStyle
    {
        //
        // Summary:
        //     Normal text.
        Regular = 0,
        //
        // Summary:
        //     Bold text.
        Bold = 1,
        //
        // Summary:
        //     Italic text.
        Italic = 2,
        //
        // Summary:
        //     Underlined text.
        Underline = 4,
        //
        // Summary:
        //     Text with a line through the middle.
        Strikeout = 8
    }
}

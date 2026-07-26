// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the Input Method Editor (IME) and Asian language options of a <see cref="T:System.Windows.Forms.RichTextBox" />.</summary>
[Flags]
public enum RichTextBoxLanguageOptions
{
    /// <summary>The control automatically changes fonts when the user changes to a different keyboard layout.</summary>
    AutoFont = 2,

    /// <summary>Adjusts font sizes by scaling the East Asian font size.</summary>
    AutoFontSizeAdjust = 16,

    /// <summary>The control automatically changes the keyboard layout when the user changes to a different font.</summary>
    AutoKeyboard = 1,

    /// <summary>The control associates an East Asian font with an accompanying non-East Asian font.</summary>
    DualFont = 128,

    /// <summary>Always gives the application notification of IME conversion.</summary>
    ImeAlwaysSendNotify = 8,

    /// <summary>Determines how the control uses the IME conversion window.</summary>
    ImeCancelComplete = 4,

    /// <summary>Uses UI default fonts.</summary>
    UIFonts = 32,
}

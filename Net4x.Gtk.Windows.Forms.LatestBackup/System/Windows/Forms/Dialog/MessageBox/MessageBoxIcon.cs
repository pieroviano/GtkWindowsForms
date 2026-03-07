// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies constants defining which information to display.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum MessageBoxIcon
    {
        /// <summary>
        /// Specifies that the message box contain no symbols.
        /// </summary>
        None = 0,

        /// <summary>
        /// The message box contains a symbol consisting of a white X in a circle with a red background.
        /// </summary>
        Hand = 0x00000010,

        /// <summary>
        /// The message box contains a symbol consisting of a question mark in a circle. The question-mark message icon is no longer recommended because it does not clearly represent a specific type of message and because the phrasing of a message as a question could apply to any message type. In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use this question mark message symbol in your message boxes. The system continues to support its inclusion only for backward compatibility.
        /// </summary>
        Question = 0x00000020,

        /// <summary>
        /// The message box contains a symbol consisting of an exclamation point in a triangle with a yellow background.
        /// </summary>
        Exclamation = 0x00000030,

        /// <summary>
        /// Specifies that the message box contains an asterisk symbol.
        /// </summary>
        Asterisk = 0x00000040,

        /// <summary>
        /// Specifies that the message box contains a hand icon. This field is
        /// constant.
        /// </summary>
        Stop = Hand,

        /// <summary>
        /// The message box contains a symbol consisting of white X in a circle with a red background.
        /// </summary>
        Error = Hand,

        /// <summary>
        /// The message box contains a symbol consisting of an exclamation point in a triangle with a yellow background.
        /// </summary>
        Warning = Exclamation,

        /// <summary>
        /// Specifies that the message box contains an asterisk icon.
        /// </summary>
        Information = Asterisk,
    }
}

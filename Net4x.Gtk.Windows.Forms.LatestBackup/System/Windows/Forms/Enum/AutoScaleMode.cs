// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the different types of automatic scaling modes supported by Windows Forms.
    /// </summary>
    public enum AutoScaleMode
    {
        /// <summary>
        /// AutoScale is turned off.
        /// </summary>
        None,

        /// <summary>
        /// Controls scale relative to the dimensions of the font the classes are using, which is typically the system font.
        /// </summary>
        Font,

        /// <summary>
        /// Controls scale relative to the display resolution. Common resolutions are 96 and 120 DPI.
        /// </summary>
        Dpi,

        /// <summary>
        /// Controls scale according to their parent's scaling mode.
        /// If there is no parent, this behaves as if AutoScaleMode.None were set.
        /// </summary>
        Inherit
    }
}

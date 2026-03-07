// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.KeyPress" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class KeyPressEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.KeyPressEventArgs" /> class.
        /// </summary>
        /// <param name="keyChar">The ASCII character corresponding to the key the user pressed.
        /// </param>
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.Control.KeyPress" /> event was handled.
        /// </summary>
        /// <returns>true if the event is handled; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the character corresponding to the key pressed.
        /// </summary>
        /// <returns>The ASCII character that is composed. For example, if the user presses SHIFT + K, this property returns an uppercase K.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public char KeyChar { get; set; }
    }
}

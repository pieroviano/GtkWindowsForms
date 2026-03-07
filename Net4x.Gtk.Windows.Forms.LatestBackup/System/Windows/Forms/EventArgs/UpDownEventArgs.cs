// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for controls that derive from the
    /// <see cref="!:System.Windows.Forms.UpDownBase" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class UpDownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.UpDownEventArgs" /> class
        /// </summary>
        /// <param name="buttonPushed">The button that was clicked on the
        /// <see cref="System.Windows.Forms.UpDownBase" /> control.
        /// </param>
        public UpDownEventArgs(int buttonPushed)
        {
            ButtonID = buttonPushed;
        }

        /// <summary>
        /// Gets a value that represents which button the user clicked.
        /// </summary>
        /// <returns>A value that represents which button the user clicked.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ButtonID { get; }

    }
}

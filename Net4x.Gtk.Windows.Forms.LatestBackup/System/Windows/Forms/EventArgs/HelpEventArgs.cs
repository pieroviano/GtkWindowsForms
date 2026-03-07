// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.HelpRequested" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class HelpEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.HelpEventArgs" /> class.
        /// </summary>
        /// <param name="mousePos">The coordinates of the mouse pointer.
        /// </param>
        public HelpEventArgs(Point mousePos)
        {
            MousePos = mousePos;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the help event was handled.
        /// </summary>
        /// <returns>true if the event is handled; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the screen coordinates of the mouse pointer.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> representing the screen coordinates of the mouse pointer.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Point MousePos { get; }

    }
}

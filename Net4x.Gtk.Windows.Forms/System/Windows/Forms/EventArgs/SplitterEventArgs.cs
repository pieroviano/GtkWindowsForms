// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for
    /// <see cref="!:System.Windows.Forms.Splitter.SplitterMoving" /> and the
    /// <see cref="!:System.Windows.Forms.Splitter.SplitterMoved" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class SplitterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes an instance of the
        /// <see cref="System.Windows.Forms.SplitterEventArgs" /> class with the specified coordinates of the mouse pointer and the coordinates of the upper-left corner of the
        /// <see cref="System.Windows.Forms.Splitter" /> control.
        /// </summary>
        /// <param name="x">The x-coordinate of the mouse pointer (in client coordinates).
        /// </param>
        /// <param name="y">The y-coordinate of the mouse pointer (in client coordinates).
        /// </param>
        /// <param name="splitX">The x-coordinate of the upper-left corner of the
        /// <see cref="System.Windows.Forms.Splitter" /> (in client coordinates).
        /// </param>
        /// <param name="splitY">The y-coordinate of the upper-left corner of the
        /// <see cref="System.Windows.Forms.Splitter" /> (in client coordinates).
        /// </param>
        public SplitterEventArgs(int x, int y, int splitX, int splitY)
        {
            X = x;
            Y = y;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of the
        /// <see cref="System.Windows.Forms.Splitter" /> (in client coordinates).
        /// </summary>
        /// <returns>The x-coordinate of the upper-left corner of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int SplitX { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of the
        /// <see cref="System.Windows.Forms.Splitter" /> (in client coordinates).
        /// </summary>
        /// <returns>The y-coordinate of the upper-left corner of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int SplitY { get; set; }

        /// <summary>
        /// Gets the x-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        /// <returns>The x-coordinate of the mouse pointer.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        /// <returns>The y-coordinate of the mouse pointer.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Y { get; }

    }
}

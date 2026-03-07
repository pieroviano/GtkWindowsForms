// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for splitter events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class SplitterCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SplitterCancelEventArgs" /> class with the specified coordinates of the mouse pointer and the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </summary>
        /// <param name="mouseCursorX">The X coordinate of the mouse pointer in client coordinates.
        /// </param>
        /// <param name="mouseCursorY">The Y coordinate of the mouse pointer in client coordinates.
        /// </param>
        /// <param name="splitX">The X coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> in client coordinates.
        /// </param>
        /// <param name="splitY">The Y coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> in client coordinates.
        /// </param>
        public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY) : base(false)
        {
            MouseCursorX = mouseCursorX;
            MouseCursorY = mouseCursorY;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <summary>
        /// Gets the X coordinate of the mouse pointer in client coordinates.
        /// </summary>
        /// <returns>An integer representing the X coordinate of the mouse pointer in client coordinates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int MouseCursorX { get; }

        /// <summary>
        /// Gets the Y coordinate of the mouse pointer in client coordinates.
        /// </summary>
        /// <returns>An integer representing the Y coordinate of the mouse pointer in client coordinates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int MouseCursorY { get; }

        /// <summary>
        /// Gets or sets the X coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> in client coordinates.
        /// </summary>
        /// <returns>An integer representing the X coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int SplitX { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> in client coordinates.
        /// </summary>
        /// <returns>An integer representing the Y coordinate of the upper left corner of the
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int SplitY { get; set; }

    }
}

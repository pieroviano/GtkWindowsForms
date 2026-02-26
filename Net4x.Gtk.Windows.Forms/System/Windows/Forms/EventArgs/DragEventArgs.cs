// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.DragDrop" /> ,
    /// <see cref="System.Windows.Forms.Control.DragEnter" /> , or
    /// <see cref="System.Windows.Forms.Control.DragOver" /> event.
    /// </summary>
    [ComVisible(true)]
    public class DragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DragEventArgs" /> class.
        /// </summary>
        /// <param name="data">The data associated with this event.
        /// </param>
        /// <param name="keyState">The current state of the SHIFT, CTRL, and ALT keys.
        /// </param>
        /// <param name="x">The x-coordinate of the mouse cursor in pixels.
        /// </param>
        /// <param name="y">The y-coordinate of the mouse cursor in pixels.
        /// </param>
        /// <param name="allowedEffect">One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </param>
        /// <param name="effect">One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </param>
        public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect)
        {
            Data = data;
            KeyState = keyState;
            X = x;
            Y = y;
            AllowedEffect = allowedEffect;
            Effect = effect;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.IDataObject" /> that contains the data associated with this event.
        /// </summary>
        /// <returns>The data associated with this event.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IDataObject Data { get; }

        /// <summary>
        /// Gets the current state of the SHIFT, CTRL, and ALT keys, as well as the state of the mouse buttons.
        /// </summary>
        /// <returns>The current state of the SHIFT, CTRL, and ALT keys and of the mouse buttons.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int KeyState { get; }

        /// <summary>
        /// Gets the x-coordinate of the mouse pointer, in screen coordinates.
        /// </summary>
        /// <returns>The x-coordinate of the mouse pointer in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the mouse pointer, in screen coordinates.
        /// </summary>
        /// <returns>The y-coordinate of the mouse pointer in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Y { get; }

        /// <summary>
        /// Gets which drag-and-drop operations are allowed by the originator (or source) of the drag event.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DragDropEffects AllowedEffect { get; }

        /// <summary>
        /// Gets or sets the target drop effect in a drag-and-drop operation.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DragDropEffects Effect { get; set; }
    }
}

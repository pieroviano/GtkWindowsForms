// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.QueryContinueDrag" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class QueryContinueDragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.QueryContinueDragEventArgs" /> class.
        /// </summary>
        /// <param name="keyState">The current state of the SHIFT, CTRL, and ALT keys.
        /// </param>
        /// <param name="escapePressed">true if the ESC key was pressed; otherwise, false.
        /// </param>
        /// <param name="action">A
        /// <see cref="System.Windows.Forms.DragAction" /> value.
        /// </param>
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action)
        {
            KeyState = keyState;
            EscapePressed = escapePressed;
            Action = action;
        }

        /// <summary>
        /// Gets or sets the status of a drag-and-drop operation.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DragAction" /> value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DragAction Action { get; set; }

        /// <summary>
        /// Gets whether the user pressed the ESC key.
        /// </summary>
        /// <returns>true if the ESC key was pressed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool EscapePressed { get; }

        /// <summary>
        /// Gets the current state of the SHIFT, CTRL, and ALT keys.
        /// </summary>
        /// <returns>The current state of the SHIFT, CTRL, and ALT keys.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int KeyState { get; }

    }
}

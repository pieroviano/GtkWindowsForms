// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.GiveFeedback" />
    /// event, which occurs during a drag operation.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class GiveFeedbackEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.GiveFeedbackEventArgs" /> class.
        /// </summary>
        /// <param name="effect">The type of drag-and-drop operation. Possible values are obtained by applying the bitwise OR (|) operation to the constants defined in the
        /// <see cref="System.Windows.Forms.DragDropEffects" />
        /// </param>
        /// <param name="useDefaultCursors">true if default pointers are used; otherwise, false.
        /// </param>
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
        {
            Effect = effect;
            UseDefaultCursors = useDefaultCursors;
        }

        /// <summary>
        /// Gets the drag-and-drop operation feedback that is displayed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DragDropEffects Effect { get; }

        /// <summary>
        /// Gets or sets whether drag operation should use the default cursors that are associated with drag-drop effects.
        /// </summary>
        /// <returns>true if the default pointers are used; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool UseDefaultCursors { get; set; }

    }
}

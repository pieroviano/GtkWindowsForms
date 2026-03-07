// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.ControlAdded" /> and
    /// <see cref="System.Windows.Forms.Control.ControlRemoved" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ControlEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ControlEventArgs" /> class for the specified control.
        /// </summary>
        /// <param name="control">The
        /// <see cref="System.Windows.Forms.Control" /> to store in this event.
        /// </param>
        public ControlEventArgs(Control control)
        {
            Control = control;
        }

        /// <summary>
        /// Gets the control object used by this event.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Control" /> used by this event.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Control Control { get; }

    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Form.FormClosing" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class FormClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.FormClosingEventArgs" /> class.
        /// </summary>
        /// <param name="closeReason">A
        /// <see cref="System.Windows.Forms.CloseReason" /> value that represents the reason why the form is being closed.
        /// </param>
        /// <param name="cancel">true to cancel the event; otherwise, false.
        /// </param>
        public FormClosingEventArgs(CloseReason closeReason, bool cancel) : base(cancel)
        {
            CloseReason = closeReason;
        }

        /// <summary>
        /// Gets a value that indicates why the form is being closed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CloseReason" /> enumerated values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CloseReason CloseReason { get; }

    }
}

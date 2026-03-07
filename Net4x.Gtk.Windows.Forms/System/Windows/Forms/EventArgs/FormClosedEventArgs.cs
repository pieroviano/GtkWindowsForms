// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Form.FormClosed" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class FormClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.FormClosedEventArgs" /> class.
        /// </summary>
        /// <param name="closeReason">A
        /// <see cref="System.Windows.Forms.CloseReason" /> value that represents the reason why the form was closed.
        /// </param>
        public FormClosedEventArgs(CloseReason closeReason)
        {
            CloseReason = closeReason;
        }

        /// <summary>
        /// Gets a value that indicates why the form was closed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.CloseReason" /> enumerated values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CloseReason CloseReason { get; }

    }
}

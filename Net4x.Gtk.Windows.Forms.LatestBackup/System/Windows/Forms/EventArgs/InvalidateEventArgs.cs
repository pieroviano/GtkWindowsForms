// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.Invalidated" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class InvalidateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.InvalidateEventArgs" /> class.
        /// </summary>
        /// <param name="invalidRect">The
        /// <see cref="System.Drawing.Rectangle" /> that contains the invalidated window area.
        /// </param>
        public InvalidateEventArgs(Rectangle invalidRect)
        {
            InvalidRect = invalidRect;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Drawing.Rectangle" /> that contains the invalidated window area.
        /// </summary>
        /// <returns>The invalidated window area.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle InvalidRect { get; }

    }
}

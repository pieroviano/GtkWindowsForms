// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.RichTextBox.ContentsResized" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ContentsResizedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ContentsResizedEventArgs" /> class.
        /// </summary>
        /// <param name="newRectangle">A
        /// <see cref="System.Drawing.Rectangle" /> that specifies the requested dimensions of the
        /// <see cref="System.Windows.Forms.RichTextBox" /> control.
        /// </param>
        public ContentsResizedEventArgs(Rectangle newRectangle)
        {
            NewRectangle = newRectangle;
        }

        /// <summary>
        /// Represents the requested size of the
        /// <see cref="System.Windows.Forms.RichTextBox" /> control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the requested size of the
        /// <see cref="System.Windows.Forms.RichTextBox" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle NewRectangle { get; }
    }
}

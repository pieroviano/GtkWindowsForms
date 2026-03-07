// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.RichTextBox.LinkClicked" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class LinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LinkClickedEventArgs" /> class.
        /// </summary>
        /// <param name="linkText">The text of the link that is clicked in the
        /// <see cref="System.Windows.Forms.RichTextBox" /> control.
        /// </param>
        public LinkClickedEventArgs(string linkText)
        {
            LinkText = linkText;
        }

        /// <summary>
        /// Gets the text of the link being clicked.
        /// </summary>
        /// <returns>The text of the link that is clicked in the
        /// <see cref="System.Windows.Forms.RichTextBox" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string LinkText { get; }
    }
}

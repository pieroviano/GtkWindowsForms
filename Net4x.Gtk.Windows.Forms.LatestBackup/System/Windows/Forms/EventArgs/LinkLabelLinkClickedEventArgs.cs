// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.LinkLabel.LinkClicked" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [ComVisible(true)]
    public class LinkLabelLinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs" /> class with the specified link.
        /// </summary>
        /// <param name="link">The
        /// <see cref="System.Windows.Forms.LinkLabel.Link" /> that was clicked.
        /// </param>
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link) : this(link, MouseButtons.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs" /> class with the specified link and the specified mouse button.
        /// </summary>
        /// <param name="link">The
        /// <see cref="System.Windows.Forms.LinkLabel.Link" /> that was clicked.
        /// </param>
        /// <param name="button">One of the
        /// <see cref="System.Windows.Forms.MouseButtons" /> values.
        /// </param>
        public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button)
        {
            Link = link;
            Button = button;
        }

        /// <summary>
        /// Gets the mouse button that causes the link to be clicked.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.MouseButtons" /> values.
        /// </returns>
        public MouseButtons Button { get; }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.LinkLabel.Link" /> that was clicked.
        /// </summary>
        /// <returns>A link on the
        /// <see cref="System.Windows.Forms.LinkLabel" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public LinkLabel.Link Link { get; }

    }
}

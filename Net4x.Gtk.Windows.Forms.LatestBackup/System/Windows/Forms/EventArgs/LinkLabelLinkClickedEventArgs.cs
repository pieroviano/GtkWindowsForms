// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.LinkLabel.LinkClicked" /> event.
/// </summary>
[ComVisible(true)]
public class LinkLabelLinkClickedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LinkLabelLinkClickedEventArgs" /> class, given the link.
    /// </summary>
    /// <param name="link">
    /// The link.
    /// </param>
    public LinkLabelLinkClickedEventArgs(LinkLabel.Link link) : this(link, MouseButtons.Left)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LinkLabelLinkClickedEventArgs" /> class.
    /// </summary>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <param name="button">
    /// The button.
    /// </param>
    public LinkLabelLinkClickedEventArgs(LinkLabel.Link link, MouseButtons button)
    {
        Link = link;
        Button = button;
    }

    /// <summary>
    /// Gets the mouseButton which causes the link to be clicked
    /// </summary>
    /// <value>
    /// The button.
    /// </value>
    public MouseButtons Button { get; }

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> that was clicked.
    /// </summary>
    /// <value>
    /// The link.
    /// </value>
    public LinkLabel.Link Link { get; }

}
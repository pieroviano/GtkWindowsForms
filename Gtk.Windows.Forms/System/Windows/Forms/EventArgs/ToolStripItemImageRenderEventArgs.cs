// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the image rendering event on a tool strip item.</summary>
public class ToolStripItemImageRenderEventArgs : ToolStripItemRenderEventArgs
{
    /// <summary>Initializes a new instance using the item image.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="item">The item to paint.</param>
    /// <param name="imageRectangle">The bounds of the image.</param>
    public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Rectangle imageRectangle)
        : base(g, item)
    {
        ImageRectangle = imageRectangle;
    }

    /// <summary>Initializes a new instance with a specified image.</summary>
    /// <param name="g">The graphics used to paint.</param>
    /// <param name="item">The item to paint.</param>
    /// <param name="image">The image to paint.</param>
    /// <param name="imageRectangle">The bounds of the image.</param>
    public ToolStripItemImageRenderEventArgs(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle)
        : base(g, item)
    {
        Image = image;
        ImageRectangle = imageRectangle;
    }

    /// <summary>Gets the image to paint.</summary>
    public Image? Image { get; }

    /// <summary>Gets the bounds of the image.</summary>
    public Rectangle ImageRectangle { get; }
}

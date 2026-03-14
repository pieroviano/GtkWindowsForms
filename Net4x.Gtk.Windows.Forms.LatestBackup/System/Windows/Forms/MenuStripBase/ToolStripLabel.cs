// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;

namespace System.Windows.Forms;

/// <summary>
/// Represents a nonselectable
/// <see cref="T:System.Windows.Forms.ToolStripItem" />
/// that renders text and images and can display hyperlinks.
/// </summary>
public class ToolStripLabel : WidgetToolStrip<Gtk.MenuItem>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripLabel" /> class.
    /// </summary>
    public ToolStripLabel() : this("", null, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ToolStripLabel(string text) : this(text, null, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripLabel(string text, Image image) : this(text, image, null, "")
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="onClick">
    /// The on click.
    /// </param>
    public ToolStripLabel(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="onClick">
    /// The on click.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    public ToolStripLabel(string text, Image image, EventHandler onClick, string name) : base(null, text, image, onClick, name)
    {
    }

}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Gtk.Windows.Forms.Controls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a nonselectable
/// <see cref="T:System.Windows.Forms.ToolStripItem" />
/// that renders text and images and can display hyperlinks.
/// </summary>
public class ToolStripLabel : ToolStripItem
{
    /// <summary>
    /// The self
    /// </summary>
    public StripToolButton self = new StripToolButton();

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
    public ToolStripLabel(string text, Image? image) : this(text, image, null, "")
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
    public ToolStripLabel(string text, Image? image, EventHandler? onClick) : this(text, image, onClick, "")
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
    public ToolStripLabel(string text, Image? image, EventHandler? onClick, string name) : base(text, image, onClick, name)
    {
        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        self.Realized += Self_Realized;
    }

    /// <summary>
    /// Gets or sets the display style.
    /// </summary>
    /// <value>
    /// The display style.
    /// </value>
    public override ToolStripItemDisplayStyle DisplayStyle
    {
        get => base.DisplayStyle;
        set
        {
            base.DisplayStyle = value;
            SetIcon(value);
        }
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public override string Name
    {
        get => self.Name;
        set => self.Name = value;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public override string Text
    {
        get => self.Text;
        set => self.Text = value;
    }

    /// <summary>
    /// Gets the widget.
    /// </summary>
    /// <value>
    /// The widget.
    /// </value>
    public override IToolMenuItem Widget { get => self; }

    private void Self_Realized(object? sender, EventArgs e)
    {
        SetIcon(DisplayStyle);
    }

    private void SetIcon(ToolStripItemDisplayStyle displayStyle)
    {
        if (displayStyle == ToolStripItemDisplayStyle.Text)
        {
        }
        else if (displayStyle == ToolStripItemDisplayStyle.Image)
        {
            if (this.Image?.Pixbuf != null)
            {
                if (self.Parent is ToolStripBase toolbar)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, toolbar.ImageScalingSize.Width, toolbar.ImageScalingSize.Height);
                }
            }
        }
        else if (displayStyle == ToolStripItemDisplayStyle.ImageAndText)
        {
            if (this.Image?.Pixbuf != null)
            {
                if (self.Parent is ToolStripBase toolbar)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, toolbar.ImageScalingSize.Width, toolbar.ImageScalingSize.Height);
                }
            }
        }
        else
        {
        }
    }
}

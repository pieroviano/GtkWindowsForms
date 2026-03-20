using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a panel in a
/// <see cref="T:System.Windows.Forms.StatusStrip" /> control.
/// </summary>
public class ToolStripStatusLabel : ToolStripLabel
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
    /// </summary>
    public ToolStripStatusLabel()
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ToolStripStatusLabel(string text)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripStatusLabel(Image image)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripStatusLabel(string text, Image image)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
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
    public ToolStripStatusLabel(string text, Image image, EventHandler onClick)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> class.
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
    public ToolStripStatusLabel(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
    {
    }

    /// <summary>
    /// Gets or sets the border sides.
    /// </summary>
    /// <value>
    /// The border sides.
    /// </value>
    [DefaultValue(ToolStripStatusLabelBorderSides.None)]
    public ToolStripStatusLabelBorderSides BorderSides
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the border style.
    /// </summary>
    /// <value>
    /// The border style.
    /// </value>
    [DefaultValue(Border3DStyle.Flat)]
    public Border3DStyle BorderStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripStatusLabel" /> is spring.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if spring; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool Spring
    {
        get;
        set;
    }

}
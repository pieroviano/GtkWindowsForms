using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a selectable
/// <see cref="T:System.Windows.Forms.ToolStripItem" />
/// that can contain text and images.
/// </summary>
public class ToolStripButton : WidgetToolStrip<Gtk.MenuItem>
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
    /// </summary>
    public ToolStripButton() : this("", null, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ToolStripButton(string text) : this(text, null, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripButton(Image image) : this("", image, null, "") { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripButton(string text, Image image) : this(text, image, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
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
    public ToolStripButton(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripButton" /> class.
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
    public ToolStripButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
    {
        DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
    }

    /// <summary>
    /// Gets or sets a value indicating whether check on click.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if check on click; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool CheckOnClick { get; set; }

    /// <summary>
    /// Gets a value indicating whether default automatic tool tip.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if default automatic tool tip; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    protected bool DefaultAutoToolTip { get; }

    /// <summary>
    /// Creates the accessibility instance.
    /// </summary>
    /// <returns>
    /// AccessibleObject.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected AccessibleObject CreateAccessibilityInstance()
    {
        return new AccessibleObject();
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:CheckedChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnCheckedChanged(EventArgs e) { }

    /// <summary>
    /// Handles the
    /// <see cref="!:CheckStateChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnCheckStateChanged(EventArgs e)
    {

    }

}
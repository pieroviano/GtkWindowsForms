using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a control that when clicked displays an associated
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" />
/// from which the user can select a single item.
/// </summary>
public class ToolStripDropDownButton : ToolStripDropDownItem
{
    public virtual event EventHandler<MouseEventArgs> MouseDown;

    public virtual event EventHandler<EventArgs> MouseLeave;

    public virtual event EventHandler<MouseEventArgs> MouseUp;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
    /// </summary>
    public ToolStripDropDownButton() { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ToolStripDropDownButton(string text) : base(text, null) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripDropDownButton(Image image) : base("", image) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripDropDownButton(string text, Image image) : base(text, image) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
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
    public ToolStripDropDownButton(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="dropDownItems">
    /// The drop down items.
    /// </param>
    public ToolStripDropDownButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> class.
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
    public ToolStripDropDownButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
    {

    }

    /// <summary>
    /// Gets or sets a value indicating whether show drop down arrow.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show drop down arrow; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    //[DefaultValue(true)]
    //public bool AutoToolTip { get; set; }
    [DefaultValue(true)]
    public bool ShowDropDownArrow
    {
        get;
        set;
    }

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
    /// Handles the
    /// <see cref="!:MouseDown" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    //protected AccessibleObject CreateAccessibilityInstance()
    //{
    //}
    //protected  ToolStripDropDown CreateDefaultDropDown()
    //{
    //}
    protected void OnMouseDown(MouseEventArgs e)
    {
        MouseDown?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:MouseLeave" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected void OnMouseLeave(EventArgs e)
    {
        MouseLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:MouseUp" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    protected void OnMouseUp(MouseEventArgs e)
    {
        MouseUp?.Invoke(this, e);
    }
}
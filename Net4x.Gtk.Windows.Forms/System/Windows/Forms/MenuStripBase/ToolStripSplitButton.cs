using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a combination of a standard button on the left and a drop-down button on the right, or the other way around if the value of
/// <see cref="T:System.Windows.Forms.RightToLeft" /> is Yes.
/// </summary>
public class ToolStripSplitButton : ToolStripDropDownItem
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
    /// </summary>
    public ToolStripSplitButton() { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ToolStripSplitButton(string text) : base(text, null) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
    /// </summary>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripSplitButton(Image image) : base("", image) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    public ToolStripSplitButton(string text, Image image) : base(text, image) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
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
    public ToolStripSplitButton(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
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
    public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> class.
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
    public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
    {

    }

    /// <summary>
    /// Gets the button bounds.
    /// </summary>
    /// <value>
    /// The button bounds.
    /// </value>
    [Browsable(false)]
    public Rectangle ButtonBounds { get; }

    /// <summary>
    /// Gets a value indicating whether button pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if button pressed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool ButtonPressed { get; }

    /// <summary>
    /// Gets a value indicating whether button selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if button selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool ButtonSelected { get; }

    /// <summary>
    /// Gets or sets the default item.
    /// </summary>
    /// <value>
    /// The default item.
    /// </value>
    [Browsable(false)]
    [DefaultValue(null)]
    public ToolStripItem DefaultItem
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the drop down button bounds.
    /// </summary>
    /// <value>
    /// The drop down button bounds.
    /// </value>
    [Browsable(false)]
    public Rectangle DropDownButtonBounds { get; }

    /// <summary>
    /// Gets a value indicating whether drop down button pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if drop down button pressed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool DropDownButtonPressed { get; }

    /// <summary>
    /// Gets a value indicating whether drop down button selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if drop down button selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool DropDownButtonSelected { get; }

    /// <summary>
    /// Gets or sets the width of the drop down button.
    /// </summary>
    /// <value>
    /// The width of the drop down button.
    /// </value>
    //[DefaultValue(true)]
    //public bool AutoToolTip { get; set; }
    //
    public int DropDownButtonWidth
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the splitter bounds.
    /// </summary>
    /// <value>
    /// The splitter bounds.
    /// </value>
    [Browsable(false)]
    public Rectangle SplitterBounds { get; }

    /// <summary>
    /// Gets a value indicating whether dismiss when clicked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if dismiss when clicked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    protected internal bool DismissWhenClicked { get; }

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
}
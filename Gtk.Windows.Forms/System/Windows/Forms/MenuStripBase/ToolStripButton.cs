using System.ComponentModel;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a selectable
/// <see cref="T:System.Windows.Forms.ToolStripItem" />
/// that can contain text and images.
/// </summary>
public class ToolStripButton : ToolStripItem
{
    /// <summary>
    /// The self
    /// </summary>
    public StripToolButton self = new StripToolButton();

    private CheckState _checkState = CheckState.Unchecked;

    private CheckState _prevCheckState = CheckState.Unchecked;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public override event EventHandler? CheckedChanged;

    /// <summary>
    /// Occurs when check state changed.
    /// </summary>
    public override event EventHandler? CheckStateChanged;

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
    public ToolStripButton(string text, Image? image) : this(text, image, null, "")
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
    public ToolStripButton(string text, Image? image, EventHandler? onClick) : this(text, image, onClick, "")
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
    public ToolStripButton(string text, Image? image, EventHandler? onClick, string name) : base(text, image, onClick, name)
    {
        self.Halign = Gtk.Align.Start;
        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        self.Realized += Self_Realized;
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
    public bool CheckOnClick
    {
        get;
        set;
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
    protected override void OnCheckedChanged(EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the
    /// <see cref="!:CheckStateChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected override void OnCheckStateChanged(EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    private void Self_Realized(object? sender, EventArgs e)
    {
        SetIcon(DisplayStyle);
    }

    private void SetIcon(ToolStripItemDisplayStyle displayStyle)
    {
        if (displayStyle == ToolStripItemDisplayStyle.Text)
        {
            self.IsImportant = false;
        }
        else if (displayStyle == ToolStripItemDisplayStyle.Image)
        {
            self.IsImportant = false;
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
            self.IsImportant = true;
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
            self.IsImportant = false;
        }
    }
}

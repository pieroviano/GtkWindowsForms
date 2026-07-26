using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Delegate ToolStripItemClickedEventHandler
/// </summary>
/// <param name="sender">
/// The sender.
/// </param>
/// <param name="e">
/// The
/// <see cref="T:System.Windows.Forms.ToolStripItemClickedEventArgs" /> instance containing the event data.
/// </param>
public delegate void ToolStripItemClickedEventHandler(object sender, ToolStripItemClickedEventArgs e);
/// <summary>
/// Provides basic functionality for controls that display a
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> when a
/// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> ,
/// <see cref="T:System.Windows.Forms.ToolStripMenuItem" /> , or
/// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> control is clicked.
/// </summary>
public abstract class ToolStripDropDownItem : ToolStripItem
{
    /// <summary>
    /// The self
    /// </summary>
    public StripMenuToolButton self = new StripMenuToolButton();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownItem" /> class.
    /// </summary>
    protected ToolStripDropDownItem() : this("", null, null, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownItem" /> class.
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
    protected ToolStripDropDownItem(string text, Image? image, EventHandler? onClick) : this(text, image, onClick, "")
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownItem" /> class.
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
    protected ToolStripDropDownItem(string text, Image? image, params ToolStripItem[] dropDownItems) : this(text, image, null, "")
    {
        DropDownItems.AddRange(dropDownItems);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownItem" /> class.
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
    protected ToolStripDropDownItem(string text, Image? image, EventHandler? onClick, string name) : base(text, image, onClick, name)
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
    /// Gets or sets the drop down.
    /// </summary>
    /// <value>
    /// The drop down.
    /// </value>
    [TypeConverter(typeof(ReferenceConverter))]
    public ToolStripDropDown? DropDown
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the drop down direction.
    /// </summary>
    /// <value>
    /// The drop down direction.
    /// </value>
    [Browsable(false)]
    public ToolStripDropDownDirection DropDownDirection
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has drop down.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance has drop down; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool HasDropDown { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has drop down items.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance has drop down items; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public virtual bool HasDropDownItems { get => DropDownItems != null && DropDownItems.Count > 0; }

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
    /// Gets the drop down location.
    /// </summary>
    /// <value>
    /// The drop down location.
    /// </value>
    protected internal virtual Point DropDownLocation { get; }

    /// <summary>
    /// Hides the drop down.
    /// </summary>
    public void HideDropDown() { }

    /// <summary>
    /// Shows the drop down.
    /// </summary>
    public void ShowDropDown() { }

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
                if (self.IconSize == Gtk.IconSize.LargeToolbar)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 30, 30);
                }
                else if (self.IconSize == Gtk.IconSize.Dialog)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 50, 50);
                }
                else
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 20, 20);
                }
            }
        }
        else if (displayStyle == ToolStripItemDisplayStyle.ImageAndText)
        {
            self.IsImportant = true;
            if (this.Image?.Pixbuf != null)
            {
                if (self.IconSize == Gtk.IconSize.LargeToolbar)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 30, 30);
                }
                else if (self.IconSize == Gtk.IconSize.Dialog)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 50, 50);
                }
                else
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 20, 20);
                }
            }
        }
        else
        {
            self.IsImportant = true;
            if (this.Image?.Pixbuf != null)
            {
                if (self.IconSize == Gtk.IconSize.LargeToolbar)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 30, 30);
                }
                else if (self.IconSize == Gtk.IconSize.Dialog)
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 50, 50);
                }
                else
                {
                    self.Image = new Gdk.Pixbuf(this.Image.PixbufData, 20, 20);
                }
            }
        }
    }
}

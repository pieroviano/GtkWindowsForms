using Gtk;
using Image = System.Drawing.Image;

namespace System.Windows.Forms;

/// <summary>
/// Represents a selectable option displayed on a
/// <see cref="T:System.Windows.Forms.MenuStrip" /> or
/// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> Although
/// <see cref="T:System.Windows.Forms.ToolStripMenuItem" />
/// replaces and adds functionality to the
/// <see cref="!:System.Windows.Forms.MenuItem" /> control of previous versions,
/// <see cref="!:System.Windows.Forms.MenuItem" />
/// is retained for both backward compatibility and future use if you choose.
/// </summary>
public class ToolStripMenuItem : ToolStripItem
{

    public StripMenuItem self = new StripMenuItem();

    public override event EventHandler? CheckedChanged;

    public override event EventHandler? CheckStateChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripMenuItem" /> class.
    /// </summary>
    public ToolStripMenuItem() : base()
    {
        checkButton.StyleContext.AddClass("MenuItemButton");
        self.Realized += Self_Realized;
        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
    }

    /// <summary>
    /// Initializes a new menu item displaying the specified text.
    /// </summary>
    /// <param name="text">The text to display on the menu item.</param>
    public ToolStripMenuItem(string? text) : this()
    {
        // The WinForms text constructors were missing, so ordinary code such as
        // new ToolStripMenuItem("Open") did not compile against this shim.
        Text = text ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new menu item displaying the specified text and image.
    /// </summary>
    /// <param name="text">The text to display on the menu item.</param>
    /// <param name="image">The image to display on the menu item.</param>
    public ToolStripMenuItem(string? text, Image? image) : this(text)
    {
        Image = image;
    }

    /// <summary>
    /// Initializes a new menu item displaying the specified text and image, and raising
    /// <see cref="E:System.Windows.Forms.ToolStripItem.Click" /> through the supplied handler.
    /// </summary>
    /// <param name="text">The text to display on the menu item.</param>
    /// <param name="image">The image to display on the menu item.</param>
    /// <param name="onClick">The handler invoked when the item is clicked.</param>
    public ToolStripMenuItem(string? text, Image? image, EventHandler? onClick) : this(text, image)
    {
        if (onClick != null)
        {
            Click += onClick;
        }
    }

    public Gtk.CheckButton checkButton => self.checkButton;

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public override bool Checked
    {
        // Checked drove the GTK check-indicator visibility while CheckState drove the actual toggle, so the
        // two never agreed. In WinForms CheckState is just the tri-state form of Checked.
        get => self.CheckState != CheckState.Unchecked;
        set
        {
            // Do not route through base.Checked: it now derives from CheckState, which this type
            // overrides, so the two setters would call each other forever.
            self.Checked = value;
            self.CheckState = value ? CheckState.Checked : CheckState.Unchecked;
        }
    }

    /// <summary>
    /// Gets or sets the state of the check.
    /// </summary>
    /// <value>
    /// The state of the check.
    /// </value>
    public override CheckState CheckState
    {
        get => self.CheckState;
        set
        {
            self.CheckState = value;
            self.Checked = value != CheckState.Unchecked;
        }
    }

    public override ToolStripItemDisplayStyle DisplayStyle
    {
        get => base.DisplayStyle;
        set => base.DisplayStyle = value;
    }

    public override Image? Image
    {
        get => base.Image;
        set
        {
            base.Image = value;
            SetIconImage(value);
        }
    }

    public override Gtk.Widget MenuItem
    {
        get => self;
        set { }
    }

    public override string Name
    {
        get => self.Name;
        set => self.Name = value;
    }

    public override string Text
    {
        get => self.Text;
        set { self.Text = value ?? string.Empty; }
    }

    public override TextImageRelation TextImageRelation
    {
        get => base.TextImageRelation;
        set
        {
            base.TextImageRelation = value;
            if (value == TextImageRelation.ImageAboveText)
            {
                checkButton.Halign = Align.Fill;
            }
            else if (value == TextImageRelation.TextAboveImage)
            {
                checkButton.Halign = Align.Fill;
            }
            else if (value == TextImageRelation.ImageBeforeText)
            {
                checkButton.Halign = Align.Start;
            }
            else if (value == TextImageRelation.TextBeforeImage)
            {
                checkButton.Halign = Align.Start;
            }
            else
            {
                checkButton.Halign = Align.Start;
            }
        }
    }

    public override IToolMenuItem Widget { get => self; }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected new virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected new virtual void OnCheckStateChanged(System.EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    private void Self_Realized(object? sender, EventArgs e)
    {
        SetIconImage(this.Image);
    }

    private void SetIconImage(Image? image)
    {
        if (self.IsRealized && image?.Pixbuf != null)
        {
            var x = 20;
            if (self.Parent is Gtk.Box) // Gtk4: Toolbar/MenuBar removed; host is a Box
                x = Math.Max(20, self.AllocatedHeight / 2);
            Gdk.Pixbuf pixbuf = image.Pixbuf.ScaleSimple(x, x, Gdk.InterpType.Bilinear);
            self.Image = pixbuf;
        }
    }
}

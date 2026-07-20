using Gtk;

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

    public override event EventHandler CheckedChanged;

    public override event EventHandler CheckStateChanged;

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
        get => base.Checked;
        set
        {
            base.Checked = value;
            self.Checked = value;
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
        set => self.CheckState = value;
    }

    public override ToolStripItemDisplayStyle DisplayStyle
    {
        get => base.DisplayStyle;
        set => base.DisplayStyle = value;
    }

    public override Drawing.Image Image
    {
        get => base.Image;
        set
        {
            base.Image = value;
            SetIconImage(value);
        }
    }

    public override Gtk.MenuItem MenuItem
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
        set { self.Text = value; }
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
                checkButton.ImagePosition = PositionType.Top;
            }
            else if (value == TextImageRelation.TextAboveImage)
            {
                checkButton.Halign = Align.Fill;
                checkButton.ImagePosition = PositionType.Bottom;
            }
            else if (value == TextImageRelation.ImageBeforeText)
            {
                checkButton.Halign = Align.Start;
                checkButton.ImagePosition = PositionType.Left;
            }
            else if (value == TextImageRelation.TextBeforeImage)
            {
                checkButton.Halign = Align.Start;
                checkButton.ImagePosition = PositionType.Right;
            }
            else
            {
                checkButton.Halign = Align.Start;
                checkButton.ImagePosition = PositionType.Left;
            }
        }
    }

    public override IToolMenuItem Widget { get => self; }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged(System.EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected virtual void OnCheckStateChanged(System.EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    private void Self_Realized(object? sender, EventArgs e)
    {
        SetIconImage(this.Image);
    }

    private void SetIconImage(Drawing.Image image)
    {
        if (self.IsRealized && image?.Pixbuf != null)
        {
            var x = 20;
            if (self.Parent is Gtk.Toolbar || self.Parent is Gtk.MenuBar)
                x = Math.Max(20, self.AllocatedHeight / 2);
            Gdk.Pixbuf pixbuf = image.Pixbuf.ScaleSimple(x, x, Gdk.InterpType.Bilinear);
            self.Image = pixbuf;
        }
    }
}

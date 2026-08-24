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
            // overrides, so the two setters would call each other forever. CheckState is the single
            // place the state is written, so both setters notify exactly once.
            CheckState = value ? CheckState.Checked : CheckState.Unchecked;
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
            var previous = self.CheckState;

            // Only CheckState is written: StripMenuItem.CheckState already drives both the toggle and the
            // inconsistent flag. Assigning self.Checked afterwards used to switch the toggle back on for
            // CheckState.Indeterminate, which is "inconsistent, not active".
            self.CheckState = value;

            if (previous == value)
            {
                return;
            }

            // WinForms raises both events from the state change itself, so a programmatic assignment
            // notifies just like a user click does.
            OnCheckedChanged(EventArgs.Empty);
            OnCheckStateChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item's
    /// <see cref="Checked" /> state flips each time the item is clicked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if a click toggles the check state; otherwise,
    /// <c>
    /// false
    /// </c> (the WinForms default), in which case the state is only ever set in code.
    /// </value>
    /// <remarks>
    /// The property was missing altogether, so the shim had no way to express a checkable menu item: the
    /// backing Gtk4 CheckButton's own toggle is always undone (see <see cref="StripMenuItem" />), which
    /// left an item like the sample's checked "Item 1" impossible to uncheck by clicking it.
    /// </remarks>
    public bool CheckOnClick { get; set; }

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
            // Stack or place the icon/caption per the relation (e.g. ImageAboveText -> icon over text, the
            // Gtk3 top-level menu look). Previously only checkButton.Halign was tweaked, so the icon always
            // stayed on the same row as the text.
            self.SetTextImageRelation(value);
        }
    }

    public override IToolMenuItem Widget { get => self; }

    /// <summary>
    /// Raises the Click event, first flipping the check state when
    /// <see cref="CheckOnClick" /> is set.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <remarks>
    /// WinForms toggles a check-on-click item from its click handling and raises CheckedChanged before
    /// Click, so handlers observe the new state; the order is kept here. Nothing happens for an ordinary
    /// command item, whose check state stays under the application's control.
    /// </remarks>
    protected override void OnClick(EventArgs e)
    {
        if (CheckOnClick)
        {
            CheckState = CheckState == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked;
        }

        base.OnClick(e);
    }

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

        // Gtk4 realizes a widget before it is allocated, so AllocatedHeight is still 0 here and the icon
        // would be stuck at the minimum size - noticeably smaller than the Gtk3 build, whose menu-bar icons
        // scale to half the item height. Re-apply once the first allocation has run.
        GLib.Idle.Add(() =>
        {
            // The item's window can be torn down before the idle runs - a short-lived form in a test does
            // exactly that - and touching a widget whose native object has been freed takes the process
            // down with no managed exception. GtkSharp zeroes the wrapper's Handle when that happens, so it
            // is a reliable liveness check.
            if (self.Handle != IntPtr.Zero)
            {
                SetIconImage(this.Image);
            }

            return false;
        });
    }

    /// <summary>
    /// Pushes the WinForms <see cref="Image" /> down to the backing widget, scaled for a menu icon.
    /// </summary>
    /// <param name="image">The image to show, or <see langword="null" /> to clear the icon.</param>
    /// <remarks>
    /// Assignment must not depend on the widget being realized. It used to, so an item whose icon was set
    /// by the Designer - which runs long before anything is on screen - showed no icon at all unless it
    /// happened to be realized again afterwards; that is why the sample's "b Menu" lost the image its Gtk3
    /// counterpart shows. Realization still matters for one thing only: the allocated height is what a tall
    /// top-level entry is scaled against, and that is unknown until then, so <see cref="Self_Realized" />
    /// re-runs this to refine the size.
    /// </remarks>
    private void SetIconImage(Image? image)
    {
        var source = image?.Pixbuf;
        if (source == null)
        {
            self.Image = null!;
            return;
        }

        var size = 20;
        if (self.IsRealized && self.Parent is Gtk.Box) // Gtk4: Toolbar/MenuBar removed; host is a Box
        {
            size = Math.Max(20, self.AllocatedHeight / 2);
        }

        // Rescaling is not free and this runs again on every allocation pass, so skip it when nothing about
        // the result would change.
        if (size == _appliedIconSize && ReferenceEquals(source, _appliedIconSource))
        {
            return;
        }

        _appliedIconSize = size;
        _appliedIconSource = source;
        self.Image = source.ScaleSimple(size, size, Gdk.InterpType.Bilinear);
    }

    /// <summary>The edge length the icon was last scaled to, so an unchanged size is not rescaled.</summary>
    private int _appliedIconSize;

    /// <summary>The pixbuf the icon was last scaled from, paired with <see cref="_appliedIconSize" />.</summary>
    private Gdk.Pixbuf? _appliedIconSource;
}

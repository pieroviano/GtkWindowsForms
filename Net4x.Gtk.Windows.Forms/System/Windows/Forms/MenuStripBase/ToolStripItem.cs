using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Layout;
using Gtk;
using Image = System.Drawing.Image;

namespace System.Windows.Forms;

/// <summary>
/// Represents the abstract base class that manages events and layout for all the elements that a
/// <see cref="T:System.Windows.Forms.ToolStrip" /> or
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> can contain.
/// </summary>
public partial class ToolStripItem : Component, IDropTarget, ISupportOleDropSource, IArrangedElement, IComponent, IDisposable, IKeyboardToolTip
{
    CssProvider provider = new CssProvider();

    private ToolStripItemAlignment _Alignment;

    private Color _BackColor;

    //  public virtual bool Focused { get { return this.IsFocus; } }

    private Font _Font = null!;

    private Color _ForeColor;

    //public virtual ISite Site { get; set; }

    private Size _size;

    private bool _widgetRealized = false;

    private ToolStripItemCollection dropDownItems;

    private bool isDoubleClick = false;

    private TextImageRelation textImageRelation;

    public virtual event EventHandler? AnchorChanged;

    public virtual event EventHandler? ButtonClick;

    public virtual event EventHandler? ButtonDoubleClick;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler? CheckedChanged;

    /// <summary>
    /// Occurs when check state changed.
    /// </summary>
    public virtual event EventHandler? CheckStateChanged;

    /// <summary>
    /// Occurs when click.
    /// </summary>
    public virtual event EventHandler? Click;

    public virtual event EventHandler? ContextMenuStripChanged;

    public virtual event EventHandler? DockChanged;

    public virtual event EventHandler? DoubleClick;

    public virtual event EventHandler? DropDownClosed;

    /// <summary>
    /// Occurs when drop down item clicked.
    /// </summary>
    public virtual event ToolStripItemClickedEventHandler? DropDownItemClicked;

    public virtual event EventHandler? DropDownOpened;

    public virtual event EventHandler? DropDownOpening;

    public virtual event EventHandler? EnabledChanged;

    /// <summary>Occurs when the user drops data on the item during a drag-and-drop operation.</summary>
    public virtual event DragEventHandler? DragDrop;

    /// <summary>Occurs when the pointer enters the item during a drag-and-drop operation.</summary>
    public virtual event DragEventHandler? DragEnter;

    /// <summary>Occurs when the pointer moves over the item during a drag-and-drop operation.</summary>
    public virtual event DragEventHandler? DragOver;

    /// <summary>Occurs when the pointer leaves the item during a drag-and-drop operation.</summary>
    public virtual event EventHandler? DragLeave;

    public virtual event EventHandler? Enter;

    public virtual event KeyEventHandler? KeyDown;

    public virtual event KeyPressEventHandler? KeyPress;

    public virtual event KeyEventHandler? KeyUp;

    public virtual event LayoutEventHandler? Layout;

    public virtual event EventHandler? Leave;

    public virtual event EventHandler? LocationChanged;

    public virtual event EventHandler? LostFocus;

    public virtual event MouseEventHandler? MouseClick;

    public virtual event MouseEventHandler? MouseDoubleClick;

    public virtual event MouseEventHandler? MouseDown;

    public virtual event EventHandler? MouseEnter;

    public virtual event EventHandler? MouseHover;

    public virtual event EventHandler? MouseLeave;

    public virtual event MouseEventHandler? MouseMove;

    public virtual event MouseEventHandler? MouseUp;

    public virtual event MouseEventHandler? MouseWheel;

    public virtual event EventHandler? Move;

    public virtual event PaintEventHandler? Paint;

    public virtual event EventHandler? RegionChanged;

    public virtual event EventHandler? Resize;

    public virtual event EventHandler? SizeChanged;

    public virtual event EventHandler? TextChanged;

    public virtual event EventHandler? VisibleChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> class.
    /// </summary>
    public ToolStripItem()
    {
        dropDownItems = new ToolStripItemCollection(this);
        this.unique_key = Guid.NewGuid().ToString().ToLower();
        var _widget = GetWidget();
        // A bare ToolStripItem (no concrete subclass overriding Widget) has no backing GTK widget - there
        // is nothing to wire. Returning here matches the pre-alignment ctor, which did no widget wiring,
        // and avoids a NullReferenceException that would crash the GTK signal callback it runs in.
        if (_widget == null)
        {
            return;
        }
        _widget.Realized += _widget_Realized;
        _widget.ButtonPressEvent += _widget_ButtonPressEvent;
        _widget.ButtonReleaseEvent += _widget_ButtonReleaseEvent;
        if (_widget is Gtk.ToolButton widget)
        {
            widget.Clicked += Widget_Clicked;
        }
        else if (_widget is Gtk.MenuToolButton widget2)
        {
            widget2.Clicked += Widget_Clicked;
        }
        else if (_widget is Gtk.MenuItem widget3)
        {
            widget3.Selected += Widget3_Selected;
            widget3.Deselected += Widget3_Deselected;
            widget3.Activated += Widget3_Activated;
        }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> class.
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
    protected ToolStripItem(string text, Drawing.Image? image, EventHandler? onClick) : this(text, image, onClick, "")
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> class.
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
    protected ToolStripItem(string text, Drawing.Image? image, EventHandler? onClick, string name) : this()
    {
        this.Name = name;
        this.Text = text;
        if (image != null && image.PixbufData != null)
            this.Image = image;

        if (onClick != null)
            Click += onClick;
    }

    public virtual ToolStripItemAlignment Alignment
    {
        get => _Alignment;
        set
        {
            _Alignment = value;
            var widget = GetWidget();
            if (value == ToolStripItemAlignment.Right)
            {
                if (widget is Gtk.MenuItem item)
                    item.RightJustified = true;

            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic size.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic size; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool AutoSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic tool tip.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic tool tip; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool AutoToolTip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public virtual Color BackColor
    {
        get => _BackColor;
        set
        {
            _BackColor = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public virtual Drawing.Image BackgroundImage
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the background image layout.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    public virtual ImageLayout BackgroundImageLayout
    {
        get;
        set;
    }

    /// <summary>
    /// Bounding rectangle of the element.
    /// </summary>
    /// <value>
    /// The bounds.
    /// </value>
    /// <remarks>
    /// This threw, so owner-draw code could not ask an item where it is. The item's position within
    /// its strip is not tracked here, so the bounds are reported at the origin with the item's size.
    /// </remarks>
    public Rectangle Bounds => new(Left, Top, Width, Height);

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
    public virtual bool Checked
    {
        // Checked and CheckState were independent auto-properties, so they never agreed.
        get => CheckState != CheckState.Unchecked;
        set => CheckState = value ? CheckState.Checked : CheckState.Unchecked;
    }

    /// <summary>
    /// Gets or sets the state of the check.
    /// </summary>
    /// <value>
    /// The state of the check.
    /// </value>
    public virtual CheckState CheckState
    {
        get;
        set;
    }

    /// <summary>
    /// Returns the element's children (on a control, this forwards to Controls)
    /// </summary>
    /// <value>
    /// The children.
    /// </value>
    /// <remarks>
    /// This threw, so a layout engine walking the element tree failed at the first item. A plain
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is a leaf - items that do host children
    /// (dropdowns) override this - so report an empty collection.
    /// </remarks>
    public virtual ArrangedElementCollection Children => ArrangedElementCollection.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is created.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if created; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Created
    {
        get;
        set;
    }

    /// <summary>
    /// DisplayRectangle is the client area of a container element.
    /// Could possibly disappear if we change control to keep an
    /// up-to-date copy of display rectangle in the property store.
    /// </summary>
    /// <value>
    /// The display rectangle.
    /// </value>
    /// <remarks>
    /// The area available for content: the bounds less the item's padding.
    /// </remarks>
    public Rectangle DisplayRectangle => new(
        Padding.Left,
        Padding.Top,
        Math.Max(0, Width - Padding.Horizontal),
        Math.Max(0, Height - Padding.Vertical));

    /// <summary>
    /// Gets or sets the display style.
    /// </summary>
    /// <value>
    /// The display style.
    /// </value>
    public virtual ToolStripItemDisplayStyle DisplayStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the drop down items.
    /// </summary>
    /// <value>
    /// The drop down items.
    /// </value>
    public virtual ToolStripItemCollection DropDownItems
    {
        get
        {
            return dropDownItems;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if enabled; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Enabled
    {
        get { return GetWidget().Sensitive; }
        set { GetWidget().Sensitive = value; }
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    public virtual Font Font
    {
        get
        {
            return _Font;
        }
        set
        {
            _Font = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the color of the fore.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    public virtual Color ForeColor
    {
        get { return _ForeColor; }
        set
        {
            _ForeColor = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has children.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance has children; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool HasChildren { get; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public virtual int Height
    {
        get => Size.Height;
        set => Size = new Size(Size.Width, value);
    }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public virtual Image? Image
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image align.
    /// </summary>
    /// <value>
    /// The image align.
    /// </value>
    public virtual ContentAlignment ImageAlign
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the image.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    public virtual int ImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image key.
    /// </summary>
    /// <value>
    /// The image key.
    /// </value>
    public virtual string ImageKey
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the image scaling.
    /// </summary>
    /// <value>
    /// The image scaling.
    /// </value>
    public virtual ToolStripItemImageScaling ImageScaling
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the image transparent.
    /// </summary>
    /// <value>
    /// The color of the image transparent.
    /// </value>
    public virtual Color ImageTransparentColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the IME mode.
    /// </summary>
    /// <value>
    /// The IME mode.
    /// </value>
    public virtual ImeMode ImeMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public virtual ToolStripItemCollection Items
    {
        get
        {
            return dropDownItems;
        }
    }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>
    /// The left.
    /// </value>
    public virtual int Left
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the menu item.
    /// </summary>
    /// <value>
    /// The menu item.
    /// </value>
    public virtual Gtk.MenuItem MenuItem
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the merge action.
    /// </summary>
    /// <value>
    /// The merge action.
    /// </value>
    public virtual MergeAction MergeAction
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the merge.
    /// </summary>
    /// <value>
    /// The index of the merge.
    /// </value>
    public virtual int MergeIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public virtual string Name
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the overflow.
    /// </summary>
    /// <value>
    /// The overflow.
    /// </value>
    public virtual ToolStripItemOverflow Overflow
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the owner.
    /// </summary>
    /// <value>
    /// The owner.
    /// </value>
    public virtual ToolStrip Owner
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets the owner item.
    /// </summary>
    /// <value>
    /// The owner item.
    /// </value>
    public virtual ToolStripItem OwnerItem { get; } = null!;

    //public override Size MaximumSize { get; set; }
    //public override Size MinimumSize { get; set; }
    /// <summary>
    /// Gets or sets the space between this item and its neighbours.
    /// </summary>
    /// <remarks>
    /// The WinForms Margin member was commented out, so ordinary code setting an item's margin did not
    /// compile against this shim. The default matches WinForms.
    /// </remarks>
    public virtual Padding Margin
    {
        get;
        set;
    } = new Padding(0, 1, 0, 2);

    /// <summary>
    /// Gets or sets the padding inside this item.
    /// </summary>
    public virtual Padding Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the parent.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    public virtual ToolStripItem? Parent
    {
        get;
        set;
    }

    /// <summary>
    /// True if the element is currently visible (some layouts, like
    /// flow, need to skip non-visible elements.)
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if participates in layout; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    /// <remarks>
    /// This threw, so flow-style layouts could not ask whether to skip an item. An item takes part in
    /// layout exactly when it is visible.
    /// </remarks>
    public bool ParticipatesInLayout
    {
        get
        {
            try
            {
                return Visible;
            }
            catch (NullReferenceException)
            {
                // Visible dereferences the backing GTK widget, which is null for a bare ToolStripItem
                // that has not been added to a strip. An item with no widget cannot participate.
                return false;
            }
        }
    }

    /// <summary>
    /// Gets the placement.
    /// </summary>
    /// <value>
    /// The placement.
    /// </value>
    public virtual ToolStripItemPlacement Placement { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> is pressed.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if pressed; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Pressed { get; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    /// <value>
    /// The region.
    /// </value>
    public virtual System.Drawing.Region Region
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets the right.
    /// </summary>
    /// <value>
    /// The right.
    /// </value>
    public virtual int Right { get; }

    /// <summary>
    /// Gets or sets the right to left.
    /// </summary>
    /// <value>
    /// The right to left.
    /// </value>
    public virtual RightToLeft RightToLeft
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether right to left automatic mirror image.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if right to left automatic mirror image; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    // public virtual bool Selected { get; }
    public virtual bool RightToLeftAutoMirrorImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public virtual Size Size
    {
        get => _size;
        set
        {
            _size = value;
            var widget = GetWidget();
            //widget.SetSizeRequest(value.Width, value.Height);
            widget.SetSizeRequest(value.Width, -1);
        }
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    public virtual object Tag
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public virtual string Text
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the text align.
    /// </summary>
    /// <value>
    /// The text align.
    /// </value>
    public virtual ContentAlignment TextAlign
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text direction.
    /// </summary>
    /// <value>
    /// The text direction.
    /// </value>
    public virtual ToolStripTextDirection TextDirection
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text image relation.
    /// </summary>
    /// <value>
    /// The text image relation.
    /// </value>
    public virtual TextImageRelation TextImageRelation
    {
        get => textImageRelation;
        set
        {
            textImageRelation = value;
            var widget = GetWidget();
            if (widget?.Parent is Gtk.Toolbar toolbar)
            {
                if (value == TextImageRelation.ImageAboveText || value == TextImageRelation.TextAboveImage)
                {
                    toolbar.ToolbarStyle = Gtk.ToolbarStyle.Both;
                    toolbar.IconSize = Gtk.IconSize.Dialog;
                }
                else if (value == TextImageRelation.ImageBeforeText || value == TextImageRelation.TextBeforeImage)
                {
                    toolbar.ToolbarStyle = Gtk.ToolbarStyle.BothHoriz;
                    toolbar.IconSize = Gtk.IconSize.SmallToolbar;
                }
                else
                {
                    toolbar.ToolbarStyle = Gtk.ToolbarStyle.BothHoriz;
                    toolbar.IconSize = Gtk.IconSize.SmallToolbar;
                }
            }
            else if (widget?.Parent is Gtk.MenuBar menubar)
            {
                if (value == TextImageRelation.ImageAboveText)
                {
                    widget.Halign = Gtk.Align.Center;
                    menubar.ChildPackDirection = Gtk.PackDirection.Ttb;
                }
                else if (value == TextImageRelation.TextAboveImage)
                {
                    widget.Halign = Gtk.Align.Center;
                    menubar.ChildPackDirection = Gtk.PackDirection.Ttb;
                }
                else if (value == TextImageRelation.ImageBeforeText || value == TextImageRelation.TextBeforeImage)
                {
                    widget.Halign = Gtk.Align.Start;
                    menubar.ChildPackDirection = Gtk.PackDirection.Ltr;
                }
                else
                {
                    widget.Halign = Gtk.Align.Start;
                    menubar.ChildPackDirection = Gtk.PackDirection.Ltr;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    /// <value>
    /// The tool tip text.
    /// </value>
    public virtual string ToolTipText
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    public virtual int Top
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the unique key.
    /// </summary>
    /// <value>
    /// The unique key.
    /// </value>
    public virtual string unique_key
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use wait cursor.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use wait cursor; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool UseWaitCursor
    {
        get;
        set;
    }

    public virtual bool Visible
    {
        get { return GetWidget().Visible; }
        set
        {
            var widget = GetWidget();
            widget.NoShowAll = value == false;
            if (value == true) { widget.ShowAll(); } else { widget.Visible = value; }
        }
    }

    /// <summary>
    /// Gets the widget.
    /// </summary>
    /// <value>
    /// The widget.
    /// </value>
    public virtual IToolMenuItem Widget
    {
        get;
        protected set;
    } = null!;

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public virtual int Width
    {
        // Width and Height were auto-properties disconnected from Size, so setting one left the
        // other stale.
        get => Size.Width;
        set => Size = new Size(value, Size.Height);
    }

    // Both threw, so no layout engine could walk upwards from an item or read a layout property off it.
    // The container is the owning item when there is one, otherwise the owning strip.
    IArrangedElement? IArrangedElement.Container => (IArrangedElement?)Parent ?? OwnerItem ?? (IArrangedElement?)Owner;

    PropertyStore IArrangedElement.Properties => _properties ??= new PropertyStore();

    /// <summary>
    /// Backing store for layout properties read and written through
    /// <see cref="T:System.Windows.Forms.Layout.IArrangedElement" />.
    /// </summary>
    private PropertyStore? _properties;

    /// <summary>
    /// Creates the control.
    /// </summary>
    public virtual void CreateControl() { }

    /// <summary>
    /// Query element for its preferred size.  There is no guarantee
    /// that layout engine will assign the element the returned size.
    /// ProposedSize is a hint about constraints.
    /// </summary>
    /// <param name="proposedSize">
    /// Size of the proposed.
    /// </param>
    /// <returns>
    /// Size.
    /// </returns>
    public Size GetPreferredSize(Size proposedSize)
    {
        // This threw, so any layout code asking an item how big it wants to be failed. Report the
        // item's own size, bounded by whatever the caller proposed.
        var width = proposedSize.Width > 0 ? Math.Min(Width, proposedSize.Width) : Width;
        var height = proposedSize.Height > 0 ? Math.Min(Height, proposedSize.Height) : Height;

        return new Size(width, height);
    }

    public Gtk.Widget GetWidget()
    {
        // Widget is null for a bare ToolStripItem (concrete items override it); the parameterless
        // constructor null-checks the result at runtime, so forgive the nullable expression here rather
        // than widening the return type (which would ripple null onto every concrete-item caller).
        return (this.Widget?.ToolItem ?? this.Widget?.MenuItem)!;
    }

    public virtual void PerformClick()
    {
        OnClick(EventArgs.Empty);
    }

    /// <summary>
    /// Performs the layout.
    /// </summary>
    public virtual void PerformLayout()
    {

    }

    /// <summary>
    /// Performs the layout.
    /// </summary>
    /// <param name="affectedControl">
    /// The affected control.
    /// </param>
    /// <param name="affectedProperty">
    /// The affected property.
    /// </param>
    public virtual void PerformLayout(Control affectedControl, string affectedProperty)
    {

    }

    /// <summary>
    /// Resumes the layout.
    /// </summary>
    public virtual void ResumeLayout()
    {

    }

    /// <summary>
    /// Resumes the layout.
    /// </summary>
    /// <param name="performLayout">
    /// if set to
    /// <c>
    /// true
    /// </c>perform layout.
    /// </param>
    public virtual void ResumeLayout(bool performLayout)
    {

    }

    /// <summary>
    /// Sets the bounds for an element. Implementors should call
    /// CommonProperties.SetSpecifiedBounds. See Control.SetBoundsCore.
    /// </summary>
    /// <param name="bounds">
    /// The bounds.
    /// </param>
    /// <param name="specified">
    /// The specified.
    /// </param>
    public void SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        // This threw outright; apply the requested parts instead.
        if (specified.HasFlag(BoundsSpecified.Size) || specified.HasFlag(BoundsSpecified.All))
        {
            Size = new Size(bounds.Width, bounds.Height);
        }
        else
        {
            if (specified.HasFlag(BoundsSpecified.Width))
            {
                Width = bounds.Width;
            }

            if (specified.HasFlag(BoundsSpecified.Height))
            {
                Height = bounds.Height;
            }
        }
    }

    /// <summary>
    /// Suspends the layout.
    /// </summary>
    public virtual void SuspendLayout()
    {

    }

    void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string? propertyName)
    {
        // This threw. An item has no layout of its own to run; the enclosing strip owns arrangement, so
        // forward the request to it when there is one and otherwise do nothing.
        if (Owner is IArrangedElement owner && !ReferenceEquals(owner, this))
        {
            owner.PerformLayout(affectedElement, propertyName);
        }
    }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        try
        {
            GetWidget()?.Dispose();
            Image?.Dispose();
            BackgroundImage?.Dispose();
            GC.SuppressFinalize(this);
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Dispose", ex);
        }
        base.Dispose(disposing);
    }

    protected virtual void InitStyle(Widget? widget)
    {
        if (widget != null)
        {
            SetStyle(widget);
        }
    }

    /// <summary>
    /// Raises the AnchorChanged event.
    /// </summary>
    protected virtual void OnAnchorChanged(System.EventArgs e)
    {
        AnchorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ButtonClick event.
    /// </summary>
    protected virtual void OnButtonClick(System.EventArgs e)
    {
        ButtonClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ButtonDoubleClick event.
    /// </summary>
    protected virtual void OnButtonDoubleClick(System.EventArgs e)
    {
        ButtonDoubleClick?.Invoke(this, e);
    }

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

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>Raises the <see cref="DragEnter" /> event.</summary>
    /// <param name="e">The drag event data.</param>
    protected virtual void OnDragEnter(DragEventArgs e)
    {
        DragEnter?.Invoke(this, e);
    }

    /// <summary>Raises the <see cref="DragOver" /> event.</summary>
    /// <param name="e">The drag event data.</param>
    protected virtual void OnDragOver(DragEventArgs e)
    {
        DragOver?.Invoke(this, e);
    }

    /// <summary>Raises the <see cref="DragDrop" /> event.</summary>
    /// <param name="e">The drag event data.</param>
    protected virtual void OnDragDrop(DragEventArgs e)
    {
        DragDrop?.Invoke(this, e);
    }

    /// <summary>Raises the <see cref="DragLeave" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnDragLeave(EventArgs e)
    {
        DragLeave?.Invoke(this, e);
    }

    void IDropTarget.OnDragEnter(DragEventArgs e) => OnDragEnter(e);

    void IDropTarget.OnDragOver(DragEventArgs e) => OnDragOver(e);

    void IDropTarget.OnDragDrop(DragEventArgs e) => OnDragDrop(e);

    void IDropTarget.OnDragLeave(EventArgs e) => OnDragLeave(e);

    /// <summary>
    /// Raises the ContextMenuStripChanged event.
    /// </summary>
    protected virtual void OnContextMenuStripChanged(System.EventArgs e)
    {
        ContextMenuStripChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DockChanged event.
    /// </summary>
    protected virtual void OnDockChanged(System.EventArgs e)
    {
        DockChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DoubleClick event.
    /// </summary>
    protected virtual void OnDoubleClick(System.EventArgs e)
    {
        DoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownClosed event.
    /// </summary>
    protected virtual void OnDropDownClosed(System.EventArgs e)
    {
        DropDownClosed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownItemClicked event.
    /// </summary>
    protected virtual void OnDropDownItemClicked(System.Windows.Forms.ToolStripItemClickedEventArgs e)
    {
        DropDownItemClicked?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownOpened event.
    /// </summary>
    protected virtual void OnDropDownOpened(System.EventArgs e)
    {
        DropDownOpened?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownOpening event.
    /// </summary>
    protected virtual void OnDropDownOpening(System.EventArgs e)
    {
        DropDownOpening?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the EnabledChanged event.
    /// </summary>
    protected virtual void OnEnabledChanged(System.EventArgs e)
    {
        EnabledChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Enter event.
    /// </summary>
    protected virtual void OnEnter(System.EventArgs e)
    {
        Enter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the KeyDown event.
    /// </summary>
    protected virtual void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
        KeyDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the KeyPress event.
    /// </summary>
    protected virtual void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
    {
        KeyPress?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the KeyUp event.
    /// </summary>
    protected virtual void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
    {
        KeyUp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Layout event.
    /// </summary>
    protected virtual void OnLayout(System.Windows.Forms.LayoutEventArgs e)
    {
        Layout?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Leave event.
    /// </summary>
    protected virtual void OnLeave(System.EventArgs e)
    {
        Leave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the LocationChanged event.
    /// </summary>
    protected virtual void OnLocationChanged(System.EventArgs e)
    {
        LocationChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the LostFocus event.
    /// </summary>
    protected virtual void OnLostFocus(System.EventArgs e)
    {
        LostFocus?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseClick event.
    /// </summary>
    protected virtual void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
    {
        MouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseDoubleClick event.
    /// </summary>
    protected virtual void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
    {
        MouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseDown event.
    /// </summary>
    protected virtual void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
        MouseDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseEnter event.
    /// </summary>
    protected virtual void OnMouseEnter(System.EventArgs e)
    {
        MouseEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseHover event.
    /// </summary>
    protected virtual void OnMouseHover(System.EventArgs e)
    {
        MouseHover?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseLeave event.
    /// </summary>
    protected virtual void OnMouseLeave(System.EventArgs e)
    {
        MouseLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseMove event.
    /// </summary>
    protected virtual void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseUp event.
    /// </summary>
    protected virtual void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
        MouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseWheel event.
    /// </summary>
    protected virtual void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
    {
        MouseWheel?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Move event.
    /// </summary>
    protected virtual void OnMove(System.EventArgs e)
    {
        Move?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Paint event.
    /// </summary>
    protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        Paint?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RegionChanged event.
    /// </summary>
    protected virtual void OnRegionChanged(System.EventArgs e)
    {
        RegionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Resize event.
    /// </summary>
    protected virtual void OnResize(System.EventArgs e)
    {
        Resize?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SizeChanged event.
    /// </summary>
    protected virtual void OnSizeChanged(System.EventArgs e)
    {
        SizeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the TextChanged event.
    /// </summary>
    protected virtual void OnTextChanged(System.EventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the VisibleChanged event.
    /// </summary>
    protected virtual void OnVisibleChanged(System.EventArgs e)
    {
        VisibleChanged?.Invoke(this, e);
    }

    protected virtual void SetStyle(Gtk.Widget widget)
    {
        var style = new StringBuilder();

        if (this._BackColor.Name != "0")
        {
            var backColor = this._BackColor;
            var color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
            style.AppendFormat("background-color:{0};background:{0};", color);
        }

        if (this._ForeColor.Name != "0")
        {
            var foreColor = this._ForeColor;
            var color = $"rgba({foreColor.R},{foreColor.G},{foreColor.B},{foreColor.A})";
            style.AppendFormat("color:{0};", color);
        }

        if (this._Font != null)
        {
            var font = this._Font;
            if (font.Unit == GraphicsUnit.Pixel)
                style.AppendFormat("font-size:{0}px;", font.Size);
            else if (font.Unit == GraphicsUnit.Inch)
                style.AppendFormat("font-size:{0}in;", font.Size);
            else if (font.Unit == GraphicsUnit.Point)
                style.AppendFormat("font-size:{0}pt;", font.Size);
            else if (font.Unit == GraphicsUnit.Millimeter)
                style.AppendFormat("font-size:{0}mm;", font.Size);
            else if (font.Unit == GraphicsUnit.Document)
                style.AppendFormat("font-size:{0}cm;", font.Size);
            else if (font.Unit == GraphicsUnit.Display)
                style.AppendFormat("font-size:{0}pc;", font.Size);
            else
                style.AppendFormat("font-size:{0}pt;", font.Size);

            if (string.IsNullOrWhiteSpace(font.FontFamily?.Name) == false)
            {
                style.AppendFormat("font-family:\"{0}\";", font.FontFamily?.Name);
            }
            if (font.Bold)
            {
                style.Append("font-weight:bold;");
            }
            if (font.Italic)
            {
                style.Append("font-style:italic;");
            }
            if (font.Underline)
            {
                style.Append("text-decoration:underline;");
            }
            if (font.Strikeout)
            {
                style.Append("text-decoration:line-through;");
            }
        }
        if (style.Length > 10)
        {
            var styleClassName = $"s{unique_key}";
            var css = new StringBuilder();
            css.AppendLine($".{styleClassName}{{{style.ToString()}}}");
            css.AppendLine($".{styleClassName}>button{{{style.ToString()}}}");
            css.AppendLine($".{styleClassName}>checkbutton{{{style.ToString()}}}");

            if (provider.LoadFromData(css.ToString()))
            {
                if (widget.StyleContext.HasClass(styleClassName))
                {
                    widget.StyleContext.RemoveClass(styleClassName);
                    Gtk.StyleContext.RemoveProviderForScreen(Gdk.Screen.Default, provider);
                }
                Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, provider, StyleProviderPriority.User);
                widget.StyleContext.AddClass(styleClassName);
            }
        }
    }

    protected virtual void UpdateStyle()
    {
        var widget = GetWidget();
        GLib.Timeout.Add(1, () =>
        {
            if (widget != null && widget.IsMapped)
                SetStyle(widget);
            return false;
        });
    }

    private void _widget_ButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
    {
        var widget = (Gtk.Widget)o;
        MouseButtons button = MouseButtons.None;
        if (args.Event.Button == 1)
            button = MouseButtons.Left;
        else if (args.Event.Button == 2)
            button = MouseButtons.Middle;
        else if (args.Event.Button == 3)
            button = MouseButtons.Right;

        var mouseArgs = new MouseEventArgs(button, 1, (int)args.Event.X, (int)args.Event.X, 0);
        OnMouseDown(mouseArgs);
        isDoubleClick = false;
        if (args.Event.Button == 1)
        {
            if (args.Event.Type == Gdk.EventType.TwoButtonPress || args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                isDoubleClick = true;
                OnDoubleClick(new EventArgs());
                OnButtonDoubleClick(new EventArgs());
            }
        }
    }

    private void _widget_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
    {
        if (!isDoubleClick)
        {
            var widget = (Gtk.Widget)o;
            MouseButtons button = MouseButtons.None;
            if (args.Event.Button == 1)
                button = MouseButtons.Left;
            else if (args.Event.Button == 2)
                button = MouseButtons.Middle;
            else if (args.Event.Button == 3)
                button = MouseButtons.Right;

            var mouseArgs3 = new MouseEventArgs(button, 1, (int)args.Event.X, (int)args.Event.X, 0);
            OnMouseClick(mouseArgs3);
            OnClick(new EventArgs());
            OnDropDownItemClicked(new ToolStripItemClickedEventArgs(this));
            OnMouseUp(mouseArgs3);
        }
    }

    private void _widget_Realized(object? sender, EventArgs e)
    {
        if (!_widgetRealized)
        {
            _widgetRealized = true;
            InitStyle((Gtk.Widget?)sender);
        }
    }

    private void Widget_Clicked(object? sender, EventArgs e)
    {
        OnButtonClick(new EventArgs());
    }

    private void Widget3_Activated(object? sender, EventArgs e)
    {
        OnDropDownOpened(EventArgs.Empty);

        if (Checked == true)
        {
            if (CheckState == CheckState.Checked)
                CheckState = CheckState.Unchecked;
            else
                CheckState = CheckState.Checked;

            OnCheckedChanged(EventArgs.Empty);
            OnCheckStateChanged(EventArgs.Empty);
        }
    }

    private void Widget3_Deselected(object? sender, EventArgs e)
    {
        OnDropDownClosed(EventArgs.Empty);
    }

    private void Widget3_Selected(object? sender, EventArgs e)
    {
        OnDropDownOpening(EventArgs.Empty);
    }
}

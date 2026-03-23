using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
/// Represents the abstract base class that manages events and layout for all the elements that a
/// <see cref="T:System.Windows.Forms.ToolStrip" /> or
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> can contain.
/// </summary>
public class ToolStripItem : Component, IDropTarget, ISupportOleDropSource, IArrangedElement, IComponent, IDisposable, IKeyboardToolTip
{

    internal Gtk.Image DefaultImage = new Gtk.Image("image-missing", Gtk.IconSize.Menu);

    private readonly ToolStripItemCollection dropDownItems;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public virtual event EventHandler CheckedChanged;

    /// <summary>
    /// Occurs when check state changed.
    /// </summary>
    public virtual event EventHandler CheckStateChanged;

    /// <summary>
    /// Occurs when click.
    /// </summary>
    public virtual event EventHandler Click;

    /// <summary>
    /// Occurs when drop down item clicked.
    /// </summary>
    public virtual event ToolStripItemClickedEventHandler DropDownItemClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItem" /> class.
    /// </summary>
    public ToolStripItem()
    {
        dropDownItems = new ToolStripItemCollection(this);
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
    protected ToolStripItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
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
    protected ToolStripItem(string text, Image image, EventHandler? onClick, string name) : this()
    {
        this.Name = name;
        this.Text = text;
        if (image != null && image.PixbufData != null)
            this.Image = image;

        if (onClick != null)
            Click += onClick;
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
    //public virtual Size Size { get; set; }
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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public virtual Image BackgroundImage
    {
        get;
        set;
    }

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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Rectangle Bounds => throw new NotImplementedException();

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
        get;
        set;
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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public ArrangedElementCollection Children => throw new NotImplementedException();

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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Rectangle DisplayRectangle => throw new NotImplementedException();

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
    //public virtual event EventHandler Disposed;
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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    //  public virtual bool Focused { get { return this.IsFocus; } }
    public virtual Font Font
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the fore.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    public virtual Color ForeColor
    {
        get;
        set;
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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public virtual System.Drawing.Image Image
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
    }

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
    }

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
    }

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
    }

    /// <summary>
    /// Gets the owner item.
    /// </summary>
    /// <value>
    /// The owner item.
    /// </value>
    public virtual ToolStripItem OwnerItem { get; }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    //public override Padding Margin { get; set; }
    //public override Size MaximumSize { get; set; }
    //public override Size MinimumSize { get; set; }
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
    public virtual ToolStripItem Parent
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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public bool ParticipatesInLayout => throw new NotImplementedException();

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
    }

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
    //public virtual ISite Site { get; set; }
    public virtual Size Size
    {
        get;
        set;
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
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    //public virtual string Text { get { return base.Label; } set { base.Label = value; } }
    public virtual string Text
    {
        get;
        set;
    }

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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    /// <value>
    /// The tool tip text.
    /// </value>
    //public virtual bool Enabled { get; set; }
    public virtual string ToolTipText
    {
        get;
        set;
    }

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

    /// <summary>
    /// Gets the widget.
    /// </summary>
    /// <value>
    /// The widget.
    /// </value>
    public virtual Gtk.Widget Widget { get; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public virtual int Width
    {
        get;
        set;
    }

    IArrangedElement IArrangedElement.Container => throw new NotImplementedException();

    PropertyStore IArrangedElement.Properties => throw new NotImplementedException();

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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public Size GetPreferredSize(Size proposedSize)
    {
        throw new NotImplementedException();
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
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public void SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Suspends the layout.
    /// </summary>
    public virtual void SuspendLayout()
    {

    }

    void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string propertyName)
    {
        throw new NotImplementedException();
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

    /// <summary>
    /// Raises the Click event.
    /// </summary>
    protected virtual void OnClick(System.EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownItemClicked event.
    /// </summary>
    protected virtual void OnDropDownItemClicked(System.Windows.Forms.ToolStripItemClickedEventArgs e)
    {
        DropDownItemClicked?.Invoke(this, e);
    }
}
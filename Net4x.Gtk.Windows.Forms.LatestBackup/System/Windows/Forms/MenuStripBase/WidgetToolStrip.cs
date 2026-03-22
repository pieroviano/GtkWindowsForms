/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Layout;
using GLib;

namespace System.Windows.Forms;

/// <summary>
/// Class WidgetToolStrip. Implements the
/// <see cref="T:System.Windows.Forms.ToolStripItem" /> Implements the
/// <see cref="T:System.Windows.Forms.IDropTarget" /> Implements the
/// <see cref="T:System.Windows.Forms.ISupportOleDropSource" /> Implements the
/// <see cref="T:System.Windows.Forms.Layout.IArrangedElement" /> Implements the
/// <see cref="T:System.ComponentModel.IComponent" /> Implements the
/// <see cref="T:System.IDisposable" /> Implements the
/// <see cref="T:System.Windows.Forms.IKeyboardToolTip" />
/// </summary>
/// <typeparam name="T">
/// </typeparam>
/// <seealso cref="T:System.Windows.Forms.ToolStripItem" />
/// <seealso cref="T:System.Windows.Forms.IDropTarget" />
/// <seealso cref="T:System.Windows.Forms.ISupportOleDropSource" />
/// <seealso cref="T:System.Windows.Forms.Layout.IArrangedElement" />
/// <seealso cref="T:System.ComponentModel.IComponent" />
/// <seealso cref="T:System.IDisposable" />
/// <seealso cref="T:System.Windows.Forms.IKeyboardToolTip" />
public class WidgetToolStrip<T> : ToolStripItem, IDropTarget, ISupportOleDropSource, IArrangedElement, IComponent, IDisposable, IKeyboardToolTip
{

    internal Gtk.Button button = new Gtk.Button();

    internal Gtk.ComboBoxText comboBox = new Gtk.ComboBoxText() { HasFrame = false };

    internal Gtk.Entry entry = new Gtk.Entry();

    internal Gtk.Viewport flagBox = new Gtk.Viewport();

    internal Gtk.RadioButton groupradio = new Gtk.RadioButton("");

    internal Gtk.Box itemBox = new Gtk.Box(Gtk.Orientation.Horizontal, 0);

    internal Gtk.Label label = new Gtk.Label();

    // internal Gtk.ProgressBar progressBar = new Gtk.ProgressBar();

    internal Gtk.LevelBar progressBar = new Gtk.LevelBar();

    readonly Gtk.CssProvider provider = new Gtk.CssProvider();

    private Color _BackColor;

    private Font _Font;

    private Color _ForeColor;

    /// <summary>
    /// Occurs when checked changed.
    /// </summary>
    public override event EventHandler CheckedChanged;

    /// <summary>
    /// Occurs when check state changed.
    /// </summary>
    public override event EventHandler CheckStateChanged;

    /// <summary>
    /// Occurs when click.
    /// </summary>
    public override event EventHandler Click;

    /// <summary>
    /// Occurs when drop down item clicked.
    /// </summary>
    public override event ToolStripItemClickedEventHandler DropDownItemClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> class.
    /// </summary>
    public WidgetToolStrip() : this(null, "", null, null, "", null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> class.
    /// </summary>
    /// <param name="stripType">
    /// Type of the strip.
    /// </param>
    /// <param name="args">
    /// The arguments.
    /// </param>
    public WidgetToolStrip(string stripType, params object[] args) : this(stripType, "", null, null, "", args)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> class.
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
    public WidgetToolStrip(string text, System.Drawing.Image image, EventHandler onClick) : this(null, text, image, onClick, "")
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> class.
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
    public WidgetToolStrip(string text, System.Drawing.Image image, EventHandler onClick, string name) : this(null, text, image, onClick, "")
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> class.
    /// </summary>
    /// <param name="stripType">
    /// Type of the strip.
    /// </param>
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
    /// <param name="args">
    /// The arguments.
    /// </param>
    protected WidgetToolStrip(string stripType, string text, System.Drawing.Image image, EventHandler onClick, string name, params object[] args) : base(text, image, onClick, name)
    {
        this.unique_key = Guid.NewGuid().ToString().ToLower();
        MenuItem = (Gtk.MenuItem)Activator.CreateInstance(typeof(T), args);
        MenuItem.StyleContext.AddProvider(provider, 900);
        if (stripType == "ToolStripSeparator")
        {
            Created = true;
        }
        else
        {
            this.StripType = stripType;
            this.MenuItem.Realized += ToolStripItem_Realized;
            this.MenuItem.Activated += MenuItem_Activated;
            this.MenuItem.ButtonReleaseEvent += MenuItem_ButtonReleaseEvent;
            this.MenuItem.Valign = Gtk.Align.Center;
            this.MenuItem.Halign = Gtk.Align.Fill;
            this.MenuItem.Vexpand = false;
            this.MenuItem.Hexpand = true;
            itemBox.Valign = Gtk.Align.Center;
            itemBox.Halign = Gtk.Align.Start;
            flagBox.BorderWidth = 0;
            flagBox.ShadowType = Gtk.ShadowType.None;
            flagBox.Hexpand = false;
            flagBox.Vexpand = false;
            if (stripType == "ToolStripDropDownItem")
            {
                button.Image = Gtk.Image.NewFromIconName("pan-down", Gtk.IconSize.Button);
                button.ImagePosition = Gtk.PositionType.Right;
                button.Relief = Gtk.ReliefStyle.None;
                button.AlwaysShowImage = true;
                button.Halign = Gtk.Align.Start;
                button.Valign = Gtk.Align.Center;
                button.Hexpand = false;
                button.Vexpand = false;
                itemBox.PackStart(flagBox, false, false, 0);
                itemBox.PackStart(button, false, false, 0);
                this.MenuItem.Add(itemBox);
            }
            else if (stripType == "ToolStripTextBox")
            {
                entry.HasFrame = false;
                entry.MaxWidthChars = 1;
                entry.WidthChars = 0;
                entry.Valign = Gtk.Align.Fill;
                entry.Halign = Gtk.Align.Fill;
                entry.IsFocus = true;
                this.MenuItem.Add(entry);
            }
            else if (stripType == "ToolStripComboBox")
            {
                this.MenuItem.Add(comboBox);
            }
            else if (stripType == "ToolStripProgressBar")
            {
                progressBar.Halign = Gtk.Align.Fill;
                progressBar.Valign = Gtk.Align.Fill;
                progressBar.Visible = true;
                this.MenuItem.Add(progressBar);
            }
            else
            {
                itemBox.PackStart(flagBox, false, false, 0);
                itemBox.PackStart(label, false, false, 0);
                this.MenuItem.Add(itemBox);
            }
        }
        Created = true;

    }

    /// <summary>
    /// Gets or sets the alignment.
    /// </summary>
    /// <value>
    /// The alignment.
    /// </value>
    public ToolStripItemAlignment Alignment
    {
        get => MenuItem.Halign == Gtk.Align.End ? ToolStripItemAlignment.Right : ToolStripItemAlignment.Left;
        set { MenuItem.Hexpand = true; MenuItem.Halign = Gtk.Align.End; MenuItem.RightJustified = value == ToolStripItemAlignment.Right; }
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
    public override bool AutoSize
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
    public override bool AutoToolTip
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
    public override Color BackColor
    {
        get => _BackColor;
        set { _BackColor = value; UpdateStyle(); }
    }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public override System.Drawing.Image BackgroundImage
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
    public override ImageLayout BackgroundImageLayout
    {
        get;
        set;
    }

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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the state of the check.
    /// </summary>
    /// <value>
    /// The state of the check.
    /// </value>
    public override CheckState CheckState
    {
        get;
        set;
    } = CheckState.Unchecked;

    /// <summary>
    /// Gets or sets the display style.
    /// </summary>
    /// <value>
    /// The display style.
    /// </value>
    public override ToolStripItemDisplayStyle DisplayStyle
    {
        get;
        set;
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
    public override bool Enabled
    {
        get { return this.Widget.Sensitive; }
        set { this.Widget.Sensitive = value; }
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    public override Font Font
    {
        get
        {
            if (_Font == null)
            {
                var fontdes = Widget.PangoContext.FontDescription;
                var size = Convert.ToInt32(fontdes.Size / Pango.Scale.PangoScale);
                return new Drawing.Font(new Drawing.FontFamily(fontdes.Family), size);
            }
            else
                return _Font;
        }
        set { _Font = value; UpdateStyle(); }
    }

    /// <summary>
    /// Gets or sets the color of the fore.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    public override Color ForeColor
    {
        get => _ForeColor;
        set { _ForeColor = value; UpdateStyle(); }
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
    public override bool HasChildren { get; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public override int Height
    {
        get { return this.Widget.HeightRequest; }
        set { this.Widget.HeightRequest = value; }
    }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public override System.Drawing.Image Image
    {
        get => base.Image;
        set
        {
            base.Image = value;
        }
    }

    /// <summary>
    /// Gets or sets the image align.
    /// </summary>
    /// <value>
    /// The image align.
    /// </value>
    public override ContentAlignment ImageAlign
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
    public override int ImageIndex
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
    public override string ImageKey
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
    public override ToolStripItemImageScaling ImageScaling
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
    public override Color ImageTransparentColor
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
    public override ImeMode ImeMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>
    /// The left.
    /// </value>
    public override int Left
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
    public override Gtk.MenuItem MenuItem
    {
        get => base.MenuItem;
        set { base.MenuItem = value; }
    }

    /// <summary>
    /// Gets or sets the merge action.
    /// </summary>
    /// <value>
    /// The merge action.
    /// </value>
    public override MergeAction MergeAction
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
    public override int MergeIndex
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
    public override ToolStripItemOverflow Overflow
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
    public override ToolStrip Owner
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
    public override ToolStripItem OwnerItem { get; }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    /// <value>
    /// The padding.
    /// </value>
    //public override Padding Margin { get; set; }
    //public override Size MaximumSize { get; set; }
    //public override Size MinimumSize { get; set; }
    public override Padding Padding
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
    public override ToolStripItem Parent
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the placement.
    /// </summary>
    /// <value>
    /// The placement.
    /// </value>
    public override ToolStripItemPlacement Placement { get; }

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
    public override bool Pressed { get; }

    /// <summary>
    /// Gets or sets the region.
    /// </summary>
    /// <value>
    /// The region.
    /// </value>
    public override System.Drawing.Region Region
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
    public override int Right { get; }

    /// <summary>
    /// Gets or sets the right to left.
    /// </summary>
    /// <value>
    /// The right to left.
    /// </value>
    public override RightToLeft RightToLeft
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
    public override bool RightToLeftAutoMirrorImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.ComponentModel.ISite">
    /// </see> of the
    /// <see cref="T:System.ComponentModel.Component">
    /// </see>.
    /// </summary>
    /// <value>
    /// The site.
    /// </value>
    public override ISite Site
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
    public override Size Size
    {
        get
        {
            return new Size(this.Widget.WidthRequest, this.Widget.HeightRequest);
        }
        set
        {
            this.Widget.SetSizeRequest(value.Width, value.Height);
        }
    }

    /// <summary>
    /// Gets or sets the type of the strip.
    /// </summary>
    /// <value>
    /// The type of the strip.
    /// </value>
    public string StripType
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
    public override object Tag
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
    public override string Text
    {
        get
        {
            if (this.StripType == "ToolStripTextBox")
            {
                return this.entry.Text;
            }
            else if (this.StripType == "ToolStripDropDownItem")
            {
                return this.button.Label;
            }
            else if (this.StripType == "ToolStripComboBox")
            {
                return this.comboBox.ActiveText;
            }

            return this.label.Text;
        }
        set { this.label.Text = value; this.button.Label = value; }
    }

    /// <summary>
    /// Gets or sets the text align.
    /// </summary>
    /// <value>
    /// The text align.
    /// </value>
    public override ContentAlignment TextAlign
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
    public override ToolStripTextDirection TextDirection
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
    public override TextImageRelation TextImageRelation
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
    public override string ToolTipText
    {
        get { return this.Widget.TooltipText; }
        set { this.Widget.TooltipText = value; }
    }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    public override int Top
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
    public override string unique_key
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
    public override bool UseWaitCursor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.WidgetToolStrip`1" /> is visible.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if visible; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public virtual bool Visible
    {
        get { return this.Widget.Visible; }
        set { this.Widget.Visible = value; this.Widget.NoShowAll = value == false; }
    }

    /// <summary>
    /// Gets the widget.
    /// </summary>
    /// <value>
    /// The widget.
    /// </value>
    public override Gtk.Widget Widget => MenuItem;

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public override int Width
    {
        get { return this.Widget.WidthRequest; }
        set { this.Widget.WidthRequest = value; }
    }

    internal void UpdateStyle()
    {
        if (this.Widget.IsMapped)
            SetStyle(this.Widget);
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected override void OnCheckedChanged(EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected override void OnCheckStateChanged(EventArgs e)
    {
        CheckStateChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Click event.
    /// </summary>
    protected override void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DropDownItemClicked event.
    /// </summary>
    protected override void OnDropDownItemClicked(System.Windows.Forms.ToolStripItemClickedEventArgs e)
    {
        DropDownItemClicked?.Invoke(this, e);
    }

    /// <summary>
    /// Sets the style.
    /// </summary>
    /// <param name="widget">
    /// The widget.
    /// </param>
    protected virtual void SetStyle(Gtk.Widget widget)
    {

        var style = new StringBuilder();
        if (this.BackColor.Name != "0")
        {
            var color = $"rgba({this.BackColor.R},{this.BackColor.G},{this.BackColor.B},{this.BackColor.A})";
            style.AppendFormat("background-color:{0};background:{0};", color);
        }
        if (this.ForeColor.Name != "0")
        {
            var color = $"rgba({this.ForeColor.R},{this.ForeColor.G},{this.ForeColor.B},{this.ForeColor.A})";
            style.AppendFormat("color:{0};", color);
        }

        if (this.Font != null)
        {
            var font = this.Font;
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
                style.AppendFormat("font-family:\"{0}\";", font.FontFamily.Name);
            }
            if ((font.Style & FontStyle.Bold) != 0)
            {
                style.Append("font-weight:bold;");
            }
            if ((font.Style & FontStyle.Italic) != 0)
            {
                style.Append("font-style:italic;");
            }
            if ((font.Style & FontStyle.Underline) != 0)
            {
                style.Append("text-decoration:underline;");
            }
            if ((font.Style & FontStyle.Strikeout) != 0)
            {
                style.Append("text-decoration:line-through;");
            }
        }

        var stylename = $"s{unique_key}";
        var css = new StringBuilder();
        css.AppendLine($".{stylename}{{{style.ToString()}}}");
        if (widget is Gtk.TextView)
        {
            css.AppendLine($".{stylename} text{{{style.ToString()}}}");
            css.AppendLine($".{stylename} .view{{{style.ToString()}}}");
        }
        css.AppendLine(" menu menuitem .MenuCheck{padding:0px;margin:0px;} menu .MenuItem{padding:0px;margin-left:-23px;}");

        if (provider.LoadFromData(css.ToString()))
        {
            widget.StyleContext.RemoveClass(stylename);
            widget.StyleContext.AddClass(stylename);
        }
    }

    private void Checkbutton_Toggled(object sender, EventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    private void MenuItem_Activated(object sender, EventArgs e)
    {
        if (this.CheckState != CheckState.Unchecked)
        {
            if (flagBox.Child is Gtk.CheckButton checkbutton)
            {
                checkbutton.Active = !checkbutton.Active;
            }
        }

        if (((Gtk.MenuItem)sender).Child is Gtk.ComboBoxText combo)
        {
            combo.Popup();
        }
        else if (((Gtk.MenuItem)sender).Child is Gtk.Entry ey)
        {
            ey.IsFocus = true;
        }
    }

    private void MenuItem_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
    {
        DropDownItemClicked?.Invoke(this, new ToolStripItemClickedEventArgs(this));

        Click?.Invoke(this, args);

    }

    private void ToolStripItem_Realized(object sender, EventArgs e)
    {
        SetStyle((Gtk.MenuItem)sender);
        var menuItem = (Gtk.MenuItem)sender;
        if (menuItem.Parent is Gtk.Menu)
        {
            flagBox.Vexpand = true;
            flagBox.Hexpand = true;
            flagBox.WidthRequest = 30;
            menuItem.StyleContext.AddClass("MenuItem");
        }
        if (this.CheckState == CheckState.Checked)
        {
            var checkbutton = new Gtk.CheckButton();
            checkbutton.StyleContext.AddClass("MenuCheck");
            checkbutton.BorderWidth = 0;
            checkbutton.Margin = 0;
            checkbutton.Halign = Gtk.Align.Center;
            checkbutton.Valign = Gtk.Align.Center;
            checkbutton.Active = this.Checked;
            checkbutton.Toggled += Checkbutton_Toggled;
            flagBox.Child = checkbutton;
        }
        else if (this.CheckState == CheckState.Indeterminate)
        {
            var checkbutton = new Gtk.RadioButton(((WidgetToolStrip<Gtk.MenuItem>)this.Parent).groupradio);
            checkbutton.StyleContext.AddClass("MenuCheck");
            checkbutton.BorderWidth = 0;
            checkbutton.Margin = 0;
            checkbutton.Halign = Gtk.Align.Center;
            checkbutton.Valign = Gtk.Align.Center;
            checkbutton.Active = this.Checked;
            checkbutton.Toggled += Checkbutton_Toggled;
            flagBox.Child = checkbutton;
        }
        else
        {
            try
            {
                if (DisplayStyle == ToolStripItemDisplayStyle.Text)
                {
                    label.NoShowAll = false;
                    label.Visible = true;
                }
                else if (DisplayStyle == ToolStripItemDisplayStyle.Image)
                {
                    label.Visible = false;
                    label.NoShowAll = true;
                    if (Image != null && Image.PixbufData != null)
                    {
                        var ico1 = new Gtk.Image(new Gdk.Pixbuf(Image.PixbufData).ScaleSimple(20, 20, Gdk.InterpType.Tiles))
                        {
                            Halign = Gtk.Align.Center,
                            Valign = Gtk.Align.Center
                        };
                        flagBox.Child = ico1;
                    }
                }
                else
                {

                    if (Image != null && Image.PixbufData != null)
                    {
                        var ico1 = new Gtk.Image(new Gdk.Pixbuf(Image.PixbufData).ScaleSimple(20, 20, Gdk.InterpType.Tiles))
                        {
                            Halign = Gtk.Align.Center,
                            Valign = Gtk.Align.Center
                        };
                        flagBox.Child = ico1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.GtkLogger.Debug($"Unexpected exception in ToolStripItem_Realized", ex);
            }
        }
        menuItem.ShowAll();

    }
}
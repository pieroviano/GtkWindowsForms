/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents the collection of data bindings for a control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[DefaultEvent("Click")]
[DefaultProperty("Text")]
[Designer(typeof(ControlDesigner))]
[ToolboxItemFilter("System.Windows.Forms")]
public partial class Control : Component, IControl, ISynchronizeInvoke, IComponent, IDisposable, ISupportInitialize, IArrangedElement, IBindableComponent
{

    internal bool _Created;

    internal bool LockLocation = false; // because the code executes sequentially， special locking

    Cairo.Context context;

    Cairo.ImageSurface image;

    Cairo.Surface surface;

    private AnchorStyles _anchor;

    private bool _autoSize;

    private BorderStyle _BorderStyle;

    private DockStyle _dock;

    private Font _Font;

    private Color _ForeColor;

    private Size _size;

    private readonly Gtk.Application app = Application.Init();

    private readonly ArrangedElementCollection arrangedElementCollection;

    private int location_x = 0;

    private int location_y = 0;

    private int size_height = 0;

    private int size_width = 0;

    private bool WidgetRealized = false;

    /// <summary>
    /// Occurs when the Anchor property value changes.
    /// </summary>
    public virtual event EventHandler AnchorChanged;

    /// <summary>
    /// Occurs when the AutoSize property value changes.
    /// </summary>
    public virtual event EventHandler AutoSizeChanged;

    /// <summary>
    /// Occurs when the BackColor property value changes.
    /// </summary>
    public virtual event EventHandler BackColorChanged;

    /// <summary>
    /// Occurs when the BackgroundImage property value changes.
    /// </summary>
    public virtual event EventHandler BackgroundImageChanged;

    /// <summary>
    /// Occurs when the BackgroundImageLayout property value changes.
    /// </summary>
    public virtual event EventHandler BackgroundImageLayoutChanged;

    /// <summary>
    /// Occurs when the BindingContext property value changes.
    /// </summary>
    public virtual event EventHandler BindingContextChanged;

    /// <summary>
    /// Occurs when the CausesValidation property value changes.
    /// </summary>
    public virtual event EventHandler CausesValidationChanged;

    /// <summary>
    /// Occurs when the focus cues (keyboard, etc.) change.
    /// </summary>
    public virtual event UICuesEventHandler ChangeUICues;

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public virtual event EventHandler Click;

    /// <summary>
    /// Occurs when the ClientSize property value changes.
    /// </summary>
    public virtual event EventHandler ClientSizeChanged;

    /// <summary>
    /// Occurs when the ContextMenuStrip property value changes.
    /// </summary>
    public virtual event EventHandler ContextMenuStripChanged;

    /// <summary>
    /// Occurs when a control is added to the Controls collection.
    /// </summary>
    public virtual event ControlEventHandler ControlAdded;

    /// <summary>
    /// Occurs when a control is removed from the Controls collection.
    /// </summary>
    public virtual event ControlEventHandler ControlRemoved;

    /// <summary>
    /// Occurs when the Cursor property value changes.
    /// </summary>
    public virtual event EventHandler CursorChanged;

    /// <summary>
    /// Occurs when the Dock property value changes.
    /// </summary>
    public virtual event EventHandler DockChanged;

    /// <summary>
    /// Occurs when the control is double-clicked.
    /// </summary>
    public virtual event EventHandler DoubleClick;

    /// <summary>
    /// Occurs after the DPI of the parent has changed.
    /// </summary>
    public virtual event EventHandler DpiChangedAfterParent;

    /// <summary>
    /// Occurs before the DPI of the parent changes.
    /// </summary>
    public virtual event EventHandler DpiChangedBeforeParent;

    /// <summary>
    /// Occurs when an object is dragged and dropped onto the control.
    /// </summary>
    public virtual event DragEventHandler DragDrop;

    /// <summary>
    /// Occurs when an object is dragged into the control's bounds.
    /// </summary>
    public virtual event DragEventHandler DragEnter;

    /// <summary>
    /// Occurs when an object is dragged out of the control's bounds.
    /// </summary>
    public virtual event EventHandler DragLeave;

    /// <summary>
    /// Occurs when an object is dragged over the control.
    /// </summary>
    public virtual event DragEventHandler DragOver;

    /// <summary>
    /// Occurs when the Enabled property value changes.
    /// </summary>
    public virtual event EventHandler EnabledChanged;

    /// <summary>
    /// Occurs when the control receives input focus.
    /// </summary>
    public virtual event EventHandler Enter;

    /// <summary>
    /// Occurs when the Font property value changes.
    /// </summary>
    public virtual event EventHandler FontChanged;

    /// <summary>
    /// Occurs when the ForeColor property value changes.
    /// </summary>
    public virtual event EventHandler ForeColorChanged;

    /// <summary>
    /// Occurs when a drag-and-drop operation reports feedback.
    /// </summary>
    public virtual event GiveFeedbackEventHandler GiveFeedback;

    /// <summary>
    /// Occurs when the control receives input focus.
    /// </summary>
    public virtual event EventHandler GotFocus;

    /// <summary>
    /// Occurs when the underlying handle is created.
    /// </summary>
    public virtual event EventHandler HandleCreated;

    /// <summary>
    /// Occurs when the underlying handle is destroyed.
    /// </summary>
    public virtual event EventHandler HandleDestroyed;

    /// <summary>
    /// Occurs when the user requests help for the control.
    /// </summary>
    public virtual event HelpEventHandler HelpRequested;

    /// <summary>
    /// Occurs when the ImeMode property value changes.
    /// </summary>
    public virtual event EventHandler ImeModeChanged;

    /// <summary>
    /// Occurs when the control is invalidated.
    /// </summary>
    public virtual event InvalidateEventHandler Invalidated;

    /// <summary>
    /// Occurs when a key is pressed while the control has focus.
    /// </summary>
    public virtual event KeyEventHandler KeyDown;

    /// <summary>
    /// Occurs when a key is pressed and then released while the control has focus.
    /// </summary>
    public virtual event KeyPressEventHandler KeyPress;

    /// <summary>
    /// Occurs when a key is released while the control has focus.
    /// </summary>
    public virtual event KeyEventHandler KeyUp;

    /// <summary>
    /// Occurs when the layout of the control changes.
    /// </summary>
    public virtual event LayoutEventHandler Layout;

    /// <summary>
    /// Occurs when the control loses input focus.
    /// </summary>
    public virtual event EventHandler Leave;

    /// <summary>
    /// Occurs when the control is loaded (realized).
    /// </summary>
     //public event EventHandler Disposed;
    public virtual event EventHandler Load;

    /// <summary>
    /// Occurs when the Location property value changes.
    /// </summary>
    public virtual event EventHandler LocationChanged;

    /// <summary>
    /// Occurs when the control loses input focus.
    /// </summary>
    public virtual event EventHandler LostFocus;

    /// <summary>
    /// Occurs when the Margin property value changes.
    /// </summary>
    public virtual event EventHandler MarginChanged;

    /// <summary>
    /// Occurs when the mouse capture state changes.
    /// </summary>
    public virtual event EventHandler MouseCaptureChanged;

    /// <summary>
    /// Occurs when the control is clicked with the mouse.
    /// </summary>
    public virtual event MouseEventHandler MouseClick;

    /// <summary>
    /// Occurs when the control is double-clicked with the mouse.
    /// </summary>
    public virtual event MouseEventHandler MouseDoubleClick;

    /// <summary>
    /// Occurs when a mouse button is pressed while the cursor is over the control.
    /// </summary>
    public virtual event MouseEventHandler MouseDown;

    /// <summary>
    /// Occurs when the mouse pointer enters the control.
    /// </summary>
    public virtual event EventHandler MouseEnter;

    /// <summary>
    /// Occurs when the mouse pointer hovers over the control.
    /// </summary>
    public virtual event EventHandler MouseHover;

    /// <summary>
    /// Occurs when the mouse pointer leaves the control.
    /// </summary>
    public virtual event EventHandler MouseLeave;

    /// <summary>
    /// Occurs when the mouse pointer moves over the control.
    /// </summary>
    public virtual event MouseEventHandler MouseMove;

    /// <summary>
    /// Occurs when a mouse button is released while the cursor is over the control.
    /// </summary>
    public virtual event MouseEventHandler MouseUp;

    /// <summary>
    /// Occurs when the mouse wheel moves while the pointer is over the control.
    /// </summary>
    public virtual event MouseEventHandler MouseWheel;

    /// <summary>
    /// Occurs when the control is moved.
    /// </summary>
    public virtual event EventHandler Move;

    /// <summary>
    /// Occurs when the Padding property value changes.
    /// </summary>
    public virtual event EventHandler PaddingChanged;

    /// <summary>
    /// Occurs when the Parent property value changes.
    /// </summary>
     //public virtual event PaintEventHandler Paint;
    public virtual event EventHandler ParentChanged;

    /// <summary>
    /// Occurs when a key is pressed while the control has focus, and allows previewing the key press.
    /// </summary>
    public virtual event PreviewKeyDownEventHandler PreviewKeyDown;

    /// <summary>
    /// Occurs when accessibility help is requested for the control.
    /// </summary>
    public virtual event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

    /// <summary>
    /// Occurs during a drag operation to determine whether the drag should continue.
    /// </summary>
    public virtual event QueryContinueDragEventHandler QueryContinueDrag;

    /// <summary>
    /// Occurs when the Region property value changes.
    /// </summary>
    public virtual event EventHandler RegionChanged;

    /// <summary>
    /// Occurs when the control is resized.
    /// </summary>
    public virtual event EventHandler Resize;

    /// <summary>
    /// Occurs when the RightToLeft property value changes.
    /// </summary>
    public virtual event EventHandler RightToLeftChanged;

    /// <summary>
    /// Occurs when the Size property value changes.
    /// </summary>
    public virtual event EventHandler SizeChanged;

    /// <summary>
    /// Occurs when the control's style changes.
    /// </summary>
    public virtual event EventHandler StyleChanged;

    /// <summary>
    /// Occurs when the system colors change.
    /// </summary>
    public virtual event EventHandler SystemColorsChanged;

    /// <summary>
    /// Occurs when the TabIndex property value changes.
    /// </summary>
    public virtual event EventHandler TabIndexChanged;

    /// <summary>
    /// Occurs when the TabStop property value changes.
    /// </summary>
    public virtual event EventHandler TabStopChanged;

    /// <summary>
    /// Occurs when the Text property value changes.
    /// </summary>
    public virtual event EventHandler TextChanged;

    /// <summary>
    /// Occurs when the control is validated.
    /// </summary>
    public virtual event EventHandler Validated;

    /// <summary>
    /// Occurs when validating the control; allows cancellation.
    /// </summary>
    public virtual event CancelEventHandler Validating;

    /// <summary>
    /// Occurs when the Visible property value changes.
    /// </summary>
    public virtual event EventHandler VisibleChanged;

    /// <summary>
    /// Occurs when the control needs to be painted. The add/remove forward to the GTK override.
    /// </summary>
    public virtual event PaintEventHandler Paint
    {
        add
        {
            ISelf.Override.Paint += value;
        }

        remove
        {
            ISelf.Override.Paint -= value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Control"/> class.
    /// Sets up GTK event handlers and default style information.
    /// </summary>
    public Control()
    {
        this.unique_key = Guid.NewGuid().ToString().ToLower();
        Gtk.Widget widget = this.Widget;
        if (widget != null)
        {
            if (widget is Gtk.Window win)
            {
                widget.Halign = Align.Fill;
                widget.Valign = Align.Fill;
            }
            else
            {
                widget.Halign = Align.Start;
                widget.Valign = Align.Start;
                widget.Expand = false;
            }

            widget.Data["Control"] = this;
            widget.StyleContext.AddClass("DefaultThemeStyle");
            widget.ButtonPressEvent += Widget_ButtonPressEvent;
            widget.ButtonReleaseEvent += Widget_ButtonReleaseEvent;
            widget.EnterNotifyEvent += Widget_EnterNotifyEvent;
            widget.MotionNotifyEvent += Widget_MotionNotifyEvent;
            widget.LeaveNotifyEvent += Widget_LeaveNotifyEvent;
            widget.ScrollEvent += Widget_ScrollEvent;
            widget.FocusInEvent += Widget_FocusInEvent;
            widget.FocusOutEvent += Widget_FocusOutEvent;
            widget.KeyPressEvent += Widget_KeyPressEvent;
            widget.KeyReleaseEvent += Widget_KeyReleaseEvent;
            widget.Realized += Widget_Realized;
            widget.ConfigureEvent += Widget_ConfigureEvent;
            ISelf.Override.PaintGraphics += Override_PaintGraphics;
            widget.SizeAllocated += Widget_SizeAllocated;
        }
    }

    /// <summary>
    /// Gets the accessible object for the control.
    /// </summary>

    public virtual AccessibleObject AccessibilityObject { get; }

    /// <summary>
    /// Gets or sets the default action description for accessibility.
    /// </summary>
    public virtual string AccessibleDefaultActionDescription { get; set; }

    /// <summary>
    /// Gets or sets the accessible description for the control.
    /// </summary>
    public virtual string AccessibleDescription { get; set; }

    /// <summary>
    /// Gets or sets the accessible name for the control.
    /// </summary>
    public virtual string AccessibleName { get; set; }

    /// <summary>
    /// Gets or sets the accessible role for the control.
    /// </summary>
    public virtual AccessibleRole AccessibleRole { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control allows drag-and-drop operations.
    /// </summary>
    public virtual bool AllowDrop { get; set; }

    /// <summary>
    /// Gets or sets how the control is anchored to its parent container.
    /// </summary>
    public virtual AnchorStyles Anchor
    {
        get => _anchor;
        set
        {
            _anchor = value;
            SetAnchorStyles(Widget, _anchor);
            AnchorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the offset of the auto-scroll position.
    /// </summary>
    public virtual Point AutoScrollOffset { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is automatically resized to fit its contents.
    /// </summary>
    public virtual bool AutoSize
    {
        get => _autoSize;
        set
        {
            _autoSize = value;
            if (_autoSize == true)
            {
                this.Widget.WidthRequest = -1;
                this.Widget.HeightRequest = -1;
            }
            else
            {
                Size = _size;
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the control.
    /// </summary>
    public virtual Color BackColor
    {
        get
        {
            if (ISelf.Override.BackColor.HasValue)
                return ISelf.Override.BackColor.Value;
            //else if (UseVisualStyleBackColor)
            //    return Color.FromName("0");
            //else
            //    return Color.Transparent; 
            else
                return Color.FromName("0");
        }

        set
        {
            ISelf.Override.BackColor = value;
            ISelf.Override.OnAddClass();
            UpdateStyle();
            Refresh();
        }
    }

    /// <summary>
    /// Gets or sets the background image for the control.
    /// </summary>
    public virtual Drawing.Image BackgroundImage
    {
        get => ISelf?.Override.BackgroundImage;
        set
        {
            if (ISelf != null)
            {
                ISelf.Override.BackgroundImage = value;
                Refresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the layout for the background image.
    /// </summary>
    public virtual ImageLayout BackgroundImageLayout
    {
        get => ISelf == null ? ImageLayout.None : ISelf.Override.BackgroundImageLayout;
        set
        {
            ISelf?.Override.BackgroundImageLayout = value;
        }
    }

    /// <summary>
    /// Gets or sets the binding context for the control.
    /// </summary>
    public virtual BindingContext BindingContext { get; set; }

    /// <summary>
    /// Gets or sets the border style for the control.
    /// </summary>
    public virtual BorderStyle BorderStyle
    {
        get
        {
            return _BorderStyle;
        }

        set
        {
            _BorderStyle = value;
            if (value == BorderStyle.FixedSingle)
            {
                this.Widget.StyleContext.RemoveClass("BorderFixed3D");
                this.Widget.StyleContext.RemoveClass("BorderNone");
                this.Widget.StyleContext.AddClass("BorderFixedSingle");
            }
            else if (value == BorderStyle.Fixed3D)
            {
                this.Widget.StyleContext.RemoveClass("BorderFixedSingle");
                this.Widget.StyleContext.RemoveClass("BorderNone");
                this.Widget.StyleContext.AddClass("BorderFixed3D");
            }
            else
            {
                this.Widget.StyleContext.RemoveClass("BorderFixedSingle");
                this.Widget.StyleContext.RemoveClass("BorderFixed3D");
                this.Widget.StyleContext.AddClass("BorderNone");
            }
        }
    }

    /// <summary>
    /// Gets the bottom position of the control relative to its container.
    /// </summary>
    public virtual int Bottom { get => this.Widget.MarginBottom; }

    /// <summary>
    /// Gets or sets the bounds of the control.
    /// </summary>
    public virtual Rectangle Bounds
    {
        get => new Rectangle(Widget.Clip.X, this.Widget.Clip.Y, this.Widget.Clip.Width, this.Widget.Clip.Height);
        set
        {
            SetBounds(value.X, value.Y, value.Width, value.Height);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the control can receive focus.
    /// </summary>
    public virtual bool CanFocus
    {
        get
        {
            return this.Widget.CanFocus;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the control can be selected.
    /// </summary>
    public virtual bool CanSelect { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the control has captured the mouse.
    /// </summary>
    public virtual bool Capture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control causes validation on focus change.
    /// </summary>
    public virtual bool CausesValidation { get; set; }

    /// <summary>
    /// Gets the arranged element collection for child layout.
    /// </summary>
    public ArrangedElementCollection Children => arrangedElementCollection;

    /// <summary>
    /// Gets the client rectangle for the control.
    /// </summary>
    public virtual Rectangle ClientRectangle
    {
        get
        {
            Gdk.Rectangle allocation = this.Widget.Allocation;

            return new Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height);
        }
    }

    /// <summary>
    /// Gets or sets the size of the client area.
    /// </summary>
    public virtual Size ClientSize
    {
        get
        {
            return new Size(Widget.AllocatedWidth, this.Widget.AllocatedHeight);
        }

        set
        {
            this.Widget.SetSizeRequest(value.Width, value.Height);
        }
    }

    /// <summary>
    /// Gets the company name associated with the control (if any).
    /// </summary>
    public virtual string CompanyName { get; }

    /// <summary>
    /// Gets a value indicating whether the control or one of its child controls has input focus.
    /// </summary>
    public virtual bool ContainsFocus { get; }

    /// <summary>
    /// Gets or sets the context menu associated with the control.
    /// </summary>
    public virtual ContextMenuStrip ContextMenuStrip { get; set; }

    /// <summary>
    /// Gets the collection of child controls.
    /// </summary>
    public virtual ControlCollection Controls { get; }

    /// <summary>
    /// Gets a value indicating whether the control has been created.
    /// </summary>
    public virtual bool Created => _Created;

    /// <summary>
    /// Gets or sets the cursor for the control.
    /// </summary>
    public virtual Cursor Cursor { get; set; }

    /// <summary>
    /// Gets the collection of data bindings for the control.
    /// </summary>
    public virtual ControlBindingsCollection DataBindings { get; }

    /// <summary>
    /// Gets the device DPI for the control.
    /// </summary>
    public virtual int DeviceDpi { get; }

    /// <summary>
    /// Gets the rectangle that represents the display area of the control.
    /// </summary>
    public virtual Rectangle DisplayRectangle { get; }

    /// <summary>
    /// Gets a value indicating whether the control is in the process of being disposed.
    /// </summary>
    public virtual bool Disposing { get; }

    /// <summary>
    /// Gets or sets how the control is docked within its parent container.
    /// </summary>
    public virtual DockStyle Dock
    {
        get
        {
            return _dock;
        }

        set
        {
            _dock = value;
            if (value == DockStyle.Fill)
            {
                this.Widget.Halign = Align.Fill;
                this.Widget.Valign = Align.Fill;
            }
            else if (value == DockStyle.Left)
            {
                this.Widget.Halign = Align.Start;
                this.Widget.Valign = Align.Fill;
            }
            else if (value == DockStyle.Top)
            {
                this.Widget.Halign = Align.Fill;
                this.Widget.Valign = Align.Start;
            }
            else if (value == DockStyle.Right)
            {
                this.Widget.Halign = Align.End;
                this.Widget.Valign = Align.Fill;
            }
            else if (value == DockStyle.Bottom)
            {
                this.Widget.Halign = Align.Fill;
                this.Widget.Valign = Align.End;
            }
            else if (value == DockStyle.None)
            {
                this.Widget.Halign = Align.Start;
                this.Widget.Valign = Align.Start;
            }

            DockChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    public virtual bool Enabled
    {
        get
        {
            return this.Widget.Sensitive;
        }

        set
        {
            this.Widget.Sensitive = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the control has input focus.
    /// </summary>
    public virtual bool Focused
    {
        get
        {
            return this.Widget.IsFocus;
        }
    }

    /// <summary>
    /// Gets or sets the font of the control.
    /// </summary>
    public virtual Font Font
    {
        get
        {
            if (_Font == null)
            {
                var fontdes = this.Widget.PangoContext.FontDescription;
                int size = Convert.ToInt32(fontdes.Size / Pango.Scale.PangoScale);
                return new Drawing.Font(new Drawing.FontFamily(fontdes.Family), size);
            }
            else
                return _Font;
        }

        set
        {
            _Font = value;
            if (string.IsNullOrWhiteSpace(_Font?.Name) == false && !this.Widget.PangoContext.Families.Any(o => o.Name == _Font.Name))
            {
                _Font = new Font("", value.Size, value.Style, value.Unit, value.GdiCharSet);
                Console.WriteLine($"\"{_Font.Name}\" font name is not supported, only English names are supported. Please confirm that the font name is correct or replace it with an English name");
            }

            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the control.
    /// </summary>
    public virtual Color ForeColor
    {
        get
        {
            return _ForeColor;
        }

        set
        {
            _ForeColor = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets the GTK container associated with this control, if any.
    /// </summary>
    public virtual Gtk.Container GtkContainer { get => GtkControl as Gtk.Container; }

    /// <summary>
    /// Gets or sets the underlying GTK control object.
    /// </summary>
    public virtual object GtkControl { get; set; }

    /// <summary>
    /// Gets the handle of the underlying GTK widget.
    /// </summary>
    public virtual IntPtr Handle { get => this.Widget == null ? IntPtr.Zero : this.Widget.Handle; }

    /// <summary>
    /// Gets a value indicating whether the control has child controls.
    /// </summary>
    public virtual bool HasChildren { get; }

    /// <summary>
    /// Gets or sets the height of the control.
    /// </summary>
    public virtual int Height
    {
        get
        {
            if (this.Widget.IsMapped == false && this.Widget is Gtk.Window wnd)
            {
                return wnd.HeightRequest == -1 ? wnd.DefaultHeight : wnd.HeightRequest;
            }

            return this.Widget.HeightRequest == -1 ? this.Widget.AllocatedHeight : this.Widget.HeightRequest;
        }

        set
        {
            this.Widget.HeightRequest = Math.Max(-1, value);
            DockChanged?.Invoke(this, EventArgs.Empty);
            AnchorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the primary image associated with the control.
    /// </summary>

    public virtual System.Drawing.Image Image { get; set; }

    /// <summary>
    /// Gets or sets the alignment of the image within the control.
    /// </summary>
    public virtual System.Drawing.ContentAlignment ImageAlign { get; set; }

    /// <summary>
    /// Gets or sets the Input Method Editor (IME) mode for the control.
    /// </summary>
    public virtual ImeMode ImeMode { get; set; }

    /// <summary>
    /// Gets a value indicating whether calls to the control must be marshaled to the UI thread.
    /// </summary>
    public virtual bool InvokeRequired { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is accessible.
    /// </summary>
    public virtual bool IsAccessible { get; set; }

    /// <summary>
    /// Gets a value indicating whether the control has been disposed.
    /// </summary>
    public virtual bool IsDisposed { get; }

    /// <summary>
    /// Gets the IControlGtk wrapper for the underlying GTK control.
    /// </summary>
    public virtual IControlGtk ISelf { get => GtkControl as IControlGtk; }

    /// <summary>
    /// Gets a value indicating whether the underlying handle has been created (realized).
    /// </summary>
    public virtual bool IsHandleCreated { get => this.Widget.IsRealized; }

    /// <summary>
    /// Gets a value indicating whether the control is mirrored for right-to-left languages.
    /// </summary>
    public virtual bool IsMirrored { get; }

    /// <summary>
    /// Gets the layout engine used by the control.
    /// </summary>
    public virtual LayoutEngine LayoutEngine { get; }

    /// <summary>
    /// Gets or sets the left position of the control relative to its container.
    /// </summary>
    public virtual int Left
    {
        get => this.Widget.MarginStart;
        set
        {
            this.Widget.MarginStart = value;
            DockChanged?.Invoke(this, EventArgs.Empty);
            AnchorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the location of the control.
    /// </summary>
    public virtual Point Location
    {
        get
        {
            return new Point(Left, Top);
        }

        set
        {
            if (LockLocation == false)
            {
                Left = value.X;
                Top = value.Y;
            }
        }
    }

    /// <summary>
    /// Gets or sets the margin around the control.
    /// </summary>
    public virtual Padding Margin { get; set; }

    /// <summary>
    /// Gets or sets the maximum size the control can be resized to.
    /// </summary>
    public virtual Size MaximumSize { get; set; }

    /// <summary>
    /// Gets or sets the minimum size the control can be resized to.
    /// </summary>
    public virtual Size MinimumSize { get; set; }

    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    public virtual string Name
    {
        get
        {
            return this.Widget.Name;
        }

        set
        {
            this.Widget.Name = value;
        }
    }

    /// <summary>
    /// Gets or sets the padding within the control.
    /// </summary>
    public virtual Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets the parent control.
    /// </summary>
    public virtual Control Parent { get; set; }

    /// <summary>
    /// Gets a value indicating whether the control participates in the layout.
    /// </summary>
    public bool ParticipatesInLayout => false;

    /// <summary>
    /// Gets the preferred size of the control.
    /// </summary>
    public virtual Size PreferredSize { get; }

    /// <summary>
    /// Gets the product name associated with the control (if any).
    /// </summary>
    public virtual string ProductName { get; }

    /// <summary>
    /// Gets the product version associated with the control (if any).
    /// </summary>
    public virtual string ProductVersion { get; }

    /// <summary>
    /// Gets a value indicating whether the control is recreating its handle.
    /// </summary>
    public virtual bool RecreatingHandle { get; }

    /// <summary>
    /// Gets or sets the region describing the window region of the control.
    /// </summary>
    public virtual Drawing.Region Region { get; set; }

    /// <summary>
    /// Gets the right position of the control relative to its container.
    /// </summary>
    public virtual int Right { get => this.Widget.MarginEnd; }

    /// <summary>
    /// Gets or sets the reading order of the control.
    /// </summary>
    public virtual RightToLeft RightToLeft { get; set; }

    /// <summary>
    /// Gets or sets the size of the control.
    /// </summary>
    public virtual Size Size
    {
        get
        {
            return new Size(Width, Height);
        }

        set
        {
            _size = value;
            if (AutoSize == false)
            {
                if (this.Widget is Gtk.Button)
                {
                    Width = value.Width > 6 ? value.Width - 6 : value.Width;
                    Height = value.Height > 6 ? value.Height - 6 : value.Height;
                }
                else
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the tab index of the control.
    /// </summary>
    public virtual int TabIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is included in tab navigation.
    /// </summary>
    public virtual bool TabStop { get; set; }

    /// <summary>
    /// Gets or sets an object that contains data about the control.
    /// </summary>
    public virtual object Tag { get; set; }

    /// <summary>
    /// Gets or sets the text associated with the control.
    /// </summary>
    public virtual string Text { get; set; }

    /// <summary>
    /// Gets or sets the top position of the control relative to its container.
    /// </summary>
    public virtual int Top
    {
        get => this.Widget.MarginTop;
        set
        {
            this.Widget.MarginTop = value;
            DockChanged?.Invoke(this, EventArgs.Empty);
            AnchorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the top-level control that contains this control.
    /// </summary>
    public virtual Control TopLevelControl { get; }

    /// <summary>
    /// Gets a unique key associated with the control (used for styling or resources).
    /// </summary>
    public string unique_key { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control should use the visual style background color.
    /// </summary>
    public virtual bool UseVisualStyleBackColor { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the wait cursor should be used for the control.
    /// </summary>
    public virtual bool UseWaitCursor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is visible.
    /// </summary>
    public virtual bool Visible
    {
        get
        {
            return this.Widget.Visible;
        }

        set
        {
            this.Widget.Visible = value;
            this.Widget.NoShowAll = value == false;
        }
    }

    /// <summary>
    /// Gets the visual style background color for the control.
    /// </summary>
    public virtual Color VisualStyleBackColor { get; }

    /// <summary>
    /// Gets the underlying GTK widget for this control.
    /// </summary>
    public virtual Gtk.Widget Widget { get => GtkControl as Gtk.Widget; }

    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    public virtual int Width
    {
        get
        {
            if (this.Widget.IsMapped == false && this.Widget is Gtk.Window wnd)
            {
                return wnd.WidthRequest == -1 ? wnd.DefaultWidth : wnd.WidthRequest;
            }

            return this.Widget.WidthRequest == -1 ? this.Widget.AllocatedWidth : this.Widget.WidthRequest;
        }

        set
        {
            this.Widget.WidthRequest = Math.Max(-1, value);
            DockChanged?.Invoke(this, EventArgs.Empty);
            AnchorChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the window target for the control.
    /// </summary>
    public virtual IWindowTarget WindowTarget { get; set; }

    /// <summary>
    /// Gets a value indicating whether show focus cues.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show focus cues; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual bool ShowFocusCues { get; }

    /// <summary>
    /// Gets a value indicating whether show keyboard cues.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show keyboard cues; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual bool ShowKeyboardCues { get; }

    IArrangedElement IArrangedElement.Container => throw new NotImplementedException();

    PropertyStore IArrangedElement.Properties => throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether the control can enable IME.
    /// </summary>
    protected virtual bool CanEnableIme { get; }

    /// <summary>
    /// Gets a value indicating whether the component can raise an event.
    /// </summary>
    protected override bool CanRaiseEvents { get; }

    /// <summary>
    /// Gets the create parameters used to create the underlying window.
    /// </summary>
    protected virtual CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = new CreateParams();
            createParams.ExStyle |= 32;

            return createParams;
        }
    }

    /// <summary>
    /// Gets the default cursor for the control.
    /// </summary>
    protected virtual Cursor DefaultCursor { get; }

    /// <summary>
    /// Gets the default IME mode for the control.
    /// </summary>
    protected virtual ImeMode DefaultImeMode { get; }

    /// <summary>
    /// Gets the default margin for the control.
    /// </summary>
    protected virtual Padding DefaultMargin { get; }

    /// <summary>
    /// Gets the default maximum size for the control.
    /// </summary>
    protected virtual Size DefaultMaximumSize { get; }

    /// <summary>
    /// Gets the default minimum size for the control.
    /// </summary>
    protected virtual Size DefaultMinimumSize { get; }

    /// <summary>
    /// Gets the default padding for the control.
    /// </summary>
    protected virtual Padding DefaultPadding { get; }

    /// <summary>
    /// Gets the default size for the control.
    /// </summary>
    protected virtual Size DefaultSize { get; }

    /// <summary>
    /// Gets or sets a value indicating whether double buffering is enabled for the control.
    /// </summary>
    protected virtual bool DoubleBuffered { get; set; }

    /// <summary>
    /// Gets or sets the font height for internal layout calculations.
    /// </summary>
    protected int FontHeight { get; set; }

    /// <summary>
    /// Gets or sets the base IME mode for the control.
    /// </summary>
    protected virtual ImeMode ImeModeBase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control should be redrawn on resize.
    /// </summary>
    protected bool ResizeRedraw { get; set; }

    /// <summary>
    /// Gets a value indicating whether scale children.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if scale children; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual bool ScaleChildren { get; }

    /// <summary>
    /// Begins initialization of the control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void BeginInit()
    {
    }

    /// <summary>
    /// Begins an asynchronous operation on the control's thread.
    /// </summary>
    /// <param name = "method">
    /// Delegate to invoke asynchronously.
    /// </param>
    /// <param name = "args">
    /// Arguments to pass to the delegate.
    /// </param>
    /// <returns>
    /// An IAsyncResult representing the asynchronous operation.
    /// </returns>
    public virtual IAsyncResult BeginInvoke(Delegate method, params object[] args)
    {
        System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(state =>
        {
            method.DynamicInvoke((object[])state);
        }, args);

        return task;
    }

    /// <summary>
    /// Begins an asynchronous operation on the control's thread.
    /// </summary>
    /// <param name = "method">
    /// Delegate to invoke asynchronously.
    /// </param>
    /// <returns>
    /// An IAsyncResult representing the asynchronous operation.
    /// </returns>
    public virtual IAsyncResult BeginInvoke(Delegate method)
    {
        return BeginInvoke(method, null);
    }

    /// <summary>
    /// Begins an asynchronous action on the control's thread.
    /// </summary>
    /// <param name = "method">
    /// Action to invoke asynchronously.
    /// </param>
    /// <returns>
    /// An IAsyncResult representing the asynchronous operation.
    /// </returns>
    public virtual IAsyncResult BeginInvoke(Action method)
    {
        System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(method);

        return task;
    }

    /// <summary>
    /// Brings the control to the front of the z-order.
    /// </summary>
    public virtual void BringToFront()
    {
    }

    /// <summary>
    /// Determines whether the specified control is contained within the control's hierarchy.
    /// </summary>
    /// <param name = "ctl">
    /// Control to locate.
    /// </param>
    /// <returns>
    /// True if the control is contained; otherwise false.
    /// </returns>
    public virtual bool Contains(Control ctl)
    {
        return false;
    }

    /// <summary>
    /// Creates the control and any child controls.
    /// </summary>
    public virtual void CreateControl()
    {
    }

    /// <summary>
    /// Creates a Drawing.Graphics object for the control.
    /// </summary>
    /// <returns>
    /// A Graphics object used to draw to the control.
    /// </returns>
    public virtual Drawing.Graphics CreateGraphics()
    {
        try
        {
            if (image == null)
                image = new Cairo.ImageSurface(Cairo.Format.Argb32, this.Widget.AllocatedWidth, this.Widget.AllocatedHeight);
            surface?.Dispose();
            surface = image.CreateSimilar(Cairo.Content.ColorAlpha, this.Widget.AllocatedWidth, this.Widget.AllocatedHeight);
            context?.Dispose();
            context = new Cairo.Context(surface);
            return new Drawing.Graphics(this.Widget, context, this.Widget.Allocation);
        }
        catch (Exception ex)
        {
            Console.WriteLine(" canvas creation failed：" + ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Creates a Drawing.Graphics object limited to the specified rectangle.
    /// </summary>
    /// <param name = "x">
    /// X coordinate of the target rectangle.
    /// </param>
    /// <param name = "y">
    /// Y coordinate of the target rectangle.
    /// </param>
    /// <param name = "width">
    /// Width of the target rectangle.
    /// </param>
    /// <param name = "height">
    /// Height of the target rectangle.
    /// </param>
    /// <returns>
    /// A Graphics object used to draw to the specified region of the control.
    /// </returns>
    public virtual Drawing.Graphics CreateGraphics(int x, int y, int width, int height)
    {
        try
        {
            if (image == null)
                image = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
            surface?.Dispose();
            surface = image.CreateSimilar(Cairo.Content.ColorAlpha, width, height);
            context?.Dispose();
            context = new Cairo.Context(surface);
            return new Drawing.Graphics(this.Widget, context, new Gdk.Rectangle(x, y, width, height));
        }
        catch (Exception ex)
        {
            Console.WriteLine(" canvas creation failed：" + ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Disposes the control and its resources.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        base.Dispose();
    }

    /// <summary>
    /// Initiates a drag-and-drop operation.
    /// </summary>
    /// <param name = "data">
    /// Data to drag.
    /// </param>
    /// <param name = "allowedEffects">
    /// Allowed drag-drop effects.
    /// </param>
    /// <returns>
    /// The result of the drag-and-drop operation.
    /// </returns>
    public virtual DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
    {
        return DragDropEffects.None;
    }

    /// <summary>
    /// Draws the control into the specified bitmap.
    /// </summary>
    /// <param name = "bitmap">
    /// Bitmap to draw into.
    /// </param>
    /// <param name = "targetBounds">
    /// Area within the bitmap to draw.
    /// </param>
    public virtual void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
    }

    /// <summary>
    /// Ends initialization of the control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void EndInit()
    {
    }

    /// <summary>
    /// Ends an asynchronous operation started by BeginInvoke.
    /// </summary>
    /// <param name = "asyncResult">
    /// The IAsyncResult returned from BeginInvoke.
    /// </param>
    /// <returns>
    /// The result from the asynchronous operation if any.
    /// </returns>
    public virtual object EndInvoke(IAsyncResult asyncResult)
    {
        if (asyncResult is System.Threading.Tasks.Task task)
        {
            if (task.IsCompleted == false && task.IsCanceled == false && task.IsFaulted == false)
                task.GetAwaiter().GetResult();
        }

        return asyncResult.AsyncState;
    }

    /// <summary>
    /// Retrieves the form that the control is on.
    /// </summary>
    /// <returns>
    /// The parent Form, or null if none is found.
    /// </returns>
    public virtual Form FindForm()
    {
        if (this.Widget.Toplevel.Data.ContainsKey("Control"))
        {
            return this.Widget.Toplevel.Data["Control"] as Form;
        }
        else
        {
            Control control = this.Parent;
            while (control != null)
            {
                if (control is Form)
                    break;
                else
                    control = this.Parent;
            }

            return control as Form;
        }
    }

    /// <summary>
    /// Attempts to set focus to the control.
    /// </summary>
    /// <returns>
    /// True if the control received focus; otherwise false.
    /// </returns>
    public virtual bool Focus()
    {
        if (this.Widget != null)
        {
            this.Widget.IsFocus = true;
            return this.Widget.IsFocus;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the child control at the specified point.
    /// </summary>
    /// <param name = "pt">
    /// Point to test, in client coordinates.
    /// </param>
    /// <returns>
    /// The child control at the point, or null if none.
    /// </returns>
    public virtual Control GetChildAtPoint(Point pt)
    {
        return null;
    }

    /// <summary>
    /// Gets the child control at the specified point, with skip options.
    /// </summary>
    /// <param name = "pt">
    /// Point to test, in client coordinates.
    /// </param>
    /// <param name = "skipValue">
    /// Specifies which children to skip.
    /// </param>
    /// <returns>
    /// The child control at the point, or null if none.
    /// </returns>
    public virtual Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
    {
        return null;
    }

    /// <summary>
    /// Returns the container control that the control is associated with.
    /// </summary>
    /// <returns>
    /// An IContainerControl if the control implements it; otherwise the control itself.
    /// </returns>
    public virtual IContainerControl GetContainerControl()
    {
        return this as IContainerControl;
    }

    /// <summary>
    /// Gets the next control in the tab order.
    /// </summary>
    /// <param name = "ctl">
    /// The starting control.
    /// </param>
    /// <param name = "forward">
    /// True to move forward; false to move backward.
    /// </param>
    /// <returns>
    /// The next control in the tab order, or null if none.
    /// </returns>
    public virtual Control GetNextControl(Control ctl, bool forward)
    {
        Control prev = null;
        Control next = null;
        bool finded = false;
        foreach (var obj in this.Controls)
        {
            if (obj is Control control)
            {
                if (finded == true)
                    next = control;
                if (control.Widget.Handle == ctl.Widget.Handle)
                {
                    finded = true;
                }

                if (finded == true)
                {
                    if (forward == false && prev != null)
                    {
                        return prev;
                    }
                    else if (forward == true && next != null)
                    {
                        return next;
                    }
                }

                prev = control;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the preferred size for the control given the proposed size.
    /// </summary>
    /// <param name = "proposedSize">
    /// The available size for the control.
    /// </param>
    /// <returns>
    /// The preferred size of the control.
    /// </returns>
    public virtual Size GetPreferredSize(Size proposedSize)
    {
        return proposedSize;
    }

    /// <summary>
    /// Hides the control.
    /// </summary>
    public virtual void Hide()
    {
        if (this.GtkControl is Misc con)
        {
            con.Hide();
        }
    }

    /// <summary>
    /// Invalidates the entire control and causes a repaint.
    /// </summary>
    public virtual void Invalidate()
    {
        Invalidate(true);
    }

    /// <summary>
    /// Invalidates the control and optionally its children.
    /// </summary>
    /// <param name = "invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(bool invalidateChildren)
    {
        Invalidate(new Rectangle(Widget.Allocation.X, Widget.Allocation.Y, Widget.Allocation.Width, Widget.Allocation.Height), invalidateChildren);
    }

    /// <summary>
    /// Invalidates the specified rectangle of the control.
    /// </summary>
    /// <param name = "rc">
    /// Rectangle to invalidate, in client coordinates.
    /// </param>
    public virtual void Invalidate(Rectangle rc)
    {
        Invalidate(rc, true);
    }

    /// <summary>
    /// Invalidates the specified rectangle of the control and optionally its children.
    /// </summary>
    /// <param name = "rc">
    /// Rectangle to invalidate, in client coordinates.
    /// </param>
    /// <param name = "invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(Rectangle rc, bool invalidateChildren)
    {
        if (this.Widget != null)
        {
            ISelf?.Override.OnAddClass();
            this.Widget.Window.InvalidateRect(new Gdk.Rectangle(rc.X, rc.Y, rc.Width, rc.Height), invalidateChildren);
            if (invalidateChildren == true && this.Widget is Gtk.Container container)
            {
                foreach (var child in container.Children)
                    child.Window.InvalidateRect(child.Allocation, invalidateChildren);
            }

            Refresh();
        }
    }

    /// <summary>
    /// Invalidates the specified region of the control.
    /// </summary>
    /// <param name = "region">
    /// Region to invalidate.
    /// </param>
    public virtual void Invalidate(Drawing.Region region)
    {
        Invalidate(region, true);
    }

    /// <summary>
    /// Invalidates the specified region of the control and optionally its children.
    /// </summary>
    /// <param name = "region">
    /// Region to invalidate.
    /// </param>
    /// <param name = "invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(Drawing.Region region, bool invalidateChildren)
    {
        if (this.Widget != null)
        {
            ISelf?.Override.OnAddClass();
            this.Widget.Window.InvalidateRect(Widget.Allocation, invalidateChildren);
        }
    }

    /// <summary>
    /// Invokes a delegate on the control's thread and returns its result.
    /// </summary>
    /// <param name = "method">
    /// Delegate to invoke.
    /// </param>
    /// <returns>
    /// The result returned by the delegate.
    /// </returns>
    public virtual object Invoke(Delegate method)
    {
        return Invoke(method, null);
    }

    /// <summary>
    /// Invokes a delegate on the control's thread and returns its result.
    /// </summary>
    /// <param name = "method">
    /// Delegate to invoke.
    /// </param>
    /// <param name = "args">
    /// Arguments to pass to the delegate.
    /// </param>
    /// <returns>
    /// The result returned by the delegate.
    /// </returns>
    public virtual object Invoke(Delegate method, params object[] args)
    {
        object result = null;
        GLib.Idle.Add(() =>
        {
            result = method.DynamicInvoke(args);
            return false;
        });

        return result;
    }

    /// <summary>
    /// Invokes an action on the control's thread.
    /// </summary>
    /// <param name = "method">
    /// Action to invoke.
    /// </param>
    public virtual void Invoke(Action method)
    {
        GLib.Idle.Add(() =>
        {
            method.Invoke();
            return false;
        });
    }

    /// <summary>
    /// Invokes a function on the control's thread and returns typed result.
    /// </summary>
    /// <typeparam name = "ENTRY">
    /// Return type.
    /// </typeparam>
    /// <param name = "method">
    /// Function to invoke.
    /// </param>
    /// <returns>
    /// Result of the function.
    /// </returns>
    public virtual ENTRY Invoke<ENTRY>(Func<ENTRY> method)
    {
        ENTRY result = default(ENTRY);
        GLib.Idle.Add(() =>
        {
            result = method.Invoke();
            return false;
        });

        return result;
    }

    /// <summary>
    /// Converts a logical unit value to device units.
    /// </summary>
    /// <param name = "value">
    /// Value in logical units.
    /// </param>
    /// <returns>
    /// Value in device units.
    /// </returns>
    public virtual int LogicalToDeviceUnits(int value)
    {
        return value;
    }

    /// <summary>
    /// Converts a logical size to device units.
    /// </summary>
    /// <param name = "value">
    /// Size in logical units.
    /// </param>
    /// <returns>
    /// Size in device units.
    /// </returns>
    public virtual Size LogicalToDeviceUnits(Size value)
    {
        return value;
    }

    /// <summary>
    /// Programmatically performs a click on the control.
    /// </summary>
    public virtual void PerformClick()
    {
        OnClick(EventArgs.Empty);
    }

    /// <summary>
    /// Forces the control to perform layout logic.
    /// </summary>
    public virtual void PerformLayout()
    {
        _Created = true;
    }

    /// <summary>
    /// Forces the control to perform layout logic for a specific affected control/property.
    /// </summary>
    /// <param name = "affectedControl">
    /// Control affected by the layout.
    /// </param>
    /// <param name = "affectedProperty">
    /// Property name that changed.
    /// </param>
    public virtual void PerformLayout(Control affectedControl, string affectedProperty)
    {
        _Created = true;
    }

    /// <summary>
    /// Converts a screen point to client coordinates.
    /// </summary>
    /// <param name = "p">
    /// Point in screen coordinates.
    /// </param>
    /// <returns>
    /// Point in client coordinates.
    /// </returns>
    public virtual Point PointToClient(Point p)
    {
        if (Widget != null)
        {
            this.Widget.Window.GetOrigin(out int x, out int y);
            if (p.X > x && p.Y > y)
                return new Point(p.X - x, p.Y - y);
        }

        return new Point(p.X, p.Y);
    }

    /// <summary>
    /// Converts a client point to screen coordinates.
    /// </summary>
    /// <param name = "p">
    /// Point in client coordinates.
    /// </param>
    /// <returns>
    /// Point in screen coordinates.
    /// </returns>
    public virtual Point PointToScreen(Point p)
    {
        if (Widget != null)
        {
            this.Widget.Window.GetOrigin(out int x, out int y);
            if (p.X < x && p.Y < y)
                return new Point(p.X + x, p.Y + y);
        }

        return new Point(p.X, p.Y);
    }

    /// <summary>
    /// Allows preprocessing of Windows messages for the control.
    /// </summary>
    /// <param name = "msg">
    /// Message to preprocess.
    /// </param>
    /// <returns>
    /// A PreProcessControlState value.
    /// </returns>
    public virtual PreProcessControlState PreProcessControlMessage(ref Message msg)
    {
        return PreProcessControlState.MessageNotNeeded;
    }

    /// <summary>
    /// Allows preprocessing of a message and returns whether it was handled.
    /// </summary>
    /// <param name = "msg">
    /// Message to preprocess.
    /// </param>
    /// <returns>
    /// True if the message was processed; otherwise false.
    /// </returns>
    public virtual bool PreProcessMessage(ref Message msg)
    {
        return false;
    }

    /// <summary>
    /// Converts a rectangle from screen coordinates to client coordinates.
    /// </summary>
    /// <param name = "r">
    /// Rectangle in screen coordinates.
    /// </param>
    /// <returns>
    /// Rectangle in client coordinates.
    /// </returns>
    public virtual Rectangle RectangleToClient(Rectangle r)
    {
        if (Widget != null)
        {
            this.Widget.Window.GetPosition(out int x, out int y);
            if (r.X > x && r.Y > y)
                return new Rectangle(r.X - x, r.Y - y, r.Width, r.Height);
        }

        return new Rectangle(r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Converts a rectangle from client coordinates to screen coordinates.
    /// </summary>
    /// <param name = "r">
    /// Rectangle in client coordinates.
    /// </param>
    /// <returns>
    /// Rectangle in screen coordinates.
    /// </returns>
    public virtual Rectangle RectangleToScreen(Rectangle r)
    {
        if (Widget != null)
        {
            this.Widget.Window.GetPosition(out int x, out int y);
            if (r.X < x && r.Y < y)
                return new Rectangle(r.X + x, r.Y + y, r.Width, r.Height);
        }

        return new Rectangle(r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Causes the control to be redrawn.
    /// </summary>
    public virtual void Refresh()
    {
        if (this.Widget != null && this.Widget.IsVisible)
        {
            ISelf?.Override.OnAddClass();
            this.Widget.QueueDraw();
            if (this.Widget is Gtk.Container container)
            {
                foreach (var child in container.Children)
                    child.QueueDraw();
            }
        }
    }

    /// <summary>
    /// Resets the BackColor property to its default value.
    /// </summary>
    public virtual void ResetBackColor()
    {
    }

    /// <summary>
    /// Clears all data bindings on the control.
    /// </summary>
    public virtual void ResetBindings()
    {
    }

    /// <summary>
    /// Resets the Cursor property to its default value.
    /// </summary>
    public virtual void ResetCursor()
    {
    }

    /// <summary>
    /// Resets the Font property to its default value.
    /// </summary>
    public virtual void ResetFont()
    {
    }

    /// <summary>
    /// Resets the ForeColor property to its default value.
    /// </summary>
    public virtual void ResetForeColor()
    {
    }

    /// <summary>
    /// Resets the IME mode to its default value.
    /// </summary>
    public virtual void ResetImeMode()
    {
    }

    /// <summary>
    /// Resets the RightToLeft property to its default value.
    /// </summary>
    public virtual void ResetRightToLeft()
    {
    }

    /// <summary>
    /// Resets the Text property to its default value.
    /// </summary>
    public virtual void ResetText()
    {
    }

    /// <summary>
    /// Resumes normal layout logic.
    /// </summary>
    public virtual void ResumeLayout()
    {
        ResumeLayout(false);
    }

    /// <summary>
    /// Resumes layout logic and optionally forces an immediate layout.
    /// </summary>
    /// <param name = "performLayout">
    /// True to force a layout immediately.
    /// </param>
    public virtual void ResumeLayout(bool performLayout)
    {
        _Created = true;
    }

    /// <summary>
    /// Scales the control by the specified ratio.
    /// </summary>
    /// <param name = "ratio">
    /// Scale ratio.
    /// </param>
    public virtual void Scale(float ratio)
    {
    }

    /// <summary>
    /// Scales the control by the specified x and y factors.
    /// </summary>
    /// <param name = "dx">
    /// Horizontal scale factor.
    /// </param>
    /// <param name = "dy">
    /// Vertical scale factor.
    /// </param>
    public virtual void Scale(float dx, float dy)
    {
    }

    /// <summary>
    /// Scales the control by the specified factor.
    /// </summary>
    /// <param name = "factor">
    /// Scale factor.
    /// </param>
    public virtual void Scale(SizeF factor)
    {
    }

    /// <summary>
    /// Scales a logical bitmap to device pixels.
    /// </summary>
    /// <param name = "logicalBitmap">
    /// Bitmap to scale (in/out parameter).
    /// </param>
    public virtual void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
    {
    }

    /// <summary>
    /// Selects the control.
    /// </summary>
    public virtual void Select()
    {
        this.Widget?.SetStateFlags(StateFlags.Selected, true);
    }

    /// <summary>
    /// Selects the next control in the tab order.
    /// </summary>
    /// <param name = "ctl">
    /// Starting control.
    /// </param>
    /// <param name = "forward">
    /// True to move forward; false to move backward.
    /// </param>
    /// <param name = "tabStopOnly">
    /// True to consider only tab stop controls.
    /// </param>
    /// <param name = "nested">
    /// True to include nested child controls.
    /// </param>
    /// <param name = "wrap">
    /// True to wrap to the first control when the end is reached.
    /// </param>
    /// <returns>
    /// True if another control was selected; otherwise false.
    /// </returns>
    public virtual bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
    {
        return false;
    }

    /// <summary>
    /// Sends the control to the back of the z-order.
    /// </summary>
    public virtual void SendToBack()
    {
    }

    /// <summary>
    /// Sets the bounds of the control.
    /// </summary>
    /// <param name = "x">
    /// Left coordinate.
    /// </param>
    /// <param name = "y">
    /// Top coordinate.
    /// </param>
    /// <param name = "width">
    /// Width.
    /// </param>
    /// <param name = "height">
    /// Height.
    /// </param>
    public virtual void SetBounds(int x, int y, int width, int height)
    {
        SetBounds(x, y, width, height, BoundsSpecified.All);
    }

    /// <summary>
    /// Sets the bounds of the control with specified bounds flags.
    /// </summary>
    /// <param name = "x">
    /// Left coordinate.
    /// </param>
    /// <param name = "y">
    /// Top coordinate.
    /// </param>
    /// <param name = "width">
    /// Width.
    /// </param>
    /// <param name = "height">
    /// Height.
    /// </param>
    /// <param name = "specified">
    /// Which bounds components to set.
    /// </param>
    public virtual void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (this.Widget != null)
        {
            Gdk.Rectangle rect = this.Widget.Clip;
            if (specified == BoundsSpecified.X)
                rect.X = x;
            else if (specified == BoundsSpecified.Y)
                rect.Y = y;
            else if (specified == BoundsSpecified.Width)
                rect.Width = width;
            else if (specified == BoundsSpecified.Height)
                rect.Height = height;
            else if (specified == BoundsSpecified.Size)
            {
                rect.Width = width;
                rect.Height = height;
            }
            else if (specified == BoundsSpecified.Location)
            {
                rect.X = x;
                rect.Y = y;
            }
            else
            {
                rect.X = x;
                rect.Y = y;
                rect.Width = width;
                rect.Height = height;
            }

            this.Widget.SetClip(rect);
        }
    }

    /// <summary>
    /// Sets the bounds of the control using a rectangle and specified components.
    /// </summary>
    /// <param name = "bounds">
    /// Bounds rectangle.
    /// </param>
    /// <param name = "specified">
    /// Which bounds components to set.
    /// </param>
    public void SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Shows the control.
    /// </summary>
    public virtual void Show()
    {
        this.Widget?.ShowAll();
    }

    /// <summary>
    /// Suspends layout logic for the control.
    /// </summary>
    public virtual void SuspendLayout()
    {
        _Created = false;
    }

    /// <summary>
    /// Updates the control and processes pending paint requests.
    /// </summary>
    public virtual void Update()
    {
        if (this.Widget != null)
        {
            this.Widget.Window.ProcessUpdates(true);
            this.Widget.QueueDraw();
        }
    }

    void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string propertyName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Releases unmanaged and optionally managed resources used by the control.
    /// </summary>
    /// <param name = "disposing">
    /// True to dispose managed resources as well as unmanaged.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            image?.Dispose();
            surface?.Dispose();
            context?.Dispose();
            if (this.Widget != null)
            {
                this.Widget.Destroy();
                this.GtkControl = null;
            }
        }
        catch
        {
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Initializes style information for the GTK widget.
    /// </summary>
    /// <param name = "widget">
    /// GTK widget to initialize style for.
    /// </param>

    //===================
    protected virtual void InitStyle(Gtk.Widget widget)
    {
        SetStyle(widget);
    }

    /// <summary>
    /// Raises the AnchorChanged event.
    /// </summary>
    protected virtual void OnAnchorChanged(System.EventArgs e)
    {
        AnchorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the AutoSizeChanged event.
    /// </summary>
    protected virtual void OnAutoSizeChanged(System.EventArgs e)
    {
        AutoSizeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BackColorChanged event.
    /// </summary>
    protected virtual void OnBackColorChanged(System.EventArgs e)
    {
        BackColorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BackgroundImageChanged event.
    /// </summary>
    protected virtual void OnBackgroundImageChanged(System.EventArgs e)
    {
        BackgroundImageChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BackgroundImageLayoutChanged event.
    /// </summary>
    protected virtual void OnBackgroundImageLayoutChanged(System.EventArgs e)
    {
        BackgroundImageLayoutChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the BindingContextChanged event.
    /// </summary>
    protected virtual void OnBindingContextChanged(System.EventArgs e)
    {
        BindingContextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CausesValidationChanged event.
    /// </summary>
    protected virtual void OnCausesValidationChanged(System.EventArgs e)
    {
        CausesValidationChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ChangeUICues event.
    /// </summary>
    protected virtual void OnChangeUICues(System.Windows.Forms.UICuesEventArgs e)
    {
        ChangeUICues?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Click"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ClientSizeChanged event.
    /// </summary>
    protected virtual void OnClientSizeChanged(System.EventArgs e)
    {
        ClientSizeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ContextMenuStripChanged event.
    /// </summary>
    protected virtual void OnContextMenuStripChanged(System.EventArgs e)
    {
        ContextMenuStripChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ControlAdded event.
    /// </summary>
    protected virtual void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
    {
        ControlAdded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ControlRemoved event.
    /// </summary>
    protected virtual void OnControlRemoved(System.Windows.Forms.ControlEventArgs e)
    {
        ControlRemoved?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CursorChanged event.
    /// </summary>
    protected virtual void OnCursorChanged(System.EventArgs e)
    {
        CursorChanged?.Invoke(this, e);
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
    /// Raises the DpiChangedAfterParent event.
    /// </summary>
    protected virtual void OnDpiChangedAfterParent(System.EventArgs e)
    {
        DpiChangedAfterParent?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DpiChangedBeforeParent event.
    /// </summary>
    protected virtual void OnDpiChangedBeforeParent(System.EventArgs e)
    {
        DpiChangedBeforeParent?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DragDrop event.
    /// </summary>
    protected virtual void OnDragDrop(System.Windows.Forms.DragEventArgs e)
    {
        DragDrop?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DragEnter event.
    /// </summary>
    protected virtual void OnDragEnter(System.Windows.Forms.DragEventArgs e)
    {
        DragEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DragLeave event.
    /// </summary>
    protected virtual void OnDragLeave(System.EventArgs e)
    {
        DragLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the DragOver event.
    /// </summary>
    protected virtual void OnDragOver(System.Windows.Forms.DragEventArgs e)
    {
        DragOver?.Invoke(this, e);
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
    /// Raises the FontChanged event.
    /// </summary>
    protected virtual void OnFontChanged(System.EventArgs e)
    {
        FontChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ForeColorChanged event.
    /// </summary>
    protected virtual void OnForeColorChanged(System.EventArgs e)
    {
        ForeColorChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the GiveFeedback event.
    /// </summary>
    protected virtual void OnGiveFeedback(System.Windows.Forms.GiveFeedbackEventArgs e)
    {
        GiveFeedback?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.GotFocus"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnGotFocus(EventArgs e)
    {
        GotFocus?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the HandleCreated event.
    /// </summary>
    protected virtual void OnHandleCreated(System.EventArgs e)
    {
        HandleCreated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the HandleDestroyed event.
    /// </summary>
    protected virtual void OnHandleDestroyed(System.EventArgs e)
    {
        HandleDestroyed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the HelpRequested event.
    /// </summary>
    protected virtual void OnHelpRequested(System.Windows.Forms.HelpEventArgs e)
    {
        HelpRequested?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ImeModeChanged event.
    /// </summary>
    protected virtual void OnImeModeChanged(System.EventArgs e)
    {
        ImeModeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the Invalidated event.
    /// </summary>
    protected virtual void OnInvalidated(System.Windows.Forms.InvalidateEventArgs e)
    {
        Invalidated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.KeyDown"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.KeyEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnKeyDown(KeyEventArgs e)
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
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.KeyUp"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.KeyEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnKeyUp(KeyEventArgs e)
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
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Load"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnLoad(EventArgs e)
    {
        Load?.Invoke(this, e);
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
    /// Raises the MarginChanged event.
    /// </summary>
    protected virtual void OnMarginChanged(System.EventArgs e)
    {
        MarginChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the MouseCaptureChanged event.
    /// </summary>
    protected virtual void OnMouseCaptureChanged(System.EventArgs e)
    {
        MouseCaptureChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseClick"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseClick(MouseEventArgs e)
    {
        MouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseDoubleClick"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        MouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseDown"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDown(MouseEventArgs e)
    {
        MouseDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseEnter"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseEnter(EventArgs e)
    {
        MouseEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseHover"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseHover(EventArgs e)
    {
        MouseHover?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseLeave"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseLeave(EventArgs e)
    {
        MouseLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseMove"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseUp"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseUp(MouseEventArgs e)
    {
        MouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseWheel"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseWheel(MouseEventArgs e)
    {
        MouseWheel?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Move"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMove(EventArgs e)
    {
        Move?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the PaddingChanged event.
    /// </summary>
    protected virtual void OnPaddingChanged(System.EventArgs e)
    {
        PaddingChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Handles painting logic for derived controls.
    /// </summary>
    /// <param name = "e">
    /// Paint event arguments.
    /// </param>
    protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
    }

    /// <summary>
    /// Raises the ParentChanged event.
    /// </summary>
    protected virtual void OnParentChanged(EventArgs e)
    {
        ParentChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the PreviewKeyDown event.
    /// </summary>
    protected virtual void OnPreviewKeyDown(System.Windows.Forms.PreviewKeyDownEventArgs e)
    {
        PreviewKeyDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the QueryAccessibilityHelp event.
    /// </summary>
    protected virtual void OnQueryAccessibilityHelp(System.Windows.Forms.QueryAccessibilityHelpEventArgs e)
    {
        QueryAccessibilityHelp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the QueryContinueDrag event.
    /// </summary>
    protected virtual void OnQueryContinueDrag(System.Windows.Forms.QueryContinueDragEventArgs e)
    {
        QueryContinueDrag?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RegionChanged event.
    /// </summary>
    protected virtual void OnRegionChanged(System.EventArgs e)
    {
        RegionChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Resize"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnResize(EventArgs e)
    {
        Resize?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RightToLeftChanged event.
    /// </summary>
    protected virtual void OnRightToLeftChanged(System.EventArgs e)
    {
        RightToLeftChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.SizeChanged"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnSizeChanged(EventArgs e)
    {
        SizeChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the StyleChanged event.
    /// </summary>
    protected virtual void OnStyleChanged(System.EventArgs e)
    {
        StyleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SystemColorsChanged event.
    /// </summary>
    protected virtual void OnSystemColorsChanged(System.EventArgs e)
    {
        SystemColorsChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the TabIndexChanged event.
    /// </summary>
    protected virtual void OnTabIndexChanged(System.EventArgs e)
    {
        TabIndexChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the TabStopChanged event.
    /// </summary>
    protected virtual void OnTabStopChanged(System.EventArgs e)
    {
        TabStopChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the TextChanged event.
    /// </summary>
    protected virtual void OnTextChanged(System.EventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Validated"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.ComponentModel.CancelEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidated(CancelEventArgs e)
    {
        Validated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Validating"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.ComponentModel.CancelEventArgs"/> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidating(CancelEventArgs e)
    {
        Validating?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.VisibleChanged"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnVisibleChanged(EventArgs e)
    {
        VisibleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Selects the control in a specified direction.
    /// </summary>
    /// <param name = "directed">
    /// If true, selection is directed.
    /// </param>
    /// <param name = "forward">
    /// Direction of selection.
    /// </param>
    protected virtual void Select(bool directed, bool forward)
    {
    }

    /// <summary>
    /// Applies styling to the GTK widget based on control properties.
    /// </summary>
    /// <param name = "widget">
    /// GTK widget to style.
    /// </param>
    protected virtual void SetStyle(Gtk.Widget widget)
    {
        StringBuilder style = new StringBuilder();
        if (widget is Gtk.Image)
        {
        }
        else
        {
            if (this.Image != null && this.Image.PixbufData != null)
            {
                string imgdir = $"Resources/{widget.WidgetPath.IterGetName(0)}";
                string imguri = $"{imgdir}/{widget.Name}_image.png";
                if (!File.Exists(imguri))
                {
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);
                    Gdk.Pixbuf imagepixbuf = new Gdk.Pixbuf(this.Image.PixbufData);
                    imagepixbuf.Save(imguri, "png");
                }

                style.AppendFormat("background:url(\"{0}\")", imguri);
                style.Append(" no-repeat");
                if (this.ImageAlign == ContentAlignment.TopLeft)
                {
                    style.Append(" top left");
                }
                else if (this.ImageAlign == ContentAlignment.TopCenter)
                {
                    style.Append(" top center");
                }
                else if (this.ImageAlign == ContentAlignment.TopRight)
                {
                    style.Append(" top right");
                }
                else if (this.ImageAlign == ContentAlignment.MiddleLeft)
                {
                    style.Append(" center left");
                }
                else if (this.ImageAlign == ContentAlignment.MiddleCenter)
                {
                    style.Append(" center center");
                }
                else if (this.ImageAlign == ContentAlignment.MiddleRight)
                {
                    style.Append(" center right");
                }
                else if (this.ImageAlign == ContentAlignment.BottomLeft)
                {
                    style.Append(" bottom left");
                }
                else if (this.ImageAlign == ContentAlignment.BottomCenter)
                {
                    style.Append(" bottom center");
                }
                else if (this.ImageAlign == ContentAlignment.BottomRight)
                {
                    style.Append(" bottom right");
                }
                else
                {
                    style.Append(" center center");
                }

                if (this.BackgroundImage != null && this.BackgroundImage.PixbufData != null)
                {
                    string bgimguri = $"{imgdir}/{widget.Name}_bgimage.png";
                    if (!File.Exists(bgimguri))
                    {
                        Gdk.Pixbuf bgpixbuf = new Gdk.Pixbuf(this.BackgroundImage.PixbufData);
                        bgpixbuf.Savev(bgimguri, "png", new string[] { }, new string[] { });
                    }

                    style.AppendFormat(",url(\"{0}\") repeat", bgimguri);
                }

                style.Append(";");
                style.Append("background-origin: padding-box;");
                style.Append("background-clip: padding-box;");
            }
            else if (this.BackgroundImage != null && this.BackgroundImage.PixbufData != null)
            {
                string bgimgdir = $"Resources/{widget.WidgetPath.IterGetName(0)}";
                string bgimguri = $"{bgimgdir}/{widget.Name}_bgimage.png";
                Gdk.Pixbuf bgpixbuf = new Gdk.Pixbuf(this.BackgroundImage.PixbufData);
                if (!File.Exists(bgimguri))
                {
                    if (!Directory.Exists(bgimgdir))
                        Directory.CreateDirectory(bgimgdir);
                    bgpixbuf.Savev(bgimguri, "png", new string[] { }, new string[] { });
                }

                style.AppendFormat("background-image:url(\"{0}\");", bgimguri);
                if (this.BackgroundImageLayout == ImageLayout.Tile)
                {
                    style.Append("background-repeat:repeat;");
                }
                else if (this.BackgroundImageLayout == ImageLayout.Zoom)
                {
                    style.Append("background-repeat:no-repeat;");
                    style.Append("background-size: contain;");
                    style.Append("background-position:center;");
                }
                else if (this.BackgroundImageLayout == ImageLayout.Stretch)
                {
                    style.Append("background-repeat:no-repeat;");
                    style.Append("background-size: cover;");
                    style.Append("background-position:center;");
                }
                else if (this.BackgroundImageLayout == ImageLayout.Center)
                {
                    style.Append("background-repeat:no-repeat;");
                    if (widget.HeightRequest < bgpixbuf.Height)
                        style.Append("background-position:top,center;");
                    else
                        style.Append("background-position:center,center;");
                }
                else
                {
                    style.Append("background-repeat:no-repeat;");
                }

                style.Append("background-origin: padding-box;");
                style.Append("background-clip: padding-box;");
                if (this.BackColor.Name != "0")
                {
                    Color backColor = this.BackColor;
                    string color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
                    style.AppendFormat("background-color:{0};", color);
                }
            }
            else if (this.BackColor.Name != "0")
            {
                Color backColor = this.BackColor;
                string color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
                style.AppendFormat("background-color:{0};background:{0};", color);
            }

            if (this.ForeColor.Name != "0")
            {
                Color foreColor = this.ForeColor;
                string color = $"rgba({foreColor.R},{foreColor.G},{foreColor.B},{foreColor.A})";
                style.AppendFormat("color:{0};", color);
            }

            if (this.Font != null)
            {
                Font font = this.Font;
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

            if (style.Length > 10)
            {
                string styleClassName = $"s{unique_key}";
                StringBuilder css = new StringBuilder();
                css.AppendLine($".{styleClassName}{{{style.ToString()}}}");
                if (widget is Gtk.TextView)
                {
                    css.AppendLine($".{styleClassName} text{{{style.ToString()}}}");
                    css.AppendLine($".{styleClassName} .view{{{style.ToString()}}}");
                }

                CssProvider provider = new CssProvider();
                if (provider.LoadFromData(css.ToString()))
                {
                    if (widget.StyleContext.HasClass(styleClassName))
                        widget.StyleContext.RemoveProvider(provider);
                    widget.StyleContext.AddProvider(provider, 900);
                    widget.StyleContext.AddClass(styleClassName);
                }
            }
        }
    }

    /// <summary>
    /// Sets the value of a ControlStyles flag.
    /// </summary>
    /// <param name = "styles">
    /// Styles to set.
    /// </param>
    /// <param name = "value">
    /// True to enable; false to disable.
    /// </param>
    protected virtual void SetStyle(ControlStyles styles, bool value)
    {
    }

    /// <summary>
    /// Updates the background styling when mapped.
    /// </summary>
    protected virtual void UpdateBackgroundStyle()
    {
        if (this.Widget != null && this.Widget.IsMapped)
            ISelf.Override.OnAddClass();
    }

    /// <summary>
    /// Updates the CSS/style for the control when mapped.
    /// </summary>
    protected virtual void UpdateStyle()
    {
        if (this.Widget != null && this.Widget.IsMapped)
            SetStyle(this.Widget);
    }

    /// <summary>
    /// Processes a Windows message for the control (placeholder).
    /// </summary>
    /// <param name = "m">
    /// Message to process.
    /// </param>
    protected virtual void WndProc(ref Message m)
    {
        //Console.WriteLine($"HWnd:{m.HWnd},WParam:{m.WParam},LParam:{m.LParam},Msg:{m.Msg}");
    }

    private void Override_PaintGraphics(Cairo.Context cr, Rectangle rec)
    {
        if (surface != null)
        {
            cr.Save();
            cr.SetSourceSurface(surface, 0, 0);
            cr.Paint();
            cr.Restore();
        }
    }

    private void SetAnchorStyles(Gtk.Widget widget, AnchorStyles anchorStyles)
    {
        if (anchorStyles.HasFlag(AnchorStyles.Left) && anchorStyles.HasFlag(AnchorStyles.Right))
        {
            widget.Halign = Gtk.Align.Fill;
        }
        else if (anchorStyles.HasFlag(AnchorStyles.Left))
        {
            widget.Halign = Gtk.Align.Start;
        }
        else if (anchorStyles.HasFlag(AnchorStyles.Right))
        {
            widget.Halign = Gtk.Align.End;
        }
        else
        {
            widget.Halign = Gtk.Align.Start;
        }

        if (anchorStyles.HasFlag(AnchorStyles.Top) && anchorStyles.HasFlag(AnchorStyles.Bottom))
        {
            widget.Valign = Gtk.Align.Fill;
        }
        else if (anchorStyles.HasFlag(AnchorStyles.Top))
        {
            widget.Valign = Gtk.Align.Start;
        }
        else if (anchorStyles.HasFlag(AnchorStyles.Bottom))
        {
            widget.Valign = Gtk.Align.End;
        }
        else
        {
            widget.Valign = Gtk.Align.Start;
        }
    }

    private void Widget_ButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        if (o is Gtk.Widget owidget && owidget.Window != null)
        {
            MouseButtons result = MouseButtons.None;
            if (args.Event.Button == 1)
                result = MouseButtons.Left;
            else if (args.Event.Button == 2)
                result = MouseButtons.Middle;
            else if (args.Event.Button == 3)
                result = MouseButtons.Right;
            owidget.Window.GetOrigin(out int x, out int y); // avoid event penetration errors
            MouseEventArgs mouseArgs = new MouseEventArgs(result, 1, (int)args.Event.XRoot - x, (int)args.Event.YRoot - y, 0);
            OnMouseDown(mouseArgs);
            MouseDown?.Invoke(this, mouseArgs);
        }
    }

    private void Widget_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (o is Gtk.Widget owidget && owidget.Window != null)
        {
            MouseButtons result = MouseButtons.None;
            if (args.Event.Button == 1)
                result = MouseButtons.Left;
            else if (args.Event.Button == 2)
                result = MouseButtons.Middle;
            else if (args.Event.Button == 3)
                result = MouseButtons.Right;
            owidget.Window.GetOrigin(out int x, out int y);
            MouseEventArgs mouseArgs = new MouseEventArgs(result, 1, (int)args.Event.XRoot - x, (int)args.Event.YRoot - y, 0);
            OnMouseUp(mouseArgs);
            MouseUp?.Invoke(this, mouseArgs);
            if (args.Event.Type == Gdk.EventType.TwoButtonPress || args.Event.Type == Gdk.EventType.DoubleButtonPress)
            {
                MouseEventArgs mouseArgs2 = new MouseEventArgs(result, 2, (int)args.Event.XRoot - x, (int)args.Event.YRoot - y, 0);
                OnMouseDoubleClick(mouseArgs2);
                MouseDoubleClick?.Invoke(this, mouseArgs2);
                DoubleClick?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnClick(EventArgs.Empty);
                MouseEventArgs mouseArgs3 = new MouseEventArgs(result, 1, (int)args.Event.XRoot - x, (int)args.Event.YRoot - y, 0);
                OnMouseClick(mouseArgs3);
                Click?.Invoke(this, EventArgs.Empty);
                MouseClick?.Invoke(this, mouseArgs3);
            }

            if (ContextMenuStrip != null)
            {
                if (args.Event.Button == 3)
                {
                    ContextMenuStrip.Widget.ShowAll();
                    ((Gtk.Menu)ContextMenuStrip.Widget).PopupAtPointer(args.Event);
                }
            }
        }
    }

    private void Widget_ConfigureEvent(object o, ConfigureEventArgs args)
    {
        OnMove(EventArgs.Empty);
        Move?.Invoke(this, args);
    }

    private void Widget_EnterNotifyEvent(object o, EnterNotifyEventArgs args)
    {
        if (Cursor != null)
        {
            if (Cursor.CursorType == Gdk.CursorType.CursorIsPixmap)
            {
                this.Widget.Window.Cursor = new Gdk.Cursor(((Gtk.Widget)o).Display, Cursor.CursorPixbuf, Cursor.CursorsXY.X, Cursor.CursorsXY.Y);
            }
            else
            {
                this.Widget.Window.Cursor = new Gdk.Cursor(((Gtk.Widget)o).Display, Cursor.CursorType);
            }
        }

        OnMouseEnter(EventArgs.Empty);
        OnMouseHover(EventArgs.Empty);
        Enter?.Invoke(this, args);
        MouseEnter?.Invoke(this, args);
        MouseHover?.Invoke(this, args);
    }

    private void Widget_FocusInEvent(object o, FocusInEventArgs args)
    {
        GotFocus?.Invoke(this, args);
    }

    private void Widget_FocusOutEvent(object o, FocusOutEventArgs args)
    {
        CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
        LostFocus?.Invoke(this, args);
        OnValidating(cancelEventArgs);
        Validating?.Invoke(this, cancelEventArgs);
        if (Validated != null && cancelEventArgs.Cancel == false)
        {
            OnValidated(cancelEventArgs);
            Validated(this, cancelEventArgs);
        }
    }

    private void Widget_KeyPressEvent(object o, Gtk.KeyPressEventArgs args)
    {
        if (KeyDown != null)
        {
            if (args.Event is Gdk.EventKey eventkey)
            {
                Keys keys = (Keys)eventkey.HardwareKeycode;
                if (eventkey.State.HasFlag(Gdk.ModifierType.Mod1Mask))
                    keys |= Keys.Alt;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ControlMask))
                    keys |= Keys.Control;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ShiftMask))
                    keys |= Keys.Shift;
                if (eventkey.State.HasFlag(Gdk.ModifierType.LockMask))
                    keys |= Keys.CapsLock;
                OnKeyDown(new KeyEventArgs(keys));
                KeyDown(this, new KeyEventArgs(keys));
            }
        }
    }

    private void Widget_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
    {
        Keys keys = (Keys)args.Event.HardwareKeycode;
        OnKeyUp(new KeyEventArgs(keys));
        KeyUp?.Invoke(this, new KeyEventArgs(keys));
        KeyPress?.Invoke(this, new KeyPressEventArgs(Convert.ToChar(keys)));
    }

    private void Widget_LeaveNotifyEvent(object o, LeaveNotifyEventArgs args)
    {
        if (Cursor != null)
        {
            this.Widget.Window.Cursor = null;
        }

        OnMouseLeave(EventArgs.Empty);
        Leave?.Invoke(this, EventArgs.Empty);
        MouseLeave?.Invoke(this, EventArgs.Empty);
    }

    private void Widget_MotionNotifyEvent(object o, MotionNotifyEventArgs args)
    {
        if (o is Gtk.Widget owidget && owidget.Window != null)
        {
            MouseButtons result = MouseButtons.None;
            // Console.WriteLine(args.Event.State);
            if (args.Event.State.HasFlag(Gdk.ModifierType.Button1Mask))
                result = MouseButtons.Left;
            else if (args.Event.State.HasFlag(Gdk.ModifierType.Button2Mask))
                result = MouseButtons.Middle;
            else if (args.Event.State.HasFlag(Gdk.ModifierType.Button3Mask))
                result = MouseButtons.Right;
            owidget.Window.GetOrigin(out int x, out int y);
            MouseEventArgs mouseArgs = new MouseEventArgs(result, 1, (int)args.Event.XRoot - x, (int)args.Event.YRoot - y, 1);
            OnMouseMove(mouseArgs);
            MouseMove?.Invoke(this, mouseArgs);
        }
    }

    private void Widget_Realized(object sender, EventArgs e)
    {
        if (WidgetRealized == false)
        {
            WidgetRealized = true;
            InitStyle((Gtk.Widget)sender);
            OnLoad(EventArgs.Empty);
            Load?.Invoke(this, e);
        }
    }

    private void Widget_ScrollEvent(object o, Gtk.ScrollEventArgs args)
    {
        MouseEventArgs mouseArgs = new MouseEventArgs(MouseButtons.None, 0, (int)args.Event.X, (int)args.Event.Y, (int)args.Event.DeltaY);
        OnMouseWheel(mouseArgs);
        MouseWheel?.Invoke(this, mouseArgs);
    }

    private void Widget_SizeAllocated(object o, SizeAllocatedArgs args)
    {
        OnResize(EventArgs.Empty);
        Resize?.Invoke(this, EventArgs.Empty);
        if (args.Allocation.Width != size_width || args.Allocation.Height != size_height)
        {
            size_width = args.Allocation.Width;
            size_height = args.Allocation.Height;
            OnSizeChanged(EventArgs.Empty);
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        if (args.Allocation.X != location_x || args.Allocation.Y != location_y)
        {
            location_x = args.Allocation.X;
            location_y = args.Allocation.Y;
            LocationChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
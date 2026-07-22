/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using Cairo;
using Gdk;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using Color = System.Drawing.Color;
using Container = Gtk.Container;
using Image = System.Drawing.Image;
using Path = System.IO.Path;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Region = System.Drawing.Region;
using Size = System.Drawing.Size;

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
public partial class Control : Component, IControl, ISynchronizeInvoke, IComponent, IDisposable, ISupportInitialize, IArrangedElement, IBindableComponent, IWin32Window
{
    /// <summary>
    /// Maps GTK key symbols onto the Windows virtual key codes used by
    /// <see cref="T:System.Windows.Forms.Keys" />.
    /// </summary>
    internal static Dictionary<Gdk.Key, int> keyboardMap = new Dictionary<Gdk.Key, int>();

    /// <summary>
    /// Optional hook invoked for every raw GTK widget event before it is translated into
    /// WinForms events. Used by derived controls that need access to the unfiltered event stream.
    /// </summary>
    public Action<Gtk.Widget, Gtk.WidgetEventArgs>? WidgetEventDo;

    internal bool _Created;

    internal bool LockLocation = false;// because the code executes sequentially， special locking

    Context? context;

    Surface? surface;

    // Off-screen backing surface for CreateGraphics. It must NOT be tied to the live GDK Window:
    // deriving the draw surface from Widget.Window (as upstream/gitee does) corrupts GTK's own
    // rendering when CreateGraphics runs inside the realize/Load path of a modal dialog, throwing a
    // NullReferenceException out of Gtk.Dialog.Run(). See RegionTests.
    ImageSurface? image;

    private AnchorStyles _anchor;

    private bool _autoSize;

    private Image? _backgroundImage;

    private ImageLayout _backgroundImageLayout;

    private BorderStyle _BorderStyle;

    private Cursor? _Cursor;

    private DockStyle _dock;

    private Font? _Font;

    private Color _ForeColor;

    private Image? _image;

    private Size _size;

    private readonly Gtk.Application app = Application.Init();

    private readonly EventHandlerList handlerList = new EventHandlerList();

    private bool Is_DoubleButtonPress = false;

    private readonly object paintEventHandler_paint = new object();

    private readonly CssProvider provider = new CssProvider();

    private int location_x = 0;

    private int location_y = 0;

    private int size_height = 0;

    private int size_width = 0;

    private bool WidgetRealized = false;

    /// <summary>
    /// Occurs when the Anchor property value changes.
    /// </summary>
    public virtual event EventHandler? AnchorChanged;

    /// <summary>
    /// Occurs when the AutoSize property value changes.
    /// </summary>
    public virtual event EventHandler? AutoSizeChanged;

    /// <summary>
    /// Occurs when the BackColor property value changes.
    /// </summary>
    public virtual event EventHandler? BackColorChanged;

    /// <summary>
    /// Occurs when the BackgroundImage property value changes.
    /// </summary>
    public virtual event EventHandler? BackgroundImageChanged;

    /// <summary>
    /// Occurs when the BackgroundImageLayout property value changes.
    /// </summary>
    public virtual event EventHandler? BackgroundImageLayoutChanged;

    /// <summary>
    /// Occurs when the BindingContext property value changes.
    /// </summary>
    public virtual event EventHandler? BindingContextChanged;

    /// <summary>
    /// Occurs when the CausesValidation property value changes.
    /// </summary>
    public virtual event EventHandler? CausesValidationChanged;

    /// <summary>
    /// Occurs when the focus cues (keyboard, etc.) change.
    /// </summary>
    public virtual event UICuesEventHandler? ChangeUICues;

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public virtual event EventHandler? Click;

    /// <summary>
    /// Occurs when the ClientSize property value changes.
    /// </summary>
    public virtual event EventHandler? ClientSizeChanged;

    /// <summary>
    /// Occurs when the ContextMenuStrip property value changes.
    /// </summary>
    public virtual event EventHandler? ContextMenuStripChanged;

    /// <summary>
    /// Occurs when a control is added to the Controls collection.
    /// </summary>
    public virtual event ControlEventHandler? ControlAdded;

    /// <summary>
    /// Occurs when a control is removed from the Controls collection.
    /// </summary>
    public virtual event ControlEventHandler? ControlRemoved;

    /// <summary>
    /// Occurs when the Cursor property value changes.
    /// </summary>
    public virtual event EventHandler? CursorChanged;

    /// <summary>
    /// Occurs when the Dock property value changes.
    /// </summary>
    public virtual event EventHandler? DockChanged;

    /// <summary>
    /// Occurs when the control is double-clicked.
    /// </summary>
    public virtual event EventHandler? DoubleClick;

    /// <summary>
    /// Occurs after the DPI of the parent has changed.
    /// </summary>
    public virtual event EventHandler? DpiChangedAfterParent;

    /// <summary>
    /// Occurs before the DPI of the parent changes.
    /// </summary>
    public virtual event EventHandler? DpiChangedBeforeParent;

    /// <summary>
    /// Occurs when an object is dragged and dropped onto the control.
    /// </summary>
    public virtual event DragEventHandler? DragDrop;

    /// <summary>
    /// Occurs when an object is dragged into the control's bounds.
    /// </summary>
    public virtual event DragEventHandler? DragEnter;

    /// <summary>
    /// Occurs when an object is dragged out of the control's bounds.
    /// </summary>
    public virtual event EventHandler? DragLeave;

    /// <summary>
    /// Occurs when an object is dragged over the control.
    /// </summary>
    public virtual event DragEventHandler? DragOver;

    /// <summary>
    /// Occurs when the Enabled property value changes.
    /// </summary>
    public virtual event EventHandler? EnabledChanged;

    /// <summary>
    /// Occurs when the control receives input focus.
    /// </summary>
    public virtual event EventHandler? Enter;

    /// <summary>
    /// Occurs when the Font property value changes.
    /// </summary>
    public virtual event EventHandler? FontChanged;

    /// <summary>
    /// Occurs when the ForeColor property value changes.
    /// </summary>
    public virtual event EventHandler? ForeColorChanged;

    /// <summary>
    /// Occurs when a drag-and-drop operation reports feedback.
    /// </summary>
    public virtual event GiveFeedbackEventHandler? GiveFeedback;

    /// <summary>
    /// Occurs when the control receives input focus.
    /// </summary>
    public virtual event EventHandler? GotFocus;

    /// <summary>
    /// Occurs when the underlying handle is created.
    /// </summary>
    public virtual event EventHandler? HandleCreated;

    /// <summary>
    /// Occurs when the underlying handle is destroyed.
    /// </summary>
    public virtual event EventHandler? HandleDestroyed;

    /// <summary>
    /// Occurs when the user requests help for the control.
    /// </summary>
    public virtual event HelpEventHandler? HelpRequested;

    /// <summary>
    /// Occurs when the ImeMode property value changes.
    /// </summary>
    public virtual event EventHandler? ImeModeChanged;

    /// <summary>
    /// Occurs when the control is invalidated.
    /// </summary>
    public virtual event InvalidateEventHandler? Invalidated;

    /// <summary>
    /// Occurs when a key is pressed while the control has focus.
    /// </summary>
    public virtual event KeyEventHandler? KeyDown;

    /// <summary>
    /// Occurs when a key is pressed and then released while the control has focus.
    /// </summary>
    public virtual event KeyPressEventHandler? KeyPress;

    /// <summary>
    /// Occurs when a key is released while the control has focus.
    /// </summary>
    public virtual event KeyEventHandler? KeyUp;

    /// <summary>
    /// Occurs when the layout of the control changes.
    /// </summary>
    public virtual event LayoutEventHandler? Layout;

    /// <summary>
    /// Occurs when the control loses input focus.
    /// </summary>
    public virtual event EventHandler? Leave;

    /// <summary>
    /// Occurs when the control is loaded (realized).
    /// </summary>
    //public virtual event EventHandler Disposed;
    public virtual event EventHandler? Load;

    /// <summary>
    /// Occurs when the Location property value changes.
    /// </summary>
    public virtual event EventHandler? LocationChanged;

    /// <summary>
    /// Occurs when the control loses input focus.
    /// </summary>
    public virtual event EventHandler? LostFocus;

    /// <summary>
    /// Occurs when the Margin property value changes.
    /// </summary>
    public virtual event EventHandler? MarginChanged;

    /// <summary>
    /// Occurs when the mouse capture state changes.
    /// </summary>
    public virtual event EventHandler? MouseCaptureChanged;

    /// <summary>
    /// Occurs when the control is clicked with the mouse.
    /// </summary>
    public virtual event MouseEventHandler? MouseClick;

    /// <summary>
    /// Occurs when the control is double-clicked with the mouse.
    /// </summary>
    public virtual event MouseEventHandler? MouseDoubleClick;

    /// <summary>
    /// Occurs when a mouse button is pressed while the cursor is over the control.
    /// </summary>
    public virtual event MouseEventHandler? MouseDown;

    /// <summary>
    /// Occurs when the mouse pointer enters the control.
    /// </summary>
    public virtual event EventHandler? MouseEnter;

    /// <summary>
    /// Occurs when the mouse pointer hovers over the control.
    /// </summary>
    public virtual event EventHandler? MouseHover;

    /// <summary>
    /// Occurs when the mouse pointer leaves the control.
    /// </summary>
    public virtual event EventHandler? MouseLeave;

    /// <summary>
    /// Occurs when the mouse pointer moves over the control.
    /// </summary>
    public virtual event MouseEventHandler? MouseMove;

    /// <summary>
    /// Occurs when a mouse button is released while the cursor is over the control.
    /// </summary>
    public virtual event MouseEventHandler? MouseUp;

    /// <summary>
    /// Occurs when the mouse wheel moves while the pointer is over the control.
    /// </summary>
    public virtual event MouseEventHandler? MouseWheel;

    /// <summary>
    /// Occurs when the control is moved.
    /// </summary>
    public virtual event EventHandler? Move;

    /// <summary>
    /// Occurs when the Padding property value changes.
    /// </summary>
    public virtual event EventHandler? PaddingChanged;

    /// <summary>
    /// Occurs when the Parent property value changes.
    /// </summary>
    //public virtual event PaintEventHandler Paint;
    public virtual event EventHandler? ParentChanged;

    /// <summary>
    /// Occurs when a key is pressed while the control has focus, and allows previewing the key press.
    /// </summary>
    public virtual event PreviewKeyDownEventHandler? PreviewKeyDown;

    /// <summary>
    /// Occurs when accessibility help is requested for the control.
    /// </summary>
    public virtual event QueryAccessibilityHelpEventHandler? QueryAccessibilityHelp;

    /// <summary>
    /// Occurs during a drag operation to determine whether the drag should continue.
    /// </summary>
    public virtual event QueryContinueDragEventHandler? QueryContinueDrag;

    /// <summary>
    /// Occurs when the Region property value changes.
    /// </summary>
    public virtual event EventHandler? RegionChanged;

    /// <summary>
    /// Occurs when the control is resized.
    /// </summary>
    public virtual event EventHandler? Resize;

    /// <summary>
    /// Occurs when the RightToLeft property value changes.
    /// </summary>
    public virtual event EventHandler? RightToLeftChanged;

    /// <summary>
    /// Occurs when the Size property value changes.
    /// </summary>
    public virtual event EventHandler? SizeChanged;

    /// <summary>
    /// Occurs when the control's style changes.
    /// </summary>
    public virtual event EventHandler? StyleChanged;

    /// <summary>
    /// Occurs when the system colors change.
    /// </summary>
    public virtual event EventHandler? SystemColorsChanged;

    /// <summary>
    /// Occurs when the TabIndex property value changes.
    /// </summary>
    public virtual event EventHandler? TabIndexChanged;

    /// <summary>
    /// Occurs when the TabStop property value changes.
    /// </summary>
    public virtual event EventHandler? TabStopChanged;

    /// <summary>
    /// Occurs when the Text property value changes.
    /// </summary>
    public virtual event EventHandler? TextChanged;

    /// <summary>
    /// Occurs when the control is validated.
    /// </summary>
    public virtual event EventHandler? Validated;

    /// <summary>
    /// Occurs when validating the control; allows cancellation.
    /// </summary>
    public virtual event CancelEventHandler? Validating;

    /// <summary>
    /// Occurs when the Visible property value changes.
    /// </summary>
    public virtual event EventHandler? VisibleChanged;

    /// <summary>
    /// Occurs when the control needs to be painted.
    /// </summary>
    public virtual event PaintEventHandler Paint
    {
        add { handlerList.AddHandler(paintEventHandler_paint, value); }
        remove { handlerList.RemoveHandler(paintEventHandler_paint, value); }
    }

    /// <summary>
    /// Initializes the shared GTK key symbol to virtual key code translation table.
    /// </summary>
    static Control()
    {
        if (keyboardMap.Count == 0)
        {
            keyboardMap = new Dictionary<Gdk.Key, int>
            {
                { Gdk.Key.BackSpace, 8 },
                { Gdk.Key.Tab, 9 },
                { Gdk.Key.ISO_Left_Tab, 9 },
                { Gdk.Key.KP_Tab, 9 },
                { Gdk.Key.Return, 13 },
                { Gdk.Key.KP_Enter, 13 },
                { Gdk.Key.Shift_L, 16 },
                { Gdk.Key.Shift_R, 161 },
                { Gdk.Key.Shift_Lock, 10 },
                { Gdk.Key.Control_L, 17 },
                { Gdk.Key.Control_R, 163 },
                { Gdk.Key.Alt_L, 18 },
                { Gdk.Key.Alt_R, 165 },
                { Gdk.Key.Pause, 19 },
                { Gdk.Key.Caps_Lock, 20 },
                { Gdk.Key.VoidSymbol, 20 },
                { Gdk.Key.Escape, 27 },
                { Gdk.Key.space, 32 },
                { Gdk.Key.Prior, 33 },
                { Gdk.Key.KP_Prior, 33 },
                { Gdk.Key.Next, 34 },
                { Gdk.Key.KP_Page_Down, 34 },
                { Gdk.Key.End, 35 },
                { Gdk.Key.KP_End, 35 },
                { Gdk.Key.Home, 36 },
                { Gdk.Key.KP_Home, 36 },
                { Gdk.Key.Left, 37 },
                { Gdk.Key.KP_Left, 37 },
                { Gdk.Key.Up, 38 },
                { Gdk.Key.KP_Up, 38 },
                { Gdk.Key.Right, 39 },
                { Gdk.Key.KP_Right, 39 },
                { Gdk.Key.Down, 40 },
                { Gdk.Key.KP_Down, 40 },
                { Gdk.Key.Insert, 45 },
                { Gdk.Key.Delete, 46 },
                { Gdk.Key.KP_Delete, 46 },
                { Gdk.Key.Key_0, 48 },
                { Gdk.Key.Key_1, 49 },
                { Gdk.Key.Key_2, 50 },
                { Gdk.Key.Key_3, 51 },
                { Gdk.Key.Key_4, 52 },
                { Gdk.Key.Key_5, 53 },
                { Gdk.Key.Key_6, 54 },
                { Gdk.Key.Key_7, 55 },
                { Gdk.Key.Key_8, 56 },
                { Gdk.Key.Key_9, 57 },
                { Gdk.Key.parenright, 48 },
                { Gdk.Key.exclam, 49 },
                { Gdk.Key.at, 50 },
                { Gdk.Key.numbersign, 51 },
                { Gdk.Key.dollar, 52 },
                { Gdk.Key.percent, 53 },
                { Gdk.Key.asciicircum, 54 },
                { Gdk.Key.ampersand, 55 },
                { Gdk.Key.asterisk, 56 },
                { Gdk.Key.parenleft, 57 },
                { Gdk.Key.A, 65 },
                { Gdk.Key.B, 66 },
                { Gdk.Key.C, 67 },
                { Gdk.Key.D, 68 },
                { Gdk.Key.E, 69 },
                { Gdk.Key.F, 70 },
                { Gdk.Key.G, 71 },
                { Gdk.Key.H, 72 },
                { Gdk.Key.I, 73 },
                { Gdk.Key.J, 74 },
                { Gdk.Key.K, 75 },
                { Gdk.Key.L, 76 },
                { Gdk.Key.M, 77 },
                { Gdk.Key.N, 78 },
                { Gdk.Key.O, 79 },
                { Gdk.Key.P, 80 },
                { Gdk.Key.Q, 81 },
                { Gdk.Key.R, 82 },
                { Gdk.Key.S, 83 },
                { Gdk.Key.T, 84 },
                { Gdk.Key.U, 85 },
                { Gdk.Key.V, 86 },
                { Gdk.Key.W, 87 },
                { Gdk.Key.X, 88 },
                { Gdk.Key.Y, 89 },
                { Gdk.Key.Z, 90 },
                { Gdk.Key.a, 65 },
                { Gdk.Key.b, 66 },
                { Gdk.Key.c, 67 },
                { Gdk.Key.d, 68 },
                { Gdk.Key.e, 69 },
                { Gdk.Key.f, 70 },
                { Gdk.Key.g, 71 },
                { Gdk.Key.h, 72 },
                { Gdk.Key.i, 73 },
                { Gdk.Key.j, 74 },
                { Gdk.Key.k, 75 },
                { Gdk.Key.l, 76 },
                { Gdk.Key.m, 77 },
                { Gdk.Key.n, 78 },
                { Gdk.Key.o, 79 },
                { Gdk.Key.p, 80 },
                { Gdk.Key.q, 81 },
                { Gdk.Key.r, 82 },
                { Gdk.Key.s, 83 },
                { Gdk.Key.t, 84 },
                { Gdk.Key.u, 85 },
                { Gdk.Key.v, 86 },
                { Gdk.Key.w, 87 },
                { Gdk.Key.x, 88 },
                { Gdk.Key.y, 89 },
                { Gdk.Key.z, 90 },
                { Gdk.Key.Meta_L, 91 },
                { Gdk.Key.Meta_R, 92 },
                { Gdk.Key.approximate, 93 },
                { Gdk.Key.KP_0, 96 },
                { Gdk.Key.KP_1, 97 },
                { Gdk.Key.KP_2, 98 },
                { Gdk.Key.KP_3, 99 },
                { Gdk.Key.KP_4, 100 },
                { Gdk.Key.KP_5, 101 },
                { Gdk.Key.KP_6, 102 },
                { Gdk.Key.KP_7, 103 },
                { Gdk.Key.KP_8, 104 },
                { Gdk.Key.KP_9, 105 },
                { Gdk.Key.KP_Multiply, 106 },
                { Gdk.Key.KP_Add, 107 },
                { Gdk.Key.KP_Subtract, 109 },
                { Gdk.Key.KP_Decimal, 110 },
                { Gdk.Key.KP_Divide, 111 },
                { Gdk.Key.F1, 112 },
                { Gdk.Key.F2, 113 },
                { Gdk.Key.F3, 114 },
                { Gdk.Key.F4, 115 },
                { Gdk.Key.F5, 116 },
                { Gdk.Key.F6, 117 },
                { Gdk.Key.F7, 118 },
                { Gdk.Key.F8, 119 },
                { Gdk.Key.F9, 120 },
                { Gdk.Key.F10, 121 },
                { Gdk.Key.F11, 122 },
                { Gdk.Key.F12, 123 },
                { Gdk.Key.Num_Lock, 144 },
                { Gdk.Key.Scroll_Lock, 145 },
                { Gdk.Key.period, 190 },
                { Gdk.Key.greater, 190 },
                { Gdk.Key.slash, 191 },
                { Gdk.Key.question, 191 },
                { Gdk.Key.quoteleft, 192 },
                { Gdk.Key.asciitilde, 192 },
                { Gdk.Key.braceleft, 219 },
                { Gdk.Key.bracketleft, 219 },
                { Gdk.Key.backslash, 220 },
                { Gdk.Key.bar, 220 },
                { Gdk.Key.braceright, 221 },
                { Gdk.Key.bracketright, 221 },
                { Gdk.Key.apostrophe, 222 },
                { Gdk.Key.quotedbl, 222 },
                { Gdk.Key.semicolon, 186 },
                { Gdk.Key.colon, 186 },
                { Gdk.Key.equal, 187 },
                { Gdk.Key.plus, 187 },
                { Gdk.Key.comma, 188 },
                { Gdk.Key.less, 188 },
                { Gdk.Key.minus, 189 },
                { Gdk.Key.underscore, 189 },
            };
        }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Control" /> class.
    /// Sets up GTK event handlers and default style information.
    /// </summary>
    public Control()
    {
        this.unique_key = Guid.NewGuid().ToString().ToLower();

        // Initialize the child collection so tests using `control.Controls` work
        // (tests expect Controls to be non-null).
        Controls = new ControlCollection(this);

        var widget = this.Widget;
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
            widget.Realized += Widget_Realized;
            if (ISelf != null)
            {
                ISelf.Override.PaintGraphics += Override_PaintGraphics;
                ISelf.Override.Paint += Override_Paint;
            }
            widget.SizeAllocated += Widget_SizeAllocated;
            widget.ParentSet += Widget_ParentSet;
            widget.WidgetEvent += Widget_WidgetEvent;
            this.CursorChanged += Control_CursorChanged;
            this.DockChanged += Control_DockChanged;
            this.AnchorChanged += Control_AnchorChanged;
        }
        GetService(this.GetType());
    }

    /// <summary>
    /// Gets the accessible object for the control.
    /// </summary>
    public virtual AccessibleObject? AccessibilityObject { get; }

    /// <summary>
    /// Gets or sets the default action description for accessibility.
    /// </summary>
    public virtual string? AccessibleDefaultActionDescription
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the accessible description for the control.
    /// </summary>
    public virtual string? AccessibleDescription
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the accessible name for the control.
    /// </summary>
    public virtual string? AccessibleName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the accessible role for the control.
    /// </summary>
    public virtual AccessibleRole AccessibleRole
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control allows drag-and-drop operations.
    /// </summary>
    public virtual bool AllowDrop
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets how the control is anchored to its parent container.
    /// </summary>
    [Browsable(true)]
    [Description("The way the control is anchored to the edges of its parent container, and the related margins.")]
    public virtual AnchorStyles Anchor
    {
        get => _anchor;
        set
        {
            _anchor = value;
            OnAnchorChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the offset of the auto-scroll position.
    /// </summary>
    public virtual Point AutoScrollOffset
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is automatically resized to fit its contents.
    /// </summary>
    [Browsable(true)]
    [Description("Whether the control is automatically resized to fit its contents.")]
    public virtual bool AutoSize
    {
        get => _autoSize;
        set
        {
            _autoSize = value;
        }
    }

    /// <summary>
    /// Gets or sets the background color of the control.
    /// </summary>
    [Browsable(true)]
    [Description("The background color of the control.")]
    public virtual Color BackColor
    {
        get
        {
            if (ISelf?.Override.BackColor.HasValue == true)
                return ISelf.Override.BackColor.Value;
            else
                return Color.FromName("0");
        }
        set
        {
            if (ISelf != null)
            {
                ISelf.Override.BackColor = value;
                UpdateStyle();
                Refresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background image for the control.
    /// </summary>
    [Browsable(true)]
    [Description("The background image of the control.")]
    public virtual Image? BackgroundImage
    {
        get => _backgroundImage;
        set
        {
            _backgroundImage = value;
            if (ISelf != null)
            {
                ISelf.Override.BackgroundImage = value;
            }
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the layout for the background image.
    /// </summary>
    [Browsable(true)]
    [Description("The way the background image of the control is positioned.")]
    public virtual ImageLayout BackgroundImageLayout
    {
        get => _backgroundImageLayout;
        set
        {
            _backgroundImageLayout = value;
            if (ISelf != null)
            {
                ISelf.Override.BackgroundImageLayout = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the binding context for the control.
    /// </summary>
    public virtual BindingContext? BindingContext
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the border style for the control.
    /// </summary>
    public virtual BorderStyle BorderStyle
    {
        get { return _BorderStyle; }
        set
        {
            _BorderStyle = value;
            if (value == BorderStyle.FixedSingle)
            {
                var widget = this.Widget;
                if (widget != null)
                {
                    widget.StyleContext.RemoveClass("BorderFixed3D");
                    widget.StyleContext.RemoveClass("BorderNone");
                    widget.StyleContext.AddClass("BorderFixedSingle");
                }
            }
            else if (value == BorderStyle.Fixed3D)
            {
                var widget = this.Widget;
                if (widget != null)
                {
                    widget.StyleContext.RemoveClass("BorderFixedSingle");
                    widget.StyleContext.RemoveClass("BorderNone");
                    widget.StyleContext.AddClass("BorderFixed3D");
                }
            }
            else
            {
                var widget = this.Widget;
                if (widget != null)
                {
                    widget.StyleContext.RemoveClass("BorderFixedSingle");
                    widget.StyleContext.RemoveClass("BorderFixed3D");
                    widget.StyleContext.AddClass("BorderNone");
                }
            }
        }
    }

    /// <summary>
    /// Gets the bottom position of the control relative to its container.
    /// </summary>
    public virtual int Bottom
    {
        get => this.Widget != null ? this.Widget.MarginBottom : 0;
    }

    /// <summary>
    /// Gets or sets the bounds of the control.
    /// </summary>
    public virtual Rectangle Bounds
    {
        get
        {
            if (Widget != null)
                return new Rectangle(Left, Top, Width, Height);
            else
                return Rectangle.Empty;
        }
        set { SetBounds(value.X, value.Y, value.Width, value.Height); }
    }

    /// <summary>
    /// Gets a value indicating whether the control can receive focus.
    /// </summary>
    public virtual bool CanFocus { get { return this.Widget != null && this.Widget.CanFocus; } }

    /// <summary>
    /// Gets a value indicating whether the control can be selected.
    /// </summary>
    public virtual bool CanSelect { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the control has captured the mouse.
    /// </summary>
    public virtual bool Capture
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control causes validation on focus change.
    /// </summary>
    public virtual bool CausesValidation
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the arranged element collection for child layout.
    /// </summary>
    public ArrangedElementCollection? Children => Controls;

    /// <summary>
    /// Gets the client rectangle for the control.
    /// </summary>
    public virtual Rectangle ClientRectangle
    {
        get
        {
            if (this.Widget != null)
            {
                var allocation = this.Widget.Allocation;
                return new Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
    }

    /// <summary>
    /// Gets or sets the size of the client area.
    /// </summary>
    public virtual Size ClientSize
    {
        get
        {
            if (Widget != null) return new Size(Widget.AllocatedWidth, this.Widget.AllocatedHeight);
            else return Size.Empty;
        }
        set { this.Widget?.SetSizeRequest(value.Width, value.Height); }
    }

    /// <summary>
    /// Gets the company name associated with the control (if any).
    /// </summary>
    public virtual string? CompanyName { get; }

    /// <summary>
    /// Gets a value indicating whether the control or one of its child controls has input focus.
    /// </summary>
    public virtual bool ContainsFocus { get; }

    /// <summary>
    /// Gets or sets the context menu associated with the control.
    /// </summary>
    public virtual ContextMenuStrip? ContextMenuStrip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the collection of child controls.
    /// </summary>
    public virtual ControlCollection? Controls { get; }

    /// <summary>
    /// Gets a value indicating whether the control has been created.
    /// </summary>
    public virtual bool Created => _Created;

    /// <summary>
    /// Gets or sets the cursor for the control.
    /// </summary>
    public virtual Cursor? Cursor
    {
        get => _Cursor;
        set
        {
            _Cursor = value;
            OnCursorChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the collection of data bindings for the control.
    /// </summary>
    public virtual ControlBindingsCollection? DataBindings { get; }

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
    [Browsable(true)]
    [Description("The way the control is docked to the edges of its parent container, and the related margins.")]
    public virtual DockStyle Dock
    {
        get
        {
            return _dock;
        }
        set
        {
            _dock = value;
            OnDockChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    public virtual bool Enabled
    {
        get
        {
            var widget = this.Widget;
            return widget != null && widget.Sensitive;
        }
        set
        {
            var widget = this.Widget;
            widget?.Sensitive = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the control has input focus.
    /// </summary>
    public virtual bool Focused { get { return this.Widget?.IsFocus ?? false; } }

    /// <summary>
    /// Gets or sets the font of the control.
    /// </summary>
    [Browsable(true)]
    [Description("The font used by the characters of the control.")]
    public virtual Font Font
    {
        get
        {
            return _Font!;
        }
        set
        {
            _Font = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the control.
    /// </summary>
    public virtual Color ForeColor
    {
        get { return _ForeColor; }
        set { _ForeColor = value; UpdateStyle(); }
    }

    /// <summary>
    /// Gets the GTK container associated with this control, if any.
    /// </summary>
    public virtual Container? GtkContainer { get => GtkControl as Gtk.Container; }

    /// <summary>
    /// Gets or sets the underlying GTK control object.
    /// </summary>
    public virtual object? GtkControl
    {
        get;
        set;
    }

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
            var widget = this.Widget;
            if (widget == null)
                return 0;
            if (widget.IsMapped == false)
            {
                if (widget is Gtk.Window wnd)
                    return wnd.HeightRequest == -1 ? wnd.DefaultHeight : wnd.HeightRequest;
                else
                    return widget.HeightRequest;
            }
            else if (widget.Valign == Align.Fill)
                return widget.AllocatedHeight;
            else
                return widget.HeightRequest == -1 ? widget.AllocatedHeight : widget.HeightRequest;
        }
        set
        {
            var widget = this.Widget;
            widget?.HeightRequest = Math.Max(-1, value);
        }
    }

    /// <summary>
    /// Gets or sets the primary image associated with the control.
    /// </summary>
    [Browsable(true)]
    [Description("The image displayed by the control.")]
    public virtual Image? Image
    {
        get => _image;
        set
        {
            _image = value;
            UpdateStyle();
        }
    }

    /// <summary>
    /// Gets or sets the alignment of the image within the control.
    /// </summary>
    [Browsable(true)]
    [Description("The way the image of the control is positioned.")]
    public virtual System.Drawing.ContentAlignment ImageAlign
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the Input Method Editor (IME) mode for the control.
    /// </summary>
    public virtual ImeMode ImeMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether calls to the control must be marshaled to the UI thread.
    /// </summary>
    public virtual bool InvokeRequired { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is accessible.
    /// </summary>
    public virtual bool IsAccessible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether the control has been disposed.
    /// </summary>
    public virtual bool IsDisposed { get; internal set; }

    /// <summary>
    /// Gets the IControlGtk wrapper for the underlying GTK control.
    /// </summary>
    public virtual IControlGtk? ISelf { get => GtkControl as IControlGtk; }

    /// <summary>
    /// Gets a value indicating whether the underlying handle has been created (realized).
    /// </summary>
    public virtual bool IsHandleCreated { get => this.Widget != null && this.Widget.Handle != IntPtr.Zero; }

    /// <summary>
    /// Gets a value indicating whether the control is mirrored for right-to-left languages.
    /// </summary>
    public virtual bool IsMirrored { get; }

    /// <summary>
    /// Gets the layout engine used by the control.
    /// </summary>
    public virtual LayoutEngine? LayoutEngine { get; }

    /// <summary>
    /// Gets or sets the left position of the control relative to its container.
    /// </summary>
    public virtual int Left
    {
        get => this.Widget?.MarginStart ?? 0;
        set
        {
            var widget = this.Widget;
            widget?.MarginStart = Math.Max(0, value);
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
    public virtual Padding Margin
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum size the control can be resized to.
    /// </summary>
    public virtual Size MaximumSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum size the control can be resized to.
    /// </summary>
    public virtual Size MinimumSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the modifier keys (Ctrl, Alt, Shift) that are currently held down.
    /// </summary>
    public static Keys ModifierKeys
    {
        get
        {
            Gdk.Display.Default.GetPointer(out var x, out var y, out ModifierType mod);
            var result = Keys.None;
            if (mod.HasFlag(Gdk.ModifierType.ControlMask))
                result |= Keys.Control;
            else if (mod.HasFlag(Gdk.ModifierType.Mod1Mask))
                result |= Keys.Alt;
            else if (mod.HasFlag(Gdk.ModifierType.ShiftMask))
                result |= Keys.Shift;
            return result;
        }
    }

    /// <summary>
    /// Gets the mouse buttons that are currently held down.
    /// </summary>
    public static MouseButtons MouseButtons
    {
        get
        {
            Gdk.Display.Default.GetPointer(out var x, out var y, out ModifierType mod);
            var result = MouseButtons.None;
            if (mod.HasFlag(Gdk.ModifierType.Button1Mask))
                result |= MouseButtons.Left;
            else if (mod.HasFlag(Gdk.ModifierType.Button2Mask))
                result |= MouseButtons.Middle;
            else if (mod.HasFlag(Gdk.ModifierType.Button3Mask))
                result |= MouseButtons.Right;
            return result;
        }
    }

    /// <summary>
    /// Gets the current position of the mouse pointer in screen coordinates.
    /// </summary>
    public static Point MousePosition
    {
        get
        {
            Gdk.Display.Default.GetPointer(out var x, out var y, out ModifierType mod);
            return new Point(x, y);
        }
    }

    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    public virtual string Name
    {
        get { return this.Widget?.Name ?? string.Empty; }
        set { this.Widget?.Name = value; }
    }

    /// <summary>
    /// Gets or sets the padding within the control.
    /// </summary>
    public virtual Padding Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the parent control.
    /// </summary>
    public virtual Control? Parent
    {
        get;
        set;
    }

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
    public virtual string? ProductName { get; }

    /// <summary>
    /// Gets the product version associated with the control (if any).
    /// </summary>
    public virtual string? ProductVersion { get; }

    /// <summary>
    /// Gets a value indicating whether the control is recreating its handle.
    /// </summary>
    public virtual bool RecreatingHandle { get; }

    /// <summary>
    /// Gets or sets the region describing the window region of the control.
    /// </summary>
    public virtual Region? Region
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the right position of the control relative to its container.
    /// </summary>
    public virtual int Right
    {
        get => this.Widget?.MarginEnd ?? 0;
    }

    /// <summary>
    /// Gets or sets the reading order of the control.
    /// </summary>
    public virtual RightToLeft RightToLeft
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the size of the control.
    /// </summary>
    [Browsable(true)]
    [Description("The width and height of the control.")]
    public virtual Size Size
    {
        get
        {
            return new Size(Width, Height);
        }
        set
        {
            _size = value;
            Width = value.Width;
            Height = value.Height;
            if (this.Widget != null && this.Widget.IsRealized && this.Widget.Parent is Gtk.Container con)
            {
                con.QueueResize();
                con.ResizeChildren();
            }
        }
    }

    /// <summary>
    /// Gets or sets the tab index of the control.
    /// </summary>
    public virtual int TabIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is included in tab navigation.
    /// </summary>
    public virtual bool TabStop
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets an object that contains data about the control.
    /// </summary>
    public virtual object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text associated with the control.
    /// </summary>
    public virtual string Text
    {
        get;
        set;
    } = null!;

    /// <summary>
    /// Gets or sets the top position of the control relative to its container.
    /// </summary>
    public virtual int Top
    {
        get => this.Widget?.MarginTop ?? 0;
        set
        {
            this.Widget?.MarginTop = Math.Max(0, value);
        }
    }

    /// <summary>
    /// Gets the top-level control that contains this control.
    /// </summary>
    public virtual Control? TopLevelControl { get; }

    /// <summary>
    /// Gets a unique key associated with the control (used for styling or resources).
    /// </summary>
    public string unique_key
    {
        get;
        protected set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control should use the visual style background color.
    /// </summary>
    public virtual bool UseVisualStyleBackColor
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the wait cursor should be used for the control.
    /// </summary>
    public virtual bool UseWaitCursor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is visible.
    /// </summary>
    public virtual bool Visible
    {
        get { return this.Widget?.Visible ?? false; }
        set
        {
            if (this.Widget != null)
            {
                this.Widget.NoShowAll = value == false;
                if (value == true)
                {
                    this.Widget.ShowAll();
                }
                else
                {
                    this.Widget.Visible = value;
                }
            }
        }
    }

    /// <summary>
    /// Gets the visual style background color for the control.
    /// </summary>
    public virtual Color VisualStyleBackColor { get; }

    /// <summary>
    /// Gets the underlying GTK widget for this control.
    /// </summary>
    public virtual Widget? Widget { get => GtkControl as Gtk.Widget; }

    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    public virtual int Width
    {
        get
        {
            var widget = this.Widget;
            if (widget == null)
                return 0;
            if (widget.IsMapped == false)
            {
                if (widget is Gtk.Window wnd)
                    return wnd.WidthRequest == -1 ? wnd.DefaultWidth : wnd.WidthRequest;
                else
                    return widget.WidthRequest;
            }
            else if (widget.Halign == Align.Fill)
                return widget.AllocatedWidth;
            else
                return widget.WidthRequest == -1 ? widget.AllocatedWidth : widget.WidthRequest;
        }
        set
        {
            this.Widget?.WidthRequest = Math.Max(-1, value);
        }
    }

    /// <summary>
    /// Gets or sets the window target for the control.
    /// </summary>
    public virtual IWindowTarget? WindowTarget
    {
        get;
        set;
    }

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

    IArrangedElement IArrangedElement.Container => this.Parent!;

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
            var createParams = new CreateParams();
            createParams.ExStyle |= 32;

            return createParams;
        }
    }

    /// <summary>
    /// Gets the default cursor for the control.
    /// </summary>
    protected virtual Cursor? DefaultCursor { get; }

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
    protected virtual bool DoubleBuffered
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the font height for internal layout calculations.
    /// </summary>
    protected int FontHeight
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the base IME mode for the control.
    /// </summary>
    protected virtual ImeMode ImeModeBase
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control should be redrawn on resize.
    /// </summary>
    protected bool ResizeRedraw
    {
        get;
        set;
    }

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
    /// <param name="method">
    /// Delegate to invoke asynchronously.
    /// </param>
    /// <param name="args">
    /// Arguments to pass to the delegate.
    /// </param>
    /// <returns>
    /// An IAsyncResult representing the asynchronous operation.
    /// </returns>
    public virtual IAsyncResult BeginInvoke(Delegate method, params object[]? args)
    {
        var task = System.Threading.Tasks.Task<object?>.Factory.StartNew(state =>
        {
            return method.DynamicInvoke((object[])state!);
        }, args);

        return task;
    }

    /// <summary>
    /// Begins an asynchronous operation on the control's thread.
    /// </summary>
    /// <param name="method">
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
    /// <param name="method">
    /// Action to invoke asynchronously.
    /// </param>
    /// <returns>
    /// An IAsyncResult representing the asynchronous operation.
    /// </returns>
    public virtual IAsyncResult BeginInvoke(Action method)
    {
        var task = System.Threading.Tasks.Task.Factory.StartNew(method);

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
    /// <param name="ctl">
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
                image = new Cairo.ImageSurface(Cairo.Format.Argb32, this.Widget?.AllocatedWidth ?? 0, this.Widget?.AllocatedHeight ?? 0);

            surface?.Dispose();
            surface = image.CreateSimilar(Cairo.Content.ColorAlpha, this.Widget?.AllocatedWidth ?? 0, this.Widget?.AllocatedHeight ?? 0);
            context?.Dispose();
            context = new Cairo.Context(surface);
            return new Drawing.Graphics(this.Widget, context, this.Widget?.Allocation ?? default);
        }
        catch (Exception ex)
        {
            GtkLogger.Info(" canvas creation failed：" + ex.Message);
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
        IsDisposed = true;
    }

    /// <summary>
    /// Initiates a drag-and-drop operation.
    /// </summary>
    /// <param name="data">
    /// Data to drag.
    /// </param>
    /// <param name="allowedEffects">
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
    /// <param name="bitmap">
    /// Bitmap to draw into.
    /// </param>
    /// <param name="targetBounds">
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
    /// <param name="asyncResult">
    /// The IAsyncResult returned from BeginInvoke.
    /// </param>
    /// <returns>
    /// The result from the asynchronous operation if any.
    /// </returns>
    public virtual object EndInvoke(IAsyncResult asyncResult)
    {
        if (asyncResult is System.Threading.Tasks.Task<object?> task1)
        {
            try
            {
                if (task1.IsCompleted == true)
                {
                    return task1.Result!;
                }
                else if (task1.IsCanceled == true)
                {
                    return null!;
                }
                else if (task1.IsFaulted == true)
                {
                    return task1.Exception!;
                }
                else
                {
                    return task1.GetAwaiter().GetResult()!;
                }
            }
            finally
            {
                task1.Dispose();
            }
        }
        else if (asyncResult is System.Threading.Tasks.Task task2)
        {
            try
            {
                if (task2.IsFaulted == true)
                {
                    return task2.Exception!;
                }

                return null!;
            }
            finally
            {
                task2.Dispose();
            }
        }

        return null!;
    }

    /// <summary>
    /// Retrieves the form that the control is on.
    /// </summary>
    /// <returns>
    /// The parent Form, or null if none is found.
    /// </returns>
    public virtual Form? FindForm()
    {
        if (this.Widget?.Toplevel.Data.ContainsKey("Control") ?? false)
        {
            return this.Widget.Toplevel.Data["Control"] as Form;
        }
        else
        {
            var control = this.Parent;
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
    /// <param name="pt">
    /// Point to test, in client coordinates.
    /// </param>
    /// <returns>
    /// The child control at the point, or null if none.
    /// </returns>
    public virtual Control? GetChildAtPoint(Point pt)
    {
        return null;
    }

    /// <summary>
    /// Gets the child control at the specified point, with skip options.
    /// </summary>
    /// <param name="pt">
    /// Point to test, in client coordinates.
    /// </param>
    /// <param name="skipValue">
    /// Specifies which children to skip.
    /// </param>
    /// <returns>
    /// The child control at the point, or null if none.
    /// </returns>
    public virtual Control? GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
    {
        return null;
    }

    /// <summary>
    /// Returns the container control that the control is associated with.
    /// </summary>
    /// <returns>
    /// An IContainerControl if the control implements it; otherwise the control itself.
    /// </returns>
    public virtual IContainerControl? GetContainerControl()
    {
        return this as IContainerControl;
    }

    /// <summary>
    /// Gets the next control in the tab order.
    /// </summary>
    /// <param name="ctl">
    /// The starting control.
    /// </param>
    /// <param name="forward">
    /// True to move forward; false to move backward.
    /// </param>
    /// <returns>
    /// The next control in the tab order, or null if none.
    /// </returns>
    public virtual Control? GetNextControl(Control ctl, bool forward)
    {
        Control? prev = null;
        var finded = false;

        var controlCollection = this.Controls;
        if (controlCollection != null)
            foreach (var obj in controlCollection)
            {
                if (obj is Control control)
                {
                    if (finded == true)
                        return control;
                    else if (control.Widget?.Handle == ctl.Widget?.Handle)
                    {
                        finded = true;
                        if (forward == false)
                            return prev;
                    }

                    prev = control;
                }
            }

        return null;
    }

    /// <summary>
    /// Returns the preferred size for the control given the proposed size.
    /// </summary>
    /// <param name="proposedSize">
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
        this.Widget?.Hide();
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
    /// <param name="invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(bool invalidateChildren)
    {
        if (Widget != null)
            Invalidate(
                new Rectangle(Widget.Allocation.X, Widget.Allocation.Y, Widget.Allocation.Width,
                    Widget.Allocation.Height), invalidateChildren);
    }

    /// <summary>
    /// Invalidates the specified rectangle of the control.
    /// </summary>
    /// <param name="rc">
    /// Rectangle to invalidate, in client coordinates.
    /// </param>
    public virtual void Invalidate(Rectangle rc)
    {
        Invalidate(rc, true);
    }

    /// <summary>
    /// Invalidates the specified rectangle of the control and optionally its children.
    /// </summary>
    /// <param name="rc">
    /// Rectangle to invalidate, in client coordinates.
    /// </param>
    /// <param name="invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(Rectangle rc, bool invalidateChildren)
    {
        if (this.Widget != null)
        {
            this.Widget.Window?.InvalidateRect(new Gdk.Rectangle(rc.X, rc.Y, rc.Width, rc.Height), invalidateChildren);
            this.Widget.SetStateFlags(StateFlags.Backdrop, true);
            GLib.Timeout.Add(50, () =>
            {
                this.Widget?.SetStateFlags(Gtk.StateFlags.Prelight, true);
                return false;
            });
        }
    }

    /// <summary>
    /// Invalidates the specified region of the control.
    /// </summary>
    /// <param name="region">
    /// Region to invalidate.
    /// </param>
    public virtual void Invalidate(Drawing.Region region)
    {
        Invalidate(region, true);
    }

    /// <summary>
    /// Invalidates the specified region of the control and optionally its children.
    /// </summary>
    /// <param name="region">
    /// Region to invalidate.
    /// </param>
    /// <param name="invalidateChildren">
    /// True to invalidate child controls as well.
    /// </param>
    public virtual void Invalidate(Drawing.Region region, bool invalidateChildren)
    {
        if (this.Widget != null)
        {
            Invalidate(
                new Rectangle(this.Widget.Allocation.X, this.Widget.Allocation.Y, this.Widget.Allocation.Width,
                    this.Widget.Allocation.Height), invalidateChildren);
        }
    }

    /// <summary>
    /// Invokes a delegate on the control's thread and returns its result.
    /// </summary>
    /// <param name="method">
    /// Delegate to invoke.
    /// </param>
    /// <returns>
    /// The result returned by the delegate.
    /// </returns>
    public virtual object? Invoke(Delegate method)
    {
        return Invoke(method, null);
    }

    /// <summary>
    /// Invokes a delegate on the control's thread and returns its result.
    /// </summary>
    /// <param name="method">
    /// Delegate to invoke.
    /// </param>
    /// <param name="args">
    /// Arguments to pass to the delegate.
    /// </param>
    /// <returns>
    /// The result returned by the delegate.
    /// </returns>
    public virtual object? Invoke(Delegate method, params object[]? args)
    {
        lock (this)
        {
            object? result = null;
            GLib.Idle.Add(() =>
            {
                result = method?.DynamicInvoke(args);
                return false;
            });

            return result;
        }
    }

    /// <summary>
    /// Invokes an action on the control's thread.
    /// </summary>
    /// <param name="method">
    /// Action to invoke.
    /// </param>
    public virtual void Invoke(Action method)
    {
        lock (this)
        {
            GLib.Idle.Add(() =>
            {
                method?.Invoke();
                return false;
            });
        }
    }

    /// <summary>
    /// Invokes a function on the control's thread and returns typed result.
    /// </summary>
    /// <typeparam name="TEntry">
    /// Return type.
    /// </typeparam>
    /// <param name="method">
    /// Function to invoke.
    /// </param>
    /// <returns>
    /// Result of the function.
    /// </returns>
    public virtual TEntry? Invoke<TEntry>(Func<TEntry> method)
    {
        lock (this)
        {
            var result = default(TEntry);
            GLib.Idle.Add(() =>
            {
                result = method.Invoke();
                return false;
            });

            return result;
        }
    }

    /// <summary>
    /// Converts a logical unit value to device units.
    /// </summary>
    /// <param name="value">
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
    /// <param name="value">
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
    /// <param name="affectedControl">
    /// Control affected by the layout.
    /// </param>
    /// <param name="affectedProperty">
    /// Property name that changed.
    /// </param>
    public virtual void PerformLayout(Control affectedControl, string affectedProperty)
    {
        _Created = true;
    }

    /// <summary>
    /// Converts a screen point to client coordinates.
    /// </summary>
    /// <param name="p">
    /// Point in screen coordinates.
    /// </param>
    /// <returns>
    /// Point in client coordinates.
    /// </returns>
    public virtual Point PointToClient(Point p)
    {
        if (Widget != null && this.Widget.Window != null)
        {
            this.Widget.Window.GetOrigin(out var x, out var y);
            return new Point(p.X - x, p.Y - y);
        }

        return new Point(p.X, p.Y);
    }

    /// <summary>
    /// Converts a client point to screen coordinates.
    /// </summary>
    /// <param name="p">
    /// Point in client coordinates.
    /// </param>
    /// <returns>
    /// Point in screen coordinates.
    /// </returns>
    public virtual Point PointToScreen(Point p)
    {
        if (Widget != null && this.Widget.Window != null)
        {
            this.Widget.Window.GetOrigin(out var x, out var y);
            return new Point(p.X + x, p.Y + y);
        }

        return new Point(p.X, p.Y);
    }

    /// <summary>
    /// Allows preprocessing of Windows messages for the control.
    /// </summary>
    /// <param name="msg">
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
    /// <param name="msg">
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
    /// <param name="r">
    /// Rectangle in screen coordinates.
    /// </param>
    /// <returns>
    /// Rectangle in client coordinates.
    /// </returns>
    public virtual Rectangle RectangleToClient(Rectangle r)
    {
        if (Widget != null && this.Widget.Window != null)
        {
            this.Widget.Window.GetOrigin(out var x, out var y);
            if (r.X > x && r.Y > y)
                return new Rectangle(r.X - x, r.Y - y, r.Width, r.Height);
        }

        return new Rectangle(r.X, r.Y, r.Width, r.Height);
    }

    /// <summary>
    /// Converts a rectangle from client coordinates to screen coordinates.
    /// </summary>
    /// <param name="r">
    /// Rectangle in client coordinates.
    /// </param>
    /// <returns>
    /// Rectangle in screen coordinates.
    /// </returns>
    public virtual Rectangle RectangleToScreen(Rectangle r)
    {
        if (Widget != null && this.Widget.Window != null)
        {
            this.Widget.Window.GetOrigin(out var x, out var y);
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
            this.Widget.QueueDraw();
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
    /// <param name="performLayout">
    /// True to force a layout immediately.
    /// </param>
    public virtual void ResumeLayout(bool performLayout)
    {
        _Created = true;
    }

    /// <summary>
    /// Scales the control by the specified ratio.
    /// </summary>
    /// <param name="ratio">
    /// Scale ratio.
    /// </param>
    public virtual void Scale(float ratio)
    {

    }

    /// <summary>
    /// Scales the control by the specified x and y factors.
    /// </summary>
    /// <param name="dx">
    /// Horizontal scale factor.
    /// </param>
    /// <param name="dy">
    /// Vertical scale factor.
    /// </param>
    public virtual void Scale(float dx, float dy)
    {

    }

    /// <summary>
    /// Scales the control by the specified factor.
    /// </summary>
    /// <param name="factor">
    /// Scale factor.
    /// </param>
    public virtual void Scale(SizeF factor)
    {

    }

    /// <summary>
    /// Scales a logical bitmap to device pixels.
    /// </summary>
    /// <param name="logicalBitmap">
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
    /// <param name="ctl">
    /// Starting control.
    /// </param>
    /// <param name="forward">
    /// True to move forward; false to move backward.
    /// </param>
    /// <param name="tabStopOnly">
    /// True to consider only tab stop controls.
    /// </param>
    /// <param name="nested">
    /// True to include nested child controls.
    /// </param>
    /// <param name="wrap">
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
        if (this.Widget != null && this.Widget.IsRealized && this.Widget.Window != null)
            this.Widget.Window.KeepBelow = true;
    }

    /// <summary>
    /// Sets the bounds of the control.
    /// </summary>
    /// <param name="x">
    /// Left coordinate.
    /// </param>
    /// <param name="y">
    /// Top coordinate.
    /// </param>
    /// <param name="width">
    /// Width.
    /// </param>
    /// <param name="height">
    /// Height.
    /// </param>
    public virtual void SetBounds(int x, int y, int width, int height)
    {
        SetBounds(x, y, width, height, BoundsSpecified.All);
    }

    /// <summary>
    /// Sets the bounds of the control with specified bounds flags.
    /// </summary>
    /// <param name="x">
    /// Left coordinate.
    /// </param>
    /// <param name="y">
    /// Top coordinate.
    /// </param>
    /// <param name="width">
    /// Width.
    /// </param>
    /// <param name="height">
    /// Height.
    /// </param>
    /// <param name="specified">
    /// Which bounds components to set.
    /// </param>
    public virtual void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if (this.Widget != null)
        {
            var rect = this.Widget.Allocation;
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
            this.Left = rect.X;
            this.Top = rect.Y;
            this.Size = new Size(rect.Width, rect.Height);
        }
    }

    /// <summary>
    /// Sets the bounds of the control using a rectangle and specified components.
    /// </summary>
    /// <param name="bounds">
    /// Bounds rectangle.
    /// </param>
    /// <param name="specified">
    /// Which bounds components to set.
    /// </param>
    public void SetBounds(Rectangle bounds, BoundsSpecified specified)
    {
        if (specified.HasFlag(BoundsSpecified.Location) || specified.HasFlag(BoundsSpecified.All))
            this.Location = bounds.Location;
        else if (specified.HasFlag(BoundsSpecified.X))
            this.Left = bounds.Left;
        else if (specified.HasFlag(BoundsSpecified.Y))
            this.Top = bounds.Top;

        if (specified.HasFlag(BoundsSpecified.Size) || specified.HasFlag(BoundsSpecified.All))
            this.Size = bounds.Size;
        else if (specified.HasFlag(BoundsSpecified.Width))
            this.Width = bounds.Width;
        else if (specified.HasFlag(BoundsSpecified.Height))
            this.Height = bounds.Height;
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
        if (this.Widget != null && this.Widget.Window != null)
        {
            this.Widget.Window.ProcessUpdates(true);
            this.Widget.QueueDraw();
        }
    }

    void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string? propertyName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Releases unmanaged and optionally managed resources used by the control.
    /// </summary>
    /// <param name="disposing">
    /// True to dispose managed resources as well as unmanaged.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            surface?.Dispose();
            context?.Dispose();
            _image?.Dispose();
            _backgroundImage?.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Dispose", ex);
        }
        try
        {
            if (Controls != null)
            {
                foreach (Control? c in Controls)
                    c?.Dispose();
                Controls.Clear();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Dispose", ex);
        }
        GC.SuppressFinalize(this);
        base.Dispose(disposing);
        GtkControl = null;
    }

    /// <summary>
    /// Initializes style information for the GTK widget.
    /// </summary>
    /// <param name="widget">
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
    /// <see cref="T:System.Windows.Forms.Control.Click" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected internal virtual void OnClick(EventArgs e)
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
    /// <see cref="T:System.Windows.Forms.Control.GotFocus" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.KeyDown" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.KeyEventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.KeyUp" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.KeyEventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.Load" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.MouseClick" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseClick(MouseEventArgs e)
    {
        MouseClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseDoubleClick" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        MouseDoubleClick?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseDown" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseDown(MouseEventArgs e)
    {
        MouseDown?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseEnter" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseEnter(EventArgs e)
    {
        MouseEnter?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseHover" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseHover(EventArgs e)
    {
        MouseHover?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseLeave" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseLeave(EventArgs e)
    {
        MouseLeave?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseMove" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseUp" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseUp(MouseEventArgs e)
    {
        MouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.MouseWheel" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Windows.Forms.MouseEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMouseWheel(MouseEventArgs e)
    {
        MouseWheel?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Move" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
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
    /// <param name="pevent">
    /// Paint event arguments.
    /// </param>
    protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs pevent)
    {

    }

    /// <summary>
    /// Paints the background of the control.
    /// </summary>
    /// <param name="pevent">
    /// Paint event arguments.
    /// </param>
    protected virtual void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
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
    /// <see cref="T:System.Windows.Forms.Control.Resize" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.SizeChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
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
    /// <see cref="T:System.Windows.Forms.Control.Validated" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.ComponentModel.CancelEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidated(CancelEventArgs e)
    {
        Validated?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.Validating" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.ComponentModel.CancelEventArgs" /> instance containing the event data.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnValidating(CancelEventArgs e)
    {
        Validating?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.Control.VisibleChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnVisibleChanged(EventArgs e)
    {
        VisibleChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Selects the control in a specified direction.
    /// </summary>
    /// <param name="directed">
    /// If true, selection is directed.
    /// </param>
    /// <param name="forward">
    /// Direction of selection.
    /// </param>
    protected virtual void Select(bool directed, bool forward)
    {

    }

    /// <summary>
    /// Applies styling to the GTK widget based on control properties.
    /// </summary>
    /// <param name="widget">
    /// GTK widget to style.
    /// </param>
    protected virtual void SetStyle(Gtk.Widget widget)
    {
        var style = new StringBuilder();
        if (widget is Gtk.Image) { }
        else
        {
            if (this._image != null && this._image.PixbufData != null)
            {
                var imgdir = Path.Combine("Resources", widget.WidgetPath.IterGetName(0)).Replace("\\", "/");
                var imguri = $"{imgdir}/{widget.Name}_image.png";
                if (!File.Exists(imguri))
                {
                    if (!Directory.Exists(imgdir))
                        Directory.CreateDirectory(imgdir);
                    var imagepixbuf = new Gdk.Pixbuf(this._image.PixbufData);
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

                if (this._backgroundImage != null && this._backgroundImage.PixbufData != null)
                {
                    var bgimguri = $"{imgdir}/{widget.Name}_bgimage.png";
                    if (!File.Exists(bgimguri))
                    {
                        var bgpixbuf = new Gdk.Pixbuf(this._backgroundImage.PixbufData);
                        bgpixbuf.Savev(bgimguri, "png", new string[] { }, new string[] { });
                    }
                    style.AppendFormat(",url(\"{0}\") repeat", bgimguri);
                }
                style.Append(";");
                style.Append("background-origin: padding-box;");
                style.Append("background-clip: padding-box;");
            }
            else if (this._backgroundImage != null && this._backgroundImage.PixbufData != null)
            {
                var bgpixbuf = new Gdk.Pixbuf(this._backgroundImage.PixbufData);
                var bgimgdir = Path.Combine("Resources", widget.WidgetPath.IterGetName(0)).Replace("\\", "/");
                var bgimguri = $"{bgimgdir}/{widget.Name}_bgimage.png";
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
                    var backColor = this.BackColor;
                    var color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
                    style.AppendFormat("background-color:{0};", color);
                }
            }
            else if (this.BackColor.Name != "0")
            {
                var backColor = this.BackColor;
                var color = $"rgba({backColor.R},{backColor.G},{backColor.B},{backColor.A})";
                style.AppendFormat("background-color:{0};background:{0};", color);
            }

            if (this.ForeColor.Name != "0")
            {
                var foreColor = this.ForeColor;
                var color = $"rgba({foreColor.R},{foreColor.G},{foreColor.B},{foreColor.A})";
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
                if (widget is Gtk.TextView)
                {
                    css.AppendLine($".{styleClassName} text{{{style.ToString()}}}");
                    css.AppendLine($".{styleClassName} .view{{{style.ToString()}}}");
                }
                if (provider.LoadFromData(css.ToString()))
                {
                    if (widget.StyleContext.HasClass(styleClassName))
                    {
                        widget.StyleContext.RemoveClass(styleClassName);
                        widget.StyleContext.RemoveProvider(provider);
                    }
                    widget.StyleContext.AddProvider(provider, (uint)StyleProviderPriority.User);
                    widget.StyleContext.AddClass(styleClassName);
                }
            }
        }
    }

    /// <summary>
    /// Sets the value of a ControlStyles flag.
    /// </summary>
    /// <param name="styles">
    /// Styles to set.
    /// </param>
    /// <param name="value">
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
        if (this.Widget != null && this.Widget.IsMapped && ISelf != null) ISelf.Override.OnAddClass();
    }

    /// <summary>
    /// Updates the CSS/style for the control when mapped.
    /// </summary>
    protected virtual void UpdateStyle()
    {
        GLib.Timeout.Add(1, () =>
        {
            if (this.Widget != null && this.Widget.IsMapped)
                SetStyle(this.Widget);
            return false;
        });
    }

    /// <summary>
    /// Processes a Windows message for the control (placeholder).
    /// </summary>
    /// <param name="m">
    /// Message to process.
    /// </param>
    protected virtual void WndProc(ref Message m)
    {
        //GtkLogger.Info($"HWnd:{m.HWnd},WParam:{m.WParam},LParam:{m.LParam},Msg:{m.Msg}");
    }

    private void Control_AnchorChanged(object? sender, EventArgs e)
    {
        SetAnchorStyle(this.Widget, _anchor);
    }

    private void Control_CursorChanged(object? sender, EventArgs e)
    {
        SetCursorStyle();
    }

    private void Control_DockChanged(object? sender, EventArgs e)
    {
        SetDockStyle(this.Widget, _dock);
    }

    private void InitDockAnchorStyle()
    {
        if (_dock != DockStyle.None)
            OnDockChanged(EventArgs.Empty);
        else if (_anchor != AnchorStyles.None)
            OnAnchorChanged(EventArgs.Empty);
    }

    private void Override_Paint(object sender, PaintEventArgs e)
    {
        OnPaintBackground(e);
        OnPaint(e);
        handlerList[paintEventHandler_paint]?.DynamicInvoke(sender, e);
    }

    private void Override_PaintGraphics(object sender, PaintGraphicsEventArgs e)
    {
        var cr = e.Context;
        if (surface != null)
        {
            cr.Save();
            cr.SetSourceSurface(surface, 0, 0);
            cr.Paint();
            cr.Restore();
        }
    }

    private void SetAnchorStyle(Gtk.Widget? widget, AnchorStyles anchorStyles)
    {
        if (widget == null || widget is FormBase || widget.IsRealized == false)
            return;

        if (widget.Parent is Gtk.Overlay lay)
        {
            var framewidth = lay.AllocatedWidth;
            var frameheight = lay.AllocatedHeight;
            var widgetwidth = widget.WidthRequest > 1 ? widget.WidthRequest : widget.AllocatedWidth;
            var widgetheight = widget.HeightRequest > 1 ? widget.HeightRequest : widget.AllocatedHeight;
            if (anchorStyles.HasFlag(AnchorStyles.Left) && anchorStyles.HasFlag(AnchorStyles.Right))
            {
                widget.MarginEnd = Math.Max(0, framewidth - widget.MarginStart - widgetwidth);
                widget.Halign = Gtk.Align.Fill;
            }
            else if (anchorStyles.HasFlag(AnchorStyles.Left))
            {
                widget.Halign = Gtk.Align.Start;
            }
            else if (anchorStyles.HasFlag(AnchorStyles.Right))
            {
                widget.MarginEnd = Math.Max(0, framewidth - widget.MarginStart - widgetwidth);
                widget.Halign = Gtk.Align.End;
            }
            else
            {
                widget.Halign = Gtk.Align.Start;
            }

            if (anchorStyles.HasFlag(AnchorStyles.Top) && anchorStyles.HasFlag(AnchorStyles.Bottom))
            {
                widget.MarginBottom = Math.Max(0, frameheight - widget.MarginTop - widgetheight);
                widget.Valign = Gtk.Align.Fill;
            }
            else if (anchorStyles.HasFlag(AnchorStyles.Top))
            {
                widget.Valign = Gtk.Align.Start;
            }
            else if (anchorStyles.HasFlag(AnchorStyles.Bottom))
            {
                widget.MarginBottom = Math.Max(0, frameheight - widget.MarginTop - widgetheight);
                widget.Valign = Gtk.Align.End;
            }
            else
            {
                widget.Valign = Gtk.Align.Start;
            }
        }
    }

    private void SetCursorStyle()
    {
        var widget = this.Widget;
        if (widget != null && widget.IsRealized && widget.Window != null)
        {
            if (_Cursor == null)
            {
                if (widget.Window.Cursor != null)
                    widget.Window.Cursor = null;
            }
            else if (_Cursor.CursorType == Gdk.CursorType.CursorIsPixmap)
            {
                widget.Window.Cursor = new Gdk.Cursor(widget.Display, _Cursor.CursorPixbuf, _Cursor.CursorsXY.X, _Cursor.CursorsXY.Y);
            }
            else
            {
                widget.Window.Cursor = new Gdk.Cursor(widget.Display, _Cursor.CursorType);
            }
        }
    }

    private void SetDockStyle(Gtk.Widget? widget, DockStyle dockStyle)
    {
        if (widget == null || widget is FormBase || widget.IsRealized == false)
            return;

        if (widget.Parent is Gtk.Overlay lay)
        {
            if (dockStyle == DockStyle.Fill)
            {
                if (widget.WidthRequest + widget.MarginStart + widget.MarginStart > lay.AllocatedWidth)
                    widget.WidthRequest = Math.Max(10, lay.AllocatedWidth - widget.MarginStart - widget.MarginStart);
                if (widget.HeightRequest + widget.MarginTop + widget.MarginTop > lay.AllocatedHeight)
                    widget.HeightRequest = Math.Max(10, lay.AllocatedHeight - widget.MarginTop - widget.MarginTop);
                widget.MarginEnd = Math.Min(3, widget.MarginStart);
                widget.MarginBottom = Math.Min(3, widget.MarginTop);
                widget.Halign = Align.Fill;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Left)
            {
                if (widget.HeightRequest + widget.MarginTop + widget.MarginTop > lay.AllocatedHeight)
                    widget.HeightRequest = Math.Max(10, lay.AllocatedHeight - widget.MarginTop - widget.MarginTop);
                widget.MarginBottom = Math.Min(3, widget.MarginTop);
                widget.Halign = Align.Start;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Top)
            {
                if (widget.WidthRequest + widget.MarginStart + widget.MarginStart > lay.AllocatedWidth)
                    widget.WidthRequest = Math.Max(10, lay.AllocatedWidth - widget.MarginStart - widget.MarginStart);
                widget.MarginEnd = Math.Min(3, widget.MarginStart);
                widget.Halign = Align.Fill;
                widget.Valign = Align.Start;
            }
            else if (dockStyle == DockStyle.Right)
            {
                if (widget.HeightRequest + widget.MarginTop + widget.MarginTop > lay.AllocatedHeight)
                    widget.HeightRequest = Math.Max(10, lay.AllocatedHeight - widget.MarginTop - widget.MarginTop);
                widget.MarginBottom = Math.Min(3, widget.MarginTop);
                widget.Halign = Align.End;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Bottom)
            {
                if (widget.WidthRequest + widget.MarginStart + widget.MarginStart > lay.AllocatedWidth)
                    widget.WidthRequest = Math.Max(10, lay.AllocatedWidth - widget.MarginStart - widget.MarginStart);
                widget.MarginEnd = Math.Min(3, widget.MarginStart);
                widget.Halign = Align.Fill;
                widget.Valign = Align.End;
            }
            else
            {
                widget.Halign = Align.Start;
                widget.Valign = Align.Start;
            }
        }
        else if (widget.Parent is Gtk.Viewport view)
        {
            if (dockStyle == DockStyle.Fill)
            {
                widget.Halign = Align.Fill;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Left)
            {
                widget.Halign = Align.Start;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Top)
            {
                widget.Halign = Align.Fill;
                widget.Valign = Align.Start;
            }
            else if (dockStyle == DockStyle.Right)
            {
                widget.Halign = Align.End;
                widget.Valign = Align.Fill;
            }
            else if (dockStyle == DockStyle.Bottom)
            {
                widget.Halign = Align.Fill;
                widget.Valign = Align.End;
            }
            else
            {
                widget.Halign = Align.Start;
                widget.Valign = Align.Start;
            }
        }
    }

    private void Widget_ParentSet(object o, Gtk.ParentSetArgs args)
    {
        InitDockAnchorStyle();
        OnParentChanged(EventArgs.Empty);
    }

    private void Widget_Realized(object sender, EventArgs e)
    {
        if (WidgetRealized == false)
        {
            WidgetRealized = true;
            InitStyle((Gtk.Widget)sender);
            OnLoad(e);
            OnCursorChanged(EventArgs.Empty);
            InitDockAnchorStyle();
        }
    }

    private void Widget_SizeAllocated(object o, SizeAllocatedArgs args)
    {
        OnResize(EventArgs.Empty);
        if (args.Allocation.Width != size_width || args.Allocation.Height != size_height)
        {
            size_width = args.Allocation.Width;
            size_height = args.Allocation.Height;
            OnSizeChanged(EventArgs.Empty);
        }
        if (args.Allocation.X != location_x || args.Allocation.Y != location_y)
        {
            location_x = args.Allocation.X;
            location_y = args.Allocation.Y;
            OnLocationChanged(EventArgs.Empty);
        }
    }

    private void Widget_WidgetEvent(object o, Gtk.WidgetEventArgs args)
    {
        if (o is not Gtk.Widget owidget || owidget.IsRealized == false)
            return;

        WidgetEventDo?.Invoke(owidget, args);
        if (args.Event.Type == Gdk.EventType.MotionNotify)
        {
            if (args.Event is Gdk.EventMotion eventmotion && owidget.Window != null)
            {
                var clicks = 0;
                MouseButtons button = MouseButtons.None;
                if (eventmotion.State.HasFlag(Gdk.ModifierType.Button1Mask))
                {
                    button |= MouseButtons.Left;
                    clicks += 1;
                }
                if (eventmotion.State.HasFlag(Gdk.ModifierType.Button2Mask))
                {
                    button |= MouseButtons.Middle;
                    clicks += 1;
                }
                if (eventmotion.State.HasFlag(Gdk.ModifierType.Button3Mask))
                {
                    button |= MouseButtons.Right;
                    clicks += 1;
                }
                owidget.Window.GetOrigin(out var x, out var y);
                OnMouseMove(new MouseEventArgs(button, clicks, (int)eventmotion.XRoot - x, (int)eventmotion.YRoot - y, 1));
            }
        }
        else if (args.Event.Type == Gdk.EventType.KeyPress)
        {
            if (args.Event is Gdk.EventKey eventkey)
            {
                var keys = (Keys)eventkey.HardwareKeycode;
                keyboardMap.TryGetValue(eventkey.Key, out var keycode);
                if (keycode != 0)
                {
                    keys = (Keys)keycode;
                    if (eventkey.Key.Equals(Gdk.Key.VoidSymbol) && eventkey.HardwareKeycode == 20)
                        keys = Keys.CapsLock;
                }
                if (eventkey.State.HasFlag(Gdk.ModifierType.Mod1Mask))
                    keys |= Keys.Alt;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ControlMask))
                    keys |= Keys.Control;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ShiftMask))
                    keys |= Keys.Shift;
                if (eventkey.State.HasFlag(Gdk.ModifierType.LockMask))
                    keys |= Keys.CapsLock;
                var keyargs = new KeyEventArgs(keys);
                OnKeyDown(keyargs);
                args.RetVal = keyargs.SuppressKeyPress;
                if (!keyargs.SuppressKeyPress)
                {
                    var keypress = new KeyPressEventArgs(Convert.ToChar(keycode));
                    OnKeyPress(keypress);
                    args.RetVal = keypress.Handled;
                }
            }
        }
        else if (args.Event.Type == Gdk.EventType.KeyRelease)
        {
            if (args.Event is Gdk.EventKey eventkey)
            {
                var keys = (Keys)eventkey.HardwareKeycode;
                if (keyboardMap.TryGetValue(eventkey.Key, out var keycode))
                {
                    keys = (Keys)keycode;
                    if (eventkey.Key.Equals(Gdk.Key.VoidSymbol) && eventkey.HardwareKeycode == 20)
                        keys = Keys.CapsLock;
                }
                if (eventkey.State.HasFlag(Gdk.ModifierType.Mod1Mask))
                    keys |= Keys.Alt;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ControlMask))
                    keys |= Keys.Control;
                if (eventkey.State.HasFlag(Gdk.ModifierType.ShiftMask))
                    keys |= Keys.Shift;
                if (eventkey.State.HasFlag(Gdk.ModifierType.LockMask))
                    keys |= Keys.CapsLock;
                var keyargs = new KeyEventArgs(keys);
                OnKeyUp(keyargs);
                args.RetVal = keyargs.Handled;
            }
        }
        else if (args.Event.Type == Gdk.EventType.ButtonPress)
        {
            if (args.Event is Gdk.EventButton eventbutton && owidget.Window != null)
            {
                MouseButtons button = MouseButtons.None;
                if (eventbutton.Button == 1)
                    button = MouseButtons.Left;
                else if (eventbutton.Button == 2)
                    button = MouseButtons.Middle;
                else if (eventbutton.Button == 3)
                    button = MouseButtons.Right;
                owidget.Window.GetOrigin(out var x, out var y);
                OnMouseDown(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
            }
        }
        else if (args.Event.Type == Gdk.EventType.TwoButtonPress || args.Event.Type == Gdk.EventType.DoubleButtonPress)
        {
            if (args.Event is Gdk.EventButton eventbutton && eventbutton.Button == 1 && owidget.Window != null)
            {
                Is_DoubleButtonPress = true;
                MouseButtons button = MouseButtons.Left;
                owidget.Window.GetOrigin(out var x, out var y);
                OnMouseDoubleClick(new MouseEventArgs(button, 2, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
                OnDoubleClick(EventArgs.Empty);
            }
        }
        else if (args.Event.Type == Gdk.EventType.ButtonRelease)
        {
            if (args.Event is Gdk.EventButton eventbutton && owidget.Window != null
                && owidget.Allocation.Contains((int)eventbutton.X, (int)eventbutton.Y))
            {
                MouseButtons button = MouseButtons.None;
                if (eventbutton.Button == 1)
                    button = MouseButtons.Left;
                else if (eventbutton.Button == 2)
                    button = MouseButtons.Middle;
                else if (eventbutton.Button == 3)
                    button = MouseButtons.Right;

                owidget.Window.GetOrigin(out var x, out var y);
                if (eventbutton.Button == 1 && Is_DoubleButtonPress == false)
                {
                    OnClick(EventArgs.Empty);
                    OnMouseClick(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
                }
                OnMouseUp(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
                if (eventbutton.Button == 3)
                {
                    if (ContextMenuStrip != null)
                    {
                        ContextMenuStrip.Widget.ShowAll();
                        ((Gtk.Menu)ContextMenuStrip.Widget).PopupAtPointer(args.Event);
                        args.RetVal = true;
                    }
                }
                Is_DoubleButtonPress = false;
            }
        }
        else if (args.Event.Type == Gdk.EventType.TouchBegin)
        {
            if (args.Args[0] is Gdk.EventTouch eventbutton && owidget.Window != null)
            {
                MouseButtons button = MouseButtons.Left;
                owidget.Window.GetOrigin(out var x, out var y);
                OnMouseDown(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
            }
        }
        else if (args.Event.Type == Gdk.EventType.TouchUpdate)
        {
            if (args.Args[0] is Gdk.EventTouch eventmotion && owidget.Window != null)
            {
                var clicks = 0;
                MouseButtons button = MouseButtons.Left;
                owidget.Window.GetOrigin(out var x, out var y);
                OnMouseMove(new MouseEventArgs(button, clicks, (int)eventmotion.XRoot - x, (int)eventmotion.YRoot - y, 1));
            }
        }
        else if (args.Event.Type == Gdk.EventType.TouchEnd)
        {
            if (args.Args[0] is Gdk.EventTouch eventbutton && owidget.Window != null)
            {
                MouseButtons button = MouseButtons.Left;
                owidget.Window.GetOrigin(out var x, out var y);
                OnClick(EventArgs.Empty);
                OnMouseClick(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
                OnMouseUp(new MouseEventArgs(button, 1, (int)eventbutton.XRoot - x, (int)eventbutton.YRoot - y, 0));
            }
        }
        else if (args.Event.Type == Gdk.EventType.Configure)
        {
            OnMove(EventArgs.Empty);
        }
        else if (args.Event.Type == Gdk.EventType.EnterNotify)
        {
            OnMouseEnter(EventArgs.Empty);
            OnMouseHover(EventArgs.Empty);
            OnEnter(EventArgs.Empty);
        }
        else if (args.Event.Type == Gdk.EventType.LeaveNotify)
        {
            OnMouseLeave(EventArgs.Empty);
            OnLeave(EventArgs.Empty);
        }
        else if (args.Event.Type == Gdk.EventType.FocusChange)
        {
            if (args.Event is Gdk.EventFocus eventfocus)
            {
                if (eventfocus.In)
                {
                    OnGotFocus(EventArgs.Empty);
                }
                else
                {
                    var cancelEventArgs = new CancelEventArgs(false);
                    OnLostFocus(EventArgs.Empty);
                    OnValidating(cancelEventArgs);
                    if (Validated != null && cancelEventArgs.Cancel == false)
                    {
                        OnValidated(cancelEventArgs);
                    }
                }
            }
        }
        else if (args.Event.Type == Gdk.EventType.Scroll)
        {
            if (args.Event is Gdk.EventScroll eventscroll)
            {
                var clicks = 0;
                MouseButtons button = MouseButtons.None;
                if (eventscroll.State.HasFlag(Gdk.ModifierType.Button1Mask))
                {
                    button |= MouseButtons.Left;
                    clicks += 1;
                }
                if (eventscroll.State.HasFlag(Gdk.ModifierType.Button2Mask))
                {
                    button |= MouseButtons.Middle;
                    clicks += 1;
                }
                if (eventscroll.State.HasFlag(Gdk.ModifierType.Button3Mask))
                {
                    button |= MouseButtons.Right;
                    clicks += 1;
                }
                var delta = (int)eventscroll.DeltaY;
                if (eventscroll.Direction == Gdk.ScrollDirection.Left || eventscroll.Direction == Gdk.ScrollDirection.Right)
                {
                    delta = (int)eventscroll.DeltaX;
                }
                var mouseArgs = new HandledMouseEventArgs(button, clicks, (int)eventscroll.X, (int)eventscroll.Y, delta);
                OnMouseWheel(mouseArgs);
                args.RetVal = mouseArgs.Handled;
            }
        }
    }
}
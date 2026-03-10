/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// Defines the base class for controls, which are components with visual representation.
    /// </summary>
    /// <filterpriority>1
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

        private Gtk.Application app = Application.Init();

        private ArrangedElementCollection arrangedElementCollection;

        private int location_x = 0;

        private int location_y = 0;

        private int size_height = 0;

        private int size_width = 0;

        private bool WidgetRealized = false;

        public virtual event EventHandler AnchorChanged;

        public virtual event EventHandler AutoSizeChanged;

        public virtual event EventHandler BackColorChanged;

        public virtual event EventHandler BackgroundImageChanged;

        public virtual event EventHandler BackgroundImageLayoutChanged;

        public virtual event EventHandler BindingContextChanged;

        public virtual event EventHandler CausesValidationChanged;

        public virtual event UICuesEventHandler ChangeUICues;

        public virtual event EventHandler Click;

        public virtual event EventHandler ClientSizeChanged;

        public virtual event EventHandler ContextMenuStripChanged;

        public virtual event ControlEventHandler ControlAdded;

        public virtual event ControlEventHandler ControlRemoved;

        public virtual event EventHandler CursorChanged;

        public virtual event EventHandler DockChanged;

        public virtual event EventHandler DoubleClick;

        public virtual event EventHandler DpiChangedAfterParent;

        public virtual event EventHandler DpiChangedBeforeParent;

        public virtual event DragEventHandler DragDrop;

        public virtual event DragEventHandler DragEnter;

        public virtual event EventHandler DragLeave;

        public virtual event DragEventHandler DragOver;

        public virtual event EventHandler EnabledChanged;

        public virtual event EventHandler Enter;

        public virtual event EventHandler FontChanged;

        public virtual event EventHandler ForeColorChanged;

        public virtual event GiveFeedbackEventHandler GiveFeedback;

        public virtual event EventHandler GotFocus;

        public virtual event EventHandler HandleCreated;

        public virtual event EventHandler HandleDestroyed;

        public virtual event HelpEventHandler HelpRequested;

        public virtual event EventHandler ImeModeChanged;

        public virtual event InvalidateEventHandler Invalidated;

        public virtual event KeyEventHandler KeyDown;

        public virtual event KeyPressEventHandler KeyPress;

        public virtual event KeyEventHandler KeyUp;

        public virtual event LayoutEventHandler Layout;

        public virtual event EventHandler Leave;

        //public event EventHandler Disposed;
        public virtual event EventHandler Load;

        public virtual event EventHandler LocationChanged;

        public virtual event EventHandler LostFocus;

        public virtual event EventHandler MarginChanged;

        public virtual event EventHandler MouseCaptureChanged;

        public virtual event MouseEventHandler MouseClick;

        public virtual event MouseEventHandler MouseDoubleClick;

        public virtual event MouseEventHandler MouseDown;

        public virtual event EventHandler MouseEnter;

        public virtual event EventHandler MouseHover;

        public virtual event EventHandler MouseLeave;

        public virtual event MouseEventHandler MouseMove;

        public virtual event MouseEventHandler MouseUp;

        public virtual event MouseEventHandler MouseWheel;

        public virtual event EventHandler Move;

        public virtual event EventHandler PaddingChanged;

        //public virtual event PaintEventHandler Paint;
        public virtual event EventHandler ParentChanged;

        public virtual event PreviewKeyDownEventHandler PreviewKeyDown;

        public virtual event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

        public virtual event QueryContinueDragEventHandler QueryContinueDrag;

        public virtual event EventHandler RegionChanged;

        public virtual event EventHandler Resize;

        public virtual event EventHandler RightToLeftChanged;

        public virtual event EventHandler SizeChanged;

        public virtual event EventHandler StyleChanged;

        public virtual event EventHandler SystemColorsChanged;

        public virtual event EventHandler TabIndexChanged;

        public virtual event EventHandler TabStopChanged;

        public virtual event EventHandler TextChanged;

        public virtual event EventHandler Validated;

        public virtual event CancelEventHandler Validating;

        public virtual event EventHandler VisibleChanged;

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

        public virtual AccessibleObject AccessibilityObject { get; }

        public virtual string AccessibleDefaultActionDescription { get; set; }

        public virtual string AccessibleDescription { get; set; }

        public virtual string AccessibleName { get; set; }

        public virtual AccessibleRole AccessibleRole { get; set; }

        public virtual bool AllowDrop { get; set; }

        public virtual AnchorStyles Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                SetAnchorStyles(Widget, _anchor);
                if (AnchorChanged != null)
                    AnchorChanged(this, EventArgs.Empty);
            }
        }

        public virtual Point AutoScrollOffset { get; set; }

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

        public virtual Drawing.Image BackgroundImage
        {
            get => ISelf == null ? null : ISelf.Override.BackgroundImage;
            set
            {
                if (ISelf != null)
                {
                    ISelf.Override.BackgroundImage = value;
                    Refresh();
                }
            }
        }

        public virtual ImageLayout BackgroundImageLayout
        {
            get => ISelf == null ? ImageLayout.None : ISelf.Override.BackgroundImageLayout;
            set
            {
                if (ISelf != null)
                {
                    ISelf.Override.BackgroundImageLayout = value;
                }
            }
        }

        public virtual BindingContext BindingContext { get; set; }

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

        public virtual int Bottom { get => this.Widget.MarginBottom; }

        public virtual Rectangle Bounds
        {
            get => new Rectangle(Widget.Clip.X, this.Widget.Clip.Y, this.Widget.Clip.Width, this.Widget.Clip.Height);
            set
            {
                SetBounds(value.X, value.Y, value.Width, value.Height);
            }
        }

        public virtual bool CanFocus
        {
            get
            {
                return this.Widget.CanFocus;
            }
        }

        public virtual bool CanSelect { get; }

        public virtual bool Capture { get; set; }

        public virtual bool CausesValidation { get; set; }

        public ArrangedElementCollection Children => arrangedElementCollection;

        public virtual Rectangle ClientRectangle
        {
            get
            {
                Gdk.Rectangle allocation = this.Widget.Allocation;

                return new Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height);
            }
        }

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

        public virtual string CompanyName { get; }

        public virtual bool ContainsFocus { get; }

        public virtual ContextMenuStrip ContextMenuStrip { get; set; }

        public virtual ControlCollection Controls { get; }

        public virtual bool Created => _Created;

        public virtual Cursor Cursor { get; set; }

        public virtual ControlBindingsCollection DataBindings { get; }

        public virtual int DeviceDpi { get; }

        public virtual Rectangle DisplayRectangle { get; }

        public virtual bool Disposing { get; }

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

                if (DockChanged != null)
                    DockChanged(this, EventArgs.Empty);
            }
        }

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

        public virtual bool Focused
        {
            get
            {
                return this.Widget.IsFocus;
            }
        }

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

        public virtual Gtk.Container GtkContainer { get => GtkControl as Gtk.Container; }

        public virtual object GtkControl { get; set; }

        public virtual IntPtr Handle { get => this.Widget == null ? IntPtr.Zero : this.Widget.Handle; }

        public virtual bool HasChildren { get; }

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
                if (DockChanged != null)
                    DockChanged(this, EventArgs.Empty);
                if (AnchorChanged != null)
                    AnchorChanged(this, EventArgs.Empty);
            }
        }

        public virtual System.Drawing.Image Image { get; set; }

        public virtual System.Drawing.ContentAlignment ImageAlign { get; set; }

        public virtual ImeMode ImeMode { get; set; }

        public virtual bool InvokeRequired { get; }

        public virtual bool IsAccessible { get; set; }

        public virtual bool IsDisposed { get; }

        public virtual IControlGtk ISelf { get => GtkControl as IControlGtk; }

        public virtual bool IsHandleCreated { get => this.Widget.IsRealized; }

        public virtual bool IsMirrored { get; }

        public virtual LayoutEngine LayoutEngine { get; }

        public virtual int Left
        {
            get => this.Widget.MarginStart;
            set
            {
                this.Widget.MarginStart = value;
                if (DockChanged != null)
                    DockChanged(this, EventArgs.Empty);
                if (AnchorChanged != null)
                    AnchorChanged(this, EventArgs.Empty);
            }
        }

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

        public virtual Padding Margin { get; set; }

        public virtual Size MaximumSize { get; set; }

        public virtual Size MinimumSize { get; set; }

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

        public virtual Padding Padding { get; set; }

        public virtual Control Parent { get; set; }

        public bool ParticipatesInLayout => false;

        public virtual Size PreferredSize { get; }

        public virtual string ProductName { get; }

        public virtual string ProductVersion { get; }

        public virtual bool RecreatingHandle { get; }

        public virtual Drawing.Region Region { get; set; }

        public virtual int Right { get => this.Widget.MarginEnd; }

        public virtual RightToLeft RightToLeft { get; set; }

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

        public virtual int TabIndex { get; set; }

        public virtual bool TabStop { get; set; }

        public virtual object Tag { get; set; }

        public virtual string Text { get; set; }

        public virtual int Top
        {
            get => this.Widget.MarginTop;
            set
            {
                this.Widget.MarginTop = value;
                if (DockChanged != null)
                    DockChanged(this, EventArgs.Empty);
                if (AnchorChanged != null)
                    AnchorChanged(this, EventArgs.Empty);
            }
        }

        public virtual Control TopLevelControl { get; }

        public string unique_key { get; protected set; }

        public virtual bool UseVisualStyleBackColor { get; set; } = true;

        public virtual bool UseWaitCursor { get; set; }

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

        public virtual Color VisualStyleBackColor { get; }

        public virtual Gtk.Widget Widget { get => GtkControl as Gtk.Widget; }

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
                if (DockChanged != null)
                    DockChanged(this, EventArgs.Empty);
                if (AnchorChanged != null)
                    AnchorChanged(this, EventArgs.Empty);
            }
        }

        public virtual IWindowTarget WindowTarget { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual bool ShowFocusCues { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual bool ShowKeyboardCues { get; }

        IArrangedElement IArrangedElement.Container => throw new NotImplementedException();

        PropertyStore IArrangedElement.Properties => throw new NotImplementedException();

        protected virtual bool CanEnableIme { get; }

        protected override bool CanRaiseEvents { get; }

        protected virtual CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = new CreateParams();
                createParams.ExStyle |= 32;

                return createParams;
            }
        }

        protected virtual Cursor DefaultCursor { get; }

        protected virtual ImeMode DefaultImeMode { get; }

        protected virtual Padding DefaultMargin { get; }

        protected virtual Size DefaultMaximumSize { get; }

        protected virtual Size DefaultMinimumSize { get; }

        protected virtual Padding DefaultPadding { get; }

        protected virtual Size DefaultSize { get; }

        protected virtual bool DoubleBuffered { get; set; }

        protected int FontHeight { get; set; }

        protected virtual ImeMode ImeModeBase { get; set; }

        protected bool ResizeRedraw { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool ScaleChildren { get; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void BeginInit()
        {
        }

        public virtual IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(state =>
            {
                method.DynamicInvoke((object[])state);
            }, args);

            return task;
        }

        public virtual IAsyncResult BeginInvoke(Delegate method)
        {
            return BeginInvoke(method, null);
        }

        public virtual IAsyncResult BeginInvoke(Action method)
        {
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(method);

            return task;
        }

        public virtual void BringToFront()
        {
        }

        public virtual bool Contains(Control ctl)
        {
            return false;
        }

        public virtual void CreateControl()
        {
        }

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

        public new void Dispose()
        {
            Dispose(true);
            base.Dispose();
        }

        public virtual DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            return DragDropEffects.None;
        }

        public virtual void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void EndInit()
        {
        }

        public virtual object EndInvoke(IAsyncResult asyncResult)
        {
            if (asyncResult is System.Threading.Tasks.Task task)
            {
                if (task.IsCompleted == false && task.IsCanceled == false && task.IsFaulted == false)
                    task.GetAwaiter().GetResult();
            }

            return asyncResult.AsyncState;
        }

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

        public virtual Control GetChildAtPoint(Point pt)
        {
            return null;
        }

        public virtual Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
        {
            return null;
        }

        public virtual IContainerControl GetContainerControl()
        {
            return this as IContainerControl;
        }

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

        public virtual Size GetPreferredSize(Size proposedSize)
        {
            return proposedSize;
        }

        public virtual void Hide()
        {
            if (this.GtkControl is Misc con)
            {
                con.Hide();
            }
        }

        public virtual void Invalidate()
        {
            Invalidate(true);
        }

        public virtual void Invalidate(bool invalidateChildren)
        {
            Invalidate(new Rectangle(Widget.Allocation.X, Widget.Allocation.Y, Widget.Allocation.Width, Widget.Allocation.Height), invalidateChildren);
        }

        public virtual void Invalidate(Rectangle rc)
        {
            Invalidate(rc, true);
        }

        public virtual void Invalidate(Rectangle rc, bool invalidateChildren)
        {
            if (this.Widget != null)
            {
                if (ISelf != null)
                    ISelf.Override.OnAddClass();
                this.Widget.Window.InvalidateRect(new Gdk.Rectangle(rc.X, rc.Y, rc.Width, rc.Height), invalidateChildren);
                if (invalidateChildren == true && this.Widget is Gtk.Container container)
                {
                    foreach (var child in container.Children)
                        child.Window.InvalidateRect(child.Allocation, invalidateChildren);
                }

                Refresh();
            }
        }

        public virtual void Invalidate(Drawing.Region region)
        {
            Invalidate(region, true);
        }

        public virtual void Invalidate(Drawing.Region region, bool invalidateChildren)
        {
            if (this.Widget != null)
            {
                if (ISelf != null)
                    ISelf.Override.OnAddClass();
                this.Widget.Window.InvalidateRect(Widget.Allocation, invalidateChildren);
            }
        }

        public virtual object Invoke(Delegate method)
        {
            return Invoke(method, null);
        }

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

        public virtual void Invoke(Action method)
        {
            GLib.Idle.Add(() =>
            {
                method.Invoke();
                return false;
            });
        }

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

        public virtual int LogicalToDeviceUnits(int value)
        {
            return value;
        }

        public virtual Size LogicalToDeviceUnits(Size value)
        {
            return value;
        }

        public virtual void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        public virtual void PerformLayout()
        {
            _Created = true;
        }

        public virtual void PerformLayout(Control affectedControl, string affectedProperty)
        {
            _Created = true;
        }

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

        public virtual PreProcessControlState PreProcessControlMessage(ref Message msg)
        {
            return PreProcessControlState.MessageNotNeeded;
        }

        public virtual bool PreProcessMessage(ref Message msg)
        {
            return false;
        }

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

        public virtual void Refresh()
        {
            if (this.Widget != null && this.Widget.IsVisible)
            {
                if (ISelf != null)
                    ISelf.Override.OnAddClass();
                this.Widget.QueueDraw();
                if (this.Widget is Gtk.Container container)
                {
                    foreach (var child in container.Children)
                        child.QueueDraw();
                }
            }
        }

        public virtual void ResetBackColor()
        {
        }

        public virtual void ResetBindings()
        {
        }

        public virtual void ResetCursor()
        {
        }

        public virtual void ResetFont()
        {
        }

        public virtual void ResetForeColor()
        {
        }

        public virtual void ResetImeMode()
        {
        }

        public virtual void ResetRightToLeft()
        {
        }

        public virtual void ResetText()
        {
        }

        public virtual void ResumeLayout()
        {
            ResumeLayout(false);
        }

        public virtual void ResumeLayout(bool performLayout)
        {
            _Created = true;
        }

        public virtual void Scale(float ratio)
        {
        }

        public virtual void Scale(float dx, float dy)
        {
        }

        public virtual void Scale(SizeF factor)
        {
        }

        public virtual void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
        {
        }

        public virtual void Select()
        {
            if (this.Widget != null)
                this.Widget.SetStateFlags(StateFlags.Selected, true);
        }

        public virtual bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            return false;
        }

        public virtual void SendToBack()
        {
        }

        public virtual void SetBounds(int x, int y, int width, int height)
        {
            SetBounds(x, y, width, height, BoundsSpecified.All);
        }

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

        public void SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            throw new NotImplementedException();
        }

        public virtual void Show()
        {
            if (this.Widget != null)
            {
                this.Widget.ShowAll();
            }
        }

        public virtual void SuspendLayout()
        {
            _Created = false;
        }

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

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (image != null)
                    image.Dispose();
                if (surface != null)
                    surface.Dispose();
                if (context != null)
                    context.Dispose();
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

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseHover(EventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseLeave(EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }

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

        protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
        }

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

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidated(CancelEventArgs e)
        {
            Validated?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidating(CancelEventArgs e)
        {
            Validating?.Invoke(this, e);
        }

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged?.Invoke(this, e);
        }

        protected virtual void Select(bool directed, bool forward)
        {
        }

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

        protected virtual void SetStyle(ControlStyles styles, bool value)
        {
        }

        protected virtual void UpdateBackgroundStyle()
        {
            if (this.Widget != null && this.Widget.IsMapped)
                ISelf.Override.OnAddClass();
        }

        protected virtual void UpdateStyle()
        {
            if (this.Widget != null && this.Widget.IsMapped)
                SetStyle(this.Widget);
        }

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
            Gtk.Widget owidget = o as Gtk.Widget;
            if (owidget != null && owidget.Window != null)
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
            Gtk.Widget owidget = o as Gtk.Widget;
            if (owidget != null && owidget.Window != null)
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
            Gtk.Widget owidget = o as Gtk.Widget;
            if (owidget != null && owidget.Window != null)
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
                if (SizeChanged != null)
                    SizeChanged(this, EventArgs.Empty);
            }

            if (args.Allocation.X != location_x || args.Allocation.Y != location_y)
            {
                location_x = args.Allocation.X;
                location_y = args.Allocation.Y;
                if (LocationChanged != null)
                    LocationChanged(this, EventArgs.Empty);
            }
        }

    }
}

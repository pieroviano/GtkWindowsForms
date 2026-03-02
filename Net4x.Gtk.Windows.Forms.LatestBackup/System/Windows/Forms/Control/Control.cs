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

        private Gtk.Application app = Application.Init();
        public string unique_key { get; protected set; }


        public virtual Gtk.Widget Widget { get => GtkControl as Gtk.Widget; }

        public virtual Gtk.Container GtkContainer { get => GtkControl as Gtk.Container; }

        public virtual IControlGtk ISelf { get => GtkControl as IControlGtk; }

        public virtual object GtkControl { get; set; }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Control" /> class with default settings.
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

        private int size_width = 0;

        private int size_height = 0;

        private int location_x = 0;

        private int location_y = 0;
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
        private void Widget_ConfigureEvent(object o, ConfigureEventArgs args)
        {
            OnMove(EventArgs.Empty);
            Move?.Invoke(this, args);
        }
        #region events

        private bool WidgetRealized = false;
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

                owidget.Window.GetOrigin(out int x, out int y);// avoid event penetration errors
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
        private void Widget_ScrollEvent(object o, Gtk.ScrollEventArgs args)
        {
            MouseEventArgs mouseArgs = new MouseEventArgs(MouseButtons.None, 0, (int)args.Event.X, (int)args.Event.Y, (int)args.Event.DeltaY);
            OnMouseWheel(mouseArgs);
            MouseWheel?.Invoke(this, mouseArgs);
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

        #endregion

        //===================
        protected virtual void InitStyle(Gtk.Widget widget)
        {
            SetStyle(widget);
        }
        protected virtual void UpdateStyle()
        {
            if (this.Widget != null && this.Widget.IsMapped)
                SetStyle(this.Widget);
        }
        protected virtual void UpdateBackgroundStyle()
        {
            if (this.Widget != null && this.Widget.IsMapped)
                ISelf.Override.OnAddClass();
        }
        protected virtual void SetStyle(Gtk.Widget widget)
        {
            StringBuilder style = new StringBuilder();
            if (widget is Gtk.Image) { }
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
        /// Sets a specified
        /// <see cref="System.Windows.Forms.ControlStyles" /> flag to either true or false.
        /// </summary>
        /// <param name="flag">The
        /// <see cref="System.Windows.Forms.ControlStyles" /> bit to set.
        /// </param>
        /// <param name="value">true to apply the specified style to the control; otherwise, false.
        /// </param>
        protected virtual void SetStyle(ControlStyles styles, bool value)
        {
        }

        #region Background

        public virtual System.Drawing.Image Image { get; set; }
        public virtual System.Drawing.ContentAlignment ImageAlign { get; set; }


        public virtual bool UseVisualStyleBackColor { get; set; } = true;

        public virtual Color VisualStyleBackColor { get; }
        /// <summary>
        /// Gets or sets the background image layout as defined in the
        /// <see cref="System.Windows.Forms.ImageLayout" /> enumeration.
        /// </summary>
        /// <returns>One of the values of
        /// <see cref="System.Windows.Forms.ImageLayout" /> (
        /// <see cref="System.Windows.Forms.ImageLayout.Center" /> ,
        /// <see cref="System.Windows.Forms.ImageLayout.None" />,
        /// <see cref="System.Windows.Forms.ImageLayout.Stretch" />,
        /// <see cref="System.Windows.Forms.ImageLayout.Tile" />, or
        /// <see cref="System.Windows.Forms.ImageLayout.Zoom" />).
        /// <see cref="System.Windows.Forms.ImageLayout.Tile" /> is the default value.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified enumeration value does not exist.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ImageLayout BackgroundImageLayout { get => ISelf == null ? ImageLayout.None : ISelf.Override.BackgroundImageLayout; set { if (ISelf != null) { ISelf.Override.BackgroundImageLayout = value; } } }
        /// <summary>
        /// Gets or sets the background image displayed in the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Image" /> that represents the image to display in the background of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Drawing.Image BackgroundImage { get => ISelf == null ? null : ISelf.Override.BackgroundImage; set { if (ISelf != null) { ISelf.Override.BackgroundImage = value; Refresh(); } } }
        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the control. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultBackColor" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        public virtual event PaintEventHandler Paint
        {
            add { ISelf.Override.Paint += value; }
            remove { ISelf.Override.Paint -= value; }
        }
        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.AccessibleObject" /> assigned to the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.AccessibleObject" /> assigned to the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        #endregion
        public virtual AccessibleObject AccessibilityObject { get; }
        /// <summary>
        /// Gets or sets the default action description of the control for use by accessibility client applications.
        /// </summary>
        /// <returns>The default action description of the control for use by accessibility client applications.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual string AccessibleDefaultActionDescription { get; set; }
        /// <summary>
        /// Gets or sets the description of the control used by accessibility client applications.
        /// </summary>
        /// <returns>The description of the control used by accessibility client applications. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string AccessibleDescription { get; set; }
        /// <summary>
        /// Gets or sets the name of the control used by accessibility client applications.
        /// </summary>
        /// <returns>The name of the control used by accessibility client applications. The default is null.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual string AccessibleName { get; set; }
        /// <summary>
        /// Gets or sets the accessible role of the control
        /// </summary>
        /// <returns>One of the values of
        /// <see cref="System.Windows.Forms.AccessibleRole" /> The default is Default.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.AccessibleRole" /> values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual AccessibleRole AccessibleRole { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the control can accept data that the user drags onto it.
        /// </summary>
        /// <returns>true if drag-and-drop operations are allowed in the control; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool AllowDrop { get; set; }

        private AnchorStyles _anchor;
        /// <summary>
        /// Gets or sets the edges of the container to which a control is bound and determines how a control is resized with its parent.
        /// </summary>
        /// <returns>A bitwise combination of the
        /// <see cref="System.Windows.Forms.AnchorStyles" /> values. The default is Top and Left.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// <summary>
        /// Gets or sets where this control is scrolled to in
        /// <see cref="System.Windows.Forms.ScrollableControl.ScrollControlIntoView(System.Windows.Forms.Control)" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> specifying the scroll location. The default is the upper-left corner of the control.
        /// </returns>
        public virtual Point AutoScrollOffset { get; set; }

        private bool _autoSize;
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>true if enabled; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool AutoSize
        {
            get => _autoSize;
            set
            {
                _autoSize = value;
                if (_autoSize == true) { this.Widget.WidthRequest = -1; this.Widget.HeightRequest = -1; } else { Size = _size; }
            }
        }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.BindingContext" /> for the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.BindingContext" /> for the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual BindingContext BindingContext { get; set; }
        /// <summary>
        /// Gets or sets the size and location of the control including its nonclient elements, in pixels, relative to the parent control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> in pixels relative to the parent control that represents the size and location of the control including its nonclient elements.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Rectangle Bounds { get => new Rectangle(Widget.Clip.X, this.Widget.Clip.Y, this.Widget.Clip.Width, this.Widget.Clip.Height); set { SetBounds(value.X, value.Y, value.Width, value.Height); } }
        /// <summary>
        /// Gets a value indicating whether the control can receive focus.
        /// </summary>
        /// <returns>true if the control can receive focus; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool CanFocus { get { return this.Widget.CanFocus; } }
        /// <summary>
        /// Gets a value indicating whether the control can be selected.
        /// </summary>
        /// <returns>true if the control can be selected; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool CanSelect { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the control has captured the mouse.
        /// </summary>
        /// <returns>true if the control has captured the mouse; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool Capture { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the control causes validation to be performed on any controls that require validation when it receives focus.
        /// </summary>
        /// <returns>true if the control causes validation to be performed on any controls requiring validation when it receives focus; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool CausesValidation { get; set; }
        /// <summary>
        /// Gets the name of the company or creator of the application containing the control.
        /// </summary>
        /// <returns>The company name or creator of the application containing the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string CompanyName { get; }
        /// <summary>
        /// Gets a value indicating whether the control, or one of its child controls, currently has the input focus.
        /// </summary>
        /// <returns>true if the control or one of its child controls currently has the input focus; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool ContainsFocus { get; }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> associated with this control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> for this control, or null if there is no
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> The default is null.
        /// </returns>
        public virtual ContextMenuStrip ContextMenuStrip { get; set; }
        /// <summary>
        /// Gets the collection of controls contained within the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Control.ControlCollection" /> representing the collection of controls contained within the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual ControlCollection Controls { get; }
        /// <summary>
        /// Gets a value indicating whether the control has been created.
        /// </summary>
        /// <returns>true if the control has been created; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Created => _Created;
        internal bool _Created;
        /// <summary>
        /// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor to display when the mouse pointer is over the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Cursor Cursor { get; set; }
        /// <summary>
        /// Gets the data bindings for the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ControlBindingsCollection" /> that contains the
        /// <see cref="System.Windows.Forms.Binding" /> objects for the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual ControlBindingsCollection DataBindings { get; }

        public virtual int DeviceDpi { get; }
        /// <summary>
        /// Gets the rectangle that represents the display area of the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the display area of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Rectangle DisplayRectangle { get; }
        /// <summary>
        /// Gets a value indicating whether the base
        /// <see cref="System.Windows.Forms.Control" /> class is in the process of disposing.
        /// </summary>
        /// <returns>true if the base
        /// <see cref="System.Windows.Forms.Control" /> class is in the process of disposing; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool Disposing { get; }

        private DockStyle _dock;
        /// <summary>
        /// Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DockStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.DockStyle.None" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.DockStyle" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// <summary>
        /// Gets or sets a value indicating whether the control can respond to user interaction.
        /// </summary>
        /// <returns>true if the control can respond to user interaction; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool Enabled { get { return this.Widget.Sensitive; } set { this.Widget.Sensitive = value; } }
        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        /// <returns>true if the control has focus; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Focused { get { return this.Widget.IsFocus; } }

        private Font _Font;
        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Font" /> to apply to the text displayed by the control. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultFont" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        private Color _ForeColor;
        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        /// <returns>The foreground
        /// <see cref="System.Drawing.Color" /> of the control. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultForeColor" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Color ForeColor
        {
            get { return _ForeColor; }
            set { _ForeColor = value; UpdateStyle(); }
        }
        /// <summary>
        /// Gets a value indicating whether the control contains one or more child controls.
        /// </summary>
        /// <returns>true if the control contains one or more child controls; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool HasChildren { get; }
        /// <summary>
        /// Gets or sets the Input Method Editor (IME) mode of the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ImeMode" /> values. The default is
        /// <see cref="System.Windows.Forms.ImeMode.Inherit" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.ImeMode" /> enumeration values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ImeMode ImeMode { get; set; }
        /// <summary>
        /// Gets a value indicating whether the caller must call an invoke method when making method calls to the control because the caller is on a different thread than the one the control was created on.
        /// </summary>
        /// <returns>true if the control's
        /// <see cref="System.Windows.Forms.Control.Handle" /> was created on a different thread than the calling thread (indicating that you must make calls to the control through an invoke method); otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool InvokeRequired { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the control is visible to accessibility applications.
        /// </summary>
        /// <returns>true if the control is visible to accessibility applications; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool IsAccessible { get; set; }
        /// <summary>
        /// Gets a value indicating whether the control has been disposed of.
        /// </summary>
        /// <returns>true if the control has been disposed of; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool IsDisposed { get; }
        /// <summary>
        /// Gets a value indicating whether the control has a handle associated with it.
        /// </summary>
        /// <returns>true if a handle has been assigned to the control; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool IsHandleCreated { get => this.Widget.IsRealized; }
        /// <summary>
        /// Gets a value indicating whether the control is mirrored.
        /// </summary>
        /// <returns>true if the control is mirrored; otherwise, false.
        /// </returns>
        public virtual bool IsMirrored { get; }
        /// <summary>
        /// Gets a cached instance of the control's layout engine.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Layout.LayoutEngine" /> for the control's contents.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual LayoutEngine LayoutEngine { get; }
        /// <summary>
        /// Gets or sets the distance, in pixels, between the top edge of the control and the top edge of its container's client area.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the distance, in pixels, between the bottom edge of the control and the top edge of its container's client area.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Gets or sets the distance, in pixels, between the left edge of the control and the left edge of its container's client area.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the distance, in pixels, between the left edge of the control and the left edge of its container's client area.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Gets the distance, in pixels, between the right edge of the control and the left edge of its container's client area.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the distance, in pixels, between the right edge of the control and the left edge of its container's client area.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int Right
        {
            get => this.Widget.MarginEnd;
        }
        /// <summary>
        /// Gets the distance, in pixels, between the bottom edge of the control and the top edge of its container's client area.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the distance, in pixels, between the bottom edge of the control and the top edge of its container's client area.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual int Bottom
        {
            get => this.Widget.MarginBottom;
        }

        internal bool LockLocation = false;// because the code executes sequentially， special locking
        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the control relative to the upper-left corner of its container.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Point" /> that represents the upper-left corner of the control relative to the upper-left corner of its container.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets the name of the control.
        /// </summary>
        /// <returns>The name of the control. The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string Name { get { return this.Widget.Name; } set { this.Widget.Name = value; } }
        /// <summary>
        /// Gets or sets padding within the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> representing the control's internal spacing characteristics.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Padding Padding { get; set; }
        /// <summary>
        /// Gets or sets the parent container of the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Control" /> that represents the parent or container control of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Control Parent { get; set; }
        /// <summary>
        /// Gets the size of a rectangular area into which the control can fit.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> containing the height and width, in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Size PreferredSize { get; }
        /// <summary>
        /// Gets the product name of the assembly containing the control.
        /// </summary>
        /// <returns>The product name of the assembly containing the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string ProductName { get; }
        /// <summary>
        /// Gets the version of the assembly containing the control.
        /// </summary>
        /// <returns>The file version of the assembly containing the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string ProductVersion { get; }
        /// <summary>
        /// Gets a value indicating whether the control is currently re-creating its handle.
        /// </summary>
        /// <returns>true if the control is currently re-creating its handle; otherwise, false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool RecreatingHandle { get; }
        /// <summary>
        /// Gets or sets the window region associated with the control.
        /// </summary>
        /// <returns>The window
        /// <see cref="System.Drawing.Region" /> associated with the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Drawing.Region Region { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.RightToLeft" /> values. The default is
        /// <see cref="System.Windows.Forms.RightToLeft.Inherit" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.RightToLeft" /> values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual RightToLeft RightToLeft { get; set; }

        private Size _size;
        /// <summary>
        /// Gets or sets the height and width of the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Size" /> that represents the height and width of the control in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets or sets the height of the control.
        /// </summary>
        /// <returns>The height of the control in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        /// <returns>The width of the control in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Gets or sets the tab order of the control within its container.
        /// </summary>
        /// <returns>The index value of the control within the set of controls within its container. The controls in the container are included in the tab order.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual int TabIndex { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control using the TAB key.
        /// </summary>
        /// <returns>true if the user can give the focus to the control using the TAB key; otherwise, false. The default is true.NoteThis property will always return true for an instance of the
        /// <see cref="System.Windows.Forms.Form" /> class.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool TabStop { get; set; }
        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the control. The default is null.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual object Tag { get; set; }
        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <returns>The text associated with this control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string Text { get; set; }
        /// <summary>
        /// Gets the parent control that is not parented by another Windows Forms control. Typically, this is the outermost
        /// <see cref="System.Windows.Forms.Form" /> that the control is contained in.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Control" /> that represents the top-level control that contains the current control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual Control TopLevelControl { get; }
        /// <summary>
        /// Gets or sets a value indicating whether to use the wait cursor for the current control and all child controls.
        /// </summary>
        /// <returns>true to use the wait cursor for the current control and all child controls; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual bool UseWaitCursor { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
        /// </summary>
        /// <returns>true if the control and all its child controls are displayed; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool Visible { get { return this.Widget.Visible; } set { this.Widget.Visible = value; this.Widget.NoShowAll = value == false; } }
        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        /// <returns>The default
        /// <see cref="System.Drawing.Size" /> of the control.
        /// </returns>
        protected virtual Size DefaultSize { get; }
        /// <summary>
        /// Gets the internal spacing, in pixels, of the contents of a control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> that represents the internal spacing of the contents of a control.
        /// </returns>
        protected virtual Padding DefaultPadding { get; }
        /// <summary>
        /// Gets the length and height, in pixels, that is specified as the default minimum size of a control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> representing the size of the control.
        /// </returns>
        protected virtual Size DefaultMinimumSize { get; }
        /// <summary>
        /// Gets the space, in pixels, that is specified by default between controls.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> that represents the default space between controls.
        /// </returns>
        protected virtual Padding DefaultMargin { get; }
        /// <summary>
        /// Gets or sets the default cursor for the control.
        /// </summary>
        /// <returns>An object of type
        /// <see cref="System.Windows.Forms.Cursor" /> representing the current default cursor.
        /// </returns>
        protected virtual Cursor DefaultCursor { get; }
        /// <summary>
        /// Determines if events can be raised on the control.
        /// </summary>
        /// <returns>true if the control is hosted as an ActiveX control whose events are not frozen; otherwise, false.
        /// </returns>
        protected override bool CanRaiseEvents { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this control should redraw its surface using a secondary buffer to reduce or prevent flicker.
        /// </summary>
        /// <returns>true if the surface of the control should be drawn using double buffering; otherwise, false.
        /// </returns>
        protected virtual bool DoubleBuffered { get; set; }
        /// <summary>
        /// Gets or sets the height of the font of the control.
        /// </summary>
        /// <returns>The height of the
        /// <see cref="System.Drawing.Font" /> of the control in pixels.
        /// </returns>
        protected int FontHeight { get; set; }
        /// <summary>
        /// Gets the length and height, in pixels, that is specified as the default maximum size of a control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point.#ctor(System.Drawing.Size)" /> representing the size of the control.
        /// </returns>
        protected virtual Size DefaultMaximumSize { get; }
        /// <summary>
        /// Gets or sets the IME mode of a control.
        /// </summary>
        /// <returns>The IME mode of the control.
        /// </returns>
        protected virtual ImeMode ImeModeBase { get; set; }
        /// <summary>
        /// Gets the default Input Method Editor (IME) mode supported by the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ImeMode" /> values.
        /// </returns>
        protected virtual ImeMode DefaultImeMode { get; }
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.Control.ImeMode" /> property can be set to an active value, to enable IME support.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        protected virtual bool CanEnableIme { get; }
        /// <summary>
        /// Gets a value that determines the scaling of child controls.
        /// </summary>
        /// <returns>true if child controls will be scaled when the
        /// <see cref="System.Windows.Forms.Control.Scale(System.Single)" /> method on this control is called; otherwise, false. The default is true.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool ScaleChildren { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the control redraws itself when resized.
        /// </summary>
        /// <returns>true if the control redraws itself when resized; otherwise, false.
        /// </returns>
        protected bool ResizeRedraw { get; set; }
        /// <summary>
        /// Gets a value indicating whether the control should display focus rectangles.
        /// </summary>
        /// <returns>true if the control should display focus rectangles; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual bool ShowFocusCues { get; }
        /// <summary>
        /// Gets a value indicating whether the user interface is in the appropriate state to show or hide keyboard accelerators.
        /// </summary>
        /// <returns>true if the keyboard accelerators are visible; otherwise, false.
        /// </returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual bool ShowKeyboardCues { get; }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.IWindowTarget" />
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public virtual IWindowTarget WindowTarget { get; set; }

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

        public virtual event EventHandler AnchorChanged;

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
        //public event EventHandler Disposed;

        public virtual event EventHandler Load;
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments, on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IAsyncResult" /> that represents the result of the
        /// <see cref="System.Windows.Forms.Control.BeginInvoke(System.Delegate)" /> operation.
        /// </returns>
        /// <param name="method">A delegate to a method that takes parameters of the same number and type that are contained in the
        /// <paramref name="args" /> parameter.
        /// </param>
        /// <param name="args">An array of objects to pass as arguments to the given method. This can be null if no arguments are needed.
        /// </param>
        /// <exception cref="System.InvalidOperationException">No appropriate window handle can be found.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(state =>
            {
                method.DynamicInvoke((object[])state);
            }, args);

            return task;
        }
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the control's underlying handle was created on.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IAsyncResult" /> that represents the result of the
        /// <see cref="System.Windows.Forms.Control.BeginInvoke(System.Delegate)" /> operation.
        /// </returns>
        /// <param name="method">A delegate to a method that takes no parameters.
        /// </param>
        /// <exception cref="System.InvalidOperationException">No appropriate window handle can be found.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual IAsyncResult BeginInvoke(Delegate method)
        {
            return BeginInvoke(method, null);
        }
        public virtual IAsyncResult BeginInvoke(Action method)
        {
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(method);
            return task;
        }
        /// <summary>
        /// Retrieves the return value of the asynchronous operation represented by the
        /// <see cref="System.IAsyncResult" /> passed.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Object" /> generated by the asynchronous operation.
        /// </returns>
        /// <param name="asyncResult">The
        /// <see cref="System.IAsyncResult" /> that represents a specific invoke asynchronous operation, returned when calling
        /// <see cref="System.Windows.Forms.Control.BeginInvoke(System.Delegate)" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">The
        /// <paramref name="asyncResult" /> parameter value is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">The
        /// <paramref name="asyncResult" /> object was not created by a preceding call of the
        /// <see cref="System.Windows.Forms.Control.BeginInvoke(System.Delegate)" /> method from the same control.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Brings the control to the front of the z-order.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void BringToFront()
        {

        }
        /// <summary>
        /// Retrieves a value indicating whether the specified control is a child of the control.
        /// </summary>
        /// <returns>true if the specified control is a child of the control; otherwise, false.
        /// </returns>
        /// <param name="ctl">The
        /// <see cref="System.Windows.Forms.Control" /> to evaluate.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Contains(Control ctl)
        {
            return false;
        }
        /// <summary>
        /// Forces the creation of the visible control, including the creation of the handle and any visible child controls.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void CreateControl()
        {

        }
        Cairo.ImageSurface image;
        Cairo.Surface surface;
        Cairo.Context context;
        /// <summary>
        /// Creates the
        /// <see cref="System.Drawing.Graphics" /> for the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> for the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Begins a drag-and-drop operation.
        /// </summary>
        /// <returns>A value from the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> enumeration that represents the final effect that was performed during the drag-and-drop operation.
        /// </returns>
        /// <param name="data">The data to drag.
        /// </param>
        /// <param name="allowedEffects">One of the
        /// <see cref="System.Windows.Forms.DragDropEffects" /> values.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            return DragDropEffects.None;
        }
        /// <summary>
        /// Supports rendering to the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to be drawn to.
        /// </param>
        /// <param name="targetBounds">The bounds within which the control is rendered.
        /// </param>
        public virtual void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
        }
        /// <summary>
        /// Retrieves the form that the control is on.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Form" /> that the control is on.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
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
        /// Sets input focus to the control.
        /// </summary>
        /// <returns>true if the input focus request was successful; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Retrieves the child control that is located at the specified coordinates.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Control" /> that represents the control that is located at the specified point.
        /// </returns>
        /// <param name="pt">A
        /// <see cref="System.Drawing.Point" /> that contains the coordinates where you want to look for a control. Coordinates are expressed relative to the upper-left corner of the control's client area.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Control GetChildAtPoint(Point pt)
        {
            return null;
        }
        /// <summary>
        /// Retrieves the child control that is located at the specified coordinates, specifying whether to ignore child controls of a certain type.
        /// </summary>
        /// <returns>The child
        /// <see cref="System.Windows.Forms.Control" /> at the specified coordinates.
        /// </returns>
        /// <param name="pt">A
        /// <see cref="System.Drawing.Point" /> that contains the coordinates where you want to look for a control. Coordinates are expressed relative to the upper-left corner of the control's client area.
        /// </param>
        /// <param name="skipValue">One of the values of
        /// <see cref="System.Windows.Forms.GetChildAtPointSkip" />, determining whether to ignore child controls of a certain type.
        /// </param>
        public virtual Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
        {
            return null;
        }
        /// <summary>
        /// Returns the next
        /// <see cref="System.Windows.Forms.ContainerControl" /> up the control's chain of parent controls.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.IContainerControl" />, that represents the parent of the
        /// <see cref="System.Windows.Forms.Control" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual IContainerControl GetContainerControl()
        {
            return this as IContainerControl;
        }
        /// <summary>
        /// Retrieves the next control forward or back in the tab order of child controls.
        /// </summary>
        /// <returns>The next
        /// <see cref="System.Windows.Forms.Control" /> in the tab order.
        /// </returns>
        /// <param name="ctl">The
        /// <see cref="System.Windows.Forms.Control" /> to start the search with.
        /// </param>
        /// <param name="forward">true to search forward in the tab order; false to search backward.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
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
        /// Retrieves the size of a rectangular area into which a control can be fitted.
        /// </summary>
        /// <returns>An ordered pair of type
        /// <see cref="System.Drawing.Size" /> representing the width and height of a rectangle.
        /// </returns>
        /// <param name="proposedSize">The custom-sized area for a control.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual Size GetPreferredSize(Size proposedSize)
        {
            return proposedSize;
        }
        /// <summary>
        /// Invalidates the entire surface of the control and causes the control to be redrawn.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Invalidate()
        {
            Invalidate(true);
        }
        /// <summary>
        /// Invalidates a specific region of the control and causes a paint message to be sent to the control. Optionally, invalidates the child controls assigned to the control.
        /// </summary>
        /// <param name="invalidateChildren">true to invalidate the control's child controls; otherwise, false.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Invalidate(bool invalidateChildren)
        {
            Invalidate(new Rectangle(Widget.Allocation.X, Widget.Allocation.Y, Widget.Allocation.Width, Widget.Allocation.Height), invalidateChildren);
        }
        /// <summary>
        /// Invalidates the specified region of the control (adds it to the control's update region, which is the area that will be repainted at the next paint operation), and causes a paint message to be sent to the control.
        /// </summary>
        /// <param name="rc">A
        /// <see cref="System.Drawing.Rectangle" /> that represents the region to invalidate.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Invalidate(Rectangle rc)
        {
            Invalidate(rc, true);
        }
        /// <summary>
        /// Invalidates the specified region of the control (adds it to the control's update region, which is the area that will be repainted at the next paint operation), and causes a paint message to be sent to the control. Optionally, invalidates the child controls assigned to the control.
        /// </summary>
        /// <param name="rc">A
        /// <see cref="System.Drawing.Rectangle" /> that represents the region to invalidate.
        /// </param>
        /// <param name="invalidateChildren">true to invalidate the control's child controls; otherwise, false.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Invalidates the specified region of the control (adds it to the control's update region, which is the area that will be repainted at the next paint operation), and causes a paint message to be sent to the control.
        /// </summary>
        /// <param name="region">The
        /// <see cref="System.Drawing.Region" /> to invalidate.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Invalidate(Drawing.Region region)
        {
            Invalidate(region, true);
        }
        /// <summary>
        /// Invalidates the specified region of the control (adds it to the control's update region, which is the area that will be repainted at the next paint operation), and causes a paint message to be sent to the control. Optionally, invalidates the child controls assigned to the control.
        /// </summary>
        /// <param name="region">The
        /// <see cref="System.Drawing.Region" /> to invalidate.
        /// </param>
        /// <param name="invalidateChildren">true to invalidate the control's child controls; otherwise, false.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Invalidate(Drawing.Region region, bool invalidateChildren)
        {
            if (this.Widget != null)
            {
                if (ISelf != null)
                    ISelf.Override.OnAddClass();
                this.Widget.Window.InvalidateRect(Widget.Allocation, invalidateChildren);
            }
        }
        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.
        /// </returns>
        /// <param name="method">A delegate that contains a method to be called in the control's thread context.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual object Invoke(Delegate method)
        {
            return Invoke(method, null);
        }
        /// <summary>
        /// Executes the specified delegate, on the thread that owns the control's underlying window handle, with the specified list of arguments.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains the return value from the delegate being invoked, or null if the delegate has no return value.
        /// </returns>
        /// <param name="method">A delegate to a method that takes parameters of the same number and type that are contained in the
        /// <paramref name="args" /> parameter.
        /// </param>
        /// <param name="args">An array of objects to pass as arguments to the specified method. This parameter can be null if the method takes no arguments.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Computes the location of the specified screen point into client coordinates.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> that represents the converted
        /// <see cref="System.Drawing.Point" />,
        /// <paramref name="p" />, in client coordinates.
        /// </returns>
        /// <param name="p">The screen coordinate
        /// <see cref="System.Drawing.Point" /> to convert.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Computes the location of the specified client point into screen coordinates.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> that represents the converted
        /// <see cref="System.Drawing.Point" />,
        /// <paramref name="p" />, in screen coordinates.
        /// </returns>
        /// <param name="p">The client coordinate
        /// <see cref="System.Drawing.Point" /> to convert.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Preprocesses keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.PreProcessControlState" /> values, depending on whether
        /// <see cref="System.Windows.Forms.Control.PreProcessMessage(System.Windows.Forms.Message)" /> is true or false and whether
        /// <see cref="System.Windows.Forms.Control.IsInputKey(System.Windows.Forms.Keys)" /> or
        /// <see cref="System.Windows.Forms.Control.IsInputChar(System.Char)" /> are true or false.
        /// </returns>
        /// <param name="msg">A
        /// <see cref="System.Windows.Forms.Message" /> that represents the message to process.
        /// </param>
        public virtual PreProcessControlState PreProcessControlMessage(ref Message msg)
        {
            return PreProcessControlState.MessageNotNeeded;
        }
        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <returns>true if the message was processed by the control; otherwise, false.
        /// </returns>
        /// <param name="msg">A
        /// <see cref="System.Windows.Forms.Message" />, passed by reference, that represents the message to process. The possible values are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool PreProcessMessage(ref Message msg)
        {
            return false;
        }
        /// <summary>
        /// Computes the size and location of the specified screen rectangle in client coordinates.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the converted
        /// <see cref="System.Drawing.Rectangle" />,
        /// <paramref name="r" />, in client coordinates.
        /// </returns>
        /// <param name="r">The screen coordinate
        /// <see cref="System.Drawing.Rectangle" /> to convert.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
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
        /// Computes the size and location of the specified client rectangle in screen coordinates.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the converted
        /// <see cref="System.Drawing.Rectangle" />,
        /// <paramref name="p" />, in screen coordinates.
        /// </returns>
        /// <param name="r">The client coordinate
        /// <see cref="System.Drawing.Rectangle" /> to convert.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
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
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.BackColor" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetBackColor()
        {

        }
        /// <summary>
        /// Causes a control bound to the
        /// <see cref="System.Windows.Forms.BindingSource" /> to reread all the items in the list and refresh their displayed values.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual void ResetBindings()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.Cursor" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetCursor()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.Font" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetFont()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.ForeColor" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetForeColor()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.ImeMode" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetImeMode()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.RightToLeft" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void ResetRightToLeft()
        {

        }
        /// <summary>
        /// Resets the
        /// <see cref="System.Windows.Forms.Control.Text" /> property to its default value.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual void ResetText()
        {

        }
        /// <summary>
        /// Resumes usual layout logic.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void ResumeLayout()
        {
            ResumeLayout(false);
        }
        /// <summary>
        /// Resumes usual layout logic, optionally forcing an immediate layout of pending layout requests.
        /// </summary>
        /// <param name="performLayout">true to execute pending layout requests; otherwise, false.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void ResumeLayout(bool performLayout)
        {
            _Created = true;
        }
        /// <summary>
        /// Scales the control and any child controls.
        /// </summary>
        /// <param name="ratio">The ratio to use for scaling.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Scale(float ratio)
        {

        }
        /// <summary>
        /// Scales the entire control and any child controls.
        /// </summary>
        /// <param name="dx">The horizontal scaling factor.
        /// </param>
        /// <param name="dy">The vertical scaling factor.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Scale(float dx, float dy)
        {

        }
        /// <summary>
        /// Scales the control and all child controls by the specified scaling factor.
        /// </summary>
        /// <param name="factor">A
        /// <see cref="System.Drawing.SizeF" /> containing the horizontal and vertical scaling factors.
        /// </param>
        public virtual void Scale(SizeF factor)
        {

        }

        public virtual void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
        {

        }
        /// <summary>
        /// Activates the control.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void Select()
        {
            if (this.Widget != null)
                this.Widget.SetStateFlags(StateFlags.Selected, true);
        }
        /// <summary>
        /// Activates the next control.
        /// </summary>
        /// <returns>true if a control was activated; otherwise, false.
        /// </returns>
        /// <param name="ctl">The
        /// <see cref="System.Windows.Forms.Control" /> at which to start the search.
        /// </param>
        /// <param name="forward">true to move forward in the tab order; false to move backward in the tab order.
        /// </param>
        /// <param name="tabStopOnly">true to ignore the controls with the
        /// <see cref="System.Windows.Forms.Control.TabStop" /> property set to false; otherwise, false.
        /// </param>
        /// <param name="nested">true to include nested (children of child controls) child controls; otherwise, false.
        /// </param>
        /// <param name="wrap">true to continue searching from the first control in the tab order after the last control has been reached; otherwise, false.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            return false;
        }
        /// <summary>
        /// Sends the control to the back of the z-order.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void SendToBack()
        {

        }
        /// <summary>
        /// Sets the bounds of the control to the specified location and size.
        /// </summary>
        /// <param name="x">The new
        /// <see cref="System.Windows.Forms.Control.Left" /> property value of the control.
        /// </param>
        /// <param name="y">The new
        /// <see cref="System.Windows.Forms.Control.Top" /> property value of the control.
        /// </param>
        /// <param name="width">The new
        /// <see cref="System.Windows.Forms.Control.Width" /> property value of the control.
        /// </param>
        /// <param name="height">The new
        /// <see cref="System.Windows.Forms.Control.Height" /> property value of the control.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void SetBounds(int x, int y, int width, int height)
        {
            SetBounds(x, y, width, height, BoundsSpecified.All);
        }
        /// <summary>
        /// Sets the specified bounds of the control to the specified location and size.
        /// </summary>
        /// <param name="x">The new
        /// <see cref="System.Windows.Forms.Control.Left" /> property value of the control.
        /// </param>
        /// <param name="y">The new
        /// <see cref="System.Windows.Forms.Control.Top" /> property value of the control.
        /// </param>
        /// <param name="width">The new
        /// <see cref="System.Windows.Forms.Control.Width" /> property value of the control.
        /// </param>
        /// <param name="height">The new
        /// <see cref="System.Windows.Forms.Control.Height" /> property value of the control.
        /// </param>
        /// <param name="specified">A bitwise combination of the
        /// <see cref="System.Windows.Forms.BoundsSpecified" /> values. For any parameter not specified, the current value will be used.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Gets the rectangle that represents the client area of the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the client area of the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual Rectangle ClientRectangle { get { Gdk.Rectangle allocation = this.Widget.Allocation; return new Rectangle(allocation.X, allocation.Y, allocation.Width, allocation.Height); } }
        /// <summary>
        /// Gets or sets the height and width of the client area of the control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that represents the dimensions of the client area of the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Size ClientSize { get { return new Size(Widget.AllocatedWidth, this.Widget.AllocatedHeight); } set { this.Widget.SetSizeRequest(value.Width, value.Height); } }
        /// <summary>
        /// Gets the window handle that the control is bound to.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that contains the window handle (HWND) of the control.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual IntPtr Handle { get => this.Widget == null ? IntPtr.Zero : this.Widget.Handle; }
        /// <summary>
        /// Gets or sets the space between controls.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> representing the space between controls.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual Padding Margin { get; set; }
        /// <summary>
        /// Gets or sets the size that is the upper limit that
        /// <see cref="System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)" /> can specify.
        /// </summary>
        /// <returns>An ordered pair of type
        /// <see cref="System.Drawing.Size" /> representing the width and height of a rectangle.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Size MaximumSize { get; set; }
        /// <summary>
        /// Gets or sets the size that is the lower limit that
        /// <see cref="System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)" /> can specify.
        /// </summary>
        /// <returns>An ordered pair of type
        /// <see cref="System.Drawing.Size" /> representing the width and height of a rectangle.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Size MinimumSize { get; set; }

        private BorderStyle _BorderStyle;
        public virtual BorderStyle BorderStyle
        {
            get { return _BorderStyle; }
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
        /// Conceals the control from the user.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Hide()
        {
            if (this.GtkControl is Misc con)
            {
                con.Hide();
            }
        }
        /// <summary>
        /// Displays the control to the user.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Show()
        {
            if (this.Widget != null)
            {
                this.Widget.ShowAll();
            }
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.PaintEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.ParentChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnParentChanged(EventArgs e)
        {
            ParentChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Temporarily suspends the layout logic for the control.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void SuspendLayout()
        {
            _Created = false;
        }
        /// <summary>
        /// Forces the control to apply layout logic to all its child controls.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual void PerformLayout()
        {
            _Created = true;
        }
        /// <summary>
        /// Forces the control to apply layout logic to all its child controls.
        /// </summary>
        /// <param name="affectedControl">A
        /// <see cref="System.Windows.Forms.Control" /> that represents the most recently changed control.
        /// </param>
        /// <param name="affectedProperty">The name of the most recently changed property on the control.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual void PerformLayout(Control affectedControl, string affectedProperty)
        {
            _Created = true;
        }
        /// <summary>
        /// Causes the control to redraw the invalidated regions within its client area.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void Update()
        {
            if (this.Widget != null)
            {
                this.Widget.Window.ProcessUpdates(true);
                this.Widget.QueueDraw();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void BeginInit()
        {

        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void EndInit()
        {

        }

        public new void Dispose()
        {
            Dispose(true);
            base.Dispose();
        }
        /// <summary>
        /// Releases the unmanaged resources used by the
        /// <see cref="System.Windows.Forms.Control" /> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
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
            catch { }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Gets the required creation parameters when the control handle is created.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.CreateParams" /> that contains the required creation parameters when the handle to the control is created.
        /// </returns>
        protected virtual CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = new CreateParams();
                createParams.ExStyle |= 32;
                return createParams;
            }
        }

        public bool ParticipatesInLayout => false;

        PropertyStore IArrangedElement.Properties => throw new NotImplementedException();

        IArrangedElement IArrangedElement.Container => throw new NotImplementedException();


        private ArrangedElementCollection arrangedElementCollection;

        public ArrangedElementCollection Children => arrangedElementCollection;
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e)
        {
            Load?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.Move" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMove(EventArgs e)
        {
            Move?.Invoke(this, e);
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidating(CancelEventArgs e)
        {
            Validating?.Invoke(this, e);
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidated(CancelEventArgs e)
        {
            Validated?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.Resize" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResize(EventArgs e)
        {
            Resize?.Invoke(this, e);
        }
        public virtual void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.Click" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseDoubleClick" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseClick" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseHover" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseHover(EventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseLeave(EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.KeyDown" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.KeyEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.KeyUp" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.KeyEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.VisibleChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.SizeChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnSizeChanged(EventArgs e)
        {
            SizeChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Activates a child control. Optionally specifies the direction in the tab order to select the control from.
        /// </summary>
        /// <param name="directed">true to specify the direction of the control to select; otherwise, false.
        /// </param>
        /// <param name="forward">true to move forward in the tab order; false to move backward in the tab order.
        /// </param>
        protected virtual void Select(bool directed, bool forward)
        {

        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.GotFocus" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnGotFocus(EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }
        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows
        /// <see cref="System.Windows.Forms.Message" /> to process.
        /// </param>
        protected virtual void WndProc(ref Message m)
        {
            //Console.WriteLine($"HWnd:{m.HWnd},WParam:{m.WParam},LParam:{m.LParam},Msg:{m.Msg}");
        }

        public void SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            throw new NotImplementedException();
        }

        void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string propertyName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseMove" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.MouseWheel" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }
    }
}

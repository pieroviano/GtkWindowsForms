/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 */

using System.ComponentModel;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a window or dialog box that makes up an application's user interface.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
[DesignerCategory("Form")]
[DefaultEvent(nameof(Load)),
InitializationEvent(nameof(Load))]
public partial class Form : ContainerControl, IWin32Window
{
    /// <summary>
    /// Backing field for the
    /// <see cref="T:System.Windows.Forms.Form.FormBorderStyle" /> applied to this form.
    /// </summary>
    public FormBorderStyle formBorderStyle = FormBorderStyle.Sizable;

    /// <summary>
    /// Underlying GTK window wrapper used by this form.
    /// </summary>
    public FormBase self = new FormBase();

    private ObjectCollection _ObjectCollection;

    private FormWindowState _WindowState = FormWindowState.Normal;

    private readonly Gtk.Application app = Application.Init();

    private readonly Gtk.Overlay contanter = new Gtk.Overlay();

    private bool Is_Control_Shown = false;

    /// <summary>
    /// Occurs after the form has been closed.
    /// </summary>
    public event FormClosedEventHandler FormClosed;

    /// <summary>
    /// Occurs as the form is being closed. Handlers can cancel the close.
    /// </summary>
    public event FormClosingEventHandler FormClosing;

    /// <summary>
    /// Occurs when the form is loaded.
    /// </summary>
    public override event EventHandler Load;

    /// <summary>
    /// Occurs the first time the form is shown.
    /// </summary>
    public event EventHandler Shown;

    /// <summary>
    /// Occurs when the form's size changes.
    /// </summary>
    public override event EventHandler SizeChanged;

    /// <summary>
    /// Occurs when the form's scroll position changes.
    /// </summary>
    public override event ScrollEventHandler Scroll
    {
        add { self.Scroll += value; }
        remove { self.Scroll += value; }
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Form" /> class.
    /// </summary>
    public Form() : base()
    {
        Init();
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Form" /> class with the specified title.
    /// </summary>
    /// <param name="title">
    /// The initial text displayed in the form's title bar.
    /// </param>
    public Form(string title) : this()
    {
        self.Title = title;
    }

    /// <summary>
    /// Gets or sets the design-time autoscale dimensions for the form.
    /// </summary>
    public SizeF AutoScaleDimensions { get; set; }

    /// <summary>
    /// Gets or sets the automatic scaling mode of the form.
    /// </summary>
    public AutoScaleMode AutoScaleMode { get; set; }

    /// <summary>
    /// Gets or sets the height and width of the client area of the form.
    /// </summary>
    public override Size ClientSize
    {
        get
        {
            return new Size(self.AllocatedWidth, self.AllocatedHeight);
        }
        set
        {
            self.WidthRequest = -1;
            self.HeightRequest = -1;
            self.SetDefaultSize(value.Width, value.Height);
        }
    }

    /// <summary>
    /// Gets the collection of controls contained within the form.
    /// </summary>
    public new ObjectCollection Controls { get { return _ObjectCollection; } }

    /// <summary>
    /// Gets or sets the dialog result produced by the form.
    /// </summary>
    public DialogResult DialogResult { get; set; }

    /// <summary>
    /// Gets or sets the border style of the form.
    /// </summary>
    public FormBorderStyle FormBorderStyle
    {
        get { return formBorderStyle; }
        set
        {
            formBorderStyle = value;
            self.Resizable = value == FormBorderStyle.Sizable || value == FormBorderStyle.SizableToolWindow;
            if (value == FormBorderStyle.None)
            {
                self.Decorated = false; // remove toolbar
            }
            else if (value == FormBorderStyle.FixedToolWindow)
            {
                self.Decorated = true;
                self.TypeHint = Gdk.WindowTypeHint.Dialog;
            }
            else if (value == FormBorderStyle.SizableToolWindow)
            {
                self.Decorated = true;
                self.TypeHint = Gdk.WindowTypeHint.Dialog;
            }
            else
            {
                self.Decorated = true;
                self.TypeHint = Gdk.WindowTypeHint.Normal;
            }
        }
    }

    /// <summary>
    /// Gets the underlying GTK control object used by the form.
    /// </summary>
    public override object GtkControl { get => self; }

    /// <summary>
    /// Gets the native window handle for this form.
    /// </summary>
    public override IntPtr Handle => self.Handle;

    /// <summary>
    /// Gets or sets the icon associated with this form.
    /// </summary>
    public System.Drawing.Icon Icon { get; set; }

    /// <summary>
    /// Gets or sets the primary menu strip for the form.
    /// </summary>
    public MenuStrip MainMenuStrip { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Maximize button is displayed.
    /// </summary>
    public bool MaximizeBox { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Minimize button is displayed.
    /// </summary>
    public bool MinimizeBox { get; set; } = true;

    /// <summary>
    /// Gets or sets the form's opacity (0.0 - fully transparent, 1.0 - fully opaque).
    /// </summary>
    public double Opacity { get { return self.Opacity; } set { self.Opacity = value; } }

    /// <summary>
    /// Gets or sets the padding within the form. Padding is applied to the internal GTK overlay margins.
    /// </summary>
    public override Padding Padding
    {
        get => base.Padding;
        set
        {
            base.Padding = value;
            contanter.MarginStart = value.Left;
            contanter.MarginTop = value.Top;
            contanter.MarginEnd = value.Right;
            contanter.MarginBottom = value.Bottom;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the form's icon is shown.
    /// </summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the form is displayed in the taskbar.
    /// </summary>
    public bool ShowInTaskbar { get { return self.SkipTaskbarHint == false; } set { self.SkipTaskbarHint = value == false; } }

    /// <summary>
    /// Gets or sets the starting position of the form when first shown.
    /// </summary>
    public FormStartPosition StartPosition { get; set; }

    /// <summary>
    /// Gets or sets the text associated with the form (displayed in the title bar).
    /// </summary>
    public override string Text { get { return self.Title; } set { self.Title = value; } }

    /// <summary>
    /// Gets or sets the state (normal, minimized, maximized) of the form.
    /// Setting this property will attempt to change the native window state when the window is mapped.
    /// </summary>
    public FormWindowState WindowState
    {
        get
        {
            return _WindowState;
        }
        set
        {
            _WindowState = value;
            if (self.IsMapped)
            {
                if (value == FormWindowState.Maximized)
                {
                    self.Maximize();
                }
                else if (value == FormWindowState.Minimized)
                {
                    self.Iconify();
                }
            }
        }
    }

    /// <summary>
    /// Activates the form and gives it focus.
    /// </summary>
    /// <returns>
    /// True if the activation succeeded; otherwise false.
    /// </returns>
    public bool Activate()
    {
        return self.Activate();
    }

    /// <summary>
    /// Closes the form. Uses a short timer to defer actual close so UI handlers can complete.
    /// </summary>
    public void Close()
    {
        if (MessageBox.Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            CloseImmediately();
            return;
        }
        var timer = new System.Windows.Forms.Timer
        {
            Interval = 100
        };

        timer.Tick += (s, e) =>
        {
            timer.Stop();
            CloseImmediately();
        };

        timer.Start();
    }

    /// <summary>
    /// Immediately closes the native window backing this form.
    /// </summary>
    public void CloseImmediately()
    {
        self?.CloseWindow();
    }

    /// <summary>
    /// Hides the form.
    /// </summary>
    public override void Hide()
    {
        self?.Hide();
    }

    /// <summary>
    /// Forces the control to perform layout; marks the form as created.
    /// </summary>
    public override void PerformLayout()
    {
        _Created = true;
    }

    /// <summary>
    /// Resumes layout logic for the form.
    /// </summary>
    /// <param name="resume">
    /// If true, resume layout processing; otherwise adjust created flag.
    /// </param>
    public override void ResumeLayout(bool resume)
    {
        _Created = resume == false;
    }

    /// <summary>
    /// Shows the form.
    /// </summary>
    public override void Show()
    {
        this.Show(null);
    }

    /// <summary>
    /// Shows the form with an optional owner window.
    /// </summary>
    /// <param name="owner">
    /// The owner window. May be null.
    /// </param>
    public void Show(IWin32Window owner)
    {
        if (owner == this)
        {
            throw new InvalidOperationException("OwnsSelfOrOwner");
        }

        if (base.Visible)
        {
            throw new InvalidOperationException("ShowDialogOnVisible");
        }

        if (!base.Enabled)
        {
            throw new InvalidOperationException("ShowDialogOnDisabled");
        }

        if (owner != null && owner is Form parent)
        {
            this.Parent = parent;
            self.SetPosition(WindowPosition.CenterOnParent);
            self.DestroyWithParent = true;
            self.Activate();
        }

        if (self.IsVisible == false)
        {
            this.FormBorderStyle = this.FormBorderStyle;
            if (this.MaximizeBox == false && this.MinimizeBox == false)
            {
                self.TypeHint = Gdk.WindowTypeHint.Dialog;
            }
            self.Resize(self.DefaultWidth, self.DefaultHeight);

            if (this.WindowState == FormWindowState.Maximized)
            {
                self.Maximize();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                self.Iconify();
            }
            if (self.IsMapped == false)
            {
                try
                {
                    if (this.ShowIcon)
                    {
                        if (this.Icon != null)
                        {
                            if (this.Icon.Pixbuf != null)
                                self.Icon = this.Icon.Pixbuf;
                            else if (this.Icon.PixbufData != null)
                                self.Icon = new Gdk.Pixbuf(this.Icon.PixbufData);
                            else if (this.Icon.FileName != null && System.IO.File.Exists(this.Icon.FileName))
                                self.SetIconFromFile(this.Icon.FileName);
                            else if (this.Icon.FileName != null && System.IO.File.Exists("Resources\\" + this.Icon.FileName))
                                self.SetIconFromFile("Resources\\" + this.Icon.FileName);
                        }
                        var titlebar = (Gtk.HeaderBar)self.Titlebar;
                        var flag = new Gtk.Image(self.Icon)
                        {
                            Visible = true
                        };
                        titlebar.PackStart(flag);
                    }
                    else
                    {
                        self.Icon = new Gdk.Pixbuf(this.GetType().Assembly, "Resources.System.view-more.png");
                    }

                }
                catch
                {

                }
            }
        }
        self.ShowAll();
    }

    /// <summary>
    /// Shows the form as a modal dialog.
    /// </summary>
    /// <returns>
    /// The dialog result produced by the form.
    /// </returns>
    public DialogResult ShowDialog()
    {
        return ShowDialog(null);
    }

    /// <summary>
    /// Shows the form as a modal dialog with the specified owner.
    /// </summary>
    /// <param name="owner">
    /// The owner window. May be null.
    /// </param>
    /// <returns>
    /// The dialog result produced by the form.
    /// </returns>
    public DialogResult ShowDialog(IWin32Window owner)
    {
        if (owner == this)
        {
            throw new ArgumentException("OwnsSelfOrOwner", "showDialog");
        }

        if (base.Visible)
        {
            throw new InvalidOperationException("ShowDialogOnVisible");
        }

        if (!base.Enabled)
        {
            throw new InvalidOperationException("ShowDialogOnDisabled");
        }
        Show(owner);
        int irun = self.Run();

        return this.DialogResult;
    }

    /// <summary>
    /// Suspends layout logic for the form.
    /// </summary>
    public override void SuspendLayout()
    {
        _Created = false;
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        self.CloseWindow();
    }

    private void Control_Shown(object sender, EventArgs e)
    {
        if (Is_Control_Shown == false)
        {
            Is_Control_Shown = true;
            if (self.Titlebar is Gtk.HeaderBar titlebar)
            {
                titlebar.DecorationLayout = "menu:close";
                if (formBorderStyle == FormBorderStyle.FixedToolWindow || formBorderStyle == FormBorderStyle.SizableToolWindow)
                {
                }
                else
                {
                    if (MaximizeBox == true)
                    {
                        var maximize = new Gtk.Button("window-maximize-symbolic", IconSize.SmallToolbar) { Name = "maximize", Visible = true, Relief = ReliefStyle.None, Valign = Align.Center, Halign = Align.Center };
                        maximize.StyleContext.AddClass("maximize");
                        maximize.StyleContext.AddClass("titlebutton");
                        maximize.Clicked += Maximize_Clicked;
                        titlebar.PackEnd(maximize);
                    }
                    if (MinimizeBox == true)
                    {
                        var minimize = new Gtk.Button("window-minimize-symbolic", IconSize.SmallToolbar) { Name = "minimize", Visible = true, Relief = ReliefStyle.None, Valign = Align.Center, Halign = Align.Center };
                        minimize.StyleContext.AddClass("minimize");
                        minimize.StyleContext.AddClass("titlebutton");
                        minimize.Clicked += Minimize_Clicked;
                        titlebar.PackEnd(minimize);
                    }
                }
            }
            OnLoadHandler();
        }
        OnShownHandler();
    }

    private void Init()
    {
        this.SetScrolledWindow(self);
        contanter.Valign = Gtk.Align.Fill;
        contanter.Halign = Gtk.Align.Fill;
        contanter.Hexpand = true;
        contanter.Vexpand = true;
        contanter.MarginBottom = 0;
        contanter.MarginEnd = 0;
        contanter.Add(new Gtk.Fixed() { Halign = Align.Fill, Valign = Align.Fill });
        self.ScrollView.Child = contanter;
        _ObjectCollection = new ObjectCollection(this, contanter);
        self.ResizeChecked += Self_ResizeChecked;
        self.Shown += Control_Shown;
        self.CloseWindowEvent += Self_CloseWindowEvent;
    }

    private void Maximize_Clicked(object sender, EventArgs e)
    {
        var maximize = (Gtk.Button)sender;
        if (maximize.Name == "restore")
        {
            self.Unmaximize();
            maximize.Image = Gtk.Image.NewFromIconName("window-maximize-symbolic", IconSize.SmallToolbar);
            maximize.Name = "maximize";
        }
        else
        {
            self.Maximize();
            maximize.Image = Gtk.Image.NewFromIconName("window-restore-symbolic", IconSize.SmallToolbar);
            maximize.Name = "restore";
        }
    }

    private void Minimize_Clicked(object sender, EventArgs e)
    {
        self.Iconify();
    }

    private void OnLoadHandler()
    {
        Load?.Invoke(this, EventArgs.Empty);
    }

    private void OnShownHandler()
    {
        Shown?.Invoke(this, EventArgs.Empty);
    }

    private bool Self_CloseWindowEvent(object sender, EventArgs e)
    {
        var closing = new FormClosingEventArgs(CloseReason.UserClosing, false);
        FormClosing?.Invoke(this, closing);

        if (closing.Cancel == false)
        {
            FormClosed?.Invoke(this, new FormClosedEventArgs(CloseReason.UserClosing));
        }

        return closing.Cancel == false;
    }

    private void Self_ResizeChecked(object sender, EventArgs e)
    {
        SizeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Placeholder type used to represent an MDI layout. Kept for compatibility.
    /// </summary>
    public class MdiLayout
    {
    }

    /// <summary>
    /// Collection that stores controls added to the form.
    /// </summary>
    public class ObjectCollection : ControlCollection
    {
        readonly Gtk.Container __owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.Form.ObjectCollection" /> class.
        /// </summary>
        /// <param name="control">
        /// The parent control.
        /// </param>
        /// <param name="owner">
        /// The GTK container that will host child controls.
        /// </param>
        public ObjectCollection(Control control, Gtk.Container owner) : base(control, owner)
        {
            __owner = owner;
        }

    }
}
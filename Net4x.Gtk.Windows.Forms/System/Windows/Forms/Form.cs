/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 */

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
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

        public FormBorderStyle formBorderStyle = FormBorderStyle.Sizable;

        public FormBase self = new FormBase();

        private ObjectCollection _ObjectCollection;

        private FormWindowState _WindowState = FormWindowState.Normal;

        private Gtk.Application app = Application.Init();

        private Gtk.Overlay contanter = new Gtk.Overlay();

        private bool Is_Control_Shown = false;

        public event FormClosedEventHandler FormClosed;

        public event FormClosingEventHandler FormClosing;

        public override event EventHandler Load;

        public event EventHandler Shown;

        public override event EventHandler SizeChanged;

        public override event ScrollEventHandler Scroll
        {
            add { self.Scroll += value; }
            remove { self.Scroll += value; }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Form" /> class.
        /// </summary>
        public Form() : base()
        {
            Init();
        }

        public Form(string title) : this()
        {
            self.Title = title;
        }

        public SizeF AutoScaleDimensions { get; set; }

        public AutoScaleMode AutoScaleMode { get; set; }

        /// <summary>
        /// Gets or sets the size of the client area of the form.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that represents the size of the form's client area.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        public new ObjectCollection Controls { get { return _ObjectCollection; } }

        /// <summary>
        /// Gets or sets the dialog result for the form.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DialogResult" /> that represents the result of the form when used as a dialog box.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is outside the range of valid values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// Gets or sets the border style of the form.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.FormBorderStyle" /> that represents the style of border to display for the form. The default is FormBorderStyle.Sizable.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is outside the range of valid values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        public override object GtkControl { get => self; }

        public override IntPtr Handle => self.Handle;

        /// <summary>
        /// Gets or sets the icon for the form.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Icon" /> that represents the icon for the form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Drawing.Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the primary menu container for the form.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.MenuStrip" /> that represents the container for the menu structure of the form. The default is null.
        /// </returns>
        public MenuStrip MainMenuStrip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the form.
        /// </summary>
        /// <returns>true to display a Maximize button for the form; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool MaximizeBox { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the form.
        /// </summary>
        /// <returns>true to display a Minimize button for the form; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool MinimizeBox { get; set; } = true;

        /// <summary>
        /// Gets or sets the opacity level of the form.
        /// </summary>
        /// <returns>The level of opacity for the form. The default is 1.00.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public double Opacity { get { return self.Opacity; } set { self.Opacity = value; } }

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
        /// Gets or sets a value indicating whether an icon is displayed in the caption bar of the form.
        /// </summary>
        /// <returns>true if the form displays an icon in the caption bar; otherwise, false. The default is true.
        /// </returns>
        public bool ShowIcon { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the form is displayed in the Windows taskbar.
        /// </summary>
        /// <returns>true to display the form in the Windows taskbar at run time; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowInTaskbar { get { return self.SkipTaskbarHint == false; } set { self.SkipTaskbarHint = value == false; } }

        /// <summary>
        /// Gets or sets the starting position of the form at run time.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.FormStartPosition" /> that represents the starting position of the form.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is outside the range of valid values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        public FormStartPosition StartPosition { get; set; }

        /// <returns>
        /// The text associated with this control.
        /// </returns>
        public override string Text { get { return self.Title; } set { self.Title = value; } }

        /// <summary>
        /// Gets or sets a value that indicates whether form is minimized, maximized, or normal.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.FormWindowState" /> that represents whether form is minimized, maximized, or normal. The default is FormWindowState.Normal.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is outside the range of valid values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Activate()
        {
            return self.Activate();
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The form was closed while a handle was being created.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">You cannot call this method from the
        /// <see cref="System.Windows.Forms.Form.Activated" /> event when
        /// <see cref="System.Windows.Forms.Form.WindowState" /> is set to
        /// <see cref="System.Windows.Forms.FormWindowState.Maximized" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

        public void CloseImmediately()
        {
            if (self != null)
            {
                self.CloseWindow();
            }
        }

        public override void Hide()
        {
            if (self != null)
            {
                self.Hide();
            }
        }

        public override void PerformLayout()
        {
            _Created = true;
        }

        public override void ResumeLayout(bool resume)
        {
            _Created = resume == false;
        }

        public override void Show()
        {
            this.Show(null);
        }

        /// <summary>
        /// Shows the form with the specified owner to the user.
        /// </summary>
        /// <param name="owner">Any object that implements
        /// <see cref="System.Windows.Forms.IWin32Window" /> and represents the top-level window that will own this form.
        /// </param>
        /// <exception cref="System.InvalidOperationException">The form being shown is already visible.-or- The form specified in the
        /// <paramref name="owner" /> parameter is the same as the form being shown.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.-or-The current process is not running in user interactive mode (for more information, see
        /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" />).
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
                            Gtk.HeaderBar titlebar = (Gtk.HeaderBar)self.Titlebar;
                            Gtk.Image flag = new Gtk.Image(self.Icon);
                            flag.Visible = true;
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
        /// Shows the form as a modal dialog box.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DialogResult" /> values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The form being shown is already visible.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.-or-The current process is not running in user interactive mode (for more information, see
        /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" />).
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Shows the form as a modal dialog box with the specified owner.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DialogResult" /> values.
        /// </returns>
        /// <param name="owner">Any object that implements
        /// <see cref="System.Windows.Forms.IWin32Window" /> that represents the top-level window that will own the modal dialog box.
        /// </param>
        /// <exception cref="System.ArgumentException">The form specified in the
        /// <paramref name="owner" /> parameter is the same as the form being shown.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The form being shown is already visible.-or- The form being shown is disabled.-or- The form being shown is not a top-level window.-or- The form being shown as a dialog box is already a modal form.-or-The current process is not running in user interactive mode (for more information, see
        /// <see cref="System.Windows.Forms.SystemInformation.UserInteractive" />).
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
                            Gtk.Button maximize = new Gtk.Button("window-maximize-symbolic", IconSize.SmallToolbar) { Name = "maximize", Visible = true, Relief = ReliefStyle.None, Valign = Align.Center, Halign = Align.Center };
                            maximize.StyleContext.AddClass("maximize");
                            maximize.StyleContext.AddClass("titlebutton");
                            maximize.Clicked += Maximize_Clicked;
                            titlebar.PackEnd(maximize);
                        }
                        if (MinimizeBox == true)
                        {
                            Gtk.Button minimize = new Gtk.Button("window-minimize-symbolic", IconSize.SmallToolbar) { Name = "minimize", Visible = true, Relief = ReliefStyle.None, Valign = Align.Center, Halign = Align.Center };
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
            Gtk.Button maximize = (Gtk.Button)sender;
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
            if (Load != null)
                Load(this, EventArgs.Empty);
        }

        private void OnShownHandler()
        {
            if (Shown != null)
                Shown(this, EventArgs.Empty);
        }

        private bool Self_CloseWindowEvent(object sender, EventArgs e)
        {
            FormClosingEventArgs closing = new FormClosingEventArgs(CloseReason.UserClosing, false);
            if (FormClosing != null)
                FormClosing(this, closing);

            if (closing.Cancel == false)
            {
                if (FormClosed != null)
                    FormClosed(this, new FormClosedEventArgs(CloseReason.UserClosing));
            }

            return closing.Cancel == false;
        }

        private void Self_ResizeChecked(object sender, EventArgs e)
        {
            if (SizeChanged != null)
                SizeChanged(this, EventArgs.Empty);
        }

        public class MdiLayout
        {
        }

        public class ObjectCollection : ControlCollection
        {
            Gtk.Container __owner;

            public ObjectCollection(Control control, Gtk.Container owner) : base(control, owner)
            {
                __owner = owner;
            }

        }
    }
}

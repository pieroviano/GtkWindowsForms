/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a single tab page in a
    /// <see cref="System.Windows.Forms.TabControl" />
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class TabPage : ContainerControl
    {

        public readonly TabPageBase self = new TabPageBase();

        public override object GtkControl => self;

        internal Gtk.Label _tabLabel = new Gtk.Label();

        private ControlCollection _controls;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TabPage" /> class.
        /// </summary>
        public TabPage() : base()
        {
            _controls = new ControlCollection(this, self.Content);
            this.Dock = DockStyle.Fill;
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TabPage" /> class and specifies the text for the tab.
        /// </summary>
        /// <param name="text">The text for the tab.
        /// </param>
        public TabPage(string text) : this()
        {
            _tabLabel.Text = text;
        }
        /// <summary>
        /// This property is not meaningful for this control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override Point Location
        {
            get
            {
                return new Point(0, 0);
            }
            set
            {
            }
        }
        /// <summary>
        /// This member is not meaningful for this control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.DockStyle" /> value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public new DockStyle Dock
        {
            get
            {
                return DockStyle.Fill;
            }
            set { base.Dock = DockStyle.Fill; }
        }
        /// <summary>
        /// Gets or sets the text to display on the tab.
        /// </summary>
        /// <returns>The text to display on the tab.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override string Text { get { return _tabLabel.Text; } set { _tabLabel.Text = value; } }
        public Gtk.Label TabLabel { get { return _tabLabel; } }

        public new ControlCollection Controls => _controls;
        /// <summary>
        /// Gets or sets the index to the image displayed on this tab.
        /// </summary>
        /// <returns>The zero-based index to the image in the
        /// <see cref="System.Windows.Forms.TabControl.ImageList" /> that appears on the tab. The default is -1, which signifies no image.
        /// </returns>
        /// <exception cref="System.ArgumentException">The
        /// <see cref="System.Windows.Forms.TabPage.ImageIndex" /> value is less than -1.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int ImageIndex { get; set; }
        /// <summary>
        /// Gets or sets the key accessor for the image in the
        /// <see cref="System.Windows.Forms.TabControl.ImageList" /> of the associated
        /// <see cref="System.Windows.Forms.TabControl" />
        /// </summary>
        /// <returns>A string representing the key of the image.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageKey { get; set; }

        public List<object> ImageList { get; set; }
        public override Padding Padding
        {
            get => base.Padding;
            set
            {
                base.Padding = value;
                self.Content.MarginStart = value.Left;
                self.Content.MarginTop = value.Top;
                self.Content.MarginEnd = value.Right;
                self.Content.MarginBottom = value.Bottom;
            }
        }

        private Size _size;
        public override Size Size { get => _size; set { _size = value; } }
    }
}

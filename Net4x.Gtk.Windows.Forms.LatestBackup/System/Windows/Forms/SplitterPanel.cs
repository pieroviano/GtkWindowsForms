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


namespace System.Windows.Forms
{
    /// <summary>
    /// Creates a panel that is associated with a
    /// <see cref="System.Windows.Forms.SplitContainer" />
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public sealed class SplitterPanel : Panel
    {

        internal SplitContainer Owner;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SplitterPanel" /> class with its specified
        /// <see cref="System.Windows.Forms.SplitContainer" />
        /// </summary>
        /// <param name="owner">The
        /// <see cref="System.Windows.Forms.SplitContainer" /> that contains the
        /// <see cref="System.Windows.Forms.SplitterPanel" />
        /// </param>
        public SplitterPanel(SplitContainer owner) : base()
        {
            self.Override.AddClass("SplitterPanel");
            Owner = owner;
            self.BorderWidth = 0;
            self.ShadowType = Gtk.ShadowType.None;
            self.Margin = 0;
            self.Halign = Gtk.Align.Fill;
            self.Valign = Gtk.Align.Fill;
            self.Hexpand = false;
            self.Vexpand = false;
        }
        /// <summary>
        /// Gets or sets which edge of the
        /// <see cref="System.Windows.Forms.SplitContainer" /> that the
        /// <see cref="System.Windows.Forms.SplitterPanel" /> is docked to. This property is not relevant to this class.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.DockStyle" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override DockStyle Dock { get { return DockStyle.Fill; } set { } }
        /// <summary>
        /// Gets or sets the height and width of the
        /// <see cref="System.Windows.Forms.SplitterPanel" /> This property is not relevant to this class.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Size" /> that represents the height and width of the
        /// <see cref="System.Windows.Forms.SplitterPanel" /> in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override Size Size { get; set; }
        /// <summary>
        /// Gets or sets the width of the
        /// <see cref="System.Windows.Forms.SplitterPanel" />
        /// </summary>
        /// <returns>The width of the
        /// <see cref="System.Windows.Forms.SplitterPanel" /> in pixels.
        /// </returns>
        public override int Width { get; set; }
        /// <summary>
        /// Gets or sets the height of the
        /// <see cref="System.Windows.Forms.SplitterPanel" />
        /// </summary>
        /// <returns>The height of the
        /// <see cref="System.Windows.Forms.SplitterPanel" />, in pixels.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The height cannot be set.
        /// </exception>
        public override int Height { get; set; }
    }
}

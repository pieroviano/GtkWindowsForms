/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */
using System;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a text box in a
    /// <see cref="System.Windows.Forms.ToolStrip" />
    /// that allows the user to enter text.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripTextBox : WidgetToolStrip<Gtk.MenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripTextBox" /> class.
        /// </summary>
        public ToolStripTextBox() : base("ToolStripTextBox", null)
        {

        }
        /// <summary>
        /// Gets or sets the maximum number of characters the user can type or paste into the text box control.
        /// </summary>
        /// <returns>The number of characters that can be entered into the control. The default is 32767 characters.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int MaxLength { get => this.entry.MaxLength; set => this.entry.MaxLength = value; }

        public override Size Size { get => base.Size; set { this.entry.WidthRequest = value.Width; this.entry.HeightRequest = value.Height; base.Size = value; } }

        public override string Text { get => this.entry.Text; set => this.entry.Text = value; }
    }

}

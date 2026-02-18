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

    public class ToolStripTextBox : WidgetToolStrip<Gtk.MenuItem>
    {
        public ToolStripTextBox() : base("ToolStripTextBox", null)
        {
            
        }

        public override Size Size { get => base.Size; set { this.entry.WidthRequest = value.Width;this.entry.HeightRequest = value.Height; base.Size = value; } }

        public int MaxLength { get => this.entry.MaxLength; set => this.entry.MaxLength = value; }
        public override string Text { get => this.entry.Text; set => this.entry.Text = value; }
    }

}
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
    /// <see cref="T:System.Windows.Forms.ToolStrip" />
    /// that allows the user to enter text.
    /// </summary>
    public class ToolStripTextBox : WidgetToolStrip<Gtk.MenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripTextBox"/> class.
        /// </summary>
        public ToolStripTextBox() : base("ToolStripTextBox", null)
        {

        }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        public int MaxLength { get => this.entry.MaxLength; set => this.entry.MaxLength = value; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public override Size Size { get => base.Size; set { this.entry.WidthRequest = value.Width; this.entry.HeightRequest = value.Height; base.Size = value; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public override string Text { get => this.entry.Text; set => this.entry.Text = value; }

    }

}

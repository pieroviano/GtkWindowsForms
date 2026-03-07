/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows status bar control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class StatusStrip : ToolStrip
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.StatusStrip" /> class.
        /// </summary>
        public StatusStrip() : base()
        {
            this.self.StyleContext.RemoveClass("ToolStrip");
            this.self.StyleContext.AddClass("StatusStrip");
            Dock = DockStyle.Bottom;
        }

        public override Point Location { get => base.Location; set => base.Location = new Point(value.X, value.Y - 12); }

        /// <summary>
        /// Gets or sets a value indicating whether ToolTips are shown for the
        /// <see cref="System.Windows.Forms.StatusStrip" />
        /// </summary>
        /// <returns>true if ToolTips are shown for the
        /// <see cref="System.Windows.Forms.StatusStrip" />; otherwise, false. The default is false.
        /// </returns>
        [DefaultValue(false)]
        public bool ShowItemToolTips { get; set; }

        public override Size Size { get => base.Size; set => base.Size = new Size(value.Width, value.Height + 12); }

        /// <summary>
        /// Gets or sets a value indicating whether a sizing handle (grip) is displayed in the lower-right corner of the control.
        /// </summary>
        /// <returns>true if a grip is displayed; otherwise, false. The default is true.
        /// </returns>
        [DefaultValue(true)]
        public bool SizingGrip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.StatusStrip" /> stretches from end to end in its container.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.StatusStrip" /> stretches from end to end in its
        /// <see cref="System.Windows.Forms.ToolStripContainer" />; otherwise, false. The default is true.
        /// </returns>
        [DefaultValue(true)]
        public bool Stretch { get; set; }
    }
}

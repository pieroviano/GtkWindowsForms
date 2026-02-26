/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a container for Windows toolbar objects.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ToolStrip : Control
    {

        public readonly ToolStripBase self = new ToolStripBase();

        public override object GtkControl => self;

        public ToolStripItemCollection toolStripItemCollection;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStrip" /> class.
        /// </summary>
        public ToolStrip() : base()
        {
            toolStripItemCollection = new ToolStripItemCollection(this);
            self.ActivateCurrent += ToolStripItem_Activated;
        }
        public ToolStrip(string owner) : base()
        {
            self.Hexpand = false;
            self.Vexpand = false;
            self.Valign = Gtk.Align.Start;
            self.Halign = Gtk.Align.Start;
            toolStripItemCollection = new ToolStripItemCollection(this, owner);
            self.ActivateCurrent += ToolStripItem_Activated;
        }
        private void ToolStripItem_Activated(object sender, ActivateCurrentArgs e)
        {
            if (DropDownItemClicked != null)
            {
                DropDownItemClicked(this, new ToolStripItemClickedEventArgs(new ToolStripItem()));
            }
            if (Click != null)
            {
                Click(sender, e);
            }
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
            if (CheckStateChanged != null)
            {
                CheckStateChanged(this, e);
            }
        }

        public override Size Size { get => base.Size; set => base.Size = new Size(value.Width, 30); }
        /// <summary>
        /// Gets all the items that belong to a
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        /// <returns>An object of type
        /// <see cref="System.Windows.Forms.ToolStripItemCollection" />, representing all the elements contained by a
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ToolStripItemCollection Items
        {
            get
            {
                return toolStripItemCollection;
            }
        }
        /// <summary>
        /// Gets or sets the size, in pixels, of an image used on a
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> value representing the size of the image, in pixels. The default is 16 x 16 pixels.
        /// </returns>
        public Size ImageScalingSize { get; set; }
        /// <summary>
        /// Gets or sets a value indicating how the
        /// <see cref="System.Windows.Forms.ToolStrip" /> lays out the items collection.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle" /> values. The possible values are
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.Table" />,
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.Flow" />,
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.StackWithOverflow" />,
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow" />, and
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value of
        /// <see cref="System.Windows.Forms.ToolStrip.LayoutStyle" /> is not one of the
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public ToolStripLayoutStyle LayoutStyle { get; set; }

        public override event EventHandler Click;

        public event EventHandler CheckedChanged;

        public event EventHandler CheckStateChanged;
        public event ToolStripItemClickedEventHandler DropDownItemClicked;
    }
}

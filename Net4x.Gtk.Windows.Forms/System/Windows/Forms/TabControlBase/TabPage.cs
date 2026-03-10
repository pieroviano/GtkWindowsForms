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
    /// <see cref="T:System.Windows.Forms.TabControl" />
    /// </summary>
    public class TabPage : ContainerControl
    {

        /// <summary>
        /// The self
        /// </summary>
        public readonly TabPageBase self = new TabPageBase();

        /// <summary>
        /// The tab label
        /// </summary>
        internal Gtk.Label _tabLabel = new Gtk.Label();

        /// <summary>
        /// The controls
        /// </summary>
        private ControlCollection _controls;

        /// <summary>
        /// The size
        /// </summary>
        private Size _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPage"/> class.
        /// </summary>
        public TabPage() : base()
        {
            _controls = new ControlCollection(this, self.Content);
            this.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPage"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public TabPage(string text) : this()
        {
            _tabLabel.Text = text;
        }

        /// <summary>
        /// Gets the controls.
        /// </summary>
        /// <value>The controls.</value>
        public new ControlCollection Controls => _controls;

        /// <summary>
        /// Gets or sets the dock.
        /// </summary>
        /// <value>The dock.</value>
        public new DockStyle Dock
        {
            get
            {
                return DockStyle.Fill;
            }
            set { base.Dock = DockStyle.Fill; }
        }

        /// <summary>
        /// Gets the GTK control.
        /// </summary>
        /// <value>The GTK control.</value>
        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the index of the image.
        /// </summary>
        /// <value>The index of the image.</value>
        public int ImageIndex { get; set; }

        /// <summary>
        /// Gets or sets the image key.
        /// </summary>
        /// <value>The image key.</value>
        public string ImageKey { get; set; }

        /// <summary>
        /// Gets or sets the image list.
        /// </summary>
        /// <value>The image list.</value>
        public List<object> ImageList { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
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
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
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

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public override Size Size { get => _size; set { _size = value; } }

        /// <summary>
        /// Gets the tab label.
        /// </summary>
        /// <value>The tab label.</value>
        public Gtk.Label TabLabel { get { return _tabLabel; } }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public override string Text { get { return _tabLabel.Text; } set { _tabLabel.Text = value; } }

    }
}

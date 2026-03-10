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
    /// <filterpriority> 2
    /// </filterpriority>
    public class TabPage : ContainerControl
    {

        public readonly TabPageBase self = new TabPageBase();

        internal Gtk.Label _tabLabel = new Gtk.Label();

        private ControlCollection _controls;

        private Size _size;

        public TabPage() : base()
        {
            _controls = new ControlCollection(this, self.Content);
            this.Dock = DockStyle.Fill;
        }

        public TabPage(string text) : this()
        {
            _tabLabel.Text = text;
        }

        public new ControlCollection Controls => _controls;

        public new DockStyle Dock
        {
            get
            {
                return DockStyle.Fill;
            }
            set { base.Dock = DockStyle.Fill; }
        }

        public override object GtkControl => self;

        public int ImageIndex { get; set; }

        public string ImageKey { get; set; }

        public List<object> ImageList { get; set; }

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

        public override Size Size { get => _size; set { _size = value; } }

        public Gtk.Label TabLabel { get { return _tabLabel; } }

        public override string Text { get { return _tabLabel.Text; } set { _tabLabel.Text = value; } }

    }
}

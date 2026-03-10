/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a
    /// <see cref="T:System.Windows.Forms.ToolStripComboBox" />
    /// that is properly rendered in a
    /// <see cref="T:System.Windows.Forms.ToolStrip" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class ToolStripComboBox : WidgetToolStrip<Gtk.MenuItem>
    {

        private ObjectCollection __itemsData;

        public event EventHandler SelectedIndexChanged;

        public event EventHandler SelectedValueChanged;

        public ToolStripComboBox() : base("ToolStripComboBox", null)
        {
            __itemsData = new ObjectCollection(this.comboBox);
            this.comboBox.Changed += ComboBox_Changed;
        }

        public bool FormattingEnabled { get; set; }

        public new ObjectCollection Items { get { return __itemsData; } }

        public int SelectedIndex { get { return base.comboBox.Active; } }

        public object SelectedItem { get { return __itemsData[SelectedIndex]; } }

        public override Size Size { get => base.Size; set { this.comboBox.WidthRequest = value.Width; this.comboBox.HeightRequest = value.Height; base.Size = value; } }

        private void ComboBox_Changed(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, e);
            if (SelectedValueChanged != null)
                SelectedValueChanged(this, e);
        }

        [ListBindable(false)]
        public class ObjectCollection : ArrayList
        {
            Gtk.ComboBoxText __owner;

            public ObjectCollection(Gtk.ComboBoxText owner)
            {
                __owner = owner;
            }

            public override int Add(object value)
            {
                __owner.AppendText(value?.ToString());

                return base.Add(value);
            }

            public override void AddRange(ICollection c)
            {
                foreach (var item in c)
                    __owner.AppendText(item.ToString());
                base.AddRange(c);
            }

            public override void Clear()
            {
                __owner.Clear();
                base.Clear();
            }

        }
    }

}

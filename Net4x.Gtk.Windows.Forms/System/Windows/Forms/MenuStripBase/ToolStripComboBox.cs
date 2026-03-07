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
    /// <see cref="System.Windows.Forms.ToolStripComboBox" />
    /// that is properly rendered in a
    /// <see cref="System.Windows.Forms.ToolStrip" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public class ToolStripComboBox : WidgetToolStrip<Gtk.MenuItem>
    {

        private ObjectCollection __itemsData;

        public event EventHandler SelectedIndexChanged;

        public event EventHandler SelectedValueChanged;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripComboBox" /> class.
        /// </summary>
        public ToolStripComboBox() : base("ToolStripComboBox", null)
        {
            __itemsData = new ObjectCollection(this.comboBox);
            this.comboBox.Changed += ComboBox_Changed;
        }

        public bool FormattingEnabled { get; set; }

        /// <summary>
        /// Gets a collection of the items contained in this
        /// <see cref="System.Windows.Forms.ToolStripComboBox" />
        /// </summary>
        /// <returns>A collection of items.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public new ObjectCollection Items { get { return __itemsData; } }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        /// <returns>A zero-based index of the currently selected item. A value of negative one (-1) is returned if no item is selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SelectedIndex { get { return base.comboBox.Active; } }

        /// <summary>
        /// Gets or sets currently selected item in the
        /// <see cref="System.Windows.Forms.ToolStripComboBox" />
        /// </summary>
        /// <returns>The object that is the currently selected item or null if there is no currently selected item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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

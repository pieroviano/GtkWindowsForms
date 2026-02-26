/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows combo box control.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class ComboBox : ListControl
    {

        public readonly ComboBoxBase self = new ComboBoxBase();

        public override object GtkControl => self;

        private ObjectCollection __itemsData;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ComboBox" /> class.
        /// </summary>
        public ComboBox() : base()
        {
            self.Entry.HasFrame = false;
            self.Entry.WidthChars = 0;

            __itemsData = new ObjectCollection(this);
            self.Realized += Self_Realized;
            self.Changed += Self_Changed;
        }

        private void Self_Changed(object sender, EventArgs e)
        {
            if (self.IsVisible)
            {
                ((EventHandler)Events["SelectedIndexChanged"])?.Invoke(this, e);
                ((EventHandler)Events["SelectedValueChanged"])?.Invoke(this, e);
                ((EventHandler)Events["SelectedItemChanged"])?.Invoke(this, e);
            }
        }

        private void Self_Realized(object sender, EventArgs e)
        {
            OnSetDataSource();
            var ws = ((Gtk.Box)self.Children[0].Parent).Children[1] as Gtk.ToggleButton;
            ws.Toggled += Ws_Toggled;
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                self.Entry.IsEditable = false;
                self.Entry.CanFocus = false;
                self.Entry.NoShowAll = true;
                self.Entry.WidthRequest = 1;
                ws.WidthRequest = self.WidthRequest;
                ws.Label = self.Entry.Text;
                ws.Image = Gtk.Image.NewFromIconName("pan-down", Gtk.IconSize.Button);
                ws.ImagePosition = Gtk.PositionType.Right;
                ws.AlwaysShowImage = true;
                ws.Valign = Align.Center;
                ws.Yalign = 0.5f;
                ws.Xalign = 0.95f;
                ws.Hexpand = true;
                ws.Image.Halign = Gtk.Align.End;
                ws.Image.Valign = Gtk.Align.Center;
                ws.Drawn += Ws_Drawn;
            }
        }

        public event EventHandler DropDown;
        private void Ws_Toggled(object sender, EventArgs e)
        {
            if (DropDown != null)
            {
                DropDown(this, e);
            }
        }
        private void Ws_Drawn(object o, Gtk.DrawnArgs args)
        {
            self.Entry.Visible = false;
            Gtk.ToggleButton ws = (Gtk.ToggleButton)o;
            ws.WidthRequest = -1;
            Pango.Context pangocontext = ws.PangoContext;
            string family = pangocontext.FontDescription.Family;
            args.Cr.SelectFontFace(family, Cairo.FontSlant.Normal, Cairo.FontWeight.Normal);
            if (this.ForeColor.Name == "0")
            {
                Gdk.RGBA color = ws.StyleContext.GetColor(StateFlags.Normal);
                args.Cr.SetSourceRGBA(color.Red, color.Green, color.Blue, color.Alpha);
            }
            else
            {
                args.Cr.SetSourceRGBA(this.ForeColor.R / 255, this.ForeColor.G / 255, this.ForeColor.B / 255, this.ForeColor.A / 255);
            }

            double fontsize = pangocontext.FontDescription.Size / Pango.Scale.PangoScale * 1.5;
            args.Cr.Save();
            args.Cr.Translate(10, (ws.AllocatedHeight - fontsize) / 2 + fontsize - 2);
            args.Cr.SetFontSize(fontsize);
            Cairo.TextExtents ext = args.Cr.TextExtents(self.Entry.Text);
            string text = self.Entry.Text;
            while (text.Length > 1 && args.Cr.TextExtents(text).Width > ws.AllocatedWidth - 40)
                text = text.Substring(0, text.Length - 1);

            args.Cr.ShowText(text);
            args.Cr.Restore();
        }


        private ComboBoxStyle _DropDownStyle;
        /// <summary>
        /// Gets or sets a value specifying the style of the combo box.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ComboBoxStyle" /> values. The default is DropDown.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The assigned value is not one of the
        /// <see cref="System.Windows.Forms.ComboBoxStyle" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public ComboBoxStyle DropDownStyle
        {
            get => _DropDownStyle;
            set
            {
                _DropDownStyle = value;
                if (value == ComboBoxStyle.DropDown)
                {
                    self.StyleContext.RemoveClass("DropDownList");
                }
                else if (value == ComboBoxStyle.DropDownList)
                {
                    self.StyleContext.AddClass("DropDownList");
                    self.Entry.IsEditable = false;
                    self.Entry.CanFocus = false;
                }
            }
        }
        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <returns>The text associated with this control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override string Text { get => self.Entry.Text; set { self.Entry.Text = value; } }
        /// <summary>
        /// Gets or sets currently selected item in the
        /// <see cref="System.Windows.Forms.ComboBox" />
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
        public object SelectedItem
        {
            get { return SelectedIndex == -1 ? null : __itemsData[SelectedIndex]; }
            set { int _index = __itemsData.IndexOf(value); if (_index != -1) { SelectedIndex = _index; } }
        }

        internal int _selectedIndex;
        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        /// <returns>A zero-based index of the currently selected item. A value of negative one (-1) is returned if no item is selected.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified index is less than or equal to -2.-or- The specified index is greater than or equal to the number of items in the combo box.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override int SelectedIndex { get { return self.Active; } set { self.Active = value; _selectedIndex = value; if (value == -1) { Text = ""; } } }

        public override object SelectedValue { get { return self.ActiveId; } set => self.ActiveId = value?.ToString(); }
        /// <summary>
        /// Gets an object representing the collection of the items contained in this
        /// <see cref="System.Windows.Forms.ComboBox" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ComboBox.ObjectCollection" /> representing the items in the
        /// <see cref="System.Windows.Forms.ComboBox" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ObjectCollection Items { get { return __itemsData; } }
        public override string GetItemText(object item)
        {
            if (item is ObjectCollection.Entry entry)
            {
                Type type = entry.Item.GetType();
                if (entry.Item is DataRow dr)
                    return dr[DisplayMember]?.ToString();
                else if (type.IsValueType && type.IsPrimitive)
                    return type.GetProperty(DisplayMember).GetValue(entry)?.ToString();
                else
                    return item?.ToString();
            }
            return item?.ToString();
        }
        public string NativeGetItemText(int index)
        {
            self.Model.GetIter(out TreeIter iter, new TreePath(new int[] { index }));
            object val = self.Model.GetValue(iter, 1);
            return val?.ToString();
        }
        public void NativeAdd(int index, string value, string text)
        {
            if (_sorted == false && index > -1)
            {
                self.Insert(index, value, text);
            }
            else
            {

                self.Append(value, text);
            }
        }

        private bool _sorted;
        /// <summary>
        /// Gets or sets a value indicating whether the items in the combo box are sorted.
        /// </summary>
        /// <returns>true if the combo box is sorted; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="System.ArgumentException">An attempt was made to sort a
        /// <see cref="System.Windows.Forms.ComboBox" /> that is attached to a data source.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Sorted { get => _sorted; set => _sorted = value; }

        public object _DataSource;
        /// <summary>
        /// Gets or sets the data source for this
        /// <see cref="System.Windows.Forms.ComboBox" />
        /// </summary>
        /// <returns>An object that implements the
        /// <see cref="System.Collections.IList" /> interface, such as a
        /// <see cref="System.Data.DataSet" /> or an
        /// <see cref="System.Array" /> The default is null.
        /// </returns>
        public override object DataSource
        {
            get => _DataSource;
            set
            {
                _DataSource = value;
                if (self.IsRealized)
                {
                    OnSetDataSource();
                }
            }
        }
        private void OnSetDataSource()
        {
            if (_DataSource != null)
            {
                if (_DataSource is DataTable dtable)
                {
                    LoadDataTableSource(dtable);
                }
                else if (_DataSource is DataView dview)
                {
                    LoadDataTableSource(dview.Table);
                }
                else if (_DataSource is IList list)
                {
                    LoadListSource(list);
                }
            }
        }
        private void LoadDataTableSource(DataTable dtable)
        {
            __itemsData.Clear();
            if (dtable.Columns.Contains(ValueMember) && dtable.Columns.Contains(DisplayMember))
            {
                foreach (DataRow row in dtable.Rows)
                    __itemsData.Add(row[ValueMember].ToString(), row[DisplayMember].ToString(), row);
            }
            else if (dtable.Columns.Contains(DisplayMember))
            {
                foreach (DataRow row in dtable.Rows)
                    __itemsData.Add("", row[DisplayMember].ToString(), row);
            }
            else
            {
                throw new Exception("DisplayMember the property is not assigned or the field name does not exist");
            }
        }
        private void LoadListSource(IList list)
        {
            __itemsData.Clear();
            if (list.Count > 0)
            {
                Type type = list[0].GetType();
                PropertyInfo valproperty = type.GetProperty(ValueMember);
                PropertyInfo disproperty = type.GetProperty(DisplayMember);
                foreach (var entry in list)
                    __itemsData.Add(valproperty?.GetValue(entry)?.ToString(), disproperty?.GetValue(entry)?.ToString(), entry);
            }
        }
    }

}

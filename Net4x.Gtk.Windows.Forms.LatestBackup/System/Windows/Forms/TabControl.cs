/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Manages a related set of tab pages.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class TabControl : ContainerControl
    {

        public readonly TabControlBase self = new TabControlBase();

        private TabControl.ControlCollection _controls;

        private TabControl.TabPageCollection _tabPageControls;

        public event DrawItemEventHandler DrawItem;

        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.TabControl" /> class.
        /// </summary>
        public TabControl() : base()
        {
            _controls = new ControlCollection(this);
            _tabPageControls = new TabPageCollection(this);
            self.SwitchPage += Self_SwitchPage;
        }

        /// <summary>
        /// Gets or sets the area of the control (for example, along the top) where the tabs are aligned.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TabAlignment" /> values. The default is Top.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property value is not a valid
        /// <see cref="System.Windows.Forms.TabAlignment" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TabAlignment Alignment
        {
            get
            {
                if (self.TabPos == PositionType.Left)
                    return TabAlignment.Left;
                else if (self.TabPos == PositionType.Top)
                    return TabAlignment.Top;
                else if (self.TabPos == PositionType.Right)
                    return TabAlignment.Right;
                else if (self.TabPos == PositionType.Bottom)
                    return TabAlignment.Bottom;
                else
                    return TabAlignment.Top;
            }
            set
            {
                if (value == TabAlignment.Left)
                    self.TabPos = PositionType.Left;
                else if (value == TabAlignment.Top)
                    self.TabPos = PositionType.Top;
                else if (value == TabAlignment.Right)
                    self.TabPos = PositionType.Right;
                else if (value == TabAlignment.Bottom)
                    self.TabPos = PositionType.Bottom;
            }
        }

        public new TabControl.ControlCollection Controls => _controls;

        /// <summary>
        /// Gets or sets the way that the control's tabs are drawn.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TabDrawMode" /> values. The default is Normal.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property value is not a valid
        /// <see cref="System.Windows.Forms.TabDrawMode" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TabDrawMode DrawMode { get; set; }

        /// <summary>
        /// gtk unique menu functionality， provided for those who need it use
        /// </summary>
        [Browsable(false)]
        public bool EnablePopup
        {
            set { self.EnablePopup = true; self.PopupEnable(); }
        }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the size of the control's tabs.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that represents the size of the tabs. The default automatically sizes the tabs to fit the icons and labels on the tabs.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The width or height of the
        /// <see cref="System.Drawing.Size" /> is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Size ItemSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether more than one row of tabs can be displayed.
        /// </summary>
        /// <returns>true if more than one row of tabs can be displayed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Multiline { get; set; }

        /// <summary>
        /// Gets or sets the index of the currently selected tab page.
        /// </summary>
        /// <returns>The zero-based index of the currently selected tab page. The default is -1, which is also the value if no tab page is selected.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is less than -1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int SelectedIndex { get { return self.CurrentPage; } set { self.CurrentPage = value; } }

        /// <summary>
        /// Gets or sets the currently selected tab page.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TabPage" /> that represents the selected tab page. If no tab page is selected, the value is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TabPage SelectedTab { get { return _controls[self.CurrentPage]; } set { } }

        public bool ShowTabs { get => self.ShowTabs; set => self.ShowTabs = value; }

        /// <summary>
        /// Gets or sets a value indicating whether a tab's ToolTip is shown when the mouse passes over the tab.
        /// </summary>
        /// <returns>true if ToolTips are shown for the tabs that have them; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ShowToolTips { get; set; }

        /// <summary>
        /// Gets or sets the way that the control's tabs are sized.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TabSizeMode" /> values. The default is Normal.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property value is not a valid
        /// <see cref="System.Windows.Forms.TabSizeMode" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public TabSizeMode SizeMode { get; set; }

        /// <summary>
        /// Gets the number of tabs in the tab strip.
        /// </summary>
        /// <returns>The number of tabs in the tab strip.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int TabCount { get => self.NPages; }

        /// <summary>
        /// Gets the collection of tab pages in this tab control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.TabControl.TabPageCollection" /> that contains the
        /// <see cref="System.Windows.Forms.TabPage" /> objects in this
        /// <see cref="System.Windows.Forms.TabControl" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public TabPageCollection TabPages { get { return _tabPageControls; } }

        /// <summary>
        /// Returns the bounding rectangle for a specified tab in this tab control.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the specified tab.
        /// </returns>
        /// <param name="index">The zero-based index of the tab you want.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">The index is less than zero.-or- The index is greater than or equal to
        /// <see cref="System.Windows.Forms.TabControl.TabPageCollection.Count" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Rectangle GetTabRect(int index)
        {
            if (index < 0 || (index >= TabCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "SR.InvalidArgument");
            }
            TabPage page = TabPages[index];
            Gtk.Label tab = page.TabLabel;
            Gdk.Rectangle rect = tab.Allocation;

            return new Rectangle(0, 0, rect.Width, rect.Height);
        }

        private void Self_SwitchPage(object o, SwitchPageArgs args)
        {
            if (SelectedIndexChanged != null && self.IsMapped)
                SelectedIndexChanged(this, new EventArgs());
        }

        /// <summary>
        /// Contains a collection of
        /// <see cref="System.Windows.Forms.Control" /> objects.
        /// </summary>
        public new class ControlCollection : List<TabPage>
        {
            TabControl _owner;

            public ControlCollection(TabControl owner)
            {
                _owner = owner;
                _owner.self.Shown += Self_Shown;
            }

            public new int Add(TabPage item)
            {
                try
                {
                    item.Parent = _owner;
                    item.TabLabel.Name = base.Count.ToString();
                    item._tabLabel.WidthRequest = _owner.ItemSize.Width;
                    item._tabLabel.HeightRequest = _owner.ItemSize.Height;
                    if (_owner.SizeMode == TabSizeMode.Fixed)
                    {
                        item._tabLabel.Ellipsize = Pango.EllipsizeMode.End;
                    }
                    else if (_owner.SizeMode == TabSizeMode.FillToRight)
                    {
                        item._tabLabel.Halign = Align.End;
                    }
                    base.Add(item);
                    item.TabLabel.Drawn += (object sender, DrawnArgs args) =>
                    {
                        if (_owner.DrawMode == TabDrawMode.OwnerDrawFixed && _owner.DrawItem != null)
                        {
                            Gtk.Label tab = (Gtk.Label)sender;
                            Gdk.Rectangle allocation = tab.Allocation;
                            args.Cr.ResetClip();
                            int width = allocation.Width + 24;
                            int height = allocation.Height + 2;
                            _owner.DrawItem(this, new DrawItemEventArgs(new Graphics(tab, args.Cr, new Gdk.Rectangle(0, 0, width, height)) { diff_left = -12, diff_top = -2 }, _owner.Font, new Rectangle(0, 0, width, height), Convert.ToInt32(tab.Name), DrawItemState.Default));

                        }
                    };
                    return _owner.self.AppendPage(item.self, item.TabLabel);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _owner.self.SetTabReorderable(item.self, true);
                    if (_owner.Widget.IsRealized)
                        item.Widget.ShowAll();
                }
            }

            public new void Remove(TabPage value)
            {
                base.Remove(value);
                _owner.self.Remove(((TabPage)value).Widget);
            }

            public new void RemoveAt(int index)
            {
                base.RemoveAt(index);
                _owner.self.RemovePage(index);
            }

            private void Self_Shown(object sender, EventArgs e)
            {
                foreach (TabPage item in this)
                {
                    _owner.self.SetMenuLabelText(item.self, _owner.self.GetTabLabelText(item.self));
                }
            }

        }

        /// <summary>
        /// Contains a collection of
        /// <see cref="System.Windows.Forms.TabPage" /> objects.
        /// </summary>
        public class TabPageCollection : IList, ICollection, IEnumerable
        {

            private TabControl _owner;

            public TabPageCollection(TabControl owner)
            {
                _owner = owner;
            }

            //[Browsable(false)]
            public int Count { get { return _owner.Controls.Count; } }

            public bool IsReadOnly { get { return false; } }

            bool IList.IsFixedSize => throw new NotImplementedException();

            bool ICollection.IsSynchronized => throw new NotImplementedException();

            object ICollection.SyncRoot => throw new NotImplementedException();

            public virtual TabPage this[string key] { get { return _owner.Controls.Find(p => p.Name == key); } }

            public virtual TabPage this[int index] { get { return _owner.Controls[index]; } set { Add(value); } }

            object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void Add(string key, string text, string imageKey)
            {
                TabPage tp = new TabPage();
                tp.Name = key;
                tp.Text = text;
                this.Add(tp);
            }

            public void Add(TabPage value)
            {
                value.Parent = _owner;
                _owner.Controls.Add(value);
            }

            public void Add(string key, string text)
            {
                this.Add(key, text, null);
            }

            public void Add(string text)
            {
                this.Add($"tabPage{Count}", text, null);
            }

            public void Add(string key, string text, int imageIndex)
            {
                this.Add(key, text, null);
            }

            public void AddRange(TabPage[] pages)
            {
                foreach (TabPage page in pages)
                    this.Add(page);
            }

            public virtual void Clear()
            {
                _owner.Controls.Clear();
            }

            public bool Contains(TabPage page)
            {
                return _owner.Controls.Contains(page);
            }

            public virtual bool ContainsKey(string key)
            {
                return _owner.Controls.FindIndex(p => p.Name == key) > -1;

            }

            public IEnumerator GetEnumerator()
            {
                return _owner.Controls.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                throw new NotImplementedException();
            }

            public int IndexOf(TabPage page)
            {
                return _owner.Controls.IndexOf(page);
            }

            public virtual int IndexOfKey(string key)
            {
                return _owner.Controls.FindIndex(p => p.Name == key);
            }

            public void Insert(int index, string key, string text, int imageIndex)
            {
                _owner.Controls.Insert(index, new TabPage() { Name = key, Text = text }); ;
            }

            public void Insert(int index, string key, string text)
            {
                Insert(index, key, text, -1);
            }

            public void Insert(int index, TabPage tabPage)
            {
                _owner.Controls.Insert(index, tabPage);
            }

            public void Insert(int index, string key, string text, string imageKey)
            {
                Insert(index, key, text, -1);
            }

            public void Insert(int index, string text)
            {
                Insert(index, $"tabPage{Count}", text, -1);
            }

            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public void Remove(TabPage value)
            {
                _owner.Controls.Remove(value);
            }

            public void RemoveAt(int index)
            {
                _owner.Controls.RemoveAt(index);
            }

            public virtual void RemoveByKey(string key)
            {
                _owner.Controls.RemoveAt(_owner.Controls.FindIndex(p => p.Name == key));
            }

            int IList.Add(object value)
            {
                throw new NotImplementedException();
            }

            bool IList.Contains(object value)
            {
                throw new NotImplementedException();
            }

            void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            void IList.Remove(object value)
            {
                throw new NotImplementedException();
            }

        }
    }
}

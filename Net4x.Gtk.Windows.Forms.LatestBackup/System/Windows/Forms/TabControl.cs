/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Manages a related set of tab pages.
/// </summary>
[DesignerCategory("Component")]
public partial class TabControl : ContainerControl
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly TabControlBase self = new TabControlBase();

    private readonly TabControl.ControlCollection _controls;

    private readonly TabControl.TabPageCollection _tabPageControls;

    /// <summary>
    /// Occurs when draw item.
    /// </summary>
    public virtual event DrawItemEventHandler DrawItem;

    /// <summary>
    /// Occurs when selected index changed.
    /// </summary>
    public virtual event EventHandler SelectedIndexChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.TabControl" /> class.
    /// </summary>
    public TabControl() : base()
    {
        _controls = new ControlCollection(this);
        _tabPageControls = new TabPageCollection(this);
        self.SwitchPage += Self_SwitchPage;
    }

    /// <summary>
    /// Gets or sets the alignment.
    /// </summary>
    /// <value>
    /// The alignment.
    /// </value>
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

    /// <summary>
    /// Gets the controls.
    /// </summary>
    /// <value>
    /// The controls.
    /// </value>
    public new TabControl.ControlCollection Controls => _controls;

    /// <summary>
    /// Gets or sets the draw mode.
    /// </summary>
    /// <value>
    /// The draw mode.
    /// </value>
    public TabDrawMode DrawMode { get; set; }

    /// <summary>
    /// gtk unique menu functionality， provided for those who need it use
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if enable popup; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [Browsable(false)]
    public bool EnablePopup
    {
        set { self.EnablePopup = true; self.PopupEnable(); }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the size of the item.
    /// </summary>
    /// <value>
    /// The size of the item.
    /// </value>
    public Size ItemSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.TabControl" /> is multiline.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if multiline; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Multiline { get; set; }

    /// <summary>
    /// Gets or sets the index of the selected.
    /// </summary>
    /// <value>
    /// The index of the selected.
    /// </value>
    public int SelectedIndex { get { return self.CurrentPage; } set { self.CurrentPage = value; } }

    /// <summary>
    /// Gets or sets the selected tab.
    /// </summary>
    /// <value>
    /// The selected tab.
    /// </value>
    public TabPage SelectedTab { get { return _controls[self.CurrentPage]; } set { } }

    /// <summary>
    /// Gets or sets a value indicating whether show tabs.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show tabs; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowTabs { get => self.ShowTabs; set => self.ShowTabs = value; }

    /// <summary>
    /// Gets or sets a value indicating whether show tool tips.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show tool tips; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowToolTips { get; set; }

    /// <summary>
    /// Gets or sets the size mode.
    /// </summary>
    /// <value>
    /// The size mode.
    /// </value>
    public TabSizeMode SizeMode { get; set; }

    /// <summary>
    /// Gets the tab count.
    /// </summary>
    /// <value>
    /// The tab count.
    /// </value>
    public int TabCount { get => self.NPages; }

    /// <summary>
    /// Gets the tab pages.
    /// </summary>
    /// <value>
    /// The tab pages.
    /// </value>
    public TabPageCollection TabPages { get { return _tabPageControls; } }

    /// <summary>
    /// Gets the tab rect.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <returns>
    /// Rectangle.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// index - SR.InvalidArgument
    /// </exception>
    public Rectangle GetTabRect(int index)
    {
        if (index < 0 || (index >= TabCount))
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "SR.InvalidArgument");
        }
        var page = TabPages[index];
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
    /// Class ControlCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.TabPage&gt;
    /// </summary>
    public new class ControlCollection : List<TabPage>
    {
        readonly TabControl _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.TabControl.ControlCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ControlCollection(TabControl owner)
        {
            _owner = owner;
            _owner.self.Shown += Self_Shown;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
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
                        var tab = (Gtk.Label)sender;
                        Gdk.Rectangle allocation= tab.Allocation;
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

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public new void Remove(TabPage value)
        {
            base.Remove(value);
            _owner.self.Remove(((TabPage)value).Widget);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
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
    /// Class TabPageCollection. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// Implements the
    /// <see cref="T:System.Collections.ICollection" /> Implements the
    /// <see cref="T:System.Collections.IEnumerable" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    /// <seealso cref="T:System.Collections.ICollection" />
    /// <seealso cref="T:System.Collections.IEnumerable" />
    public class TabPageCollection : IList, ICollection, IEnumerable
    {

        private readonly TabControl _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.TabControl.TabPageCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public TabPageCollection(TabControl owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        //[Browsable(false)]
        public int Count { get { return _owner.Controls.Count; } }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> is read-only.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is read only; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        public bool IsReadOnly { get { return false; } }

        bool IList.IsFixedSize => throw new NotImplementedException();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.TabPage" /> with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// TabPage.
        /// </returns>
        public virtual TabPage this[string key] { get { return _owner.Controls.Find(p => p.Name == key); } }

        /// <summary>
        /// Gets or sets the
        /// <see cref="T:System.Windows.Forms.TabPage" /> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// TabPage.
        /// </returns>
        public virtual TabPage this[int index] { get { return _owner.Controls[index]; } set { Add(value); } }

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        public void Add(string key, string text, string imageKey)
        {
            var tp = new TabPage
            {
                Name = key,
                Text = text
            };
            this.Add(tp);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(TabPage value)
        {
            value.Parent = _owner;
            _owner.Controls.Add(value);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Add(string key, string text)
        {
            this.Add(key, text, null);
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Add(string text)
        {
            this.Add($"tabPage{Count}", text, null);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        public void Add(string key, string text, int imageIndex)
        {
            this.Add(key, text, null);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="pages">
        /// The pages.
        /// </param>
        public void AddRange(TabPage[] pages)
        {
            foreach (TabPage page in pages)
                this.Add(page);
        }

        /// <summary>
        /// Removes all items from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        public virtual void Clear()
        {
            _owner.Controls.Clear();
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if contains [the specified page]; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public bool Contains(TabPage page)
        {
            return _owner.Controls.Contains(page);
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// <c>
        /// true
        /// </c> if the specified key contains key; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </returns>
        public virtual bool ContainsKey(string key)
        {
            return _owner.Controls.FindIndex(p => p.Name == key) > -1;

        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _owner.Controls.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        /// <param name="value">
        /// The object to locate in the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <returns>
        /// The index of
        /// <paramref name="value">
        /// value
        /// </paramref> if found in the list; otherwise, -1.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public int IndexOf(TabPage page)
        {
            return _owner.Controls.IndexOf(page);
        }

        /// <summary>
        /// Indexes the of key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// System.Int32.
        /// </returns>
        public virtual int IndexOfKey(string key)
        {
            return _owner.Controls.FindIndex(p => p.Name == key);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageIndex">
        /// Index of the image.
        /// </param>
        public void Insert(int index, string key, string text, int imageIndex)
        {
            _owner.Controls.Insert(index, new TabPage() { Name = key, Text = text }); ;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Insert(int index, string key, string text)
        {
            Insert(index, key, text, -1);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="tabPage">
        /// The tab page.
        /// </param>
        public void Insert(int index, TabPage tabPage)
        {
            _owner.Controls.Insert(index, tabPage);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="imageKey">
        /// The image key.
        /// </param>
        public void Insert(int index, string key, string text, string imageKey)
        {
            Insert(index, key, text, -1);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Insert(int index, string text)
        {
            Insert(index, $"tabPage{Count}", text, -1);
        }

        /// <summary>
        /// Inserts an item to the
        /// <see cref="T:System.Collections.IList">
        /// </see> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="value">
        /// The object to insert into the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Remove(TabPage value)
        {
            _owner.Controls.Remove(value);
        }

        /// <summary>
        /// Removes the
        /// <see cref="T:System.Collections.IList">
        /// </see> item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            _owner.Controls.RemoveAt(index);
        }

        /// <summary>
        /// Removes the by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
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
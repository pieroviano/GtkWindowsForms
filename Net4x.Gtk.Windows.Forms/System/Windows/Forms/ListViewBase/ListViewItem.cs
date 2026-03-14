using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
namespace System.Windows.Forms;

/// <summary>
/// Represents an item in a
/// <see cref="T:System.Windows.Forms.ListView" /> control.
/// </summary>
public class ListViewItem : ICloneable, ISerializable, IKeyboardToolTip
{
    /// <summary>
    /// The checked
    /// </summary>
    internal bool _checked;

    /// <summary>
    /// The flow box child
    /// </summary>
    internal Gtk.FlowBoxChild _flowBoxChild;

    /// <summary>
    /// The group
    /// </summary>
    internal ListViewGroup _group;

    /// <summary>
    /// The list view
    /// </summary>
    internal ListView _listView;

    /// <summary>
    /// The selected
    /// </summary>
    internal bool _selected;

    /// <summary>
    /// The text
    /// </summary>
    internal string _text = string.Empty;

    /// <summary>
    /// The identifier
    /// </summary>
    internal int ID;

    /// <summary>
    /// The subitems
    /// </summary>
    private ListViewSubItemCollection _subitems;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    public ListViewItem()
    {
        InitListViewItem("", -1, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public ListViewItem(string text)
    {
        InitListViewItem(text, -1, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    public ListViewItem(string text, int imageIndex)
    {
        InitListViewItem(text, imageIndex, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    public ListViewItem(string[] items)
    {
        InitListViewItem(items, -1, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    public ListViewItem(string[] items, int imageIndex)
    {
        InitListViewItem(items, imageIndex, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font)
    {
        InitListViewItem(items, imageIndex, "", foreColor, backColor, font, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="subItems">
    /// The sub items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    public ListViewItem(ListViewSubItem[] subItems, int imageIndex)
    {
        InitListViewItem(subItems, imageIndex, "", null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(ListViewGroup group)
    {
        InitListViewItem("", -1, "", null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string text, ListViewGroup group)
    {
        InitListViewItem(text, -1, null, null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string text, int imageIndex, ListViewGroup group)
    {
        InitListViewItem(text, imageIndex, null, null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string[] items, ListViewGroup group)
    {
        InitListViewItem(items, -1, "", null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string[] items, int imageIndex, ListViewGroup group)
    {
        InitListViewItem(items, imageIndex, "", null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group)
    {
        InitListViewItem(items, imageIndex, "", foreColor, backColor, font, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="subItems">
    /// The sub items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group)
    {
        InitListViewItem(subItems, imageIndex, "", null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    public ListViewItem(string text, string imageKey)
    {
        InitListViewItem(text, -1, imageKey, null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    public ListViewItem(string[] items, string imageKey)
    {
        InitListViewItem(items, -1, imageKey, null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font)
    {
        InitListViewItem(items, -1, imageKey, foreColor, backColor, font, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="subItems">
    /// The sub items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    public ListViewItem(ListViewSubItem[] subItems, string imageKey)
    {
        InitListViewItem(subItems, -1, imageKey, null, null, null, null);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string text, string imageKey, ListViewGroup group)
    {
        InitListViewItem(text, -1, imageKey, null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string[] items, string imageKey, ListViewGroup group)
    {
        InitListViewItem(items, -1, imageKey, null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group)
    {
        InitListViewItem(items, -1, imageKey, foreColor, backColor, font, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="subItems">
    /// The sub items.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group)
    {
        InitListViewItem(subItems, -1, imageKey, null, null, null, group);
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> class.
    /// </summary>
    /// <param name="info">
    /// The information.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    protected ListViewItem(SerializationInfo info, StreamingContext context)
    {

    }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public Color? BackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the bounds.
    /// </summary>
    /// <value>
    /// The bounds.
    /// </value>
    public Rectangle Bounds
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> is checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Checked
    {
        get => _checked;

        set
        {
            _checked = value;
            _listView?.NativeCheckItem(this, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> is focused.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if focused; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Focused
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    public Font Font
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the fore.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    public Color? ForeColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    /// <value>
    /// The group.
    /// </value>
    public ListViewGroup Group
    {
        get { return _group; }
        set { _group = value; }
    }

    /// <summary>
    /// Gets or sets the index of the image.
    /// </summary>
    /// <value>
    /// The index of the image.
    /// </value>
    public int ImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image key.
    /// </summary>
    /// <value>
    /// The image key.
    /// </value>
    public string ImageKey
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the image list.
    /// </summary>
    /// <value>
    /// The image list.
    /// </value>
    public ImageList ImageList
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets the indent count.
    /// </summary>
    /// <value>
    /// The indent count.
    /// </value>
    public int IndentCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the ListView.
    /// </summary>
    /// <value>
    /// The ListView.
    /// </value>
    public ListView ListView
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Point Position
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.ListViewItem" /> is selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            _listView?.NativeSelectItem(this, value);
        }
    }

    /// <summary>
    /// Gets or sets the index of the state image.
    /// </summary>
    /// <value>
    /// The index of the state image.
    /// </value>
    public int StateImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the sub items.
    /// </summary>
    /// <value>
    /// The sub items.
    /// </value>
    public ListViewSubItemCollection SubItems
    {
        get => _subitems;
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    public object Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _listView?.NativeUpdateText(this, value);

        }
    }

    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    /// <value>
    /// The tool tip text.
    /// </value>
    public string ToolTipText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use item style for sub items.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use item style for sub items; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool UseItemStyleForSubItems
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the accessibility object.
    /// </summary>
    /// <value>
    /// The accessibility object.
    /// </value>
    internal virtual AccessibleObject AccessibilityObject
    {
        get;
    }

    /// <summary>
    /// Gets the image indexer.
    /// </summary>
    /// <value>
    /// The image indexer.
    /// </value>
    internal ListViewItemImageIndexer ImageIndexer
    {
        get;
    }

    /// <summary>
    /// Gets or sets the type of the item.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    internal string ItemType
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether state image set.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if state image set; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    internal bool StateImageSet
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether state selected.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if state selected; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    internal bool StateSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Begins the edit.
    /// </summary>
    //internal string RelateFlowBoxChildKey { get; set; }
    public void BeginEdit()
    {

    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public virtual object Clone()
    {
        return null;
    }

    /// <summary>
    /// Ensures the visible.
    /// </summary>
    public virtual void EnsureVisible()
    {

    }

    /// <summary>
    /// Populates a
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">
    /// The
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> to populate with data.
    /// </param>
    /// <param name="context">
    /// The destination (see
    /// <see cref="T:System.Runtime.Serialization.StreamingContext">
    /// </see>) for this serialization.
    /// </param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

    /// <summary>
    /// Gets the sub item at.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// ListViewSubItem.
    /// </returns>
    public ListViewSubItem GetSubItemAt(int x, int y)
    {
        if (_listView is not null && _listView.IsHandleCreated && _listView.View == View.Details)
        {
            _listView.GetSubItemAt(x, y, out int iItem, out int iSubItem);
            if (Index > -1 && iSubItem > -1 && iSubItem < SubItems.Count)
            {
                return SubItems[iSubItem];
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    public virtual void Remove()
    {

    }

    /// <summary>
    /// Hosts the specified parent.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    /// <param name="id">
    /// The identifier.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    internal void Host(ListView parent, int id, int index)
    {

    }

    /// <summary>
    /// Initializes the ListView item.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    internal void InitListViewItem(string text, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
    {
        _subitems = new ListViewSubItemCollection(this)
        {
            text
        };
        this.Text = text;
        this.ImageIndex = imageIndex;
        this.ImageKey = imageKey;
        this.ForeColor = foreColor;
        this.BackColor = backColor;
        this.Font = font;
        this.Group = group;
    }

    /// <summary>
    /// Initializes the ListView item.
    /// </summary>
    /// <param name="items">
    /// The items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    internal void InitListViewItem(string[] items, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
    {
        InitListViewItem(items.Length > 0 ? items[0] : "", imageIndex, imageKey, foreColor, backColor, font, group);
        foreach (string item in items)
            _subitems.Add(item);

    }

    /// <summary>
    /// Initializes the ListView item.
    /// </summary>
    /// <param name="subItems">
    /// The sub items.
    /// </param>
    /// <param name="imageIndex">
    /// Index of the image.
    /// </param>
    /// <param name="imageKey">
    /// The image key.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="group">
    /// The group.
    /// </param>
    internal void InitListViewItem(ListViewSubItem[] subItems, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
    {
        InitListViewItem(subItems.Length > 0 ? subItems[0].Text : "", imageIndex, imageKey, foreColor, backColor, font, group);
        foreach (ListViewSubItem item in subItems)
            _subitems.Add(item);
    }

    /// <summary>
    /// Class ListViewSubItem.
    /// </summary>
    public class ListViewSubItem
    {
        /// <summary>
        /// The label
        /// </summary>
        internal Gtk.Label _label;

        /// <summary>
        /// The text
        /// </summary>
        internal string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> class.
        /// </summary>
        public ListViewSubItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public ListViewSubItem(ListViewItem owner, string text)
        {
            ListViewItem = owner;
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="foreColor">
        /// Color of the fore.
        /// </param>
        /// <param name="backColor">
        /// Color of the back.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
        {
            ListViewItem = owner;
            this.Text = text;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
        }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public Color? BackColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Rectangle Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public Font Font
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>
        /// The color of the fore.
        /// </value>
        public Color? ForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ListView item.
        /// </summary>
        /// <value>
        /// The ListView item.
        /// </value>
        public ListViewItem ListViewItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _label?.Text = value;
            }
        }

        /// <summary>
        /// Resets the style.
        /// </summary>
        public void ResetStyle()
        {

        }

    }

    /// <summary>
    /// Class ListViewSubItemCollection.
    /// Implements the System.Collections.Generic.List&lt;System.Windows.Forms.ListViewItem.ListViewSubItem&gt;
    /// </summary>
    public class ListViewSubItemCollection : List<ListViewSubItem>
    {
        /// <summary>
        /// The owner
        /// </summary>
        private readonly ListViewItem _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItemCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ListViewSubItemCollection(ListViewItem owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the
        /// <see cref="T:System.Windows.Forms.ListViewItem.ListViewSubItem" /> with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// ListViewSubItem.
        /// </returns>
        public virtual ListViewSubItem this[string key]
        {
            get
            {
                return base.Find(w => w.Name == key);
            }
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// ListViewSubItem.
        /// </returns>
        public ListViewSubItem Add(string text)
        {
            ListViewSubItem sub = new ListViewSubItem(_owner, text);
            Add(sub);

            return sub;
        }

        /// <summary>
        /// Adds the specified text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="foreColor">
        /// Color of the fore.
        /// </param>
        /// <param name="backColor">
        /// Color of the back.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        /// <returns>
        /// ListViewSubItem.
        /// </returns>
        public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
        {
            ListViewSubItem sub = new ListViewSubItem(_owner, text, foreColor, backColor, font);
            Add(sub);

            return sub;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(ListViewSubItem[] items)
        {
            foreach (ListViewSubItem item in items)
                Add(item);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(string[] items)
        {
            foreach (string item in items)
                Add(item);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="foreColor">
        /// Color of the fore.
        /// </param>
        /// <param name="backColor">
        /// Color of the back.
        /// </param>
        /// <param name="font">
        /// The font.
        /// </param>
        public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
        {
            foreach (string item in items)
                Add(item, foreColor, backColor, font);
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
            return base.FindIndex(w => w.Name == key) > -1;

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
            return base.FindIndex(w => w.Name == key);

        }

        /// <summary>
        /// Removes the by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public virtual void RemoveByKey(string key)
        {
            RemoveAt(base.FindIndex(w => w.Name == key));
        }

    }
}
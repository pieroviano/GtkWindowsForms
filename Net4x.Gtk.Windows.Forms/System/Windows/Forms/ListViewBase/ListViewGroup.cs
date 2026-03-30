using System.Runtime.Serialization;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a group of items displayed within a
/// <see cref="T:System.Windows.Forms.ListView" /> control.
/// </summary>
public sealed class ListViewGroup : ISerializable
{
    /// <summary>
    /// The default ListView group key
    /// </summary>
    public static readonly string defaultListViewGroupKey = "_DefaultListViewGroup_";

    /// <summary>
    /// The serial unique identifier
    /// </summary>
    public readonly string SerialGuid = Guid.NewGuid().ToString();

    internal readonly Gtk.FlowBox FlowBox = new() { Orientation = Gtk.Orientation.Horizontal, Name = Guid.NewGuid().ToString() };

    readonly ListView.ListViewItemCollection? _items;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewGroup" /> class.
    /// </summary>
    public ListViewGroup() : this("", "")
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewGroup" /> class.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="headerText">
    /// The header text.
    /// </param>
    public ListViewGroup(string key, string headerText)
    {
        this.Name = string.IsNullOrWhiteSpace(key) ? headerText : key;
        this.Header = headerText;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewGroup" /> class.
    /// </summary>
    /// <param name="header">
    /// The header.
    /// </param>
    public ListViewGroup(string header) : this(header, header)
    {

    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewGroup" /> class.
    /// </summary>
    /// <param name="header">
    /// The header.
    /// </param>
    /// <param name="headerAlignment">
    /// The header alignment.
    /// </param>
    public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header, header)
    {
        this.HeaderAlignment = headerAlignment;

    }

    /// <summary>
    /// Gets or sets the state of the collapsed.
    /// </summary>
    /// <value>
    /// The state of the collapsed.
    /// </value>
    public ListViewGroupCollapsedState CollapsedState
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the footer.
    /// </summary>
    /// <value>
    /// The footer.
    /// </value>
    public string? Footer
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the footer alignment.
    /// </summary>
    /// <value>
    /// The footer alignment.
    /// </value>
    public HorizontalAlignment FooterAlignment
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header.
    /// </summary>
    /// <value>
    /// The header.
    /// </value>
    public string Header
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header alignment.
    /// </summary>
    /// <value>
    /// The header alignment.
    /// </value>
    public HorizontalAlignment HeaderAlignment
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public ListView.ListViewItemCollection Items
    {
        get
        {
            var coll = new ListView.ListViewItemCollection(ListView);
            var all = ListView?.Items.FindAll(m => m.Group?.SerialGuid == this.SerialGuid);
            coll.AddRange(all ?? []);

            return coll;
        }
    }

    /// <summary>
    /// Gets the ListView.
    /// </summary>
    /// <value>
    /// The ListView.
    /// </value>
    public ListView? ListView
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
    /// Gets or sets the subtitle.
    /// </summary>
    /// <value>
    /// The subtitle.
    /// </value>
    public string? Subtitle
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
    public object? Tag
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the task link.
    /// </summary>
    /// <value>
    /// The task link.
    /// </value>
    public string? TaskLink
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index of the title image.
    /// </summary>
    /// <value>
    /// The index of the title image.
    /// </value>
    public int TitleImageIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the title image key.
    /// </summary>
    /// <value>
    /// The title image key.
    /// </value>
    public string? TitleImageKey
    {
        get;
        set;
    }

    internal Box? _groupbox
    {
        get;
        set;
    }

    /// <summary>
    /// Creates the default ListView group.
    /// </summary>
    /// <returns>
    /// ListViewGroup.
    /// </returns>
    public static ListViewGroup CreateDefaultListViewGroup()
    {
        var _defaultGroup = new ListViewGroup("default", HorizontalAlignment.Left)
        {
            Header = "default",
            Name = ListViewGroup.defaultListViewGroupKey,
            Subtitle = ""
        };

        return _defaultGroup;
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }
}
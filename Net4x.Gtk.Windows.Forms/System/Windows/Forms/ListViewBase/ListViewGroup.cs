using Gtk;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a group of items displayed within a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public sealed class ListViewGroup : ISerializable
    {
        public static readonly string defaultListViewGroupKey = "_DefaultListViewGroup_";

        public readonly string SerialGuid = Guid.NewGuid().ToString();

        internal readonly Gtk.FlowBox FlowBox = new Gtk.FlowBox() { Orientation = Gtk.Orientation.Horizontal, Name = Guid.NewGuid().ToString() };

        ListView.ListViewItemCollection _items;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewGroup" /> class using the default header text of "ListViewGroup" and the default left header alignment.
        /// </summary>
        public ListViewGroup() : this("", "")
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewGroup" /> class using the specified values to initialize the
        /// <see cref="System.Windows.Forms.ListViewGroup.Name" /> and
        /// <see cref="System.Windows.Forms.ListViewGroup.Header" /> properties.
        /// </summary>
        /// <param name="key">The initial value of the
        /// <see cref="System.Windows.Forms.ListViewGroup.Name" /> property.
        /// </param>
        /// <param name="headerText">The initial value of the
        /// <see cref="System.Windows.Forms.ListViewGroup.Header" /> property.
        /// </param>
        public ListViewGroup(string key, string headerText)
        {
            this.Name = string.IsNullOrWhiteSpace(key) ? headerText : key;
            this.Header = headerText;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewGroup" /> class using the specified value to initialize the
        /// <see cref="System.Windows.Forms.ListViewGroup.Header" /> property and using the default left header alignment.
        /// </summary>
        /// <param name="header">The text to display for the group header.
        /// </param>
        public ListViewGroup(string header) : this(header, header)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewGroup" /> class using the specified header text and the specified header alignment.
        /// </summary>
        /// <param name="header">The text to display for the group header.
        /// </param>
        /// <param name="headerAlignment">One of the
        /// <see cref="System.Windows.Forms.HorizontalAlignment" /> values that specifies the alignment of the header text.
        /// </param>
        public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header, header)
        {
            this.HeaderAlignment = headerAlignment;

        }


        public ListViewGroupCollapsedState CollapsedState
        {
            get;
            set;
        }

        public string Footer
        {
            get;
            set;
        }


        public HorizontalAlignment FooterAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the header text for the group.
        /// </summary>
        /// <returns>The text to display for the group header. The default is "ListViewGroup".
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the alignment of the group header text.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.HorizontalAlignment" /> values that specifies the alignment of the header text. The default is
        /// <see cref="System.Windows.Forms.HorizontalAlignment.Left" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
        /// <see cref="System.Windows.Forms.HorizontalAlignment" /> value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public HorizontalAlignment HeaderAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection containing all items associated with this group.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView.ListViewItemCollection" /> that contains all the items in the group. If there are no items in the group, an empty
        /// <see cref="System.Windows.Forms.ListView.ListViewItemCollection" /> object is returned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListView.ListViewItemCollection Items
        {
            get
            {
                ListView.ListViewItemCollection coll = new ListView.ListViewItemCollection(ListView);
                var all = ListView.Items.FindAll(m => m.Group.SerialGuid == this.SerialGuid);
                coll.AddRange(all);

                return coll;
            }
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ListView" /> control that contains this group.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListView" /> control that contains this group.
        /// </returns>
        public ListView ListView
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <returns>The name of the group.
        /// </returns>
        public string Name
        {
            get;
            set;
        }


        public string Subtitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the object that contains data about the group.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> for storing the additional data.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Tag
        {
            get;
            set;
        }


        public string TaskLink
        {
            get;
            set;
        }

        public int TitleImageIndex
        {
            get;
            set;
        }

        public string TitleImageKey
        {
            get;
            set;
        }

        internal Gtk.Box _groupbox { get; set; }

        public static ListViewGroup CreateDefaultListViewGroup()
        {
            ListViewGroup _defaultGroup = new ListViewGroup("default", HorizontalAlignment.Left);
            _defaultGroup.Header = "default";
            _defaultGroup.Name = ListViewGroup.defaultListViewGroupKey;
            _defaultGroup.Subtitle = "";

            return _defaultGroup;
        }

        /// <summary>
        /// Populates a
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> to populate with data.
        /// </param>
        /// <param name="context">The destination (see
        /// <see cref="System.Runtime.Serialization.StreamingContext" />) for this serialization.
        /// </param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

    }
}

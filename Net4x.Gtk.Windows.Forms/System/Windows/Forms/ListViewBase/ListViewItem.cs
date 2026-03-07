using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
namespace System.Windows.Forms
{
    /// <summary>
    /// Represents an item in a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ListViewItem : ICloneable, ISerializable, IKeyboardToolTip
    {



        internal bool _checked;

        internal Gtk.FlowBoxChild _flowBoxChild;

        internal ListViewGroup _group;

        internal ListView _listView;

        internal bool _selected;

        internal string _text = string.Empty;

        internal int ID;

        private ListViewSubItemCollection _subitems;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with default values.
        /// </summary>
        public ListViewItem()
        {
            InitListViewItem("", -1, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified item text.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        public ListViewItem(string text)
        {
            InitListViewItem(text, -1, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified item text and the image index position of the item's icon.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        public ListViewItem(string text, int imageIndex)
        {
            InitListViewItem(text, imageIndex, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with an array of strings representing subitems.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        public ListViewItem(string[] items)
        {
            InitListViewItem(items, -1, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon and an array of strings representing subitems.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        public ListViewItem(string[] items, int imageIndex)
        {
            InitListViewItem(items, imageIndex, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon; the foreground color, background color, and font of the item; and an array of strings representing subitems.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        /// <param name="foreColor">A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of the item.
        /// </param>
        /// <param name="backColor">A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item.
        /// </param>
        /// <param name="font">A
        /// <see cref="System.Drawing.Font" /> that represents the font to display the item's text in.
        /// </param>
        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font)
        {
            InitListViewItem(items, imageIndex, "", foreColor, backColor, font, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon and an array of
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> objects.
        /// </summary>
        /// <param name="subItems">An array of type
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> that represents the subitems of the item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        public ListViewItem(ListViewSubItem[] subItems, int imageIndex)
        {
            InitListViewItem(subItems, imageIndex, "", null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class and assigns it to the specified group.
        /// </summary>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(ListViewGroup group)
        {
            InitListViewItem("", -1, "", null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified item text and assigns it to the specified group.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string text, ListViewGroup group)
        {
            InitListViewItem(text, -1, null, null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified item text and the image index position of the item's icon, and assigns the item to the specified group.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string text, int imageIndex, ListViewGroup group)
        {
            InitListViewItem(text, imageIndex, null, null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with an array of strings representing subitems, and assigns the item to the specified group.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string[] items, ListViewGroup group)
        {
            InitListViewItem(items, -1, "", null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon and an array of strings representing subitems, and assigns the item to the specified group.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string[] items, int imageIndex, ListViewGroup group)
        {
            InitListViewItem(items, imageIndex, "", null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon; the foreground color, background color, and font of the item; and an array of strings representing subitems. Assigns the item to the specified group.
        /// </summary>
        /// <param name="items">An array of strings that represent the subitems of the new item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        /// <param name="foreColor">A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of the item.
        /// </param>
        /// <param name="backColor">A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item.
        /// </param>
        /// <param name="font">A
        /// <see cref="System.Drawing.Font" /> that represents the font to display the item's text in.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group)
        {
            InitListViewItem(items, imageIndex, "", foreColor, backColor, font, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the image index position of the item's icon and an array of
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> objects, and assigns the item to the specified group.
        /// </summary>
        /// <param name="subItems">An array of type
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> that represents the subitems of the item.
        /// </param>
        /// <param name="imageIndex">The zero-based index of the image within the
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ListView" /> that contains the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group)
        {
            InitListViewItem(subItems, imageIndex, "", null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified text and image.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        public ListViewItem(string text, string imageKey)
        {
            InitListViewItem(text, -1, imageKey, null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified item and subitem text and image.
        /// </summary>
        /// <param name="items">An array containing the text of the subitems of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        public ListViewItem(string[] items, string imageKey)
        {
            InitListViewItem(items, -1, imageKey, null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the subitems containing the specified text, image, colors, and font.
        /// </summary>
        /// <param name="items">An array of strings that represent the text of the subitems for the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the item.
        /// </param>
        /// <param name="foreColor">A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of the item.
        /// </param>
        /// <param name="backColor">A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item.
        /// </param>
        /// <param name="font">A
        /// <see cref="System.Drawing.Font" /> to apply to the item text.
        /// </param>
        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font)
        {
            InitListViewItem(items, -1, imageKey, foreColor, backColor, font, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified subitems and image.
        /// </summary>
        /// <param name="subItems">An array of
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> objects.
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        public ListViewItem(ListViewSubItem[] subItems, string imageKey)
        {
            InitListViewItem(subItems, -1, imageKey, null, null, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified text, image, and group.
        /// </summary>
        /// <param name="text">The text to display for the item. This should not exceed 259 characters.
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string text, string imageKey, ListViewGroup group)
        {
            InitListViewItem(text, -1, imageKey, null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with subitems containing the specified text, image, and group.
        /// </summary>
        /// <param name="items">An array of strings that represents the text for subitems of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string[] items, string imageKey, ListViewGroup group)
        {
            InitListViewItem(items, -1, imageKey, null, null, null, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the subitems containing the specified text, image, colors, font, and group.
        /// </summary>
        /// <param name="items">An array of strings that represents the text of the subitems for the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the item.
        /// </param>
        /// <param name="foreColor">A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of the item.
        /// </param>
        /// <param name="backColor">A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item.
        /// </param>
        /// <param name="font">A
        /// <see cref="System.Drawing.Font" /> to apply to the item text.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group)
        {
            InitListViewItem(items, -1, imageKey, foreColor, backColor, font, group);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified subitems, image, and group.
        /// </summary>
        /// <param name="subItems">An array of
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> objects that represent the subitems of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </param>
        /// <param name="imageKey">The name of the image within the
        /// <see cref="System.Windows.Forms.ListViewItem.ImageList" /> of the owning
        /// <see cref="System.Windows.Forms.ListView" /> to display in the item.
        /// </param>
        /// <param name="group">The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to assign the item to.
        /// </param>
        public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group)
        {
            InitListViewItem(subItems, -1, imageKey, null, null, null, group);
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> class with the specified serialization information and streaming context.
        /// </summary>
        /// <param name="info">A
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> containing information about the
        /// <see cref="System.Windows.Forms.ListViewItem" /> to be initialized.
        /// </param>
        /// <param name="context">A
        /// <see cref="System.Runtime.Serialization.StreamingContext" /> that indicates the source destination and context information of a serialized stream.
        /// </param>
        protected ListViewItem(SerializationInfo info, StreamingContext context)
        {

        }
        /// <summary>
        /// Gets or sets the background color of the item's text.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item's text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color? BackColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the bounding rectangle of the item, including subitems.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounding rectangle of the item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Rectangle Bounds
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is checked.
        /// </summary>
        /// <returns>true if the item is checked; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Checked
        {
            get => _checked;

            set
            {
                _checked = value;
                if (_listView != null)
                    _listView.NativeCheckItem(this, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item has focus within the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        /// <returns>true if the item has focus; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Focused
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the item.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Font" /> to apply to the text displayed by the control. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultFont" /> property if the
        /// <see cref="System.Windows.Forms.ListViewItem" /> is not associated with a
        /// <see cref="System.Windows.Forms.ListView" /> control; otherwise, the font specified in the
        /// <see cref="System.Windows.Forms.Control.Font" /> property for the
        /// <see cref="System.Windows.Forms.ListView" /> control is used.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Font Font
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the foreground color of the item's text.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the foreground color of the item's text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color? ForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the group to which the item is assigned.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewGroup" /> to which the item is assigned.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewGroup Group
        {
            get { return _group; }
            set { _group = value; }
        }

        /// <summary>
        /// Gets or sets the index of the image that is displayed for the item.
        /// </summary>
        /// <returns>The zero-based index of the image in the
        /// <see cref="System.Windows.Forms.ImageList" /> that is displayed for the item. The default is -1.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value specified is less than -1.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int ImageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key for the image that is displayed for the item.
        /// </summary>
        /// <returns>The key for the image that is displayed for the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ImageList" /> that contains the image displayed with the item.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ImageList" /> used by the
        /// <see cref="System.Windows.Forms.ListView" /> control that contains the image displayed with the item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ImageList ImageList
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the number of small image widths by which to indent the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <returns>The number of small image widths by which to indent the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">When setting
        /// <see cref="System.Windows.Forms.ListViewItem.IndentCount" />, the number specified is less than 0.
        /// </exception>
        public int IndentCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the zero-based index of the item within the
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        /// <returns>The zero-based index of the item within the
        /// <see cref="System.Windows.Forms.ListView.ListViewItemCollection" /> of the
        /// <see cref="System.Windows.Forms.ListView" /> control, or -1 if the item is not associated with a
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ListView" /> control that contains the item.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView" /> that contains the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListView ListView
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the name associated with this
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <returns>The name of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position of the upper-left corner of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Point" /> at the upper-left corner of the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The
        /// <see cref="System.Windows.Forms.ListViewItem.Position" /> is set when the containing
        /// <see cref="System.Windows.Forms.ListView" /> is in virtual mode.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <returns>true if the item is selected; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (_listView != null)
                    _listView.NativeSelectItem(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the index of the state image (an image such as a selected or cleared check box that indicates the state of the item) that is displayed for the item.
        /// </summary>
        /// <returns>The zero-based index of the state image in the
        /// <see cref="System.Windows.Forms.ImageList" /> that is displayed for the item.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value specified for this property is less than -1.-or- The value specified for this property is greater than 14.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public int StateImageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection containing all subitems of the item.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItemCollection" /> that contains the subitems.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ListViewSubItemCollection SubItems
        {
            get => _subitems;
        }

        /// <summary>
        /// Gets or sets an object that contains data to associate with the item.
        /// </summary>
        /// <returns>An object that contains information that is associated with the item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        /// <returns>The text to display for the item. This should not exceed 259 characters.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (_listView != null)
                    _listView.NativeUpdateText(this, value);

            }
        }

        /// <summary>
        /// Gets or sets the text shown when the mouse pointer rests on the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        /// <returns>The text shown when the mouse pointer rests on the
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string ToolTipText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ListViewItem.Font" />,
        /// <see cref="System.Windows.Forms.ListViewItem.ForeColor" />, and
        /// <see cref="System.Windows.Forms.ListViewItem.BackColor" /> properties for the item are used for all its subitems.
        /// </summary>
        /// <returns>true if all subitems use the font, foreground color, and background color settings of the item; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool UseItemStyleForSubItems
        {
            get;
            set;
        }
        internal virtual AccessibleObject AccessibilityObject
        {
            get;
        }

        internal ListViewItemImageIndexer ImageIndexer
        {
            get;
        }

        internal string ItemType
        {
            get;
            set;
        }

        internal bool StateImageSet
        {
            get;
        }

        internal bool StateSelected
        {
            get;
            set;
        }
        /// <summary>
        /// Places the item text into edit mode.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The
        /// <see cref="System.Windows.Forms.ListView.LabelEdit" /> property of the associated
        /// <see cref="System.Windows.Forms.ListView" /> is not set to true.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //internal string RelateFlowBoxChildKey { get; set; }
        public void BeginEdit()
        {

        }

        /// <summary>
        /// Creates an identical copy of the item.
        /// </summary>
        /// <returns>An object that represents an item that has the same text, image, and subitems associated with it as the cloned item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual object Clone()
        {
            return null;
        }

        /// <summary>
        /// Ensures that the item is visible within the control, scrolling the contents of the control, if necessary.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void EnsureVisible()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        /// <summary>
        /// Returns the subitem of the
        /// <see cref="System.Windows.Forms.ListViewItem" /> at the specified coordinates.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> at the specified x- and y-coordinates.
        /// </returns>
        /// <param name="x">The x-coordinate.
        /// </param>
        /// <param name="y">The y-coordinate.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
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
        /// Removes the item from its associated
        /// <see cref="System.Windows.Forms.ListView" /> control.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual void Remove()
        {

        }

        internal void Host(ListView parent, int id, int index)
        {

        }

        internal void InitListViewItem(string text, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
        {
            _subitems = new ListViewSubItemCollection(this);
            _subitems.Add(text);
            this.Text = text;
            this.ImageIndex = imageIndex;
            this.ImageKey = imageKey;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
            this.Font = font;
            this.Group = group;
        }

        internal void InitListViewItem(string[] items, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
        {
            InitListViewItem(items.Length > 0 ? items[0] : "", imageIndex, imageKey, foreColor, backColor, font, group);
            foreach (string item in items)
                _subitems.Add(item);

        }

        internal void InitListViewItem(ListViewSubItem[] subItems, int imageIndex, string imageKey, Color? foreColor, Color? backColor, Font font, ListViewGroup group)
        {
            InitListViewItem(subItems.Length > 0 ? subItems[0].Text : "", imageIndex, imageKey, foreColor, backColor, font, group);
            foreach (ListViewSubItem item in subItems)
                _subitems.Add(item);
        }
        /// <summary>
        /// Represents a subitem of a
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        public class ListViewSubItem
        {

            internal Gtk.Label _label;

            internal string _text = string.Empty;
            public ListViewSubItem()
            {

            }

            public ListViewSubItem(ListViewItem owner, string text)
            {
                ListViewItem = owner;
                this.Text = text;
            }

            public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
            {
                ListViewItem = owner;
                this.Text = text;
                this.ForeColor = foreColor;
                this.BackColor = backColor;
                this.Font = font;
            }
            public Color? BackColor
            {
                get;
                set;
            }

            public Rectangle Bounds
            {
                get;
                set;
            }

            public Font Font
            {
                get;
                set;
            }

            public Color? ForeColor
            {
                get;
                set;
            }

            public ListViewItem ListViewItem
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public object Tag
            {
                get;
                set;
            }

            public string Text
            {
                get => _text;
                set
                {
                    _text = value;
                    if (_label != null)
                        _label.Text = value;
                }
            }

            public void ResetStyle()
            {

            }

        }

        /// <summary>
        /// Represents a collection of
        /// <see cref="System.Windows.Forms.ListViewItem.ListViewSubItem" /> objects stored in a
        /// <see cref="System.Windows.Forms.ListViewItem" />
        /// </summary>
        public class ListViewSubItemCollection : List<ListViewSubItem>
        {

            private ListViewItem _owner;

            public ListViewSubItemCollection(ListViewItem owner)
            {
                _owner = owner;
            }
            public virtual ListViewSubItem this[string key]
            {
                get
                {
                    return base.Find(w => w.Name == key);
                }
            }

            public ListViewSubItem Add(string text)
            {
                ListViewSubItem sub = new ListViewSubItem(_owner, text);
                Add(sub);

                return sub;
            }

            public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
            {
                ListViewSubItem sub = new ListViewSubItem(_owner, text, foreColor, backColor, font);
                Add(sub);

                return sub;
            }

            public void AddRange(ListViewSubItem[] items)
            {
                foreach (ListViewSubItem item in items)
                    Add(item);
            }

            public void AddRange(string[] items)
            {
                foreach (string item in items)
                    Add(item);
            }

            public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
            {
                foreach (string item in items)
                    Add(item, foreColor, backColor, font);
            }

            public virtual bool ContainsKey(string key)
            {
                return base.FindIndex(w => w.Name == key) > -1;

            }

            public virtual int IndexOfKey(string key)
            {
                return base.FindIndex(w => w.Name == key);

            }

            public virtual void RemoveByKey(string key)
            {
                RemoveAt(base.FindIndex(w => w.Name == key));
            }
        }
    }
}

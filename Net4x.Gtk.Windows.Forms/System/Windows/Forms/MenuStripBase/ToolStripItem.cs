using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the abstract base class that manages events and layout for all the elements that a
    /// <see cref="System.Windows.Forms.ToolStrip" /> or
    /// <see cref="System.Windows.Forms.ToolStripDropDown" /> can contain.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ToolStripItem : Component, IDropTarget, ISupportOleDropSource, IArrangedElement, IComponent, IDisposable, IKeyboardToolTip
    {

        public virtual string unique_key { get; protected set; }

        public virtual Gtk.Widget Widget { get; }

        public virtual Gtk.MenuItem MenuItem { get; set; }

        public virtual bool Created { get; set; }

        public virtual bool Checked { get; set; }

        public virtual CheckState CheckState { get; set; }

        internal Gtk.Image DefaultImage = new Gtk.Image("image-missing", Gtk.IconSize.Menu);
        /// <summary>
        /// Gets or sets the image that is displayed on a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Image" /> to be displayed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual System.Drawing.Image Image { get; set; }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> class.
        /// </summary>
        public ToolStripItem()
        {
            dropDownItems = new ToolStripItemCollection(this);
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> class with the specified name, image, and event handler.
        /// </summary>
        /// <param name="text">A
        /// <see cref="System.String" /> representing the name of the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to display on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="onClick">Raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event when the user clicks the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        protected ToolStripItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> class with the specified display text, image, event handler, and name.
        /// </summary>
        /// <param name="text">The text to display on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="image">The Image to display on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="onClick">The event handler for the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </param>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        protected ToolStripItem(string text, Image image, EventHandler onClick, string name) : this()
        {
            this.Name = name;
            this.Text = text;
            if (image != null && image.PixbufData != null)
                this.Image = image;

            if (onClick != null)
                Click += onClick;
        }

        public virtual void CreateControl() { }
        public virtual ToolStripItemCollection Items
        {
            get
            {
                return dropDownItems;
            }
        }
        private ToolStripItemCollection dropDownItems;

        //public virtual event EventHandler Disposed;

        public virtual ToolStripItemCollection DropDownItems
        {
            get
            {
                return dropDownItems;
            }
        }
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <returns>A string representing the name. The default value is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets or sets the text that is to be displayed on the item.
        /// </summary>
        /// <returns>A string representing the item's text. The default value is the empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //public virtual string Text { get { return base.Label; } set { base.Label = value; } }
        public virtual string Text { get; set; }
        /// <summary>
        /// Gets or sets the color to treat as transparent in a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> image.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Color" /> values.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Color ImageTransparentColor { get; set; }
        /// <summary>
        /// Gets or sets whether text and images are displayed on a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripItemDisplayStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText" /> .
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ToolStripItemDisplayStyle DisplayStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to use the
        /// <see cref="System.Windows.Forms.ToolStripItem.Text" /> property or the
        /// <see cref="System.Windows.Forms.ToolStripItem.ToolTipText" /> property for the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> ToolTip.
        /// </summary>
        /// <returns>true to use the
        /// <see cref="System.Windows.Forms.ToolStripItem.Text" /> property for the ToolTip; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        //public virtual Size Size { get; set; }
        public virtual bool AutoToolTip { get; set; }
        /// <summary>
        /// Gets or sets the background image displayed in the item.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Image" /> that represents the image to display in the background of the item.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual Image BackgroundImage { get; set; }
        /// <summary>
        /// Gets or sets the background image layout used for the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ImageLayout" /> values. The default value is
        /// <see cref="System.Windows.Forms.ImageLayout.Tile" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual ImageLayout BackgroundImageLayout { get; set; }
        /// <summary>
        /// Gets or sets the text that appears as a
        /// <see cref="System.Windows.Forms.ToolTip" /> for a control.
        /// </summary>
        /// <returns>A string representing the ToolTip text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        //public virtual bool Enabled { get; set; }
        public virtual string ToolTipText { get; set; }
        /// <summary>
        /// Gets or sets the alignment of the image on a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.ContentAlignment" /> values. The default is
        /// <see cref="System.Drawing.ContentAlignment.MiddleLeft" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Drawing.ContentAlignment" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ContentAlignment ImageAlign { get; set; }
        /// <summary>
        /// Gets or sets the index value of the image that is displayed on the item.
        /// </summary>
        /// <returns>The zero-based index of the image in the
        /// <see cref="System.Windows.Forms.ToolStrip.ImageList" /> that is displayed for the item. The default is -1, signifying that the image list is empty.
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
        public virtual int ImageIndex { get; set; }
        /// <summary>
        /// Gets or sets the key accessor for the image in the
        /// <see cref="System.Windows.Forms.ToolStrip.ImageList" /> that is displayed on a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>A string representing the key of the image.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual string ImageKey { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether an image on a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is automatically resized to fit in a container.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripItemImageScaling" /> values. The default is
        /// <see cref="System.Windows.Forms.ToolStripItemImageScaling.SizeToFit" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ToolStripItemImageScaling ImageScaling { get; set; }
        /// <summary>
        /// Gets or sets the position of
        /// <see cref="System.Windows.Forms.ToolStripItem" /> text and image relative to each other.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.TextImageRelation" /> values. The default is
        /// <see cref="System.Windows.Forms.TextImageRelation.ImageBeforeText" />
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual TextImageRelation TextImageRelation { get; set; }
        /// <summary>
        /// Gets the orientation of text used on a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripTextDirection" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ToolStripTextDirection TextDirection { get; set; }
        /// <summary>
        /// Gets or sets the alignment of the text on a
        /// <see cref="System.Windows.Forms.ToolStripLabel" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.ContentAlignment" /> values. The default is
        /// <see cref="System.Drawing.ContentAlignment.MiddleRight" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Drawing.ContentAlignment" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ContentAlignment TextAlign { get; set; }
        /// <summary>
        /// Mirrors automatically the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> image when the
        /// <see cref="System.Windows.Forms.ToolStripItem.RightToLeft" /> property is set to
        /// <see cref="System.Windows.Forms.RightToLeft.Yes" />
        /// </summary>
        /// <returns>true to automatically mirror the image; otherwise, false. The default is false.
        /// </returns>
        // public virtual bool Selected { get; }
        public virtual bool RightToLeftAutoMirrorImage { get; set; }
        /// <summary>
        /// Gets a value indicating whether the state of the item is pressed.
        /// </summary>
        /// <returns>true if the state of the item is pressed; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool Pressed { get; }
        /// <summary>
        /// Gets the current layout of the item.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripItemPlacement" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual ToolStripItemPlacement Placement { get; }
        /// <summary>
        /// Gets or sets whether the item is attached to the
        /// <see cref="System.Windows.Forms.ToolStrip" /> or
        /// <see cref="System.Windows.Forms.ToolStripOverflowButton" /> or can float between the two.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripItemOverflow" /> values. The default is
        /// <see cref="System.Windows.Forms.ToolStripItemOverflow.AsNeeded" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.ToolStripItemOverflow" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual ToolStripItemOverflow Overflow { get; set; }
        /// <summary>
        /// Gets the parent
        /// <see cref="System.Windows.Forms.ToolStripItem" /> of this
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>The parent
        /// <see cref="System.Windows.Forms.ToolStripItem" /> of this
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        public virtual ToolStripItem OwnerItem { get; }
        /// <summary>
        /// Gets or sets the owner of this item.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ToolStrip" /> that owns or is to own the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual ToolStrip Owner { get; set; }
        /// <summary>
        /// Gets or sets the position of a merged item within the current
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </summary>
        /// <returns>An integer representing the index of the merged item, if a match is found, or -1 if a match is not found.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual int MergeIndex { get; set; }
        /// <summary>
        /// Gets or sets how child menus are merged with parent menus.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.MergeAction" /> values. The default is
        /// <see cref="System.Windows.Forms.MergeAction.MatchOnly" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.MergeAction" /> values.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        public virtual MergeAction MergeAction { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the parent control of the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is enabled.
        /// </summary>
        /// <returns>true if the parent control of the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the font of the text displayed by the item.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Font" /> to apply to the text displayed by the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultFont" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //  public virtual bool Focused { get { return this.IsFocus; } }
        public virtual Font Font { get; set; }
        /// <summary>
        /// Gets or sets the foreground color of the item.
        /// </summary>
        /// <returns>The foreground
        /// <see cref="System.Drawing.Color" /> of the item. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultForeColor" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Color ForeColor { get; set; }
        /// <summary>
        /// Gets or sets the background color for the item.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the background color of the item. The default is the value of the
        /// <see cref="System.Windows.Forms.Control.DefaultBackColor" /> property.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Color BackColor { get; set; }

        public virtual bool HasChildren { get; }
        /// <summary>
        /// Gets or sets a value indicating whether the item is automatically sized.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ToolStripItem" /> is automatically sized; otherwise, false. The default value is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual bool AutoSize { get; set; }
        /// <summary>
        /// Gets or sets the height, in pixels, of a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the height, in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int Height { get; set; }
        public virtual ImeMode ImeMode { get; set; }

        public virtual int Left
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the internal spacing, in pixels, between the item's contents and its edges.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Padding" /> representing the item's internal spacing, in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //public override Padding Margin { get; set; }
        //public override Size MaximumSize { get; set; }
        //public override Size MinimumSize { get; set; }
        public virtual Padding Padding { get; set; }
        /// <summary>
        /// Gets or sets the parent container of the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ToolStrip" /> that is the parent container of the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        public virtual ToolStripItem Parent { get; set; }

        public virtual System.Drawing.Region Region { get; set; }
        public virtual int Right { get; }
        /// <summary>
        /// Gets or sets a value indicating whether items are to be placed from right to left and text is to be written from right to left.
        /// </summary>
        /// <returns>true if items are to be placed from right to left and text is to be written from right to left; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual RightToLeft RightToLeft { get; set; }
        /// <summary>
        /// Gets or sets the size of the item.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" />, representing the width and height of a rectangle.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //public virtual ISite Site { get; set; }
        public virtual Size Size { get; set; }
        /// <summary>
        /// Gets or sets the object that contains data about the item.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the control. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public virtual object Tag { get; set; }
        public virtual int Top
        {
            get;
            set;
        }
        public virtual void ResumeLayout()
        {

        }

        public virtual void ResumeLayout(bool performLayout)
        {

        }
        public virtual void SuspendLayout()
        {

        }

        public virtual void PerformLayout()
        {

        }

        public virtual void PerformLayout(Control affectedControl, string affectedProperty)
        {

        }

        public void SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Retrieves the size of a rectangular area into which a control can be fit.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> ordered pair, representing the width and height of a rectangle.
        /// </returns>
        /// <param name="constrainingSize">The custom-sized area for a control.
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Size GetPreferredSize(Size proposedSize)
        {
            throw new NotImplementedException();
        }

        void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string propertyName)
        {
            throw new NotImplementedException();
        }


        public virtual bool UseWaitCursor { get; set; }
        /// <summary>
        /// Gets or sets the width in pixels of a
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the width in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual int Width { get; set; }
        /// <summary>
        /// Gets the size and location of the item.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the size and location of the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle Bounds => throw new NotImplementedException();

        public Rectangle DisplayRectangle => throw new NotImplementedException();

        public bool ParticipatesInLayout => throw new NotImplementedException();

        PropertyStore IArrangedElement.Properties => throw new NotImplementedException();

        IArrangedElement IArrangedElement.Container => throw new NotImplementedException();

        public ArrangedElementCollection Children => throw new NotImplementedException();


        public virtual event EventHandler Click;

        public virtual event EventHandler CheckedChanged;

        public virtual event EventHandler CheckStateChanged;
        public virtual event ToolStripItemClickedEventHandler DropDownItemClicked;
    }

}

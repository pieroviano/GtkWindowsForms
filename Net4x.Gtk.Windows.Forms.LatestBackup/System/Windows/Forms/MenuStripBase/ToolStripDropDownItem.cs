using System;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms
{

    public delegate void ToolStripItemClickedEventHandler(object sender, ToolStripItemClickedEventArgs e);
    /// <summary>
    /// Provides basic functionality for controls that display a
    /// <see cref="System.Windows.Forms.ToolStripDropDown" /> when a
    /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> ,
    /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> , or
    /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
    /// control is clicked.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public abstract class ToolStripDropDownItem : WidgetToolStrip<Gtk.MenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> class.
        /// </summary>
        protected ToolStripDropDownItem() : this("", null, null, "")
        {

        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> class with the specified display text, image, and action to take when the drop-down control is clicked.
        /// </summary>
        /// <param name="text">The display text of the drop-down control.
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the control.
        /// </param>
        /// <param name="onClick">The action to take when the drop-down control is clicked.
        /// </param>
        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {

        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> class with the specified display text, image, and
        /// <see cref="System.Windows.Forms.ToolStripItem" /> collection that the drop-down control contains.
        /// </summary>
        /// <param name="text">The display text of the drop-down control.
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the control.
        /// </param>
        /// <param name="dropDownItems">A
        /// <see cref="System.Windows.Forms.ToolStripItem" /> collection that the drop-down control contains.
        /// </param>
        protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems) : this(text, image, null, "")
        {
            DropDownItems.AddRange(dropDownItems);
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> class with the specified display text, image, action to take when the drop-down control is clicked, and control name.
        /// </summary>
        /// <param name="text">The display text of the drop-down control.
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the control.
        /// </param>
        /// <param name="onClick">The action to take when the drop-down control is clicked.
        /// </param>
        /// <param name="name">The name of the control.
        /// </param>
        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name) : base("ToolStripDropDownItem", text, image, onClick, name)
        {
            DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
        }


        public override string Text { get => base.Text; set => base.Text = value; } //+ " ▼"


        [Browsable(false)]

        public bool HasDropDown { get; }
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> has
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> controls associated with it.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> has
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> controls; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public virtual bool HasDropDownItems { get; }
        /// <summary>
        /// Gets or sets a value indicating the direction in which the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> emerges from its parent container.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownDirection" /> values.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The property is set to a value that is not one of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownDirection" /> values.
        /// </exception>
        //public ToolStripItemCollection DropDownItems { get; }
        [Browsable(false)]
        public ToolStripDropDownDirection DropDownDirection { get; set; }
        /// <summary>
        /// Gets or sets the
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> that will be displayed when this
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> is clicked.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> that is associated with the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [TypeConverter(typeof(ReferenceConverter))]
        public ToolStripDropDown DropDown { get; set; }
        /// <summary>
        /// Gets the screen coordinates, in pixels, of the upper-left corner of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" />
        /// </summary>
        /// <returns>A Point representing the x and y screen coordinates, in pixels.
        /// </returns>
        protected internal virtual Point DropDownLocation { get; }

        public event EventHandler DropDownOpening;

        public event EventHandler DropDownClosed;

        public event EventHandler DropDownOpened;
        /// <summary>
        /// Makes a visible
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> hidden.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //public event ToolStripItemClickedEventHandler DropDownItemClicked;
        public void HideDropDown() { }
        /// <summary>
        /// Displays the
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> control associated with this
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" />
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The
        /// <see cref="System.Windows.Forms.ToolStripDropDownItem" /> is the same as the parent
        /// <see cref="System.Windows.Forms.ToolStrip" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void ShowDropDown() { }

    }
}

using System;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a control that when clicked displays an associated
    /// <see cref="System.Windows.Forms.ToolStripDropDown" />
    /// from which the user can select a single item.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripDropDownButton : ToolStripDropDownItem
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class.
        /// </summary>
        public ToolStripDropDownButton() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class that displays the specified text.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        public ToolStripDropDownButton(string text) : base(text, null) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class that displays the specified image.
        /// </summary>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        public ToolStripDropDownButton(Image image) : base("", image) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class that displays the specified text and image.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        public ToolStripDropDownButton(string text, Image image) : base(text, image) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class that displays the specified text and image and raises the Click event.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="onClick">The event handler for the
        /// <see cref="System.Windows.Forms.Control.Click" /> event.
        /// </param>
        public ToolStripDropDownButton(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="dropDownItems">An array of type
        /// <see cref="System.Windows.Forms.ToolStripItem" /> containing the items of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        public ToolStripDropDownButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> class that has the specified name, displays the specified text and image, and raises the Click event.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        /// <param name="onClick">The event handler for the
        /// <see cref="System.Windows.Forms.Control.Click" /> event.
        /// </param>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />
        /// </param>
        public ToolStripDropDownButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether an arrow is displayed on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />, which indicates that further options are available in a drop-down list.
        /// </summary>
        /// <returns>true to show an arrow on the
        /// <see cref="System.Windows.Forms.ToolStripDropDownButton" />; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //[DefaultValue(true)]
        //public bool AutoToolTip { get; set; }
        [DefaultValue(true)]
        public bool ShowDropDownArrow { get; set; }

        /// <summary>
        /// Gets a value indicating whether to display the
        /// <see cref="System.Windows.Forms.ToolTip" /> that is defined as the default.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        protected bool DefaultAutoToolTip { get; }

        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" /> that contains the event data.
        /// </param>
        //protected AccessibleObject CreateAccessibilityInstance()
        //{
        //}
        //protected  ToolStripDropDown CreateDefaultDropDown()
        //{
        //}
        protected void OnMouseDown(MouseEventArgs e) { }

        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected void OnMouseLeave(EventArgs e) { }

        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.MouseEventArgs" />  that contains the event data.
        /// </param>
        protected void OnMouseUp(MouseEventArgs e)
        {

        }

    }
}

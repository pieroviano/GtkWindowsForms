using System;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a combination of a standard button on the left and a drop-down button on the right, or the other way around if the value of
    /// <see cref="System.Windows.Forms.RightToLeft" /> is Yes.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripSplitButton : ToolStripDropDownItem
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class.
        /// </summary>
        public ToolStripSplitButton() { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified text.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        public ToolStripSplitButton(string text) : base(text, null) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified image.
        /// </summary>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        public ToolStripSplitButton(Image image) : base("", image) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified text and image.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        public ToolStripSplitButton(string text, Image image) : base(text, image) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified display text, image, and
        /// <see cref="System.Windows.Forms.Control.Click" /> event handler.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="onClick">Raises the
        /// <see cref="System.Windows.Forms.Control.Click" /> event when the user clicks the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        public ToolStripSplitButton(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified text, image, and
        /// <see cref="System.Windows.Forms.ToolStripItem" /> array.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="dropDownItems">A
        /// <see cref="System.Windows.Forms.ToolStripItem" /> array of controls.
        /// </param>
        public ToolStripSplitButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> class with the specified display text, image,
        /// <see cref="System.Windows.Forms.Control.Click" /> event handler, and name.
        /// </summary>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="onClick">Raises the
        /// <see cref="System.Windows.Forms.Control.Click" /> event when the user clicks the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </param>
        public ToolStripSplitButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {

        }
        /// <summary>
        /// Gets a value indicating whether the drop-down button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is selected.
        /// </summary>
        /// <returns>true if the drop-down button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is selected; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public bool DropDownButtonSelected { get; }
        /// <summary>
        /// Gets a value indicating whether the drop-down portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is in the pressed state.
        /// </summary>
        /// <returns>true if the drop-down portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is in the pressed state; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public bool DropDownButtonPressed { get; }
        /// <summary>
        /// Gets the size and location, in screen coordinates, of the drop-down button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the size and location of the drop-down button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />, in screen coordinates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public Rectangle DropDownButtonBounds { get; }
        /// <summary>
        /// Gets or sets the portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> that is activated when the control is first selected.
        /// </summary>
        /// <returns>A Forms.ToolStripItem representing the portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> that is activated when first selected. The default value is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        [DefaultValue(null)]
        public ToolStripItem DefaultItem { get; set; }
        /// <summary>
        /// Gets a value indicating whether the standard button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is selected or the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton.DropDownButtonPressed" /> property is true.
        /// </summary>
        /// <returns>true if the button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is selected or whether
        /// <see cref="System.Windows.Forms.ToolStripSplitButton.DropDownButtonPressed" /> is true; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public bool ButtonSelected { get; }
        /// <summary>
        /// Gets a value indicating whether the button portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is in the pressed state.
        /// </summary>
        /// <returns>true if the button portion of the
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> is in the pressed state; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public bool ButtonPressed { get; }
        /// <summary>
        /// Gets the size and location of the standard button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the size and location of the standard button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Browsable(false)]
        public Rectangle ButtonBounds { get; }
        /// <summary>
        /// The width, in pixels, of the drop-down button portion of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Int32" /> representing the width in pixels.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The specified value is less than zero (0).
        /// </exception>
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
        //
        public int DropDownButtonWidth { get; set; }
        /// <summary>
        /// Gets the boundaries of the separator between the standard and drop-down button portions of a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Rectangle" /> that represents the size and location of the separator.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public Rectangle SplitterBounds { get; }
        /// <summary>
        /// Gets a value indicating whether to display the
        /// <see cref="System.Windows.Forms.ToolTip" /> that is defined as the default.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        protected bool DefaultAutoToolTip { get; }
        /// <summary>
        /// Gets a value indicating whether items on a
        /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> are hidden after they are clicked.
        /// </summary>
        /// <returns>true if the items are hidden after they are clicked; otherwise, false.
        /// </returns>
        protected internal bool DismissWhenClicked { get; }

    }
}

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays a single column header in a
    /// <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultProperty("Text")]
    public class ColumnHeader : Component, ICloneable
    {
        internal int _index;

        internal string _name;

        internal string _text;

        internal int _width = 120;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnHeader" /> class.
        /// </summary>
        public ColumnHeader()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnHeader" /> class with the image specified.
        /// </summary>
        /// <param name="imageIndex">The index of the image to display in the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </param>
        public ColumnHeader(int imageIndex)
        {
            ImageIndex = imageIndex;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColumnHeader" /> class with the image specified.
        /// </summary>
        /// <param name="imageKey">The key of the image to display in the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </param>
        public ColumnHeader(string imageKey)
        {
            ImageKey = imageKey;
        }

        public ColumnHeader(string text, int width)
        {
            Text = text;
            Width = width;
        }

        /// <summary>
        /// Gets or sets the display order of the column relative to the currently displayed columns.
        /// </summary>
        /// <returns>The display order of the column, relative to the currently displayed columns.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Localizable(true)]
        public int DisplayIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the image displayed in the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>The index of the image displayed in the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <see cref="System.Windows.Forms.ColumnHeader.ImageIndex" /> is less than -1.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(-1)]
        public int ImageIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the key of the image displayed in the column.
        /// </summary>
        /// <returns>The key of the image displayed in the column.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageKey { get; set; }

        /// <summary>
        /// Gets the image list associated with the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.ImageList" /> associated with the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public ImageList ImageList
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the location with the
        /// <see cref="System.Windows.Forms.ListView" /> control's
        /// <see cref="System.Windows.Forms.ListView.ColumnHeaderCollection" /> of this column.
        /// </summary>
        /// <returns>The zero-based index of the column header within the
        /// <see cref="System.Windows.Forms.ListView.ColumnHeaderCollection" /> of the
        /// <see cref="System.Windows.Forms.ListView" /> control it is contained in.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// Gets the
        /// <see cref="System.Windows.Forms.ListView" /> control the
        /// <see cref="System.Windows.Forms.ColumnHeader" /> is located in.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ListView" /> control that represents the control that contains the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public ListView ListView
        {
            [CompilerGenerated]
            get;
        }

        /// <summary>
        /// Gets or sets the name of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>The name of the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an object that contains data to associate with the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data to associate with the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Localizable(false)]
        [Bindable(true)]
        [DefaultValue(null)]
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text displayed in the column header.
        /// </summary>
        /// <returns>The text displayed in the column header.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Localizable(true)]
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text displayed in the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.HorizontalAlignment" /> values. The default is
        /// <see cref="System.Windows.Forms.HorizontalAlignment.Left" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Localizable(true)]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <returns>The width of the column, in pixels.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [Localizable(true)]
        [DefaultValue(60)]
        public int Width
        {
            get;
            set;
        } = 100;

        /// <summary>
        /// Creates an identical copy of the current
        /// <see cref="System.Windows.Forms.ColumnHeader" /> that is not attached to any list view control.
        /// </summary>
        /// <returns>An object representing a copy of this
        /// <see cref="System.Windows.Forms.ColumnHeader" /> object.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public object Clone()
        {
            return null;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the
        /// <see cref="System.Windows.Forms.ColumnHeader" />
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {

        }

    }
}

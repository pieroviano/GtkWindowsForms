/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkSystem.Windows.Forms.Utility;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows picture box control for displaying an image.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class PictureBox : Control
    {

        private readonly PictureBoxBase self = new PictureBoxBase();

        public override object GtkControl => self;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PictureBox" /> class.
        /// </summary>
        public PictureBox()
        {
            self.Shown += Self_Shown;
        }

        private void Self_Shown(object sender, EventArgs e)
        {
            int width = Width;
            int height = Height;
            if (this.MaximumSize.Width > 0)
            {
                width = Math.Min(this.MaximumSize.Width, Width);
            }
            if (this.MaximumSize.Height > 0)
            {
                height = Math.Min(this.MaximumSize.Height, Height);
            }
            if (this.MinimumSize.Width > 0)
            {
                width = Math.Min(this.MinimumSize.Width, width);
            }
            if (this.MinimumSize.Height > 0)
            {
                height = Math.Min(this.MinimumSize.Height, height);
            }
            if (_image != null && _image.Pixbuf != null)
            {
                ImageUtility.ScaleImageByPictureBoxSizeMode(_image.Pixbuf, width, height, out Gdk.Pixbuf newImagePixbuf, SizeMode);
                self.Pixbuf = newImagePixbuf;
            }
            else if (InitialImage != null && InitialImage.Pixbuf != null)
            {
                ImageUtility.ScaleImageByPictureBoxSizeMode(InitialImage.Pixbuf, width, height, out Gdk.Pixbuf newImagePixbuf, SizeMode);
                self.Pixbuf = newImagePixbuf;
            }
        }
        /// <summary>
        /// Indicates how the image is displayed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.PictureBoxSizeMode" /> values. The default is
        /// <see cref="System.Windows.Forms.PictureBoxSizeMode.Normal" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.PictureBoxSizeMode" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public PictureBoxSizeMode SizeMode { get; set; }
        /// <summary>
        /// Gets or sets the image displayed in the
        /// <see cref="System.Windows.Forms.PictureBox" /> control when the main image is loading.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Image" /> displayed in the picture box control when the main image is loading.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Drawing.Image InitialImage { get; set; }

        private string _ImageLocation;
        /// <summary>
        /// Gets or sets the path or URL for the image to display in the
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </summary>
        /// <returns>The path or URL for the image to display in the
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string ImageLocation { get { return _ImageLocation; } set { _ImageLocation = value; Load(value); } }


        private System.Drawing.Image _image;
        /// <summary>
        /// Gets or sets the image that is displayed by
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Image" /> to display.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override System.Drawing.Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (self.IsRealized && _image != null && _image.PixbufData != null)
                {
                    Self_Shown(null, null);
                }
            }
        }
        /// <summary>
        /// Gets or sets the image to display when an error occurs during the image-loading process or if the image load is canceled.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Image" /> to display if an error occurs during the image-loading process or if the image load is canceled.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Drawing.Image ErrorImage { get; set; }
        /// <summary>
        /// Indicates the border style for the control.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.BorderStyle" /> enumeration values. The default is
        /// <see cref="System.Windows.Forms.BorderStyle.None" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Windows.Forms.BorderStyle" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DefaultValue(BorderStyle.None)]
        public override BorderStyle BorderStyle { get; set; }
        /// <summary>
        /// Cancels an asynchronous image load.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        public void CancelAsync() { }
        /// <summary>
        /// Sets the
        /// <see cref="System.Windows.Forms.PictureBox.ImageLocation" /> to the specified URL and displays the image indicated.
        /// </summary>
        /// <param name="url">The path for the image to display in the
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="url" /> is null or an empty string.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// <paramref name="url" />
        /// refers to an image on the Web that cannot be accessed.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="url" /> refers to a file that is not an image.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// <paramref name="url" />
        /// refers to a file that does not exist.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Load(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            { return; }
            else if (url.Contains("://") && Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                GLib.IFile file = GLib.FileFactory.NewForUri(result);
                GLib.FileInputStream stream = file.Read(new GLib.Cancellable());
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream, new GLib.Cancellable());
                _image = new Bitmap(0, 0);
                _image.Pixbuf = pixbuf;
            }
            else
            {
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(url.Replace("\\\\", "/").Replace("\\", "/"));
                _image = new Bitmap(0, 0);
                _image.Pixbuf = pixbuf;
            }
            if (self.IsMapped && self.IsVisible)
            {
                Self_Shown(null, null);
            }
        }
        /// <summary>
        /// Displays the image specified by the
        /// <see cref="System.Windows.Forms.PictureBox.ImageLocation" /> property of the
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="System.Windows.Forms.PictureBox.ImageLocation" /> is null or an empty string.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public new void Load()
        {
            try
            {
                if (System.IO.File.Exists(ImageLocation))
                {
                    Load(ImageLocation);
                }
            }
            catch { }
        }
        /// <summary>
        /// Loads the image asynchronously.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void LoadAsync()
        {
            if (System.IO.File.Exists(ImageLocation))
            {
                LoadAsync(ImageLocation);
            }
        }
        /// <summary>
        /// Loads the image at the specified location, asynchronously.
        /// </summary>
        /// <param name="url">The path for the image to display in the
        /// <see cref="System.Windows.Forms.PictureBox" />
        /// </param>
        /// <filterpriority>2
        /// </filterpriority>
        public void LoadAsync(string url)
        {
            Threading.Tasks.Task.Run(() => Gtk.Application.Invoke(new EventHandler((o, e) =>
            {
                Load(url);
            })));
        }

        public override void EndInit()
        {

        }
    }
}

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

        private System.Drawing.Image _image;

        private string _ImageLocation;

        private readonly PictureBoxBase self = new PictureBoxBase();

        public PictureBox()
        {
            self.Shown += Self_Shown;
        }

        [DefaultValue(BorderStyle.None)]
        public override BorderStyle BorderStyle { get; set; }

        public System.Drawing.Image ErrorImage { get; set; }

        public override object GtkControl => self;

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

        public string ImageLocation { get { return _ImageLocation; } set { _ImageLocation = value; Load(value); } }

        public System.Drawing.Image InitialImage { get; set; }

        public PictureBoxSizeMode SizeMode { get; set; }

        public void CancelAsync() { }

        public override void EndInit()
        {

        }

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

        public void LoadAsync()
        {
            if (System.IO.File.Exists(ImageLocation))
            {
                LoadAsync(ImageLocation);
            }
        }

        public void LoadAsync(string url)
        {
            Threading.Tasks.Task.Run(() => Gtk.Application.Invoke(new EventHandler((o, e) =>
            {
                Load(url);
            })));
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

    }
}

/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkSystem.Windows.Forms.Utility;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows picture box control for displaying an image.
/// </summary>
[DesignerCategory("Component")]
public partial class PictureBox : Control
{

    private Image? _image;

    private string? _ImageLocation;

    private readonly PictureBoxBase self = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PictureBox" /> class.
    /// </summary>
    public PictureBox()
    {
        self.Shown += Self_Shown;
    }

    /// <summary>
    /// Gets or sets the border style for the control.
    /// </summary>
    /// <value>
    /// The border style.
    /// </value>
    [DefaultValue(BorderStyle.None)]
    public override BorderStyle BorderStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the error image.
    /// </summary>
    /// <value>
    /// The error image.
    /// </value>
    public Image? ErrorImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public override System.Drawing.Image? Image
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
    /// Gets or sets the image location.
    /// </summary>
    /// <value>
    /// The image location.
    /// </value>
    public string? ImageLocation
    {
        get { return _ImageLocation; }
        set { _ImageLocation = value; Load(value); }
    }

    /// <summary>
    /// Gets or sets the initial image.
    /// </summary>
    /// <value>
    /// The initial image.
    /// </value>
    public Image? InitialImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the size mode.
    /// </summary>
    /// <value>
    /// The size mode.
    /// </value>
    public PictureBoxSizeMode SizeMode
    {
        get;
        set;
    }

    /// <summary>
    /// Cancels the asynchronous.
    /// </summary>
    public void CancelAsync() { }

    /// <summary>
    /// Signals the object that initialization is complete.
    /// </summary>
    public override void EndInit()
    {

    }

    /// <summary>
    /// Loads the specified URL.
    /// </summary>
    /// <param name="url">
    /// The URL.
    /// </param>
    public new void Load(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        { return; }
        else if ((url?.Contains("://") ?? false) && Uri.TryCreate(url, UriKind.Absolute, out var result))
        {
            var file = GLib.FileFactory.NewForUri(result);
            var stream = file.Read(new GLib.Cancellable());
            var pixbuf = new Gdk.Pixbuf(stream, new GLib.Cancellable());
            _image = new Bitmap(0, 0)
            {
                Pixbuf = pixbuf
            };
        }
        else
        {
            var pixbuf = new Gdk.Pixbuf(url?.Replace("\\\\", "/").Replace("\\", "/"));
            _image = new Bitmap(0, 0)
            {
                Pixbuf = pixbuf
            };
        }
        if (self.IsMapped && self.IsVisible)
        {
            Self_Shown(null, null);
        }
    }

    /// <summary>
    /// Loads this instance.
    /// </summary>
    public new void Load()
    {
        try
        {
            if (System.IO.File.Exists(ImageLocation))
            {
                Load(ImageLocation);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug($"Unexpected exception in Load", ex);
        }
    }

    /// <summary>
    /// Loads the asynchronous.
    /// </summary>
    public void LoadAsync()
    {
        if (System.IO.File.Exists(ImageLocation))
        {
            LoadAsync(ImageLocation);
        }
    }

    /// <summary>
    /// Loads the asynchronous.
    /// </summary>
    /// <param name="url">
    /// The URL.
    /// </param>
    public void LoadAsync(string? url)
    {
        Threading.Tasks.Task.Run(() => Gtk.Application.Invoke(new EventHandler((o, e) =>
        {
            Load(url);
        })));
    }

    private void Self_Shown(object? sender, EventArgs? e)
    {
        var width = Width;
        var height = Height;
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
            ImageUtility.ScaleImageByPictureBoxSizeMode(_image.Pixbuf, width, height, out var newImagePixbuf, SizeMode);
            self.Pixbuf = newImagePixbuf;
        }
        else if (InitialImage != null && InitialImage.Pixbuf != null)
        {
            ImageUtility.ScaleImageByPictureBoxSizeMode(InitialImage.Pixbuf, width, height, out var newImagePixbuf, SizeMode);
            self.Pixbuf = newImagePixbuf;
        }
    }
}
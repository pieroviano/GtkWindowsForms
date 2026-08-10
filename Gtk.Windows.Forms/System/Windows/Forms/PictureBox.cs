/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows picture box control for displaying an image.
/// </summary>
[DesignerCategory("Component")]
public partial class PictureBox : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly PictureBoxBase self = new();

    private Image? _backgroundImage;

    private Image? _image;

    private string? _ImageLocation;

    private Image? _initialImage;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PictureBox" /> class.
    /// </summary>
    public PictureBox()
    {
        self.Override.sender = this;
    }

    /// <summary>
    /// Gets or sets the background image displayed in the control.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public override Image? BackgroundImage
    {
        get => _backgroundImage;
        set { _backgroundImage = value; self.BackgroundImage = value?.Pixbuf; }
    }

    /// <summary>
    /// Gets or sets the background image layout.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    public override ImageLayout BackgroundImageLayout
    {
        get => self.BackgroundImageLayout;
        set => self.BackgroundImageLayout = value;
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
    public override Image? Image
    {
        get { return _image; }
        set
        {
            _image = value;
            self.Image = _image?.Pixbuf;
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
        get => _ImageLocation;
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
        get => _initialImage;
        set { _initialImage = value; if (_image == null) { this.Image = value; } }
    }

    /// <summary>
    /// Gets or sets the maximum size of the control.
    /// </summary>
    /// <value>
    /// The maximum size.
    /// </value>
    public override Size MaximumSize
    {
        get => self.MaximumSize;
        set => self.MaximumSize = value;
    }

    /// <summary>
    /// Gets or sets the minimum size of the control.
    /// </summary>
    /// <value>
    /// The minimum size.
    /// </value>
    public override Size MinimumSize
    {
        get => self.MinimumSize;
        set => self.MinimumSize = value;
    }

    /// <summary>
    /// Gets or sets the size mode.
    /// </summary>
    /// <value>
    /// The size mode.
    /// </value>
    public PictureBoxSizeMode SizeMode
    {
        get => self.SizeMode;
        set { self.SizeMode = value; }
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
        if (self.contaner.GetChildrenArray().Length > 1)
        {
            self.contaner.GetChildrenArray()[1].Destroy();
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }
        else if (url!.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
        {
            // supports animation， animated images are not scaled or positioned
            var image = new Gtk.Image(new Gdk.Pixbuf(url.Replace("\\\\", "/").Replace("\\", "/"))) // Gtk4: GtkImage animation dropped; static first frame
            {
                Halign = Gtk.Align.Center,
                Valign = Gtk.Align.Center,
                // Must be visible: adding an unmapped-but-invisible child to an already-shown overlay
                // trips a fatal "gtk_widget_map: widget visible" GTK-CRITICAL when the parent maps it.
                Visible = true
            };
            self.contaner.AddOverlay(image);
        }
        else if (url.Contains("://") && Uri.TryCreate(url, UriKind.Absolute, out var result))
        {
            var file = GLib.FileFactory.NewForUri(result);
            var stream = file.Read(new GLib.Cancellable());
            var pixbuf = new Gdk.Pixbuf(stream, new GLib.Cancellable());
            this.Image = new Bitmap(pixbuf.Width, pixbuf.Height) { Pixbuf = pixbuf };
        }
        else
        {
            var path = url.Replace("\\\\", "/").Replace("\\", "/");
            // A missing local file is a no-op, not an error (Load is expected not to throw). Upstream
            // dropped this File.Exists guard during the alignment, so a bad path threw a GLib.GException.
            if (!System.IO.File.Exists(path))
            {
                return;
            }
            var pixbuf = new Gdk.Pixbuf(path);
            this.Image = new Bitmap(pixbuf.Width, pixbuf.Height) { Pixbuf = pixbuf };
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
        Threading.Tasks.Task.Run(() => GLib.Idle.Add(0, () => { Load(url); return false; }));
    }
}

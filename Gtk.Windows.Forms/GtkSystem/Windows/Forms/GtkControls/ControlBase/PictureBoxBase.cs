using System.Drawing;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// PictureBoxBase class.
/// </summary>
/// <seealso cref="Gtk.Viewport" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class PictureBoxBase : Gtk.Viewport, IControlGtk
{
    /// <summary>
    /// The overlay container that hosts the drawing area and any additional overlay widgets (for example animated images).
    /// </summary>
    public Gtk.Overlay contaner = new Gtk.Overlay();

    private Gdk.Pixbuf? _backgroundImage;

    private ImageLayout _backgroundImageLayout = ImageLayout.None;

    private Gdk.Pixbuf? _drawBackgroundImage;

    private Gdk.Pixbuf? _drawImage;

    private Gdk.Pixbuf? _image;

    private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PictureBoxBase" /> class.
    /// </summary>
    public PictureBoxBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("PictureBox");
        this.Halign = Gtk.Align.Start;
        this.Valign = Gtk.Align.Start;
        this.Realized += PictureBoxBase_Realized;
        var drawingArea = new Gtk.DrawingArea();
        drawingArea.DrawFunc = DrawingArea_DrawFunc;
        // Paint is raised from this child, not from the viewport, so an invalidation has to land here.
        this.Override.PaintSurface = drawingArea;
        contaner.AddChild(drawingArea);
        this.AddChild(contaner);
    }

    /// <summary>
    /// Gets or sets the background image that is painted behind the control content.
    /// </summary>
    /// <value>
    /// The background image, or
    /// <c>
    /// null
    /// </c> when no background image is set.
    /// </value>
    public Gdk.Pixbuf? BackgroundImage
    {
        get => _backgroundImage;
        set
        {
            _backgroundImage = value;
            ScaleBackgroundImage();
        }
    }

    /// <summary>
    /// Gets or sets the layout used to paint
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PictureBoxBase.BackgroundImage" />.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    public ImageLayout BackgroundImageLayout
    {
        get => _backgroundImageLayout;
        set
        {
            _backgroundImageLayout = value;
            ScaleBackgroundImage();
        }
    }

    /// <summary>
    /// Gets or sets the image displayed by the control.
    /// </summary>
    /// <value>
    /// The image, or
    /// <c>
    /// null
    /// </c> when no image is set.
    /// </value>
    public Gdk.Pixbuf? Image
    {
        get => _image;
        set
        {
            _image = value;
            ScaleImage();
        }
    }

    /// <summary>
    /// Gets or sets the maximum size honoured when scaling the image in
    /// <see cref="!:PictureBoxSizeMode.AutoSize" /> mode.
    /// </summary>
    /// <value>
    /// The maximum size.
    /// </value>
    public Size MaximumSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum size honoured when scaling the image in
    /// <see cref="!:PictureBoxSizeMode.AutoSize" /> mode.
    /// </summary>
    /// <value>
    /// The minimum size.
    /// </value>
    public Size MinimumSize
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler" /> and System.Drawing via
    /// <see cref="!:PaintEventHandler" />). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
    /// </summary>
    /// <value>
    /// The active
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> used by the control.
    /// </value>
    public GtkControlOverride Override
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets how the image is sized and positioned inside the control.
    /// </summary>
    /// <value>
    /// The size mode.
    /// </value>
    public PictureBoxSizeMode SizeMode
    {
        get => _sizeMode;
        set
        {
            _sizeMode = value;
            ScaleImage();
        }
    }

    /// <summary>
    /// Produces the pixbuf that should actually be painted for a background image, applying the requested
    /// <see cref="!:ImageLayout" />.
    /// </summary>
    /// <param name="image">
    /// The source image.
    /// </param>
    /// <param name="layoutMode">
    /// The layout mode.
    /// </param>
    /// <param name="width">
    /// The target width.
    /// </param>
    /// <param name="height">
    /// The target height.
    /// </param>
    /// <returns>
    /// The scaled, centered or tiled image to paint.
    /// </returns>
    public Gdk.Pixbuf ScaleDrawnBackground(Gdk.Pixbuf image, ImageLayout layoutMode, int width, int height)
    {
        Gdk.Pixbuf? result = null;
        if (layoutMode == ImageLayout.None)
        {
            // tile the original image starting from the top-left corner
            result = image;
        }
        else if (layoutMode == ImageLayout.Stretch)
        { // freely scale to fill the entire image
            result = image.ScaleSimple(width, height, Gdk.InterpType.Hyper);
        }
        else if (layoutMode == ImageLayout.Center)
        {
            // take the center of the original image
            int offsetx = (width - image.Width) / 2;
            int offsety = (height - image.Height) / 2;
            var imageSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
            result = imageSurface.ToPixbuf(0, 0, width, height);
            int copy_width = offsetx > 0 ? image.Width : width;
            int copy_height = offsety > 0 ? image.Height : height;
            image.CopyArea(Math.Max(0, -offsetx), Math.Max(0, -offsety), copy_width, copy_height, result, Math.Max(0, offsetx), Math.Max(0, offsety));
            imageSurface.Dispose();
        }
        else if (layoutMode == ImageLayout.Zoom)
        {
            // scale original image proportionally, display full image
            double scaleW = width * 1f / image.Width;
            double scaleH = height * 1f / image.Height;
            if (scaleW > scaleH)
            {
                result = image.ScaleSimple((int)(Math.Max(1, image.Width * scaleH)), height, Gdk.InterpType.Hyper);
            }
            else
            {
                result = image.ScaleSimple(width, (int)(Math.Max(1, image.Height * scaleW)), Gdk.InterpType.Hyper);
            }
        }
        else if (layoutMode == ImageLayout.Tile)
        {
            var imageSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
            result = imageSurface.ToPixbuf(0, 0, width, height);
            int copywidth = Math.Min(width, image.Width);
            int copyheight = Math.Min(height, image.Height);
            // tile background image, fill with original image
            for (int y = 0; y < height; y += image.Height)
            {
                for (int x = 0; x < width; x += image.Width)
                {
                    image.CopyArea(0, 0, Math.Min(copywidth, width - x), Math.Min(copyheight, height - y), result, x, y);
                }
            }
            imageSurface.Dispose();
        }
        else
        {
            result = image;
        }

        return result;
    }

    /// <summary>
    /// Paints the background image, the override content and the image into the drawing area. Invoked by GTK as the
    /// drawing area's draw function.
    /// </summary>
    /// <param name="area">
    /// The drawing area being rendered.
    /// </param>
    /// <param name="cr">
    /// The Cairo context to paint on.
    /// </param>
    /// <param name="width">
    /// The width of the drawing area, in pixels.
    /// </param>
    /// <param name="height">
    /// The height of the drawing area, in pixels.
    /// </param>
    private void DrawingArea_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        if (_drawBackgroundImage != null)
        {
            cr.Save();
            Gdk.CairoHelper.SetSourcePixbuf(cr, _drawBackgroundImage, 0, 0);
            cr.Paint();
            cr.Restore();
        }
        Override.OnPaint(cr);
        if (_drawImage != null)
        {
            cr.Save();
            Gdk.CairoHelper.SetSourcePixbuf(cr, _drawImage, 0, 0);
            cr.Paint();
            cr.Restore();
        }
    }

    /// <summary>
    /// Handles the Realized event and rescales the background image and the image now that the allocation is known.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs" /> instance containing the event data.
    /// </param>
    private void PictureBoxBase_Realized(object? sender, EventArgs e)
    {
        ScaleBackgroundImage();
        ScaleImage();
    }

    /// <summary>
    /// Recomputes the background image that is actually painted, based on the current layout and allocation.
    /// </summary>
    private void ScaleBackgroundImage()
    {
        if (_backgroundImage != null && this.IsRealized)
            _drawBackgroundImage = ScaleDrawnBackground(_backgroundImage, _backgroundImageLayout, Math.Max(this.WidthRequest, this.AllocatedWidth), Math.Max(this.HeightRequest, this.AllocatedHeight));
        else
            _drawBackgroundImage = null;
    }

    /// <summary>
    /// Produces the pixbuf that should actually be painted for the image, applying the requested
    /// <see cref="!:PictureBoxSizeMode" />.
    /// </summary>
    /// <param name="image">
    /// The source image.
    /// </param>
    /// <param name="sizeMode">
    /// The size mode.
    /// </param>
    /// <param name="width">
    /// The target width.
    /// </param>
    /// <param name="height">
    /// The target height.
    /// </param>
    /// <returns>
    /// The scaled, centered or tiled image to paint.
    /// </returns>
    private Gdk.Pixbuf ScaleDrawnSource(Gdk.Pixbuf image, PictureBoxSizeMode sizeMode, int width, int height)
    {
        Gdk.Pixbuf? result = null;
        if (sizeMode == PictureBoxSizeMode.Normal)
        {
            // tile the original image starting from the top-left corner
            result = image;
        }
        else if (sizeMode == PictureBoxSizeMode.StretchImage)
        {
            // freely scale to fill the entire image
            result = image.ScaleSimple(width, height, Gdk.InterpType.Hyper);
        }
        else if (sizeMode == PictureBoxSizeMode.CenterImage)
        {
            // take the center of the original image
            int offsetx = (width - image.Width) / 2;
            int offsety = (height - image.Height) / 2;
            var imageSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
            result = imageSurface.ToPixbuf(0, 0, width, height);
            int copy_width = offsetx > 0 ? image.Width : width;
            int copy_height = offsety > 0 ? image.Height : height;
            image.CopyArea(Math.Max(0, -offsetx), Math.Max(0, -offsety), copy_width, copy_height, result, Math.Max(0, offsetx), Math.Max(0, offsety));
            imageSurface.Dispose();
        }
        else if (sizeMode == PictureBoxSizeMode.Zoom)
        {
            // scale original image proportionally, display full image
            double scaleW = width * 1f / image.Width;
            double scaleH = height * 1f / image.Height;
            if (scaleW > scaleH)
            {
                result = image.ScaleSimple((int)(Math.Max(1, image.Width * scaleH)), height, Gdk.InterpType.Hyper);
            }
            else
            {
                result = image.ScaleSimple(width, (int)(Math.Max(1, image.Height * scaleW)), Gdk.InterpType.Hyper);
            }
        }
        else if (sizeMode == PictureBoxSizeMode.AutoSize)
        {
            // original image without scaling, stretch PictureBox
            if (MaximumSize.Width > 0 && MaximumSize.Height > 0 && image.Width > MaximumSize.Width && image.Height > MaximumSize.Height)
                result = image.ScaleSimple(MaximumSize.Width, MaximumSize.Height, Gdk.InterpType.Hyper);
            else if (MaximumSize.Width > 0 && image.Width > MaximumSize.Width)
                result = image.ScaleSimple(MaximumSize.Width, image.Height, Gdk.InterpType.Hyper);
            else if (MaximumSize.Width > 0 && image.Height > MaximumSize.Height)
                result = image.ScaleSimple(image.Width, MaximumSize.Height, Gdk.InterpType.Hyper);
            else
                result = image;

            if (MinimumSize.Width > 0 && MinimumSize.Height > 0 && result.Width < MinimumSize.Width && result.Height < MinimumSize.Height)
                result = result.ScaleSimple(MinimumSize.Width, MinimumSize.Height, Gdk.InterpType.Hyper);
            else if (MinimumSize.Width > 0 && result.Width < MinimumSize.Width)
                result = result.ScaleSimple(MinimumSize.Width, result.Height, Gdk.InterpType.Hyper);
            else if (MinimumSize.Height > 0 && result.Height < MinimumSize.Height)
                result = result.ScaleSimple(result.Width, MinimumSize.Height, Gdk.InterpType.Hyper);

            if (this.WidthRequest < result.Width)
                this.WidthRequest = result.Width;
            if (this.HeightRequest < result.Height)
                this.HeightRequest = result.Height;
        }
        else if (sizeMode == PictureBoxSizeMode.Tile)
        {
            var imageSurface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height);
            result = imageSurface.ToPixbuf(0, 0, width, height);
            int copywidth = Math.Min(width, image.Width);
            int copyheight = Math.Min(height, image.Height);
            // tile background image, fill with original image
            for (int y = 0; y < height; y += image.Height)
            {
                for (int x = 0; x < width; x += image.Width)
                {
                    image.CopyArea(0, 0, Math.Min(copywidth, width - x), Math.Min(copyheight, height - y), result, x, y);
                }
            }
            imageSurface.Dispose();
        }
        else
        {
            result = image;
        }

        return result;
    }

    /// <summary>
    /// Recomputes the image that is actually painted, based on the current size mode and allocation.
    /// </summary>
    private void ScaleImage()
    {
        if (_image != null && this.IsRealized)
            _drawImage = ScaleDrawnSource(_image, _sizeMode, Math.Max(this.WidthRequest, this.AllocatedWidth), Math.Max(this.HeightRequest, this.AllocatedHeight));
        else
            _drawImage = null;
    }
}
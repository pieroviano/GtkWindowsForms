using System.Drawing;
using System.Windows.Forms;
using Gtk;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// GtkControlOverride is a class that provides an implementation of the IControlOverride interface for GTK controls.
/// It allows developers to customize the appearance and behavior of GTK controls by providing properties for background color,
/// background image, image layout, and image alignment. The class also includes methods for adding and removing CSS classes,
/// clearing native backgrounds, and handling drawing events for the control's background and images. By using GtkControlOverride,
/// developers can easily enhance the visual aspects of their GTK controls while maintaining compatibility with the underlying GTK framework.
///
/// GtkControlOverride is a reusable helper attached to GTK widget wrappers to centralize painting, background/image handling and CSS-class management. It’s constructed by controls (in their constructors), then the control calls its lifecycle helpers from GTK overrides.
/// Where it’s created(examples in your repo)
/// What GtkControlOverride provides / how it’s used
/// •	Construction: pass the GTK Widget instance to the ctor: new GtkControlOverride(widget).
/// •	CSS management: AddClass(string) / RemoveClass(string) and OnAddClass() applies the list to container.StyleContext.
/// •	Background handling:
/// •	BackColor, BackgroundImage, BackgroundImageLayout properties control background painting.
/// •	OnDrawnBackground(Cairo.Context, Gdk.Rectangle) paints the BackColor and scaled background image, then raises DrawnBackground.
/// •	Foreground/image painting:
/// •	Image / ImageAlign for a displayed image; OnDrawnImage(...) draws it.
/// •	Paint events: •	OnPaint(Cairo.Context, Gdk.Rectangle) raises:
/// •	PaintGraphics(Cairo.Context, Rectangle) — for direct Cairo drawing callbacks.
/// •	Paint(object, PaintEventArgs) — for System.Drawing-style painting; it builds a Graphics wrapper and passes it in PaintEventArgs.
/// •	ClearNativeBackground() is present as a hook(empty by default) for platform-specific background clearing.
/// Typical usage pattern in a control
/// •	Create override in your control constructor.
/// •	Add style classes with Override.AddClass(...).
/// •	In the control’s GTK draw handler call:
/// •	Override.OnDrawnBackground(cr, area) — to paint background and image, or
/// •	Override.OnDrawnImage(cr, area) — to paint the image.
/// •	In OnDrawn(Cairo.Context) you can call Override.OnPaint(cr, area) to raise Paint/PaintGraphics for custom drawing.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlOverride" />
public sealed class GtkControlOverride : IControlOverride
{

    private readonly Widget container;

    private readonly List<string> cssList = new();

    /// <summary>
    /// The sender used when raising the paint events.
    /// </summary>
    public object sender;

    /// <summary>
    /// Occurs when the background is drawn.
    /// </summary>
    // Gtk 4: no Gtk3 DrawnHandler/DrawnArgs; expose the Cairo context directly to subscribers.
    public event Action<object, Cairo.Context>? DrawnBackground;

    /// <summary>
    /// Occurs when paint.
    /// </summary>
    public event PaintEventHandler? Paint;

    /// <summary>
    /// Occurs when paint graphics.
    /// </summary>
    public event PaintGraphicsEventHandler? PaintGraphics;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride" /> class.
    /// </summary>
    /// <param name="container">
    /// The container.
    /// </param>
    public GtkControlOverride(Widget container)
    {
        this.container = container;
        this.sender = container;
    }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public System.Drawing.Color? BackColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the colour that is to be painted as transparent instead of opaque - see
    /// <see cref="P:System.Windows.Forms.Form.TransparencyKey" />. <see langword="null" /> when no key is
    /// set, which is the normal case.
    /// </summary>
    public System.Drawing.Color? TransparencyKey
    {
        get;
        set;
    }

    /// <summary>
    /// Whether <see cref="BackColor" /> is the colour the transparency key names, in which case the
    /// background is not painted at all but punched out of the surface.
    /// </summary>
    private bool BackColorIsKeyed =>
        TransparencyKey.HasValue
        && BackColor.HasValue
        && TransparencyKey.Value.R == BackColor.Value.R
        && TransparencyKey.Value.G == BackColor.Value.G
        && TransparencyKey.Value.B == BackColor.Value.B;

    /// <summary>
    /// Paints the background colour, or clears the surface to full transparency when that colour is the
    /// one the transparency key names.
    /// </summary>
    /// <param name="cr">The Cairo context.</param>
    /// <returns><see langword="true" /> when something was painted.</returns>
    private bool PaintBackColor(Cairo.Context cr)
    {
        if (!BackColor.HasValue)
        {
            return false;
        }

        cr.Save();
        if (BackColorIsKeyed)
        {
            // Painting zero alpha in the default OVER operator is a no-op; SOURCE is what actually
            // replaces the surface content, which is how the window ends up see-through.
            cr.Operator = Cairo.Operator.Source;
            cr.SetSourceRGBA(0, 0, 0, 0);
        }
        else
        {
            cr.SetSourceRGBA(BackColor.Value.R / 255f, BackColor.Value.G / 255f, BackColor.Value.B / 255f, BackColor.Value.A / 255f);
        }

        cr.Paint();
        cr.Restore();

        return true;
    }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public Image? BackgroundImage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the background image layout.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    public ImageLayout BackgroundImageLayout
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public Image? Image
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the image align.
    /// </summary>
    /// <value>
    /// The image align.
    /// </value>
    public System.Drawing.ContentAlignment ImageAlign
    {
        get;
        set;
    }

    internal void RaisePaint(UserControl control) =>
        Paint?.Invoke(
            control,
            new PaintEventArgs(null!, new System.Drawing.Rectangle())
        );

    /// <summary>
    /// Adds the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    public void AddClass(string cssClass)
    {
        cssList.Add(cssClass);
    }

    /// <summary>
    /// Clears the native background.
    /// </summary>
    public void ClearNativeBackground()
    {
    }

    /// <summary>
    /// Draws the color of the back.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <param name="area">
    /// The area.
    /// </param>
    public void DrawnBackColor(Cairo.Context cr, Gdk.Rectangle area)
    {
        PaintBackColor(cr);
    }

    /// <summary>
    /// Called when add class.
    /// </summary>
    public void OnAddClass()
    {
        foreach (var cssClass in cssList)
        {
            if (container.StyleContext.HasClass(cssClass))
                container.StyleContext.RemoveClass(cssClass);
            container.StyleContext.AddClass(cssClass);
        }
        ClearNativeBackground();
    }

    /// <summary>
    /// Called when drawn background.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <param name="area">
    /// The area.
    /// </param>
    public void OnDrawnBackground(Cairo.Context cr, Gdk.Rectangle area)
    {
        PaintBackColor(cr);
        if (BackgroundImage != null && BackgroundImage.Pixbuf != null)
        {
            SetDrawnBackground(cr, BackgroundImage.Pixbuf, BackgroundImageLayout, area.Width, area.Height);
        }

        DrawnBackground?.Invoke(this.container, cr);
    }

    /// <summary>
    /// Called when drawn image.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <param name="area">
    /// The area.
    /// </param>
    public void OnDrawnImage(Cairo.Context cr, Gdk.Rectangle area)
    {
        if (Image != null && Image.Pixbuf != null)
        {
            SetDrawImage(cr, Image.Pixbuf, area, ImageAlign);
        }
    }

    /// <summary>
    /// Called when paint.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <param name="area">
    /// The area.
    /// </param>
    public void OnPaint(Cairo.Context cr, Gdk.Rectangle area)
    {
        if (PaintGraphics != null)
        {
            cr.Save();
            PaintGraphics.Invoke(this, new PaintGraphicsEventArgs(cr, new Rectangle(area.X, area.Y, area.Width, area.Height)));
            cr.Restore();
        }
        Paint?.Invoke(sender, new PaintEventArgs(new Graphics(container, cr, area), new Rectangle(area.X, area.Y, area.Width, area.Height)));
    }

    /// <summary>
    /// Called when paint.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    public void OnPaint(Cairo.Context cr)
    {
        if (PaintGraphics != null)
        {
            {
                var ce = cr.ClipExtents(); var clipRect = new Gdk.Rectangle((int)ce.X, (int)ce.Y, (int)ce.Width, (int)ce.Height); // Gtk4: GetClipRectangle -> ClipExtents
                cr.Save();
                PaintGraphics.Invoke(this, new PaintGraphicsEventArgs(cr, new Rectangle(0, 0, clipRect.Width, clipRect.Height)));
                cr.Restore();
            }
        }
        if (Paint != null)
        {
            var ce = cr.ClipExtents();
            var clipRect = new Gdk.Rectangle((int)ce.X, (int)ce.Y, (int)ce.Width, (int)ce.Height);
            Paint.Invoke(sender, new PaintEventArgs(new Graphics(container, cr, clipRect), new Rectangle(0, 0, clipRect.Width, clipRect.Height)));
        }
    }

    /// <summary>
    /// Removes the class.
    /// </summary>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    public void RemoveClass(string cssClass)
    {
        cssList.Remove(cssClass);
    }

    /// <summary>
    /// Draws the image at the given alignment.
    /// </summary>
    /// <param name="ctx">
    /// The Cairo context.
    /// </param>
    /// <param name="img">
    /// The image.
    /// </param>
    /// <param name="rec">
    /// The area.
    /// </param>
    /// <param name="ImageAlign">
    /// The image align.
    /// </param>
    public void SetDrawImage(Cairo.Context ctx, Gdk.Pixbuf img, Gdk.Rectangle rec, ContentAlignment ImageAlign)
    {
        ctx.Save();
        if (ImageAlign == ContentAlignment.TopLeft)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.TopCenter)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) / 2 + rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.TopRight)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) + rec.X, rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleLeft)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleCenter)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) / 2 + rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.MiddleRight)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) + rec.X, (img.Height - rec.Height) / 2 + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomLeft)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, rec.X, (img.Height - rec.Height) + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomCenter)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) / 2 + rec.X, (img.Height - rec.Height) + rec.Y);
        else if (ImageAlign == ContentAlignment.BottomRight)
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, (img.Width - rec.Width) + rec.X, (img.Height - rec.Height) + rec.Y);
        else
            Gdk.CairoHelper.SetSourcePixbuf(ctx, img, rec.X, rec.Y);

        using (var p = ctx.GetSource())
        {
            if (p is Cairo.SurfacePattern pattern)
            {
                pattern.Filter = Cairo.Filter.Fast;
            }
        }
        ctx.Paint();
        ctx.Restore();
    }

    /// <summary>
    /// Draws the background image using the specified layout.
    /// </summary>
    /// <param name="cr">
    /// The Cairo context.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    /// <param name="layoutMode">
    /// The layout mode.
    /// </param>
    /// <param name="width">
    /// The width.
    /// </param>
    /// <param name="height">
    /// The height.
    /// </param>
    public void SetDrawnBackground(Cairo.Context cr, Gdk.Pixbuf image, ImageLayout layoutMode, int width, int height)
    {
        if (layoutMode == ImageLayout.None)
        {
            // tile the original image starting from the top-left corner
            Gdk.CairoHelper.SetSourcePixbuf(cr, image, 0, 0);
        }
        else if (layoutMode == ImageLayout.Stretch)
        { // freely scale to fill the entire image
            Gdk.CairoHelper.SetSourcePixbuf(cr, image.ScaleSimple(width, height, Gdk.InterpType.Nearest), 0, 0);
        }
        else if (layoutMode == ImageLayout.Center)
        {
            // take the center of the original image
            int offsetx = (width - image.Width) / 2;
            int offsety = (height - image.Height) / 2;
            Gdk.CairoHelper.SetSourcePixbuf(cr, image, offsetx, offsety);
        }
        else if (layoutMode == ImageLayout.Zoom)
        {
            // scale original image proportionally， display full image
            double scaleW = width * 1f / image.Width;
            double scaleH = height * 1f / image.Height;
            if (scaleW > scaleH)
                Gdk.CairoHelper.SetSourcePixbuf(cr, image.ScaleSimple((int)(image.Width * scaleH), height, Gdk.InterpType.Nearest), 0, 0);
            else
                Gdk.CairoHelper.SetSourcePixbuf(cr, image.ScaleSimple(width, (int)(image.Height * scaleW), Gdk.InterpType.Nearest), 0, 0);
        }
        else if (layoutMode == ImageLayout.Tile)
        {
            // tile background image， fill with original image
            for (int y = 0; y < height; y += image.Height)
            {
                for (int x = 0; x < width; x += image.Width)
                {
                    Gdk.CairoHelper.SetSourcePixbuf(cr, image, x, y);
                    cr.Paint();
                }
            }
        }
        else
        {
            Gdk.CairoHelper.SetSourcePixbuf(cr, image, 0, 0);
        }
        cr.Paint();
    }

    /// <summary>
    /// Raises the PaintGraphics event.
    /// </summary>
    private void OnPaintGraphics(GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventArgs e)
    {
        PaintGraphics?.Invoke(this, e);
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gtk;
using GtkSystem.Windows.Forms.Utility;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// PaintGraphicsEventHandler delegate.
/// </summary>
/// <param name="cr">
/// The Cairo context.
/// </param>
/// <param name="rec">
/// The record.
/// </param>
public delegate void PaintGraphicsEventHandler(Cairo.Context cr, Rectangle rec);
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

    private System.Drawing.Image _BackgroundImage;

    private Gdk.Pixbuf backgroundPixbuf;

    private readonly Widget container;

    private readonly List<string> cssList = new List<string>();

    private Gdk.Pixbuf imagePixbuf;

    /// <summary>
    /// Occurs when the background is drawn.
    /// </summary>
    public event DrawnHandler DrawnBackground;

    /// <summary>
    /// Occurs when paint.
    /// </summary>
    public event PaintEventHandler Paint;

    /// <summary>
    /// Occurs when paint graphics.
    /// </summary>
    public event PaintGraphicsEventHandler PaintGraphics;

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
    }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public System.Drawing.Color? BackColor { get; set; }

    /// <summary>
    /// Gets or sets the background image.
    /// </summary>
    /// <value>
    /// The background image.
    /// </value>
    public System.Drawing.Image BackgroundImage { get { return _BackgroundImage; } set { _BackgroundImage = value; backgroundPixbuf = null; } }

    /// <summary>
    /// Gets or sets the background image layout.
    /// </summary>
    /// <value>
    /// The background image layout.
    /// </value>
    public ImageLayout BackgroundImageLayout { get; set; }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public System.Drawing.Image Image { get; set; }

    /// <summary>
    /// Gets or sets the image align.
    /// </summary>
    /// <value>
    /// The image align.
    /// </value>
    public System.Drawing.ContentAlignment ImageAlign { get; set; }

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
        if (BackColor.HasValue)
        {
            cr.Save();
            cr.SetSourceRGBA(BackColor.Value.R / 255f, BackColor.Value.G / 255f, BackColor.Value.B / 255f, BackColor.Value.A / 255f);
            cr.Paint();
            cr.Restore();
        }
    }

    /// <summary>
    /// Called when add class.
    /// </summary>
    public void OnAddClass()
    {
        foreach (string cssClass in cssList)
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
        if (BackColor.HasValue)
        {
            cr.Save();
            cr.SetSourceRGBA(BackColor.Value.R / 255f, BackColor.Value.G / 255f, BackColor.Value.B / 255f, BackColor.Value.A / 255f);
            cr.Paint();
            cr.Restore();
        }
        if (BackgroundImage != null && BackgroundImage.PixbufData != null)
        {
            if (backgroundPixbuf == null || backgroundPixbuf.Width != area.Width || backgroundPixbuf.Height != area.Height)
            {
                ImageUtility.ScaleImageByImageLayout(BackgroundImage.PixbufData, area.Width, area.Height, out backgroundPixbuf, BackgroundImageLayout);
            }
            ImageUtility.DrawImage(cr, backgroundPixbuf, area, ContentAlignment.TopLeft);
        }

        if (DrawnBackground != null)
        {
            var args = new DrawnArgs() { Args = new object[] { cr } };
            DrawnBackground(this.container, args);
        }
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
        if (Image != null && Image.PixbufData != null)
        {
            if (imagePixbuf == null || imagePixbuf.Width != area.Width || imagePixbuf.Height != area.Height)
            {
                var imagepixbuf = new Gdk.Pixbuf(Image.PixbufData);
                imagePixbuf = imagepixbuf.ScaleSimple(area.Width, area.Height, Gdk.InterpType.Nearest);
            }
            ImageUtility.DrawImage(cr, imagePixbuf, area, ImageAlign);
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
        PaintGraphics?.Invoke(cr, new Rectangle(area.X, area.Y, area.Width, area.Height));
        Paint?.Invoke(this.container, new PaintEventArgs(new Graphics(container, cr, area), new Rectangle(area.X, area.Y, area.Width, area.Height)));
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

}
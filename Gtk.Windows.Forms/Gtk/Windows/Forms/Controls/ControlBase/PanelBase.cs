using Gtk;

namespace Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// PanelBase class
/// </summary>
/// <seealso cref="Gtk.Windows.Forms.Controls.ControlBase.ScrollableBoxBase" />
public sealed class PanelBase : ScrollableBoxBase
{
    /// <summary>
    /// The overlay container that hosts the background drawing area.
    /// </summary>
    public Gtk.Overlay contaner = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="Gtk.Windows.Forms.Controls.ControlBase.PanelBase" /> class.
    /// </summary>
    public PanelBase() : base()
    {
        this.StyleContext.AddClass("Panel");
        contaner.SetMargin(0);
        contaner.Halign = Align.Fill;
        contaner.Valign = Align.Fill;
        contaner.Hexpand = false;
        contaner.Vexpand = false;
        var background = new Gtk.DrawingArea();
        background.DrawFunc = Background_DrawFunc;
        // Paint is raised from this child, not from the scroller, so an invalidation has to land here.
        Override.PaintSurface = background;
        contaner.AddChild(background);
        this.Add(contaner);
    }

    private void Background_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        Override.OnPaint(cr);
    }
}

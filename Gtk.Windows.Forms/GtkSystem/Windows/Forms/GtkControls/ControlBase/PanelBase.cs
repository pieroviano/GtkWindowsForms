using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// PanelBase class
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
public sealed class PanelBase : ScrollableBoxBase
{
    /// <summary>
    /// The overlay container that hosts the background drawing area.
    /// </summary>
    public Gtk.Overlay contaner = new();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.PanelBase" /> class.
    /// </summary>
    public PanelBase() : base()
    {
        this.StyleContext.AddClass("Panel");
        contaner.Margin = 0;
        contaner.Halign = Align.Fill;
        contaner.Valign = Align.Fill;
        contaner.Hexpand = false;
        contaner.Vexpand = false;
        var background = new Gtk.DrawingArea();
        background.DrawFunc = Background_DrawFunc;
        contaner.Add(background);
        this.Add(contaner);
    }

    private void Background_DrawFunc(Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
    {
        Override.OnPaint(cr);
    }
}

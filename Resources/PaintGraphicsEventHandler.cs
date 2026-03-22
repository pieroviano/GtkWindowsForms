using System.Drawing;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// Provides data for the PaintGraphics event.
/// </summary>
public class PaintGraphicsEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaintGraphicsEventArgs"/> class.
    /// </summary>
    /// <param name="context">The Cairo context.</param>
    /// <param name="rectangle">The rectangle area to paint.</param>
    public PaintGraphicsEventArgs(Cairo.Context context, Rectangle rectangle)
    {
        Context = context;
        Rectangle = rectangle;
    }

    /// <summary>
    /// Gets the Cairo context used for drawing.
    /// </summary>
    public Cairo.Context Context { get; }

    /// <summary>
    /// Gets the rectangle area to paint.
    /// </summary>
    public Rectangle Rectangle { get; }
}

/// <summary>
/// Represents the method that will handle the PaintGraphics event.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">A <see cref="PaintGraphicsEventArgs"/> that contains the event data.</param>
public delegate void PaintGraphicsEventHandler(object sender, PaintGraphicsEventArgs e);

using Gtk;
using System.Drawing;
using System.Windows.Forms;
using static GtkSystem.Windows.Forms.GtkControls.ControlBase.GtkControlOverride;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public interface IControlOverride
    {
        event DrawnHandler DrawnBackground;

        event PaintEventHandler Paint;

        event PaintGraphicsEventHandler PaintGraphics;

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        Color? BackColor { get; set; }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        System.Drawing.Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the background image layout.
        /// </summary>
        /// <value>
        /// The background image layout.
        /// </value>
        ImageLayout BackgroundImageLayout { get; set; }

        /// <summary>
        /// Adds the class.
        /// </summary>
        /// <param name="cssClass">
        /// The CSS class.
        /// </param>
        void AddClass(string cssClass);

        /// <summary>
        /// Called when add class.
        /// </summary>
        void OnAddClass();

        /// <summary>
        /// Called when drawn background.
        /// </summary>
        /// <param name="cr">
        /// The Cairo context.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        void OnDrawnBackground(Cairo.Context cr, Gdk.Rectangle area);

        /// <summary>
        /// Called when paint.
        /// </summary>
        /// <param name="cr">
        /// The Cairo context.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        void OnPaint(Cairo.Context cr, Gdk.Rectangle area);

    }
}

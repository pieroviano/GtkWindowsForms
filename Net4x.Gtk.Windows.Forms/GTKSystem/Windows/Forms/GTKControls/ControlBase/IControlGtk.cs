using System;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// IControlGtk interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IControlGtk : IDisposable
    {
        /// <summary>
        /// Gets or sets the <see cref="GtkControlOverride"/> instance that centralizes drawing and styling behavior for this control. This override encapsulates background painting (color and image), image drawing, CSS class management and raises paint events so the control can handle custom painting (both direct Cairo via <see cref="PaintGraphicsEventHandler"/> and System.Drawing via <see cref="PaintEventHandler"/>). The override is typically created and configured in the control's constructor and invoked from the GTK drawing/show handlers.
        /// </summary>
        /// <value>
        /// The active <see cref="GtkControlOverride"/> used by the control.
        /// </value>
        GtkControlOverride Override { get; set; }

    }
}

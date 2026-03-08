using System;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public interface IControlGtk : IDisposable
    {
        /// <summary>
        /// Gets or sets the override.
        /// </summary>
        /// <value>
        /// The override.
        /// </value>
        GtkControlOverride Override { get; set; }

    }
}

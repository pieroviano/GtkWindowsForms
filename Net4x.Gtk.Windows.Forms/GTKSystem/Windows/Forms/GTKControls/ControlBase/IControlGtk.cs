using System;
using System.Windows.Forms;


namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public interface IControlGtk : IDisposable
    {
        GtkControlOverride Override { get; set; }
    }
}
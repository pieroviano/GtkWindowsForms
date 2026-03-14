using Gtk;

namespace System.Windows.Forms;
/// <summary>
/// Class DialogEventArgs. Implements the
/// <see cref="T:System.EventArgs" />
/// </summary>
/// <seealso cref="T:System.EventArgs" />
public class DialogEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DialogEventArgs" /> class.
    /// </summary>
    /// <param name="dia">
    /// The dia.
    /// </param>
    public DialogEventArgs(Dialog dia)
    {
        Dialog = dia;
    }

    /// <summary>
    /// Gets the dialog.
    /// </summary>
    /// <value>
    /// The dialog.
    /// </value>
    public Dialog Dialog { get; }

}
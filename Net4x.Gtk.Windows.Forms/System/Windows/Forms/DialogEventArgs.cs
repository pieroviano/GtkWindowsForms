using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Class DialogEventArgs.
/// Implements the <see cref="System.EventArgs" />
/// </summary>
/// <seealso cref="System.EventArgs" />
public class DialogEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogEventArgs"/> class.
    /// </summary>
    /// <param name="dia">The dia.</param>
    public DialogEventArgs(Dialog dia)
    {
        Dialog = dia;
    }

    /// <summary>
    /// Gets the dialog.
    /// </summary>
    /// <value>The dialog.</value>
    public Dialog Dialog { get; }

}

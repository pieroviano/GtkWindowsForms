using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a control that allows the user to select a single item from a list that is displayed when the user clicks a
/// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> Although
/// <see cref="T:System.Windows.Forms.ToolStripDropDownMenu" /> and
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" />
/// replace and add functionality to the
/// <see cref="!:System.Windows.Forms.Menu" />
/// control of previous versions,
/// <see cref="!:System.Windows.Forms.Menu" />
/// is retained for both backward compatibility and future use if you choose.
/// </summary>
public class ToolStripDropDown : ToolStrip
{
    /// <summary>
    /// The self — a native GTK popup menu. Hides <see cref="ToolStrip" />'s toolbar-box <c>self</c>; the
    /// drop-down is rendered as a real <c>Gtk.Menu</c> while inheriting the <see cref="Control" /> API
    /// surface from <see cref="ToolStrip" />.
    /// </summary>
    public new StripDropDown self = new StripDropDown();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> class.
    /// </summary>
    public ToolStripDropDown() : base()
    {

    }

    /// <summary>
    /// Gets the underlying GTK control — the native popup menu, so the inherited <see cref="Control" />
    /// plumbing targets it rather than the toolbar box.
    /// </summary>
    public override object GtkControl => self;
}

using System.Drawing;
using System.Text;

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
public class ToolStripDropDown : ToolStripItem
{
    /// <summary>
    /// The self
    /// </summary>
    //public readonly ToolStripDropDownBase self = new ToolStripDropDownBase();
    public readonly Gtk.Menu self = new Gtk.Menu();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> class.
    /// </summary>
    public ToolStripDropDown() : base()
    {

    }

    /// <summary>
    /// Gets or sets the size of the image scaling.
    /// </summary>
    /// <value>
    /// The size of the image scaling.
    /// </value>
    public Size ImageScalingSize { get; set; }

    /// <summary>
    /// Gets the widget.
    /// </summary>
    /// <value>
    /// The widget.
    /// </value>
    public override Gtk.Widget Widget => self;

}
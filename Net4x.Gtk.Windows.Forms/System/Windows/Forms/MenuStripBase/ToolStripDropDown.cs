using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a control that allows the user to select a single item from a list that is displayed when the user clicks a
    /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> Although
    /// <see cref="System.Windows.Forms.ToolStripDropDownMenu" /> and
    /// <see cref="System.Windows.Forms.ToolStripDropDown" />
    /// replace and add functionality to the
    /// <see cref="!:System.Windows.Forms.Menu" />
    /// control of previous versions,
    /// <see cref="!:System.Windows.Forms.Menu" />
    /// is retained for both backward compatibility and future use if you choose.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripDropDown : ToolStripItem
    {
        //public readonly ToolStripDropDownBase self = new ToolStripDropDownBase();

        public readonly Gtk.Menu self = new Gtk.Menu();
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDown" /> class.
        /// </summary>
        public ToolStripDropDown() : base()
        {

        }

        public Size ImageScalingSize { get; set; }

        public override Gtk.Widget Widget => self;
    }
}

namespace System.Windows.Forms;

/// <summary>
/// Represents a line used to group items of a
/// <see cref="T:System.Windows.Forms.ToolStrip" /> or the drop-down items of a
/// <see cref="T:System.Windows.Forms.MenuStrip" /> or
/// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> or other
/// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> control.
/// </summary>
public class ToolStripSeparator : ToolStripItem
{

    public StripSeparator self = new StripSeparator();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripSeparator" /> class.
    /// </summary>
    public ToolStripSeparator() : base()
    {

    }

    public override IToolMenuItem Widget { get => self; }
}

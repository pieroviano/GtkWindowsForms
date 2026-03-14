namespace System.Windows.Forms;

/// <summary>
/// Determines the alignment of a
/// <see cref="T:System.Windows.Forms.ToolStripItem" /> in a
/// <see cref="T:System.Windows.Forms.ToolStrip" />
/// </summary>
/// <filterpriority> 2
/// </filterpriority>
public enum ToolStripItemAlignment
{

    /// <summary>
    /// Specifies that the System.Windows.Forms.ToolStripItem is to be anchored toward
    /// the left or top end of the System.Windows.Forms.ToolStrip, depending on the System.Windows.Forms.ToolStrip
    /// orientation. If the value of System.Windows.Forms.RightToLeft is Yes, items marked
    /// as System.Windows.Forms.ToolStripItemAlignment.Left are aligned to the right
    /// side of the System.Windows.Forms.ToolStrip.
    /// </summary>
    Left = 0,

    /// <summary>
    /// Specifies that the System.Windows.Forms.ToolStripItem is to be anchored toward
    /// the right or bottom end of the System.Windows.Forms.ToolStrip, depending on the
    /// System.Windows.Forms.ToolStrip orientation. If the value of System.Windows.Forms.RightToLeft
    /// is Yes, items marked as System.Windows.Forms.ToolStripItemAlignment.Right are
    /// aligned to the left side of the System.Windows.Forms.ToolStrip.
    /// </summary>
    Right = 1
}
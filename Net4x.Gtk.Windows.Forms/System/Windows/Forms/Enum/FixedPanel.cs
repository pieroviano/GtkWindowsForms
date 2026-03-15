using System;

namespace System.Windows.Forms;

/// <summary>
/// Specifies that
/// <see cref="T:System.Windows.Forms.SplitContainer.Panel1" /> ,
/// <see cref="T:System.Windows.Forms.SplitContainer.Panel2" />
/// , or neither panel is fixed.
/// </summary>
public enum FixedPanel
{
    /// <summary>
    /// The none
    /// </summary>
    None = 0,
    /// <summary>
    /// The panel1
    /// </summary>
    Panel1 = 1,
    /// <summary>
    /// The panel2
    /// </summary>
    Panel2 = 2
}
using System;

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="!:System.Windows.Forms.ToolStrip.ItemClicked" /> event.
/// </summary>
public class ToolStripItemClickedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripItemClickedEventArgs" /> class.
    /// </summary>
    /// <param name="clickedItem">
    /// The clicked item.
    /// </param>
    public ToolStripItemClickedEventArgs(ToolStripItem clickedItem)
    {
        this.ClickedItem = clickedItem;
    }

    /// <summary>
    /// Gets the clicked item.
    /// </summary>
    /// <value>
    /// The clicked item.
    /// </value>
    public ToolStripItem ClickedItem { get; }
}
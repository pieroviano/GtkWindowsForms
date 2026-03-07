using System;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.ToolStrip.ItemClicked" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripItemClickedEventArgs : EventArgs
    {

        public ToolStripItemClickedEventArgs(ToolStripItem clickedItem)
        {
            this.ClickedItem = clickedItem;
        }

        public ToolStripItem ClickedItem { get; }
    }
}

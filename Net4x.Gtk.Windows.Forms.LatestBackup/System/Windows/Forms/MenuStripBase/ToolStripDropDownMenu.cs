using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides basic functionality for the
    /// <see cref="T:System.Windows.Forms.ContextMenuStrip" /> control. Although
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownMenu" /> and
    /// <see cref="T:System.Windows.Forms.ToolStripDropDown" />
    /// replace and add functionality to the
    /// <see cref="!:System.Windows.Forms.Menu" />
    /// control of previous versions,
    /// <see cref="!:System.Windows.Forms.Menu" />
    /// is retained for both backward compatibility and future use if you choose.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripDropDownMenu : ToolStripDropDown
    {

        public ToolStripDropDownMenu()
        {

        }

        //public Rectangle DisplayRectangle { get; }

        //public LayoutEngine LayoutEngine { get; }

        public ToolStripLayoutStyle LayoutStyle { get; set; }

        [DefaultValue(false)]

        public bool ShowCheckMargin { get; set; }

        [DefaultValue(true)]

        public bool ShowImageMargin { get; set; } = true;

        protected internal Size MaxItemSize { get; }

        protected Padding DefaultPadding { get; }

        protected internal ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) { return null; }

        protected void OnFontChanged(EventArgs e) { }

        protected void OnLayout(LayoutEventArgs e) { }

        protected void OnPaintBackground(PaintEventArgs e) { }

        protected void SetDisplayedItems() { }

    }
}

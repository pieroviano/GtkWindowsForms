using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides basic functionality for the
    /// <see cref="System.Windows.Forms.ContextMenuStrip" /> control. Although
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
    public class ToolStripDropDownMenu : ToolStripDropDown
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripDropDownMenu" /> class.
        /// </summary>
        public ToolStripDropDownMenu()
        {

        }
        /// <summary>
        /// Gets or sets a value indicating how the items of
        /// <see cref="System.Windows.Forms.ContextMenuStrip" /> are displayed.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.ToolStripLayoutStyle.Flow" />
        /// </returns>
        //public Rectangle DisplayRectangle { get; }
        //public LayoutEngine LayoutEngine { get; }
        public ToolStripLayoutStyle LayoutStyle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether space for an image is shown on the left edge of the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" />
        /// </summary>
        /// <returns>true if the image margin is shown; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool ShowImageMargin { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether space for a check mark is shown on the left edge of the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" />
        /// </summary>
        /// <returns>true if the check margin is shown; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool ShowCheckMargin { get; set; }
        /// <summary>
        /// Gets the internal spacing, in pixels, of the control.
        /// </summary>
        /// <returns>A Padding object representing the spacing.
        /// </returns>
        protected Padding DefaultPadding { get; }
        /// <summary>
        /// Gets the maximum height and width, in pixels, of the
        /// <see cref="System.Windows.Forms.ContextMenuStrip" />
        /// </summary>
        /// <returns>A Size object representing the height and width of the control, in pixels.
        /// </returns>
        protected internal Size MaxItemSize { get; }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripDropDown.FontChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected void OnFontChanged(EventArgs e) { }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.Control.Layout" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.LayoutEventArgs" /> that contains the event data.
        /// </param>
        protected void OnLayout(LayoutEventArgs e) { }
        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.Windows.Forms.PaintEventArgs" /> that contains the event data.
        /// </param>
        protected void OnPaintBackground(PaintEventArgs e) { }
        /// <summary>
        /// Resets the collection of displayed and overflow items after a layout is done.
        /// </summary>
        protected void SetDisplayedItems() { }
        /// <summary>
        /// Creates a default
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> with the specified text, image, and event handler on a new
        /// <see cref="System.Windows.Forms.ToolStripDropDownMenu" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" />, or a
        /// <see cref="System.Windows.Forms.ToolStripSeparator" /> if the
        /// <paramref name="text" /> parameter is a hyphen (-).
        /// </returns>
        /// <param name="text">The text to use for the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> If the
        /// <paramref name="text" /> parameter is a hyphen (-), this method creates a
        /// <see cref="System.Windows.Forms.ToolStripSeparator" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to display on the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" />
        /// </param>
        /// <param name="onClick">An event handler that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event when the
        /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> is clicked.
        /// </param>
        protected internal ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) { return null; }
    }
}

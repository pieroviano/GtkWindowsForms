using System;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a selectable
    /// <see cref="System.Windows.Forms.ToolStripItem" />
    /// that can contain text and images.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripButton : WidgetToolStrip<Gtk.MenuItem>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class.
        /// </summary>
        public ToolStripButton() : this("", null, null, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class that displays the specified text.
        /// </summary>
        /// <param name="text">The text to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        public ToolStripButton(string text) : this(text, null, null, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class that displays the specified image.
        /// </summary>
        /// <param name="image">The image to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        public ToolStripButton(Image image) : this("", image, null, "") { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class that displays the specified text and image.
        /// </summary>
        /// <param name="text">The text to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        /// <param name="image">The image to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        public ToolStripButton(string text, Image image) : this(text, image, null, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class that displays the specified text and image and that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </summary>
        /// <param name="text">The text to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        /// <param name="image">The image to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        /// <param name="onClick">An event handler that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </param>
        public ToolStripButton(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> class with the specified name that displays the specified text and image and that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </summary>
        /// <param name="text">The text to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        /// <param name="image">The image to display on the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        /// <param name="onClick">An event handler that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </param>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </param>
        public ToolStripButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> should automatically appear pressed in and not pressed in when clicked.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ToolStripButton" /> should automatically appear pressed in and not pressed in when clicked; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool CheckOnClick { get; set; }

        /// <summary>
        /// Gets a value indicating whether to display the ToolTip that is defined as the default.
        /// </summary>
        /// <returns>true in all cases.
        /// </returns>
        protected bool DefaultAutoToolTip { get; }

        /// <summary>
        /// Creates a new accessibility object for the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </summary>
        /// <returns>A new
        /// <see cref="System.Windows.Forms.AccessibleObject" /> for the
        /// <see cref="System.Windows.Forms.ToolStripButton" />
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected AccessibleObject CreateAccessibilityInstance()
        {
            return new AccessibleObject();
        }

        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripButton.CheckedChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnCheckedChanged(EventArgs e) { }

        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.ToolStripButton.CheckStateChanged" /> event.
        /// </summary>
        /// <param name="e">An
        /// <see cref="System.EventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {

        }

    }
}

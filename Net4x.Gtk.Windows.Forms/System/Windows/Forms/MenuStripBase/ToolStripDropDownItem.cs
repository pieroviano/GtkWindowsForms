using System;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms
{

    /// <summary>
    /// Delegate ToolStripItemClickedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ToolStripItemClickedEventArgs"/> instance containing the event data.</param>
    public delegate void ToolStripItemClickedEventHandler(object sender, ToolStripItemClickedEventArgs e);
    /// <summary>
    /// Provides basic functionality for controls that display a
    /// <see cref="T:System.Windows.Forms.ToolStripDropDown" /> when a
    /// <see cref="T:System.Windows.Forms.ToolStripDropDownButton" /> ,
    /// <see cref="T:System.Windows.Forms.ToolStripMenuItem" /> , or
    /// <see cref="T:System.Windows.Forms.ToolStripSplitButton" /> control is clicked.
    /// </summary>
    public abstract class ToolStripDropDownItem : WidgetToolStrip<Gtk.MenuItem>
    {

        /// <summary>
        /// Occurs when [drop down closed].
        /// </summary>
        public event EventHandler DropDownClosed;

        /// <summary>
        /// Occurs when [drop down opened].
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when [drop down opening].
        /// </summary>
        public event EventHandler DropDownOpening;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripDropDownItem"/> class.
        /// </summary>
        protected ToolStripDropDownItem() : this("", null, null, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripDropDownItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="image">The image.</param>
        /// <param name="onClick">The on click.</param>
        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripDropDownItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="image">The image.</param>
        /// <param name="dropDownItems">The drop down items.</param>
        protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems) : this(text, image, null, "")
        {
            DropDownItems.AddRange(dropDownItems);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripDropDownItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="image">The image.</param>
        /// <param name="onClick">The on click.</param>
        /// <param name="name">The name.</param>
        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name) : base("ToolStripDropDownItem", text, image, onClick, name)
        {
            DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
        }

        /// <summary>
        /// Gets or sets the drop down.
        /// </summary>
        /// <value>The drop down.</value>
        [TypeConverter(typeof(ReferenceConverter))]
        public ToolStripDropDown DropDown { get; set; }

        //public ToolStripItemCollection DropDownItems { get; }

        /// <summary>
        /// Gets or sets the drop down direction.
        /// </summary>
        /// <value>The drop down direction.</value>
        [Browsable(false)]

        public ToolStripDropDownDirection DropDownDirection { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has drop down.
        /// </summary>
        /// <value><c>true</c> if this instance has drop down; otherwise, <c>false</c>.</value>
        [Browsable(false)]

        public bool HasDropDown { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has drop down items.
        /// </summary>
        /// <value><c>true</c> if this instance has drop down items; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public virtual bool HasDropDownItems { get; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public override string Text { get => base.Text; set => base.Text = value; } //+ " ▼"

        /// <summary>
        /// Gets the drop down location.
        /// </summary>
        /// <value>The drop down location.</value>
        protected internal virtual Point DropDownLocation { get; }

        //public event ToolStripItemClickedEventHandler DropDownItemClicked;

        /// <summary>
        /// Hides the drop down.
        /// </summary>
        public void HideDropDown() { }

        /// <summary>
        /// Shows the drop down.
        /// </summary>
        public void ShowDropDown() { }

    }
}

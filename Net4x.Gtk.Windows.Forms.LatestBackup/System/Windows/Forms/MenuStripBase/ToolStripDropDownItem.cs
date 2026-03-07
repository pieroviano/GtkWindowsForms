using System;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms
{

    public delegate void ToolStripItemClickedEventHandler(object sender, ToolStripItemClickedEventArgs e);
    /// <summary>
    /// Provides basic functionality for controls that display a
    /// <see cref="System.Windows.Forms.ToolStripDropDown" /> when a
    /// <see cref="System.Windows.Forms.ToolStripDropDownButton" /> ,
    /// <see cref="System.Windows.Forms.ToolStripMenuItem" /> , or
    /// <see cref="System.Windows.Forms.ToolStripSplitButton" /> control is clicked.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public abstract class ToolStripDropDownItem : WidgetToolStrip<Gtk.MenuItem>
    {

        public event EventHandler DropDownClosed;

        public event EventHandler DropDownOpened;

        public event EventHandler DropDownOpening;
        protected ToolStripDropDownItem() : this("", null, null, "")
        {

        }

        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {

        }

        protected ToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems) : this(text, image, null, "")
        {
            DropDownItems.AddRange(dropDownItems);
        }

        protected ToolStripDropDownItem(string text, Image image, EventHandler onClick, string name) : base("ToolStripDropDownItem", text, image, onClick, name)
        {
            DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
        }

        [TypeConverter(typeof(ReferenceConverter))]
        public ToolStripDropDown DropDown { get; set; }

        //public ToolStripItemCollection DropDownItems { get; }

        [Browsable(false)]

        public ToolStripDropDownDirection DropDownDirection { get; set; }

        [Browsable(false)]

        public bool HasDropDown { get; }

        [Browsable(false)]
        public virtual bool HasDropDownItems { get; }

        public override string Text { get => base.Text; set => base.Text = value; } //+ " ▼"

        protected internal virtual Point DropDownLocation { get; }

        //public event ToolStripItemClickedEventHandler DropDownItemClicked;

        public void HideDropDown() { }

        public void ShowDropDown() { }

    }
}

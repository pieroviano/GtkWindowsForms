using System;
using System.Drawing;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a control that when clicked displays an associated
    /// <see cref="System.Windows.Forms.ToolStripDropDown" />
    /// from which the user can select a single item.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripDropDownButton : ToolStripDropDownItem
    {

        public ToolStripDropDownButton() { }

        public ToolStripDropDownButton(string text) : base(text, null) { }

        public ToolStripDropDownButton(Image image) : base("", image) { }

        public ToolStripDropDownButton(string text, Image image) : base(text, image) { }

        public ToolStripDropDownButton(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }

        public ToolStripDropDownButton(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }

        public ToolStripDropDownButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {

        }

        //[DefaultValue(true)]
        //public bool AutoToolTip { get; set; }

        [DefaultValue(true)]
        public bool ShowDropDownArrow { get; set; }

        protected bool DefaultAutoToolTip { get; }

        //protected AccessibleObject CreateAccessibilityInstance()
        //{

        //}

        //protected  ToolStripDropDown CreateDefaultDropDown()
        //{

        //}

        protected void OnMouseDown(MouseEventArgs e) { }

        protected void OnMouseLeave(EventArgs e) { }

        protected void OnMouseUp(MouseEventArgs e)
        {

        }

    }
}

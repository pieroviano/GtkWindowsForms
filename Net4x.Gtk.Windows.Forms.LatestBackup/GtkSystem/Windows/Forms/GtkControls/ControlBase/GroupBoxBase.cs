using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// GroupBoxBase is a base class for GTK group box controls, providing common functionality and properties for derived classes. It implements the IControlGtk and IScrollableBoxBase interfaces, allowing it to integrate with the GTK system and provide consistent behavior across different types of group box controls. The GroupBoxBase class includes features such as auto-scrolling, event handling for scrolling, and custom drawing capabilities.
    /// </summary>
    /// <seealso cref="Gtk.Frame" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" />
    public sealed class GroupBoxBase : Gtk.Frame, IControlGtk, IScrollableBoxBase
    {

        public event System.Windows.Forms.ScrollEventHandler Scroll;

        public GroupBoxBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("GroupBox");
            this.LabelXalign = 0.03f;
            base.Valign = Gtk.Align.Start;
            base.Halign = Gtk.Align.Start;
        }

        public bool AutoScroll { get; set; }

        public bool HScroll { get; set; } = false;

        public GtkControlOverride Override { get; set; }

        public bool VScroll { get; set; } = false;

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
        }

        public void Pack(Widget child, Align align, bool expand)
        {
            child.Valign = align;
            child.Halign = align;
            child.Expand = expand;
            base.Add(child);
        }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnPaint(cr, rec);

            return base.OnDrawn(cr);
        }

        protected override void OnShown()
        {
            Override.OnAddClass();
            base.OnShown();
        }

    }
}

using Gtk;
using System;
using System.Linq;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public sealed class TabControlBase : Gtk.Notebook, IControlGtk
    {
        public TabControlBase() : base()
        {
            this.Scrollable = true;
            this.EnablePopup = false;
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("TabControl");
            base.Halign = Gtk.Align.Start;
            base.Valign = Gtk.Align.Start;
        }

        public GtkControlOverride Override { get; set; }

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
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

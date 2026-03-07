using Gdk;
using System;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public abstract class ScrollableBoxBase : Gtk.ScrolledWindow, IControlGtk, IScrollableBoxBase
    {

        public event ScrollEventHandler Scroll;
        public ScrollableBoxBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.ShadowType = Gtk.ShadowType.None;
            this.BorderWidth = 1;
            this.Events = Gdk.EventMask.AllEventsMask;
            base.Halign = Gtk.Align.Start;
            base.Valign = Gtk.Align.Start;
            base.Hexpand = false;
            base.Vexpand = false;
            base.VscrollbarPolicy = Gtk.PolicyType.External;
            base.HscrollbarPolicy = Gtk.PolicyType.External;
            base.OverlayScrolling = false;
            base.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
            base.Vadjustment.ValueChanged += Vadjustment_ValueChanged;
        }
        public virtual bool AutoScroll
        {
            get => base.VscrollbarPolicy == Gtk.PolicyType.Automatic;
            set
            {
                if (value == true)
                {
                    if (VScroll)
                        base.VscrollbarPolicy = Gtk.PolicyType.Automatic;
                    if (HScroll)
                        base.HscrollbarPolicy = Gtk.PolicyType.Automatic;
                }
                else
                {
                    base.VscrollbarPolicy = Gtk.PolicyType.External;
                    base.HscrollbarPolicy = Gtk.PolicyType.External;
                }
            }
        }

        public bool HScroll { get; set; } = true;

        public GtkControlOverride Override { get; set; }

        public bool VScroll { get; set; } = true;

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
        }
        protected virtual Gdk.Rectangle GetDrawRectangle()
        {
            return new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = GetDrawRectangle();
            Override.OnPaint(cr, rec);

            return base.OnDrawn(cr);
        }

        protected override void OnShown()
        {
            Override.OnAddClass();
            base.OnShown();
        }

        private void Hadjustment_ValueChanged(object sender, EventArgs e)
        {
            if (Scroll != null)
            {
                Gtk.Adjustment adj = (Gtk.Adjustment)sender;
                Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.HorizontalScroll));
            }
        }

        private void Vadjustment_ValueChanged(object sender, EventArgs e)
        {
            if (Scroll != null)
            {
                Gtk.Adjustment adj = (Gtk.Adjustment)sender;
                Scroll(this, new System.Windows.Forms.ScrollEventArgs(ScrollEventType.ThumbTrack, (int)(adj.Value > adj.StepIncrement ? (adj.Value - adj.StepIncrement) : adj.Value), (int)adj.Value, ScrollOrientation.VerticalScroll));
            }
        }
    }
}

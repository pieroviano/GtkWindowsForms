using Gtk;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// FormBase is a base class for GTK form controls, providing common functionality and properties for derived classes. It implements the IControlGtk, IScrollableBoxBase, and IWin32Window interfaces, allowing it to integrate with the GTK system and provide consistent behavior across different types of form controls. The FormBase class includes features such as auto-scrolling, event handling for closing the window, and custom drawing capabilities.
    /// </summary>
    /// <seealso cref="Gtk.Dialog" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IScrollableBoxBase" />
    /// <seealso cref="T:System.Windows.Forms.IWin32Window" />
    public sealed class FormBase : Gtk.Dialog, IControlGtk, IScrollableBoxBase, IWin32Window
    {

        public readonly Gtk.ScrolledWindow ScrollView = new Gtk.ScrolledWindow();

        public event CloseWindowHandler CloseWindowEvent;

        public event System.Windows.Forms.ScrollEventHandler Scroll;

        public FormBase(Gtk.Window parent = null) : base("title", Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive), DialogFlags.UseHeaderBar)
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("Form");
            this.WindowPosition = Gtk.WindowPosition.Center;
            this.BorderWidth = 0;
            this.ContentArea.BorderWidth = 0;
            this.ContentArea.Spacing = 0;
            this.ContentArea.Homogeneous = false;

            this.SetDefaultSize(100, 100);
            this.TypeHint = Gdk.WindowTypeHint.Normal;
            this.AppPaintable = false;
            this.Deletable = true;
            this.Response += FormBase_Response;
            ScrollView.BorderWidth = 0;
            ScrollView.Valign = Gtk.Align.Fill;
            ScrollView.Halign = Gtk.Align.Fill;
            ScrollView.Hexpand = true;
            ScrollView.Vexpand = true;
            ScrollView.OverlayScrolling = true;
            ScrollView.KineticScrolling = true;
            ScrollView.HscrollbarPolicy = PolicyType.Automatic;
            ScrollView.VscrollbarPolicy = PolicyType.Automatic;
            ScrollView.Hadjustment.ValueChanged += Hadjustment_ValueChanged;
            ScrollView.Vadjustment.ValueChanged += Vadjustment_ValueChanged;
            this.ContentArea.PackStart(ScrollView, true, true, 0);
            this.Drawn += FormBase_Drawn;
            this.Close += FormBase_Close;
        }

        public bool AutoScroll
        {
            get => ScrollView.VscrollbarPolicy == Gtk.PolicyType.Automatic;
            set
            {
                if (value == true)
                {
                    if (VScroll)
                        ScrollView.VscrollbarPolicy = Gtk.PolicyType.Automatic;
                    if (HScroll)
                        ScrollView.HscrollbarPolicy = Gtk.PolicyType.Automatic;
                }
                else
                {
                    ScrollView.VscrollbarPolicy = Gtk.PolicyType.External;
                    ScrollView.HscrollbarPolicy = Gtk.PolicyType.External;
                }
            }
        }

        public bool HScroll { get; set; } = true;

        public GtkControlOverride Override { get; set; }

        public bool VScroll { get; set; } = true;

        public new void Add(Gtk.Widget child)
        {
            ScrollView.Child = child;
        }

        public void AddClass(string cssClass)
        {
            this.Override.AddClass(cssClass);
        }

        public void CloseWindow()
        {
            this.Respond(ResponseType.DeleteEvent);
        }

        private void FormBase_Close(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this, System.Properties.Resources.Gtk_FormBase_FormBase_Close, "Esc keyboard operation hints", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Respond(ResponseType.DeleteEvent);
            }
        }

        private void FormBase_Drawn(object o, DrawnArgs args)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnPaint(args.Cr, rec);
        }

        private void FormBase_Response(object o, ResponseArgs args)
        {
            if (args.ResponseId == ResponseType.DeleteEvent)
            {
                if (CloseWindowEvent(this, EventArgs.Empty))
                {
                    this.OnClose();
                    this.Group.CurrentGrab?.Destroy();
                    this.Destroy();
                }
                else
                    this.Run();
            }
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

        public delegate bool CloseWindowHandler(object sender, EventArgs e);

    }
}

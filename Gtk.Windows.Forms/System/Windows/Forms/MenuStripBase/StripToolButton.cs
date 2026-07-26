using Gtk;

namespace System.Windows.Forms
{

    public class StripToolButton : Gtk.ToolButton, IToolMenuItem
    {

        private Gdk.Pixbuf _image = null!;

        public StripToolButton() : base(null, "") { this.Homogeneous = false; }

        public bool Checked
        {
            get;
            set;
        }

        public CheckState CheckState
        {
            get;
            set;
        }

        public ToolStripItemDisplayStyle DisplayStyle
        {
            get;
            set;
        }

        public Gdk.Pixbuf Image
        {
            get => _image;
            set
            {
                _image = value;
                this.IconWidget = new Gtk.Image(value) { Visible = true };
            }
        }

        public Gtk.Widget MenuItem { get; } = null!;

        public string Text
        {
            get => base.Label;
            set => base.Label = value;
        }

        public Gtk.Widget ToolItem { get => this; }
    }
}
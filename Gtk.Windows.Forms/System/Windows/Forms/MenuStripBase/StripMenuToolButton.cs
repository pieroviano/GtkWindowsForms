using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuToolButton : Gtk.MenuButton, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        private Gdk.Pixbuf _image = null!;

        public StripMenuToolButton() : base(null, "") { this.Homogeneous = false; }

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
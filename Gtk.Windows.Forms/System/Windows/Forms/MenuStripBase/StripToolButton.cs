using Gtk;

namespace System.Windows.Forms
{

    public class StripToolButton : Gtk.Button, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        private Gdk.Pixbuf _image = null!;

        public StripToolButton() : base() { } // Gtk4: MenuButton/Button ctor takes no (label,?) pair; Homogeneous removed

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
                this.Child = new Gtk.Image(value) { Visible = true }; // Gtk4: Button/MenuButton use Child, not IconWidget
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
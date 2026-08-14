using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuToolButton : Gtk.MenuButton, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        private Gdk.Pixbuf _image = null!;
        private string _text = string.Empty;
        private ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.ImageAndText;

        public StripMenuToolButton() : base()
        {
            // Gtk4: MenuButton/Button ctor takes no (label,?) pair; Homogeneous removed.
            // A custom Child otherwise suppresses the built-in dropdown arrow, so force it to stay.
            this.AlwaysShowArrow = true;
            // Flatten via a direct class on this node (see StripToolButton for why the ancestor selector
            // is not enough); the rule also targets the MenuButton's inner >button node.
            this.StyleContext.AddClass("ToolStripButtonFlat");
        }

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
            get => _displayStyle;
            set
            {
                _displayStyle = value;
                RebuildContent();
            }
        }

        public Gdk.Pixbuf Image
        {
            get => _image;
            set
            {
                _image = value;
                RebuildContent();
            }
        }

        public Gtk.Widget MenuItem { get; } = null!;

        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                RebuildContent();
            }
        }

        public Gtk.Widget ToolItem { get => this; }

        /// <summary>
        /// Gtk4: a Button/MenuButton shows a single Child, so image and text must be composed into one
        /// widget rather than set independently (setting one used to clobber the other). Builds the Child
        /// from the current image/text honouring <see cref="DisplayStyle"/>; falls back to whatever content
        /// is available when the style is unset (None). The dropdown arrow is kept via AlwaysShowArrow.
        /// </summary>
        internal void RebuildContent()
        {
            bool wantImage = _displayStyle != ToolStripItemDisplayStyle.Text && _image != null;
            bool wantText = _displayStyle != ToolStripItemDisplayStyle.Image && !string.IsNullOrEmpty(_text);

            if (wantImage && wantText)
            {
                var box = new Gtk.Box(Gtk.Orientation.Horizontal, 4) { Visible = true };
                box.Append(new Gtk.Image(_image) { Visible = true });
                box.Append(new Gtk.Label(_text) { Visible = true });
                this.Child = box;
            }
            else if (wantImage)
            {
                this.Child = new Gtk.Image(_image) { Visible = true };
            }
            else if (wantText)
            {
                this.Child = new Gtk.Label(_text) { Visible = true };
            }
        }
    }
}

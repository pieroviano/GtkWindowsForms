using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuItem : Gtk.Box, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        public Gtk.CheckButton checkButton = new Gtk.CheckButton() { Visible = true }; // Gtk4: CheckButton has no Relief

        // Gtk4: a CheckButton renders EITHER its Label OR its Child, and assigning Child clears any prior
        // label. The previous code set `checkButton.Child = new Gtk.Image(...)` for an item's icon, which
        // wiped out the text - menu items with an image showed no caption (the "scattered icons, no labels"
        // Gtk4 regression). The button now hosts a small [icon][label] box so both show together, and the
        // check-indicator square is collapsed away by the .MenuItemButton / .MenuBarItem CSS (Gtk3 achieved
        // the same flat look with checkbutton.DrawIndicator = false, which Gtk4 removed).
        private readonly Gtk.Box contentBox = new Gtk.Box(Gtk.Orientation.Horizontal, 4) { Visible = true };

        private readonly Gtk.Image iconImage = new Gtk.Image() { Visible = false };

        private readonly Gtk.Label textLabel = new Gtk.Label() { Visible = false, Halign = Align.Start, Valign = Align.Center };

        private Gdk.Pixbuf _Image = null!;

        public StripMenuItem() : base()
        {
            base.Visible = true;
            checkButton.Halign = Align.Start;
            checkButton.Hexpand = true;
            checkButton.Vexpand = true;
            contentBox.Append(iconImage);
            contentBox.Append(textLabel);
            checkButton.Child = contentBox; // Gtk4: Box.Add -> Append; the button's content is the icon+label box
            base.Append(checkButton); // Gtk4: Box.Add -> Append
        }

        public bool Checked
        {
            // Gtk4 removed CheckButton.DrawIndicator; Checked reflects the toggle state (Active),
            // consistent with CheckState below.
            get => checkButton.Active;
            set => checkButton.Active = value;
        }

        public CheckState CheckState
        {
            get => checkButton.Active == true ? CheckState.Checked : CheckState.Unchecked;
            set => checkButton.Active = value == CheckState.Checked;
        }

        public ToolStripItemDisplayStyle DisplayStyle
        {
            get;
            set;
        }

        public Gdk.Pixbuf Image
        {
            get => _Image;
            set
            {
                _Image = value;
                if (value == null)
                {
                    iconImage.Visible = false;
                }
                else
                {
                    // Update the persistent icon widget instead of replacing the button child, so the
                    // caption Label sitting next to it survives.
                    iconImage.Pixbuf = value;
                    iconImage.Visible = true;
                }
            }
        }

        public Gtk.Widget MenuItem { get => this; }

        public string Text
        {
            get => textLabel.Text;
            set
            {
                textLabel.Text = value ?? string.Empty;
                textLabel.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public Gtk.Widget ToolItem { get; } = null!;

        protected override void Dispose(bool disposing)
        {
            _Image?.Dispose();
            checkButton.Dispose();
            base.Dispose(disposing);
        }
    }
}

using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuItem : Gtk.Box, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        public Gtk.CheckButton checkButton = new Gtk.CheckButton() { Visible = true }; // Gtk4: CheckButton has no Relief

        private Gdk.Pixbuf _Image = null!;

        public StripMenuItem() : base()
        {
            base.Visible = true;
            checkButton.Halign = Align.Start;
            checkButton.Hexpand = true;
            checkButton.Vexpand = true;
            checkButton.Hexpand = true;
            // Gtk4-removed: checkButton.Xalign = 0f;
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
                }
                else
                {
                    checkButton.Child = new Gtk.Image(value);
                }
            }
        }

        public Gtk.Widget MenuItem { get => this; }

        public string Text
        {
            get => checkButton.Label;
            set => checkButton.Label = value;
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
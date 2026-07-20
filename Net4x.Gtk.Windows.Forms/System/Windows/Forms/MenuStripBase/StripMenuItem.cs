using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuItem : Gtk.MenuItem, IToolMenuItem
    {

        public Gtk.CheckButton checkButton = new Gtk.CheckButton() { Visible = true, Relief = ReliefStyle.None };

        private Gdk.Pixbuf _Image;

        public StripMenuItem() : base()
        {
            base.Visible = true;
            checkButton.ImagePosition = PositionType.Left;
            checkButton.Halign = Align.Start;
            checkButton.Expand = true;
            checkButton.Hexpand = true;
            checkButton.Xalign = 0f;
            checkButton.DrawIndicator = false;
            base.Add(checkButton);
        }

        public bool Checked
        {
            get => checkButton.DrawIndicator;
            set => checkButton.DrawIndicator = value;
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
                    checkButton.DrawIndicator = false;
                    checkButton.AlwaysShowImage = false;
                }
                else
                {
                    checkButton.DrawIndicator = false;
                    checkButton.AlwaysShowImage = true;
                    checkButton.Image = new Gtk.Image(value);
                }
            }
        }

        public Gtk.Widget MenuItem { get => this; }

        public string Text
        {
            get => checkButton.Label;
            set => checkButton.Label = value;
        }

        public Gtk.Widget ToolItem { get; }

        protected override void Dispose(bool disposing)
        {
            _Image?.Dispose();
            checkButton.Dispose();
            base.Dispose(disposing);
        }
    }
}
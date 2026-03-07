using Gtk;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Prompts the user to choose a font from among those installed on the local computer.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class FontDialog : CommonDialog
    {

        private Font? _font;

        private static Gtk.Window ActiveWindow = null;
        public FontDialog() : base()
        {
            this.Reset();
        }

        [DefaultValue(true)]
        public virtual bool AllowFullOpen { get; set; }

        public Font? Font { get => _font; set => _font = value; }

        [DefaultValue(false)]

        public virtual bool FullOpen { get; set; }

        [DefaultValue(false)]
        public virtual bool ShowHelp { get; set; }

        protected virtual IntPtr Instance { get; }

        protected virtual int Options { get; }

        public override void Reset()
        {
            _font = null;
        }

        public override string ToString() => $"{_font.Name} {_font.Size}";
        protected override bool RunDialog(IWin32Window owner)
        {
            Gtk.FontChooserDialog fontChooserDialog = null;
            if (owner != null && owner is Form ownerform)
            {
                fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, ownerform.self);
                fontChooserDialog.WindowPosition = Gtk.WindowPosition.CenterOnParent;
            }
            else
            {
                fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, null);
                fontChooserDialog.WindowPosition = Gtk.WindowPosition.Center;
            }
            fontChooserDialog.IconName = "font-x-generic";
            fontChooserDialog.KeepAbove = true;
            if (null != _font)
                fontChooserDialog.Font = _font.Name + " " + (int)_font.Size;
            if (FullOpen && AllowFullOpen)
                fontChooserDialog.Fullscreen();
            int res = fontChooserDialog.Run();
            FontStyle fontStyle = FontStyle.Regular;
            switch (fontChooserDialog.FontDesc.Weight)
            {
                case Pango.Weight.Bold:
                case Pango.Weight.Ultrabold:
                case Pango.Weight.Semibold:
                    fontStyle |= FontStyle.Bold;
                    break;
            }
            switch (fontChooserDialog.FontDesc.Style)
            {
                case Pango.Style.Italic:
                case Pango.Style.Oblique:
                    fontStyle |= FontStyle.Italic;
                    break;
            }
            _font = new Font(fontChooserDialog.FontDesc.Family, (int)(fontChooserDialog.FontDesc.Size / Pango.Scale.PangoScale), fontStyle);

            fontChooserDialog.Dispose();
            fontChooserDialog.Destroy();

            return res == -5;
        }
    }
}

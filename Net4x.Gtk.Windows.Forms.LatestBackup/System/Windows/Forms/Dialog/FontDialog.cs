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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.FontDialog" /> class.
        /// </summary>
        public FontDialog() : base()
        {
            this.Reset();
        }

        [DefaultValue(true)]
        public virtual bool AllowFullOpen { get; set; }


        private Font? _font;
        /// <summary>
        /// Gets or sets the selected font.
        /// </summary>
        /// <returns>The selected font.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Font? Font { get => _font; set => _font = value; }

        [DefaultValue(false)]

        public virtual bool FullOpen { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays a Help button.
        /// </summary>
        /// <returns>true if the dialog box displays a Help button; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool ShowHelp { get; set; }

        protected virtual IntPtr Instance { get; }
        /// <summary>
        /// Gets values to initialize the
        /// <see cref="System.Windows.Forms.FontDialog" />
        /// </summary>
        /// <returns>A bitwise combination of internal values that initializes the
        /// <see cref="System.Windows.Forms.FontDialog" />
        /// </returns>
        protected virtual int Options { get; }
        /// <summary>
        /// Resets all dialog box options to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public override void Reset()
        {
            _font = null;
        }


        private static Gtk.Window ActiveWindow = null;
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
        /// <summary>
        /// Retrieves a string that includes the name of the current font selected in the dialog box.
        /// </summary>
        /// <returns>A string that includes the name of the currently selected font.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString() => $"{_font.Name} {_font.Size}";
    }
}

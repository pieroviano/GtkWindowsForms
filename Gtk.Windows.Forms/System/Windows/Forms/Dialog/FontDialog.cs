using System.ComponentModel;
using System.Drawing;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Prompts the user to choose a font from among those installed on the local computer.
/// </summary>
public class FontDialog : CommonDialog
{

    private Font? _font;

    private static readonly Gtk.Window? ActiveWindow = null;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.FontDialog" /> class.
    /// </summary>
    public FontDialog() : base()
    {
        this.Reset();
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow full open.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow full open; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public virtual bool AllowFullOpen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    public Font? Font
    {
        get => _font;
        set => _font = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether full open.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if full open; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool FullOpen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show help.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show help; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool ShowHelp
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    protected virtual IntPtr Instance { get; }

    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    protected virtual int Options { get; }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public override void Reset()
    {
        _font = null;
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"{_font?.Name} {_font?.Size}";

    /// <summary>
    /// Runs the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    protected override bool RunDialog(IWin32Window? owner)
    {
        FontChooserDialog? fontChooserDialog = null;
        if (owner != null && owner is Form ownerform)
        {
            fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, ownerform.self)
            {
                WindowPosition = 0
            };
        }
        else
        {
            fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, null)
            {
                WindowPosition = 0
            };
        }
        fontChooserDialog.IconName = "font-x-generic";
        if (null != _font)
            fontChooserDialog.Font = _font.Name + " " + (int)_font.Size;
        if (FullOpen && AllowFullOpen)
            fontChooserDialog.Fullscreen();
        var res = fontChooserDialog.Run();
        var fontStyle = FontStyle.Regular;
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
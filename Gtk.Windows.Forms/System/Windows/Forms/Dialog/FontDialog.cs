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
    /// Gets or sets a value indicating whether the dialog offers the strikeout, underline and colour options.
    /// </summary>
    /// <remarks>
    /// GTK's font chooser has no effects section, so this carries the caller's choice without changing the dialog,
    /// as <see cref="AllowFullOpen" /> does.
    /// </remarks>
    [DefaultValue(true)]
    public virtual bool ShowEffects { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog offers a colour choice.</summary>
    [DefaultValue(false)]
    public virtual bool ShowColor { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog offers an Apply button.</summary>
    [DefaultValue(false)]
    public virtual bool ShowApply { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog lists fixed-pitch fonts only.</summary>
    [DefaultValue(false)]
    public virtual bool FixedPitchOnly { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog rejects a font that is not installed.</summary>
    [DefaultValue(true)]
    public virtual bool FontMustExist { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog lists vector fonts.</summary>
    [DefaultValue(true)]
    public virtual bool AllowVectorFonts { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog allows a simulated font style.</summary>
    [DefaultValue(true)]
    public virtual bool AllowSimulations { get; set; }

    /// <summary>Gets or sets a value indicating whether the dialog allows the character set to be changed.</summary>
    [DefaultValue(true)]
    public virtual bool AllowScriptChange { get; set; }

    /// <summary>Gets or sets the colour the dialog reports for the chosen font.</summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>Gets or sets the smallest point size the dialog accepts; 0 for no limit.</summary>
    [DefaultValue(0)]
    public int MinSize { get; set; }

    /// <summary>Gets or sets the largest point size the dialog accepts; 0 for no limit.</summary>
    [DefaultValue(0)]
    public int MaxSize { get; set; }

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
        ShowEffects = true;
        ShowColor = false;
        ShowApply = false;
        FixedPitchOnly = false;
        FontMustExist = true;
        AllowVectorFonts = true;
        AllowSimulations = true;
        AllowScriptChange = true;
        Color = Color.Black;
        MinSize = 0;
        MaxSize = 0;
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
            fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, ownerform.self);
        }
        else
        {
            fontChooserDialog = new Gtk.FontChooserDialog(System.Properties.Resources.Gtk_FontDialog_RunDialog_Select_Font, null);
        }
        fontChooserDialog.IconName = "font-x-generic";
        if (null != _font)
            fontChooserDialog.Font = _font.Name + " " + (int)_font.Size;
        if (FullOpen && AllowFullOpen)
            fontChooserDialog.Fullscreen();
        var res = fontChooserDialog.RunDialog();
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
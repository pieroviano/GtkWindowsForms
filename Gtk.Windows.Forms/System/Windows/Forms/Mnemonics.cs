using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// WinForms marks the mnemonic of a caption with <c>&amp;</c> and writes a literal ampersand as <c>&amp;&amp;</c>;
/// GTK marks it with <c>_</c> and writes a literal underscore as <c>__</c>. Converting between them is what keeps a
/// caption written for WinForms - <c>&amp;Run</c> - underlining its R here instead of showing the ampersand.
/// </summary>
internal static class Mnemonics
{
    /// <summary>
    /// The caption as GTK spells it.
    /// </summary>
    /// <param name="text">The WinForms caption.</param>
    /// <param name="useMnemonic">
    /// Whether <c>&amp;</c> marks a mnemonic. When it does not, WinForms shows the ampersand, and so does this.
    /// </param>
    public static string ToGtk(string? text, bool useMnemonic = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        if (!useMnemonic)
        {
            // Nothing is a marker, so nothing needs escaping; the caller leaves UseUnderline off.
            return text!;
        }

        var result = new StringBuilder(text!.Length + 4);
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '_')
            {
                result.Append("__"); // GTK's own marker, which the caption meant literally
            }
            else if (c != '&')
            {
                result.Append(c);
            }
            else if (i + 1 < text.Length && text[i + 1] == '&')
            {
                result.Append('&'); // "&&" is a literal ampersand
                i++;
            }
            else if (i + 1 < text.Length)
            {
                result.Append('_'); // "&X" marks X
            }
            // a trailing '&' marks nothing, and WinForms drops it
        }
        return result.ToString();
    }

    /// <summary>
    /// The caption as GTK markup whose mnemonic character is explicitly underlined.
    /// </summary>
    /// <param name="text">The WinForms caption, with <c>&amp;</c> markers.</param>
    /// <returns>
    /// Markup for <c>gtk_label_set_markup_with_mnemonic</c>: the first access key is <c>&lt;u&gt;_X&lt;/u&gt;</c>,
    /// a literal <c>&amp;&amp;</c> is <c>&amp;amp;</c>, a literal underscore is <c>__</c>, and a trailing
    /// <c>&amp;</c> is dropped.
    /// </returns>
    /// <remarks>
    /// GTK 4 draws the underline of a mnemonic only while mnemonics are visible (Alt held), whereas WinForms
    /// menus and captions as this library presents them show it at rest. The underline is therefore also
    /// asked for as ordinary markup on the same character. While Alt is held GTK adds its own underline on
    /// the same range, and a Pango attribute of one type over one range draws once, so the two never double up.
    /// Only the first marker keeps the <c>_</c> (GTK honours a single mnemonic per label); later ones are
    /// underlined but are not access keys.
    /// </remarks>
    public static string ToMarkup(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var result = new StringBuilder(text!.Length + 16);
        var haveKey = false;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c != '&')
            {
                AppendEscaped(result, c);
            }
            else if (i + 1 < text.Length && text[i + 1] == '&')
            {
                result.Append("&amp;");
                i++;
            }
            else if (i + 1 < text.Length)
            {
                var next = text[i + 1];
                var length = char.IsHighSurrogate(next) && i + 2 < text.Length && char.IsLowSurrogate(text[i + 2]) ? 2 : 1;
                if (next == '_')
                {
                    result.Append("__"); // GTK cannot mark an underscore, so it stays a literal one
                }
                else
                {
                    result.Append("<u>");
                    if (!haveKey)
                    {
                        result.Append('_');
                        haveKey = true;
                    }

                    for (var k = 0; k < length; k++)
                    {
                        AppendEscaped(result, text[i + 1 + k]);
                    }

                    result.Append("</u>");
                    i += length - 1;
                }

                i++;
            }
            // a trailing '&' marks nothing, and WinForms drops it
        }

        return result.ToString();
    }

    private static void AppendEscaped(StringBuilder result, char c)
    {
        switch (c)
        {
            case '_': result.Append("__"); break;
            case '<': result.Append("&lt;"); break;
            case '>': result.Append("&gt;"); break;
            case '&': result.Append("&amp;"); break;
            default: result.Append(c); break;
        }
    }

    /// <summary>
    /// Shows a WinForms caption on a GTK label: the access key underlined at rest and activatable with Alt.
    /// </summary>
    /// <param name="label">The label that displays the caption.</param>
    /// <param name="text">The WinForms caption.</param>
    /// <param name="useMnemonic">
    /// Whether <c>&amp;</c> marks an access key; when it does not the caption is shown verbatim.
    /// </param>
    /// <remarks>
    /// Every caption-bearing control goes through here. <see cref="Gtk.Label.Text" /> afterwards reports the
    /// displayed text, the markers consumed. Plain <c>set_text</c> would clear the underline flag, so the
    /// caption is always set through the label's markup-with-mnemonic path instead.
    /// </remarks>
    public static void Apply(Gtk.Label label, string? text, bool useMnemonic = true)
    {
        if (useMnemonic)
        {
            label.MarkupWithMnemonic = ToMarkup(text);
        }
        else
        {
            label.Text = text ?? string.Empty; // clears the markup and underline flags
        }
    }
}

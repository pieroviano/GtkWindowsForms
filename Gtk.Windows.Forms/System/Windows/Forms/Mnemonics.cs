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
}

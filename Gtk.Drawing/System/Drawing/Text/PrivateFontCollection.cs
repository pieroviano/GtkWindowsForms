using System.IO;

namespace System.Drawing.Text;

/// <summary>
/// Provides a collection of font families built from font files added by the application.
/// </summary>
/// <remarks>
/// <see cref="M:System.Drawing.Text.PrivateFontCollection.AddFontFile(System.String)" /> records the font
/// as a <see cref="T:System.Drawing.FontFamily" /> so the collection's <c>Families</c> reflects it. On this
/// GTK/Pango layer, actually rendering with a private font also requires that font to be discoverable by
/// fontconfig; registering the file with the system font configuration is outside this shim, so a private
/// font that fontconfig cannot see falls back to a default face when drawn — a documented deviation.
/// </remarks>
public sealed class PrivateFontCollection : FontCollection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Drawing.Text.PrivateFontCollection" /> class.
    /// </summary>
    public PrivateFontCollection()
    {
    }

    /// <summary>
    /// Adds a font from the specified file to this <see cref="T:System.Drawing.Text.PrivateFontCollection" />.
    /// </summary>
    /// <param name="filename">A <see cref="T:System.String" /> that contains the path of the font file to add.</param>
    public void AddFontFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var name = Path.GetFileNameWithoutExtension(filename);
        if (!families.Exists(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            families.Add(new FontFamily(name));
        }
    }

    /// <summary>
    /// Adds a font contained in system memory to this
    /// <see cref="T:System.Drawing.Text.PrivateFontCollection" />.
    /// </summary>
    /// <param name="memory">The address of the font to add.</param>
    /// <param name="length">The number of bytes in the font data.</param>
    /// <remarks>
    /// Registering an in-memory font requires the platform GDI+/native font APIs, which are not available on
    /// the GTK layer; this call is a no-op. Ship the font as a file and use
    /// <see cref="M:System.Drawing.Text.PrivateFontCollection.AddFontFile(System.String)" /> instead.
    /// </remarks>
    public void AddMemoryFont(IntPtr memory, int length)
    {
    }
}

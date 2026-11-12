using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
        try
        {
            if (File.Exists(filename) && TryGetFamilyName(File.ReadAllBytes(filename), out var parsed))
            {
                name = parsed;
            }
        }
        catch (IOException)
        {
            // Fall back to the file name if the font cannot be read.
        }

        AddFamily(name);
    }

    /// <summary>
    /// Adds a font contained in system memory to this
    /// <see cref="T:System.Drawing.Text.PrivateFontCollection" />.
    /// </summary>
    /// <param name="memory">The address of the font to add.</param>
    /// <param name="length">The number of bytes in the font data.</param>
    /// <remarks>
    /// The font's own family name is read from its OpenType <c>name</c> table and recorded as a
    /// <see cref="T:System.Drawing.FontFamily" />, so <c>Families</c> reflects the added font. Rendering still
    /// requires the face to be discoverable by fontconfig (see the type remarks); when it is not, drawing
    /// falls back to a default face.
    /// </remarks>
    public void AddMemoryFont(IntPtr memory, int length)
    {
        if (memory == IntPtr.Zero || length <= 0)
        {
            return;
        }

        var data = new byte[length];
        Marshal.Copy(memory, data, 0, length);

        var name = TryGetFamilyName(data, out var parsed) ? parsed : FontFamily.GenericSansSerif.Name;
        AddFamily(name);
    }

    private void AddFamily(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (!families.Exists(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            families.Add(new FontFamily(name));
        }
    }

    // Reads the Font Family name (name ID 1) from an sfnt (TrueType/OpenType) font's `name` table. All
    // sfnt integers are big-endian. Parsing is fully bounds-checked: malformed data yields false rather
    // than throwing, so a bad font never crashes the caller.
    private static bool TryGetFamilyName(byte[] d, out string? name)
    {
        name = null;
        if (d.Length < 12)
        {
            return false;
        }

        var baseOffset = 0;
        if (ReadUInt32(d, 0) == 0x74746366) // 'ttcf' — a TrueType collection; use its first font.
        {
            if (d.Length < 16)
            {
                return false;
            }

            baseOffset = (int)ReadUInt32(d, 12);
        }

        if (baseOffset < 0 || baseOffset + 6 > d.Length)
        {
            return false;
        }

        int numTables = ReadUInt16(d, baseOffset + 4);
        var nameTableOffset = -1;
        for (var i = 0; i < numTables; i++)
        {
            var record = baseOffset + 12 + i * 16;
            if (record + 16 > d.Length)
            {
                return false;
            }

            if (ReadUInt32(d, record) == 0x6E616D65) // 'name'
            {
                nameTableOffset = (int)ReadUInt32(d, record + 8);
                break;
            }
        }

        if (nameTableOffset < 0 || nameTableOffset + 6 > d.Length)
        {
            return false;
        }

        int count = ReadUInt16(d, nameTableOffset + 2);
        var stringBase = nameTableOffset + ReadUInt16(d, nameTableOffset + 4);
        var bestScore = -1;
        for (var i = 0; i < count; i++)
        {
            var record = nameTableOffset + 6 + i * 12;
            if (record + 12 > d.Length)
            {
                break;
            }

            int platformId = ReadUInt16(d, record);
            int encodingId = ReadUInt16(d, record + 2);
            int languageId = ReadUInt16(d, record + 4);
            int nameId = ReadUInt16(d, record + 6);
            int len = ReadUInt16(d, record + 8);
            var offset = stringBase + ReadUInt16(d, record + 10);
            if (nameId != 1 || len <= 0 || offset < 0 || offset + len > d.Length)
            {
                continue;
            }

            // Windows (3) and Unicode (0) platforms store UTF-16BE; Macintosh (1) stores single-byte text.
            var text = platformId is 3 or 0
                ? Encoding.BigEndianUnicode.GetString(d, offset, len)
                : Encoding.ASCII.GetString(d, offset, len);

            var score = (platformId == 3 && encodingId == 1 ? 2 : 0) + (languageId is 0 or 0x409 ? 1 : 0);
            if (score > bestScore && !string.IsNullOrWhiteSpace(text))
            {
                bestScore = score;
                name = text;
            }
        }

        return !string.IsNullOrWhiteSpace(name);
    }

    private static ushort ReadUInt16(byte[] d, int pos) => (ushort)((d[pos] << 8) | d[pos + 1]);

    private static uint ReadUInt32(byte[] d, int pos) =>
        (uint)((d[pos] << 24) | (d[pos + 1] << 16) | (d[pos + 2] << 8) | d[pos + 3]);
}

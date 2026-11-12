extern alias gtk;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using gtk::System.Drawing.Text;

namespace System.Drawing;

/// <summary>
/// Verifies the font collections restored to the Drawing shim: <see cref="PrivateFontCollection" /> (an
/// app-supplied set) and <see cref="InstalledFontCollection" /> (the system fonts, enumerated from Pango).
/// </summary>
[Collection("NonParallelCollection")]
public class FontCollectionTests
{
    [Fact]
    public void Collections_DeriveFromFontCollection()
    {
        Assert.True(typeof(FontCollection).IsAssignableFrom(typeof(PrivateFontCollection)));
        Assert.True(typeof(FontCollection).IsAssignableFrom(typeof(InstalledFontCollection)));
    }

    [Fact]
    public void PrivateFontCollection_AddFontFile_AddsFamily()
    {
        using var pfc = new PrivateFontCollection();
        Assert.NotNull(pfc.Families);
        Assert.Empty(pfc.Families!);

        pfc.AddFontFile(@"/fonts/MyCustomFont.ttf");
        Assert.Single(pfc.Families!);
        Assert.Equal("MyCustomFont", pfc.Families![0].Name);

        // adding the same file again does not duplicate the family
        pfc.AddFontFile(@"/other/MyCustomFont.ttf");
        Assert.Single(pfc.Families!);

        pfc.AddFontFile(@"/fonts/Another.otf");
        Assert.Equal(2, pfc.Families!.Length);
    }

    [Fact]
    public void PrivateFontCollection_AddFontFile_Null_Throws()
    {
        using var pfc = new PrivateFontCollection();
#pragma warning disable CS8625
        Assert.Throws<ArgumentNullException>(() => pfc.AddFontFile(null));
#pragma warning restore CS8625
    }

    [Fact]
    public void PrivateFontCollection_AddMemoryFont_NullOrEmpty_AddsNothing()
    {
        using var pfc = new PrivateFontCollection();
        pfc.AddMemoryFont(IntPtr.Zero, 0);
        Assert.Empty(pfc.Families!);
    }

    // Regression: AddMemoryFont was a no-op, so Families stayed empty and callers doing Families[0]
    // (e.g. loading a bundled font from a resource) threw "Index was outside the bounds of the array."
    [Fact]
    public void PrivateFontCollection_AddMemoryFont_RegistersParsedFamilyName()
    {
        var font = BuildMinimalSfnt("GtkTestFamily");
        var ptr = Marshal.AllocHGlobal(font.Length);
        try
        {
            Marshal.Copy(font, 0, ptr, font.Length);
            using var pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptr, font.Length);

            Assert.Single(pfc.Families!);
            Assert.Equal("GtkTestFamily", pfc.Families![0].Name);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    [Fact]
    public void InstalledFontCollection_Families_IsNonNull()
    {
        using var ifc = new InstalledFontCollection();
        // Enumerates the Pango font map; the array is always non-null (usually non-empty on a real system).
        Assert.NotNull(ifc.Families);
    }

    // Builds the smallest sfnt the family-name reader understands: an offset table, one `name` table
    // record, and a `name` table carrying the Font Family (name ID 1) as a Windows UTF-16BE string.
    private static byte[] BuildMinimalSfnt(string family)
    {
        var value = Encoding.BigEndianUnicode.GetBytes(family);
        var name = new List<byte>();
        AddU16(name, 0);                 // format
        AddU16(name, 1);                 // count
        AddU16(name, 6 + 12);            // stringOffset (relative to name table start)
        AddU16(name, 3);                 // platformID = Windows
        AddU16(name, 1);                 // encodingID = Unicode BMP
        AddU16(name, 0x409);             // languageID = en-US
        AddU16(name, 1);                 // nameID = Font Family
        AddU16(name, value.Length);      // length
        AddU16(name, 0);                 // offset (within string storage)
        name.AddRange(value);

        const int nameTableOffset = 12 + 16;
        var font = new List<byte>();
        AddU32(font, 0x00010000);        // sfnt version
        AddU16(font, 1);                 // numTables
        AddU16(font, 0); AddU16(font, 0); AddU16(font, 0); // search hints (unused by the reader)
        font.AddRange(Encoding.ASCII.GetBytes("name"));
        AddU32(font, 0);                 // checksum
        AddU32(font, (uint)nameTableOffset);
        AddU32(font, (uint)name.Count);
        font.AddRange(name);
        return font.ToArray();
    }

    private static void AddU16(List<byte> b, int v) { b.Add((byte)(v >> 8)); b.Add((byte)v); }

    private static void AddU32(List<byte> b, uint v)
    {
        b.Add((byte)(v >> 24)); b.Add((byte)(v >> 16)); b.Add((byte)(v >> 8)); b.Add((byte)v);
    }
}

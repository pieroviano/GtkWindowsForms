extern alias gtk;
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
    public void PrivateFontCollection_AddMemoryFont_DoesNotThrow_AndAddsNothing()
    {
        using var pfc = new PrivateFontCollection();
        pfc.AddMemoryFont(IntPtr.Zero, 0); // documented no-op on GTK
        Assert.Empty(pfc.Families!);
    }

    [Fact]
    public void InstalledFontCollection_Families_IsNonNull()
    {
        using var ifc = new InstalledFontCollection();
        // Enumerates the Pango font map; the array is always non-null (usually non-empty on a real system).
        Assert.NotNull(ifc.Families);
    }
}

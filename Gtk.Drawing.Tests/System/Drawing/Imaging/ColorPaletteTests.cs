extern alias gtk;
using System.Reflection;
using System.Runtime.InteropServices;
using ColorPalette = gtk::System.Drawing.Imaging.ColorPalette;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.

namespace System.Drawing.Imaging;

public class ColorPaletteTests
{
    #region Constructors

    [Fact]
    public void DefaultConstructor_CreatesSingleEntry()
    {
        var palette = CreatePalette();

        Assert.NotNull(palette.Entries);
        Assert.Single(palette.Entries);
    }

    [Fact]
    public void Constructor_WithCount_CreatesCorrectSize()
    {
        var palette = CreatePalette(5);

        Assert.Equal(5, palette.Entries.Length);
    }

    #endregion

    #region Entries

    [Fact]
    public void Entries_CanBeModified()
    {
        var palette = CreatePalette(2);

        palette.Entries[0] = Color.Red;
        palette.Entries[1] = Color.Blue;

        Assert.Equal(Color.Red, palette.Entries[0]);
        Assert.Equal(Color.Blue, palette.Entries[1]);
    }

    #endregion

    #region Flags

    [Fact]
    public void Flags_Default_IsZero()
    {
        var palette = CreatePalette();

        Assert.Equal(0, palette.Flags);
    }

    #endregion

    #region ConvertToMemory

    [Fact]
    public void ConvertToMemory_WritesCorrectData()
    {
        var palette = CreatePalette(2);
        palette.Entries[0] = Color.Red;
        palette.Entries[1] = Color.Green;

        IntPtr ptr = palette.ConvertToMemory();

        try
        {
            int flags = Marshal.ReadInt32(ptr);
            int count = Marshal.ReadInt32(ptr + 4);

            Assert.Equal(0, flags);
            Assert.Equal(2, count);

            int color0 = Marshal.ReadInt32(ptr + 8);
            int color1 = Marshal.ReadInt32(ptr + 12);

            Assert.Equal(Color.Red.ToArgb(), color0);
            Assert.Equal(Color.Green.ToArgb(), color1);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    #endregion

    #region ConvertFromMemory

    [Fact]
    public void ConvertFromMemory_ReadsCorrectData()
    {
        int count = 2;
        IntPtr ptr = Marshal.AllocHGlobal(4 * (2 + count));

        try
        {
            Marshal.WriteInt32(ptr, 0, 123); // flags
            Marshal.WriteInt32(ptr + 4, 0, count);

            Marshal.WriteInt32(ptr + 8, 0, Color.Blue.ToArgb());
            Marshal.WriteInt32(ptr + 12, 0, Color.Yellow.ToArgb());

            var palette = CreatePalette();

            palette.ConvertFromMemory(ptr);

            Assert.Equal(123, palette.Flags);
            Assert.Equal(2, palette.Entries.Length);
            Assert.Equal(Color.Blue.ToArgb(), palette.Entries[0].ToArgb());
            Assert.Equal(Color.Yellow.ToArgb(), palette.Entries[1].ToArgb());
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    #endregion

    #region RoundTrip

    [Fact]
    public void ConvertToMemory_Then_FromMemory_RoundTrip()
    {
        var original = CreatePalette(3);
        original.Entries[0] = Color.Red;
        original.Entries[1] = Color.Green;
        original.Entries[2] = Color.Blue;

        IntPtr ptr = original.ConvertToMemory();

        try
        {
            var copy = CreatePalette();

            copy.ConvertFromMemory(ptr);

            Assert.Equal(original.Entries.Length, copy.Entries.Length);

            for (int i = 0; i < original.Entries.Length; i++)
            {
                Assert.Equal(original.Entries[i].ToArgb(), copy.Entries[i].ToArgb());
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    #endregion

    #region Helpers

    private ColorPalette CreatePalette()
    {
        return (ColorPalette)Activator.CreateInstance(
            typeof(ColorPalette),
            nonPublic: true);
    }

    private ColorPalette CreatePalette(int count)
    {
        return (ColorPalette)Activator.CreateInstance(
            typeof(ColorPalette),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { count },
            culture: null);
    }

    #endregion
}
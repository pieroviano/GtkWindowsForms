extern alias gtk;

using Icon = gtk::System.Drawing.Icon;
using IcoImageDecoder = gtk::System.Drawing.IcoImageDecoder;

namespace System.Drawing;

[Collection("NonParallelCollection")]
public class IconTests
{
    [Fact]
    public void IcoImageDecoder_DecodesTestIco_WithoutTheNativeLoader()
    {
        // Regression: the Gtk4 runtime often has no gdk-pixbuf ICO loader, so new Gdk.Pixbuf(icoBytes) throws
        // "Unrecognized image file format" and every .ico (icons, cursors, designer resources) failed to
        // load. The managed decoder must turn the .ico into a pixbuf regardless of the native loader.
        var bytes = File.ReadAllBytes("test.ico");

        var pixbuf = IcoImageDecoder.TryDecode(bytes);

        Assert.NotNull(pixbuf);
        Assert.Equal(16, pixbuf!.Width);
        Assert.Equal(16, pixbuf.Height);
        Assert.True(pixbuf.HasAlpha, "A 32bpp .ico frame carries an alpha channel.");
    }

    [Fact]
    public void Clone_ReturnsNewIcon()
    {
        var icon = new Icon("test.ico");
        var clone = (Icon)icon.Clone();
        Assert.NotSame(icon, clone);
        Assert.Equal(icon.Width, clone.Width);
        Assert.Equal(icon.Height, clone.Height);
    }

    [Fact]
    public void Constructor_ByteArray_CreatesIcon()
    {
        // Arrange
        var bytes = File.ReadAllBytes("test.ico"); // Ensure test.ico exists in test output

        // Act
        var icon = new Icon(bytes);

        // Assert
        Assert.NotNull(icon.Pixbuf);
        Assert.True(icon.Width > 0);
        Assert.True(icon.Height > 0);
    }

    [Fact]
    public void Constructor_FileName_CreatesIcon()
    {
        var icon = new Icon("test.ico");
        Assert.NotNull(icon.Pixbuf);
    }

    [Fact]
    public void Constructor_Stream_CreatesIcon()
    {
        using var stream = File.OpenRead("test.ico");
        var icon = new Icon(stream);

        Assert.NotNull(icon.Pixbuf);
    }

    [Fact]
    public void Constructor_Stream_WithUndecodableData_DoesNotThrow_AndKeepsRawBytes()
    {
        // Regression: GdkPixbuf on some Gtk4 runtimes has no loader for the Windows .ico format that
        // WinForms designer icon resources ship as, so new Gdk.Pixbuf(bytes) throws GLib.GException
        // 'Unrecognized image file format'. The Icon(Stream) ctor used to let that escape, crashing
        // Form.InitializeComponent (repro: GtkWinFormsApp/Form3 loading its Panel.ico fallback). It must
        // keep the raw bytes and leave the pixbuf empty instead, matching Bitmap(Stream).
        var undecodable = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
        using var stream = new MemoryStream(undecodable);

        Icon icon = null!;
        var exception = Record.Exception(() => icon = new Icon(stream));

        Assert.Null(exception);
        Assert.NotNull(icon);
        Assert.Null(icon.Pixbuf);
        Assert.NotNull(icon.PixbufData);
    }

    [Fact]
    public void Dispose_ClearsPixbuf()
    {
        var icon = new Icon("test.ico");
        icon.Dispose();
        Assert.Null(icon.PixbufData);
        Assert.Null(icon.Pixbuf);
    }

    [Fact]
    public void Save_WritesData()
    {
        var icon = new Icon("test.ico");
        using var ms = new MemoryStream();
        icon.Save(ms);
        Assert.True(ms.Length > 0);
    }
}
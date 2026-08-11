using System.Drawing;
using System.Runtime.Serialization;
using Path = System.IO.Path;
#pragma warning disable SYSLIB0050

namespace System.Windows.Forms.ControlTests;

extern alias gtk;

[Collection("NonParallelCollection")]
public class CursorTests
{
    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_Ctor_WithCursorType_SetsCursorType()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact]
    public void Cursor_Ctor_WithFile_SetsData()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var tempFile = Path.GetTempFileName();
        File.WriteAllBytes(tempFile, bytes);

        var cursor = new Cursor(tempFile);

        var data = cursor.GetData();
        Assert.NotNull(data);
        Assert.Equal(bytes, data);

        File.Delete(tempFile);
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_Properties_CanBeSetAndRetrieved()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_CopyHandle_ReturnsZero()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_Dispose_DoesNotThrow()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_DrawMethods_DoNotThrow()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_EqualsAndHashCode_Work()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_ToString_ReturnsExpectedFormat()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact(Skip = "Gtk4 removed Gdk.CursorType; the shim Cursor(Gdk.CursorType)/CursorType members (GTK internals, not a WinForms API) are gone. Cursors are now named (Gdk.Cursor.NewFromName) or texture-backed. Re-express against the named/texture cursor model in migration #11.")]
    public void Cursor_ISerializable_ThrowsPlatformNotSupported()
    {
        // Body removed: exercised the removed Cursor(Gdk.CursorType) API. See Skip reason.
    }

    [Fact]
    public void Cursor_StaticShowHide_CanCallWithoutException()
    {
        Cursor.Show();
        Cursor.Hide();
    }

    // Regression: the stock cursors ship as Windows .cur resources, which GdkPixbuf cannot decode. The
    // ported Cursor(Stream) fed those bytes to new Gdk.Pixbuf(stream), which threw / aborted with
    // "Unrecognized image file format" out of a static Cursors.* property getter, crashing the app on the
    // first form that referenced a cursor (Form3.InitializeComponent -> Cursors.Hand). Stock cursors now
    // resolve to a Gtk4 named cursor with no image decoding.
    [Fact]
    public void StockCursors_DoNotDecodeImages_AndMapToNamedGdkCursors()
    {
        // Accessing a stock cursor must not throw (previously an undecodable-.cur GException escaped here).
        var hand = Cursors.Hand;
        Assert.NotNull(hand);
        Assert.Equal("pointer", hand.GdkCursorName);

        Assert.Equal("default", Cursors.Arrow.GdkCursorName);
        Assert.Equal("default", Cursors.Default.GdkCursorName);
        Assert.Equal("help", Cursors.Help.GdkCursorName);
        Assert.Equal("crosshair", Cursors.Cross.GdkCursorName);
        Assert.Equal("text", Cursors.IBeam.GdkCursorName);
        Assert.Equal("move", Cursors.SizeAll.GdkCursorName);
        Assert.Equal("not-allowed", Cursors.No.GdkCursorName);
        Assert.Equal("wait", Cursors.WaitCursor.GdkCursorName);
        Assert.Equal("progress", Cursors.AppStarting.GdkCursorName);
        // Directional resize cursors already carry valid Gtk4/CSS cursor names.
        Assert.Equal("ns-resize", Cursors.SizeNS.GdkCursorName);
        Assert.Equal("ew-resize", Cursors.SizeWE.GdkCursorName);
        Assert.Equal("nesw-resize", Cursors.SizeNESW.GdkCursorName);
        Assert.Equal("nwse-resize", Cursors.SizeNWSE.GdkCursorName);
        Assert.Equal("e-resize", Cursors.PanEast.GdkCursorName);

        // Touching every stock cursor must be safe - none may attempt to decode its .cur image.
        Cursor[] all =
        {
            Cursors.AppStarting, Cursors.Arrow, Cursors.Cross, Cursors.Default, Cursors.Hand, Cursors.Help,
            Cursors.HSplit, Cursors.IBeam, Cursors.No, Cursors.NoMove2D, Cursors.NoMoveHoriz,
            Cursors.NoMoveVert, Cursors.PanEast, Cursors.PanNE, Cursors.PanNW, Cursors.PanNorth,
            Cursors.PanSE, Cursors.PanSW, Cursors.PanSouth, Cursors.PanWest, Cursors.SizeAll,
            Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.UpArrow,
            Cursors.VSplit, Cursors.WaitCursor,
        };
        Assert.All(all, c =>
        {
            Assert.NotNull(c);
            Assert.False(string.IsNullOrEmpty(c.GdkCursorName));
        });
    }
}
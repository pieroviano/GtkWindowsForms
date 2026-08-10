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
}
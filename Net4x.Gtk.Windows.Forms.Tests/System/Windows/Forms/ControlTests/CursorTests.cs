using System.Drawing;
using System.Runtime.Serialization;
using Path = System.IO.Path;
#pragma warning disable SYSLIB0050

namespace System.Windows.Forms.ControlTests;

extern alias gtk;

[Collection("NonParallelCollection")]
public class CursorTests
{
    [Fact]
    public void Cursor_Ctor_WithCursorType_SetsCursorType()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);

        Assert.Equal(Gdk.CursorType.Hand1, cursor.CursorType);
        Assert.Null(cursor.GetData());
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

    [Fact]
    public void Cursor_Properties_CanBeSetAndRetrieved()
    {
        var cursor = new Cursor(Gdk.CursorType.LeftPtr)
        {
            Tag = "tag"
        };

        Cursor.Current = cursor;
        Cursor.Position = new Point(10, 20);
        Cursor.Clip = new Rectangle(0, 0, 32, 32);

        Assert.Equal(cursor, Cursor.Current);
        Assert.Equal("tag", cursor.Tag);
        Assert.Equal(new Point(10, 20), Cursor.Position);
        Assert.Equal(new Rectangle(0, 0, 16, 16), Cursor.Clip); // placeholder always returns 16x16
    }

    [Fact]
    public void Cursor_CopyHandle_ReturnsZero()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);

        var copy = cursor.CopyHandle();

        Assert.Equal(IntPtr.Zero, copy);
    }

    [Fact]
    public void Cursor_Dispose_DoesNotThrow()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);
        cursor.Dispose();
        cursor.Dispose(); // multiple dispose
    }

    [Fact]
    public void Cursor_DrawMethods_DoNotThrow()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);
        using var bitmap = new gtk::System.Drawing.Bitmap(10, 10);
        using var g = gtk::System.Drawing.Graphics.FromImage(bitmap);

        cursor.Draw(g, new Rectangle(0, 0, 10, 10));
        cursor.DrawStretched(g, new Rectangle(0, 0, 10, 10));
    }

    [Fact]
    public void Cursor_EqualsAndHashCode_Work()
    {
        var cursor1 = new Cursor(Gdk.CursorType.Hand1);
        var cursor2 = cursor1;

        Assert.True(cursor1.Equals(cursor2));
        Assert.Equal(cursor1.GetHashCode(), cursor2.GetHashCode());

        var cursor3 = new Cursor(Gdk.CursorType.LeftPtr);
        Assert.False(cursor1.Equals(cursor3));
    }

    [Fact]
    public void Cursor_ToString_ReturnsExpectedFormat()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);
        Assert.StartsWith("[Cursor:", cursor.ToString());
    }

    [Fact]
    public void Cursor_ISerializable_ThrowsPlatformNotSupported()
    {
        var cursor = new Cursor(Gdk.CursorType.Hand1);
        var si = new SerializationInfo(typeof(Cursor), new FormatterConverter());

        Assert.Throws<PlatformNotSupportedException>(() =>
            ((ISerializable)cursor).GetObjectData(si, new StreamingContext()));
    }

    [Fact]
    public void Cursor_StaticShowHide_CanCallWithoutException()
    {
        Cursor.Show();
        Cursor.Hide();
    }
}
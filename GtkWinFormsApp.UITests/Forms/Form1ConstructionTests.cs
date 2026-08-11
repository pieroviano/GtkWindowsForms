using GtkWinFormsApp;
using Application = System.Windows.Forms.Application;

namespace GtkWinFormsApp.UITests.Forms;

/// <summary>
/// Form1 is the "main window" opened by Form3's <c>button1_Click</c> ("Open Main Window" ->
/// <c>new Form1().ShowDialog(this)</c>). These construct it directly (no UI driver / GApplication
/// registration needed), standing in for the crash the user hit on that click.
/// </summary>
[Collection("NonParallelCollection")]
public class Form1ConstructionTests
{
    [Fact]
    public void Constructing_Form1_DoesNotThrow()
    {
        // Regression: Form1 hosts a DateTimePicker, whose ctor loaded MonthCalendar.ico via
        // new Gdk.Pixbuf(assembly, resource). GdkPixbuf has no .ico loader on this runtime, so
        // PixbufLoader.Close threw GLib.GException "Unrecognized image file format" - which escaped
        // InitializeComponent and crashed the app the moment "Open Main Window" was clicked. The icon load
        // must degrade to the named "open-menu" icon instead of throwing.
        Application.Init();

        var ex = Record.Exception(() => new Form1());

        Assert.Null(ex);
    }
}

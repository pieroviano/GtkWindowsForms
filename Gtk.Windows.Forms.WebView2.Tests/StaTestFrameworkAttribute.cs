// This assembly constructs GTK widgets (WebView2Backend owns a Gtk.DrawingArea), so it needs the same
// dedicated STA thread the main test assembly uses: GDK 4 calls OleInitialize on Windows, which fails on
// the MTA thread-pool threads the default xUnit host provides, and the failure surfaces as a GLib fatal
// error box rather than a test assertion.
[assembly: Xunit.TestFramework("GtkWinForms.Tests.StaTestFramework", "Gtk.Windows.Forms.WebView2.Tests")]

// This assembly constructs GTK widgets (InternetExplorerBackend owns a Gtk.DrawingArea), so it needs the
// same dedicated STA thread the main test assembly uses: GDK 4 calls OleInitialize on Windows, which fails
// on the MTA thread-pool threads the default xUnit host provides, and - for this backend especially -
// hosting the WebBrowser ActiveX control requires an STA. The failure would otherwise surface as a GLib
// fatal error box rather than a test assertion.
[assembly: Xunit.TestFramework("GtkWinForms.Tests.StaTestFramework", "Gtk.Windows.Forms.InternetExplorer.Tests")]

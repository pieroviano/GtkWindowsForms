// Activates the shared STA test framework for this assembly. Separate from StaTestFramework.cs because
// the attribute's second argument is the *declaring* assembly's name, and that file is linked into every
// test project that constructs GTK widgets.
[assembly: Xunit.TestFramework("GtkWinForms.Tests.StaTestFramework", "Gtk.Windows.Forms.Tests")]

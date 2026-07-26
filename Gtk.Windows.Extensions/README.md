Gtk.Windows.Extensions

Additions to the `System.Windows.Forms` surface provided by `Gtk.Windows.Forms` that are **not** part
of the real WinForms API. They live in a separate package so the core library stays a faithful
WinForms compatibility layer; add your own extension classes here.

## AutoClosingMessageBox

An `IMessageBox` that shows a message box and disposes the underlying GTK dialog after a timeout —
useful for unattended/automated runs where no one is there to click OK.

```csharp
using System.Windows.Forms;

// Install behind the static MessageBox helpers: every MessageBox.Show(...) now
// auto-closes after 500 ms.
AutoClosingMessageBox.Install(500);
MessageBox.Show("Auto closing");

// Restore the normal, blocking message box.
MessageBox.ReplaceInstance();
```

`MessageBox.ReplaceInstance(int)` in `Gtk.Windows.Forms` used to install this type; it is now obsolete
and always installs the default blocking message box. Call `AutoClosingMessageBox.Install(timeout)`
instead.

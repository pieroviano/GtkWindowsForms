This is a tool accompanying the Gtk.Windows.Forms framework project, 
used to open the Visual Studio form designer.

A GTK project targets a plain `netX.0` moniker with `UseWindowsForms` off, so the SDK writes it a
designer runtimeconfig naming `Microsoft.NETCore.App` alone. Visual Studio's out-of-process designer
server is a real Windows Forms process: started that way it cannot load `System.Windows.Forms` and
Visual Studio reports a `ServerProcess` failure. These targets add `Microsoft.WindowsDesktop.App` to
`obj\<tfm>\<AssemblyName>.designer.runtimeconfig.json`, and empty the matching `.designer.deps.json`
so the server resolves Windows Forms from the shared framework rather than from the GTK
implementation the project compiles against.

`Gtk.Windows.Forms.Base` depends on this package, so referencing that one is enough. Mark either
generated file read-only to keep a version of your own.

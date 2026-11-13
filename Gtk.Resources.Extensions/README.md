# Gtk.Resources.Extensions

**This package ships an assembly that is intentionally empty.** That is the whole point of it — it is
not a stub waiting to be filled in, and nothing is missing.

## Why an empty assembly

`Gtk.Resources.Extensions` is part of
[Gtk.Windows.Forms](https://www.nuget.org/packages/Gtk.Windows.Forms), a WinForms-compatibility layer
implemented on top of GTK so existing C# WinForms apps compile once and run on Windows, Linux and
macOS.

WinForms designer files lean heavily on `.resx` resources (images, icons, layout data), historically
read through `System.Resources.Extensions`, whose implementation is built on `BinaryFormatter`.
`BinaryFormatter` is obsolete, is disabled by default on modern .NET, and its resource reader is not
what this stack uses.

So the assembly in this package carries **no code**, and its `AssemblyName` is literally
`System.Resources.Extensions`. Dropped into an application's output folder it *displaces* Microsoft's
assembly of that name, so nothing can bind to the `BinaryFormatter`-based implementation.

The working reader is not missing — it lives in the `Gtk.Windows.Forms` library itself, under
`GtkSystem/Resources/Extensions/`, in namespace `Gtk.Resources.Extensions`
(`DeserializingResourceReader` and friends). Resource type resolution in consuming projects is pointed
at those types by the MSBuild props/targets and `content/` source files that `Gtk.Windows.Forms`
injects. Nothing needs the displaced types, which is exactly why displacing them is safe.

## What you have to do

Reference the package from your application (the sample apps in the repository do this) so the empty
assembly lands in the output folder.

One caveat: **displacement only takes effect if this assembly wins MSBuild's conflict resolution**
against Microsoft's `System.Resources.Extensions` NuGet package. If something in your dependency graph
pulls that package in and wins, Microsoft's implementation is what ends up in the output folder and
this package has no effect.

## Links

- Source and issues: <https://bitbucket.org/piero-viano/net4x.gtk.windows.forms>
- Main package: <https://www.nuget.org/packages/Gtk.Windows.Forms>

This project is a maintained fork of
[GTKSystem.Windows.Forms](https://github.com/easywebfactory/gtksystem-windows-forms) (MIT).

using System.Runtime.InteropServices;

namespace Gtk.Windows.Forms.Controls.ControlBase;

internal class GLibrary
{

    private static readonly Dictionary<string, IntPtr> _customlibraries;

    private static readonly Dictionary<Library, IntPtr> _libraries;

    private static readonly Dictionary<Library, string[]> _libraryDefinitions;

    static GLibrary()
    {
        _customlibraries = new Dictionary<string, IntPtr>();
        _libraries = new Dictionary<Library, IntPtr>();
        _libraryDefinitions = new Dictionary<Library, string[]>
        {
            [Library.GLib] = new string[4] { "libglib-2.0-0.dll", "libglib-2.0.so.0", "libglib-2.0.0.dylib", "glib-2.dll" },
            [Library.GObject] = new string[4] { "libgobject-2.0-0.dll", "libgobject-2.0.so.0", "libgobject-2.0.0.dylib", "gobject-2.dll" },
            [Library.Cairo] = new string[4] { "libcairo-2.dll", "libcairo.so.2", "libcairo.2.dylib", "cairo.dll" },
            [Library.Gio] = new string[4] { "libgio-2.0-0.dll", "libgio-2.0.so.0", "libgio-2.0.0.dylib", "gio-2.dll" },
            [Library.Atk] = new string[4] { "libatk-1.0-0.dll", "libatk-1.0.so.0", "libatk-1.0.0.dylib", "atk-1.dll" },
            [Library.Pango] = new string[4] { "libpango-1.0-0.dll", "libpango-1.0.so.0", "libpango-1.0.0.dylib", "pango-1.dll" },
            [Library.Gdk] = new string[4] { "libgdk-3-0.dll", "libgdk-3.so.0", "libgdk-3.0.dylib", "gdk-3.dll" },
            [Library.GdkPixbuf] = new string[4] { "libgdk_pixbuf-2.0-0.dll", "libgdk_pixbuf-2.0.so.0", "libgdk_pixbuf-2.0.dylib", "gdk_pixbuf-2.dll" },
            [Library.Gtk] = new string[4] { "libgtk-3-0.dll", "libgtk-3.so.0", "libgtk-3.0.dylib", "gtk-3.dll" },
            [Library.PangoCairo] = new string[4] { "libpangocairo-1.0-0.dll", "libpangocairo-1.0.so.0", "libpangocairo-1.0.0.dylib", "pangocairo-1.dll" },
            [Library.GtkSource] = new string[4] { "libgtksourceview-4-0.dll", "libgtksourceview-4.so.0", "libgtksourceview-4.0.dylib", "gtksourceview-4.dll" },
            // WebKitGTK for GTK 4 is a different library from the GTK 3 one: the GTK 3 binding is
            // webkit2gtk-4.0/4.1 (soname libwebkit2gtk-4.0.so.37), the GTK 4 binding is webkitgtk-6.0
            // (soname libwebkitgtk-6.0.so.4, WebKitGTK >= 2.44). Loading the 4.0 library into a GTK 4
            // process pulls in GTK 3 alongside GTK 4 and aborts, so naming it here could only ever fail.
            [Library.Webkit] = new[] { "libwebkitgtk-6.0-4.dll", "libwebkitgtk-6.0.so.4", "libwebkitgtk-6.0.4.dylib", "webkitgtk-6.0.dll" }
        };
    }

    public static IntPtr Load(Library library)
    {
        var value = IntPtr.Zero;
        if (_libraries.TryGetValue(library, out value))
        {
            return value;
        }

        if (FuncLoader.IsWindows)
        {
            value = FuncLoader.LoadLibrary(_libraryDefinitions[library][0]);
            if (value == IntPtr.Zero)
            {
                SetDllDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gtk", "3.24.24"));
                value = FuncLoader.LoadLibrary(_libraryDefinitions[library][0]);
            }
        }
        else if (FuncLoader.IsOSX)
        {
            value = FuncLoader.LoadLibrary(_libraryDefinitions[library][2]);
            if (value == IntPtr.Zero)
            {
                value = FuncLoader.LoadLibrary("/usr/local/lib/" + _libraryDefinitions[library][2]);
            }
        }
        else
        {
            value = FuncLoader.LoadLibrary(_libraryDefinitions[library][1]);
        }

        if (value == IntPtr.Zero)
        {
            for (var i = 0; i < _libraryDefinitions[library].Length; i++)
            {
                value = FuncLoader.LoadLibrary(_libraryDefinitions[library][i]);
                if (value != IntPtr.Zero)
                {
                    break;
                }
            }
        }

        if (value == IntPtr.Zero)
        {
            throw new DllNotFoundException(library.ToString() + ": " + string.Join(", ", _libraryDefinitions[library]));
        }

        _libraries[library] = value;

        return value;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetDllDirectory(string lpPathName);

}
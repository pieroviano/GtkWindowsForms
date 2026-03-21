using System.Drawing;
using System.Runtime.InteropServices;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// GtkMenuItem
/// </summary>
public class GtkMenuItem : Gtk.MenuItem
{

    private static readonly d_gtk_image_menu_item_get_always_show_image gtk_image_menu_item_get_always_show_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_always_show_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_always_show_image"));

    private static readonly d_gtk_image_menu_item_get_image gtk_image_menu_item_get_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_image"));

    private static readonly d_gtk_image_menu_item_get_type gtk_image_menu_item_get_type = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_type>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_type"));

    private static readonly d_gtk_image_menu_item_set_always_show_image gtk_image_menu_item_set_always_show_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_set_always_show_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_set_always_show_image"));

    private static readonly d_gtk_image_menu_item_set_image gtk_image_menu_item_set_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_set_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_set_image"));

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.GtkMenuItem" /> class.
    /// </summary>
    public GtkMenuItem()
    {
    }

    /// <summary>
    /// Gets the type of the g.
    /// </summary>
    /// <value>
    /// The type of the g.
    /// </value>
    public new static GLib.GType GType
    {
        get
        {
            var val = gtk_image_menu_item_get_type();

            return new GLib.GType(val);
        }
    }

    [GLib.Property("always-show-image")]
    internal bool AlwaysShowImage
    {
        get
        {
            return gtk_image_menu_item_get_always_show_image(base.Handle);
        }
        set
        {
            gtk_image_menu_item_set_always_show_image(base.Handle, value);
        }
    }

    internal Gtk.Image DefaultImage
    {
        get;
        set;
    } = new Gtk.Image("image-missing", Gtk.IconSize.SmallToolbar);

    [GLib.Property("Image")]
    internal Gtk.Widget IcoImage
    {
        get
        {
            return GLib.Object.GetObject(gtk_image_menu_item_get_image(base.Handle)) as Gtk.Widget;
        }
        set
        {
            gtk_image_menu_item_set_image(base.Handle, value?.Handle ?? IntPtr.Zero);
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    private delegate bool d_gtk_image_menu_item_get_always_show_image(IntPtr raw);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    private delegate IntPtr d_gtk_image_menu_item_get_image(IntPtr raw);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr d_gtk_image_menu_item_get_type();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    private delegate void d_gtk_image_menu_item_set_always_show_image(IntPtr raw, bool always_show);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

    private delegate void d_gtk_image_menu_item_set_image(IntPtr raw, IntPtr image);

}
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms
{
    /// <summary>
    /// GtkMenuItem
    /// </summary>
    public class GtkMenuItem : Gtk.MenuItem
    {

        /// <summary>
        /// The GTK image menu item get always show image
        /// </summary>
        private static d_gtk_image_menu_item_get_always_show_image gtk_image_menu_item_get_always_show_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_always_show_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_always_show_image"));

        /// <summary>
        /// The GTK image menu item get image
        /// </summary>
        private static d_gtk_image_menu_item_get_image gtk_image_menu_item_get_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_image"));

        /// <summary>
        /// The GTK image menu item get type
        /// </summary>
        private static d_gtk_image_menu_item_get_type gtk_image_menu_item_get_type = FuncLoader.LoadFunction<d_gtk_image_menu_item_get_type>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_get_type"));

        /// <summary>
        /// The GTK image menu item set always show image
        /// </summary>
        private static d_gtk_image_menu_item_set_always_show_image gtk_image_menu_item_set_always_show_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_set_always_show_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_set_always_show_image"));

        /// <summary>
        /// The GTK image menu item set image
        /// </summary>
        private static d_gtk_image_menu_item_set_image gtk_image_menu_item_set_image = FuncLoader.LoadFunction<d_gtk_image_menu_item_set_image>(FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_image_menu_item_set_image"));

        /// <summary>
        /// Initializes a new instance of the <see cref="GtkMenuItem"/> class.
        /// </summary>
        public GtkMenuItem()
        {
        }

        /// <summary>
        /// Gets the type of the g.
        /// </summary>
        /// <value>The type of the g.</value>
        public new static GLib.GType GType
        {
            get
            {
                IntPtr val = gtk_image_menu_item_get_type();

                return new GLib.GType(val);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [always show image].
        /// </summary>
        /// <value><c>true</c> if [always show image]; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Gets or sets the default image.
        /// </summary>
        /// <value>The default image.</value>
        internal Gtk.Image DefaultImage { get; set; } = new Gtk.Image("image-missing", Gtk.IconSize.SmallToolbar);

        /// <summary>
        /// Gets or sets the icon image.
        /// </summary>
        /// <value>The icon image.</value>
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

        /// <summary>
        /// Delegate d_gtk_image_menu_item_get_always_show_image
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate bool d_gtk_image_menu_item_get_always_show_image(IntPtr raw);

        /// <summary>
        /// Delegate d_gtk_image_menu_item_get_image
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <returns>IntPtr.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate IntPtr d_gtk_image_menu_item_get_image(IntPtr raw);

        /// <summary>
        /// Delegate d_gtk_image_menu_item_get_type
        /// </summary>
        /// <returns>IntPtr.</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_gtk_image_menu_item_get_type();

        /// <summary>
        /// Delegate d_gtk_image_menu_item_set_always_show_image
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="always_show">if set to <c>true</c> [always show].</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate void d_gtk_image_menu_item_set_always_show_image(IntPtr raw, bool always_show);

        /// <summary>
        /// Delegate d_gtk_image_menu_item_set_image
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="image">The image.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        private delegate void d_gtk_image_menu_item_set_image(IntPtr raw, IntPtr image);

    }

}

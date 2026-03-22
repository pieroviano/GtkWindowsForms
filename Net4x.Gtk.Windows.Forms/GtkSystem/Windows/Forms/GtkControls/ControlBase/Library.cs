using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GLib;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

internal enum Library
{
    GLib,
    GObject,
    Cairo,
    Gio,
    Atk,
    Pango,
    PangoCairo,
    Gdk,
    GdkPixbuf,
    Gtk,
    GtkSource,
    Webkit
}
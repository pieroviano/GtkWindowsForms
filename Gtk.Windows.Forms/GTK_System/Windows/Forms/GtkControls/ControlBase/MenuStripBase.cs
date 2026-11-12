using System;
using System.Drawing;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

public class MenuStripBase : Gtk.Box, IControlGtk
{
    public MenuStripBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.StyleContext.AddClass("MenuStrip");
        this.Hexpand = false;
        this.Vexpand = false;
        this.Valign = Gtk.Align.Start;
        this.Halign = Gtk.Align.Fill;
    }

    public Size ImageScalingSize
    {
        get;
        set;
    } = new Size(20, 20);

    public GtkControlOverride Override
    {
        get;
        set;
    }
}
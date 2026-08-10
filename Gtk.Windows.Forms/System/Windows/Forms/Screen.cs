// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a display device or multiple display devices on a single system.
/// </summary>
public partial class Screen
{

    private readonly int _bitDepth;

    private readonly Rectangle _bounds;

    private readonly string _deviceName;

    private Gdk.Monitor _monitor;

    private readonly bool _primary;

    private Rectangle _workingArea = Rectangle.Empty;

    private static Screen[]? s_screens;

    internal Screen(int monitornum) : this(GetMonitorByIndex(monitornum))
    {
    }

    internal Screen(Gdk.Monitor monitor)
    {
        _monitor = monitor;
        // Gtk4/Gdk4 dropped the notion of a primary monitor and per-monitor visuals; treat the
        // first monitor in the display's list as primary and report a conventional 32bpp depth.
        _primary = ReferenceEquals(monitor, GetMonitorByIndex(0));
        _deviceName = monitor.Display.Name;
        _bitDepth = 32;
        // Gdk4 monitors expose only Geometry (no separate work area); use it for both.
        Gdk.Rectangle rect = monitor.Geometry;
        _workingArea = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        _bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
    }

    /// <summary>
    /// Gtk4 replacement for <c>Gdk.Display.GetMonitor(i)</c>: indexes the display's monitor
    /// <see cref="GLib.IListModel"/>. Returns the first monitor when the index is out of range.
    /// </summary>
    private static Gdk.Monitor GetMonitorByIndex(int index)
    {
        var monitors = Gdk.Display.Default.Monitors;
        var count = (int)monitors.NItems;
        if (count == 0)
        {
            return null!;
        }

        if (index < 0 || index >= count)
        {
            index = 0;
        }

        return (Gdk.Monitor)monitors.GetObject((uint)index);
    }

    public static Screen[] AllScreens
    {
        get
        {
            if (s_screens is null)
            {
                int nmonitors = (int)Gdk.Display.Default.Monitors.NItems;
                s_screens = new Screen[nmonitors];
                for (int i = 0; i < nmonitors; i++)
                {
                    s_screens[i] = new Screen(i);
                }
            }

            return s_screens;
        }
    }

    public int BitsPerPixel => _bitDepth;

    public Rectangle Bounds => _bounds;

    public string DeviceName => _deviceName;

    public bool Primary => _primary;

    public static Screen? PrimaryScreen
    {
        get
        {
            return new Screen(GetMonitorByIndex(0));
        }
    }

    public Rectangle WorkingArea
    {
        get
        {
            return _workingArea;
        }
    }

    public override bool Equals(object? obj) => obj is Screen comp && _monitor.Handle == comp._monitor.Handle;

    public static Screen FromControl(Control control)
    {
        if (control == null || control.Widget == null)
            throw new ArgumentNullException();

        // Gtk4 removed Gdk.Window/GetMonitorAtWindow. Resolve the monitor from the widget's
        // top-level surface when it is realized; otherwise fall back to the primary monitor.
        var top = control.Widget;
        while (top.Parent != null)
            top = top.Parent;
        if (top is Gtk.Window win && win.Surface is Gdk.Surface surface)
        {
            var monitor = Gdk.Display.Default.GetMonitorAtSurface(surface);
            if (monitor != null)
                return new Screen(monitor);
        }

        return new Screen(GetMonitorByIndex(0));
    }

    public static Screen FromHandle(IntPtr hwnd)
    {
        if (Gtk.Window.TryGetObject(hwnd) is Gtk.Window win && win.Surface is Gdk.Surface surface)
        {
            Gdk.Monitor monitor = Gdk.Display.Default.GetMonitorAtSurface(surface);
            if (monitor != null)
                return new Screen(monitor);
        }

        return new Screen(GetMonitorByIndex(0));
    }

    public static Screen FromPoint(Point point)
    {
        // Gdk4 removed GetMonitorAtPoint; locate the monitor whose geometry contains the point.
        var monitors = Gdk.Display.Default.Monitors;
        var count = (int)monitors.NItems;
        for (int i = 0; i < count; i++)
        {
            var monitor = (Gdk.Monitor)monitors.GetObject((uint)i);
            var geometry = monitor.Geometry;
            if (point.X >= geometry.Left && point.X < geometry.Left + geometry.Width &&
                point.Y >= geometry.Top && point.Y < geometry.Top + geometry.Height)
            {
                return new Screen(monitor);
            }
        }

        return new Screen(GetMonitorByIndex(0));
    }

    public static Screen FromRectangle(Rectangle rect)
    {
        return FromPoint(rect.Location);
    }

    public static Rectangle GetBounds(Point pt) => FromPoint(pt).Bounds;

    public static Rectangle GetBounds(Rectangle rect) => FromRectangle(rect).Bounds;

    public static Rectangle GetBounds(Control ctl) => FromControl(ctl).Bounds;

    public override int GetHashCode() => _monitor.GetHashCode();

    public static Rectangle GetWorkingArea(Point pt) => FromPoint(pt).WorkingArea;

    public static Rectangle GetWorkingArea(Rectangle rect) => FromRectangle(rect).WorkingArea;

    public static Rectangle GetWorkingArea(Control ctl) => FromControl(ctl).WorkingArea;

    public override string ToString()
        => $"{GetType().Name}[Bounds={_bounds} WorkingArea={WorkingArea} Primary={_primary} DeviceName={_deviceName}";

}

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Provides information about the current system environment.
/// </summary>
/// <remarks>
/// This type was missing entirely, and layout code reads it constantly. The values below are the GTK
/// equivalents where one is available and the customary Windows defaults otherwise, so that layout
/// arithmetic written against WinForms produces sensible results rather than failing to compile.
/// </remarks>
public static class SystemInformation
{
    /// <summary>Gets the thickness, in pixels, of a flat window border.</summary>
    public static Size BorderSize => new(1, 1);

    /// <summary>Gets the thickness, in pixels, of a sizing border.</summary>
    public static Size FrameBorderSize => new(4, 4);

    /// <summary>Gets the thickness, in pixels, of a fixed-dialog border.</summary>
    public static Size FixedFrameBorderSize => new(3, 3);

    /// <summary>Gets the height, in pixels, of the standard title bar.</summary>
    public static int CaptionHeight => 24;

    /// <summary>Gets the height, in pixels, of the title bar of a tool window.</summary>
    public static int ToolWindowCaptionHeight => 18;

    /// <summary>Gets the standard size, in pixels, of a title-bar button.</summary>
    public static Size CaptionButtonSize => new(24, 24);

    /// <summary>Gets the standard size, in pixels, of a tool-window title-bar button.</summary>
    public static Size ToolWindowCaptionButtonSize => new(18, 18);

    /// <summary>Gets the height, in pixels, of one line of a menu.</summary>
    public static int MenuHeight => 24;

    /// <summary>Gets the standard size, in pixels, of a menu-bar button.</summary>
    public static Size MenuButtonSize => new(24, 24);

    /// <summary>Gets the width, in pixels, of a vertical scroll bar.</summary>
    public static int VerticalScrollBarWidth => 16;

    /// <summary>Gets the height, in pixels, of a horizontal scroll bar.</summary>
    public static int HorizontalScrollBarHeight => 16;

    /// <summary>Gets the height, in pixels, of the arrow button on a vertical scroll bar.</summary>
    public static int VerticalScrollBarArrowHeight => 16;

    /// <summary>Gets the width, in pixels, of the arrow button on a horizontal scroll bar.</summary>
    public static int HorizontalScrollBarArrowWidth => 16;

    /// <summary>Gets the height, in pixels, of the thumb on a vertical scroll bar.</summary>
    public static int VerticalScrollBarThumbHeight => 16;

    /// <summary>Gets the width, in pixels, of the thumb on a horizontal scroll bar.</summary>
    public static int HorizontalScrollBarThumbWidth => 16;

    /// <summary>Gets the dimensions, in pixels, of the area a double-click must occur within.</summary>
    public static Size DoubleClickSize => new(4, 4);

    /// <summary>Gets the maximum interval, in milliseconds, between the clicks of a double-click.</summary>
    public static int DoubleClickTime => 500;

    /// <summary>Gets the dimensions, in pixels, a drag must exceed to start a drag-and-drop operation.</summary>
    public static Size DragSize => new(4, 4);

    /// <summary>Gets the default dimensions, in pixels, of an icon.</summary>
    public static Size IconSize => new(32, 32);

    /// <summary>Gets the dimensions, in pixels, of a small icon.</summary>
    public static Size SmallIconSize => new(16, 16);

    /// <summary>Gets the dimensions, in pixels, of the grid an icon is aligned to.</summary>
    public static Size IconSpacingSize => new(75, 75);

    /// <summary>Gets the dimensions, in pixels, of a cursor.</summary>
    public static Size CursorSize => new(32, 32);

    /// <summary>Gets the height, in pixels, of a three-dimensional sizing border.</summary>
    public static Size Border3DSize => new(2, 2);

    /// <summary>Gets the number of lines to scroll for one detent of the mouse wheel.</summary>
    public static int MouseWheelScrollLines => 3;

    /// <summary>Gets the number of buttons on the mouse.</summary>
    public static int MouseButtons => 3;

    /// <summary>Gets a value indicating whether the mouse has a wheel.</summary>
    public static bool MouseWheelPresent => true;

    /// <summary>Gets a value indicating whether the left and right mouse buttons are swapped.</summary>
    public static bool MouseButtonsSwapped => false;

    /// <summary>Gets a value indicating whether the drop-shadow effect is enabled.</summary>
    public static bool IsDropShadowEnabled => true;

    /// <summary>Gets a value indicating whether hot tracking of user-interface elements is enabled.</summary>
    public static bool IsHotTrackingEnabled => true;

    /// <summary>Gets a value indicating whether menu fade animation is enabled.</summary>
    public static bool IsMenuFadeEnabled => true;

    /// <summary>Gets a value indicating whether the user relies on the keyboard rather than the mouse.</summary>
    public static bool IsKeyboardPreferred => false;

    /// <summary>Gets the border multiplier factor used when determining the thickness of a window border.</summary>
    public static int BorderMultiplierFactor => 1;

    /// <summary>Gets the width, in pixels, of the client area of a maximized top-level window.</summary>
    public static Rectangle WorkingArea => VirtualScreen;

    /// <summary>Gets the bounds of the virtual screen.</summary>
    public static Rectangle VirtualScreen
    {
        get
        {
            var display = Gdk.Display.Default;
            var monitor = (Gdk.Monitor?)(display?.Monitors.NItems > 0 ? display.Monitors.GetObject(0) : null); // Gtk4
            if (monitor == null)
            {
                return new Rectangle(0, 0, 1920, 1080);
            }

            var geometry = monitor.Geometry;

            return new Rectangle(geometry.X, geometry.Y, geometry.Width, geometry.Height);
        }
    }

    /// <summary>Gets the bounds of the primary display.</summary>
    public static Size PrimaryMonitorSize => VirtualScreen.Size;

    /// <summary>Gets the maximized size, in pixels, of a window on the primary display.</summary>
    public static Size PrimaryMonitorMaximizedWindowSize => PrimaryMonitorSize;

    /// <summary>Gets the number of display monitors.</summary>
    public static int MonitorCount => Math.Max(1, (int?)Gdk.Display.Default?.Monitors.NItems ?? 1);

    /// <summary>Gets a value indicating whether all the monitors use the same pixel colour format.</summary>
    public static bool MonitorsSameDisplayFormat => true;

    /// <summary>Gets the computer name of the current system.</summary>
    public static string ComputerName => Environment.MachineName;

    /// <summary>Gets the user name of the current thread.</summary>
    public static string UserName => Environment.UserName;

    /// <summary>Gets the domain the current user belongs to.</summary>
    public static string UserDomainName => Environment.UserDomainName;

    /// <summary>Gets a value indicating whether the current process is running in user-interactive mode.</summary>
    public static bool UserInteractive => Environment.UserInteractive;

    /// <summary>Gets the size, in pixels, of the working area with the given bounds taken into account.</summary>
    public static int SizingBorderWidth => FrameBorderSize.Width;

    /// <summary>Gets the height, in pixels, of the standard caption area including borders.</summary>
    public static int VerticalFocusThickness => 1;

    /// <summary>Gets the width, in pixels, of the left and right edges of the focus rectangle.</summary>
    public static int HorizontalFocusThickness => 1;
}

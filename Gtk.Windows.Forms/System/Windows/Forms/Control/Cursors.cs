using Gtk.Windows.Forms.Resources;

namespace System.Windows.Forms;

/// <summary>
/// Provides a collection of
/// <see cref="T:System.Windows.Forms.Cursor" />
/// objects for use by a Windows Forms application.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public static class Cursors
{

    private static Cursor? s_appStarting;

    private static Cursor? s_arrow;

    private static Cursor? s_cross;

    private static Cursor? s_defaultCursor;

    private static Cursor? s_hand;

    private static Cursor? s_help;

    private static Cursor? s_hSplit;

    private static Cursor? s_iBeam;

    private static Cursor? s_no;

    private static Cursor? s_noMove2D;

    private static Cursor? s_noMoveHoriz;

    private static Cursor? s_noMoveVert;

    private static Cursor? s_panEast;

    private static Cursor? s_panNE;

    private static Cursor? s_panNorth;

    private static Cursor? s_panNW;

    private static Cursor? s_panSE;

    private static Cursor? s_panSouth;

    private static Cursor? s_panSW;

    private static Cursor? s_panWest;

    private static Cursor? s_sizeAll;

    private static Cursor? s_sizeNESW;

    private static Cursor? s_sizeNS;

    private static Cursor? s_sizeNWSE;

    private static Cursor? s_sizeWE;

    private static Cursor? s_upArrow;

    private static Cursor? s_vSplit;

    private static Cursor? s_wait;

    /// <summary>
    /// Gets the application starting.
    /// </summary>
    /// <value>
    /// The application starting.
    /// </value>
    public static Cursor AppStarting => s_appStarting ??= new Cursor(AssemblyResources.ToCursorsUri("circle.cur"), 0, 0);

    /// <summary>
    /// Gets the arrow.
    /// </summary>
    /// <value>
    /// The arrow.
    /// </value>
    public static Cursor Arrow => s_arrow ??= new Cursor(AssemblyResources.ToCursorsUri("arrow.cur"), 0, 0);

    /// <summary>
    /// Gets the cross.
    /// </summary>
    /// <value>
    /// The cross.
    /// </value>
    public static Cursor Cross => s_cross ??= new Cursor(AssemblyResources.ToCursorsUri("cross.cur"), 10, 10);

    /// <summary>
    /// Gets the default.
    /// </summary>
    /// <value>
    /// The default.
    /// </value>
    public static Cursor Default => s_defaultCursor ??= new Cursor(AssemblyResources.ToCursorsUri("default.cur"), 0, 0);

    /// <summary>
    /// Gets the hand.
    /// </summary>
    /// <value>
    /// The hand.
    /// </value>
    public static Cursor Hand => s_hand ??= new Cursor(AssemblyResources.ToCursorsUri("hand.cur"), 0, 0);

    /// <summary>
    /// Gets the help.
    /// </summary>
    /// <value>
    /// The help.
    /// </value>
    public static Cursor Help => s_help ??= new Cursor(AssemblyResources.ToCursorsUri("help.cur"), 0, 0);

    /// <summary>
    /// Gets the h split.
    /// </summary>
    /// <value>
    /// The h split.
    /// </value>
    public static Cursor HSplit => s_hSplit ??= new Cursor(AssemblyResources.ToCursorsUri("size_ver.cur"), 0, 10);

    /// <summary>
    /// Gets the i beam.
    /// </summary>
    /// <value>
    /// The i beam.
    /// </value>
    public static Cursor IBeam => s_iBeam ??= new Cursor(AssemblyResources.ToCursorsUri("xterm.cur"), 0, 0);

    /// <summary>
    /// Gets the no.
    /// </summary>
    /// <value>
    /// The no.
    /// </value>
    public static Cursor No => s_no ??= new Cursor(AssemblyResources.ToCursorsUri("not-allowed.cur"), 0, 0);

    /// <summary>
    /// Gets the no move2 d.
    /// </summary>
    /// <value>
    /// The no move2 d.
    /// </value>
    public static Cursor NoMove2D => s_noMove2D ??= new Cursor(AssemblyResources.ToCursorsUri("move.cur"), 10, 10);

    /// <summary>
    /// Gets the no move horiz.
    /// </summary>
    /// <value>
    /// The no move horiz.
    /// </value>
    public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new Cursor(AssemblyResources.ToCursorsUri("size_ver.cur"), 0, 10);

    /// <summary>
    /// Gets the no move vert.
    /// </summary>
    /// <value>
    /// The no move vert.
    /// </value>
    public static Cursor NoMoveVert => s_noMoveVert ??= new Cursor(AssemblyResources.ToCursorsUri("size_hor.cur"), 10, 0);

    /// <summary>
    /// Gets the pan east.
    /// </summary>
    /// <value>
    /// The pan east.
    /// </value>
    public static Cursor PanEast => s_panEast ??= new Cursor(AssemblyResources.ToCursorsUri("e-resize.cur"), 20, 0);

    /// <summary>
    /// Gets the pan ne.
    /// </summary>
    /// <value>
    /// The pan ne.
    /// </value>
    public static Cursor PanNE => s_panNE ??= new Cursor(AssemblyResources.ToCursorsUri("ne-resize.cur"), 20, 0);

    /// <summary>
    /// Gets the pan north.
    /// </summary>
    /// <value>
    /// The pan north.
    /// </value>
    public static Cursor PanNorth => s_panNorth ??= new Cursor(AssemblyResources.ToCursorsUri("n-resize.cur"), 10, 0);

    /// <summary>
    /// Gets the pan nw.
    /// </summary>
    /// <value>
    /// The pan nw.
    /// </value>
    public static Cursor PanNW => s_panNW ??= new Cursor(AssemblyResources.ToCursorsUri("nw-resize.cur"), 0, 0);

    /// <summary>
    /// Gets the pan se.
    /// </summary>
    /// <value>
    /// The pan se.
    /// </value>
    public static Cursor PanSE => s_panSE ??= new Cursor(AssemblyResources.ToCursorsUri("se-resize.cur"), 20, 20);

    /// <summary>
    /// Gets the pan south.
    /// </summary>
    /// <value>
    /// The pan south.
    /// </value>
    public static Cursor PanSouth => s_panSouth ??= new Cursor(AssemblyResources.ToCursorsUri("s-resize.cur"), 0, 20);

    /// <summary>
    /// Gets the pan sw.
    /// </summary>
    /// <value>
    /// The pan sw.
    /// </value>
    public static Cursor PanSW => s_panSW ??= new Cursor(AssemblyResources.ToCursorsUri("sw-resize.cur"), 0, 20);

    /// <summary>
    /// Gets the pan west.
    /// </summary>
    /// <value>
    /// The pan west.
    /// </value>
    public static Cursor PanWest => s_panWest ??= new Cursor(AssemblyResources.ToCursorsUri("w-resize.cur"), 0, 0);

    /// <summary>
    /// Gets the size all.
    /// </summary>
    /// <value>
    /// The size all.
    /// </value>
    public static Cursor SizeAll => s_sizeAll ??= new Cursor(AssemblyResources.ToCursorsUri("size_all.cur"), 10, 10);

    /// <summary>
    /// Gets the size nesw.
    /// </summary>
    /// <value>
    /// The size nesw.
    /// </value>
    public static Cursor SizeNESW => s_sizeNESW ??= new Cursor(AssemblyResources.ToCursorsUri("nesw-resize.cur"), 10, 10);

    /// <summary>
    /// Gets the size ns.
    /// </summary>
    /// <value>
    /// The size ns.
    /// </value>
    public static Cursor SizeNS => s_sizeNS ??= new Cursor(AssemblyResources.ToCursorsUri("ns-resize.cur"), 0, 10);

    /// <summary>
    /// Gets the size nwse.
    /// </summary>
    /// <value>
    /// The size nwse.
    /// </value>
    public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor(AssemblyResources.ToCursorsUri("nwse-resize.cur"), 10, 10);

    /// <summary>
    /// Gets the size we.
    /// </summary>
    /// <value>
    /// The size we.
    /// </value>
    public static Cursor SizeWE => s_sizeWE ??= new Cursor(AssemblyResources.ToCursorsUri("ew-resize.cur"), 10, 0);

    /// <summary>
    /// Gets up arrow.
    /// </summary>
    /// <value>
    /// Up arrow.
    /// </value>
    public static Cursor UpArrow => s_upArrow ??= new Cursor(AssemblyResources.ToCursorsUri("sb_up_arrow.cur"), 0, 0);

    /// <summary>
    /// Gets the v split.
    /// </summary>
    /// <value>
    /// The v split.
    /// </value>
    public static Cursor VSplit => s_vSplit ??= new Cursor(AssemblyResources.ToCursorsUri("size_hor.cur"), 10, 0);

    /// <summary>
    /// Gets the wait cursor.
    /// </summary>
    /// <value>
    /// The wait cursor.
    /// </value>
    public static Cursor WaitCursor => s_wait ??= new Cursor(AssemblyResources.ToCursorsUri("wait_i.cur"), 0, 0);

}
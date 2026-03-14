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
    public static Cursor AppStarting => s_appStarting ??= new Cursor(Gdk.CursorType.Circle);

    /// <summary>
    /// Gets the arrow.
    /// </summary>
    /// <value>
    /// The arrow.
    /// </value>
    public static Cursor Arrow => s_arrow ??= new Cursor(Gdk.CursorType.Arrow);

    /// <summary>
    /// Gets the cross.
    /// </summary>
    /// <value>
    /// The cross.
    /// </value>
    public static Cursor Cross => s_cross ??= new Cursor(Gdk.CursorType.Cross);

    /// <summary>
    /// Gets the default.
    /// </summary>
    /// <value>
    /// The default.
    /// </value>
    public static Cursor Default => s_defaultCursor ??= new Cursor(Gdk.CursorType.Arrow);

    /// <summary>
    /// Gets the hand.
    /// </summary>
    /// <value>
    /// The hand.
    /// </value>
    public static Cursor Hand => s_hand ??= new Cursor(Gdk.CursorType.Hand1);

    /// <summary>
    /// Gets the help.
    /// </summary>
    /// <value>
    /// The help.
    /// </value>
    public static Cursor Help => s_help ??= new Cursor(Gdk.CursorType.QuestionArrow);

    /// <summary>
    /// Gets the h split.
    /// </summary>
    /// <value>
    /// The h split.
    /// </value>
    public static Cursor HSplit => s_hSplit ??= new Cursor("Resources.Cursors.hsplit.cur", 25, 25);

    /// <summary>
    /// Gets the i beam.
    /// </summary>
    /// <value>
    /// The i beam.
    /// </value>
    public static Cursor IBeam => s_iBeam ??= new Cursor(Gdk.CursorType.Xterm);

    /// <summary>
    /// Gets the no.
    /// </summary>
    /// <value>
    /// The no.
    /// </value>
    public static Cursor No => s_no ??= new Cursor("Resources.Cursors.aero_unavail.cur", 0, 0);

    /// <summary>
    /// Gets the no move2 d.
    /// </summary>
    /// <value>
    /// The no move2 d.
    /// </value>
    public static Cursor NoMove2D => s_noMove2D ??= new Cursor("Resources.Cursors.nomove2d.cur", 25, 25);

    /// <summary>
    /// Gets the no move horiz.
    /// </summary>
    /// <value>
    /// The no move horiz.
    /// </value>
    public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new Cursor("Resources.Cursors.nomoveh.cur", 25, 25);

    /// <summary>
    /// Gets the no move vert.
    /// </summary>
    /// <value>
    /// The no move vert.
    /// </value>
    public static Cursor NoMoveVert => s_noMoveVert ??= new Cursor("Resources.Cursors.nomovev.cur", 25, 25);

    /// <summary>
    /// Gets the pan east.
    /// </summary>
    /// <value>
    /// The pan east.
    /// </value>
    public static Cursor PanEast => s_panEast ??= new Cursor("Resources.Cursors.east.cur", 25, 25);

    /// <summary>
    /// Gets the pan ne.
    /// </summary>
    /// <value>
    /// The pan ne.
    /// </value>
    public static Cursor PanNE => s_panNE ??= new Cursor("Resources.Cursors.ne.cur", 25, 25);

    /// <summary>
    /// Gets the pan north.
    /// </summary>
    /// <value>
    /// The pan north.
    /// </value>
    public static Cursor PanNorth => s_panNorth ??= new Cursor("Resources.Cursors.north.cur", 25, 25);

    /// <summary>
    /// Gets the pan nw.
    /// </summary>
    /// <value>
    /// The pan nw.
    /// </value>
    public static Cursor PanNW => s_panNW ??= new Cursor("Resources.Cursors.nw.cur", 25, 25);

    /// <summary>
    /// Gets the pan se.
    /// </summary>
    /// <value>
    /// The pan se.
    /// </value>
    public static Cursor PanSE => s_panSE ??= new Cursor("Resources.Cursors.se.cur", 25, 25);

    /// <summary>
    /// Gets the pan south.
    /// </summary>
    /// <value>
    /// The pan south.
    /// </value>
    public static Cursor PanSouth => s_panSouth ??= new Cursor("Resources.Cursors.south.cur", 25, 25);

    /// <summary>
    /// Gets the pan sw.
    /// </summary>
    /// <value>
    /// The pan sw.
    /// </value>
    public static Cursor PanSW => s_panSW ??= new Cursor("Resources.Cursors.sw.cur", 25, 25);

    /// <summary>
    /// Gets the pan west.
    /// </summary>
    /// <value>
    /// The pan west.
    /// </value>
    public static Cursor PanWest => s_panWest ??= new Cursor("Resources.Cursors.west.cur", 25, 25);

    /// <summary>
    /// Gets the size all.
    /// </summary>
    /// <value>
    /// The size all.
    /// </value>
    public static Cursor SizeAll => s_sizeAll ??= new Cursor(Gdk.CursorType.Fleur);

    /// <summary>
    /// Gets the size nesw.
    /// </summary>
    /// <value>
    /// The size nesw.
    /// </value>
    public static Cursor SizeNESW => s_sizeNESW ??= new Cursor("Resources.Cursors.aero_nesw.cur", 15, 15);

    /// <summary>
    /// Gets the size ns.
    /// </summary>
    /// <value>
    /// The size ns.
    /// </value>
    public static Cursor SizeNS => s_sizeNS ??= new Cursor("Resources.Cursors.aero_ns.cur", 15, 15);

    /// <summary>
    /// Gets the size nwse.
    /// </summary>
    /// <value>
    /// The size nwse.
    /// </value>
    public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor("Resources.Cursors.aero_nwse.cur", 15, 15);

    /// <summary>
    /// Gets the size we.
    /// </summary>
    /// <value>
    /// The size we.
    /// </value>
    public static Cursor SizeWE => s_sizeWE ??= new Cursor("Resources.Cursors.aero_ew.cur", 15, 15);

    /// <summary>
    /// Gets up arrow.
    /// </summary>
    /// <value>
    /// Up arrow.
    /// </value>
    public static Cursor UpArrow => s_upArrow ??= new Cursor(Gdk.CursorType.SbUpArrow);

    /// <summary>
    /// Gets the v split.
    /// </summary>
    /// <value>
    /// The v split.
    /// </value>
    public static Cursor VSplit => s_vSplit ??= new Cursor("Resources.Cursors.vsplit.cur", 25, 25);

    /// <summary>
    /// Gets the wait cursor.
    /// </summary>
    /// <value>
    /// The wait cursor.
    /// </value>
    public static Cursor WaitCursor => s_wait ??= new Cursor(Gdk.CursorType.Watch);

}
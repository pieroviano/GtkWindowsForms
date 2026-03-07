namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a collection of
    /// <see cref="System.Windows.Forms.Cursor" />
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
        /// Gets the cursor that appears when an application starts.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears when an application starts.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor AppStarting => s_appStarting ??= new Cursor(Gdk.CursorType.Circle);

        /// <summary>
        /// Gets the arrow cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the arrow cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor Arrow => s_arrow ??= new Cursor(Gdk.CursorType.Arrow);

        /// <summary>
        /// Gets the crosshair cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the crosshair cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor Cross => s_cross ??= new Cursor(Gdk.CursorType.Cross);

        /// <summary>
        /// Gets the default cursor, which is usually an arrow cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the default cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor Default => s_defaultCursor ??= new Cursor(Gdk.CursorType.Arrow);

        /// <summary>
        /// Gets the hand cursor, typically used when hovering over a Web link.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the hand cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor Hand => s_hand ??= new Cursor(Gdk.CursorType.Hand1);

        /// <summary>
        /// Gets the Help cursor, which is a combination of an arrow and a question mark.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the Help cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor Help => s_help ??= new Cursor(Gdk.CursorType.QuestionArrow);

        /// <summary>
        /// Gets the cursor that appears when the mouse is positioned over a horizontal splitter bar.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears when the mouse is positioned over a horizontal splitter bar.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor HSplit => s_hSplit ??= new Cursor("Resources.Cursors.hsplit.cur", 25, 25);

        /// <summary>
        /// Gets the I-beam cursor, which is used to show where the text cursor appears when the mouse is clicked.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the I-beam cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor IBeam => s_iBeam ??= new Cursor(Gdk.CursorType.Xterm);

        /// <summary>
        /// Gets the cursor that indicates that a particular region is invalid for the current operation.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that indicates that a particular region is invalid for the current operation.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor No => s_no ??= new Cursor("Resources.Cursors.aero_unavail.cur", 0, 0);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is not moving, but the window can be scrolled in both a horizontal and vertical direction.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is not moving.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor NoMove2D => s_noMove2D ??= new Cursor("Resources.Cursors.nomove2d.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is not moving, but the window can be scrolled in a horizontal direction.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is not moving.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor NoMoveHoriz => s_noMoveHoriz ??= new Cursor("Resources.Cursors.nomoveh.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is not moving, but the window can be scrolled in a vertical direction.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is not moving.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor NoMoveVert => s_noMoveVert ??= new Cursor("Resources.Cursors.nomovev.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally to the right.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally to the right.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanEast => s_panEast ??= new Cursor("Resources.Cursors.east.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically upward and to the right.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically upward and to the right.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanNE => s_panNE ??= new Cursor("Resources.Cursors.ne.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling vertically in an upward direction.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling vertically in an upward direction.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanNorth => s_panNorth ??= new Cursor("Resources.Cursors.north.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically upward and to the left.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically upward and to the left.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanNW => s_panNW ??= new Cursor("Resources.Cursors.nw.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically downward and to the right.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically downward and to the right.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanSE => s_panSE ??= new Cursor("Resources.Cursors.se.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling vertically in a downward direction.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling vertically in a downward direction.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanSouth => s_panSouth ??= new Cursor("Resources.Cursors.south.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically downward and to the left.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally and vertically downward and to the left.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanSW => s_panSW ??= new Cursor("Resources.Cursors.sw.cur", 25, 25);

        /// <summary>
        /// Gets the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally to the left.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears during wheel operations when the mouse is moving and the window is scrolling horizontally to the left.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor PanWest => s_panWest ??= new Cursor("Resources.Cursors.west.cur", 25, 25);

        /// <summary>
        /// Gets the four-headed sizing cursor, which consists of four joined arrows that point north, south, east, and west.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the four-headed sizing cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor SizeAll => s_sizeAll ??= new Cursor(Gdk.CursorType.Fleur);

        /// <summary>
        /// Gets the two-headed diagonal (northeast/southwest) sizing cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents two-headed diagonal (northeast/southwest) sizing cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor SizeNESW => s_sizeNESW ??= new Cursor("Resources.Cursors.aero_nesw.cur", 15, 15);

        /// <summary>
        /// Gets the two-headed vertical (north/south) sizing cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the two-headed vertical (north/south) sizing cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor SizeNS => s_sizeNS ??= new Cursor("Resources.Cursors.aero_ns.cur", 15, 15);

        /// <summary>
        /// Gets the two-headed diagonal (northwest/southeast) sizing cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the two-headed diagonal (northwest/southeast) sizing cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor SizeNWSE => s_sizeNWSE ??= new Cursor("Resources.Cursors.aero_nwse.cur", 15, 15);

        /// <summary>
        /// Gets the two-headed horizontal (west/east) sizing cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the two-headed horizontal (west/east) sizing cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor SizeWE => s_sizeWE ??= new Cursor("Resources.Cursors.aero_ew.cur", 15, 15);

        /// <summary>
        /// Gets the up arrow cursor, typically used to identify an insertion point.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the up arrow cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor UpArrow => s_upArrow ??= new Cursor(Gdk.CursorType.SbUpArrow);

        /// <summary>
        /// Gets the cursor that appears when the mouse is positioned over a vertical splitter bar.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the cursor that appears when the mouse is positioned over a vertical splitter bar.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor VSplit => s_vSplit ??= new Cursor("Resources.Cursors.vsplit.cur", 25, 25);

        /// <summary>
        /// Gets the wait cursor, typically an hourglass shape.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the wait cursor.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor WaitCursor => s_wait ??= new Cursor(Gdk.CursorType.Watch);
    }
}

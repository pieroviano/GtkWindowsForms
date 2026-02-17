namespace System.Windows.Forms
{

    /// <summary>
    /// Defines constants that indicate the alignment of content within a System.Windows.Forms.DataGridView
    /// cell.
    /// </summary>
    public enum DataGridViewContentAlignment
    {

        /// <summary>
        /// The alignment is not set.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// The content is aligned vertically at the top and horizontally at the left of
        /// a cell.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        /// The content is aligned vertically at the top and horizontally at the center of
        /// a cell.
        /// </summary>
        TopCenter = 2,

        /// <summary>
        /// The content is aligned vertically at the top and horizontally at the right of
        /// a cell.
        /// </summary>
        TopRight = 4,

        /// <summary>
        /// The content is aligned vertically at the middle and horizontally at the left
        /// of a cell.
        /// </summary>
        MiddleLeft = 16,

        /// <summary>
        /// The content is aligned at the vertical and horizontal center of a cell.
        /// </summary>
        MiddleCenter = 32,

        /// <summary>
        /// The content is aligned vertically at the middle and horizontally at the right
        /// of a cell.
        /// </summary>
        MiddleRight = 64,

        /// <summary>
        /// The content is aligned vertically at the bottom and horizontally at the left
        /// of a cell.
        /// </summary>
        BottomLeft = 256,

        /// <summary>
        /// The content is aligned vertically at the bottom and horizontally at the center
        /// of a cell.
        /// </summary>
        BottomCenter = 512,

        /// <summary>
        /// The content is aligned vertically at the bottom and horizontally at the right
        /// of a cell.
        /// </summary>
        BottomRight = 1024
    }
}
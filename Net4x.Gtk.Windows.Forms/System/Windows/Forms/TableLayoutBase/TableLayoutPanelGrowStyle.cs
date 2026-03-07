namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how a
    /// <see cref="System.Windows.Forms.TableLayoutPanel" />
    /// will gain additional rows or columns after its existing cells are full.
    /// </summary>
    public enum TableLayoutPanelGrowStyle
    {
        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> does not allow additional rows or columns after it is full.
        /// </summary>
        FixedSize,
        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> gains additional rows after it is full.
        /// </summary>
        AddRows,
        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.TableLayoutPanel" /> gains additional columns after it is full.
        /// </summary>
        AddColumns
    }
}

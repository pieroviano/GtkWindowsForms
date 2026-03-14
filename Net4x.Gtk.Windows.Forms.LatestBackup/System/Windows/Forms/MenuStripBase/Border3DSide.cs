namespace System.Windows.Forms;

/// <summary>
/// Specifies the sides of a rectangle to apply a three-dimensional border to.
/// </summary>
[Flags]
public enum Border3DSide
{
    /// <summary>
    /// The left
    /// </summary>
    Left = 0x1,
    /// <summary>
    /// The top
    /// </summary>
    Top = 0x2,
    /// <summary>
    /// The right
    /// </summary>
    Right = 0x4,
    /// <summary>
    /// The bottom
    /// </summary>
    Bottom = 0x8,
    /// <summary>
    /// The middle
    /// </summary>
    Middle = 0x800,
    /// <summary>
    /// All
    /// </summary>
    All = 0x80F
}
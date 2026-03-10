namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the sides of a rectangle to apply a three-dimensional border to.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [Flags]
    public enum Border3DSide
    {
        Left = 0x1,
        Top = 0x2,
        Right = 0x4,
        Bottom = 0x8,
        Middle = 0x800,
        All = 0x80F
    }
}

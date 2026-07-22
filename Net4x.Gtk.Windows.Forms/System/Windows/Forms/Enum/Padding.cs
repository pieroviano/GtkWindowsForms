using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents padding or margin information associated with a user interface (UI) element.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public struct Padding
{

    /// <summary>
    /// Provides a System.Windows.Forms.Padding object with no padding.
    /// </summary>
    public static readonly Padding Empty;

    /// <summary>
    /// Initializes a new instance of the System.Windows.Forms.Padding class using the
    /// supplied padding size for all edges.
    /// </summary>
    /// <param name="all">
    /// The number of pixels to be used for padding for all edges.
    /// </param>
    public Padding(int all)
    {
        Left = all;
        Top = all;
        Right = all;
        Bottom = all;
    }

    /// <summary>
    /// Initializes a new instance of the System.Windows.Forms.Padding class using a
    /// separate padding size for each edge.
    /// </summary>
    /// <param name="left">
    /// The padding size, in pixels, for the left edge.
    /// </param>
    /// <param name="top">
    /// The padding size, in pixels, for the top edge.
    /// </param>
    /// <param name="right">
    /// The padding size, in pixels, for the right edge.
    /// </param>
    /// <param name="bottom">
    /// The padding size, in pixels, for the bottom edge.
    /// </param>
    public Padding(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Gets or sets the padding value for all the edges.
    /// </summary>
    /// <returns>
    /// The padding, in pixels, for all edges if the same; otherwise, -1.
    /// </returns>
    /// <remarks>
    /// This was a stored auto-property that the four-edge constructor set to <c>left</c>, so a Padding
    /// with differing edges reported the left edge as if every edge matched it. WinForms computes All,
    /// returning -1 when the edges differ; setting it applies the value to every edge.
    /// </remarks>
    [RefreshProperties(RefreshProperties.All)]
    public int All
    {
        get => Left == Top && Top == Right && Right == Bottom ? Left : -1;
        set
        {
            Left = value;
            Top = value;
            Right = value;
            Bottom = value;
        }
    }

    /// <summary>
    /// Gets or sets the padding value for the bottom edge.
    /// </summary>
    /// <returns>
    /// The padding, in pixels, for the bottom edge.
    /// </returns>
    [RefreshProperties(RefreshProperties.All)]
    public int Bottom
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the combined padding for the right and left edges.
    /// </summary>
    /// <returns>
    /// Gets the sum, in pixels, of the System.Windows.Forms.Padding.Left and System.Windows.Forms.Padding.Right padding values.
    /// </returns>
    /// <remarks>
    /// This computed <c>Right - Left</c>, contradicting its own documentation and WinForms, where
    /// Horizontal is the sum of the two edges. Symmetric padding therefore reported 0, so every
    /// "client width = Width - Padding.Horizontal" calculation silently used the full width.
    /// </remarks>
    [Browsable(false)]
    public int Horizontal { get { return Left + Right; } }

    /// <summary>
    /// Gets or sets the padding value for the left edge.
    /// </summary>
    /// <returns>
    /// The padding, in pixels, for the left edge.
    /// </returns>
    [RefreshProperties(RefreshProperties.All)]
    public int Left
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the padding value for the right edge.
    /// </summary>
    /// <returns>
    /// The padding, in pixels, for the right edge.
    /// </returns>
    [RefreshProperties(RefreshProperties.All)]
    public int Right
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the padding information in the form of a System.Drawing.Size.
    /// </summary>
    /// <returns>
    /// A System.Drawing.Size containing the padding information.
    /// </returns>
    /// <remarks>
    /// This was an auto-property nothing ever assigned, so it always reported an empty size.
    /// </remarks>
    [Browsable(false)]
    public Size Size => new(Horizontal, Vertical);

    /// <summary>
    /// Gets or sets the padding value for the top edge.
    /// </summary>
    /// <returns>
    /// The padding, in pixels, for the top edge.
    /// </returns>
    [RefreshProperties(RefreshProperties.All)]
    public int Top
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the combined padding for the top and bottom edges.
    /// </summary>
    /// <returns>
    /// Gets the sum, in pixels, of the System.Windows.Forms.Padding.Top and System.Windows.Forms.Padding.Bottom padding values.
    /// </returns>
    /// <remarks>
    /// This computed <c>Bottom - Top</c>; see the note on
    /// <see cref="P:System.Windows.Forms.Padding.Horizontal" />.
    /// </remarks>
    [Browsable(false)]
    public int Vertical { get { return Top + Bottom; } }

    /// <summary>
    /// Computes the sum of the two specified System.Windows.Forms.Padding values.
    /// </summary>
    /// <param name="p1">
    /// A System.Windows.Forms.Padding.
    /// </param>
    /// <param name="p2">
    /// A System.Windows.Forms.Padding.
    /// </param>
    /// <returns>
    /// A System.Windows.Forms.Padding that contains the sum of the two specified System.Windows.Forms.Padding values.
    /// </returns>
    public static Padding Add(Padding p1, Padding p2)
    {
        return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
    }

    /// <summary>
    /// Determines whether the value of the specified object is equivalent to the current
    /// System.Windows.Forms.Padding.
    /// </summary>
    /// <param name="other">
    /// The object to compare to the current System.Windows.Forms.Padding.
    /// </param>
    /// <returns>
    /// true if the System.Windows.Forms.Padding objects are equivalent; otherwise, false.
    /// </returns>
    public override bool Equals(object other)
    {
        if (other is Padding p)
        {
            return Left == p.Left
                && Top == p.Top
                && Right == p.Right
                && Bottom == p.Bottom;
        }

        return false;
    }

    /// <summary>
    /// Generates a hash code for the current System.Windows.Forms.Padding.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    public override int GetHashCode()
    {
        return Left.GetHashCode() + Top.GetHashCode() + Right.GetHashCode() + Bottom.GetHashCode();
    }

    /// <summary>
    /// Subtracts one specified System.Windows.Forms.Padding value from another.
    /// </summary>
    /// <param name="p1">
    /// A System.Windows.Forms.Padding.
    /// </param>
    /// <param name="p2">
    /// A System.Windows.Forms.Padding.
    /// </param>
    /// <returns>
    /// A System.Windows.Forms.Padding that contains the result of the subtraction of one specified System.Windows.Forms.Padding value from another.
    /// </returns>
    public static Padding Subtract(Padding p1, Padding p2)
    {
        return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
    }

    /// <summary>
    /// Returns a string that represents the current System.Windows.Forms.Padding.
    /// </summary>
    /// <returns>
    /// A System.String that represents the current System.Windows.Forms.Padding.
    /// </returns>
    public override string ToString()
    {
        return $"{{Top: {Top}, Left: {Left}, Bottom: {Bottom}, Right: {Right}}}";
    }
}
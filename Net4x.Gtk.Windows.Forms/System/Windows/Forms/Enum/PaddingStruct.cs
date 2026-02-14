using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace System.Windows.Forms
{
    //
    // Summary:
    //     Represents padding or margin information associated with a user interface (UI)
    //     element.

    public struct Padding
    {
        //
        // Summary:
        //     Provides a System.Windows.Forms.Padding object with no padding.
        public static readonly Padding Empty;

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.Padding class using the
        //     supplied padding size for all edges.
        //
        // Parameter:
        //   all:
        //     The number of pixels to be used for padding for all edges.
        public Padding(int all) {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
            All = all;
            Size = new Size(0, 0);
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.Padding class using a
        //     separate padding size for each edge.
        //
        // Parameter:
        //   left:
        //     The padding size, in pixels, for the left edge.
        //
        //   top:
        //     The padding size, in pixels, for the top edge.
        //
        //   right:
        //     The padding size, in pixels, for the right edge.
        //
        //   bottom:
        //     The padding size, in pixels, for the bottom edge.
        public Padding(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

            All = left;
            Size = new Size(right - left, bottom - top);
        }

        //
        // Summary:
        //     Gets the combined padding for the right and left edges.
        //
        // Return result:
        //     Gets the sum, in pixels, of the System.Windows.Forms.Padding.Left and System.Windows.Forms.Padding.Right
        //     padding values.
        [Browsable(false)]
        public int Horizontal { get { return Right - Left; } }
        //
        // Summary:
        //     Gets or sets the padding value for the top edge.
        //
        // Return result:
        //     The padding, in pixels, for the top edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Top { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the right edge.
        //
        // Return result:
        //     The padding, in pixels, for the right edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Right { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the left edge.
        //
        // Return result:
        //     The padding, in pixels, for the left edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Left { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for the bottom edge.
        //
        // Return result:
        //     The padding, in pixels, for the bottom edge.
        [RefreshProperties(RefreshProperties.All)]
        public int Bottom { get; set; }
        //
        // Summary:
        //     Gets or sets the padding value for all the edges.
        //
        // Return result:
        //     The padding, in pixels, for all edges if the same; otherwise, -1.
        [RefreshProperties(RefreshProperties.All)]
        public int All { get; set; }
        //
        // Summary:
        //     Gets the padding information in the form of a System.Drawing.Size.
        //
        // Return result:
        //     A System.Drawing.Size containing the padding information.
        [Browsable(false)]
        public Size Size { get; }
        //
        // Summary:
        //     Gets the combined padding for the top and bottom edges.
        //
        // Return result:
        //     Gets the sum, in pixels, of the System.Windows.Forms.Padding.Top and System.Windows.Forms.Padding.Bottom
        //     padding values.
        [Browsable(false)]
        public int Vertical { get { return Bottom - Top; } }

        //
        // Summary:
        //     Computes the sum of the two specified System.Windows.Forms.Padding values.
        //
        // Parameter:
        //   p1:
        //     A System.Windows.Forms.Padding.
        //
        //   p2:
        //     A System.Windows.Forms.Padding.
        //
        // Return result:
        //     A System.Windows.Forms.Padding that contains the sum of the two specified System.Windows.Forms.Padding
        //     values.
        public static Padding Add(Padding p1, Padding p2) {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }
        //
        // Summary:
        //     Subtracts one specified System.Windows.Forms.Padding value from another.
        //
        // Parameter:
        //   p1:
        //     A System.Windows.Forms.Padding.
        //
        //   p2:
        //     A System.Windows.Forms.Padding.
        //
        // Return result:
        //     A System.Windows.Forms.Padding that contains the result of the subtraction of
        //     one specified System.Windows.Forms.Padding value from another.
        public static Padding Subtract(Padding p1, Padding p2) {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }
        //
        // Summary:
        //     Determines whether the value of the specified object is equivalent to the current
        //     System.Windows.Forms.Padding.
        //
        // Parameter:
        //   other:
        //     The object to compare to the current System.Windows.Forms.Padding.
        //
        // Return result:
        //     true if the System.Windows.Forms.Padding objects are equivalent; otherwise, false.
        public override bool Equals(object other) {
            return false;
        }
        //
        // Summary:
        //     Generates a hash code for the current System.Windows.Forms.Padding.
        //
        // Return result:
        //     A 32-bit signed integer hash code.
        public override int GetHashCode() {
            return Left.GetHashCode() + Top.GetHashCode() + Right.GetHashCode() + Bottom.GetHashCode();
        }
        //
        // Summary:
        //     Returns a string that represents the current System.Windows.Forms.Padding.
        //
        // Return result:
        //     A System.String that represents the current System.Windows.Forms.Padding.
        public override string ToString() {
            return this.GetType().Name;
        }

        //
        // Summary:
        //     Performs vector addition on the two specified System.Windows.Forms.Padding objects,
        //     resulting in a new System.Windows.Forms.Padding.
        //
        // Parameter:
        //   p1:
        //     The first System.Windows.Forms.Padding to add.
        //
        //   p2:
        //     The second System.Windows.Forms.Padding to add.
        //
        // Return result:
        //     A new System.Windows.Forms.Padding that results from adding p1 and p2.
        public static Padding operator +(Padding p1, Padding p2) {
            return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
        }
        //
        // Summary:
        //     Performs vector subtraction on the two specified System.Windows.Forms.Padding
        //     objects, resulting in a new System.Windows.Forms.Padding.
        //
        // Parameter:
        //   p1:
        //     The System.Windows.Forms.Padding to subtract from (the minuend).
        //
        //   p2:
        //     The System.Windows.Forms.Padding to subtract from (the subtrahend).
        //
        // Return result:
        //     The System.Windows.Forms.Padding result of subtracting p2 from p1.
        public static Padding operator -(Padding p1, Padding p2) {
            return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
        }
        //
        // Summary:
        //     Tests whether two specified System.Windows.Forms.Padding objects are equivalent.
        //
        // Parameter:
        //   p1:
        //     A System.Windows.Forms.Padding to test.
        //
        //   p2:
        //     A System.Windows.Forms.Padding to test.
        //
        // Return result:
        //     true if the two System.Windows.Forms.Padding objects are equal; otherwise, false.
        public static bool operator ==(Padding p1, Padding p2) { return p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom; }
        //
        // Summary:
        //     Tests whether two specified System.Windows.Forms.Padding objects are not equivalent.
        //
        // Parameter:
        //   p1:
        //     A System.Windows.Forms.Padding to test.
        //
        //   p2:
        //     A System.Windows.Forms.Padding to test.
        //
        // Return result:
        //     true if the two System.Windows.Forms.Padding objects are different; otherwise,
        //     false.
        public static bool operator !=(Padding p1, Padding p2) {
            return p1.Left != p2.Left || p1.Top != p2.Top || p1.Right != p2.Right || p1.Bottom != p2.Bottom;
        }
    }
}

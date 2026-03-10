using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides information about a DpiChanged event.
    /// </summary>
    public sealed class DpiChangedEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DpiChangedEventArgs"/> class.
        /// </summary>
        /// <param name="old">The old.</param>
        /// <param name="m">The m.</param>
        /// &lt;devdov&gt;
        /// Parameter units are pixels(dots) per inch.
        internal DpiChangedEventArgs(int old, Message m)
        {

        }

        /// <summary>
        /// Gets the device dpi new.
        /// </summary>
        /// <value>The device dpi new.</value>
        public int DeviceDpiNew { get; }

        /// <summary>
        /// Gets the device dpi old.
        /// </summary>
        /// <value>The device dpi old.</value>
        public int DeviceDpiOld { get; }

        /// <summary>
        /// Gets the suggested rectangle.
        /// </summary>
        /// <value>The suggested rectangle.</value>
        public Rectangle SuggestedRectangle { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";

    }
}

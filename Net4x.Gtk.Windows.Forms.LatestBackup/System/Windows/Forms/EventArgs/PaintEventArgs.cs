// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Control.Paint" /> event.
    /// NOTE: Please keep this class consistent with PrintPageEventArgs.
    /// </summary>
    public class PaintEventArgs : EventArgs, IDisposable
    {

        /// <summary>
        /// DC (Display context) for obtaining the graphics object. Used to delay
        /// getting the graphics object until absolutely necessary (for perf reasons)
        /// </summary>
        private readonly IntPtr _dc = IntPtr.Zero;

        /// <summary>
        /// Graphics object with which painting should be done.
        /// </summary>
        private Graphics _graphics;

        /// <summary>
        /// See ResetGraphics()
        /// </summary>
        private GraphicsState _savedGraphicsState;

        private IntPtr oldPal = IntPtr.Zero;

#if DEBUG
        private static readonly TraceSwitch s_paintEventFinalizationSwitch = new TraceSwitch("PaintEventFinalization", "Tracks the creation and finalization of PaintEvent objects");

        internal static string GetAllocationStack()
        {
            if (s_paintEventFinalizationSwitch.TraceVerbose)
            {
                return Environment.StackTrace;
            }
            else
            {
                return "Enabled 'PaintEventFinalization' trace switch to see stack of allocation";
            }
        }
        private readonly string AllocationSite = PaintEventArgs.GetAllocationStack();
#endif

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PaintEventArgs" /> class with the specified graphics and clipping rectangle.
        /// </summary>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> used to paint the item.
        /// </param>
        /// <param name="clipRect">The
        /// <see cref="System.Drawing.Rectangle" /> that represents the rectangle in which to paint.
        /// </param>
        public PaintEventArgs(Graphics graphics, Rectangle clipRect)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            ClipRectangle = clipRect;
        }

        /// <summary>
        /// Internal version of constructor for performance
        /// We try to avoid getting the graphics object until needed
        /// </summary>
        internal PaintEventArgs(IntPtr dc, Rectangle clipRect)
        {
            Debug.Assert(dc != IntPtr.Zero, "dc is not initialized.");

            _dc = dc;
            ClipRectangle = clipRect;
        }

        /// <summary>
        /// Gets the rectangle in which to paint.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Rectangle" /> in which to paint.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle ClipRectangle { get; }

        /// <summary>
        /// Gets the graphics used to paint.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> object used to paint. The
        /// <see cref="System.Drawing.Graphics" /> object provides methods for drawing objects on the display device.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Graphics Graphics
        {
            get
            {

                return _graphics;
            }
        }

        /// <summary>
        /// Gets the HDC this paint event is connected to.  If there is no associated
        /// HDC, or the GDI+ Graphics object has been created (meaning GDI+ now owns the
        /// HDC), 0 is returned.
        /// </summary>
        internal IntPtr HDC => _graphics == null ? _dc : IntPtr.Zero;

        /// <summary>
        /// Disposes of the resources (other than memory) used by the
        /// <see cref="PaintEventArgs" />.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If ControlStyles.AllPaintingInWmPaint, we call this method
        /// after OnPaintBackground so it appears to OnPaint that it's getting a fresh
        /// Graphics.  We want to make sure AllPaintingInWmPaint is purely an optimization,
        /// and doesn't change behavior, so we need to make sure any clipping regions established
        /// in OnPaintBackground don't apply to OnPaint.
        /// </summary>
        internal void ResetGraphics()
        {

        }

        /// <summary>
        /// Releases the unmanaged resources used by the
        /// <see cref="System.Windows.Forms.PaintEventArgs" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {

            if (oldPal != IntPtr.Zero && _dc != IntPtr.Zero)
            {
                //SafeNativeMethods.SelectPalette(new HandleRef(this, _dc), new HandleRef(this, oldPal), 0);
                oldPal = IntPtr.Zero;
            }
        }

    }
}

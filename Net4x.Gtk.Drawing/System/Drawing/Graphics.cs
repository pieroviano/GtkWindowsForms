using Cairo;
using Gdk;
using Pango;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace System.Drawing
{
    /// <summary>
    /// Encapsulates a GDI+ drawing surface. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public sealed class Graphics : MarshalByRefObject, IDeviceContext, IDisposable
    {

        public Cairo.Context context;

        private Gdk.Rectangle rectangle;

        private Gtk.Widget widget;

        public static Cairo.Context Context;

        internal double diff_left { get; set; }

        internal double diff_top { get; set; }
        internal Graphics(Gtk.Widget widget, Cairo.Context context, Gdk.Rectangle rectangle)
        {
            Graphics.Context = context;

            this.widget = widget;
            this.context = context;
            this.rectangle = rectangle;
            this.Clip = new Region(new Rectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height));
        }

        internal Graphics(Cairo.Context context, Gdk.Rectangle rectangle)
        {
            this.context = context;
            this.rectangle = rectangle;
            this.Clip = new Region(new Rectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height));
        }

        public delegate bool DrawImageAbort(IntPtr callbackdata);

        public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);
        /// <summary>
        /// Gets or sets a
        /// <see cref="System.Drawing.Region" /> that limits the drawing region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.Region" /> that limits the portion of this
        /// <see cref="System.Drawing.Graphics" /> that is currently available for drawing.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public Region Clip
        {
            get;
            set;
        }
        /// <summary>
        /// Gets a
        /// <see cref="System.Drawing.RectangleF" /> structure that bounds the clipping region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.RectangleF" /> structure that represents a bounding rectangle for the clipping region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public RectangleF ClipBounds
        {
            get
            {
                return new RectangleF(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
            }

        }
        /// <summary>
        /// Gets a value that specifies how composited images are drawn to this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// This property specifies a member of the
        /// <see cref="System.Drawing.Drawing2D.CompositingMode" />
        /// enumeration. The default is
        /// <see cref="System.Drawing.Drawing2D.CompositingMode.SourceOver" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public CompositingMode CompositingMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the rendering quality of composited images drawn to this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// This property specifies a member of the
        /// <see cref="System.Drawing.Drawing2D.CompositingQuality" />
        /// enumeration. The default is
        /// <see cref="System.Drawing.Drawing2D.CompositingQuality.Default" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public CompositingQuality CompositingQuality
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the horizontal resolution of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// The value, in dots per inch, for the horizontal resolution supported by this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public float DpiX
        {
            get;
        }
        /// <summary>
        /// Gets the vertical resolution of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// The value, in dots per inch, for the vertical resolution supported by this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public float DpiY
        {
            get;
        }
        /// <summary>
        /// Gets or sets the interpolation mode associated with this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Drawing2D.InterpolationMode" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public InterpolationMode InterpolationMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets a value indicating whether the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> is empty.
        /// </summary>
        /// <returns>true if the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> is empty; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsClipEmpty
        {
            get
            {
                return false;
            }

        }
        /// <summary>
        /// Gets a value indicating whether the visible clipping region of this
        /// <see cref="System.Drawing.Graphics" /> is empty.
        /// </summary>
        /// <returns>true if the visible portion of the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> is empty; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisibleClipEmpty
        {
            get;
        }
        /// <summary>
        /// Gets or sets the scaling between world units and page units for this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// This property specifies a value for the scaling between world units and page units for this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public float PageScale
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the unit of measure used for page coordinates in this
        /// <see cref="System.Drawing.Graphics" />
        /// .
        /// </summary>
        /// <returns>
        /// One of the
        /// <see cref="System.Drawing.GraphicsUnit" /> values other than
        /// <see cref="System.Drawing.GraphicsUnit.World" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        /// <see cref="System.Drawing.Graphics.PageUnit" /> is set to
        /// <see cref="System.Drawing.GraphicsUnit.World" />, which is not a physical unit.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public GraphicsUnit PageUnit
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or set a value specifying how pixels are offset during rendering of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// This property specifies a member of the
        /// <see cref="System.Drawing.Drawing2D.PixelOffsetMode" />
        /// enumeration
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public PixelOffsetMode PixelOffsetMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the rendering origin of this
        /// <see cref="System.Drawing.Graphics" /> for dithering and for hatch brushes.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> structure that represents the dither origin for 8-bits-per-pixel and 16-bits-per-pixel dithering and is also used to set the origin for hatch brushes.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public Point RenderingOrigin
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the rendering quality for this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Drawing2D.SmoothingMode" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public SmoothingMode SmoothingMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the gamma correction value for rendering text.
        /// </summary>
        /// <returns>The gamma correction value used for rendering antialiased and ClearType text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public int TextContrast
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }

        }
        /// <summary>
        /// Gets or sets the rendering mode for text associated with this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Drawing.Text.TextRenderingHint" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }

        }
        /// <summary>
        /// Gets or sets a copy of the geometric world transformation for this
        /// <see cref="System.Drawing.Graphics" />
        /// .
        /// </summary>
        /// <returns>
        /// A copy of the
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> that represents the geometric world transformation for this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public Drawing2D.Matrix Transform
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }

        }
        /// <summary>
        /// Gets the bounding rectangle of the visible clipping region of this
        /// <see cref="System.Drawing.Graphics" />
        /// .
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.RectangleF" /> structure that represents a bounding rectangle for the visible clipping region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Drawing.Printing.PrintingPermission, System.Drawing, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public RectangleF VisibleClipBounds
        {
            get
            {
                return new RectangleF(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
            }

        }
        /// <summary>
        /// Adds a comment to the current
        /// <see cref="System.Drawing.Imaging.Metafile" />
        /// </summary>
        /// <param name="data">Array of bytes that contains the comment.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void AddMetafileComment(byte[] data)
        {
        }
        /// <summary>
        /// Saves a graphics container with the current state of this
        /// <see cref="System.Drawing.Graphics" /> and opens and uses a new graphics container.
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.Drawing2D.GraphicsContainer" /> that represents the state of this
        /// <see cref="System.Drawing.Graphics" /> at the time of the method call.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public GraphicsContainer BeginContainer()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Saves a graphics container with the current state of this
        /// <see cref="System.Drawing.Graphics" /> and opens and uses a new graphics container with the specified scale transformation.
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.Drawing2D.GraphicsContainer" /> that represents the state of this
        /// <see cref="System.Drawing.Graphics" /> at the time of the method call.
        /// </returns>
        /// <param name="dstrect">
        /// <see cref="System.Drawing.Rectangle" /> structure that, together with the
        /// <paramref name="srcrect" /> parameter, specifies a scale transformation for the container.
        /// </param>
        /// <param name="srcrect">
        /// <see cref="System.Drawing.Rectangle" /> structure that, together with the
        /// <paramref name="dstrect" /> parameter, specifies a scale transformation for the container.
        /// </param>
        /// <param name="unit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure for the container.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Saves a graphics container with the current state of this
        /// <see cref="System.Drawing.Graphics" /> and opens and uses a new graphics container with the specified scale transformation.
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.Drawing2D.GraphicsContainer" /> that represents the state of this
        /// <see cref="System.Drawing.Graphics" /> at the time of the method call.
        /// </returns>
        /// <param name="dstrect">
        /// <see cref="System.Drawing.RectangleF" /> structure that, together with the
        /// <paramref name="srcrect" /> parameter, specifies a scale transformation for the new graphics container.
        /// </param>
        /// <param name="srcrect">
        /// <see cref="System.Drawing.RectangleF" /> structure that, together with the
        /// <paramref name="dstrect" /> parameter, specifies a scale transformation for the new graphics container.
        /// </param>
        /// <param name="unit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure for the container.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            throw new System.NotImplementedException();
        }

        internal void SetTranslateWithDifference(double x, double y)
        {
            this.context.Translate(diff_left + x, diff_top + y);
        }

        internal void SetSourcePen(Pen pen, bool isfill)
        {
            if (pen.Brush is SolidBrush sbrush)
            {
                this.context.SetSourceRGBA(sbrush.Color.R / 255f, sbrush.Color.G / 255f, sbrush.Color.B / 255f, sbrush.Color.A / 255f);
            }

            else if (pen.Brush is LinearGradientBrush lbrush)
            {
                double maxsize = Math.Max(diff_left + lbrush.Rectangle.Right, diff_top + lbrush.Rectangle.Bottom); //渐变角度定为方形45度
                using Cairo.LinearGradient gradient = new Cairo.LinearGradient(diff_left + lbrush.Rectangle.Left, diff_top + lbrush.Rectangle.Top, maxsize, maxsize);
                int linearcount = lbrush.LinearColors.Length;
                int idx = 0;
                foreach (Color color in lbrush.LinearColors)
                    gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

                Cairo.Matrix matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
                matrix.Rotate(Math.PI * 45 / 180);//弧度
                gradient.Matrix = matrix;
                using Cairo.Pattern pattern = Cairo.Pattern.Lookup(gradient.Handle, false);
                this.context.SetSource(pattern);
            }

            else if (pen.Brush is HatchBrush hbrush)
            {
                this.context.SetSourceRGBA(hbrush.ForegroundColor.R / 255f, hbrush.ForegroundColor.G / 255f, hbrush.ForegroundColor.B / 255f, hbrush.ForegroundColor.A / 255f);
            }

            else if (pen.Brush is PathGradientBrush pbrush)
            {
                double maxsize = Math.Max(diff_left + pbrush.Rectangle.Right, diff_top + pbrush.Rectangle.Bottom); //渐变角度定为方形45度
                using Cairo.LinearGradient gradient = new Cairo.LinearGradient(diff_left + pbrush.Rectangle.Left, diff_top + pbrush.Rectangle.Top, maxsize, maxsize);
                int linearcount = pbrush.SurroundColors.Length;
                double centeridx = Math.Floor((double)linearcount / 2);
                int idx = 0;
                foreach (Color color in pbrush.SurroundColors)
                {
                    if (idx == centeridx)
                        gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(pbrush.CenterColor.R / 255f, pbrush.CenterColor.G / 255f, pbrush.CenterColor.B / 255f, pbrush.CenterColor.A / 255f));
                    else
                        gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
                }

                Cairo.Matrix matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
                matrix.Rotate(Math.PI * 45 / 180);//弧度
                gradient.Matrix = matrix;
                using Cairo.Pattern pattern = Cairo.Pattern.Lookup(gradient.Handle, false);
                this.context.SetSource(pattern);
            }

            else
            {
                this.context.SetSourceRGBA(pen.Color.R / 255f, pen.Color.G / 255f, pen.Color.B / 255f, pen.Color.A / 255f);
            }

            this.context.LineWidth = pen.Width;
            if (!isfill)
            {
                this.context.LineWidth = Math.Max(1, pen.Width);
                if (pen.DashCap == DashCap.Flat)
                    this.context.LineCap = Cairo.LineCap.Square;
                else if (pen.DashCap == DashCap.Round)
                    this.context.LineCap = Cairo.LineCap.Round;
                else if (pen.DashCap == DashCap.Triangle)
                    this.context.LineCap = Cairo.LineCap.Butt;
                if (pen.DashStyle == DashStyle.Dot)
                    this.context.SetDash(new double[] { 2 }, pen.DashOffset);
                else if (pen.DashStyle == DashStyle.Dash)
                    this.context.SetDash(new double[] { 6 }, pen.DashOffset);
                else if (pen.DashStyle == DashStyle.DashDot)
                    this.context.SetDash(new double[] { 6, 2, 6 }, pen.DashOffset);
            }

        }
        /// <summary>
        /// Clears the entire drawing surface and fills it with the specified background color.
        /// </summary>
        /// <param name="color">
        /// <see cref="System.Drawing.Color" /> structure that represents the background color of the drawing surface.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void Clear(Color color)
        {
            this.context.Save();
            this.context.SetSourceRGB(color.R / 255f, color.G / 255f, color.B / 255f);
            this.SetTranslateWithDifference(0, 0);
            this.context.Paint();
            this.context.Restore();
        }
        /// <summary>
        /// Performs a bit-block transfer of color data, corresponding to a rectangle of pixels, from the screen to the drawing surface of the
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="upperLeftSource">The point at the upper-left corner of the source rectangle.
        /// </param>
        /// <param name="upperLeftDestination">The point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="blockRegionSize">The size of the area to be transferred.
        /// </param>
        /// <exception cref="System.ComponentModel.Win32Exception">The operation failed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
        {
        }
        /// <summary>
        /// Performs a bit-block transfer of color data, corresponding to a rectangle of pixels, from the screen to the drawing surface of the
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="upperLeftSource">The point at the upper-left corner of the source rectangle.
        /// </param>
        /// <param name="upperLeftDestination">The point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="blockRegionSize">The size of the area to be transferred.
        /// </param>
        /// <param name="copyPixelOperation">One of the
        /// <see cref="System.Drawing.CopyPixelOperation" /> values.
        /// </param>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        /// <paramref name="copyPixelOperation" /> is not a member of
        /// <see cref="System.Drawing.CopyPixelOperation" />
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">The operation failed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
        }
        /// <summary>
        /// Performs a bit-block transfer of the color data, corresponding to a rectangle of pixels, from the screen to the drawing surface of the
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="sourceX">The x-coordinate of the point at the upper-left corner of the source rectangle.
        /// </param>
        /// <param name="sourceY">The y-coordinate of the point at the upper-left corner of the source rectangle.
        /// </param>
        /// <param name="destinationX">The x-coordinate of the point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="destinationY">The y-coordinate of the point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="blockRegionSize">The size of the area to be transferred.
        /// </param>
        /// <exception cref="System.ComponentModel.Win32Exception">The operation failed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
        {
        }
        /// <summary>
        /// Performs a bit-block transfer of the color data, corresponding to a rectangle of pixels, from the screen to the drawing surface of the
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="sourceX">The x-coordinate of the point at the upper-left corner of the source rectangle.
        /// </param>
        /// <param name="sourceY">The y-coordinate of the point at the upper-left corner of the source rectangle
        /// </param>
        /// <param name="destinationX">The x-coordinate of the point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="destinationY">The y-coordinate of the point at the upper-left corner of the destination rectangle.
        /// </param>
        /// <param name="blockRegionSize">The size of the area to be transferred.
        /// </param>
        /// <param name="copyPixelOperation">One of the
        /// <see cref="System.Drawing.CopyPixelOperation" /> values.
        /// </param>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        /// <paramref name="copyPixelOperation" /> is not a member of
        /// <see cref="System.Drawing.CopyPixelOperation" />
        /// </exception>
        /// <exception cref="System.ComponentModel.Win32Exception">The operation failed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
        }
        /// <summary>
        /// Releases all resources used by this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Dispose()
        {
        }

        private void DrawArcCore(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            this.context.Save();
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, false);
            this.context.NewPath();
            double radius = Math.Min(width / 2, height / 2);
            this.context.Arc(x + radius, y + radius, radius, Math.PI * startAngle / 180, Math.PI * (startAngle + sweepAngle) / 180);
            this.context.Stroke();
            this.context.Restore();
        }
        /// <summary>
        /// Draws an arc representing a portion of an ellipse specified by a
        /// <see cref="System.Drawing.Rectangle" />
        /// structure.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the arc.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that defines the boundaries of the ellipse.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to ending point of the arc.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArcCore(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws an arc representing a portion of an ellipse specified by a
        /// <see cref="System.Drawing.RectangleF" />
        /// structure.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the arc.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that defines the boundaries of the ellipse.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to ending point of the arc.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArcCore(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws an arc representing a portion of an ellipse specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the arc.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to ending point of the arc.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="rects" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="rects" /> is a zero-length array.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawArcCore(pen, x, y, width, height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws an arc representing a portion of an ellipse specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the arc.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the rectangle that defines the ellipse.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to ending point of the arc.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            DrawArcCore(pen, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Collect the positions of all coordinate points on the Bézier curve
        /// </summary>
        /// <param name="points">
        /// </param>
        /// <returns>
        /// </returns>
        private List<PointF> GetBezierPoints(List<PointF> points)
        {
            float seedNum = 0;
            for (int i = 1; i < points.Count; i++)
            {
                seedNum += Math.Abs(points[i].X - points[i - 1].X) + Math.Abs(points[i].Y - points[i - 1].Y);
            }

            seedNum += seedNum * 0.2f;
            float pStep = 1 / seedNum;
            List<PointF> rpoint = new List<PointF>();
            for (float pTime = 0; pTime <= 1; pTime += pStep)
            {
                List<PointF> lfpr = CalculateBezier(points, pTime);
                PointF fpr = lfpr[0];
                rpoint.Add(fpr);
            }

            return rpoint;
        }

        /// <summary>
        /// Calculate the position of a single coordinate point on the Bézier curve
        /// </summary>
        /// <param name="points">贝塞尔条件坐标集合
        /// </param>
        /// <param name="time">Time factor
        /// </param>
        /// <returns>
        /// </returns>
        private List<PointF> CalculateBezier(List<PointF> points, float time)
        {
            List<PointF> nList = new List<PointF> { };
            int listNum = points.Count;
            if (listNum < 2)
            {
                return points.ToList();
            }

            for (int n = 1; n < listNum; n++)
            {
                float nowX = (points[n].X - points[n - 1].X) * time + points[n - 1].X;
                float nowY = (points[n].Y - points[n - 1].Y) * time + points[n - 1].Y;
                PointF nowP = new PointF(nowX, nowY);
                nList.Add(nowP);
            }

            List<PointF> p = CalculateBezier(nList, time);
            return p;
        }

        private void DrawBeziersCore(Pen pen, PointF[] points)
        {
            List<PointF> data = GetBezierPoints(points.ToList());
            DrawLinesCore(pen, data.ToArray());
        }
        /// <summary>
        /// Draws a Bézier spline defined by four
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> structure that determines the color, width, and style of the curve.
        /// </param>
        /// <param name="pt1">
        /// <see cref="System.Drawing.Point" /> structure that represents the starting point of the curve.
        /// </param>
        /// <param name="pt2">
        /// <see cref="System.Drawing.Point" /> structure that represents the first control point for the curve.
        /// </param>
        /// <param name="pt3">
        /// <see cref="System.Drawing.Point" /> structure that represents the second control point for the curve.
        /// </param>
        /// <param name="pt4">
        /// <see cref="System.Drawing.Point" /> structure that represents the ending point of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            DrawBeziersCore(pen, new PointF[] { new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y), new PointF(pt3.X, pt3.Y), new PointF(pt4.X, pt4.Y) });
        }
        /// <summary>
        /// Draws a Bézier spline defined by four
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the curve.
        /// </param>
        /// <param name="pt1">
        /// <see cref="System.Drawing.PointF" /> structure that represents the starting point of the curve.
        /// </param>
        /// <param name="pt2">
        /// <see cref="System.Drawing.PointF" /> structure that represents the first control point for the curve.
        /// </param>
        /// <param name="pt3">
        /// <see cref="System.Drawing.PointF" /> structure that represents the second control point for the curve.
        /// </param>
        /// <param name="pt4">
        /// <see cref="System.Drawing.PointF" /> structure that represents the ending point of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            DrawBeziersCore(pen, new PointF[] { pt1, pt2, pt3, pt4 });
        }
        /// <summary>
        /// Draws a Bézier spline defined by four ordered pairs of coordinates that represent points.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the curve.
        /// </param>
        /// <param name="x1">The x-coordinate of the starting point of the curve.
        /// </param>
        /// <param name="y1">The y-coordinate of the starting point of the curve.
        /// </param>
        /// <param name="x2">The x-coordinate of the first control point of the curve.
        /// </param>
        /// <param name="y2">The y-coordinate of the first control point of the curve.
        /// </param>
        /// <param name="x3">The x-coordinate of the second control point of the curve.
        /// </param>
        /// <param name="y3">The y-coordinate of the second control point of the curve.
        /// </param>
        /// <param name="x4">The x-coordinate of the ending point of the curve.
        /// </param>
        /// <param name="y4">The y-coordinate of the ending point of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            DrawBeziersCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4) });
        }
        /// <summary>
        /// Draws a series of Bézier splines from an array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the points that determine the curve. The number of points in the array should be a multiple of 3 plus 1, such as 4, 7, or 10.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawBeziers(Pen pen, PointF[] points)
        {
            DrawBeziersCore(pen, points);
        }
        /// <summary>
        /// Draws a series of Bézier splines from an array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that represent the points that determine the curve. The number of points in the array should be a multiple of 3 plus 1, such as 4, 7, or 10.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawBeziers(Pen pen, Point[] points)
        {
            DrawBeziersCore(pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        }

        private void DrawCurveCore(bool isClosePath, bool isfill, Pen pen, PointF[] points, int offset, int numberOfSegments, float tension, FillMode fillmode)
        {
            if (points.Length > 1)
            {
                this.context.Save();
                this.SetTranslateWithDifference(offset, offset);
                this.SetSourcePen(pen, isfill);
                this.context.NewPath();
                this.context.CurveTo(points[0].X, points[0].Y, points[1].X, points[1].Y, points[2].X, points[2].Y);
                if (isClosePath)
                    this.context.ClosePath();
                if (isfill)
                {
                    this.context.FillRule = fillmode == FillMode.Winding ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;
                    this.context.Fill();
                }

                else
                    this.context.Stroke();

                this.context.Restore();
            }

        }
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            DrawCurveCore(true, false, pen, points, 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures using a specified tension.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled. This parameter is required but is ignored.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
        }
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of
        /// <see cref="System.Drawing.Point" /> structures using a specified tension.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled. This parameter is required but ignored.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, PointF[] points)
        {
            DrawCurveCore(false, false, pen, points, 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.PointF" /> structures. The drawing begins offset from the beginning of the array.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <param name="offset">Offset from the first element in the array of the
        /// <paramref name="points" /> parameter to the starting point in the curve.
        /// </param>
        /// <param name="numberOfSegments">Number of segments after the starting point to include in the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            DrawCurveCore(false, false, pen, points, offset, numberOfSegments, 0, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.PointF" /> structures using a specified tension. The drawing begins offset from the beginning of the array.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <param name="offset">Offset from the first element in the array of the
        /// <paramref name="points" /> parameter to the starting point in the curve.
        /// </param>
        /// <param name="numberOfSegments">Number of segments after the starting point to include in the curve.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            DrawCurveCore(false, false, pen, points, offset, numberOfSegments, tension, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.PointF" /> structures using a specified tension.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the points that define the curve.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            DrawCurveCore(false, false, pen, points, 0, 0, tension, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, Point[] points)
        {
            DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.Point" /> structures using a specified tension.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <param name="offset">Offset from the first element in the array of the
        /// <paramref name="points" /> parameter to the starting point in the curve.
        /// </param>
        /// <param name="numberOfSegments">Number of segments after the starting point to include in the curve.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), offset, numberOfSegments, tension, FillMode.Winding);
        }
        /// <summary>
        /// Draws a cardinal spline through a specified array of
        /// <see cref="System.Drawing.Point" /> structures using a specified tension.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and height of the curve.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, FillMode.Winding);
        }
        /// <summary>
        /// Draws an ellipse specified by a bounding
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the ellipse.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that defines the boundaries of the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            DrawEllipseCore(pen, rect.X, rect.Y, rect.Width, rect.Height, false, FillMode.Winding);
        }
        /// <summary>
        /// Draws an ellipse defined by a bounding
        /// <see cref="System.Drawing.RectangleF" />
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the ellipse.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that defines the boundaries of the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            DrawEllipseCore(pen, rect.X, rect.Y, rect.Width, rect.Height, false, FillMode.Winding);
        }
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle specified by coordinates for the upper-left corner of the rectangle, a height, and a width.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the ellipse.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            DrawEllipseCore(pen, x, y, width, height, false, FillMode.Winding);
        }
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle specified by a pair of coordinates, a height, and a width.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the ellipse.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            DrawEllipseCore(pen, x, y, width, height, false, FillMode.Winding);
        }

        private void DrawEllipseCore(Pen pen, float x, float y, float width, float height, bool isfill, FillMode fillmode)
        {
            this.context.Save();
            this.SetTranslateWithDifference(x + width / 2, y + height / 2);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();
            float r = (width + height) / 4;
            double rs = Math.Min(0.1, 2 / r);
            for (double t = 0; t < 2 * Math.PI; t += rs)
            {
                double x2_1 = width * Math.Cos(t) / 2;
                double y2_1 = height * Math.Sin(t) / 2;
                this.context.LineTo(x2_1, y2_1);
            }

            this.context.ClosePath();
            if (isfill)
            {
                this.context.FillRule = fillmode == FillMode.Winding ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;
                this.context.Fill();
            }

            else
            {
                this.context.Stroke();
            }

            this.context.Restore();
            QueueDraw();
        }
        /// <summary>
        /// Draws the image represented by the specified
        /// <see cref="System.Drawing.Icon" /> within the area specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="icon">
        /// <see cref="System.Drawing.Icon" /> to draw.
        /// </param>
        /// <param name="targetRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the resulting image on the display surface. The image contained in the
        /// <paramref name="icon" /> parameter is scaled to the dimensions of this rectangular area.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="icon" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            DrawImage(new Bitmap(icon.PixbufData), targetRect);
        }
        /// <summary>
        /// Draws the image represented by the specified
        /// <see cref="System.Drawing.Icon" /> at the specified coordinates.
        /// </summary>
        /// <param name="icon">
        /// <see cref="System.Drawing.Icon" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="icon" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawIcon(Icon icon, int x, int y)
        {
            DrawImage(new Bitmap(icon.PixbufData), new Point(x, y));
        }
        /// <summary>
        /// Draws the image represented by the specified
        /// <see cref="System.Drawing.Icon" /> without scaling the image.
        /// </summary>
        /// <param name="icon">
        /// <see cref="System.Drawing.Icon" /> to draw.
        /// </param>
        /// <param name="targetRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the resulting image. The image is not scaled to fit this rectangle, but retains its original size. If the image is larger than the rectangle, it is clipped to fit inside it.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="icon" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            DrawImage(new Bitmap(icon.PixbufData), targetRect);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" />, using its original physical size, at the specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="point">
        /// <see cref="System.Drawing.Point" /> structure that represents the location of the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImage(Image image, Point point)
        {
            DrawImageScaledCore(image, new Rectangle(point.X, point.Y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" />, using its original physical size, at the specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="point">
        /// <see cref="System.Drawing.PointF" /> structure that represents the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImage(Image image, PointF point)
        {
            DrawImageScaledCore(image, new Rectangle((int)point.X, (int)point.Y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified shape and size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, PointF[] destPoints)
        {
            DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }

        private void DrawImageUnscaledCore(Image image, int x, int y, int width, int height, bool clipped = false)
        {
            Gdk.Pixbuf img = new Gdk.Pixbuf(image.PixbufData);
            if (width == 0)
                width = img.Width;
            if (height == 0)
                height = img.Height;
            using (var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height))
            {
                Gdk.Pixbuf newimg = new Gdk.Pixbuf(surface, 0, 0, width, height);
                img.CopyArea(x, y, width, height, newimg, 0, 0);
                this.context.Save();
                this.SetTranslateWithDifference(x, y);
                Gdk.CairoHelper.SetSourcePixbuf(this.context, newimg, 0, 0);

                using (var p = this.context.GetSource())
                {
                    if (p is Cairo.SurfacePattern pattern)
                    {
                        if (this.CompositingQuality == CompositingQuality.HighSpeed)
                        {
                            pattern.Filter = Cairo.Filter.Fast;
                        }

                        else if (this.CompositingQuality == CompositingQuality.HighQuality)
                        {
                            pattern.Filter = Cairo.Filter.Good;
                        }

                        else
                            pattern.Filter = Cairo.Filter.Best;
                    }

                }

                this.context.Paint();
                this.context.Restore();
            }

        }

        private void DrawImageScaledCore(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            Gdk.Pixbuf img = new Gdk.Pixbuf(image.PixbufData);
            if (srcWidth == 0)
                srcWidth = img.Width;
            if (srcHeight == 0)
                srcHeight = img.Height;
            if (destRect.Width == 0)
                destRect.Width = img.Width;
            if (destRect.Height == 0)
                destRect.Height = img.Height;
            using (var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, destRect.Width, destRect.Height))
            {
                Gdk.Pixbuf scaleimg = new Gdk.Pixbuf(surface, 0, 0, destRect.Width, destRect.Height);

                img.Scale(scaleimg, 0, 0, destRect.Width, destRect.Height, srcX, srcY, destRect.Width / srcWidth, destRect.Height / srcHeight, Gdk.InterpType.Tiles);
                this.context.Save();
                this.SetTranslateWithDifference(destRect.X, destRect.Y);
                Gdk.CairoHelper.SetSourcePixbuf(this.context, scaleimg, 0, 0);
                using (var p = this.context.GetSource())
                {
                    if (p is Cairo.SurfacePattern pattern)
                    {
                        if (this.CompositingQuality == CompositingQuality.HighSpeed)
                        {
                            pattern.Filter = Cairo.Filter.Fast;
                        }

                        else if (this.CompositingQuality == CompositingQuality.HighQuality)
                        {
                            pattern.Filter = Cairo.Filter.Good;
                        }

                        else
                            pattern.Filter = Cairo.Filter.Best;
                    }

                }

                this.context.Paint();
                this.context.Restore();
                QueueDraw();
            }

        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <param name="callbackData">
        /// Value specifying additional data for the
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate to use when checking whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)" />
        /// method.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
        {
            DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, new IntPtr(callbackData));
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified shape and size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Point[] destPoints)
        {
            DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <param name="callbackData">
        /// Value specifying additional data for the
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate to use when checking whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)" />
        /// method.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
        {
            DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, new IntPtr(callbackData));
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImageScaledCore(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for
        /// <paramref name="image" />
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttrs">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <param name="callbackData">
        /// Value specifying additional data for the
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate to use when checking whether to stop execution of the DrawImage method.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttrs">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttrs">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcX">The x-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcY">The y-coordinate of the upper-left corner of the portion of the source image to draw.
        /// </param>
        /// <param name="srcWidth">Width of the portion of the source image to draw.
        /// </param>
        /// <param name="srcHeight">Height of the portion of the source image to draw.
        /// </param>
        /// <param name="srcUnit">Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used to determine the source rectangle.
        /// </param>
        /// <param name="imageAttrs">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies recoloring and gamma information for the
        /// <paramref name="image" /> object.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate that specifies a method to call during the drawing of the image. This method is called frequently to check whether to stop execution of the
        /// <see cref="System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)" />
        /// method according to application-determined criteria.
        /// </param>
        /// <param name="callbackData">
        /// Value specifying additional data for the
        /// <see cref="System.Drawing.Graphics.DrawImageAbort" /> delegate to use when checking whether to stop execution of the DrawImage method.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImageScaledCore(image, new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height), rect.X, rect.Y, rect.Width, rect.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified portion of the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn image. The image is scaled to fit the rectangle.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, new Rectangle((int)destRect.X, (int)destRect.Y, (int)destRect.Width, (int)destRect.Height), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified image, using its original physical size, at the location specified by a coordinate pair.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, int x, int y)
        {
            DrawImageScaledCore(image, new Rectangle(x, y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws a portion of an image at a specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the
        /// <paramref name="image" />
        /// object to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, new Rectangle(x, y, srcRect.Width + x, srcRect.Height + y), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="width">Width of the drawn image.
        /// </param>
        /// <param name="height">Height of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            DrawImageScaledCore(image, new Rectangle(x, y, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" />, using its original physical size, at the specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, float x, float y)
        {
            DrawImageScaledCore(image, new Rectangle((int)x, (int)y, image.Width, image.Height), x, y, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws a portion of an image at a specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the units of measure used by the
        /// <paramref name="srcRect" /> parameter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            DrawImageScaledCore(image, new Rectangle((int)x, (int)y, image.Width, image.Height), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws the specified
        /// <see cref="System.Drawing.Image" /> at the specified location and with the specified size.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="width">Width of the drawn image.
        /// </param>
        /// <param name="height">Height of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImageScaledCore(image, new Rectangle((int)x, (int)y, (int)width, (int)height), 0, 0, width, height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
        }
        /// <summary>
        /// Draws a specified image using its original physical size at a specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="point">
        /// <see cref="System.Drawing.Point" /> structure that specifies the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImageUnscaledCore(image, point.X, point.Y, image.Width, image.Height);
        }
        /// <summary>
        /// Draws a specified image using its original physical size at a specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> that specifies the upper-left corner of the drawn image. The X and Y properties of the rectangle specify the upper-left corner. The Width and Height properties are ignored.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImageUnscaledCore(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Draws the specified image using its original physical size at the location specified by a coordinate pair.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImageUnscaledCore(image, x, y, image.Width, image.Height);
        }
        /// <summary>
        /// Draws a specified image using its original physical size at a specified location.
        /// </summary>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.
        /// </param>
        /// <param name="width">Not used.
        /// </param>
        /// <param name="height">Not used.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            DrawImageUnscaledCore(image, x, y, width, height);
        }
        /// <summary>
        /// Draws the specified image without scaling and clips it, if necessary, to fit in the specified rectangle.
        /// </summary>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to draw.
        /// </param>
        /// <param name="rect">The
        /// <see cref="System.Drawing.Rectangle" /> in which to draw the image.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            DrawImageUnscaledCore(image, rect.X, rect.Y, rect.Width, rect.Height, true);
        }

        private void DrawLinesCore(Pen pen, PointF[] points)
        {
            if (points.Length > 0)
            {
                this.context.Save();
                this.SetTranslateWithDifference(0, 0);
                this.SetSourcePen(pen, false);
                this.context.NewPath();
                foreach (PointF p in points)
                {
                    this.context.LineTo(p.X, p.Y);
                }

                this.context.Stroke();
                this.context.Restore();
                QueueDraw();
            }

        }
        /// <summary>
        /// Draws a line connecting two
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line.
        /// </param>
        /// <param name="pt1">
        /// <see cref="System.Drawing.Point" /> structure that represents the first point to connect.
        /// </param>
        /// <param name="pt2">
        /// <see cref="System.Drawing.Point" /> structure that represents the second point to connect.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        /// <summary>
        /// Draws a line connecting two
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line.
        /// </param>
        /// <param name="pt1">
        /// <see cref="System.Drawing.PointF" /> structure that represents the first point to connect.
        /// </param>
        /// <param name="pt2">
        /// <see cref="System.Drawing.PointF" /> structure that represents the second point to connect.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
        /// <summary>
        /// Draws a line connecting the two points specified by the coordinate pairs.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line.
        /// </param>
        /// <param name="x1">The x-coordinate of the first point.
        /// </param>
        /// <param name="y1">The y-coordinate of the first point.
        /// </param>
        /// <param name="x2">The x-coordinate of the second point.
        /// </param>
        /// <param name="y2">The y-coordinate of the second point.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            DrawLinesCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
        }
        /// <summary>
        /// Draws a line connecting the two points specified by the coordinate pairs.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line.
        /// </param>
        /// <param name="x1">The x-coordinate of the first point.
        /// </param>
        /// <param name="y1">The y-coordinate of the first point.
        /// </param>
        /// <param name="x2">The x-coordinate of the second point.
        /// </param>
        /// <param name="y2">The y-coordinate of the second point.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            DrawLinesCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
        }
        /// <summary>
        /// Draws a series of line segments that connect an array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line segments.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the points to connect.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawLines(Pen pen, PointF[] points)
        {
            DrawLinesCore(pen, points);
        }
        /// <summary>
        /// Draws a series of line segments that connect an array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the line segments.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that represent the points to connect.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawLines(Pen pen, Point[] points)
        {
            DrawLinesCore(pen, Array.ConvertAll<Point, PointF>(points, o => new PointF(o.X, o.Y)));
        }
        /// <summary>
        /// Draws a
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the path.
        /// </param>
        /// <param name="path">
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="path" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            DrawPathCore(pen, path, false);
        }

        private void DrawPathCore(Pen pen, GraphicsPath path, bool isfill)
        {
            this.context.Save();
            path.Context = this.context;
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();
            foreach (object o in path.list)
            {
                if (o is GraphicsPath.FigureMode start && start.start == true)
                {
                    this.context.NewSubPath();
                }

                else if (o is GraphicsPath.ArcMode arc)
                {
                    double rw = arc.rect.Width / 2;
                    double rh = arc.rect.Height / 2;
                    double ra = Math.Min(rw, rh);
                    this.context.Arc(arc.rect.X + rw, arc.rect.Y + rh, ra, Math.PI * arc.startAngle / 180, Math.PI * (arc.startAngle + arc.sweepAngle) / 180);
                }

                else if (o is GraphicsPath.BezierMode bezier)
                {
                    this.context.MoveTo(bezier.pt1.X, bezier.pt1.Y);
                    List<PointF> data = GetBezierPoints(new List<PointF>() { bezier.pt1, bezier.pt2, bezier.pt3, bezier.pt4 });
                    foreach (PointF point in data)
                    {
                        this.context.LineTo(point.X, point.Y);
                    }

                }

                else if (o is GraphicsPath.BeziersMode beziers)
                {
                    List<PointF> data = GetBezierPoints(beziers.points.ToList());
                    foreach (PointF point in data)
                    {
                        this.context.LineTo(point.X, point.Y);
                    }

                }

                else if (o is GraphicsPath.ClosedCurveMode closedcurve)
                {
                    this.context.CurveTo(closedcurve.points[0].X, closedcurve.points[0].Y, closedcurve.points[1].X, closedcurve.points[1].Y, closedcurve.points[2].X, closedcurve.points[2].Y);
                    this.context.FillRule = closedcurve.fillmode == FillMode.Winding ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;
                    this.context.ClosePath();
                    this.context.NewSubPath();
                }

                else if (o is GraphicsPath.CurveMode curve)
                {
                    this.context.CurveTo(curve.points[0].X + curve.offset, curve.points[0].Y + curve.offset, curve.points[1].X + curve.offset, curve.points[1].Y + curve.offset, curve.points[2].X + curve.offset, curve.points[2].Y + curve.offset);
                }

                else if (o is GraphicsPath.EllipseMode ellipse)
                {
                    this.context.NewSubPath();
                    float r = (ellipse.rect.Width + ellipse.rect.Height) / 4;
                    double rs = Math.Min(0.1, 2 / r);
                    for (double t = 0; t < 2 * Math.PI; t += rs)
                    {
                        double x2_1 = ellipse.rect.Width * Math.Cos(t) / 2;
                        double y2_1 = ellipse.rect.Height * Math.Sin(t) / 2;
                        this.context.LineTo(x2_1 + ellipse.rect.X + ellipse.rect.Width / 2, y2_1 + ellipse.rect.Y + ellipse.rect.Height / 2);
                    }

                    this.context.ClosePath();
                    this.context.NewSubPath();
                }

                else if (o is GraphicsPath.LineMode line)
                {
                    this.context.LineTo(line.pt1.X, line.pt1.Y);
                    this.context.LineTo(line.pt2.X, line.pt2.Y);
                }

                else if (o is GraphicsPath.LinesMode lines)
                {
                    this.context.MoveTo(lines.points[0].X, lines.points[0].Y);
                    foreach (PointF p in lines.points)
                    {
                        this.context.LineTo(p.X, p.Y);
                    }

                }

                else if (o is GraphicsPath.PieMode pie)
                {
                    this.context.NewSubPath();
                    double rw = pie.rect.Width / 2;
                    double rh = pie.rect.Height / 2;
                    double ra = Math.Min(rw, rh);
                    this.context.Arc(pie.rect.X + rw, pie.rect.Y + rh, ra, Math.PI * pie.startAngle / 180, Math.PI * (pie.startAngle + pie.sweepAngle) / 180);
                    this.context.LineTo(pie.rect.X + rw, pie.rect.Y + rh);
                    this.context.ClosePath();
                    this.context.NewSubPath();
                }

                else if (o is GraphicsPath.PolygonMode polygon)
                {
                    this.context.NewSubPath();
                    foreach (PointF p in polygon.points)
                    {
                        this.context.LineTo(p.X, p.Y);
                    }

                    this.context.ClosePath();
                    this.context.NewSubPath();
                }

                else if (o is GraphicsPath.RectangleMode rectangle)
                {
                    this.context.Rectangle(rectangle.rect.X, rectangle.rect.Y, rectangle.rect.Width, rectangle.rect.Height);
                }

                else if (o is GraphicsPath.RectanglesMode rectangles)
                {
                    foreach (RectangleF rect in rectangles.rects)
                    {
                        this.context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    }

                }

                else if (o is GraphicsPath.StringMode str)
                {
                    string text = str.text;
                    if (str.layoutRect.Width > 0)
                    {
                        while (text.Length > 0 && this.context.TextExtents(text).Width > str.layoutRect.Width)
                            text = text.Substring(0, text.Length - 1);
                    }

                    float textSize = str.emSize < 1 ? 14f : str.emSize;
                    FontFamily font = str.family;
                    string family = font?.Name;
                    if (this.widget != null)
                    {
                        Pango.Context pangocontext = this.widget.PangoContext;
                        if (string.IsNullOrWhiteSpace(family) || pangocontext.Families.Any(f => f.Name == family) == false)
                        {
                            family = pangocontext.FontDescription.Family;
                        }

                    }

                    this.context.SelectFontFace(family, str.style == 2 ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, str.style == 1 ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
                    this.context.SetFontSize(textSize);
                    TextExtents textext = this.context.TextExtents(text);
                    this.context.MoveTo(str.layoutRect.X, str.layoutRect.Y + textext.Height);
                    this.context.ShowText(text);
                }

                else if (o is GraphicsPath.PathMode addpath)
                {
                    DrawPath(pen, addpath.path);
                }

                if (path.IsCloseAllFigures == true || (o is GraphicsPath.FigureMode close && close.close == true))
                {
                    this.context.ClosePath();
                }

            }

            if (isfill == true)
                this.context.Fill();
            else
                this.context.Stroke();

            if (path.matrix != null)
            {
                this.context.Matrix = ConvertToMatrix(path.matrix);
            }

            this.context.Restore();
        }

        private Cairo.Matrix ConvertToMatrix(Drawing2D.Matrix matrix)
        {
            Cairo.Matrix CairoMatrix = new Cairo.Matrix(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);
            CairoMatrix.Init(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);

            CairoMatrix.Translate(matrix.OffsetX, matrix.OffsetY);
            CairoMatrix.Scale(matrix.scaleX, matrix.scaleY);
            CairoMatrix.Rotate(matrix.angle);
            CairoMatrix.Multiply(ConvertToMatrix(matrix.multiply));
            if (matrix.invert)
                CairoMatrix.Invert();

            return CairoMatrix;
        }

        private void DrawPieCore(bool isfill, Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            this.context.Save();
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();
            double radius = Math.Min(width / 2, height / 2);
            this.context.MoveTo(x + radius, y + radius);
            this.context.Arc(x + radius, y + radius, radius, Math.PI * startAngle / 180, Math.PI * (startAngle + sweepAngle) / 180);
            this.context.LineTo(x + radius, y + radius);
            this.context.ClosePath();
            if (isfill)
                this.context.Fill();
            else
                this.context.Stroke();
            this.context.Restore();
        }
        /// <summary>
        /// Draws a pie shape defined by an ellipse specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure and two radial lines.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the pie shape.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="startAngle">Angle measured in degrees clockwise from the x-axis to the first side of the pie shape.
        /// </param>
        /// <param name="sweepAngle">Angle measured in degrees clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie shape.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws a pie shape defined by an ellipse specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure and two radial lines.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the pie shape.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that represents the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="startAngle">Angle measured in degrees clockwise from the x-axis to the first side of the pie shape.
        /// </param>
        /// <param name="sweepAngle">Angle measured in degrees clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie shape.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws a pie shape defined by an ellipse specified by a coordinate pair, a width, a height, and two radial lines.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the pie shape.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="startAngle">Angle measured in degrees clockwise from the x-axis to the first side of the pie shape.
        /// </param>
        /// <param name="sweepAngle">Angle measured in degrees clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie shape.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Draws a pie shape defined by an ellipse specified by a coordinate pair, a width, a height, and two radial lines.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the pie shape.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse from which the pie shape comes.
        /// </param>
        /// <param name="startAngle">Angle measured in degrees clockwise from the x-axis to the first side of the pie shape.
        /// </param>
        /// <param name="sweepAngle">Angle measured in degrees clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie shape.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            DrawPieCore(false, pen, x, y, width, height, startAngle, sweepAngle);
        }

        private void DrawPolygonCore(bool isfill, Pen pen, PointF[] points, FillMode fillmode)
        {
            if (points.Length > 0)
            {
                this.context.Save();
                this.SetTranslateWithDifference(0, 0);
                this.SetSourcePen(pen, isfill);
                this.context.NewPath();
                foreach (PointF p in points)
                {
                    this.context.LineTo(p.X, p.Y);
                }

                this.context.ClosePath();
                if (isfill)
                {
                    this.context.FillRule = fillmode == FillMode.Winding ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;
                    this.context.Fill();
                }

                else
                    this.context.Stroke();
                this.context.Restore();
            }

        }
        /// <summary>
        /// Draws a polygon defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the polygon.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the vertices of the polygon.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            DrawPolygonCore(false, pen, points, FillMode.Winding);
        }
        /// <summary>
        /// Draws a polygon defined by an array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the polygon.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that represent the vertices of the polygon.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawPolygon(Pen pen, Point[] points)
        {
            DrawPolygonCore(false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), FillMode.Winding);
        }

        private void DrawRectangleCore(bool isfill, Pen pen, float x, float y, float width, float height)
        {
            this.context.Save();
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();
            this.context.Rectangle(x, y, width, height);
            if (isfill)
                this.context.Fill();
            else
                this.context.Stroke();
            this.context.Restore();
            QueueDraw();
        }
        /// <summary>
        /// Draws a rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="pen">A
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the rectangle.
        /// </param>
        /// <param name="rect">A
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the rectangle to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangleCore(false, pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(Pen pen, RectangleF rect)
        {
            DrawRectangleCore(false, pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Draws a rectangle specified by a coordinate pair, a width, and a height.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the rectangle.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.
        /// </param>
        /// <param name="width">Width of the rectangle to draw.
        /// </param>
        /// <param name="height">Height of the rectangle to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            DrawRectangleCore(false, pen, x, y, width, height);
        }
        /// <summary>
        /// Draws a rectangle specified by a coordinate pair, a width, and a height.
        /// </summary>
        /// <param name="pen">A
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the rectangle.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.
        /// </param>
        /// <param name="width">The width of the rectangle to draw.
        /// </param>
        /// <param name="height">The height of the rectangle to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            DrawRectangleCore(false, pen, x, y, width, height);
        }
        /// <summary>
        /// Draws a series of rectangles specified by
        /// <see cref="System.Drawing.RectangleF" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the outlines of the rectangles.
        /// </param>
        /// <param name="rects">Array of
        /// <see cref="System.Drawing.RectangleF" /> structures that represent the rectangles to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="rects" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="rects" /> is a zero-length array.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            foreach (RectangleF rect in rects)
                DrawRectangle(pen, rect);

        }
        /// <summary>
        /// Draws a series of rectangles specified by
        /// <see cref="System.Drawing.Rectangle" /> structures.
        /// </summary>
        /// <param name="pen">
        /// <see cref="System.Drawing.Pen" /> that determines the color, width, and style of the outlines of the rectangles.
        /// </param>
        /// <param name="rects">Array of
        /// <see cref="System.Drawing.Rectangle" /> structures that represent the rectangles to draw.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="pen" /> is null.-or-
        /// <paramref name="rects" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="rects" /> is a zero-length array.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            foreach (Rectangle rect in rects)
                DrawRectangle(pen, rect);
        }
        /// <summary>
        /// Draws the specified text string at the specified location with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects.
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="point">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the upper-left corner of the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            DrawString(s, font, brush, new RectangleF(point.X + this.rectangle.X, point.Y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), new StringFormat());
        }
        /// <summary>
        /// Draws the specified text string at the specified location with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects using the formatting attributes of the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="point">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the upper-left corner of the drawn text.
        /// </param>
        /// <param name="format">
        /// <see cref="System.Drawing.StringFormat" /> that specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(point.X + this.rectangle.X, point.Y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), format);
        }
        /// <summary>
        /// Draws the specified text string in the specified rectangle with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects.
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="layoutRectangle">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location of the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, new StringFormat());
        }
        /// <summary>
        /// Draws the specified text string in the specified rectangle with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects using the formatting attributes of the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="layoutRectangle">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location of the drawn text.
        /// </param>
        /// <param name="format">
        /// <see cref="System.Drawing.StringFormat" /> that specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                this.context.Save();
                string family = font?.Name;
                float textSize = 14f;
                if (font != null)
                {
                    textSize = font.Size;
                    if (font.Unit == GraphicsUnit.Point)
                        textSize = font.Size * 1 / 72 * 96;
                    if (font.Unit == GraphicsUnit.Inch)
                        textSize = font.Size * 96;

                    if (this.widget != null)
                    {
                        Pango.Context pangocontext = this.widget.PangoContext;
                        if (string.IsNullOrWhiteSpace(family) || pangocontext.Families.Any(f => f.Name == family) == false)
                        {
                            family = pangocontext.FontDescription.Family;
                        }

                    }

                }

                this.context.SetFontSize(textSize);
                this.context.SelectFontFace(family, font.Style.HasFlag(FontStyle.Italic) ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, font.Style.HasFlag(FontStyle.Bold) ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
                TextExtents textext = this.context.TextExtents(text);
                this.SetTranslateWithDifference(layoutRectangle.X, layoutRectangle.Y + textext.Height);
                this.SetSourcePen(new Pen(brush, 1), false);
                this.context.ShowText(text);
                this.context.Stroke();
                this.context.Restore();
                QueueDraw();
            }

        }
        /// <summary>
        /// Draws the specified text string at the specified location with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects.
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn text.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x + this.rectangle.X, y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), new StringFormat());
        }
        /// <summary>
        /// Draws the specified text string at the specified location with the specified
        /// <see cref="System.Drawing.Brush" /> and
        /// <see cref="System.Drawing.Font" /> objects using the formatting attributes of the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <param name="s">String to draw.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the color and texture of the drawn text.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the drawn text.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the drawn text.
        /// </param>
        /// <param name="format">
        /// <see cref="System.Drawing.StringFormat" /> that specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="s" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(x + this.rectangle.X, y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), format);
        }
        /// <summary>
        /// Closes the current graphics container and restores the state of this
        /// <see cref="System.Drawing.Graphics" /> to the state saved by a call to the
        /// <see cref="System.Drawing.Graphics.BeginContainer" /> method.
        /// </summary>
        /// <param name="container">
        /// <see cref="System.Drawing.Drawing2D.GraphicsContainer" /> that represents the container this method restores.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void EndContainer(GraphicsContainer container)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.Point" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display at a specified point using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoint">
        /// <see cref="System.Drawing.PointF" /> structure that specifies the location of the upper-left corner of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structures that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.PointF" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records in a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified parallelogram using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destPoints">Array of three
        /// <see cref="System.Drawing.Point" /> structures that define a parallelogram that determines the size and location of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records of the specified
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="srcUnit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
        }
        /// <summary>
        /// Sends the records of a selected rectangle from a
        /// <see cref="System.Drawing.Imaging.Metafile" />, one at a time, to a callback method for display in a specified rectangle using specified image attributes.
        /// </summary>
        /// <param name="metafile">
        /// <see cref="System.Drawing.Imaging.Metafile" /> to enumerate.
        /// </param>
        /// <param name="destRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the location and size of the drawn metafile.
        /// </param>
        /// <param name="srcRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the portion of the metafile, relative to its upper-left corner, to draw.
        /// </param>
        /// <param name="unit">
        /// Member of the
        /// <see cref="System.Drawing.GraphicsUnit" /> enumeration that specifies the unit of measure used to determine the portion of the metafile that the rectangle specified by the
        /// <paramref name="srcRect" /> parameter contains.
        /// </param>
        /// <param name="callback">
        /// <see cref="System.Drawing.Graphics.EnumerateMetafileProc" /> delegate that specifies the method to which the metafile records are sent.
        /// </param>
        /// <param name="callbackData">
        /// Internal pointer that is required, but ignored. You can pass
        /// <see cref="System.IntPtr.Zero" /> for this parameter.
        /// </param>
        /// <param name="imageAttr">
        /// <see cref="System.Drawing.Imaging.ImageAttributes" /> that specifies image attribute information for the drawn image.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
        }
        /// <summary>
        /// Updates the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to exclude the area specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that specifies the rectangle to exclude from the clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void ExcludeClip(Rectangle rect)
        {
        }
        /// <summary>
        /// Updates the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to exclude the area specified by a
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// <see cref="System.Drawing.Region" /> that specifies the region to exclude from the clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void ExcludeClip(Region region)
        {
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures using the specified fill mode.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, 0, fillmode);
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.PointF" /> structures using the specified fill mode and tension.
        /// </summary>
        /// <param name="brush">A
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that define the spline.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, tension, fillmode);
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, Point[] points)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.Point" /> structures using the specified fill mode.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, fillmode);
        }
        /// <summary>
        /// Fills the interior of a closed cardinal spline curve defined by an array of
        /// <see cref="System.Drawing.Point" /> structures using the specified fill mode and tension.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that define the spline.
        /// </param>
        /// <param name="fillmode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the curve is filled.
        /// </param>
        /// <param name="tension">Value greater than or equal to 0.0F that specifies the tension of the curve.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
        }
        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            DrawEllipseCore(new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height, true, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that represents the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            DrawEllipseCore(new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height, true, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            DrawEllipseCore(new Pen(brush, 0), x, y, width, height, true, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            DrawEllipseCore(new Pen(brush, 0), x, y, width, height, true, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of a
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="path">
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> that represents the path to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="path" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            DrawPathCore(new Pen(brush, 1), path, true);
        }
        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure and two radial lines.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the first side of the pie section.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie section.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse specified by a pair of coordinates, a width, a height, and two radial lines.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the first side of the pie section.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie section.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            DrawPieCore(true, new Pen(brush, 0), x, y, width, height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse specified by a pair of coordinates, a width, a height, and two radial lines.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse from which the pie section comes.
        /// </param>
        /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the first side of the pie section.
        /// </param>
        /// <param name="sweepAngle">Angle in degrees measured clockwise from the
        /// <paramref name="startAngle" /> parameter to the second side of the pie section.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            DrawPieCore(true, new Pen(brush, 0), x, y, width, height, startAngle, sweepAngle);
        }
        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        /// <see cref="System.Drawing.PointF" /> structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the vertices of the polygon to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPolygon(Brush brush, PointF[] points)
        {
            FillPolygon(brush, points, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        /// <see cref="System.Drawing.PointF" /> structures using the specified fill mode.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the vertices of the polygon to fill.
        /// </param>
        /// <param name="fillMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines the style of the fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            DrawPolygonCore(true, new Pen(brush, 0), points, fillMode);
        }
        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        /// <see cref="System.Drawing.Point" /> structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that represent the vertices of the polygon to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPolygon(Brush brush, Point[] points)
        {
            FillPolygon(brush, points, FillMode.Winding);
        }
        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        /// <see cref="System.Drawing.Point" /> structures using the specified fill mode.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="points">Array of
        /// <see cref="System.Drawing.Point" /> structures that represent the vertices of the polygon to fill.
        /// </param>
        /// <param name="fillMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.FillMode" /> enumeration that determines the style of the fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="points" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            DrawPolygonCore(true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), fillMode);
        }
        /// <summary>
        /// Fills the interior of a rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the rectangle to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillRectangle(Brush brush, Rectangle rect)
        {
            DrawRectangleCore(true, new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Fills the interior of a rectangle specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that represents the rectangle to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillRectangle(Brush brush, RectangleF rect)
        {
            DrawRectangleCore(true, new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.
        /// </param>
        /// <param name="width">Width of the rectangle to fill.
        /// </param>
        /// <param name="height">Height of the rectangle to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            DrawRectangleCore(true, new Pen(brush, 0), x, y, width, height);
        }
        /// <summary>
        /// Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.
        /// </param>
        /// <param name="width">Width of the rectangle to fill.
        /// </param>
        /// <param name="height">Height of the rectangle to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            DrawRectangleCore(true, new Pen(brush, 0), x, y, width, height);
        }
        /// <summary>
        /// Fills the interiors of a series of rectangles specified by
        /// <see cref="System.Drawing.RectangleF" />
        /// structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rects">Array of
        /// <see cref="System.Drawing.RectangleF" /> structures that represent the rectangles to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="rects" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="Rects" /> is a zero-length array.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            foreach (RectangleF rect in rects)
                FillRectangle(brush, rect);
        }
        /// <summary>
        /// Fills the interiors of a series of rectangles specified by
        /// <see cref="System.Drawing.Rectangle" />
        /// structures.
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="rects">Array of
        /// <see cref="System.Drawing.Rectangle" /> structures that represent the rectangles to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="rects" /> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="rects" /> is a zero-length array.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            foreach (Rectangle rect in rects)
                FillRectangle(brush, rect);
        }
        /// <summary>
        /// Fills the interior of a
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="brush">
        /// <see cref="System.Drawing.Brush" /> that determines the characteristics of the fill.
        /// </param>
        /// <param name="region">
        /// <see cref="System.Drawing.Region" /> that represents the area to fill.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="brush" /> is null.-or-
        /// <paramref name="region" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void FillRegion(Brush brush, Region region)
        {
        }

        ~Graphics()
        {
        }
        /// <summary>
        /// Creates a new
        /// <see cref="System.Drawing.Graphics" /> from the specified handle to a device context.
        /// </summary>
        /// <returns>This method returns a new
        /// <see cref="System.Drawing.Graphics" /> for the specified device context.
        /// </returns>
        /// <param name="hdc">Handle to a device context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Creates a new
        /// <see cref="System.Drawing.Graphics" /> from the specified handle to a device context and handle to a device.
        /// </summary>
        /// <returns>This method returns a new
        /// <see cref="System.Drawing.Graphics" /> for the specified device context and device.
        /// </returns>
        /// <param name="hdc">Handle to a device context.
        /// </param>
        /// <param name="hdevice">Handle to a device.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns a
        /// <see cref="System.Drawing.Graphics" /> for the specified device context.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Graphics" /> for the specified device context.
        /// </returns>
        /// <param name="hdc">Handle to a device context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdcInternal(IntPtr hdc)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Creates a new
        /// <see cref="System.Drawing.Graphics" /> from the specified handle to a window.
        /// </summary>
        /// <returns>This method returns a new
        /// <see cref="System.Drawing.Graphics" /> for the specified window handle.
        /// </returns>
        /// <param name="hwnd">Handle to a window.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwnd(IntPtr hwnd)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Creates a new
        /// <see cref="System.Drawing.Graphics" /> for the specified windows handle.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Graphics" /> for the specified window handle.
        /// </returns>
        /// <param name="hwnd">Handle to a window.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwndInternal(IntPtr hwnd)
        {
            throw new System.NotImplementedException();
        }

        private static Cairo.ImageSurface imagesurface;

        private static Cairo.Surface simisurface;
        private static Cairo.Context imagecontext;
        /// <summary>
        /// Creates a new
        /// <see cref="System.Drawing.Graphics" /> from the specified
        /// <see cref="System.Drawing.Image" />
        /// </summary>
        /// <returns>
        /// This method returns a new
        /// <see cref="System.Drawing.Graphics" /> for the specified
        /// <see cref="System.Drawing.Image" />
        /// </returns>
        /// <param name="image">
        /// <see cref="System.Drawing.Image" /> from which to create the new
        /// <see cref="System.Drawing.Graphics" />
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="image" /> is null.
        /// </exception>
        /// <exception cref="System.Exception">
        /// <paramref name="image" /> has an indexed pixel format or its format is undefined.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public static Graphics FromImage(Image image)
        {
            int _width = image.Width;
            int _height = image.Height;

            if (_width < 1)
                throw new ArgumentOutOfRangeException("Image.Width不能小于等于0");
            if (_height < 1)
                throw new ArgumentOutOfRangeException("Image.Height不能小于等于0");

            if (imagesurface == null)
                imagesurface = new Cairo.ImageSurface(Cairo.Format.Argb32, _width, _height);

            simisurface?.Dispose();
            simisurface = imagesurface.CreateSimilar(Cairo.Content.ColorAlpha, _width, _height);
            imagecontext?.Dispose();
            imagecontext = new Cairo.Context(simisurface);
            image.Pixbuf = new Pixbuf(simisurface, 0, 0, image.Width, image.Height);
            return new Drawing.Graphics(image, imagecontext, new Gdk.Rectangle(0, 0, _width, _height));
        }
        /// <summary>
        /// Forces execution of all pending graphics operations and returns immediately without waiting for the operations to finish.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Flush()
        {
            Flush(FlushIntention.Flush);
        }
        /// <summary>
        /// Forces execution of all pending graphics operations with the method waiting or not waiting, as specified, to return before the operations finish.
        /// </summary>
        /// <param name="intention">Member of the
        /// <see cref="System.Drawing.Drawing2D.FlushIntention" /> enumeration that specifies whether the method returns immediately or waits for any existing operations to finish.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Flush(FlushIntention intention)
        {
            try
            {
                if (this.widget is Image image && Graphics.simisurface != null && Graphics.simisurface.Status == Cairo.Status.Success)
                {
                    image.Pixbuf = new Pixbuf(Graphics.simisurface, 0, 0, image.Width, image.Height);
                }

            }

            finally
            {
                this.widget?.QueueDraw();
            }

        }

        private void QueueDraw()
        {

        }
        /// <summary>
        /// Gets the cumulative graphics context.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> representing the cumulative graphics context.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetContextInfo()
        {
            return this.context;
        }
        /// <summary>
        /// Gets a handle to the current Windows halftone palette.
        /// </summary>
        /// <returns>Internal pointer that specifies the handle to the palette.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static IntPtr GetHalftonePalette()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Gets the handle to the device context associated with this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>Handle to the device context associated with this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public IntPtr GetHdc()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Gets the nearest color to the specified
        /// <see cref="System.Drawing.Color" /> structure.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="System.Drawing.Color" /> structure that represents the nearest color to the one specified with the
        /// <paramref name="color" /> parameter.
        /// </returns>
        /// <param name="color">
        /// <see cref="System.Drawing.Color" /> structure for which to find a match.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color GetNearestColor(Color color)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Updates the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to the intersection of the current clip region and the specified
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure to intersect with the current clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void IntersectClip(Rectangle rect)
        {
        }
        /// <summary>
        /// Updates the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to the intersection of the current clip region and the specified
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure to intersect with the current clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void IntersectClip(RectangleF rect)
        {
        }
        /// <summary>
        /// Updates the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to the intersection of the current clip region and the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// <see cref="System.Drawing.Region" /> to intersect with the current region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void IntersectClip(Region region)
        {
        }
        /// <summary>
        /// Indicates whether the specified
        /// <see cref="System.Drawing.Point" /> structure is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the point specified by the
        /// <paramref name="point" /> parameter is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="point">
        /// <see cref="System.Drawing.Point" /> structure to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Point point)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the specified
        /// <see cref="System.Drawing.PointF" /> structure is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the point specified by the
        /// <paramref name="point" /> parameter is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="point">
        /// <see cref="System.Drawing.PointF" /> structure to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(PointF point)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the rectangle specified by the
        /// <paramref name="rect" /> parameter is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(Rectangle rect)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the rectangle specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the rectangle specified by the
        /// <paramref name="rect" /> parameter is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public bool IsVisible(RectangleF rect)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the point specified by a pair of coordinates is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the point defined by the
        /// <paramref name="x" /> and
        /// <paramref name="y" /> parameters is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="x">The x-coordinate of the point to test for visibility.
        /// </param>
        /// <param name="y">The y-coordinate of the point to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsVisible(int x, int y)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the rectangle specified by a pair of coordinates, a width, and a height is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the rectangle defined by the
        /// <paramref name="x" />,
        /// <paramref name="y" />,
        /// <paramref name="width" />, and
        /// <paramref name="height" /> parameters is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test for visibility.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test for visibility.
        /// </param>
        /// <param name="width">Width of the rectangle to test for visibility.
        /// </param>
        /// <param name="height">Height of the rectangle to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsVisible(int x, int y, int width, int height)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the point specified by a pair of coordinates is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the point defined by the
        /// <paramref name="x" /> and
        /// <paramref name="y" /> parameters is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="x">The x-coordinate of the point to test for visibility.
        /// </param>
        /// <param name="y">The y-coordinate of the point to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsVisible(float x, float y)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Indicates whether the rectangle specified by a pair of coordinates, a width, and a height is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <returns>
        /// true if the rectangle defined by the
        /// <paramref name="x" />,
        /// <paramref name="y" />,
        /// <paramref name="width" />, and
        /// <paramref name="height" /> parameters is contained within the visible clip region of this
        /// <see cref="System.Drawing.Graphics" />; otherwise, false.
        /// </returns>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to test for visibility.
        /// </param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to test for visibility.
        /// </param>
        /// <param name="width">Width of the rectangle to test for visibility.
        /// </param>
        /// <param name="height">Height of the rectangle to test for visibility.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public bool IsVisible(float x, float y, float width, float height)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Gets an array of
        /// <see cref="System.Drawing.Region" /> objects, each of which bounds a range of character positions within the specified string.
        /// </summary>
        /// <returns>This method returns an array of
        /// <see cref="System.Drawing.Region" /> objects, each of which bounds a range of character positions within the specified string.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="layoutRect">
        /// <see cref="System.Drawing.RectangleF" /> structure that specifies the layout rectangle for the string.
        /// </param>
        /// <param name="stringFormat">
        /// <see cref="System.Drawing.StringFormat" /> that represents formatting information, such as line spacing, for the string.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified by the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" /> and formatted with the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified by the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter and the
        /// <paramref name="stringFormat" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> defines the text format of the string.
        /// </param>
        /// <param name="origin">
        /// <see cref="System.Drawing.PointF" /> structure that represents the upper-left corner of the string.
        /// </param>
        /// <param name="stringFormat">
        /// <see cref="System.Drawing.StringFormat" /> that represents formatting information, such as line spacing, for the string.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" /> within the specified layout area.
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified by the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> defines the text format of the string.
        /// </param>
        /// <param name="layoutArea">
        /// <see cref="System.Drawing.SizeF" /> structure that specifies the maximum layout area for the text.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" /> and formatted with the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified in the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter and the
        /// <paramref name="stringFormat" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> defines the text format of the string.
        /// </param>
        /// <param name="layoutArea">
        /// <see cref="System.Drawing.SizeF" /> structure that specifies the maximum layout area for the text.
        /// </param>
        /// <param name="stringFormat">
        /// <see cref="System.Drawing.StringFormat" /> that represents formatting information, such as line spacing, for the string.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" /> and formatted with the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size of the string, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the
        /// <paramref name="text" /> parameter as drawn with the
        /// <paramref name="font" /> parameter and the
        /// <paramref name="stringFormat" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="layoutArea">
        /// <see cref="System.Drawing.SizeF" /> structure that specifies the maximum layout area for the text.
        /// </param>
        /// <param name="stringFormat">
        /// <see cref="System.Drawing.StringFormat" /> that represents formatting information, such as line spacing, for the string.
        /// </param>
        /// <param name="charactersFitted">Number of characters in the string.
        /// </param>
        /// <param name="linesFilled">Number of text lines in the string.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified in the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the format of the string.
        /// </param>
        /// <param name="width">Maximum width of the string in pixels.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, int width)
        {
            return MeasureString(text, font, width, StringFormat.GenericDefault);
        }
        /// <summary>
        /// Measures the specified string when drawn with the specified
        /// <see cref="System.Drawing.Font" /> and formatted with the specified
        /// <see cref="System.Drawing.StringFormat" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.SizeF" /> structure that represents the size, in the units specified by the
        /// <see cref="System.Drawing.Graphics.PageUnit" /> property, of the string specified in the
        /// <paramref name="text" />
        /// parameter as drawn with the
        /// <paramref name="font" /> parameter and the
        /// <paramref name="stringFormat" /> parameter.
        /// </returns>
        /// <param name="text">String to measure.
        /// </param>
        /// <param name="font">
        /// <see cref="System.Drawing.Font" /> that defines the text format of the string.
        /// </param>
        /// <param name="width">Maximum width of the string.
        /// </param>
        /// <param name="format">
        /// <see cref="System.Drawing.StringFormat" /> that represents formatting information, such as line spacing, for the string.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="font" /> is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            float textSize = 14f;
            string family = font?.Name;
            if (font != null)
            {
                textSize = font.Size;
                if (font.Unit == GraphicsUnit.Point)
                    textSize = font.Size * 1 / 72 * 96;
                if (font.Unit == GraphicsUnit.Inch)
                    textSize = font.Size * 96;

                if (this.widget != null)
                {
                    Pango.Context pangocontext = this.widget.PangoContext;
                    if (string.IsNullOrWhiteSpace(family) || pangocontext.Families.Any(f => f.Name == family) == false)
                    {
                        family = pangocontext.FontDescription.Family;
                    }

                }

            }

            this.context.SelectFontFace(family, font.Italic ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, font.Bold ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
            this.context.SetFontSize(textSize);
            var extents = this.context.TextExtents(text);
            return new SizeF((float)Math.Max(width, extents.Width), (float)extents.Height);
        }
        /// <summary>
        /// Multiplies the world transformation of this
        /// <see cref="System.Drawing.Graphics" /> and specified the
        /// <see cref="System.Drawing.Drawing2D.Matrix" />
        /// </summary>
        /// <param name="matrix">4x4
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> that multiplies the world transformation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void MultiplyTransform(Drawing2D.Matrix matrix)
        {
            this.context.Matrix?.Multiply(ConvertToMatrix(matrix));
        }
        /// <summary>
        /// Multiplies the world transformation of this
        /// <see cref="System.Drawing.Graphics" /> and specified the
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> in the specified order.
        /// </summary>
        /// <param name="matrix">4x4
        /// <see cref="System.Drawing.Drawing2D.Matrix" /> that multiplies the world transformation.
        /// </param>
        /// <param name="order">Member of the
        /// <see cref="System.Drawing.Drawing2D.MatrixOrder" /> enumeration that determines the order of the multiplication.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void MultiplyTransform(Drawing2D.Matrix matrix, MatrixOrder order)
        {
            this.context.Matrix?.Multiply(ConvertToMatrix(matrix));
        }
        /// <summary>
        /// Releases a device context handle obtained by a previous call to the
        /// <see cref="System.Drawing.Graphics.GetHdc" /> method of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void ReleaseHdc()
        {
        }
        /// <summary>
        /// Releases a device context handle obtained by a previous call to the
        /// <see cref="System.Drawing.Graphics.GetHdc" /> method of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="hdc">
        /// Handle to a device context obtained by a previous call to the
        /// <see cref="System.Drawing.Graphics.GetHdc" /> method of this
        /// <see cref="System.Drawing.Graphics" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ReleaseHdc(IntPtr hdc)
        {
        }
        /// <summary>
        /// Releases a handle to a device context.
        /// </summary>
        /// <param name="hdc">Handle to a device context.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReleaseHdcInternal(IntPtr hdc)
        {
        }
        /// <summary>
        /// Resets the clip region of this
        /// <see cref="System.Drawing.Graphics" /> to an infinite region.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void ResetClip()
        {
            this.context.ResetClip();
        }
        /// <summary>
        /// Resets the world transformation matrix of this
        /// <see cref="System.Drawing.Graphics" /> to the identity matrix.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void ResetTransform()
        {
            this.context.Rotate(Math.PI / 180 * _angle * -1);
        }
        /// <summary>
        /// Restores the state of this
        /// <see cref="System.Drawing.Graphics" /> to the state represented by a
        /// <see cref="System.Drawing.Drawing2D.GraphicsState" />
        /// </summary>
        /// <param name="gstate">
        /// <see cref="System.Drawing.Drawing2D.GraphicsState" /> that represents the state to which to restore this
        /// <see cref="System.Drawing.Graphics" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void Restore(GraphicsState gstate)
        {
            this.context.Restore();
        }

        private float _angle = 0;
        /// <summary>
        /// Applies the specified rotation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="angle">Angle of rotation in degrees.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void RotateTransform(float angle)
        {
            this.context.Rotate(Math.PI / 180 * angle);
            _angle = angle;
        }
        /// <summary>
        /// Applies the specified rotation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" /> in the specified order.
        /// </summary>
        /// <param name="angle">Angle of rotation in degrees.
        /// </param>
        /// <param name="order">Member of the
        /// <see cref="System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies whether the rotation is appended or prepended to the matrix transformation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void RotateTransform(float angle, MatrixOrder order)
        {
            this.context.Rotate(Math.PI / 180 * angle);
            _angle = angle;
        }
        /// <summary>
        /// Saves the current state of this
        /// <see cref="System.Drawing.Graphics" /> and identifies the saved state with a
        /// <see cref="System.Drawing.Drawing2D.GraphicsState" />
        /// </summary>
        /// <returns>
        /// This method returns a
        /// <see cref="System.Drawing.Drawing2D.GraphicsState" /> that represents the saved state of this
        /// <see cref="System.Drawing.Graphics" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public GraphicsState Save()
        {
            return new GraphicsState();
        }
        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" /> by prepending it to the object's transformation matrix.
        /// </summary>
        /// <param name="sx">Scale factor in the x direction.
        /// </param>
        /// <param name="sy">Scale factor in the y direction.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void ScaleTransform(float sx, float sy)
        {
            this.context.Scale(sx, sy);
        }
        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" /> in the specified order.
        /// </summary>
        /// <param name="sx">Scale factor in the x direction.
        /// </param>
        /// <param name="sy">Scale factor in the y direction.
        /// </param>
        /// <param name="order">Member of the
        /// <see cref="System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies whether the scaling operation is prepended or appended to the transformation matrix.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            this.context.Scale(sx, sy);
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> that represents the new clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(GraphicsPath path)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the result of the specified operation combining the current clip region and the specified
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" />
        /// </summary>
        /// <param name="path">
        /// <see cref="System.Drawing.Drawing2D.GraphicsPath" /> to combine.
        /// </param>
        /// <param name="combineMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.CombineMode" /> enumeration that specifies the combining operation to use.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the Clip property of the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="g">
        /// <see cref="System.Drawing.Graphics" /> from which to take the new clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(Graphics g)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the result of the specified combining operation of the current clip region and the
        /// <see cref="System.Drawing.Graphics.Clip" /> property of the specified
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="g">
        /// <see cref="System.Drawing.Graphics" /> that specifies the clip region to combine.
        /// </param>
        /// <param name="combineMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.CombineMode" /> enumeration that specifies the combining operation to use.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(Graphics g, CombineMode combineMode)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure that represents the new clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(Rectangle rect)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the result of the specified operation combining the current clip region and the rectangle specified by a
        /// <see cref="System.Drawing.Rectangle" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.Rectangle" /> structure to combine.
        /// </param>
        /// <param name="combineMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.CombineMode" /> enumeration that specifies the combining operation to use.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the rectangle specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure that represents the new clip region.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(RectangleF rect)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the result of the specified operation combining the current clip region and the rectangle specified by a
        /// <see cref="System.Drawing.RectangleF" /> structure.
        /// </summary>
        /// <param name="rect">
        /// <see cref="System.Drawing.RectangleF" /> structure to combine.
        /// </param>
        /// <param name="combineMode">Member of the
        /// <see cref="System.Drawing.Drawing2D.CombineMode" /> enumeration that specifies the combining operation to use.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
        }
        /// <summary>
        /// Sets the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> to the result of the specified operation combining the current clip region and the specified
        /// <see cref="System.Drawing.Region" />
        /// </summary>
        /// <param name="region">
        /// <see cref="System.Drawing.Region" /> to combine.
        /// </param>
        /// <param name="combineMode">Member from the
        /// <see cref="System.Drawing.Drawing2D.CombineMode" /> enumeration that specifies the combining operation to use.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        public void SetClip(Region region, CombineMode combineMode)
        {
        }
        /// <summary>
        /// Transforms an array of points from one coordinate space to another using the current world and page transformations of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="destSpace">Member of the
        /// <see cref="System.Drawing.Drawing2D.CoordinateSpace" /> enumeration that specifies the destination coordinate space.
        /// </param>
        /// <param name="srcSpace">Member of the
        /// <see cref="System.Drawing.Drawing2D.CoordinateSpace" /> enumeration that specifies the source coordinate space.
        /// </param>
        /// <param name="pts">Array of
        /// <see cref="System.Drawing.PointF" /> structures that represent the points to transform.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
        }
        /// <summary>
        /// Transforms an array of points from one coordinate space to another using the current world and page transformations of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="destSpace">Member of the
        /// <see cref="System.Drawing.Drawing2D.CoordinateSpace" /> enumeration that specifies the destination coordinate space.
        /// </param>
        /// <param name="srcSpace">Member of the
        /// <see cref="System.Drawing.Drawing2D.CoordinateSpace" /> enumeration that specifies the source coordinate space.
        /// </param>
        /// <param name="pts">Array of
        /// <see cref="System.Drawing.Point" /> structures that represents the points to transformation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
        {
        }
        /// <summary>
        /// Translates the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> by specified amounts in the horizontal and vertical directions.
        /// </summary>
        /// <param name="dx">The x-coordinate of the translation.
        /// </param>
        /// <param name="dy">The y-coordinate of the translation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TranslateClip(int dx, int dy)
        {
        }
        /// <summary>
        /// Translates the clipping region of this
        /// <see cref="System.Drawing.Graphics" /> by specified amounts in the horizontal and vertical directions.
        /// </summary>
        /// <param name="dx">The x-coordinate of the translation.
        /// </param>
        /// <param name="dy">The y-coordinate of the translation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TranslateClip(float dx, float dy)
        {
        }
        /// <summary>
        /// Changes the origin of the coordinate system by prepending the specified translation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" />
        /// </summary>
        /// <param name="dx">The x-coordinate of the translation.
        /// </param>
        /// <param name="dy">The y-coordinate of the translation.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Append);
        }
        /// <summary>
        /// Changes the origin of the coordinate system by applying the specified translation to the transformation matrix of this
        /// <see cref="System.Drawing.Graphics" /> in the specified order.
        /// </summary>
        /// <param name="dx">The x-coordinate of the translation.
        /// </param>
        /// <param name="dy">The y-coordinate of the translation.
        /// </param>
        /// <param name="order">Member of the
        /// <see cref="System.Drawing.Drawing2D.MatrixOrder" /> enumeration that specifies whether the translation is prepended or appended to the transformation matrix.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            this.SetTranslateWithDifference(dx, dy);
        }

    }
}

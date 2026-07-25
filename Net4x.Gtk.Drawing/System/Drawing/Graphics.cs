using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Cairo;
using Gdk;
using Gtk;
using Context = Cairo.Context;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace System.Drawing;

/// <summary>
/// Encapsulates a GDI+ drawing surface. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class Graphics : MarshalByRefObject, IDeviceContext, IDisposable
{
    /// <summary>
    /// The underlying Cairo context used by this Graphics instance.
    /// </summary>
    /// <remarks>
    /// This was public, which put a raw, unsynchronised substrate handle on the shipped API surface and
    /// let callers bypass the ownership bookkeeping below. It is internal now; the Forms library and both
    /// test assemblies already have <c>InternalsVisibleTo</c> grants.
    /// </remarks>
    internal Cairo.Context context;

    private float _angle = 0;

    private readonly Image? image;

    // These were static, so every Graphics created by FromImage shared one Cairo surface and context:
    // allocating a second one disposed the first one's drawing surface out from under it. Two Graphics
    // objects over two different bitmaps could not coexist, which is exactly what double buffering needs,
    // and it is also why each CreateGraphics call was observed to dispose the previous surface.
    private ImageSurface? ownedSurface;

    private bool ownsContext;

    private readonly Gdk.Rectangle rectangle;

    /// <summary>Current clipping bounds; starts as the full surface and follows SetClip/IntersectClip/ResetClip.</summary>
    private RectangleF _clipBounds;

    private readonly Widget? widget;

    internal Graphics(Widget? widget, Cairo.Context context, Gdk.Rectangle rectangle) : this(context, rectangle)
    {
        this.widget = widget;
    }

    internal Graphics(Image widget, Cairo.Context context, Gdk.Rectangle rectangle) : this(context, rectangle)
    {
        this.image = widget;
    }

    internal Graphics(Cairo.Context context, Gdk.Rectangle rectangle)
    {
        this.context = context;
        this.rectangle = rectangle;
        this._clipBounds = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        // Assign the backing field rather than the property: the surface starts unclipped, so there is
        // nothing to push down to Cairo yet.
        this._clip = new Region(new Rectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height));
    }

    /// <summary>The clip region this Graphics is currently honouring.</summary>
    private Region _clip;

    /// <summary>
    /// Gets or sets the clipping region for this Graphics object.
    /// </summary>
    /// <remarks>
    /// This used to be an auto-property assigned once in the constructor: no SetClip/IntersectClip/
    /// ResetClip overload ever wrote to it, so the getter reported the full surface no matter what had
    /// been clipped, and the setter stored a region that was never pushed down to Cairo - assigning a
    /// clip region did nothing at all. It is now backed by the same state every clip mutator maintains.
    /// </remarks>
    public Region Clip
    {
        get => _clip;
        set => SetClip(value ?? new Region(), CombineMode.Replace);
    }

    /// <summary>
    /// Gets the bounds of the clipping region of this Graphics object.
    /// </summary>
    public RectangleF ClipBounds
    {
        get
        {
            return this._clipBounds;
        }
    }

    /// <summary>
    /// Gets or sets the compositing mode for this Graphics object.
    /// </summary>
    public CompositingMode CompositingMode
    {
        get => _compositingMode;
        set
        {
            _compositingMode = value;
            this.context.Operator = value == CompositingMode.SourceCopy
                ? Cairo.Operator.Source
                : Cairo.Operator.Over;
        }
    }

    private CompositingMode _compositingMode = CompositingMode.SourceOver;

    /// <summary>
    /// Gets or sets the compositing quality for this Graphics object.
    /// </summary>
    public CompositingQuality CompositingQuality
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the horizontal resolution, in dots per inch.
    /// </summary>
    public float DpiX => DefaultDpi;

    /// <summary>
    /// Gets the vertical resolution, in dots per inch.
    /// </summary>
    public float DpiY => DefaultDpi;

    /// <summary>Screen DPI assumed for this surface (GDI+ reports the device DPI; 96 is the norm).</summary>
    private const float DefaultDpi = 96f;

    /// <summary>
    /// Gets or sets the interpolation mode used for scaling operations.
    /// </summary>
    public InterpolationMode InterpolationMode
    {
        // Nothing consumed this, so image scaling always used a fixed filter. It now selects the
        // Gdk interpolation used when images are resampled.
        get;
        set;
    } = InterpolationMode.Default;

    /// <summary>
    /// The Gdk filter matching <see cref="P:System.Drawing.Graphics.InterpolationMode" />.
    /// </summary>
    private Gdk.InterpType ImageInterpolation => InterpolationMode switch
    {
        InterpolationMode.NearestNeighbor => Gdk.InterpType.Nearest,
        InterpolationMode.Low => Gdk.InterpType.Nearest,
        InterpolationMode.Bilinear or InterpolationMode.HighQualityBilinear => Gdk.InterpType.Bilinear,
        InterpolationMode.Bicubic or InterpolationMode.HighQualityBicubic or InterpolationMode.High => Gdk.InterpType.Hyper,
        _ => Gdk.InterpType.Tiles
    };

    /// <summary>
    /// Gets a value specifying whether the current clipping region is empty.
    /// </summary>
    public bool IsClipEmpty
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the visible clipping region is empty.
    /// </summary>
    public bool IsVisibleClipEmpty
    {
        get;
    }

    /// <summary>
    /// Gets or sets the scale of the page.
    /// </summary>
    public float PageScale
    {
        // PageUnit and PageScale were inert auto-properties, so a Graphics set to Millimeter or Inch
        // still drew at pixel coordinates - a geometric error, not a cosmetic one. Fold them into
        // the current transform instead.
        get => _pageScale;
        set
        {
            ApplyPageTransform(_pageUnit, _pageScale, _pageUnit, value);
            _pageScale = value;
        }
    }

    private float _pageScale = 1f;

    private GraphicsUnit _pageUnit = GraphicsUnit.Display;

    /// <summary>
    /// Scales the current transform by the change in page unit/scale, so the page transform composes
    /// with whatever world transform is already in effect.
    /// </summary>
    private void ApplyPageTransform(GraphicsUnit oldUnit, float oldScale, GraphicsUnit newUnit, float newScale)
    {
        var previous = PixelsPerUnit(oldUnit) * oldScale;
        var next = PixelsPerUnit(newUnit) * newScale;
        if (previous <= 0d || next <= 0d)
        {
            return;
        }

        var delta = next / previous;
        if (Math.Abs(delta - 1d) > 1e-9d)
        {
            this.context.Scale(delta, delta);
        }
    }

    /// <summary>
    /// How many device pixels one unit of <paramref name="unit" /> covers at the default 96 dpi.
    /// </summary>
    private static double PixelsPerUnit(GraphicsUnit unit) => unit switch
    {
        GraphicsUnit.Point => DefaultDpi / 72d,
        GraphicsUnit.Inch => DefaultDpi,
        GraphicsUnit.Document => DefaultDpi / 300d,
        GraphicsUnit.Millimeter => DefaultDpi / 25.4d,
        _ => 1d
    };

    /// <summary>
    /// Gets or sets the unit of measure used for page coordinates.
    /// </summary>
    public GraphicsUnit PageUnit
    {
        get => _pageUnit;
        set
        {
            ApplyPageTransform(_pageUnit, _pageScale, value, _pageScale);
            _pageUnit = value;
        }
    }

    /// <summary>
    /// Gets or sets the pixel offset mode.
    /// </summary>
    public PixelOffsetMode PixelOffsetMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the rendering origin.
    /// </summary>
    public Point RenderingOrigin
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the smoothing mode for this Graphics object.
    /// </summary>
    public SmoothingMode SmoothingMode
    {
        // These were plain auto-properties with no consumer anywhere in the file, so setting
        // SmoothingMode = AntiAlias read back correctly and changed nothing about the rendering.
        get => _smoothingMode;
        set
        {
            _smoothingMode = value;
            this.context.Antialias = value switch
            {
                SmoothingMode.None or SmoothingMode.HighSpeed => Cairo.Antialias.None,
                SmoothingMode.AntiAlias or SmoothingMode.HighQuality => Cairo.Antialias.Default,
                _ => Cairo.Antialias.Default
            };
        }
    }

    private SmoothingMode _smoothingMode = SmoothingMode.Default;

    /// <summary>
    /// Gets or sets the gamma correction applied to rendered text, from 0 to 12.
    /// </summary>
    /// <remarks>
    /// The getter threw while the setter silently accepted, so a value could be assigned and never read
    /// back. It now round-trips. Cairo has no equivalent knob, so the value does not affect rendering.
    /// </remarks>
    public int TextContrast
    {
        get;
        set;
    } = 4;

    /// <summary>
    /// Gets or sets the text rendering hint. Not implemented in this wrapper.
    /// </summary>
    public TextRenderingHint TextRenderingHint { get; set; } = TextRenderingHint.SystemDefault;

    /// <summary>
    /// Gets or sets the world transformation matrix for this Graphics object.
    /// </summary>
    public Drawing2D.Matrix Transform
    {
        get
        {
            // The getter used to throw and the setter was a no-op, so the world transform could not be
            // read back or assigned at all. Report the live Cairo matrix instead.
            var m = this.context.Matrix;
            if (ReferenceEquals(m, null))
            {
                return new Drawing2D.Matrix();
            }

            return new Drawing2D.Matrix((float)m.Xx, (float)m.Yx, (float)m.Xy, (float)m.Yy, (float)m.X0, (float)m.Y0);
        }

        set
        {
            var converted = ConvertToMatrix(value);
            if (!ReferenceEquals(converted, null))
            {
                this.context.Matrix = converted;
            }
        }
    }

    /// <summary>
    /// Gets the bounds of the visible clipping region of this Graphics object.
    /// </summary>
    public RectangleF VisibleClipBounds
    {
        get
        {
            return new RectangleF(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
        }
    }

    internal double diff_left
    {
        get;
        set;
    }

    internal double diff_top
    {
        get;
        set;
    }

    /// <summary>
    /// Adds a comment to a metafile. Not implemented.
    /// </summary>
    /// <param name="data">Comment data.
    /// </param>
    public void AddMetafileComment(byte[] data)
    {
    }

    /// <summary>
    /// Begins a graphics container. Not implemented.
    /// </summary>
    /// <returns>A new GraphicsContainer.
    /// </returns>
    public GraphicsContainer BeginContainer()
    {
        // Cairo's state stack gives exactly the save/restore semantics a container needs.
        this.context.Save();
        return new GraphicsContainer();
    }

    /// <summary>
    /// Begins a graphics container with the specified source and destination rectangles.
    /// Not implemented.
    /// </summary>
    public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
    {
        return BeginContainer(new RectangleF(dstrect.X, dstrect.Y, dstrect.Width, dstrect.Height),
            new RectangleF(srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height), unit);
    }

    /// <summary>
    /// Begins a graphics container with floating point rectangles. Not implemented.
    /// </summary>
    public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
    {
        this.context.Save();
        if (srcrect.Width > 0 && srcrect.Height > 0)
        {
            // Map the source rectangle onto the destination rectangle for drawing inside the container.
            this.context.Translate(dstrect.X, dstrect.Y);
            this.context.Scale(dstrect.Width / srcrect.Width, dstrect.Height / srcrect.Height);
            this.context.Translate(-srcrect.X, -srcrect.Y);
        }

        return new GraphicsContainer();
    }

    /// <summary>
    /// Clears the drawing surface with the specified color.
    /// </summary>
    /// <param name="color">The color to clear with.
    /// </param>
    public void Clear(Color color)
    {
        this.context.Save();
        this.context.SetSourceRGB(color.R / 255f, color.G / 255f, color.B / 255f);
        this.SetTranslateWithDifference(0, 0);
        this.context.Paint();
        this.context.Restore();
    }

    /// <summary>
    /// Copies pixels from the screen. Not implemented.
    /// </summary>
    public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
    {
    }

    /// <summary>
    /// Copies pixels from the screen with copy operation. Not implemented.
    /// </summary>
    public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    {
    }

    /// <summary>
    /// Copies pixels from the screen. Not implemented.
    /// </summary>
    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
    {
    }

    /// <summary>
    /// Copies pixels from the screen with copy operation. Not implemented.
    /// </summary>
    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    {
    }

    /// <summary>
    /// Releases resources used by the Graphics object.
    /// </summary>
    public void Dispose()
    {
        // Flush pending drawing back to the source image (if this Graphics was created via FromImage) and
        // release the drawing surface. Without this, drawing onto a Bitmap and then disposing the Graphics
        // never reached the bitmap's pixels.
        Flush();
    }

    /// <summary>
    /// Draws an arc defined by a rectangle.
    /// </summary>
    public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
    {
        DrawArcCore(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws an arc defined by a floating-point rectangle.
    /// </summary>
    public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
    {
        DrawArcCore(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws an arc with integer coordinates.
    /// </summary>
    public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
        DrawArcCore(pen, x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws an arc with floating-point coordinates.
    /// </summary>
    public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        DrawArcCore(pen, x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws a Bezier curve specified by four points.
    /// </summary>
    public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
    {
        DrawBeziersCore(pen, new PointF[] { new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y), new PointF(pt3.X, pt3.Y), new PointF(pt4.X, pt4.Y) });
    }

    /// <summary>
    /// Draws a Bezier curve specified by four floating-point points.
    /// </summary>
    public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
    {
        DrawBeziersCore(pen, new PointF[] { pt1, pt2, pt3, pt4 });
    }

    /// <summary>
    /// Draws a Bezier curve using explicit coordinates.
    /// </summary>
    public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        DrawBeziersCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4) });
    }

    /// <summary>
    /// Draws a sequence of Bezier curves.
    /// </summary>
    public void DrawBeziers(Pen pen, PointF[] points)
    {
        DrawBeziersCore(pen, points);
    }

    /// <summary>
    /// Draws a sequence of Bezier curves from integer points.
    /// </summary>
    public void DrawBeziers(Pen pen, Point[] points)
    {
        DrawBeziersCore(pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
    }

    /// <summary>
    /// Draws a closed curve defined by an array of points.
    /// </summary>
    public void DrawClosedCurve(Pen pen, PointF[] points)
    {
        DrawCurveCore(true, false, pen, points, 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a closed curve with specified tension and fill mode.
    /// </summary>
    public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
    {
        DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
    }

    /// <summary>
    /// Draws a closed curve from integer points.
    /// </summary>
    public void DrawClosedCurve(Pen pen, Point[] points)
    {
        DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a closed curve from integer points with tension and fill mode.
    /// </summary>
    public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
    {
        DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
    }

    /// <summary>
    /// Draws a cardinal spline through a specified array of points.
    /// </summary>
    public void DrawCurve(Pen pen, PointF[] points)
    {
        DrawCurveCore(false, false, pen, points, 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline with offset and number of segments.
    /// </summary>
    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
    {
        DrawCurveCore(false, false, pen, points, offset, numberOfSegments, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline with tension.
    /// </summary>
    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
    {
        DrawCurveCore(false, false, pen, points, offset, numberOfSegments, tension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline with specified tension.
    /// </summary>
    public void DrawCurve(Pen pen, PointF[] points, float tension)
    {
        DrawCurveCore(false, false, pen, points, 0, 0, tension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline from integer points.
    /// </summary>
    public void DrawCurve(Pen pen, Point[] points)
    {
        DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline from integer points with offset, segments and tension.
    /// </summary>
    public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
    {
        DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), offset, numberOfSegments, tension, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline from integer points with tension.
    /// </summary>
    public void DrawCurve(Pen pen, Point[] points, float tension)
    {
        DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, FillMode.Winding);
    }

    /// <summary>
    /// Draws an ellipse specified by a rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, Rectangle rect)
    {
        DrawEllipseCore(pen, rect.X, rect.Y, rect.Width, rect.Height, false, FillMode.Winding);
    }

    /// <summary>
    /// Draws an ellipse specified by a floating-point rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, RectangleF rect)
    {
        DrawEllipseCore(pen, rect.X, rect.Y, rect.Width, rect.Height, false, FillMode.Winding);
    }

    /// <summary>
    /// Draws an ellipse with integer coordinates.
    /// </summary>
    public void DrawEllipse(Pen pen, int x, int y, int width, int height)
    {
        DrawEllipseCore(pen, x, y, width, height, false, FillMode.Winding);
    }

    /// <summary>
    /// Draws an ellipse with floating-point coordinates.
    /// </summary>
    public void DrawEllipse(Pen pen, float x, float y, float width, float height)
    {
        DrawEllipseCore(pen, x, y, width, height, false, FillMode.Winding);
    }

    /// <summary>
    /// Draws an icon into the specified rectangle.
    /// </summary>
    public void DrawIcon(Icon icon, Rectangle targetRect)
    {
        DrawImage(new Bitmap(icon.PixbufData), targetRect);
    }

    /// <summary>
    /// Draws an icon at the specified position.
    /// </summary>
    public void DrawIcon(Icon icon, int x, int y)
    {
        DrawImage(new Bitmap(icon.PixbufData), new Point(x, y));
    }

    /// <summary>
    /// Draws an icon without stretching into the target rectangle.
    /// </summary>
    public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
    {
        DrawImage(new Bitmap(icon.PixbufData), targetRect);
    }

    /// <summary>
    /// Draws the specified image at the given point.
    /// </summary>
    public void DrawImage(Image image, Point point)
    {
        DrawImageScaledCore(image, new Rectangle(point.X, point.Y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws the specified image at the given floating point location.
    /// </summary>
    public void DrawImage(Image image, PointF point)
    {
        DrawImageScaledCore(image, new Rectangle((int)point.X, (int)point.Y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws the image into the polygon defined by three destination points.
    /// </summary>
    public void DrawImage(Image image, PointF[] destPoints)
    {
        DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into destination polygon.
    /// </summary>
    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into destination polygon with image attributes.
    /// </summary>
    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
    {
        DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into destination polygon with image attributes and abort callback.
    /// </summary>
    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
    {
        DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into destination polygon with image attributes and callback data.
    /// </summary>
    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
    {
        DrawImageScaledCore(image, new Rectangle((int)destPoints[0].X, (int)destPoints[0].Y, (int)destPoints[1].X - (int)destPoints[0].X, (int)destPoints[2].Y - (int)destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, new IntPtr(callbackData));
    }

    /// <summary>
    /// Draws an image into the polygon specified by integer points.
    /// </summary>
    public void DrawImage(Image image, Point[] destPoints)
    {
        DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the polygon specified by integer points.
    /// </summary>
    public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the polygon specified by integer points with image attributes.
    /// </summary>
    public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
    {
        DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the polygon specified by integer points with image attributes and abort callback.
    /// </summary>
    public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
    {
        DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the polygon specified by integer points with image attributes and callback data.
    /// </summary>
    public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
    {
        DrawImageScaledCore(image, new Rectangle(destPoints[0].X, destPoints[0].Y, destPoints[1].X - destPoints[0].X, destPoints[2].Y - destPoints[0].Y), 0, 0, srcRect.Width, srcRect.Height, srcUnit, imageAttr, callback, new IntPtr(callbackData));
    }

    /// <summary>
    /// Draws an image into the specified destination rectangle.
    /// </summary>
    public void DrawImage(Image image, Rectangle rect)
    {
        DrawImageScaledCore(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle with image attributes.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle with image attributes and callback.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle with image attributes, callback and callback data.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle using floating point source coordinates.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle using floating point source coordinates with image attributes.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle using floating point source coordinates with image attributes and callback.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into the specified destination rectangle using floating point source coordinates with image attributes, callback and callback data.
    /// </summary>
    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
    {
        DrawImageScaledCore(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
    }

    /// <summary>
    /// Draws an image using a floating-point destination rectangle.
    /// </summary>
    public void DrawImage(Image image, RectangleF rect)
    {
        DrawImageScaledCore(image, new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height), rect.X, rect.Y, rect.Width, rect.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image into a floating-point destination rectangle.
    /// </summary>
    public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, new Rectangle((int)destRect.X, (int)destRect.Y, (int)destRect.Width, (int)destRect.Height), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws an image at the specified integer coordinates.
    /// </summary>
    public void DrawImage(Image image, int x, int y)
    {
        DrawImageScaledCore(image, new Rectangle(x, y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image at the specified integer coordinates.
    /// </summary>
    public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, new Rectangle(x, y, srcRect.Width + x, srcRect.Height + y), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws an image with the specified size.
    /// </summary>
    public void DrawImage(Image image, int x, int y, int width, int height)
    {
        DrawImageScaledCore(image, new Rectangle(x, y, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws an image at the specified floating point location.
    /// </summary>
    public void DrawImage(Image image, float x, float y)
    {
        DrawImageScaledCore(image, new Rectangle((int)x, (int)y, image.Width, image.Height), x, y, image.Width, image.Height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws a portion of an image with floating point source rectangle at the specified location.
    /// </summary>
    public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        DrawImageScaledCore(image, new Rectangle((int)x, (int)y, image.Width, image.Height), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws an image scaled to the specified floating point size.
    /// </summary>
    public void DrawImage(Image image, float x, float y, float width, float height)
    {
        DrawImageScaledCore(image, new Rectangle((int)x, (int)y, (int)width, (int)height), 0, 0, width, height, GraphicsUnit.Pixel, null, null, IntPtr.Zero);
    }

    /// <summary>
    /// Draws an image without scaling at a specified point.
    /// </summary>
    public void DrawImageUnscaled(Image image, Point point)
    {
        DrawImageUnscaledCore(image, point.X, point.Y, image.Width, image.Height);
    }

    /// <summary>
    /// Draws an image without scaling into a specified rectangle.
    /// </summary>
    public void DrawImageUnscaled(Image image, Rectangle rect)
    {
        DrawImageUnscaledCore(image, rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Draws an image without scaling at the specified coordinates.
    /// </summary>
    public void DrawImageUnscaled(Image image, int x, int y)
    {
        DrawImageUnscaledCore(image, x, y, image.Width, image.Height);
    }

    /// <summary>
    /// Draws an image without scaling at the specified coordinates with explicit size.
    /// </summary>
    public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
    {
        DrawImageUnscaledCore(image, x, y, width, height);
    }

    /// <summary>
    /// Draws an image unscaled and clipped to the specified rectangle.
    /// </summary>
    public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
    {
        DrawImageUnscaledCore(image, rect.X, rect.Y, rect.Width, rect.Height, true);
    }

    /// <summary>
    /// Draws a line between two points.
    /// </summary>
    public void DrawLine(Pen pen, Point pt1, Point pt2)
    {
        DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
    }

    /// <summary>
    /// Draws a line between two floating-point points.
    /// </summary>
    public void DrawLine(Pen pen, PointF pt1, PointF pt2)
    {
        DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
    }

    /// <summary>
    /// Draws a line between two integer coordinates.
    /// </summary>
    public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
    {
        DrawLinesCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
    }

    /// <summary>
    /// Draws a line between two floating-point coordinates.
    /// </summary>
    public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
    {
        DrawLinesCore(pen, new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
    }

    /// <summary>
    /// Draws a sequence of connected lines.
    /// </summary>
    public void DrawLines(Pen pen, PointF[] points)
    {
        DrawLinesCore(pen, points);
    }

    /// <summary>
    /// Draws a sequence of connected lines from integer points.
    /// </summary>
    public void DrawLines(Pen pen, Point[] points)
    {
        DrawLinesCore(pen, Array.ConvertAll<Point, PointF>(points, o => new PointF(o.X, o.Y)));
    }

    /// <summary>
    /// Draws the outline of a GraphicsPath.
    /// </summary>
    public void DrawPath(Pen pen, GraphicsPath? path)
    {
        DrawPathCore(pen, path, false);
    }

    /// <summary>
    /// Draws a pie defined by a rectangle.
    /// </summary>
    public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
    {
        DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws a pie defined by a floating-point rectangle.
    /// </summary>
    public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
    {
        DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws a pie with integer coordinates.
    /// </summary>
    public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
        DrawPieCore(false, pen, x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws a pie with floating-point coordinates.
    /// </summary>
    public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        DrawPieCore(false, pen, x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Draws a polygon defined by an array of points.
    /// </summary>
    public void DrawPolygon(Pen pen, PointF[] points)
    {
        DrawPolygonCore(false, pen, points, FillMode.Winding);
    }

    /// <summary>
    /// Draws a polygon from integer points.
    /// </summary>
    public void DrawPolygon(Pen pen, Point[] points)
    {
        DrawPolygonCore(false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), FillMode.Winding);
    }

    /// <summary>
    /// Draws a rectangle specified by a Rectangle structure.
    /// </summary>
    public void DrawRectangle(Pen pen, Rectangle rect)
    {
        DrawRectangleCore(false, pen, rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Draws a rectangle specified by a RectangleF structure.
    /// </summary>
    public void DrawRectangle(Pen pen, RectangleF rect)
    {
        DrawRectangleCore(false, pen, rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Draws a rectangle with integer coordinates.
    /// </summary>
    public void DrawRectangle(Pen pen, int x, int y, int width, int height)
    {
        DrawRectangleCore(false, pen, x, y, width, height);
    }

    /// <summary>
    /// Draws a rectangle with floating-point coordinates.
    /// </summary>
    public void DrawRectangle(Pen pen, float x, float y, float width, float height)
    {
        DrawRectangleCore(false, pen, x, y, width, height);
    }

    /// <summary>
    /// Draws multiple rectangles from floating-point rectangles.
    /// </summary>
    public void DrawRectangles(Pen pen, RectangleF[] rects)
    {
        foreach (var rect in rects)
            DrawRectangle(pen, rect);

    }

    /// <summary>
    /// Draws multiple rectangles from integer rectangles.
    /// </summary>
    public void DrawRectangles(Pen pen, Rectangle[] rects)
    {
        foreach (var rect in rects)
            DrawRectangle(pen, rect);
    }

    /// <summary>
    /// Draws the specified string at the given point using the provided font and brush.
    /// </summary>
    public void DrawString(string s, Font font, Brush brush, PointF point)
    {
        DrawString(s, font, brush, new RectangleF(point.X + this.rectangle.X, point.Y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), new StringFormat());
    }

    /// <summary>
    /// Draws the specified string at the given point using the provided font, brush and format.
    /// </summary>
    public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
    {
        DrawString(s, font, brush, new RectangleF(point.X + this.rectangle.X, point.Y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), format);
    }

    /// <summary>
    /// Draws the specified string within the specified layout rectangle.
    /// </summary>
    public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
    {
        DrawString(s, font, brush, layoutRectangle, new StringFormat());
    }

    /// <summary>
    /// Draws the specified string within the specified layout rectangle using the given format.
    /// </summary>
    /// <param name="text">Text to draw.
    /// </param>
    /// <param name="font">Font to use.
    /// </param>
    /// <param name="brush">Brush used to draw.
    /// </param>
    /// <param name="layoutRectangle">Layout rectangle.
    /// </param>
    /// <param name="format">String format to apply.
    /// </param>
    public void DrawString(string text, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
    {
        if (string.IsNullOrEmpty(text) == false)
        {
            this.context.Save();
            if (font != null)
            {
                double textSize = font.Size;
                if (font.Unit == GraphicsUnit.Point)
                    textSize = font.Size * 1 / 72 * 96;
                else if (font.Unit == GraphicsUnit.Inch)
                    textSize = font.Size * 96;
                this.context.SetFontSize(textSize);
                this.context.SelectFontFace(font.Name, font.Style.HasFlag(FontStyle.Italic) ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, font.Style.HasFlag(FontStyle.Bold) ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
            }

            this.SetSourcePen(new Pen(brush, 1), false);
            double fontsize = this.context.FontExtents.Height;
            double desent = this.context.FontExtents.Descent;
            TextExtents textext = this.context.TextExtents(text);
            if (format == null)
            {
                this.SetTranslateWithDifference(layoutRectangle.X + desent / 2, layoutRectangle.Y + fontsize - desent);
            }

            else
            {
                double hAlign = desent / 2, vAlign = textext.Height;
                if (format.Alignment == StringAlignment.Center)
                    hAlign -= textext.Width / 2 + desent / 2;
                else if (format.Alignment == StringAlignment.Far)
                    hAlign -= textext.Width + desent;

                if (format.LineAlignment == StringAlignment.Center)
                    vAlign -= textext.Height / 2 + desent / 2;
                else if (format.LineAlignment == StringAlignment.Far)
                    vAlign -= textext.Height + desent;

                if (format.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical))
                {
                    vAlign -= textext.Height - desent;
                    this.context.Rotate(90 * Math.PI / 180);
                    this.SetTranslateWithDifference(layoutRectangle.X + hAlign, 0 - layoutRectangle.Y - vAlign);
                }

                else
                {
                    this.SetTranslateWithDifference(layoutRectangle.X + hAlign, layoutRectangle.Y + vAlign);
                }
            }

            this.context.ShowText(text);
            this.context.Stroke();
            this.context.Restore();
        }
    }

    /// <summary>
    /// Draws the specified string at the given coordinates.
    /// </summary>
    public void DrawString(string s, Font font, Brush brush, float x, float y)
    {
        DrawString(s, font, brush, new RectangleF(x + this.rectangle.X, y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), new StringFormat());
    }

    /// <summary>
    /// Draws the specified string at the given coordinates using a StringFormat.
    /// </summary>
    public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
    {
        DrawString(s, font, brush, new RectangleF(x + this.rectangle.X, y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height), format);
    }

    /// <summary>
    /// Ends a graphics container. Not implemented.
    /// </summary>
    public void EndContainer(GraphicsContainer container)
    {
        // Was a no-op, so state changed inside a container leaked out of it.
        this.context.Restore();
    }

    /// <summary>
    /// Enumerates records in a metafile. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile for floating-point destination. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile for floating-point destination with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile for floating-point destination with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with floating-point source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with floating-point source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates records in a metafile with floating-point source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points with source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points with source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile to destination points with source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points with source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points with source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile to integer destination points with source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle with source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle with source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a destination rectangle with source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle with callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle with image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle with source rectangle. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle with source rectangle and callback data. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
    {
    }

    /// <summary>
    /// Enumerates a metafile into a floating-point destination rectangle with source rectangle, callback data and image attributes. Not implemented.
    /// </summary>
    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
    }

    /// <summary>
    /// Excludes the specified rectangle from the current clip region.
    /// </summary>
    public void ExcludeClip(Rectangle rect)
    {
        ExcludeClipCore(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
    }

    /// <summary>
    /// Excludes the specified region from the current clip region. The region is approximated by its bounds.
    /// </summary>
    public void ExcludeClip(Region region)
    {
        if (region == null)
        {
            return;
        }

        ExcludeClipCore(region.GetBounds(this));
    }

    /// <summary>
    /// Clips to the current clip bounds minus <paramref name="rect"/> using the even-odd fill rule, so that
    /// painting inside the excluded rectangle is suppressed. Was a no-op, so exclusions never took effect.
    /// </summary>
    private void ExcludeClipCore(RectangleF rect)
    {
        var outer = this._clipBounds;
        var previousRule = this.context.FillRule;
        this.context.NewPath();
        this.context.Rectangle(outer.X, outer.Y, outer.Width, outer.Height);
        this.context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        this.context.FillRule = Cairo.FillRule.EvenOdd;
        this.context.Clip();
        this.context.FillRule = previousRule;
        this.context.NewPath();
    }

    /// <summary>
    /// Fills a closed curve defined by an array of points.
    /// </summary>
    public void FillClosedCurve(Brush brush, PointF[] points)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Fills a closed curve with specified fill mode.
    /// </summary>
    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, DefaultCurveTension, fillmode);
    }

    /// <summary>
    /// Fills a closed curve with specified fill mode and tension.
    /// </summary>
    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, tension, fillmode);
    }

    /// <summary>
    /// Fills a closed curve from integer points.
    /// </summary>
    public void FillClosedCurve(Brush brush, Point[] points)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, DefaultCurveTension, FillMode.Winding);
    }

    /// <summary>
    /// Fills a closed curve from integer points with fill mode.
    /// </summary>
    public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, DefaultCurveTension, fillmode);
    }

    /// <summary>
    /// Fills a closed curve from integer points with fill mode and tension.
    /// </summary>
    public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, tension, fillmode);
    }

    /// <summary>
    /// Fills an ellipse specified by a rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, Rectangle rect)
    {
        DrawEllipseCore(new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height, true, FillMode.Winding);
    }

    /// <summary>
    /// Fills an ellipse specified by a floating-point rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, RectangleF rect)
    {
        DrawEllipseCore(new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height, true, FillMode.Winding);
    }

    /// <summary>
    /// Fills an ellipse with integer coordinates.
    /// </summary>
    public void FillEllipse(Brush brush, int x, int y, int width, int height)
    {
        DrawEllipseCore(new Pen(brush, 0), x, y, width, height, true, FillMode.Winding);
    }

    /// <summary>
    /// Fills an ellipse with floating-point coordinates.
    /// </summary>
    public void FillEllipse(Brush brush, float x, float y, float width, float height)
    {
        DrawEllipseCore(new Pen(brush, 0), x, y, width, height, true, FillMode.Winding);
    }

    /// <summary>
    /// Fills the interior of a GraphicsPath.
    /// </summary>
    public void FillPath(Brush brush, GraphicsPath path)
    {
        DrawPathCore(new Pen(brush, 1), path, true);
    }

    /// <summary>
    /// Fills a pie defined by a rectangle.
    /// </summary>
    public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
    {
        FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Fills a pie with integer coordinates.
    /// </summary>
    public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
        DrawPieCore(true, new Pen(brush, 0), x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Fills a pie with floating-point coordinates.
    /// </summary>
    public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        DrawPieCore(true, new Pen(brush, 0), x, y, width, height, startAngle, sweepAngle);
    }

    /// <summary>
    /// Fills a polygon defined by points.
    /// </summary>
    public void FillPolygon(Brush brush, PointF[] points)
    {
        FillPolygon(brush, points, FillMode.Winding);
    }

    /// <summary>
    /// Fills a polygon defined by points with specified fill mode.
    /// </summary>
    public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
    {
        DrawPolygonCore(true, new Pen(brush, 0), points, fillMode);
    }

    /// <summary>
    /// Fills a polygon from integer points.
    /// </summary>
    public void FillPolygon(Brush brush, Point[] points)
    {
        FillPolygon(brush, points, FillMode.Winding);
    }

    /// <summary>
    /// Fills a polygon from integer points with specified fill mode.
    /// </summary>
    public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
    {
        DrawPolygonCore(true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), fillMode);
    }

    /// <summary>
    /// Fills a rectangle specified by a Rectangle structure.
    /// </summary>
    public void FillRectangle(Brush brush, Rectangle rect)
    {
        DrawRectangleCore(true, new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Fills a rectangle specified by a RectangleF structure.
    /// </summary>
    public void FillRectangle(Brush brush, RectangleF rect)
    {
        DrawRectangleCore(true, new Pen(brush, 0), rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Fills a rectangle with integer coordinates.
    /// </summary>
    public void FillRectangle(Brush brush, int x, int y, int width, int height)
    {
        DrawRectangleCore(true, new Pen(brush, 0), x, y, width, height);
    }

    /// <summary>
    /// Fills a rectangle with floating-point coordinates.
    /// </summary>
    public void FillRectangle(Brush brush, float x, float y, float width, float height)
    {
        DrawRectangleCore(true, new Pen(brush, 0), x, y, width, height);
    }

    /// <summary>
    /// Fills multiple rectangles defined by floating-point rectangles.
    /// </summary>
    public void FillRectangles(Brush brush, RectangleF[] rects)
    {
        foreach (var rect in rects)
            FillRectangle(brush, rect);
    }

    /// <summary>
    /// Fills multiple rectangles defined by integer rectangles.
    /// </summary>
    public void FillRectangles(Brush brush, Rectangle[] rects)
    {
        foreach (var rect in rects)
            FillRectangle(brush, rect);
    }

    /// <summary>
    /// Fills the interior of the specified region. Not implemented.
    /// </summary>
    public void FillRegion(Brush brush, Region region)
    {
        // Was a no-op, so FillRegion painted nothing at all. Fill the region's extent: exact for the
        // rectangular/path regions this shim models, matching Region's own bounds-based GetRegionScans.
        if (brush == null || region == null)
        {
            return;
        }

        var bounds = region.GetBounds(this);
        if (bounds.Width > 0 && bounds.Height > 0)
        {
            FillRectangle(brush, bounds);
        }
    }

    /// <summary>
    /// Flushes any pending drawing operations.
    /// </summary>
    public void Flush()
    {
        Flush(FlushIntention.Sync);
    }

    /// <summary>
    /// Flushes any pending drawing operations with the given intention.
    /// </summary>
    /// <param name="intention">The flush intention.
    /// </param>
    public void Flush(FlushIntention intention)
    {
        try
        {
            if (this.image != null && this.ownedSurface != null && this.ownedSurface.Status == Cairo.Status.Success)
            {
                this.image.Pixbuf = new Pixbuf(this.ownedSurface, 0, 0, this.image.Width, this.image.Height);
            }
        }

        finally
        {
            if (intention == FlushIntention.Sync)
            {
                this.ownedSurface?.Dispose();
                this.ownedSurface = null;
                if (this.ownsContext)
                {
                    this.context?.Dispose();
                    this.ownsContext = false;
                }
            }
        }

    }

    /// <summary>
    /// Creates a Graphics object from a device context handle. Not implemented.
    /// </summary>
    /// <param name="hdc">Device context handle.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc)
    {
        throw new PlatformNotSupportedException("GDI device contexts are not available on this platform; create a Graphics from an Image instead.");
    }

    /// <summary>
    /// Creates a Graphics object from a device context handle and device handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
    {
        throw new PlatformNotSupportedException("GDI device contexts are not available on this platform; create a Graphics from an Image instead.");
    }

    /// <summary>
    /// Internal helper to create a Graphics object from HDC. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdcInternal(IntPtr hdc)
    {
        throw new PlatformNotSupportedException("GDI device contexts are not available on this platform; create a Graphics from an Image instead.");
    }

    /// <summary>
    /// Creates a Graphics object from a window handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwnd(IntPtr hwnd)
    {
        throw new PlatformNotSupportedException("Window handles are not available on this platform; draw through the control's paint event instead.");
    }

    /// <summary>
    /// Internal helper to create a Graphics object from a window handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwndInternal(IntPtr hwnd)
    {
        throw new PlatformNotSupportedException("Window handles are not available on this platform; draw through the control's paint event instead.");
    }

    /// <summary>
    /// This method must be executed before useFlush() method outputImage.
    /// Prepare a Graphics instance for drawing into an Image.
    /// </summary>
    /// <param name="image">The target Image to draw into.
    /// </param>
    /// <returns>A Graphics object that draws into the provided image.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when image dimensions are invalid.
    /// </exception>
    public static Graphics FromImage(Image image)
    {
        var _width = image.Width;
        var _height = image.Height;

        if (_width < 1)
            throw new ArgumentOutOfRangeException("Image.Width不能小于等于0");
        if (_height < 1)
            throw new ArgumentOutOfRangeException("Image.Height不能小于等于0");

        var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, _width, _height);
        var imageContext = new Cairo.Context(surface);

        // The blank surface was published straight back as the image's pixbuf, so simply asking an image
        // for a Graphics erased everything already drawn on it - breaking both the "load an image, then
        // annotate it" and the "draw in two passes" patterns. GDI+ composes onto the existing content, so
        // seed the surface from it and let the flush on Dispose publish the result.
        var existing = image.Pixbuf;
        if (existing != null)
        {
            imageContext.Save();
            Gdk.CairoHelper.SetSourcePixbuf(imageContext, existing, 0, 0);
            imageContext.Operator = Cairo.Operator.Source;
            imageContext.Paint();
            imageContext.Restore();
        }
        else
        {
            image.Pixbuf = new Pixbuf(surface, 0, 0, image.Width, image.Height);
        }

        return new Drawing.Graphics(image, imageContext, new Gdk.Rectangle(0, 0, _width, _height))
        {
            ownedSurface = surface,
            ownsContext = true
        };
    }

    /// <summary>
    /// Returns the cumulative clip region and the cumulative transform offset, as GDI+ does:
    /// an <see cref="object" /> array of <c>{ Region, PointF }</c>.
    /// </summary>
    /// <remarks>
    /// This returned <see cref="Cairo.Context" />: a raw, unsynchronised substrate handle, handed out
    /// through a public method on a type whose whole job is to own that handle's lifetime. A caller
    /// holding it could draw, dispose or reconfigure behind the <c>ownsContext</c>/<c>ownedSurface</c>
    /// bookkeeping beside it, and keeping the field <c>internal</c> bought nothing while this method
    /// published it anyway. It now answers the question GDI+ callers actually ask this API - what is
    /// currently clipped, and by how much is drawing offset.
    /// </remarks>
    /// <returns>
    /// <c>object[] { Region clip, PointF transformOffset }</c>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object GetContextInfo()
    {
        var transform = Transform;

        return new object[] { Clip, new PointF(transform.OffsetX, transform.OffsetY) };
    }

    /// <summary>
    /// Gets the halftone palette. Not implemented.
    /// </summary>
    public static IntPtr GetHalftonePalette()
    {
        throw new PlatformNotSupportedException("GDI halftone palettes are not available on this platform.");
    }

    /// <summary>
    /// Returns a handle to the device context for this Graphics object. Not implemented.
    /// </summary>
    public IntPtr GetHdc()
    {
        throw new PlatformNotSupportedException("GDI device contexts are not available on this platform; create a Graphics from an Image instead.");
    }

    /// <summary>
    /// Returns the nearest color available in the palette. Not implemented.
    /// </summary>
    public Color GetNearestColor(Color color)
    {
        // Rendering targets here are 32bpp ARGB, so every colour is representable exactly.
        return color;
    }

    /// <summary>
    /// Intersects the clip region with the specified rectangle. Not implemented.
    /// </summary>
    public void IntersectClip(Rectangle rect)
    {
        // Was a no-op, so narrowing an existing clip had no effect at all.
        SetClip(rect, CombineMode.Intersect);
    }

    /// <summary>
    /// Intersects the clip region with the specified floating-point rectangle. Not implemented.
    /// </summary>
    public void IntersectClip(RectangleF rect)
    {
        SetClip(rect, CombineMode.Intersect);
    }

    /// <summary>
    /// Intersects the clip region with the specified region.
    /// </summary>
    public void IntersectClip(Region region)
    {
        // Was a no-op, so narrowing the clip with a region had no effect at all.
        if (region == null)
        {
            return;
        }

        SetClip(region, CombineMode.Intersect);
    }

    /// <summary>
    /// Determines whether the specified point is contained within the visible clip region.
    /// </summary>
    public bool IsVisible(Point point)
    {
        return IsVisible(point.X, (float)point.Y);
    }

    /// <summary>
    /// Determines whether the specified floating-point point is contained within the visible clip region.
    /// </summary>
    public bool IsVisible(PointF point)
    {
        // The whole IsVisible family threw; the clip bounds are already tracked, so answer from them.
        return ClipBounds.Contains(point);
    }

    /// <summary>
    /// Determines whether the specified rectangle is contained within the visible clip region.
    /// </summary>
    public bool IsVisible(Rectangle rect)
    {
        return IsVisible((float)rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    /// Determines whether the specified floating-point rectangle is contained within the visible clip region.
    /// </summary>
    public bool IsVisible(RectangleF rect)
    {
        return ClipBounds.IntersectsWith(rect);
    }

    /// <summary>
    /// Determines whether the specified coordinates are visible.
    /// </summary>
    public bool IsVisible(int x, int y)
    {
        return IsVisible((float)x, y);
    }

    /// <summary>
    /// Determines whether the specified integer rectangle is visible.
    /// </summary>
    public bool IsVisible(int x, int y, int width, int height)
    {
        return IsVisible((float)x, y, width, height);
    }

    /// <summary>
    /// Determines whether the specified floating point coordinates are visible.
    /// </summary>
    public bool IsVisible(float x, float y)
    {
        return IsVisible(new PointF(x, y));
    }

    /// <summary>
    /// Determines whether the specified floating point rectangle is visible.
    /// </summary>
    public bool IsVisible(float x, float y, float width, float height)
    {
        return IsVisible(new RectangleF(x, y, width, height));
    }

    /// <summary>
    /// Measures character ranges for the specified text. Not implemented.
    /// </summary>
    public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
    {
        // Was throwing NotImplementedException. Produce one region per requested range. Positions come from
        // the *cumulative* Cairo advance of the text preceding each boundary, so kerning and side-bearings
        // between the preceding run and the range are counted once (measuring each substring in isolation
        // double-counts them). Regions are one line tall, matching WinForms' single-line hit-test regions.
        var ranges = stringFormat?.MeasurableCharacterRanges;
        if (text == null || font == null || ranges == null || ranges.Length == 0)
        {
            return Array.Empty<Region>();
        }

        this.context.Save();
        double textSize = 14;
        if (font != null)
        {
            textSize = font.Size;
            if (font.Unit == GraphicsUnit.Point)
                textSize = font.Size * 1 / 72 * 96;
            else if (font.Unit == GraphicsUnit.Inch)
                textSize = font.Size * 96;
            this.context.SetFontSize(textSize);
            this.context.SelectFontFace(font.Name, font.Style.HasFlag(FontStyle.Italic) ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, font.Style.HasFlag(FontStyle.Bold) ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
        }

        var lineHeight = (float)this.context.FontExtents.Height;

        // Cumulative advance up to (but not including) character index n, including the trailing advance of
        // the run — this is where the next glyph starts, so it is the true left edge of a range at n.
        float AdvanceTo(int n) => n <= 0 ? 0f : (float)this.context.TextExtents(text.Substring(0, n)).XAdvance;

        var regions = new Region[ranges.Length];
        for (var i = 0; i < ranges.Length; i++)
        {
            var first = Math.Max(0, Math.Min(ranges[i].First, text.Length));
            var length = Math.Max(0, Math.Min(ranges[i].Length, text.Length - first));

            var startX = AdvanceTo(first);
            var endX = AdvanceTo(first + length);

            regions[i] = new Region(new RectangleF(
                layoutRect.X + startX,
                layoutRect.Y,
                Math.Max(0f, endX - startX),
                lineHeight));
        }

        this.context.Restore();
        return regions;
    }

    /// <summary>
    /// Measures the specified string. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font)
    {
        return MeasureString(text, font, new SizeF(-1, -1), StringFormat.GenericDefault, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Measures the specified string using an origin and format. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
    {
        return MeasureString(text, font, new SizeF(origin) { Width = -1, Height = -1 }, stringFormat, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Measures the specified string within a layout area. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea)
    {
        return MeasureString(text, font, layoutArea, StringFormat.GenericDefault, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Measures the specified string within a layout area and format. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
    {
        return MeasureString(text, font, layoutArea, stringFormat, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Measures the specified string and returns fitted characters and lines. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
    {
        this.context.Save();
        double textSize = 14;
        if (font != null)
        {
            textSize = font.Size;
            if (font.Unit == GraphicsUnit.Point)
                textSize = font.Size * 1 / 72 * 96;
            else if (font.Unit == GraphicsUnit.Inch)
                textSize = font.Size * 96;
            this.context.SetFontSize(textSize);
            this.context.SelectFontFace(font.Name, font.Style.HasFlag(FontStyle.Italic) ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, font.Style.HasFlag(FontStyle.Bold) ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
        }

        var fontheight = (float)this.context.FontExtents.Height;
        double desent = this.context.FontExtents.Descent;
        TextExtents textext = this.context.TextExtents(text);
        var width = (float)(textext.Width + desent);
        var height = fontheight;

        // The measurement used to be single-line and then simply clamped to the layout area, so text that
        // would wrap in WinForms reported the width of the box and the height of one line. Lay the words
        // out against the available width instead and report the real number of lines.
        var lines = 1;
        var wraps = !stringFormat.FormatFlags.HasFlag(StringFormatFlags.NoWrap)
                    && !stringFormat.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical)
                    && layoutArea.Width > 0
                    && layoutArea.Width < float.MaxValue
                    && width > layoutArea.Width;
        if (wraps)
        {
            var widest = 0f;
            var current = string.Empty;
            foreach (var word in text.Split(' '))
            {
                var candidate = current.Length == 0 ? word : current + " " + word;
                var candidateWidth = (float)(this.context.TextExtents(candidate).Width + desent);
                if (candidateWidth <= layoutArea.Width || current.Length == 0)
                {
                    current = candidate;
                    widest = Math.Max(widest, candidateWidth);
                }
                else
                {
                    lines++;
                    widest = Math.Max(widest, (float)(this.context.TextExtents(current).Width + desent));
                    current = word;
                }
            }

            width = Math.Min(layoutArea.Width, Math.Max(widest, (float)(this.context.TextExtents(current).Width + desent)));
            height = fontheight * lines;
        }

        this.context.Restore();
        var result = new SizeF(width, height);
        if (stringFormat.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical))
        {
            if (layoutArea.Width > -1)
                result.Width = Math.Min(layoutArea.Width, height);
            if (layoutArea.Height > -1)
                result.Height = Math.Min(layoutArea.Height, width);
        }

        else
        {
            if (layoutArea.Width > -1)
                result.Width = Math.Min(layoutArea.Width, width);
            if (layoutArea.Height > -1)
                result.Height = Math.Min(layoutArea.Height, height);
        }

        charactersFitted = text.Length;
        linesFilled = Math.Max(1, Convert.ToInt32(result.Height / fontheight));

        return result;
    }

    /// <summary>
    /// Measures a string to fit a width using the specified format.
    /// </summary>
    public SizeF MeasureString(string text, Font font, int width)
    {
        return MeasureString(text, font, new SizeF(width, -1), StringFormat.GenericDefault, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Measures a string to fit a width using the specified format.
    /// </summary>
    public SizeF MeasureString(string text, Font font, int width, StringFormat format)
    {
        return MeasureString(text, font, new SizeF(width, -1), format, out int charactersFitted, out int linesFilled);
    }

    /// <summary>
    /// Multiplies the current transform by the specified matrix.
    /// </summary>
    public void MultiplyTransform(Drawing2D.Matrix matrix)
    {
        MultiplyTransform(matrix, MatrixOrder.Prepend);
    }

    /// <summary>
    /// Multiplies the current transform by the specified matrix with the given order.
    /// </summary>
    public void MultiplyTransform(Drawing2D.Matrix matrix, MatrixOrder order)
    {
        // context.Matrix returns a copy, so multiplying it in place changed nothing; the product has to be
        // assigned back for the world transform to actually change.
        var factor = ConvertToMatrix(matrix);
        if (ReferenceEquals(factor, null))
        {
            return;
        }

        var current = this.context.Matrix;
        if (ReferenceEquals(current, null))
        {
            this.context.Matrix = factor;
            return;
        }

        if (order == MatrixOrder.Prepend)
        {
            factor.Multiply(current);
            this.context.Matrix = factor;
        }
        else
        {
            current.Multiply(factor);
            this.context.Matrix = current;
        }
    }

    /// <summary>
    /// Releases a device context handle obtained by GetHdc. Not implemented.
    /// </summary>
    public void ReleaseHdc()
    {
    }

    /// <summary>
    /// Releases the specified device context handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void ReleaseHdc(IntPtr hdc)
    {
    }

    /// <summary>
    /// Internal helper to release HDC. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ReleaseHdcInternal(IntPtr hdc)
    {
    }

    /// <summary>
    /// Resets the clipping region to infinite.
    /// </summary>
    public void ResetClip()
    {
        this.context.ResetClip();
        this._clipBounds = new RectangleF(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
        // Left Clip reporting whatever it had been set to, even though the clip was gone.
        this._clip = new Region(new Rectangle(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height));
    }

    /// <summary>
    /// Resets the transform applied to the context.
    /// </summary>
    public void ResetTransform()
    {
        // Only the rotation was being undone, so translations and scales survived a ResetTransform.
        this.context.Matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
        this._angle = 0;
    }

    /// <summary>
    /// Restores the graphics state from a saved state.
    /// </summary>
    /// <param name="gstate">Graphics state to restore.
    /// </param>
    public void Restore(GraphicsState gstate)
    {
        this.context.Restore();
    }

    /// <summary>
    /// Rotates the drawing context by the specified angle.
    /// </summary>
    /// <param name="angle">Angle in degrees.
    /// </param>
    public void RotateTransform(float angle)
    {
        this.context.Rotate(Math.PI / 180 * angle);
        _angle = angle;
    }

    /// <summary>
    /// Rotates the drawing context by the specified angle with the given matrix order.
    /// </summary>
    public void RotateTransform(float angle, MatrixOrder order)
    {
        this.context.Rotate(Math.PI / 180 * angle);
        _angle = angle;
    }

    /// <summary>
    /// Saves the current graphics state.
    /// </summary>
    public GraphicsState Save()
    {
        this.context.Save();

        return new GraphicsState();
    }

    /// <summary>
    /// Scales the drawing context.
    /// </summary>
    public void ScaleTransform(float sx, float sy)
    {
        this.context.Scale(sx, sy);
    }

    /// <summary>
    /// Scales the drawing context with the given order.
    /// </summary>
    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        this.context.Scale(sx, sy);
    }

    /// <summary>
    /// Sets the clipping region to the specified path.
    /// </summary>
    public void SetClip(GraphicsPath path)
    {
        SetClip(path, CombineMode.Replace);
    }

    /// <summary>
    /// Sets the clipping region to the specified path with combine mode. Curved paths are approximated
    /// by the band decomposition <see cref="Region(GraphicsPath)"/> produces.
    /// </summary>
    public void SetClip(GraphicsPath path, CombineMode combineMode)
    {
        // Was a no-op, so clipping to a path never restricted anything.
        if (path == null)
        {
            return;
        }

        // Went through path.GetBounds(), which threw away everything but the bounding box even when the
        // path decomposed into several bands. Region(GraphicsPath) keeps that decomposition.
        SetClip(new Region(path), combineMode);
    }

    /// <summary>
    /// Sets the clipping region from another Graphics object. Not implemented.
    /// </summary>
    public void SetClip(Graphics g)
    {
        SetClip(g, CombineMode.Replace);
    }

    /// <summary>
    /// Sets the clipping region from another Graphics object with combine mode.
    /// </summary>
    public void SetClip(Graphics g, CombineMode combineMode)
    {
        if (g == null)
        {
            return;
        }

        // Copied only the other context's bounding box, so a multi-band clip arrived here flattened.
        SetClip(g.Clip, combineMode);
    }

    /// <summary>
    /// Sets the clipping region to the specified rectangle. Not implemented.
    /// </summary>
    public void SetClip(Rectangle rect)
    {
        SetClip(rect, CombineMode.Replace);
    }

    /// <summary>
    /// Sets the clipping region to the specified rectangle with combine mode. Not implemented.
    /// </summary>
    public void SetClip(Rectangle rect, CombineMode combineMode)
    {
        SetClip(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), combineMode);
    }

    /// <summary>
    /// Sets the clipping region to the specified floating-point rectangle. Not implemented.
    /// </summary>
    public void SetClip(RectangleF rect)
    {
        SetClip(rect, CombineMode.Replace);
    }

    /// <summary>
    /// Sets the clipping region to the specified floating-point rectangle with combine mode. Not implemented.
    /// </summary>
    public void SetClip(RectangleF rect, CombineMode combineMode)
    {
        SetClip(new Region(rect), combineMode);
    }

    /// <summary>
    /// Sets the clipping region to the specified region.
    /// </summary>
    public void SetClip(Region region)
    {
        // The Region overload of SetClip was missing entirely, so callers could not clip with a Region.
        SetClip(region, CombineMode.Replace);
    }

    /// <summary>
    /// Sets the clipping region to the specified region with combine mode.
    /// </summary>
    /// <remarks>
    /// Every SetClip/IntersectClip overload funnels through here. Combining happens on the
    /// <see cref="Region" /> - which implements the whole CombineMode family exactly for rectangular
    /// inputs - and the result is then pushed to Cairo in one go. Previously only Replace and Intersect
    /// were honoured: Union, Exclude, Xor and Complement all silently behaved like Intersect, because
    /// Cairo's own Clip() can only ever narrow.
    /// </remarks>
    public void SetClip(Region region, CombineMode combineMode)
    {
        if (region == null)
        {
            return;
        }

        var combined = _clip.Clone();
        switch (combineMode)
        {
            case CombineMode.Replace:
                combined = region.Clone();
                break;
            case CombineMode.Intersect:
                combined.Intersect(region);
                break;
            case CombineMode.Union:
                combined.Union(region);
                break;
            case CombineMode.Exclude:
                combined.Exclude(region);
                break;
            case CombineMode.Xor:
                combined.Xor(region);
                break;
            case CombineMode.Complement:
                combined.Complement(region);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(combineMode), combineMode, "Unknown CombineMode.");
        }

        ApplyClip(combined);
    }

    /// <summary>
    /// Makes <paramref name="clip" /> the active clip: records it as <see cref="Clip" />, refreshes
    /// <see cref="ClipBounds" />, and re-applies it to the Cairo context from scratch.
    /// </summary>
    private void ApplyClip(Region clip)
    {
        _clip = clip;

        // Cairo's Clip() only ever intersects with what is already there, so any change - not just a
        // Replace - has to start from a clean clip and re-apply the whole region.
        context.ResetClip();
        context.NewPath();

        var surfaceBounds = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        if (clip.IsInfinite(this))
        {
            _clipBounds = surfaceBounds;

            return;
        }

        // A region that still knows its own path is clipped through that path, so an ellipse or a
        // rounded rectangle clips on its true curve instead of on the rectangles the band list
        // decomposes it into. Bands remain the fallback - and the only representation left once the
        // region has been combined with another.
        var exactPath = clip.ExactPath;
        if (exactPath != null && exactPath.list.Count > 0)
        {
            context.FillRule = exactPath.FillMode == FillMode.Winding
                ? Cairo.FillRule.Winding
                : Cairo.FillRule.EvenOdd;
            ReplayPath(exactPath, null, forClip: true);
            context.ClosePath();
            context.Clip();
            context.NewPath();
            _clipBounds = exactPath.GetBounds();

            return;
        }

        var bands = clip.Bands;
        if (bands.Count == 0)
        {
            // An empty clip must reject everything. Clipping to a zero-area rectangle is how that is
            // expressed in Cairo; leaving the context unclipped would draw the whole surface instead.
            context.Rectangle(0, 0, 0, 0);
            context.Clip();
            context.NewPath();
            _clipBounds = RectangleF.Empty;

            return;
        }

        var union = bands[0];
        foreach (var band in bands)
        {
            context.Rectangle(band.X, band.Y, band.Width, band.Height);
            union = RectangleF.Union(union, band);
        }

        context.Clip();
        context.NewPath();
        _clipBounds = union;
    }

    /// <summary>
    /// Transforms points between coordinate spaces. Not implemented.
    /// </summary>
    public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
    {
        if (pts == null || destSpace == srcSpace)
        {
            return;
        }

        // World -> Device applies the current transform; the reverse undoes it.
        var toDevice = destSpace == CoordinateSpace.Device;
        for (var i = 0; i < pts.Length; i++)
        {
            double x = pts[i].X;
            double y = pts[i].Y;
            if (toDevice)
            {
                this.context.UserToDevice(ref x, ref y);
            }
            else
            {
                this.context.DeviceToUser(ref x, ref y);
            }

            pts[i] = new PointF((float)x, (float)y);
        }
    }

    /// <summary>
    /// Transforms points between coordinate spaces. Not implemented.
    /// </summary>
    public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
    {
    }

    /// <summary>
    /// Translates the clip region by the specified integer amounts. Not implemented.
    /// </summary>
    public void TranslateClip(int dx, int dy)
    {
        // This called itself instead of the float overload, so any use of it overflowed the stack.
        TranslateClip((float)dx, (float)dy);
    }

    /// <summary>
    /// Translates the clip region by the specified floating point amounts.
    /// </summary>
    public void TranslateClip(float dx, float dy)
    {
        // Cairo's Clip() only ever intersects, so translating onto a disjoint area used to clip everything
        // away. Replace the clip with the shifted rectangle instead.
        var extents = context.ClipExtents();
        SetClip(
            new RectangleF((float)(extents.X + dx), (float)(extents.Y + dy), (float)extents.Width, (float)extents.Height),
            CombineMode.Replace);
    }

    /// <summary>
    /// Translates the drawing origin by the specified amounts.
    /// </summary>
    public void TranslateTransform(float dx, float dy)
    {
        TranslateTransform(dx, dy, MatrixOrder.Append);
    }

    /// <summary>
    /// Translates the drawing origin by the specified amounts with matrix order.
    /// </summary>
    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        context.Translate(dx, dy);
    }

    internal void SetSourcePen(Pen pen, bool isfill)
    {
        if (pen.Brush is SolidBrush sbrush)
        {
            this.context.SetSourceRGBA(sbrush.Color.R / 255f, sbrush.Color.G / 255f, sbrush.Color.B / 255f, sbrush.Color.A / 255f);
        }

        else if (pen.Brush is LinearGradientBrush lbrush)
        {
            var maxsize = Math.Max(diff_left + lbrush.Rectangle.Right, diff_top + lbrush.Rectangle.Bottom); //渐变角度定为方形45度
            using var gradient = new Cairo.LinearGradient(diff_left + lbrush.Rectangle.Left, diff_top + lbrush.Rectangle.Top, maxsize, maxsize);
            var linearcount = lbrush.LinearColors.Length;
            var idx = 0;
            foreach (var color in lbrush.LinearColors)
                gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

            var matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
            matrix.Rotate(Math.PI * 45 / 180);//弧度
            gradient.Matrix = matrix;
            using var pattern = Cairo.Pattern.Lookup(gradient.Handle, false);
            this.context.SetSource(pattern);
        }

        else if (pen.Brush is HatchBrush hbrush)
        {
            this.context.SetSourceRGBA(hbrush.BackgroundColor.R / 255f, hbrush.BackgroundColor.G / 255f, hbrush.BackgroundColor.B / 255f, hbrush.BackgroundColor.A / 255f);
            this.context.Paint();
            this.context.SetSourceRGBA(hbrush.ForegroundColor.R / 255f, hbrush.ForegroundColor.G / 255f, hbrush.ForegroundColor.B / 255f, hbrush.ForegroundColor.A / 255f);
        }

        else if (pen.Brush is TextureBrush tbrush)
        {
            // TextureBrush did not exist; it paints through a Cairo surface pattern, which supports the
            // tile and clamp wrap modes natively.
            using var pattern = tbrush.CreatePattern();
            if (pattern != null)
            {
                this.context.SetSource(pattern);
            }
        }

        else if (pen.Brush is PathGradientBrush pbrush)
        {
            // this brush is fixed to a radial gradient
            var radius = Math.Max(pbrush.Rectangle.Width, pbrush.Rectangle.Height);
            var linearcount = (pbrush.SurroundColors?.Length ?? 0) + 1;
            using var gradient = new Cairo.RadialGradient(diff_left + pbrush.CenterPoint.X, diff_top + pbrush.CenterPoint.Y, radius / linearcount, diff_left + pbrush.CenterPoint.X, diff_top + pbrush.CenterPoint.Y, radius);
            gradient.AddColorStop(1 / linearcount, new Cairo.Color(pbrush.CenterColor.R / 255f, pbrush.CenterColor.G / 255f, pbrush.CenterColor.B / 255f, pbrush.CenterColor.A / 255f));
            var idx = 1;
            if (pbrush.SurroundColors != null)
            {
                foreach (var color in pbrush.SurroundColors)
                {
                    gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
                }
            }

            this.context.SetSource(gradient);
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

    internal void SetTranslateWithDifference(double x, double y)
    {
        this.context.Translate(diff_left + x, diff_top + y);
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
        var nList = new List<PointF> { };
        var listNum = points.Count;
        if (listNum < 2)
        {
            return points.ToList();
        }

        for (var n = 1; n < listNum; n++)
        {
            var nowX = (points[n].X - points[n - 1].X) * time + points[n - 1].X;
            var nowY = (points[n].Y - points[n - 1].Y) * time + points[n - 1].Y;
            var nowP = new PointF(nowX, nowY);
            nList.Add(nowP);
        }

        var p = CalculateBezier(nList, time);

        return p;
    }

    private Cairo.Matrix? ConvertToMatrix(Matrix? matrix)
    {
        if (matrix != null)
        {
            var cairoMatrix = new Cairo.Matrix(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);
            cairoMatrix.Init(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);

            // scaleX/scaleY default to 0 and were applied unconditionally, collapsing every matrix that had
            // never been scaled to all-zero. Only apply a scale that was actually requested.
            if (matrix.scaleX != 0f || matrix.scaleY != 0f)
            {
                cairoMatrix.Scale(matrix.scaleX, matrix.scaleY);
            }

            // Matrix.Rotate takes degrees; Cairo takes radians.
            if (matrix.angle != 0f)
            {
                cairoMatrix.Rotate(Math.PI / 180 * matrix.angle);
            }

            // Cairo.Matrix.Multiply(null) aborts the process natively, so only multiply when there is one.
            // Cairo.Matrix overloads == and dereferences both sides, so compare by reference instead.
            var nested = ConvertToMatrix(matrix.multiply);
            if (!ReferenceEquals(nested, null))
            {
                cairoMatrix.Multiply(nested);
            }

            if (matrix.invert)
                cairoMatrix.Invert();

            return cairoMatrix;
        }

        return null;
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

    private void DrawBeziersCore(Pen pen, PointF[] points)
    {
        var data = GetBezierPoints(points.ToList());
        DrawLinesCore(pen, data.ToArray());
    }

    /// <summary>GDI+'s default cardinal-spline tension for the overloads that do not take one.</summary>
    private const float DefaultCurveTension = 0.5f;

    private void DrawCurveCore(bool isClosePath, bool isfill, Pen pen, PointF[] points, int offset, int numberOfSegments, float tension, FillMode fillmode)
    {
        if (points.Length > 1)
        {
            this.context.Save();
            this.SetTranslateWithDifference(offset, offset);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();

            // Draw a cardinal spline through EVERY point. The previous code emitted a single Bezier from
            // points[0..2], so any further points were ignored (and a 2-point curve threw).
            var count = points.Length;
            var segments = isClosePath ? count : count - 1;
            this.context.MoveTo(points[0].X, points[0].Y);
            for (var i = 0; i < segments; i++)
            {
                var p1 = points[i % count];
                var p2 = points[(i + 1) % count];
                PointF p0, p3;
                if (isClosePath)
                {
                    p0 = points[((i - 1) + count) % count];
                    p3 = points[(i + 2) % count];
                }
                else
                {
                    p0 = points[Math.Max(0, i - 1)];
                    p3 = points[Math.Min(count - 1, i + 2)];
                }

                var scale = tension / 3f;
                var c1 = new PointF(p1.X + ((p2.X - p0.X) * scale), p1.Y + ((p2.Y - p0.Y) * scale));
                var c2 = new PointF(p2.X - ((p3.X - p1.X) * scale), p2.Y - ((p3.Y - p1.Y) * scale));
                this.context.CurveTo(c1.X, c1.Y, c2.X, c2.Y, p2.X, p2.Y);
            }

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

    private void DrawEllipseCore(Pen pen, float x, float y, float width, float height, bool isfill, FillMode fillmode)
    {
        this.context.Save();
        this.SetTranslateWithDifference(x + width / 2, y + height / 2);
        this.SetSourcePen(pen, isfill);
        this.context.NewPath();
        var r = (width + height) / 4;
        var rs = Math.Min(0.1, 2 / r);
        for (double t = 0; t < 2 * Math.PI; t += rs)
        {
            var x2_1 = width * Math.Cos(t) / 2;
            var y2_1 = height * Math.Sin(t) / 2;
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
    }

    private void DrawImageScaledCore(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes? imageAttrs, DrawImageAbort? callback, IntPtr callbackData)
    {
        var img = new Gdk.Pixbuf(image.PixbufData);
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
            var scaleimg = new Gdk.Pixbuf(surface, 0, 0, destRect.Width, destRect.Height);

            // Gdk's offsetX/offsetY position the SCALED source inside the destination, so selecting the
            // source sub-rectangle needs -srcX*scale (passing srcX directly ignored the source rectangle
            // and drew the wrong part of the image).
            var scaleX = destRect.Width / srcWidth;
            var scaleY = destRect.Height / srcHeight;
            img.Scale(scaleimg, 0, 0, destRect.Width, destRect.Height, -srcX * scaleX, -srcY * scaleY, scaleX, scaleY, ImageInterpolation);
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
        }
    }

    private void DrawImageUnscaledCore(Image image, int x, int y, int width, int height, bool clipped = false)
    {
        var img = new Gdk.Pixbuf(image.PixbufData);
        if (width == 0)
            width = img.Width;
        if (height == 0)
            height = img.Height;
        using (var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, width, height))
        {
            var newimg = new Gdk.Pixbuf(surface, 0, 0, width, height);
            // Copy from the SOURCE origin: (x,y) is the destination position (applied below via
            // SetTranslateWithDifference), not an offset into the source image. Using it here read a
            // region outside the source, so nothing was drawn.
            var copyWidth = Math.Min(width, img.Width);
            var copyHeight = Math.Min(height, img.Height);
            img.CopyArea(0, 0, copyWidth, copyHeight, newimg, 0, 0);
            this.context.Save();
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

            this.SetTranslateWithDifference(x, y);
            Gdk.CairoHelper.SetSourcePixbuf(this.context, newimg, 0, 0);
            this.context.Paint();
            this.context.Restore();
        }
    }

    private void DrawLinesCore(Pen pen, PointF[] points)
    {
        if (points.Length > 0)
        {
            this.context.Save();
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, false);
            this.context.NewPath();
            foreach (var p in points)
            {
                this.context.LineTo(p.X, p.Y);
            }

            this.context.Stroke();
            this.context.Restore();
        }
    }

    /// <summary>
    /// Walks a <see cref="GraphicsPath" />'s recorded figures into the current Cairo path.
    /// </summary>
    /// <param name="path">The path to replay.</param>
    /// <param name="pen">The pen nested sub-paths are drawn with; unused when <paramref name="forClip" /> is true.</param>
    /// <param name="forClip">
    /// True when the caller is going to <c>Clip()</c> on the result rather than fill or stroke it. Two
    /// figure kinds have to behave differently in that case: text contributes its glyph outlines
    /// (<c>TextPath</c>) instead of being painted (<c>ShowText</c>), and a nested path contributes its
    /// geometry instead of being drawn as a side effect.
    /// </param>
    /// <remarks>
    /// Pulled out of <see cref="DrawPathCore" /> so that <see cref="SetClip(Region, Drawing2D.CombineMode)" />
    /// can clip to a region's true shape - curves included - rather than to the rectangles its band list
    /// decomposes that shape into.
    /// </remarks>
    private void ReplayPath(GraphicsPath path, Pen? pen, bool forClip)
    {
        foreach (var o in path.list)
        {
            if (o is GraphicsPath.FigureMode start && start.start == true)
            {
                this.context.NewSubPath();
            }

            else if (o is GraphicsPath.ArcMode arc)
            {
                double rw = arc.rect.Width / 2;
                double rh = arc.rect.Height / 2;
                var ra = Math.Min(rw, rh);
                this.context.Arc(arc.rect.X + rw, arc.rect.Y + rh, ra, Math.PI * arc.startAngle / 180, Math.PI * (arc.startAngle + arc.sweepAngle) / 180);
            }

            else if (o is GraphicsPath.BezierMode bezier)
            {
                this.context.MoveTo(bezier.pt1.X, bezier.pt1.Y);
                var data = GetBezierPoints(new List<PointF>() { bezier.pt1, bezier.pt2, bezier.pt3, bezier.pt4 });
                foreach (var point in data)
                {
                    this.context.LineTo(point.X, point.Y);
                }
            }

            else if (o is GraphicsPath.BeziersMode beziers)
            {
                var data = GetBezierPoints(beziers.points.ToList());
                foreach (var point in data)
                {
                    this.context.LineTo(point.X, point.Y);
                }
            }

            else if (o is GraphicsPath.ClosedCurveMode closedcurve)
            {
                if (closedcurve.points != null)
                    this.context.CurveTo(closedcurve.points[0].X, closedcurve.points[0].Y, closedcurve.points[1].X,
                        closedcurve.points[1].Y, closedcurve.points[2].X, closedcurve.points[2].Y);
                this.context.FillRule = closedcurve.fillmode == FillMode.Winding ? Cairo.FillRule.Winding : Cairo.FillRule.EvenOdd;
                this.context.ClosePath();
                this.context.NewSubPath();
            }

            else if (o is GraphicsPath.CurveMode curve)
            {
                if (curve.points != null)
                    this.context.CurveTo(curve.points[0].X + curve.offset, curve.points[0].Y + curve.offset,
                        curve.points[1].X + curve.offset, curve.points[1].Y + curve.offset,
                        curve.points[2].X + curve.offset, curve.points[2].Y + curve.offset);
            }

            else if (o is GraphicsPath.EllipseMode ellipse)
            {
                this.context.NewSubPath();
                var r = (ellipse.rect.Width + ellipse.rect.Height) / 4;
                var rs = Math.Min(0.1, 2 / r);
                for (double t = 0; t < 2 * Math.PI; t += rs)
                {
                    var x2_1 = ellipse.rect.Width * Math.Cos(t) / 2;
                    var y2_1 = ellipse.rect.Height * Math.Sin(t) / 2;
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
                if (lines.points != null)
                {
                    this.context.MoveTo(lines.points[0].X, lines.points[0].Y);
                    foreach (var p in lines.points)
                    {
                        this.context.LineTo(p.X, p.Y);
                    }
                }
            }

            else if (o is GraphicsPath.PieMode pie)
            {
                this.context.NewSubPath();
                double rw = pie.rect.Width / 2;
                double rh = pie.rect.Height / 2;
                var ra = Math.Min(rw, rh);
                this.context.Arc(pie.rect.X + rw, pie.rect.Y + rh, ra, Math.PI * pie.startAngle / 180, Math.PI * (pie.startAngle + pie.sweepAngle) / 180);
                this.context.LineTo(pie.rect.X + rw, pie.rect.Y + rh);
                this.context.ClosePath();
                this.context.NewSubPath();
            }

            else if (o is GraphicsPath.PolygonMode polygon)
            {
                this.context.NewSubPath();
                if (polygon.points != null)
                    foreach (var p in polygon.points)
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
                if (rectangles.rects != null)
                    foreach (var rect in rectangles.rects)
                    {
                        this.context.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    }
            }

            else if (o is GraphicsPath.StringMode str)
            {
                var text = str.text;
                var textSize = str.emSize < 1 ? 14f : str.emSize;
                var family = str.family?.Name;
                if (string.IsNullOrWhiteSpace(family) && this.widget != null)
                {
                    var pangocontext = this.widget.PangoContext;
                    family = pangocontext.FontDescription.Family;
                }

                this.context.SelectFontFace(family, str.style == 2 ? Cairo.FontSlant.Italic : Cairo.FontSlant.Normal, str.style == 1 ? Cairo.FontWeight.Bold : Cairo.FontWeight.Normal);
                this.context.SetFontSize(textSize);
                var textext = this.context.TextExtents(text);

                if (str.format == null)
                {
                    this.context.MoveTo(str.layoutRect.X, str.layoutRect.Y + textext.Height);
                }

                else
                {
                    double hAlign = 0, vAlign = 0;
                    if (str.format.Alignment == StringAlignment.Center)
                        hAlign -= textext.Width / 2;
                    else if (str.format.Alignment == StringAlignment.Far)
                        hAlign -= textext.Width;

                    if (str.format.LineAlignment == StringAlignment.Center)
                        vAlign -= textext.Height / 2;
                    else if (str.format.LineAlignment == StringAlignment.Far)
                        vAlign -= textext.Height;

                    if (str.format.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical))
                    {
                        double desent = this.context.FontExtents.Descent;
                        vAlign = desent + 2;
                        this.context.Rotate(90 * Math.PI / 180);
                        this.context.MoveTo(str.layoutRect.X + hAlign, 0 - str.layoutRect.Y - vAlign);
                    }

                    else
                    {
                        this.context.MoveTo(str.layoutRect.X + hAlign, str.layoutRect.Y + vAlign);
                    }
                }

                if (forClip)
                {
                    // Painting the glyphs would draw, not clip; their outlines are what the clip needs.
                    this.context.TextPath(text);
                }
                else
                {
                    this.context.ShowText(text);
                }
            }

            else if (o is GraphicsPath.PathMode addpath)
            {
                if (forClip)
                {
                    if (addpath.path != null)
                    {
                        ReplayPath(addpath.path, pen, forClip);
                    }
                }
                else
                {
                    DrawPath(pen, addpath.path);
                }
            }

            if (path.IsCloseAllFigures == true || (o is GraphicsPath.FigureMode close && close.close == true))
            {
                this.context.ClosePath();
            }
        }
    }

    private void DrawPathCore(Pen pen, GraphicsPath? path, bool isfill)
    {
        if (path == null)
            return;
        this.context.Save();
        path.Context = this.context;
        this.SetTranslateWithDifference(0, 0);
        this.SetSourcePen(pen, isfill);
        this.context.NewPath();
        ReplayPath(path, pen, forClip: false);

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

    private void DrawPolygonCore(bool isfill, Pen pen, PointF[] points, FillMode fillmode)
    {
        if (points.Length > 0)
        {
            this.context.Save();
            this.SetTranslateWithDifference(0, 0);
            this.SetSourcePen(pen, isfill);
            this.context.NewPath();
            foreach (var p in points)
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
        for (var i = 1; i < points.Count; i++)
        {
            seedNum += Math.Abs(points[i].X - points[i - 1].X) + Math.Abs(points[i].Y - points[i - 1].Y);
        }

        seedNum += seedNum * 0.2f;
        var pStep = 1 / seedNum;
        var rpoint = new List<PointF>();
        for (float pTime = 0; pTime <= 1; pTime += pStep)
        {
            var lfpr = CalculateBezier(points, pTime);
            var fpr = lfpr[0];
            rpoint.Add(fpr);
        }

        return rpoint;
    }

    /// <summary>
    /// Delegate used to optionally abort image drawing.
    /// </summary>
    /// <param name="callbackdata">Callback data pointer.
    /// </param>
    /// <returns>True to continue, false to abort.
    /// </returns>
    public delegate bool DrawImageAbort(IntPtr callbackdata);

    /// <summary>
    /// Delegate used when enumerating metafile records.
    /// </summary>
    public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData);

}
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Cairo;
using Gdk;
using Pango;

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
    public Cairo.Context context;

    /// <summary>
    /// Global/static current Cairo context for convenience in some operations.
    /// </summary>
    public static Cairo.Context Context;

    private float _angle = 0;

    private static Cairo.Context imagecontext;

    private static Cairo.ImageSurface imagesurface;

    private Gdk.Rectangle rectangle;

    private static Cairo.Surface simisurface;

    private readonly Gtk.Widget widget;

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

    /// <summary>
    /// Gets or sets the clipping region for this Graphics object.
    /// </summary>
    public Region Clip
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the bounds of the clipping region of this Graphics object.
    /// </summary>
    public RectangleF ClipBounds
    {
        get
        {
            return new RectangleF(this.rectangle.X, this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
        }
    }

    /// <summary>
    /// Gets or sets the compositing mode for this Graphics object.
    /// </summary>
    public CompositingMode CompositingMode
    {
        get;
        set;
    }

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
    public float DpiX
    {
        get;
    }

    /// <summary>
    /// Gets the vertical resolution, in dots per inch.
    /// </summary>
    public float DpiY
    {
        get;
    }

    /// <summary>
    /// Gets or sets the interpolation mode used for scaling operations.
    /// </summary>
    public InterpolationMode InterpolationMode
    {
        get;
        set;
    }

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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the unit of measure used for page coordinates.
    /// </summary>
    public GraphicsUnit PageUnit
    {
        get;
        set;
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
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text contrast used by this Graphics object.
    /// Not implemented in this wrapper.
    /// </summary>
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
    /// Gets or sets the text rendering hint. Not implemented in this wrapper.
    /// </summary>
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
    /// Gets or sets the transformation matrix for this Graphics object.
    /// Not fully implemented in this wrapper.
    /// </summary>
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
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Begins a graphics container with the specified source and destination rectangles.
    /// Not implemented.
    /// </summary>
    public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Begins a graphics container with floating point rectangles. Not implemented.
    /// </summary>
    public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
    {
        throw new System.NotImplementedException();
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
        DrawCurveCore(true, false, pen, points, 0, 0, 0, FillMode.Winding);
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
        DrawCurveCore(true, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
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
        DrawCurveCore(false, false, pen, points, 0, 0, 0, FillMode.Winding);
    }

    /// <summary>
    /// Draws a cardinal spline with offset and number of segments.
    /// </summary>
    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
    {
        DrawCurveCore(false, false, pen, points, offset, numberOfSegments, 0, FillMode.Winding);
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
        DrawCurveCore(false, false, pen, Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
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
    public void DrawPath(Pen pen, GraphicsPath path)
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
        foreach (RectangleF rect in rects)
            DrawRectangle(pen, rect);

    }

    /// <summary>
    /// Draws multiple rectangles from integer rectangles.
    /// </summary>
    public void DrawRectangles(Pen pen, Rectangle[] rects)
    {
        foreach (Rectangle rect in rects)
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
            string family = font?.Name;
            var textSize = 14f;
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
    /// Excludes the specified rectangle from the current clip region. Not implemented.
    /// </summary>
    public void ExcludeClip(Rectangle rect)
    {
    }

    /// <summary>
    /// Excludes the specified region from the current clip region. Not implemented.
    /// </summary>
    public void ExcludeClip(Region region)
    {
    }

    /// <summary>
    /// Fills a closed curve defined by an array of points.
    /// </summary>
    public void FillClosedCurve(Brush brush, PointF[] points)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, 0, FillMode.Winding);
    }

    /// <summary>
    /// Fills a closed curve with specified fill mode.
    /// </summary>
    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), points, 0, 0, 0, fillmode);
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
        DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, FillMode.Winding);
    }

    /// <summary>
    /// Fills a closed curve from integer points with fill mode.
    /// </summary>
    public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
    {
        DrawCurveCore(true, true, new Pen(brush, 0), Array.ConvertAll(points, p => new PointF(p.X, p.Y)), 0, 0, 0, fillmode);
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
        foreach (RectangleF rect in rects)
            FillRectangle(brush, rect);
    }

    /// <summary>
    /// Fills multiple rectangles defined by integer rectangles.
    /// </summary>
    public void FillRectangles(Brush brush, Rectangle[] rects)
    {
        foreach (Rectangle rect in rects)
            FillRectangle(brush, rect);
    }

    /// <summary>
    /// Fills the interior of the specified region. Not implemented.
    /// </summary>
    public void FillRegion(Brush brush, Region region)
    {
    }

    /// <summary>
    /// Flushes any pending drawing operations.
    /// </summary>
    public void Flush()
    {
        Flush(FlushIntention.Flush);
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

    /// <summary>
    /// Creates a Graphics object from a device context handle. Not implemented.
    /// </summary>
    /// <param name="hdc">Device context handle.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Creates a Graphics object from a device context handle and device handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Internal helper to create a Graphics object from HDC. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdcInternal(IntPtr hdc)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Creates a Graphics object from a window handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwnd(IntPtr hwnd)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Internal helper to create a Graphics object from a window handle. Not implemented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwndInternal(IntPtr hwnd)
    {
        throw new System.NotImplementedException();
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
    /// Returns the underlying context info object. Exposed for advanced uses.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public object GetContextInfo()
    {
        return this.context;
    }

    /// <summary>
    /// Gets the halftone palette. Not implemented.
    /// </summary>
    public static IntPtr GetHalftonePalette()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a handle to the device context for this Graphics object. Not implemented.
    /// </summary>
    public IntPtr GetHdc()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the nearest color available in the palette. Not implemented.
    /// </summary>
    public Color GetNearestColor(Color color)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Intersects the clip region with the specified rectangle. Not implemented.
    /// </summary>
    public void IntersectClip(Rectangle rect)
    {
    }

    /// <summary>
    /// Intersects the clip region with the specified floating-point rectangle. Not implemented.
    /// </summary>
    public void IntersectClip(RectangleF rect)
    {
    }

    /// <summary>
    /// Intersects the clip region with the specified region. Not implemented.
    /// </summary>
    public void IntersectClip(Region region)
    {
    }

    /// <summary>
    /// Determines whether the specified point is contained within the visible clip region. Not implemented.
    /// </summary>
    public bool IsVisible(Point point)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified floating-point point is contained within the visible clip region. Not implemented.
    /// </summary>
    public bool IsVisible(PointF point)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified rectangle is contained within the visible clip region. Not implemented.
    /// </summary>
    public bool IsVisible(Rectangle rect)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified floating-point rectangle is contained within the visible clip region. Not implemented.
    /// </summary>
    public bool IsVisible(RectangleF rect)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified coordinates are visible. Not implemented.
    /// </summary>
    public bool IsVisible(int x, int y)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified integer rectangle is visible. Not implemented.
    /// </summary>
    public bool IsVisible(int x, int y, int width, int height)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified floating point coordinates are visible. Not implemented.
    /// </summary>
    public bool IsVisible(float x, float y)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Determines whether the specified floating point rectangle is visible. Not implemented.
    /// </summary>
    public bool IsVisible(float x, float y, float width, float height)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures character ranges for the specified text. Not implemented.
    /// </summary>
    public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures the specified string. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures the specified string using an origin and format. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures the specified string within a layout area. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures the specified string within a layout area and format. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures the specified string and returns fitted characters and lines. Not implemented.
    /// </summary>
    public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Measures a string to fit a width using the specified format.
    /// </summary>
    public SizeF MeasureString(string text, Font font, int width)
    {
        return MeasureString(text, font, width, StringFormat.GenericDefault);
    }

    /// <summary>
    /// Measures a string to fit a width using the specified format.
    /// </summary>
    public SizeF MeasureString(string text, Font font, int width, StringFormat format)
    {
        var textSize = 14f;
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
    /// Multiplies the current transform by the specified matrix.
    /// </summary>
    public void MultiplyTransform(Drawing2D.Matrix matrix)
    {
        this.context.Matrix?.Multiply(ConvertToMatrix(matrix));
    }

    /// <summary>
    /// Multiplies the current transform by the specified matrix with the given order.
    /// </summary>
    public void MultiplyTransform(Drawing2D.Matrix matrix, MatrixOrder order)
    {
        this.context.Matrix?.Multiply(ConvertToMatrix(matrix));
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
    }

    /// <summary>
    /// Resets the transform applied to the context.
    /// </summary>
    public void ResetTransform()
    {
        this.context.Rotate(Math.PI / 180 * _angle * -1);
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
    /// Sets the clipping region to the specified path. Not implemented.
    /// </summary>
    public void SetClip(GraphicsPath path)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified path with combine mode. Not implemented.
    /// </summary>
    public void SetClip(GraphicsPath path, CombineMode combineMode)
    {
    }

    /// <summary>
    /// Sets the clipping region from another Graphics object. Not implemented.
    /// </summary>
    public void SetClip(Graphics g)
    {
    }

    /// <summary>
    /// Sets the clipping region from another Graphics object with combine mode. Not implemented.
    /// </summary>
    public void SetClip(Graphics g, CombineMode combineMode)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified rectangle. Not implemented.
    /// </summary>
    public void SetClip(Rectangle rect)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified rectangle with combine mode. Not implemented.
    /// </summary>
    public void SetClip(Rectangle rect, CombineMode combineMode)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified floating-point rectangle. Not implemented.
    /// </summary>
    public void SetClip(RectangleF rect)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified floating-point rectangle with combine mode. Not implemented.
    /// </summary>
    public void SetClip(RectangleF rect, CombineMode combineMode)
    {
    }

    /// <summary>
    /// Sets the clipping region to the specified region with combine mode. Not implemented.
    /// </summary>
    public void SetClip(Region region, CombineMode combineMode)
    {
    }

    /// <summary>
    /// Transforms points between coordinate spaces. Not implemented.
    /// </summary>
    public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
    {
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
    }

    /// <summary>
    /// Translates the clip region by the specified floating point amounts. Not implemented.
    /// </summary>
    public void TranslateClip(float dx, float dy)
    {
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
        this.SetTranslateWithDifference(dx, dy);
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
            using var gradient = new Cairo.LinearGradient(diff_left + lbrush.Rectangle.Left, diff_top + lbrush.Rectangle.Top, maxsize, maxsize);
            int linearcount = lbrush.LinearColors.Length;
            var idx = 0;
            foreach (Color color in lbrush.LinearColors)
                gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

            var matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
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
            using var gradient = new Cairo.LinearGradient(diff_left + pbrush.Rectangle.Left, diff_top + pbrush.Rectangle.Top, maxsize, maxsize);
            int linearcount = pbrush.SurroundColors.Length;
            var centeridx = Math.Floor((double)linearcount / 2);
            var idx = 0;
            foreach (Color color in pbrush.SurroundColors)
            {
                if (idx == centeridx)
                    gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(pbrush.CenterColor.R / 255f, pbrush.CenterColor.G / 255f, pbrush.CenterColor.B / 255f, pbrush.CenterColor.A / 255f));
                else
                    gradient.AddColorStop((++idx) / linearcount, new Cairo.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
            }

            var matrix = new Cairo.Matrix(1, 0, 0, 1, 0, 0);
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
        int listNum = points.Count;
        if (listNum < 2)
        {
            return points.ToList();
        }

        for (int n = 1; n < listNum; n++)
        {
            float nowX = (points[n].X - points[n - 1].X) * time + points[n - 1].X;
            float nowY = (points[n].Y - points[n - 1].Y) * time + points[n - 1].Y;
            var nowP = new PointF(nowX, nowY);
            nList.Add(nowP);
        }

        var p = CalculateBezier(nList, time);

        return p;
    }

    private Cairo.Matrix ConvertToMatrix(Drawing2D.Matrix matrix)
    {
        var CairoMatrix = new Cairo.Matrix(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);
        CairoMatrix.Init(matrix.m11, matrix.m12, matrix.m21, matrix.m22, matrix.dx, matrix.dy);

        CairoMatrix.Translate(matrix.OffsetX, matrix.OffsetY);
        CairoMatrix.Scale(matrix.scaleX, matrix.scaleY);
        CairoMatrix.Rotate(matrix.angle);
        CairoMatrix.Multiply(ConvertToMatrix(matrix.multiply));
        if (matrix.invert)
            CairoMatrix.Invert();

        return CairoMatrix;
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
        QueueDraw();
    }

    private void DrawImageScaledCore(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
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
                var ra = Math.Min(rw, rh);
                this.context.Arc(arc.rect.X + rw, arc.rect.Y + rh, ra, Math.PI * arc.startAngle / 180, Math.PI * (arc.startAngle + arc.sweepAngle) / 180);
            }

            else if (o is GraphicsPath.BezierMode bezier)
            {
                this.context.MoveTo(bezier.pt1.X, bezier.pt1.Y);
                var data = GetBezierPoints(new List<PointF>() { bezier.pt1, bezier.pt2, bezier.pt3, bezier.pt4 });
                foreach (PointF point in data)
                {
                    this.context.LineTo(point.X, point.Y);
                }
            }

            else if (o is GraphicsPath.BeziersMode beziers)
            {
                var data = GetBezierPoints(beziers.points.ToList());
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
                var rs = Math.Min(0.1, 2 / r);
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
                var ra = Math.Min(rw, rh);
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
        var pStep = 1 / seedNum;
        var rpoint = new List<PointF>();
        for (float pTime = 0; pTime <= 1; pTime += pStep)
        {
            var lfpr = CalculateBezier(points, pTime);
            PointF fpr = lfpr[0];
            rpoint.Add(fpr);
        }

        return rpoint;
    }

    private void QueueDraw()
    {

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
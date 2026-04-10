using System.Collections;
using System.Collections.Generic;
using Pango;
using Context = Cairo.Context;

namespace System.Drawing.Drawing2D;

/// <summary>
/// Represents a series of connected lines and curves. This class cannot be inherited.
/// </summary>
public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
{
    /// <summary>
    /// The size points
    /// </summary>
    public List<PointF> SizePoints = new List<PointF>();

    internal ArrayList list = new ArrayList();

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with a
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.FillMode" /> value of
    /// <see cref="T:System.Drawing.Drawing2D.FillMode.Alternate" />.
    /// </summary>
    public GraphicsPath()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration.
    /// </summary>
    /// <param name="fillMode">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the interior of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> is filled.
    /// </param>
    public GraphicsPath(FillMode fillMode) : this(new PointF[0], null, fillMode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> array with the specified
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and
    /// <see cref="T:System.Drawing.PointF" /> arrays.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that defines the coordinates of the points that make up this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </param>
    /// <param name="types">
    /// An array of
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the
    /// <paramref name="pts" /> array.
    /// </param>
    public GraphicsPath(PointF[] pts, byte[] types) : this(pts, types, FillMode.Alternate)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> array with the specified
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and
    /// <see cref="T:System.Drawing.PointF" /> arrays and with the specified
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration element.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that defines the coordinates of the points that make up this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </param>
    /// <param name="types">
    /// An array of
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the
    /// <paramref name="pts" /> array.
    /// </param>
    /// <param name="fillMode">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.
    /// </param>
    public GraphicsPath(PointF[] pts, byte[]? types, FillMode fillMode)
    {
        this.PathPoints = pts;
        this.PointCount = pts.Length;
        this.PathTypes = types;
        this.FillMode = fillMode;
        this.PathData = new PathData() { Points = pts, Types = types };
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and
    /// <see cref="T:System.Drawing.Point" /> arrays.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that defines the coordinates of the points that make up this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </param>
    /// <param name="types">
    /// An array of
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the
    /// <paramref name="pts" /> array.
    /// </param>
    public GraphicsPath(Point[] pts, byte[] types) : this(pts, types, FillMode.Alternate)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> class with the specified
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> and
    /// <see cref="T:System.Drawing.Point" /> arrays and with the specified
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration element.
    /// </summary>
    /// <param name="pts">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that defines the coordinates of the points that make up this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </param>
    /// <param name="types">
    /// An array of
    /// <see cref="T:System.Drawing.Drawing2D.PathPointType" /> enumeration elements that specifies the type of each corresponding point in the
    /// <paramref name="pts" /> array.
    /// </param>
    /// <param name="fillMode">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.
    /// </param>
    public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode) : this(Array.ConvertAll<Point, PointF>(pts, o => new PointF(o.X, o.Y)), types, fillMode)
    {
    }

    /// <summary>
    /// Gets or sets a
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that determines how the interiors of shapes in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> enumeration that specifies how the interiors of shapes in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> are filled.
    /// </returns>
    public FillMode FillMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a
    /// <see cref="T:System.Drawing.Drawing2D.PathData" /> that encapsulates arrays of points for this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.PathData" /> that encapsulates arrays for both the points and types for this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </returns>
    public PathData? PathData
    {
        get;

        private set;
    }

    /// <summary>
    /// Gets the points in the path.
    /// </summary>
    /// <returns>
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> objects that represent the path.
    /// </returns>
    public PointF[]? PathPoints
    {
        get;

        private set;
    }

    /// <summary>
    /// Gets the types of the corresponding points in the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array.
    /// </summary>
    /// <returns>
    /// An array of bytes that specifies the types of the corresponding points in the path.
    /// </returns>
    public byte[]? PathTypes
    {
        get;

        private set;
    }

    /// <summary>
    /// Gets the number of elements in the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> or the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> array.
    /// </summary>
    /// <returns>
    /// An integer that specifies the number of elements in the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> or the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> array.
    /// </returns>
    public int PointCount
    {
        get;

        private set;
    }

    internal Context? Context
    {
        get;
        set;
    }

    internal PointF[]? destPoints
    {
        get;
        set;
    }

    internal float flatness
    {
        get;
        set;
    }

    internal bool IsCloseAllFigures
    {
        get;
        set;
    }

    internal Matrix? matrix
    {
        get;
        set;
    }

    internal Pen? pen
    {
        get;
        set;
    }

    internal RectangleF srcRect
    {
        get;
        set;
    }

    internal WarpMode warpMode
    {
        get;
        set;
    }

    /// <summary>
    /// Appends an elliptical arc to the current figure.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> that represents the rectangular bounds of the ellipse from which the arc is taken.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle of the arc, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the arc.
    /// </param>
    public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new ArcMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Appends an elliptical arc to the current figure.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangular bounds of the ellipse from which the arc is taken.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle of the arc, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the arc.
    /// </param>
    public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new ArcMode() { rect = rect, startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Appends an elliptical arc to the current figure.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="width">
    /// The width of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="height">
    /// The height of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle of the arc, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the arc.
    /// </param>
    public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(x, y), new PointF(x + width, y + height));
        list.Add(new ArcMode() { rect = new RectangleF(x, y, width, height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Appends an elliptical arc to the current figure.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="width">
    /// The width of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="height">
    /// The height of the rectangular region that defines the ellipse from which the arc is drawn.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle of the arc, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the arc.
    /// </param>
    public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(x, y), new PointF(x + width, y + height));
        list.Add(new ArcMode() { rect = new RectangleF(x, y, width, height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Adds a cubic Bézier curve to the current figure.
    /// </summary>
    /// <param name="pt1">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the starting point of the curve.
    /// </param>
    /// <param name="pt2">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the first control point for the curve.
    /// </param>
    /// <param name="pt3">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the second control point for the curve.
    /// </param>
    /// <param name="pt4">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the endpoint of the curve.
    /// </param>
    public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
    {
        AddPathPoints(new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y), new PointF(pt3.X, pt3.Y), new PointF(pt4.X, pt4.Y));
        list.Add(new BezierMode() { pt1 = new PointF(pt1.X, pt1.Y), pt2 = new PointF(pt2.X, pt2.Y), pt3 = new PointF(pt3.X, pt3.Y), pt4 = new PointF(pt4.X, pt4.Y) });
    }

    /// <summary>
    /// Adds a cubic Bézier curve to the current figure.
    /// </summary>
    /// <param name="pt1">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the starting point of the curve.
    /// </param>
    /// <param name="pt2">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the first control point for the curve.
    /// </param>
    /// <param name="pt3">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the second control point for the curve.
    /// </param>
    /// <param name="pt4">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the endpoint of the curve.
    /// </param>
    public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
    {
        AddPathPoints(new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y), new PointF(pt3.X, pt3.Y), new PointF(pt4.X, pt4.Y));
        list.Add(new BezierMode() { pt1 = new PointF(pt1.X, pt1.Y), pt2 = new PointF(pt2.X, pt2.Y), pt3 = new PointF(pt3.X, pt3.Y), pt4 = new PointF(pt4.X, pt4.Y) });
    }

    /// <summary>
    /// Adds a cubic Bézier curve to the current figure.
    /// </summary>
    /// <param name="x1">
    /// The x-coordinate of the starting point of the curve.
    /// </param>
    /// <param name="y1">
    /// The y-coordinate of the starting point of the curve.
    /// </param>
    /// <param name="x2">
    /// The x-coordinate of the first control point for the curve.
    /// </param>
    /// <param name="y2">
    /// The y-coordinate of the first control point for the curve.
    /// </param>
    /// <param name="x3">
    /// The x-coordinate of the second control point for the curve.
    /// </param>
    /// <param name="y3">
    /// The y-coordinate of the second control point for the curve.
    /// </param>
    /// <param name="x4">
    /// The x-coordinate of the endpoint of the curve.
    /// </param>
    /// <param name="y4">
    /// The y-coordinate of the endpoint of the curve.
    /// </param>
    public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        AddPathPoints(new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4));
        list.Add(new BezierMode() { pt1 = new PointF(x1, y1), pt2 = new PointF(x2, y2), pt3 = new PointF(x3, y3), pt4 = new PointF(x4, y4) });
    }

    /// <summary>
    /// Adds a cubic Bézier curve to the current figure.
    /// </summary>
    /// <param name="x1">
    /// The x-coordinate of the starting point of the curve.
    /// </param>
    /// <param name="y1">
    /// The y-coordinate of the starting point of the curve.
    /// </param>
    /// <param name="x2">
    /// The x-coordinate of the first control point for the curve.
    /// </param>
    /// <param name="y2">
    /// The y-coordinate of the first control point for the curve.
    /// </param>
    /// <param name="x3">
    /// The x-coordinate of the second control point for the curve.
    /// </param>
    /// <param name="y3">
    /// The y-coordinate of the second control point for the curve.
    /// </param>
    /// <param name="x4">
    /// The x-coordinate of the endpoint of the curve.
    /// </param>
    /// <param name="y4">
    /// The y-coordinate of the endpoint of the curve.
    /// </param>
    public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        AddPathPoints(new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4));
        list.Add(new BezierMode() { pt1 = new PointF(x1, y1), pt2 = new PointF(x2, y2), pt3 = new PointF(x3, y3), pt4 = new PointF(x4, y4) });
    }

    /// <summary>
    /// Adds a sequence of connected cubic Bézier curves to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curves.
    /// </param>
    public void AddBeziers(PointF[] points)
    {
        AddPathPoints(points);
        list.Add(new BeziersMode() { points = points });
    }

    /// <summary>
    /// Adds a sequence of connected cubic Bézier curves to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curves.
    /// </param>
    public void AddBeziers(params Point[] points)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new BeziersMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)) });
    }

    /// <summary>
    /// Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.
    /// </param>
    public void AddClosedCurve(PointF[] points)
    {
        AddPathPoints(points);
        list.Add(new ClosedCurveMode() { points = points });
    }

    /// <summary>
    /// Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="tension">
    /// A value between from 0 through 1 that specifies the amount that the curve bends between points, with 0 being the smallest curve (sharpest corner) and 1 being the smoothest curve.
    /// </param>
    public void AddClosedCurve(PointF[] points, float tension)
    {
        AddPathPoints(points);
        list.Add(new ClosedCurveMode() { points = points, tension = tension });
    }

    /// <summary>
    /// Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.
    /// </param>
    public void AddClosedCurve(Point[] points)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new ClosedCurveMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)) });
    }

    /// <summary>
    /// Adds a closed curve to this path. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="tension">
    /// A value between from 0 through 1 that specifies the amount that the curve bends between points, with 0 being the smallest curve (sharpest corner) and 1 being the smoothest curve.
    /// </param>
    public void AddClosedCurve(Point[] points, float tension)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new ClosedCurveMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)), tension = tension });
    }

    /// <summary>
    /// Adds a spline curve to the current figure. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.
    /// </param>
    public void AddCurve(PointF[] points)
    {
        AddPathPoints(points);
        list.Add(new CurveMode() { points = points });
    }

    /// <summary>
    /// Adds a spline curve to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="offset">
    /// The index of the element in the
    /// <paramref name="points" /> array that is used as the first point in the curve.
    /// </param>
    /// <param name="numberOfSegments">
    /// The number of segments used to draw the curve. A segment can be thought of as a line connecting two points.
    /// </param>
    /// <param name="tension">
    /// A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.
    /// </param>
    public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
    {
        AddPathPoints(points);
        list.Add(new CurveMode() { points = points, offset = offset, numberOfSegments = numberOfSegments, tension = tension });
    }

    /// <summary>
    /// Adds a spline curve to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="tension">
    /// A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.
    /// </param>
    public void AddCurve(PointF[] points, float tension)
    {
        AddPathPoints(points);
        list.Add(new CurveMode() { points = points, tension = tension });
    }

    /// <summary>
    /// Adds a spline curve to the current figure. A cardinal spline curve is used because the curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.
    /// </param>
    public void AddCurve(Point[] points)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new CurveMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)) });
    }

    /// <summary>
    /// Adds a spline curve to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="offset">
    /// The index of the element in the
    /// <paramref name="points" /> array that is used as the first point in the curve.
    /// </param>
    /// <param name="numberOfSegments">
    /// A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.
    /// </param>
    /// <param name="tension">
    /// A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.
    /// </param>
    public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new CurveMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)), offset = offset, numberOfSegments = numberOfSegments, tension = tension });
    }

    /// <summary>
    /// Adds a spline curve to the current figure.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the curve.
    /// </param>
    /// <param name="tension">
    /// A value that specifies the amount that the curve bends between control points. Values greater than 1 produce unpredictable results.
    /// </param>
    public void AddCurve(Point[] points, float tension)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new CurveMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)), tension = tension });
    }

    /// <summary>
    /// Adds an ellipse to the current path.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> that represents the bounding rectangle that defines the ellipse.
    /// </param>
    public void AddEllipse(Rectangle rect)
    {
        AddPathPoints(new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Bottom));
        list.Add(new EllipseMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) });
    }

    /// <summary>
    /// Adds an ellipse to the current path.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the bounding rectangle that defines the ellipse.
    /// </param>
    public void AddEllipse(RectangleF rect)
    {
        AddPathPoints(new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Bottom));
        list.Add(new EllipseMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) });
    }

    /// <summary>
    /// Adds an ellipse to the current path.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="width">
    /// The width of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="height">
    /// The height of the bounding rectangle that defines the ellipse.
    /// </param>
    public void AddEllipse(int x, int y, int width, int height)
    {
        AddPathPoints(new PointF(x, y), new PointF(width, height));
        list.Add(new EllipseMode() { rect = new RectangleF(x, y, width, height) });
    }

    /// <summary>
    /// Adds an ellipse to the current path.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper left corner of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="width">
    /// The width of the bounding rectangle that defines the ellipse.
    /// </param>
    /// <param name="height">
    /// The height of the bounding rectangle that defines the ellipse.
    /// </param>
    public void AddEllipse(float x, float y, float width, float height)
    {
        AddPathPoints(new PointF(x, y), new PointF(width, height));
        list.Add(new EllipseMode() { rect = new RectangleF(x, y, width, height) });
    }

    /// <summary>
    /// Appends a line segment to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="pt1">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the starting point of the line.
    /// </param>
    /// <param name="pt2">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the endpoint of the line.
    /// </param>
    public void AddLine(Point pt1, Point pt2)
    {
        AddPathPoints(new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y));
        list.Add(new LineMode() { pt1 = new PointF(pt1.X, pt1.Y), pt2 = new PointF(pt2.X, pt2.Y) });
    }

    /// <summary>
    /// Appends a line segment to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="pt1">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the starting point of the line.
    /// </param>
    /// <param name="pt2">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the endpoint of the line.
    /// </param>
    public void AddLine(PointF pt1, PointF pt2)
    {
        AddPathPoints(new PointF(pt1.X, pt1.Y), new PointF(pt2.X, pt2.Y));
        list.Add(new LineMode() { pt1 = new PointF(pt1.X, pt1.Y), pt2 = new PointF(pt2.X, pt2.Y) });
    }

    /// <summary>
    /// Appends a line segment to the current figure.
    /// </summary>
    /// <param name="x1">
    /// The x-coordinate of the starting point of the line.
    /// </param>
    /// <param name="y1">
    /// The y-coordinate of the starting point of the line.
    /// </param>
    /// <param name="x2">
    /// The x-coordinate of the endpoint of the line.
    /// </param>
    /// <param name="y2">
    /// The y-coordinate of the endpoint of the line.
    /// </param>
    public void AddLine(int x1, int y1, int x2, int y2)
    {
        AddPathPoints(new PointF(x1, y1), new PointF(x2, y2));
        list.Add(new LineMode() { pt1 = new PointF(x1, y1), pt2 = new PointF(x2, y2) });
    }

    /// <summary>
    /// Appends a line segment to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="x1">
    /// The x-coordinate of the starting point of the line.
    /// </param>
    /// <param name="y1">
    /// The y-coordinate of the starting point of the line.
    /// </param>
    /// <param name="x2">
    /// The x-coordinate of the endpoint of the line.
    /// </param>
    /// <param name="y2">
    /// The y-coordinate of the endpoint of the line.
    /// </param>
    public void AddLine(float x1, float y1, float x2, float y2)
    {
        AddPathPoints(new PointF(x1, y1), new PointF(x2, y2));
        list.Add(new LineMode() { pt1 = new PointF(x1, y1), pt2 = new PointF(x2, y2) });
    }

    /// <summary>
    /// Appends a series of connected line segments to the end of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that represents the points that define the line segments to add.
    /// </param>
    public void AddLines(PointF[] points)
    {
        AddPathPoints(points);
        list.Add(new LinesMode() { points = points });
    }

    /// <summary>
    /// Appends a series of connected line segments to the end of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that represents the points that define the line segments to add.
    /// </param>
    public void AddLines(Point[] points)
    {
        AddPathPoints(Array.ConvertAll(points, p => new PointF(p.X, p.Y)));
        list.Add(new LinesMode() { points = Array.ConvertAll(points, p => new PointF(p.X, p.Y)) });
    }

    /// <summary>
    /// Appends the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to this path.
    /// </summary>
    /// <param name="addingPath">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to add.
    /// </param>
    /// <param name="connect">
    /// A Boolean value that specifies whether the first figure in the added path is part of the last figure in this path. A value of
    /// <see langword="true" /> specifies that (if possible) the first figure in the added path is part of the last figure in this path. A value of
    /// <see langword="false" /> specifies that the first figure in the added path is separate from the last figure in this path.
    /// </param>
    public void AddPath(GraphicsPath addingPath, bool connect)
    {
        list.Add(new PathMode() { path = addingPath, connect = connect });
    }

    /// <summary>
    /// Adds the outline of a pie shape to this path.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> that represents the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle for the pie section, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from
    /// <paramref name="startAngle" />.
    /// </param>
    public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new PieMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Adds the outline of a pie shape to this path.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="width">
    /// The width of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="height">
    /// The height of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle for the pie section, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from
    /// <paramref name="startAngle" />.
    /// </param>
    public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(x, y), new PointF(x + width, y + height));
        list.Add(new PieMode() { rect = new RectangleF(x, y, width, height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Adds the outline of a pie shape to this path.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="width">
    /// The width of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="height">
    /// The height of the bounding rectangle that defines the ellipse from which the pie is drawn.
    /// </param>
    /// <param name="startAngle">
    /// The starting angle for the pie section, measured in degrees clockwise from the x-axis.
    /// </param>
    /// <param name="sweepAngle">
    /// The angle between
    /// <paramref name="startAngle" /> and the end of the pie section, measured in degrees clockwise from
    /// <paramref name="startAngle" />.
    /// </param>
    public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        AddPathPoints(new PointF(x, y), new PointF(x + width, y + height));
        list.Add(new PieMode() { rect = new RectangleF(x, y, width, height), startAngle = startAngle, sweepAngle = sweepAngle });
    }

    /// <summary>
    /// Adds a polygon to this path.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that defines the polygon to add.
    /// </param>
    public void AddPolygon(PointF[] points)
    {
        AddPathPoints(points);
        list.Add(new PolygonMode() { points = Array.ConvertAll(points, r => new PointF(r.X, r.Y)) });
    }

    /// <summary>
    /// Adds a polygon to this path.
    /// </summary>
    /// <param name="points">
    /// An array of
    /// <see cref="T:System.Drawing.Point" /> structures that defines the polygon to add.
    /// </param>
    public void AddPolygon(Point[] points)
    {
        AddPathPoints(Array.ConvertAll(points, r => new PointF(r.X, r.Y)));
        list.Add(new PolygonMode() { points = Array.ConvertAll(points, r => new PointF(r.X, r.Y)) });
    }

    /// <summary>
    /// Adds a rectangle to this path.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle to add.
    /// </param>
    public void AddRectangle(Rectangle rect)
    {
        AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new RectangleMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) });
    }

    /// <summary>
    /// Adds a rectangle to this path.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle to add.
    /// </param>
    public void AddRectangle(RectangleF rect)
    {
        AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new RectangleMode() { rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) });
    }

    /// <summary>
    /// Adds a series of rectangles to this path.
    /// </summary>
    /// <param name="rects">
    /// An array of
    /// <see cref="T:System.Drawing.RectangleF" /> structures that represents the rectangles to add.
    /// </param>
    public void AddRectangles(RectangleF[] rects)
    {
        foreach (var rect in rects)
            AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));

        list.Add(new RectanglesMode() { rects = rects });
    }

    /// <summary>
    /// Adds a series of rectangles to this path.
    /// </summary>
    /// <param name="rects">
    /// An array of
    /// <see cref="T:System.Drawing.Rectangle" /> structures that represents the rectangles to add.
    /// </param>
    public void AddRectangles(Rectangle[] rects)
    {
        foreach (RectangleF rect in rects)
            AddPathPoints(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom));
        list.Add(new RectanglesMode() { rects = Array.ConvertAll(rects, r => new RectangleF(r.X, r.Y, r.Width, r.Height)) });
    }

    /// <summary>
    /// Adds a text string to this path.
    /// </summary>
    /// <param name="s">
    /// The
    /// <see cref="T:System.String" /> to add.
    /// </param>
    /// <param name="family">
    /// A
    /// <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.
    /// </param>
    /// <param name="style">
    /// A
    /// <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).
    /// </param>
    /// <param name="emSize">
    /// The height of the em square box that bounds the character.
    /// </param>
    /// <param name="origin">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that represents the point where the text starts.
    /// </param>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.
    /// </param>
    public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
    {
        AddPathPoints(new PointF(0, 0), new PointF(emSize * s.Length, emSize));
        list.Add(new StringMode() { text = s, family = family, style = style, emSize = emSize, layoutRect = new RectangleF(origin.X, origin.Y, 0, 0), format = format });
    }

    /// <summary>
    /// Adds a text string to this path.
    /// </summary>
    /// <param name="s">
    /// The
    /// <see cref="T:System.String" /> to add.
    /// </param>
    /// <param name="family">
    /// A
    /// <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.
    /// </param>
    /// <param name="style">
    /// A
    /// <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).
    /// </param>
    /// <param name="emSize">
    /// The height of the em square box that bounds the character.
    /// </param>
    /// <param name="origin">
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the point where the text starts.
    /// </param>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.
    /// </param>
    public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
    {
        AddPathPoints(new PointF(0, 0), new PointF(emSize * s.Length, emSize));
        list.Add(new StringMode() { text = s, family = family, style = style, emSize = emSize, layoutRect = new RectangleF(origin.X, origin.Y, 0, 0), format = format });
    }

    /// <summary>
    /// Adds a text string to this path.
    /// </summary>
    /// <param name="s">
    /// The
    /// <see cref="T:System.String" /> to add.
    /// </param>
    /// <param name="family">
    /// A
    /// <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.
    /// </param>
    /// <param name="style">
    /// A
    /// <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).
    /// </param>
    /// <param name="emSize">
    /// The height of the em square box that bounds the character.
    /// </param>
    /// <param name="layoutRect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle that bounds the text.
    /// </param>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.
    /// </param>
    public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
    {
        AddPathPoints(new PointF(layoutRect.Left, layoutRect.Top), new PointF(layoutRect.Right, layoutRect.Bottom));
        list.Add(new StringMode() { text = s, family = family, style = style, emSize = emSize, layoutRect = new RectangleF(layoutRect.X, layoutRect.Y, layoutRect.Width, layoutRect.Height), format = format });
    }

    /// <summary>
    /// Adds a text string to this path.
    /// </summary>
    /// <param name="s">
    /// The
    /// <see cref="T:System.String" /> to add.
    /// </param>
    /// <param name="family">
    /// A
    /// <see cref="T:System.Drawing.FontFamily" /> that represents the name of the font with which the test is drawn.
    /// </param>
    /// <param name="style">
    /// A
    /// <see cref="T:System.Drawing.FontStyle" /> enumeration that represents style information about the text (bold, italic, and so on). This must be cast as an integer (see the example code later in this section).
    /// </param>
    /// <param name="emSize">
    /// The height of the em square box that bounds the character.
    /// </param>
    /// <param name="layoutRect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that bounds the text.
    /// </param>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Drawing.StringFormat" /> that specifies text formatting information, such as line spacing and alignment.
    /// </param>
    public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
    {
        AddPathPoints(new PointF(layoutRect.Left, layoutRect.Top), new PointF(layoutRect.Right, layoutRect.Bottom));
        list.Add(new StringMode() { text = s, family = family, style = style, emSize = emSize, layoutRect = new RectangleF(layoutRect.X, layoutRect.Y, layoutRect.Width, layoutRect.Height), format = format });
    }

    /// <summary>
    /// Clears all markers from this path.
    /// </summary>
    public void ClearMarkers()
    {
        // Markers are not tracked in this lightweight implementation.
        // Provide a no-op to match System.Drawing.GraphicsPath behavior.
    }

    /// <summary>
    /// Creates an exact copy of this path.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> this method creates, cast as an object.
    /// </returns>
    public object? Clone()
    {
        // Deep-ish clone: copy primitive data and clone nested GraphicsPath entries.
        var gp = new GraphicsPath
        {
            FillMode = this.FillMode,
            PathPoints = this.PathPoints != null ? (PointF[])this.PathPoints.Clone() : null,
            PathTypes = this.PathTypes != null ? (byte[])this.PathTypes.Clone() : null,
            PointCount = this.PointCount,
            PathData = this.PathData,
            SizePoints = new List<PointF>(this.SizePoints) // PathData is used as informational here
        };

        // clone list contents conservatively
        foreach (var o in this.list)
        {
            if (o == null) { gp.list.Add(null); continue; }
            switch (o)
            {
                case ArcMode am:
                    gp.list.Add(new ArcMode() { rect = am.rect, startAngle = am.startAngle, sweepAngle = am.sweepAngle });
                    break;
                case EllipseMode em:
                    gp.list.Add(new EllipseMode() { rect = em.rect });
                    break;
                case PieMode pm:
                    gp.list.Add(new PieMode() { rect = pm.rect, startAngle = pm.startAngle, sweepAngle = pm.sweepAngle });
                    break;
                case RectangleMode rm:
                    gp.list.Add(new RectangleMode() { rect = rm.rect });
                    break;
                case RectanglesMode rsm:
                    gp.list.Add(new RectanglesMode() { rects = rsm.rects != null ? (RectangleF[])rsm.rects.Clone() : null });
                    break;
                case PolygonMode pol:
                    gp.list.Add(new PolygonMode() { points = pol.points != null ? (PointF[])pol.points.Clone() : null });
                    break;
                case LinesMode lsm:
                    gp.list.Add(new LinesMode() { points = lsm.points != null ? (PointF[])lsm.points.Clone() : null });
                    break;
                case LineMode lm:
                    gp.list.Add(new LineMode() { pt1 = lm.pt1, pt2 = lm.pt2 });
                    break;
                case BezierMode bz:
                    gp.list.Add(new BezierMode() { pt1 = bz.pt1, pt2 = bz.pt2, pt3 = bz.pt3, pt4 = bz.pt4 });
                    break;
                case BeziersMode bzs:
                    gp.list.Add(new BeziersMode() { points = bzs.points != null ? (PointF[])bzs.points.Clone() : null });
                    break;
                case ClosedCurveMode ccm:
                    gp.list.Add(new ClosedCurveMode() { points = ccm.points != null ? (PointF[])ccm.points.Clone() : null, tension = ccm.tension, fillmode = ccm.fillmode });
                    break;
                case CurveMode crm:
                    gp.list.Add(new CurveMode() { points = crm.points != null ? (PointF[])crm.points.Clone() : null, offset = crm.offset, numberOfSegments = crm.numberOfSegments, tension = crm.tension });
                    break;
                case PathMode pmode:
                    gp.list.Add(new PathMode() { connect = pmode.connect, path = pmode.path != null ? pmode.path.Clone() as GraphicsPath : null });
                    break;
                case StringMode sm:
                    gp.list.Add(new StringMode() { text = sm.text, family = sm.family, style = sm.style, emSize = sm.emSize, layoutRect = sm.layoutRect, format = sm.format });
                    break;
                default:
                    // unknown types: attempt shallow copy
                    gp.list.Add(o);
                    break;
            }
        }

        return gp;
    }

    /// <summary>
    /// Closes all open figures in this path and starts a new figure. It closes each open figure by connecting a line from its endpoint to its starting point.
    /// </summary>
    public void CloseAllFigures()
    {
        IsCloseAllFigures = true;
    }

    /// <summary>
    /// Closes the current figure and starts a new figure. If the current figure contains a sequence of connected lines and curves, the method closes the loop by connecting a line from the endpoint to the starting point.
    /// </summary>
    public void CloseFigure()
    {
        list.Add(new FigureMode() { start = false, close = true });
    }

    /// <summary>
    /// Releases all resources used by this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public void Dispose()
    {
        // release managed resources
        PathPoints = null;
        PathTypes = null;
        PathData = null;
        SizePoints?.Clear();
        list?.Clear();
        Context = null;
        destPoints = null;
        matrix = null;
        pen = null;
    }

    /// <summary>
    /// Converts each curve in this path into a sequence of connected line segments.
    /// </summary>
    public void Flatten()
    {
        // No-op in this lightweight implementation. Full flattening requires curve tessellation.
    }

    /// <summary>
    /// Applies the specified transform and then converts each curve in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> into a sequence of connected line segments.
    /// </summary>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> before flattening.
    /// </param>
    public void Flatten(Matrix matrix)
    {
        if (matrix == null) throw new ArgumentNullException(nameof(matrix));
        // Apply transform and leave curve data intact; real flatten would approximate curves.
        if (PathPoints != null && PathPoints.Length > 0)
            matrix.TransformPoints(PathPoints);
        for (int i = 0; i < SizePoints.Count; i++)
        {
            var p = SizePoints[i];
            var arr = new[] { p };
            matrix.TransformPoints(arr);
            SizePoints[i] = arr[0];
        }
        // Note: list entries containing nested GraphicsPath are not transformed here.
    }

    /// <summary>
    /// Converts each curve in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> into a sequence of connected line segments.
    /// </summary>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> before flattening.
    /// </param>
    /// <param name="flatness">
    /// Specifies the maximum permitted error between the curve and its flattened approximation. A value of 0.25 is the default. Reducing the flatness value will increase the number of line segments in the approximation.
    /// </param>
    public void Flatten(Matrix matrix, float flatness)
    {
        // Defer to Flatten(matrix) for this lightweight implementation.
        Flatten(matrix);
    }

    /// <summary>
    /// Returns a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </returns>
    public RectangleF GetBounds()
    {
        // compute bounds by iterating stored primitives and points
        RectangleF bounds = RectangleF.Empty;

        void Union(RectangleF r)
        {
            if (r.IsEmpty) return;
            if (bounds.IsEmpty)
                bounds = r;
            else
            {
                float left = Math.Min(bounds.Left, r.Left);
                float top = Math.Min(bounds.Top, r.Top);
                float right = Math.Max(bounds.Right, r.Right);
                float bottom = Math.Max(bounds.Bottom, r.Bottom);
                bounds = new RectangleF(left, top, right - left, bottom - top);
            }
        }

        // include explicit PathPoints if present
        if (this.PathPoints != null && this.PathPoints.Length > 0)
        {
            float l = this.PathPoints[0].X, r = this.PathPoints[0].X, t = this.PathPoints[0].Y, b = this.PathPoints[0].Y;
            foreach (var p in this.PathPoints)
            {
                if (p.X < l) l = p.X;
                if (p.X > r) r = p.X;
                if (p.Y < t) t = p.Y;
                if (p.Y > b) b = p.Y;
            }
            Union(new RectangleF(l, t, r - l, b - t));
        }

        // include SizePoints (internal point list)
        if (SizePoints != null && SizePoints.Count > 0)
        {
            float l = SizePoints[0].X, r = SizePoints[0].X, t = SizePoints[0].Y, b = SizePoints[0].Y;
            foreach (var p in SizePoints)
            {
                if (p.X < l) l = p.X;
                if (p.X > r) r = p.X;
                if (p.Y < t) t = p.Y;
                if (p.Y > b) b = p.Y;
            }
            Union(new RectangleF(l, t, r - l, b - t));
        }

        // iterate stored list of mode objects and union their bounds
        foreach (var o in list)
        {
            if (o == null) continue;
            switch (o)
            {
                case ArcMode am:
                    Union(am.rect);
                    break;
                case EllipseMode em:
                    Union(em.rect);
                    break;
                case PieMode pm:
                    Union(pm.rect);
                    break;
                case RectangleMode rm:
                    Union(rm.rect);
                    break;
                case RectanglesMode rsm:
                    if (rsm.rects != null)
                    {
                        foreach (var rr in rsm.rects)
                            Union(rr);
                    }
                    break;
                case PolygonMode pol:
                    if (pol.points != null && pol.points.Length > 0)
                    {
                        float l = pol.points[0].X, r = pol.points[0].X, t = pol.points[0].Y, b = pol.points[0].Y;
                        foreach (var p in pol.points)
                        {
                            if (p.X < l) l = p.X;
                            if (p.X > r) r = p.X;
                            if (p.Y < t) t = p.Y;
                            if (p.Y > b) b = p.Y;
                        }
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case LinesMode lsm:
                    if (lsm.points != null && lsm.points.Length > 0)
                    {
                        float l = lsm.points[0].X, r = lsm.points[0].X, t = lsm.points[0].Y, b = lsm.points[0].Y;
                        foreach (var p in lsm.points)
                        {
                            if (p.X < l) l = p.X;
                            if (p.X > r) r = p.X;
                            if (p.Y < t) t = p.Y;
                            if (p.Y > b) b = p.Y;
                        }
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case LineMode lm:
                    Union(new RectangleF(Math.Min(lm.pt1.X, lm.pt2.X), Math.Min(lm.pt1.Y, lm.pt2.Y),
                                         Math.Abs(lm.pt2.X - lm.pt1.X), Math.Abs(lm.pt2.Y - lm.pt1.Y)));
                    break;
                case BezierMode bz:
                    {
                        float l = Math.Min(Math.Min(bz.pt1.X, bz.pt2.X), Math.Min(bz.pt3.X, bz.pt4.X));
                        float r = Math.Max(Math.Max(bz.pt1.X, bz.pt2.X), Math.Max(bz.pt3.X, bz.pt4.X));
                        float t = Math.Min(Math.Min(bz.pt1.Y, bz.pt2.Y), Math.Min(bz.pt3.Y, bz.pt4.Y));
                        float b = Math.Max(Math.Max(bz.pt1.Y, bz.pt2.Y), Math.Max(bz.pt3.Y, bz.pt4.Y));
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case BeziersMode bzs:
                    if (bzs.points != null && bzs.points.Length > 0)
                    {
                        float l = bzs.points[0].X, r = bzs.points[0].X, t = bzs.points[0].Y, b = bzs.points[0].Y;
                        foreach (var p in bzs.points)
                        {
                            if (p.X < l) l = p.X;
                            if (p.X > r) r = p.X;
                            if (p.Y < t) t = p.Y;
                            if (p.Y > b) b = p.Y;
                        }
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case ClosedCurveMode ccm:
                    if (ccm.points != null && ccm.points.Length > 0)
                    {
                        float l = ccm.points[0].X, r = ccm.points[0].X, t = ccm.points[0].Y, b = ccm.points[0].Y;
                        foreach (var p in ccm.points)
                        {
                            if (p.X < l) l = p.X;
                            if (p.X > r) r = p.X;
                            if (p.Y < t) t = p.Y;
                            if (p.Y > b) b = p.Y;
                        }
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case CurveMode crm:
                    if (crm.points != null && crm.points.Length > 0)
                    {
                        float l = crm.points[0].X, r = crm.points[0].X, t = crm.points[0].Y, b = crm.points[0].Y;
                        foreach (var p in crm.points)
                        {
                            if (p.X < l) l = p.X;
                            if (p.X > r) r = p.X;
                            if (p.Y < t) t = p.Y;
                            if (p.Y > b) b = p.Y;
                        }
                        Union(new RectangleF(l, t, r - l, b - t));
                    }
                    break;
                case PathMode pmode:
                    if (pmode.path != null)
                        Union(pmode.path.GetBounds());
                    break;
                case StringMode sm:
                    Union(sm.layoutRect);
                    break;
                default:
                    // unknown item - ignore
                    break;
            }
        }

        return bounds;
    }

    /// <summary>
    /// Returns a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when this path is transformed by the specified
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <param name="matrix">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transformation to be applied to this path before the bounding rectangle is calculated. This path is not permanently transformed; the transformation is used only during the process of calculating the bounding rectangle.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </returns>
    public RectangleF GetBounds(Matrix matrix)
    {
        if (matrix == null) throw new ArgumentNullException(nameof(matrix));
        var b = GetBounds();
        if (b.IsEmpty) return RectangleF.Empty;

        var pts = new[] { new PointF(b.Left, b.Top), new PointF(b.Right, b.Top), new PointF(b.Right, b.Bottom), new PointF(b.Left, b.Bottom) };
        matrix.TransformPoints(pts);
        float left = pts[0].X, right = pts[0].X, top = pts[0].Y, bottom = pts[0].Y;
        foreach (var p in pts)
        {
            if (p.X < left) left = p.X;
            if (p.X > right) right = p.X;
            if (p.Y < top) top = p.Y;
            if (p.Y > bottom) bottom = p.Y;
        }
        return new RectangleF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Returns a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when the current path is transformed by the specified
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> and drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />.
    /// </summary>
    /// <param name="matrix">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transformation to be applied to this path before the bounding rectangle is calculated. This path is not permanently transformed; the transformation is used only during the process of calculating the bounding rectangle.
    /// </param>
    /// <param name="pen">
    /// The
    /// <see cref="T:System.Drawing.Pen" /> with which to draw the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents a rectangle that bounds this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </returns>
    public RectangleF GetBounds(Matrix matrix, Pen pen)
    {
        var b = GetBounds(matrix);
        if (b.IsEmpty) return b;
        if (pen == null) return b;
        // Inflate by pen width/2 as conservative approximation
        float inflate = pen.Width / 2f;
        return RectangleF.Inflate(b, inflate, inflate);
    }

    /// <summary>
    /// Gets the last point in the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.PointF" /> that represents the last point in this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </returns>
    public PointF GetLastPoint()
    {
        if (PathPoints != null && PathPoints.Length > 0)
            return PathPoints[PathPoints.Length - 1];
        if (SizePoints != null && SizePoints.Count > 0)
            return SizePoints[SizePoints.Count - 1];
        return new PointF(0, 0);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />.
    /// </summary>
    /// <param name="point">
    /// A
    /// <see cref="T:System.Drawing.Point" /> that specifies the location to test.
    /// </param>
    /// <param name="pen">
    /// The
    /// <see cref="T:System.Drawing.Pen" /> to test.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> if the specified point is contained within the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsOutlineVisible(Point point, Pen pen)
    {
        return IsOutlineVisible(new PointF(point.X, point.Y), pen);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" /> and using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
    {
        return IsOutlineVisible(new PointF(pt.X, pt.Y), pen, graphics);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />.
    /// /// </summary>
    public bool IsOutlineVisible(PointF point, Pen pen)
    {
        // conservative: check whether point falls inside bounds inflated by pen width
        if (pen == null) throw new ArgumentNullException(nameof(pen));
        var b = GetBounds();
        if (b.IsEmpty) return false;
        var inflated = RectangleF.Inflate(b, pen.Width / 2f, pen.Width / 2f);
        return inflated.Contains(point);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" /> and using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
    {
        if (graphics == null) throw new ArgumentNullException(nameof(graphics));
        return IsOutlineVisible(pt, pen);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />.
    /// </summary>
    public bool IsOutlineVisible(int x, int y, Pen pen)
    {
        return IsOutlineVisible(new PointF(x, y), pen);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" /> and using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
    {
        return IsOutlineVisible(new PointF(x, y), pen, graphics);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" />.
    /// </summary>
    public bool IsOutlineVisible(float x, float y, Pen pen)
    {
        return IsOutlineVisible(new PointF(x, y), pen);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within (under) the outline of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> when drawn with the specified
    /// <see cref="T:System.Drawing.Pen" /> and using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
    {
        return IsOutlineVisible(new PointF(x, y), pen, graphics);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(Point point)
    {
        return IsVisible(new PointF(point.X, point.Y));
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(Point pt, Graphics graphics)
    {
        if (graphics == null) throw new ArgumentNullException(nameof(graphics));
        return IsVisible(new PointF(pt.X, pt.Y), graphics);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(PointF point)
    {
        // uses a temporary Graphics to evaluate visibility similarly to other overloads
        using (var bmp = new Bitmap(1, 1))
        using (var g = Graphics.FromImage(bmp))
        {
            return IsVisible(point, g);
        }
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(PointF pt, Graphics graphics)
    {
        if (graphics == null) throw new ArgumentNullException(nameof(graphics));
        // conservative approximation: if point is inside the computed bounds, consider it visible.
        var b = GetBounds();
        if (b.IsEmpty) return false;
        if (b.Contains(pt)) return true;

        // fallback: check explicit PathPoints / SizePoints
        if (this.PathPoints != null)
        {
            foreach (var p in this.PathPoints)
            {
                if (Math.Abs(p.X - pt.X) < 1e-6f && Math.Abs(p.Y - pt.Y) < 1e-6f)
                    return true;
            }
        }

        if (this.SizePoints != null)
        {
            foreach (var p in this.SizePoints)
            {
                if (Math.Abs(p.X - pt.X) < 1e-6f && Math.Abs(p.Y - pt.Y) < 1e-6f)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(int x, int y)
    {
        return IsVisible(new PointF(x, y));
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />, using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsVisible(int x, int y, Graphics graphics)
    {
        return IsVisible(new PointF(x, y), graphics);
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public bool IsVisible(float x, float y)
    {
        return IsVisible(new PointF(x, y));
    }

    /// <summary>
    /// Indicates whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> in the visible clip region of the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    public bool IsVisible(float x, float y, Graphics graphics)
    {
        return IsVisible(new PointF(x, y), graphics);
    }

    /// <summary>
    /// Empties the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> and
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathTypes" /> arrays and sets the
    /// <see cref="T:System.Drawing.Drawing2D.FillMode" /> to
    /// <see cref="T:System.Drawing.Drawing2D.FillMode.Alternate" />.
    /// </summary>
    public void Reset()
    {
        this.PathPoints = null;
        this.PathTypes = null;
        this.FillMode = FillMode.Alternate;
    }

    /// <summary>
    /// Reverses the order of points in the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.PathPoints" /> array of this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public void Reverse()
    {
        if (PathPoints != null)
            Array.Reverse(PathPoints);
        SizePoints.Reverse();
    }

    /// <summary>
    /// Sets a marker on this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    public void SetMarkers()
    {
        // markers not tracked in this implementation; no-op
    }

    /// <summary>
    /// Starts a new figure without closing the current figure. All subsequent points added to the path are added to this new figure.
    /// </summary>
    public void StartFigure()
    {
        list.Add(new FigureMode() { start = true, close = false });
    }

    /// <summary>
    /// Applies a transform matrix to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that represents the transformation to apply.
    /// </param>
    public void Transform(Matrix matrix)
    {
        this.matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
        if (PathPoints != null)
            matrix.TransformPoints(PathPoints);
        for (int i = 0; i < SizePoints.Count; i++)
        {
            var p = SizePoints[i];
            var arr = new[] { p };
            matrix.TransformPoints(arr);
            SizePoints[i] = arr[0];
        }
    }

    /// <summary>
    /// Applies a warp transform, defined by a rectangle and a parallelogram, to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="destPoints">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by
    /// <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.
    /// </param>
    /// <param name="srcRect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by
    /// <paramref name="destPoints" />.
    /// </param>
    public void Warp(PointF[] destPoints, RectangleF srcRect)
    {
        this.destPoints = destPoints;
        this.srcRect = srcRect;
    }

    /// <summary>
    /// Applies a warp transform, defined by a rectangle and a parallelogram, to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="destPoints">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by
    /// <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.
    /// </param>
    /// <param name="srcRect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by
    /// <paramref name="destPoints" />.
    /// </param>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.
    /// </param>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
    {
        this.destPoints = destPoints;
        this.srcRect = srcRect;
        this.matrix = matrix;
    }

    /// <summary>
    /// Applies a warp transform, defined by a rectangle and a parallelogram, to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="destPoints">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by
    /// <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.
    /// </param>
    /// <param name="srcRect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by
    /// <paramref name="destPoints" />.
    /// </param>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.
    /// </param>
    /// <param name="warpMode">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.WarpMode" /> enumeration that specifies whether this warp operation uses perspective or bilinear mode.
    /// </param>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
    {
        this.destPoints = destPoints;
        this.srcRect = srcRect;
        this.matrix = matrix;
        this.warpMode = warpMode;
    }

    /// <summary>
    /// Applies a warp transform, defined by a rectangle and a parallelogram, to this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="destPoints">
    /// An array of
    /// <see cref="T:System.Drawing.PointF" /> structures that define a parallelogram to which the rectangle defined by
    /// <paramref name="srcRect" /> is transformed. The array can contain either three or four elements. If the array contains three elements, the lower-right corner of the parallelogram is implied by the first three points.
    /// </param>
    /// <param name="srcRect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> that represents the rectangle that is transformed to the parallelogram defined by
    /// <paramref name="destPoints" />.
    /// </param>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a geometric transform to apply to the path.
    /// </param>
    /// <param name="warpMode">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.WarpMode" /> enumeration that specifies whether this warp operation uses perspective or bilinear mode.
    /// </param>
    /// <param name="flatness">
    /// A value from 0 through 1 that specifies how flat the resulting path is. For more information, see the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath.Flatten" /> methods.
    /// </param>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
    {
        this.destPoints = destPoints;
        this.srcRect = srcRect;
        this.matrix = matrix;
        this.warpMode = warpMode;
        this.flatness = flatness;
    }

    /// <summary>
    /// Adds an additional outline to the path.
    /// </summary>
    /// <param name="pen">
    /// A
    /// <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.
    /// </param>
    public void Widen(Pen pen)
    {
        this.pen = pen;
    }

    /// <summary>
    /// Adds an additional outline to the
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="pen">
    /// A
    /// <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.
    /// </param>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transform to apply to the path before widening.
    /// </param>
    public void Widen(Pen pen, Matrix matrix)
    {
        this.pen = pen;
        this.matrix = matrix;
    }

    /// <summary>
    /// Replaces this
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> with curves that enclose the area that is filled when this path is drawn by the specified pen.
    /// </summary>
    /// <param name="pen">
    /// A
    /// <see cref="T:System.Drawing.Pen" /> that specifies the width between the original outline of the path and the new outline this method creates.
    /// </param>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that specifies a transform to apply to the path before widening.
    /// </param>
    /// <param name="flatness">
    /// A value that specifies the flatness for curves.
    /// </param>
    public void Widen(Pen pen, Matrix matrix, float flatness)
    {
        this.pen = pen;
        this.matrix = matrix;
        this.flatness = flatness;
    }

    private void AddPathPoints(params PointF[] points)
    {
        foreach (var p in points)
            SizePoints.Add(p);
    }

    /// <summary>
    /// ArcMode is used to store the parameters of an arc segment in a graphics path. It contains a rectangle that defines the bounding box of the arc, a starting angle that specifies where the arc begins, and a sweep angle that determines how far the arc extends from the starting angle.
    /// </summary>
    public class ArcMode
    {
        /// <summary>
        /// Gets or sets the rect.
        /// </summary>
        /// <value>
        /// The rect.
        /// </value>
        public RectangleF rect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>
        /// The start angle.
        /// </value>
        public float startAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sweep angle.
        /// </summary>
        /// <value>
        /// The sweep angle.
        /// </value>
        public float sweepAngle
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a cubic Bézier segment parameters.
    /// </summary>
    public class BezierMode
    {
        /// <summary>
        /// Gets or sets the first control/starting point.
        /// </summary>
        public PointF pt1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second control point.
        /// </summary>
        public PointF pt2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third control point.
        /// </summary>
        public PointF pt3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the endpoint of the bezier curve.
        /// </summary>
        public PointF pt4
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a sequence of connected cubic Bézier curves.
    /// </summary>
    public class BeziersMode
    {
        /// <summary>
        /// Gets or sets the points that define the Bézier curves.
        /// </summary>
        public PointF[]? points
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a closed curve (cardinal spline) parameters.
    /// </summary>
    public class ClosedCurveMode
    {
        /// <summary>
        /// Gets or sets the fill mode used for closed curve filling.
        /// </summary>
        public FillMode fillmode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the points that define the closed curve.
        /// </summary>
        public PointF[]? points
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tension of the closed curve.
        /// </summary>
        public float tension
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a spline curve parameters.
    /// </summary>
    public class CurveMode
    {
        /// <summary>
        /// Gets or sets the number of segments to use when drawing the curve.
        /// </summary>
        public int numberOfSegments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset index in the points array.
        /// </summary>
        public int offset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the points that define the spline.
        /// </summary>
        public PointF[]? points
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tension for the spline.
        /// </summary>
        public float tension
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores an ellipse parameters.
    /// </summary>
    public class EllipseMode
    {
        /// <summary>
        /// Gets or sets the bounding rectangle for the ellipse.
        /// </summary>
        public RectangleF rect
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a figure control marker.
    /// </summary>
    public class FigureMode
    {
        /// <summary>
        /// Gets or sets a value indicating whether this figure is closed.
        /// </summary>
        public bool close
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this figure is a start marker.
        /// </summary>
        public bool start
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a single line segment.
    /// </summary>
    public class LineMode
    {
        /// <summary>
        /// Gets or sets the starting point of the line.
        /// </summary>
        public PointF pt1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the endpoint of the line.
        /// </summary>
        public PointF pt2
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores multiple connected line segments.
    /// </summary>
    public class LinesMode
    {
        /// <summary>
        /// Gets or sets the points that define the connected lines.
        /// </summary>
        public PointF[]? points
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Describes a nested path to add.
    /// </summary>
    public class PathMode
    {
        /// <summary>
        /// Gets or sets whether the path should be connected to the previous figure.
        /// </summary>
        public bool connect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path to add.
        /// </summary>
        public GraphicsPath? path
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a pie segment parameters.
    /// </summary>
    public class PieMode
    {
        /// <summary>
        /// Gets or sets the bounding rectangle for the pie.
        /// </summary>
        public RectangleF rect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the starting angle for the pie.
        /// </summary>
        public float startAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sweep angle for the pie.
        /// </summary>
        public float sweepAngle
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a polygon definition.
    /// </summary>
    public class PolygonMode
    {
        /// <summary>
        /// Gets or sets the points that define the polygon.
        /// </summary>
        public PointF[]? points
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores a rectangle definition.
    /// </summary>
    public class RectangleMode
    {
        /// <summary>
        /// Gets or sets the rectangle.
        /// </summary>
        public RectangleF rect
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores multiple rectangles.
    /// </summary>
    public class RectanglesMode
    {
        /// <summary>
        /// Gets or sets the rectangles.
        /// </summary>
        public RectangleF[]? rects
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Stores string drawing parameters used when text is added to the path.
    /// </summary>
    public class StringMode
    {
        /// <summary>
        /// Gets or sets the em size (font em height) used for the string.
        /// </summary>
        public float emSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font family for the string.
        /// </summary>
        public FontFamily? family
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the optional string format for layout.
        /// </summary>
        public StringFormat? format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the layout rectangle for the string.
        /// </summary>
        public RectangleF layoutRect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style (FontStyle value encoded as int) for the string.
        /// </summary>
        public int style
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the string text.
        /// </summary>
        public string? text
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Determines whether the specified rect is visible.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <param name="graphics">The graphics.</param>
    /// <returns>
    ///   <c>true</c> if the specified rect is visible; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">graphics</exception>
    public bool IsVisible(RectangleF rect, Graphics graphics)
    {
        if (graphics == null) throw new ArgumentNullException(nameof(graphics));

        // Conservative, fast check: if path bounding box intersects query rect -> visible
        var b = GetBounds();
        if (b.IsEmpty) return false;
        if (b.IntersectsWith(rect)) return true;

        // Fallback: check any explicit points stored in the path
        if (PathPoints != null)
        {
            foreach (var p in PathPoints)
            {
                if (rect.Contains(p))
                    return true;
            }
        }

        if (SizePoints != null)
        {
            foreach (var p in SizePoints)
            {
                if (rect.Contains(p))
                    return true;
            }
        }

        // No intersection detected by conservative checks
        return false;
    }
}
using System.Drawing.Drawing2D;

namespace System.Drawing;

/// <summary>
/// Describes the interior of a graphics shape composed of rectangles and paths. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed class Region : MarshalByRefObject, IDisposable
{
    private GraphicsPath graphicsPath = new GraphicsPath();

    private RectangleF rectangle = new RectangleF(0, 0, 0, 0);

    private RegionData regionData = new RegionData();

    // internal state flags to represent empty/infinite semantics
    private bool isEmpty = false;
    private bool isInfinite = false;

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    public Region()
    {
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" /> with the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="path">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> that defines the new
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="path" /> is
    /// <see langword="null" />.
    /// </exception>
    public Region(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        graphicsPath = path.Clone() as GraphicsPath ?? new GraphicsPath();
        rectangle = graphicsPath.GetBounds();
        isEmpty = rectangle.IsEmpty;
        isInfinite = false;
        UpdateRegionData();
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" /> from the specified data.
    /// </summary>
    /// <param name="rgnData">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.RegionData" /> that defines the interior of the new
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="rgnData" /> is
    /// <see langword="null" />.
    /// </exception>
    public Region(RegionData rgnData)
    {
        regionData = rgnData ?? throw new ArgumentNullException(nameof(rgnData));
        // default conservative state - we can't reconstruct the path easily without native calls
        graphicsPath = new GraphicsPath();
        rectangle = RectangleF.Empty;
        isEmpty = false;
        isInfinite = false;
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" /> from the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.Rectangle" /> structure that defines the interior of the new
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public Region(Rectangle rect)
    {
        rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        graphicsPath = new GraphicsPath();
        graphicsPath.AddRectangle(rectangle);
        isEmpty = rectangle.IsEmpty;
        isInfinite = false;
        UpdateRegionData();
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" /> from the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure.
    /// </summary>
    /// <param name="rect">
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> structure that defines the interior of the new
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public Region(RectangleF rect)
    {
        rectangle = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        graphicsPath = new GraphicsPath();
        graphicsPath.AddRectangle(rectangle);
        isEmpty = rectangle.IsEmpty;
        isInfinite = false;
        UpdateRegionData();
    }

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Region" /> that this method creates.
    /// </returns>
    public Region Clone()
    {
        var clone = new Region
        {
            graphicsPath = graphicsPath.Clone() as GraphicsPath ?? new GraphicsPath(),
            rectangle = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height),
            regionData = regionData,
            isEmpty = isEmpty,
            isInfinite = isInfinite // conservative copy (RegionData is immutable-like here)
        };
        return clone;
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain the portion of the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> that does not intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="path">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to complement this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="path" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Complement(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        // Approximate complement: treat complement as "set to provided path" if current is infinite,
        // otherwise subtract intersection area from current bounding box (approximation).
        if (isInfinite)
        {
            graphicsPath = path.Clone() as GraphicsPath ?? new GraphicsPath();
            rectangle = graphicsPath.GetBounds();
            isInfinite = false;
            isEmpty = rectangle.IsEmpty;
        }
        else if (isEmpty)
        {
            // complement of empty is empty in this simplistic model
        }
        else
        {
            var pathBounds = path.GetBounds();
            if (rectangle.IntersectsWith(pathBounds))
            {
                // simple approximation: reduce our rectangle by intersection bounding box
                var inter = RectangleF.Intersect(rectangle, pathBounds);
                if (inter == rectangle)
                {
                    MakeEmpty();
                }
                else
                {
                    // shrink bounding box conservatively (not geometrically exact)
                    rectangle = rectangle.SubtractSafe(inter);
                    graphicsPath = new GraphicsPath();
                    if (!rectangle.IsEmpty)
                        graphicsPath.AddRectangle(rectangle);
                }
            }
        }
        UpdateRegionData();
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain the portion of the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure that does not intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to complement this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Complement(Rectangle rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        Complement(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain the portion of the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure that does not intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to complement this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Complement(RectangleF rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(rect);
        Complement(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain the portion of the specified
    /// <see cref="T:System.Drawing.Region" /> that does not intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> object to complement this
    /// <see cref="T:System.Drawing.Region" /> object.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="region" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Complement(Region region)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        Complement(region.graphicsPath);
    }

    /// <summary>
    /// Releases all resources used by this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    public void Dispose()
    {
        rectangle = new RectangleF();
        graphicsPath?.Dispose();
        graphicsPath = new GraphicsPath();
        regionData = new RegionData();
        isEmpty = true;
        isInfinite = false;
    }

    /// <summary>
    /// Tests whether the specified
    /// <see cref="T:System.Drawing.Region" /> is identical to this
    /// <see cref="T:System.Drawing.Region" /> on the specified drawing surface.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the interior of region is identical to the interior of this region when the transformation associated with the
    /// <paramref name="g" /> parameter is applied; otherwise,
    /// <see langword="false" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="g" /> or
    /// <paramref name="region" /> is
    /// <see langword="null" />.
    /// </exception>
    public bool Equals(Region region, Graphics g)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        if (g == null) throw new ArgumentNullException(nameof(g));
        // conservative check: compare bounding boxes and emptiness/infinite state
        return isEmpty == region.isEmpty && isInfinite == region.isInfinite && rectangle.Equals(region.rectangle);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="path">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to exclude from this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="path" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Exclude(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        if (isEmpty) return;
        if (isInfinite)
        {
            // infinite minus a finite path -> still infinite (approx)
            return;
        }

        var pathBounds = path.GetBounds();
        if (rectangle.IntersectsWith(pathBounds))
        {
            // if the excluding rect fully covers current rectangle, region becomes empty
            if (pathBounds.Contains(rectangle))
            {
                MakeEmpty();
            }
            else
            {
                // conservative approximation: shrink the bounding rectangle by removing the intersection
                var inter = RectangleF.Intersect(rectangle, pathBounds);
                rectangle = rectangle.SubtractSafe(inter);
                graphicsPath = new GraphicsPath();
                if (!rectangle.IsEmpty)
                    graphicsPath.AddRectangle(rectangle);
            }
        }
        UpdateRegionData();
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to exclude from this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Exclude(Rectangle rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        Exclude(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to exclude from this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Exclude(RectangleF rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(rect);
        Exclude(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to contain only the portion of its interior that does not intersect with the specified
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> to exclude from this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="region" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Exclude(Region region)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        Exclude(region.graphicsPath);
    }

    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" /> from a handle to the specified existing GDI region.
    /// </summary>
    /// <param name="hrgn">
    /// A handle to an existing
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <returns>
    /// The new
    /// <see cref="T:System.Drawing.Region" />.
    /// </returns>
    public static Region? FromHrgn(IntPtr hrgn)
    {
        // Not able to recreate native GDI region without platform interop in this implementation.
        return null;
    }

    /// <summary>
    /// Gets a
    /// <see cref="T:System.Drawing.RectangleF" /> structure that represents a rectangle that bounds this
    /// <see cref="T:System.Drawing.Region" /> on the drawing surface of a
    /// <see cref="T:System.Drawing.Graphics" /> object.
    /// </summary>
    /// <param name="g">
    /// The
    /// <see cref="T:System.Drawing.Graphics" /> on which this
    /// <see cref="T:System.Drawing.Region" /> is drawn.
    /// </param>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.RectangleF" /> structure that represents the bounding rectangle for this
    /// <see cref="T:System.Drawing.Region" /> on the specified drawing surface.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="g" /> is
    /// <see langword="null" />.
    /// </exception>
    public RectangleF GetBounds(Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        if (isInfinite)
        {
            // return the clip bounds as an approximation for infinite regions
            return g.ClipBounds;
        }
        return rectangle.IsEmpty ? g.ClipBounds : rectangle;
    }

    /// <summary>
    /// Returns a Windows handle to this
    /// <see cref="T:System.Drawing.Region" /> in the specified graphics context.
    /// </summary>
    /// <param name="g">
    /// The
    /// <see cref="T:System.Drawing.Graphics" /> on which this
    /// <see cref="T:System.Drawing.Region" /> is drawn.
    /// </param>
    /// <returns>
    /// A Windows handle to this
    /// <see cref="T:System.Drawing.Region" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="g" /> is
    /// <see langword="null" />.
    /// </exception>
    public IntPtr GetHrgn(Graphics g)
    {
        // Native handle creation is platform-specific and not implemented here.
        return IntPtr.Zero;
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.RegionData" /> that represents the information that describes this
    /// <see cref="T:System.Drawing.Region" />.
    /// </returns>
    public RegionData GetRegionData()
    {
        return regionData;
    }

    /// <summary>
    /// Returns an array of
    /// <see cref="T:System.Drawing.RectangleF" /> structures that approximate this
    /// <see cref="T:System.Drawing.Region" /> after the specified matrix transformation is applied.
    /// </summary>
    /// <param name="matrix">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> that represents a geometric transformation to apply to the region.
    /// </param>
    /// <returns>
    /// An array of
    /// <see cref="T:System.Drawing.RectangleF" /> structures that approximate this
    /// <see cref="T:System.Drawing.Region" /> after the specified matrix transformation is applied.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="matrix" /> is
    /// <see langword="null" />.
    /// </exception>
    public RectangleF[] GetRegionScans(System.Drawing.Drawing2D.Matrix matrix)
    {
        if (matrix == null) throw new ArgumentNullException(nameof(matrix));
        if (isInfinite)
            return new[] { new RectangleF(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity) };

        // approximate by transforming the bounding rectangle
        var r = rectangle;
        var pts = new[] { new PointF(r.Left, r.Top), new PointF(r.Right, r.Top), new PointF(r.Right, r.Bottom), new PointF(r.Left, r.Bottom) };
        matrix.TransformPoints(pts);
        var bounds = RectangleFExtensions.FromPoints(pts);
        return new[] { bounds };
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="path">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Intersect(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        if (isInfinite)
        {
            graphicsPath = path.Clone() as GraphicsPath ?? new GraphicsPath();
            rectangle = graphicsPath.GetBounds();
            isInfinite = false;
            isEmpty = rectangle.IsEmpty;
        }
        else if (isEmpty)
        {
            return;
        }
        else
        {
            var pb = path.GetBounds();
            var inter = RectangleF.Intersect(rectangle, pb);
            if (inter.IsEmpty)
            {
                MakeEmpty();
            }
            else
            {
                rectangle = inter;
                graphicsPath = new GraphicsPath();
                graphicsPath.AddRectangle(rectangle);
            }
        }
        UpdateRegionData();
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Intersect(Rectangle rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        Intersect(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Intersect(RectangleF rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(rect);
        Intersect(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> to intersect with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Intersect(Region region)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        Intersect(region.graphicsPath);
    }

    /// <summary>
    /// Tests whether this
    /// <see cref="T:System.Drawing.Region" /> has an empty interior on the specified drawing surface.
    /// </summary>
    /// <param name="g">
    /// The
    /// <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the interior of this
    /// <see cref="T:System.Drawing.Region" /> is empty when the transformation associated with
    /// <paramref name="g" /> is applied; otherwise,
    /// <see langword="false" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="g" /> is
    /// <see langword="null" />.
    /// </exception>
    public bool IsEmpty(Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        return isEmpty || (rectangle.IsEmpty && graphicsPath.PointCount == 0);
    }

    /// <summary>
    /// Tests whether this
    /// <see cref="T:System.Drawing.Region" /> has an infinite interior on the specified drawing surface.
    /// </summary>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a drawing surface.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the interior of this
    /// <see cref="T:System.Drawing.Region" /> is infinite when the transformation associated with
    /// <paramref name="g" /> is applied; otherwise,
    /// <see langword="false" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="g" /> is
    /// <see langword="null" />.
    /// </exception>
    public bool IsInfinite(Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        return isInfinite;
    }

    /// <summary>
    /// Tests whether the specified
    /// <see cref="T:System.Drawing.Point" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="point">
    /// The
    /// <see cref="T:System.Drawing.Point" /> structure to test.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when
    /// <paramref name="point" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(Point point)
    {
        return IsVisible(new PointF(point.X, point.Y));
    }

    /// <summary>
    /// Tests whether the specified
    /// <see cref="T:System.Drawing.Point" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="point">
    /// The
    /// <see cref="T:System.Drawing.Point" /> structure to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when
    /// <paramref name="point" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(Point point, Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        return IsVisible(new PointF(point.X, point.Y), g);
    }

    /// <summary>
    /// Tests whether the specified
    /// <see cref="T:System.Drawing.PointF" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="point">
    /// The
    /// <see cref="T:System.Drawing.PointF" /> structure to test.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when
    /// <paramref name="point" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(PointF point)
    {
        if (isInfinite) return true;
        if (isEmpty) return false;
        if (rectangle.IsEmpty && graphicsPath.PointCount == 0) return false;
        if (!rectangle.IsEmpty && rectangle.Contains(point)) return true;
        // fallback to path hit-test
        using (var bmp = new Bitmap(1, 1))
        using (var g = Graphics.FromImage(bmp))
        {
            return graphicsPath.IsVisible(point, g);
        }
    }

    /// <summary>
    /// Tests whether the specified
    /// <see cref="T:System.Drawing.PointF" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="point">
    /// The
    /// <see cref="T:System.Drawing.PointF" /> structure to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when
    /// <paramref name="point" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(PointF point, Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        if (isInfinite) return true;
        if (isEmpty) return false;
        if (!rectangle.IsEmpty && rectangle.Contains(point)) return true;
        return graphicsPath.IsVisible(point, g);
    }

    /// <summary>
    /// Tests whether any portion of the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to test.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> when any portion of
    /// <paramref name="rect" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(Rectangle rect)
    {
        return IsVisible(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
    }

    /// <summary>
    /// Tests whether any portion of the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> when any portion of
    /// <paramref name="rect" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(Rectangle rect, Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        return IsVisible(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), g);
    }

    /// <summary>
    /// Tests whether any portion of the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to test.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> when any portion of
    /// <paramref name="rect" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(RectangleF rect)
    {
        if (isInfinite) return true;
        if (isEmpty) return false;
        if (!rectangle.IsEmpty && rectangle.IntersectsWith(rect)) return true;
        // fallback to path hit-test using a small bitmap
        using (var bmp = new Bitmap(1, 1))
        using (var g = Graphics.FromImage(bmp))
        {
            return graphicsPath.IsVisible(rect, g);
        }
    }

    /// <summary>
    /// Tests whether any portion of the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// This method returns
    /// <see langword="true" /> when any portion of
    /// <paramref name="rect" /> is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(RectangleF rect, Graphics g)
    {
        if (g == null) throw new ArgumentNullException(nameof(g));
        if (isInfinite) return true;
        if (isEmpty) return false;
        if (!rectangle.IsEmpty && rectangle.IntersectsWith(rect)) return true;
        return graphicsPath.IsVisible(rect, g);
    }

    /// <summary>
    /// Tests whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" /> object when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" /> object.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the point to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the point to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(int x, int y, Graphics g)
    {
        return IsVisible(new Point(x, y), g);

    }

    /// <summary>
    /// Tests whether any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="width">
    /// The width of the rectangle to test.
    /// </param>
    /// <param name="height">
    /// The height of the rectangle to test.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(int x, int y, int width, int height)
    {
        return IsVisible(new Rectangle(x, y, width, height));

    }

    /// <summary>
    /// Tests whether any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="width">
    /// The width of the rectangle to test.
    /// </param>
    /// <param name="height">
    /// The height of the rectangle to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(int x, int y, int width, int height, Graphics g)
    {
        return IsVisible(new Rectangle(x, y, width, height), g);
    }

    /// <summary>
    /// Tests whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the point to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the point to test.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(float x, float y)
    {
        return IsVisible(new PointF(x, y));
    }

    /// <summary>
    /// Tests whether the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the point to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the point to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when the specified point is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(float x, float y, Graphics g)
    {
        return IsVisible(new PointF(x, y), g);
    }

    /// <summary>
    /// Tests whether any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="width">
    /// The width of the rectangle to test.
    /// </param>
    /// <param name="height">
    /// The height of the rectangle to test.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" /> object; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(float x, float y, float width, float height)
    {
        return IsVisible(new RectangleF(x, y, width, height));
    }

    /// <summary>
    /// Tests whether any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" /> when drawn using the specified
    /// <see cref="T:System.Drawing.Graphics" />.
    /// </summary>
    /// <param name="x">
    /// The x-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="y">
    /// The y-coordinate of the upper-left corner of the rectangle to test.
    /// </param>
    /// <param name="width">
    /// The width of the rectangle to test.
    /// </param>
    /// <param name="height">
    /// The height of the rectangle to test.
    /// </param>
    /// <param name="g">
    /// A
    /// <see cref="T:System.Drawing.Graphics" /> that represents a graphics context.
    /// </param>
    /// <returns>
    /// <see langword="true" /> when any portion of the specified rectangle is contained within this
    /// <see cref="T:System.Drawing.Region" />; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public bool IsVisible(float x, float y, float width, float height, Graphics g)
    {
        return IsVisible(new RectangleF(x, y, width, height), g);
    }

    /// <summary>
    /// Initializes this
    /// <see cref="T:System.Drawing.Region" /> to an empty interior.
    /// </summary>
    public void MakeEmpty()
    {
        isEmpty = true;
        isInfinite = false;
        graphicsPath?.Reset();
        rectangle = RectangleF.Empty;
        UpdateRegionData();
    }

    /// <summary>
    /// Initializes this
    /// <see cref="T:System.Drawing.Region" /> object to an infinite interior.
    /// </summary>
    public void MakeInfinite()
    {
        isInfinite = true;
        isEmpty = false;
        graphicsPath?.Reset();
        rectangle = RectangleF.Empty;
        UpdateRegionData();
    }

    /// <summary>
    /// Releases the handle of the
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="regionHandle">
    /// The handle to the
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="regionHandle" /> is
    /// <see langword="null" />.
    /// </exception>
    public void ReleaseHrgn(IntPtr regionHandle)
    {
        // nothing to do in managed implementation
    }

    /// <summary>
    /// Transforms this
    /// <see cref="T:System.Drawing.Region" /> by the specified
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" />.
    /// </summary>
    /// <param name="matrix">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.Matrix" /> by which to transform this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="matrix" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Transform(System.Drawing.Drawing2D.Matrix matrix)
    {
        if (matrix == null) throw new ArgumentNullException(nameof(matrix));
        if (isInfinite || isEmpty) return;
        graphicsPath.Transform(matrix);
        rectangle = graphicsPath.GetBounds();
        UpdateRegionData();
    }

    /// <summary>
    /// Offsets the coordinates of this
    /// <see cref="T:System.Drawing.Region" /> by the specified amount.
    /// </summary>
    /// <param name="dx">
    /// The amount to offset this
    /// <see cref="T:System.Drawing.Region" /> horizontally.
    /// </param>
    /// <param name="dy">
    /// The amount to offset this
    /// <see cref="T:System.Drawing.Region" /> vertically.
    /// </param>
    public void Translate(int dx, int dy)
    {
        Translate((float)dx, (float)dy);
    }

    /// <summary>
    /// Offsets the coordinates of this
    /// <see cref="T:System.Drawing.Region" /> by the specified amount.
    /// </summary>
    /// <param name="dx">
    /// The amount to offset this
    /// <see cref="T:System.Drawing.Region" /> horizontally.
    /// </param>
    /// <param name="dy">
    /// The amount to offset this
    /// <see cref="T:System.Drawing.Region" /> vertically.
    /// </param>
    public void Translate(float dx, float dy)
    {
        if (isInfinite || isEmpty) return;
        var m = new System.Drawing.Drawing2D.Matrix();
        m.Translate(dx, dy);
        graphicsPath.Transform(m);
        rectangle = graphicsPath.GetBounds();
        UpdateRegionData();
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union of itself and the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="path">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to unite with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="path" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Union(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        if (isInfinite) return;
        if (isEmpty)
        {
            graphicsPath = path.Clone() as GraphicsPath ?? new GraphicsPath();
            rectangle = graphicsPath.GetBounds();
            isEmpty = rectangle.IsEmpty;
        }
        else
        {
            if (path.Clone() is GraphicsPath clone)
                graphicsPath.AddPath(clone, false);
            rectangle = RectangleF.Union(rectangle, path.GetBounds());
        }
        UpdateRegionData();
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union of itself and the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to unite with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Union(Rectangle rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        Union(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union of itself and the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to unite with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Union(RectangleF rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(rect);
        Union(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union of itself and the specified
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> to unite with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="region" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Union(Region region)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        Union(region.graphicsPath);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" />.
    /// </summary>
    /// <param name="path">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> to
    /// <see cref="T:System.Drawing.Region.Xor" /> with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Xor(GraphicsPath path)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));
        if (isInfinite)
        {
            // infinite xor finite -> still infinite
            return;
        }
        if (isEmpty)
        {
            graphicsPath = path.Clone() as GraphicsPath ?? new GraphicsPath();
            rectangle = graphicsPath.GetBounds();
            isEmpty = rectangle.IsEmpty;
            UpdateRegionData();
            return;
        }

        // approximate XOR using bounding rectangles: union - intersection (conservative)
        var unionRect = RectangleF.Union(rectangle, path.GetBounds());
        var inter = RectangleF.Intersect(rectangle, path.GetBounds());
        if (inter.IsEmpty)
        {
            Union(path);
        }
        else if (inter == rectangle && inter == path.GetBounds())
        {
            MakeEmpty();
        }
        else
        {
            graphicsPath = new GraphicsPath();
            graphicsPath.AddRectangle(unionRect);
            // not a precise XOR, but provides consistent bounding info
            rectangle = unionRect;
            UpdateRegionData();
        }
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Rectangle" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.Rectangle" /> structure to
    /// <see cref="T:System.Drawing.Region.Xor" /> with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Xor(Rectangle rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        Xor(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified
    /// <see cref="T:System.Drawing.RectangleF" /> structure.
    /// </summary>
    /// <param name="rect">
    /// The
    /// <see cref="T:System.Drawing.RectangleF" /> structure to
    /// <see cref="T:System.Drawing.Region.Xor(System.Drawing.Drawing2D.GraphicsPath)" /> with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    public void Xor(RectangleF rect)
    {
        var gp = new GraphicsPath();
        gp.AddRectangle(rect);
        Xor(gp);
    }

    /// <summary>
    /// Updates this
    /// <see cref="T:System.Drawing.Region" /> to the union minus the intersection of itself with the specified
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    /// <param name="region">
    /// The
    /// <see cref="T:System.Drawing.Region" /> to
    /// <see cref="T:System.Drawing.Region.Xor" /> with this
    /// <see cref="T:System.Drawing.Region" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="region" /> is
    /// <see langword="null" />.
    /// </exception>
    public void Xor(Region region)
    {
        if (region == null) throw new ArgumentNullException(nameof(region));
        Xor(region.graphicsPath);
    }

    // helper to keep regionData in sync with our simple managed representation.
    private void UpdateRegionData()
    {
        // RegionData cannot be filled without native or System.Drawing.Region; create fresh placeholder
        regionData = new RegionData();
    }
}

// small helpers used above
internal static class RectangleFExtensions
{
    public static RectangleF FromPoints(PointF[] pts)
    {
        if (pts == null || pts.Length == 0) return RectangleF.Empty;
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

    // conservative subtract: remove intersection area from original bounding rectangle,
    // returns an approximation rectangle (may shrink to empty)
    public static RectangleF SubtractSafe(this RectangleF original, RectangleF subtract)
    {
        if (original.IsEmpty) return RectangleF.Empty;
        if (subtract.IsEmpty) return original;
        if (!original.IntersectsWith(subtract)) return original;
        // if subtract fully contains original, result empty
        if (subtract.Contains(original)) return RectangleF.Empty;
        // otherwise shrink by intersection rectangle on one axis as a conservative approximation
        var inter = RectangleF.Intersect(original, subtract);
        var leftWidth = inter.Left - original.Left;
        var rightWidth = original.Right - inter.Right;
        var topHeight = inter.Top - original.Top;
        var bottomHeight = original.Bottom - inter.Bottom;

        // pick larger remaining width or height to keep a conservative rectangle
        if (Math.Max(leftWidth + rightWidth, topHeight + bottomHeight) <= 0)
            return RectangleF.Empty;

        if ((leftWidth + rightWidth) >= (topHeight + bottomHeight))
        {
            // keep horizontal remainder
            float newLeft = original.Left;
            float newRight = original.Right;
            if (leftWidth >= rightWidth)
                newRight = inter.Left;
            else
                newLeft = inter.Right;
            return new RectangleF(newLeft, original.Top, Math.Max(0, newRight - newLeft), original.Height);
        }
        else
        {
            // keep vertical remainder
            float newTop = original.Top;
            float newBottom = original.Bottom;
            if (topHeight >= bottomHeight)
                newBottom = inter.Top;
            else
                newTop = inter.Bottom;
            return new RectangleF(original.Left, newTop, original.Width, Math.Max(0, newBottom - newTop));
        }
    }
}
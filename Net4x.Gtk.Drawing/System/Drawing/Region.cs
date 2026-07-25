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

    // ---------------------------------------------------------------------
    // The region was backed by a single RectangleF, so every combine operation
    // collapsed to rectangle arithmetic: Xor of two partially overlapping
    // rectangles returned their union, Exclude shrank the bounding box, and a
    // non-rectangular region was not representable at all. The interior is now
    // a list of disjoint rectangles - the band list GDI+ itself uses - which
    // makes the whole combine family exact for rectangular inputs.
    //
    // A GraphicsPath contributes its bounding rectangle, so curved regions are
    // still approximated by their bounds; the path is kept alongside so that
    // clipping can use the true shape.
    // ---------------------------------------------------------------------

    private List<RectangleF> bands = new();

    /// <summary>The disjoint rectangles whose union is this region.</summary>
    internal IReadOnlyList<RectangleF> Bands => bands;

    /// <summary>
    /// True while the interior is still described exactly by <see cref="graphicsPath" /> - that is, the
    /// region came straight from a <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> and has not been
    /// combined with anything since. The band list only approximates such a region by its bounding
    /// rectangle, so a consumer that can honour the real shape (clipping) should ask for
    /// <see cref="ExactPath" /> first.
    /// </summary>
    private bool isPathExact;

    /// <summary>
    /// The path that describes this region exactly, or <see langword="null" /> when only the band list
    /// does - after any combine operation, and for regions built from rectangles.
    /// </summary>
    internal GraphicsPath? ExactPath => isPathExact ? graphicsPath : null;

    /// <summary>Replaces the interior with the given rectangles and refreshes the derived state.</summary>
    private void SetBands(List<RectangleF> value)
    {
        bands = Compact(value);
        isInfinite = false;
        isEmpty = bands.Count == 0;
        isPathExact = false;
        rectangle = BoundsOf(bands);
        graphicsPath = new GraphicsPath();
        foreach (var band in bands)
        {
            graphicsPath.AddRectangle(band);
        }

        UpdateRegionData();
    }

    /// <summary>Drops degenerate rectangles.</summary>
    private static List<RectangleF> Compact(List<RectangleF> value)
    {
        var result = new List<RectangleF>(value.Count);
        foreach (var band in value)
        {
            if (band.Width > 0f && band.Height > 0f)
            {
                result.Add(band);
            }
        }

        return result;
    }

    /// <summary>The smallest rectangle containing every band.</summary>
    private static RectangleF BoundsOf(List<RectangleF> value)
    {
        if (value.Count == 0)
        {
            return RectangleF.Empty;
        }

        var result = value[0];
        for (var i = 1; i < value.Count; i++)
        {
            result = RectangleF.Union(result, value[i]);
        }

        return result;
    }

    /// <summary>Adds <paramref name="a" /> minus <paramref name="b" /> to <paramref name="into" />, as up to four disjoint rectangles.</summary>
    private static void SubtractInto(RectangleF a, RectangleF b, List<RectangleF> into)
    {
        var overlap = RectangleF.Intersect(a, b);
        if (overlap.Width <= 0f || overlap.Height <= 0f)
        {
            into.Add(a);

            return;
        }

        if (overlap.Top > a.Top)
        {
            into.Add(new RectangleF(a.Left, a.Top, a.Width, overlap.Top - a.Top));
        }

        if (overlap.Bottom < a.Bottom)
        {
            into.Add(new RectangleF(a.Left, overlap.Bottom, a.Width, a.Bottom - overlap.Bottom));
        }

        if (overlap.Left > a.Left)
        {
            into.Add(new RectangleF(a.Left, overlap.Top, overlap.Left - a.Left, overlap.Height));
        }

        if (overlap.Right < a.Right)
        {
            into.Add(new RectangleF(overlap.Right, overlap.Top, a.Right - overlap.Right, overlap.Height));
        }
    }

    /// <summary>Everything in <paramref name="from" /> that is not in <paramref name="what" />.</summary>
    private static List<RectangleF> SubtractBands(List<RectangleF> from, List<RectangleF> what)
    {
        var current = new List<RectangleF>(from);
        foreach (var cut in what)
        {
            var next = new List<RectangleF>();
            foreach (var piece in current)
            {
                SubtractInto(piece, cut, next);
            }

            current = next;
        }

        return Compact(current);
    }

    /// <summary>Everything in either operand, kept disjoint.</summary>
    private static List<RectangleF> UnionBands(List<RectangleF> a, List<RectangleF> b)
    {
        var result = new List<RectangleF>(a);
        foreach (var addition in b)
        {
            result.AddRange(SubtractBands(new List<RectangleF> { addition }, result));
        }

        return Compact(result);
    }

    /// <summary>Everything in both operands.</summary>
    private static List<RectangleF> IntersectBands(List<RectangleF> a, List<RectangleF> b)
    {
        var result = new List<RectangleF>();
        foreach (var x in a)
        {
            foreach (var y in b)
            {
                var overlap = RectangleF.Intersect(x, y);
                if (overlap.Width > 0f && overlap.Height > 0f)
                {
                    result.Add(overlap);
                }
            }
        }

        return result;
    }

    /// <summary>Everything in exactly one of the operands.</summary>
    private static List<RectangleF> XorBands(List<RectangleF> a, List<RectangleF> b) =>
        UnionBands(SubtractBands(a, b), SubtractBands(b, a));

    /// <summary>The bands a path contributes: its bounding rectangle.</summary>
    private static List<RectangleF> BandsOfPath(GraphicsPath path)
    {
        var bounds = path.GetBounds();

        return bounds.Width > 0f && bounds.Height > 0f
            ? new List<RectangleF> { bounds }
            : new List<RectangleF>();
    }

    /// <summary>The bands of another region, as a fresh list.</summary>
    private static List<RectangleF> BandsOfRegion(Region region) => new(region.bands);


    /// <summary>
    /// Initializes a new
    /// <see cref="T:System.Drawing.Region" />.
    /// </summary>
    public Region()
    {
        // A default Region is infinite in GDI+ (it covers the entire coordinate space). Leaving it
        // non-infinite made IsInfinite report false and skewed the combine operations that branch on it.
        isInfinite = true;
        isEmpty = false;
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
        var clone = path.Clone() as GraphicsPath ?? new GraphicsPath();
        SetBands(BandsOfPath(clone));
        // Keep the real path so that clipping can use the true shape rather than the bounding box.
        graphicsPath = clone;
        isPathExact = true;
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
        graphicsPath = new GraphicsPath();
        rectangle = RectangleF.Empty;
        isEmpty = false;
        isInfinite = false;
        // Region data produced by this implementation carries the bounding rectangle and the state flags,
        // so a Region can be reconstructed from it instead of coming back empty.
        TryRestoreFromRegionData(regionData);
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
        SetBands(new List<RectangleF> { new(rect.X, rect.Y, rect.Width, rect.Height) });
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
        SetBands(new List<RectangleF> { new(rect.X, rect.Y, rect.Width, rect.Height) });
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
            isInfinite = isInfinite,
            isPathExact = isPathExact,
            // The band list has to be copied, not shared: without this, emptying the original emptied
            // the clone as well.
            bands = new List<RectangleF>(bands)
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

        // Complement replaces this region with the part of the argument that is outside it.
        if (isInfinite)
        {
            SetBands(new List<RectangleF>());

            return;
        }

        SetBands(SubtractBands(BandsOfPath(path), bands));
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

        if (isInfinite)
        {
            SetBands(new List<RectangleF>());

            return;
        }

        if (region.isInfinite)
        {
            MakeInfinite();

            return;
        }

        SetBands(SubtractBands(BandsOfRegion(region), bands));
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
        isPathExact = false;
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
        if (isInfinite) return;

        SetBands(SubtractBands(bands, BandsOfPath(path)));
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
        if (isInfinite) return;
        if (region.isInfinite)
        {
            SetBands(new List<RectangleF>());

            return;
        }

        SetBands(SubtractBands(bands, BandsOfRegion(region)));
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
        // This returned the single bounding rectangle; it now returns the actual bands, which is what
        // GetRegionScans means.
        var scans = new RectangleF[bands.Count];
        for (var i = 0; i < bands.Count; i++)
        {
            var band = bands[i];
            if (matrix == null)
            {
                scans[i] = band;

                continue;
            }

            var corners = new[] { new PointF(band.Left, band.Top), new PointF(band.Right, band.Bottom) };
            matrix.TransformPoints(corners);
            scans[i] = RectangleF.FromLTRB(
                Math.Min(corners[0].X, corners[1].X),
                Math.Min(corners[0].Y, corners[1].Y),
                Math.Max(corners[0].X, corners[1].X),
                Math.Max(corners[0].Y, corners[1].Y));
        }

        return scans;
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

        // Intersecting the infinite region with a shape yields that shape.
        SetBands(isInfinite ? BandsOfPath(path) : IntersectBands(bands, BandsOfPath(path)));
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
        if (region.isInfinite) return;

        SetBands(isInfinite ? BandsOfRegion(region) : IntersectBands(bands, BandsOfRegion(region)));
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
        // An infinite region clears the backing rectangle, so the fallback test below used to report it
        // as empty - an infinite region is never empty.
        if (isInfinite) return false;
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

        // This tested the bounding rectangle, so a point in the hole of an L-shaped region counted as
        // visible. It now tests the bands.
        foreach (var band in bands)
        {
            if (band.Contains(point))
            {
                return true;
            }
        }

        return false;
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

        foreach (var band in bands)
        {
            if (band.IntersectsWith(rect))
            {
                return true;
            }
        }

        return false;
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
        SetBands(new List<RectangleF>());
    }

    /// <summary>
    /// Initializes this
    /// <see cref="T:System.Drawing.Region" /> object to an infinite interior.
    /// </summary>
    public void MakeInfinite()
    {
        bands = new List<RectangleF>();
        isInfinite = true;
        isEmpty = false;
        isPathExact = false;
        graphicsPath = new GraphicsPath();
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

        var moved = new List<RectangleF>(bands.Count);
        foreach (var band in bands)
        {
            moved.Add(new RectangleF(band.X + dx, band.Y + dy, band.Width, band.Height));
        }

        SetBands(moved);
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

        SetBands(UnionBands(bands, BandsOfPath(path)));
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
        if (isInfinite) return;
        if (region.isInfinite)
        {
            MakeInfinite();

            return;
        }

        // These used to funnel through the other region's GraphicsPath, so region-vs-region combines were
        // only as good as the bounding box of that path. They now work on the band lists directly.
        SetBands(UnionBands(bands, BandsOfRegion(region)));
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
        if (isInfinite) return;

        // This returned the union of the two bounding boxes when they partly overlapped, which is the
        // opposite of an exclusive-or: the overlap was kept rather than removed.
        SetBands(XorBands(bands, BandsOfPath(path)));
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
        if (isInfinite || region.isInfinite) return;

        SetBands(XorBands(bands, BandsOfRegion(region)));
    }

    // helper to keep regionData in sync with our simple managed representation.
    /// <summary>
    /// Serialized layout of <see cref="RegionData.Data"/>: four floats for the bounding rectangle followed
    /// by the empty and infinite flags.
    /// </summary>
    private const int RegionDataLength = (4 * sizeof(float)) + 2;

    private void UpdateRegionData()
    {
        // The data used to be an empty placeholder, so a Region could not be round-tripped through its
        // RegionData. Serialize the bounding rectangle and the state flags instead.
        var data = new byte[RegionDataLength];
        Buffer.BlockCopy(BitConverter.GetBytes(rectangle.X), 0, data, 0, sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rectangle.Y), 0, data, sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rectangle.Width), 0, data, 2 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(rectangle.Height), 0, data, 3 * sizeof(float), sizeof(float));
        data[4 * sizeof(float)] = (byte)(isEmpty ? 1 : 0);
        data[(4 * sizeof(float)) + 1] = (byte)(isInfinite ? 1 : 0);
        regionData = new RegionData { Data = data };
    }

    /// <summary>
    /// Restores the rectangle and state flags previously written by <see cref="UpdateRegionData"/>.
    /// Returns false when the data is absent or not in that layout.
    /// </summary>
    private bool TryRestoreFromRegionData(RegionData rgnData)
    {
        var data = rgnData.Data;
        if (data == null || data.Length < RegionDataLength)
        {
            return false;
        }

        rectangle = new RectangleF(
            BitConverter.ToSingle(data, 0),
            BitConverter.ToSingle(data, sizeof(float)),
            BitConverter.ToSingle(data, 2 * sizeof(float)),
            BitConverter.ToSingle(data, 3 * sizeof(float)));
        isEmpty = data[4 * sizeof(float)] != 0;
        isInfinite = data[(4 * sizeof(float)) + 1] != 0;
        graphicsPath = new GraphicsPath();
        if (!rectangle.IsEmpty)
        {
            graphicsPath.AddRectangle(rectangle);
        }

        return true;
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
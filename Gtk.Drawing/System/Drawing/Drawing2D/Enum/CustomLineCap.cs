
namespace System.Drawing.Drawing2D;

/// <summary>
/// Encapsulates a custom user-defined line cap.
/// </summary>
public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
{

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class with the specified outline and fill.
    /// </summary>
    /// <param name="fillPath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.
    /// </param>
    /// <param name="strokePath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.
    /// </param>
    public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath) : this(fillPath, strokePath, LineCap.Flat, 0f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class from the specified existing
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration with the specified outline and fill.
    /// </summary>
    /// <param name="fillPath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.
    /// </param>
    /// <param name="strokePath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.
    /// </param>
    /// <param name="baseCap">
    /// The line cap from which to create the custom cap.
    /// </param>
    public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap) : this(fillPath, strokePath, baseCap, 0f)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> class from the specified existing
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration with the specified outline, fill, and inset.
    /// </summary>
    /// <param name="fillPath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the fill for the custom cap.
    /// </param>
    /// <param name="strokePath">
    /// A
    /// <see cref="T:System.Drawing.Drawing2D.GraphicsPath" /> object that defines the outline of the custom cap.
    /// </param>
    /// <param name="baseCap">
    /// The line cap from which to create the custom cap.
    /// </param>
    /// <param name="baseInset">
    /// The distance between the cap and the line.
    /// </param>
    public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
    {
        // The constructors discarded everything they were given, the getters threw while the setters
        // silently accepted, and Clone returned null - so a custom cap could be built but never inspected,
        // copied or applied. The state is now stored; the cap still does not render (Cairo has no
        // user-defined cap primitive), which is recorded.
        _fillPath = fillPath;
        _strokePath = strokePath;
        BaseCap = baseCap;
        BaseInset = baseInset;
    }

    private readonly GraphicsPath? _fillPath;

    private readonly GraphicsPath? _strokePath;

    private LineCap _startCap = LineCap.Flat;

    private LineCap _endCap = LineCap.Flat;

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration on which this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> is based.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration on which this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> is based.
    /// </returns>
    public LineCap BaseCap
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the distance between the cap and the line.
    /// </summary>
    /// <returns>
    /// The distance between the beginning of the cap and the end of the line.
    /// </returns>
    public float BaseInset
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the
    /// <see cref="T:System.Drawing.Drawing2D.LineJoin" /> enumeration that determines how lines that compose this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object are joined.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineJoin" /> enumeration this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object uses to join lines.
    /// </returns>
    public LineJoin StrokeJoin
    {
        get;
        set;
    } = LineJoin.Miter;

    /// <summary>
    /// Gets or sets the amount by which to scale this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> Class object with respect to the width of the
    /// <see cref="T:System.Drawing.Pen" /> object.
    /// </summary>
    /// <returns>
    /// The amount by which to scale the cap.
    /// </returns>
    public float WidthScale
    {
        get;
        set;
    } = 1f;

    /// <summary>
    /// Creates an exact copy of this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" />.
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> this method creates, cast as an object.
    /// </returns>
    public object? Clone()
    {
        return new CustomLineCap(
            _fillPath?.Clone() as GraphicsPath ?? new GraphicsPath(),
            _strokePath?.Clone() as GraphicsPath ?? new GraphicsPath(),
            BaseCap,
            BaseInset)
        {
            StrokeJoin = StrokeJoin,
            WidthScale = WidthScale,
            _startCap = _startCap,
            _endCap = _endCap
        };
    }

    /// <summary>
    /// Releases all resources used by this
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> object.
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Gets the caps used to start and end lines that make up this custom cap.
    /// </summary>
    /// <param name="startCap">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the beginning of a line within this cap.
    /// </param>
    /// <param name="endCap">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the end of a line within this cap.
    /// </param>
    public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
    {
        startCap = _startCap;
        endCap = _endCap;
    }

    /// <summary>
    /// Sets the caps used to start and end lines that make up this custom cap.
    /// </summary>
    /// <param name="startCap">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the beginning of a line within this cap.
    /// </param>
    /// <param name="endCap">
    /// The
    /// <see cref="T:System.Drawing.Drawing2D.LineCap" /> enumeration used at the end of a line within this cap.
    /// </param>
    public void SetStrokeCaps(LineCap startCap, LineCap endCap)
    {
        _startCap = startCap;
        _endCap = endCap;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.Drawing.Drawing2D.CustomLineCap" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true" /> to release both managed and unmanaged resources;
    /// <see langword="false" /> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
    }
}
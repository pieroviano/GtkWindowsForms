extern alias gtk;
using Pen = gtk::System.Drawing.Pen;
using SolidBrush = gtk::System.Drawing.SolidBrush;
using TextureBrush = gtk::System.Drawing.TextureBrush;
using Bitmap = gtk::System.Drawing.Bitmap;
using Icon = gtk::System.Drawing.Icon;
using HatchBrush = gtk::System.Drawing.Drawing2D.HatchBrush;
using HatchStyle = gtk::System.Drawing.Drawing2D.HatchStyle;
using LinearGradientBrush = gtk::System.Drawing.Drawing2D.LinearGradientBrush;
using PenType = gtk::System.Drawing.Drawing2D.PenType;
using CustomLineCap = gtk::System.Drawing.Drawing2D.CustomLineCap;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;
using LineCap = gtk::System.Drawing.Drawing2D.LineCap;
using LineJoin = gtk::System.Drawing.Drawing2D.LineJoin;

namespace System.Drawing;

/// <summary>Covers the last §4.4 throw sites: <c>Pen.PenType</c> threw, <c>Icon.FromHandle</c> threw
/// <c>NotImplementedException</c> instead of the platform verdict, and <see cref="CustomLineCap"/>
/// discarded its constructor arguments, threw from every getter while its setters silently accepted, and
/// cloned to null.</summary>
[Collection("NonParallelCollection")]
public class PenTypeAndCustomLineCapFunctionalTests
{
    // ---- Pen.PenType ------------------------------------------------------

    [Fact]
    public void PenType_IsSolidColorForAColourPen()
    {
        using var pen = new Pen(Color.Red);

        Assert.Equal(PenType.SolidColor, pen.PenType);
    }

    [Fact]
    public void PenType_FollowsTheKindOfBrush()
    {
        using var texture = new Bitmap(4, 4);

        Assert.Equal(PenType.SolidColor, new Pen(new SolidBrush(Color.Red), 1f).PenType);
        Assert.Equal(PenType.HatchFill, new Pen(new HatchBrush(HatchStyle.Cross, Color.Red), 1f).PenType);
        Assert.Equal(PenType.LinearGradient,
            new Pen(new LinearGradientBrush(new Rectangle(0, 0, 10, 10), Color.Red, Color.Blue, 0f), 1f).PenType);
        Assert.Equal(PenType.TextureFill, new Pen(new TextureBrush(texture), 1f).PenType);
    }

    // ---- Icon.FromHandle --------------------------------------------------

    [Fact]
    public void IconFromHandle_ReportsThatHandlesAreUnavailable()
    {
        Assert.Throws<PlatformNotSupportedException>(() => Icon.FromHandle(IntPtr.Zero));
    }

    // ---- CustomLineCap ----------------------------------------------------

    private static GraphicsPath Arrow()
    {
        var path = new GraphicsPath();
        path.AddPolygon(new[] { new Point(0, 0), new Point(4, -8), new Point(-4, -8) });

        return path;
    }

    [Fact]
    public void TheConstructorArgumentsAreKept()
    {
        using var cap = new CustomLineCap(Arrow(), Arrow(), LineCap.Round, 2.5f);

        Assert.Equal(LineCap.Round, cap.BaseCap);
        Assert.Equal(2.5f, cap.BaseInset, 3);
    }

    [Fact]
    public void TheDefaultsMatchWinForms()
    {
        using var cap = new CustomLineCap(Arrow(), Arrow());

        Assert.Equal(LineCap.Flat, cap.BaseCap);
        Assert.Equal(0f, cap.BaseInset, 3);
        Assert.Equal(LineJoin.Miter, cap.StrokeJoin);
        Assert.Equal(1f, cap.WidthScale, 3);
    }

    [Fact]
    public void ThePropertiesRoundTrip()
    {
        using var cap = new CustomLineCap(Arrow(), Arrow())
        {
            BaseCap = LineCap.Triangle,
            BaseInset = 1.5f,
            StrokeJoin = LineJoin.Bevel,
            WidthScale = 2f
        };

        Assert.Equal(LineCap.Triangle, cap.BaseCap);
        Assert.Equal(1.5f, cap.BaseInset, 3);
        Assert.Equal(LineJoin.Bevel, cap.StrokeJoin);
        Assert.Equal(2f, cap.WidthScale, 3);
    }

    [Fact]
    public void StrokeCapsRoundTripThroughSetAndGet()
    {
        using var cap = new CustomLineCap(Arrow(), Arrow());

        cap.SetStrokeCaps(LineCap.Round, LineCap.Square);
        cap.GetStrokeCaps(out var start, out var end);

        Assert.Equal(LineCap.Round, start);
        Assert.Equal(LineCap.Square, end);
    }

    [Fact]
    public void CloneIsARealIndependentCopy()
    {
        using var original = new CustomLineCap(Arrow(), Arrow(), LineCap.Round, 3f)
        {
            StrokeJoin = LineJoin.Round,
            WidthScale = 2f
        };
        original.SetStrokeCaps(LineCap.Triangle, LineCap.Round);

        var clone = Assert.IsType<CustomLineCap>(original.Clone());
        clone.BaseCap = LineCap.Flat;
        clone.SetStrokeCaps(LineCap.Flat, LineCap.Flat);

        Assert.Equal(LineCap.Round, original.BaseCap);
        Assert.Equal(3f, clone.BaseInset, 3);
        Assert.Equal(LineJoin.Round, clone.StrokeJoin);
        Assert.Equal(2f, clone.WidthScale, 3);
        original.GetStrokeCaps(out var start, out _);
        Assert.Equal(LineCap.Triangle, start);
    }
}

extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Font = gtk::System.Drawing.Font;
using StringFormat = gtk::System.Drawing.StringFormat;
using CharacterRange = gtk::System.Drawing.CharacterRange;

namespace System.Drawing;

/// <summary>
/// Behavioral fidelity for <c>Graphics.MeasureCharacterRanges</c> — the text hit-testing primitive used to
/// locate substrings within drawn text (selection highlighting, link regions). The earlier implementation
/// measured each range's preceding run and the range itself in isolation and added the widths, which
/// double-counts the kerning/side-bearing at the boundary: adjacent ranges then overlapped or left gaps.
/// These tests assert the WinForms invariants that hold regardless of the underlying text engine: ranges
/// stay ordered, contiguous ranges tile the line without overlap, and each region sits inside the text.
/// </summary>
[Collection("NonParallelCollection")]
public class MeasureCharacterRangesFunctionalTests
{
    // Surface that swallows the flush-on-dispose GLib.GException some environments raise from
    // Pixbuf.SaveToBuffer — same guard the other Graphics functional tests use (see GraphicsApiFunctionalTests).
    private sealed class Target : IDisposable
    {
        private readonly Bitmap _bmp = new(400, 80);
        public Graphics G { get; }
        public Target() => G = Graphics.FromImage(_bmp);
        public void Dispose() { try { G.Dispose(); } catch { } _bmp.Dispose(); }
    }

    private static StringFormat WithRanges(params CharacterRange[] ranges)
    {
        var fmt = new StringFormat();
        fmt.SetMeasurableCharacterRanges(ranges);
        return fmt;
    }

    [Fact]
    public void NoRanges_ReturnsEmpty()
    {
        using var t = new Target();
        using var font = new Font("Arial", 12);
        var regions = t.G.MeasureCharacterRanges("hello", font, new RectangleF(0, 0, 400, 80), new StringFormat());
        Assert.Empty(regions);
    }

    [Fact]
    public void OneRegionPerRequestedRange()
    {
        using var t = new Target();
        using var font = new Font("Arial", 12);
        using var fmt = WithRanges(new CharacterRange(0, 2), new CharacterRange(2, 3));
        var regions = t.G.MeasureCharacterRanges("Hello World", font, new RectangleF(0, 0, 400, 80), fmt);
        Assert.Equal(2, regions.Length);
        Assert.All(regions, r => Assert.NotNull(r));
    }

    [Fact]
    public void ContiguousRangesTileWithoutOverlapOrGap()
    {
        // "Hello" split as [0,3) and [3,2) — the second must start exactly where the first ends. With the
        // old isolated-substring math the boundary kerning was counted twice and the edges did not meet.
        using var t = new Target();
        using var font = new Font("Arial", 12);
        using var fmt = WithRanges(new CharacterRange(0, 3), new CharacterRange(3, 2));

        var regions = t.G.MeasureCharacterRanges("Hello", font, new RectangleF(10, 5, 380, 70), fmt);

        var first = regions[0].GetBounds(t.G);
        var second = regions[1].GetBounds(t.G);

        var firstRight = first.X + first.Width;
        Assert.True(Math.Abs(second.X - firstRight) < 1.0f,
            $"contiguous ranges must abut: first ends at {firstRight}, second starts at {second.X}");
    }

    [Fact]
    public void TiledSubRangesCoverExactlyTheSameSpanAsOneWholeRange()
    {
        // Internal consistency: splitting a string into two contiguous ranges must cover exactly the same
        // horizontal span as a single range over the whole string. This is the kerning-aware invariant the
        // old isolated-substring math broke — it is checked against MeasureCharacterRanges itself rather
        // than MeasureString (which pads its reported width by the font descent).
        using var t = new Target();
        using var font = new Font("Arial", 14);
        const string text = "WAVETeam";

        using var whole = WithRanges(new CharacterRange(0, text.Length));
        using var split = WithRanges(new CharacterRange(0, 4), new CharacterRange(4, 4));

        var wholeBounds = t.G.MeasureCharacterRanges(text, font, new RectangleF(0, 0, 380, 70), whole)[0].GetBounds(t.G);
        var parts = t.G.MeasureCharacterRanges(text, font, new RectangleF(0, 0, 380, 70), split);
        var a = parts[0].GetBounds(t.G);
        var b = parts[1].GetBounds(t.G);

        Assert.True(a.Width > 0 && b.Width > 0);
        // First range starts at the layout origin.
        Assert.True(Math.Abs(a.X - 0f) < 1.0f);
        // Second range abuts the first (no double-counted boundary kerning).
        Assert.True(Math.Abs(b.X - (a.X + a.Width)) < 1.0f);
        // The tiled coverage equals the single whole-range span within sub-pixel tolerance.
        var covered = (b.X + b.Width) - a.X;
        Assert.True(Math.Abs(covered - wholeBounds.Width) < 1.0f,
            $"tiled ranges should span exactly the whole range: covered={covered}, whole={wholeBounds.Width}");
    }

    [Fact]
    public void RegionsAreOneLineTallAndOrderedLeftToRight()
    {
        using var t = new Target();
        using var font = new Font("Arial", 12);
        using var fmt = WithRanges(new CharacterRange(0, 1), new CharacterRange(2, 1), new CharacterRange(5, 1));

        var regions = t.G.MeasureCharacterRanges("abcdefgh", font, new RectangleF(0, 0, 380, 70), fmt);
        var bounds = Array.ConvertAll(regions, r => r.GetBounds(t.G));

        for (var i = 1; i < bounds.Length; i++)
            Assert.True(bounds[i].X >= bounds[i - 1].X, "ranges must be ordered left to right");

        Assert.All(bounds, b => Assert.True(b.Height > 0));
        for (var i = 1; i < bounds.Length; i++)
            Assert.True(Math.Abs(bounds[i].Height - bounds[0].Height) < 0.5f);
    }

    [Fact]
    public void RangeClampedToTextLength_DoesNotThrow()
    {
        using var t = new Target();
        using var font = new Font("Arial", 12);
        using var fmt = WithRanges(new CharacterRange(3, 100));

        var regions = t.G.MeasureCharacterRanges("abcde", font, new RectangleF(0, 0, 380, 70), fmt);
        Assert.Single(regions);
        var b = regions[0].GetBounds(t.G);
        Assert.True(b.Width >= 0);
    }
}

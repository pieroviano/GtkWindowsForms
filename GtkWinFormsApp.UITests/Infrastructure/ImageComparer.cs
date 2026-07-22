using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// Tolerant PNG comparison (SampleUITests-Plan.md §6). Two images match when they share dimensions and the
/// fraction of pixels whose worst channel differs by more than <see cref="ChannelTolerance"/> stays at or
/// below <see cref="MaxDiffFraction"/>. The tolerance absorbs anti-aliasing/theme noise; a real visual
/// regression moves far more than 0.5% of the pixels.
/// </summary>
internal static class ImageComparer
{
    /// <summary>Per-channel (0-255) delta a pixel may drift before it counts as "different".</summary>
    private const int ChannelTolerance = 12;

    /// <summary>Fraction of differing pixels that is still considered a match (0.5%).</summary>
    private const double MaxDiffFraction = 0.005;

    /// <summary>True when the run was asked to (re)write baselines instead of asserting.</summary>
    internal static bool ShouldUpdateBaselines
    {
        get
        {
            var value = Environment.GetEnvironmentVariable("UPDATE_UI_BASELINES");
            return string.Equals(value, "1", StringComparison.Ordinal)
                   || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed record Result(bool Match, string Message, byte[]? DiffPng);

    internal static Result Compare(byte[] baselinePng, byte[] actualPng)
    {
        using var baseline = Image.Load<Rgba32>(baselinePng);
        using var actual = Image.Load<Rgba32>(actualPng);

        if (baseline.Width != actual.Width || baseline.Height != actual.Height)
        {
            return new Result(
                false,
                $"dimensions {actual.Width}x{actual.Height} differ from baseline {baseline.Width}x{baseline.Height}",
                null);
        }

        var count = baseline.Width * baseline.Height;
        var pixelsA = new Rgba32[count];
        var pixelsB = new Rgba32[count];
        baseline.CopyPixelDataTo(pixelsA);
        actual.CopyPixelDataTo(pixelsB);

        var red = new Rgba32(255, 0, 0, 255);
        var black = new Rgba32(0, 0, 0, 255);
        var diffPixels = new Rgba32[count];

        long differing = 0;
        for (var i = 0; i < count; i++)
        {
            var pa = pixelsA[i];
            var pb = pixelsB[i];
            var worst = Math.Max(
                Math.Max(Math.Abs(pa.R - pb.R), Math.Abs(pa.G - pb.G)),
                Math.Max(Math.Abs(pa.B - pb.B), Math.Abs(pa.A - pb.A)));

            if (worst > ChannelTolerance)
            {
                differing++;
                diffPixels[i] = red;
            }
            else
            {
                diffPixels[i] = black;
            }
        }

        var fraction = count == 0 ? 0 : (double)differing / count;
        var match = fraction <= MaxDiffFraction;
        var message = $"{differing}/{count} px ({fraction:P2}) exceed channel tolerance {ChannelTolerance} " +
                      $"(threshold {MaxDiffFraction:P2})";

        byte[]? diffPng = null;
        if (!match)
        {
            using var diff = Image.LoadPixelData<Rgba32>(diffPixels, baseline.Width, baseline.Height);
            using var stream = new MemoryStream();
            diff.SaveAsPng(stream);
            diffPng = stream.ToArray();
        }

        return new Result(match, message, diffPng);
    }
}

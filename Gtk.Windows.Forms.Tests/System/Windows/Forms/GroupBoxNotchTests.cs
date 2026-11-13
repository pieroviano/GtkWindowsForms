#pragma warning disable CS0612

namespace System.Windows.Forms;

using Gtk.Windows.Forms.Controls.ControlBase;

/// <summary>
/// A <see cref="GroupBox" /> notches its caption out of its top border, as WinForms does.
/// </summary>
/// <remarks>
/// Gtk3 got the notch for free: <c>gtk_frame_draw</c> painted the border with
/// <c>gtk_render_frame_gap</c>, which leaves the caption's extent out of the top edge. Gtk4 deleted that
/// primitive - a frame allocates its caption inside the content box and lets ordinary CSS box rendering draw
/// an uninterrupted rectangle - so the shim has to draw the border itself and suppress the theme's.
/// </remarks>
[Collection("NonParallelCollection")]
public class GroupBoxNotchTests
{
    private const int Height = 100;

    private const int Width = 200;

    // Constructing a GroupBox builds GTK widgets, which needs the type system initialised.
    public GroupBoxNotchTests() => Application.Init();

    [Fact]
    public void TheCaption_IsCutOutOfTheTopEdge()
    {
        // Half-pixel top so the one pixel line lands squarely on row 10 instead of straddling two rows.
        using var surface = Stroke(top: 10.5, gapStart: 8, gapEnd: 60);
        var pixels = surface.Data;

        Assert.True(HasInk(surface, pixels, 5, 10), "the top edge must resume left of the caption");
        Assert.True(HasInk(surface, pixels, 70, 10), "the top edge must resume right of the caption");
        Assert.False(HasInk(surface, pixels, 30, 10), "the caption's own span must be left unpainted");
    }

    [Fact]
    public void TheRemainingThreeEdges_AreClosed()
    {
        using var surface = Stroke(top: 10.5, gapStart: 8, gapEnd: 60);
        var pixels = surface.Data;

        Assert.True(HasInk(surface, pixels, 0, 50), "left edge");
        Assert.True(HasInk(surface, pixels, Width - 1, 50), "right edge");
        Assert.True(HasInk(surface, pixels, 100, Height - 1), "bottom edge");
    }

    [Fact]
    public void WithoutACaption_TheBorderIsAPlainRectangle()
    {
        // Before the first allocation there is no caption to measure, and a captionless WinForms group box
        // draws a closed rectangle - so the degenerate gap must not leave the top edge broken.
        using var surface = Stroke(top: 0.5, gapStart: 0, gapEnd: 0);
        var pixels = surface.Data;

        Assert.True(HasInk(surface, pixels, 30, 0), "an unnotched top edge must be continuous");
        Assert.True(HasInk(surface, pixels, 100, 0), "an unnotched top edge must be continuous");
    }

    [Fact]
    public void TheShimDrawsTheBorder_AndTheThemesOwnIsSuppressed()
    {
        // The two halves only work as a pair: without the CSS rule the theme's rectangle is still there and
        // the notch is invisible behind it; without the drawing there is no border at all.
        var box = new GroupBox { Text = "caption" };

        Assert.True(box.self.StyleContext.HasClass("GroupBox"),
            "the CSS rule is keyed on this class");
        Assert.Contains(".GroupBox{", Application.UserPriorityCss, StringComparison.Ordinal);
        Assert.Contains("border-width:0", GroupBoxRule(), StringComparison.Ordinal);
        Assert.NotNull(typeof(GroupBoxBase).GetMethod(
            "OnSnapshot",
            Reflection.BindingFlags.Instance | Reflection.BindingFlags.NonPublic
            | Reflection.BindingFlags.Public | Reflection.BindingFlags.DeclaredOnly));
    }

    [Fact]
    public void TheFramesContentBox_IsTheWidgetBox()
    {
        // Gtk4 gives a widget's own drawing a context whose origin is its CSS content box and clips it
        // there, so a border or padding of any width both shifts the shim's border and swallows its right
        // and bottom edges. Turning the border transparent is therefore not enough - it has to be zero,
        // and so does the padding, or the notch is drawn into a box that is not the one it is measured in.
        var rule = GroupBoxRule();

        Assert.Contains("border-width:0", rule, StringComparison.Ordinal);
        Assert.Contains("padding:0", rule, StringComparison.Ordinal);

        // Taking the background away would wipe GroupBox.BackColor, applied through this same class.
        Assert.DoesNotContain("background", rule, StringComparison.Ordinal);
    }

    private static string GroupBoxRule()
    {
        var css = Application.UserPriorityCss;
        var start = css.IndexOf(".GroupBox{", StringComparison.Ordinal);
        Assert.True(start >= 0, "the CSS half of the group box border is missing");

        var end = css.IndexOf('}', start);
        Assert.True(end > start, "the .GroupBox rule is not closed");

        return css.Substring(start, end - start + 1);
    }

    private static bool HasInk(Cairo.ImageSurface surface, byte[] pixels, int x, int y)
    {
        // Argb32 is a native-endian 32 bit word, so the alpha channel is the last byte of each pixel.
        return pixels[(y * surface.Stride) + (x * 4) + 3] > 0;
    }

    private static Cairo.ImageSurface Stroke(double top, double gapStart, double gapEnd)
    {
        var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, Width, Height);

        using (var cr = new Cairo.Context(surface))
        {
            // No antialiasing, so a pixel is either on the path or it is not.
            cr.Antialias = Cairo.Antialias.None;
            cr.SetSourceRGBA(0, 0, 0, 1);
            cr.LineWidth = 1;
            GroupBoxBase.StrokeNotchedFrame(cr, Width, Height, top, gapStart, gapEnd);
        }

        surface.Flush();
        return surface;
    }
}
#pragma warning restore CS0612

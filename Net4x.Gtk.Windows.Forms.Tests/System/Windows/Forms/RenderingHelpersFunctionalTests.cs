extern alias gtk;
using System.Windows.Forms;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Font = gtk::System.Drawing.Font;
using Color = global::System.Drawing.Color;
using Point = global::System.Drawing.Point;
using Rectangle = global::System.Drawing.Rectangle;
using Size = global::System.Drawing.Size;
using SystemColors = global::System.Drawing.SystemColors;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Covers the rendering and metrics helpers that EMULATION-GAPS.md §3.1 calls "the highest-frequency
/// break": <see cref="TextRenderer"/>, <see cref="ControlPaint"/> and <see cref="SystemInformation"/>. These
/// types did not exist at all, so any source using them failed to compile.</summary>
[Collection("NonParallelCollection")]
public class RenderingHelpersFunctionalTests
{
    private static Font TestFont() => new("Sans", 10f);

    private static bool IsWhitish(Color c) => c.R > 200 && c.G > 200 && c.B > 200;

    /// <summary>Counts the pixels that are not the white background.</summary>
    private static int InkCount(Action<Graphics> draw, int size = 60)
    {
        using var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            draw(g);
        }

        var n = 0;
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                if (!IsWhitish(bmp.GetPixel(x, y))) n++;
            }
        }

        return n;
    }

    // ---- TextRenderer ----------------------------------------------------

    [Fact]
    public void MeasureText_ReturnsAPositiveSizeForRealText()
    {
        var size = TextRenderer.MeasureText("Hello", TestFont());

        Assert.True(size.Width > 0, $"measured width should be positive but was {size.Width}");
        Assert.True(size.Height > 0, $"measured height should be positive but was {size.Height}");
    }

    [Fact]
    public void MeasureText_OfEmptyOrNullTextIsEmpty()
    {
        Assert.Equal(Size.Empty, TextRenderer.MeasureText(string.Empty, TestFont()));
        Assert.Equal(Size.Empty, TextRenderer.MeasureText(null, TestFont()));
    }

    [Fact]
    public void MeasureText_WithoutAFontIsEmpty()
    {
        Assert.Equal(Size.Empty, TextRenderer.MeasureText("Hello", null));
    }

    [Fact]
    public void MeasureText_LongerTextIsWider()
    {
        var shortText = TextRenderer.MeasureText("i", TestFont());
        var longText = TextRenderer.MeasureText("mmmmmmmmmm", TestFont());

        Assert.True(longText.Width > shortText.Width,
            $"'mmmmmmmmmm' ({longText.Width}) should measure wider than 'i' ({shortText.Width})");
    }

    [Fact]
    public void MeasureText_NoPaddingIsNarrowerThanTheDefaultPadding()
    {
        var padded = TextRenderer.MeasureText("Hello", TestFont(), new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.Default);
        var unpadded = TextRenderer.MeasureText("Hello", TestFont(), new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding);

        Assert.True(unpadded.Width < padded.Width,
            $"NoPadding ({unpadded.Width}) should measure narrower than the default ({padded.Width})");
    }

    [Fact]
    public void MeasureText_WrapsWithinAProposedWidth()
    {
        const string text = "the quick brown fox jumps over the lazy dog";
        var unconstrained = TextRenderer.MeasureText(text, TestFont());
        var wrapped = TextRenderer.MeasureText(text, TestFont(), new Size(60, int.MaxValue));

        Assert.True(wrapped.Height > unconstrained.Height,
            "text constrained to a narrow box should wrap onto more lines and so be taller");
    }

    [Fact]
    public void MeasureText_SingleLineDoesNotWrap()
    {
        const string text = "the quick brown fox jumps over the lazy dog";
        var wrapped = TextRenderer.MeasureText(text, TestFont(), new Size(60, int.MaxValue));
        var single = TextRenderer.MeasureText(text, TestFont(), new Size(60, int.MaxValue),
            TextFormatFlags.SingleLine);

        Assert.True(single.Height < wrapped.Height,
            "SingleLine should keep the text on one line even when the proposed box is narrow");
    }

    [Fact]
    public void MeasureText_OnAGraphicsAgreesWithTheStaticOverload()
    {
        using var bmp = new Bitmap(10, 10);
        using var g = Graphics.FromImage(bmp);

        var viaGraphics = TextRenderer.MeasureText(g, "Hello", TestFont());
        var viaStatic = TextRenderer.MeasureText("Hello", TestFont());

        Assert.Equal(viaStatic, viaGraphics);
    }

    [Fact]
    public void DrawText_PutsInkOnTheSurface()
    {
        Assert.True(InkCount(g => TextRenderer.DrawText(g, "Hi", TestFont(), new Point(2, 2), Color.Black)) > 0,
            "DrawText should have drawn something");
    }

    [Fact]
    public void DrawText_WithNoTextDrawsNothing()
    {
        Assert.Equal(0, InkCount(g => TextRenderer.DrawText(g, string.Empty, TestFont(), new Point(2, 2), Color.Black)));
    }

    [Fact]
    public void DrawText_PaintsTheBackColourBehindTheText()
    {
        using var bmp = new Bitmap(40, 20);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            TextRenderer.DrawText(g, "Hi", TestFont(), new Rectangle(0, 0, 40, 20), Color.Black, Color.Red);
        }

        // A corner well away from the glyphs should carry the requested background.
        var corner = bmp.GetPixel(38, 18);
        Assert.True(corner.R > 200 && corner.G < 100, $"the back colour should fill the bounds but the corner was {corner}");
    }

    [Fact]
    public void DrawText_RightAlignedLandsFurtherRightThanLeftAligned()
    {
        static int MeanInkX(TextFormatFlags flags)
        {
            using var bmp = new Bitmap(80, 20);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                TextRenderer.DrawText(g, "Hi", TestFont(), new Rectangle(0, 0, 80, 20), Color.Black, flags);
            }

            long sum = 0;
            var n = 0;
            for (var x = 0; x < 80; x++)
            {
                for (var y = 0; y < 20; y++)
                {
                    if (IsWhitish(bmp.GetPixel(x, y))) continue;
                    sum += x;
                    n++;
                }
            }

            return n == 0 ? -1 : (int)(sum / n);
        }

        var left = MeanInkX(TextFormatFlags.Left);
        var right = MeanInkX(TextFormatFlags.Right);

        Assert.True(left >= 0 && right >= 0, "both alignments should draw something");
        Assert.True(right > left, $"right-aligned text ({right}) should sit further right than left-aligned ({left})");
    }

    [Fact]
    public void DrawText_HorizontallyCentredLandsBetweenLeftAndRight()
    {
        static int MeanInkX(TextFormatFlags flags)
        {
            using var bmp = new Bitmap(80, 20);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                TextRenderer.DrawText(g, "Hi", TestFont(), new Rectangle(0, 0, 80, 20), Color.Black, flags);
            }

            long sum = 0;
            var n = 0;
            for (var x = 0; x < 80; x++)
            {
                for (var y = 0; y < 20; y++)
                {
                    if (IsWhitish(bmp.GetPixel(x, y))) continue;
                    sum += x;
                    n++;
                }
            }

            return n == 0 ? -1 : (int)(sum / n);
        }

        var left = MeanInkX(TextFormatFlags.Left);
        var centre = MeanInkX(TextFormatFlags.HorizontalCenter);
        var right = MeanInkX(TextFormatFlags.Right);

        Assert.InRange(centre, left + 1, right - 1);
    }

    [Fact]
    public void DrawText_OnANonGraphicsContextIsIgnored()
    {
        // Must not throw when the device context is not one this shim can draw on.
        TextRenderer.DrawText(null, "Hi", TestFont(), new Point(0, 0), Color.Black);
    }

    // ---- ControlPaint ----------------------------------------------------

    [Fact]
    public void Light_MovesTheColourTowardsWhite()
    {
        var lightened = ControlPaint.Light(Color.FromArgb(255, 100, 100, 100));

        Assert.True(lightened.R > 100, $"Light should raise the red channel but produced {lightened.R}");
        Assert.Equal(255, lightened.A);
    }

    [Fact]
    public void Dark_MovesTheColourTowardsBlack()
    {
        var darkened = ControlPaint.Dark(Color.FromArgb(255, 100, 100, 100));

        Assert.True(darkened.R < 100, $"Dark should lower the red channel but produced {darkened.R}");
    }

    [Fact]
    public void LightLightAndDarkDarkAreTheExtremes()
    {
        var baseColor = Color.FromArgb(255, 100, 120, 140);

        Assert.Equal(255, ControlPaint.LightLight(baseColor).R);
        Assert.Equal(0, ControlPaint.DarkDark(baseColor).R);
    }

    [Fact]
    public void LightAndDarkOrderTheShades()
    {
        var baseColor = Color.FromArgb(255, 128, 128, 128);

        Assert.True(ControlPaint.DarkDark(baseColor).R < ControlPaint.Dark(baseColor).R);
        Assert.True(ControlPaint.Dark(baseColor).R < baseColor.R);
        Assert.True(baseColor.R < ControlPaint.Light(baseColor).R);
        Assert.True(ControlPaint.Light(baseColor).R < ControlPaint.LightLight(baseColor).R);
    }

    [Fact]
    public void LightWithAFractionInterpolates()
    {
        var baseColor = Color.FromArgb(255, 0, 0, 0);

        Assert.Equal(0, ControlPaint.Light(baseColor, 0f).R);
        Assert.Equal(255, ControlPaint.Light(baseColor, 1f).R);
        Assert.InRange(ControlPaint.Light(baseColor, 0.5f).R, 120, 135);
    }

    [Fact]
    public void DrawBorder_PutsInkOnTheSurface()
    {
        Assert.True(
            InkCount(g => ControlPaint.DrawBorder(g, new Rectangle(5, 5, 40, 30), Color.Black, ButtonBorderStyle.Solid)) > 0,
            "DrawBorder should have drawn an outline");
    }

    [Fact]
    public void DrawBorder_WithStyleNoneDrawsNothing()
    {
        Assert.Equal(0,
            InkCount(g => ControlPaint.DrawBorder(g, new Rectangle(5, 5, 40, 30), Color.Black, ButtonBorderStyle.None)));
    }

    [Fact]
    public void DrawBorder3D_PutsInkOnTheSurface()
    {
        Assert.True(InkCount(g => ControlPaint.DrawBorder3D(g, new Rectangle(5, 5, 40, 30))) > 0,
            "DrawBorder3D should have drawn an outline");
    }

    [Fact]
    public void DrawBorder3D_SunkenAndRaisedDifferInAppearance()
    {
        static Color TopLeftPixel(Border3DStyle style)
        {
            using var bmp = new Bitmap(40, 40);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                ControlPaint.DrawBorder3D(g, new Rectangle(0, 0, 40, 40), style);
            }

            return bmp.GetPixel(1, 0);
        }

        Assert.NotEqual(TopLeftPixel(Border3DStyle.Sunken), TopLeftPixel(Border3DStyle.Raised));
    }

    [Fact]
    public void DrawFocusRectangle_PutsInkOnTheSurface()
    {
        Assert.True(InkCount(g => ControlPaint.DrawFocusRectangle(g, new Rectangle(5, 5, 40, 30))) > 0,
            "DrawFocusRectangle should have drawn an outline");
    }

    [Fact]
    public void DrawCheckBox_DrawsMoreWhenChecked()
    {
        var unchecked_ = InkCount(g => ControlPaint.DrawCheckBox(g, new Rectangle(5, 5, 20, 20), ButtonState.Normal));
        var checked_ = InkCount(g => ControlPaint.DrawCheckBox(g, new Rectangle(5, 5, 20, 20), ButtonState.Checked));

        Assert.True(checked_ > unchecked_,
            $"a checked box ({checked_}) should carry more ink than an unchecked one ({unchecked_})");
    }

    [Fact]
    public void DrawButton_PutsInkOnTheSurface()
    {
        Assert.True(InkCount(g => ControlPaint.DrawButton(g, new Rectangle(5, 5, 40, 20), ButtonState.Normal)) > 0,
            "DrawButton should have drawn the button");
    }

    [Fact]
    public void TheDrawHelpersRejectANullGraphics()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ControlPaint.DrawBorder(null!, new Rectangle(0, 0, 10, 10), Color.Black, ButtonBorderStyle.Solid));
        Assert.Throws<ArgumentNullException>(() => ControlPaint.DrawBorder3D(null!, new Rectangle(0, 0, 10, 10)));
        Assert.Throws<ArgumentNullException>(() => ControlPaint.DrawFocusRectangle(null!, new Rectangle(0, 0, 10, 10)));
        Assert.Throws<ArgumentNullException>(() => ControlPaint.DrawCheckBox(null!, new Rectangle(0, 0, 10, 10), ButtonState.Normal));
        Assert.Throws<ArgumentNullException>(() => ControlPaint.DrawButton(null!, new Rectangle(0, 0, 10, 10), ButtonState.Normal));
    }

    [Fact]
    public void TheDrawHelpersIgnoreAnEmptyRectangle()
    {
        Assert.Equal(0, InkCount(g => ControlPaint.DrawBorder3D(g, new Rectangle(5, 5, 0, 0))));
        Assert.Equal(0, InkCount(g => ControlPaint.DrawButton(g, new Rectangle(5, 5, 0, 0), ButtonState.Normal)));
    }

    // ---- SystemInformation -----------------------------------------------

    [Fact]
    public void TheMetricsLayoutCodeReadsArePositive()
    {
        Assert.True(SystemInformation.CaptionHeight > 0, "CaptionHeight should be positive");
        Assert.True(SystemInformation.MenuHeight > 0, "MenuHeight should be positive");
        Assert.True(SystemInformation.VerticalScrollBarWidth > 0, "VerticalScrollBarWidth should be positive");
        Assert.True(SystemInformation.HorizontalScrollBarHeight > 0, "HorizontalScrollBarHeight should be positive");
        Assert.True(SystemInformation.DoubleClickTime > 0, "DoubleClickTime should be positive");
        Assert.True(SystemInformation.BorderSize.Width > 0, "BorderSize should be positive");
        Assert.True(SystemInformation.FrameBorderSize.Width > 0, "FrameBorderSize should be positive");
    }

    [Fact]
    public void IconSizesAreOrdered()
    {
        Assert.True(SystemInformation.SmallIconSize.Width < SystemInformation.IconSize.Width,
            "a small icon should be smaller than a standard icon");
    }

    [Fact]
    public void TheVirtualScreenHasArea()
    {
        var screen = SystemInformation.VirtualScreen;

        Assert.True(screen.Width > 0 && screen.Height > 0, $"the virtual screen should have area but was {screen}");
        Assert.Equal(screen.Size, SystemInformation.PrimaryMonitorSize);
    }

    [Fact]
    public void MonitorCountIsAtLeastOne()
    {
        Assert.True(SystemInformation.MonitorCount >= 1, "there should be at least one monitor");
    }

    [Fact]
    public void TheEnvironmentStringsAreReported()
    {
        Assert.False(string.IsNullOrEmpty(SystemInformation.ComputerName), "ComputerName should be reported");
        Assert.False(string.IsNullOrEmpty(SystemInformation.UserName), "UserName should be reported");
    }

    // ---- SystemBrushes / SystemFonts --------------------------------------

    [Fact]
    public void SystemBrushesMatchTheirSystemColours()
    {
        var brush = Assert.IsType<gtk::System.Drawing.SolidBrush>(gtk::System.Drawing.SystemBrushes.Control);

        Assert.Equal(SystemColors.Control, brush.Color);
    }

    [Fact]
    public void SystemBrushesCanFillARectangle()
    {
        Assert.True(InkCount(g => g.FillRectangle(gtk::System.Drawing.SystemBrushes.ControlText, 0, 0, 20, 20)) > 0,
            "a system brush should be usable for filling");
    }

    [Fact]
    public void SystemFontsAreUsableAndDistinct()
    {
        using var caption = gtk::System.Drawing.SystemFonts.CaptionFont;
        using var normal = gtk::System.Drawing.SystemFonts.DefaultFont;

        Assert.True(caption.Size > 0, "a system font should have a positive size");
        Assert.True(caption.Bold, "the caption font is bold in WinForms");
        Assert.False(normal.Bold, "the default font is not bold");
    }

    [Fact]
    public void SystemFontsCanBeLookedUpByName()
    {
        using var byName = gtk::System.Drawing.SystemFonts.GetFontByName("MenuFont");

        Assert.NotNull(byName);
        Assert.Null(gtk::System.Drawing.SystemFonts.GetFontByName("NoSuchFont"));
    }

    [Fact]
    public void SystemFontsCanMeasureText()
    {
        using var font = gtk::System.Drawing.SystemFonts.MenuFont;

        var size = TextRenderer.MeasureText("File", font);

        Assert.True(size.Width > 0, "a system font should be usable for measurement");
    }
}

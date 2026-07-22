extern alias gtk;
using Graphics = gtk::System.Drawing.Graphics;
using Bitmap = gtk::System.Drawing.Bitmap;
using Icon = gtk::System.Drawing.Icon;
using SystemIcons = gtk::System.Drawing.SystemIcons;
using TextureBrush = gtk::System.Drawing.TextureBrush;
using Matrix = gtk::System.Drawing.Drawing2D.Matrix;
using WrapMode = gtk::System.Drawing.Drawing2D.WrapMode;

namespace System.Drawing;

/// <summary>Covers the last two Class A gaps in <c>Net4x.Gtk.Drawing</c>:
/// <see cref="TextureBrush"/>, the one core GDI+ brush type that was missing, and <see cref="SystemIcons"/>.
/// Also covers the <see cref="Matrix"/> members that used to throw (§4.4).</summary>
[Collection("NonParallelCollection")]
public class TextureBrushAndSystemIconsFunctionalTests
{
    /// <summary>A 4x4 image whose left half is red and right half is blue.</summary>
    private static Bitmap Texture()
    {
        var bmp = new Bitmap(4, 4);
        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 4; y++)
            {
                bmp.SetPixel(x, y, x < 2 ? Color.Red : Color.Blue);
            }
        }

        return bmp;
    }

    // ---- TextureBrush ----------------------------------------------------

    [Fact]
    public void TextureBrush_ExposesTheImageItWasBuiltWith()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture);

        Assert.Same(texture, brush.Image);
        Assert.Equal(WrapMode.Tile, brush.WrapMode);
    }

    [Fact]
    public void TextureBrush_ImageRectangleDefaultsToTheWholeImage()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture);

        Assert.Equal(0f, brush.ImageRectangle.X, 2);
        Assert.Equal(4f, brush.ImageRectangle.Width, 2);
        Assert.Equal(4f, brush.ImageRectangle.Height, 2);
    }

    [Fact]
    public void TextureBrush_ARegionConstructorRecordsThatRegion()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture, new Rectangle(1, 1, 2, 2));

        Assert.Equal(1f, brush.ImageRectangle.X, 2);
        Assert.Equal(2f, brush.ImageRectangle.Width, 2);
    }

    [Fact]
    public void TextureBrush_WrapModeRoundTrips()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture, WrapMode.Clamp);

        Assert.Equal(WrapMode.Clamp, brush.WrapMode);
    }

    [Fact]
    public void TextureBrush_RejectsANullImage()
    {
        Assert.Throws<ArgumentNullException>(() => new TextureBrush(null!));
    }

    [Fact]
    public void TextureBrush_ActuallyFillsWithTheImage()
    {
        using var texture = Texture();
        using var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.FillRectangle(new TextureBrush(texture), 0, 0, 16, 16);
        }

        // Every pixel should have come from the texture, so none should still be white.
        var white = 0;
        for (var x = 0; x < 16; x++)
        {
            for (var y = 0; y < 16; y++)
            {
                var p = bmp.GetPixel(x, y);
                if (p.R > 200 && p.G > 200 && p.B > 200) white++;
            }
        }

        Assert.True(white < 256, $"a texture fill should have replaced the white background but {white} pixels remain white");
    }

    [Fact]
    public void TextureBrush_TilesTheImageAcrossALargerArea()
    {
        using var texture = Texture();
        using var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            g.FillRectangle(new TextureBrush(texture, WrapMode.Tile), 0, 0, 16, 16);
        }

        // The 4-pixel-wide texture repeats, so far-right pixels still carry texture colour, not white.
        var farRight = bmp.GetPixel(14, 8);
        Assert.False(farRight.R > 200 && farRight.G > 200 && farRight.B > 200,
            $"tiling should repeat the texture across the whole area but the far right was {farRight}");
    }

    [Fact]
    public void TextureBrush_CloneIsIndependent()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture, WrapMode.Tile);

        var clone = Assert.IsType<TextureBrush>(brush.Clone());
        clone.WrapMode = WrapMode.Clamp;

        Assert.Equal(WrapMode.Tile, brush.WrapMode);
        Assert.Equal(WrapMode.Clamp, clone.WrapMode);
        Assert.Same(texture, clone.Image);
    }

    [Fact]
    public void TextureBrush_TransformOperationsAffectItsMatrix()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture);

        brush.TranslateTransform(10f, 20f);

        Assert.Equal(10f, brush.Transform.OffsetX, 2);
        Assert.Equal(20f, brush.Transform.OffsetY, 2);

        brush.ResetTransform();

        Assert.Equal(0f, brush.Transform.OffsetX, 2);
    }

    [Fact]
    public void TextureBrush_RotateAndScaleDoNotThrow()
    {
        using var texture = Texture();
        var brush = new TextureBrush(texture);

        brush.RotateTransform(45f);
        brush.ScaleTransform(2f, 2f);
        brush.MultiplyTransform(new Matrix());

        Assert.NotNull(brush.Transform);
    }

    // ---- Matrix members that used to throw (§4.4) -------------------------

    [Fact]
    public void Matrix_ElementsReportsTheSixValues()
    {
        using var matrix = new Matrix(1f, 2f, 3f, 4f, 5f, 6f);

        var elements = matrix.Elements;

        Assert.Equal(6, elements.Length);
        Assert.Equal(new[] { 1f, 2f, 3f, 4f, 5f, 6f }, elements);
    }

    [Fact]
    public void Matrix_ElementsRoundTripThroughTheConstructor()
    {
        using var original = new Matrix(2f, 0f, 0f, 3f, 7f, 9f);
        var e = original.Elements;

        using var rebuilt = new Matrix(e[0], e[1], e[2], e[3], e[4], e[5]);

        Assert.Equal(original.Elements, rebuilt.Elements);
    }

    [Fact]
    public void Matrix_IsIdentityIsTrueForAFreshMatrix()
    {
        using var identity = new Matrix();

        Assert.True(identity.IsIdentity, "a new matrix should be the identity");
    }

    [Fact]
    public void Matrix_IsIdentityIsFalseAfterATranslation()
    {
        using var matrix = new Matrix();
        matrix.Translate(5f, 5f);

        Assert.False(matrix.IsIdentity, "a translated matrix is not the identity");
    }

    [Fact]
    public void Matrix_IsInvertibleForANonDegenerateMatrix()
    {
        using var matrix = new Matrix(2f, 0f, 0f, 3f, 0f, 0f);

        Assert.True(matrix.IsInvertible, "a matrix with a non-zero determinant is invertible");
    }

    [Fact]
    public void Matrix_IsNotInvertibleWhenTheDeterminantIsZero()
    {
        using var degenerate = new Matrix(0f, 0f, 0f, 0f, 0f, 0f);

        Assert.False(degenerate.IsInvertible, "a matrix with a zero determinant is not invertible");
    }

    // ---- SystemIcons -----------------------------------------------------

    public static TheoryData<string> IconNames() => new()
    {
        "Application", "Asterisk", "Error", "Exclamation", "Hand",
        "Information", "Question", "Warning", "Shield", "WinLogo"
    };

    [Theory]
    [MemberData(nameof(IconNames))]
    public void EverySystemIconIsAvailableAndHasPixels(string name)
    {
        var property = typeof(SystemIcons).GetProperty(name);
        Assert.NotNull(property);

        using var icon = Assert.IsType<Icon>(property!.GetValue(null));

        Assert.True(icon.Width > 0 && icon.Height > 0,
            $"SystemIcons.{name} should have positive dimensions but was {icon.Width}x{icon.Height}");
    }

    [Fact]
    public void SystemIconsCanBeConvertedToABitmap()
    {
        using var icon = SystemIcons.Error;

        using var bitmap = icon.ToBitmap();

        Assert.NotNull(bitmap);
        Assert.True(bitmap!.Width > 0, "the converted bitmap should have pixels");
    }

    [Fact]
    public void TheAliasedIconsMatchTheirTargets()
    {
        using var asterisk = SystemIcons.Asterisk;
        using var information = SystemIcons.Information;
        using var exclamation = SystemIcons.Exclamation;
        using var warning = SystemIcons.Warning;

        Assert.Equal(information.Width, asterisk.Width);
        Assert.Equal(warning.Width, exclamation.Width);
    }
}

extern alias gtk;
using System.Windows.Forms;
using Bitmap = gtk::System.Drawing.Bitmap;
using Color = global::System.Drawing.Color;
using Rectangle = global::System.Drawing.Rectangle;
using Size = global::System.Drawing.Size;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>Covers the <c>Gtk.Windows.Forms</c> gaps: the
/// <c>VirtualModel</c> misspelling (§2.12), <c>SetStyle</c>/<c>GetStyle</c> having no backing state (§2.4)
/// and <c>DrawToBitmap</c> being an empty method (§2.14).</summary>
[Collection("NonParallelCollection")]
public class EmulationGapFunctionalTests
{
    /// <summary>Exposes the protected style API so a test can exercise the WinForms recipe.</summary>
    private sealed class StyledControl : Panel
    {
        public void Apply(ControlStyles styles, bool value) => SetStyle(styles, value);

        public bool Query(ControlStyles style) => GetStyle(style);
    }

    // ---- §2.12 ListView.VirtualMode --------------------------------------

    [Fact]
    public void ListView_ExposesVirtualModeUnderTheWinFormsName()
    {
        using var view = new ListView { VirtualMode = false };

        Assert.False(view.VirtualMode);

        view.VirtualMode = true;

        Assert.True(view.VirtualMode);
    }

    [Fact]
    public void ListView_TheOldMisspellingStillAgreesWithVirtualMode()
    {
#pragma warning disable CS0618 // the alias is deliberately obsolete
        using var view = new ListView();

        view.VirtualMode = false;
        Assert.False(view.VirtualModel);

        view.VirtualModel = true;
        Assert.True(view.VirtualMode);
#pragma warning restore CS0618
    }

    [Fact]
    public void ListView_VirtualListSizeAndVirtualModeAreIndependent()
    {
        using var view = new ListView { VirtualMode = true, VirtualListSize = 1000 };

        Assert.True(view.VirtualMode);
        Assert.Equal(1000, view.VirtualListSize);
    }

    // ---- §2.4 SetStyle / GetStyle ----------------------------------------

    [Fact]
    public void SetStyle_RecordsTheStyleSoGetStyleCanReadItBack()
    {
        var control = new StyledControl();

        control.Apply(ControlStyles.ResizeRedraw, true);

        Assert.True(control.Query(ControlStyles.ResizeRedraw), "a style that was switched on should read back on");
    }

    [Fact]
    public void SetStyle_CanSwitchAStyleOffAgain()
    {
        var control = new StyledControl();
        control.Apply(ControlStyles.ResizeRedraw, true);

        control.Apply(ControlStyles.ResizeRedraw, false);

        Assert.False(control.Query(ControlStyles.ResizeRedraw), "a style that was switched off should read back off");
    }

    [Fact]
    public void SetStyle_AppliesTheStandardCustomControlRecipeAsAWhole()
    {
        var control = new StyledControl();
        const ControlStyles recipe = ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                                     | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw;

        control.Apply(recipe, true);

        Assert.True(control.Query(ControlStyles.UserPaint), "UserPaint should be set");
        Assert.True(control.Query(ControlStyles.AllPaintingInWmPaint), "AllPaintingInWmPaint should be set");
        Assert.True(control.Query(ControlStyles.OptimizedDoubleBuffer), "OptimizedDoubleBuffer should be set");
        Assert.True(control.Query(ControlStyles.ResizeRedraw), "ResizeRedraw should be set");
        Assert.True(control.Query(recipe), "the whole recipe should read back as set");
    }

    [Fact]
    public void SetStyle_LeavesOtherStylesAlone()
    {
        var control = new StyledControl();
        var selectableBefore = control.Query(ControlStyles.Selectable);

        control.Apply(ControlStyles.ResizeRedraw, true);

        Assert.Equal(selectableBefore, control.Query(ControlStyles.Selectable));
    }

    [Fact]
    public void TheWinFormsDefaultStylesAreSetOnAFreshControl()
    {
        var control = new StyledControl();

        Assert.True(control.Query(ControlStyles.UserPaint), "UserPaint is on by default in WinForms");
        Assert.True(control.Query(ControlStyles.Selectable), "Selectable is on by default in WinForms");
        Assert.True(control.Query(ControlStyles.StandardClick), "StandardClick is on by default in WinForms");
        Assert.False(control.Query(ControlStyles.ResizeRedraw), "ResizeRedraw is off by default in WinForms");
    }

    [Fact]
    public void QueryingACombinationRequiresEveryBit()
    {
        var control = new StyledControl();
        control.Apply(ControlStyles.ResizeRedraw, true);

        Assert.False(control.Query(ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer),
            "a combination should only read back when every bit in it is set");
    }

    // ---- §2.14 Control.DrawToBitmap --------------------------------------

    [Fact]
    public void DrawToBitmap_PaintsTheControlBackgroundIntoTheBitmap()
    {
        using var control = new Panel { Size = new Size(20, 20), BackColor = Color.Red };
        using var bitmap = new Bitmap(20, 20);

        control.DrawToBitmap(bitmap, new Rectangle(0, 0, 20, 20));

        var pixel = bitmap.GetPixel(10, 10);
        Assert.True(pixel.R > 200 && pixel.G < 100 && pixel.B < 100,
            $"DrawToBitmap should have painted the control's red background but the pixel was {pixel}");
    }

    [Fact]
    public void DrawToBitmap_LeavesNoTransparentPixels()
    {
        using var control = new Panel { Size = new Size(16, 16), BackColor = Color.Blue };
        using var bitmap = new Bitmap(16, 16);

        control.DrawToBitmap(bitmap, new Rectangle(0, 0, 16, 16));

        Assert.Equal(255, bitmap.GetPixel(8, 8).A);
    }

    [Fact]
    public void DrawToBitmap_RejectsANullBitmap()
    {
        using var control = new Panel { Size = new Size(10, 10) };

        Assert.Throws<ArgumentNullException>(() => control.DrawToBitmap(null!, new Rectangle(0, 0, 10, 10)));
    }

    [Fact]
    public void DrawToBitmap_HonoursASmallerTargetRectangle()
    {
        using var control = new Panel { Size = new Size(20, 20), BackColor = Color.Green };
        using var bitmap = new Bitmap(20, 20);

        control.DrawToBitmap(bitmap, new Rectangle(0, 0, 8, 8));

        Assert.Equal(8, bitmap.Width);
        Assert.Equal(8, bitmap.Height);
    }

    [Fact]
    public void DrawToBitmap_OnADifferentControlUsesThatControlsBackground()
    {
        using var red = new Panel { Size = new Size(12, 12), BackColor = Color.Red };
        using var blue = new Panel { Size = new Size(12, 12), BackColor = Color.Blue };
        using var first = new Bitmap(12, 12);
        using var second = new Bitmap(12, 12);

        red.DrawToBitmap(first, new Rectangle(0, 0, 12, 12));
        blue.DrawToBitmap(second, new Rectangle(0, 0, 12, 12));

        Assert.True(first.GetPixel(6, 6).R > second.GetPixel(6, 6).R,
            "each control should render its own background colour");
    }
}

namespace System.Windows.Forms;

extern alias gtk;

using Rectangle = global::System.Drawing.Rectangle;
using Color = global::System.Drawing.Color;

/// <summary>
/// Verifies the ToolStrip theming surface added for source that customizes tool-strip appearance:
/// <see cref="ProfessionalColorTable" />/<see cref="ProfessionalColors" />, the
/// <see cref="ToolStripRenderer" /> Draw-to-OnRender wiring, <see cref="ToolStripManager" />, and the render
/// event-args constructor round-trips. These are model/rendering types with no realized widgets, so no form
/// is needed.
/// </summary>
public class ToolStripRenderingFunctionalTests
{
    private sealed class SpyRenderer : ToolStripRenderer
    {
        public bool ButtonDrawn;
        public bool ArrowDrawn;

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) => ButtonDrawn = true;

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) => ArrowDrawn = true;
    }

    [Fact]
    public void ProfessionalColors_DelegateToColorTable()
    {
        var table = new ProfessionalColorTable();
        Assert.Equal(table.MenuBorder, ProfessionalColors.MenuBorder);
        Assert.Equal(table.ButtonSelectedHighlight, ProfessionalColors.ButtonSelectedHighlight);
        Assert.Equal(table.ToolStripGradientBegin, ProfessionalColors.ToolStripGradientBegin);
    }

    [Fact]
    public void ProfessionalColorTable_ColorsAreValid()
    {
        var table = new ProfessionalColorTable();
        // A representative sample must resolve to real (non-empty) colors.
        Assert.False(table.MenuBorder.IsEmpty);
        Assert.False(table.SeparatorDark.IsEmpty);
        Assert.False(table.ToolStripDropDownBackground.IsEmpty);
    }

    [Fact]
    public void ToolStripRenderer_DrawInvokesMatchingOnRender()
    {
        var renderer = new SpyRenderer();

        renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(null!, null!));
        Assert.True(renderer.ButtonDrawn);

        renderer.DrawArrow(new ToolStripArrowRenderEventArgs(null!, null!, new Rectangle(0, 0, 8, 8), Color.Black, ArrowDirection.Down));
        Assert.True(renderer.ArrowDrawn);
    }

    [Fact]
    public void ToolStripManager_RendererAndRenderMode()
    {
        // Default renderer is professional.
        Assert.IsType<ToolStripProfessionalRenderer>(ToolStripManager.Renderer);

        ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
        Assert.IsType<ToolStripSystemRenderer>(ToolStripManager.Renderer);

        var custom = new SpyRenderer();
        ToolStripManager.Renderer = custom;
        Assert.Same(custom, ToolStripManager.Renderer);
        Assert.Equal(ToolStripManagerRenderMode.Custom, ToolStripManager.RenderMode);

        // restore default so other tests see a clean state
        ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
    }

    [Fact]
    public void ToolStripProfessionalRenderer_ExposesColorTable()
    {
        var table = new ProfessionalColorTable();
        var renderer = new ToolStripProfessionalRenderer(table);
        Assert.Same(table, renderer.ColorTable);

        Assert.NotNull(new ToolStripProfessionalRenderer().ColorTable);
    }

    [Fact]
    public void RenderEventArgs_RoundTripConstructorArgs()
    {
        var arrowRect = new Rectangle(1, 2, 3, 4);
        var arrow = new ToolStripArrowRenderEventArgs(null!, null!, arrowRect, Color.Red, ArrowDirection.Left);
        Assert.Equal(arrowRect, arrow.ArrowRectangle);
        Assert.Equal(Color.Red, arrow.ArrowColor);
        Assert.Equal(ArrowDirection.Left, arrow.Direction);

        var textRect = new Rectangle(5, 6, 7, 8);
        var text = new ToolStripItemTextRenderEventArgs(null!, null!, "hello", textRect, Color.Blue,
            gtk::System.Drawing.SystemFonts.DefaultFont, TextFormatFlags.Right);
        Assert.Equal("hello", text.Text);
        Assert.Equal(textRect, text.TextRectangle);
        Assert.Equal(Color.Blue, text.TextColor);
        Assert.Equal(TextFormatFlags.Right, text.TextFormat);

        var render = new ToolStripRenderEventArgs(null!, null!, new Rectangle(0, 0, 10, 20), Color.Green);
        Assert.Equal(new Rectangle(0, 0, 10, 20), render.AffectedBounds);
        Assert.Equal(Color.Green, render.BackColor);
    }
}

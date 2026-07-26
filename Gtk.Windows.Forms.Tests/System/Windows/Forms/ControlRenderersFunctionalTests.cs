namespace System.Windows.Forms;

extern alias gtk;

using System.Windows.Forms.VisualStyles;
using Size = global::System.Drawing.Size;
using Point = global::System.Drawing.Point;
using Rectangle = global::System.Drawing.Rectangle;

/// <summary>
/// Verifies the visual-style control renderers (<see cref="ButtonRenderer" />, <see cref="CheckBoxRenderer" />
/// and the rest). The size/support constants are pure values (no GTK); a smoke test then draws with each
/// renderer into an off-screen bitmap inside a realized form to confirm the rendering paths execute.
/// </summary>
[Collection("NonParallelCollection")]
public class ControlRenderersFunctionalTests
{
    [Fact]
    public void SupportAndSizeConstants()
    {
        Assert.True(ComboBoxRenderer.IsSupported);
        Assert.True(TextBoxRenderer.IsSupported);
        Assert.True(TabRenderer.IsSupported);
        Assert.True(ScrollBarRenderer.IsSupported);
        Assert.True(TrackBarRenderer.IsSupported);
        Assert.True(ProgressBarRenderer.IsSupported);
        Assert.True(ButtonRenderer.RenderMatchingApplicationState);

        Assert.Equal(new Size(13, 13), CheckBoxRenderer.GetGlyphSize(null!, CheckBoxState.CheckedNormal));
        Assert.Equal(new Size(13, 13), RadioButtonRenderer.GetGlyphSize(null!, RadioButtonState.CheckedNormal));
        Assert.Equal(6, ProgressBarRenderer.ChunkThickness);
        Assert.Equal(2, ProgressBarRenderer.ChunkSpaceThickness);
        Assert.Equal(new Size(16, 16), ScrollBarRenderer.GetThumbGripSize(null!, ScrollBarState.Normal));
        Assert.Equal(new Size(21, 11), TrackBarRenderer.GetTopPointingThumbSize(null!, TrackBarThumbState.Normal));

        Assert.False(ButtonRenderer.IsBackgroundPartiallyTransparent(PushButtonState.Normal));
        Assert.False(CheckBoxRenderer.IsBackgroundPartiallyTransparent(CheckBoxState.CheckedNormal));
    }

    [Fact]
    public void Renderers_DrawWithoutThrowing()
    {
        var form = new Form();

        form.Load += (s, e) =>
        {
            try
            {
                using var bmp = new gtk::System.Drawing.Bitmap(64, 64);
                using var g = gtk::System.Drawing.Graphics.FromImage(bmp);
                var font = gtk::System.Drawing.SystemFonts.DefaultFont;
                var r = new Rectangle(2, 2, 60, 24);

                // A representative call on each renderer must execute against a real Graphics.
                ButtonRenderer.DrawButton(g, r, "OK", font, false, PushButtonState.Normal);
                CheckBoxRenderer.DrawCheckBox(g, new Point(2, 2), CheckBoxState.CheckedNormal);
                RadioButtonRenderer.DrawRadioButton(g, new Point(2, 30), RadioButtonState.CheckedNormal);
                GroupBoxRenderer.DrawGroupBox(g, r, "Group", font, GroupBoxState.Normal);
                ComboBoxRenderer.DrawTextBox(g, r, ComboBoxState.Normal);
                ComboBoxRenderer.DrawDropDownButton(g, new Rectangle(40, 2, 16, 16), ComboBoxState.Normal);
                TextBoxRenderer.DrawTextBox(g, r, TextBoxState.Normal);
                TabRenderer.DrawTabItem(g, r, "Tab", font, TabItemState.Selected);
                ProgressBarRenderer.DrawHorizontalBar(g, r);
                ProgressBarRenderer.DrawHorizontalChunks(g, new Rectangle(2, 2, 30, 20));
                ScrollBarRenderer.DrawArrowButton(g, new Rectangle(0, 0, 16, 16), ScrollBarArrowButtonState.UpNormal);
                ScrollBarRenderer.DrawVerticalThumb(g, new Rectangle(0, 16, 16, 30), ScrollBarState.Normal);
                TrackBarRenderer.DrawHorizontalTrack(g, r);
                TrackBarRenderer.DrawHorizontalThumb(g, new Rectangle(20, 2, 11, 21), TrackBarThumbState.Normal);
                TrackBarRenderer.DrawHorizontalTicks(g, r, 5, EdgeStyle.Etched);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}

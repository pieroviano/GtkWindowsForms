namespace System.Windows.Forms;

extern alias gtk;

using System.Windows.Forms.VisualStyles;
using Rectangle = global::System.Drawing.Rectangle;
using Point = global::System.Drawing.Point;

/// <summary>
/// Verifies the VisualStyles catalogue and renderer: the <see cref="VisualStyleElement" /> element hierarchy
/// (class/part/state metadata), and <see cref="VisualStyleRenderer" /> parameter tracking, query defaults
/// and drawing. The metadata checks are GTK-free; a smoke test then draws with the renderer into an
/// off-screen bitmap inside a realized form.
/// </summary>
[Collection("NonParallelCollection")]
public class VisualStyleFunctionalTests
{
    [Fact]
    public void ElementCatalogue_ExposesClassPartState()
    {
        var normal = VisualStyleElement.Button.PushButton.Normal;
        Assert.NotNull(normal);
        Assert.Equal("BUTTON", normal.ClassName);
        Assert.Equal(1, normal.Part);
        Assert.Equal(1, normal.State);

        // a later part/state has distinct indices
        var comboHot = VisualStyleElement.ComboBox.DropDownButton.Hot;
        Assert.Equal("COMBOBOX", comboHot.ClassName);
        Assert.Equal(2, comboHot.State);

        // TextBox maps to the EDIT class, as in Windows themes
        Assert.Equal("EDIT", VisualStyleElement.TextBox.TextEdit.Normal.ClassName);

        var created = VisualStyleElement.CreateElement("MYCLASS", 5, 7);
        Assert.Equal("MYCLASS", created.ClassName);
        Assert.Equal(5, created.Part);
        Assert.Equal(7, created.State);
    }

    [Fact]
    public void Renderer_TracksParametersAndReturnsDefaults()
    {
        var r = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Pressed);
        Assert.Equal("BUTTON", r.Class);
        Assert.Equal(1, r.Part);
        Assert.Equal(3, r.State);
        Assert.Equal(IntPtr.Zero, r.Handle);
        Assert.Equal(0, r.LastHResult);

        Assert.True(VisualStyleRenderer.IsSupported);
        Assert.True(VisualStyleRenderer.IsElementDefined(VisualStyleElement.Menu.Item.Normal));

        r.SetParameters("EDIT", 1, 2);
        Assert.Equal("EDIT", r.Class);
        Assert.Equal(2, r.State);

        // query defaults
        Assert.False(r.GetBoolean(BooleanProperty.Transparent));
        Assert.Equal(0, r.GetInteger(IntegerProperty.ImageCount));
        Assert.False(r.GetColor(ColorProperty.TextColor).IsEmpty);
        Assert.False(r.IsBackgroundPartiallyTransparent());
    }

    [Fact]
    public void Renderer_DrawsWithoutThrowing()
    {
        var form = new Form();

        form.Load += (s, e) =>
        {
            try
            {
                using var bmp = new gtk::System.Drawing.Bitmap(64, 32);
                using var g = gtk::System.Drawing.Graphics.FromImage(bmp);
                var bounds = new Rectangle(1, 1, 60, 28);

                var r = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
                r.DrawBackground(g, bounds);
                r.DrawText(g, bounds, "Hi");
                var interior = r.DrawEdge(g, bounds, Edges.Left | Edges.Top | Edges.Right | Edges.Bottom, EdgeStyle.Raised, EdgeEffects.None);
                Assert.True(interior.Width < bounds.Width);

                Assert.Equal(bounds, r.GetBackgroundContentRectangle(g, bounds));
                Assert.Equal(HitTestCode.Nowhere, r.HitTestBackground(g, bounds, new Point(5, 5), HitTestOptions.BackgroundSegment));
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

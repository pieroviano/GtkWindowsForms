namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// The WinForms custom-paint path must be reachable on Gtk4.
/// </summary>
/// <remarks>
/// The port had disabled it wholesale, on every widget, with the comment "Gtk 4: Widget.Drawn is gone and
/// there is no overridable OnSnapshot; custom paint moves to CSS (deferred)". The premise was wrong -
/// GtkSharp does generate <c>protected virtual void OnSnapshot</c>, and <c>Snapshot.AppendCairo</c> yields
/// a Cairo context - so <see cref="Control.BackgroundImage" /> was never painted and no <c>Paint</c>
/// handler ever ran. Comparing snapshots of the sample on both engines is what exposed it.
/// </remarks>
[Collection("NonParallelCollection")]
public class CustomPaintPathTests
{
    // Constructing these controls builds GTK widgets, which needs the type system initialised.
    public CustomPaintPathTests() => Application.Init();

    [Fact]
    public void EveryPaintingWidget_OverridesOnSnapshot()
    {
        // The bridge has to exist on each widget that owns a drawing override; one missing override is one
        // control type whose BackgroundImage and Paint silently do nothing.
        var widgets = new[]
        {
            // Fully qualified: inside namespace System.Windows.Forms the short names would bind to the
            // WinForms classes of the same name, not the GTK widgets that do the drawing.
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.ButtonBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.BoxBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.LabelBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.LayoutBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.LinkLabelBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.TabControlBase),
            typeof(GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase),
        };

        var missing = widgets.Where(t => !DeclaresSnapshotOverride(t)).Select(t => t.Name).ToList();

        Assert.True(missing.Count == 0,
            "these widgets do not bridge snapshot to the paint path: " + string.Join(", ", missing));
    }

    [Fact]
    public void AssigningABackgroundImage_ReachesTheDrawingOverride()
    {
        // Control.BackgroundImage has to land on the override the snapshot bridge reads from, or the bridge
        // has nothing to draw.
        using var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 4, 4);
        var image = new gtk::System.Drawing.Bitmap(4, 4) { Pixbuf = pixbuf };

        var button = new Button { BackgroundImage = image };

        Assert.NotNull(button.self.Override.BackgroundImage);
        Assert.Same(image, button.self.Override.BackgroundImage);
    }

    [Fact]
    public void SubscribingToPaint_ReachesTheDrawingOverride()
    {
        var panel = new Panel();
        var handled = 0;

        panel.Paint += (_, _) => handled++;

        // The override is what the snapshot bridge invokes; if the event never reaches it, Paint is dead.
        Assert.True(panel.self.Override.HasPaintSubscribers,
            "a Paint subscriber must be visible to the drawing override");
        Assert.Equal(0, handled);
    }

    [Fact]
    public void BackgroundImageCssUri_IsAbsolute()
    {
        // Gtk3 emitted a relative url() and resolved it against the process working directory. Gtk4 loads
        // this stylesheet from a string, which has no base URI, so a relative url() resolves to nothing and
        // the image silently never appeared - on every form and panel background in the sample.
        var uri = Control.ToCssUri(Path.Combine("Resources", "x", "y_bgimage.png"));

        Assert.StartsWith("file:///", uri, StringComparison.Ordinal);
        Assert.DoesNotContain("\\", uri);
        Assert.EndsWith("y_bgimage.png", uri, StringComparison.Ordinal);
    }

    private static bool DeclaresSnapshotOverride(Type type)
    {
        return type.GetMethod(
            "OnSnapshot",
            Reflection.BindingFlags.Instance | Reflection.BindingFlags.NonPublic
            | Reflection.BindingFlags.Public | Reflection.BindingFlags.DeclaredOnly) != null;
    }
}

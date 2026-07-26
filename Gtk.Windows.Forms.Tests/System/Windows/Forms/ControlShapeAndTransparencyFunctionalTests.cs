extern alias gtk;
using System.Drawing;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using Region = gtk::System.Drawing.Region;
using GraphicsPath = gtk::System.Drawing.Drawing2D.GraphicsPath;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for non-rectangular control shapes (<see cref="Control.Region" />) and colour-keyed
/// window transparency (<c>Form.TransparencyKey</c>).
/// <para>
/// <see cref="Control.Region" /> existed as an inert auto-property: it stored a region and shaped nothing,
/// so a round button or a skinned splash form compiled, ran and stayed rectangular - the silent-failure
/// shape of gap. <c>TransparencyKey</c> and <c>AllowTransparency</c> did not exist at all, so the standard
/// shaped-window idiom did not compile.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class ControlShapeAndTransparencyFunctionalTests
{
    /// <summary>
    /// Runs <paramref name="body" /> once <paramref name="form" /> is up, so the widgets under test have
    /// real GDK handles - a shape cannot be applied to an unrealized widget.
    /// </summary>
    private static void WhenShown(Form form, Action body)
    {
        ExceptionDispatchInfo? failure = null;

        form.Load += (_, _) =>
        {
            try
            {
                body();
            }
            catch (Exception ex)
            {
                failure = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        failure?.Throw();
    }

    private static Region EllipticalRegion(int size)
    {
        var path = new GraphicsPath();
        path.AddEllipse(0, 0, size, size);

        return new Region(path);
    }

    [Fact]
    public void Region_RoundTrips()
    {
        using var button = new Button();
        var region = new Region(new Rectangle(0, 0, 10, 10));

        button.Region = region;

        Assert.Same(region, button.Region);
    }

    [Fact]
    public void Region_DefaultsToNull()
    {
        using var button = new Button();

        Assert.Null(button.Region);
    }

    [Fact]
    public void Region_RaisesRegionChangedOnceForEachRealChange()
    {
        using var button = new Button();
        var raised = 0;
        button.RegionChanged += (_, _) => raised++;
        var region = new Region(new Rectangle(0, 0, 10, 10));

        button.Region = region;
        button.Region = region; // same instance - not a change
        button.Region = null;

        Assert.Equal(2, raised);
    }

    [Fact]
    public void Region_AssignedBeforeTheHandleExists_ShapesTheWindowOnceRealized()
    {
        using var form = new Form { Width = 200, Height = 200 };

        // Assigned while unrealized: the shape has to be remembered and applied at realize time.
        form.Region = EllipticalRegion(200);

        WhenShown(form, () =>
        {
            var window = form.Widget?.Window;
            Assert.NotNull(window);
            Assert.True(window!.IsShaped, "the form's GDK window must carry the shape");
        });
    }

    [Fact]
    public void Region_AssignedAfterRealize_ShapesTheWindow()
    {
        using var form = new Form { Width = 200, Height = 200 };

        WhenShown(form, () =>
        {
            var window = form.Widget?.Window;
            Assert.NotNull(window);
            Assert.False(window!.IsShaped, "nothing has been shaped yet");

            form.Region = EllipticalRegion(200);

            Assert.True(window.IsShaped);
        });
    }

    [Fact]
    public void Region_SetToNull_ClearsTheShape()
    {
        using var form = new Form { Width = 200, Height = 200 };

        WhenShown(form, () =>
        {
            form.Region = EllipticalRegion(200);
            Assert.True(form.Widget!.Window.IsShaped);

            form.Region = null;

            Assert.False(form.Widget!.Window.IsShaped, "a null region means the whole rectangle again");
        });
    }

    [Fact]
    public void Region_OnAWindowlessWidget_IsStoredAndHarmless()
    {
        // A GTK button draws onto its parent and owns no GDK window, so there is nothing to shape. The
        // documented behaviour is that the region round-trips and nothing throws - not that it is refused.
        using var form = new Form { Width = 200, Height = 200 };
        using var button = new Button { Width = 60, Height = 60 };
        form.Controls.Add(button);
        var region = EllipticalRegion(60);

        WhenShown(form, () =>
        {
            var exception = Record.Exception(() => button.Region = region);

            Assert.Null(exception);
            Assert.Same(region, button.Region);
        });
    }

    [Fact]
    public void TransparencyKey_DefaultsToEmptyAndOpaque()
    {
        using var form = new Form();

        Assert.Equal(Color.Empty, form.TransparencyKey);
        Assert.False(form.AllowTransparency);
    }

    [Fact]
    public void TransparencyKey_RoundTripsAndTurnsAllowTransparencyOn()
    {
        using var form = new Form();

        form.TransparencyKey = Color.Magenta;

        Assert.Equal(Color.Magenta, form.TransparencyKey);
        Assert.True(form.AllowTransparency, "assigning a key implies transparency is allowed");
    }

    [Fact]
    public void AllowTransparency_TurnedOff_ClearsTheKey()
    {
        using var form = new Form { TransparencyKey = Color.Magenta };

        form.AllowTransparency = false;

        Assert.Equal(Color.Empty, form.TransparencyKey);
    }

    [Fact]
    public void TransparencyKey_ReachesThePaintPath()
    {
        using var form = new Form { BackColor = Color.Magenta };

        form.TransparencyKey = Color.Magenta;

        // The background painter is what turns the key into transparency; if the key never reaches it the
        // form reports a key it does not honour.
        Assert.Equal(Color.Magenta, form.self.Override.TransparencyKey);

        form.TransparencyKey = Color.Empty;

        Assert.Null(form.self.Override.TransparencyKey);
    }

    [Fact]
    public void TransparencyKey_ClearedWhenTransparencyIsDisallowed()
    {
        using var form = new Form { BackColor = Color.Magenta, TransparencyKey = Color.Magenta };

        form.AllowTransparency = false;

        Assert.Null(form.self.Override.TransparencyKey);
    }
}

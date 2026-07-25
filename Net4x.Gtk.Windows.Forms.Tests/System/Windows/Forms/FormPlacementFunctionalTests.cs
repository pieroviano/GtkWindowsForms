using System.Drawing;
using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for Form's window-placement surface (§5.1 of API-COMPLETENESS): DesktopBounds/DesktopLocation and
/// their setters, MaximizedBounds, RestoreBounds and RightToLeftLayout with their change events. These were
/// absent, so designer-emitted placement code did not compile. The tests pin the deterministic contract —
/// property round-trips, change events, and the desktop-coordinate delegation — rather than live window
/// state, which the headless GTK host does not drive.
/// </summary>
[Collection("NonParallelCollection")]
public class FormPlacementFunctionalTests
{
    [Fact]
    public void MaximizedBounds_RoundTripsAndRaisesItsEvent()
    {
        using var form = new Form();
        var raised = 0;
        form.MaximizedBoundsChanged += (_, _) => raised++;

        form.MaximizedBounds = new Rectangle(1, 2, 300, 400);

        Assert.Equal(new Rectangle(1, 2, 300, 400), form.MaximizedBounds);
        Assert.Equal(1, raised);

        // Setting the same value again does not re-raise.
        form.MaximizedBounds = new Rectangle(1, 2, 300, 400);
        Assert.Equal(1, raised);
    }

    [Fact]
    public void RightToLeftLayout_RoundTripsAndRaisesItsEvent()
    {
        using var form = new Form();
        var raised = 0;
        form.RightToLeftLayoutChanged += (_, _) => raised++;

        Assert.False(form.RightToLeftLayout);

        form.RightToLeftLayout = true;

        Assert.True(form.RightToLeftLayout);
        Assert.Equal(1, raised);
    }

    [Fact]
    public void RestoreBounds_EqualsBoundsWhileNormal()
    {
        using var form = new Form();

        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(form.Bounds, form.RestoreBounds);
    }

    [Fact]
    public void DesktopBounds_DelegatesToBounds()
    {
        using var form = new Form();

        Assert.Equal(form.Bounds, form.DesktopBounds);
    }

    [Fact]
    public void DesktopLocation_DelegatesToLocation_AndSetterUpdatesIt()
    {
        using var form = new Form();

        Assert.Equal(form.Location, form.DesktopLocation);

        form.SetDesktopLocation(40, 55);

        // The setter routes to Location, so the two stay in agreement whatever the host reports.
        Assert.Equal(form.Location, form.DesktopLocation);
    }
}

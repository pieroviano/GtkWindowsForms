using System.ComponentModel;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Tests for <see cref="BindableComponent"/> (§4 of API-COMPLETENESS): the base for a non-control component
/// that supports data binding. It was absent, so a bindable non-control component could not be written
/// against the real base. The tests pin that it is a Component and IBindableComponent, exposes a non-null
/// DataBindings/BindingContext, and raises BindingContextChanged on assignment.
/// </summary>
[Collection("NonParallelCollection")]
public class BindableComponentFunctionalTests
{
    static BindableComponentFunctionalTests()
    {
        using var _ = new Form();
    }

    private sealed class Probe : BindableComponent
    {
    }

    [Fact]
    public void BindableComponent_IsAComponentAndBindableComponent()
    {
        using var probe = new Probe();

        Assert.IsAssignableFrom<Component>(probe);
        Assert.IsAssignableFrom<IBindableComponent>(probe);
    }

    [Fact]
    public void DataBindings_IsNeverNull()
    {
        using var probe = new Probe();

        Assert.NotNull(probe.DataBindings);
        // Same instance each access.
        Assert.Same(probe.DataBindings, probe.DataBindings);
    }

    [Fact]
    public void BindingContext_RoundTripsAndRaisesItsEvent()
    {
        using var probe = new Probe();
        var raised = 0;
        probe.BindingContextChanged += (_, _) => raised++;

        Assert.NotNull(probe.BindingContext);

        var context = new BindingContext();
        probe.BindingContext = context;

        Assert.Same(context, probe.BindingContext);
        Assert.True(raised >= 1);
    }

    [Fact]
    public void BindingContext_ReachableThroughTheInterface()
    {
        using var probe = new Probe();
        IBindableComponent bindable = probe;

        Assert.NotNull(bindable.DataBindings);
        Assert.NotNull(bindable.BindingContext);
    }
}

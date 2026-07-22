using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for <see cref="Control" />'s disposal contract.
/// <para>
/// <c>Dispose()</c> was declared <c>new</c>, shadowing <see cref="global::System.ComponentModel.Component.Dispose()" />.
/// That made disposal both non-idempotent (the shadow ran <c>Dispose(true)</c> and then
/// <c>base.Dispose()</c>, which ran it again) and path-dependent: everything reaching the control through
/// <see cref="IDisposable" /> or <c>Component</c> - container teardown and the <c>components.Dispose()</c>
/// in generated Designer code - missed the shadow and left <c>IsDisposed</c> false.
/// </para>
/// <para>
/// <c>Dispose(bool)</c> is still *entered* once per call, exactly as in WinForms, where the disposed-guard
/// also lives inside it. What must happen only once is the work: releasing the resources and walking the
/// children. These tests assert that effect rather than the entry count.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class ControlDisposalFunctionalTests
{
    /// <summary>
    /// Counts how many times the disposal walk did real work on this control - that is, how often it was
    /// entered while not already disposed.
    /// </summary>
    private sealed class CountingControl : Control
    {
        public int EffectiveDisposals { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                EffectiveDisposals++;
            }

            base.Dispose(disposing);
        }
    }

    [Fact]
    public void Dispose_ThroughIDisposable_MarksTheControlDisposed()
    {
        var control = new Control();

        // The path generated Designer code and container teardown actually take.
        ((IDisposable)control).Dispose();

        Assert.True(control.IsDisposed,
            "IsDisposed must be set no matter which Dispose entry point the caller used");
    }

    [Fact]
    public void Dispose_ThroughComponent_MarksTheControlDisposed()
    {
        global::System.ComponentModel.Component control = new Control();

        control.Dispose();

        Assert.True(((Control)control).IsDisposed);
    }

    [Fact]
    public void Dispose_Directly_MarksTheControlDisposed()
    {
        var control = new Control();

        control.Dispose();

        Assert.True(control.IsDisposed);
    }

    [Fact]
    public void Dispose_DoesTheWorkOnlyOnce()
    {
        var control = new CountingControl();

        control.Dispose();
        control.Dispose();

        Assert.Equal(1, control.EffectiveDisposals);
    }

    [Fact]
    public void Dispose_WalksChildrenExactlyOnce()
    {
        var parent = new Control();
        var child = new CountingControl();
        parent.Controls!.Add(child);

        parent.Dispose();

        Assert.Equal(1, child.EffectiveDisposals);
        Assert.True(child.IsDisposed, "a child reached by the parent's walk must be marked disposed too");
    }

    [Fact]
    public void Dispose_OfAChildAlreadyDisposed_DoesNotRunTwiceWhenTheParentGoes()
    {
        var parent = new Control();
        var child = new CountingControl();
        parent.Controls!.Add(child);

        // Disposing a child explicitly and then disposing the parent is ordinary teardown ordering.
        child.Dispose();
        parent.Dispose();

        Assert.Equal(1, child.EffectiveDisposals);
    }

    [Fact]
    public void Dispose_ClearsTheChildCollection()
    {
        var parent = new Control();
        parent.Controls!.Add(new Control());
        parent.Controls!.Add(new Control());

        parent.Dispose();

        Assert.Empty(parent.Controls!);
    }

    [Fact]
    public void Dispose_OfADeepTree_DisposesEveryDescendantOnce()
    {
        var root = new Control();
        var middle = new CountingControl();
        var leaf = new CountingControl();
        root.Controls!.Add(middle);
        middle.Controls!.Add(leaf);

        root.Dispose();

        Assert.Equal(1, middle.EffectiveDisposals);
        Assert.Equal(1, leaf.EffectiveDisposals);
    }
}

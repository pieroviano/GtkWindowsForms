using System.Windows.Forms;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the three <see cref="Control" /> methods that used to compile, run and do
/// nothing: <see cref="Control.BringToFront" />, <see cref="Control.SendToBack" /> and
/// <see cref="Control.CreateControl" />.
/// <para>
/// The oracle for the two z-order methods is the parent's child order - what
/// <c>Controls.GetChildIndex</c> reports - because that is the half of the WinForms contract observable
/// without a display. The GDK raise/lower they also perform needs a realized window and is out of reach
/// here; <c>GtkWinFormsApp.UITests</c> is where rendering order is covered.
/// </para>
/// </summary>
[Collection("NonParallelCollection")]
public class ControlZOrderFunctionalTests
{
    private static (Control Parent, Control First, Control Second, Control Third) ThreeChildren()
    {
        var parent = new Control();
        var first = new Control();
        var second = new Control();
        var third = new Control();
        parent.Controls!.Add(first);
        parent.Controls!.Add(second);
        parent.Controls!.Add(third);

        return (parent, first, second, third);
    }

    [Fact]
    public void BringToFront_MovesTheControlToTheFrontOfItsSiblings()
    {
        var (parent, first, second, third) = ThreeChildren();

        third.BringToFront();

        Assert.Equal(0, parent.Controls!.GetChildIndex(third));
        Assert.Equal(1, parent.Controls!.GetChildIndex(first));
        Assert.Equal(2, parent.Controls!.GetChildIndex(second));
    }

    [Fact]
    public void BringToFront_OnAControlAlreadyInFront_ChangesNothing()
    {
        var (parent, first, second, third) = ThreeChildren();

        first.BringToFront();

        Assert.Equal(0, parent.Controls!.GetChildIndex(first));
        Assert.Equal(1, parent.Controls!.GetChildIndex(second));
        Assert.Equal(2, parent.Controls!.GetChildIndex(third));
    }

    [Fact]
    public void SendToBack_MovesTheControlBehindItsSiblings()
    {
        var (parent, first, second, third) = ThreeChildren();

        first.SendToBack();

        Assert.Equal(2, parent.Controls!.GetChildIndex(first));
        Assert.Equal(0, parent.Controls!.GetChildIndex(second));
        Assert.Equal(1, parent.Controls!.GetChildIndex(third));
    }

    [Fact]
    public void BringToFront_ThenSendToBack_ReturnsTheControlToTheEnd()
    {
        var (parent, _, _, third) = ThreeChildren();

        third.BringToFront();
        third.SendToBack();

        Assert.Equal(2, parent.Controls!.GetChildIndex(third));
    }

    [Fact]
    public void BringToFront_WithNoParent_DoesNotThrow()
    {
        var orphan = new Control();

        orphan.BringToFront();
        orphan.SendToBack();
    }

    [Fact]
    public void CreateControl_MarksTheControlCreated()
    {
        var control = new Control();
        Assert.False(control.Created, "a control nobody has created or shown is not created yet");

        control.CreateControl();

        Assert.True(control.Created);
    }

    [Fact]
    public void CreateControl_CreatesTheChildrenToo()
    {
        var parent = new Control();
        var child = new Control();
        var grandchild = new Control();
        parent.Controls!.Add(child);
        child.Controls!.Add(grandchild);

        parent.CreateControl();

        Assert.True(child.Created, "WinForms creates the whole child tree, not just the control asked");
        Assert.True(grandchild.Created);
    }

    [Fact]
    public void CreateControl_IsIdempotent()
    {
        var control = new Control();

        control.CreateControl();
        control.CreateControl();

        Assert.True(control.Created);
    }
}

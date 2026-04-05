#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace System.Windows.Forms.ControlTests;

[Collection("NonParallelCollection")]
public class ContainerControlTests
{
    [Fact]
    public void ActivateControl_NoGtkWidget_ReturnsFalse()
    {
        var container = new ContainerControl();
        var child = new Label();

        var result = container.ActivateControl(child);

        Assert.False(result);
        Assert.Equal(child, container.ActiveControl);
    }

    [Fact]
    public void ActivateControl_Null_ReturnsFalse()
    {
        var container = new ContainerControl();

        var result = container.ActivateControl(null);

        Assert.False(result);
        Assert.Null(container.ActiveControl);
    }

    [Fact]
    public void ActivateControl_SetsActiveControl()
    {
        var container = new ContainerControl();
        var child = new Label();

        container.ActivateControl(child);

        Assert.Equal(child, container.ActiveControl);
    }

    [Fact]
    public void ActiveControl_Default_IsNull()
    {
        var container = new ContainerControl();

        Assert.Null(container.ActiveControl);
    }

    [Fact]
    public void ActiveControl_ReturnsStored_WhenNoGtkFocus()
    {
        var container = new ContainerControl();
        var child = new Label();

        container.Controls.Add(child);
        container.ActiveControl = child;

        var result = container.ActiveControl;

        Assert.Equal(child, result);
    }

    [Fact]
    public void ActiveControl_SetNull_ReturnsNull()
    {
        var container = new ContainerControl
        {
            ActiveControl = null
        };

        Assert.Null(container.ActiveControl);
    }

    [Fact]
    public void ActiveControl_Setter_SetsValue()
    {
        var container = new ContainerControl();
        var child = new Label();

        container.ActiveControl = child;

        Assert.Equal(child, container.ActiveControl);
    }

    [Fact]
    public void ActiveControl_SwitchingControls_UpdatesCorrectly()
    {
        var container = new ContainerControl();
        var c1 = new Label();
        var c2 = new Label();

        container.Controls.Add(c1);
        container.Controls.Add(c2);

        container.ActiveControl = c1;
        Assert.Equal(c1, container.ActiveControl);

        container.ActiveControl = c2;
        Assert.Equal(c2, container.ActiveControl);
    }

    [Fact]
    public void StatusStrip_SetAndGet_Works()
    {
        var container = new ContainerControl();
        var strip = new StatusStrip();

        container.StatusStrip = strip;

        Assert.Equal(strip, container.StatusStrip);
    }

    private class TestControl : Label
    {
        public bool ActivateCalled { get; private set; }

        public override bool Focus()
        {
            ActivateCalled = true;
            return true;
        }
    }
}
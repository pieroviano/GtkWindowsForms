using System.ComponentModel;
using System.Windows.Forms;

namespace GtkWinForms.Tests.System.Windows.Forms;

/// <summary>
/// Behavioural tests for the data-source side of change notification, flagged "still unestablished" in
/// EMULATION-GAPS §5.6: when a source implements <see cref="INotifyPropertyChanged"/> and raises
/// <c>PropertyChanged</c>, a bound control must refresh automatically without an explicit
/// <see cref="Binding.ReadValue"/>. This is the everyday one-way-to-target flow behind observable models.
/// </summary>
[Collection("NonParallelCollection")]
public class InpcBindingFunctionalTests
{
    static InpcBindingFunctionalTests()
    {
        // Seed GTK so constructing a TextBox in isolation does not hit the control-before-Form crash.
        using var _ = new Form();
    }

    private sealed class Observable : INotifyPropertyChanged
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [Fact]
    public void RaisingPropertyChangedOnTheSourceRefreshesTheControl()
    {
        using var box = new TextBox();
        var model = new Observable { Name = "initial" };
        box.DataBindings.Add(new Binding("Text", model, "Name", true));

        // Establish the initial pull.
        box.DataBindings[0].ReadValue();
        Assert.Equal("initial", box.Text);

        // No explicit ReadValue: the source-side notification must drive the refresh.
        model.Name = "changed";

        Assert.Equal("changed", box.Text);
    }

    [Fact]
    public void SubsequentSourceChangesKeepFlowingToTheControl()
    {
        using var box = new TextBox();
        var model = new Observable { Name = "a" };
        box.DataBindings.Add(new Binding("Text", model, "Name", true));
        box.DataBindings[0].ReadValue();

        model.Name = "b";
        Assert.Equal("b", box.Text);

        model.Name = "c";
        Assert.Equal("c", box.Text);
    }
}

using System.Reflection;
using System.Threading;
using Gtk.Windows.Forms.InternetExplorer;

namespace GtkWinForms.Tests.InternetExplorer;

/// <summary>
/// Tests the dedicated STA apartment used to host the control when the GTK thread is not an STA. It must
/// genuinely be an STA and must surface the exceptions of the work it runs.
/// </summary>
public class StaApartmentTests
{
    [Fact]
    public void Invoke_Runs_The_Work_On_An_Sta_Thread()
    {
        using var apartment = new StaApartment();

        var state = apartment.Invoke(() => Thread.CurrentThread.GetApartmentState());

        Assert.Equal(ApartmentState.STA, state);
    }

    [Fact]
    public void Invoke_Returns_The_Functions_Result()
    {
        using var apartment = new StaApartment();

        Assert.Equal(42, apartment.Invoke(() => 42));
    }

    [Fact]
    public void Invoke_Propagates_The_Works_Exception()
    {
        using var apartment = new StaApartment();

        Assert.Throws<TargetInvocationException>(() => apartment.Invoke(() => throw new InvalidOperationException()));
    }
}

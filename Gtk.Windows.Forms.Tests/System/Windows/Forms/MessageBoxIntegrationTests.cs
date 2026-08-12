using Mocks;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class MessageBoxIntegrationTests
{
    [Fact]
    public void MessageBox_Show_LogsMessage_WhenFormRuns()
    {
        // Arrange
        var mockMessageBox = MockMessageBox.ReplaceMessageBoxWithMock();

        // Call MessageBox.Show from a form's Load handler - the "message box shown while a form is running"
        // scenario - and drive it with ShowDialog's short, scoped modal loop.
        //
        // The original test used Application.Run(form), whose GLOBAL Gtk main loop runs until Application
        // .Exit. In the shared multi-thousand-test process that loop pumps GLib sources left behind by
        // earlier tests (timers referencing torn-down windows), which crashes the host
        // (gtk_widget_get_realized: GTK_IS_WIDGET failed / 0xC0000005). ShowDialog closed immediately from
        // Load runs only a brief nested loop - like every other modal test in the suite - so it does not sit
        // pumping that leftover state. The mock makes the logged message deterministic.
        var form = new Form();
        form.Load += (s, e) =>
        {
            MessageBox.Show("Test message");
            form.Close();
        };

        // Act
        form.ShowDialog();

        // Assert
        Assert.Contains(mockMessageBox.LoggedMessages, msg => msg.Contains("Test message"));
    }
}

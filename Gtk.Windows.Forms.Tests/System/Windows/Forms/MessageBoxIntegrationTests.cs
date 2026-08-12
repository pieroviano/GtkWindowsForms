using Mocks;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class MessageBoxIntegrationTests
{
    // Constructing/showing a Form needs GTK initialised; do it up front so this class is safe to run first.
    public MessageBoxIntegrationTests() => Application.Init();

    [Fact]
    public void MessageBox_Show_LogsMessage_WhenFormRuns()
    {
        // Arrange
        var mockMessageBox = MockMessageBox.ReplaceMessageBoxWithMock();

        var form = new Form();

        // Drive the sequence on the GTK main loop that Application.Run pumps (on THIS - the harness's - STA
        // thread), via GLib timeouts. The previous version spun a second STA thread AND used async/await +
        // Task.Delay in the Load handler; the continuations then resumed on a thread-pool thread and touched
        // GTK off its owning thread, racing with the rest of the suite and crashing the host
        // (gtk_window_present: GTK_IS_WINDOW failed ... 0xC0000005). Keeping everything on the one GTK thread
        // makes it deterministic.
        form.Load += (s, e) =>
        {
            GLib.Timeout.Add(200, () =>
            {
                MessageBox.Show("Test message");
                GLib.Timeout.Add(100, () =>
                {
                    Application.DontQuit = false;
                    Application.Exit();
                    Application.DontQuit = true;
                    return false;
                });

                return false;
            });
        };

        // Act
        Application.Run(form);

        // Assert
        Assert.Contains(mockMessageBox.LoggedMessages, msg => msg.Contains("Test message"));
    }
}

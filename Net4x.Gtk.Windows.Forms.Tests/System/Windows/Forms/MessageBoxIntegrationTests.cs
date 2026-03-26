using Mocks;

namespace System.Windows.Forms;

public class MessageBoxIntegrationTests
{
    [Fact]
    public void MessageBox_Show_LogsMessage_WhenFormRuns()
    {
        async Task OnLoadAsync(Form form) {
            await Task.Delay(1000); // Wait 1 second
            MessageBox.Show("Test message");
            await Task.Delay(500); // Wait 0.5 second
            form.Close();
        }

        // Arrange
        var mockMessageBox = MockMessageBox.ReplaceMessageBoxWithMock();

        // Act
        var thread=new Thread(() =>
        {
            var form = new Form();

            form.Load += async (s, e) => await OnLoadAsync(form);

            Application.Run(form);
        });
#pragma warning disable CA1416
        thread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
        thread.Start();
        thread.Join();

        // Assert
        Assert.Contains(mockMessageBox.LoggedMessages, msg => msg.Contains("Test message"));
    }
}

using System.Windows.Forms;
using AutoClosingMessageBoxApp;
using GLib;

namespace AutoClosingMessageBox.Tests
{
    public class TestAutoClosingMessageBox
    {
        [Fact]
        public void AutoClosingMessageBox_Works()
        {
            try
            {
#if DEBUG
                var p = System.Diagnostics.Process.Start(@"..\..\..\..\AutoClosingMessageBoxApp\bin\Debug\net10.0\AutoClosingMessageBoxApp.exe");
#else
                var p = System.Diagnostics.Process.Start(@"..\..\..\..\AutoClosingMessageBoxApp\bin\Release\net10.0\AutoClosingMessageBoxApp.exe");
#endif
                p.WaitForExit();
                Assert.Equal(0, p.ExitCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Fail();
            }
        }
    }
}

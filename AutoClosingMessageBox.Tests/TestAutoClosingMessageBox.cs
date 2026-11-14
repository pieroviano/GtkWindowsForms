using System.Windows.Forms;
using AutoClosingMessageBoxApp;
using GLib;

namespace AutoClosingMessageBox.Tests;

public class TestAutoClosingMessageBox
{
    [Fact]
    public void AutoClosingMessageBox_Works()
    {
        try
        {
            // The ProjectReference copies the app's apphost next to this assembly. Resolve it from there rather
            // than via a hard-coded "..\..\..\..\...\AutoClosingMessageBoxApp.exe": backslashes and the .exe
            // suffix are Windows-only, so Process.Start threw on Linux/macOS and the test always failed there.
            var appHost = Path.Combine(AppContext.BaseDirectory,
                OperatingSystem.IsWindows() ? "AutoClosingMessageBoxApp.exe" : "AutoClosingMessageBoxApp");
            using var p = System.Diagnostics.Process.Start(appHost)!;
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
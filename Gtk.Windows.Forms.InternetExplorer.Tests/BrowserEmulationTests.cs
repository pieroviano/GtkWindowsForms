using Gtk.Windows.Forms.InternetExplorer;

namespace GtkWinForms.Tests.InternetExplorer;

/// <summary>
/// Tests that the IE11 rendering opt-in writes the registry value the embedded control reads. Uses a
/// throwaway executable name under the current user's hive, then cleans it up, so it needs no privileges
/// and leaves nothing behind.
/// </summary>
public class BrowserEmulationTests
{
    [Fact]
    public void Write_Sets_The_Ie11_Emulation_Value_For_An_Executable()
    {
        var name = "gtkwf_ie_test_" + Guid.NewGuid().ToString("N") + ".exe";
        try
        {
            Assert.True(BrowserEmulation.Write(name, BrowserEmulation.Ie11EdgeMode));

            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(BrowserEmulation.FeatureSubKeyPath);
            Assert.NotNull(key);
            Assert.Equal(BrowserEmulation.Ie11EdgeMode, (int)key!.GetValue(name)!);
        }
        finally
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                BrowserEmulation.FeatureSubKeyPath, writable: true);
            key?.DeleteValue(name, throwOnMissingValue: false);
        }
    }

    [Fact]
    public void Write_Is_Idempotent_When_The_Value_Is_Already_Set()
    {
        var name = "gtkwf_ie_test_" + Guid.NewGuid().ToString("N") + ".exe";
        try
        {
            Assert.True(BrowserEmulation.Write(name, BrowserEmulation.Ie11EdgeMode));
            Assert.True(BrowserEmulation.Write(name, BrowserEmulation.Ie11EdgeMode));
        }
        finally
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                BrowserEmulation.FeatureSubKeyPath, writable: true);
            key?.DeleteValue(name, throwOnMissingValue: false);
        }
    }
}

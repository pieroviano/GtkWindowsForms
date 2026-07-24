using System.Globalization;
using System.Windows.Forms;

namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// One-time GTK/WinForms startup shared by every UI test. Mirrors the sample's own
/// <c>Program.Main</c> bootstrap so the forms behave exactly as when the app runs. The library's
/// <see cref="Application.Init()"/> is idempotent (guards on its internal <c>App == null</c>), but we also
/// gate here so the culture/theme pinning runs exactly once.
/// </summary>
internal static class UiApp
{
    /// <summary>How long a modal MessageBox lingers before the harness auto-dismisses it (ms).</summary>
    private const int AutoCloseMessageBoxMs = 300;

    private static readonly object Gate = new();
    private static bool _initialized;

    /// <summary>Initializes GTK + the WinForms shim on first call; a no-op afterwards.</summary>
    internal static void EnsureInitialized()
    {
        lock (Gate)
        {
            if (_initialized)
            {
                return;
            }

            // Pin culture so on-screen text and number/date formatting are reproducible between runs -
            // screenshots and text assertions depend on it (English, per SampleUITests-Plan.md §6/§9).
            var culture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            System.AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

            // Install the auto-closing MessageBox with a short timeout. Sample forms pop modal message
            // boxes the manual tester dismisses by hand (e.g. Form2's FormClosing/FormClosed); with a
            // positive timeout every MessageBox auto-dismisses, which is the automated equivalent of that
            // click and stops teardown from deadlocking on a dialog with no user.
            AutoClosingMessageBox.Install(AutoCloseMessageBoxMs);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Init();

            _initialized = true;
        }
    }
}

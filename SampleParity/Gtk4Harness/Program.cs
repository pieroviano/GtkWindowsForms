using System.Windows.Forms;
using GtkWinFormsApp;

namespace SampleParity;

/// <summary>
/// Drives this repo's Gtk4 sample (<c>GtkWinFormsApp</c>) through the same scenario catalogue as the
/// reference harness, so the two traces can be compared.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

        return HarnessMain.Run(args, new Gtk4SampleForms());
    }
}

/// <summary>Maps the catalogue's logical form names onto the Gtk4 sample's types.</summary>
internal sealed class Gtk4SampleForms : ISampleForms
{
    /// <summary>How long a modal message box lingers before dismissing itself.</summary>
    private const int AutoCloseMessageBoxMs = 300;

    /// <inheritdoc />
    public IEnumerable<string> Names => new[] { "Form1", "Form2", "Form3", "Form4", "Form5", "Test1" };

    /// <inheritdoc />
    /// <remarks>
    /// This side has the library's replaceable auto-closing MessageBox, which is the same mechanism the
    /// existing UI suite installs, so the dialog dismisses itself rather than needing to be reaped from a
    /// timer as on the Gtk3 side.
    /// </remarks>
    public void InstallModalGuard()
    {
        AutoClosingMessageBox.Install(AutoCloseMessageBoxMs);

        // AutoClosingMessageBox only covers the library's own MessageBox. Sample buttons also open plain
        // GtkDialogs - file and colour choosers, print dialogs - which nothing dismisses, and the harness
        // then waits forever for a user who is not there. The reference harness needs a generic reaper for
        // the same reason, so both sides sweep any modal toplevel that is not a form under test.
        GLib.Timeout.Add(ModalGuardIntervalMs, () =>
        {
            foreach (var window in Gtk.Window.ListToplevels())
            {
                if (window is Gtk.Dialog dialog && dialog.Visible)
                {
                    dialog.Destroy();
                }
            }

            return true;
        });
    }

    /// <summary>How often to look for a modal dialog to dismiss.</summary>
    private const uint ModalGuardIntervalMs = 250;

    /// <inheritdoc />
    public Form? Create(string logicalName)
    {
        return logicalName switch
        {
            "Form1" => new Form1(),
            "Form2" => new Form2(),
            "Form3" => new Form3(),
            "Form4" => new Form4(),
            "Form5" => new Form5(),
            "Test1" => new Test1(),
            _ => null,
        };
    }
}

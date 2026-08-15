using System.Windows.Forms;
using GTKWinFormsApp;

namespace SampleParity;

/// <summary>
/// Drives the reference sample - <c>gtksystem-windows-forms/Samples/GTKWinFormsApp</c>, which links
/// upstream's Gtk3 library - through the shared scenario catalogue.
/// </summary>
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // The sample's own Program.cs sets this before doing anything else.
        AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

        return HarnessMain.Run(args, new Gtk3SampleForms());
    }
}

/// <summary>Maps the catalogue's logical form names onto the reference sample's types.</summary>
internal sealed class Gtk3SampleForms : ISampleForms
{
    /// <summary>How often to look for a modal dialog to dismiss.</summary>
    private const uint ModalGuardIntervalMs = 250;

    /// <inheritdoc />
    public IEnumerable<string> Names => new[] { "Form1", "Form2", "Form3", "Form4", "Form5", "Test1" };

    /// <inheritdoc />
    /// <remarks>
    /// Upstream's <c>MessageBox</c> is a plain static class with no replaceable instance, and the submodule
    /// is vendored third-party code we do not modify - so the auto-closing MessageBox this repo ships is not
    /// available here. It blocks in <c>Gtk.Dialog.Run()</c>, which spins a nested main loop, and a GLib
    /// timeout still runs inside that loop. Dismissing the dialog from a timer is therefore the one lever
    /// that works without touching upstream.
    /// </remarks>
    public void InstallModalGuard()
    {
        GLib.Timeout.Add(ModalGuardIntervalMs, () =>
        {
            foreach (var window in Gtk.Window.ListToplevels())
            {
                if (window is Gtk.Dialog dialog && dialog.Visible)
                {
                    // Ok rather than Cancel: a handler that asks "really close?" should get the answer a
                    // tester clicking through would give, so teardown proceeds.
                    dialog.Respond(Gtk.ResponseType.Ok);
                }
            }

            return true;
        });
    }

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

using System.Globalization;
using System.Windows.Forms;

namespace SampleParity;

/// <summary>
/// The body of both harness executables: run the catalogue against whichever sample the harness supplies,
/// and write the trace out.
/// </summary>
/// <remarks>
/// Compiled into each harness as linked source rather than shared as an assembly, because each side binds a
/// different <c>System.Windows.Forms</c> - the reference sample links upstream's Gtk3 library, the other
/// links this repo's Gtk4 one, and the two cannot coexist in a process at all.
/// </remarks>
internal static class HarnessMain
{
    /// <summary>Runs the catalogue and writes the trace.</summary>
    /// <param name="args">Command line; <c>--out &lt;path&gt;</c> selects the trace file.</param>
    /// <param name="forms">The sample's form factory.</param>
    /// <returns>0 when every scenario ran, 1 when the trace could not be written.</returns>
    public static int Run(string[] args, ISampleForms forms)
    {
        // Pinned so that culture-dependent captions and number formats cannot differ between the two runs
        // for reasons unrelated to the port.
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

        var output = ReadOption(args, "--out") ?? "trace.tsv";
        var includePointer = args.Contains("--pointer");

        Application.Init();

        // Before any form is shown: teardown closes every form, and a sample handler that pops a modal
        // dialog on FormClosing would otherwise block the harness forever.
        forms.InstallModalGuard();

        var trace = new TraceWriter();

        foreach (var scenario in ScenarioCatalogue.All())
        {
            if (scenario.RequiresPointer && !includePointer)
            {
                continue;
            }

            RunOne(scenario, forms, trace);
        }

        trace.Save(output);
        Console.WriteLine($"{trace.Records.Count} records -> {output}");

        return 0;
    }

    private static void RunOne(Scenario scenario, ISampleForms forms, TraceWriter trace)
    {
        Form? form = null;

        try
        {
            form = forms.Create(scenario.Form);

            if (form == null)
            {
                trace.Add(scenario.Id, 0, TraceKind.Error, scenario.Form, "error", "form not provided");
                return;
            }

            form.Show();

            var context = new ScenarioContext(scenario.Id, form, trace);
            context.Pump(20);

            scenario.Body(context);
        }
        catch (Exception ex)
        {
            // Recorded rather than thrown: one broken scenario must not stop the rest of the catalogue, and
            // the parity test fails on any Error record anyway.
            trace.Add(scenario.Id, 999, TraceKind.Error, scenario.Form, "exception", ex.GetType().Name);
        }
        finally
        {
            try
            {
                form?.Close();
                form?.Dispose();
            }
            catch (Exception ex)
            {
                trace.Add(scenario.Id, 1000, TraceKind.Error, scenario.Form, "teardown", ex.GetType().Name);
            }

            for (var i = 0; i < 10; i++)
            {
                Application.DoEvents();
            }
        }
    }

    private static string? ReadOption(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);

        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
